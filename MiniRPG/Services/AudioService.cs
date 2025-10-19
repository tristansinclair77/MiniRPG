using System;
using System.IO;
using System.Media;

namespace MiniRPG.Services
{
    public static class AudioService
    {
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
        // TODO: // Replace with cross-fade audio engine later.
        // TODO: // Add smooth crossfade and environmental sound effects later
        // TODO: Add dynamic volume crossfades and weather ambience later
    }
}
