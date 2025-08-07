
namespace BubbleControlls.Models;

public interface ITreeViewDataSource
{
    public BubbleTreeViewItem GetRoot();
    public List<BubbleTreeViewItem> GetChildren(int nodeId);
    public int GetLevel(int itemID);
    public BubbleTreeViewItem? FindByID(int itemID);
    public int GetParentIndex(int itemID);
    public void SetExpanded(int itemID, bool expanded);
}
