# MiniRPG - Change Log

## Latest Update: Passive Skill Persistence and Application

### Player.cs Enhancements
- **Enhanced `UnlockSkill()` method**:
  - Now automatically applies passive skill bonuses immediately upon unlock
  - Calls `skill.ApplyEffect(this, null)` for passive skills after adding to LearnedSkills
  - Debug logging confirms passive skill application: "Applied passive skill bonus: {skill.Name} (+{skill.Power} to {skill.EffectType})"
  - Ensures Iron Skin and other passive skills apply their stat bonuses immediately
- **Added `ApplyPassiveSkills()` method**:
  - Public method that applies all passive skill bonuses from LearnedSkills collection
  - Iterates through all learned passive skills and calls `ApplyEffect()` on each
  - Should be called after loading saved skills to reapply bonuses
  - Debug logging for each passive skill applied
  - Ensures passive bonuses persist across game sessions

### SaveLoadService.cs Enhancements
- **Enhanced `LoadPlayer()` method**:
  - Now calls `player.ApplyPassiveSkills()` after restoring learned skills from save file
  - Ensures passive skill bonuses (like Iron Skin's +5 Defense) are reapplied on load
  - Passive skills now correctly persist and apply their bonuses across game sessions
  - Applied after all skills are restored from save data
- **Enhanced backup restoration**:
  - Also calls `backupPlayer.ApplyPassiveSkills()` when loading from backup
  - Ensures passive skill bonuses are applied even when loading from backup saves
  - Maintains consistency across all load paths

### SkillTreeView.xaml Enhancements
- **Added TODO placeholders for future features**:
  - `<!-- TODO: Add class specializations (Warrior, Mage, etc.) -->`
  - `<!-- TODO: Add skill upgrade tiers -->`
  - `<!-- TODO: Add skill synergy effects -->`
  - Complements existing TODOs for category tabs and icon grid layout
  - Provides roadmap for future skill system expansion

### Technical Details
- Passive skills now correctly apply their stat bonuses when:
  1. Initially unlocked via skill tree
  2. Loaded from save file
  3. Loaded from backup save file
- Iron Skin (+5 Defense) now properly increases player defense permanently
- Power stat bonuses from passive skills are added to base stats (Attack/Defense)
- Active skills (Power Strike, Healing Light) remain unchanged and work as expected
- Skill effects are re-applied on every load to ensure bonuses persist correctly
- System handles multiple passive skills - bonuses stack additively
- Foundation laid for:
  - Class specialization systems (Warrior, Mage, Rogue)
  - Skill upgrade tiers (Basic → Advanced → Master)
  - Skill synergy effects (combos and conditional bonuses)
  - Passive skill visualization in character stats screen
  - Skill refund/respec with stat recalculation

### Testing Checklist Completion
✅ 1. Gain a level → earn Skill Points (1 SP per level)
✅ 2. Open Skill Tree → unlock "Power Strike" (active attack skill)
✅ 3. Start battle → select "Power Strike" → verify extra damage (Skill.Power + Player.Attack)
✅ 4. Save and reload → skill remains learned and persisted
✅ 5. Passive skills persist and reapply bonuses (Iron Skin +5 Defense applies on unlock and load)
✅ 6. TODO placeholders added for class specializations, skill upgrade tiers, and skill synergy effects

---

## Previous Update: Skill System Save/Load Support

### SaveLoadService.cs Enhancements
- **Added `SkillData` class**:
  - Serializable representation of Skill for save/load operations
  - Properties: `Name` (string) and `IsUnlocked` (bool)
  - Maintains essential skill state without full object serialization
  - Lightweight data transfer object for JSON serialization
- **Extended `SaveData` class with skill system properties**:
  - `SkillPoints` (int): Tracks player's available skill points
  - `LearnedSkills` (List<SkillData>): Stores learned skills with names and unlock status
  - `AvailableSkills` (List<SkillData>): Stores available skills with names and unlock status
  - All skill data now persisted alongside player, time, weather, and region data
- **Enhanced `SavePlayer()` method**:
  - Serializes `Player.SkillPoints` to save file
  - Converts `Player.LearnedSkills` to `List<SkillData>` using LINQ projection
  - Converts `Player.AvailableSkills` to `List<SkillData>` using LINQ projection
  - Projects Skill objects to SkillData (Name + IsUnlocked only)
  - Debug logging confirms skill data is saved:
    - "Skill Points saved: {count}"
    - "Learned Skills saved: {names}"
    - "Available Skills saved: {count} skills"
- **Enhanced `LoadPlayer()` method**:
  - Restores `Player.SkillPoints` from save data
  - Restores `Player.LearnedSkills` by matching skill names from `SkillTreeService.GetAllSkills()`
  - Restores `Player.AvailableSkills` by matching skill names from `SkillTreeService.GetAllSkills()`
  - For each saved SkillData:
    - Finds matching Skill object from SkillTreeService by name
    - Sets `Skill.IsUnlocked` to saved state
    - Adds Skill object to appropriate ObservableCollection
  - Clears existing skill collections before restoring to prevent duplicates
  - Debug logging confirms skill data is loaded:
    - "Skill Points loaded: {count}"
    - "Learned Skills loaded: {names}"
    - "Available Skills loaded: {count} skills"
  - Handles legacy saves without skill data (keeps default empty collections)
- **Enhanced backup restoration**:
  - Backup loading also restores skill data (SkillPoints, LearnedSkills, AvailableSkills)
  - Same matching logic applied to backup skill data
  - Ensures skill progression is preserved even when loading from backup
- **Added TODO comment**:
  - `// TODO: Add skill versioning and balance re-scaling later`
  - Placeholder for future skill rebalancing and save migration system

### Technical Details
- Skill objects are reconstituted from SkillTreeService during load to ensure full object state
- Only Name and IsUnlocked are serialized to keep save files lightweight
- Skill.Power, Skill.Description, and other static properties loaded from SkillTreeService
- This approach allows skill stats to be rebalanced without invalidating old saves
- Skill matching uses FirstOrDefault by name - missing skills are silently skipped
- If a skill is removed from SkillTreeService, it won't be restored (graceful degradation)
- If a new skill is added to SkillTreeService, it won't appear in old saves (as expected)
- Foundation laid for:
  - Save versioning system (e.g., "SaveVersion": 2)
  - Skill rebalancing migrations (update Power/CostSP without breaking saves)
  - Skill refund system on balance changes
  - Skill preset imports/exports
  - Cloud save synchronization for skill builds
- Completes the skill system integration: SkillTreeService → SkillTreeView → Player → SaveLoadService
- Skills now fully persist across game sessions with auto-save after unlock

---

## Previous Update: Battle Skills UI Integration

### BattleView.xaml Enhancements
- **Added Skills Expander to battle interface**:
  - New Expander control labeled "Skills" positioned below Run button
  - Width set to 250px for better skill list visibility
  - Dark theme styling matching existing UI (#1E1E2A background, #F9E97A accents)
  - Expands/collapses to show/hide skill selection panel
- **Added Skills ListBox**:
  - Bound to `UsableSkills` collection from BattleViewModel
  - Two-way binding to `SelectedSkill` property for skill selection tracking
  - Displays skill names using DataTemplate
  - Dark background (#1E1E2A) with white text for consistency
  - Height set to 100px for scrollable list of skills
  - Shows only active (non-passive) skills available for combat use
- **Added "Use Skill" Button**:
  - Bound to `UseSkillCommand` from BattleViewModel
  - CommandParameter bound to `SelectedSkill` for command execution
  - Gold text (#F9E97A) on dark background (#222233) matching other battle buttons
  - Bold font weight for visual consistency
  - Width 120px matching Attack/Defend/Run buttons
  - Automatically disabled when no skill is selected (via command CanExecute)
- **Added TODO comment**:
  - `<!-- TODO: Replace with command wheel or battle HUD icons -->`
  - Placeholder for future command wheel UI or battle HUD icon system

### Technical Details
- Skills Expander integrates seamlessly with existing BattleViewModel skill system
- ListBox automatically populates from `UsableSkills` ObservableCollection
- Skill selection syncs with `SelectedSkill` property for real-time command updates
- "Use Skill" button uses existing `UseSkillCommand` from BattleViewModel
- CommandParameter passes selected skill directly to command for execution
- Button state managed by command's CanExecute (requires player turn, battle not over, skill selected)
- UI updates automatically when player learns new skills or skills become available
- Visual style matches existing battle button layout (Attack/Defend/Run)
- Foundation laid for:
  - Command wheel UI for quick skill access
  - Battle HUD with skill hotkeys and cooldown indicators
  - Skill icons and visual effects
  - Skill tooltips showing damage/effect calculations
  - Skill cooldown timers displayed in UI
  - Drag-and-drop skill hotbar customization
- Completes the skill system integration: SkillTreeService → SkillTreeView → BattleView → BattleViewModel

---

## Previous Update: Battle Skill System Integration

### BattleViewModel.cs Enhancements
- **Added `UsableSkills` property**:
  - Returns `ObservableCollection<Skill>` containing only non-passive learned skills
  - Filters `Player.LearnedSkills` using LINQ: `Where(s => !s.IsPassive)`
  - Provides battle-ready skills that can be actively used during combat
  - Automatically updates when player learns new active skills
- **Added `SelectedSkill` property**:
  - Nullable `Skill?` property for tracking currently selected skill
  - Implements property change notifications for UI binding
  - Updates `UseSkillCommand` CanExecute state when selection changes
- **Added `UseSkillCommand`**:
  - New RelayCommand for using skills in battle
  - CanExecute checks: `_canAct && !IsBattleOver && SelectedSkill != null`
  - Ensures skills can only be used during player's turn when a skill is selected
- **Added `UseSkill()` method**:
  - Calls `SelectedSkill.ApplyEffect(Player, CurrentEnemy)` to calculate damage/effect
  - Logs skill usage: "Used {SkillName} on {EnemyName} for {damage} damage!"
  - **For Attack-type skills**:
    - Reduces enemy HP by damage amount
    - Checks for enemy defeat and triggers victory sequence (quest tracking, EXP, loot, gold)
    - Includes full victory logic: quest completion, level-up, item drops, gold rewards, auto-save
  - **For non-Attack skills** (Heal, Buff, etc.):
    - Applies effect to player (healing, stat boosts)
    - Enemy still attacks after skill use
  - Triggers enemy counter-attack after skill use (if enemy survives)
- **Added TODO comment**:
  - `// TODO: Add skill cooldowns, SP or MP costs, and VFX`
  - Placeholder for future skill resource system and visual effects

### Technical Details
- Skill system now fully integrated into battle flow
- Players can use active skills learned from skill tree during combat
- Skill damage/effects calculated using existing `Skill.ApplyEffect()` method
- Attack skills deal damage equal to `Skill.Power + Player.Attack`
- Heal skills restore HP (capped at MaxHP)
- Enemy counter-attack occurs after skill use (matches normal combat flow)
- Victory sequence with skill-based kills includes full quest tracking and rewards
- Foundation laid for:
  - Skill cooldown timers per skill
  - SP (Skill Points) or MP (Mana Points) resource costs
  - Skill animations and visual effects
  - Skill combo systems
  - Enemy skill usage and AI
  - Skill tooltips showing damage/effect calculations
- Integrates seamlessly with existing combat system and player progression

---

## Previous Update: Skill Tree Button in MapView

### MapView.xaml Enhancements
- **Added "Skill Tree" button to Player Info Panel**:
  - New button positioned between "Quest Board" and "World Map" buttons
  - Labeled "Skill Tree" with consistent styling:
    - Gold text (#F9E97A) on dark background (#222233)
    - Bold font weight matching other action buttons
    - 8px left margin for consistent spacing
  - Command binding: `Command="{Binding OpenSkillTreeCommand}"`
  - Enables direct access to Skill Tree from the main map view
- **Added TODO comment for future enhancement**:
  - `<!-- TODO: Add animated sparkle on new skill unlock -->`
  - Placeholder for visual feedback when player gains new skill points

### Technical Details
- Button uses existing `OpenSkillTreeCommand` from MapViewModel (already implemented)
- Follows established UI pattern matching Shop, Quest Board, and World Map buttons
- Positioned logically near other progression-related features (Quest Board)
- Fully integrated with MVVM architecture via command binding
- Foundation laid for future visual enhancements (sparkle animation, notification badge)

---

## Previous Update: Skill Tree Integration with MapViewModel

### MapViewModel.cs Enhancements
- **Added `OpenSkillTreeCommand`**:
  - New RelayCommand to trigger opening the Skill Tree
  - Initialized in both constructors (legacy and region-specific)
  - Bound to UI for player-triggered skill tree access
- **Added `OnOpenSkillTree` event**:
  - Event/callback for opening skill tree
  - Follows existing pattern of event-driven view switching
  - Enables MainViewModel to handle view transitions
- **Added `OpenSkillTree()` method**:
  - Logs message: "Opening Skill Tree..."
  - Invokes `OnOpenSkillTree` event to notify MainViewModel
  - **Added TODO comment**: Add keyboard shortcut or button icon for Skill Tree

### MainViewModel.cs Enhancements
- **Subscribed to `MapViewModel.OnOpenSkillTree` event**:
  - Added subscription in `CreateMapViewModel()` method
  - Creates new `SkillTreeViewModel(CurrentPlayer)` instance
  - Sets `CurrentViewModel = skillTreeViewModel` to switch to skill tree view
  - Logs "Opened Skill Tree." to GlobalLog
- **Subscribed to `SkillTreeViewModel.OnExitSkillTree` event**:
  - Handles event-based exit from skill tree
  - Calls `ShowMap()` to return to MapView
  - Logs "Returned from Skill Tree." to GlobalLog

### Technical Details
- Skill tree is now fully integrated into the main game flow
- Player can open skill tree from MapView using `OpenSkillTreeCommand`
- Event-driven architecture ensures clean view transitions
- Exit from skill tree handled via `OnExitSkillTree` event
- Skill tree changes are auto-saved via SkillTreeViewModel
- Foundation laid for keyboard shortcuts and UI button icons
- Consistent with existing view switching patterns (shop, quest board, dialogue, etc.)

---

## Previous Update: Skill Tree UI

### SkillTreeView.xaml (New File)
- **Created Skill Tree user interface in Views folder**:
  - Full MVVM implementation with data binding
  - Dark theme matching existing UI style (#222233 background, #F9E97A accents)
- **XAML layout includes**:
  - **Header**: "Skill Tree" title in large gold text
  - **Skill Points Display**: Shows current available Skill Points for the player
  - **Skills List**: Scrollable ListBox bound to `AllSkills` collection
    - Each skill entry shows:
      - Skill Name (with [UNLOCKED] badge if already unlocked)
      - Description text
      - Level requirement and SP cost
      - Skill type (Attack, Defense, Heal) and power value
      - [Passive] or [Active] tag color-coded
    - Skills displayed in bordered cards with hover-friendly layout
  - **Unlock Button**: Bound to `UnlockSkillCommand`
    - Automatically disabled if requirements not met (level, SP cost)
  - **Close Button**: Bound to `ExitSkillTreeCommand` to return to previous view
- **Added TODO comments**:
  - `<!-- TODO: Add category tabs for Attack / Defense / Magic -->`
  - `<!-- TODO: Replace with icon grid layout and progress lines -->`

### SkillTreeView.xaml.cs (New File)
- **Created code-behind file**:
  - Simple UserControl initialization
  - No business logic (follows MVVM pattern)

### SkillTreeViewModel.cs (New File)
- **Created SkillTreeViewModel in ViewModels folder**:
  - Inherits from `BaseViewModel` for property change notifications
  - Full MVVM pattern implementation
- **Added properties**:
  - `Player Player`: Reference to current player for skill unlocking
  - `ObservableCollection<Skill> AllSkills`: Collection of all available skills
  - `Skill? SelectedSkill`: Tracks currently selected skill in the list
  - Property change notifications trigger command CanExecute updates
- **Constructor(Player player)**:
  - Loads `AllSkills = new ObservableCollection<Skill>(SkillTreeService.GetAllSkills())`
  - Sets `Player.AvailableSkills = AllSkills` to link player skills to service data
  - Initializes commands
- **Added commands**:
  - `UnlockSkillCommand`: Unlocks selected skill if requirements are met
    - Calls `Player.UnlockSkill(SelectedSkill)` for skill unlocking logic
    - Refreshes `Player.SkillPoints` display via property notifications
    - Auto-saves player progress after unlocking
    - CanExecute checks level requirement, skill point cost, and unlock status
  - `ExitSkillTreeCommand`: Fires `OnExitSkillTree` event to return to previous view
- **Added TODO comment**:
  - // TODO: Add skill preview popups and animations later

### InverseBooleanToVisibilityConverter.cs (New File)
- **Created inverse boolean to visibility converter**:
  - Converts `false` to `Visibility.Visible` and `true` to `Visibility.Collapsed`
  - Used for displaying [Active] tag when skill is not passive
  - Implements `IValueConverter` interface for XAML binding
  - Supports two-way conversion

### Technical Details
- Skill Tree UI fully integrated with existing skill system (Player, Skill, SkillTreeService)
- Player gains 1 skill point per level automatically (from Player.GainExperience)
- Skills are loaded from SkillTreeService and assigned to Player.AvailableSkills
- Skill unlock validation handled by Player.UnlockSkill method
- Player.UnlockSkill deducts skill points, marks skill as unlocked, and adds to LearnedSkills
- Auto-save ensures skill unlocks are persisted immediately
- Event-driven architecture allows parent ViewModels to handle view transitions via OnExitSkillTree
- Foundation laid for:
  - Category tabs (Attack, Defense, Magic specializations)
  - Visual skill tree with prerequisite lines and tier progression
  - Skill icons and animated unlock effects
  - Skill preview popups showing detailed information
  - Skill trainer NPCs offering region-specific skills
  - Quest rewards granting unique skills

---

## Previous Update: Skill Tree Service

### SkillTreeService.cs (New File)
- **Created static `SkillTreeService` class in Services folder**:
  - Provides centralized skill tree data management
  - Static class for easy access throughout the application
- **Added `GetAllSkills()` static method**:
  - Returns `List<Skill>` containing all available skills in the game
  - Includes three example skills:
    - **Power Strike**: Attack skill, +5 Power, Level 2, Cost 1 SP, "Attack" effect type
    - **Iron Skin**: Passive defense skill, +5 Power, Level 3, Cost 1 SP, "Defense" effect type
    - **Healing Light**: Healing skill, 15 HP restoration, Level 4, Cost 2 SP, "Heal" effect type
  - All skills initialized as unlocked=false by default
- **Added TODO comment**:
  - // TODO: Add skill tree tier data and class specialization later

### Technical Details
- Centralizes skill data for use by UI, player systems, and skill trainers
- Skills can be retrieved and added to player's AvailableSkills collection
- Foundation laid for:
  - Skill tree UI displaying available skills
  - Skill dependencies and prerequisites (e.g., "Requires Power Strike")
  - Class-specific skill branches (Warrior, Mage, Rogue)
  - Skill tier systems (Basic, Advanced, Master)
  - Skill trainers offering region-specific skills
  - Quest rewards granting unique skills
- Integrates seamlessly with existing Player.UnlockSkill() and Skill.cs systems
- Can be expanded to include dynamic skill generation or modding support

---

## Previous Update: Player Skill System Integration

### Player.cs Enhancements
- **Added skill system properties**:
  - `SkillPoints`: Integer property tracking available skill points for unlocking skills
  - Implements property change notifications for UI binding
  - Defaults to 0 on player creation
  - `LearnedSkills`: ObservableCollection<Skill> storing unlocked skills
  - `AvailableSkills`: ObservableCollection<Skill> storing skills available to unlock
  - Both collections initialized as empty and ready for skill management
- **Added `GainSkillPoints(int amount)` method**:
  - Simple method to award skill points: `SkillPoints += amount`
  - Can be called when player levels up or completes special quests
- **Added `UnlockSkill(Skill skill)` method**:
  - Checks if player has enough skill points: `SkillPoints >= skill.CostSP`
  - Verifies skill is not already unlocked: `!skill.IsUnlocked`
  - If conditions met:
    - Deducts skill point cost: `SkillPoints -= skill.CostSP`
    - Marks skill as unlocked: `skill.IsUnlocked = true`
    - Adds skill to LearnedSkills collection
    - Logs message: `"Unlocked skill: {skill.Name}"`
- **Enhanced `GainExperience(int amount)` method**:
  - Now awards 1 skill point per level gained
  - Calls `GainSkillPoints(1)` within level-up loop
  - Ensures players accumulate skill points as they progress
  - Integrated seamlessly with existing level-up rewards (HP, Attack, Defense)
- **Added TODO comment**:
  - // TODO: Add skill respec (reset) feature later

### Technical Details
- Complete skill point economy now integrated with player progression
- Players earn 1 skill point per level, providing consistent skill unlocking rate
- Skill unlock validation prevents spending more points than available
- LearnedSkills collection can be bound to UI for skill tree display
- AvailableSkills collection enables dynamic skill offerings (shop, trainer, quest rewards)
- Foundation laid for:
  - Skill tree UI with unlock buttons
  - Skill respec/reset functionality (refund skill points)
  - Skill trainers and skill books as quest rewards
  - Level-gated skill availability (using Skill.LevelRequirement)
  - Passive skill auto-application on unlock
  - Active skill hotbar integration in battle system
- System integrates with existing Skill.cs model for complete skill progression

---

## Previous Update: Skill System

### Skill.cs (New File)
- **Created `Skill` class in Models folder**:
  - Inherits from `BaseViewModel` for property change notifications
  - Full support for data binding in MVVM architecture
- **Added core skill properties**:
  - `Name`: The skill's display name
  - `Description`: Detailed description of what the skill does
  - `LevelRequirement`: Minimum player level required to unlock
  - `CostSP`: Skill Points required to unlock the skill
  - `Power`: Damage or bonus magnitude of the skill effect
  - `IsPassive`: Boolean indicating if skill is passive or active
  - `IsUnlocked`: Boolean tracking if player has unlocked this skill
  - `EffectType`: String defining skill category ("Attack", "Defense", "Heal", "Buff")
- **Added `ApplyEffect(Player player, string? target)` method**:
  - **Passive skill behavior**: Permanently modifies player stats
    - "Attack" effect: Increases player's Attack stat by Power
    - "Defense" effect: Increases player's Defense stat by Power
    - "Buff" effect: Increases both Attack and Defense by Power/2
    - Returns 0 for passive skills
  - **Active skill behavior**: Returns damage or healing value
    - "Attack" effect: Returns Power + player.Attack as damage
    - "Heal" effect: Restores HP (capped at MaxHP) and returns heal amount
    - "Defense" effect: Temporary defense boost (returns 0)
    - Default: Returns Power value
  - Accepts nullable enemy target parameter for active skills
- **Added TODO comments**:
  - // TODO: Add skill animations and icons later
  - // TODO: Add skill cooldown timers later

### Technical Details
- Skill system foundation laid for future skill tree and progression mechanics
- All properties implement property change notifications for UI binding
- Passive skills permanently enhance player stats (for equipped/learned skills)
- Active skills can deal damage, heal, or provide temporary buffs
- Enemy target parameter accepts null for self-targeted or passive skills
- Effect system is extensible - new effect types can be added easily
- Foundation laid for:
  - Skill tree UI and unlock progression
  - Skill Point economy and leveling rewards
  - Skill animations and visual effects
  - Cooldown system for active skills
  - Skill icons and tooltips
  - Combo system and skill synergies

---

## Previous Update: Complete Weather and Season System

### EnvironmentService.cs Enhancements
- **Added `Season` enum**:
  - Four seasons: Spring, Summer, Autumn, Winter
  - Used for tracking current season in the game world
- **Added `CurrentSeason` static property**:
  - Defaults to `Season.Spring`
  - Public getter with private setter for controlled season changes
- **Added `DaysPerSeason` constant**:
  - Set to 30 days per season
  - Defines the cycle length for seasonal transitions
- **Added `OnSeasonChanged` event**:
  - Event triggered when season changes
  - Passes the new Season as an event argument
  - Enables UI and gameplay systems to react to season changes
- **Added `UpdateSeason()` private method**:
  - Calculates season based on current day (30-day cycles)
  - Triggers OnSeasonChanged event when season transitions occur
  - Called automatically from UpdateLighting()
- **Enhanced `RandomizeWeather()` with seasonal probabilities**:
  - **Spring**: More rain and storms (40% Clear, 40% Rain, 15% Storm, 5% Fog)
  - **Summer**: Mostly clear with rare storms (70% Clear, 10% Rain, 15% Storm, 5% Fog)
  - **Autumn**: Foggy and rainy (40% Clear, 30% Rain, 20% Fog, 10% Storm)
  - **Winter**: Snow and fog (30% Clear, 10% Rain, 40% Snow, 15% Fog, 5% Storm)
  - Mountain regions have different seasonal patterns emphasizing snow
- **Added comprehensive TODO comments**:
  - Storm damage or travel delays
  - Seasonal events (harvest festival, snow day)
  - Region-based microclimates
  - Regional temperature data
  - Crop growth or festival triggers per season

### AudioService.cs Enhancements
- **Added weather-based ambient audio system**:
  - Separate `_weatherPlayer` for weather-specific sounds
  - Subscribes to `EnvironmentService.OnWeatherChanged` event
- **Added `PlayWeatherAmbience(WeatherType weather)` method**:
  - Clear → stops weather sounds
  - Rain → loops rain.wav
  - Storm → loops storm.wav
  - Snow → loops snow.wav
  - Fog → loops fog.wav
- **Added `OnWeatherChanged` event handler**:
  - Automatically switches weather ambience when weather changes
  - Properly disposes previous weather sounds before playing new ones
- **Added `PlayWeatherLoop(string fileName)` private method**:
  - Manages weather sound player lifecycle
  - Stops and disposes previous weather tracks before playing new ones

### SaveLoadService.cs Enhancements
- **Extended SaveData class with environment persistence**:
  - Added `Weather` string field for storing weather state
  - Added `Season` string field for storing season state
- **Enhanced SavePlayer() method**:
  - Serializes current weather and season to save file
  - Debug logging confirms environment state is saved
- **Enhanced LoadPlayer() method**:
  - Restores weather and season from save data using reflection
  - Uses Enum.TryParse to safely convert string values to enums
  - Sets EnvironmentService.Weather and CurrentSeason via reflection
  - Works for both new SaveData format and legacy format
  - Also restores environment from backup saves

### MapViewModel.cs Enhancements
- **Added `CurrentSeason` property**:
  - Binds to `EnvironmentService.CurrentSeason` for UI display
  - Provides real-time season data to the view
- **Subscribed to `EnvironmentService.OnSeasonChanged` event**:
  - Both constructors now subscribe to season change events
  - Added `OnSeasonChanged` handler method
- **Added `OnSeasonChanged` handler**:
  - Logs message to GlobalLog: "The season has changed to {newSeason}!"
  - Calls OnPropertyChanged to update UI display
  - Provides player feedback when seasons transition
- **Enhanced `UpdateEnvironmentColor()` method**:
  - Now combines time of day, weather, and season for dynamic lighting
  - **Weather effects**:
    - Rain/Storm: Darkens and adds blue tint
    - Snow: Brightens and adds white tint
    - Fog: Adds gray tint
  - **Seasonal tints**:
    - Spring: Slight green tint
    - Summer: Slight yellow/warm tint
    - Autumn: Slight orange tint
    - Winter: Slight blue/cool tint
- **Added TODO comment**:
  - Region-specific weather overrides (e.g., Desert = no rain)

### MapView.xaml Enhancements
- **Added season display to region header**:
  - Shows current season after time of day
  - Season text displayed in light green color
  - Uses data binding to `CurrentSeason` property in MapViewModel
- **Added weather visibility converters to resources**:
  - `RainVisibilityConverter` - shows rain effects only during rain
  - `SnowVisibilityConverter` - shows snow effects only during snow
  - `FogVisibilityConverter` - shows fog effects only during fog
- **Added Canvas overlay for weather effects**:
  - High Z-index (999) to appear above all other content
  - IsHitTestVisible=False to prevent interaction interference
  - **Rain effect placeholders**:
    - Vertical blue streaks (2px wide, 20px tall)
    - Semi-transparent (#6080FF, 60% opacity)
    - Multiple streaks positioned across the screen
  - **Snow effect placeholders**:
    - White circular dots (3-5px diameter)
    - Semi-transparent (80% opacity)
    - Multiple snowflakes positioned across the screen
  - **Fog effect placeholders**:
    - Semi-transparent gray rectangle (#808080, 30% opacity)
    - Covers entire screen area
- **Added comprehensive TODO comments**:
  - Season-based map recolors and decorations
  - Seasonal music themes
  - Replace placeholder effects with shader-based particle system
  - Add lightning flashes for storms
  - Add opacity animation for fog

### WeatherVisibilityConverter.cs (New File)
- **Created three weather visibility converters**:
  - `RainVisibilityConverter`: Visible for Rain weather, Collapsed otherwise
  - `SnowVisibilityConverter`: Visible for Snow weather, Collapsed otherwise
  - `FogVisibilityConverter`: Visible for Fog weather, Collapsed otherwise
  - All implement `IValueConverter` interface
  - Enable conditional visibility of weather effect overlays

### TimeService.cs Enhancements
- **Added TODO comments for future season features**:
  - Seasonal festivals and quests
  - Temperature effects for survival mechanics
  - (Note: Season logic implemented in EnvironmentService instead of TimeService)

### Technical Details
- Complete weather and season system with visual, audio, and persistence
- Seasons cycle every 30 in-game days (Spring → Summer → Autumn → Winter → repeat)
- Weather probabilities dynamically adjust based on current season and region type
- Environment lighting combines time of day, weather conditions, and seasonal tints
- Weather effects include placeholder visual overlays (rain streaks, snow dots, fog overlay)
- Audio system plays layered ambient sounds: time-based + weather-based
- Save/load system persists weather, season, time, and unlocked regions
- Event-driven architecture ensures real-time updates across all systems
- UI displays current season, weather (color-coded), day, and time
- Foundation laid for:
  - Animated particle-based weather systems
  - Storm damage and travel interruptions
  - Seasonal festivals and events
  - Region-specific microclimates and temperature zones
  - Crop growth mechanics tied to seasons

### Testing Checklist Completion
✅ 1. Observe initial weather and lighting → displays correctly with seasonal tints
✅ 2. Advance time → weather changes every 6 hours with seasonal variety
✅ 3. Confirm ambience audio updates → separate loops for time of day and weather
✅ 4. Observe lighting tint shifts → combines day/night, weather, and season
✅ 5. Progress through 30+ in-game days → seasons cycle properly (Spring → Summer → Autumn → Winter)
✅ 6. Confirm seasonal weather variety → Spring has more rain, Summer is clear, Autumn is foggy, Winter has snow
✅ 7. Save and reload → weather, season, time, and environment persist correctly
✅ 8. Weather effect overlays → placeholder visuals for rain, snow, and fog display based on weather type

---

## Previous Update: Weather Display UI

### MapView.xaml Enhancements
- **Added weather display to region header**:
  - New TextBlock showing current weather near the region/time header
  - Weather text is color-coded based on weather type:
    - Blue for Rain and Storm
    - White for Snow
    - Gray for Fog
    - Yellow for Clear
  - Uses data binding to `CurrentWeather` property in MapViewModel
- **Added TODO comments**:
  - `<!-- TODO: Add weather icons or animated particle layers -->`
  - `<!-- TODO: Add ambient rain and snow overlays -->`
- **Added WeatherColorConverter to resources**:
  - Registered new converter for use in weather display

### MapViewModel.cs Enhancements
- **Added `CurrentWeather` property**:
  - Binds to `EnvironmentService.Weather` for UI display
  - Provides real-time weather data to the view
- **Updated `OnWeatherChanged` handler**:
  - Now calls `OnPropertyChanged(nameof(CurrentWeather))` to notify UI
  - Ensures weather display updates immediately when weather changes
  - Continues to log weather changes to GlobalLog

### WeatherColorConverter.cs (New File)
- **Created new value converter**:
  - Converts `WeatherType` enum to appropriate color brushes
  - Implements `IValueConverter` interface
  - Color mapping:
    - `WeatherType.Rain` and `WeatherType.Storm` → Blue
    - `WeatherType.Snow` → White
    - `WeatherType.Fog` → Gray
    - `WeatherType.Clear` → Yellow
  - Default fallback color is White

### Technical Details
- Weather display integrates seamlessly with existing weather system
- UI updates automatically when weather changes (every 6 hours or on event)
- Color-coded weather provides visual feedback at a glance
- Foundation laid for future weather icons and animated particle effects

---

## Previous Update: TimeService Weather Integration

### TimeService.cs Enhancements
- **Added `CurrentRegionName` static property**:
  - Tracks the current region name for weather calculations
  - Defaults to "Default"
  - Set externally by MapViewModel when region changes
  - Enables region-aware weather changes every 6 hours
- **Subscribed to `OnHourChanged` event**:
  - Static constructor subscribes to the service's own OnHourChanged event
  - Enables automatic weather change handling
- **Added `HandleWeatherChanges` event handler**:
  - Checks if the current hour is divisible by 6 (0, 6, 12, 18)
  - Calls `EnvironmentService.RandomizeWeather()` with CurrentRegionName or "Default"
  - Automatically triggers weather changes every 6 hours as time advances
- **Added TODO comment**:
  - // TODO: Add gradual weather transitions and forecast system

### MapViewModel.cs Enhancements
- **Sets `TimeService.CurrentRegionName`**:
  - Updated region-specific constructor to set the current region name in TimeService
  - Ensures weather changes are region-appropriate
- **Subscribed to `EnvironmentService.OnWeatherChanged` event**:
  - Both constructors now subscribe to weather change events
  - Added `OnWeatherChanged` handler method
- **Added `OnWeatherChanged` handler**:
  - Logs message to GlobalLog: "The weather changes to {newWeather}."
  - Provides player feedback when weather changes occur
  - Integrates with existing event-driven architecture

### Technical Details
- Weather changes now fully automated through TimeService's event subscription
- Every 6 hours (at hours 0, 6, 12, 18), weather randomizes based on current region
- Region-aware weather patterns ensure appropriate weather for the player's location
- Event-driven architecture allows multiple systems to react to weather changes
- Weather log messages appear in the game's GlobalLog for player awareness
- System integrates seamlessly with existing time advancement from battles, resting, fast travel, and inn stays
- Foundation laid for gradual weather transitions and forecast systems in future updates

---

## Previous Update: Dynamic Weather System

### EnvironmentService.cs Enhancements
- **Added `WeatherType` enum**:
  - Five weather types: Clear, Rain, Storm, Snow, Fog
  - Used for tracking current weather conditions
- **Added `Weather` static property**:
  - Defaults to `WeatherType.Clear`
  - Public getter with private setter for controlled weather changes
- **Added `OnWeatherChanged` event**:
  - Event triggered when weather changes
  - Passes the new WeatherType as an event argument
  - Enables UI and gameplay systems to react to weather changes
- **Added `RandomizeWeather(string regionName)` method**:
  - Uses Random to pick weather based on region type
  - **Plains/Town weather probabilities**:
    - 60% Clear, 25% Rain, 10% Fog, 5% Storm
  - **Mountain weather probabilities**:
    - 40% Clear, 40% Snow, 20% Fog
  - Region type determined by parsing region name (plains, town, mountain, woods, etc.)
  - Sets Weather property and triggers OnWeatherChanged event when weather changes
- **Added automatic weather changes**:
  - Weather randomizes every 6 in-game hours
  - Integrated into `UpdateLighting()` method
  - Tracks hours since last weather change
- **Added helper method `DetermineRegionType(string regionName)`**:
  - Parses region names to categorize them (Mountain, Town, Plains)
  - Enables region-appropriate weather patterns
  - Woods/Forest regions use Plains weather probabilities
- **Added TODO comments**:
  - // TODO: Add localized weather (per-region tracking)
  - // TODO: Add weather-based enemy effects

### Technical Details
- Weather system is event-driven and integrates with existing time advancement
- Static Random instance ensures consistent probability distribution
- Weather changes automatically every 6 hours as time advances (battles, resting, fast travel, inn stays)
- Foundation laid for future weather-based gameplay mechanics (enemy buffs/debuffs, visual effects)
- Region-aware weather patterns make the world feel more dynamic and realistic
- Event architecture allows UI to display weather icons and effects to show weather visuals

---

## Previous Update: Time-Based Building Interior NPC Visibility

### BuildingInteriorViewModel.cs Enhancements
- **Added `VisibleOccupants` property**:
  - Filters NPCs based on time and building type
  - For home buildings (`IsHome = true`): NPCs appear when NOT available outside (i.e., at home during off-hours)
  - For non-home buildings (shops, inns): NPCs appear during their work hours
  - NPCs now automatically appear/disappear from buildings based on time of day
- **Subscribed to `TimeService.OnHourChanged` event**:
  - Automatically refreshes visible occupants when hour changes
  - Updates UI in real-time when time advances (from resting, traveling, or staying at inn)
- **Enhanced `StayAtInn()` method**:
  - Now calls `EnvironmentService.UpdateLighting()` after time advancement
  - Refreshes visible occupants after staying at inn
  - Ensures NPCs appear/disappear correctly after player rests
- **Memory management**:
  - Unsubscribes from time events in `ExitBuilding()` to prevent memory leaks

### BuildingInteriorView.xaml Enhancements
- **Updated occupant binding**:
  - Changed from `Occupants` to `VisibleOccupants` for time-aware NPC filtering
  - NPCs now only appear in buildings during appropriate hours
- **Added TODO**: Add lighting for building windows at night

### Technical Details
- NPCs at home buildings appear when outside their work schedule (e.g., Mira appears at home at night)
- NPCs at shop buildings appear during their work hours (e.g., Shopkeeper appears at shop during business hours)
- System integrates with existing time advancement from battles, resting, fast travel, and inn stays
- Event-driven architecture ensures real-time updates when time changes
- Completes the time-based NPC scheduling system across all game areas (map and building interiors)

### Testing Checklist Completion
✅ 1. Observe NPC availability during morning → some appear.
✅ 2. Rest or travel to advance hours → NPCs disappear or change.
✅ 3. Confirm environment color and ambient sounds change accordingly.
✅ 4. Enter buildings → verify NPCs at home at night.
✅ 5. Save and reload → time and environment persist.
✅ 6. Add TODO placeholders:
   - <!-- TODO: Add festivals and events on specific days -->
   - <!-- TODO: Add lighting for building windows at night -->
   - <!-- TODO: Add time-based shop opening hours -->

---

## Previous Update: Time Persistence in Save System

### SaveLoadService.cs Enhancements
- **Extended save data with time persistence**:
  - `SaveData` class already includes `Day` and `Hour` fields for time tracking
  - `SavePlayer()` serializes current `TimeService.Day` and `TimeService.Hour` to save file
  - Debug logging confirms time is saved with each save operation
- **Enhanced LoadPlayer() with time restoration**:
  - Restores `TimeService.Day` and `TimeService.Hour` from save data
  - **Triggers `EnvironmentService.UpdateLighting()`** after restoring time values
  - Ensures lighting state (Daylight/Twilight/Night) matches the loaded game time
  - Works for both new SaveData format and legacy Player-only format
  - Also triggers lighting update when loading from backup saves
- **Added TODO**: Add autosave on midnight or event triggers later

### Technical Details
- Time data is persisted in the `SaveData` wrapper class alongside Player and UnlockedRegions
- When loading, the time is restored first, then `UpdateLighting()` is called to sync the environment
- The lighting update ensures the visual state matches the loaded time (e.g., Night lighting at midnight)
- Backup save restoration also includes time recovery and lighting sync
- Foundation laid for future autosave triggers on time events (midnight, dawn, etc.)

---

## Previous Update: NPC Home Assignment System

### Building.cs Enhancements
- **Added `IsHome` property**: 
  - Boolean property to identify buildings that serve as NPC homes
  - Defaults to `false`
  - Can be set to `true` for buildings assigned as homes for specific NPCs

### WorldMapService.cs Enhancements
- **Assigned buildings as homes for specific NPCs**:
  - Mira's Home is marked as `IsHome = true` and assigned as Mira's home
  - General Shop is marked as `IsHome = true` and assigned as Shopkeeper's home
- **Updated NPC initialization**:
  - `mira.CurrentLocation` set to "Mira's Home"
  - `shopkeeper.CurrentLocation` set to "General Shop"
  - NPCs now have their home locations properly initialized
- **Added TODO**: Add NPC bedtime dialogues and idle animations

### Technical Details
- Buildings can now be designated as NPC homes using the `IsHome` property
- NPCs have their `CurrentLocation` property set to their home building name during initialization
- This enables future features like NPC pathfinding, schedules, and location-based interactions
- Foundation laid for bedtime routines and idle animations

---

## Previous Update: Time-Based NPC Scheduling System

### TimeService Enhancements
- **Added `OnHourChanged` event**: 
  - Event triggered whenever `AdvanceHours()` modifies the Hour property
  - Passes the new hour value as an event argument
  - Enables real-time reactions to time changes throughout the game
- **Event firing logic**: 
  - Compares old hour vs new hour before firing the event
  - Ensures event only fires when hour actually changes
  - Handles day rollover properly (when hour goes from 23 to 0)

### MapViewModel Enhancements
- **Subscribed to `TimeService.OnHourChanged` event**:
  - Both constructors now subscribe to the time change event
  - Automatically refreshes NearbyNPCs when hour changes
  - Updates time display in UI
- **New `OnHourChanged(object? sender, int newHour)` handler**:
  - Calls `RefreshTimeDisplay()` to update UI
  - Calls `RefreshNearbyNPCs()` to update NPC availability using `IsAvailableNow()`
  - Calls `LogTimedEvents(int hour)` to log time-specific messages
- **New `LogTimedEvents(int hour)` method**:
  - Logs contextual messages at specific times:
    - 6:00 AM: "The sun rises. Shops are opening for the day."
    - 12:00 PM: "It's noon. The sun is at its peak."
    - 8:00 PM: "Shops are closing for the night."
    - 12:00 AM: "Midnight strikes. The world grows quiet."
  - Added TODO: Add fade-to-night transitions for NPC sprites later

### Technical Details
- Event-driven architecture ensures NPCs automatically appear/disappear based on their schedules
- Existing `RefreshNearbyNPCs()` method already logs NPC arrivals and departures with context-aware messages
- Time-based events are now fully automated - no manual refresh needed
- System integrates seamlessly with existing time advancement from battles, resting, and fast travel

---

## Previous Update: Dynamic Ambient Audio System

### AudioService Enhancements
- **Added `PlayAmbientForLighting(string lighting)` method**: 
  - Plays ambient sounds based on lighting conditions
  - "Daylight" → birds.wav
  - "Twilight" → crickets.wav
  - "Night" → nightwind.wav
- **Integrated with EnvironmentService**: 
  - AudioService now subscribes to `EnvironmentService.OnLightingChanged` event
  - Automatically switches ambient loops when lighting changes (day/night cycle)
- **New private `PlayAmbientLoop(string fileName)` method**:
  - Manages ambient sound player lifecycle
  - Properly stops and disposes previous ambient tracks before playing new ones
- **Added TODO**: Add layered environmental tracks (rain, crowd chatter, wind)

### Technical Details
- Static constructor added to AudioService to initialize event subscription
- New static field `_ambientPlayer` for managing ambient sound playback
- Event-driven architecture ensures ambient sounds stay synchronized with the game's time/lighting system
