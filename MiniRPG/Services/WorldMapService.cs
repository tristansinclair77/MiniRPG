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
                FactionName = "Greenfield Town Faction",
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
            
            // Set NPC faction affiliation to region's faction if unspecified
            if (string.IsNullOrEmpty(mira.FactionAffiliation))
                mira.FactionAffiliation = greenfieldTown.FactionName;
            if (string.IsNullOrEmpty(shopkeeper.FactionAffiliation))
                shopkeeper.FactionAffiliation = greenfieldTown.FactionName;
            
            // Set the region on NPC quests
            if (mira.OfferedQuest != null)
                mira.OfferedQuest.Region = greenfieldTown;
            
            regions.Add(greenfieldTown);

            // Slime Plains - Low-level enemy area
            var slimePlains = new Region("Slime Plains", "Open grasslands infested with slimes of all sizes.")
            {
                FactionName = "Slime Plains Faction",
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
                FactionName = "Goblin Woods Faction",
                AvailableEnemies = new ObservableCollection<string>
                {
                    "Goblin",
                    "Goblin Chief"
                }
            };
            regions.Add(goblinWoods);

            // Bandit Hideout - Enemy area controlled by Bandit Clan
            var banditHideout = new Region("Bandit Hideout", "A dangerous outpost controlled by bandits and outlaws.")
            {
                FactionName = "Bandit Clan",
                AvailableEnemies = new ObservableCollection<string>
                {
                    "Bandit",
                    "Bandit Leader"
                }
            };
            regions.Add(banditHideout);

            // TODO: Add NPC bedtime dialogues and idle animations
            // TODO: Add unique faction quests
            // TODO: Add dynamic faction wars
            // TODO: Add story-altering reputation thresholds

            return regions;
        }

        // TODO: Add fast travel, region unlocks, and area difficulty scaling later
    }
}
