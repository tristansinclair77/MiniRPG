using System.Collections.Generic;
using MiniRPG.Services;

namespace MiniRPG.Models
{
    public class Item
    {
        private string _name;
        private string _description;
        
        public string Name 
        { 
            get => _name;
            set 
            { 
                _name = value;
                // Auto-update description from service if not explicitly set
                if (string.IsNullOrEmpty(_description))
                {
                    _description = ItemInfoService.GetItemDescription(value);
                }
            }
        }
        
        public string Description 
        { 
            get => string.IsNullOrEmpty(_description) ? ItemInfoService.GetItemDescription(Name) : _description;
            set => _description = value;
        }
        
        public string Type { get; set; } // e.g. "Consumable", "Material", "Weapon"
        public int Value { get; set; } // gold or sell price
        public bool IsEquippable { get; set; } = false;
        public string SlotType { get; set; } // "Weapon", "Armor", "Accessory"
        public int AttackBonus { get; set; } = 0;
        public int DefenseBonus { get; set; } = 0;

        public Item(string name, string description, string type, int value)
        {
            _name = name;
            _description = description;
            Type = type;
            Value = value;
            
            // Register description in ItemInfoService if provided
            if (!string.IsNullOrEmpty(description))
            {
                ItemInfoService.AddOrUpdateItemDescription(name, description);
            }
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
