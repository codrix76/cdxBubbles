using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BubbleControlls.Models;
using BubbleControlls.ViewModels;
using Timer = System.Timers.Timer;

namespace BubbleControlls.ControlViews
{
    public partial class BubbleMenu
    {
        #region Variablen
        private bool _isLoaded;
        private BubbleMenuViewModel _viewModel = new BubbleMenuViewModel();
        private double _bubbleMainMenuSize = 80;
        private double _bubbleMenuBigSize = 60;
        private double _bubbleMenuSmallSize = 35;
        private double _bubbleMenuSpacing = 10;
        private double _defaultMargin = 10;
        private double _menuWidth;
        private double _menuHeight;
        private Point _menuCenter = new Point(0, 0);
        private BubbleRingControl _pathMenuRing = new BubbleRingControl();
        private BubbleRingControl _selectedMenuRing = new BubbleRingControl();
        private BubbleRingControl _additionalMenuRing = new BubbleRingControl();
        private Timer _hideTimer;
        private bool _ringsVisible;
        private BubbleAlignmentValues alignmentValues = new BubbleAlignmentValues();
        #endregion

        #region Properties
        public double MenuHideSeconds { get; set; } = 3;
        public BubbleVisualTheme MenuStyleTheme { get; set; } = BubbleVisualThemes.Standard();
        public DistributionAlignmentType DistributionAlignment { get => _viewModel.DistributionAlignment;
            set => _viewModel.DistributionAlignment = value; }
        public BubbleMenuAlignmentType BubbleMenuAlignment
        {
            get => (BubbleMenuAlignmentType)GetValue(BubbleMenuAlignmentProperty);
            set => SetValue(BubbleMenuAlignmentProperty, value);
        }
        public static readonly DependencyProperty BubbleMenuAlignmentProperty =
            DependencyProperty.Register(nameof(BubbleMenuAlignment), typeof(BubbleMenuAlignmentType), typeof(BubbleMenu),
                new PropertyMetadata(BubbleMenuAlignmentType.TopLeftCorner, OnBubbleMenuAlignmentChanged));
        private static void OnBubbleMenuAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BubbleMenu b && b.ActualHeight > 0)
            {
                b.SetMainWindowAlignment();
            }
        }

        public double BubbleMainMenuSize
        {
            get => (double)GetValue(BubbleMainMenuSizeProperty);
            set => SetValue(BubbleMainMenuSizeProperty, value);
        }
        public static readonly DependencyProperty BubbleMainMenuSizeProperty =
            DependencyProperty.Register(nameof(BubbleMainMenuSize), typeof(double), typeof(BubbleMenu),
                new PropertyMetadata(80d, OnBubbleMainMenuSizeChanged));
        private static void OnBubbleMainMenuSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BubbleMenu b && b.ActualHeight > 0)
            {
                b._bubbleMainMenuSize = (double)e.NewValue;
                //b.SetMainMenuBubblePosition(b.BubbleMenuAlignment);
            }
        }


        public double BubbleMenuBigSize
        {
            get => (double)GetValue(BubbleMenuBigSizeProperty);
            set => SetValue(BubbleMenuBigSizeProperty, value);
        }
        public static readonly DependencyProperty BubbleMenuBigSizeProperty =
            DependencyProperty.Register(nameof(BubbleMenuBigSize), typeof(double), typeof(BubbleMenu),
                new PropertyMetadata(60d, OnBubbleMenuBigSizeChanged));
        private static void OnBubbleMenuBigSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BubbleMenu b && b.ActualHeight > 0)
            {
               b._bubbleMenuBigSize = (double)e.NewValue;
               //b.SetMainMenuBubblePosition(b.BubbleMenuAlignment);
            }
        }

        public double BubbleMenuSmallSize
        {
            get => (double)GetValue(BubbleMenuSmallSizeProperty);
            set => SetValue(BubbleMenuSmallSizeProperty, value);
        }
        public static readonly DependencyProperty BubbleMenuSmallSizeProperty =
            DependencyProperty.Register(nameof(BubbleMenuSmallSize), typeof(double), typeof(BubbleMenu),
                new PropertyMetadata(35d, OnBubbleMenuSmallSizeChanged));
        private static void OnBubbleMenuSmallSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BubbleMenu b && b.ActualHeight > 0)
            {
                b._bubbleMenuSmallSize = (double)e.NewValue;
                //b.SetMainMenuBubblePosition(b.BubbleMenuAlignment);
            }
        }
        public double BubbleMenuSpacing
        {
            get => (double)GetValue(BubbleMenuSpacingProperty);
            set => SetValue(BubbleMenuSpacingProperty, value);
        }
        public static readonly DependencyProperty BubbleMenuSpacingProperty =
            DependencyProperty.Register(nameof(BubbleMenuSpacing), typeof(double), typeof(BubbleMenu),
                new PropertyMetadata(10d, OnBubbleMenuSpacingChanged));
        private static void OnBubbleMenuSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BubbleMenu b && b.ActualHeight > 0)
            {
                b._bubbleMenuSpacing = (double)e.NewValue;
                //b.SetMainMenuBubblePosition(b.BubbleMenuAlignment);
            }
        }
        public BubbleMenuItem MainMenu
        {
            get { return _viewModel.BubbleMenuHandler.MainMenu; }
            set
            {
                _viewModel.BubbleMenuHandler.MainMenu = value;
                if (_isLoaded) BuildMenuVisual(); // oder Dispatcher.Invoke}
            }
        }
        #endregion
        public BubbleMenu()
        {
            _hideTimer = new Timer(MenuHideSeconds);
            _hideTimer.Stop();

            double[] menuSizes = new [] {_bubbleMainMenuSize,_bubbleMenuSmallSize, _bubbleMenuBigSize, _bubbleMenuBigSize };
            InitializeComponent();
            this.Loaded += (_, _) => {
                if (this.DataContext == null)
                {
                    this.DataContext = new BubbleMenuViewModel();
                    _isLoaded = true;
                    _viewModel.SetMenuLevelSizes(menuSizes,_bubbleMenuSpacing);
                    BuildMenuVisual();
                    SetMainWindowAlignment();
                }
                this.MouseEnter += (_, _) =>
                {
                    _hideTimer.Stop();
                    ShowRings();
                };

                this.MouseLeave += (_, _) =>
                {
                    StartHideTimer();
                };
            };
        }

        #region Events
        private void Bubble_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (sender is Bubble clickedBubble)
            {
                string bubbleId = clickedBubble.Name; // oder eigene ID per Tag, Property etc.
                _viewModel.BubbleClicked(bubbleId);   // Weiterleitung ins VM

                UpdateMenu();
            }
        }
        #endregion
        #region Methods
        private void BuildMenuVisual()
        {
            MenuCanvas.Children.Clear();

            // Ermittlung dder benötigten Mindestlängen
            double minLength = _viewModel.GetMenuLevelLenght;
            double minHeight = minLength;

            // Control Grösse anpassen
            // Menü Masse mit zusätzlichen Spacing
            _menuWidth = minLength + _defaultMargin + _bubbleMenuSpacing;
            _menuHeight = _menuWidth;
            
            alignmentValues = _viewModel.UpdateAlignmentValues(BubbleMenuAlignment,
                minHeight,minLength,_menuHeight, _menuWidth, BubbleMainMenuSize);

            _menuCenter = alignmentValues.MenuCenter;
            this.Width = alignmentValues.MenuWidth;
            this.Height = alignmentValues.MenuHeight;
            _pathMenuRing.Height = alignmentValues.MenuHeight;
            _pathMenuRing.Width = alignmentValues.MenuWidth;

            _pathMenuRing.StartAngle = alignmentValues.StartAngle;
            _pathMenuRing.EndAngle = alignmentValues.EndAngle;
            _selectedMenuRing.StartAngle = alignmentValues.StartAngle;
            _selectedMenuRing.EndAngle = alignmentValues.EndAngle;
            _additionalMenuRing.StartAngle = alignmentValues.StartAngle;
            _additionalMenuRing.EndAngle = alignmentValues.EndAngle;

            ShowMainMenu();
            BuildMenuRings();
        }
        /// <summary>
        /// Positioniert das MainWindow entsprechend des MenuAlignmentProperties
        /// </summary>
        public void SetMainWindowAlignment()
        {
            if (!_isLoaded) { return; }
            
            Window? mainWindow = Window.GetWindow(this);
            if (mainWindow != null)
            {
                mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                mainWindow.Top = alignmentValues.WindowTop;
                mainWindow.Left = alignmentValues.WindowLeft;
            }
        }

        private void ShowMainMenu()
        {
            BubbleMenuItem mnu = _viewModel.BubbleMenuHandler.MainMenu;
            Bubble mainMenu = new Bubble()
            { 
                Name = "MainMenu",
                Text = mnu.Text,
                ToolTipText = mnu.Tooltip,
                Height = _bubbleMainMenuSize,
                TextIconLayout = TextIconLayout.IconLeftOfText,
                HorizontalAlignment = HorizontalAlignment.Center,
                BorderDistance = 6,
                RenderStyle = BubbleRenderStyle.Style3D
                
            };
            if(mnu.IconPath != null) mainMenu.Icon = new BitmapImage(new Uri(mnu.IconPath));
            mainMenu.MouseLeftButtonDown += Bubble_Clicked;

            mainMenu.ApplyTheme(MenuStyleTheme);
            Canvas.SetTop(mainMenu, _menuCenter.Y - _bubbleMainMenuSize / 2);
            Canvas.SetLeft(mainMenu, _menuCenter.X - _bubbleMainMenuSize / 2);
            
            MenuCanvas.Children.Add(mainMenu);
        }
        private void BuildMenuRings()
        {
            Point ringCenter = alignmentValues.RingCenter;

            _pathMenuRing.ApplyTheme(MenuStyleTheme);
            _pathMenuRing.Center = ringCenter;
            _pathMenuRing.RadiusX = BubbleMainMenuSize + BubbleMenuBigSize + _bubbleMenuSpacing * 2;
            _pathMenuRing.RadiusY = BubbleMainMenuSize + BubbleMenuBigSize + _bubbleMenuSpacing * 2;
            _pathMenuRing.PathWidth = BubbleMenuSmallSize + 10;
            _pathMenuRing.ElementDistance = 5;

            Canvas.SetTop(_pathMenuRing, 0);
            Canvas.SetLeft(_pathMenuRing, 0);
            MenuCanvas.Children.Add(_pathMenuRing);

            _selectedMenuRing.ApplyTheme(MenuStyleTheme);
            _selectedMenuRing.Center = ringCenter;
            _selectedMenuRing.RadiusX = _pathMenuRing.RadiusX + BubbleMenuBigSize + _bubbleMenuSpacing;
            _selectedMenuRing.RadiusY = _pathMenuRing.RadiusY + BubbleMenuBigSize + _bubbleMenuSpacing;
            _selectedMenuRing.PathWidth = BubbleMenuBigSize + 10;
            _selectedMenuRing.ElementDistance = 10;

            Canvas.SetTop(_selectedMenuRing, 0);
            Canvas.SetLeft(_selectedMenuRing, 0);
            MenuCanvas.Children.Add(_selectedMenuRing);

            _additionalMenuRing.ApplyTheme(MenuStyleTheme);
            _additionalMenuRing.Center = ringCenter;
            _additionalMenuRing.RadiusX = _selectedMenuRing.RadiusX + BubbleMenuBigSize + _bubbleMenuSpacing;
            _additionalMenuRing.RadiusY = _selectedMenuRing.RadiusY + BubbleMenuBigSize + _bubbleMenuSpacing;
            _additionalMenuRing.PathWidth = BubbleMenuBigSize + 10;
            _additionalMenuRing.ElementDistance = 10;

            Canvas.SetTop(_additionalMenuRing, 0);
            Canvas.SetLeft(_additionalMenuRing, 0);
            MenuCanvas.Children.Add(_additionalMenuRing);

            //_pathMenuRing.Visibility = Visibility.Collapsed;
            //_selectedMenuRing.Visibility = Visibility.Collapsed;
            _additionalMenuRing.Visibility = Visibility.Collapsed;
        }
        public void UpdateMenu()
        {
            RemoveMenuElements();
            ShowPathMenu();
            ShowSelectionMenu();
            _viewModel.BubbleMenuHandler.ResetDeleted();
        }
        private void RemoveMenuElements()
        {
            _pathMenuRing.RemoveElements();
            _selectedMenuRing.RemoveElements();
            _additionalMenuRing.RemoveElements();
        }
        private void ShowPathMenu()
        {
            List<UIElement> pathElements = new List<UIElement>();
            foreach (var item in _viewModel.BubbleMenuHandler.MenuPath)
            { 
                Bubble bubble = new Bubble
                {
                    Name = item.Name,
                    Text = item.Text,
                    ToolTipText = item.Tooltip,
                    Height = _bubbleMenuSmallSize,
                    DataContext = item,
                    BorderDistance = 5,
                    RenderStyle = BubbleRenderStyle.Style3D
                };
                if (item.IconPath != null)
                    bubble.Icon = new BitmapImage(new Uri(item.IconPath));

                bubble.MouseLeftButtonDown += Bubble_Clicked;
                pathElements.Add(bubble);                
            }
            _pathMenuRing.AddElements(pathElements);
        }
        private void ShowSelectionMenu()
        {
            List<UIElement> elements = new List<UIElement>();
            // Neue Auswahl-Bubbles aus ViewModel
            foreach (var item in _viewModel.BubbleMenuHandler.SelectableMenus)
            {
                var bubble = new Bubble
                {
                    Name = item.Name,
                    Text = item.Text,
                    ToolTipText = item.Tooltip,
                    Height = _bubbleMenuBigSize,
                    DataContext = item,
                    BorderDistance = 5,
                    RenderStyle = BubbleRenderStyle.Style3D
                };

                if (item.IconPath != null)
                    bubble.Icon = new BitmapImage(new Uri(item.IconPath));

                bubble.MouseLeftButtonDown += Bubble_Clicked;
                elements.Add(bubble);
            }
            _selectedMenuRing.AddElements(elements);
        }

        private void ShowRings()
        {
            if (_ringsVisible)
                return;

            _ringsVisible = true;
            _pathMenuRing.Visibility = Visibility.Visible;
            _selectedMenuRing.Visibility = Visibility.Visible;
            //_additionalMenuRing.Visibility = Visibility.Visible;
        }
        private void StartHideTimer()
        {
            _hideTimer.Stop();
            _hideTimer = new Timer(MenuHideSeconds * 1000); // 5 Sekunden
            _hideTimer.Elapsed += (_, _) =>
            {
                _hideTimer.Stop();
                Dispatcher.Invoke(() =>
                {
                    _ringsVisible = false;
                    _pathMenuRing.Visibility = Visibility.Collapsed;
                    _selectedMenuRing.Visibility = Visibility.Collapsed;
                    _additionalMenuRing.Visibility = Visibility.Collapsed;
                });
            };
            _hideTimer.Start();
        }
        #endregion
    }
}
