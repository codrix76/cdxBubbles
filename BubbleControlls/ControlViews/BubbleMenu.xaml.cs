using BubbleControlls.Helpers;
using BubbleControlls.Models;
using BubbleControlls.ViewModels;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace BubbleControlls.ControlViews
{
    public partial class BubbleMenu : UserControl
    {
        #region Variablen
        private bool _isLoaded = false;
        private BubbleMenuViewModel _viewModel = new BubbleMenuViewModel();
        private double _bubbleMenuBigSize = 60;
        private double _bubbleMenuSmallSize = 35;
        private double _bubbleMenuSpacing = 10;
        private double _defaultMargin = 10;
        private double _menuWidth = 0;
        private double _menuHeight = 0;
        private Point _menuCenter = new Point(0, 0);
        #endregion

        #region Properties
        public double TopCornerHeightFactor { get; set; } = 1; 
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
                BubbleMenuAlignmentType align = (BubbleMenuAlignmentType)e.NewValue;
                b.SetMainWindowAlignment();
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
            InitializeComponent();
            this.Loaded += (s, e) => {
                if (this.DataContext == null)
                {
                    this.DataContext = new BubbleMenuViewModel();
                    _isLoaded = true;
                    _viewModel.SetMenuLevelSizes(BubbleMenuBigSize, BubbleMenuSmallSize, _bubbleMenuSpacing);
                    BuildMenuVisual();
                    SetMainWindowAlignment();
                }
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
            // Ab hier: alle Bubbles aus _viewModel.BubbleMenuHandler.MainMenu rekursiv zeichnen
            int depth = _viewModel.BubbleMenuHandler.GetMaxDepth();
            (int itemLevel, int itemCount) itemsCount = _viewModel.BubbleMenuHandler.GetLevelWithMaxSubItemCount();

            // Ermittlung dder benötigten Mindestlängen
            double minLength = _viewModel.GetMenuLevelLenght;
            double minHeight = minLength;

            //MainMenu Center
            SetMenuAreaValues(minLength, minHeight);

            // Control Grösse anpassen
            // Menü Masse mit zusätzlichen Spacing
            _menuWidth = minLength + _defaultMargin + _bubbleMenuSpacing;
            _menuHeight = _menuWidth;


            if (BubbleMenuAlignment == BubbleMenuAlignmentType.TopLeftCorner)
                _menuHeight = _menuHeight * TopCornerHeightFactor;
            if (BubbleMenuAlignment == BubbleMenuAlignmentType.TopEdge)
                _menuWidth = _menuWidth * 2;
            if (BubbleMenuAlignment == BubbleMenuAlignmentType.LeftEdge)
                _menuHeight = _menuHeight * 2;

            this.Width = _menuWidth;
            this.Height = _menuHeight;
            //Distribution Axis
            _viewModel.SetDistributionRadian(BubbleMenuAlignment);

            //Ränder für Menu Bubble bereich
            MenuCanvas.Children.Add(ViewHelper.DrawLine(_menuCenter, _viewModel.MenuAreaFrom, Brushes.LightGray, 2, false));
            MenuCanvas.Children.Add(ViewHelper.DrawLine(_menuCenter, _viewModel.MenuAreaTo, Brushes.LightGray, 2, false));

            // Zeichne Menu Bahnen
            for (int i = 0; i < _viewModel.MenuLevelSizes.Length; i++)
            {
                var pathCenter = ViewHelper.DrawArc(
                    _menuCenter, 
                    _viewModel.MenuLevelSizes[i].Center - Math.Min(_menuCenter.X, _menuCenter.Y), 
                    _viewModel.MenuAreaFromRadian
                    , _viewModel.MenuAreaToRadian, 
                    Brushes.WhiteSmoke, 1, false);
                pathCenter.StrokeDashArray = new DoubleCollection { 2, 2 };
                MenuCanvas.Children.Add(pathCenter);

                var borderpath = ViewHelper.DrawArc(
                    _menuCenter, 
                    _viewModel.MenuLevelSizes[i].End - Math.Min(_menuCenter.X, _menuCenter.Y), 
                    _viewModel.MenuAreaFromRadian
                    , _viewModel.MenuAreaToRadian, 
                    Brushes.LightGray, 1, false);
                borderpath.StrokeDashArray = new DoubleCollection { 6, 2 };
                MenuCanvas.Children.Add(borderpath);
            }

            // Distribution punkte anzeigen
            for (int i = 0; i < _viewModel.MenuLevelSizes.Length; i++)
            {
                var marker = ViewHelper.CreateMarker(
                    _menuCenter, 
                    _viewModel.DistributionRadian, 
                    _viewModel.MenuLevelSizes[i].Center - Math.Min(_menuCenter.X, _menuCenter.Y), 
                    3, Brushes.Red);
                MenuCanvas.Children.Add(marker);
            }
            ShowMainMenu();
        }
        /// <summary>
        /// Positioniert das MainWindow entsprechend des MenuAlignmentProperties
        /// </summary>
        public void SetMainWindowAlignment()
        {
            if (!_isLoaded) { return; }
            
            BubbleMenuAlignmentType align = BubbleMenuAlignment;
            Window? mainWindow = Window.GetWindow(this);
            if (mainWindow != null)
            {
                mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                double screenWidth = SystemParameters.PrimaryScreenWidth;
                double screenHeight = SystemParameters.PrimaryScreenHeight;
                if (align == BubbleMenuAlignmentType.TopLeftCorner)
                {
                    mainWindow.Top = 0;
                    mainWindow.Left = 0;
                }
                else if (align == BubbleMenuAlignmentType.LeftEdge)
                {
                    mainWindow.Top = (screenHeight - _menuHeight) / 2;
                    mainWindow.Left = 0;
                }
                else if (align == BubbleMenuAlignmentType.TopEdge)
                {
                    mainWindow.Top = 0;
                    mainWindow.Left = (screenWidth - _menuWidth) / 2;
                }

            }
        }

        private void SetMenuAreaValues(double length, double height)
        {
            switch (BubbleMenuAlignment)
            {
                case BubbleMenuAlignmentType.TopLeftCorner:
                    _menuCenter = new Point(BubbleMenuBigSize / 2, BubbleMenuBigSize / 2);
                    _viewModel.MenuAreaFrom = new Point(length, _menuCenter.Y);
                    _viewModel.MenuAreaTo = new Point(_menuCenter.X, height);
                    _viewModel.MenuAreaFromRadian = ViewHelper.DegToRad(0);
                    _viewModel.MenuAreaToRadian = ViewHelper.DegToRad(90);
                    break;
                case BubbleMenuAlignmentType.TopEdge:
                    _menuCenter = new Point(length + BubbleMenuBigSize / 2, BubbleMenuBigSize / 2);
                    _viewModel.MenuAreaFrom = new Point(_menuCenter.X + length, _menuCenter.Y);
                    _viewModel.MenuAreaTo = new Point(_menuCenter.X - length, _menuCenter.Y);

                    _viewModel.MenuAreaFromRadian = ViewHelper.DegToRad(0);
                    _viewModel.MenuAreaToRadian = ViewHelper.DegToRad(180);
                    break;
                case BubbleMenuAlignmentType.LeftEdge:
                    _menuCenter = new Point(BubbleMenuBigSize / 2, height + BubbleMenuBigSize / 2);
                    _viewModel.MenuAreaFrom = new Point(_menuCenter.X, _menuCenter.Y - height);
                    _viewModel.MenuAreaTo = new Point(_menuCenter.X, height + height);
                    _viewModel.MenuAreaFromRadian = ViewHelper.DegToRad(270);
                    _viewModel.MenuAreaToRadian = ViewHelper.DegToRad(90);
                    break;
                default:
                    break;
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
                Height = _bubbleMenuBigSize,
                TextIconLayout = TextIconLayout.IconLeftOfText,
                HorizontalAlignment = HorizontalAlignment.Center,
                BorderDistance = 3,
                RenderStyle = BubbleRenderStyle.Style3D
            };
            if(mnu.IconPath != null) mainMenu.Icon = new BitmapImage(new Uri(mnu.IconPath));
            mainMenu.MouseLeftButtonDown += Bubble_Clicked;
            Canvas.SetTop(mainMenu, _menuCenter.Y - _bubbleMenuBigSize / 2);
            Canvas.SetLeft(mainMenu, _menuCenter.X - _bubbleMenuBigSize / 2);
            MenuCanvas.Children.Add(mainMenu);
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
            // Bestehende Auswahl-Bubbles entfernen 
            var toRemoveItems = _viewModel.BubbleMenuHandler.GuiItemsToRemove;
            // 2. Liste der Namen dieser Elemente erstellen
            var namesToRemove = toRemoveItems
                .Select(i => i.Name)
                .ToHashSet();

            // 3. Alle passenden Bubbles auf dem Canvas finden
            var toRemove = MenuCanvas.Children
            .OfType<Bubble>()
            .Where(b => b.DataContext is BubbleMenuItem bmi && namesToRemove.Contains(bmi.Name))
            .ToList();

            for (int i = MenuCanvas.Children.Count - 1; i >= 0; i--)
            {
                if (MenuCanvas.Children[i] is Bubble b && toRemove.Contains(b))
                {
                    MenuCanvas.Children.RemoveAt(i);
                }
            }

        }
        private void ShowPathMenu()
        {
            int count = _viewModel.BubbleMenuHandler.MenuPath.Count;
            for (int i = 0; i < _viewModel.BubbleMenuHandler.MenuPath.Count; i++)
            {
                var item = _viewModel.BubbleMenuHandler.MenuPath[i];
                UIElement? foundElement = FindElementByName(MenuCanvas, item.Name);
                Bubble? bubble = null;
                if (foundElement != null) { bubble = (Bubble)foundElement; }
                else
                {
                    bubble = new Bubble
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
                    MenuCanvas.Children.Add(bubble);
                }
                // Position berechnen
                double radius = _viewModel.MenuLevelSizes[(int)BubbleMenuLevel.Path].Center - Math.Min(_menuCenter.X, _menuCenter.Y);
                double angle = ViewHelper.GetLevelAngle(radius, _bubbleMenuSmallSize + _bubbleMenuSpacing);

                Point pos = ViewHelper.CalculateBubblePosition(
                    _menuCenter,
                    radius,
                    _viewModel.DistributionRadian,
                    angle,
                    count,
                    i,
                    DistributionAlignment
                    );
                Canvas.SetLeft(bubble, pos.X - bubble.Height / 2);
                Canvas.SetTop(bubble, pos.Y - bubble.Height / 2);
            }
        }
        private void ShowSelectionMenu()
        {
            // Neue Auswahl-Bubbles aus ViewModel
            int count = _viewModel.BubbleMenuHandler.SelectableMenus.Count;
            int i = 0;
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

                // Position berechnen
                double radius = _viewModel.MenuLevelSizes[(int)BubbleMenuLevel.Selection].Center - Math.Min(_menuCenter.X, _menuCenter.Y);
                double angle = ViewHelper.GetLevelAngle(radius, _bubbleMenuBigSize + _bubbleMenuSpacing);
                
                Point pos = ViewHelper.CalculateBubblePosition(
                    _menuCenter,
                    radius,
                    _viewModel.DistributionRadian,
                    angle,
                    count,
                    i,
                    DistributionAlignment
                    );
                Canvas.SetLeft(bubble, pos.X - bubble.Height / 2);
                Canvas.SetTop(bubble, pos.Y - bubble.Height / 2);

                MenuCanvas.Children.Add(bubble);
                i++;
            }
        }
        public static UIElement? FindElementByName(Canvas canvas, string name)
        {
            foreach (UIElement child in canvas.Children)
            {
                if (child is FrameworkElement fe && fe.Name == name)
                    return child;
            }
            return null;
        }
        #endregion
    }
}
