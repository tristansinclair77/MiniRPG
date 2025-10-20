using System.Collections.ObjectModel;
using MiniRPG.Services;

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
        /// The hour of the day when this NPC becomes available (0-23).
        /// </summary>
        public int AvailableStartHour { get; set; }
        
        /// <summary>
        /// The hour of the day when this NPC becomes unavailable (0-23).
        /// </summary>
        public int AvailableEndHour { get; set; }
        
        /// <summary>
        /// The current location of the NPC (e.g., "Town Square", "Home", "Shop").
        /// </summary>
        public string CurrentLocation { get; set; }
        
        /// <summary>
        /// The faction affiliation of this NPC. Defaults to region's faction if unspecified.
        /// </summary>
        public string FactionAffiliation { get; set; }
        
        /// <summary>
        /// The amount of reputation gained with the NPC's faction when completing their quest.
        /// </summary>
        public int ReputationImpactOnQuestCompletion { get; set; } = 5;

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
            CurrentLocation = string.Empty;
            FactionAffiliation = string.Empty;
        }
        
        /// <summary>
        /// Checks if the NPC is currently available based on the current game time.
        /// </summary>
        /// <returns>True if the current hour is within the NPC's available hours, false otherwise.</returns>
        public bool IsAvailableNow()
        {
            return TimeService.Hour >= AvailableStartHour && TimeService.Hour < AvailableEndHour;
        }

        // Example NPC usage:
        // new NPC("Mira", "QuestGiver", "Good to see you!")
        // {
        //     AvailableStartHour = 8,
        //     AvailableEndHour = 20,
        //     CurrentLocation = "Market Square",
        //     DialogueLines = new() { "Slimes have been attacking!", "Please defeat 3 of them." },
        //     OfferedQuest = new Quest("Slime Hunt", "Defeat 3 Slimes for Mira.", 3, 50, 20)
        // };

        // TODO: Add weekly schedules and holiday routines later
        // TODO: Add portrait, voice, and branching dialogue options later
        // TODO: Add faction emblems and alignment dialogue filters
    }
}
