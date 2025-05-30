using cdxBubbleComponents.Views;
using cdxBubbleComponents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace cdxBubbleComponents.Models
{
    class BubbleElementModel
    {
        // --- Identität ---
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Label { get; set; }
        public string? Icon { get; set; }
        public string? Tooltip { get; set; }
        public string? GroupId { get; set; }
        public BubbleElementType ElementType { get; set; } = BubbleElementType.Default;

        // --- Darstellung ---
        public bool HasIcon => Icon != null;
        public bool HasText => !string.IsNullOrWhiteSpace(Label);

        public bool IsIconOnly => HasIcon && !HasText;
        public bool IsTextOnly => !HasIcon && HasText;
        public bool IsIconAndText => HasIcon && HasText;
        public double Size { get; set; } = 40;                  // Radius oder Breite
        public Point PolarOffset { get; set; } = new(0, 0);     // Position relativ zum Anker (für Sektor-Layout)
        public Point AbsolutePosition { get; set; } = new();    // Wird vom Renderer gesetzt

        public BubbleVisualStyle Style { get; set; } = new();   // Style-Daten
        public BubbleRenderMode RenderMode { get; set; } = BubbleRenderMode.Standard;

        public bool IsTextual => string.IsNullOrWhiteSpace(Icon) && !string.IsNullOrWhiteSpace(Label);

        // --- Zustand ---
        public bool IsEnabled { get; set; } = true;
        public bool IsVisible { get; set; } = true;
        public bool IsActive { get; set; } = false;
        public bool IsHighlighted { get; set; } = false;
        public bool IsSelectable { get; set; } = true;

        public int UsageScore { get; set; } = 0;                // Für adaptive Reaktion
        public bool AnimateOnOpen { get; set; } = true;
        public bool AnimateConnections { get; set; } = true;

        // --- Interaktion ---
        public Action<BubbleElement>? OnClick { get; set; }
        public Action<BubbleElement>? OnHoverEnter { get; set; }
        public Action<BubbleElement>? OnHoverLeave { get; set; }
        public Action<BubbleElement>? OnMouseDown { get; set; }
        public Action<BubbleElement>? OnMouseUp { get; set; }
        public Action<BubbleElement>? OnDoubleClick { get; set; }

        // --- Erweiterung / Meta ---
        public Dictionary<string, object> Metadata { get; set; } = new();

        public override string ToString() => Label ?? Id;

        public BubbleElementModel()
        {
            Style = new BubbleVisualStyle(); // wird automatisch gesetzt
            Size = 50;
            IsEnabled = true;
        }
    }
}
