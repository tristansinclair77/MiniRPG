using System;

namespace MiniRPG.Services
{
    /// <summary>
    /// Provides environment and lighting management for the game world.
    /// </summary>
    public static class EnvironmentService
    {
        /// <summary>
        /// The current lighting state of the game world.
        /// </summary>
        public static string CurrentLighting { get; private set; } = "Daylight";

        /// <summary>
        /// Event triggered when the lighting changes.
        /// </summary>
        public static event EventHandler<string>? OnLightingChanged;

        /// <summary>
        /// Updates the lighting based on the current time of day.
        /// Triggers OnLightingChanged event if the lighting changes.
        /// </summary>
        public static void UpdateLighting()
        {
            string previousLighting = CurrentLighting;
            string newLighting;

            switch (TimeService.GetTimeOfDay())
            {
                case "Morning":
                case "Afternoon":
                    newLighting = "Daylight";
                    break;
                case "Evening":
                    newLighting = "Twilight";
                    break;
                case "Night":
                    newLighting = "Night";
                    break;
                default:
                    newLighting = "Daylight";
                    break;
            }

            if (newLighting != previousLighting)
            {
                CurrentLighting = newLighting;
                OnLightingChanged?.Invoke(null, CurrentLighting);
            }
        }

        // TODO: Add regional weather, fog, and rain states later
    }
}
