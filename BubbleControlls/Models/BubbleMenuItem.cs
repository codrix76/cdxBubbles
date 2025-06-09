using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public bool IsEnabled { get; set; } = true;
        public bool IsHighlighted { get; set; } = false;
        public Color? CustomColor { get; set; }

        public List<BubbleMenuItem> SubItems { get; set; } = new();
        public Func<List<BubbleMenuItem>>? LoadSubItems { get; set; }

        public Action<BubbleMenuItem>? OnClick { get; set; }
        public Action<BubbleMenuItem>? OnMouseEnter { get; set; }

        public bool IsSelectable { get; set; } = true;
        public bool CloseMenuOnClick { get; set; } = true;

        public bool HasDynamicSubItems => LoadSubItems != null;
    }
}
