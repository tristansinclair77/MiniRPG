# MiniRPG - Change Log

## Latest Update: NPC Interaction System Integration

### New Features ✨

#### MapViewModel Integration with NPC System
- **Added**: `NearbyNPCs` - ObservableCollection<NPC> property
  - Automatically populated with all NPCs from `DialogueService.GetAllNPCs()`
  - Provides list of NPCs available for interaction in the current area
- **Added**: `TalkToNPCCommand` - RelayCommand that accepts NPC parameter
  - Triggers when player interacts with an NPC
  - Raises `OnTalkToNPC` event to communicate with MainViewModel
- **Event Added**: `OnTalkToNPC` event with NPC parameter for parent ViewModel integration

#### MainViewModel NPC Dialogue Integration
- **Subscribed** to `MapViewModel.OnTalkToNPC` event
  - When triggered, creates new `DialogueViewModel` with selected NPC and CurrentPlayer
  - Sets CurrentViewModel to DialogueViewModel for dialogue interaction
  - Logs interaction: "You approach [NPC Name]."
- **Subscribed** to `DialogueViewModel.OnDialogueExit` event
  - Returns to MapViewModel when dialogue ends
  - Seamless navigation between map and dialogue views
- **Added TODO**: "Add NPC proximity detection and movement system later"
  - Placeholder for future feature where NPCs only appear when player is near
  - Movement system will allow player to walk around the map

#### System Flow
1. Player sees list of nearby NPCs in MapView
2. Player clicks on NPC, triggering `TalkToNPCCommand`
3. MainViewModel receives event and creates DialogueViewModel
4. Player interacts with NPC dialogue system
5. Player exits dialogue, returning to MapView
6. Future: NPCs will only be "nearby" based on player position

---

## Previous Update: DialogueViewModel Implementation

### New Features ✨

#### DialogueViewModel Class Implementation
- **Added**: `DialogueViewModel.cs` in the ViewModels folder
- **Inheritance**: Inherits from `BaseViewModel` for MVVM support
- **Properties**:
  - `CurrentNPC` - The NPC being interacted with
  - `CurrentLine` - The current dialogue line being displayed
  - `CurrentIndex` - Index tracking position in dialogue sequence
  - `Player` - The player character reference
- **Commands**:
  - `NextCommand` - Advances through dialogue lines sequentially
  - `AcceptQuestCommand` - Accepts the quest offered by NPC (only available when OfferedQuest exists and dialogue is complete)
  - `ExitDialogueCommand` - Exits the dialogue and triggers `OnDialogueExit` event
- **Features**:
  - Constructor initializes with NPC and Player, sets CurrentLine to NPC's Greeting
  - `NextDialogue()` method navigates through DialogueLines collection
  - When dialogue end is reached and OfferedQuest exists, displays quest acceptance prompt
  - `AcceptQuest()` method adds quest to player's quest log and logs acceptance message
  - Event-driven exit mechanism for integration with parent ViewModels
- **Quest Integration**:
  - Automatically detects when NPC has an offered quest
  - Enables AcceptQuestCommand only after all dialogue lines are viewed
  - Logs quest acceptance: "You accepted [quest title] from [NPC name]."
- **Future Enhancements**:
  - Branching paths and player dialogue choices
  - Emotion indicators and quest gating

---

## Previous Update: DialogueView UI Implementation

### New Features ✨

#### DialogueView XAML Implementation
- **Added**: `DialogueView.xaml` and `DialogueView.xaml.cs` in the Views folder
- **Features**:
  - Card-style dialogue interface with border and themed background (#292944 with #444466 border)
  - **NPC Name Display**: Bold, large text (24pt) in gold color (#F9E97A)
  - **NPC Role Display**: Italic subtitle showing NPC's role
  - **Dialogue Text Area**: Scrollable, multi-line text block with text wrapping for displaying dialogue
  - **Three Action Buttons**:
    - **Next Button**: Advances through dialogue lines (green-tinted)
    - **Accept Quest Button**: Conditionally visible only when `HasOfferedQuest` is true (uses BoolToVisibilityConverter)
    - **Leave Button**: Exits the dialogue (red-tinted)
  - Consistent color scheme matching other views in the game
  - Hover effects and disabled states for all buttons
- **Bindings Required**:
  - `CurrentNPC.Name` - NPC's name
  - `CurrentNPC.Role` - NPC's role
  - `CurrentDialogue` - Current dialogue text
  - `HasOfferedQuest` - Boolean for quest button visibility
  - `NextDialogueCommand` - Command for Next button
  - `AcceptQuestCommand` - Command for Accept Quest button
  - `LeaveDialogueCommand` - Command for Leave button
- **Future Enhancements Planned**:
  - Character portrait display
  - Typing animation effects with voice blips
  - Relationship tracker visuals
  
---

## Previous Update: DialogueService Implementation

### New Features ✨

#### DialogueService Class Implementation
- **Added**: `DialogueService.cs` in the Services folder
- **Features**:
  - Static service for centralized NPC and dialogue management
  - `GetAllNPCs()` method returns predefined NPCs: Mira (QuestGiver), Shopkeeper (Merchant), and Guard (Villager)
  - `GetDialogueForNPC(string name)` method retrieves dialogue lines for specific NPCs by name
  - Each NPC comes with appropriate greeting and dialogue lines
- **NPCs Included**:
  - **Mira**: Quest giver who needs help with slime attacks
  - **Shopkeeper**: Merchant with welcoming shop dialogue
  - **Guard**: Village protector with safety warnings
- **Future Enhancements**: Branching dialogue with choices and relationship/affection system planned

---

## Previous Update: NPC System Added

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
