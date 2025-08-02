using BubbleControlls.ControlViews;
using BubbleControlls.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

public class BubbleContextWindow : Window
{
    private readonly BubbleRingControl _ring = new();
    private Canvas _canvas = new Canvas();
    private List<BubbleMenuItem> _items = new();
    private int _maxMenuElements = 5;
    private double _menuItemSize = 50;
    private double _menuItemDistance = 5;
    private bool _isLeft = false;
    private double maxWidth = 0;
    private bool _isClosing = false;
    public bool AutoCloseOnClick { get; set; } = true;
    public BubbleMenuItem? SelectedItem { get; private set; }
    public BubbleVisualTheme Theme { get; set; } = BubbleVisualThemes.Standard();
    public BubbleRenderStyle RenderStyle { get; set; } = BubbleRenderStyle.StylePlane;
    public int MaxMenuElements { get => _maxMenuElements; set => _maxMenuElements = value; }
    public double MenuItemSize { get => _menuItemSize; set => _menuItemSize = value; }
    public double MenuItemDistance { get => _menuItemDistance; set => _menuItemDistance = value; }

    public BubbleContextWindow()
    {
        this.AllowsTransparency = true;
        this.WindowStyle = WindowStyle.None;
        this.Background = Brushes.Transparent;
        this.ShowInTaskbar = true;
        this.Topmost = true;
        this.SizeToContent = SizeToContent.WidthAndHeight;
        this.ResizeMode = ResizeMode.NoResize;

        _canvas.Width = 300;
        _canvas.Height = 300;

        Content = _canvas;

        this.Deactivated += (_, _) =>
        {
            if (!_isClosing)
                this.Close();
        };
    }

    public void ShowAt(Point screenPosition, List<BubbleMenuItem> items)
    {
        var screenSize = new Size(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);

        _items = items;
        _isLeft = false;
        _isClosing = false;

        BuildRing();

        Point pos = new Point(screenPosition.X, screenPosition.Y);
        pos.Y -= _canvas.Height/2;
        if (pos.Y < 0) pos.Y = 0;
        if (pos.X + _canvas.Width > screenSize.Width)
        {
            _isLeft = true;
            pos.X -= _canvas.Width;
        }

        this.Left = pos.X;
        this.Top = pos.Y;
        //this.Show();
        this.ShowDialog();
    }
    private void BuildRing()
    {
        _canvas.Children.Clear();

        _ring.ApplyTheme(Theme);
        List<UIElement> bubbles = BuildMenu();
        _ring.AddElements(bubbles);
        maxWidth += 30;
        _ring.Height = (_menuItemSize) * (_maxMenuElements+1);
        _ring.Width = maxWidth * 2;
        if (_isLeft)
        {
            _ring.StartAngle = 150;
            _ring.EndAngle = 210;
            _ring.IsInverted = true;
            _ring.Center = new Point(_ring.Width, _ring.Height / 2);
        }
        else
        {
            _ring.StartAngle = 330;
            _ring.EndAngle = 30;
            _ring.IsInverted = false;
            _ring.Center = new Point(0, _ring.Height / 2);
        }

        _ring.RadiusX = _ring.Width * 0.6;
        _ring.RadiusY = _ring.Height * 1.0;
        _ring.PathWidth = maxWidth;

        _canvas.Children.Add(_ring);
        _canvas.Height = _ring.Height + 0.9;
        _canvas.Width = _ring.Width * 0.8;
    }
    private List<UIElement> BuildMenu()
    {
        List<UIElement> bubbles = new();
        maxWidth = 0;
        foreach (var item in _items)
        {
            var bubble = new Bubble
            {
                Text = item.Text,
                ToolTipText = item.Tooltip,
                Height = _menuItemSize,
                DataContext = item,
                BorderDistance = 5,
                RenderStyle = RenderStyle,
                TextIconLayout = TextIconLayout.IconLeftOfText
            };

            if (item.IconPath != null)
                bubble.Icon = new BitmapImage(new Uri(item.IconPath));

            double width = MeasureBubbleWidth(bubble);
            if (width > maxWidth)
                maxWidth = width;
            bubble.MouseLeftButtonDown += BubbleMenuItemClick;
            //bubble.MouseLeftButtonDown += (_, _) =>
            //{
            //    item.OnClick?.Invoke(item);
            //    if (AutoCloseOnClick && !_isClosing)
            //    {
            //        _isClosing = true;
            //        this.Topmost = false;
            //        Dispatcher.BeginInvoke(() => this.Close(), DispatcherPriority.Background);
            //    }
            //};

            bubbles.Add(bubble);
        }

        _ring.RemoveElements();
        return bubbles;

    }

    private void BubbleMenuItemClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is Bubble bubble && bubble.DataContext is BubbleMenuItem item)
        {
            SelectedItem = item;
            item.OnClick?.Invoke(item);

            if (AutoCloseOnClick && !_isClosing)
            {
                _isClosing = true;
                this.Topmost = false;
                Dispatcher.BeginInvoke(() => this.Close(), DispatcherPriority.Background);
            }
        }
    }

    private double MeasureBubbleWidth(Bubble bubble)
    {
        var panel = new Canvas(); // Dummy-VisualTree
        panel.Children.Add(bubble);

        panel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        panel.Arrange(new Rect(0, 0, panel.DesiredSize.Width, panel.DesiredSize.Height));

        panel.Children.Remove(bubble);
        return bubble.DesiredSize.Width;
    }
}
