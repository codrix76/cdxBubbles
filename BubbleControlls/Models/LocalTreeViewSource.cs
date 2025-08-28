namespace BubbleControlls.Models;

public class LocalTreeViewSource : ITreeViewDataSource
{
    private BubbleTreeViewItem _root = new BubbleTreeViewItem(BubbleTreeViewItem.ROOTKEY, "Root");

    public LocalTreeViewSource()
    {
        _root.IsExpanded = true;
    }

    public List<BubbleTreeViewItem> GetRoot()
    {
        return _root.Children;
    }
    public void SetRoot(List<BubbleTreeViewItem> root)
    {
        _root.Children.AddRange(root);
    }
    public List<BubbleTreeViewItem> GetChildren(string key)
    {
        var item = _root.FindByID(key);
        return item?.Children ?? new List<BubbleTreeViewItem>();
    }

    public BubbleTreeViewItem? FindByID(string key)
        => _root.FindByID(key);

    /// <summary>
    /// Sucht den Index des Vorfahren, der direktes Kind der Super-Root ist.
    /// </summary>
    public int GetParentIndex(string key)
    {
        var current = _root.FindByID(key);
        if (current == null) return -1;

        while (current != null && current.Parent != null && current.Parent != _root)
            current = current.Parent;

        if (current?.Parent == _root)
            return _root.Children.IndexOf(current);

        return -1;
    }

    public string GetRootParentKey(string key)
    {
        var current = _root.FindByID(key);
        if (current == null) return "";

        while (current != null && current.Parent != null && current.Parent != _root)
            current = current.Parent;

        if (current != null && (current.Parent == null || current.Parent == _root))
            return current.Key;

        return "";
    }
}
