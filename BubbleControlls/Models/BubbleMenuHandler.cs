using BubbleControlls.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BubbleControlls.Models
{
    public class BubbleMenuHandler
    {
        #region Variablen
        private BubbleMenuItem _mainMenu = new BubbleMenuItem
        {
            Text = "Main Menu"
        };
        private List<BubbleMenuItem> _menuPath = new List<BubbleMenuItem>();
        private List<BubbleMenuItem> _selectableMenus = new List<BubbleMenuItem>();
        private List<BubbleMenuItem> _guiItemsToRemove = new List<BubbleMenuItem>();
        #endregion

        #region Properties
        public BubbleMenuItem MainMenu { get => _mainMenu; set => _mainMenu = value; }
        public List<BubbleMenuItem> MenuPath { get => _menuPath; }
        public List<BubbleMenuItem> SelectableMenus { get => _selectableMenus; }
        public List<BubbleMenuItem> GuiItemsToRemove { get => _guiItemsToRemove; set => _guiItemsToRemove = value; }

        #endregion

        public void AddMenu(BubbleMenuItem menuItem, BubbleMenuItem? parentMenu = null)
        {
            if (menuItem == null)
            {
                throw new ArgumentNullException(nameof(menuItem));
            }
            if (parentMenu == null)
            {
                _mainMenu.SubItems.Add(menuItem);
                menuItem.Parent = _mainMenu;
            }
            else
            {
                parentMenu.SubItems.Add(menuItem);
                menuItem.Parent = parentMenu;
            }
        }
        public void RemoveMenu(BubbleMenuItem menuItem)
        {
            if (menuItem == null)
            {
                throw new ArgumentNullException(nameof(menuItem));
            }
            BubbleMenuItem? parent = menuItem.Parent;
            if (parent != null)
            {
                parent.SubItems.Remove(menuItem);
            }
            else
            {
                _mainMenu.SubItems.Remove(menuItem);
            }
        }
        public BubbleMenuItem? FindMenu(string name)
        {
            if (_mainMenu.Name == name)
                return _mainMenu;

            return _mainMenu.SubItems.FindDeep(name, x => x.Name, x => x.SubItems);
        }

        public int GetMaxDepth()
        {
            return GetDepthRecursive(_mainMenu);
        }

        private int GetDepthRecursive(BubbleMenuItem item)
        {
            if (item.SubItems == null || item.SubItems.Count == 0)
                return 1;

            int max = 0;
            foreach (var sub in item.SubItems)
            {
                int depth = GetDepthRecursive(sub);
                if (depth > max)
                    max = depth;
            }

            return max + 1;
        }
        public int GetMaxSubItemCount()
        {
            return GetMaxSubItemCountRecursive(_mainMenu);
        }

        private int GetMaxSubItemCountRecursive(BubbleMenuItem item)
        {
            int currentCount = item.SubItems?.Count ?? 0;
            int maxInChildren = 0;

            if (item.SubItems != null)
            {
                foreach (var child in item.SubItems)
                {
                    int childMax = GetMaxSubItemCountRecursive(child);
                    if (childMax > maxInChildren)
                        maxInChildren = childMax;
                }
            }

            return Math.Max(currentCount, maxInChildren);
        }
        public (int itemLevel, int itemCount) GetLevelWithMaxSubItemCount()
        {
            return GetLevelRecursive(_mainMenu, 1);
        }

        private (int level, int count) GetLevelRecursive(BubbleMenuItem item, int currentLevel)
        {
            int currentCount = item.SubItems?.Count ?? 0;
            int maxLevel = currentLevel;
            int maxCount = currentCount;

            if (item.SubItems != null)
            {
                foreach (var child in item.SubItems)
                {
                    var (childLevel, childCount) = GetLevelRecursive(child, currentLevel + 1);
                    if (childCount > maxCount)
                    {
                        maxCount = childCount;
                        maxLevel = childLevel;
                    }
                }
            }

            return (maxLevel, maxCount);
        }

        public void HandleClick(string name)
        {
            BubbleMenuItem? selected = FindMenu(name);
            if (selected == null)
                return;

            // Klick auf MainMenu (kein Parent)
            if (selected.Parent == null)
            {
                _guiItemsToRemove.AddRange(_menuPath);
                _guiItemsToRemove.AddRange(_selectableMenus);
                _guiItemsToRemove.ForEach(item => item.Level = BubbleMenuLevel.Deleted);
                _menuPath.Clear();
                _selectableMenus.Clear();

                if (selected.SubItems.Count > 0)
                {
                    _selectableMenus.AddRange(selected.SubItems);
                    _selectableMenus.ForEach(item => item.Level = BubbleMenuLevel.Selection);
                }
                return;
            }

            // Klick auf Menü im Pfad → Pfad abschneiden und Submenüs anzeigen
            int existingIndex = _menuPath.IndexOf(selected);
            if (existingIndex >= 0)
            {
                // Nur bis inklusive dieses Elements behalten
                _guiItemsToRemove.AddRange(_menuPath.Skip(existingIndex + 1).ToList());
                _guiItemsToRemove.AddRange(_selectableMenus);
                _guiItemsToRemove.ForEach(item => item.Level = BubbleMenuLevel.Deleted);

                _menuPath.RemoveRange(existingIndex + 1, _menuPath.Count - (existingIndex + 1));

                _selectableMenus.Clear();
                _selectableMenus.AddRange(selected.SubItems);
                _selectableMenus.ForEach(item => item.Level = BubbleMenuLevel.Selection);
                return;
            }

            // Klick auf Menü mit SubItems → navigieren
            if (selected.SubItems.Count > 0)
            {
                _guiItemsToRemove.AddRange(_selectableMenus);
                _guiItemsToRemove.ForEach(item => item.Level = BubbleMenuLevel.Deleted);

                _menuPath.Add(selected);
                _menuPath[_menuPath.Count - 1].Level = BubbleMenuLevel.Path;

                _selectableMenus.Clear();
                _selectableMenus.AddRange(selected.SubItems);
                _selectableMenus.ForEach(item => item.Level = BubbleMenuLevel.Selection);
                return;
            }

            // Klick auf Menü **ohne** SubItems → Aktion auslösen
            // → Kein Ändern des Pfads oder der Submenüs
            selected.OnClick?.Invoke(selected);
        }

        public List<BubbleMenuItem> FindAllItemsWithLevel(BubbleMenuLevel level)
        {
            var result = new List<BubbleMenuItem>();

            void Traverse(BubbleMenuItem item)
            {
                if (item.Level == level)
                    result.Add(item);

                foreach (var child in item.SubItems)
                    Traverse(child);
            }

            if (MainMenu != null)
                Traverse(MainMenu);

            return result;
        }

        public void ResetDeleted()
        {
            //var toRemoveItems = FindAllItemsWithLevel(BubbleMenuLevel.Deleted);
            //foreach (var item in toRemoveItems)
            //{
            //    item.Level = BubbleMenuLevel.Neutral;
            //}
            _guiItemsToRemove.ForEach(item => item.Level = BubbleMenuLevel.Neutral);
            _guiItemsToRemove.Clear();
        }
    }
}
