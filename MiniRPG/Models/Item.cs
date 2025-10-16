using System.Collections.Generic;

namespace MiniRPG.Models
{
    public class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } // e.g. "Consumable", "Material", "Weapon"
        public int Value { get; set; } // gold or sell price
        public bool IsEquippable { get; set; } = false;
        public string SlotType { get; set; } // "Weapon", "Armor", "Accessory"
        public int AttackBonus { get; set; } = 0;
        public int DefenseBonus { get; set; } = 0;

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
                new Item("Slime Goo", "Sticky residue from a defeated slime.", "Material", 5),
                new Item("Wooden Sword", "A basic starter weapon.", "Weapon", 0)
                {
                    IsEquippable = true,
                    SlotType = "Weapon",
                    AttackBonus = 2
                },
                new Item("Leather Armor", "Simple protection made of hide.", "Armor", 0)
                {
                    IsEquippable = true,
                    SlotType = "Armor",
                    DefenseBonus = 1
                }
            };
        }
        // TODO: // Add rarity, weight, and elemental affinity later
    }
}
