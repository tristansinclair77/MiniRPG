using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MiniRPG.Models;

namespace MiniRPG.Converters
{
    /// <summary>
    /// Converts a Faction object to a color brush based on its standing.
    /// </summary>
    public class FactionStandingColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Faction faction)
            {
                string standing = faction.GetStanding();
                return standing switch
                {
                    "Allied" => Brushes.LightGreen,
                    "Hostile" => Brushes.Red,
                    "Neutral" => Brushes.LightGray,
                    _ => Brushes.White
                };
            }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
