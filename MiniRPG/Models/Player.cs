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

        public int Attack { get; set; }
        public int Defense { get; set; }

        public ObservableCollection<Item> Inventory { get; set; } = new();

        public Player(string name = "Hero")
        {
            Name = name;
            MaxHP = 30;
            HP = MaxHP;
            Attack = 5;
            Defense = 2;
            Level = 1;
            Experience = 0;
            ExperienceToNextLevel = 10;
            // TODO: Future - include inventory, experience, and leveling system
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
                Attack += 1;
                Defense += 1;
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
    }
}
