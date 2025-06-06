using BubbleControlls.ControlViews;
using BubbleControlls.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using IOPath = System.IO.Path;

namespace BubblesDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        private readonly List<string> _recentProjects = new()
        {
            @"C:\Projekte\Alpha.dstprj",
            @"C:\Projekte\Beta.dstprj",
            @"C:\DochLustig\CetaProjekt.dstprj"
        };

        private double _bubbleHeight = 50;          //SubBubble Höhe
        private double _bubbleDistance = 5;        // Abstand zwischen den SubBubbles
        private double _subBubbleMaxAngle = 150;    // Obergrenze für SubBubble Bereich
        private double _subBubbleRadius = 260;      // Abstand der SubBubbles zu MainBubble
        private Point _mainBubbleCenter = new Point(0, 0);  // Zentrum des MainBubble, wird gesetzt sobald es die werte zur verfügung stehen
        private bool _showLines = false;
        private BubbleVisualTheme _bubbleVisualTheme = BubbleVisualThemes.Standard();
        //private BubbleVisualTheme _bubbleVisualTheme = BubbleVisualThemes.HudBlue();
        //private BubbleVisualTheme _bubbleVisualTheme = BubbleVisualThemes.EclipseCore();
        //private BubbleVisualTheme _bubbleVisualTheme = BubbleVisualThemes.Dark();
        //private BubbleVisualTheme _bubbleVisualTheme = BubbleVisualThemes.NeonEdge();
        public StartWindow()
        {
            Console.WriteLine("Demo Bubble");
            InitializeComponent();

            MainBubble.ApplyTheme(_bubbleVisualTheme);
            MainBubble.RenderStyle = BubbleRenderStyle.Style3D;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _mainBubbleCenter = new Point(150, RootCanvas.ActualHeight / 2); // MainBubble-Mitte
            CreateStartupBubbles(); // falls vorhanden

            // Position der MainBubble
            double x = 100;
            double y = 250;
            double bubbleSize = 250;
            double glowSize = 230;

            Canvas.SetLeft(MainBubble, x);
            Canvas.SetTop(MainBubble, y);

            Canvas.SetLeft(MainGlowPulse, x - (glowSize - bubbleSize) / 2);
            Canvas.SetTop(MainGlowPulse, y - (glowSize - bubbleSize) / 2);
            Canvas.SetZIndex(MainGlowPulse, 0);
            Canvas.SetZIndex(MainBubble, 1);
            
            MainGlowPulse.Width = glowSize;
            MainGlowPulse.Height = glowSize;

            ApplyPulseEffect();

        }
        private void ApplyPulseEffect()
        {
            if (MainGlowPulse.Effect is not DropShadowEffect effect)
                return;

            var opacityAnim = new DoubleAnimation
            {
                From = 0.3,
                To = 1,
                Duration = TimeSpan.FromSeconds(1.5),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            effect.BeginAnimation(DropShadowEffect.OpacityProperty, opacityAnim);

            var blurAnim = new DoubleAnimation
            {
                From = 80,
                To = 100,
                Duration = TimeSpan.FromSeconds(1.5),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            effect.BeginAnimation(DropShadowEffect.BlurRadiusProperty, blurAnim);
        }

        private void CreateStartupBubbles()
        {
            int totalCount = _recentProjects.Count + 3;
            List<Point> positions = CalculateBubblePositions(totalCount);

            // Fixe SubBubbles
            CreateSubBubble("Neues Projekt", "ein neues Projekt erstellen", positions[0], OnAnyBubbleClicked);
            CreateSubBubble("Projekt öffnen", "ein vorhandenes Projekt laden", positions[1], OnAnyBubbleClicked);
            for (int i = 0; i < _recentProjects.Count; i++)
                CreateSubBubble(ShortenPath(_recentProjects[i]), _recentProjects[i], positions[i + 2], OnAnyBubbleClicked);
            CreateSubBubble("Beenden", "Anwendung beenden", positions[totalCount -1], OnExitClicked);
        }
        private List<Point> CalculateBubblePositions(int totalCount)
        {
            var positions = new List<Point>();
            if (totalCount <= 0)
                return positions;

            double arcLength = totalCount * _bubbleHeight + (totalCount - 1) * _bubbleDistance;
            double maxAngle = Math.Min(arcLength / _subBubbleRadius, _subBubbleMaxAngle * Math.PI / 180.0);

            double angleStep = (totalCount == 1) ? 0 : maxAngle / (totalCount - 1);

            for (int i = 0; i < totalCount; i++)
            {
                double angle = -maxAngle / 2 + i * angleStep;
                double x = _mainBubbleCenter.X + _subBubbleRadius * Math.Cos(angle);
                double y = _mainBubbleCenter.Y + _subBubbleRadius * Math.Sin(angle);
                positions.Add(new Point(x, y));
            }

            return positions;
        }
        private void CreateSubBubble(string display, string fullPath, Point position, MouseButtonEventHandler clickHandler)
        {
            // X = konstant rechts von der MainBubble, Y = variiert vertikal
            double x = position.X;
            double y = position.Y;

            var bubble = new Bubble
            {
                Height = _bubbleHeight,
                Text = display,
                ToolTipText = fullPath,
                FontSizeValue = 20,
                TextIconLayout = TextIconLayout.IconLeftOfText,
                HorizontalAlignment = HorizontalAlignment.Center,
                BorderDistance = 10
            };
            bubble.ApplyTheme(_bubbleVisualTheme);
            bubble.RenderStyle = BubbleRenderStyle.Style3D;
            //if (positionIndex == 0)
            //    bubble.Icon = new BitmapImage(new Uri("pack://application:,,,/Assets/ProjectNew.ico"));
            //else if (positionIndex < totalCount - 1)
            //    bubble.Icon = new BitmapImage(new Uri("pack://application:,,,/Assets/Project.ico"));

            bubble.PreviewMouseLeftButtonDown += clickHandler;

            Canvas.SetLeft(bubble, x);
            Canvas.SetTop(bubble, y - bubble.Height / 2);

            if (_showLines)
            {
                // Linie von MainBubble-Mitte zur SubBubble-Mitte
                double lineOffset = 8;
                var startX = _mainBubbleCenter.X + (MainBubble.Width / 1.3) + lineOffset;
                //var startX = _mainBubbleCenter.X;
                var path = CreateBezierCurve(
                    new Point(startX, _mainBubbleCenter.Y),
                    new Point(x, y)
                );
                RootCanvas.Children.Add(path);
                Canvas.SetZIndex(path, 0);
            }

            RootCanvas.Children.Add(bubble);
            Canvas.SetZIndex(bubble, 1);
        }

        private Path CreateBezierCurve(Point start, Point end)
        {
            // Differenzvektor
            double dx = end.X - start.X;
            double dy = end.Y - start.Y;

            // Verhältnis der Y-Differenz zur X-Distanz
            double weight = 0.3; // wie stark die Bezier-Kurve sich wölbt

            // Steuerpunkte mittig, aber mit Y-Einfluss für sanfte Biegung
            var cp1 = new Point(start.X + dx * 0.5, start.Y + dy * weight);
            var cp2 = new Point(start.X + dx * 0.5, end.Y - dy * weight);

            var segment = new BezierSegment(cp1, cp2, end, true);
            var figure = new PathFigure { StartPoint = start, Segments = new PathSegmentCollection { segment }, IsClosed = false };
            var geometry = new PathGeometry(new[] { figure });

            return new Path
            {
                Stroke = new SolidColorBrush(Color.FromRgb(130, 160, 230)),
                StrokeThickness = 2,
                Data = geometry,
                IsHitTestVisible = false
            };
        }

        private string ShortenPath(string fullPath)
        {
            string file = IOPath.GetFileName(fullPath);
            string start = IOPath.GetPathRoot(fullPath)?.TrimEnd('\\') ?? "";
            string shortPath = $"{start}\\...\\{file}";
            return shortPath.Length > 20 ? shortPath[..17] + "…" : shortPath;
        }
        private void OnAnyBubbleClicked(object sender, MouseButtonEventArgs e)
        {
            var mainWindow = new MainWindow();  // oder dein echtes App-Fenster
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();

            this.Close(); // das aktuelle StartWindow schließen
        }
        private void OnNewProjectClicked(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("Neues Projekt starten");
        }
        private void OnOpenProjectClicked(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("Neues Projekt starten");
        }
        private void OnRecentClicked(object sender, MouseButtonEventArgs e)
        {
            //if (sender is FrameworkElement fe)
            //    MessageBox.Show($"Projekt öffnen: {fe.ToolTip}");
        }
        private void OnExitClicked(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}