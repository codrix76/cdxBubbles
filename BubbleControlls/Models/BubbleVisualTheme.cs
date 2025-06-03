using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

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
    }
}
