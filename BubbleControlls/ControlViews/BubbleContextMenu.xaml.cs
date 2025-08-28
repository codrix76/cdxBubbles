using BubbleControlls.Helpers;
using BubbleControlls.Models;
using BubbleControlls.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BubbleControlls.ControlViews
{
    /// <summary>
    /// Interaktionslogik für BubbleContextMenu.xaml
    /// </summary>
    public partial class BubbleContextMenu : UserControl
    {
        #region Variablen
        //private bool _isLoaded;
        //private readonly BubbleMenuViewModel _viewModel = new();
        //private readonly double _defaultMargin = 10;
        //private double _menuWidth;
        //private double _menuHeight;
        //private Point _menuCenter = new(0, 0);
        private readonly BubbleRingControl _menuRing = new();
        private List<BubbleMenuItem> _menuItems = new List<BubbleMenuItem>();

        #endregion

        public BubbleContextMenu()
        {
            InitializeComponent();
        }
        #region Properties
        public BubbleVisualTheme MenuStyleTheme { get; set; } = BubbleVisualThemes.Standard();
        public List<BubbleMenuItem> MenuItems { get => _menuItems; set => _menuItems = value; }
        public bool AutoCloseOnClick { get; set; } = true;

        #endregion
        #region Dependecny Properties
        public BubbleRenderStyle BubbleMenuStyle
        {
            get => (BubbleRenderStyle)GetValue(BubbleRenderStyleProperty);
            set => SetValue(BubbleRenderStyleProperty, value);
        }
        public static readonly DependencyProperty BubbleRenderStyleProperty =
            DependencyProperty.Register(nameof(BubbleMenuStyle), typeof(BubbleRenderStyle), typeof(BubbleContextMenu),
                new PropertyMetadata(BubbleRenderStyle.Style3D, null));

        public BubbleMenuAlignmentType BubbleMenuAlignment
        {
            get => (BubbleMenuAlignmentType)GetValue(BubbleMenuAlignmentProperty);
            set => SetValue(BubbleMenuAlignmentProperty, value);
        }
        public static readonly DependencyProperty BubbleMenuAlignmentProperty =
            DependencyProperty.Register(nameof(BubbleMenuAlignment), typeof(BubbleMenuAlignmentType), typeof(BubbleContextMenu),
                new PropertyMetadata(BubbleMenuAlignmentType.TopLeftCorner, OnBubbleMenuAlignmentChanged));
        private static void OnBubbleMenuAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BubbleContextMenu { ActualHeight: > 0 } b)
            {
                //b.SetMainWindowAlignment();
            }
        }

        public double BubbleMainMenuSize
        {
            get => (double)GetValue(BubbleMainMenuSizeProperty);
            set => SetValue(BubbleMainMenuSizeProperty, value);
        }
        public static readonly DependencyProperty BubbleMainMenuSizeProperty =
            DependencyProperty.Register(nameof(BubbleMainMenuSize), typeof(double), typeof(BubbleContextMenu),
                new PropertyMetadata(80d));

        public double BubbleMenuBigSize
        {
            get => (double)GetValue(BubbleMenuBigSizeProperty);
            set => SetValue(BubbleMenuBigSizeProperty, value);
        }
        public static readonly DependencyProperty BubbleMenuBigSizeProperty =
            DependencyProperty.Register(nameof(BubbleMenuBigSize), typeof(double), typeof(BubbleContextMenu),
                new PropertyMetadata(60d));

        public double BubbleMenuSmallSize
        {
            get => (double)GetValue(BubbleMenuSmallSizeProperty);
            set => SetValue(BubbleMenuSmallSizeProperty, value);
        }
        public static readonly DependencyProperty BubbleMenuSmallSizeProperty =
            DependencyProperty.Register(nameof(BubbleMenuSmallSize), typeof(double), typeof(BubbleContextMenu),
                new PropertyMetadata(35d));

        public double BubbleMenuSpacing
        {
            get => (double)GetValue(BubbleMenuSpacingProperty);
            set => SetValue(BubbleMenuSpacingProperty, value);
        }
        
        public static readonly DependencyProperty BubbleMenuSpacingProperty =
            DependencyProperty.Register(nameof(BubbleMenuSpacing), typeof(double), typeof(BubbleContextMenu),
                new PropertyMetadata(10d));

        #endregion

        public void ShowAt(Point mousePosition, List<BubbleMenuItem>? items = null)
        {
            if (items == null || items.Count == 0)
                return;

            MenuItems = items;

            // Bildschirmgröße
            var screenSize = new Size(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);

            // Menügröße schätzen (kann später verfeinert werden)
            var menuSize = new Size(300, 300);

            // Platzierung berechnen
            var placement = BubbleContextPlacer.GetBestPlacement(mousePosition, menuSize, screenSize);

            // Winkel übernehmen
            _menuRing.StartAngle = placement.StartAngle;
            _menuRing.EndAngle = placement.EndAngle;

            // Setup Ring
            _menuRing.Center = new Point(menuSize.Width / 2, menuSize.Height / 2);
            _menuRing.RadiusX = BubbleMenuBigSize + BubbleMenuSpacing * 2;
            _menuRing.RadiusY = BubbleMenuBigSize + BubbleMenuSpacing * 2;
            _menuRing.PathWidth = BubbleMenuBigSize + 10;
            _menuRing.ElementDistance = 10;
            _menuRing.IsCentered = true;
            _menuRing.IsInverted = false;
            _menuRing.ApplyTheme(MenuStyleTheme);

            // Bubbles bauen
            List<UIElement> bubbles = new();
            foreach (var item in MenuItems)
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
                    RenderStyle = BubbleMenuStyle
                };
                if (item.IconPath != null)
                    bubble.Icon = new BitmapImage(new Uri(item.IconPath));

                bubble.ApplyTheme(MenuStyleTheme);
                bubble.MouseLeftButtonDown += (_, _) =>
                {
                    item.OnClick?.Invoke(item);
                    if (AutoCloseOnClick)
                        this.Visibility = Visibility.Collapsed;
                };
                bubbles.Add(bubble);
            }

            _menuRing.RemoveElements();
            _menuRing.AddElements(bubbles);

            // Positionieren
            Canvas.SetLeft(this, placement.MenuTopLeft.X);
            Canvas.SetTop(this, placement.MenuTopLeft.Y);
            this.Visibility = Visibility.Visible;
        }

        public void ApplyTheme(BubbleVisualTheme theme)
        {
            MenuStyleTheme = theme;

            // Ring-Thema direkt anwenden
            _menuRing.ApplyTheme(theme);

            // Optional: bereits platzierte Bubbles ebenfalls neu stylen
            foreach (var child in _menuRing.Children.OfType<Bubble>())
            {
                child.ApplyTheme(theme);
            }
        }
    }
}
