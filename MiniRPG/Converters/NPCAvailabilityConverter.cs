using System;
using System.Globalization;
using System.Windows.Data;
using MiniRPG.Models;

namespace MiniRPG.Converters
{
    /// <summary>
    /// Converter that returns NPC availability status text.
    /// Returns "(Available)" if NPC.IsAvailableNow() is true, "(Away)" otherwise.
    /// </summary>
    public class NPCAvailabilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NPC npc)
            {
                return npc.IsAvailableNow() ? " (Available)" : " (Away)";
            }
            
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    /// <summary>
    /// Converter that returns color based on NPC availability.
    /// Returns Green if NPC.IsAvailableNow() is true, Gray otherwise.
    /// </summary>
    public class NPCAvailabilityColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NPC npc)
            {
                return npc.IsAvailableNow() ? "LimeGreen" : "Gray";
            }
            
            return "White";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
