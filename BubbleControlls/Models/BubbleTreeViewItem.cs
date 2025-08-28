using System.Windows.Media;


namespace BubbleControlls.Models
{
    public class BubbleTreeViewItem
    {
        public static readonly string ROOTKEY = "%_-$ROOT$-_%";

        public event Action<BubbleTreeViewItem>? ItemAdded;
        public event Action<BubbleTreeViewItem>? ItemRemoved;

        //public int ID { get; set; }
        public string Key { get; set; }
        public string Label { get; set; }
        public List<BubbleTreeViewItem> Children { get; set; } = new List<BubbleTreeViewItem>();
        public BubbleTreeViewItem? Parent { get; set; } = null;
        public bool IsExpanded { get; set; } = false;
        public bool IsExpandeable => Children.Count > 0;
        public Brush? CustomColor { get; set; } = null;

        //public BubbleTreeViewItem(int id, string label)
        public BubbleTreeViewItem(string key, string label)
        {
            Key = key;
            Label = label;
        }

        public void Add(BubbleTreeViewItem item)
        {
            item.Parent = this;
            Children.Add(item);
            ItemAdded?.Invoke(item);
        }
        public void Remove(BubbleTreeViewItem item)
        {
            Children.Remove(item);
            ItemRemoved?.Invoke(item);
        }

        public BubbleTreeViewItem? FindByID(string key)
        {
            if (this.Key == key)
                return this;

            foreach (var child in Children)
            {
                var found = child.FindByID(key);
                if (found != null)
                    return found;
            }

            return null;
        }
        public static int GetLevel(BubbleTreeViewItem node)
        {
            int level = 0;
            while (node.Parent != null)
            {
                level++;
                node = node.Parent;
            }
            return level;
        }
    }
}
