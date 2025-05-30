using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace cdxBubbleComponents.Converters
{
    public class SizeToHeightConverter : IValueConverter
    {
        public double BaseHeight { get; set; } = 40.0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double factor)
                return factor * BaseHeight;
            return BaseHeight;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
