# MiniRPG - Change Log

## Latest Update: Building Entry System Integration

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
   - Player can talk to NPCs (transitions to DialogueView, returns to BuildingInteriorView)
   - Player can exit building (returns to MapView)

#### Example Usage// In MapView.xaml, bind to a button or list item:
<Button Content="Enter" Command="{Binding EnterBuildingCommand}" 
        CommandParameter="{Binding SelectedBuilding}" />

// MapViewModel automatically handles the command:
// EnterBuildingCommand -> OnEnterBuilding?.Invoke(selectedBuilding)

// MainViewModel receives the event and creates the interior view:
mapVM.OnEnterBuilding += selectedBuilding =>
{
    var buildingVM = new BuildingInteriorViewModel(selectedBuilding, CurrentPlayer);
    buildingVM.OnExitBuilding += () => ShowMap();
    CurrentViewModel = buildingVM;
    // TODO: Add animated fade transition between outdoor and indoor views
};
#### Potential Future Enhancements
Based on the TODO comment:
- **Animated Fade Transitions**: Smooth visual transitions between views
  - Fade-out outdoor MapView
  - Brief transition animation (door opening, screen fade)
  - Fade-in indoor BuildingInteriorView
  - Reverse animation when exiting
- **Building Entry Animations**: Visual feedback for entering/exiting
  - Character sprite walking to building door
  - Door opening animation
  - Interior reveal animation
- **Sound Effects**: Audio cues for immersion
  - Door opening/closing sounds
  - Footstep sounds while entering
  - Different ambient sounds for interior vs exterior
- **Loading Screens**: For larger buildings or complex interiors
  - Show building name and description during load
  - Tips or lore text during transition

#### Files Modified
- `MiniRPG\ViewModels\MapViewModel.cs`
- `MiniRPG\ViewModels\MainViewModel.cs`
- `MiniRPG\Readme.md`

---

## Previous Update: BuildingInteriorViewModel Enhancement

### Changes Made ✨

#### BuildingInteriorViewModel.cs - Updated
- **Updated**: `BuildingInteriorViewModel` class in ViewModels folder to match requirements
  - **Properties Added**:
    - `Player Player`: The player character (required for dialogue interactions)
  - **Constructor Updated**:
    - **Signature**: Now requires both `Building building` and `Player player` parameters
    - **Example**: `new BuildingInteriorViewModel(building, player)`
    - **Logging**: Automatically logs entry message when entering a building
      - Format: `"Entered {building.Name}."`
  - **TODO Comment Added**:
    - `// Add room-based navigation and building-specific events`
    - Foundation for multi-room buildings
    - Support for building-specific gameplay mechanics

#### Requirements Fulfilled
All requirements from Instructions.txt have been implemented:
- ✅ Inherits from BaseViewModel
- ✅ Property: `Building CurrentBuilding`
- ✅ Property: `Player Player`
- ✅ Property: `NPC? SelectedNPC`
- ✅ Command: `TalkToNPCCommand` - Opens DialogueViewModel with selected NPC and Player
- ✅ Command: `ExitBuildingCommand` - Triggers OnExitBuilding event
- ✅ Constructor: Accepts `Building building` and `Player player` parameters
- ✅ Constructor: Logs entry message: `"Entered {building.Name}."`
- ✅ TODO comment: `// Add room-based navigation and building-specific events`

#### Integration with Existing System
The updated BuildingInteriorViewModel now properly integrates with:
- **Player Model**: Receives player reference for dialogue system
- **DialogueViewModel**: Can pass both NPC and Player to DialogueViewModel constructor
- **MainViewModel**: Can coordinate view transitions with full context
- **Event System**: Uses events for clean separation of concerns

#### Updated Example Usagevar building = currentRegion.Buildings.First(b => b.Name == "Mira's Home");
var buildingVM = new BuildingInteriorViewModel(building, CurrentPlayer);

buildingVM.OnTalkToNPC += selectedNPC =>
{
    var dialogueVM = new DialogueViewModel(selectedNPC, CurrentPlayer, GlobalLog);
    dialogueVM.OnDialogueExit += () => ShowBuildingInterior(building, CurrentPlayer);
    CurrentViewModel = dialogueVM;
};

buildingVM.OnExitBuilding += () => ShowMap();
CurrentViewModel = buildingVM;
#### Potential Future Enhancements
Based on the new TODO comment:
- **Room-Based Navigation**: Navigate between different rooms within a building
  - Multi-room buildings (e.g., Inn with common room, bedrooms, kitchen)
  - Room transitions with loading/fade animations
  - Different NPCs and interactions per room
  - Map of building interior
- **Building-Specific Events**: Custom gameplay mechanics per building type
  - **Shop Buildings**: Purchase interface, inventory management
  - **Inn Buildings**: Rest/heal mechanics, room rental
  - **Guild Buildings**: Quest board, guild contracts, reputation system
  - **House Buildings**: NPC-specific story events, private conversations
  - **Training Halls**: Skill training, stat upgrades
  - **Libraries**: Lore books, research mechanics
- **Dynamic Building States**: Buildings change based on time or story progress
  - Time-of-day variations (NPCs move between rooms)
  - Quest-triggered changes (shop gets new inventory after quest)
  - Seasonal decorations and events

#### Files Modified
- `MiniRPG\ViewModels\BuildingInteriorViewModel.cs`
- `MiniRPG\Readme.md`

---

## Previous Update: Building Interior View System

### New Features ✨

// ...existing code...
