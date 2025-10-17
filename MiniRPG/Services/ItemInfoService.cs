using System.Collections.Generic;
using MiniRPG.Models;

namespace MiniRPG.Services
{
    /// <summary>
    /// Service for managing item information and descriptions.
    /// Provides centralized item data lookup for UI display.
    /// </summary>
    public static class ItemInfoService
    {
        private static readonly Dictionary<string, string> ItemDescriptions = new Dictionary<string, string>
        {
            { "Potion", "Restores 10 HP" },
            { "Slime Goo", "Sticky residue from a defeated slime." },
            { "Wooden Sword", "A basic starter weapon." },
            { "Leather Armor", "Simple protection made of hide." },
            { "Iron Sword", "A sturdy iron blade that deals good damage." },
            { "Steel Armor", "Well-crafted armor providing solid defense." }
        };

        /// <summary>
        /// Gets the description for a specific item by name.
        /// Returns a default message if item is not found.
        /// </summary>
        /// <param name="itemName">The name of the item</param>
        /// <returns>The item description</returns>
        public static string GetItemDescription(string itemName)
        {
            if (ItemDescriptions.TryGetValue(itemName, out string? description))
            {
                return description;
            }
            return "No description available.";
        }

        /// <summary>
        /// Adds or updates an item description in the lookup table.
        /// </summary>
        /// <param name="itemName">The name of the item</param>
        /// <param name="description">The item description</param>
        public static void AddOrUpdateItemDescription(string itemName, string description)
        {
            ItemDescriptions[itemName] = description;
        }

        // TODO: Add item rarity information
        // TODO: Add item stat comparison utilities
    }
}
