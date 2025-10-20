using System.Collections.Generic;
using System.Linq;
using MiniRPG.Models;

namespace MiniRPG.Services
{
    /// <summary>
    /// Provides faction data and faction management functionality.
    /// </summary>
    public static class FactionService
    {
        /// <summary>
        /// Returns a list of all available factions in the game.
        /// </summary>
        public static List<Faction> GetAllFactions()
        {
            return new List<Faction>
            {
                new Faction("Greenfield Town", "A peaceful farming settlement where villagers live simple lives, tending to crops and livestock."),
                new Faction("Adventurer's Guild", "A neutral quest hub where adventurers gather to accept quests, trade stories, and seek glory."),
                new Faction("Bandit Clan", "An antagonistic faction of outlaws and raiders who prey on travelers and settlements.")
            };
        }

        /// <summary>
        /// Retrieves a faction by its name from the available factions.
        /// </summary>
        /// <param name="name">The name of the faction to retrieve</param>
        /// <returns>The faction with the specified name, or null if not found</returns>
        public static Faction? GetFactionByName(string name)
        {
            return GetAllFactions().FirstOrDefault(f => f.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase));
        }

        // TODO: Add faction alliances, rivalries, and reputation decay over time
    }
}
