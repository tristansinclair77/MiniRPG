---

## MainWindow.xaml Layout Update

- Replaced the default Grid in MainWindow.xaml with a modern WPF layout for SimpleRPG prototype.
- Added a top MenuBar with two buttons: "Map" and "Battle".
- Inserted a main ContentControl below the MenuBar to switch between MapView and BattleView.
- Added a bottom multi-line, read-only, scrollable TextBox for combat logs/messages.
- Used proper Grid row definitions and named each control for clarity.
- Commented XAML to indicate where images or art assets can be inserted.

---

## BaseViewModel Added

- Created a new C# class named BaseViewModel in the ViewModels folder.
- Implements INotifyPropertyChanged and provides a protected OnPropertyChanged(string propertyName) method.
- Follows standard MVVM pattern for property change notification.

---

## MainViewModel and Supporting Classes Added

- Created MainViewModel.cs in the ViewModels folder.
  - Inherits from BaseViewModel.
  - Adds CurrentViewModel property (BaseViewModel).
  - Adds ShowMapCommand and ShowBattleCommand properties using RelayCommand.
  - Commands switch CurrentViewModel between new MapViewModel() and new BattleViewModel().
  - Comments added for future transitions/animations.
- Implemented RelayCommand in ViewModels folder for MVVM command binding.
- Added placeholder MapViewModel and BattleViewModel classes inheriting from BaseViewModel.

---

## RelayCommand Refactored

- Refactored RelayCommand in ViewModels folder:
  - Implements ICommand for MVVM button bindings.
  - Constructor accepts Action<object?> execute and optional Func<object?, bool>? canExecute.
  - Implements CanExecuteChanged, CanExecute, and Execute with proper null checking.
  - Added comments for future parameter expansion and context support.

---

## MapView UserControl Added

- Created MapView.xaml and MapView.xaml.cs as a new UserControl.
  - Displays a label "Choose a Battle Location".
  - ListBox named LocationList bound to Locations collection in MapViewModel.
  - "Fight!" button bound to StartBattleCommand.
  - Uses Grid layout and includes comment for future background map art.
- Updated MapViewModel to implement Locations collection and StartBattleCommand for binding.

---

## MapViewModel Updated

- Updated MapViewModel in ViewModels folder:
  - Inherits from BaseViewModel.
  - Adds ObservableCollection<string> Locations, prefilled with "Forest", "Cave", "Ruins".
  - Adds SelectedLocation property.
  - Adds StartBattleCommand (RelayCommand).
  - When executed, logs "Starting battle at [SelectedLocation]" using Debug.WriteLine.
  - Added TODO comment for future connection to BattleViewModel and enemy data loading.

---

## BattleView UserControl Added

- Created BattleView.xaml and BattleView.xaml.cs as a new UserControl.
  - Top TextBlock shows "Battle in Progress".
  - Center StackPanel with three Buttons: "Attack", "Defend", "Run".
  - Bottom TextBox (read-only, multi-line) bound to CombatLog.
  - Comments added for future character/enemy art.
  - All elements named and bound for later logic.
- Updated BattleViewModel to implement CombatLog property and commands for Attack, Defend, Run.

---

## BattleViewModel Refactored

- Refactored BattleViewModel in ViewModels folder:
  - Inherits from BaseViewModel.
  - Adds ObservableCollection<string> CombatLog.
  - Adds three RelayCommands: AttackCommand, DefendCommand, RunCommand.
  - Each command adds a new line to CombatLog describing the action, with random damage/block values.
  - Uses System.Random for simple randomization.
  - Includes TODO comments for future HP tracking and enemy AI logic.

---

## BattleViewModel Uses GameService

- BattleViewModel now uses GameService for enemy and damage logic.
  - Sets CurrentEnemy = GameService.GetRandomEnemy() on initialization.
  - AttackCommand uses GameService.CalculateDamage() and logs: "You hit {CurrentEnemy} for {dmg} damage!"
  - DefendCommand logs: "You brace for the next attack!"
  - RunCommand logs: "You fled from battle!" and disables further actions.
  - TODO added for future enemy turn and HP logic.

---

## BattleViewModel Battle Logic Added

- Added PlayerHP (default 30), EnemyHP (default 20), CurrentEnemy, and IsBattleOver properties.
- AttackCommand now deals damage, updates HP, logs results, and checks for victory/defeat.
- DefendCommand reduces next incoming damage by 50%.
- RunCommand ends battle and logs escape.
- EnemyAttack method handles enemy turn and defeat logic.
- Added TODO for future Stat object implementation.

---

## MainWindow MVVM Binding and DataTemplates (Fixed Namespaces)

- Set DataContext = new MainViewModel() in MainWindow.xaml.cs constructor.
- Added DataTemplates for MapViewModel and BattleViewModel in MainWindow.xaml Window.Resources using correct namespaces:
  - xmlns:vm="clr-namespace:MiniRPG.ViewModels"
  - xmlns:v="clr-namespace:MiniRPG"
  - <DataTemplate DataType="{x:Type vm:MapViewModel}"><v:MapView /></DataTemplate>
  - <DataTemplate DataType="{x:Type vm:BattleViewModel}"><v:BattleView /></DataTemplate>
- Bound ContentControl to CurrentViewModel for view switching.
- Added comments for future fade transitions between views.

---

## GlobalLog Integration and Log Binding

- MainViewModel now includes ObservableCollection<string> GlobalLog and AddLog(string message) method.
- MapViewModel and BattleViewModel receive GlobalLog reference and append messages to it.
- MainWindow.xaml bottom log is now a ListBox bound to GlobalLog, read-only, multi-line, with a comment for future pixel-art styling.

---

## GameService Utility Class Added

- Created GameService.cs in Services folder.
  - Static method GetRandomEnemy() returns a random enemy name from a list ("Slime", "Goblin", "Wolf").
  - Static method CalculateDamage() returns a random int (1–10).
  - TODO comments for future expansion: enemy stats, player stats, battle rewards.

---

## GameService Random Loot Support

- Added GetRandomLoot() to GameService:
  - 50% chance to return a random Item from Item.GetSampleItems(), otherwise returns null.
  - TODO for rarity tiers and loot tables.

---

## MainWindow TODO Placeholders Added

- Added TODO comment placeholders at the top of MainWindow.xaml for:
  - Background music
  - Player sprite
  - Enemy sprite
  - Location art

---

## BattleView HP Bars and Art TODOs

- Added Player HP and Enemy HP ProgressBars to BattleView, bound to PlayerHP and EnemyHP, with Maximums matching ViewModel defaults.
- Kept Attack/Defend/Run buttons and CombatLog box.
- Added TODO comments for animated health bars, pixel-art overlays, and player/enemy portraits.

---

## MapViewModel and MainViewModel Event Integration

- MapViewModel now raises an OnStartBattle event when StartBattleCommand is executed.
- MainViewModel subscribes to OnStartBattle, creates a new BattleViewModel, sets CurrentViewModel, and logs entry.
- TODO added for fade transition and music change between map and battle.

---

## Player Model Added

- Created Player.cs in Models folder.
  - Properties: Name, HP, MaxHP, Attack, Defense.
  - Constructor sets defaults (HP = 30, Attack = 5, Defense = 2).
  - TODO for future inventory, experience, and leveling system.

---

## BattleViewModel Uses Player Model

- Added Player property to BattleViewModel, initialized in constructor.
- Replaced PlayerHP references with Player.HP and Player.MaxHP.
- Updated damage logic to subtract from Player.HP and check for defeat.
- Bound PlayerHPBar in BattleView.xaml to Player.HP and Maximum to Player.MaxHP.
- Added TODO for persisting Player object between battles.

---

## UI Tidy: MainWindow.xaml & BattleView.xaml

- Added light background color (#222233) to both MainWindow and BattleView.
- Styled all Buttons with Margin="5" and Padding="8,4".
- Added Border around CombatLog areas for both views.
- Added TODO comment placeholders for art assets, music/audio triggers, and future SceneManager/Animation layer.

---

## Battle End Event & MainViewModel Handling

- BattleViewModel now exposes BattleEnded event, invoked with "Victory" or "Defeat".
- MainViewModel subscribes to BattleEnded, logs result, waits 1 second, and switches back to MapViewModel.
- TODO added for future rewards and experience points after victory.

---

## BattleView Overlay for Result

- Added BattleResult property to BattleViewModel, set to "Victory" or "Defeat" when battle ends.
- BattleView.xaml now displays a centered overlay TextBlock bound to BattleResult, visible when IsBattleOver is true.
- Overlay uses large bold font and color (LimeGreen for victory, Red for defeat) via ResultToColorConverter.
- Added TODO comments for future animated transitions and art-based victory screens.

---

## BattleView: Return to Map Button

- Added 'Return to Map' button to BattleView overlay, bound to ReturnToMapCommand in BattleViewModel.
- Button is visible when IsBattleOver is true and calls BattleEnded?.Invoke("Return") when clicked.
- Added TODO for animated return transition to Map screen.

---

## BattleViewModel: ReturnToMapCommand Logging & TODO

- ReturnToMapCommand now logs "Returning to map..." to CombatLog and GlobalLog.
- Sets IsBattleOver = true to ensure overlay/button visibility.
- Added TODO for hooking into save/load for persistent battle results.

---

## BooleanToVisibilityConverter Added

- Created Converters folder and added BooleanToVisibilityConverter.cs.
  - Implements IValueConverter to convert true → Visibility.Visible, false → Visibility.Collapsed.
  - Includes TODO for future game state UI converters.

---

## BooleanToVisibilityConverter in App.xaml & BattleView.xaml

- Added BooleanToVisibilityConverter to App.xaml resources and defined xmlns:converters.
- Updated BattleView.xaml to use BoolToVis for overlay/button visibility and added xmlns:converters.
- Added TODO for future state-based ViewModel system.

---

## Testing Checklist TODOs Added

- Added TODO placeholders to MainWindow.xaml:
  - <!-- TODO: Add save system -->
  - <!-- TODO: Add inventory and level-up after victory -->

---

## Player Leveling System Added

- Player.cs now includes:
  - int Level (default 1)
  - int Experience (default 0)
  - int ExperienceToNextLevel (default 10)
  - GainExperience(int amount) method for experience and level-up logic.
  - Level-up increases stats and restores HP.
  - TODO for future level-up animation, ability points, and save to file.

---

## BattleViewModel Experience & Level-Up

- When the player defeats an enemy:
  - Awards random experience (5–15).
  - Calls Player.GainExperience and logs experience gain.
  - If leveled up, logs new level and stat increase.
  - Calls BattleEnded?.Invoke("Victory") after.
  - TODO for loot drops and enemy-specific EXP scaling.

---

## Player Persistence in MainViewModel & BattleViewModel

- MainViewModel now has a public property: Player CurrentPlayer { get; set; } = new Player();
- When creating BattleViewModel, passes the same Player instance for persistent stats and HP.
- BattleViewModel constructor updated to accept and assign Player.
- TODO added for save/load player data to file system.

---

## MapView Player Info Display

- MapView.xaml now displays player info at the top:
  - Level, HP/MaxHP, EXP/ExperienceToNextLevel (bound to Player in MapViewModel).
- MapViewModel accepts Player in constructor and exposes it for binding.
- MainViewModel passes CurrentPlayer to MapViewModel.
- Added TODO comments for pixel-art status window and animated portrait/player sprite.

---

## MapViewModel Player Binding & TODOs

- MapViewModel now has a constructor accepting Player and exposes it for binding.
- MainViewModel passes CurrentPlayer to MapViewModel for player info display.
- Added TODOs for currency, inventory, and gear tabs next.

---

## MapView Rest Command Added

- MapViewModel now includes RestCommand:
  - Sets Player.HP to Player.MaxHP, notifies property changed, and logs recovery.
- MapView.xaml adds a "Rest" button under player info, bound to RestCommand.
- Added TODO for future inn scene and cost-based healing.

---

## MainWindow Card-Style Layout & TODOs

- Added card-style Border behind main ContentControl with proper margin and spacing.
- Used darker background for log area and added margin for spacing.
- Added placeholder TODO comments for pixel-art UI, main menu/title screen, and background music manager.

---

## Final Testing Checklist TODOs Added

- Added TODO placeholders to MainWindow.xaml:
  - <!-- TODO: Save Player to JSON file -->
  - <!-- TODO: Add Load feature on startup -->
  - <!-- TODO: Add main menu scene -->

---

## SaveLoadService Added

- Created SaveLoadService.cs in Services folder.
  - Uses System.Text.Json for serialization.
  - SavePlayer serializes Player to JSON and writes to file.
  - LoadPlayer reads file and deserializes Player from JSON.
  - Includes exception handling and TODO for saving inventory, map progress, and settings.

---

## MainViewModel Save/Load Integration

- On construction, attempts to load existing player via SaveLoadService.LoadPlayer().
- If no save exists, creates a new Player.
- Adds SaveCommand using RelayCommand to save player data.
- TODO for auto-save after each battle or major event.

---

## MapView Save Game Button Added

- Added 'Save Game' button next to 'Rest' in MapView.xaml, bound to SaveCommand in MainViewModel using RelativeSource.
- Added TODO for auto-save indicator and custom UI slot menu.

---

## BattleViewModel Autosave on Victory

- When battle ends in victory:
  - Calls SaveLoadService.SavePlayer(Player).
  - Appends "Progress saved!" to CombatLog and GlobalLog.
  - TODO for autosave indicator animation on screen.

---

## App Startup Save/Load Logic

- On Application_Startup, checks for player_save.json and loads Player if available, otherwise creates new Player.
- Injects Player into MainViewModel.
- TODO for proper title screen with "New Game" or "Continue".

---

## MapView Save Confirmation Message

- Added IsSaveConfirmed property and SaveCommand to MapViewModel.
- SaveCommand sets IsSaveConfirmed true, hides after 2 seconds.
- MapView.xaml displays 'Game Saved!' message below Save button, bound to IsSaveConfirmed.
- Added TODO for pixel-art popup animation.

---

## MapViewModel Save Confirmation Animation Logic

- IsSaveConfirmed property now set in Rest and Save actions.
- Added async HideSaveConfirmation() method to hide message after 2 seconds.
- TODO for future animation system for save confirmation message.

---

## Final Save/Load & Main Menu TODOs Added

- Added TODO placeholders to MainWindow.xaml:
  - <!-- TODO: Encrypt save file -->
  - <!-- TODO: Add multiple save slots -->
  - <!-- TODO: Add "Continue" option on main menu -->

---

## TitleView UserControl Added

- Created TitleView.xaml and TitleView.xaml.cs.
- XAML layout: large centered title text ("Simple RPG") and two large buttons ("New Game" and "Continue") bound to NewGameCommand and ContinueCommand.
- All elements centered in a Grid.
- Added TODO comments for logo art, background music, intro scene, and button animation.

---

## TitleViewModel Added

- Created TitleViewModel.cs in ViewModels folder.
- Inherits from BaseViewModel.
- Adds NewGameCommand and ContinueCommand (RelayCommand).
- Adds event TitleSelectionMade, invoked on command execution.
- TODO for fade transition and intro dialogue.

---

## MainViewModel Title Screen Integration

- On startup, sets CurrentViewModel = new TitleViewModel().
- Subscribes to TitleSelectionMade event:
  - If "New": creates new Player and MapViewModel, logs "A new adventure begins!"
  - If "Continue": loads Player from save or creates new, logs "Welcome back!"
- Sets CurrentViewModel to MapViewModel after selection.
- Keeps GlobalLog persistent.
- TODO for intro cutscene on new game start.

---

## TitleViewModel DataTemplate & Animated Fade TODO

- Added DataTemplate for TitleViewModel in MainWindow.xaml resources to load TitleView.
- Added TODO for animated fade between title and map.

---

## AudioService Added

- Created AudioService.cs in Services folder.
- Provides PlayTitleTheme, PlayMapTheme, and PlayBattleTheme methods using System.Media.SoundPlayer and placeholder .wav files.
- Checks file existence before playing.
- TODO for cross-fade audio engine.

---

## MainViewModel AudioService Integration

- Calls AudioService.PlayTitleTheme() when showing TitleViewModel.
- Calls AudioService.PlayMapTheme() when switching to MapViewModel.
- Calls AudioService.PlayBattleTheme() when switching to BattleViewModel.
- Uses try/catch to prevent exceptions if audio files are missing.
- TODO for smooth fade transitions between tracks.

---

## TitleView Keyboard Shortcuts & ToolTips

- Added KeyBindings to TitleView:
  - Enter triggers NewGameCommand
  - C triggers ContinueCommand
- Added ToolTips to buttons showing shortcuts.
- Added TODO for keyboard navigation and controller input.

---

## Title Screen & Save/Load Testing TODOs Added

- Added TODO placeholders to MainWindow.xaml:
  - <!-- TODO: Add splash screen -->
  - <!-- TODO: Add animated intro text -->
  - <!-- TODO: Add save-slot selection -->

---

## Item Model Added

- Created Item.cs in Models folder.
- Item class has Name, Description, Type, and Value properties.
- Constructor for quick setup.
- Static GetSampleItems() returns a few basic items.
- TODO for item effects and rarity expansion.

---

## Player Inventory Support Added

- Player now has ObservableCollection<Item> Inventory property.
- AddItem(Item item) method adds to inventory and logs to Debug.
- Inventory is serializable with SaveLoadService.
- TODO for inventory capacity and sorting.

---

## BattleViewModel Loot Drop Integration

- After awarding EXP on victory:
  - Calls GameService.GetRandomLoot().
  - If loot is found, adds to Player inventory and logs the item.
  - Otherwise, logs that no items were found.
  - Calls SaveLoadService.SavePlayer(Player) after loot assignment.
  - TODO for visual loot display in post-battle summary.

---

## MapView Inventory UI Added

- Added Expander in MapView.xaml for Inventory, bound to Player.Inventory.
- Displays each item's Name and Type; shows Description for selected item.
- Added SelectedInventoryItem property to MapViewModel for selection binding.
- TODOs for pixel-art inventory icons and category tabs.

---

## MapViewModel: UseItemCommand Added

- Added UseItemCommand (RelayCommand) to MapViewModel.
- Command accepts an Item as parameter.
- When executed:
  - If Item.Type == "Consumable" and Item.Name == "Potion":
    - Sets Player.HP = Math.Min(Player.MaxHP, Player.HP + 10).
    - Removes the item from Player.Inventory.
    - Logs "You used a Potion and recovered 10 HP." to GlobalLog.
  - Otherwise, logs "That item cannot be used now." to GlobalLog.
- Added TODO comment for future targeting and status effects logic.

---

## MapView.xaml: Use Item Button Added

- Added a Button labeled "Use Item" under the Inventory ListBox in MapView.xaml.
- Button's Command is bound to UseItemCommand.
- CommandParameter is bound to the SelectedItem of the Inventory ListBox.
- Added TODO comment for future contextual radial menu or drag/drop inventory UI.

---

## MapViewModel: UseItemCommand Autosave Added

- Modified UseItemCommand in MapViewModel.cs:
  - After successfully using an item, calls SaveLoadService.SavePlayer(Player).
  - Sets IsSaveConfirmed = true to show save confirmation message.
  - Awaits HideSaveConfirmation to reset confirmation after delay.
  - Added TODO for future autosave toggle option.

---

## Testing Checklist & TODO Placeholders Added

- Testing checklist for inventory, loot, persistence, item use, and save confirmation:
  1. Defeat enemies to collect random loot.
  2. Verify inventory updates in MapView.
  3. Close and reopen app — ensure items persist.
  4. Use potion items to heal and confirm they disappear after use.
  5. Check save confirmation message appears.
- Added TODO placeholders:
  - <!-- TODO: Add equipment and stat bonuses -->
  - <!-- TODO: Add item shop -->
  - <!-- TODO: Add crafting and material combination -->

---

## UI Readability Improvements

- Updated TitleView.xaml:
  - New Game and Continue buttons now use dark text (#222233) on a gold background (#F9E97A) for high contrast.
- Updated MapView.xaml:
  - All buttons and important labels use gold text (#F9E97A) on dark backgrounds (#222233).
- Updated BattleView.xaml:
  - All action buttons and status text use gold text (#F9E97A) on dark backgrounds (#222233).
- Updated MainWindow.xaml:
  - Global button style now uses gold text (#F9E97A) on dark backgrounds (#222233) and bold font for visibility.
- Ensured all UI elements have sufficient contrast for readability.

---

## Bug Fix: Inventory ListBox XAML Error

- Fixed XamlParseException in MapView.xaml by removing DisplayMemberPath from InventoryListBox when using ItemTemplate.
- Inventory now displays correctly and the Continue button error is resolved.

---

## Bug Fix: Fight Button Now Switches to BattleView

- Updated MainViewModel.cs:
  - Subscribed to MapViewModel.OnStartBattle in both New and Continue game flows.
  - Clicking Fight now properly switches to BattleView and starts a battle.
