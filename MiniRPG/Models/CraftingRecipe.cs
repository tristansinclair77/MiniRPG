using System.Collections.Generic;
using System.Linq;

namespace MiniRPG.Models
{
    /// <summary>
    /// Represents a crafting recipe for creating items from materials and gold.
    /// </summary>
    public class CraftingRecipe
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Item ResultItem { get; set; }
        public Dictionary<string, int> RequiredMaterials { get; set; }
        public int RequiredGold { get; set; }

        public CraftingRecipe(string name, string description, Item resultItem, Dictionary<string, int> requiredMaterials, int requiredGold)
        {
            Name = name;
            Description = description;
            ResultItem = resultItem;
            RequiredMaterials = requiredMaterials;
            RequiredGold = requiredGold;
        }

        /// <summary>
        /// Checks if the player has all required materials and gold to craft this recipe.
        /// </summary>
        /// <param name="player">The player to check</param>
        /// <returns>True if the player can craft this recipe, false otherwise</returns>
        public bool CanCraft(Player player)
        {
            // Check if player has enough gold
            if (player.Gold < RequiredGold)
            {
                return false;
            }

            // Check if player has each required material in sufficient quantity
            foreach (var material in RequiredMaterials)
            {
                string materialName = material.Key;
                int requiredAmount = material.Value;
                int playerAmount = player.Inventory.Count(item => item != null && item.Name == materialName);

                if (playerAmount < requiredAmount)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Attempts to craft the recipe item. If successful, removes required materials and gold,
        /// and adds the result item to the player's inventory.
        /// </summary>
        /// <param name="player">The player performing the crafting</param>
        public void Craft(Player player)
        {
            if (!CanCraft(player))
            {
                return;
            }

            // Remove required materials from inventory
            foreach (var material in RequiredMaterials)
            {
                string materialName = material.Key;
                int requiredAmount = material.Value;

                for (int i = 0; i < requiredAmount; i++)
                {
                    var itemToRemove = player.Inventory.FirstOrDefault(item => item != null && item.Name == materialName);
                    if (itemToRemove != null)
                    {
                        player.RemoveItem(itemToRemove);
                    }
                }
            }

            // Remove required gold
            player.SpendGold(RequiredGold);

            // Add result item to inventory
            player.AddItem(ResultItem);

            // Log crafting success
            System.Diagnostics.Debug.WriteLine($"Crafted {ResultItem.Name}!");
        }

        // TODO: Add crafting time, rarity tiers, and skill-based bonuses later
    }
}
