# MiniRPG - Change Log

## Latest Update: Environment Service - Lighting System

### Changes Made ✨

#### EnvironmentService.cs - New Lighting Management Service
- **Added**: `EnvironmentService` static class
  - **Purpose**: Manage environmental effects and lighting states in the game world
  - **Location**: Services folder
  - **Type**: Static service class

- **Added**: `CurrentLighting` static property (string)
  - **Purpose**: Track the current lighting state of the game world
  - **Default Value**: "Daylight"
  - **Access**: Public getter, private setter
  - **Values**: "Daylight", "Twilight", "Night"

- **Added**: `OnLightingChanged` static event (EventHandler<string>)
  - **Purpose**: Notify subscribers when lighting changes
  - **Parameter**: New lighting state (string)
  - **Usage**: Subscribe to receive lighting change notifications

- **Added**: `UpdateLighting()` method
  - **Purpose**: Update lighting based on current time of day
  - **Implementation**: 
    - Calls `TimeService.GetTimeOfDay()` to determine current time period
    - Maps time periods to lighting states using switch statement
    - Triggers `OnLightingChanged` event only when lighting actually changes
  - **Mapping**:
    - "Morning" or "Afternoon" → "Daylight"
    - "Evening" → "Twilight"
    - "Night" → "Night"

- **Added**: TODO comment for future features
  - `// TODO: Add regional weather, fog, and rain states later`
  - **Purpose**: Placeholder for weather system expansion

#### Implementation Details

**Lighting State Mapping:**

| Time of Day | TimeService Hour Range | Lighting State |
|------------|----------------------|----------------|
| Morning | 6:00 - 11:59 | Daylight |
| Afternoon | 12:00 - 17:59 | Daylight |
| Evening | 18:00 - 23:59 | Twilight |
| Night | 0:00 - 5:59 | Night |

**UpdateLighting() Logic:**public static void UpdateLighting()
{
    string previousLighting = CurrentLighting;
    string newLighting;

    switch (TimeService.GetTimeOfDay())
    {
        case "Morning":
        case "Afternoon":
            newLighting = "Daylight";
            break;
        case "Evening":
            newLighting = "Twilight";
            break;
        case "Night":
            newLighting = "Night";
            break;
        default:
            newLighting = "Daylight";
            break;
    }

    if (newLighting != previousLighting)
    {
        CurrentLighting = newLighting;
        OnLightingChanged?.Invoke(null, CurrentLighting);
    }
}
**Event Subscription Example:**// Subscribe to lighting changes
EnvironmentService.OnLightingChanged += (sender, newLighting) =>
{
    Debug.WriteLine($"Lighting changed to: {newLighting}");
    // Update UI, apply visual effects, etc.
};

// Update lighting after time changes
TimeService.AdvanceHours(8);
EnvironmentService.UpdateLighting();
#### Example Usage Scenarios

**Scenario 1: Morning to Evening Transition**
1. **Initial**: Day 1, Morning (10:00), Lighting = "Daylight"
2. **Action**: Rest for 8 hours
3. **Result**: Day 1, Evening (18:00)
4. **Call**: `EnvironmentService.UpdateLighting()`
5. **Lighting**: Changes from "Daylight" to "Twilight"
6. **Event**: `OnLightingChanged` triggered with "Twilight"

**Scenario 2: Evening to Night Transition**
1. **Initial**: Day 1, Evening (22:00), Lighting = "Twilight"
2. **Action**: Battle (1 hour)
3. **Result**: Day 1, Evening (23:00), still "Twilight"
4. **Call**: `EnvironmentService.UpdateLighting()`
5. **Lighting**: Remains "Twilight" (no change)
6. **Event**: Not triggered (lighting unchanged)
7. **Action**: Battle again (1 hour)
8. **Result**: Day 2, Night (0:00)
9. **Call**: `EnvironmentService.UpdateLighting()`
10. **Lighting**: Changes from "Twilight" to "Night"
11. **Event**: `OnLightingChanged` triggered with "Night"

**Scenario 3: Full Day Cycle**
1. **Start**: Day 1, Night (2:00), Lighting = "Night"
2. **Rest** (8 hours): Day 1, Morning (10:00), Lighting = "Daylight" ✨ Event triggered
3. **Rest** (8 hours): Day 1, Evening (18:00), Lighting = "Twilight" ✨ Event triggered
4. **Rest** (8 hours): Day 2, Night (2:00), Lighting = "Night" ✨ Event triggered

**Scenario 4: No Lighting Change**
1. **Initial**: Day 1, Morning (8:00), Lighting = "Daylight"
2. **Battle**: Advance 1 hour to 9:00 (still Morning)
3. **Call**: `EnvironmentService.UpdateLighting()`
4. **Lighting**: Remains "Daylight"
5. **Event**: Not triggered (no change)

#### Integration Points

**Where to Call UpdateLighting():**

| Location | Method | Time Change | Call UpdateLighting() |
|----------|--------|-------------|----------------------|
| MapViewModel | `StartBattle()` | +1 hour | ✅ After time advance |
| MapViewModel | `FastTravel()` | +2 hours | ✅ After time advance |
| MapViewModel | `Rest()` | +8 hours | ✅ After time advance |
| BuildingInteriorViewModel | `StayAtInn()` | +8 hours | ✅ After time advance |
| SaveLoadService | `LoadPlayer()` | Restore time | ✅ After loading |
| Any | Manual time change | Variable | ✅ After any time manipulation |

**Suggested Integration in MapViewModel:**private async void Rest()
{
    TimeService.AdvanceHours(8);
    EnvironmentService.UpdateLighting(); // Add this call
    RefreshTimeDisplay();
    
    Player.HP = Player.MaxHP;
    OnPropertyChanged(nameof(Player));
    _globalLog?.Add($"You rest and recover all HP. It is now Day {CurrentDay}, {TimeOfDay}.");
    IsSaveConfirmed = true;
    await HideSaveConfirmation();
}
#### Future Use Cases

**Visual Effects:**
- Apply screen tints/overlays based on CurrentLighting
- Adjust sprite brightness/saturation
- Show/hide light sources (torches, lanterns)
- Modify shadow directions and intensity

**Gameplay Effects:**
- Certain enemies only appear at night
- Quest objectives that require specific lighting
- Stealth mechanics affected by lighting
- NPC behavior changes based on lighting

**Weather Integration (TODO):**
- Rain reduces visibility
- Fog creates atmospheric effects
- Regional weather patterns (desert, forest, mountains)
- Weather affects travel time and random encounters

#### Future Enhancements (TODOs)

- `// TODO: Add regional weather, fog, and rain states later`
  - **Weather States**: Clear, Rainy, Foggy, Stormy, Snowy
  - **Regional Weather**: Different weather patterns per region
  - **Weather Events**: Random weather changes over time
  - **Gameplay Impact**: Weather affects visibility, encounter rates, travel costs
  
- **Potential Improvements**:
  - Seasonal lighting variations (longer days in summer, shorter in winter)
  - Eclipse or special celestial events
  - Indoor vs outdoor lighting states
  - Dynamic shadows and ambient lighting
  - Weather sounds (rain, thunder, wind)
  - Temperature system tied to time and weather

---

## Previous Update: NPC Availability UI Display with Value Converters

### Changes Made ✨

#### NPCAvailabilityConverter.cs - New Value Converters
- **Added**: `NPCAvailabilityConverter` class
  - **Purpose**: Convert NPC availability status to display text
  - **Implementation**: Returns " (Available)" if `NPC.IsAvailableNow()` is true, " (Away)" otherwise
  - **Type**: IValueConverter
  - **Usage**: Binds to NPC objects in the ListBox to show availability status

- **Added**: `NPCAvailabilityColorConverter` class
  - **Purpose**: Convert NPC availability status to display color
  - **Implementation**: Returns "LimeGreen" if available, "Gray" if away, "White" as fallback
  - **Type**: IValueConverter
  - **Usage**: Binds to NPC objects to colorize availability status text

#### MapView.xaml - NPC Availability Display
- **Enhanced**: NPCs ListBox ItemTemplate
  - **Added**: Availability status display with colored text
  - **Binding**: Uses `NPCAvailabilityConverter` to show "(Available)" or "(Away)"
  - **Color**: Uses `NPCAvailabilityColorConverter` to display in green or gray
  - **Integration**: Works alongside existing quest status indicator

- **Added**: Converter resources
  - `<converters:NPCAvailabilityConverter x:Key="NPCAvailability" />`
  - `<converters:NPCAvailabilityColorConverter x:Key="NPCAvailabilityColor" />`

- **Added**: TODO comments for future features
  - `<!-- TODO: Replace with day/night portrait lighting -->` - Dynamic portrait lighting based on time
  - `<!-- TODO: Add animated 'open/closed' shop icons -->` - Visual shop status indicators

#### Implementation Details

**NPC List Display Format:**[NPC Name] ([Role]) ([Availability]) ([Quest Status])
**Example Display:**
- **Available NPC with Quest**: "Mira (QuestGiver) (Available) (Quest!)" 
  - Name: White/Bold
  - Role: Gray
  - Availability: Green/Bold
  - Quest Status: Yellow/Bold
  
- **Away Merchant**: "The Shopkeeper (Merchant) (Away)"
  - Name: White/Bold
  - Role: Gray
  - Availability: Gray/Bold

**Converter Logic:**// NPCAvailabilityConverter
public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
{
    if (value is NPC npc)
    {
        return npc.IsAvailableNow() ? " (Available)" : " (Away)";
    }
    return string.Empty;
}

// NPCAvailabilityColorConverter
public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
{
    if (value is NPC npc)
    {
        return npc.IsAvailableNow() ? "LimeGreen" : "Gray";
    }
    return "White";
}**XAML Binding:**<StackPanel Orientation="Horizontal">
    <TextBlock Text="{Binding Name}" FontWeight="Bold" Foreground="White" />
    <TextBlock Text="(" Foreground="#CCCCCC" />
    <TextBlock Text="{Binding Role}" Foreground="#CCCCCC" />
    <TextBlock Text=")" Foreground="#CCCCCC" />
    
    <!-- Availability Status with Color -->
    <TextBlock Text="{Binding Converter={StaticResource NPCAvailability}}" FontWeight="Bold">
        <TextBlock.Foreground>
            <Binding Converter="{StaticResource NPCAvailabilityColor}" />
        </TextBlock.Foreground>
    </TextBlock>
    
    <!-- Quest Status -->
    <TextBlock Foreground="#F9E97A" FontWeight="Bold">
        <TextBlock.Text>
            <MultiBinding Converter="{StaticResource NPCQuestStatus}">
                <Binding />
                <Binding Path="DataContext.Player" RelativeSource="{RelativeSource AncestorType=UserControl}" />
            </MultiBinding>
        </TextBlock.Text>
    </TextBlock>
</StackPanel>#### Example Usage Scenarios

**Scenario 1: NPC Opens Shop in Morning**
1. **Initial**: Day 1, Night (6:00 AM), Merchant has AvailableStartHour = 8
2. **Display**: "Merchant (Shopkeeper) (Away)" in gray
3. **Action**: Rest for 8 hours
4. **Result**: Day 1, Afternoon (14:00)
5. **Display**: "Merchant (Shopkeeper) (Available)" in green
6. **Log**: "Merchant has opened for the morning."

**Scenario 2: Quest Giver with Available Quest**
1. **Time**: Day 1, Morning (10:00), Mira available (8-20)
2. **Display**: "Mira (QuestGiver) (Available) (Quest!)"
  - Name: White
  - Role: Gray
  - Available: Green
  - Quest!: Yellow
3. **Player Action**: Accept quest from Mira
4. **Display**: "Mira (QuestGiver) (Available)"
5. **Player Action**: Complete quest and turn in
6. **Display**: "Mira (QuestGiver) (Available) (Thanks!)"

**Scenario 3: NPC Goes Home at Night**
1. **Time**: Day 1, Evening (19:00), Shopkeeper available (8-20)
2. **Display**: "Shopkeeper (Merchant) (Available)" in green
3. **Action**: Battle (1 hour advance)
4. **Result**: Day 1, Evening (20:00)
5. **Display**: "Shopkeeper (Merchant) (Away)" in gray
6. **Log**: "Shopkeeper has closed shop for the day."

**Scenario 4: Mixed NPC Availability**
1. **Time**: Day 1, Night (2:00 AM)
2. **NPCs in List**:
   - "Night Guard (Guard) (Available)" - Green (Available 20-6)
   - "Mira (QuestGiver) (Away)" - Gray (Available 8-20)
   - "Shopkeeper (Merchant) (Away) (Quest!)" - Gray availability, Yellow quest marker (Available 8-20)
3. **Action**: Rest for 8 hours
4. **Result**: Day 1, Morning (10:00)
5. **Updated List**:
   - "Mira (QuestGiver) (Available) (Quest!)" - Green (now available)
   - "Shopkeeper (Merchant) (Available) (Quest!)" - Green (now available)
   - Night Guard no longer in list (not available)

#### Integration with Existing Systems

**Value Converter Flow:**
1. **Data Binding**: MapView.xaml binds to `NearbyNPCs` collection
2. **ItemTemplate**: Each NPC in ListBox uses DataTemplate
3. **Availability Check**: `NPCAvailabilityConverter` calls `npc.IsAvailableNow()`
4. **Time Service**: `IsAvailableNow()` checks `TimeService.Hour` against NPC schedule
5. **Color Binding**: `NPCAvailabilityColorConverter` determines text color
6. **UI Update**: TextBlock displays colored status text

**Automatic Updates:**
- When `RefreshNearbyNPCs()` is called (time changes), the collection updates
- WPF data binding automatically refreshes the UI
- Converters are re-evaluated for each NPC in the updated list
- Display text and colors update accordingly

#### Future Enhancements (TODOs)

- `<!-- TODO: Replace with day/night portrait lighting -->`
  - Dynamic NPC portrait lighting effects based on time of day
  - Morning: bright/warm lighting
  - Afternoon: neutral lighting
  - Evening: warm/orange lighting
  - Night: cool/blue lighting or dim effects

- `<!-- TODO: Add animated 'open/closed' shop icons -->`
  - Visual indicators for shop status (open/closed signs)
  - Animated transitions when shops open/close
  - Custom icons for different building types (shops, inns, etc.)
  - Flashing or glowing effects for available quest givers

- **Potential Improvements**:
  - Tooltip on hover showing NPC schedule details
  - Visual indicators for NPC location on map
  - Availability countdown timer for upcoming NPCs
  - Custom availability icons instead of text
  - Sound effects when NPCs become available/unavailable

---

## Previous Update: Dynamic NPC Scheduling & Time-Based Appearance System

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
);**Time Change Detection:**public void RefreshTimeDisplay()
{
    OnPropertyChanged(nameof(CurrentDay));
    OnPropertyChanged(nameof(TimeOfDay));
    
    // Check if hour has changed significantly
    if (TimeService.Hour != _lastHour)
    {
        RefreshNearbyNPCs();
        _lastHour = TimeService.Hour;
    }
}**NPC Refresh Logic:**private void RefreshNearbyNPCs()
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
}**Context-Aware Message Generation:**

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
};#### Implementation Details

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
Debug.WriteLine($"Time loaded - Day {TimeService.Day}, Hour {TimeService.Hour}");**Time Display Binding:**<TextBlock HorizontalAlignment="Center" Margin="0,4,0,0">
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
}#### Testing Scenarios

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
