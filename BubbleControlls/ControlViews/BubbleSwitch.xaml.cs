using BubbleControlls.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BubbleControlls.ControlViews
{
    /// <summary>
    /// Interaktionslogik für BubbleSwitch.xaml
    /// </summary>
    public partial class BubbleSwitch : UserControl
    {
        public event Action<BubbleSwitch>? Toggled;
        public event Action<BubbleSwitch>? Expanded;
        public event Action<BubbleSwitch>? Colapsed;
        public event Action<BubbleSwitch, MouseButtonEventArgs>? Clicked;
        public event Action<BubbleSwitch, MouseButtonEventArgs>? RightClicked;
        public event Action<BubbleSwitch>? Selected;
        #region Variablen
        public string Key { get; set; }
        private BubbleVisualTheme _theme = BubbleVisualThemes.Standard();
        private bool _isSwitchable = true;
        private bool _isSwitched = false;
        private bool _isSelected = false;
        private bool _isSelectable = false;
        #endregion

        #region Properties
        public bool IsSwitchable
        {
            get => _isSwitchable;
            set { 
                _isSwitchable = value;
                ToggleSwitch();
            }
        }
        public bool IsSwitched
        {
            get => _isSwitched;
            set { _isSwitched = value;
                ToggleSwitch();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set 
            {
                if (_isSelectable)
                {
                    _isSelected = value;
                    if (_isSelected)
                    {
                        Background = SelectionColor;
                    }
                    else
                    {
                        Background = Brushes.Transparent;
                    }
                    Selected?.Invoke(this);
                }
                
            }
        }
        public bool IsSelectable { get => _isSelectable; set => _isSelectable = value; }

        #endregion

        #region DependecyProperties
        public static readonly DependencyProperty InnerBackgroundProperty =
            DependencyProperty.Register(nameof(InnerBackground), typeof(Brush), typeof(BubbleSwitch),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(240, 240, 240))));

        public Brush InnerBackground
        {
            get => (Brush)GetValue(InnerBackgroundProperty);
            set => SetValue(InnerBackgroundProperty, value);
        }

        public static readonly DependencyProperty InnerBorderBrushProperty =
            DependencyProperty.Register(nameof(InnerBorderBrush), typeof(Brush), typeof(BubbleSwitch),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(173, 173, 173))));

        public Brush InnerBorderBrush
        {
            get => (Brush)GetValue(InnerBorderBrushProperty);
            set => SetValue(InnerBorderBrushProperty, value);
        }

        public static readonly DependencyProperty InnerBorderThicknessProperty =
            DependencyProperty.Register(nameof(InnerBorderThickness), typeof(Thickness), typeof(BubbleSwitch),
                new PropertyMetadata(new Thickness(2)));

        public Thickness InnerBorderThickness
        {
            get => (Thickness)GetValue(InnerBorderThicknessProperty);
            set => SetValue(InnerBorderThicknessProperty, value);
        }

        public static readonly DependencyProperty OuterBackgroundProperty =
            DependencyProperty.Register(nameof(OuterBackground), typeof(Brush), typeof(BubbleSwitch),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(200, 200, 200))));

        public static readonly DependencyProperty SelectionColorProperty =
            DependencyProperty.Register(nameof(SelectionColor), typeof(Brush), typeof(BubbleSwitch),
                new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public Brush SelectionColor
        {
            get => (Brush)GetValue(SelectionColorProperty);
            set => SetValue(SelectionColorProperty, value);
        }
        public Brush OuterBackground
        {
            get => (Brush)GetValue(OuterBackgroundProperty);
            set => SetValue(OuterBackgroundProperty, value);
        }

        public static readonly DependencyProperty OuterBorderBrushProperty =
            DependencyProperty.Register(nameof(OuterBorderBrush), typeof(Brush), typeof(BubbleSwitch),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(200, 200, 200))));

        public Brush OuterBorderBrush
        {
            get => (Brush)GetValue(OuterBorderBrushProperty);
            set => SetValue(OuterBorderBrushProperty, value);
        }

        public static readonly DependencyProperty OuterBorderThicknessProperty =
            DependencyProperty.Register(nameof(OuterBorderThickness), typeof(Thickness), typeof(BubbleSwitch),
                new PropertyMetadata(new Thickness(2)));

        public Thickness OuterBorderThickness
        {
            get => (Thickness)GetValue(OuterBorderThicknessProperty);
            set => SetValue(OuterBorderThicknessProperty, value);
        }

        public static readonly DependencyProperty SwitchLabelProperty =
            DependencyProperty.Register(
                nameof(SwitchLabel),
                typeof(string),
                typeof(BubbleSwitch),
                new PropertyMetadata("", OnLabelChanged));

        public string SwitchLabel
        {
            get => (string)GetValue(SwitchLabelProperty);
            set => SetValue(SwitchLabelProperty, value);
        }
        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as BubbleSwitch;
            if (control == null)
                return;

            control.SwitchLabelText.Text = e.NewValue as string;

            // z. B. automatische Breitenanpassung erzwingen
            control.InvalidateMeasure();
        }
        public static readonly DependencyProperty SwitchForegroundProperty =
            DependencyProperty.Register(nameof(SwitchForeground), typeof(Brush), typeof(BubbleSwitch), new PropertyMetadata(Brushes.Black));

        public Brush SwitchForeground
        {
            get => (Brush)GetValue(SwitchForegroundProperty);
            set => SetValue(SwitchForegroundProperty, value);
        }

        public static readonly DependencyProperty SwitchFontFamilyProperty =
            DependencyProperty.Register(nameof(SwitchFontFamily), typeof(FontFamily), typeof(BubbleSwitch), new PropertyMetadata(new FontFamily("Segoe UI")));

        public FontFamily SwitchFontFamily
        {
            get => (FontFamily)GetValue(SwitchFontFamilyProperty);
            set => SetValue(SwitchFontFamilyProperty, value);
        }

        public static readonly DependencyProperty SwitchFontSizeProperty =
            DependencyProperty.Register(nameof(SwitchFontSize), typeof(double), typeof(BubbleSwitch), new PropertyMetadata(12.0));

        public double SwitchFontSize
        {
            get => (double)GetValue(SwitchFontSizeProperty);
            set => SetValue(SwitchFontSizeProperty, value);
        }

        public static readonly DependencyProperty SwitchFontWeightProperty =
            DependencyProperty.Register(nameof(SwitchFontWeight), typeof(FontWeight), typeof(BubbleSwitch), new PropertyMetadata(FontWeights.Normal));

        public FontWeight SwitchFontWeight
        {
            get => (FontWeight)GetValue(SwitchFontWeightProperty);
            set => SetValue(SwitchFontWeightProperty, value);
        }

        public static readonly DependencyProperty SwitchFontStyleProperty =
            DependencyProperty.Register(nameof(SwitchFontStyle), typeof(FontStyle), typeof(BubbleSwitch), new PropertyMetadata(FontStyles.Normal));

        public FontStyle SwitchFontStyle
        {
            get => (FontStyle)GetValue(SwitchFontStyleProperty);
            set => SetValue(SwitchFontStyleProperty, value);
        }

        #endregion
        public BubbleSwitch()
        {
            InitializeComponent();
            this.DataContext = this;
            Loaded += (_, _) => {
                Dispatcher.BeginInvoke(new Action(() => ToggleSwitch()));
            };
            OuterBorder.MouseLeftButtonUp += OuterBorder_MouseLeftButtonUp;
            OuterBorder.MouseRightButtonUp += OuterBorder_MouseRightButtonUp;
            InnerBorder.MouseLeftButtonUp += InnerBorder_MouseLeftButtonUp;
            Key = "newSwitch";
        }


        public BubbleSwitch(string key)
        {
            InitializeComponent();
            this.DataContext = this;
            Loaded += (_, _) => {
                Dispatcher.BeginInvoke(new Action(() => ToggleSwitch()));
            };
            OuterBorder.MouseLeftButtonUp += OuterBorder_MouseLeftButtonUp;
            OuterBorder.MouseRightButtonUp += OuterBorder_MouseRightButtonUp;
            InnerBorder.MouseLeftButtonUp += InnerBorder_MouseLeftButtonUp;
            Key = key;
        }

        private void OuterBorder_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true; // verhindert, dass OuterBorder das Event auch bekommt
            RightClicked?.Invoke(this, e);
        }

        private void InnerBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true; // verhindert, dass OuterBorder das Event auch bekommt
            Clicked?.Invoke(this, e);

        }
        private void OuterBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!_isSwitchable)
                return;
            else
                _isSwitched = !_isSwitched;
            ToggleSwitch();
            Toggled?.Invoke(this);
            if (_isSwitched)
                Expanded?.Invoke(this);
            else
                Colapsed?.Invoke(this);
        }
        #region Methods
        public void SwitchON()
        {
            _isSwitched = true;
            ToggleSwitch(false);
        }
        public void SwitchOFF()
        { 
            _isSwitched = false;
            ToggleSwitch(false);
        }
        public void ApplyDefaults()
        {
            // Farben
            InnerBackground = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            InnerBorderBrush = new SolidColorBrush(Color.FromRgb(173, 173, 173));
            InnerBorderThickness = new Thickness(2);

            OuterBackground = new SolidColorBrush(Color.FromRgb(200, 200, 200));
            OuterBorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200));
            OuterBorderThickness = new Thickness(2);

            Foreground = new SolidColorBrush(Color.FromRgb(30, 30, 30));

            // Schrift
            FontFamily = new FontFamily("Segoe UI");
            FontSize = 12;
            FontWeight = FontWeights.Normal;
            FontStyle = FontStyles.Normal;

        }
        public void ApplyTheme(BubbleVisualTheme? theme)
        {
            if (theme == null) return;

            // Farben
            InnerBackground = theme.Background ?? BubbleVisualThemes.Standard().Background!;
            InnerBorderBrush = theme.Border ?? BubbleVisualThemes.Standard().Border!;
            InnerBorderThickness = theme.BorderThickness;

            OuterBackground = theme.BackgroundBack ?? BubbleVisualThemes.Standard().BackgroundBack!;
            OuterBorderBrush = theme.OuterBorderColor ?? BubbleVisualThemes.Standard().OuterBorderColor!;
            OuterBorderThickness = theme.OuterBorderThickness;

            Foreground = theme.Foreground;

            // Schrift
            FontFamily = theme.FontFamily;
            FontSize = theme.FontSize;
            FontWeight = theme.FontWeight;
            FontStyle = theme.FontStyle;

        }

        private void ToggleSwitch(bool raiseEvent = true)
        {
            if (OuterBorder.ActualWidth == 0)
                return; // Layout ist noch nicht bereit

            if (!_isSwitchable)
            {
                InnerBorder.Width = OuterBorder.ActualWidth;
                InnerBorder.HorizontalAlignment = HorizontalAlignment.Center;
            }
            else
            {
                OuterBorder.Width = InnerBorder.ActualWidth + 13;
                //InnerBorder.Width = OuterBorder.ActualWidth - 13;
                InnerBorder.HorizontalAlignment = _isSwitched ? HorizontalAlignment.Right : HorizontalAlignment.Left;
                //if (raiseEvent) Toggled?.Invoke(this);
            }
        }
        #endregion
    }
}
