using System;
using System.Globalization;
using System.Windows.Data;
using MiniRPG.Models;

namespace MiniRPG.Converters
{
    /// <summary>
    /// Converts a Faction object to its standing text (Allied, Neutral, or Hostile).
    /// </summary>
    public class FactionStandingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Faction faction)
            {
                return faction.GetStanding();
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
