using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using MiniRPG.Models;

namespace MiniRPG.Services
{
    public static class SaveLoadService
    {
        public static void SavePlayer(Player player, string filePath = "player_save.json")
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(player, options);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving player: {ex.Message}");
            }
        }

        public static Player? LoadPlayer(string filePath = "player_save.json")
        {
            try
            {
                if (!File.Exists(filePath))
                    return null;
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<Player>(json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading player: {ex.Message}");
                return null;
            }
        }
        // TODO: Extend to save inventory, map progress, and settings later
    }
}
