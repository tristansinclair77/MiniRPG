# MiniRPG - Change Log

## Latest Update: Crafting Integration with Blacksmith and Workshop Buildings

### BuildingInteriorViewModel.cs (Modified)
- **Added `OpenCraftingCommand` property**: `ICommand? OpenCraftingCommand`
  - Command to open the crafting menu from inside a building
  - Only initialized for buildings with Type "Blacksmith" or "Workshop"
- **Added `OnOpenCrafting` event**: `event Action? OnOpenCrafting`
  - Event triggered when player opens the crafting menu
  - Invoked by OpenCraftingCommand
- **Modified constructor**:
  - Added conditional initialization of `OpenCraftingCommand`
  - If building type is "Blacksmith" or "Workshop", creates RelayCommand that calls `OpenCrafting()`
  - Command is null for other building types
- **Added `OpenCrafting()` method**:
  - Private method that invokes the `OnOpenCrafting` event
  - Allows MainViewModel to handle the transition to CraftingViewModel

### MainViewModel.cs (Modified)
- **Subscribed to `OnOpenCrafting` event** in `CreateBuildingInteriorViewModel()` method
- When `OnOpenCrafting` is triggered:
  - Creates new `CraftingViewModel` with `CurrentPlayer`
  - Sets `CurrentViewModel = craftingVM`
  - Logs "Entered crafting menu."
  - Subscribes to `OnExitCrafting` event to return to BuildingInteriorViewModel
- When `OnExitCrafting` is triggered from crafting menu:
  - Calls `CreateBuildingInteriorViewModel(selectedBuilding)` to return to building interior
  - Logs "Returned from crafting menu."
- **Added TODO comment**: `// TODO: Add special blacksmith NPC interactions before crafting`
  - Indicates future enhancement for NPC-specific crafting dialogues and bonuses
- **Note**: Subscription to `OnOpenCrafting` is added in two places:
  - Main subscription after creating BuildingInteriorViewModel
  - Re-subscription when returning from NPC dialogue inside building

### Implementation Notes
- Crafting menu is only accessible from Blacksmith and Workshop building types
- Player can enter crafting menu directly from building interior
- After exiting crafting, player returns to the building interior (not to map)
- System gracefully handles navigation: Building Interior → Crafting → Building Interior → Map
- Ready for future enhancements: blacksmith NPC bonuses, special crafting recipes per location
- Follows existing event subscription pattern used for other building interactions

---

## Previous Update: Crafting View Implementation (Updated)

### CraftingViewModel.cs (Updated)
- **Updated ViewModel for Crafting & Enhancement interface**
- Inherits from `BaseViewModel`
- Properties:
  - `Player Player`: Reference to the player character
  - `ObservableCollection<CraftingRecipe> AllRecipes`: Collection of all available crafting recipes
  - `CraftingRecipe? SelectedRecipe`: Currently selected recipe in the UI
- Commands:
  - `CraftCommand`: Command to craft the selected recipe
  - `ExitCraftingCommand`: Command to close the crafting view
- Constructor:
  - **Simplified constructor**: Now accepts only `Player player` parameter
  - Initializes `AllRecipes` with all recipes from `CraftingService.GetAllRecipes()`
  - Initializes commands
- Events:
  - `OnExitCrafting`: Event triggered when exiting the crafting view
- **Method `CraftItem()` (Updated)**:
  - Validates that a recipe is selected
  - If `SelectedRecipe.CanCraft(Player)` returns true:
    - Calls `SelectedRecipe.Craft(Player)` to perform crafting
    - Calls `SaveLoadService.SavePlayer(Player)` to auto-save after crafting
    - Logs success message using Debug.WriteLine
  - Else:
    - Logs "Missing materials or gold." using Debug.WriteLine
  - Updates UI properties after crafting attempt
- **TODO comment added**:
  - Add bulk crafting and auto-select available recipes

### Implementation Notes
- Constructor simplified to remove globalLog dependency
- Now uses Debug.WriteLine for logging instead of global log
- Auto-saves player data after successful crafting
- Integrates with existing SaveLoadService for persistence
- Ready for future enhancements: bulk crafting, recipe filtering, auto-selection of craftable recipes

---

## Previous Update: Crafting View Implementation

### CraftingViewModel.cs (New File)
- **Created ViewModel for Crafting & Enhancement interface**
- Inherits from `BaseViewModel`
- Properties:
  - `Player Player`: Reference to the player character
  - `ObservableCollection<CraftingRecipe> AllRecipes`: Collection of all available crafting recipes
  - `CraftingRecipe? SelectedRecipe`: Currently selected recipe in the UI
  - `bool CanCraftSelected`: Computed property that checks if the selected recipe can be crafted with current resources
- Commands:
  - `CraftCommand`: Command to craft the selected recipe (enabled only when CanCraftSelected is true)
  - `ExitCraftingCommand`: Command to close the crafting view
- Constructor:
  - Accepts `Player player` and `ObservableCollection<string> globalLog` parameters
  - Initializes `AllRecipes` with all recipes from `CraftingService.GetAllRecipes()`
  - Initializes commands with appropriate predicates
- Events:
  - `OnExitCrafting`: Event triggered when exiting the crafting view
- **Method `CraftItem()`**:
  - Validates that a recipe is selected
  - Checks if player has required materials and gold
  - Checks inventory capacity
  - Calls `SelectedRecipe.Craft(Player)` to perform crafting
  - Logs success/failure messages to global log
  - Updates UI properties after crafting
- **TODO comments added**:
  - Add crafting progress bar animation
  - Add recipe category tabs (Weapons, Armor, Potions)

### CraftingView.xaml (New File)
- **Created Crafting & Enhancement UI screen**
- Layout structure:
  - **Header**: "Crafting & Enhancement" title displayed at top
  - **Two-column layout**:
    - **Left panel (40%)**: Available Recipes ListBox
      - Title: "Available Recipes"
      - Bound to `AllRecipes` collection
      - Displays recipe name and result item type
      - Selected item bound to `SelectedRecipe`
    - **Right panel (60%)**: Selected Recipe Details
      - Recipe name and description
      - "Creates:" section showing result item details
      - "Required Materials:" section with scrollable list
        - Displays each material name and required quantity
        - Shows required gold amount
      - "Your Gold:" display showing player's current gold
      - "Craft Item" button bound to `CraftCommand`
        - Enabled/disabled based on CanCraftSelected property
  - **Exit button** at bottom, centered, bound to `ExitCraftingCommand`
- Styling:
  - Background color: #222233 (dark blue-gray)
  - Panel backgrounds: #292944 and #333344 (darker shades)
  - Gold text color: #F9E97A (yellow-gold)
  - Accent text: LightBlue for labels
  - Consistent with existing view styling (ReputationView, ShopView)
- **TODO comments added**:
  - Add crafting progress bar animation
  - Add recipe category tabs (Weapons, Armor, Potions)

### CraftingView.xaml.cs (New File)
- **Created code-behind for CraftingView**
- Standard UserControl initialization
- Namespace: `MiniRPG`

### Implementation Notes
- CraftingView integrates seamlessly with existing CraftingService and CraftingRecipe model
- Uses RelayCommand pattern consistent with other ViewModels in the project
- Material requirements displayed dynamically using ItemsControl and data binding
- Craft button automatically enables/disables based on player resources
- Ready for future enhancements: progress bars, category filtering, recipe discovery
- Follows MVVM pattern and existing code style conventions

---

## Previous Update: Item Model Enhancement - Material Identification

### Item.cs (Modified)
- **Added `IsMaterial` property**: `bool IsMaterial { get; set; }`
  - Identifies whether an item is a crafting material
  - Used to distinguish crafting materials from other item types
- **Updated `GetSampleItems()` method**: Added crafting material items
  - Added **"Iron Ore"** (Material, Value: 10 gold, IsMaterial: true)
    - Description: "Raw iron ore used in crafting."
    - Used in Iron Sword recipe (requires 2x)
  - Added **"Wood"** (Material, Value: 5 gold, IsMaterial: true)
    - Description: "Sturdy wood for crafting."
    - Used in Iron Sword recipe (requires 1x)
  - Added **"Bottle"** (Material, Value: 3 gold, IsMaterial: true)
    - Description: "An empty bottle for potions."
    - Used in Health Potion and Mana Potion recipes (requires 1x)
  - Added **"Herb"** (Material, Value: 8 gold, IsMaterial: true)
    - Description: "A medicinal herb."
    - Used in Health Potion recipe (requires 1x)
- **Added TODO comment**: `// Add item rarity and material grade (e.g., Fine Iron Ore)`
  - Indicates future enhancement for material quality tiers

### Implementation Notes
- IsMaterial property enables filtering and UI differentiation for crafting materials
- All four crafting materials marked correspond to materials used in CraftingService recipes
- Material values set to be affordable for early-game crafting
- Ready for expansion with material rarity and quality grades (e.g., Fine/Superior/Legendary Iron Ore)

---

## Previous Update: Crafting System - CraftingService

### CraftingService.cs (New File)
- **Created static `CraftingService` class** in Services folder
- Provides centralized management of crafting recipes and crafting operations
- **Static Method `GetAllRecipes()`**: Returns `List<CraftingRecipe>`
  - Returns all available crafting recipes in the game
  - Example recipes included:
    * **"Iron Sword"**: Requires 2x "Iron Ore", 1x "Wood", 50 gold
      - Result: Iron Sword (Weapon, +5 Attack, Value: 100 gold)
    * **"Health Potion"**: Requires 1x "Herb", 1x "Bottle", 10 gold
      - Result: Health Potion (Consumable, Restores 20 HP, Value: 30 gold)
    * **"Steel Armor"**: Requires 3x "Steel Ingot", 2x "Leather", 100 gold
      - Result: Steel Armor (Armor, +8 Defense, Value: 200 gold)
    * **"Mana Potion"**: Requires 1x "Magic Crystal", 1x "Bottle", 15 gold
      - Result: Mana Potion (Consumable, Restores magical energy, Value: 40 gold)
    * **"Lucky Charm"**: Requires 5x "Silver Coin", 1x "Four-Leaf Clover", 25 gold
      - Result: Lucky Charm (Accessory, +2 Defense, Value: 80 gold)
- **Static Method `GetRecipesByCategory(string category)`**: Returns `List<CraftingRecipe>`
  - Filters all recipes by the ResultItem's Type property
  - Supported categories: "Weapon", "Consumable", "Armor", "Accessory"
  - Uses LINQ to filter recipes where `ResultItem.Type == category`
- **TODO comment added**: `// TODO: Add recipe discovery system and rarity progression`

### Implementation Notes
- All recipes are instantiated with proper Item objects including stats for equippable items
- Recipe materials use a `Dictionary<string, int>` to map material names to required quantities
- Integrates with existing `CraftingRecipe` model's `CanCraft()` and `Craft()` methods
- Ready for expansion with recipe discovery system and rarity progression
- Follows existing service pattern (static class with static methods)

---

## Previous Update: Crafting System - CraftingRecipe Model

### CraftingRecipe.cs (New File)
- **Created new `CraftingRecipe` class** in Models folder
- Represents a crafting recipe for creating items from materials and gold
- Properties:
  - `string Name`: The name of the recipe
  - `string Description`: Description of what the recipe creates
  - `Item ResultItem`: The item that is produced when crafting
  - `Dictionary<string, int> RequiredMaterials`: Dictionary mapping material names to required quantities
  - `int RequiredGold`: Amount of gold required to craft
- Constructor:
  - `CraftingRecipe(string name, string description, Item resultItem, Dictionary<string, int> requiredMaterials, int requiredGold)`
  - Initializes all properties with provided values
- **Method `bool CanCraft(Player player)`**:
  - Checks if player has enough gold
  - Validates player has each required material in sufficient quantity
  - Returns true if all requirements are met, false otherwise
- **Method `void Craft(Player player)`**:
  - First calls `CanCraft()` to validate requirements
  - If successful:
    - Removes all required materials from player inventory
    - Deducts required gold from player
    - Adds the result item to player inventory
    - Logs "Crafted [ResultItem.Name]!" using Debug.WriteLine
  - If requirements not met, returns without changes
- **TODO comment added**: `// TODO: Add crafting time, rarity tiers, and skill-based bonuses later`

### Implementation Notes
- Uses LINQ to count and find materials in player inventory
- Gracefully handles cases where materials cannot be removed
- Integrates with existing Player inventory and gold management systems
- Ready for extension with crafting time, rarity tiers, and skill-based bonuses

---

## Previous Update: Faction-Based Shop Pricing and Trading Restrictions

### ShopViewModel.cs (Modified)
- **Added `_currentRegion` field**: Stores the current region for faction-based price calculations
- **Modified constructor**: Now accepts optional `Region? currentRegion` parameter
  - Constructor signature: `ShopViewModel(Player player, ObservableCollection<string> globalLog, Region? currentRegion = null)`
- **Added `ApplyFactionPriceModifiers()` method**: Applies reputation-based price adjustments on initialization
  - Gets faction from current region's FactionName
  - Calculates price modifier based on reputation:
    - Reputation >= 50: 0.9x modifier (10% discount)
    - Reputation <= -50: 1.2x modifier (20% price increase)
    - Otherwise: 1.0x modifier (no change)
  - Adjusts both BuyPrice and SellPrice for all items in shop inventory
  - Logs price adjustment messages to inform player
- **Added `CanBuy(object? parameter)` method**: Determines if player can purchase from shop
  - Returns false if faction reputation < -50
  - Used as canExecute predicate for BuyCommand
- **Modified `ExecuteBuy()` method**: Added reputation check at start
  - If faction reputation < -50, logs "They refuse to do business with you." and returns
  - Prevents purchase when shop refuses to trade
- **Updated `BuyCommand` initialization**: Now uses `CanBuy` predicate to disable button when reputation is too low
  - Command initialization: `BuyCommand = new RelayCommand(ExecuteBuy, CanBuy);`
- **Added TODO comment**: `// TODO: Add special item discounts for Allied factions`

### MainViewModel.cs (Modified)
- **Updated `OnOpenShop` event handler** in `CreateMapViewModel()` method
  - Changed ShopViewModel instantiation from `new ShopViewModel(CurrentPlayer, GlobalLog)` to `new ShopViewModel(CurrentPlayer, GlobalLog, _currentRegion)`
  - Now passes the current region to enable faction-based pricing

### Implementation Notes
- Price modifiers are applied to all items in shop inventory during initialization
- Shop will refuse to sell (BuyCommand disabled) when player's reputation with region's faction is below -50
- Price adjustments work retroactively - existing shop items have their prices modified based on current reputation
- System gracefully handles cases where no region or faction is available (defaults to normal prices)

---

## Previous Update: Reputation Button Added to MapView

### MapView.xaml (Modified)
- **Added "Reputation" button** in the top navigation bar, positioned after "Skill Tree" button and before "World Map" button
- Button properties:
  - Label: "Reputation"
  - Command binding: `{Binding OpenReputationCommand}`
  - Styling: Matches existing button style (Foreground="#F9E97A", Background="#222233", FontWeight="Bold")
  - Margin: "8,0,0,0"
- **Added TODO comment**: `<!-- TODO: Replace with crest icon buttons later -->`
  - Positioned above the Reputation button to indicate future enhancement for icon-based navigation

---

## Previous Update: Reputation Integration in MapView and MainView

### MapViewModel.cs (Modified)
- **Added `OpenReputationCommand`**: New RelayCommand to open the Reputation view
- **Added `OnOpenReputation` event**: Event triggered when opening the Reputation view
- **Added `OpenReputation()` method**: Private method that logs "Viewing reputation and faction standings..." and invokes `OnOpenReputation` event
- Changes made in both constructors:
  - Legacy constructor: Added `OpenReputationCommand` initialization
  - Region-based constructor: Added `OpenReputationCommand` initialization
- Command initialization: `OpenReputationCommand = new RelayCommand(_ => OpenReputation());`

### MainViewModel.cs (Modified)
- **Subscribed to `OnOpenReputation` event** in `CreateMapViewModel()` method
- When `OnOpenReputation` is triggered:
  - Creates new `ReputationViewModel` with `CurrentPlayer`
  - Sets `CurrentViewModel = reputationVM`
  - Logs "Viewing reputation and faction standings."
  - Subscribes to `OnExitReputation` event to return to MapViewModel
- When `OnExitReputation` is triggered:
  - Calls `ShowMap()` to return to map view
  - Logs "Returned from Reputation view."
- **Added TODO comment**: `// TODO: Add faction reputation pop-ups on events`

---

## Previous Update: Reputation View Implementation

### ReputationViewModel.cs (New File)
- **Created ViewModel for Reputation & Factions interface**
- Inherits from `BaseViewModel`
- Properties:
  - `Player Player`: Reference to the player character
  - `ObservableCollection<Faction> Factions`: Collection of all player factions
- Commands:
  - `ExitReputationCommand`: Command to close the reputation view
- Constructor:
  - Accepts `Player player` parameter
  - Initializes `Factions = new ObservableCollection<Faction>(Player.Factions.Values)`
  - Initializes `ExitReputationCommand` to trigger `OnExitReputation` event
- Events:
  - `OnExitReputation`: Event triggered when exiting the reputation view
- **TODO comment added**: Add sorting and filtering by friendliness later

### ReputationView.xaml (New File)
- **Created Reputation & Factions UI screen**
- Header displays "Reputation & Factions"
- ListBox bound to `Factions` collection displaying:
  - Faction Name (in gold color #F9E97A)
  - Reputation score (in light blue)
  - Standing status (Allied/Neutral/Hostile with color-coded indicators)
- Close button bound to `ExitReputationCommand`
- **TODO comments added**:
  - Add faction emblems and colored standing indicators
  - Add hover tooltips for faction lore

### ReputationView.xaml.cs (New File)
- **Created code-behind for ReputationView**
- Standard UserControl initialization

### FactionStandingConverter.cs (New File)
- **Created value converter** to convert Faction object to standing text
- Returns "Allied", "Neutral", or "Hostile" based on faction reputation thresholds
- Uses `Faction.GetStanding()` method

### FactionStandingColorConverter.cs (New File)
- **Created value converter** to convert Faction object to color brush based on standing
- Color mapping:
  - Allied → LightGreen
  - Hostile → Red
  - Neutral → LightGray
  - Unknown → White

---

## Previous Update: Faction System Enhancement

### Region.cs (Modified)
- **Added `FactionName` property** to represent the faction associated with each region
- Sample data initialization: Each region now has its faction name (e.g., "Greenfield Town" → "Greenfield Town Faction")

### NPC.cs (Modified)
- **Added `FactionAffiliation` property** to represent the NPC's faction membership
  - Defaults to the region's faction if unspecified
- **Added `ReputationImpactOnQuestCompletion` property** with a default value of 5
  - Controls how much reputation is gained with the NPC's faction when completing their quest
- **Added TODO comment**: Add faction emblems and alignment dialogue filters

### WorldMapService.cs (Modified)
- Updated sample data to initialize `FactionName` for all regions:
  - "Greenfield Town" → "Greenfield Town Faction"
  - "Slime Plains" → "Slime Plains Faction"
  - "Goblin Woods" → "Goblin Woods Faction"
- NPCs in Greenfield Town (Mira and Shopkeeper) now have their `FactionAffiliation` set to the region's faction

---

## Previous Update: Faction Service

### FactionService.cs (New File)
- **Created static `FactionService` class in Services
