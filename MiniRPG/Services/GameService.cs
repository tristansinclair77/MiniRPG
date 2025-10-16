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
        private static readonly List<string> _enemies = new() { "Slime", "Goblin", "Wolf" };
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
        /// Returns a random damage value (1-10).
        /// </summary>
        public static int CalculateDamage()
        {
            return _random.Next(1, 11);
        }

        /// <summary>
        /// Returns a random loot item or null (50% chance).
        /// </summary>
        public static Item? GetRandomLoot()
        {
            var items = Item.GetSampleItems();
            if (_random.NextDouble() > 0.5)
                return items[_random.Next(items.Count)];
            return null;
            // TODO: Add rarity tiers and loot tables later
        }

        // TODO: Expand with enemy stats, player stats, battle rewards
    }
}
