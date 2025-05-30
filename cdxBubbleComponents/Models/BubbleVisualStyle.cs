using System.Windows;
using System.Windows.Media;

namespace cdxBubbleComponents.Models;

public class BubbleVisualStyle
{
    public string FontFamily { get; set; } = "Segoe UI";
    public double FontSize { get; set; } = 14;
    public FontWeight FontWeight { get; set; } = FontWeights.Bold;
    public FontStyle FontStyle { get; set; } = FontStyles.Normal;
    public double SizeFactor { get; set; } = 1.8; // guter Startwert für Textbubbles
    
    public Brush Background { get; set; } = new SolidColorBrush(Color.FromRgb(70, 130, 180));
    public Brush Foreground { get; set; } = Brushes.White;
    public Brush Border { get; set; } = Brushes.DarkSlateGray;
    public Brush? OuterBorderColor { get; set; } = null;
    public Thickness OuterBorderThickness { get; set; } = new Thickness(0);

    public Brush ConnectionLine { get; set; } = Brushes.Gray;
    public BubbleContentLayout ContentLayout { get; set; } = BubbleContentLayout.Auto;
    public Brush HighlightColor { get; set; } = Brushes.White;
    public Brush? HoverHighlightColor { get; set; } = null;
    public double HoverEffectStrength { get; set; } = 1.5;
    public Brush? ActiveColor { get; set; } = null;


    public Thickness BorderThickness { get; set; } = new Thickness(0.5);
    public double Transparency { get; set; } = 0.0;

    public bool Use3DGradient { get; set; } = true;
    public string StyleId { get; set; } = "default";
    
    public double IconScaleFactor { get; set; } = 0.8;
    public CornerRadius CornerRadiusTextual { get; set; } = new CornerRadius(20);
    public CornerRadius CornerRadiusCircular { get; set; } = new CornerRadius(100);

    public double BrightnessHoverFactor { get; set; } = 1.15;
    public double BrightnessPressedTop { get; set; } = 0.9;
    public double BrightnessPressedBottom { get; set; } = 2.5;
    public double BrightnessFallback { get; set; } = 1.3;

    public double GradientLinearEnd { get; set; } = 0.6;
    public double GradientRadialRadiusX { get; set; } = 0.6;
    public double GradientRadialRadiusY { get; set; } = 0.6;
    public Point GradientRadialOrigin { get; set; } = new(0.3, 0.3);
}