using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using BubbleControlls.Models;

namespace BubbleControlls.ControlViews
{
    /// <summary>
    /// Eine spezialisierte Canvas-Komponente, die UI-Elemente entlang einer elliptischen Bahn anordnen kann.
    /// Unterstützt Rotation per Mausrad und visuelle Hervorhebung bei Hover.
    /// </summary>
    public class BubbleRingControl : Canvas
    {
        private double _scrollTarget = 0.0;
        private Rect _scrollBackHitbox;
        private Rect _scrollForwardHitbox;
        
        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="BubbleRingControl"/>.
        /// </summary>
        public BubbleRingControl()
        {
            this.MouseWheel += OnMouseWheel;
            this.MouseEnter += (s, e) => IsGlowActive = true;
            this.MouseLeave += (s, e) => IsGlowActive = false;
            UpdateGlowEffect();
            this.MouseDown += OnMouseDown;
            CompositionTarget.Rendering += OnRenderFrame;
        }

        #region Properties

        /// <summary>
        /// Interner Scrollversatz entlang der Laufbahn (in Radiant oder Elementabständen).
        /// Wird durch Mausrad gesteuert. Kein DP.
        /// </summary>
        public double ScrollOffset { get; set; } = 0.0;
        
        #endregion

        #region Dependency Properties
        /// <summary>
        /// Farbe der Bahn.
        /// </summary>
        public static readonly DependencyProperty RingBackgroundProperty =
            DependencyProperty.Register(nameof(RingBackground), typeof(Brush), typeof(BubbleRingControl),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(80, 0, 255, 255)), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <inheritdoc cref="RingBackgroundProperty"/>
        public Brush RingBackground
        {
            get => (Brush)GetValue(RingBackgroundProperty);
            set => SetValue(RingBackgroundProperty, value);
        }
        
        /// <summary>
        /// Randfarbe der Bahn.
        /// </summary>
        public static readonly DependencyProperty RingBorderBrushProperty =
            DependencyProperty.Register(nameof(RingBorderBrush), typeof(Brush), typeof(BubbleRingControl),
                new FrameworkPropertyMetadata(
                    new SolidColorBrush(Color.FromArgb(150, 0, 255, 255)),
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    OnRingBorderBrushChanged
                ));
        
        private static void OnRingBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (BubbleRingControl)d;
            ctrl.UpdateGlowEffect();
        }
        
        /// <inheritdoc cref="RingBorderBrushProperty"/>
        public Brush RingBorderBrush
        {
            get => (Brush)GetValue(RingBorderBrushProperty);
            set => SetValue(RingBorderBrushProperty, value);
        }
        
        /// <summary>
        /// Transparenz der Bahn.
        /// </summary>
        public static readonly DependencyProperty RingOpacityProperty =
            DependencyProperty.Register(nameof(RingOpacity), typeof(int), typeof(BubbleRingControl),
                new FrameworkPropertyMetadata(50, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <inheritdoc cref="RingOpacityProperty"/>
        public int RingOpacity
        {
            get => (int)GetValue(RingOpacityProperty);
            set => SetValue(RingOpacityProperty, value);
        }
        
        /// <summary>
        /// Transparenz des Bahnrandes.
        /// </summary>
        public static readonly DependencyProperty RingBorderOpacityProperty =
            DependencyProperty.Register(nameof(RingBorderOpacity), typeof(int), typeof(BubbleRingControl),
                new FrameworkPropertyMetadata(80, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <inheritdoc cref="RingBorderOpacityProperty"/>
        public int RingBorderOpacity
        {
            get => (int)GetValue(RingBorderOpacityProperty);
            set => SetValue(RingBorderOpacityProperty, value);
        }
        
        /// <summary>
        /// Stärke des Bahnrandes.
        /// </summary>
        public static readonly DependencyProperty RingBorderThicknessProperty =
            DependencyProperty.Register(nameof(RingBorderThickness), typeof(int), typeof(BubbleRingControl),
                new FrameworkPropertyMetadata(2, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <inheritdoc cref="RingBorderThicknessProperty"/>
        public int RingBorderThickness
        {
            get => (int)GetValue(RingBorderThicknessProperty);
            set => SetValue(RingBorderThicknessProperty, value);
        }
        
        /// <summary>
        /// Zentrum der elliptischen Bahn, relativ zur Zeichenfläche.
        /// </summary>
        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register(nameof(Center), typeof(Point), typeof(BubbleRingControl),
                new FrameworkPropertyMetadata(new Point(150, 50), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <inheritdoc cref="CenterProperty"/>
        public Point Center
        {
            get => (Point)GetValue(CenterProperty);
            set => SetValue(CenterProperty, value);
        }

        /// <summary>
        /// Horizontale Ausdehnung der Bahn (halbe Breite der Ellipse).
        /// </summary>
        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register(nameof(RadiusX), typeof(double), typeof(BubbleRingControl),
                new FrameworkPropertyMetadata(150.0, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <inheritdoc cref="RadiusXProperty"/>
        public double RadiusX
        {
            get => (double)GetValue(RadiusXProperty);
            set => SetValue(RadiusXProperty, value);
        }

        /// <summary>
        /// Vertikale Ausdehnung der Bahn (halbe Höhe der Ellipse).
        /// </summary>
        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register(nameof(RadiusY), typeof(double), typeof(BubbleRingControl),
                new FrameworkPropertyMetadata(200.0, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <inheritdoc cref="RadiusYProperty"/>
        public double RadiusY
        {
            get => (double)GetValue(RadiusYProperty);
            set => SetValue(RadiusYProperty, value);
        }
        
        /// <summary>
        /// Startwinkel.
        /// </summary>
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register(nameof(StartAngle), typeof(double), typeof(BubbleRingControl),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Der Startwinkel des sichtbaren Bereichs in Grad. 0 = rechts, 90 = unten, 180 = links, 270 = oben.
        /// </summary>
        public double StartAngle
        {
            get => (double)GetValue(StartAngleProperty);
            set => SetValue(StartAngleProperty, value);
        }

        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register(nameof(EndAngle), typeof(double), typeof(BubbleRingControl),
                new FrameworkPropertyMetadata(90.0, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Der Endwinkel des sichtbaren Bereichs in Grad. 0 = rechts, 90 = unten, 180 = links, 270 = oben.
        /// </summary>
        public double EndAngle
        {
            get => (double)GetValue(EndAngleProperty);
            set => SetValue(EndAngleProperty, value);
        }
        
        /// <summary>
        /// Rotationswinkel der gesamten Bahn in Grad.
        /// </summary>
        public static readonly DependencyProperty RingRotationProperty =
            DependencyProperty.Register(nameof(RingRotation), typeof(double), typeof(BubbleRingControl),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <inheritdoc cref="RingRotationProperty"/>
        public double RingRotation
        {
            get => (double)GetValue(RingRotationProperty);
            set => SetValue(RingRotationProperty, value);
        }
        
        /// <summary>
        /// Breite der Bahn in Pixel (Dicke der gezeichneten Ringfläche).
        /// </summary>
        public static readonly DependencyProperty PathWidthProperty =
            DependencyProperty.Register(nameof(PathWidth), typeof(double), typeof(BubbleRingControl),
                new FrameworkPropertyMetadata(60.0, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <inheritdoc cref="PathWidthProperty"/>
        public double PathWidth
        {
            get => (double)GetValue(PathWidthProperty);
            set => SetValue(PathWidthProperty, value);
        }

        /// <summary>
        /// Gibt an, ob der Glow-Effekt bei Hover aktiv ist.
        /// </summary>
        public static readonly DependencyProperty IsGlowActiveProperty =
            DependencyProperty.Register(nameof(IsGlowActive), typeof(bool), typeof(BubbleRingControl),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <inheritdoc cref="IsGlowActiveProperty"/>
        public bool IsGlowActive
        {
            get => (bool)GetValue(IsGlowActiveProperty);
            set => SetValue(IsGlowActiveProperty, value);
        }

        /// <summary>
        /// Abstand zwischen zwei Elementen auf der Bahn.
        /// </summary>
        public static readonly DependencyProperty ElementDistanceProperty =
            DependencyProperty.Register(nameof(ElementDistance), typeof(double), typeof(BubbleRingControl),
                new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <inheritdoc cref="ElementDistanceProperty"/>
        public double ElementDistance
        {
            get => (double)GetValue(ElementDistanceProperty);
            set => SetValue(ElementDistanceProperty, value);
        }

        /// <summary>
        /// Aktuelle Ausrichtung der Laufbahn.
        /// </summary>
        public static readonly DependencyProperty TrackAlignmentProperty =
            DependencyProperty.Register(nameof(TrackAlignment), typeof(BubbleTrackAlignment), typeof(BubbleRingControl),
                new FrameworkPropertyMetadata(BubbleTrackAlignment.Center, FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <inheritdoc cref="TrackAlignmentProperty"/>
        public BubbleTrackAlignment TrackAlignment
        {
            get => (BubbleTrackAlignment)GetValue(TrackAlignmentProperty);
            set => SetValue(TrackAlignmentProperty, value);
        }
        
        #endregion

        #region Event Handlers

        /// <summary>
        /// Reagiert auf Mausradbewegung und verändert <see cref="CurrentAngle"/>, um die Kinder neu zu positionieren.
        /// </summary>
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            _scrollTarget += (e.Delta > 0 ? -1 : +1) * ElementDistance;
            InvalidateVisual(); // später durch Repositionierung ersetzen
        }

        // Klick auf Pfeile erkennen
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(this);

            if (_scrollBackHitbox.Contains(pos))
            {
                _scrollTarget = Math.Max(_scrollTarget - ElementDistance, 0);
                e.Handled = true;
            }
            else if (_scrollForwardHitbox.Contains(pos))
            {
                double maxOffset = Math.Max(0, (Children.Count - 1) * ElementDistance - Math.Abs(EndAngleRad - StartAngleRad));
                _scrollTarget = Math.Min(_scrollTarget + ElementDistance, maxOffset);
                e.Handled = true;
            }
        }
        #endregion
        
        #region Overrides
        /// <summary>
        /// Zeichnet den elliptischen Ring mit optionalem Glow-Effekt.
        /// </summary>
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            double outerRx = RadiusX;
            double outerRy = RadiusY;
            double innerRx = RadiusX - PathWidth;
            double innerRy = RadiusY - PathWidth;

            if (innerRx < 0 || innerRy < 0) return;

            // Erzeuge Geometrie für Ring (Zwischenraum zwischen zwei Ellipsen)
            StreamGeometry ringGeometry = new StreamGeometry();
            using (StreamGeometryContext ctx = ringGeometry.Open())
            {
                // Berechne Punkte
                Point outerStart = new Point(
                    Center.X + outerRx * Math.Cos(StartAngleRad + RingRotationRad),
                    Center.Y + outerRy * Math.Sin(StartAngleRad + RingRotationRad));

                Point outerEnd = new Point(
                    Center.X + outerRx * Math.Cos(EndAngleRad + RingRotationRad),
                    Center.Y + outerRy * Math.Sin(EndAngleRad + RingRotationRad));

                Point innerEnd = new Point(
                    Center.X + innerRx * Math.Cos(EndAngleRad + RingRotationRad),
                    Center.Y + innerRy * Math.Sin(EndAngleRad + RingRotationRad));

                Point innerStart = new Point(
                    Center.X + innerRx * Math.Cos(StartAngleRad + RingRotationRad),
                    Center.Y + innerRy * Math.Sin(StartAngleRad + RingRotationRad));

                // Winkeldifferenz
                double sweepAngle = EndAngleRad - StartAngleRad;
                bool isLargeArc = Math.Abs(sweepAngle) > Math.PI;
                var sweepDir = SweepDirection.Clockwise;

                ctx.BeginFigure(outerStart, true, true);

                // Äußerer Bogen
                ctx.ArcTo(outerEnd, new Size(outerRx, outerRy), 0, isLargeArc, 
                    sweepDir, true, false);

                // Verbindungslinie zu innerem Ende
                ctx.LineTo(innerEnd, true, false);

                // Innerer Bogen (gegenläufig)
                ctx.ArcTo(innerStart, new Size(innerRx, innerRy), 0, isLargeArc, 
                    SweepDirection.Counterclockwise, true, false);

                // Verbindung zurück zum Anfang (implizit durch isClosed = true)
            }

            ringGeometry.Freeze();

            // Füll- und Strichfarbe
            Brush fillBrush = RingBackground.Clone();
            if (fillBrush is SolidColorBrush scb1)
            {
                var c = scb1.Color;
                c.A = (byte)Math.Clamp(RingOpacity, 0, 255);
                fillBrush = new SolidColorBrush(c);
            }
            
            Pen borderPen = new Pen(RingBorderBrush.Clone(), RingBorderThickness);
            if (borderPen.Brush is SolidColorBrush p1)
            {
                var c = p1.Color;
                c.A = (byte)Math.Clamp(RingBorderOpacity, 0, 255);
                borderPen = new Pen(new SolidColorBrush(c), RingBorderThickness);
            }
            if (IsGlowActive)
            {
                // Transparenz anheben
                if (fillBrush is SolidColorBrush f)
                {
                    var c = f.Color;
                    c.A = (byte)Math.Min(255, c.A + 50);
                    fillBrush = new SolidColorBrush(c);
                }

                if (borderPen.Brush is SolidColorBrush b)
                {
                    var c = b.Color;
                    c.A = (byte)Math.Min(255, c.A + 50);
                    borderPen = new Pen(new SolidColorBrush(c), RingBorderThickness + 1);
                }
            }
            
            dc.DrawGeometry(fillBrush, borderPen, ringGeometry);
            
            
            // Zeichenlogik für Scrollpfeile + Klickflächen (ersetze im OnRender)
            _scrollBackHitbox = Rect.Empty;
            _scrollForwardHitbox = Rect.Empty;

            double totalSpan = (Children.Count - 1) * ElementDistance;
            double visibleSpan = Math.Abs(EndAngleRad - StartAngleRad);
            bool canScrollBack = ScrollOffset > 0;
            bool canScrollForward = totalSpan > visibleSpan + ScrollOffset;

            double arrowSize = 10;

            Point start = new Point(
                Center.X + RadiusX * Math.Cos(StartAngleRad + RingRotationRad),
                Center.Y + RadiusY * Math.Sin(StartAngleRad + RingRotationRad));
            Point end = new Point(
                Center.X + RadiusX * Math.Cos(EndAngleRad + RingRotationRad),
                Center.Y + RadiusY * Math.Sin(EndAngleRad + RingRotationRad));

            void DrawArrow(Point center, double angle, bool isVisible, out Rect hitbox)
            {
                hitbox = Rect.Empty;
                if (!isVisible) return;

                Vector dir = new Vector(Math.Cos(angle), Math.Sin(angle));
                Vector ortho = new Vector(-dir.Y, dir.X);

                Point p1 = center + dir * arrowSize;
                Point p2 = center - dir * arrowSize * 0.6 + ortho * arrowSize * 0.6;
                Point p3 = center - dir * arrowSize * 0.6 - ortho * arrowSize * 0.6;

                StreamGeometry geo = new StreamGeometry();
                using (var ctx = geo.Open())
                {
                    ctx.BeginFigure(p1, true, true);
                    ctx.LineTo(p2, true, false);
                    ctx.LineTo(p3, true, false);
                }
                geo.Freeze();

                dc.DrawGeometry(Brushes.LightGray, new Pen(Brushes.Black, 1), geo);

                hitbox = new Rect(p1, p2); // einfache Treffbox für Klick
            }

            // Pfeile zeichnen
            DrawArrow(start, StartAngleRad + RingRotationRad, canScrollBack, out _scrollBackHitbox);
            DrawArrow(end, EndAngleRad + RingRotationRad, canScrollForward, out _scrollForwardHitbox);
            _scrollTarget = Math.Clamp(_scrollTarget, 0, GetMaxScrollOffset());
        }
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in Children)
            {
                if (child != null)
                {
                    child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                }
            }

            return base.MeasureOverride(availableSize); // oder z.B. new Size(Width, Height);
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            double radiusX = RadiusX - PathWidth / 2.0;
            double radiusY = RadiusY - PathWidth / 2.0;
            int count = this.Children.Count;
            if (count == 0)
                return finalSize;

            double visibleSpan = (EndAngleRad - StartAngleRad + 2 * Math.PI) % (2 * Math.PI);
            double totalPixelSpan = (count - 1) * ElementDistance;

            double startAngle;
            switch (TrackAlignment)
            {
                case BubbleTrackAlignment.Center:
                    startAngle = StartAngleRad + (visibleSpan - totalPixelSpan / radiusY) / 2.0;
                    break;
                case BubbleTrackAlignment.Start:
                    startAngle = StartAngleRad;
                    break;
                case BubbleTrackAlignment.End:
                    startAngle = EndAngleRad - totalPixelSpan / radiusY;
                    break;
                default:
                    startAngle = StartAngleRad;
                    break;
            }

            ScrollOffset = Math.Clamp(ScrollOffset, 0, GetMaxScrollOffset());

            var positions = CalculateBubblePositionsWithEqualSpacing(
                Center,
                radiusX,
                radiusY,
                startAngle - ScrollOffset / radiusY,
                ElementDistance,
                count
            ).ToList();

            foreach (var (child, pos) in positions)
            {
                if (child == null) continue;

                // Berechne Rotation der Position um das Zentrum
                double dx = pos.X - Center.X;
                double dy = pos.Y - Center.Y;
                double x = pos.X;
                double y = pos.Y;

                // Sichtbarkeitsprüfung über Winkel
                double rotatedAngle = Math.Atan2(y - Center.Y, x - Center.X);
                double visibleStart = StartAngleRad + RingRotationRad;
                double visibleEnd = EndAngleRad + RingRotationRad;
                double norm = (rotatedAngle - visibleStart + 2 * Math.PI) % (2 * Math.PI);
                double range = (visibleEnd - visibleStart + 2 * Math.PI) % (2 * Math.PI);
                bool isVisible = norm <= range;

                child.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                if (!isVisible) continue;

                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size desired = child.DesiredSize;

                double left = x - desired.Width / 2;
                double top = y - desired.Height / 2;

                child.Arrange(new Rect(new Point(left, top), desired));
            }

            return finalSize;
        }
        #endregion
        
        #region Methods
        
        private double GetMaxScrollOffset()
        {
            double totalSpan = (Children.Count - 1) * ElementDistance;
            double visibleSpan = (EndAngleRad - StartAngleRad + 2 * Math.PI) % (2 * Math.PI);
            return Math.Max(0, totalSpan - visibleSpan);
        }
        bool IsAngleInRange(double angle, double start, double end)
        {
            double norm = (angle - start + 2 * Math.PI) % (2 * Math.PI);
            double range = (end - start + 2 * Math.PI) % (2 * Math.PI);
            return norm <= range;
        }
        
        private void UpdateGlowEffect()
        {
            if (IsGlowActive && RingBorderBrush is SolidColorBrush solid)
            {
                this.Effect = new DropShadowEffect
                {
                    Color = solid.Color,
                    BlurRadius = 40,
                    ShadowDepth = 0,
                    Opacity = 0.8
                };
            }
            else
            {
                this.Effect = null;
            }
        }
        
        // Scrollen animieren
        private void OnRenderFrame(object sender, EventArgs e)
        {
            if (Math.Abs(ScrollOffset - _scrollTarget) > 0.01)
            {
                ScrollOffset += (_scrollTarget - ScrollOffset) * 0.2;
                InvalidateArrange();
            }
        }
        
        /// <summary>
        /// Gibt den Startwinkel in Radiant zurück.
        /// </summary>
        private double StartAngleRad => StartAngle * Math.PI / 180.0;

        /// <summary>
        /// Gibt den Endwinkel in Radiant zurück.
        /// </summary>
        private double EndAngleRad => EndAngle * Math.PI / 180.0;
        
        /// <summary>
        /// Gibt die RingRotation in Radiant zurück.
        /// </summary>
        private double RingRotationRad => RingRotation * Math.PI / 180.0;
        
        private IEnumerable<(UIElement Element, Point Position)> CalculateBubblePositionsWithEqualSpacing(
            Point center, double radiusX, double radiusY, double startAngleRad, double distancePx, int maxElements)
        {
            var results = new List<(UIElement, Point)>();
            if (Children.Count == 0 || maxElements <= 0)
                return results;

            double angleStep = 0.005;
            double angle = startAngleRad;
            double rotatedStartX = center.X + radiusX * Math.Cos(angle + RingRotationRad);
            double rotatedStartY = center.Y + radiusY * Math.Sin(angle + RingRotationRad);
            Point last = new Point(rotatedStartX, rotatedStartY);
            int currentIndex = 0;

            for (int i = 0; i < Children.Count && currentIndex < maxElements; )
            {
                angle += angleStep;
                double x = center.X + radiusX * Math.Cos(angle + RingRotationRad);
                double y = center.Y + radiusY * Math.Sin(angle + RingRotationRad);
                Point current = new Point(x, y);

                double dist = Math.Sqrt(Math.Pow(current.X - last.X, 2) + Math.Pow(current.Y - last.Y, 2));

                if (dist >= distancePx)
                {
                    results.Add((Children[i], current));
                    last = current;
                    currentIndex++;
                    i++;
                }
            }

            return results;
        }
        #endregion
    }
}
