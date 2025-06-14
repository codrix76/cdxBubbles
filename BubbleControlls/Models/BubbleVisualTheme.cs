using System.Windows;
using System.Windows.Media;

namespace BubbleControlls.Models
{
    public class BubbleVisualTheme
    {
        public Brush Background { get; set; }
        public Brush Border { get; set; }
        public Brush OuterBorderColor { get; set; }

        public Color HighlightColor { get; set; }
        public Color BackgroundDarkColor { get; set; }
        public Color OuterBorderHighlightColor { get; set; }
        public Color OuterBorderDarkColor { get; set; }
        public Color InnerBorderHighlightColor { get; set; }
        public Color InnerBorderDarkColor { get; set; }

        public Brush Foreground { get; set; }
        public FontFamily FontFamily { get; set; }
        public double FontSize { get; set; } = 14.0;
        public FontWeight FontWeight { get; set; } = FontWeights.Normal;
        public FontStyle FontStyle { get; set; } = FontStyles.Normal;

        public Thickness BorderThickness { get; set; }
        public Thickness OuterBorderThickness { get; set; }

        public bool Use3DGradient { get; set; }
        
        // Ergänzung für BubbleRingControl:

        // Farben
        public Brush RingBackground { get; set; }
        public Brush RingBorderBrush { get; set; }

        // Transparenzen (0–255)
        public int RingOpacity { get; set; } = 50;
        public int RingBorderOpacity { get; set; } = 80;

        // Stärke
        public int RingBorderThickness { get; set; } = 2;
        // Dimensionen
        public double ScrollArrowHeight { get; set; } = 8.0;
    }
}
