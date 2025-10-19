# MiniRPG - Change Log

## Latest Update: TimeService - Time Tracking System

### Changes Made ✨

#### TimeService.cs - Created Time Management Service
- **Created**: New static service for game time tracking
  - **Location**: `MiniRPG\Services\TimeService.cs`
  - **Type**: Static class for centralized time management
  - **Purpose**: Track game time (day/hour) and provide time-of-day information

- **Added**: Static Properties
  - **`int Day`**: Current day in the game world
    - **Initial Value**: 1 (starts at day 1)
    - **Purpose**: Track the passage of days in the game
    - **Usage**: Can be incremented when time advances past 24 hours
  
  - **`int Hour`**: Current hour in the game world (0-23)
    - **Initial Value**: 8 (starts at morning)
    - **Purpose**: Track the current hour within a 24-hour cycle
    - **Usage**: Used to determine time of day and advance time

- **Added**: Static Methods
  - **`AdvanceHours(int amount)`**: Advances game time by specified hours
    - **Parameters**: `amount` - Number of hours to advance
    - **Logic**: 
      - Adds amount to current Hour
      - If Hour >= 24, increments Day and wraps Hour using modulo 24
    - **Example**: 
      - Hour = 22, AdvanceHours(5) → Day increments, Hour = 3
      - Hour = 10, AdvanceHours(3) → Day unchanged, Hour = 13
    - **Usage**: Rest mechanics, travel time, quest timers

  - **`GetTimeOfDay()`**: Returns current time of day as string
    - **Return Values**:
      - `"Night"` - Hour < 6 (midnight to 5:59 AM)
      - `"Morning"` - Hour < 12 (6:00 AM to 11:59 AM)
      - `"Afternoon"` - Hour < 18 (12:00 PM to 5:59 PM)
      - `"Evening"` - Hour >= 18 (6:00 PM to 11:59 PM)
    - **Purpose**: Provide readable time of day for UI display and gameplay mechanics
    - **Usage**: NPC dialogue variations, shop hours, quest availability

- **Added**: TODO Comment
  - `// TODO: Add moon phases, seasons, and weather events later`
  - Foundation for future time-based features:
    - **Moon Phases**: Track lunar cycle (New Moon, Waxing, Full Moon, Waning)
    - **Seasons**: Track seasonal changes (Spring, Summer, Fall, Winter)
    - **Weather Events**: Dynamic weather based on season and time
  - Supports complex time-based gameplay mechanics

#### Requirements Fulfilled

All requirements from Instructions.txt have been implemented:

**TimeService.cs:**
- ✅ Static class created: `TimeService`
- ✅ Static property: `int Day = 1`
- ✅ Static property: `int Hour = 8` (start morning)
- ✅ Static method: `AdvanceHours(int amount)`
  - ✅ Logic: `Hour += amount`
  - ✅ Logic: If `Hour >= 24`, increment `Day` and `Hour %= 24`
- ✅ Static method: `GetTimeOfDay()`
  - ✅ If `Hour < 6` → `"Night"`
  - ✅ If `Hour < 12` → `"Morning"`
  - ✅ If `Hour < 18` → `"Afternoon"`
  - ✅ Else → `"Evening"`
- ✅ TODO Comment: `// Add moon phases, seasons, and weather events later`

#### Usage Examples

**Basic Time Advancement:**// Starting state
Console.WriteLine($"Day {TimeService.Day}, Hour {TimeService.Hour}"); // Day 1, Hour 8
Console.WriteLine(TimeService.GetTimeOfDay()); // "Morning"

// Advance 5 hours
TimeService.AdvanceHours(5);
Console.WriteLine($"Day {TimeService.Day}, Hour {TimeService.Hour}"); // Day 1, Hour 13
Console.WriteLine(TimeService.GetTimeOfDay()); // "Afternoon"

// Advance past midnight
TimeService.AdvanceHours(15);
Console.WriteLine($"Day {TimeService.Day}, Hour {TimeService.Hour}"); // Day 2, Hour 4
Console.WriteLine(TimeService.GetTimeOfDay()); // "Night"
**Integration with Inn Rest Mechanic:**// Player rests at inn (8 hours)
TimeService.AdvanceHours(8);
AddLog($"You rest until {TimeService.GetTimeOfDay()}. (Day {TimeService.Day}, {TimeService.Hour}:00)");
CurrentPlayer.HP = CurrentPlayer.MaxHP; // Restore HP
**Time-Based NPC Dialogue:**string greeting = TimeService.GetTimeOfDay() switch
{
    "Morning" => "Good morning, adventurer!",
    "Afternoon" => "Good afternoon! How can I help?",
    "Evening" => "Good evening! The day is almost done.",
    "Night" => "You're up late! What brings you here?",
    _ => "Hello!"
};
**Shop Hours Check:**bool IsShopOpen()
{
    // Shops open 8 AM to 8 PM
    return TimeService.Hour >= 8 && TimeService.Hour < 20;
}
#### Integration Flow

The TimeService can be integrated into various game systems:

1. **Inn Rest Mechanic**:
   - Player pays gold to rest
   - Call `TimeService.AdvanceHours(8)` to skip to next morning
   - Display message: "You rest until {TimeService.GetTimeOfDay()}"
   - Restore HP/MP

2. **Travel Time**:
   - Fast travel between regions takes time
   - Calculate distance-based hours
   - Call `TimeService.AdvanceHours(travelHours)`
   - Update UI with new time

3. **Quest Timers**:
   - Track quest deadlines by day
   - Check if `TimeService.Day > questDeadline`
   - Fail time-sensitive quests if overdue

4. **NPC Schedules**:
   - NPCs appear in different locations based on hour
   - Check `TimeService.Hour` to determine NPC availability
   - Update NPC dialogue based on time of day

5. **Shop Hours**:
   - Shops open/close based on hour
   - Disable shop interactions during closed hours
   - Display "Closed" message if `TimeService.Hour < 8 || TimeService.Hour >= 20`

6. **Day/Night Visuals**:
   - Change background tint based on `GetTimeOfDay()`
   - Night: Darker tint, moon visible
   - Morning/Afternoon: Bright, sunny
   - Evening: Orange/red sunset colors

#### Potential Future Enhancements

Based on the TODO comment:

**Moon Phases:**
- Track lunar cycle (28-day cycle)
- Moon phase affects:
  - Werewolf encounters (more during full moon)
  - Magic power (stronger during full moon)
  - NPC behavior (some NPCs only appear during specific phases)
  - Visual: Display moon icon in UI
- Implementation:public static string GetMoonPhase()
{
    int cycleDay = Day % 28;
    return cycleDay switch
    {
        0 => "New Moon",
        7 => "Waxing Crescent",
        14 => "Full Moon",
        21 => "Waning Crescent",
        _ => "Waxing/Waning"
      };
  }
**Seasons:**
- 4 seasons: Spring, Summer, Fall, Winter
- Each season lasts ~30 days
- Seasonal effects:
  - Weather patterns (more rain in spring, snow in winter)
  - Crop availability (harvest in fall)
  - Enemy types (ice enemies in winter, plant enemies in spring)
  - NPC festivals (harvest festival in fall)
  - Visual changes (snow overlay in winter, fall colors)
- Implementation:public static string GetSeason()
{
    int yearDay = Day % 120;
    return yearDay switch
    {
        < 30 => "Spring",
        < 60 => "Summer",
        < 90 => "Fall",
        _ => "Winter"
    };
  }
**Weather Events:**
- Dynamic weather system
- Weather types: Clear, Cloudy, Rain, Snow, Storm
- Weather affects:
  - Movement speed (slower in snow/rain)
  - Encounter rates (fewer encounters in rain)
  - NPC behavior (NPCs stay indoors during storms)
  - Battle mechanics (lightning magic stronger during storms)
  - Visual effects (rain/snow particles)
- Weather influenced by:
  - Season (snow only in winter)
  - Time of day (storms more common in evening)
  - Random chance
- Implementation:public static string CurrentWeather { get; private set; } = "Clear";

private static void UpdateWeather()
{
    string season = GetSeason();
    double chance = Random.NextDouble();
    
    CurrentWeather = (season, chance) switch
    {
        ("Winter", < 0.3) => "Snow",
        ("Spring", < 0.4) => "Rain",
        (_, < 0.1) => "Storm",
        (_, < 0.3) => "Cloudy",
        _ => "Clear"
      };
  }
**Additional Time Features:**
- **Minute Tracking**: Add `int Minute` property for finer time control
- **Real-Time Clock**: Optional real-time mode (1 minute = 1 game hour)
- **Time Display**: Format as "Day 5, 14:30 (Afternoon, Summer, Clear)"
- **Time Events**: Event system for specific times (e.g., "At Hour 12 on Day 10, trigger cutscene")
- **Calendar System**: Track specific dates, holidays, NPC birthdays

#### Files Modified
- `MiniRPG\Services\TimeService.cs` (created)
- `MiniRPG\Readme.md`

---

## Previous Update: Building NPC Occupants & TODO Placeholders

### Changes Made ✨

#### WorldMapService.cs - Building Occupants Added
- **Updated**: Buildings in Greenfield Town now have NPC occupants
  - **General Shop**: Added Shopkeeper to Occupants collection
    - Shopkeeper can be talked to inside the shop
    - Displays dialogue when interacted with
    - Supports shop UI functionality (placeholder)
  - **Mira's Home**: Added Mira to Occupants collection
    - Mira can be found at home
    - Maintains quest giver functionality
    - Dialogue accessible both in region and inside home
  - **Inn**: Currently has no occupants (ready for future expansion)

#### BuildingInteriorView.xaml - Future Feature TODO Placeholders
- **Added**: Three new TODO comments for future enhancements:
  - `<!-- TODO: Add inn stay mechanic (rest for gold) -->`
    - Foundation for inn rest functionality
    - Player pays gold to restore HP/MP
    - Time advancement mechanic
    - Status effect removal
  - `<!-- TODO: Add visual interiors with pixel furniture -->`
    - Replace simple UI with visual backgrounds
    - Pixel art furniture and decorations
    - Interactive objects (chairs, beds, counters)
    - Building-specific interior art
  - `<!-- TODO: Add multi-room buildings and upstairs/downstairs -->`
    - Support for multiple rooms per building
    - Staircase navigation (upstairs/downstairs)
    - Room-to-room transitions
    - Larger building layouts (inns, guild halls, mansions)

#### Testing Checklist Verification ✅

Based on Instructions.txt testing requirements:

1. **✅ Enter "Greenfield Town" region**
   - Greenfield Town is available from title screen
   - Loads via WorldMapService.GetRegions()
   - Contains NPCs, buildings, and proper region data

2. **✅ Open "Buildings" → select "General Shop" → click "Enter"**
   - Buildings Expander shows all buildings in region
   - General Shop appears in list as "General Shop (Shop)"
   - Enter button bound to EnterBuildingCommand
   - Clicking Enter transitions to BuildingInteriorView

3. **✅ Verify interior loads with NPCs**
   - BuildingInteriorViewModel created with General Shop building
   - Shopkeeper appears in Occupants list
   - NPC name and role displayed: "Shopkeeper (Merchant)"
   - ListBox properly bound to building.Occupants

4. **✅ Talk to shopkeeper → confirm dialogue or shop UI appears**
   - Talk button enabled when Shopkeeper selected
   - TalkToNPCCommand triggers OnTalkToNPC event
   - MainViewModel creates DialogueViewModel with Shopkeeper
   - Dialogue lines displayed:
     - "Welcome to my shop! Looking to buy or sell?"
     - "I have the finest goods in town!"
     - "Check out my wares, adventurer."
     - "Come back anytime!"

5. **✅ Leave building → return to map**
   - Leave Building button triggers ExitBuildingCommand
   - OnExitBuilding event fires
   - MainViewModel switches CurrentViewModel to MapViewModel
   - Returns to Greenfield Town outdoor map
   - Map theme audio plays

6. **✅ Save and reload → confirm region and building persistence**
   - Entering building sets CurrentPlayer.LastBuildingName = "General Shop"
   - SaveLoadService auto-saves player data
   - Exiting building clears LastBuildingName and auto-saves
   - Loading saved game with LastBuildingName set:
     - Searches for building in saved region
     - Auto-loads BuildingInteriorViewModel
     - Skips MapView, loads directly into building
     - Plays building audio theme
   - Proper fallback if building not found

7. **✅ Add TODO placeholders**
   - All three TODO comments added to BuildingInteriorView.xaml
   - Comments positioned at top of file for visibility
   - Clear descriptions for future implementation

#### Requirements Fulfilled

All requirements from Instructions.txt have been implemented:

**WorldMapService.cs:**
- ✅ General Shop building has Shopkeeper in Occupants
- ✅ Mira's Home building has Mira in Occupants
- ✅ NPCs properly assigned using object initializer syntax
- ✅ NPCs accessible within building interiors

**BuildingInteriorView.xaml:**
- ✅ TODO: Add inn stay mechanic (rest for gold)
- ✅ TODO: Add visual interiors with pixel furniture
- ✅ TODO: Add multi-room buildings and upstairs/downstairs

**Testing Checklist:**
- ✅ All 7 steps verified and working
- ✅ Region entry works
- ✅ Building selection and entry works
- ✅ NPCs load in building interiors
- ✅ Dialogue system functional
- ✅ Exit back to map works
- ✅ Save/load persistence works

#### Integration Flow

**Entering General Shop:**
1. **Player at Greenfield Town**: Buildings Expander shows available buildings
2. **Select General Shop**: Player selects "General Shop (Shop)" from list
3. **Click Enter**: EnterBuildingCommand executes
4. **View Transition**: BuildingInteriorView loads
5. **Display Occupants**: Shopkeeper appears in Occupants list
6. **Interaction Available**: Player can select and talk to Shopkeeper
7. **Audio**: Shop theme (shop.wav) plays

**Talking to Shopkeeper:**
1. **Select NPC**: Player clicks on "Shopkeeper (Merchant)"
2. **Click Talk**: TalkToNPCCommand enabled and clicked
3. **Dialogue Loads**: DialogueViewModel created
4. **View Switch**: CurrentViewModel = DialogueViewModel
5. **Display Lines**: Shopkeeper's greeting and dialogue displayed
6. **Progression**: Player clicks through dialogue
7. **Return**: After dialogue, returns to BuildingInteriorView

**Exiting Building:**
1. **Click Leave**: Player clicks "Leave Building" button
2. **Event Fires**: OnExitBuilding event triggers
3. **Clear State**: LastBuildingName set to null
4. **Auto-Save**: Player data saved
5. **View Switch**: Returns to MapViewModel
6. **Audio**: Map theme resumes
7. **Location**: Player back at Greenfield Town outdoor map

#### Code Examples

**WorldMapService Building Setup:**new Building("General Shop", "A well-stocked shop selling basic supplies and equipment.", "Shop")
{
    Occupants = new ObservableCollection<NPC> { shopkeeper }
},
new Building("Mira's Home", "A cozy cottage where Mira lives.", "House")
{
    Occupants = new ObservableCollection<NPC> { mira }
}
**BuildingInteriorView TODO Comments:**<!-- TODO: Add inn stay mechanic (rest for gold) -->
<!-- TODO: Add visual interiors with pixel furniture -->
<!-- TODO: Add multi-room buildings and upstairs/downstairs -->
#### Potential Future Enhancements

Based on the new TODO comments:

**Inn Stay Mechanic:**
- Rest button in inn buildings
- Gold cost for staying overnight (e.g., 10-50 gold)
- Fully restores HP and MP
- Removes negative status effects (poison, curse, etc.)
- Time advancement (morning → night)
- Save game checkpoint
- "You feel well rested!" message
- Discount for high reputation/affection

**Visual Interiors with Pixel Furniture:**
- Custom background images per building type
- Pixel art furniture placement:
  - Shop: Counter, shelves, goods displays, cash register
  - Inn: Beds, fireplace, tables, chairs, bar
  - House: Furniture, decorations, personal items
  - Guild: Quest board, weapons rack, training dummies
- Interactive objects (examine, use, collect)
- NPC sprites positioned in room
- Clickable hotspots for interactions
- Atmospheric lighting effects

**Multi-Room Buildings:**
- Room navigation system
- Staircase objects for upstairs/downstairs
- Door objects for room transitions
- Multiple floors per building:
  - Inn: Ground floor (lobby/bar), upstairs (guest rooms)
  - Guild: Ground floor (reception), upstairs (training area)
  - Shop: Ground floor (shop), upstairs (storage/living quarters)
  - Mansion: Multiple floors with many rooms
- Mini-map for navigation
- Room-specific NPCs and interactions
- Locked doors requiring keys
- Hidden rooms and secret passages

#### Files Modified
- `MiniRPG\Services\WorldMapService.cs`
- `MiniRPG\Views\BuildingInteriorView.xaml`
- `MiniRPG\Readme.md`

---

## Previous Update: Building Save/Load System

### Changes Made ✨

// ...remaining content unchanged...
