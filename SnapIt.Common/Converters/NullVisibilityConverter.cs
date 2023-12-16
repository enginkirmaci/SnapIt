using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SnapIt.Common.Converters;

public class NullVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter != null && (bool)parameter == true)
        {
            return value != null ? Visibility.Collapsed : Visibility.Visible;
        }

        return value == null ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}