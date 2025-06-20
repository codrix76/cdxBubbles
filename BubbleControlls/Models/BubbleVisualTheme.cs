using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Media;

namespace BubbleControlls.Models
{
    public class BubbleVisualTheme
    {
        [Display(Name = "Bubble Einstellungen", Description = "Alle Einstellungen für das Bubble Control")]
        public BubbleVisualsBubble BubbleVisuals { get; set; } = new BubbleVisualsBubble();
        // Ring-spezifisch

        [Display(Name = "BubbleRing Einstellungen", Description = "Alle Einstellungen für das BubbleRing Control")] 
        public BubbleVisualsRings BubbleRingVisuals { get; set; } = new BubbleVisualsRings();

        // BubbleinfoBox spezifisch
        [Display(Name = "BubbleInfo Einstellungen", Description = "Alle Einstellungen für das BubbleInfo Control")]
        public BubbleVisualInfoBox BubbleInfoBoxVisuals { get; set; } = new BubbleVisualInfoBox();


    }
    public class BubbleVisualsBubble
    {
        [Display(Name = "Hintergrund", Description = "Hintergrundfarbe der Bubble-Innenfläche")]
        public Brush? Background { get; set; }

        [Display(Name = "Innenrahmen", Description = "Farbe des inneren Bubble-Rahmens")]
        public Brush? Border { get; set; }

        [Display(Name = "Außenrahmen", Description = "Farbe des äußeren Bubble-Rahmens")]
        public Brush? OuterBorderColor { get; set; }

        [Display(Name = "Glanzfarbe", Description = "Lichtpunktfarbe für plastische Effekte")]
        public Color HighlightColor { get; set; }

        [Display(Name = "Hintergrund (dunkel)", Description = "Dunklere Variante des Hintergrunds für 3D-Effekt")]
        public Color BackgroundDarkColor { get; set; }

        [Display(Name = "Außenrahmen (Highlight)", Description = "Hellerer Rand für Außenrahmen")]
        public Color OuterBorderHighlightColor { get; set; }

        [Display(Name = "Außenrahmen (dunkel)", Description = "Dunklerer Rand für Außenrahmen")]
        public Color OuterBorderDarkColor { get; set; }

        [Display(Name = "Innenrahmen (Highlight)", Description = "Hellerer Rand für Innenrahmen")]
        public Color InnerBorderHighlightColor { get; set; }

        [Display(Name = "Innenrahmen (dunkel)", Description = "Dunklerer Rand für Innenrahmen")]
        public Color InnerBorderDarkColor { get; set; }

        [Display(Name = "Textfarbe", Description = "Farbe des angezeigten Textes")]
        public Brush? Foreground { get; set; }

        [Display(Name = "Schriftart", Description = "Verwendete Schriftfamilie")]
        public FontFamily? FontFamily { get; set; }

        [Display(Name = "Schriftgröße", Description = "Größe des angezeigten Textes in Punkten")]
        public double FontSize { get; set; } = 14.0;

        [Display(Name = "Schriftgewicht", Description = "Dicke der Schrift (z. B. Normal, Bold)")]
        public FontWeight FontWeight { get; set; } = FontWeights.Normal;

        [Display(Name = "Schriftstil", Description = "Schriftschnitt wie Normal oder Kursiv")]
        public FontStyle FontStyle { get; set; } = FontStyles.Normal;

        [Display(Name = "Innenrahmenstärke", Description = "Dicke des inneren Rahmens")]
        public Thickness BorderThickness { get; set; }

        [Display(Name = "Außenrahmenstärke", Description = "Dicke des äußeren Rahmens")]
        public Thickness OuterBorderThickness { get; set; }

        [Display(Name = "3D-Gradient aktiv", Description = "Verwendet plastische 3D-Darstellung per Farbverlauf")]
        public bool Use3DGradient { get; set; }

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
        public double ScrollArrowHeight { get; set; } = 8.0;
    }

    public class BubbleVisualInfoBox
    {
        [Display(Name = "BubbleInfo Hintergrund", Description = "Hintergrundfarbe der BubbleInfo-Innenfläche")]
        public Brush? BubbleInfoBackground { get; set; }

        [Display(Name = "BubbleInfo-Innenrahmen", Description = "Farbe des inneren BubbleInfo-Rahmens")]
        public Brush? BubbleInfoBorder { get; set; }

        [Display(Name = "BubbleInfo-Außenrahmen", Description = "Farbe des äußeren BubbleInfo-Rahmens")]
        public Brush? BubbleInfoOuterBorderColor { get; set; }

        [Display(Name = "BubbleInfo Glanzfarbe", Description = "Lichtpunktfarbe für plastische Effekte")]
        public Color BubbleInfoHighlightColor { get; set; }

        [Display(Name = "BubbleInfo Außenrahmen (Highlight)", Description = "Hellerer Rand für Außenrahmen")]
        public Color BubbleInfoOuterBorderHighlightColor { get; set; }

        [Display(Name = "BubbleInfo Innenrahmen (Highlight)", Description = "Hellerer Rand für Innenrahmen")]
        public Color BubbleInfoInnerBorderHighlightColor { get; set; }

        [Display(Name = "BubbleInfo Textfarbe", Description = "Farbe des angezeigten Textes")]
        public Brush? BubbleInfoForeground { get; set; }

        [Display(Name = "BubbleInfo Innenrahmenstärke", Description = "Dicke des inneren Rahmens")]
        public Thickness BubbleInfoBorderThickness { get; set; }

        [Display(Name = "BubbleInfo Außenrahmenstärke", Description = "Dicke des äußeren Rahmens")]
        public Thickness BubbleInfoOuterBorderThickness { get; set; }
    }
}
