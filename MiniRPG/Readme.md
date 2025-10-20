# MiniRPG - Change Log

## Latest Update: Reputation View Implementation

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
