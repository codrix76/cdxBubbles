using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using BubbleControlls.Geometry;
using BubbleControlls.Helpers;
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
        private bool _canScrollBack = false;
        private bool _canScrollForward = false;
        private List<UIElement> _elements = new();
        private List<BubblePlacement> _positions = new();
        private bool _elementsPlaced = false;
        private double _scrollMin = 0;
        private double _scrollMax = 0;
        private bool _canScroll = true;
        private EllipsePath _ellipsePath = new EllipsePath(new Point(),0,0,0);
        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="BubbleRingControl"/>.
        /// </summary>
        public BubbleRingControl()
        {
            ApplyTheme(BubbleVisualThemes.Standard());
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
        /// <summary>
        /// Scrollschrittweite
        /// </summary>
        public double ScrollStep { get; set; } = 10.0;
        #endregion

        #region Dependency Properties
        /// <summary>
        /// Farbe der Bahn.
        /// </summary>
        public static readonly DependencyProperty RingBackgroundProperty =
            DependencyProperty.Register(nameof(RingBackground), typeof(Brush), typeof(BubbleRingControl),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(50, 100, 149, 237)), FrameworkPropertyMetadataOptions.AffectsRender));

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
                    new SolidColorBrush(Color.FromArgb(80, 100, 149, 237)),
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
        public static readonly DependencyProperty ScrollArrowHeightProperty =
            DependencyProperty.Register(nameof(ScrollArrowHeight), typeof(double), typeof(BubbleRingControl),
                new FrameworkPropertyMetadata(8.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double ScrollArrowHeight
        {
            get => (double)GetValue(ScrollArrowHeightProperty);
            set => SetValue(ScrollArrowHeightProperty, value);
        }
        
        #endregion

        #region Event Handlers

        /// <summary>
        /// Reagiert auf Mausradbewegung und verändert, um die Kinder neu zu positionieren.
        /// </summary>
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Debug.WriteLine($"OnMouseWheel: _canScroll{_canScroll}");
            if (!_canScroll)
                return;
            // Maus ↓ → Inhalt ↑ → ScrollOffset steigt
            _scrollTarget += (e.Delta < 0 ? +ScrollStep : -ScrollStep);

            _scrollTarget = Math.Clamp(_scrollTarget, _scrollMin, _scrollMax);
            Debug.WriteLine($"OnMouseWheel: ScrollTarget={_scrollTarget:F2}, ScrollOffset={ScrollOffset:F2}");
            InvalidateArrange(); // optional, wenn du sofort visuelle Reaktion willst
        }

        // Klick auf Pfeile erkennen
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine($"OnMouseDown: _canScroll{_canScroll}");
            if (!_canScroll)
                return;

            Point pos = e.GetPosition(this);

            if (_scrollBackHitbox.Contains(pos))
            {
                _scrollTarget = Math.Max(_scrollTarget - ScrollStep, _scrollMin);
                Debug.WriteLine($"OnMouseDown: _scrollBackHitbox - _scrollTarget{_scrollTarget}, _scrollMin {_scrollMin}");
                e.Handled = true;
            }
            else if (_scrollForwardHitbox.Contains(pos))
            {
                _scrollTarget = Math.Min(_scrollTarget + ScrollStep, _scrollMax);
                Debug.WriteLine($"OnMouseDown: _scrollForwardHitbox - _scrollTarget{_scrollTarget}, _scrollMax {_scrollMax}");
                e.Handled = true;
            }
            InvalidateArrange(); 
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

            Point start = new Point(
                Center.X + RadiusX * Math.Cos(StartAngleRad + RingRotationRad),
                Center.Y + RadiusY * Math.Sin(StartAngleRad + RingRotationRad));
            Point end = new Point(
                Center.X + RadiusX * Math.Cos(EndAngleRad + RingRotationRad),
                Center.Y + RadiusY * Math.Sin(EndAngleRad + RingRotationRad));

            // Pfeile zeichnen
            _scrollBackHitbox = DrawArrow(dc, start, StartAngleRad + RingRotationRad, _canScrollBack, true);
            _scrollForwardHitbox = DrawArrow(dc, end, EndAngleRad + RingRotationRad, _canScrollForward, false);
        }
        Rect DrawArrow(DrawingContext dc, Point center, double angle, bool isVisible, bool isStart)
        {
            Rect hitbox = new Rect(0,0,1,1) ;
            if (!isVisible) return hitbox;

            Vector dir = new Vector(Math.Cos(angle), Math.Sin(angle));
            Vector ortho = new Vector(-dir.Y, dir.X);
            double height = 10;
            
            Point p1 = new Point(
                center.X - PathWidth * Math.Cos(angle),
                center.Y - PathWidth * Math.Sin(angle));

            Point p2 = center;

            Point midpoint = new Point(
                (p1.X + p2.X) / 2,
                (p1.Y + p2.Y) / 2);
            Vector tangent = new Vector(-Math.Sin(angle), Math.Cos(angle));
            if (isStart)
                tangent *= -1;
            
            Point p3 = new Point(
                midpoint.X + height * tangent.X,
                midpoint.Y + height * tangent.Y);

            StreamGeometry geo = new StreamGeometry();
            using (var ctx = geo.Open())
            {
                ctx.BeginFigure(p1, true, true);
                ctx.LineTo(p2, true, false);
                ctx.LineTo(p3, true, false);
            }
            geo.Freeze();
            
            dc.DrawGeometry(RingBorderBrush, new Pen(RingBorderBrush, RingBorderThickness), geo);
            double minX = Math.Min(p1.X, Math.Min(p2.X, p3.X));
            double maxX = Math.Max(p1.X, Math.Max(p2.X, p3.X));
            double minY = Math.Min(p1.Y, Math.Min(p2.Y, p3.Y));
            double maxY = Math.Max(p1.Y, Math.Max(p2.Y, p3.Y));

            hitbox = new Rect(new Point(minX, minY), new Point(maxX, maxY));
            return hitbox;
        }
        
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var element in _elements)
            {
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }
            return base.MeasureOverride(availableSize);
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0)
                return finalSize;
            bool rePosition = false;
            if (_positions.Count == Children.Count)
            {
                for (int i  = 0; i < Children.Count; i++)
                {
                    Children[i].Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    var size = Children[i].DesiredSize;

                    if (size != _positions[i].Size)
                        rePosition = true;
                }
            }

            if (rePosition)
            {
                _positions = new List<BubblePlacement>();
                AdjustPlacement();
                UpdateScrollLimits();
            }

            for (int i = 0; i < _positions.Count; i++)
            {
                var placement = _positions[i];
                var child = Children[i];
                
                double angle = NormalizeAngle(placement.AngleRad);
                double start = NormalizeAngle(StartAngleRad + RingRotationRad);
                double end = NormalizeAngle(EndAngleRad + RingRotationRad);
                bool isVisible =
                    (end > start && angle >= start && angle <= end) ||
                    (end < start && (angle >= start || angle <= end)); // Bereich über 0 hinaus

                if (!isVisible)
                {
                    child.Arrange(new Rect(0, 0, 0, 0));
                    continue;
                }

                double left = placement.Center.X - placement.Size.Width / 2;
                double top = placement.Center.Y - placement.Size.Height / 2;
                child.Arrange(new Rect(new Point(left, top), placement.Size));
            }

            return finalSize;
        }
        
        #endregion
        
        #region Methods
        // Hilfsmethode innerhalb der Klasse
        double NormalizeAngle(double rad)
        {
            while (rad < 0) rad += 2 * Math.PI;
            while (rad >= 2 * Math.PI) rad -= 2 * Math.PI;
            return rad;
        }
        private double GetMaxScrollOffset()
        {
            double totalSpan = (Children.Count - 1) * ElementDistance;
            double visibleSpan = (EndAngleRad - StartAngleRad + 2 * Math.PI) % (2 * Math.PI);
            return Math.Max(0, totalSpan - visibleSpan);
        }
        private double GetMinScrollOffset()
        {
            // z.B. negative Scrollwerte ermöglichen, damit erste Bubble auch wieder sichtbar wird
            return -Children.Count * ElementDistance; // oder ein anderer sinnvoller Puffer
        }

        private void UpdateGlowEffect()
        {
            if (IsGlowActive && RingBorderBrush is SolidColorBrush solid)
            {
                this.Effect = new DropShadowEffect
                {
                    Color = solid.Color,
                    BlurRadius = 200,
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
            if (_elementsPlaced)
            {
                AdjustPlacement();
                UpdateScrollLimits();
                InvalidateArrange();
                _elementsPlaced = false; // <- einmalig
            }
            if (Math.Abs(ScrollOffset - _scrollTarget) > 0.01)
            {
                ScrollOffset += (_scrollTarget - ScrollOffset) * 0.8; 
                AdjustPlacement();
                UpdateScrollLimits();
                InvalidateArrange();
            }
            else
            {
                ScrollOffset = _scrollTarget; // <<< sauber einrasten
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
        
        private double Lerp(double a, double b, double t) => a + (b - a) * t;
        public void AddElements(IEnumerable<UIElement> elements)
        {
            _elementsPlaced = false;
            _elements = new List<UIElement>(elements);
            _positions = new List<BubblePlacement>();
            ScrollOffset = 0;

            Children.Clear();
            foreach (var el in _elements)
                Children.Add(el);

            _elementsPlaced = true;
            // Alle Children Angelegt nun Measure
            InvalidateMeasure();
        }

        private void AdjustPlacement()
        {
            // Ellipsenparameter aus Control
            double radiusX = RadiusX - PathWidth / 2.0;
            double radiusY = RadiusY - PathWidth / 2.0;
            Point center = Center;
            double rotation = RingRotation;

            double spacing = ElementDistance;
            
            _ellipsePath = new EllipsePath(center, radiusX, radiusY, rotation);
            var placer = new BubblePlacer(_ellipsePath, spacing);
            var sizes = new List<Size>();
            foreach (UIElement child in Children)
            {
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                sizes.Add(child.DesiredSize);
            }
            
            double baseStart = NormalizeAngle(StartAngleRad + RingRotationRad - ViewHelper.DegToRad(ScrollOffset));
            Debug.WriteLine($"ScrollOffset{ScrollOffset}, baseStart: {baseStart}, endAngleRad = {baseStart + (EndAngleRad - StartAngleRad)}");
            _positions = placer.PlaceBubbles(
                sizes,
                TrackAlignment,
                baseStart,
                baseStart + (EndAngleRad - StartAngleRad),
                ScrollOffset
            ).ToList();
            foreach (var p in  _positions )
            {
                //Debug.WriteLine(p.ToString());
            }
            Debug.WriteLine("----------------------------------------------------");
        }
        
        private void UpdateScrollLimits()
        {
            if (_positions.Count == 0 || _elements.Count == 0)
            {
                _canScroll = false;
                _canScrollBack = false;
                _canScrollForward = false;
                _scrollMin = _scrollMax = 0;
                return;
            }

            // Schritt 1: Gesamtlänge aller Elemente berechnen
            double totalLength = 0;
            foreach (var el in _elements)
            {
                Size size = el.DesiredSize;
                Vector tangent = new Vector(1, 0); // Approximation, da wir keinen realen Pfadpunkt haben
                double r = BubblePlacer.ComputeProjectedRadius(size, tangent);
                totalLength += 2 * r;
            }

            // Schritt 2: Platz, der entlang der Bahn maximal darstellbar ist (sichtbare Arc-Länge)
            double visibleLength = _ellipsePath.GetArcLengthBetween(StartAngleRad, EndAngleRad);

            // Schritt 3: Entscheidung
            Debug.WriteLine($"[ScrollLimit] totalLength: {totalLength:F2}, visibleLength: {visibleLength:F2}");
            _canScroll = totalLength > visibleLength;
            _scrollMin = 0.0;
            _scrollMax = Math.Max(0, totalLength - visibleLength);
            
            const double epsilon = 0.01;
            _canScrollBack = _canScroll && ScrollOffset > _scrollMin + epsilon;
            _canScrollForward = _canScroll && ScrollOffset < _scrollMax - epsilon;
            // Debug
            Debug.WriteLine($"[ScrollLimit] min: {_scrollMin:F2}, max: {_scrollMax:F2}, scrollable: {_canScroll}");
        }
        
        public void ApplyTheme(BubbleVisualTheme style)
        {
            RingBackground = style.RingBackground;
            RingBorderBrush = style.RingBorderBrush;
            RingOpacity = style.RingOpacity;
            RingBorderOpacity = style.RingBorderOpacity;
            RingBorderThickness = style.RingBorderThickness;
            ScrollArrowHeight = style.ScrollArrowHeight;
            
        }

        
        #endregion
    }
}
