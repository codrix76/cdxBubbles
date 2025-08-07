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
                ThemeName = "HudBlue",
                Background = new SolidColorBrush(Color.FromRgb(15, 30, 60)),
                Foreground = new SolidColorBrush(Colors.WhiteSmoke),
                TitleBackground = new SolidColorBrush(Color.FromRgb(40, 80, 130)),
                TitleEffectColor1 = Color.FromRgb(180, 220, 255),
                TitleEffectColor2 = Color.FromRgb(10, 25, 50),
            
                Border = new SolidColorBrush(Color.FromRgb(70, 170, 255)),
                OuterBorderColor = new SolidColorBrush(Color.FromRgb(20, 110, 190)),

                EffectColor1 = Color.FromRgb(180, 220, 255),
                EffectColor2 = Color.FromRgb(10, 25, 50),
                OuterBorderEffectColor1 = Color.FromRgb(100, 200, 255),
                OuterBorderEffectColor2 = Color.FromRgb(10, 40, 80),
                InnerBorderEffectColor1 = Color.FromRgb(130, 200, 255),
                InnerBorderEffectColor2 = Color.FromRgb(20, 40, 80),

                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                FontStyle = FontStyles.Normal,

                BorderThickness = new Thickness(1.5),
                OuterBorderThickness = new Thickness(2.5),

                Use3DGradient = true,

                RingVisuals = new BubbleVisualsRings
                {
                    RingBackground = new SolidColorBrush(Color.FromArgb(50, 100, 149, 237)),
                    RingBorderBrush = new SolidColorBrush(Color.FromArgb(80, 100, 149, 237)),
                    RingOpacity = 50,
                    RingBorderOpacity = 80,
                    RingBorderThickness = 1,
                    RingScrollArrowHeight = 12.0
                },

                // InfoBoxVisuals = new BubbleVisualInfoBox
                // {
                //     InfoBackground = new SolidColorBrush(Color.FromRgb(30, 60, 100)),
                //     InfoBorder = new SolidColorBrush(Color.FromRgb(70, 170, 255)),
                //     InfoOuterBorderColor = new SolidColorBrush(Color.FromRgb(20, 110, 190)),
                //
                //     InfoEffectColor1 = Color.FromRgb(180, 220, 255),
                //     InfoOuterBorderEffectColor1 = Color.FromRgb(100, 200, 255),
                //     InfoInnerBorderEffectColor1 = Color.FromRgb(130, 200, 255),
                //
                //     InfoForeground = new SolidColorBrush(Colors.WhiteSmoke),
                //     InfoBorderThickness = new Thickness(1.5),
                //     InfoOuterBorderThickness = new Thickness(2.5)
                // }
            };


        }

        public static BubbleVisualTheme Dark()
        {
            return new BubbleVisualTheme
            {
                ThemeName = "Dark",
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                Foreground = new SolidColorBrush(Colors.Gainsboro),
                TitleBackground = new SolidColorBrush(Color.FromRgb(50, 50, 50)),
                TitleEffectColor1 = Color.FromRgb(110, 110, 120),
                TitleEffectColor2 = Color.FromRgb(25, 25, 25),
                
                Border = new SolidColorBrush(Color.FromRgb(90, 90, 90)),
                OuterBorderColor = new SolidColorBrush(Color.FromRgb(60, 60, 60)),

                EffectColor1 = Color.FromRgb(110, 110, 120),
                EffectColor2 = Color.FromRgb(25, 25, 25),
                OuterBorderEffectColor1 = Color.FromRgb(100, 100, 100),
                OuterBorderEffectColor2 = Color.FromRgb(20, 20, 20),
                InnerBorderEffectColor1 = Color.FromRgb(120, 120, 130),
                InnerBorderEffectColor2 = Color.FromRgb(40, 40, 40),
                
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 13,
                FontWeight = FontWeights.Normal,
                FontStyle = FontStyles.Normal,

                BorderThickness = new Thickness(1),
                OuterBorderThickness = new Thickness(1.5),

                Use3DGradient = true,
                

                RingVisuals = new BubbleVisualsRings
                {
                    RingBackground = new SolidColorBrush(Color.FromArgb(50, 60, 60, 60)),
                    RingBorderBrush = new SolidColorBrush(Color.FromArgb(80, 90, 90, 90)),
                    RingOpacity = 50,
                    RingBorderOpacity = 80,
                    RingBorderThickness = 1,
                    RingScrollArrowHeight = 12.0
                },
                //
                // InfoBoxVisuals = new BubbleVisualInfoBox
                // {
                //     InfoBackground = new SolidColorBrush(Color.FromRgb(45, 45, 48)),
                //     InfoBorder = new SolidColorBrush(Color.FromRgb(90, 90, 90)),
                //     InfoOuterBorderColor = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
                //
                //     InfoEffectColor1 = Color.FromRgb(110, 110, 120),
                //     InfoOuterBorderEffectColor1 = Color.FromRgb(100, 100, 100),
                //     InfoInnerBorderEffectColor1 = Color.FromRgb(120, 120, 130),
                //
                //     InfoForeground = new SolidColorBrush(Colors.Gainsboro),
                //     InfoBorderThickness = new Thickness(1),
                //     InfoOuterBorderThickness = new Thickness(1.5)
                // }
            };
        }

        public static BubbleVisualTheme Standard()
        {
            return new BubbleVisualTheme
            {
                ThemeName = "Standard",
                Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                BackgroundBack = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
                Foreground = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                TitleBackground = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
                TitleEffectColor1 = Color.FromRgb(255, 255, 255),
                TitleEffectColor2 = Color.FromRgb(200, 200, 200),
                
                Border = new SolidColorBrush(Color.FromRgb(173, 173, 173)),
                OuterBorderColor = new SolidColorBrush(Color.FromRgb(200, 200, 200)),

                EffectColor1 = Color.FromRgb(255, 255, 255),
                EffectColor2 = Color.FromRgb(200, 200, 200),
                OuterBorderEffectColor1 = Color.FromRgb(255, 255, 255),
                OuterBorderEffectColor2 = Color.FromRgb(160, 160, 160),
                InnerBorderEffectColor1 = Color.FromRgb(255, 255, 255),
                InnerBorderEffectColor2 = Color.FromRgb(180, 180, 180),

                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 14,
                FontWeight = FontWeights.Normal,
                FontStyle = FontStyles.Normal,

                BorderThickness = new Thickness(1),
                OuterBorderThickness = new Thickness(1.5),

                Use3DGradient = false,
                

                RingVisuals = new BubbleVisualsRings
                {
                    RingBackground = new SolidColorBrush(Color.FromArgb(50, 200, 200, 200)),
                    RingBorderBrush = new SolidColorBrush(Color.FromArgb(80, 160, 160, 160)),
                    RingOpacity = 50,
                    RingBorderOpacity = 80,
                    RingBorderThickness = 1,
                    RingScrollArrowHeight = 12.0
                },

                // InfoBoxVisuals = new BubbleVisualInfoBox
                // {
                //     InfoBackground = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                //     InfoBorder = new SolidColorBrush(Color.FromRgb(173, 173, 173)),
                //     InfoOuterBorderColor = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
                //
                //     InfoEffectColor1 = Color.FromRgb(255, 255, 255),
                //     InfoOuterBorderEffectColor1 = Color.FromRgb(255, 255, 255),
                //     InfoInnerBorderEffectColor1 = Color.FromRgb(255, 255, 255),
                //
                //     InfoForeground = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                //     InfoBorderThickness = new Thickness(1),
                //     InfoOuterBorderThickness = new Thickness(1.5)
                // }
            };
        }


    }
}
