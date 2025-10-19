using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MiniRPG.Services;

namespace MiniRPG.Converters
{
    /// <summary>
    /// Multi-value converter that determines if a quest is within 1 day of expiration.
    /// Returns red color if within 1 day of expiration, white otherwise.
    /// </summary>
    public class QuestExpirationColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0] = ExpireDay (int?)
            // values[1] = CurrentDay (int)
            
            if (values.Length >= 2 && values[0] is int expireDay && values[1] is int currentDay)
            {
                int daysUntilExpiration = expireDay - currentDay;
                
                // Red if within 1 day of expiration (0 or 1 day remaining)
                if (daysUntilExpiration <= 1)
                {
                    return new SolidColorBrush(Colors.Red);
                }
            }
            
            // Default white color
            return new SolidColorBrush(Colors.White);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
