using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BubbleControlls.ControlViews
{
    public partial class BubbleColorPickerWindow : Window
    {
        private BitmapSource? _colorBitmap;

        public Color SelectedColor { get; private set; }

        public BubbleColorPickerWindow()
        {
            InitializeComponent();
            UpdatePreview();

            RedBox.TextChanged += (_, __) => UpdatePreview();
            GreenBox.TextChanged += (_, __) => UpdatePreview();
            BlueBox.TextChanged += (_, __) => UpdatePreview();
            AlphaBox.TextChanged += (_, __) => UpdatePreview();
            HexBox.TextChanged += (_, __) => UpdateFromHex();

            OkButton.Click += (_, __) => { DialogResult = true; Close(); };
            CancelButton.Click += (_, __) => { DialogResult = false; Close(); };

            LoadColorBitmap();
            ColorWheel.MouseLeftButtonDown += ColorWheel_MouseLeftButtonDown;
        }

        private void LoadColorBitmap()
        {
            var uri = new Uri("pack://application:,,,/BubbleControlls;component/Assets/ColorWheel.png", UriKind.Absolute);

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = uri;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            _colorBitmap = bitmap;
            ColorWheel.Fill = new ImageBrush(_colorBitmap);
        }

        private void UpdatePreview()
        {
            //if (byte.TryParse(RedBox.Text, out byte r) &&
            //    byte.TryParse(GreenBox.Text, out byte g) &&
            //    byte.TryParse(BlueBox.Text, out byte b) &&
            //    byte.TryParse(AlphaBox.Text, out byte a))
            //{
            //    SelectedColor = Color.FromArgb(a, r, g, b);
            //    PreviewEllipse.Fill = new SolidColorBrush(SelectedColor);
            //    HexBox.Text = $"#{a:X2}{r:X2}{g:X2}{b:X2}";
            //}
        }

        private void ColorWheel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_colorBitmap == null) return;

            Point position = e.GetPosition(ColorWheel);
            int x = (int)(position.X / ColorWheel.Width * _colorBitmap.PixelWidth);
            int y = (int)(position.Y / ColorWheel.Height * _colorBitmap.PixelHeight);

            if (x < 0 || y < 0 || x >= _colorBitmap.PixelWidth || y >= _colorBitmap.PixelHeight)
                return;

            byte[] pixels = new byte[4];
            Int32Rect rect = new Int32Rect(x, y, 1, 1);
            _colorBitmap.CopyPixels(rect, pixels, 4, 0);

            SelectedColor = Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
            PreviewEllipse.Fill = new SolidColorBrush(SelectedColor);
            RedBox.Text = SelectedColor.R.ToString();
            GreenBox.Text = SelectedColor.G.ToString();
            BlueBox.Text = SelectedColor.B.ToString();
            AlphaBox.Text = SelectedColor.A.ToString();
            HexBox.Text = $"#{SelectedColor.A:X2}{SelectedColor.R:X2}{SelectedColor.G:X2}{SelectedColor.B:X2}";
        }
        private void UpdateFromHex()
        {
            try
            {
                string hex = HexBox.Text.TrimStart('#');
                if (hex.Length == 8)
                {
                    byte a = Convert.ToByte(hex.Substring(0, 2), 16);
                    byte r = Convert.ToByte(hex.Substring(2, 2), 16);
                    byte g = Convert.ToByte(hex.Substring(4, 2), 16);
                    byte b = Convert.ToByte(hex.Substring(6, 2), 16);

                    RedBox.Text = r.ToString();
                    GreenBox.Text = g.ToString();
                    BlueBox.Text = b.ToString();
                    AlphaBox.Text = a.ToString();

                    SelectedColor = Color.FromArgb(a, r, g, b);
                    PreviewEllipse.Fill = new SolidColorBrush(SelectedColor);
                }
            }
            catch
            {
                // Ignoriere ungültige Eingabe
            }
        }
    }
}
