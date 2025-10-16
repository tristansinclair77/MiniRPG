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
    }
}
