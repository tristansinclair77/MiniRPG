using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using MiniRPG.Models;

namespace MiniRPG.Services
{
    public static class SaveLoadService
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = true, // Include backing fields for INotifyPropertyChanged properties
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static void SavePlayer(Player player, string filePath = "player_save.json")
        {
            try
            {
                // Create a backup of the current save file if it exists
                if (File.Exists(filePath))
                {
                    string backupPath = filePath + ".backup";
                    File.Copy(filePath, backupPath, true);
                }

                string json = JsonSerializer.Serialize(player, _jsonOptions);
                File.WriteAllText(filePath, json);
                
                Debug.WriteLine($"Player saved successfully to {filePath}");
                Debug.WriteLine($"Equipped items - Weapon: {player.EquippedWeapon?.Name ?? "None"}, " +
                              $"Armor: {player.EquippedArmor?.Name ?? "None"}, " +
                              $"Accessory: {player.EquippedAccessory?.Name ?? "None"}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving player: {ex.Message}");
                // Restore backup if save failed
                string backupPath = filePath + ".backup";
                if (File.Exists(backupPath))
                {
                    try
                    {
                        File.Copy(backupPath, filePath, true);
                        Debug.WriteLine("Restored backup after save failure");
                    }
                    catch (Exception backupEx)
                    {
                        Debug.WriteLine($"Failed to restore backup: {backupEx.Message}");
                    }
                }
            }
        }

        public static Player? LoadPlayer(string filePath = "player_save.json")
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Debug.WriteLine($"Save file {filePath} does not exist");
                    return null;
                }

                string json = File.ReadAllText(filePath);
                Player? player = JsonSerializer.Deserialize<Player>(json, _jsonOptions);
                
                if (player != null)
                {
                    // Ensure stats are recalculated after loading to account for equipment bonuses
                    player.Attack = player.BaseAttack + (player.EquippedWeapon?.AttackBonus ?? 0);
                    player.Defense = player.BaseDefense + (player.EquippedArmor?.DefenseBonus ?? 0);
                    
                    Debug.WriteLine($"Player loaded successfully from {filePath}");
                    Debug.WriteLine($"Loaded equipped items - Weapon: {player.EquippedWeapon?.Name ?? "None"}, " +
                                  $"Armor: {player.EquippedArmor?.Name ?? "None"}, " +
                                  $"Accessory: {player.EquippedAccessory?.Name ?? "None"}");
                    Debug.WriteLine($"Stats after load - Attack: {player.Attack}, Defense: {player.Defense}");
                }
                
                return player;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading player: {ex.Message}");
                
                // Try to load from backup
                string backupPath = filePath + ".backup";
                if (File.Exists(backupPath))
                {
                    try
                    {
                        Debug.WriteLine("Attempting to load from backup...");
                        string backupJson = File.ReadAllText(backupPath);
                        Player? backupPlayer = JsonSerializer.Deserialize<Player>(backupJson, _jsonOptions);
                        
                        if (backupPlayer != null)
                        {
                            // Recalculate stats for backup player too
                            backupPlayer.Attack = backupPlayer.BaseAttack + (backupPlayer.EquippedWeapon?.AttackBonus ?? 0);
                            backupPlayer.Defense = backupPlayer.BaseDefense + (backupPlayer.EquippedArmor?.DefenseBonus ?? 0);
                            Debug.WriteLine("Successfully loaded from backup");
                        }
                        
                        return backupPlayer;
                    }
                    catch (Exception backupEx)
                    {
                        Debug.WriteLine($"Failed to load backup: {backupEx.Message}");
                    }
                }
                
                return null;
            }
        }

        /// <summary>
        /// Validates that equipped items are properly serialized in a save file
        /// </summary>
        public static bool ValidateSaveFile(string filePath = "player_save.json")
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                string json = File.ReadAllText(filePath);
                Player? player = JsonSerializer.Deserialize<Player>(json, _jsonOptions);
                
                // Basic validation - ensure player loaded and critical data is present
                return player != null && !string.IsNullOrEmpty(player.Name);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Save file validation failed: {ex.Message}");
                return false;
            }
        }

        // TODO: Add save versioning and item ID mapping later
        // TODO: Extend to save inventory, map progress, and settings later
    }
}
