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
    
    public class ActiveToBrushConverter : IValueConverter
    {
        public Brush ActiveBrush { get; set; } = Brushes.LimeGreen;
        public Brush InactiveBrush { get; set; } = Brushes.Gray;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive)
                return isActive ? ActiveBrush : InactiveBrush;
            return InactiveBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
    

}
