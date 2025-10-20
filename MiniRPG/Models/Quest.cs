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
        public int ReputationReward { get; set; } = 5;
        
        /// <summary>
        /// The day on which this quest expires (null = no expiry).
        /// </summary>
        public int? ExpireDay { get; set; }

        /// <summary>
        /// The NPC who gave this quest (optional).
        /// </summary>
        public NPC? QuestGiver { get; set; }

        /// <summary>
        /// The region where this quest was obtained (optional).
        /// </summary>
        public Region? Region { get; set; }

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

        /// <summary>
        /// Checks if the quest has expired based on the current day.
        /// </summary>
        /// <returns>True if the quest has expired, false otherwise</returns>
        public bool IsExpired() => ExpireDay.HasValue && Services.TimeService.Day > ExpireDay.Value;

        // TODO: Add quest types: Fetch, Deliver, Explore
        // TODO: Add quest giver NPC info
        // TODO: Add quest rewards summary UI
        // TODO: Add storyline quest chains
        // TODO: Add quest completion cutscenes
        // TODO: Add quest timers displayed on quest board
    }
}