
using cdxBubbleComponents.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace cdxBubbleComponents.Views
{

    public partial class BubbleElement : UserControl
    {
        public BubbleElement()
        {
            InitializeComponent();
#if DEBUG
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                // Design-Daten für Vorschau
                this.DataContext = new BubbleElementModel
                {
                    Label = "Demo",
                    IsActive = true,
                    IsHighlighted = false,
                    Style = new BubbleVisualStyle
                    {
                        Background = Brushes.DarkSlateBlue,   // ✅ Brush statt Color
                        Foreground = Brushes.White,
                        BorderThickness = new Thickness(1),
                        CornerRadiusCircular = new CornerRadius(20),
                        CornerRadiusTextual = new CornerRadius(12),
                        SizeFactor = 1.0,
                        FontSize = 16,
                        Transparency = 1.0
                    }
                };

            }
#endif
        }
    }
}
