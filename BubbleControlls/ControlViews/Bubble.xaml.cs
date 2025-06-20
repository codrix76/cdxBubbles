using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using BubbleControlls.Models;
using BubbleControlls.ViewModels;

namespace BubbleControlls.ControlViews
{
    public partial class Bubble
    {
        public Bubble()
        {
            InitializeComponent();
            ApplyTheme(BubbleVisualThemes.Standard());
            this.Loaded += (_, _)  => {
                if (this.DataContext == null)
                {
                    this.DataContext = new BubbleViewModel();
                    var current = this.TextIconLayout;
                    OnTextIconLayoutChanged(this, new DependencyPropertyChangedEventArgs(
                        TextIconLayoutProperty,
                        current, // oldValue (irrelevant hier)
                        current  // newValue
                    ));
                }
            };
            InnerBorder.MouseEnter += InnerBorder_MouseEnter;
            InnerBorder.MouseLeave += InnerBorder_MouseLeave;
            InnerBorder.MouseLeftButtonDown += OnMouseDown;
            InnerBorder.MouseLeftButtonUp += OnMouseUp;
            InnerBorder.MouseRightButtonDown += OnMouseRightDown;
            InnerBorder.MouseRightButtonUp += OnMouseRightUp;
            this.MouseLeftButtonDown += (_, _)  =>
            {
                this.Focus(); // Fokus setzen
            };
            InnerBorder.IsHitTestVisible = true;
            this.FocusVisualStyle = null;
        }

        private void InnerBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            InnerBorder.Effect = new DropShadowEffect
            {
                Color = ((SolidColorBrush)BackgroundBrush).Color,
                BlurRadius = 15,
                ShadowDepth = 0,
                Opacity = 0.7
            };
        }

        private void InnerBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            InnerBorder.Effect = null;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            InnerBorder.RenderTransform = new ScaleTransform(
                0.95, 0.95,
                InnerBorder.ActualWidth / 2,
                InnerBorder.ActualHeight / 2);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            InnerBorder.RenderTransform = null;
        }
        
        private void OnMouseRightDown(object sender, MouseButtonEventArgs e)
        {
            InnerBorder.RenderTransform = new ScaleTransform(
                0.95, 0.95,
                InnerBorder.ActualWidth / 2,
                InnerBorder.ActualHeight / 2);
        }

        private void OnMouseRightUp(object sender, MouseButtonEventArgs e)
        {
            InnerBorder.RenderTransform = null;
        }
        // private void OnMouseLeaveCancelClickEffect(object sender, MouseEventArgs e)
        // {
        //     InnerBorder.RenderTransform = null;
        // }
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            ActivateGlow(); // deine visuelle Reaktion
        }
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            DeactivateGlow(); // Glow entfernen
        }
        public void ActivateGlow()
        {
            var animation = new DoubleAnimation(0, 0.8, TimeSpan.FromMilliseconds(300));
            var effect = new DropShadowEffect
            {
                Color = ((SolidColorBrush)OuterBorderBrush).Color,
                BlurRadius = 30,
                ShadowDepth = 0,
                Opacity = 0
            };
            OuterBorder.Effect = effect;
            effect.BeginAnimation(DropShadowEffect.OpacityProperty, animation);
        }
        public void DeactivateGlow()
        {
            OuterBorder.Effect = null;
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            double outerHeight = OuterBorder.ActualHeight;
            double offset = BorderDistance;
            double cornerRadiusOuter = outerHeight / 2;
            double cornerRadiusInner = Math.Max(0, (outerHeight - offset) / 2);

            OuterBorder.CornerRadius = new CornerRadius(cornerRadiusOuter);
            InnerBorder.CornerRadius = new CornerRadius(cornerRadiusInner);

            // Abstand über Padding regeln, nicht über Height
            InnerBorder.ClearValue(HeightProperty);
            InnerBorder.Margin = new Thickness(offset / 2);

            // Bubble darf nicht zu schmal werden (Text + Icon + Puffer)
            OuterBorder.MinWidth = Math.Max(outerHeight, InnerBorder.DesiredSize.Width + (offset*3));

        }

        // ---- Farben & Brushes ----

        public Brush BackgroundBrush
        {
            get => (Brush)GetValue(BackgroundBrushProperty);
            set => SetValue(BackgroundBrushProperty, value);
        }
        public static readonly DependencyProperty BackgroundBrushProperty =
            DependencyProperty.Register(nameof(BackgroundBrush), typeof(Brush), typeof(Bubble),
                new PropertyMetadata(Brushes.SteelBlue, OnBackgroundBrushChanged));

        private static void OnBackgroundBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Bubble b && b.InnerBorder != null)
                b.InnerBorder.Background = (Brush)e.NewValue;
        }

        public Brush BorderBrushInner
        {
            get => (Brush)GetValue(BorderBrushInnerProperty);
            set => SetValue(BorderBrushInnerProperty, value);
        }
        public static readonly DependencyProperty BorderBrushInnerProperty =
            DependencyProperty.Register(nameof(BorderBrushInner), typeof(Brush), typeof(Bubble),
                new PropertyMetadata(Brushes.CornflowerBlue, OnBorderBrushInnerChanged));

        private static void OnBorderBrushInnerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Bubble b && b.InnerBorder != null)
                b.InnerBorder.BorderBrush = (Brush)e.NewValue;
        }

        public Brush OuterBorderBrush
        {
            get => (Brush)GetValue(OuterBorderBrushProperty);
            set => SetValue(OuterBorderBrushProperty, value);
        }
        public static readonly DependencyProperty OuterBorderBrushProperty =
            DependencyProperty.Register(nameof(OuterBorderBrush), typeof(Brush), typeof(Bubble),
                new PropertyMetadata(Brushes.Gray, OnOuterBorderBrushChanged));

        private static void OnOuterBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Bubble b && b.OuterBorder != null)
                b.OuterBorder.BorderBrush = (Brush)e.NewValue;
        }

        public Color OuterBorderHighlightColor
        {
            get => (Color)GetValue(OuterBorderHighlightColorProperty);
            set => SetValue(OuterBorderHighlightColorProperty, value);
        }
        public static readonly DependencyProperty OuterBorderHighlightColorProperty =
            DependencyProperty.Register(nameof(OuterBorderHighlightColor), typeof(Color), typeof(Bubble),
                new PropertyMetadata(Colors.WhiteSmoke));

        public Color OuterBorderDarkColor
        {
            get => (Color)GetValue(OuterBorderDarkColorProperty);
            set => SetValue(OuterBorderDarkColorProperty, value);
        }
        public static readonly DependencyProperty OuterBorderDarkColorProperty =
            DependencyProperty.Register(nameof(OuterBorderDarkColor), typeof(Color), typeof(Bubble),
                new PropertyMetadata(Colors.DarkSlateBlue));

        public Color InnerBorderHighlightColor
        {
            get => (Color)GetValue(InnerBorderHighlightColorProperty);
            set => SetValue(InnerBorderHighlightColorProperty, value);
        }
        public static readonly DependencyProperty InnerBorderHighlightColorProperty =
            DependencyProperty.Register(nameof(InnerBorderHighlightColor), typeof(Color), typeof(Bubble),
                new PropertyMetadata(Colors.WhiteSmoke));

        public Color InnerBorderDarkColor
        {
            get => (Color)GetValue(InnerBorderDarkColorProperty);
            set => SetValue(InnerBorderDarkColorProperty, value);
        }
        public static readonly DependencyProperty InnerBorderDarkColorProperty =
            DependencyProperty.Register(nameof(InnerBorderDarkColor), typeof(Color), typeof(Bubble),
                new PropertyMetadata(Colors.Black));

        public Color BackgroundHighlightColor
        {
            get => (Color)GetValue(BackgroundHighlightColorProperty);
            set => SetValue(BackgroundHighlightColorProperty, value);
        }
        public static readonly DependencyProperty BackgroundHighlightColorProperty =
            DependencyProperty.Register(nameof(BackgroundHighlightColor), typeof(Color), typeof(Bubble),
                new PropertyMetadata(Colors.WhiteSmoke));

        public Color BackgroundDarkColor
        {
            get => (Color)GetValue(BackgroundDarkColorProperty);
            set => SetValue(BackgroundDarkColorProperty, value);
        }
        public static readonly DependencyProperty BackgroundDarkColorProperty =
            DependencyProperty.Register(nameof(BackgroundDarkColor), typeof(Color), typeof(Bubble),
                new PropertyMetadata(Colors.Black));


        // ---- Rahmen & Layout ----
        public BubbleRenderStyle RenderStyle
        {
            get => (BubbleRenderStyle)GetValue(RenderStyleProperty);
            set => SetValue(RenderStyleProperty, value);
        }

        public static readonly DependencyProperty RenderStyleProperty =
            DependencyProperty.Register(nameof(RenderStyle), typeof(BubbleRenderStyle), typeof(Bubble),
                new PropertyMetadata(BubbleRenderStyle.StylePlane, OnRenderStyleChanged));

        private static void OnRenderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Bubble b || b.InnerBorder == null)
                return;

            switch ((BubbleRenderStyle)e.NewValue)
            {
                case BubbleRenderStyle.StylePlane:
                    b.InnerBorder.Background = b.BackgroundBrush;
                    break;

                case BubbleRenderStyle.Style3D:
                    b.InnerBorder.Background = new RadialGradientBrush
                    {
                        GradientOrigin = new Point(0.3, 0.3),
                        Center = new Point(0.5, 0.5),
                        RadiusX = 0.6,
                        RadiusY = 0.6,
                        GradientStops = new GradientStopCollection
                        {
                            new GradientStop(b.BackgroundHighlightColor, 0.0),
                            new GradientStop(b.BackgroundBrush is SolidColorBrush sb ? sb.Color : Colors.SteelBlue, 0.6),
                            new GradientStop(Darken(b.BackgroundDarkColor, 0.4f), 1.0)
                        }
                    };
                    b.OuterBorder.BorderBrush = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1),
                        GradientStops = new GradientStopCollection
                        {
                            new GradientStop(b.OuterBorderHighlightColor, 0),
                            new GradientStop(b.OuterBorderBrush is SolidColorBrush sbo ? sbo.Color :Colors.CornflowerBlue, 0.5),
                            new GradientStop(b.OuterBorderDarkColor, 1)
                        }
                    };
                    b.InnerBorder.BorderBrush = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1),
                        GradientStops = new GradientStopCollection
                        {
                            new GradientStop(b.InnerBorderHighlightColor, 0),
                            new GradientStop(b.BorderBrushInner is SolidColorBrush sbi ? sbi.Color :Colors.CornflowerBlue, 0.5),
                            new GradientStop(b.InnerBorderDarkColor, 1)
                        }
                    };
                    break;
            }
        }
        private static Color Darken(Color color, float factor)
        {
            return Color.FromArgb(
                255, // volle Deckkraft
                (byte)(color.R * factor),
                (byte)(color.G * factor),
                (byte)(color.B * factor)
            );
        }
        public double BorderDistance
        {
            get => (double)GetValue(BorderDistanceProperty);
            set => SetValue(BorderDistanceProperty, value);
        }
        public static readonly DependencyProperty BorderDistanceProperty =
            DependencyProperty.Register(nameof(BorderDistance), typeof(double), typeof(Bubble),
                new PropertyMetadata(10.0));

        public Thickness InnerBorderThickness
        {
            get => (Thickness)GetValue(InnerBorderThicknessProperty);
            set => SetValue(InnerBorderThicknessProperty, value);
        }
        public static readonly DependencyProperty InnerBorderThicknessProperty =
            DependencyProperty.Register(nameof(InnerBorderThickness), typeof(Thickness), typeof(Bubble),
                new PropertyMetadata(new Thickness(2)));

        public Thickness OuterBorderThickness
        {
            get => (Thickness)GetValue(OuterBorderThicknessProperty);
            set => SetValue(OuterBorderThicknessProperty, value);
        }
        public static readonly DependencyProperty OuterBorderThicknessProperty =
            DependencyProperty.Register(nameof(OuterBorderThickness), typeof(Thickness), typeof(Bubble),
                new PropertyMetadata(new Thickness(2)));

        // ---- Text ----
        public string ToolTipText
        {
            get => (string)GetValue(ToolTipTextProperty);
            set => SetValue(ToolTipTextProperty, value);
        }

        public static readonly DependencyProperty ToolTipTextProperty =
            DependencyProperty.Register(nameof(ToolTipText), typeof(string), typeof(Bubble),
                new PropertyMetadata(string.Empty, OnToolTipTextChanged));

        private static void OnToolTipTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Bubble b)
            {
                ToolTipService.SetToolTip(b, e.NewValue);
            }
        }
        public string FontFamilyName
        {
            get => (string)GetValue(FontFamilyNameProperty);
            set => SetValue(FontFamilyNameProperty, value);
        }
        public static readonly DependencyProperty FontFamilyNameProperty =
            DependencyProperty.Register(nameof(FontFamilyName), typeof(string), typeof(Bubble),
                new PropertyMetadata("Segoe UI"));

        public double FontSizeValue
        {
            get => (double)GetValue(FontSizeValueProperty);
            set => SetValue(FontSizeValueProperty, value);
        }
        public static readonly DependencyProperty FontSizeValueProperty =
            DependencyProperty.Register(nameof(FontSizeValue), typeof(double), typeof(Bubble),
                new PropertyMetadata(14.0));

        public FontWeight FontWeightValue
        {
            get => (FontWeight)GetValue(FontWeightValueProperty);
            set => SetValue(FontWeightValueProperty, value);
        }
        public static readonly DependencyProperty FontWeightValueProperty =
            DependencyProperty.Register(nameof(FontWeightValue), typeof(FontWeight), typeof(Bubble),
                new PropertyMetadata(FontWeights.Bold));

        public FontStyle FontStyleValue
        {
            get => (FontStyle)GetValue(FontStyleValueProperty);
            set => SetValue(FontStyleValueProperty, value);
        }
        public static readonly DependencyProperty FontStyleValueProperty =
            DependencyProperty.Register(nameof(FontStyleValue), typeof(FontStyle), typeof(Bubble),
                new PropertyMetadata(FontStyles.Normal));

        // ---- Text & Icon ----

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(Bubble),
                new PropertyMetadata(string.Empty , OnTextOrIconChanged));

        public ImageSource? Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(Bubble),
                new PropertyMetadata(null , OnTextOrIconChanged));
        private static void OnTextOrIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Bubble b)
            {
                OnTextIconLayoutChanged(b, new DependencyPropertyChangedEventArgs(TextIconLayoutProperty, b.TextIconLayout, b.TextIconLayout));
            }
        }
        public TextIconLayout TextIconLayout
        {
            get => (TextIconLayout)GetValue(TextIconLayoutProperty);
            set => SetValue(TextIconLayoutProperty, value);
        }
        public static readonly DependencyProperty TextIconLayoutProperty =
            DependencyProperty.Register(nameof(TextIconLayout), typeof(TextIconLayout), typeof(Bubble),
                new PropertyMetadata(TextIconLayout.Auto, OnTextIconLayoutChanged));
        private static void OnTextIconLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Bubble b || b.IconImage == null || b.BubbleText == null || b.ContentGrid == null)
                return;

            bool hasIcon = b.Icon != null;
            bool hasText = !string.IsNullOrWhiteSpace(b.Text);
            b.ContentGrid.RowDefinitions.Clear();
            b.ContentGrid.ColumnDefinitions.Clear();

            b.BubbleText.Visibility = Visibility.Visible;
            b.IconImage.Visibility = Visibility.Visible;


            if (hasIcon && hasText)
            {
                b.BubbleText.Padding = new Thickness(0);
                switch (b.TextIconLayout)
                {
                    case TextIconLayout.IconLeftOfText:
                        // Zeilen
                        b.ContentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
                        b.ContentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                        b.ContentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });

                        // Spalten
                        b.ContentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Icon
                        b.ContentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Text

                        Grid.SetRow(b.IconImage, 1);
                        Grid.SetColumn(b.IconImage, 0);

                        Grid.SetRow(b.BubbleText, 1);
                        Grid.SetColumn(b.BubbleText,1);

                        double leftPadding = Math.Max(6, b.InnerBorder.ActualWidth * 0.04);
                        b.IconImage.Margin = new Thickness(leftPadding, 0, 6, 0);

                        b.IconImage.HorizontalAlignment = HorizontalAlignment.Center;
                        b.IconImage.VerticalAlignment = VerticalAlignment.Center;
                        b.BubbleText.HorizontalAlignment = HorizontalAlignment.Left;
                        b.BubbleText.VerticalAlignment = VerticalAlignment.Center;
                        b.BubbleText.Margin = new Thickness(0, -1.5, 0, 0);
                        break;

                    case TextIconLayout.IconAboveText:
                    default:
                        b.ContentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.5, GridUnitType.Star) });
                        b.ContentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                        b.ContentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                        Grid.SetRow(b.IconImage, 1);
                        Grid.SetRow(b.BubbleText, 2);
                        //b.IconImage.Margin = new Thickness(0, 0, 0, -b.InnerBorder.ActualHeight / 7);

                        b.IconImage.HorizontalAlignment = HorizontalAlignment.Center;
                        b.IconImage.VerticalAlignment = VerticalAlignment.Bottom;
                        b.BubbleText.HorizontalAlignment = HorizontalAlignment.Center;
                        b.BubbleText.VerticalAlignment = VerticalAlignment.Top;
                        
                        break;
                }
            }
            else if (hasIcon)
            {
                b.ContentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
                b.ContentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.6, GridUnitType.Star) });
                b.ContentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
                Grid.SetRow(b.IconImage, 1);
                b.BubbleText.Visibility = Visibility.Collapsed;
                double leftMarge = -6;
                b.IconImage.Margin = new Thickness(leftMarge, 0, leftMarge, 0);
            }
            else if (hasText)
            {
                b.IconImage.Visibility = Visibility.Collapsed;
                b.BubbleText.Padding = new Thickness(12, 0, 12, 0);
            }
            b.ContentGrid.InvalidateMeasure();
            b.ContentGrid.InvalidateArrange();
            b.ContentGrid.UpdateLayout();
        }

        public void ApplyTheme(BubbleVisualTheme style)
        {
            // Farben
            BackgroundBrush = style.BubbleVisuals.Background!;
            OuterBorderBrush = style.BubbleVisuals.OuterBorderColor!;
            BorderBrushInner = style.BubbleVisuals.Border!;

            BackgroundHighlightColor = style.BubbleVisuals.HighlightColor;
            BackgroundDarkColor = style.BubbleVisuals.BackgroundDarkColor;
            OuterBorderHighlightColor = style.BubbleVisuals.OuterBorderHighlightColor;
            OuterBorderDarkColor = style.BubbleVisuals.OuterBorderDarkColor;
            InnerBorderHighlightColor = style.BubbleVisuals.InnerBorderHighlightColor;
            InnerBorderDarkColor = style.BubbleVisuals.InnerBorderDarkColor;

            // Font
            BubbleText.Foreground = style.BubbleVisuals.Foreground ?? Brushes.White;
            FontFamilyName = style.BubbleVisuals.FontFamily?.Source ?? "Segoe UI";
            FontSizeValue = style.BubbleVisuals.FontSize;
            FontWeightValue = style.BubbleVisuals.FontWeight;
            FontStyleValue = style.BubbleVisuals.FontStyle;

            // Ränder
            OuterBorderThickness = style.BubbleVisuals.OuterBorderThickness;
            InnerBorderThickness = style.BubbleVisuals.BorderThickness;

            // Layout-Stil
            RenderStyle = style.BubbleVisuals.Use3DGradient ? BubbleRenderStyle.Style3D : BubbleRenderStyle.StylePlane;
        }
    }
}