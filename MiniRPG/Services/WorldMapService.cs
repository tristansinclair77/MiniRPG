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

            // Get all NPCs from DialogueService
            var allNPCs = DialogueService.GetAllNPCs();
            var mira = allNPCs.Find(npc => npc.Name == "Mira")!;
            var shopkeeper = allNPCs.Find(npc => npc.Name == "Shopkeeper")!;

            // Create buildings with home assignments
            var miraHome = new Building("Mira's Home", "A cozy cottage where Mira lives.", "House")
            {
                Occupants = new ObservableCollection<NPC> { mira },
                IsHome = true
            };
            
            var generalShop = new Building("General Shop", "A well-stocked shop selling basic supplies and equipment.", "Shop")
            {
                Occupants = new ObservableCollection<NPC> { shopkeeper },
                IsHome = true
            };
            
            var inn = new Building("Inn", "A cozy inn where travelers can rest and recover.", "Inn");

            // Assign home buildings to NPCs and set CurrentLocation
            mira.CurrentLocation = miraHome.Name;
            shopkeeper.CurrentLocation = generalShop.Name;

            // Greenfield Town - Starting area with NPCs
            var greenfieldTown = new Region("Greenfield Town", "A quiet settlement surrounded by plains.")
            {
                NPCs = new ObservableCollection<NPC>
                {
                    mira,
                    shopkeeper
                },
                Buildings = new ObservableCollection<Building>
                {
                    generalShop,
                    inn,
                    miraHome
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

            // TODO: Add NPC bedtime dialogues and idle animations

            return regions;
        }

        // TODO: Add fast travel, region unlocks, and area difficulty scaling later
    }
}
