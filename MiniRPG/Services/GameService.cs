using System;
using System.Collections.Generic;
using MiniRPG.Models;

namespace MiniRPG.Services
{
    /// <summary>
    /// Provides game-related utility methods.
    /// </summary>
    public static class GameService
    {
        private static readonly List<string> _enemies = new() { "Slime", "Goblin", "Wolf", "Dwarf", "Bandit" };
        private static readonly Random _random = new();

        /// <summary>
        /// Returns a random enemy name from the list.
        /// </summary>
        public static string GetRandomEnemy()
        {
            int idx = _random.Next(_enemies.Count);
            return _enemies[idx];
        }

        /// <summary>
        /// Returns a random enemy name based on the region.
        /// </summary>
        /// <param name="regionName">The name of the region to get enemies from</param>
        /// <returns>A random enemy name appropriate for the region</returns>
        public static string GetRandomEnemy(string regionName)
        {
            List<string> regionalEnemies;

            if (regionName == "Slime Plains")
            {
                regionalEnemies = new List<string> { "Slime", "Big Slime" };
            }
            else if (regionName == "Goblin Woods")
            {
                regionalEnemies = new List<string> { "Goblin", "Goblin Chief" };
            }
            else if (regionName == "Bandit Hideout")
            {
                regionalEnemies = new List<string> { "Bandit", "Bandit Leader" };
            }
            else
            {
                // Fallback to default enemy list for unknown regions
                regionalEnemies = _enemies;
            }

            int idx = _random.Next(regionalEnemies.Count);
            return regionalEnemies[idx];
            
            // TODO: Add regional difficulty scaling and boss encounters
        }

        /// <summary>
        /// Gets the faction affiliation for an enemy type.
        /// Returns null if the enemy has no faction affiliation.
        /// </summary>
        /// <param name="enemyName">The name of the enemy</param>
        /// <returns>The faction name for the enemy, or null if not affiliated</returns>
        public static string? GetEnemyFaction(string enemyName)
        {
            if (enemyName.Contains("Bandit", StringComparison.OrdinalIgnoreCase))
            {
                return "Bandit Clan";
            }
            // Add more enemy-to-faction mappings as needed
            // Slimes and Goblins are not part of any faction
            return null;
        }

        /// <summary>
        /// Returns a random damage value (1-10).
        /// </summary>
        public static int CalculateDamage()
        {
            return _random.Next(1, 11);
        }

        /// <summary>
        /// Returns a random loot item based on drop rates:
        /// 20% chance: Weapon (Wooden Sword)
        /// 20% chance: Armor (Leather Armor)
        /// 60% chance: Consumable or Material (Potion, Slime Goo)
        /// </summary>
        public static Item? GetRandomLoot()
        {
            double roll = _random.NextDouble();

            if (roll < 0.05) // 5% chance for weapon
            {
                return new Item("Bitch-nasty Sword", "A fuck-ass sick nasty weapon.", "Weapon", 5552)
                {
                    IsEquippable = true,
                    SlotType = "Weapon",
                    AttackBonus = 1300
                };
            }
            else if (roll < 0.2) // 20% chance for weapon
            {
                return new Item("Wooden Sword", "A basic starter weapon.", "Weapon", 0)
                {
                    IsEquippable = true,
                    SlotType = "Weapon",
                    AttackBonus = 3
                };
            }
            else if (roll < 0.4) // 20% chance for armor (0.2 to 0.4)
            {
                double chance = _random.NextDouble();
                if (chance < 0.95)
                {
                    return new Item("Titty Armor", "The best protection, if you know what I mean.", "Armor", 0)
                    {
                        IsEquippable = true,
                        SlotType = "Armor",
                        DefenseBonus = -500
                    };
                }
                else
                {
                    return new Item("Leather Armor", "Simple protection made of hide.", "Armor", 0)
                    {
                        IsEquippable = true,
                        SlotType = "Armor",
                        DefenseBonus = 1
                    };
                }
            }
            else if (roll < 1.0) // 60% chance for consumable/material (0.4 to 1.0)
            {
                // Randomly choose between consumable and material
                if (_random.NextDouble() < 0.7) // 70% of the 60% = ~42% overall chance for potion
                {
                    return new Item("Potion", "Restores 10 HP", "Consumable", 25);
                }
                else // 30% of the 60% = ~18% overall chance for slime goo
                {
                    return new Item("Slime Goo", "Sticky residue from a defeated slime.", "Material", 5);
                }
            }
            
            // This should never happen with the current logic, but return null as fallback
            return null;
            // TODO: Later - tie loot tables to enemy type and area difficulty
        }

        // TODO: Expand with enemy stats, player stats, battle rewards
    }
}
