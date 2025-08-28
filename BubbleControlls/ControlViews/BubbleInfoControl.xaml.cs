using BubbleControlls.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BubbleControlls.ControlViews
{
    /// <summary>
    /// Interaktionslogik für BubbleInfoControl.xaml
    /// </summary>
    public partial class BubbleInfoControl : UserControl
    {
        public BubbleInfoControl()
        {
            InitializeComponent();
            this.DataContext = this;
            Loaded += (_, _) => ApplyAllVisuals();
        }

        public void ApplyTheme(BubbleVisualTheme? theme)
        {
            if (theme == null) return;

            // Farben
            BackgroundBrush = theme.InfoBoxVisuals.InfoBackground ?? BubbleVisualThemes.Standard().InfoBoxVisuals.InfoBackground!;
            BackgroundEffectBrush = new SolidColorBrush(theme.InfoBoxVisuals.InfoEffectColor1);

            OuterBorderBrush = theme.InfoBoxVisuals.InfoOuterBorderColor ?? BubbleVisualThemes.Standard().InfoBoxVisuals.InfoOuterBorderColor!;
            OuterBorderEffectBrush = new SolidColorBrush(theme.InfoBoxVisuals.InfoOuterBorderEffectColor1);

            InnerBorderBrush = theme.InfoBoxVisuals.InfoBorder ?? BubbleVisualThemes.Standard().InfoBoxVisuals.InfoBorder!;
            InnerBorderEffectBrush = new SolidColorBrush(theme.InfoBoxVisuals.InfoInnerBorderEffectColor1);

            Foreground = theme.InfoBoxVisuals.InfoForeground ?? BubbleVisualThemes.Standard().InfoBoxVisuals.InfoForeground!;

            // Schrift
            FontFamily = theme.FontFamily ?? BubbleVisualThemes.Standard().FontFamily!;
            FontSize = theme.FontSize;
            FontWeight = theme.FontWeight;
            FontStyle = theme.FontStyle;

            // Rahmen
            BorderThicknessOuter = theme.InfoBoxVisuals.InfoOuterBorderThickness;
            BorderThicknessInner = theme.InfoBoxVisuals.InfoBorderThickness;
            BorderOffset = 6.0; // Optional: Theme kann das auch mitliefern

            // Layout
            BackgroundOpacity = 0.85; // optional aus theme übernehmen, falls vorhanden

            // Text
            TextMargin = new Thickness(15);
        }

        private void ApplyAllVisuals()
        {
            var offset = BorderOffset;
            if (InnerBorder != null)
            {
                InnerBorder.Margin = new Thickness(offset);
            }

            OuterBorder.BorderBrush = OuterBorderBrush;
            InnerBorder!.BorderBrush = InnerBorderBrush;
            UpdateBackgroundBrush(BackgroundBrush);
            InnerBorder.Opacity = BackgroundOpacity;

            // Falls nötig:
            TextDisplay.Foreground = Foreground;
            TextDisplay.FontFamily = FontFamily;
            TextDisplay.FontSize = FontSize;
            TextDisplay.FontWeight = FontWeight;
            TextDisplay.FontStyle = FontStyle;
            TextDisplay.TextAlignment = TextAlignment;

        }
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == BorderOffsetProperty)
            {
                ApplyAllVisuals();
            }
        }

        private Brush CreateOuterBorderBrush()
        {
            Color col1 = ((SolidColorBrush)OuterBorderBrush).Color;
            Color col2 = ((SolidColorBrush)OuterBorderEffectBrush).Color;

            Brush res = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(col1, 0.0),
                    new GradientStop(col2, 0.5),
                    new GradientStop(col1, 1.0)
                }
            };
            return res;
        }

        private Brush CreateInnerBorderBrush()
        {
            Brush res = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(Color.FromArgb(255, 100, 149, 237), 0.0), // CornflowerBlue
                    new GradientStop(Color.FromArgb(255, 25, 25, 112), 1.0)    // MidnightBlue
                }
            };

            return res;
        }
        private Brush CreateBackgroundBrush()
        {
            Color col1 = ((SolidColorBrush)BackgroundBrush).Color;
            Color col2 = ((SolidColorBrush)BackgroundEffectBrush).Color;

            Brush res = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1),
                Opacity = 0.6,
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(col1, 0.0),
                    new GradientStop(col2, 0.5),
                    new GradientStop(col1, 1.0)
                }
            };

            return res;
        }

        private void UpdateBackgroundBrush(Brush brush)
        {
            if (brush is SolidColorBrush solid)
            {
                Color col1 = solid.Color;
                Color col2 = ((SolidColorBrush)BackgroundEffectBrush).Color;

                var gradient = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1),
                    Opacity = BackgroundOpacity, // Deine DP
                    GradientStops = new GradientStopCollection
                    {
                        new GradientStop(col1, 0.0),       // Start (dunkel)
                        new GradientStop(col2, 0.3),       // Aufhellung beginnt
                        new GradientStop(col2, 0.5),       // heller Bereich zentriert
                        new GradientStop(col2, 0.7),       // bleibt kurz hell
                        new GradientStop(col1, 1.0)        // Rückkehr zu dunkel
                    }
                };

                InnerBorder.Background = gradient;
            }
            else
            {
                // Fallback: direkt setzen, falls es ein anderer Brush ist
                InnerBorder.Background = brush;
            }
        }
        #region Common
        //public static readonly DependencyProperty BoxSizeToContentProperty =
        //    DependencyProperty.Register(nameof(BoxSizeToContent), typeof(SizeToContent), typeof(BubbleInfoControl),
        //new FrameworkPropertyMetadata(SizeToContent.Manual, OnBoxSizeToContentChanged));

        //public SizeToContent BoxSizeToContent
        //{
        //    get => (SizeToContent)GetValue(BoxSizeToContentProperty);
        //    set => SetValue(BoxSizeToContentProperty, value);
        //}
        //private static void OnBoxSizeToContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (d is BubbleInfoControl box)
        //    {
        //        box.SizeToContent = (SizeToContent)e.NewValue;
        //    }
        //}

        #endregion

        #region Farben & Brushes

        public static readonly DependencyProperty OuterBorderBrushProperty =
            DependencyProperty.Register(nameof(OuterBorderBrush), typeof(Brush), typeof(BubbleInfoControl),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(200, 200, 200)), FrameworkPropertyMetadataOptions.AffectsRender, OnVisualBrushChanged));

        public Brush OuterBorderBrush
        {
            get => (Brush)GetValue(OuterBorderBrushProperty);
            set => SetValue(OuterBorderBrushProperty, value);
        }

        public static readonly DependencyProperty OuterBorderEffectBrushProperty =
            DependencyProperty.Register(nameof(OuterBorderEffectBrush), typeof(Brush), typeof(BubbleInfoControl),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 255, 255)), FrameworkPropertyMetadataOptions.AffectsRender, OnVisualBrushChanged));

        public Brush OuterBorderEffectBrush
        {
            get => (Brush)GetValue(OuterBorderEffectBrushProperty);
            set => SetValue(OuterBorderEffectBrushProperty, value);
        }

        public static readonly DependencyProperty InnerBorderBrushProperty =
            DependencyProperty.Register(nameof(InnerBorderBrush), typeof(Brush), typeof(BubbleInfoControl),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(173, 173, 173)), FrameworkPropertyMetadataOptions.AffectsRender, OnVisualBrushChanged));

        public Brush InnerBorderBrush
        {
            get => (Brush)GetValue(InnerBorderBrushProperty);
            set => SetValue(InnerBorderBrushProperty, value);
        }

        public static readonly DependencyProperty InnerBorderEffectBrushProperty =
            DependencyProperty.Register(nameof(InnerBorderEffectBrush), typeof(Brush), typeof(BubbleInfoControl),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 255, 255)), FrameworkPropertyMetadataOptions.AffectsRender, OnVisualBrushChanged));

        public Brush InnerBorderEffectBrush
        {
            get => (Brush)GetValue(InnerBorderEffectBrushProperty);
            set => SetValue(InnerBorderEffectBrushProperty, value);
        }

        public static readonly DependencyProperty BackgroundBrushProperty =
            DependencyProperty.Register(nameof(BackgroundBrush), typeof(Brush), typeof(BubbleInfoControl),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(240, 240, 240)), FrameworkPropertyMetadataOptions.AffectsRender, OnVisualBrushChanged));

        public Brush BackgroundBrush
        {
            get => (Brush)GetValue(BackgroundBrushProperty);
            set => SetValue(BackgroundBrushProperty, value);
        }

        public static readonly DependencyProperty BackgroundEffectBrushProperty =
            DependencyProperty.Register(nameof(BackgroundEffectBrush), typeof(Brush), typeof(BubbleInfoControl),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 255, 255)), FrameworkPropertyMetadataOptions.AffectsRender, OnVisualBrushChanged));

        public Brush BackgroundEffectBrush
        {
            get => (Brush)GetValue(BackgroundEffectBrushProperty);
            set => SetValue(BackgroundEffectBrushProperty, value);
        }
        public static readonly DependencyProperty BackgroundOpacityProperty =
            DependencyProperty.Register(nameof(BackgroundOpacity), typeof(double), typeof(BubbleInfoControl),
        new FrameworkPropertyMetadata(0.9, FrameworkPropertyMetadataOptions.AffectsRender, OnVisualBrushChanged));

        public double BackgroundOpacity
        {
            get => (double)GetValue(BackgroundOpacityProperty);
            set => SetValue(BackgroundOpacityProperty, value);
        }

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(BubbleInfoControl),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(30, 30, 30)), FrameworkPropertyMetadataOptions.AffectsRender));

        public new Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        #endregion

        #region Schrift

        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register(nameof(FontFamily), typeof(FontFamily), typeof(BubbleInfoControl),
                new FrameworkPropertyMetadata(new FontFamily("Segoe UI"), FrameworkPropertyMetadataOptions.AffectsRender));

        public new FontFamily FontFamily
        {
            get => (FontFamily)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register(nameof(FontSize), typeof(double), typeof(BubbleInfoControl),
                new FrameworkPropertyMetadata(14.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public new double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly DependencyProperty FontWeightProperty =
            DependencyProperty.Register(nameof(FontWeight), typeof(FontWeight), typeof(BubbleInfoControl),
                new FrameworkPropertyMetadata(FontWeights.Normal, FrameworkPropertyMetadataOptions.AffectsRender));

        public new FontWeight FontWeight
        {
            get => (FontWeight)GetValue(FontWeightProperty);
            set => SetValue(FontWeightProperty, value);
        }

        public static readonly DependencyProperty FontStyleProperty =
            DependencyProperty.Register(nameof(FontStyle), typeof(FontStyle), typeof(BubbleInfoControl),
                new FrameworkPropertyMetadata(FontStyles.Normal, FrameworkPropertyMetadataOptions.AffectsRender));

        public new FontStyle FontStyle
        {
            get => (FontStyle)GetValue(FontStyleProperty);
            set => SetValue(FontStyleProperty, value);
        }

        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register(nameof(TextAlignment), typeof(TextAlignment), typeof(BubbleInfoControl),
                new FrameworkPropertyMetadata(TextAlignment.Left, FrameworkPropertyMetadataOptions.AffectsRender));

        public TextAlignment TextAlignment
        {
            get => (TextAlignment)GetValue(TextAlignmentProperty);
            set => SetValue(TextAlignmentProperty, value);
        }

        #endregion

        #region Rahmen

        public static readonly DependencyProperty BorderThicknessOuterProperty =
            DependencyProperty.Register(nameof(BorderThicknessOuter), typeof(Thickness), typeof(BubbleInfoControl),
                new FrameworkPropertyMetadata(new Thickness(1.5), FrameworkPropertyMetadataOptions.AffectsRender));

        public Thickness BorderThicknessOuter
        {
            get => (Thickness)GetValue(BorderThicknessOuterProperty);
            set => SetValue(BorderThicknessOuterProperty, value);
        }

        public static readonly DependencyProperty BorderThicknessInnerProperty =
            DependencyProperty.Register(nameof(BorderThicknessInner), typeof(Thickness), typeof(BubbleInfoControl),
                new FrameworkPropertyMetadata(new Thickness(1), FrameworkPropertyMetadataOptions.AffectsRender));

        public Thickness BorderThicknessInner
        {
            get => (Thickness)GetValue(BorderThicknessInnerProperty);
            set => SetValue(BorderThicknessInnerProperty, value);
        }

        public static readonly DependencyProperty BorderOffsetProperty =
            DependencyProperty.Register(nameof(BorderOffset), typeof(double), typeof(BubbleInfoControl),
                new FrameworkPropertyMetadata(5.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double BorderOffset
        {
            get => (double)GetValue(BorderOffsetProperty);
            set => SetValue(BorderOffsetProperty, value);
        }

        #endregion

        #region Textinhalt

        public static readonly DependencyProperty DisplayTextProperty =
            DependencyProperty.Register(nameof(DisplayText), typeof(string), typeof(BubbleInfoControl),
                new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender));

        public string DisplayText
        {
            get => (string)GetValue(DisplayTextProperty);
            set => SetValue(DisplayTextProperty, value);
        }

        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register(nameof(TextMargin), typeof(Thickness), typeof(BubbleInfoControl),
        new FrameworkPropertyMetadata(new Thickness(15), OnTextMarginChanged));

        public Thickness TextMargin
        {
            get => (Thickness)GetValue(TextMarginProperty);
            set => SetValue(TextMarginProperty, value);
        }
        private static void OnTextMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BubbleInfoControl box && box.TextDisplay != null)
            {
                box.TextDisplay.Margin = (Thickness)e.NewValue;
            }
        }
        #endregion

        #region Common
        public static readonly DependencyProperty DisplayIconProperty =
            DependencyProperty.Register(nameof(DisplayIcon), typeof(ImageSource), typeof(BubbleInfoControl),
        new FrameworkPropertyMetadata(null, OnDisplayIconChanged));

        public ImageSource DisplayIcon
        {
            get => (ImageSource)GetValue(DisplayIconProperty);
            set => SetValue(DisplayIconProperty, value);
        }
        private static void OnDisplayIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BubbleInfoControl control && control.InfoDisplayIcon != null)
            {
                control.InfoDisplayIcon.Visibility = e.NewValue is ImageSource ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        #endregion
        private static void OnVisualBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not BubbleInfoControl box)
                return;

            if (e.Property == OuterBorderBrushProperty && box.OuterBorder != null)
            {
                box.OuterBorder.BorderBrush = (Brush)e.NewValue;
                box.OuterBorder.BorderBrush = box.CreateOuterBorderBrush();
            }
            else if (e.Property == InnerBorderBrushProperty && box.InnerBorder != null)
            {
                box.InnerBorder.BorderBrush = (Brush)e.NewValue;
                box.InnerBorder.BorderBrush = box.CreateInnerBorderBrush();
            }
            else if (e.Property == BackgroundBrushProperty && box.InnerBorder != null)
            {
                box.UpdateBackgroundBrush((Brush)e.NewValue);
            }
            else if (e.Property == BackgroundOpacityProperty && box.InnerBorder != null)
            {
                box.InnerBorder.Opacity = (double)e.NewValue;
                box.InnerBorder.Background = box.CreateBackgroundBrush();
            }
        }
    }
}
