namespace MiniRPG.Models
{
    /// <summary>
    /// Represents a quest that the player can undertake and complete.
    /// </summary>
    public class Quest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public int RequiredKills { get; set; }
        public int CurrentKills { get; set; }
        public int RewardGold { get; set; }
        public int RewardExp { get; set; }
        public Item? RewardItem { get; set; }

        /// <summary>
        /// Constructor for creating a new quest with kill requirements.
        /// </summary>
        /// <param name="title">The quest title</param>
        /// <param name="description">The quest description</param>
        /// <param name="requiredKills">Number of kills required to complete the quest</param>
        /// <param name="rewardGold">Gold reward for completing the quest</param>
        /// <param name="rewardExp">Experience reward for completing the quest</param>
        /// <param name="rewardItem">Optional item reward for completing the quest</param>
        public Quest(string title, string description, int requiredKills, int rewardGold, int rewardExp, Item? rewardItem = null)
        {
            Title = title;
            Description = description;
            RequiredKills = requiredKills;
            CurrentKills = 0;
            RewardGold = rewardGold;
            RewardExp = rewardExp;
            RewardItem = rewardItem;
            IsCompleted = false;
        }

        /// <summary>
        /// Checks if the quest should be marked as completed based on current progress.
        /// </summary>
        public void CheckProgress()
        {
            if (CurrentKills >= RequiredKills)
            {
                IsCompleted = true;
            }
        }

        // TODO: Add quest types: Fetch, Deliver, Explore
        // TODO: Add quest giver NPC info
    }
}