using BubbleControlls.Models;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BubbleControlls.ControlViews
{
    /// <summary>
    /// Interaktionslogik für BubbleMsgBox.xaml
    /// </summary>
    public partial class BubbleMsgBox : Window
    {
        private MessageBoxResult _result = MessageBoxResult.None;

        public string Caption { get; set; } = "";

        public static MessageBoxResult Show(string message)
            => Show(message, "", MessageBoxButton.OK, MessageBoxImage.None, null);

        public static MessageBoxResult Show(string message, string caption)
            => Show(message, caption, MessageBoxButton.OK, MessageBoxImage.None, null);

        public static MessageBoxResult Show(string message, string caption, MessageBoxButton buttons)
            => Show(message, caption, buttons, MessageBoxImage.None, null);

        public static MessageBoxResult Show(string message, string caption, MessageBoxButton buttons, MessageBoxImage icon)
            => Show(message, caption, buttons, icon, null);

        public static MessageBoxResult Show(string message, string caption = "", MessageBoxButton buttons = MessageBoxButton.OK,
            MessageBoxImage icon = MessageBoxImage.None, Window? owner = null, BubbleVisualTheme? theme = null)
        {
            var box = new BubbleMsgBox(message, caption, buttons, icon, theme)
            {
                Owner = owner
            };

            box.ShowDialog();
            return box._result;
        }
        public BubbleMsgBox(string message, string caption, MessageBoxButton buttons, MessageBoxImage icon, BubbleVisualTheme? theme = null)
        {
            InitializeComponent();
            this.DataContext = this;
            Loaded += (_, _) =>
            {
                if (Owner == null)
                {
                    Left = (SystemParameters.WorkArea.Width - ActualWidth) / 2 + SystemParameters.WorkArea.Left;
                    Top = (SystemParameters.WorkArea.Height - ActualHeight) / 2 + SystemParameters.WorkArea.Top;
                }
            };
            InfoBox.DisplayText = message;
            Caption = caption;

            SetupCaption(caption);
            SetupIcon(icon);
            SetupButtons(buttons);

            if (theme == null)
                theme = BubbleVisualThemes.Standard();

            ApplyTheme(theme);
        }

        public void ApplyTheme(BubbleVisualTheme theme)
        {
            if (theme == null)
                return;

            var visuals = theme.BubbleBaseVisuals;

            // Hintergrund (Fenster)
            //this.Background = visuals.Background ?? Brushes.Transparent;

            // Titelzeile
            InfoTitle.Background = visuals.TitleBackground ?? Brushes.DimGray;
            InfoTitle.Foreground = visuals.Foreground ?? Brushes.White;

            // Info-Inhalt
            InfoBox.ApplyTheme(theme);

            // Buttons
            foreach (var child in ButtonPanel.Children)
            {
                if (child is Bubble b)
                    b.ApplyTheme(theme);
            }
        }
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void SetupIcon(MessageBoxImage icon)
        {
            string? iconPath = icon switch
            {
                MessageBoxImage.Information => "../Assets/info.png",
                MessageBoxImage.Warning => "../Assets/warning.png",
                MessageBoxImage.Error => "../Assets/error.png",
                MessageBoxImage.Question => "../Assets/question.png",
                _ => null
            };

            if (iconPath != null)
            {
                InfoBox.DisplayIcon = new BitmapImage(new Uri(iconPath, UriKind.Relative));
            }
        }

        private void SetupCaption(string caption)
        {
            if (!string.IsNullOrEmpty(caption))
                InfoTitle.Visibility = Visibility.Visible;
            else 
                InfoTitle.Visibility = Visibility.Collapsed;
        }
        private void SetupButtons(MessageBoxButton buttons)
        {
            void AddButton(string text, MessageBoxResult result)
            {
                var b = new Bubble
                {
                    Text = text,
                    Height = 40,
                    Margin = new Thickness(5)
                };

                b.MouseLeftButtonUp += (s, e) =>
                {
                    if (b.IsMouseOver) // nur wenn Maus beim Loslassen über dem Button ist
                    {
                        _result = result;
                        DialogResult = true;
                        Close();
                    }
                };

                ButtonPanel.Children.Add(b);
            }

            switch (buttons)
            {
                case MessageBoxButton.OK:
                    AddButton("OK", MessageBoxResult.OK);
                    break;
                case MessageBoxButton.OKCancel:
                    AddButton("OK", MessageBoxResult.OK);
                    AddButton("Abbrechen", MessageBoxResult.Cancel);
                    break;
                case MessageBoxButton.YesNo:
                    AddButton("Ja", MessageBoxResult.Yes);
                    AddButton("Nein", MessageBoxResult.No);
                    break;
                case MessageBoxButton.YesNoCancel:
                    AddButton("Ja", MessageBoxResult.Yes);
                    AddButton("Nein", MessageBoxResult.No);
                    AddButton("Abbrechen", MessageBoxResult.Cancel);
                    break;
            }
        }

        public static MessageBoxResult Show(string message, string caption = "",
            MessageBoxButton buttons = MessageBoxButton.OK,
            MessageBoxImage icon = MessageBoxImage.None,
            Window? owner = null)
        {
            var box = new BubbleMsgBox(message, caption, buttons, icon)
            {
                Owner = owner
            };

            box.ShowDialog();
            return box._result;
        }
    }
}
