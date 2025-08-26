using BubbleControlls.Helpers;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

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
        private List<BubbleMenuItem> _contextMenus = new List<BubbleMenuItem>();
        private List<BubbleMenuItem> _guiItemsToRemove = new List<BubbleMenuItem>();
        private Dictionary<string, BubbleMenuItem> _menuMap = new Dictionary<string, BubbleMenuItem>();
        #endregion

        #region Properties
        public BubbleMenuItem MainMenu { get => _mainMenu; set { _mainMenu = value; RefreshMenuMap(); } }
        public List<BubbleMenuItem> PathMenus { get => _menuPath; }
        public List<BubbleMenuItem> SelectableMenus { get => _selectableMenus; }
        public List<BubbleMenuItem> ContextMenus { get => _contextMenus; }

        #endregion

        private void RefreshMenuMap()
        { 
            _menuMap.Clear();
            var stack = new Stack<BubbleMenuItem>();
            stack.Push(_mainMenu);

            while (stack.Count > 0)
            {
                var n = stack.Pop();
                _menuMap.Add(n.Name, n);

                // SubMenu pushen
                var subItems = n.SubItems;
                for (int i = 0; i < subItems.Count; i++)
                    stack.Push(subItems[i]);
                var contextItem = n.ContextItems;
                for (int i = 0; i < contextItem.Count; i++)
                    stack.Push(contextItem[i]);
            }
        }
        /// <summary>
        /// Adding a new Menu, with optional parent
        /// </summary>
        /// <param name="menuItem">menuItem to add</param>
        /// <param name="parentMenu">parentMenu for menuItem</param>
        /// <exception cref="ArgumentNullException"></exception>
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
            _menuMap[menuItem.Name] = menuItem;
        }
        /// <summary>
        /// removing given BubbleMenuItem menuItem
        /// </summary>
        /// <param name="menuItem"></param>
        /// <exception cref="ArgumentNullException"></exception>
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
            _menuMap.Remove(menuItem.Name);
        }
        /// <summary>
        /// find Menu by Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>BubbleMenuItem</returns>
        public BubbleMenuItem? FindMenu(string name)
        {
            if (_mainMenu.Name == name)
                return _mainMenu;

            _menuMap.TryGetValue(name, out var menuItem);
            if (menuItem != null)
                return menuItem;

            return null;
        }

        /// <summary>
        /// Liefert die MenüTiefe
        /// </summary>
        /// <returns>int count</returns>
        public int GetMaxDepth()
        {
            return GetDepthRecursive(_mainMenu);
        }

        private int GetDepthRecursive(BubbleMenuItem item)
        {
            if (item.SubItems.Count == 0)
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
        /// <summary>
        ///  liefert Anzahl aller SubItems 
        /// </summary>
        /// <returns>count</returns>
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
        /// <summary>
        /// liefert ein Tupel mit level und max Anzahl Subitems
        /// </summary>
        /// <returns>(int itemLevel, int itemCount)</returns>
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

        /// <summary>
        /// Handles Menu submenu building
        /// </summary>
        /// <param name="name">menuName</param>
        public void HandleClick(string name, bool isContext)
        {
            BubbleMenuItem? selected = FindMenu(name);
            if (selected == null)
                return;

            if (isContext)
            {
                _contextMenus.Clear();
                _contextMenus.AddRange(selected.ContextItems);
                return;
            }
            _contextMenus.Clear();
            // Klick auf MainMenu (kein Parent)
            if (selected.Parent == null)
            {
                _guiItemsToRemove.AddRange(_menuPath);
                _guiItemsToRemove.AddRange(_selectableMenus);
                _guiItemsToRemove.AddRange(_contextMenus);
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

            if (MainMenu.SubItems.Count == 0)
                Traverse(MainMenu);

            return result;
        }

        public void ResetDeleted()
        {
            _guiItemsToRemove.ForEach(item => item.Level = BubbleMenuLevel.Neutral);
            _guiItemsToRemove.Clear();
        }
    }
}
