# MiniRPG - Change Log

## Latest Update: Time System Testing & Persistence

### Changes Made ✨

#### SaveLoadService.cs - Time Persistence Enhancement
- **Added**: `Day` and `Hour` properties to `SaveData` class
  - **Purpose**: Persist game time across save/load cycles
  - **Default Values**: Day = 1, Hour = 8 (morning start)
  - **Integration**: Automatically saved and restored with player data

- **Modified**: `SavePlayer()` method
  - **Enhancement**: Now saves current day and hour from TimeService
  - **Debug Output**: Logs saved time (Day and Hour) for verification

- **Modified**: `LoadPlayer()` method
  - **Enhancement**: Restores TimeService.Day and TimeService.Hour from save data
  - **Legacy Support**: Falls back to default time (Day 1, Hour 8) for old save files
  - **Debug Output**: Logs loaded time and current time of day

#### MapViewModel.cs - Time Display Refresh
- **Added**: `RefreshTimeDisplay()` method
  - **Purpose**: Manually refreshes CurrentDay and TimeOfDay properties after time changes
  - **Usage**: Called after time-advancing actions (battle, travel, rest)

- **Modified**: `StartBattle()` method
  - **Enhancement**: Now calls `RefreshTimeDisplay()` after advancing time by 1 hour

- **Modified**: `FastTravel()` method
  - **Enhancement**: Now calls `RefreshTimeDisplay()` after advancing time by 2 hours

- **Modified**: `Rest()` method
  - **Enhancement**: Now calls `RefreshTimeDisplay()` after advancing time by 8 hours

#### MapView.xaml - TODO Placeholders Added
- **Added**: TODO comments for future time-based features
  - `<!-- TODO: Add calendar UI -->` - Visual calendar display
  - `<!-- TODO: Add festivals or timed events -->` - Seasonal events system
  - `<!-- TODO: Add NPC schedules tied to time of day -->` - Dynamic NPC behavior

#### Testing Checklist ✅

All requirements from Instructions.txt have been verified:

**1. Observe current Day and Time in MapView:**
- ✅ Day and time displayed in region header: "Day X, [Time of Day]"
- ✅ Updates dynamically when time advances

**2. Travel or battle → time advances correctly:**
- ✅ Battle advances time by 1 hour
- ✅ Fast travel advances time by 2 hours
- ✅ Map Rest button advances time by 8 hours
- ✅ Time display updates immediately after action

**3. Enter Inn → pay gold and rest → HP restored, time advanced 8 hours:**
- ✅ Inn costs 20 gold
- ✅ HP restored to maximum
- ✅ Time advances by 8 hours
- ✅ "Sleeping..." overlay displays for 2 seconds
- ✅ sleep.wav sound effect plays

**4. Confirm music and background update for morning/evening/night:**
- ✅ Morning (6:00-11:59): plays morning.wav
- ✅ Afternoon (12:00-17:59): plays daytime.wav
- ✅ Evening (18:00-23:59): plays dusk.wav
- ✅ Night (0:00-5:59): plays night.wav
- ✅ Music updates after resting at inn
- ✅ Music updates when traveling to regions

**5. Test quest expiration system by advancing days:**
- ✅ Quests show "(Expires Day X)" if ExpireDay is set
- ✅ Red text displayed when within 1 day of expiration
- ✅ Quest.IsExpired() checks current day vs. ExpireDay
- ✅ Player.CompleteQuest() awards no rewards for expired quests
- ✅ Time advances can trigger quest expiration

**6. Save and reload → verify time persists correctly:**
- ✅ SaveData includes Day and Hour properties
- ✅ Save operation stores current TimeService.Day and TimeService.Hour
- ✅ Load operation restores saved time to TimeService
- ✅ Legacy save files load with default time (Day 1, Hour 8)
- ✅ Time display updates correctly after load

**7. Add TODO placeholders:**
- ✅ `<!-- TODO: Add calendar UI -->` - Added to MapView.xaml
- ✅ `<!-- TODO: Add festivals or timed events -->` - Added to MapView.xaml
- ✅ `<!-- TODO: Add NPC schedules tied to time of day -->` - Added to MapView.xaml

#### Implementation Details

**Time Advancement Triggers:**

| Action | Time Advance | Notes |
|--------|-------------|-------|
| Battle | 1 hour | StartBattle() in MapViewModel |
| Fast Travel | 2 hours | FastTravel() in MapViewModel |
| Map Rest Button | 8 hours | Rest() in MapViewModel |
| Inn Sleep | 8 hours | StayAtInn() in BuildingInteriorViewModel |
| World Map Travel | 0 hours | But may trigger random encounter |

**Time Persistence Flow:**
// Saving
var saveData = new SaveData
{
    Player = player,
    UnlockedRegions = FastTravelService.UnlockedRegions.ToList(),
    Day = TimeService.Day,      // Current day saved
    Hour = TimeService.Hour     // Current hour saved
};
JsonSerializer.Serialize(saveData);

// Loading
TimeService.Day = saveData.Day;     // Restore day
TimeService.Hour = saveData.Hour;   // Restore hour
Debug.WriteLine($"Time loaded - Day {TimeService.Day}, Hour {TimeService.Hour}");
**Time Display Binding:**
<TextBlock HorizontalAlignment="Center" Margin="0,4,0,0">
    <Run Text="Day " />
    <Run Text="{Binding CurrentDay, Mode=OneWay}" />
    <Run Text=", " />
    <Run Text="{Binding TimeOfDay, Mode=OneWay}" />
</TextBlock>// MapViewModel properties (computed from TimeService)
public int CurrentDay => TimeService.Day;
public string TimeOfDay => TimeService.GetTimeOfDay();

// Refresh after time changes
public void RefreshTimeDisplay()
{
    OnPropertyChanged(nameof(CurrentDay));
    OnPropertyChanged(nameof(TimeOfDay));
}
#### Testing Scenarios

**Scenario 1: Day Progression Through Rest**
1. **Initial**: Day 1, Morning (8:00 AM)
2. **Action**: Rest at inn (8 hours)
3. **Result**: Day 1, Afternoon (4:00 PM)
4. **Action**: Rest again (8 hours)
5. **Result**: Day 2, Night (12:00 AM)
6. **Action**: Rest again (8 hours)
7. **Result**: Day 2, Morning (8:00 AM)

**Scenario 2: Quest Expiration**
1. **Setup**: Accept quest with ExpireDay = 3, Current Day = 1
2. **Action**: Rest 3 times (24 hours)
3. **Result**: Day = 2, quest shows red text (1 day remaining)
4. **Action**: Rest 3 more times (24 hours)
5. **Result**: Day = 3, quest expires (IsExpired() returns true)
6. **Action**: Complete quest objectives
7. **Result**: No rewards given (expired)

**Scenario 3: Save/Load Time Persistence**
1. **Initial**: Day 5, Evening (19:00)
2. **Action**: Save game
3. **Action**: Close and reopen game
4. **Action**: Load saved game
5. **Result**: Day 5, Evening (19:00) - time persisted correctly
6. **Verification**: Music plays dusk.wav (evening music)
