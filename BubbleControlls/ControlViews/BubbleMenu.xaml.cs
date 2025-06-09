using BubbleControlls.Helpers;
using BubbleControlls.Models;
using BubbleControlls.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BubbleControlls.ControlViews
{
    public partial class BubbleMenu : UserControl
    {
        #region Variablen
        private bool _isLoaded = false;
        private BubbleMenuViewModel _viewModel = new BubbleMenuViewModel();
        private double _bubbleMenuBigSize = 70;
        private double _bubbleMenuSmallSize = 45;
        private double _bubbleMenuSpacing = 10;
        private double _defaultMargin = 10;
        private double _menuWidth = 0;
        private double _menuHeight = 0;
        #endregion

        #region Properties
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
                new PropertyMetadata(70d, OnBubbleMenuBigSizeChanged));
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
                new PropertyMetadata(45d, OnBubbleMenuSmallSizeChanged));
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

        private void BuildMenuVisual()
        {
            MenuCanvas.Children.Clear();
            // Ab hier: alle Bubbles aus _viewModel.BubbleMenuHandler.MainMenu rekursiv zeichnen
            int depth = _viewModel.BubbleMenuHandler.GetMaxDepth();
            (int itemLevel, int itemCount) itemsCount = _viewModel.BubbleMenuHandler.GetLevelWithMaxSubItemCount();

            // Ermittlung dder benötigten Mindestlängen
            double minLength = _viewModel.GetMenuLevelLenght;
            double minHeight = minLength;

            // Menü Masse mit zusätzlichen Spacing
            _menuWidth = minLength + _defaultMargin + _bubbleMenuSpacing;
            _menuHeight = _menuWidth;

            

            //MainMenu Center
            Point center = new Point(0, 0);
            switch (BubbleMenuAlignment)
            {
                case BubbleMenuAlignmentType.TopLeftCorner:
                    center = new Point(BubbleMenuBigSize / 2, BubbleMenuBigSize / 2);
                    _viewModel.MenuAreaFrom = new Point(minLength, center.Y);
                    _viewModel.MenuAreaTo = new Point(center.X, minHeight);
                    _viewModel.MenuAreaFromRadian = ViewHelper.DegToRad(0);
                    _viewModel.MenuAreaToRadian = ViewHelper.DegToRad(90);
                    break;
                case BubbleMenuAlignmentType.TopEdge:
                    center = new Point(minLength + BubbleMenuBigSize / 2, BubbleMenuBigSize / 2);
                    _viewModel.MenuAreaFrom = new Point(center.X + minLength, center.Y);
                    _viewModel.MenuAreaTo = new Point(center.X - minLength, center.Y);

                    _viewModel.MenuAreaFromRadian = ViewHelper.DegToRad(0);
                    _viewModel.MenuAreaToRadian = ViewHelper.DegToRad(180);
                    break;
                case BubbleMenuAlignmentType.LeftEdge:
                    center = new Point(BubbleMenuBigSize / 2, minHeight + BubbleMenuBigSize / 2);
                    _viewModel.MenuAreaFrom = new Point(center.X, center.Y - minHeight);
                    _viewModel.MenuAreaTo = new Point(center.X, minHeight + minHeight);
                    _viewModel.MenuAreaFromRadian = ViewHelper.DegToRad(270);
                    _viewModel.MenuAreaToRadian = ViewHelper.DegToRad(90);
                    break;
                default:
                    break;
            }
            // Control Grösse anpassen
            this.Width = center.X + minLength + _defaultMargin;
            this.Height = center.Y + minLength + _defaultMargin;

            //Distribution Axis
            _viewModel.SetDistributionRadian(BubbleMenuAlignment);

            //Ränder für Menu Bubble bereich
            MenuCanvas.Children.Add(ViewHelper.DrawLine(center, _viewModel.MenuAreaFrom, Brushes.Green, 2, false));
            MenuCanvas.Children.Add(ViewHelper.DrawLine(center, _viewModel.MenuAreaTo, Brushes.Orange, 2, false));

            // Zeichne Menu Bahnen
            for (int i = 0; i < _viewModel.MenuLevelSizes.Length; i++)
            {
                //var path = ViewHelper.DrawArc(center, _viewModel.MenuLevelSizes[i].Center, _viewModel.MenuLevelSizes[i].Center, Brushes.Blue, 1, false);
                var path = ViewHelper.DrawArc(center, _viewModel.MenuLevelSizes[i].Center, _viewModel.MenuAreaFromRadian, _viewModel.MenuAreaToRadian, Brushes.Blue, 1, false);
                path.StrokeDashArray = new DoubleCollection { 2, 2 };
                MenuCanvas.Children.Add(path);
                //path = ViewHelper.DrawArc(center, _viewModel.MenuLevelSizes[i].End, _viewModel.MenuLevelSizes[i].End, Brushes.Red, 1, false);
                path = ViewHelper.DrawArc(center, _viewModel.MenuLevelSizes[i].End, _viewModel.MenuAreaFromRadian, _viewModel.MenuAreaToRadian, Brushes.Red, 1, false);
                path.StrokeDashArray = new DoubleCollection { 6, 2 };
                MenuCanvas.Children.Add(path);
            }

            // Distribution punkte anzeigen
            for (int i = 0; i < _viewModel.MenuLevelSizes.Length; i++)
            {
                var marker = ViewHelper.CreateMarker(center, _viewModel.DistributionRadian, _viewModel.MenuLevelSizes[i].Center, 3, Brushes.Purple);
                MenuCanvas.Children.Add(marker);
            }
        }
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
    }
}
