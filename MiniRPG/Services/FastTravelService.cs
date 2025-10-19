using System.Collections.ObjectModel;
using System.Linq;

namespace MiniRPG.Services
{
    /// <summary>
    /// Service for managing fast travel between unlocked regions.
    /// Provides region unlock tracking and fast travel functionality.
    /// </summary>
    public static class FastTravelService
    {
        /// <summary>
        /// Collection of region names that the player has unlocked for fast travel.
        /// </summary>
        public static ObservableCollection<string> UnlockedRegions { get; } = new();

        /// <summary>
        /// Unlocks a region for fast travel if it hasn't been unlocked already.
        /// </summary>
        /// <param name="regionName">The name of the region to unlock</param>
        public static void UnlockRegion(string regionName)
        {
            if (!UnlockedRegions.Contains(regionName))
            {
                UnlockedRegions.Add(regionName);
            }
        }

        /// <summary>
        /// Checks if a region is unlocked for fast travel.
        /// </summary>
        /// <param name="regionName">The name of the region to check</param>
        /// <returns>True if the region is unlocked, false otherwise</returns>
        public static bool IsUnlocked(string regionName)
        {
            return UnlockedRegions.Contains(regionName);
        }

        /// <summary>
        /// Gets a copy of the list of unlocked regions.
        /// </summary>
        /// <returns>A new list containing all unlocked region names</returns>
        public static ObservableCollection<string> GetUnlockedRegions()
        {
            return new ObservableCollection<string>(UnlockedRegions.ToList());
        }

        // TODO: Add cost, cooldown, and transport types (airship, portal, carriage)
    }
}
