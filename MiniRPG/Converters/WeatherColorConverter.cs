using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MiniRPG.Services;

namespace MiniRPG.Converters
{
    /// <summary>
    /// Converter that returns color based on weather type.
    /// Blue for Rain, White for Snow, Gray for Fog, Yellow for Clear.
    /// </summary>
    public class WeatherColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WeatherType weather)
            {
                switch (weather)
                {
                    case WeatherType.Rain:
                    case WeatherType.Storm:
                        return new SolidColorBrush(Colors.Blue);
                    case WeatherType.Snow:
                        return new SolidColorBrush(Colors.White);
                    case WeatherType.Fog:
                        return new SolidColorBrush(Colors.Gray);
                    case WeatherType.Clear:
                        return new SolidColorBrush(Colors.Yellow);
                    default:
                        return new SolidColorBrush(Colors.White);
                }
            }
            
            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
