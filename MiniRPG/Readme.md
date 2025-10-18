# MiniRPG - Change Log

## Latest Update: NPC System Added

### New Features ✨

#### NPC Class Implementation
- **Added**: `NPC.cs` in the Models folder
- **Features**:
  - Core NPC properties: Name, Role, Greeting
  - `ObservableCollection<string>` for dynamic dialogue lines
  - Quest integration with `OfferedQuest` property (nullable)
  - Constructor for quick NPC setup
- **Example Usage**:new NPC("Mira", "QuestGiver", "Oh! Adventurer, can you help me?")
{
    DialogueLines = new() { "Slimes have been attacking!", "Please defeat 3 of them." },
    OfferedQuest = new Quest("Slime Hunt", "Defeat 3 Slimes for Mira.", 3, 50, 20)
  };- **Future Enhancements**: Portrait support, voice acting, and branching dialogue options planned

---

## Previous Update: Shop & Inventory Improvements + Bug Fixes

### Issues Fixed ✅

#### 1. Shop Item Details Display Fixed
- **Problem**: Item details section was empty when selecting shop items
- **Solution**: Created `ItemInfoService` for centralized item description management
  - Static service with item description lookup table
  - Automatically provides descriptions for all items throughout the game
  - Easy to extend - just add new items to the lookup table
- **Item Model Enhanced**: 
  - Integrated with `ItemInfoService` for automatic description resolution
  - New items automatically register their descriptions
  - Descriptions available everywhere items are referenced
- **ShopView Redesigned**:
  - Fixed item details binding to properly display selected item info
  - Shows item name, description, buy price, and sell price
  - Split-panel design: shop inventory on left, player inventory on right

#### 2. Sell Button Functionality Implemented
- **Problem**: Sell button did nothing
- **Solution**: Complete sell system implemented
  - Added player inventory display in shop (right panel)
  - Select items from your inventory to sell them
  - Sell prices calculated: half of item value or shop's sell price if available
  - Gold automatically added to player account
  - Item removed from inventory with proper count updates
  - Transaction messages logged to game log

#### 3. Quest Board NullReferenceException Fixed
- **Problem**: Clicking "Accept Quest" button caused crash
- **Solution**: Enhanced null safety in `QuestBoardViewModel`
  - Added null checks for Player, ActiveQuests, and CompletedQuests
  - Added `ArgumentNullException` for constructor parameter validation
  - Initialized collections properly to prevent null reference errors
  - Command initialized before quest loading
