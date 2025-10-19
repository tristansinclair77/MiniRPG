using System;

namespace MiniRPG.Services
{
    /// <summary>
    /// Provides time tracking and time-of-day functionality for the game.
    /// </summary>
    public static class TimeService
    {
        /// <summary>
        /// Event triggered when the hour changes.
        /// </summary>
        public static event EventHandler<int>? OnHourChanged;

        /// <summary>
        /// The current day in the game world.
        /// </summary>
        public static int Day { get; set; } = 1;

        /// <summary>
        /// The current hour in the game world (0-23). Starts at 8 (morning).
        /// </summary>
        public static int Hour { get; set; } = 8;

        /// <summary>
        /// The current region name for weather calculations. Defaults to "Default".
        /// </summary>
        public static string CurrentRegionName { get; set; } = "Default";

        /// <summary>
        /// Static constructor to initialize event subscriptions.
        /// </summary>
        static TimeService()
        {
            // Subscribe to our own OnHourChanged event to handle weather changes
            OnHourChanged += HandleWeatherChanges;
        }

        /// <summary>
        /// Handles weather changes every 6 hours.
        /// </summary>
        private static void HandleWeatherChanges(object? sender, int newHour)
        {
            // Check if hour is divisible by 6 (0, 6, 12, 18)
            if (newHour % 6 == 0)
            {
                // Use CurrentRegionName or "Default" as fallback
                string regionName = string.IsNullOrEmpty(CurrentRegionName) ? "Default" : CurrentRegionName;
                
                EnvironmentService.RandomizeWeather(regionName);
                
                // Log the weather change
                System.Diagnostics.Debug.WriteLine($"The weather changes to {EnvironmentService.Weather}.");
            }
        }

        /// <summary>
        /// Advances the game time by the specified number of hours.
        /// Automatically increments the day when hour reaches 24 or more.
        /// </summary>
        /// <param name="amount">Number of hours to advance</param>
        public static void AdvanceHours(int amount)
        {
            int oldHour = Hour;
            Hour += amount;
            if (Hour >= 24)
            {
                Day++;
                Hour %= 24;
            }
            
            // Fire event if hour has changed
            if (Hour != oldHour)
            {
                OnHourChanged?.Invoke(null, Hour);
            }
        }

        /// <summary>
        /// Returns the current time of day based on the hour.
        /// </summary>
        /// <returns>
        /// "Night" if hour is less than 6,
        /// "Morning" if hour is less than 12,
        /// "Afternoon" if hour is less than 18,
        /// "Evening" otherwise
        /// </returns>
        public static string GetTimeOfDay()
        {
            if (Hour < 6)
            {
                return "Night";
            }
            else if (Hour < 12)
            {
                return "Morning";
            }
            else if (Hour < 18)
            {
                return "Afternoon";
            }
            else
            {
                return "Evening";
            }
        }

        // TODO: Add moon phases, seasons, and weather events later
        // TODO: Add gradual weather transitions and forecast system
        // TODO: Add seasonal festivals and quests
        // TODO: Add temperature effects for survival mechanics
    }
}
