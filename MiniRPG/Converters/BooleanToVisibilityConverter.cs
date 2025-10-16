using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MiniRPG.Converters
{
    /// <summary>
    /// Converts a boolean value to Visibility for UI binding.
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && b)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility v)
                return v == Visibility.Visible;
            return false;
        }
        // TODO: Add more converters later for game state UI control
    }
}
