using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BubbleControlls.Models;
using Xceed.Wpf.Toolkit;
using System.ComponentModel.DataAnnotations;

namespace BubbleTools
{
    public static class ThemeEditorBuilder
    {
        public static void Build(StackPanel panel, BubbleVisualTheme theme, Action onChanged)
        {
            panel.Children.Clear();

            var props = typeof(BubbleVisualTheme).GetProperties();

            foreach (var prop in props)
            {
                var display = prop.GetCustomAttribute<DisplayAttribute>();
                string name = display?.Name ?? prop.Name;
                string desc = display?.Description ?? "";

                // Container mit Label
                var container = new StackPanel { Margin = new Thickness(0, 5, 0, 5) };
                var label = new TextBlock
                {
                    Text = name,
                    ToolTip = string.IsNullOrWhiteSpace(desc) ? null : desc,
                    FontWeight = FontWeights.SemiBold
                };
                container.Children.Add(label);

                FrameworkElement? editor = CreateEditor(prop, theme, onChanged);
                if (editor != null)
                    container.Children.Add(editor);

                panel.Children.Add(container);
            }
        }

        private static FrameworkElement? CreateEditor(PropertyInfo prop, BubbleVisualTheme theme, Action onChanged)
        {
            var type = prop.PropertyType;

            // COLOR
            if (type == typeof(Color))
            {
                var picker = new ColorPicker
                {
                    UsingAlphaChannel = false,
                    SelectedColor = (Color?)prop.GetValue(theme)
                };
                picker.SelectedColorChanged += (_, e) =>
                {
                    if (e.NewValue is Color c)
                    {
                        prop.SetValue(theme, c);
                        onChanged();
                    }
                };
                return picker;
            }

            // BRUSH (SolidColorBrush)
            if (type == typeof(Brush))
            {
                var brush = prop.GetValue(theme) as SolidColorBrush;
                var picker = new ColorPicker
                {
                    UsingAlphaChannel = true,
                    SelectedColor = brush?.Color
                };
                picker.SelectedColorChanged += (_, e) =>
                {
                    if (e.NewValue is Color c)
                    {
                        prop.SetValue(theme, new SolidColorBrush(c));
                        onChanged();
                    }
                };
                return picker;
            }

            // DOUBLE
            if (type == typeof(double))
            {
                var slider = new Slider
                {
                    Minimum = 0,
                    Maximum = 100,
                    Width = 200,
                    Value = (double)prop.GetValue(theme)
                };
                slider.ValueChanged += (_, _) =>
                {
                    prop.SetValue(theme, slider.Value);
                    onChanged();
                };
                return slider;
            }

            // INT
            if (type == typeof(int))
            {
                var slider = new Slider
                {
                    Minimum = 0,
                    Maximum = 255,
                    Width = 200,
                    Value = (int)prop.GetValue(theme)
                };
                slider.ValueChanged += (_, _) =>
                {
                    prop.SetValue(theme, (int)slider.Value);
                    onChanged();
                };
                return slider;
            }

            // BOOL
            if (type == typeof(bool))
            {
                var box = new CheckBox
                {
                    IsChecked = (bool)prop.GetValue(theme),
                };
                box.Checked += (_, _) => { prop.SetValue(theme, true); onChanged(); };
                box.Unchecked += (_, _) => { prop.SetValue(theme, false); onChanged(); };
                return box;
            }

            // FONT FAMILY (TextBox)
            if (type == typeof(FontFamily))
            {
                var box = new TextBox
                {
                    Text = ((FontFamily?)prop.GetValue(theme))?.Source ?? "",
                    Width = 200
                };
                box.LostFocus += (_, _) =>
                {
                    prop.SetValue(theme, new FontFamily(box.Text));
                    onChanged();
                };
                return box;
            }

            // FONT WEIGHT (TextBox einfach)
            if (type == typeof(FontWeight))
            {
                var box = new TextBox
                {
                    Text = ((FontWeight)prop.GetValue(theme)).ToString(),
                    Width = 200
                };
                box.LostFocus += (_, _) =>
                {
                    if (Enum.TryParse<FontWeight>(box.Text, true, out var weight))
                    {
                        prop.SetValue(theme, weight);
                        onChanged();
                    }
                };
                return box;
            }

            return null;
        }
    }
}
