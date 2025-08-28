namespace BubbleControlls.Models;

public interface ITreeViewDataSource
{
    List<BubbleTreeViewItem> GetRoot();
    void SetRoot(List<BubbleTreeViewItem> root);
    List<BubbleTreeViewItem> GetChildren(string key);
    BubbleTreeViewItem? FindByID(string key);
    /// <summary>
    /// Index des nächstliegenden Vorfahren unterhalb der (virtuellen) Super-Root.
    /// -1 wenn es keinen solchen Vorfahren gibt (z. B. bei isolierten Knoten).
    /// </summary>
    int GetParentIndex(string key);
    string GetRootParentKey(string key);
}