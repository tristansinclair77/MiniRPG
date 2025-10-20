# MiniRPG - Change Log

## Latest Update: SaveLoadService Enhanced Serialization

### SaveLoadService.cs (Modified)
- **Enhanced Serialization**:
  - Added `PropertyNameCaseInsensitive = true` to JsonSerializerOptions to ensure robust property matching
  - Serialization now explicitly logs `Item.EnhancementLevel` for all equipped items (Weapon, Armor, Accessory)
  - Player.Inventory materials quantities are tracked and logged during save operations
  - Crafted items added dynamically with enhancement levels are properly serialized and logged
  - Enhanced debug logging shows material quantities (e.g., "Iron Ore x3") and enhanced items (e.g., "Steel Sword +2")
  
- **Enhanced Deserialization**:
  - Added `ValidateAndDeduplicateInventory()` method to prevent duplicate entries during load
  - Validation compares items by Name, Type, EnhancementLevel, AttackBonus, and DefenseBonus
  - Duplicate items are automatically removed and logged during deserialization
  - Works for both main save file and backup file loading paths
  - Enhanced debug logging shows loaded materials quantities and enhancement levels
  
- **Stats Recalculation**:
  - Stats are recalculated after loading to include equipment bonuses with enhancement levels
  - Comments updated to clarify that enhancement bonuses are included in stat calculations
  
- **New TODO Added**:// TODO: Add crafting history log and achievement tracking  This TODO indicates future feature to track crafting activities and player achievements

---

## Previous Update: Crafting & Enhancement UI System

### CraftingView.xaml (Modified)
- **Restructured with TabControl**: Split the view into two tabs - "Crafting" and "Enhancement"
- **Added Enhancement Tab**: 
  - Displays equipped weapon and armor with current stats
  - Shows enhancement level (+0, +1, +2, etc.) for each item
  - Displays attack/defense bonuses for equipped items
  - "Enhance" button for each item slot (enabled only when item is equipped)
  - Enhancement cost information: 100 Gold + 1 Iron Ore
  - Real-time player gold display
- **Added TODOs**:
  - Animation and visual glow for upgraded items
  - Enhancement fail chance for higher tiers

### CraftingViewModel.cs (Modified)
- **Added `EnhanceItemCommand`**: New command to enhance equipped items
- **Added `EnhanceItem(Item? item)` method**:
  - Validates item is not null
  - Calls `item.Enhance(Player, cost: 100, materialName: "Iron Ore")`
  - Saves player data after successful enhancement
  - Updates UI bindings to reflect changes

### NullToBooleanConverter.cs (New)
- **Created converter** for enabling/disabling buttons based on null checks
- Returns true if value is not null, false otherwise
- Used to enable "Enhance" buttons only when equipment slot has an item

### App.xaml (Modified)
- **Added `NullToBooleanConverter`** to application resources for XAML binding

---

## Previous Update: Item Enhancement System

### Item.cs (Modified)
- **Added `EnhancementLevel` property**: Tracks the number of times an item has been enhanced (default: 0)
- **Added `Enhance(Player player, int cost, string materialName)` method**: 
  - Validates that player has enough gold and the required material
  - Consumes gold using `Player.SpendGold(cost)`
  - Consumes material from player's inventory using `Player.RemoveItem(material)`
  - Increments `EnhancementLevel` by 1
  - Increases item stats:
    - Weapons: `AttackBonus` increases by +2
    - Armor: `DefenseBonus` increases by +1
  - Logs enhancement success message
- **Added TODO**: Enhancement success rate and rarity effects for future implementation

---

## Previous Update: GameService Loot System - Crafting Materials Drop Pool

### GameService.cs (Modified)
- **Modified `GetRandomLoot()` method**: Expanded drop pool to include crafting materials
- **New drop rate distribution**:
  - 5% chance: Legendary
