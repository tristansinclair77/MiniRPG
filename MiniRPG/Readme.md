# MiniRPG - Change Log

## Latest Update: BuildingInteriorViewModel Enhancement

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

#### BuildingInteriorView.xaml
- **Added**: New `BuildingInteriorView.xaml` view in the Views folder
  - **Purpose**: Displays building interior with NPCs and interactions
  - **XAML Layout Components**:
    - **Building Name Header**: Styled header displaying building name with yellow (#F9E97A) text
    - **Building Description**: Text block showing building description
    - **Occupants Expander**: Expandable section listing NPCs in the building
      - List box displaying NPC names and roles
      - NPC selection support with visual styling
    - **Talk Button**: Interactive button bound to `TalkToNPCCommand`
      - Enabled only when an NPC is selected
      - Styled with hover and disabled states
    - **Leave Building Button**: Button bound to `ExitBuildingCommand`
      - Allows player to exit the building and return to map
  - **TODO Comments Added**:
    - `<!-- TODO: Add interior background art -->`
    - `<!-- TODO: Add custom music per building type -->`
    - `<!-- TODO: Add clickable NPC sprites -->`
  - **Visual Styling**:
    - Dark theme consistent with existing views (#222233 background)
    - Purple-tinted UI elements (#292944, #444466)
    - Yellow accent color for highlights (#F9E97A)
    - Rounded corners and borders for modern look
    - Hover effects on buttons

#### BuildingInteriorView.xaml.cs
- **Added**: Code-behind file for BuildingInteriorView
  - Simple UserControl initialization
  - Follows established pattern from other views

#### BuildingInteriorViewModel.cs
- **Added**: New `BuildingInteriorViewModel` class in ViewModels folder
  - **Properties**:
    - `Building CurrentBuilding`: The building being displayed
    - `string BuildingName`: Read-only property for building name
    - `string BuildingDescription`: Read-only property for building description
    - `ObservableCollection<NPC> Occupants`: Collection of NPCs in the building
    - `NPC? SelectedNPC`: Currently selected NPC for interactions
  - **Commands**:
    - `ICommand TalkToNPCCommand`: Initiates dialogue with selected NPC
      - Can only execute when an NPC is selected
      - Raises `OnTalkToNPC` event with selected NPC
    - `ICommand ExitBuildingCommand`: Exits the building
      - Raises `OnExitBuilding` event to return to map
  - **Events**:
    - `event Action<NPC>? OnTalkToNPC`: Triggered when player talks to an NPC
    - `event Action? OnExitBuilding`: Triggered when player leaves the building
  - **Constructor**: Accepts a `Building` parameter to initialize the view
  - **Inherits**: `BaseViewModel` for INotifyPropertyChanged support

#### Purpose & Benefits
The Building Interior View System provides:
- **Interior Navigation**: Players can enter buildings and explore interiors
- **NPC Interaction**: View and interact with NPCs inside buildings
- **Immersive Experience**: Dedicated view for building interiors creates depth
- **Extensible Design**: Foundation for building-specific features (shops, inns, guilds)
- **MVVM Pattern**: Proper separation of concerns with ViewModel and View
- **Event-Driven**: Events allow MainViewModel to coordinate view transitions

#### Integration Points
The BuildingInteriorView integrates with:
- **Building Model**: Displays building name, description, and occupants
- **NPC Model**: Lists occupants and enables dialogue interactions
- **DialogueView**: Can transition to dialogue when talking to NPCs
- **MapView**: Return to map when exiting building
- **MainViewModel**: Coordinates view transitions (future integration)

#### Future Integration (Next Steps)
To fully integrate the Building Interior View:
1. **Add to MapView**: Create "Enter Building" button or building list
2. **Update MapViewModel**: 
   - Add `OnEnterBuilding` event
   - Add command to enter selected building
3. **Update MainViewModel**:
   - Subscribe to `OnEnterBuilding` event from MapViewModel
   - Create BuildingInteriorViewModel when player enters building
   - Handle transitions between MapView and BuildingInteriorView
   - Subscribe to BuildingInteriorViewModel events for NPC dialogue and exiting

#### Potential Future Enhancements
Based on the TODO comments:
- **Interior Background Art**: Custom background images per building type
  - Cozy cottage interior for houses
  - Shop counter and shelves for stores
  - Bar and beds for inns
  - Guild hall with quest board for guilds
- **Custom Music Per Building Type**: Unique ambient music
  - Calm, warm music for homes
  - Lively tavern music for inns
  - Mysterious music for guild halls
  - Upbeat shopping music for stores
- **Clickable NPC Sprites**: Visual NPC representations
  - Character portraits or pixel art sprites
  - Click on sprite to initiate dialogue
  - NPC animations (idle, walking, gestures)
  - Visual quest markers above NPCs

#### Example Usage// In MainViewModel, when player enters a building:
var building = currentRegion.Buildings.First(b => b.Name == "Mira's Home");
var buildingVM = new BuildingInteriorViewModel(building);

buildingVM.OnTalkToNPC += selectedNPC =>
{
    var dialogueVM = new DialogueViewModel(selectedNPC, CurrentPlayer, GlobalLog);
    dialogueVM.OnDialogueExit += () => ShowBuildingInterior(building);
    CurrentViewModel = dialogueVM;
};

buildingVM.OnExitBuilding += () => ShowMap();
CurrentViewModel = buildingVM;
#### Files Added
- `MiniRPG\Views\BuildingInteriorView.xaml`
- `MiniRPG\Views\BuildingInteriorView.xaml.cs`
- `MiniRPG\ViewModels\BuildingInteriorViewModel.cs`

---

## Previous Update: Region Buildings Integration

### New Features ✨

#### Region.cs Model Updates
- **Added**: `ObservableCollection<Building> Buildings { get; set; }` property
  - **Purpose**: Allows regions to contain a collection of buildings
  - **Initialization**: Automatically initialized as empty collection in constructor
  - **Integration**: Connects Building model with Region model for location hierarchy
  - **Usage Example**:var region = new Region("Greenfield Town", "A quiet settlement surrounded by plains.");
region.Buildings.Add(new Building("General Shop", "A shop selling supplies.", "Shop"));
#### WorldMapService.cs Updates
- **Updated**: Greenfield Town region now includes Buildings collection
  - **"General Shop"**:
    - Type: "Shop"
    - Description: "A well-stocked shop selling basic supplies and equipment."
  - **"Inn"**:
    - Type: "Inn"
    - Description: "A cozy inn where travelers can rest and recover."
  - **"Mira's Home"**:
    - Type: "House"
    - Description: "A cozy cottage where Mira lives."

#### Region.cs TODO Comment
- **Added**: New TODO comment for future enhancements:
  - `// Add unlockable guilds or special event locations`
  - Foundation for dynamic building unlocks
  - Support for special event-triggered locations
  - Guild system integration points

#### Purpose & Benefits
The Buildings integration provides:
- **Hierarchical Structure**: Regions now contain buildings as sub-locations
- **Content Organization**: NPCs, shops, and services can be organized by building
- **Immersive World**: Players can visualize distinct locations within regions
- **Extensible Design**: Foundation for building-specific interactions (shops, inns, guilds)
- **Quest Integration**: Buildings can be quest objectives or contain quest NPCs

#### Potential Future Enhancements
Based on the TODO comment:
- **Unlockable Guilds**: Player can join guilds housed in special buildings
  - Warrior's Guild, Mage's Guild, Thieves' Guild
  - Unlock after meeting requirements (level, quests, items)
  - Access to guild-specific quests and rewards
- **Special Event Locations**: Buildings that appear based on story progress
  - Seasonal event buildings (Festival Hall during celebrations)
  - Quest-triggered locations (Hidden Laboratory after completing quest chain)
  - Time-limited buildings (Traveling Merchant's Tent)
- **Building Interiors**: Navigate inside buildings to interact with NPCs and objects
- **Building Upgrades**: Improve buildings with player investment
- **Dynamic Availability**: Buildings appear/disappear based on game state

---

## Previous Update: Building Model Added

### New Features ✨

#### Building.cs Model Class
- **Added**: New `Building.cs` class in the Models folder
  - **Properties**:
    - `string Name`: The name of the building
    - `string Description`: A descriptive text about the building
    - `string Type`: Building category (e.g., "Shop", "Inn", "House", "Guild")
    - `ObservableCollection<NPC> Occupants`: NPCs residing in or managing the building
  - **Constructor**: Accepts name, description, and type parameters
    - Initializes all properties with defaults
    - Creates empty Occupants collection
  - **Example Usage**:new Building("Mira's Home", "A cozy cottage where Mira lives.", "House");- **TODO Comment**: Placeholder for future enhancements:
  - Building art/graphics
  - Entry coordinates for positioning
  - Special functions (shop interface, rest mechanics, etc.)

#### Purpose
The Building model provides structure for locations within regions:
- **Organizational**: Group NPCs by their workplace/residence
- **Immersive**: Create distinct locations players can visit
- **Extensible**: Foundation for interior views, shop systems, inns, guilds
- **Modular**: Can be added to Region.Buildings collection in future updates

#### Potential Integration Points
- **Regions**: Buildings could be added to regions as points of interest
- **NPCs**: Occupants link NPCs to specific buildings
- **Shop System**: "Shop" type buildings can host merchant NPCs
- **Inn System**: "Inn" type buildings for rest/healing mechanics
- **Quest Hubs**: "Guild" type buildings for quest boards and contracts
- **Housing**: "House" type buildings for NPC residences and story locations

---

## Previous Update: World Map Travel System - Complete Testing Checklist & TODO Comments

### New Features ✨

// ...existing code...
