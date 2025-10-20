using System;
using System.Globalization;
using System.Windows.Data;

namespace MiniRPG.Converters
{
    /// <summary>
    /// Converts a null/non-null value to boolean for UI binding.
    /// Returns true if value is not null, false otherwise.
    /// </summary>
    public class NullToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
