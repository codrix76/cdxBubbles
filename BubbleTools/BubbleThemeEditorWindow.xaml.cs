using BubbleControlls.ControlViews;
using BubbleControlls.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace BubbleTools
{
    /// <summary>
    /// Interaktionslogik für BubbleThemeEditorWindow.xaml
    /// </summary>
    public partial class BubbleThemeEditorWindow : Window
    {
        private BubbleVisualTheme _currentTheme = BubbleVisualThemes.Standard();
        private BubbleMenu PreviewBubble = new BubbleMenu();
        private BubbleInfoControl PreviewInfoControl = new BubbleInfoControl();
        public BubbleThemeEditorWindow()
        {
            InitializeComponent();
            UpdatePreview(_currentTheme);
            ThemeEditorBuilder.Build(ConfigPanel, _currentTheme, () =>
            {
                UpdatePreview(_currentTheme);
                PreviewBubble.InvalidateVisual();
            });
            DisplayButton.Click += DisplayButton_Click;
            SaveButton.Click += SaveButton_Click;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "theme.json");
            ExportToFile(_currentTheme, path);
        }

        private void DisplayButton_Click(object sender, RoutedEventArgs e)
        {
            string code = GenerateCode(_currentTheme, "MyGeneratedTheme");
            Window codeWindow = CreateCodeWindow(code);
            codeWindow.Show();
        }

        private void UpdatePreview(BubbleVisualTheme currentTheme)
        {
            PreviewPanel.Children.Clear();
            PreviewBubble = new BubbleMenu();
            PreviewBubble.Width = 180;
            PreviewBubble.Height = 180;
            PreviewBubble.VerticalAlignment = VerticalAlignment.Top;
            PreviewBubble.HorizontalAlignment = HorizontalAlignment.Left;
            PreviewBubble.MenuStyleTheme = currentTheme;

            PreviewPanel.Children.Add(PreviewBubble);
            
            PreviewInfoControl = new  BubbleInfoControl();
            PreviewInfoControl.Width = 200;
            PreviewInfoControl.Height = 100;
            PreviewInfoControl.DisplayText = "Demo Text";
            PreviewInfoControl.ApplyTheme(currentTheme);
            
            PreviewPanel.Children.Add(PreviewInfoControl);
        }

        private Window CreateCodeWindow(string code)
        {
            var window = new Window
            {
                Title = "Exportierter Theme-Code",
                Width = 800,
                Height = 600,
                Content = new TextBox
                {
                    Text = code,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 13,
                    IsReadOnly = true,
                    AcceptsReturn = true,
                    AcceptsTab = true,
                    TextWrapping = TextWrapping.Wrap,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    Margin = new Thickness(10)
                }
            };
            return window;
        }
        private string GenerateCode(BubbleVisualTheme theme, string methodName = "MyTheme")
        {
            var sb = new StringBuilder();

            sb.AppendLine($"public static BubbleVisualTheme {methodName}()");
            sb.AppendLine("{");
            sb.AppendLine("    return new BubbleVisualTheme");
            sb.AppendLine("    {");

            var props = typeof(BubbleVisualTheme).GetProperties();

            foreach (var prop in props)
            {
                var val = prop.GetValue(theme);

                if (val is SolidColorBrush brush)
                {
                    var c = brush.Color;
                    sb.AppendLine($"        {prop.Name} = new SolidColorBrush(Color.FromArgb({c.A}, {c.R}, {c.G}, {c.B})),");
                }
                else if (val is Color color)
                {
                    sb.AppendLine($"        {prop.Name} = Color.FromArgb({color.A}, {color.R}, {color.G}, {color.B}),");
                }
                else if (val is FontFamily font)
                {
                    sb.AppendLine($"        {prop.Name} = new FontFamily(\"{font.Source}\"),");
                }
                else if (val is FontWeight weight)
                {
                    sb.AppendLine($"        {prop.Name} = FontWeights.{weight.ToString()},");
                }
                else if (val is FontStyle style)
                {
                    sb.AppendLine($"        {prop.Name} = FontStyles.{style.ToString()},");
                }
                else if (val is Thickness t)
                {
                    sb.AppendLine($"        {prop.Name} = new Thickness({t.Left}),");
                }
                else if (val is bool b)
                {
                    sb.AppendLine($"        {prop.Name} = {b.ToString().ToLower()},");
                }
                else if (val is int i)
                {
                    sb.AppendLine($"        {prop.Name} = {i},");
                }
                else if (val is double d)
                {
                    sb.AppendLine($"        {prop.Name} = {d.ToString(System.Globalization.CultureInfo.InvariantCulture)},");
                }
            }

            sb.AppendLine("    };");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private void ExportToFile(BubbleVisualTheme theme, string filePath)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto
            };

            string json = JsonConvert.SerializeObject(theme, settings);
            File.WriteAllText(filePath, json);
        }

        private string ExportToString(BubbleVisualTheme theme)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto
            };

            return JsonConvert.SerializeObject(theme, settings);
        }

    }
}
