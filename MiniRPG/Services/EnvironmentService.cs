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
    /// Seasons available in the game.
    /// </summary>
    public enum Season
    {
        Spring,
        Summer,
        Autumn,
        Winter
    }

    /// <summary>
    /// Provides environment and lighting management for the game world.
    /// </summary>
    public static class EnvironmentService
    {
        private static Random _random = new Random();
        private static int _hoursSinceWeatherChange = 0;

        /// <summary>
        /// Days per season (30 days = 1 season).
        /// </summary>
        public const int DaysPerSeason = 30;

        /// <summary>
        /// The current lighting state of the game world.
        /// </summary>
        public static string CurrentLighting { get; private set; } = "Daylight";

        /// <summary>
        /// The current weather in the game world.
        /// </summary>
        public static WeatherType Weather { get; private set; } = WeatherType.Clear;

        /// <summary>
        /// The current season in the game world.
        /// </summary>
        public static Season CurrentSeason { get; private set; } = Season.Spring;

        /// <summary>
        /// Event triggered when the lighting changes.
        /// </summary>
        public static event EventHandler<string>? OnLightingChanged;

        /// <summary>
        /// Event triggered when the weather changes.
        /// </summary>
        public static event EventHandler<WeatherType>? OnWeatherChanged;

        /// <summary>
        /// Event triggered when the season changes.
        /// </summary>
        public static event EventHandler<Season>? OnSeasonChanged;

        /// <summary>
        /// Updates the lighting based on the current time of day and weather.
        /// Triggers OnLightingChanged event if the lighting changes.
        /// Also checks for weather changes every 6 hours and season changes.
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

            // Check for season changes
            UpdateSeason();

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
        /// Updates the current season based on the current day.
        /// Triggers OnSeasonChanged event if the season changes.
        /// </summary>
        private static void UpdateSeason()
        {
            Season previousSeason = CurrentSeason;
            
            // Calculate season based on day (30 days per season)
            int seasonIndex = ((TimeService.Day - 1) / DaysPerSeason) % 4;
            Season newSeason = (Season)seasonIndex;

            if (newSeason != previousSeason)
            {
                CurrentSeason = newSeason;
                OnSeasonChanged?.Invoke(null, CurrentSeason);
                
                // Log season change
                System.Diagnostics.Debug.WriteLine($"Season changed to {CurrentSeason}.");
            }
        }

        /// <summary>
        /// Randomizes weather based on the region type and current season.
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
                    newWeather = GetSeasonalWeatherForPlains(roll);
                    break;

                case "Mountain":
                    newWeather = GetSeasonalWeatherForMountain(roll);
                    break;

                default:
                    // Default to Plains weather
                    newWeather = GetSeasonalWeatherForPlains(roll);
                    break;
            }

            if (newWeather != previousWeather)
            {
                Weather = newWeather;
                OnWeatherChanged?.Invoke(null, Weather);
            }
        }

        /// <summary>
        /// Gets seasonal weather for Plains/Town regions.
        /// </summary>
        private static WeatherType GetSeasonalWeatherForPlains(int roll)
        {
            switch (CurrentSeason)
            {
                case Season.Spring:
                    // Spring: More rain, occasional storms
                    // 40% Clear, 40% Rain, 15% Storm, 5% Fog
                    if (roll < 40)
                        return WeatherType.Clear;
                    else if (roll < 80)
                        return WeatherType.Rain;
                    else if (roll < 95)
                        return WeatherType.Storm;
                    else
                        return WeatherType.Fog;

                case Season.Summer:
                    // Summer: Mostly clear, occasional storms
                    // 70% Clear, 10% Rain, 15% Storm, 5% Fog
                    if (roll < 70)
                        return WeatherType.Clear;
                    else if (roll < 80)
                        return WeatherType.Rain;
                    else if (roll < 95)
                        return WeatherType.Storm;
                    else
                        return WeatherType.Fog;

                case Season.Autumn:
                    // Autumn: Foggy and rainy
                    // 40% Clear, 30% Rain, 20% Fog, 10% Storm
                    if (roll < 40)
                        return WeatherType.Clear;
                    else if (roll < 70)
                        return WeatherType.Rain;
                    else if (roll < 90)
                        return WeatherType.Fog;
                    else
                        return WeatherType.Storm;

                case Season.Winter:
                    // Winter: Snow and fog
                    // 30% Clear, 10% Rain, 40% Snow, 15% Fog, 5% Storm
                    if (roll < 30)
                        return WeatherType.Clear;
                    else if (roll < 40)
                        return WeatherType.Rain;
                    else if (roll < 80)
                        return WeatherType.Snow;
                    else if (roll < 95)
                        return WeatherType.Fog;
                    else
                        return WeatherType.Storm;

                default:
                    return WeatherType.Clear;
            }
        }

        /// <summary>
        /// Gets seasonal weather for Mountain regions.
        /// </summary>
        private static WeatherType GetSeasonalWeatherForMountain(int roll)
        {
            switch (CurrentSeason)
            {
                case Season.Spring:
                    // Spring: Mixed weather
                    // 30% Clear, 20% Rain, 30% Snow, 20% Fog
                    if (roll < 30)
                        return WeatherType.Clear;
                    else if (roll < 50)
                        return WeatherType.Rain;
                    else if (roll < 80)
                        return WeatherType.Snow;
                    else
                        return WeatherType.Fog;

                case Season.Summer:
                    // Summer: Less snow, more clear
                    // 50% Clear, 20% Rain, 10% Snow, 20% Fog
                    if (roll < 50)
                        return WeatherType.Clear;
                    else if (roll < 70)
                        return WeatherType.Rain;
                    else if (roll < 80)
                        return WeatherType.Snow;
                    else
                        return WeatherType.Fog;

                case Season.Autumn:
                    // Autumn: Foggy
                    // 30% Clear, 20% Rain, 20% Snow, 30% Fog
                    if (roll < 30)
                        return WeatherType.Clear;
                    else if (roll < 50)
                        return WeatherType.Rain;
                    else if (roll < 70)
                        return WeatherType.Snow;
                    else
                        return WeatherType.Fog;

                case Season.Winter:
                    // Winter: Mostly snow
                    // 20% Clear, 5% Rain, 60% Snow, 15% Fog
                    if (roll < 20)
                        return WeatherType.Clear;
                    else if (roll < 25)
                        return WeatherType.Rain;
                    else if (roll < 85)
                        return WeatherType.Snow;
                    else
                        return WeatherType.Fog;

                default:
                    return WeatherType.Snow;
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
        // TODO: Add storm damage or travel delays
        // TODO: Add seasonal events (harvest festival, snow day)
        // TODO: Add region-based microclimates
        // TODO: Add regional temperature data
        // TODO: Add crop growth or festival triggers per season
    }
}
