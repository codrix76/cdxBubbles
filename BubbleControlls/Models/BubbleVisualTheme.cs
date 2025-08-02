using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Media;

namespace BubbleControlls.Models
{
    public class BubbleVisualTheme
    {
        [Display(Name = "Hintergrund", Description = "Hintergrundfarbe der Komponenten")]
        public Brush? Background { get; set; }
        [Display(Name = "Textfarbe", Description = "Farbe des angezeigten Textes")]
        public Brush? Foreground { get; set; }
        [Display(Name = "Titelfarbe", Description = "Farbe der Titelzeile")]
        public Brush? TitleBackground { get; set; }
        [Display(Name = "Hintergund2", Description = "Farbe des äußeren Hintegrund")]
        public Brush? BackgroundBack { get; set; } = Brushes.Transparent;
        [Display(Name = "Innenrahmen", Description = "Farbe des inneren Rahmens")]
        public Brush? Border { get; set; }
        [Display(Name = "Außenrahmen", Description = "Farbe des äußeren Rahmens")]
        public Brush? OuterBorderColor { get; set; }
        // Effektfarben
        [Display(Name = "Glanzfarbe", Description = "Lichtpunktfarbe für plastische Effekte")]
        public Color EffectColor1 { get; set; }
        [Display(Name = "Hintergrund (dunkel)", Description = "Dunklere Variante des Hintergrunds für 3D-Effekt")]
        public Color EffectColor2 { get; set; }
        [Display(Name = "Titel Effektfarbe 1 (Highlight)", Description = "Lichtpunktfarbe für plastische Effekte")]
        public Color TitleEffectColor1 { get; set; }
        [Display(Name = "Titel Effektfarbe 2", Description = "Dunklere Variante des Hintergrunds für 3D-Effekt")]
        public Color TitleEffectColor2 { get; set; }
        [Display(Name = "Außenrahmen (Highlight)", Description = "Hellerer Rand für Außenrahmen")]
        public Color OuterBorderEffectColor1 { get; set; }
        [Display(Name = "Außenrahmen (dunkel)", Description = "Dunklerer Rand für Außenrahmen")]
        public Color OuterBorderEffectColor2 { get; set; }
        [Display(Name = "Innenrahmen (Highlight)", Description = "Hellerer Rand für Innenrahmen")]
        public Color InnerBorderEffectColor1 { get; set; }
        [Display(Name = "Innenrahmen (dunkel)", Description = "Dunklerer Rand für Innenrahmen")]
        public Color InnerBorderEffectColor2 { get; set; }
        // Schrift
        [Display(Name = "Schriftart", Description = "Verwendete Schriftfamilie")]
        public FontFamily? FontFamily { get; set; }
        [Display(Name = "Schriftgröße", Description = "Größe des angezeigten Textes in Punkten")]
        public double FontSize { get; set; } = 14.0;
        [Display(Name = "Schriftgewicht", Description = "Dicke der Schrift (z. B. Normal, Bold)")]
        public FontWeight FontWeight { get; set; } = FontWeights.Normal;
        [Display(Name = "Schriftstil", Description = "Schriftschnitt wie Normal oder Kursiv")]
        public FontStyle FontStyle { get; set; } = FontStyles.Normal;
        // Ränder Stärke
        [Display(Name = "Innenrahmenstärke", Description = "Dicke des inneren Rahmens")]
        public Thickness BorderThickness { get; set; }
        [Display(Name = "Außenrahmenstärke", Description = "Dicke des äußeren Rahmens")]
        public Thickness OuterBorderThickness { get; set; }

        // Sonstiges
        [Display(Name = "3D-Gradient aktiv", Description = "Verwendet plastische 3D-Darstellung per Farbverlauf")]
        public bool Use3DGradient { get; set; }
        
        // Ring-spezifisch
        [Display(Name = "BubbleRing Einstellungen", Description = "Alle Einstellungen für das BubbleRing Control")] 
        public BubbleVisualsRings RingVisuals { get; set; } = new BubbleVisualsRings();

        // BubbleinfoBox spezifisch
        private BubbleVisualInfoBox? _infoBoxVisuals;

        [Display(Name = "BubbleInfo Einstellungen", Description = "Alle Einstellungen für das BubbleInfo Control")]
        public BubbleVisualInfoBox InfoBoxVisuals
        {
            get
            {
                if (_infoBoxVisuals == null)
                {
                    _infoBoxVisuals = new BubbleVisualInfoBox
                    {
                        InfoBackground = this.Background,
                        InfoForeground = this.Foreground,
                        InfoBorder = this.Border,
                        InfoOuterBorderColor = this.OuterBorderColor,
                        InfoEffectColor1 = this.EffectColor1,
                        InfoOuterBorderEffectColor1 = this.OuterBorderEffectColor1,
                        InfoInnerBorderEffectColor1 = this.InnerBorderEffectColor1,
                        InfoBorderThickness = this.BorderThickness,
                        InfoOuterBorderThickness = this.OuterBorderThickness
                    };
                }
                return _infoBoxVisuals;
            }
            set => _infoBoxVisuals = value;
        }
    }

    public class BubbleVisualsRings
    {
        [Display(Name = "Ring-Hintergrund", Description = "Hintergrundfarbe des Bubble-Rings")]
        public Brush? RingBackground { get; set; }

        [Display(Name = "Ring-Randfarbe", Description = "Farbe des Randes im Bubble-Ring")]
        public Brush? RingBorderBrush { get; set; }

        [Display(Name = "Ring-Transparenz", Description = "Deckkraft des Ringhintergrunds (0–255)")]
        public int RingOpacity { get; set; } = 50;

        [Display(Name = "Ring-Randtransparenz", Description = "Deckkraft des Ringrandes (0–255)")]
        public int RingBorderOpacity { get; set; } = 80;

        [Display(Name = "Ring-Randstärke", Description = "Stärke des Rings in Pixeln")]
        public int RingBorderThickness { get; set; } = 2;

        [Display(Name = "Scrollpfeil-Höhe", Description = "Höhe der Pfeile für Ringscrollen")]
        public double RingScrollArrowHeight { get; set; } = 8.0;
    }

    public class BubbleVisualInfoBox
    {
        [Display(Name = "Info Hintergrund", Description = "Hintergrundfarbe der Info-Innenfläche")]
        public Brush? InfoBackground { get; set; }

        [Display(Name = "Info-Innenrahmen", Description = "Farbe des inneren Info-Rahmens")]
        public Brush? InfoBorder { get; set; }

        [Display(Name = "Info-Außenrahmen", Description = "Farbe des äußeren Info-Rahmens")]
        public Brush? InfoOuterBorderColor { get; set; }

        [Display(Name = "Info Glanzfarbe", Description = "Lichtpunktfarbe für plastische Effekte")]
        public Color InfoEffectColor1 { get; set; }

        [Display(Name = "Info Außenrahmen (Highlight)", Description = "Hellerer Rand für Außenrahmen")]
        public Color InfoOuterBorderEffectColor1 { get; set; }

        [Display(Name = "Info Innenrahmen (Highlight)", Description = "Hellerer Rand für Innenrahmen")]
        public Color InfoInnerBorderEffectColor1 { get; set; }

        [Display(Name = "Info Textfarbe", Description = "Farbe des angezeigten Textes")]
        public Brush? InfoForeground { get; set; }

        [Display(Name = "Info Innenrahmenstärke", Description = "Dicke des inneren Rahmens")]
        public Thickness InfoBorderThickness { get; set; }

        [Display(Name = "Info Außenrahmenstärke", Description = "Dicke des äußeren Rahmens")]
        public Thickness InfoOuterBorderThickness { get; set; }
    }
}
