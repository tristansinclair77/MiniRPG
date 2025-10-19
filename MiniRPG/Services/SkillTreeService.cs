using System.Collections.Generic;
using MiniRPG.Models;

namespace MiniRPG.Services
{
    /// <summary>
    /// Provides skill tree data and skill management functionality.
    /// </summary>
    public static class SkillTreeService
    {
        /// <summary>
        /// Returns a list of all available skills in the game.
        /// </summary>
        public static List<Skill> GetAllSkills()
        {
            return new List<Skill>
            {
                new Skill
                {
                    Name = "Power Strike",
                    Description = "A powerful melee attack that deals bonus damage.",
                    Power = 5,
                    LevelRequirement = 2,
                    CostSP = 1,
                    EffectType = "Attack",
                    IsPassive = false,
                    IsUnlocked = false
                },
                new Skill
                {
                    Name = "Iron Skin",
                    Description = "Hardens your skin, permanently increasing defense.",
                    Power = 5,
                    LevelRequirement = 3,
                    CostSP = 1,
                    EffectType = "Defense",
                    IsPassive = true,
                    IsUnlocked = false
                },
                new Skill
                {
                    Name = "Healing Light",
                    Description = "Channel healing energy to restore HP.",
                    Power = 15,
                    LevelRequirement = 4,
                    CostSP = 2,
                    EffectType = "Heal",
                    IsPassive = false,
                    IsUnlocked = false
                }
            };
            
            // TODO: Add skill tree tier data and class specialization later
        }
    }
}
