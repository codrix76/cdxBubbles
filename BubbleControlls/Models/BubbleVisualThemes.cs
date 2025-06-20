using System.Windows;
using System.Windows.Media;
using System.IO;
using Newtonsoft.Json;

namespace BubbleControlls.Models
{
    public static class BubbleVisualThemes
    {
        public static BubbleVisualTheme LoadThemeFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Theme-Datei nicht gefunden.", filePath);

            var json = File.ReadAllText(filePath);
            var theme = JsonConvert.DeserializeObject<BubbleVisualTheme>(json);

            if (theme == null)
                throw new InvalidDataException("Die Theme-Datei konnte nicht korrekt gelesen werden.");

            return theme;
        }

        public static BubbleVisualTheme HudBlue()
        {
            return new BubbleVisualTheme
            {
                BubbleVisuals = new BubbleVisualsBubble
                {
                    Background = new SolidColorBrush(Color.FromRgb(30, 60, 100)),
                    Border = new SolidColorBrush(Color.FromRgb(70, 170, 255)),
                    OuterBorderColor = new SolidColorBrush(Color.FromRgb(20, 110, 190)),

                    HighlightColor = Color.FromRgb(180, 220, 255),
                    BackgroundDarkColor = Color.FromRgb(10, 25, 50),
                    OuterBorderHighlightColor = Color.FromRgb(100, 200, 255),
                    OuterBorderDarkColor = Color.FromRgb(10, 40, 80),
                    InnerBorderHighlightColor = Color.FromRgb(130, 200, 255),
                    InnerBorderDarkColor = Color.FromRgb(20, 40, 80),

                    Foreground = new SolidColorBrush(Colors.WhiteSmoke),
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 14,
                    FontWeight = FontWeights.SemiBold,
                    FontStyle = FontStyles.Normal,

                    BorderThickness = new Thickness(1.5),
                    OuterBorderThickness = new Thickness(2.5),

                    Use3DGradient = true
                },

                BubbleRingVisuals = new BubbleVisualsRings
                {
                    RingBackground = new SolidColorBrush(Color.FromArgb(50, 100, 149, 237)),
                    RingBorderBrush = new SolidColorBrush(Color.FromArgb(80, 100, 149, 237)),
                    RingOpacity = 50,
                    RingBorderOpacity = 80,
                    RingBorderThickness = 1,
                    ScrollArrowHeight = 12.0
                },

                BubbleInfoBoxVisuals = new BubbleVisualInfoBox
                {
                    BubbleInfoBackground = new SolidColorBrush(Color.FromRgb(30, 60, 100)),
                    BubbleInfoBorder = new SolidColorBrush(Color.FromRgb(70, 170, 255)),
                    BubbleInfoOuterBorderColor = new SolidColorBrush(Color.FromRgb(20, 110, 190)),

                    BubbleInfoHighlightColor = Color.FromRgb(180, 220, 255),
                    BubbleInfoOuterBorderHighlightColor = Color.FromRgb(100, 200, 255),
                    BubbleInfoInnerBorderHighlightColor = Color.FromRgb(130, 200, 255),

                    BubbleInfoForeground = new SolidColorBrush(Colors.WhiteSmoke),
                    BubbleInfoBorderThickness = new Thickness(1.5),
                    BubbleInfoOuterBorderThickness = new Thickness(2.5)
                }
            };

        }

        public static BubbleVisualTheme Dark()
        {
            return new BubbleVisualTheme
            {
                BubbleVisuals = new BubbleVisualsBubble
                {
                    Background = new SolidColorBrush(Color.FromRgb(45, 45, 48)),
                    Border = new SolidColorBrush(Color.FromRgb(90, 90, 90)),
                    OuterBorderColor = new SolidColorBrush(Color.FromRgb(60, 60, 60)),

                    HighlightColor = Color.FromRgb(110, 110, 120),
                    BackgroundDarkColor = Color.FromRgb(25, 25, 25),
                    OuterBorderHighlightColor = Color.FromRgb(100, 100, 100),
                    OuterBorderDarkColor = Color.FromRgb(20, 20, 20),
                    InnerBorderHighlightColor = Color.FromRgb(120, 120, 130),
                    InnerBorderDarkColor = Color.FromRgb(40, 40, 40),

                    Foreground = new SolidColorBrush(Colors.Gainsboro),
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 13,
                    FontWeight = FontWeights.Normal,
                    FontStyle = FontStyles.Normal,

                    BorderThickness = new Thickness(1),
                    OuterBorderThickness = new Thickness(1.5),

                    Use3DGradient = true
                },

                BubbleRingVisuals = new BubbleVisualsRings
                {
                    RingBackground = new SolidColorBrush(Color.FromArgb(50, 60, 60, 60)),
                    RingBorderBrush = new SolidColorBrush(Color.FromArgb(80, 90, 90, 90)),
                    RingOpacity = 50,
                    RingBorderOpacity = 80,
                    RingBorderThickness = 1,
                    ScrollArrowHeight = 12.0
                },

                BubbleInfoBoxVisuals = new BubbleVisualInfoBox
                {
                    BubbleInfoBackground = new SolidColorBrush(Color.FromRgb(45, 45, 48)),
                    BubbleInfoBorder = new SolidColorBrush(Color.FromRgb(90, 90, 90)),
                    BubbleInfoOuterBorderColor = new SolidColorBrush(Color.FromRgb(60, 60, 60)),

                    BubbleInfoHighlightColor = Color.FromRgb(110, 110, 120),
                    BubbleInfoOuterBorderHighlightColor = Color.FromRgb(100, 100, 100),
                    BubbleInfoInnerBorderHighlightColor = Color.FromRgb(120, 120, 130),

                    BubbleInfoForeground = new SolidColorBrush(Colors.Gainsboro),
                    BubbleInfoBorderThickness = new Thickness(1),
                    BubbleInfoOuterBorderThickness = new Thickness(1.5)
                }
            };
        }

        public static BubbleVisualTheme Standard()
        {
            return new BubbleVisualTheme
            {
                BubbleVisuals = new BubbleVisualsBubble
                {
                    Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                    Border = new SolidColorBrush(Color.FromRgb(173, 173, 173)),
                    OuterBorderColor = new SolidColorBrush(Color.FromRgb(200, 200, 200)),

                    HighlightColor = Color.FromRgb(255, 255, 255),
                    BackgroundDarkColor = Color.FromRgb(200, 200, 200),
                    OuterBorderHighlightColor = Color.FromRgb(255, 255, 255),
                    OuterBorderDarkColor = Color.FromRgb(160, 160, 160),
                    InnerBorderHighlightColor = Color.FromRgb(255, 255, 255),
                    InnerBorderDarkColor = Color.FromRgb(180, 180, 180),

                    Foreground = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 14,
                    FontWeight = FontWeights.Normal,
                    FontStyle = FontStyles.Normal,

                    BorderThickness = new Thickness(1),
                    OuterBorderThickness = new Thickness(1.5),

                    Use3DGradient = false
                },

                BubbleRingVisuals = new BubbleVisualsRings
                {
                    RingBackground = new SolidColorBrush(Color.FromArgb(50, 200, 200, 200)),
                    RingBorderBrush = new SolidColorBrush(Color.FromArgb(80, 160, 160, 160)),
                    RingOpacity = 50,
                    RingBorderOpacity = 80,
                    RingBorderThickness = 1,
                    ScrollArrowHeight = 12.0
                },

                BubbleInfoBoxVisuals = new BubbleVisualInfoBox
                {
                    BubbleInfoBackground = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                    BubbleInfoBorder = new SolidColorBrush(Color.FromRgb(173, 173, 173)),
                    BubbleInfoOuterBorderColor = new SolidColorBrush(Color.FromRgb(200, 200, 200)),

                    BubbleInfoHighlightColor = Color.FromRgb(255, 255, 255),
                    BubbleInfoOuterBorderHighlightColor = Color.FromRgb(255, 255, 255),
                    BubbleInfoInnerBorderHighlightColor = Color.FromRgb(255, 255, 255),

                    BubbleInfoForeground = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                    BubbleInfoBorderThickness = new Thickness(1),
                    BubbleInfoOuterBorderThickness = new Thickness(1.5)
                }
            };
        }


    }
}
