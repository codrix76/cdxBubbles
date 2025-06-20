using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        private double _scrollTarget;
        private Rect _scrollBackHitbox;
        private Rect _scrollForwardHitbox;
        private List<UIElement> _elements = new();
        private List<BubblePlacement> _positions = new();
        private bool _elementsPlaced;
        private const double ScrollMin = 0;
        private double _scrollMax;
        private bool _canScroll = true;
        private double _scrollStepSmall;
        private double _scrollStepLarge;
        private const double ScrollElementFactor = 0.6;
        private EllipsePath _ellipsePath = new (new Point(),0,0,0);
        private BubbleRingRenderData? _bubbleRingRenderData;
        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="BubbleRingControl"/>.
        /// </summary>
        public BubbleRingControl()
        {
            ApplyTheme(BubbleVisualThemes.Standard());
            this.MouseWheel += OnMouseWheel;
            this.MouseEnter += (_, _) => IsGlowActive = true;
            this.MouseLeave += (_, _) => IsGlowActive = false;
            this.MouseDown += OnMouseDown;
            this.IsVisibleChanged += (_, _)  =>
            {
                if (this.IsVisible)
                    CompositionTarget.Rendering += OnRenderFrame;
                else
                    CompositionTarget.Rendering -= OnRenderFrame;
            };
        }

        #region Properties
        public bool ForceRefresh { get; set; } = false;
        private double BubbleOffset { get; set; }
        /// <summary>
        /// Interner Scroll Versatz entlang der Laufbahn (in Radiant oder Elementabständen).
        /// Wird durch Mausrad gesteuert. Kein DP.
        /// </summary>
        private double ScrollOffset { get; set; }
        /// <summary>
        /// gibt an, ob das Menu zentriert ausgerichtet werden soll
        /// </summary>
        public bool IsCentered { get; set; }
        /// <summary>
        /// gibt an, ob die Reihenfolge invertiert werden soll
        /// </summary>
        public bool IsInverted { get; set; }
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
                    FrameworkPropertyMetadataOptions.AffectsRender
                ));
        
        
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

        private double MouseDeltaToRad(int delta)
        {
            return (delta / 120.0) * _scrollStepSmall;
        }
        /// <summary>
        /// Reagiert auf Mausradbewegung und verändert, um die Kinder neu zu positionieren.
        /// </summary>
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Debug.WriteLine($"OnMouseWheel: _canScroll: {_canScroll}");
            if (!_canScroll)
                return;
            double deltaRad = MouseDeltaToRad(e.Delta);
            // Maus ↓ → Inhalt ↑ → ScrollOffset steigt
            //_scrollTarget += IsInverted ? +deltaRad : -deltaRad;
            _scrollTarget += -deltaRad;
            Debug.WriteLine($"OnMouseWheel: ScrollTarget change: {_scrollTarget}");
            _scrollTarget = Math.Clamp(_scrollTarget, ScrollMin, _scrollMax);
            Debug.WriteLine($"OnMouseWheel: ScrollTarget: {_scrollTarget:F2}, ScrollOffset: {ScrollOffset:F2}, max: {_scrollMax}");
            InvalidateArrange();
        }

        // Klick auf Pfeile erkennen
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            double direction = IsInverted ? -1 : 1;
            Debug.WriteLine($"OnMouseDown: _canScroll{_canScroll}");
            if (!_canScroll)
                return;

            Point pos = e.GetPosition(this);

            if (_scrollBackHitbox.Contains(pos))
            {
                _scrollTarget = Math.Max(_scrollTarget - (direction * _scrollStepLarge), ScrollMin);
                Debug.WriteLine($"OnMouseDown: _scrollBackHitbox - _scrollTarget{_scrollTarget}, _scrollMin {ScrollMin}");
                e.Handled = true;
            }
            else if (_scrollForwardHitbox.Contains(pos))
            {
                _scrollTarget = Math.Min(_scrollTarget + (direction * _scrollStepLarge), _scrollMax);
                Debug.WriteLine($"OnMouseDown: _scrollForwardHitbox - _scrollTarget{_scrollTarget}, _scrollMax {_scrollMax}");
                e.Handled = true;
            }
            _scrollTarget = Math.Clamp(_scrollTarget, ScrollMin, _scrollMax);
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
            if (_bubbleRingRenderData == null || !_bubbleRingRenderData.initialized) return;
            BubbleRingRenderer.DrawRing(dc, _bubbleRingRenderData);

            Debug.WriteLine($"IsGlowActive: {IsGlowActive}");
            if (IsGlowActive)
                BubbleRingRenderer.DrawGlow(dc, _bubbleRingRenderData);

            double startAngleRad = GeometryHelper.DegToRad(_bubbleRingRenderData.StartAngleDeg + _bubbleRingRenderData.RotationDeg);
            double endAngleRad   = GeometryHelper.DegToRad(_bubbleRingRenderData.EndAngleDeg + _bubbleRingRenderData.RotationDeg);

            //var scrollOffsetstart = GeometryHelper.EllipticalPoint(
            //    _bubbleRingRenderData.Center,
            //    _bubbleRingRenderData.RadiusX,
            //    _bubbleRingRenderData.RadiusY,
            //    StartAngleRad
            //);
            //dc.DrawLine(new Pen(Brushes.Green, 1), _bubbleRingRenderData.Center, scrollOffsetstart);

            //var scrollOffset = GeometryHelper.EllipticalPoint(
            //    _bubbleRingRenderData.Center,
            //    _bubbleRingRenderData.RadiusX,
            //    _bubbleRingRenderData.RadiusY,
            //    ScrollOffset
            //);
            //dc.DrawLine(new Pen(Brushes.OrangeRed, 1), _bubbleRingRenderData.Center, scrollOffset);

            //var scrollOffsetEnd = GeometryHelper.EllipticalPoint(
            //    _bubbleRingRenderData.Center,
            //    _bubbleRingRenderData.RadiusX,
            //    _bubbleRingRenderData.RadiusY,
            //    EndAngleRad
            //);
            //dc.DrawLine(new Pen(Brushes.Yellow, 1), _bubbleRingRenderData.Center, scrollOffsetEnd);

            //var scrolltarget = GeometryHelper.EllipticalPoint(
            //    _bubbleRingRenderData.Center,
            //    _bubbleRingRenderData.RadiusX,
            //    _bubbleRingRenderData.RadiusY,
            //    _scrollTarget
            //);
            //dc.DrawLine(new Pen(Brushes.LightSkyBlue, 1), _bubbleRingRenderData.Center, scrolltarget);

            //var scrollmid = GeometryHelper.EllipticalPoint(
            //    _bubbleRingRenderData.Center,
            //    _bubbleRingRenderData.RadiusX,
            //    _bubbleRingRenderData.RadiusY,
            //    BubbleOffset
            //);
            //dc.DrawLine(new Pen(Brushes.Blue, 1), _bubbleRingRenderData.Center, scrollmid);

            _scrollBackHitbox = BubbleRingRenderer.DrawArrow(
                
                dc,
                GeometryHelper.EllipticalPoint(_bubbleRingRenderData.Center, _bubbleRingRenderData.RadiusX, _bubbleRingRenderData.RadiusY, startAngleRad),
                startAngleRad,
                true,
                _bubbleRingRenderData.Border,
                _bubbleRingRenderData.PathWidth,
                ScrollArrowHeight
            );

            _scrollForwardHitbox = BubbleRingRenderer.DrawArrow(
                dc,
                GeometryHelper.EllipticalPoint(_bubbleRingRenderData.Center, _bubbleRingRenderData.RadiusX, _bubbleRingRenderData.RadiusY, endAngleRad),
                endAngleRad,
                false,
                _bubbleRingRenderData.Border,
                _bubbleRingRenderData.PathWidth,
                ScrollArrowHeight
            );
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

            if (rePosition )
            {
                _positions = new List<BubblePlacement>();
                AdjustPlacement();
                UpdateScrollLimits();
            }

            for (int i = 0; i < _positions.Count; i++)
            {
                var placement = _positions[i];
                var child = Children[i];
                
                // double angle = NormalizeRad(placement.AngleRad);
                // double start = NormalizeRad(StartAngleRad + RingRotationRad);
                // double end = NormalizeRad(EndAngleRad + RingRotationRad);
                // bool isVisible =
                //     (end > start && angle >= start && angle <= end) ||
                //     (end < start && (angle >= start || angle <= end)); // Bereich über 0 hinaus

                //if (!isVisible)
                //{
                //    child.Arrange(new Rect(0, 0, 0, 0));
                //    continue;
                //}

                double left = placement.Center.X - placement.Size.Width / 2;
                double top = placement.Center.Y - placement.Size.Height / 2;
                child.Arrange(new Rect(new Point(left, top), placement.Size));
                //Debug.WriteLine($"ArrangeOverride| Left:{left}, Top:{top}, Placement:{placement}");
            }
            return finalSize;
        }
        
        #endregion
        
        #region Methods
        
        // Scrollen animieren
        private void OnRenderFrame(object? sender, EventArgs e)
        {
            if (!IsVisible) return;
            
            if (_elementsPlaced || ForceRefresh)
            {
                AdjustPlacement();
                UpdateScrollLimits();
                InvalidateArrange();
                _elementsPlaced = false;
                _bubbleRingRenderData = CreateRenderData();
                InvalidateVisual();
                ForceRefresh = false;
            }
            if (Math.Abs(ScrollOffset - _scrollTarget) > 0.001)
            {
                ScrollOffset += (_scrollTarget - ScrollOffset) * 0.25;
                AdjustPlacement();
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

        public void AddElements(IEnumerable<UIElement> elements)
        {
            _elementsPlaced = false;
            _elements = new List<UIElement>(elements);
            List<UIElement> sorted = IsInverted ? _elements.Reverse<UIElement>().ToList() : _elements;
            _positions = new List<BubblePlacement>();
            ScrollOffset = 0;
            
            Children.Clear();
            foreach (var el in sorted)
                Children.Add(el);

            _elementsPlaced = true;
            InvalidateMeasure();
        }

        public void RemoveElements()
        {
            _elementsPlaced = false;
            _elements = new List<UIElement>();
            _positions = new List<BubblePlacement>();
            ScrollOffset = 0;

            Children.Clear();

            InvalidateMeasure();
        }
        public void AdjustPlacement()
        {
            // Ellipsenparameter aus Control
            double radiusX = RadiusX - PathWidth / 2.0;
            double radiusY = RadiusY - PathWidth / 2.0;
            Point center = Center;
            double spacing = ElementDistance;
            
            _ellipsePath = new EllipsePath(center, radiusX, radiusY, 0);
            var placer = new BubblePlacer(_ellipsePath, spacing);
            var sizes = new List<Size>();
            foreach (UIElement child in Children)
            {
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                sizes.Add(child.DesiredSize);
            }
            
            double baseStart = GeometryHelper.NormalizeRad(StartAngleRad  + BubbleOffset - ScrollOffset);
            
            _positions = placer.PlaceBubbles(
                sizes,
                baseStart
            ).ToList();
            //foreach (var pos in _positions)
            //{
            //    Debug.WriteLine($"AdjustPlacement : {pos}");
            //}
            //Debug.WriteLine("----------------------------------------");
        }
        public void UpdateScrollLimits()
        {
            if (_positions.Count == 0 || _elements.Count == 0)
            {
                _canScroll = false;
                _scrollMax = 0;
                return;
            }
            
            // Gesamt-Radion der Elemente
            double startRad = GeometryHelper.NormalizeRad(_positions.First().AngleRad);
            double endRad = _positions.Last().AngleRad;
            double contentRad = GeometryHelper.GetArcBetween(startRad, endRad);
            // Radion des sichtbaren Bereichs
            double visibleRad = GeometryHelper.GetArcBetween(StartAngleRad, EndAngleRad);
            double visibleRad2 = GeometryHelper.GetArcClockwise(StartAngleRad, EndAngleRad);
            double rawSpacingRad = GeometryHelper.GetAngleAfterDistance(_ellipsePath, StartAngleRad, ElementDistance);
            double spacingrad = GeometryHelper.NormalizeRad(rawSpacingRad - StartAngleRad);
            
            // Radian Abstand zwischen Elementen
            double elementRad = GeometryHelper.ComputeDeltaTheta(_positions.First(), RadiusX, RadiusY);
            _scrollStepSmall = elementRad * ScrollElementFactor;
            _scrollStepLarge = elementRad;
            
            // Werte setzen
            _canScroll = contentRad > visibleRad2;
            if (_canScroll)
            {
                _scrollMax = Math.Abs(visibleRad - endRad - (elementRad ));
                _scrollTarget = IsInverted ? _scrollMax : ScrollMin;
                ScrollOffset = IsInverted ? _scrollMax - 0.002: ScrollMin;
                BubbleOffset = ScrollMin;
            }
            else
            {
                _scrollTarget = ScrollMin;
                _scrollMax = ScrollMin;
                ScrollOffset = 0.002;   // triggert einmal die einen mini scrolling in OnRenderFrame
                if (IsCentered)
                {
                    double contMid = contentRad == 0 ? elementRad / 2 : contentRad;
                    
                    BubbleOffset = (visibleRad2 - contMid - elementRad - spacingrad) / 2 ;
                }
                else
                {
                    double diff = Math.Abs(visibleRad2 - contentRad - elementRad * 2);
                    BubbleOffset = IsInverted ? diff : ScrollMin;
                }
            }


            // Debug
            Debug.WriteLine($"[UpdateScrollLimits] visibleRad: {visibleRad:F2}, contentRad: {contentRad:F2}," +
                            $" min: {ScrollMin:F2}, max: {_scrollMax:F2}, scrollable: {_canScroll}" +
                            $" ScrollOffset: {ScrollOffset}, scrollTarget: {_scrollTarget}");
        }
        
        /// <summary>
        /// setzt ein gewünschtes Theme für das Objekt
        /// </summary>
        /// <param name="style">BubbleVisualTheme</param>
        public void ApplyTheme(BubbleVisualTheme style)
        {
            RingBackground = style.BubbleRingVisuals.RingBackground!;
            RingBorderBrush = style.BubbleRingVisuals.RingBorderBrush!;
            RingOpacity = style.BubbleRingVisuals.RingOpacity;
            RingBorderOpacity = style.BubbleRingVisuals.RingBorderOpacity;
            RingBorderThickness = style.BubbleRingVisuals.RingBorderThickness;
            ScrollArrowHeight = style.BubbleRingVisuals.ScrollArrowHeight;
        }
        
        private BubbleRingRenderData CreateRenderData()
        {
            return new BubbleRingRenderData
            {
                initialized = true,
                Center = Center,
                RadiusX = RadiusX,
                RadiusY = RadiusY,
                PathWidth = PathWidth,
                StartAngleDeg = StartAngle,
                EndAngleDeg = EndAngle,
                RotationDeg = RingRotation,
                Fill = RingBackground.Clone(),
                Border = RingBorderBrush.Clone(),
                FillOpacity = RingOpacity,
                BorderOpacity = RingBorderOpacity,
                BorderThickness = RingBorderThickness,
                StartAngleRad = StartAngleRad,
                EndAngleRad = EndAngleRad,
                RingRotationRad = RingRotationRad,
                RingOpacity = RingOpacity,
                Placements = _positions.ToList()
            };
        }

        public void Refresh()
        {
            ForceRefresh = true;
        }
        
        #endregion
    }
}
