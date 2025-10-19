using System.Collections.ObjectModel;

namespace MiniRPG.Models
{
    /// <summary>
    /// Represents a region or location in the game world.
    /// </summary>
    public class Region
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ObservableCollection<NPC> NPCs { get; set; }
        public ObservableCollection<string> AvailableEnemies { get; set; }
        public ObservableCollection<Quest> LocalQuests { get; set; }

        /// <summary>
        /// Constructor for creating a new region with basic information.
        /// </summary>
        /// <param name="name">The region's name</param>
        /// <param name="description">The region's description</param>
        public Region(string name, string description)
        {
            Name = name;
            Description = description;
            NPCs = new ObservableCollection<NPC>();
            AvailableEnemies = new ObservableCollection<string>();
            LocalQuests = new ObservableCollection<Quest>();
        }

        // Example usage:
        // new Region("Greenfield Town", "A quiet settlement surrounded by plains.")

        // TODO: Add travel cost, difficulty, background art, and music theme later
    }
}
