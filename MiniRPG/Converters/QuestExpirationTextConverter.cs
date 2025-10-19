using System;
using System.Globalization;
using System.Windows.Data;

namespace MiniRPG.Converters
{
    /// <summary>
    /// Converts a Quest's ExpireDay property to display text like "(Expires Day X)" or empty string if no expiration.
    /// </summary>
    public class QuestExpirationTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int expireDay)
            {
                return $" (Expires Day {expireDay})";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
