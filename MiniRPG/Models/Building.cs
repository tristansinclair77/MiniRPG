using System.Collections.ObjectModel;

namespace MiniRPG.Models
{
    /// <summary>
    /// Represents a building in the game world.
    /// </summary>
    public class Building
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } // e.g. "Shop", "Inn", "House", "Guild"
        public ObservableCollection<NPC> Occupants { get; set; }

        /// <summary>
        /// Constructor for creating a new building with basic information.
        /// </summary>
        /// <param name="name">The building's name</param>
        /// <param name="description">The building's description</param>
        /// <param name="type">The building's type (e.g., "Shop", "Inn", "House", "Guild")</param>
        public Building(string name, string description, string type)
        {
            Name = name;
            Description = description;
            Type = type;
            Occupants = new ObservableCollection<NPC>();
        }

        // Example usage:
        // new Building("Mira's Home", "A cozy cottage where Mira lives.", "House");

        // TODO: Add building art, entry coordinates, and special functions later
    }
}
