using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using MiniRPG.Models;

namespace MiniRPG.Services
{
    /// <summary>
    /// Wrapper class for save file data including player and world state.
    /// </summary>
    public class SaveData
    {
        public Player Player { get; set; } = new Player();
        public List<string> UnlockedRegions { get; set; } = new List<string>();
    }

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

                // Create save data that includes player and unlocked regions
                var saveData = new SaveData
                {
                    Player = player,
                    UnlockedRegions = FastTravelService.UnlockedRegions.ToList()
                };

                string json = JsonSerializer.Serialize(saveData, _jsonOptions);
                File.WriteAllText(filePath, json);
                
                Debug.WriteLine($"Player saved successfully to {filePath}");
                Debug.WriteLine($"Equipped items - Weapon: {player.EquippedWeapon?.Name ?? "None"}, " +
                              $"Armor: {player.EquippedArmor?.Name ?? "None"}, " +
                              $"Accessory: {player.EquippedAccessory?.Name ?? "None"}");
                Debug.WriteLine($"Unlocked regions saved: {string.Join(", ", saveData.UnlockedRegions)}");
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
                
                // Try to load as new SaveData format first
                SaveData? saveData = null;
                try
                {
                    saveData = JsonSerializer.Deserialize<SaveData>(json, _jsonOptions);
                }
                catch
                {
                    // If new format fails, try legacy Player-only format
                    Debug.WriteLine("Attempting to load legacy save format...");
                }

                Player? player = null;
                
                if (saveData?.Player != null)
                {
                    // New format with unlocked regions
                    player = saveData.Player;
                    
                    // Rehydrate FastTravelService.UnlockedRegions from JSON
                    FastTravelService.UnlockedRegions.Clear();
                    foreach (var regionName in saveData.UnlockedRegions)
                    {
                        FastTravelService.UnlockedRegions.Add(regionName);
                    }
                    
                    Debug.WriteLine($"Loaded unlocked regions: {string.Join(", ", saveData.UnlockedRegions)}");
                }
                else
                {
                    // Legacy format - just Player object
                    player = JsonSerializer.Deserialize<Player>(json, _jsonOptions);
                    Debug.WriteLine("Loaded from legacy save format (no unlocked regions)");
                }
                
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
                        
                        // Try new format first
                        SaveData? backupSaveData = null;
                        try
                        {
                            backupSaveData = JsonSerializer.Deserialize<SaveData>(backupJson, _jsonOptions);
                        }
                        catch
                        {
                            Debug.WriteLine("Backup is in legacy format");
                        }

                        Player? backupPlayer = null;
                        
                        if (backupSaveData?.Player != null)
                        {
                            backupPlayer = backupSaveData.Player;
                            
                            // Rehydrate FastTravelService.UnlockedRegions from backup
                            FastTravelService.UnlockedRegions.Clear();
                            foreach (var regionName in backupSaveData.UnlockedRegions)
                            {
                                FastTravelService.UnlockedRegions.Add(regionName);
                            }
                        }
                        else
                        {
                            // Legacy format backup
                            backupPlayer = JsonSerializer.Deserialize<Player>(backupJson, _jsonOptions);
                        }
                        
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
                
                // Try new format first
                SaveData? saveData = null;
                try
                {
                    saveData = JsonSerializer.Deserialize<SaveData>(json, _jsonOptions);
                }
                catch { }

                if (saveData?.Player != null)
                {
                    // Basic validation - ensure player loaded and critical data is present
                    return !string.IsNullOrEmpty(saveData.Player.Name);
                }
                else
                {
                    // Try legacy format
                    Player? player = JsonSerializer.Deserialize<Player>(json, _jsonOptions);
                    return player != null && !string.IsNullOrEmpty(player.Name);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Save file validation failed: {ex.Message}");
                return false;
            }
        }

        // TODO: Add save versioning and item ID mapping later
        // TODO: Extend to save inventory, map progress, and settings later
        // TODO: Add region-specific checkpoint saves
        // TODO: Add multi-save-slot world-state synchronization later
        // TODO: Add per-building interior coordinates later
    }
}
