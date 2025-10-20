# MiniRPG - Change Log

## Latest Update: Faction System Enhancement

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
