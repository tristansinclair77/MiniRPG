using System;
using System.IO;
using System.Media;

namespace MiniRPG.Services
{
    public static class AudioService
    {
        private static SoundPlayer? _ambientPlayer;

        static AudioService()
        {
            // Subscribe to lighting changes for dynamic ambient sound switching
            EnvironmentService.OnLightingChanged += OnLightingChanged;
        }

        /// <summary>
        /// Event handler for lighting changes. Switches ambient loop accordingly.
        /// </summary>
        private static void OnLightingChanged(object? sender, string lighting)
        {
            PlayAmbientForLighting(lighting);
        }

        /// <summary>
        /// Plays ambient sound based on the current lighting condition.
        /// </summary>
        /// <param name="lighting">The lighting state: "Daylight", "Twilight", or "Night"</param>
        public static void PlayAmbientForLighting(string lighting)
        {
            string ambientFile = lighting switch
            {
                "Daylight" => "birds.wav",
                "Twilight" => "crickets.wav",
                "Night" => "nightwind.wav",
                _ => "birds.wav"
            };

            PlayAmbientLoop(ambientFile);
        }

        /// <summary>
        /// Plays an ambient sound in a loop.
        /// </summary>
        private static void PlayAmbientLoop(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    _ambientPlayer?.Stop();
                    _ambientPlayer?.Dispose();
                    _ambientPlayer = new SoundPlayer(fileName);
                    _ambientPlayer.PlayLooping();
                }
            }
            catch (Exception ex)
            {
                // Optionally log or ignore
            }
        }

        public static void PlayTitleTheme()
        {
            PlayWavIfExists("title_theme.wav");
        }

        public static void PlayMapTheme()
        {
            PlayWavIfExists("map_theme.wav");
        }

        public static void PlayBattleTheme()
        {
            PlayWavIfExists("battle_theme.wav");
        }

        public static void PlayBuildingTheme(string buildingType)
        {
            switch (buildingType)
            {
                case "Inn":
                    PlayWavIfExists("inn.wav");
                    break;
                case "Shop":
                    PlayWavIfExists("shop.wav");
                    break;
                default:
                    PlayWavIfExists("interior.wav");
                    break;
            }
        }

        /// <summary>
        /// Plays the sleep sound effect when player rests at an inn.
        /// </summary>
        public static void PlaySleep()
        {
            PlayWavOnceIfExists("sleep.wav");
        }

        /// <summary>
        /// Updates background music based on current time of day.
        /// Should be called when player travels, rests, or enters a region.
        /// </summary>
        public static void UpdateMusicForTime()
        {
            switch (TimeService.GetTimeOfDay())
            {
                case "Morning":
                    PlayWavIfExists("morning.wav");
                    break;
                case "Afternoon":
                    PlayWavIfExists("daytime.wav");
                    break;
                case "Evening":
                    PlayWavIfExists("dusk.wav");
                    break;
                case "Night":
                    PlayWavIfExists("night.wav");
                    break;
            }
        }

        private static void PlayWavIfExists(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    using var player = new SoundPlayer(fileName);
                    player.PlayLooping();
                }
            }
            catch (Exception ex)
            {
                // Optionally log or ignore
            }
        }

        private static void PlayWavOnceIfExists(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    using var player = new SoundPlayer(fileName);
                    player.Play();
                }
            }
            catch (Exception ex)
            {
                // Optionally log or ignore
            }
        }
        
        // TODO: // Replace with cross-fade audio engine later.
        // TODO: // Add smooth crossfade and environmental sound effects later
        // TODO: Add dynamic volume crossfades and weather ambience later
        // TODO: Add layered environmental tracks (rain, crowd chatter, wind)
    }
}
