# MiniRPG - Change Log

## Latest Update: Item Enhancement System

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
