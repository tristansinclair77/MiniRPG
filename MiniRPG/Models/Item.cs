using System.Collections.Generic;

namespace MiniRPG.Models
{
    public class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } // e.g. "Consumable", "Material", "Weapon"
        public int Value { get; set; } // gold or sell price

        public Item(string name, string description, string type, int value)
        {
            Name = name;
            Description = description;
            Type = type;
            Value = value;
        }

        public static List<Item> GetSampleItems()
        {
            return new List<Item>
            {
                new Item("Potion", "Restores 10 HP", "Consumable", 25),
                new Item("Slime Goo", "Sticky residue from a defeated slime.", "Material", 5)
            };
        }
        // TODO: // Expand with item effects and rarity later
    }
}
