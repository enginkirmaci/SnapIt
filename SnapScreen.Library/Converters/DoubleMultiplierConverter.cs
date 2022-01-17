using System;
using System.Globalization;
using System.Windows.Data;

namespace SnapScreen.Library.Converters
{
    public class DoubleMultiplierConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value * 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value / 100;
        }
    }
}