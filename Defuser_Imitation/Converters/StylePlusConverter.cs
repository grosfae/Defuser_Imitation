using System;
using System.Globalization;
using System.Windows.Data;

namespace Defuser_Imitation.Converters
{
    public class StylePlusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && parameter != null)
            {
                double param = double.Parse(parameter.ToString());
                double WidthValue = (double)value;
                return WidthValue + param;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
