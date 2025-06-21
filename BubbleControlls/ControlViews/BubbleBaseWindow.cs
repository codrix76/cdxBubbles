using BubbleControlls.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BubbleControlls.ControlViews
{
    [ContentProperty(nameof(BubbleContent))]
    public class BubbleBaseWindow : Window
    {
        protected Border OuterBorder;
        protected Border InnerBorder;
        protected Border TitelBorder;
        protected StackPanel ButtonStack;
        protected Image _Icon;
        protected ContentPresenter _bubblePresenter;
        //protected Grid _footerArea;
        //private readonly BubbleVisualTheme _theme = BubbleVisualThemes.Standard();
        private Point? _resizeStartPoint = null;
        public BubbleVisualTheme WindowTheme { get; set; } = BubbleVisualThemes.Dark();
        public BubbleBaseWindow()
        {
            AllowsTransparency = true;
            Background = Brushes.Transparent;
            WindowStyle = WindowStyle.None;
            SizeToContent = SizeToContent.Manual;
            ResizeMode = ResizeMode.CanResize;
            Width = 800;
            Height = 450;

            // Hauptlayout
            var layoutGrid = new Grid();
            layoutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Title
            layoutGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            //layoutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Footer

            // Resize Rectangle
            Rectangle resizeRect = CreateResizeRect();

            // === TITELZEILE ===
            TitelBorder = CreateTitleBorder();

            _Icon = new Image
            {
                Width = 16,
                Height = 16,
                Margin = new Thickness(8, 0, 4, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            BindingOperations.SetBinding(_Icon, Image.SourceProperty, new Binding("Icon") { Source = this });
            Grid.SetColumn(_Icon, 0);

            var titleBarGrid = CreateTitleGrid();
            
            // Titeltext
            var titleText = CreateTitleText();

            BindingOperations.SetBinding(titleText, TextBlock.TextProperty, new Binding("Title") { Source = this });
            Grid.SetColumn(titleText, 1);
            titleBarGrid.Children.Add(_Icon);
            titleBarGrid.Children.Add(titleText);

            // Button-Stack (Min / Max / Close)
            ButtonStack = CreateTitleSymbolPanel();
            Grid.SetColumn(ButtonStack, 2);
            
            titleBarGrid.Children.Add(ButtonStack);
            titleBarGrid.Background = Brushes.Transparent;

            // DragMove bei Klick auf Titelzeile
            titleBarGrid.MouseLeftButtonDown += (_, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    DragMove();
            };

            TitelBorder.Child = titleBarGrid;
            Grid.SetRow(TitelBorder, 0);
            layoutGrid.Children.Add(TitelBorder);

            // ===== INHALT mit Doppelrahmen =====
            OuterBorder = CreateOuterBorder();
            InnerBorder = CreateInnerBorder();

            _bubblePresenter = new ContentPresenter();
            InnerBorder.Child = _bubblePresenter;
            OuterBorder.Child = InnerBorder;

            Grid.SetRow(OuterBorder, 1);
            layoutGrid.Children.Add(OuterBorder);

            //// ===== FOOTER (vorbereitet, leer) =====
            //_footerArea = new Grid
            //{
            //    Height = 30,
            //    Margin = new Thickness(12, 0, 12, 12)
            //};
            //Grid.SetRow(_footerArea, 2);
            //layoutGrid.Children.Add(_footerArea);

            layoutGrid.Children.Add(resizeRect);
            Grid.SetRowSpan(resizeRect, 2);
            base.Content = layoutGrid;
            ApplyTheme(WindowTheme);
            this.Loaded += (_, _) =>
            {
                UpdateTitleButtons(); // z. B. hier ist ShowMinMax korrekt
            };
        }

        private void UpdateTitleButtons()
        {
            ButtonStack.Children.Clear();
            if (ShowMinMax)
            {
                ButtonStack.Children.Add(CreateTitleSymbolText("🗕", () => WindowState = WindowState.Minimized));
                ButtonStack.Children.Add(CreateTitleSymbolText("🗖", ToggleMaximize));
            }
            ButtonStack.Children.Add(CreateTitleSymbolText("✖", Close));

            if (_Icon.Source == null)
            {
                _Icon.Visibility = Visibility.Collapsed;
            }
            else
            {
                _Icon.Visibility = Visibility.Visible;
            }
        }
        private Rectangle CreateResizeRect()
        {
            var resizeRect = new Rectangle
            {
                Width = 16,
                Height = 16,
                Fill = Brushes.Transparent,
                Cursor = Cursors.SizeNWSE,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0),
                IsHitTestVisible = true
            };
            resizeRect.MouseLeftButtonDown += (s, e) =>
            {
                _resizeStartPoint = e.GetPosition(this);
                resizeRect.CaptureMouse();
            };

            resizeRect.MouseMove += (s, e) =>
            {
                if (_resizeStartPoint is null) return;

                Point current = e.GetPosition(this);
                double dx = current.X - _resizeStartPoint.Value.X;
                double dy = current.Y - _resizeStartPoint.Value.Y;

                Width = Math.Max(Width + dx, MinWidth);
                Height = Math.Max(Height + dy, MinHeight);

                _resizeStartPoint = current;
            };

            resizeRect.MouseLeftButtonUp += (s, e) =>
            {
                _resizeStartPoint = null;
                resizeRect.ReleaseMouseCapture();
            };
            return resizeRect;
        }
        private Grid CreateTitleGrid()
        {
            var titleBarGrid = new Grid
            {
                Height = 30,
                Margin = new Thickness(3,5,3,5)            };
            titleBarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Icon
            titleBarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Title
            titleBarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Buttons
            return titleBarGrid;
        }
        private TextBlock CreateTitleText()
        {
            var titleText = new TextBlock
            {
                Foreground = WindowTheme.Foreground ?? Brushes.White,
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(12, 0, 0, 0)
            };
            return titleText;
        }
        private StackPanel CreateTitleSymbolPanel()
        {
            var buttonStack = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0)
            };
            if (ShowMinMax)
            {
                buttonStack.Children.Add(CreateTitleSymbolText("🗕", () => WindowState = WindowState.Minimized));
                buttonStack.Children.Add(CreateTitleSymbolText("🗖", ToggleMaximize));
            }
            buttonStack.Children.Add(CreateTitleSymbolText("✖", Close));
            return buttonStack;
        }
        private TextBlock CreateTitleSymbolText(string symbol, Action action)
        {
            var tb = new TextBlock
            {
                Text = symbol,
                Foreground = Brushes.White,
                FontSize = 14,
                Margin = new Thickness(6, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Cursor = Cursors.Hand
            };
            tb.MouseLeftButtonDown += (_, __) => action();
            tb.MouseEnter += (_, __) => tb.Opacity = 0.7;
            tb.MouseLeave += (_, __) => tb.Opacity = 1.0;
            return tb;
        }
        private Border CreateTitleBorder()
        {
            var titleBorder = new Border
            {
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(2,0,2,2)
            };
            return titleBorder;
        }
        private Border CreateOuterBorder()
        {
            var outerBorder = new Border
            {
                CornerRadius = new CornerRadius(20),
                Padding = new Thickness(6),
                Background = Brushes.Transparent
            };
            return outerBorder;
        }
        private Border CreateInnerBorder()
        {
            var innerBorder = new Border
            {
                CornerRadius = new CornerRadius(16),
                Padding = new Thickness(6)
            };
            return innerBorder;
        }
        private void ToggleMaximize()
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        public virtual void ApplyTheme(BubbleVisualTheme theme)
        {
            if (theme == null)
                return;

            if (Show3D)
            {
                TitelBorder.Background = new LinearGradientBrush(theme.TitleEffectColor1, theme.TitleEffectColor2, 90);
                OuterBorder.BorderBrush = new LinearGradientBrush(theme.OuterBorderEffectColor1, theme.OuterBorderEffectColor2, 90);
                InnerBorder.BorderBrush = new LinearGradientBrush(theme.InnerBorderEffectColor1, theme.InnerBorderEffectColor2, 90);
            }
            else
            {
                TitelBorder.Background = theme.TitleBackground ?? Brushes.Gray;
                OuterBorder.BorderBrush = theme.OuterBorderColor ?? Brushes.Gray;
                InnerBorder.BorderBrush = theme.Border ?? Brushes.DarkGray;
                
            }
            InnerBorder.Background = theme.Background ?? Brushes.Transparent;

            FontFamily = theme.FontFamily ?? new FontFamily("Segoe UI");
            FontSize = theme.FontSize;
            FontWeight = theme.FontWeight;
            FontStyle = theme.FontStyle;
            Foreground = theme.Foreground ?? Brushes.White;

            OuterBorder.BorderThickness = theme.OuterBorderThickness;
            InnerBorder.BorderThickness = theme.BorderThickness;
        }

        public object BubbleContent
        {
            get => _bubblePresenter.Content;
            set => _bubblePresenter.Content = value;
        }

        //public Grid FooterArea => _footerArea;
        public static readonly DependencyProperty ShowMinMaxProperty =
            DependencyProperty.Register(nameof(ShowMinMax), typeof(bool), typeof(BubbleBaseWindow), new PropertyMetadata(true));

        public bool ShowMinMax
        {
            get => (bool)GetValue(ShowMinMaxProperty);
            set => SetValue(ShowMinMaxProperty, value);
        }
        public bool Show3D { get; set; } = true;
    }

    public static class GridExtensions
    {
        public static T WithGridRow<T>(this T element, int row) where T : UIElement
        {
            Grid.SetRow(element, row);
            return element;
        }
    }
}
