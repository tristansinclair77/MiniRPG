using System.Collections.Generic;
using System.Linq;
using MiniRPG.Models;

namespace MiniRPG.Services
{
    /// <summary>
    /// Static service for managing crafting recipes and crafting operations.
    /// </summary>
    public static class CraftingService
    {
        /// <summary>
        /// Returns all available crafting recipes.
        /// </summary>
        /// <returns>List of all crafting recipes</returns>
        public static List<CraftingRecipe> GetAllRecipes()
        {
            var recipes = new List<CraftingRecipe>();

            // Iron Sword recipe
            var ironSwordMaterials = new Dictionary<string, int>
            {
                { "Iron Ore", 2 },
                { "Wood", 1 }
            };
            var ironSword = new Item("Iron Sword", "A sturdy blade forged from iron.", "Weapon", 100)
            {
                IsEquippable = true,
                SlotType = "Weapon",
                AttackBonus = 5
            };
            recipes.Add(new CraftingRecipe(
                "Iron Sword",
                "Forge a sturdy iron sword from ore and wood.",
                ironSword,
                ironSwordMaterials,
                50
            ));

            // Health Potion recipe
            var healthPotionMaterials = new Dictionary<string, int>
            {
                { "Herb", 1 },
                { "Bottle", 1 }
            };
            var healthPotion = new Item("Health Potion", "Restores 20 HP", "Consumable", 30);
            recipes.Add(new CraftingRecipe(
                "Health Potion",
                "Brew a healing potion from herbs and a bottle.",
                healthPotion,
                healthPotionMaterials,
                10
            ));

            // Steel Armor recipe
            var steelArmorMaterials = new Dictionary<string, int>
            {
                { "Steel Ingot", 3 },
                { "Leather", 2 }
            };
            var steelArmor = new Item("Steel Armor", "Heavy armor providing excellent protection.", "Armor", 200)
            {
                IsEquippable = true,
                SlotType = "Armor",
                DefenseBonus = 8
            };
            recipes.Add(new CraftingRecipe(
                "Steel Armor",
                "Craft sturdy steel armor with leather padding.",
                steelArmor,
                steelArmorMaterials,
                100
            ));

            // Mana Potion recipe
            var manaPotionMaterials = new Dictionary<string, int>
            {
                { "Magic Crystal", 1 },
                { "Bottle", 1 }
            };
            var manaPotion = new Item("Mana Potion", "Restores magical energy", "Consumable", 40);
            recipes.Add(new CraftingRecipe(
                "Mana Potion",
                "Distill magical essence into a usable potion.",
                manaPotion,
                manaPotionMaterials,
                15
            ));

            // Lucky Charm recipe
            var luckyCharmMaterials = new Dictionary<string, int>
            {
                { "Silver Coin", 5 },
                { "Four-Leaf Clover", 1 }
            };
            var luckyCharm = new Item("Lucky Charm", "Increases fortune and luck.", "Accessory", 80)
            {
                IsEquippable = true,
                SlotType = "Accessory",
                DefenseBonus = 2
            };
            recipes.Add(new CraftingRecipe(
                "Lucky Charm",
                "Create a charm that brings good fortune.",
                luckyCharm,
                luckyCharmMaterials,
                25
            ));

            // TODO: Add recipe discovery system and rarity progression

            return recipes;
        }

        /// <summary>
        /// Returns all crafting recipes filtered by the result item's type (category).
        /// </summary>
        /// <param name="category">The item type to filter by (e.g., "Weapon", "Consumable", "Armor", "Accessory")</param>
        /// <returns>List of recipes that produce items of the specified type</returns>
        public static List<CraftingRecipe> GetRecipesByCategory(string category)
        {
            var allRecipes = GetAllRecipes();
            return allRecipes.Where(r => r.ResultItem.Type == category).ToList();
        }
    }
}
