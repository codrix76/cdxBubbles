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
            Text = "Main Menu",
            SubItems = new List<BubbleMenuItem>()
        };
        #endregion

        #region Properties
        public BubbleMenuItem MainMenu { get => _mainMenu; set => _mainMenu = value; }

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
    }
}
