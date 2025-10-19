# MiniRPG - Change Log

## Latest Update: Building Save/Load System

### Changes Made ✨

#### Player.cs - LastBuildingName Property Added
- **Added**: `string? LastBuildingName` property
  - **Purpose**: Track the last building the player was in for save/load persistence
  - **Getter/Setter**: Standard property with OnPropertyChanged notification
  - **Nullable**: Allows null when player is not in a building
  - **Integration**: Automatically saved to player save file via SaveLoadService
  - **Location**: Added after LastRegionName property in Player model

#### SaveLoadService.cs - Building State Persistence
- **Updated**: SaveData class now includes LastBuildingName
  - **Automatic Serialization**: Property is automatically serialized with existing Player data
  - **Backward Compatibility**: Null values ignored during serialization (DefaultIgnoreCondition.WhenWritingNull)
  - **Load Support**: LastBuildingName restored from save file when player loads game

- **Added**: TODO Comment
  - `// TODO: Add per-building interior coordinates later`
  - Foundation for future feature
  - Supports saving player position within building interiors
  - Enables exact positioning when re-entering buildings

#### MainViewModel.cs - Building Auto-Load on Game Load
- **Updated**: `OnTitleSelectionMade()` method - Continue Game Flow
  - **Building Detection**: After loading saved region, checks if LastBuildingName is set
  - **Building Lookup**: Searches for building in saved region's Buildings collection
  - **Auto-Load Logic**:
    1. If LastBuildingName != null and building found in region
    2. Creates BuildingInteriorViewModel with saved building
    3. Plays appropriate building audio theme
    4. Sets CurrentViewModel to building interior
    5. Adds log message indicating player location
    6. Exits early to avoid loading MapView
  - **Fallback**: If building not found, loads normal MapView

- **Added**: `CreateBuildingInteriorViewModel(Building)` helper method
  - **Purpose**: Centralized method for creating BuildingInteriorViewModel with all event subscriptions
  - **Parameters**: Building instance to create interior for
  - **Event Subscriptions**:
    - OnTalkToNPC: Handles NPC dialogue within building
    - OnExitBuilding: Clears LastBuildingName and returns to MapView
    - Nested dialogue handlers with proper exit flow
  - **Return**: Fully configured BuildingInteriorViewModel
  - **Reusability**: Used in both game load and building entry scenarios

- **Updated**: Building Entry Event Handler
  - **Save on Entry**: Sets `CurrentPlayer.LastBuildingName = selectedBuilding.Name`
  - **Auto-Save**: Calls `SaveLoadService.SavePlayer(CurrentPlayer)` when entering building
  - **Refactored**: Uses CreateBuildingInteriorViewModel helper method
  - **Log Message**: Adds entry log message

- **Updated**: Building Exit Event Handlers
  - **Clear on Exit**: Sets `CurrentPlayer.LastBuildingName = null`
  - **Auto-Save**: Calls `SaveLoadService.SavePlayer(CurrentPlayer)` when exiting building
  - **Multiple Exit Paths**: Handles exit via:
    - Direct Exit button click
    - Exiting through dialogue chains
    - Nested dialogue completion
  - **Audio Restoration**: Plays MapTheme when returning to outdoor map

#### Requirements Fulfilled
All requirements from Instructions.txt have been implemented:

**Player.cs:**
- ✅ Property: `string? LastBuildingName` added to Player model
- ✅ Serialization: Automatically included in save data

**SaveLoadService.cs:**
- ✅ Save: LastBuildingName saved with player data when entering building
- ✅ Load: LastBuildingName restored from save file
- ✅ Auto-Load: If LastBuildingName != null on load, auto-loads building interior view
- ✅ TODO Comment: `// Add per-building interior coordinates later`

**MainViewModel.cs:**
- ✅ Entry Save: Player saved when entering a building (LastBuildingName set)
- ✅ Exit Clear: LastBuildingName cleared when exiting building
- ✅ Auto-Load: Building interior automatically loaded on game continue if LastBuildingName set
- ✅ Helper Method: CreateBuildingInteriorViewModel for code reuse

#### Integration Flow
The complete building save/load system now works as follows:

**Entering a Building:**
1. **User Action**: Player selects building and clicks Enter
2. **MapViewModel**: EnterBuildingCommand triggers OnEnterBuilding event
3. **MainViewModel**: Receives event, creates BuildingInteriorViewModel
4. **Save State**: Sets LastBuildingName = building.Name
5. **Auto-Save**: Calls SaveLoadService.SavePlayer(CurrentPlayer)
6. **View Switch**: CurrentViewModel = BuildingInteriorViewModel
7. **Audio**: Plays building-specific theme

**Exiting a Building:**
1. **User Action**: Player clicks Exit button or completes dialogue
2. **BuildingInteriorViewModel**: OnExitBuilding event fires
3. **MainViewModel**: Receives event
4. **Clear State**: Sets LastBuildingName = null
5. **Auto-Save**: Calls SaveLoadService.SavePlayer(CurrentPlayer)
6. **View Switch**: CurrentViewModel = MapViewModel
7. **Audio**: Plays map theme

**Loading a Game with Building State:**
1. **User Action**: Player selects "Continue" from title screen
2. **Load Player**: SaveLoadService.LoadPlayer() restores player data
3. **Check Region**: If LastRegionName set, loads saved region
4. **Check Building**: If LastBuildingName != null, searches for building in region
5. **Auto-Load Building**: If found, creates BuildingInteriorViewModel
6. **Set View**: CurrentViewModel = BuildingInteriorViewModel (skips MapView)
7. **Audio**: Plays building theme
8. **Fallback**: If building not found, loads MapView normally

#### Code Flow Example// In OnTitleSelectionMade - Continue Game
if (!string.IsNullOrEmpty(CurrentPlayer.LastBuildingName))
{
    var savedBuilding = savedRegion.Buildings?.FirstOrDefault(b => b.Name == CurrentPlayer.LastBuildingName);
    if (savedBuilding != null)
    {
        var buildingVM = CreateBuildingInteriorViewModel(savedBuilding);
        CurrentViewModel = buildingVM;
        try { AudioService.PlayBuildingTheme(savedBuilding.Type); } catch { }
        AddLog($"You are in {savedBuilding.Name}.");
        return; // Exit early
    }
}

// In CreateBuildingInteriorViewModel helper
private BuildingInteriorViewModel CreateBuildingInteriorViewModel(Building selectedBuilding)
{
    var buildingVM = new BuildingInteriorViewModel(selectedBuilding, CurrentPlayer);
    
    buildingVM.OnExitBuilding += () =>
    {
        CurrentPlayer.LastBuildingName = null;
        SaveLoadService.SavePlayer(CurrentPlayer);
        ShowMap();
        try { AudioService.PlayMapTheme(); } catch { }
    };
    
    // ... other event subscriptions ...
    
    return buildingVM;
}

// In Building Entry Event
mapVM.OnEnterBuilding += selectedBuilding =>
{
    var buildingVM = CreateBuildingInteriorViewModel(selectedBuilding);
    CurrentViewModel = buildingVM;
    
    CurrentPlayer.LastBuildingName = selectedBuilding.Name;
    SaveLoadService.SavePlayer(CurrentPlayer);
    
    AddLog($"You enter {selectedBuilding.Name}.");
};
#### Save File Example
The player save file now includes LastBuildingName:
{
  "player": {
    "name": "Hero",
    "lastRegionName": "Greenfield Town",
    "lastBuildingName": "Mira's Home",
    "hp": 30,
    "maxHP": 30,
    "level": 1,
    "gold": 100
    // ... other player properties ...
  },
  "unlockedRegions": ["Greenfield Town"]
}
#### User Experience Improvements
- **Seamless Continuation**: Players resume exactly where they left off
- **No Navigation Required**: Skip map navigation when loading inside building
- **Context Preservation**: Maintains player location state across sessions
- **Auto-Save**: Eliminates risk of losing building location progress
- **Proper Cleanup**: LastBuildingName cleared on exit prevents stale state

#### Potential Future Enhancements
Based on the TODO comment:
- **Per-Building Interior Coordinates**:
  - Save player X/Y position within building
  - Restore exact position when re-entering
  - Supports larger building interiors with movement
  - Grid-based positioning system
  - Coordinate validation on load
  
- **Building State Persistence**:
  - Save NPC conversation progress within buildings
  - Track items collected in building
  - Remember chest/container open states
  - Store building-specific quest progress
  
- **Multi-Floor Buildings**:
  - Save current floor/level within building
  - Support staircase navigation state
  - Restore floor on game load
  
- **Building Interaction History**:
  - Track visited buildings
  - Save first-visit flags for cutscenes
  - Persistent NPC relationship states within buildings

#### Files Modified
- `MiniRPG\Models\Player.cs`
- `MiniRPG\Services\SaveLoadService.cs`
- `MiniRPG\ViewModels\MainViewModel.cs`
- `MiniRPG\Readme.md`

---

## Previous Update: Building Audio Theme System

### Changes Made ✨

#### AudioService.cs - PlayBuildingTheme Method Added
- **Added**: `PlayBuildingTheme(string buildingType)` method
  - **Purpose**: Play different audio themes based on building type
  - **Parameters**: `buildingType` - The type of building (e.g., "Inn", "Shop", "House", "Guild")
  - **Logic**: Switch statement to determine appropriate audio file:
    - **"Inn"** → plays `inn.wav`
    - **"Shop"** → plays `shop.wav`
    - **default** → plays `interior.wav` (for all other building types)
  - **Implementation**: Calls `PlayWavIfExists()` with appropriate file name
  - **Error Handling**: Inherits try-catch from PlayWavIfExists method

- **Added**: TODO Comment
  - `// TODO: // Add smooth crossfade and environmental sound effects later`
  - Foundation for future audio improvements
  - Supports smooth transitions between themes
  - Environmental ambient sounds (e.g., fireplace crackling in inn, crowd noise in shop)

#### MainViewModel.cs - Building Audio Integration
- **Updated**: `CreateMapViewModel()` method - Building Entry Event Handler
  - **Audio on Entry**: Calls `AudioService.PlayBuildingTheme(selectedBuilding.Type)`
    - Plays building-specific theme when entering
    - Based on Building.Type property
    - Wrapped in try-catch for error handling
  - **Audio on Exit**: Calls `AudioService.PlayMapTheme()`
    - Returns to map theme when exiting building
    - Called in main exit handler: `buildingVM.OnExitBuilding += () => { ShowMap(); try { AudioService.PlayMapTheme(); } catch { } };`
    - Called in nested dialogue exit handlers to ensure map theme plays when returning to outdoor map
    - Wrapped in try-catch for error handling

#### Requirements Fulfilled
All requirements from Instructions.txt have been implemented:

**AudioService.cs:**
- ✅ Method: `PlayBuildingTheme(string buildingType)` added
- ✅ Switch Logic:
  - ✅ Case "Inn" → plays inn.wav
  - ✅ Case "Shop" → plays shop.wav
  - ✅ Default → plays interior.wav
- ✅ TODO Comment: `// Add smooth crossfade and environmental sound effects later`

**MainViewModel.cs:**
- ✅ Entry Call: `AudioService.PlayBuildingTheme(Building.Type)` when entering building
- ✅ Exit Call: `AudioService.PlayMapTheme()` when exiting building

#### Integration Flow
The complete building audio system now works as follows:

1. **Building Entry**: Player enters building via EnterBuildingCommand
2. **Audio Switch**: `AudioService.PlayBuildingTheme(selectedBuilding.Type)` executes
3. **Theme Selection**:
   - If building type is "Inn" → plays inn.wav
   - If building type is "Shop" → plays shop.wav
   - Otherwise → plays interior.wav (House, Guild, etc.)
4. **Building Interior**: Player interacts with NPCs, dialogue, etc.
5. **Building Exit**: Player clicks Exit button or completes dialogue chain
6. **Audio Restore**: `AudioService.PlayMapTheme()` executes
7. **Return to Map**: Map theme plays, player returns to outdoor MapView

#### Code Flow Example// In MainViewModel.CreateMapViewModel()
mapVM.OnEnterBuilding += selectedBuilding =>
{
    var buildingVM = new BuildingInteriorViewModel(selectedBuilding, CurrentPlayer);
    
    // ... event subscriptions ...
    
    buildingVM.OnExitBuilding += () =>
    {
        ShowMap();
        try { AudioService.PlayMapTheme(); } catch { }
    };
    
    CurrentViewModel = buildingVM;
    try { AudioService.PlayBuildingTheme(selectedBuilding.Type); } catch { }
    AddLog($"You enter {selectedBuilding.Name}.");
};

// In AudioService
public static void PlayBuildingTheme(string buildingType)
{
    switch (buildingType)
    {
        case "Inn":
            PlayWavIfExists("inn.wav");
            break;
        case "Shop":
            PlayWavIfExists("shop.wav");
            break;
        default:
            PlayWavIfExists("interior.wav");
            break;
    }
}#### Audio File Requirements
The system expects the following audio files in the application directory:
- **inn.wav**: Cozy, relaxing music for inn buildings
- **shop.wav**: Upbeat, commercial music for shop buildings
- **interior.wav**: Generic indoor ambiance for other building types (House, Guild, etc.)
- **map_theme.wav**: Outdoor exploration theme (existing)

#### Building Type Mapping
Based on the Building model, the following types are supported:
- **"Inn"** → inn.wav (dedicated inn theme)
- **"Shop"** → shop.wav (dedicated shop theme)
- **"House"** → interior.wav (generic interior)
- **"Guild"** → interior.wav (generic interior)
- **Custom Types** → interior.wav (fallback for any undefined types)

#### Potential Future Enhancements
Based on the TODO comment:
- **Smooth Crossfade**:
  - Fade-out current theme when switching
  - Fade-in new theme for seamless transition
  - Configurable fade duration (e.g., 1-2 seconds)
  - Prevents abrupt audio cuts
  - Volume envelope control
  
- **Environmental Sound Effects**:
  - **Inn**: Fireplace crackling, distant chatter, mug clinks
  - **Shop**: Cash register, door chime, customer murmurs
  - **House**: Clock ticking, footsteps, creaking floors
  - **Guild**: Weapon sharpening, armor clanking, rowdy voices
  - Layered ambient sounds over music theme
  - Dynamic volume based on player actions
  
- **Positional Audio**:
  - Volume changes based on NPC proximity
  - Stereo panning for directional sound
  - Echo/reverb for large buildings
  
- **Time-of-Day Variations**:
  - Different themes for day/night
  - Quieter music at night
  - Unique ambient sounds per time period

#### Files Modified
- `MiniRPG\Services\AudioService.cs`
- `MiniRPG\ViewModels\MainViewModel.cs`
- `MiniRPG\Readme.md`

---

## Previous Update: BuildingInteriorViewModel Exit Functionality Enhancement

### Changes Made ✨

#### BuildingInteriorViewModel.cs - Exit Building Event Implementation
- **Updated**: `ExitBuilding()` method
  - **Functionality**: Properly invokes `OnExitBuilding?.Invoke()` event
  - **Purpose**: Signals MainViewModel to switch back to MapViewModel
  - **TODO Comment Added**: `// TODO: Add fade-to-black transition between scenes`
    - Foundation for smooth visual transitions
    - Supports fade-out/fade-in animation when exiting buildings
    - Future enhancement for better UX

#### MainViewModel.cs - Event Listener Verification
- **Verified**: MainViewModel properly listens to `OnExitBuilding` event
  - **Handler**: `buildingVM.OnExitBuilding += () => ShowMap();`
  - **Action**: Calls `ShowMap()` which creates new MapViewModel with CurrentPlayer and currentRegion
  - **Result**: Seamless transition back to outdoor MapView after exiting building

#### Requirements Fulfilled
All requirements from Instructions.txt have been implemented:

**BuildingInteriorViewModel.cs:**
- ✅ Event: `Action OnExitBuilding` already declared
- ✅ Command: `ExitBuildingCommand` executes `OnExitBuilding?.Invoke()`
- ✅ TODO Comment: `// TODO: Add fade-to-black transition between scenes`

**MainViewModel.cs:**
- ✅ Listener: MainViewModel subscribes to `OnExitBuilding` event
- ✅ Handler: Switches back to MapViewModel(CurrentPlayer, currentRegion) via ShowMap()

#### Integration Flow
The complete building exit system now works as follows:

1. **BuildingInteriorView**: Player clicks "Exit" button
2. **BuildingInteriorViewModel**: ExitBuildingCommand executes
3. **ExitBuilding Method**: Invokes OnExitBuilding event
4. **MainViewModel Handler**: Receives event notification
5. **ShowMap Method**: Creates new MapViewModel with CurrentPlayer and current region
6. **View Transition**: CurrentViewModel switches back to MapView
7. **Player Location**: Returns to outdoor map in same region

#### Code Flow Example// In BuildingInteriorViewModel
private void ExitBuilding()
{
    // TODO: Add fade-to-black transition between scenes
    OnExitBuilding?.Invoke();
}

// In MainViewModel.CreateMapViewModel()
buildingVM.OnExitBuilding += () => ShowMap();

// ShowMap() creates fresh MapViewModel
private void ShowMap()
{
    var mapVM = CreateMapViewModel();
    CurrentViewModel = mapVM;
    try { AudioService.PlayMapTheme(); } catch { }
    AddLog("Switched to MapView");
}#### Potential Future Enhancements
Based on the TODO comment:
- **Fade-to-Black Transition**:
  - Fade-out animation when exiting building
  - Black screen transition effect
  - Fade-in animation showing outdoor MapView
  - Reverse of entry animation for consistency
  - Duration: ~0.5-1.0 seconds for smooth experience
  
- **Exit Animation Variations**:
  - Different transitions based on building type
  - Door closing sound effects
  - Character sprite walking out animation
  - Time-of-day changes (e.g., enter during day, exit at night)
  
- **Context Preservation**:
  - Save player position near building entrance
  - Highlight exited building on map
  - Show "You left [Building Name]" notification
  - Brief cooldown before re-entering same building

#### Files Modified
- `MiniRPG\ViewModels\BuildingInteriorViewModel.cs`
- `MiniRPG\Readme.md`

---

## Previous Update: Buildings UI Integration in MapView

### Changes Made ✨

#### MapView.xaml - Buildings Expander Added
- **Added**: Buildings Expander section
  - **Title**: "Buildings"
  - **Location**: Grid.Row="7", positioned after NPCs Expander
  - **Styling**: Consistent with existing expanders (White foreground, #292944 background)
  - **Margin**: "0,0,0,10" for proper spacing

- **Added**: Buildings ListBox
  - **Name**: `BuildingListBox`
  - **Binding**: `ItemsSource="{Binding RegionBuildings}"`
  - **Selected Item**: `SelectedItem="{Binding SelectedBuilding, Mode=TwoWay}"`
  - **Styling**: Background="#222233", Foreground="White", Height="100"
  - **Template**: Displays building Name (Bold, White) and Type (Gray) in parentheses
    - Format: "Building Name (Type)"
    - Example: "Mira's Home (House)"

- **Added**: Enter Building Button
  - **Label**: "Enter"
  - **Command**: `{Binding EnterBuildingCommand}`
  - **CommandParameter**: `{Binding SelectedItem, ElementName=BuildingListBox}`
  - **Styling**: Foreground="#F9E97A", Background="#222233", FontWeight="Bold"
  - **Margin**: "0,8,0,0" for spacing above button

- **Added**: TODO Comments
  - `<!-- TODO: Add clickable building icons on visual map -->`
    - Future: Replace list with interactive map markers
    - Visual building icons on the region map
    - Click-to-enter functionality on map icons
  - `<!-- TODO: Add building entry animation -->`
    - Future: Add animated transitions when entering buildings
    - Door opening animations
    - Fade/transition effects

#### MapViewModel.cs - SelectedBuilding Property Added
- **Added**: `Building? SelectedBuilding` property
  - **Purpose**: Track the currently selected building in the ListBox
  - **Getter/Setter**: Standard property with OnPropertyChanged notification
  - **Command Refresh**: Calls `RaiseCanExecuteChanged()` on EnterBuildingCommand
  - **Two-Way Binding**: Supports UI selection synchronization

#### Requirements Fulfilled
All requirements from Instructions.txt have been implemented:

**MapView.xaml:**
- ✅ Expander: Added with title "Buildings"
- ✅ ListBox: Bound to RegionBuildings
- ✅ Display: Each building's Name and Type shown
- ✅ Button: Labeled "Enter" bound to EnterBuildingCommand
- ✅ CommandParameter: Set to SelectedItem from BuildingListBox
- ✅ TODO Comment: `<!-- TODO: Add clickable building icons on visual map -->`
- ✅ TODO Comment: `<!-- TODO: Add building entry animation -->`

**MapViewModel.cs:**
- ✅ Property: `Building? SelectedBuilding` added for two-way binding support

#### Integration Flow
The complete buildings UI integration now works as follows:

1. **MapView Display**: Buildings Expander shows all buildings in RegionBuildings collection
2. **User Selection**: Player selects a building from the ListBox
3. **SelectedBuilding Property**: Updates via two-way binding
4. **Enter Button**: Enabled when a building is selected
5. **Command Execution**: EnterBuildingCommand executes with selected building as parameter
6. **Event Trigger**: OnEnterBuilding event fires in MapViewModel
7. **View Transition**: MainViewModel creates BuildingInteriorViewModel and switches view

#### UI Layout
The Buildings Expander appears in the MapView with the following structure:Buildings ▼
┌─────────────────────────────────────┐
│ Building Name 1 (Type1)             │
│ Building Name 2 (Type2)             │
│ Building Name 3 (Type3)             │
└─────────────────────────────────────┘
[        Enter        ]#### Example Data Display
When a region has buildings, they appear as:
- "Mira's Home (House)"
- "General Store (Shop)"
- "Town Inn (Inn)"
- "Adventurer's Guild (Guild)"

#### Potential Future Enhancements
Based on the TODO comments:
- **Clickable Building Icons on Visual Map**:
  - Replace text-based ListBox with visual region map
  - Building icons positioned on map coordinates
  - Click on icon to select/enter building
  - Hover effects showing building name and type
  - Mini building sprites or markers
  
- **Building Entry Animation**:
  - Fade-out transition when entering
  - Door opening animation
  - Character sprite walking to building
  - Screen transition effects (e.g., swirl, fade, slide)
  - Sound effects for door opening
  - Brief loading screen with building description

#### Files Modified
- `MiniRPG\Views\MapView.xaml`
- `MiniRPG\ViewModels\MapViewModel.cs`
- `MiniRPG\Readme.md`

---

## Previous Update: Building Entry System Integration

### Changes Made ✨

#### MapViewModel.cs - Building Entry Feature
- **Added**: `ObservableCollection<Building> RegionBuildings` property
  - **Purpose**: Display all buildings available in the current region
  - **Population**: Automatically populated from `Region.Buildings` in constructor
  - **Integration**: Allows UI to display and select buildings from the region
  - **Default Behavior**: Initialized as empty collection in legacy constructor

- **Added**: `ICommand EnterBuildingCommand`
  - **Type**: RelayCommand accepting Building parameter
  - **Purpose**: Command to enter a selected building
  - **Binding**: Can be bound to buttons in MapView UI with building as parameter
  - **Execution**: Triggers `OnEnterBuilding` event with selected building

- **Added**: `event Action<Building>? OnEnterBuilding`
  - **Purpose**: Event fired when player enters a building
  - **Parameters**: Selected Building instance
  - **Subscribers**: MainViewModel subscribes to coordinate view transition
  - **Usage**: Signals transition from outdoor MapView to indoor BuildingInteriorView

#### MainViewModel.cs - Building Entry Event Handling
- **Updated**: `CreateMapViewModel()` method
  - **Subscription Added**: Subscribe to `mapVM.OnEnterBuilding` event
  - **Event Handler Logic**:
    1. Create new `BuildingInteriorViewModel` with selected building and current player
    2. Subscribe to `OnTalkToNPC` event from BuildingInteriorViewModel
       - Create DialogueViewModel with NPC and player
       - Handle return to building interior after dialogue
       - Re-subscribe to building events after dialogue
    3. Subscribe to `OnExitBuilding` event
       - Returns to MapView via `ShowMap()` method
    4. Set CurrentViewModel to BuildingInteriorViewModel
    5. Add entry log message
  - **TODO Comment Added**: `// TODO: Add animated fade transition between outdoor and indoor views`
    - Foundation for smooth visual transitions
    - Supports fade-out/fade-in animation between exterior and interior

#### Requirements Fulfilled
All requirements from Instructions.txt have been implemented:

**MapViewModel:**
- ✅ Property: `ObservableCollection<Building> RegionBuildings`
- ✅ Population: Populated from `current Region.Buildings` in constructor
- ✅ Command: `RelayCommand EnterBuildingCommand` (accepts Building parameter)
- ✅ Event: `OnEnterBuilding(Building)` triggered when command is executed

**MainViewModel:**
- ✅ Subscription: Subscribed to `OnEnterBuilding` event from MapViewModel
- ✅ View Transition: Sets `CurrentViewModel = new BuildingInteriorViewModel(selectedBuilding, CurrentPlayer)`
- ✅ Subscription: Subscribed to `ExitBuilding` event to return to MapViewModel
- ✅ TODO Comment: `// Add animated fade transition between outdoor and indoor views`

#### Integration Flow
The complete building entry system now works as follows:

1. **MapView**: Player selects a building from RegionBuildings collection
2. **MapViewModel**: EnterBuildingCommand executes, triggers OnEnterBuilding event
3. **MainViewModel**: Receives event, creates BuildingInteriorViewModel
4. **BuildingInteriorView**: Displays building interior with NPCs and interactions
5. **Exit Options**:
   - Player exits building via Exit button
   - Completes dialogue chain and exits
6. **MainViewModel**: Handles exit event, switches back to MapViewModel
7. **MapView**: Displays outdoor map with player near building

#### Code Flow Example// In MapViewModel
public ObservableCollection<Building> RegionBuildings { get; private set; }

public ICommand EnterBuildingCommand => new RelayCommand<Building>(building =>
{
    OnEnterBuilding?.Invoke(building);
});

// In MainViewModel.CreateMapViewModel()
mapVM.OnEnterBuilding += selectedBuilding =>
{
    var buildingVM = new BuildingInteriorViewModel(selectedBuilding, CurrentPlayer);
    buildingVM.OnExitBuilding += () => ShowMap();
    CurrentViewModel = buildingVM;
    AddLog($"You enter {selectedBuilding.Name}.");
};
#### Potential Future Enhancements
Based on the TODO comment:
- **Animated Fade Transition**:
  - Smooth fade-out/fade-in effect when entering/exiting buildings
  - Consistent with other transitions (e.g., region changes)
  - Configurable duration for smooth experience
  
- **Building-Specific Entry Effects**:
  - Door opening animation
  - Sound effects for door creak, bell chime, etc.
  - Character sprite walking into building
  
- **Dynamic Building Interactions**:
  - Context-sensitive actions based on building type
  - Unique entry/exit animations for special buildings
  - NPC greetings or cutscenes on first entry

#### Files Modified
- `MiniRPG\ViewModels\MapViewModel.cs`
- `MiniRPG\ViewModels\MainViewModel.cs`
- `MiniRPG\Readme.md`
