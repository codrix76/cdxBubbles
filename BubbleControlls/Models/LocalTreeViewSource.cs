using System.Data.Common;

namespace BubbleControlls.Models;

public class LocalTreeViewSource : ITreeViewDataSource
{
    private BubbleTreeViewItem _root;

    public LocalTreeViewSource()
    {
        _root = new BubbleTreeViewItem(0, "Root");
    }
    public LocalTreeViewSource(BubbleTreeViewItem root)
    {
        _root = root;
    }

    public BubbleTreeViewItem GetRoot()
    { return _root; }

    public List<BubbleTreeViewItem> GetChildren(int itemID)
    {
        if (itemID == 0)
            return _root.Children;

        BubbleTreeViewItem? item = _root.FindByID(itemID);
        if (item == null) return new();
        return item.Children;
    }

    public int GetLevel(int itemID)
    {
        BubbleTreeViewItem? item=  _root.FindByID(itemID);
        if (item == null) return 0;
        return BubbleTreeViewItem.GetLevel(item);
    }

    public BubbleTreeViewItem? FindByID(int itemID)
    {
        return _root.FindByID(itemID);
    }

    public int GetParentIndex(int itemID)
    {
        BubbleTreeViewItem? item = _root.FindByID(itemID);
        if (_root == null || item == null)
            return -1;

        var current = item;

        // Hochlaufen bis direkt unter der Root
        while (current != null && current.Parent != _root)
        {
            current = current.Parent;
        }

        if (current?.Parent == _root)
        {
            return _root.Children.IndexOf(current);
        }

        return -1; // kein gültiger Root-Child-Vorfahre
    }
    public void SetExpanded(int itemID, bool expanded)
    {
        BubbleTreeViewItem? item = _root.FindByID(itemID);
        if (item != null) item.IsExpanded = expanded;
    }
}
