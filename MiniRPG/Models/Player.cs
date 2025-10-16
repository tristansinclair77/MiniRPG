using System.Collections.ObjectModel;
using System.Diagnostics;
using MiniRPG.ViewModels;

namespace MiniRPG.Models
{
    /// <summary>
    /// Represents the player character.
    /// </summary>
    public class Player : BaseViewModel
    {
        public string Name { get; set; }

        private int _hp;
        public int HP
        {
            get => _hp;
            set { _hp = value; OnPropertyChanged(); }
        }

        private int _maxHP;
        public int MaxHP
        {
            get => _maxHP;
            set { _maxHP = value; OnPropertyChanged(); }
        }

        private int _level = 1;
        public int Level
        {
            get => _level;
            set { _level = value; OnPropertyChanged(); }
        }

        private int _experience = 0;
        public int Experience
        {
            get => _experience;
            set { _experience = value; OnPropertyChanged(); }
        }

        private int _experienceToNextLevel = 10;
        public int ExperienceToNextLevel
        {
            get => _experienceToNextLevel;
            set { _experienceToNextLevel = value; OnPropertyChanged(); }
        }

        // Base stats (original values before equipment bonuses)
        public int BaseAttack { get; set; }
        public int BaseDefense { get; set; }

        // Calculated stats (base + equipment bonuses)
        private int _attack;
        public int Attack
        {
            get => _attack;
            set { _attack = value; OnPropertyChanged(); }
        }

        private int _defense;
        public int Defense
        {
            get => _defense;
            set { _defense = value; OnPropertyChanged(); }
        }

        // Equipment slots
        private Item? _equippedWeapon;
        public Item? EquippedWeapon
        {
            get => _equippedWeapon;
            set { _equippedWeapon = value; OnPropertyChanged(); }
        }

        private Item? _equippedArmor;
        public Item? EquippedArmor
        {
            get => _equippedArmor;
            set { _equippedArmor = value; OnPropertyChanged(); }
        }

        private Item? _equippedAccessory;
        public Item? EquippedAccessory
        {
            get => _equippedAccessory;
            set { _equippedAccessory = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Item> Inventory { get; set; } = new();

        public Player(string name = "Hero")
        {
            Name = name;
            MaxHP = 30;
            HP = MaxHP;
            BaseAttack = 5;
            BaseDefense = 2;
            Attack = BaseAttack;
            Defense = BaseDefense;
            Level = 1;
            Experience = 0;
            ExperienceToNextLevel = 10;
            // TODO: Future - include inventory, experience, and leveling system
        }

        public bool EquipItem(Item item)
        {
            if (!item.IsEquippable)
                return false;

            switch (item.SlotType)
            {
                case "Weapon":
                    EquippedWeapon = item;
                    break;
                case "Armor":
                    EquippedArmor = item;
                    break;
                case "Accessory":
                    EquippedAccessory = item;
                    break;
                default:
                    return false;
            }

            // Recalculate effective stats
            Attack = BaseAttack + (EquippedWeapon?.AttackBonus ?? 0);
            Defense = BaseDefense + (EquippedArmor?.DefenseBonus ?? 0);

            return true;
        }

        public bool GainExperience(int amount)
        {
            Experience += amount;
            bool leveledUp = false;
            while (Experience >= ExperienceToNextLevel)
            {
                Level++;
                Experience -= ExperienceToNextLevel;
                ExperienceToNextLevel = (int)(ExperienceToNextLevel * 1.5);
                MaxHP += 5;
                BaseAttack += 1;
                BaseDefense += 1;
                // Recalculate effective stats after leveling up
                Attack = BaseAttack + (EquippedWeapon?.AttackBonus ?? 0);
                Defense = BaseDefense + (EquippedArmor?.DefenseBonus ?? 0);
                HP = MaxHP;
                leveledUp = true;
                // TODO: Later - play level-up animation, add ability points, and save to file
            }
            return leveledUp;
        }

        public void AddItem(Item item)
        {
            Inventory.Add(item);
            Debug.WriteLine($"{item.Name} added to inventory.");
            // TODO: // Add inventory capacity and sorting later
        }

        // TODO: // Add unequip logic and visual equipment preview later
    }
}
