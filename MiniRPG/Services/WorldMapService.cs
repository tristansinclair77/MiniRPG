using System.Collections.Generic;
using System.Collections.ObjectModel;
using MiniRPG.Models;

namespace MiniRPG.Services
{
    /// <summary>
    /// Service for managing regions and world map data.
    /// Provides centralized region information and world map structure.
    /// </summary>
    public static class WorldMapService
    {
        /// <summary>
        /// Gets a list of all regions in the game world.
        /// </summary>
        /// <returns>A list containing all available regions</returns>
        public static List<Region> GetRegions()
        {
            var regions = new List<Region>();

            // Greenfield Town - Starting area with NPCs
            var greenfieldTown = new Region("Greenfield Town", "A quiet settlement surrounded by plains.")
            {
                NPCs = new ObservableCollection<NPC>
                {
                    DialogueService.GetAllNPCs().Find(npc => npc.Name == "Mira")!,
                    DialogueService.GetAllNPCs().Find(npc => npc.Name == "Shopkeeper")!
                }
            };
            regions.Add(greenfieldTown);

            // Slime Plains - Low-level enemy area
            var slimePlains = new Region("Slime Plains", "Open grasslands infested with slimes of all sizes.")
            {
                AvailableEnemies = new ObservableCollection<string>
                {
                    "Slime",
                    "Big Slime"
                }
            };
            regions.Add(slimePlains);

            // Goblin Woods - Mid-level enemy area
            var goblinWoods = new Region("Goblin Woods", "A dark forest inhabited by hostile goblin tribes.")
            {
                AvailableEnemies = new ObservableCollection<string>
                {
                    "Goblin",
                    "Goblin Chief"
                }
            };
            regions.Add(goblinWoods);

            return regions;
        }

        // TODO: Add fast travel, region unlocks, and area difficulty scaling later
    }
}
