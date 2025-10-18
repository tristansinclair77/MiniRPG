using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MiniRPG.Models;

namespace MiniRPG.Services
{
    /// <summary>
    /// Service for managing NPCs and their dialogue.
    /// Provides centralized NPC data and dialogue lookup.
    /// </summary>
    public static class DialogueService
    {
        /// <summary>
        /// Gets a list of all NPCs in the game.
        /// </summary>
        /// <returns>A list containing all NPCs (Mira, Shopkeeper, Guard)</returns>
        public static List<NPC> GetAllNPCs()
        {
            var npcs = new List<NPC>();

            // Mira - Quest Giver
            var mira = new NPC("Mira", "QuestGiver", "Oh! Adventurer, can you help me?")
            {
                DialogueLines = new ObservableCollection<string>
                {
                    "Slimes have been attacking our village!",
                    "Please defeat 3 of them.",
                    "I'll reward you handsomely if you help us."
                },
                OfferedQuest = new Quest("Slime Hunt", "Defeat 3 Slimes for Mira.", 3, 50, 20)
            };
            npcs.Add(mira);

            // Shopkeeper - Merchant
            var shopkeeper = new NPC("Shopkeeper", "Merchant", "Welcome to my shop! Looking to buy or sell?")
            {
                DialogueLines = new ObservableCollection<string>
                {
                    "I have the finest goods in town!",
                    "Check out my wares, adventurer.",
                    "Come back anytime!"
                }
            };
            npcs.Add(shopkeeper);

            // Guard - Villager
            var guard = new NPC("Guard", "Villager", "Halt! State your business.")
            {
                DialogueLines = new ObservableCollection<string>
                {
                    "We keep this village safe from monsters.",
                    "Be careful out there, it's dangerous beyond the walls.",
                    "The captain is always looking for brave adventurers."
                }
            };
            npcs.Add(guard);

            return npcs;
        }

        /// <summary>
        /// Gets the dialogue lines for a specific NPC by name.
        /// </summary>
        /// <param name="name">The name of the NPC</param>
        /// <returns>The NPC's dialogue lines, or null if NPC not found</returns>
        public static ObservableCollection<string>? GetDialogueForNPC(string name)
        {
            var npc = GetAllNPCs().FirstOrDefault(n => n.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase));
            return npc?.DialogueLines;
        }

        // TODO: Add branching dialogue and choices later
        // TODO: Add relationship/affection system
    }
}
