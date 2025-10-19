using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MiniRPG.Services;

namespace MiniRPG.Converters
{
    /// <summary>
    /// Converter that returns Visibility.Visible for Rain weather, Collapsed otherwise.
    /// </summary>
    public class RainVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WeatherType weather)
            {
                return weather == WeatherType.Rain ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter that returns Visibility.Visible for Snow weather, Collapsed otherwise.
    /// </summary>
    public class SnowVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WeatherType weather)
            {
                return weather == WeatherType.Snow ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter that returns Visibility.Visible for Fog weather, Collapsed otherwise.
    /// </summary>
    public class FogVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WeatherType weather)
            {
                return weather == WeatherType.Fog ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
