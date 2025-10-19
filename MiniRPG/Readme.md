# MiniRPG - Change Log

## Latest Update: Dynamic NPC Scheduling & Time-Based Appearance System

### Changes Made ✨

#### MapViewModel.cs - Dynamic NPC Refresh on Time Changes
- **Enhanced**: Constructor methods now filter NPCs by availability
  - **Legacy Constructor**: Filters `DialogueService.GetAllNPCs()` using `IsAvailableNow()`
  - **Region Constructor**: Filters `region.NPCs` using `IsAvailableNow()`
  - **Purpose**: Only show NPCs that are currently available based on their schedules

- **Added**: `_region` private field (Region?)
  - **Purpose**: Store reference to region for refreshing NPCs when time changes
  - **Usage**: Used in `RefreshNearbyNPCs()` to access full NPC list

- **Added**: `_lastHour` private field (int)
  - **Purpose**: Track the last hour to detect significant time changes
  - **Usage**: Compared in `RefreshTimeDisplay()` to trigger NPC refresh

- **Enhanced**: `RefreshTimeDisplay()` method
  - **New Behavior**: Now checks if hour has changed and calls `RefreshNearbyNPCs()`
  - **Purpose**: Automatically update NPC list when time advances
  - **Usage**: Called after battle, travel, rest, or any time-advancing action

- **Added**: `RefreshNearbyNPCs()` private method
  - **Purpose**: Refresh the NearbyNPCs list based on current time
  - **Implementation**: 
    - Compares previous NPCs with currently available NPCs
    - Logs appearance/disappearance messages for each change
    - Updates NearbyNPCs collection with filtered list
  - **Supports**: Both legacy mode (DialogueService) and region mode

- **Added**: `GetNPCDisappearanceMessage(NPC npc)` private method
  - **Purpose**: Generate context-aware log messages when NPCs leave
  - **Examples**:
    - "Mira has gone home for the night." (evening/night)
    - "The shopkeeper has closed shop for the day." (merchants)
    - "Guard has left for the morning." (morning departures)
  - **Implementation**: Uses NPC role and current time to create appropriate messages

- **Added**: `GetNPCAppearanceMessage(NPC npc)` private method
  - **Purpose**: Generate context-aware log messages when NPCs arrive
  - **Examples**:
    - "The shopkeeper has opened for the morning." (merchants)
    - "Mira has arrived for the afternoon." (afternoon arrivals)
    - "Guard has arrived." (generic arrivals)
  - **Implementation**: Uses NPC role and current time to create appropriate messages

- **Added**: TODO comment for future features
  - `// TODO: Add NPC pathfinding or animated map icons later`
  - **Purpose**: Placeholder for visual NPC movement system

#### Implementation Details

**NPC Filtering Flow:**// When constructing MapViewModel with region
NearbyNPCs = new ObservableCollection<NPC>(
    region.NPCs.Where(npc => npc.IsAvailableNow())
);
**Time Change Detection:**public void RefreshTimeDisplay()
{
    OnPropertyChanged(nameof(CurrentDay));
    OnPropertyChanged(nameof(TimeOfDay));
    
    // Check if hour has changed significantly
    if (TimeService.Hour != _lastHour)
    {
        RefreshNearbyNPCs();
        _lastHour = TimeService.Hour;
    }
}
**NPC Refresh Logic:**private void RefreshNearbyNPCs()
{
    var previousNPCs = NearbyNPCs.ToList();
    var availableNPCs = _region.NPCs.Where(npc => npc.IsAvailableNow()).ToList();
    
    // Find NPCs that disappeared
    var disappeared = previousNPCs.Where(prev => 
        !availableNPCs.Any(curr => curr.Name == prev.Name)
    ).ToList();
    foreach (var npc in disappeared)
    {
        _globalLog?.Add(GetNPCDisappearanceMessage(npc));
    }
    
    // Find NPCs that appeared
    var appeared = availableNPCs.Where(curr => 
        !previousNPCs.Any(prev => prev.Name == curr.Name)
    ).ToList();
    foreach (var npc in appeared)
    {
        _globalLog?.Add(GetNPCAppearanceMessage(npc));
    }
    
    NearbyNPCs = new ObservableCollection<NPC>(availableNPCs);
}
**Context-Aware Message Generation:**

| Time of Day | NPC Role | Appearance Message | Disappearance Message |
|------------|----------|-------------------|----------------------|
| Morning (6-12) | Merchant/Shopkeeper | "X has opened for the morning." | "X has left for the morning." |
| Morning (6-12) | Any | "X has arrived for the morning." | "X has left for the morning." |
| Afternoon (12-18) | Any | "X has appeared for the afternoon." | "X is no longer around." |
| Evening/Night (18-6) | Any | "X has arrived." | "X has gone home for the night." |
| Any | Merchant/Shopkeeper | "X has opened for the morning." | "X has closed shop for the day." |

#### Example Usage Scenarios

**Scenario 1: Shop Opens in the Morning**
1. **Initial**: Day 1, Night (2:00 AM), Shopkeeper not available (AvailableStartHour = 8)
2. **Action**: Rest for 8 hours
3. **Result**: Day 1, Morning (10:00 AM)
4. **Log Message**: "The shopkeeper has opened for the morning."
5. **UI Update**: Shopkeeper appears in NearbyNPCs list

**Scenario 2: Quest Giver Goes Home at Night**
1. **Initial**: Day 1, Evening (19:00), Mira available (AvailableEndHour = 20)
2. **Action**: Battle (1 hour advance)
3. **Result**: Day 1, Evening (20:00)
4. **Log Message**: "Mira has gone home for the night."
5. **UI Update**: Mira removed from NearbyNPCs list

**Scenario 3: Multiple NPCs Change During Long Rest**
1. **Initial**: Day 1, Evening (19:00)
   - Mira available (8-20)
   - Shopkeeper available (8-20)
   - Night Guard available (20-6)
2. **Action**: Rest for 8 hours
3. **Result**: Day 2, Morning (3:00 AM)
4. **Log Messages**:
   - "Mira has gone home for the night."
   - "The shopkeeper has closed shop for the day."
   - "Night Guard has arrived."
5. **UI Update**: NearbyNPCs shows only Night Guard

**Scenario 4: Fast Travel Between Time Zones**
1. **Initial**: Day 1, Morning (10:00), Location A
   - Merchant available (8-18)
2. **Action**: Fast travel to Location B (2 hours)
3. **Result**: Day 1, Afternoon (12:00), Location B
   - Different merchant with schedule (14-22)
4. **Log Message**: "Merchant is no longer around." (from Location A context)
5. **Log Message**: (No appearance yet, merchant starts at 14:00)
6. **Action**: Battle (1 hour)
7. **Result**: Day 1, Afternoon (13:00)
8. **Action**: Battle again (1 hour)
9. **Result**: Day 1, Afternoon (14:00)
10. **Log Message**: "Merchant has appeared for the afternoon."

#### Integration Points

**Time Advancement Triggers → NPC Refresh:**

| Action | Time Change | RefreshTimeDisplay() Called | NPCs Refreshed |
|--------|-------------|---------------------------|----------------|
| Battle | +1 hour | ✅ Yes | ✅ If hour changed |
| Fast Travel | +2 hours | ✅ Yes | ✅ If hour changed |
| Map Rest | +8 hours | ✅ Yes | ✅ If hour changed |
| Inn Sleep | +8 hours | (BuildingInteriorViewModel) | (After returning to map) |
| Save Game | No change | ❌ No | ❌ No |
| Use Item | No change | ❌ No | ❌ No |

**NPC Availability Check Points:**
1. **MapViewModel Construction**: Initial filtering when entering region
2. **RefreshTimeDisplay()**: After any time advancement
3. **RefreshNearbyNPCs()**: When hour changes (automatic)

#### Future Enhancements (TODOs)

- `// TODO: Add NPC pathfinding or animated map icons later`
  - Visual representation of NPC movement on map
  - Animated transitions when NPCs appear/disappear
  - Path visualization for NPC schedules
  
- **Potential Improvements**:
  - Week-based schedules (NPCs off on certain days)
  - Holiday/festival special schedules
  - Weather-dependent availability
  - Relationship-based availability changes
  - Location transitions (NPCs move between map locations)
  - Custom audio cues for NPC arrivals/departures

---

## Previous Update: NPC Availability & Scheduling System

### Changes Made ✨

#### NPC.cs - Time-Based NPC Availability
- **Added**: `AvailableStartHour` property (int)
  - **Purpose**: Define when an NPC becomes available during the day (0-23 hour format)
  - **Usage**: Set during NPC initialization to control daily schedules

- **Added**: `AvailableEndHour` property (int)
  - **Purpose**: Define when an NPC becomes unavailable during the day (0-23 hour format)
  - **Usage**: Works with AvailableStartHour to create availability windows

- **Added**: `CurrentLocation` property (string)
  - **Purpose**: Track where the NPC is currently located in the game world
  - **Examples**: "Town Square", "Home", "Shop", "Market Square"
  - **Usage**: Can be used for location-based NPC interactions and pathfinding

- **Added**: `IsAvailableNow()` method
  - **Purpose**: Check if the NPC is currently available based on game time
  - **Implementation**: Returns true if `TimeService.Hour >= AvailableStartHour && TimeService.Hour < AvailableEndHour`
  - **Integration**: Can be used in dialogue systems, quest givers, and shop interactions

- **Added**: TODO comment for future features
  - `// TODO: Add weekly schedules and holiday routines later`
  - **Purpose**: Placeholder for more complex scheduling systems

#### Example NPC Initializationnew NPC("Mira", "QuestGiver", "Good to see you!")
{
    AvailableStartHour = 8,
    AvailableEndHour = 20,
    CurrentLocation = "Market Square",
    DialogueLines = new() { "Slimes have been attacking!", "Please defeat 3 of them." },
    OfferedQuest = new Quest("Slime Hunt", "Defeat 3 Slimes for Mira.", 3, 50, 20)
};
#### Implementation Details

**NPC Availability Logic:**
- NPCs can now have specific hours of operation
- `IsAvailableNow()` checks against `TimeService.Hour` for real-time availability
- Example schedules:
  - Shop keeper: 8:00 - 20:00 (12 hours)
  - Inn keeper: 18:00 - 24:00 (evening/night only)
  - Quest giver: 6:00 - 22:00 (most of the day)
  - Guard: 0:00 - 24:00 (always available)

**Use Cases:**
1. **Dialogue Systems**: Check `IsAvailableNow()` before allowing NPC interaction
2. **Quest Givers**: Only offer quests during available hours
3. **Shops**: Open/close based on NPC availability
4. **Dynamic World**: NPCs appear/disappear based on time of day

**Future Enhancements (TODOs):**
- Weekly schedules (e.g., closed on specific days)
- Holiday routines (special schedules for festivals/events)
- Location transitions (NPCs move between locations throughout the day)
- Relationship-based availability (NPCs may avoid/seek player based on reputation)

---

## Previous Update: Time System Testing & Persistence

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

**Time Persistence Flow:**// Saving
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
**Time Display Binding:**<TextBlock HorizontalAlignment="Center" Margin="0,4,0,0">
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
