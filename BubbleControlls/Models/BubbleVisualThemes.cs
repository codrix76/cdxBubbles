using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace BubbleControlls.Models
{
    public static class BubbleVisualThemes
    {
        public static BubbleVisualTheme HudBlue()
        {
            return new BubbleVisualTheme
            {
                // Farben
                Background = new SolidColorBrush(Color.FromRgb(30, 60, 100)),               // dunkles Blau
                Border = new SolidColorBrush(Color.FromRgb(70, 170, 255)),                  // kräftiges HUD-Blau
                OuterBorderColor = new SolidColorBrush(Color.FromRgb(20, 110, 190)),

                HighlightColor = Color.FromRgb(180, 220, 255),                              // hellblauer Glanzpunkt
                BackgroundDarkColor = Color.FromRgb(10, 25, 50),
                OuterBorderHighlightColor = Color.FromRgb(100, 200, 255),
                OuterBorderDarkColor = Color.FromRgb(10, 40, 80),
                InnerBorderHighlightColor = Color.FromRgb(130, 200, 255),
                InnerBorderDarkColor = Color.FromRgb(20, 40, 80),

                // Font
                Foreground = new SolidColorBrush(Colors.WhiteSmoke),
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                FontStyle = FontStyles.Normal,

                // Ränder
                BorderThickness = new Thickness(1.5),
                OuterBorderThickness = new Thickness(2.5),

                // Layout
                Use3DGradient = true
            };

        }

        public static BubbleVisualTheme Dark()
        {
            return new BubbleVisualTheme
            {
                Background = new SolidColorBrush(Color.FromRgb(45, 45, 48)),               // VS-Dunkelgrau
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
            };
        }

        public static BubbleVisualTheme EclipseCore()
        {
            return new BubbleVisualTheme
            {
                Background = new SolidColorBrush(Color.FromRgb(10, 10, 14)),                // fast schwarzes Blau
                Border = new SolidColorBrush(Color.FromRgb(40, 80, 140)),
                OuterBorderColor = new SolidColorBrush(Color.FromRgb(20, 40, 60)),

                HighlightColor = Color.FromRgb(100, 180, 255),                              // kühler Glanz
                BackgroundDarkColor = Color.FromRgb(4, 4, 6),
                OuterBorderHighlightColor = Color.FromRgb(80, 130, 200),
                OuterBorderDarkColor = Color.FromRgb(10, 20, 30),
                InnerBorderHighlightColor = Color.FromRgb(60, 120, 200),
                InnerBorderDarkColor = Color.FromRgb(15, 30, 50),

                Foreground = new SolidColorBrush(Color.FromRgb(200, 220, 255)),             // eisiges Grau-Blau

                FontFamily = new FontFamily("Consolas"),
                FontSize = 13.5,
                FontWeight = FontWeights.Medium,
                FontStyle = FontStyles.Normal,

                BorderThickness = new Thickness(1.3),
                OuterBorderThickness = new Thickness(2.0),

                Use3DGradient = true
            };
        }
        public static BubbleVisualTheme NeonEdge()
        {
            return new BubbleVisualTheme
            {
                // Hintergrund: dunkles Violett-Schwarz
                Background = new SolidColorBrush(Color.FromRgb(15, 5, 25)),
                Border = new SolidColorBrush(Color.FromRgb(255, 20, 147)), // Neon-Pink
                OuterBorderColor = new SolidColorBrush(Color.FromRgb(0, 255, 255)), // Cyan-Glow

                // Licht- und Schatten
                HighlightColor = Color.FromRgb(255, 105, 180), // HotPink
                BackgroundDarkColor = Color.FromRgb(5, 0, 10),
                OuterBorderHighlightColor = Color.FromRgb(0, 255, 255),
                OuterBorderDarkColor = Color.FromRgb(0, 50, 80),
                InnerBorderHighlightColor = Color.FromRgb(255, 255, 0), // Gelber Stich
                InnerBorderDarkColor = Color.FromRgb(30, 0, 50),

                // Schrift: grell, techno
                Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 180)), // Mintgrün

                FontFamily = new FontFamily("Orbitron"), // Wenn du installiert hast – sonst z. B. "Consolas"
                FontSize = 14.5,
                FontWeight = FontWeights.ExtraBold,
                FontStyle = FontStyles.Normal,

                BorderThickness = new Thickness(1.8),
                OuterBorderThickness = new Thickness(2.2),

                Use3DGradient = true
            };
        }
        public static BubbleVisualTheme Standard()
        {
            return new BubbleVisualTheme
            {
                // Grundfarben basierend auf typischen Windows 10/11 UI-Tönen
                Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),       // klassisches Control-Hellgrau
                Border = new SolidColorBrush(Color.FromRgb(173, 173, 173)),           // SystemBorderBrush-ähnlich
                OuterBorderColor = new SolidColorBrush(Color.FromRgb(200, 200, 200)),

                HighlightColor = Color.FromRgb(255, 255, 255),                        // heller Lichtpunkt
                BackgroundDarkColor = Color.FromRgb(200, 200, 200),
                OuterBorderHighlightColor = Color.FromRgb(255, 255, 255),
                OuterBorderDarkColor = Color.FromRgb(160, 160, 160),
                InnerBorderHighlightColor = Color.FromRgb(255, 255, 255),
                InnerBorderDarkColor = Color.FromRgb(180, 180, 180),

                Foreground = new SolidColorBrush(Color.FromRgb(30, 30, 30)),         // SystemTextBrush-ähnlich

                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 14,
                FontWeight = FontWeights.Normal,
                FontStyle = FontStyles.Normal,

                BorderThickness = new Thickness(1),
                OuterBorderThickness = new Thickness(1.5),

                Use3DGradient = false
            };
        }

    }
}
