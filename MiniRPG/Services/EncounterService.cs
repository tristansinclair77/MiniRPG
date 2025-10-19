using System;

namespace MiniRPG.Services
{
    /// <summary>
    /// Service for managing random encounters and encounter logic.
    /// </summary>
    public static class EncounterService
    {
        private static readonly Random _random = new();

        /// <summary>
        /// Determines if an encounter should be triggered.
        /// </summary>
        /// <returns>True if an encounter should be triggered (25% chance), false otherwise</returns>
        public static bool ShouldTriggerEncounter()
        {
            return _random.NextDouble() < 0.25; // 25% chance
        }

        /// <summary>
        /// Gets a random enemy for an encounter based on the region.
        /// </summary>
        /// <param name="regionName">The name of the region where the encounter occurs</param>
        /// <returns>The name of the enemy to encounter</returns>
        public static string GetEncounterEnemy(string regionName)
        {
            return GameService.GetRandomEnemy(regionName);
        }

        // Add encounter rarity tiers and enemy groups later
        // Add roaming miniboss encounters
    }
}
