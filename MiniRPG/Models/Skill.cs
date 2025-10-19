using MiniRPG.ViewModels;

namespace MiniRPG.Models
{
    /// <summary>
    /// Represents a skill that can be learned and used by the player.
    /// </summary>
    public class Skill : BaseViewModel
    {
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        private int _levelRequirement;
        public int LevelRequirement
        {
            get => _levelRequirement;
            set { _levelRequirement = value; OnPropertyChanged(); }
        }

        private int _costSP;
        /// <summary>
        /// Skill Points required to unlock this skill.
        /// </summary>
        public int CostSP
        {
            get => _costSP;
            set { _costSP = value; OnPropertyChanged(); }
        }

        private int _power;
        /// <summary>
        /// Damage or bonus magnitude of the skill.
        /// </summary>
        public int Power
        {
            get => _power;
            set { _power = value; OnPropertyChanged(); }
        }

        private bool _isPassive;
        public bool IsPassive
        {
            get => _isPassive;
            set { _isPassive = value; OnPropertyChanged(); }
        }

        private bool _isUnlocked;
        public bool IsUnlocked
        {
            get => _isUnlocked;
            set { _isUnlocked = value; OnPropertyChanged(); }
        }

        private string _effectType = string.Empty;
        /// <summary>
        /// Type of effect: "Attack", "Defense", "Heal", "Buff", etc.
        /// </summary>
        public string EffectType
        {
            get => _effectType;
            set { _effectType = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Applies the skill's effect to the player and optionally to a target enemy.
        /// </summary>
        /// <param name="player">The player using the skill</param>
        /// <param name="target">The target enemy (null for passive/self-targeted skills)</param>
        /// <returns>Damage or healing value (0 for passive skills)</returns>
        public int ApplyEffect(Player player, string? target)
        {
            if (IsPassive)
            {
                // Modify player stats for passive skills
                switch (EffectType)
                {
                    case "Attack":
                        player.Attack += Power;
                        break;
                    case "Defense":
                        player.Defense += Power;
                        break;
                    case "Buff":
                        // Generic buff - could increase multiple stats
                        player.Attack += Power / 2;
                        player.Defense += Power / 2;
                        break;
                }
                return 0; // Passive skills don't return damage/healing
            }
            else
            {
                // Active skills return damage or healing value
                switch (EffectType)
                {
                    case "Attack":
                        return Power + player.Attack; // Damage to enemy
                    case "Heal":
                        int healAmount = Power;
                        player.HP = Math.Min(player.HP + healAmount, player.MaxHP);
                        return healAmount;
                    case "Defense":
                        // Temporary defense boost (returns 0 as it's a buff)
                        return 0;
                    default:
                        return Power; // Default damage/effect value
                }
            }
        }

        // TODO: Add skill animations and icons later
        // TODO: Add skill cooldown timers later
    }
}
