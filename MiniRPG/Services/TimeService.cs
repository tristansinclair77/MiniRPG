using System;

namespace MiniRPG.Services
{
    /// <summary>
    /// Provides time tracking and time-of-day functionality for the game.
    /// </summary>
    public static class TimeService
    {
        /// <summary>
        /// The current day in the game world.
        /// </summary>
        public static int Day { get; set; } = 1;

        /// <summary>
        /// The current hour in the game world (0-23). Starts at 8 (morning).
        /// </summary>
        public static int Hour { get; set; } = 8;

        /// <summary>
        /// Advances the game time by the specified number of hours.
        /// Automatically increments the day when hour reaches 24 or more.
        /// </summary>
        /// <param name="amount">Number of hours to advance</param>
        public static void AdvanceHours(int amount)
        {
            Hour += amount;
            if (Hour >= 24)
            {
                Day++;
                Hour %= 24;
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
    }
}
