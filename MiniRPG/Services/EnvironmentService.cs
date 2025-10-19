using System;

namespace MiniRPG.Services
{
    /// <summary>
    /// Weather types available in the game.
    /// </summary>
    public enum WeatherType
    {
        Clear,
        Rain,
        Storm,
        Snow,
        Fog
    }

    /// <summary>
    /// Provides environment and lighting management for the game world.
    /// </summary>
    public static class EnvironmentService
    {
        private static Random _random = new Random();
        private static int _hoursSinceWeatherChange = 0;

        /// <summary>
        /// The current lighting state of the game world.
        /// </summary>
        public static string CurrentLighting { get; private set; } = "Daylight";

        /// <summary>
        /// The current weather in the game world.
        /// </summary>
        public static WeatherType Weather { get; private set; } = WeatherType.Clear;

        /// <summary>
        /// Event triggered when the lighting changes.
        /// </summary>
        public static event EventHandler<string>? OnLightingChanged;

        /// <summary>
        /// Event triggered when the weather changes.
        /// </summary>
        public static event EventHandler<WeatherType>? OnWeatherChanged;

        /// <summary>
        /// Updates the lighting based on the current time of day.
        /// Triggers OnLightingChanged event if the lighting changes.
        /// Also checks for weather changes every 6 hours.
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

            // Check for weather changes every 6 hours
            _hoursSinceWeatherChange++;
            if (_hoursSinceWeatherChange >= 6)
            {
                _hoursSinceWeatherChange = 0;
                // Default to "Plains" if no specific region is provided
                RandomizeWeather("Plains");
            }
        }

        /// <summary>
        /// Randomizes weather based on the region type.
        /// </summary>
        /// <param name="regionName">The name of the region to determine weather probabilities</param>
        public static void RandomizeWeather(string regionName)
        {
            WeatherType previousWeather = Weather;
            WeatherType newWeather;

            // Determine region type from region name
            string regionType = DetermineRegionType(regionName);

            int roll = _random.Next(100);

            switch (regionType)
            {
                case "Plains":
                case "Town":
                    // 60% Clear, 25% Rain, 10% Fog, 5% Storm
                    if (roll < 60)
                        newWeather = WeatherType.Clear;
                    else if (roll < 85)
                        newWeather = WeatherType.Rain;
                    else if (roll < 95)
                        newWeather = WeatherType.Fog;
                    else
                        newWeather = WeatherType.Storm;
                    break;

                case "Mountain":
                    // 40% Clear, 40% Snow, 20% Fog
                    if (roll < 40)
                        newWeather = WeatherType.Clear;
                    else if (roll < 80)
                        newWeather = WeatherType.Snow;
                    else
                        newWeather = WeatherType.Fog;
                    break;

                default:
                    // Default to Plains weather
                    if (roll < 60)
                        newWeather = WeatherType.Clear;
                    else if (roll < 85)
                        newWeather = WeatherType.Rain;
                    else if (roll < 95)
                        newWeather = WeatherType.Fog;
                    else
                        newWeather = WeatherType.Storm;
                    break;
            }

            if (newWeather != previousWeather)
            {
                Weather = newWeather;
                OnWeatherChanged?.Invoke(null, Weather);
            }
        }

        /// <summary>
        /// Determines the region type from the region name for weather calculation.
        /// </summary>
        private static string DetermineRegionType(string regionName)
        {
            string lowerName = regionName.ToLower();

            if (lowerName.Contains("mountain") || lowerName.Contains("peak") || lowerName.Contains("highland"))
            {
                return "Mountain";
            }
            else if (lowerName.Contains("town") || lowerName.Contains("city") || lowerName.Contains("village"))
            {
                return "Town";
            }
            else if (lowerName.Contains("plain") || lowerName.Contains("field") || lowerName.Contains("grassland"))
            {
                return "Plains";
            }
            else if (lowerName.Contains("woods") || lowerName.Contains("forest"))
            {
                return "Plains"; // Woods get similar weather to plains
            }
            else
            {
                return "Plains"; // Default
            }
        }

        // TODO: Add localized weather (per-region tracking)
        // TODO: Add weather-based enemy effects
    }
}
