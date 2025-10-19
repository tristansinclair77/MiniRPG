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

        // Region tracking for save/load
        private string? _lastRegionName;
        public string? LastRegionName
        {
            get => _lastRegionName;
            set { _lastRegionName = value; OnPropertyChanged(); }
        }

        // Building tracking for save/load
        private string? _lastBuildingName;
        public string? LastBuildingName
        {
            get => _lastBuildingName;
            set { _lastBuildingName = value; OnPropertyChanged(); }
        }

        // Inventory management
        private int _maxInventoryCapacity = 40;
        public int MaxInventoryCapacity
        {
            get => _maxInventoryCapacity;
            set { _maxInventoryCapacity = value; OnPropertyChanged(); OnPropertyChanged(nameof(InventoryCount)); }
        }

        public int InventoryCount => Inventory?.Count(item => item != null) ?? 0;

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

        private int _gold = 100;
        public int Gold
        {
            get => _gold;
            set { _gold = value; OnPropertyChanged(); }
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

        public ObservableCollection<Item?> Inventory { get; set; } = new();

        // Quest system
        public ObservableCollection<Quest> ActiveQuests { get; set; } = new();
        public ObservableCollection<Quest> CompletedQuests { get; set; } = new();

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
            Gold = 100;
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
            if (Inventory.Count >= MaxInventoryCapacity)
            {
                Debug.WriteLine($"{item.Name} could not be collected: Inventory Full!");
                return;
            }
            Inventory.Add(item);
            OnPropertyChanged(nameof(InventoryCount));
            Debug.WriteLine($"{item.Name} added to inventory.");
            // TODO: // Add inventory capacity and sorting later
        }

        public void RemoveItem(Item item)
        {
            if (Inventory.Remove(item))
            {
                OnPropertyChanged(nameof(InventoryCount));
            }
        }

        public void AddGold(int amount) => Gold += amount;

        public bool SpendGold(int amount)
        {
            if (Gold >= amount)
            {
                Gold -= amount;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Adds a quest to the player's active quest list.
        /// </summary>
        /// <param name="quest">The quest to add</param>
        public void AddQuest(Quest quest) => ActiveQuests.Add(quest);

        /// <summary>
        /// Completes a quest, moving it to completed quests and awarding rewards.
        /// </summary>
        /// <param name="quest">The quest to complete</param>
        public void CompleteQuest(Quest quest)
        {
            // Move quest from ActiveQuests to CompletedQuests
            if (ActiveQuests.Contains(quest))
            {
                ActiveQuests.Remove(quest);
                CompletedQuests.Add(quest);
                
                // Check if quest expired
                if (quest.IsExpired())
                {
                    // Award reduced or no rewards for expired quest
                    Debug.WriteLine($"Quest '{quest.Title}' completed but expired! No rewards given.");
                }
                else
                {
                    // Award full rewards
                    AddGold(quest.RewardGold);
                    GainExperience(quest.RewardExp);
                    
                    if (quest.RewardItem != null)
                    {
                        AddItem(quest.RewardItem);
                    }
                    
                    // Log reward message
                    Debug.WriteLine($"Quest '{quest.Title}' completed! Rewards: {quest.RewardGold} gold, {quest.RewardExp} experience" +
                        (quest.RewardItem != null ? $", {quest.RewardItem.Name}" : ""));
                }
            }
        }

        public void NotifyInventoryChanged()
        {
            OnPropertyChanged(nameof(InventoryCount));
        }

        public void CompactInventory()
        {
            for (int i = Inventory.Count - 1; i >= 0; i--)
            {
                if (Inventory[i] == null)
                {
                    Inventory.RemoveAt(i);
                }
            }
            NotifyInventoryChanged();
        }

        // TODO: Later - add quest categories (Main, Side, Daily)
        // TODO: Add currency icons and multi-currency support later
        // TODO: // Add unequip logic and visual equipment preview later
    }
}
