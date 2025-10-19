namespace MiniRPG.Models
{
    /// <summary>
    /// Represents a faction in the game world with reputation tracking.
    /// </summary>
    public class Faction
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Reputation { get; set; }       // -100 (enemy) to +100 (ally)
        public int ThresholdFriendly { get; set; } = 50;
        public int ThresholdHostile { get; set; } = -50;

        /// <summary>
        /// Constructor for creating a new Faction with basic information.
        /// </summary>
        /// <param name="name">The faction's name</param>
        /// <param name="description">Description of the faction</param>
        public Faction(string name, string description)
        {
            Name = name;
            Description = description;
            Reputation = 0;  // Start neutral
        }

        /// <summary>
        /// Adjusts the faction reputation by the specified amount.
        /// Reputation is clamped between -100 and +100.
        /// </summary>
        /// <param name="amount">The amount to adjust reputation by (positive or negative)</param>
        public void AdjustReputation(int amount)
        {
            Reputation = Math.Clamp(Reputation + amount, -100, 100);
        }

        /// <summary>
        /// Gets the current standing with this faction based on reputation.
        /// </summary>
        /// <returns>"Allied", "Neutral", or "Hostile" based on reputation thresholds</returns>
        public string GetStanding()
        {
            if (Reputation >= ThresholdFriendly)
            {
                return "Allied";
            }
            else if (Reputation <= ThresholdHostile)
            {
                return "Hostile";
            }
            else
            {
                return "Neutral";
            }
        }

        // TODO: Add icons, territory ownership, and alliances later
    }
}
