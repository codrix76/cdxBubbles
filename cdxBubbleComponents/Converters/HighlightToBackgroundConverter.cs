using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace cdxBubbleComponents.Converters
{
    public class HighlightToBackgroundConverter : IValueConverter
    {
        public Brush HighlightBrush { get; set; } = Brushes.LightSkyBlue;
        public Brush NormalBrush { get; set; } = Brushes.Transparent;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isHighlighted)
                return isHighlighted ? HighlightBrush : NormalBrush;
            return NormalBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
