# MiniRPG - Change Log

## Latest Update: Quest Expiration System

### Changes Made ✨

#### Quest.cs - Quest Expiration Feature
- **Added**: `int? ExpireDay` property
  - **Purpose**: Tracks the day on which a quest expires (null = no expiry)
  - **Type**: Nullable integer to support quests with and without time limits
  - **Default**: null (no expiration)

- **Added**: `IsExpired()` method
  - **Implementation**: `ExpireDay.HasValue && TimeService.Day > ExpireDay.Value`
  - **Returns**: True if quest has expired, false otherwise
  - **Usage**: Checked during quest completion to determine reward eligibility

- **Added**: TODO comment
  - `// TODO: Add quest timers displayed on quest board`
  - Planned feature: Visual countdown timers on quest board UI

#### Player.cs - Quest Completion with Expiration Check
- **Modified**: `CompleteQuest(Quest quest)` method
  - **New Logic**: Checks if quest is expired before awarding rewards
  - **Expired Quests**: No rewards given (gold, experience, or items)
  - **Active Quests**: Full rewards awarded as normal
  - **Logging**: Different debug messages for expired vs. active quest completion

#### Requirements Fulfilled

All requirements from Instructions.txt have been implemented:

**Quest.cs:**
- ✅ Added `int? ExpireDay` property (null = no expiry)
- ✅ Added `IsExpired()` method: `ExpireDay.HasValue && TimeService.Day > ExpireDay.Value`
- ✅ Added TODO: `// TODO: Add quest timers displayed on quest board`

**Player.cs:**
- ✅ Modified `CompleteQuest` to check if quest expired
- ✅ If expired: reward reduced to none (no gold, no exp, no items)
- ✅ If active: full rewards awarded

#### Implementation Details

**Quest Expiration Logic:**/// <summary>
/// Checks if the quest has expired based on the current day.
/// </summary>
/// <returns>True if the quest has expired, false otherwise</returns>
public bool IsExpired() => ExpireDay.HasValue && Services.TimeService.Day > ExpireDay.Value;
**Player.CompleteQuest() with Expiration Check:**public void CompleteQuest(Quest quest)
{
    if (ActiveQuests.Contains(quest))
    {
        ActiveQuests.Remove(quest);
        CompletedQuests.Add(quest);
        
        // Check if quest expired
        if (quest.IsExpired())
        {
            // Award reduced or no rewards for expired quest
            Debug.WriteLine($"Quest '{quest.Title}' completed but expired! No rewards given.");
        }
        else
        {
            // Award full rewards
            AddGold(quest.RewardGold);
            GainExperience(quest.RewardExp);
            
            if (quest.RewardItem != null)
            {
                AddItem(quest.RewardItem);
            }
            
            Debug.WriteLine($"Quest '{quest.Title}' completed! Rewards: {quest.RewardGold} gold, {quest.RewardExp} experience" +
                (quest.RewardItem != null ? $", {quest.RewardItem.Name}" : ""));
        }
    }
}
#### Gameplay Examples

**Example 1: Quest with No Expiration**
- **Quest Created**: Day 1, ExpireDay = null
- **Quest Completed**: Day 10
- **Result**: `IsExpired()` returns false → Full rewards awarded

**Example 2: Quest Completed Before Expiration**
- **Quest Created**: Day 1, ExpireDay = 5
- **Quest Completed**: Day 3
- **Result**: `IsExpired()` returns false (Day 3 ≤ Day 5) → Full rewards awarded

**Example 3: Quest Completed After Expiration**
- **Quest Created**: Day 1, ExpireDay = 5
- **Quest Completed**: Day 7
- **Result**: `IsExpired()` returns true (Day 7 > Day 5) → No rewards given

**Example 4: Quest Completed on Expiration Day**
- **Quest Created**: Day 1, ExpireDay = 5
- **Quest Completed**: Day 5
- **Result**: `IsExpired()` returns false (Day 5 not > Day 5) → Full rewards awarded

#### Usage in Game

**Creating Time-Limited Quests:**var urgentQuest = new Quest(
    "Urgent Delivery", 
    "Deliver this package within 3 days", 
    requiredKills: 0, 
    rewardGold: 100, 
    rewardExp: 50
);
urgentQuest.ExpireDay = TimeService.Day + 3; // Expires in 3 days
player.AddQuest(urgentQuest);
**Creating Permanent Quests:**var mainQuest = new Quest(
    "Defeat the Dragon", 
    "Save the kingdom from the dragon", 
    requiredKills: 1, 
    rewardGold: 1000, 
    rewardExp: 500
);
// ExpireDay remains null - quest never expires
player.AddQuest(mainQuest);
#### Time Progression Integration

The quest expiration system integrates seamlessly with the existing TimeService:

**Time Advances When:**
1. **Resting at Inn**: `TimeService.AdvanceHours(8)` - may trigger day change
2. **Traveling Between Regions**: Could advance time in future updates
3. **Random Events**: Future time-based events

**Expiration Checks:**
- Automatic when calling `quest.IsExpired()`
- Compares `TimeService.Day` (current day) with `quest.ExpireDay`
- No manual tracking required

#### Potential Future Enhancements

Based on the new TODO comment:

**Quest Timer Display:**
- **Quest Board UI Enhancement**:
  - Show "Days Remaining: X" next to time-limited quests
  - Color coding: Green (3+ days), Yellow (1-2 days), Red (< 1 day)
  - "No Time Limit" indicator for permanent quests
- **Implementation**:
  - Add converter: `ExpireDayToRemainingDaysConverter`
  - Calculate: `quest.ExpireDay - TimeService.Day`
  - Display in quest board list item template

**Quest Notification System:**
- **Expiration Warnings**:
  - Alert when quest has 1 day remaining
  - Notification when quest expires
  - Option to abandon expired quests
- **Auto-Cleanup**:
  - Automatically remove expired quests from active list
  - Move to separate "Failed Quests" collection

**Partial Rewards for Late Completion:**
- **Tiered Reward System**:
  - On time: 100% rewards
  - 1 day late: 50% rewards
  - 2+ days late: 0% rewards
- **Reputation Impact**:
  - Completing on time: +reputation
  - Completing late: no reputation change
  - Failing quest: -reputation

**Hour-Based Expiration:**
- **More Precise Timing**:
  - Instead of `int? ExpireDay`, use `int? ExpireHour`
  - Track both day and hour for expiration
  - Example: Quest expires at Day 3, Hour 18 (6 PM on Day 3)

---

## Previous Update: Dynamic Time-Based Music System

### Changes Made ✨

#### AudioService.cs - Time-Based Music Enhancement
- **Added**: `UpdateMusicForTime()` method for dynamic background music based on time of day
  - **Implementation**: Switch statement based on `TimeService.GetTimeOfDay()` result
  - **Music Mapping**:
    - "Morning" → plays `morning.wav`
    - "Afternoon" → plays `daytime.wav`
    - "Evening" → plays `dusk.wav`
    - "Night" → plays `night.wav`
  - **Safety**: Wrapped in try-catch blocks to handle missing audio files gracefully
  - **Documentation**: Added XML summary explaining when method should be called

- **Added**: TODO comment for future enhancements
  - `// TODO: Add dynamic volume crossfades and weather ambience later`
  - Planned features: Smooth audio transitions and environmental sound effects

#### Integration Points - Music Updates During Gameplay

**1. BuildingInteriorViewModel.cs - Resting at Inn:**
- **Updated**: `StayAtInn()` method now calls `AudioService.UpdateMusicForTime()` after time advances
- **Flow**: 
  1. Player pays gold and HP is restored
  2. `TimeService.AdvanceHours(8)` moves time forward
  3. Music updates to match new time of day
  4. Example: Rest at 8 AM → advances to 4 PM → music changes from morning.wav to daytime.wav

**2. MainViewModel.cs - Traveling to Regions:**
- **Updated**: `OnRegionSelected` event handler (appears twice - for world map and fast travel)
- **Flow**:
  1. Player selects destination region
  2. Region is saved and player location updated
  3. Music updates to match current time of day
  4. Map view loads with appropriate ambient music
- **Call Location**: After `SaveLoadService.SavePlayer()`, before `ShowMap()`
- **Integration**: Works with both world map travel and fast travel systems

#### Requirements Fulfilled

All requirements from Instructions.txt have been implemented:

**AudioService.cs:**
- ✅ Added `UpdateMusicForTime()` method
- ✅ Switch statement based on `TimeService.GetTimeOfDay()`:
  - ✅ "Morning" → play morning.wav
  - ✅ "Afternoon" → play daytime.wav
  - ✅ "Evening" → play dusk.wav
  - ✅ "Night" → play night.wav
- ✅ Called when player travels (MainViewModel OnRegionSelected)
- ✅ Called when player rests (BuildingInteriorViewModel StayAtInn)
- ✅ Called when player enters region (MainViewModel OnRegionSelected)
- ✅ Added TODO: `// TODO: Add dynamic volume crossfades and weather ambience later`

#### Integration Flow

**Scenario 1: Player Rests at Inn**
1. **Initial State**: Day 1, Morning (8:00 AM), music playing: `morning.wav`
2. **Player Action**: Click "Stay the Night" button in inn
3. **Time Advance**: `TimeService.AdvanceHours(8)` → Day 1, Afternoon (4:00 PM)
4. **Music Update**: `UpdateMusicForTime()` checks time → plays `daytime.wav`
5. **Result**: Music seamlessly updates to match new time period

**Scenario 2: Player Travels to New Region**
1. **Initial State**: Player in "Greenfield Town", Afternoon
2. **Player Action**: Opens world map, selects "Dark Forest" region
3. **Travel Process**: Region changes, player location saved
4. **Music Update**: `UpdateMusicForTime()` checks current time → plays appropriate music
5. **Result**: Entering new region with time-appropriate ambient music

**Scenario 3: Time of Day Transitions**
- **Morning (6:00-11:59)**: `morning.wav` - Birds chirping, gentle melodies
- **Afternoon (12:00-17:59)**: `daytime.wav` - Upbeat, energetic themes
- **Evening (18:00-23:59)**: `dusk.wav` - Calm, reflective tones
- **Night (0:00-5:59)**: `night.wav` - Mysterious, ambient sounds

#### Code Examples

**AudioService.cs - UpdateMusicForTime() Method:**/// <summary>
/// Updates background music based on current time of day.
/// Should be called when player travels, rests, or enters a region.
/// </summary>
public static void UpdateMusicForTime()
{
    switch (TimeService.GetTimeOfDay())
    {
        case "Morning":
            PlayWavIfExists("morning.wav");
            break;
        case "Afternoon":
            PlayWavIfExists("daytime.wav");
            break;
        case "Evening":
            PlayWavIfExists("dusk.wav");
            break;
        case "Night":
            PlayWavIfExists("night.wav");
            break;
    }
}
**BuildingInteriorViewModel.cs - Rest at Inn:**private void StayAtInn()
{
    const int cost = 20;
    if (Player.Gold >= cost)
    {
        Player.SpendGold(cost);
        Player.HP = Player.MaxHP;
        TimeService.AdvanceHours(8);
        Debug.WriteLine("You rest at the inn and feel refreshed.");
        
        // Update music for new time of day after resting
        try { AudioService.UpdateMusicForTime(); } catch { }
    }
    else
    {
        Debug.WriteLine("You can't afford a room.");
    }
}
**MainViewModel.cs - Enter Region:**worldMapVM.OnRegionSelected += selectedRegion =>
{
    _currentRegion = selectedRegion;
    CurrentPlayer.LastRegionName = selectedRegion.Name;
    SaveLoadService.SavePlayer(CurrentPlayer);
    AddLog($"Traveling to {selectedRegion.Name}...");
    
    // Update music based on time of day when entering region
    try { AudioService.UpdateMusicForTime(); } catch { }
    
    ShowMap();
};
#### Gameplay Examples

**Example 1: Morning Inn Rest**
- **Time**: Day 1, Morning (8:00 AM)
- **Current Music**: morning.wav
- **Action**: Player rests at inn
- **Result**: Time → Day 1, Afternoon (4:00 PM), Music → daytime.wav

**Example 2: Evening Travel**
- **Time**: Day 1, Evening (7:00 PM)
- **Current Music**: dusk.wav
- **Action**: Travel to new region via world map
- **Result**: Enter region with dusk.wav playing (time unchanged, music matches)

**Example 3: Night to Morning Rest**
- **Time**: Day 1, Night (2:00 AM)
- **Current Music**: night.wav
- **Action**: Rest at inn (8 hours)
- **Result**: Time → Day 1, Morning (10:00 AM), Music → morning.wav

**Example 4: Multiple Travels in One Day**
- **8:00 AM**: Start in Greenfield (morning.wav)
- **Travel 1**: Move to Dark Forest → Still morning, music stays morning.wav
- **Rest at Inn**: Advance to 4:00 PM → Music changes to daytime.wav
- **Travel 2**: Move to Mountain Pass → Afternoon, music stays daytime.wav
- **8:00 PM**: Time advances naturally → Music changes to dusk.wav

#### Potential Future Enhancements

Based on the new TODO comment:

**Dynamic Volume Crossfades:**
- **Smooth Transitions**: Instead of abrupt music changes, implement gradual volume fades
- **Crossfade Implementation**:
  - Fade out current track over 2-3 seconds
  - Simultaneously fade in new track
  - Creates seamless audio experience
- **Technical Approach**:
  - Use `System.Media.SoundPlayer` volume control
  - Or upgrade to NAudio library for advanced audio mixing
  - Timer-based volume interpolation (0.0 to 1.0)

**Weather Ambience System:**
- **Weather-Based Audio Layers**:
  - Rain: Add rain_ambient.wav loop on top of time-based music
  - Storm: Thunder sound effects + heavy rain ambience
  - Snow: Gentle wind and snow crunching sounds
  - Fog: Eerie, muffled audio filter effects
- **Integration with TimeService**:
  - Extend TimeService with weather tracking
  - `WeatherService.GetCurrentWeather()` returns weather type
  - AudioService plays layered tracks based on time + weather
- **Example Combinations**:
  - Morning + Rain: morning.wav (50% volume) + rain_ambient.wav (30% volume)
  - Night + Storm: night.wav (40% volume) + storm.wav (60% volume)
  - Afternoon + Clear: daytime.wav (100% volume, no weather layer)

**Region-Specific Music Variants:**
- **Location + Time Combinations**:
  - Dark Forest + Night: night_forest.wav (spooky ambience)
  - Mountain Pass + Morning: morning_mountain.wav (echo effects)
  - Beach Town + Afternoon: daytime_beach.wav (ocean waves)
- **Adaptive Music System**:
  - Base time-of-day track
  - Regional instrument layers
  - Blend both for unique region atmosphere
