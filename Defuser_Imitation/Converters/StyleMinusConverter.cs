using System.Globalization;
using System.Windows.Data;

namespace Defuser_Imitation.Converters
{
    public class StyleMinusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value != null && parameter != null)
            {
                double param = double.Parse(parameter.ToString());
                double widthValue = (double)value;
                return widthValue - param;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
