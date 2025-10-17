using System.Collections.Generic;
using System.Linq;
using MiniRPG.Models;

namespace MiniRPG.Services
{
    /// <summary>
    /// Provides quest-related utility methods and quest management.
    /// </summary>
    public static class QuestService
    {
        /// <summary>
        /// Returns a list of available quests that players can undertake.
        /// </summary>
        /// <returns>List of available Quest objects</returns>
        public static List<Quest> GetAvailableQuests()
        {
            return new List<Quest>
            {
                new Quest(
                    "Slime Hunt",
                    "Defeat 3 Slimes terrorizing the outskirts.",
                    3, // requiredKills
                    50, // rewardGold
                    20, // rewardExp
                    new Item("Potion", "A healing potion as reward", "Consumable", 25) // rewardItem
                ),
                new Quest(
                    "Goblin Problem",
                    "The local goblins have become too aggressive. Eliminate 5 of them to restore peace.",
                    5, // requiredKills
                    100, // rewardGold
                    40, // rewardExp
                    new Item("Iron Sword", "A sturdy iron blade.", "Weapon", 0) // rewardItem
                    {
                        IsEquippable = true,
                        SlotType = "Weapon",
                        AttackBonus = 4
                    }
                )
            };
        }

        /// <summary>
        /// Finds a quest by its title from the available quests list.
        /// </summary>
        /// <param name="title">The title of the quest to find</param>
        /// <returns>The Quest object if found, otherwise null</returns>
        public static Quest? FindQuestByTitle(string title)
        {
            var availableQuests = GetAvailableQuests();
            return availableQuests.FirstOrDefault(quest => quest.Title.Equals(title, System.StringComparison.OrdinalIgnoreCase));
        }

        // TODO: Add dynamic quest generation based on region and player level
        // TODO: Add quest chain progression
    }
}