using System.Collections.ObjectModel;

namespace MiniRPG.Models
{
    /// <summary>
    /// Represents a non-player character (NPC) in the game world.
    /// </summary>
    public class NPC
    {
        public string Name { get; set; }
        public string Role { get; set; } // e.g. "Villager", "Merchant", "QuestGiver"
        public string Greeting { get; set; }
        public ObservableCollection<string> DialogueLines { get; set; }
        public Quest? OfferedQuest { get; set; }

        /// <summary>
        /// Constructor for creating a new NPC with basic information.
        /// </summary>
        /// <param name="name">The NPC's name</param>
        /// <param name="role">The NPC's role (e.g., "Villager", "Merchant", "QuestGiver")</param>
        /// <param name="greeting">The NPC's initial greeting message</param>
        public NPC(string name, string role, string greeting)
        {
            Name = name;
            Role = role;
            Greeting = greeting;
            DialogueLines = new ObservableCollection<string>();
            OfferedQuest = null;
        }

        // Example NPC usage:
        // new NPC("Mira", "QuestGiver", "Oh! Adventurer, can you help me?")
        // {
        //     DialogueLines = new() { "Slimes have been attacking!", "Please defeat 3 of them." },
        //     OfferedQuest = new Quest("Slime Hunt", "Defeat 3 Slimes for Mira.", 3, 50, 20)
        // };

        // TODO: Add portrait, voice, and branching dialogue options later
    }
}
