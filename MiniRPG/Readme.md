# MiniRPG - Change Log

## Latest Update: Crafting System - CraftingRecipe Model

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
