using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using BubbleControlls.ControlViews;
using BubbleControlls.Models;

namespace BubblesDemo
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isDragging;
        private Point _dragOffset;
        
        public MainWindow()
        {
            InitializeComponent();
            
            this.Loaded += (s, e) =>
            {
                BubbleMenu.BubbleMenuAlignment = BubbleMenuAlignmentType.Free;
                //BubbleMenu.Background = Brushes.DarkSlateBlue;
                BubbleMenu.BubbleMainMenuSize = 80.0;
                BubbleMenu.BubbleMenuBigSize = 55.0;
                BubbleMenu.BubbleMenuSmallSize = 40.0;
                BubbleMenu.MainMenu = BuildMenu();
                BubbleMenu.MenuStyleTheme = BubbleVisualThemes.HudBlue();
            };
            BubbleMenu.MouseLeftButtonDown += OnMainMenuMouseDown;
            BubbleMenu.MouseMove += OnMainMenuMouseMove;
            BubbleMenu.MouseLeftButtonUp  += OnMainMenuMouseUp;
        }

        private void OnMainMenuMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && BubbleMenu.BubbleMenuAlignment == BubbleMenuAlignmentType.Free)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    _isDragging = true;
                    var mousePos = e.GetPosition(null);
                    var window = Window.GetWindow(this);
                    _dragOffset = new Point(mousePos.X, mousePos.Y);
                    Mouse.Capture((IInputElement)sender);
                }
            }
        }
        private void OnMainMenuMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                var currentPos = e.GetPosition(null);
                var diff = Point.Subtract(currentPos, (Vector)_dragOffset);
                var window = Window.GetWindow(this);
                window.Left += diff.X;
                window.Top += diff.Y;
            }
        }

        private void OnMainMenuMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            Mouse.Capture(null);

            if(BubbleMenu.BubbleMenuAlignment == BubbleMenuAlignmentType.Free) AdjustVisibleAngleRange(); 
        }
        private void AdjustVisibleAngleRange()
        {
            var ringRadius = BubbleMenu.MenuRadius; // Annahme: Kreisförmig
            var screenCenter = new Point(SystemParameters.PrimaryScreenWidth / 2, SystemParameters.PrimaryScreenHeight / 2);
            var ringScreenPos = this.PointToScreen(BubbleMenu.ViewValues.RingCenter);

            (double StartAngle, double EndAngle) angleVisible = CalculateVisibleAngleRange(ringScreenPos, ringRadius,ringRadius); // eigene Methode
            if (angleVisible.StartAngle == angleVisible.EndAngle)
            {
                angleVisible.StartAngle = 270;
                angleVisible.EndAngle = 269.9;
            }
            
            BubbleMenu.ViewValues.StartAngle = angleVisible.StartAngle;
            BubbleMenu.ViewValues.EndAngle = angleVisible.EndAngle;
            Debug.WriteLine($"StartAngle: {angleVisible.StartAngle}, EndAngle: {angleVisible.EndAngle} ");
            BubbleMenu.InvalidateMenu(angleVisible.StartAngle, angleVisible.EndAngle);
            //BubbleMenu.InvalidateVisual();
        }
        private (double StartAngle, double EndAngle) CalculateVisibleAngleRange(Point centerScreen, double radiusX, double radiusY)
        {
            Rect screenBounds = new Rect(0, 0, SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);

            List<(int start, int end)> visibleSegments = new();
            bool inVisibleSegment = false;
            int segmentStart = 0;

            for (int deg = 0; deg <= 360; deg++)
            {
                double rad = deg * Math.PI / 180.0;
                double x = centerScreen.X + radiusX * Math.Cos(rad);
                double y = centerScreen.Y + radiusY * Math.Sin(rad);
                Point p = new(x, y);

                if (screenBounds.Contains(p))
                {
                    if (!inVisibleSegment)
                    {
                        segmentStart = deg;
                        inVisibleSegment = true;
                    }
                }
                else
                {
                    if (inVisibleSegment)
                    {
                        visibleSegments.Add((segmentStart, deg - 1));
                        inVisibleSegment = false;
                    }
                }
            }

            // Offener Bereich bis 360?
            if (inVisibleSegment)
            {
                if (visibleSegments.Count > 0 && visibleSegments[0].start == 0)
                {
                    // zusammenhängender Bereich um 0° herum
                    var first = visibleSegments[0];
                    visibleSegments[0] = (segmentStart, first.end + 360);
                }
                else
                {
                    visibleSegments.Add((segmentStart, 360));
                }
            }

            if (visibleSegments.Count == 0)
                return (0, 0); // Nichts sichtbar

            // Wähle längstes Intervall
            var longest = visibleSegments
                .OrderByDescending(seg => seg.end - seg.start)
                .First();

            return (longest.start % 360, longest.end % 360);
        }
        private BubbleMenuItem BuildMenu()
        {
            BubbleMenuItem mainMenu = new BubbleMenuItem()
            {
                Name = "MainMenu",
                Tooltip = "Hauptmenu",
                Text = "",
                IconPath = "pack://application:,,,/Assets/Destiny.png",
                OnClick = (item) => {
                    MessageBox.Show($"Aktion ausgelöst von: {item.Text}");
                }
            };
            BubbleMenuItem subItem = new BubbleMenuItem()
            {
                Name = "DataMenu",
                Tooltip = "Daten laden/konfigurieren",
                Text = "",
                IconPath = "pack://application:,,,/Assets/Data.png",
                OnClick = (item) => {
                    MessageBox.Show($"Aktion ausgelöst von: {item.Text}");
                }
            };
            BubbleMenuItem subItem1 = new BubbleMenuItem()
            {
                Name = "DataMenu01",
                Tooltip = "Data 01",
                Text = "",
                IconPath = "pack://application:,,,/Assets/chart01.png",
                OnClick = (item) => {
                    MessageBox.Show($"Aktion ausgelöst von: {item.Text}");
                }
            };
            subItem.AddSubItem(subItem1);
            mainMenu.AddSubItem(subItem);
            subItem = new BubbleMenuItem()
            {
                Name = "AnalyseMenu",
                Tooltip = "Analyse",
                Text = "",
                IconPath = "pack://application:,,,/Assets/analyse.ico",
                OnClick = (item) => {
                    MessageBox.Show($"Aktion ausgelöst von: {item.Text}");
                }
            };
            subItem1 = new BubbleMenuItem()
            {
                Name = "AnalyseMenu02",
                Tooltip = "Analyse 02",
                Text = "",
                IconPath = "pack://application:,,,/Assets/analyse02.png",
                OnClick = (item) => {
                    MessageBox.Show($"Aktion ausgelöst von: {item.Text}");
                }
            };
            subItem.AddSubItem(subItem1);
            subItem1 = new BubbleMenuItem()
            {
                Name = "AnalyseMenu03",
                Tooltip = "Analyse 03",
                Text = "",
                IconPath = "pack://application:,,,/Assets/curve01.png",
                OnClick = (item) => {
                    MessageBox.Show($"Aktion ausgelöst von: {item.Text}");
                }
            };
            BubbleMenuItem subItem2 = new BubbleMenuItem()
            {
                Name = "AnalyseMenu031",
                Tooltip = "Analyse 03.01",
                Text = "",
                IconPath = "pack://application:,,,/Assets/curve02.png",
                OnClick = (item) => {
                    MessageBox.Show($"Aktion ausgelöst von: {item.Text}");
                }
            };
            subItem.AddContextItem(subItem2);
            subItem.AddContextItem(subItem1);
            mainMenu.AddSubItem(subItem);
            subItem = new BubbleMenuItem()
            {
                Name = "CloseProjectMenu",
                Tooltip = "Projekt schliessen",
                Text = "",
                IconPath = "pack://application:,,,/Assets/back_arrow.png",
                OnClick = (item) => {
                    MessageBox.Show($"Aktion ausgelöst von: {item.Text}");
                }
            };
            mainMenu.AddSubItem(subItem);
            subItem = new BubbleMenuItem()
            {
                Name = "SettingsMenu",
                Tooltip = "Einstellungen",
                Text = "",
                IconPath = "pack://application:,,,/Assets/Settings01.png",
                OnClick = (item) => {
                    MessageBox.Show($"Aktion ausgelöst von: {item.Text}");
                }
            };
            mainMenu.AddContextItem(subItem);
            mainMenu.AddContextItem(subItem);
            mainMenu.AddContextItem(subItem);
            subItem = new BubbleMenuItem()
            {
                Name = "CloseMenu",
                Tooltip = "Beenden",
                Text = "",
                IconPath = "pack://application:,,,/Assets/exit.png",
                OnClick = (item) => {
                    MessageBox.Show($"Aktion ausgelöst von: {item.Text}");
                }
            };
            mainMenu.AddSubItem(subItem);
            return mainMenu;
        }
    }
}
