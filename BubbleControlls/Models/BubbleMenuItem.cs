using System.Windows.Media;

namespace BubbleControlls.Models
{
    public class BubbleMenuItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public BubbleMenuItem? Parent { get; set; } = null;
        public string Name { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Tooltip { get; set; } = string.Empty;
        public string? IconPath { get; set; }
        public BubbleMenuLevel Level { get; set; } = BubbleMenuLevel.Neutral;

        public bool IsEnabled { get; set; } = true;
        public bool IsHighlighted { get; set; } = false;
        public Color? CustomColor { get; set; }

        public List<BubbleMenuItem> SubItems { get; private set; } = new();
        public List<BubbleMenuItem> ContextItems { get; private set; } = new();
        public Func<List<BubbleMenuItem>>? LoadSubItems { get; set; }
        public Func<List<BubbleMenuItem>>? LoadContextItems { get; set; }

        public Action<BubbleMenuItem>? OnClick { get; set; }
        public Action<BubbleMenuItem>? OnMouseEnter { get; set; }

        public bool IsSelectable { get; set; } = true;
        public bool CloseMenuOnClick { get; set; } = true;

        public bool HasDynamicSubItems => LoadSubItems != null;
        public bool HasDynamicContextItems => LoadContextItems != null;

        public void AddSubItem(BubbleMenuItem item)
        {
            item.Parent = this;
            SubItems.Add(item);
        }
        public void AddContextItem(BubbleMenuItem item)
        {
            item.Parent = this;
            ContextItems.Add(item);
        }
    }
}
