using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using BubbleControlls.Models;
using BubbleControlls.ViewModels;

namespace BubbleControlls.ControlViews
{
    public partial class BubbleMenu
    {
        #region Variablen
        private bool _isLoaded;
        private readonly BubbleMenuViewModel _viewModel = new ();
        private readonly double _defaultMargin = 10;
        private double _menuWidth;
        private double _menuHeight;
        private Point _menuCenter = new (0, 0);
        private readonly BubbleRingControl _pathMenuRing = new ();
        private readonly BubbleRingControl _selectedMenuRing = new ();
        private readonly BubbleRingControl _additionalMenuRing = new ();
        private readonly DispatcherTimer  _hideTimer;
        private bool _ringsVisible;
        private BubbleAlignmentValues _alignmentValues = new ();
        #endregion

        #region Properties
        public double MenuHideSeconds { get; set; } = 3;
        public BubbleVisualTheme MenuStyleTheme { get; set; } = BubbleVisualThemes.Standard();
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
            if (d is BubbleMenu { ActualHeight: > 0 } b)
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
                new PropertyMetadata(80d));
        
        public double BubbleMenuBigSize
        {
            get => (double)GetValue(BubbleMenuBigSizeProperty);
            set => SetValue(BubbleMenuBigSizeProperty, value);
        }
        public static readonly DependencyProperty BubbleMenuBigSizeProperty =
            DependencyProperty.Register(nameof(BubbleMenuBigSize), typeof(double), typeof(BubbleMenu),
                new PropertyMetadata(60d));

        public double BubbleMenuSmallSize
        {
            get => (double)GetValue(BubbleMenuSmallSizeProperty);
            set => SetValue(BubbleMenuSmallSizeProperty, value);
        }
        public static readonly DependencyProperty BubbleMenuSmallSizeProperty =
            DependencyProperty.Register(nameof(BubbleMenuSmallSize), typeof(double), typeof(BubbleMenu),
                new PropertyMetadata(35d));

        public double BubbleMenuSpacing
        {
            get => (double)GetValue(BubbleMenuSpacingProperty);
            set => SetValue(BubbleMenuSpacingProperty, value);
        }
        public static readonly DependencyProperty BubbleMenuSpacingProperty =
            DependencyProperty.Register(nameof(BubbleMenuSpacing), typeof(double), typeof(BubbleMenu),
                new PropertyMetadata(10d));

        public BubbleMenuItem MainMenu
        {
            get => _viewModel.BubbleMenuHandler.MainMenu;
            set
            {
                _viewModel.BubbleMenuHandler.MainMenu = value;
                if (_isLoaded) BuildMenuVisual();
            }
        }
        #endregion
        public BubbleMenu()
        {
            _hideTimer = new DispatcherTimer();
            _hideTimer.Tick += (_, _) => OnHideTimer();
            _hideTimer.Stop();

            double[] menuSizes = [BubbleMainMenuSize, BubbleMenuSmallSize, BubbleMenuBigSize, BubbleMenuBigSize];
            InitializeComponent();
            this.Loaded += (_, _) => {
                if (this.DataContext == null)
                {
                    this.DataContext = new BubbleMenuViewModel();
                    _isLoaded = true;
                    _viewModel.SetMenuLevelSizes(menuSizes, BubbleMenuSpacing);
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

            // Ermittlung der benötigten Mindestlängen
            double minLength = _viewModel.GetMenuLevelLenght;
            double minHeight = minLength;

            // Control Grösse anpassen
            // Menü Masse mit zusätzlichen Spacing
            _menuWidth = minLength + _defaultMargin + BubbleMenuSpacing;
            _menuHeight = _menuWidth;
            
            _alignmentValues = _viewModel.UpdateAlignmentValues(BubbleMenuAlignment,
                minHeight,minLength,_menuHeight, _menuWidth, BubbleMainMenuSize);

            _menuCenter = _alignmentValues.MenuCenter;
            this.Width = _alignmentValues.MenuWidth;
            this.Height = _alignmentValues.MenuHeight;
            _pathMenuRing.Height = _alignmentValues.MenuHeight;
            _pathMenuRing.Width = _alignmentValues.MenuWidth;

            _pathMenuRing.StartAngle = _alignmentValues.StartAngle;
            _pathMenuRing.EndAngle = _alignmentValues.EndAngle;
            _selectedMenuRing.StartAngle = _alignmentValues.StartAngle;
            _selectedMenuRing.EndAngle = _alignmentValues.EndAngle;
            _additionalMenuRing.StartAngle = _alignmentValues.StartAngle;
            _additionalMenuRing.EndAngle = _alignmentValues.EndAngle;

            ShowMainMenu();
            BuildMenuRings();
        }
        /// <summary>
        /// Positioniert das MainWindow entsprechend des MenuAlignmentProperties
        /// </summary>
        private void SetMainWindowAlignment()
        {
            if (!_isLoaded) { return; }
            
            Window? mainWindow = Window.GetWindow(this);
            if (mainWindow != null)
            {
                mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                mainWindow.Top = _alignmentValues.WindowTop;
                mainWindow.Left = _alignmentValues.WindowLeft;
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
                Height = BubbleMainMenuSize,
                TextIconLayout = TextIconLayout.IconLeftOfText,
                HorizontalAlignment = HorizontalAlignment.Center,
                BorderDistance = 6,
                RenderStyle = BubbleRenderStyle.Style3D
                
            };
            if(mnu.IconPath != null) mainMenu.Icon = new BitmapImage(new Uri(mnu.IconPath));
            mainMenu.MouseLeftButtonDown += Bubble_Clicked;

            mainMenu.ApplyTheme(MenuStyleTheme);
            Canvas.SetTop(mainMenu, _menuCenter.Y - BubbleMainMenuSize / 2);
            Canvas.SetLeft(mainMenu, _menuCenter.X - BubbleMainMenuSize / 2);
            
            MenuCanvas.Children.Add(mainMenu);
        }
        private void BuildMenuRings()
        {
            Point ringCenter = _alignmentValues.RingCenter;

            _pathMenuRing.ApplyTheme(MenuStyleTheme);
            _pathMenuRing.Center = ringCenter;
            _pathMenuRing.RadiusX = BubbleMainMenuSize + BubbleMenuBigSize + BubbleMenuSpacing * 2;
            _pathMenuRing.RadiusY = BubbleMainMenuSize + BubbleMenuBigSize + BubbleMenuSpacing * 2;
            _pathMenuRing.PathWidth = BubbleMenuSmallSize + 10;
            _pathMenuRing.ElementDistance = 5;
            _pathMenuRing.IsCentered = _alignmentValues.IsCentered;
            _pathMenuRing.IsInverted = _alignmentValues.IsInverted;
            
            Canvas.SetTop(_pathMenuRing, 0);
            Canvas.SetLeft(_pathMenuRing, 0);
            MenuCanvas.Children.Add(_pathMenuRing);

            _selectedMenuRing.ApplyTheme(MenuStyleTheme);
            _selectedMenuRing.Center = ringCenter;
            _selectedMenuRing.RadiusX = _pathMenuRing.RadiusX + BubbleMenuBigSize + BubbleMenuSpacing;
            _selectedMenuRing.RadiusY = _pathMenuRing.RadiusY + BubbleMenuBigSize + BubbleMenuSpacing;
            _selectedMenuRing.PathWidth = BubbleMenuBigSize + 10;
            _selectedMenuRing.ElementDistance = 10;
            _selectedMenuRing.IsCentered = _alignmentValues.IsCentered;
            _selectedMenuRing.IsInverted = _alignmentValues.IsInverted;

            Canvas.SetTop(_selectedMenuRing, 0);
            Canvas.SetLeft(_selectedMenuRing, 0);
            MenuCanvas.Children.Add(_selectedMenuRing);

            _additionalMenuRing.ApplyTheme(MenuStyleTheme);
            _additionalMenuRing.Center = ringCenter;
            _additionalMenuRing.RadiusX = _selectedMenuRing.RadiusX + BubbleMenuBigSize + BubbleMenuSpacing;
            _additionalMenuRing.RadiusY = _selectedMenuRing.RadiusY + BubbleMenuBigSize + BubbleMenuSpacing;
            _additionalMenuRing.PathWidth = BubbleMenuBigSize + 10;
            _additionalMenuRing.ElementDistance = 10;
            _additionalMenuRing.IsCentered = _alignmentValues.IsCentered;
            _additionalMenuRing.IsInverted = _alignmentValues.IsInverted;

            Canvas.SetTop(_additionalMenuRing, 0);
            Canvas.SetLeft(_additionalMenuRing, 0);
            MenuCanvas.Children.Add(_additionalMenuRing);

            //_pathMenuRing.Visibility = Visibility.Collapsed;
            //_selectedMenuRing.Visibility = Visibility.Collapsed;
            _additionalMenuRing.Visibility = Visibility.Collapsed;
        }
        private void UpdateMenu()
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
                    Height = BubbleMenuSmallSize,
                    Width = BubbleMenuSmallSize,
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
                    Height = BubbleMenuBigSize,
                    Width = BubbleMenuBigSize,
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
            _hideTimer.Interval = TimeSpan.FromSeconds(MenuHideSeconds);
            _hideTimer.Start();
        }

        private void OnHideTimer()
        {
            _ringsVisible = false;
            _pathMenuRing.Visibility = Visibility.Collapsed;
            _selectedMenuRing.Visibility = Visibility.Collapsed;
            _additionalMenuRing.Visibility = Visibility.Collapsed;
            _hideTimer.Stop();
        }
        #endregion
    }
}
