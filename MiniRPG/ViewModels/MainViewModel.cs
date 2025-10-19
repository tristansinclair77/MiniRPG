using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using MiniRPG.Models;
using MiniRPG.Services;

namespace MiniRPG.ViewModels
{
    /// <summary>
    /// MainViewModel manages the current view, global log, and commands for switching views.
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private BaseViewModel _currentViewModel;
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                if (_currentViewModel != value)
                {
                    _currentViewModel = value;
                    OnPropertyChanged();
                    // Future: Insert transition/animation logic here when switching views
                }
            }
        }

        public ObservableCollection<string> GlobalLog { get; } = new();

        public Player CurrentPlayer { get; set; }

        public ICommand ShowMapCommand { get; }
        public ICommand ShowBattleCommand { get; }
        public ICommand SaveCommand { get; }

        private Region? _currentRegion;
        private Region? _targetRegion; // Store the target region when encounter happens

        public MainViewModel()
        {
            // Show title screen on startup
            var titleVM = new TitleViewModel();
            titleVM.TitleSelectionMade += OnTitleSelectionMade;
            CurrentViewModel = titleVM;
            try { AudioService.PlayTitleTheme(); } catch { }

            ShowMapCommand = new RelayCommand(_ => ShowMap());
            ShowBattleCommand = new RelayCommand(_ => ShowBattle());
            SaveCommand = new RelayCommand(_ => SaveLoadService.SavePlayer(CurrentPlayer));
        }

        private void OnTitleSelectionMade(string selection)
        {
            if (selection == "New")
            {
                CurrentPlayer = new Player();
                var mapVM = CreateMapViewModel();
                CurrentViewModel = mapVM;
                try { AudioService.PlayMapTheme(); } catch { }
                AddLog("A new adventure begins!");
                // TODO: Add intro cutscene for new game start
            }
            else if (selection == "Continue")
            {
                var loadedPlayer = SaveLoadService.LoadPlayer();
                CurrentPlayer = loadedPlayer ?? new Player();
                
                // If player has a saved region, try to load it
                if (!string.IsNullOrEmpty(CurrentPlayer.LastRegionName))
                {
                    var regions = WorldMapService.GetRegions();
                    var savedRegion = regions.FirstOrDefault(r => r.Name == CurrentPlayer.LastRegionName);
                    if (savedRegion != null)
                    {
                        _currentRegion = savedRegion;
                        AddLog($"Resuming adventure in {savedRegion.Name}...");
                        
                        // If player has a saved building, auto-load that building's interior view
                        if (!string.IsNullOrEmpty(CurrentPlayer.LastBuildingName))
                        {
                            var savedBuilding = savedRegion.Buildings?.FirstOrDefault(b => b.Name == CurrentPlayer.LastBuildingName);
                            if (savedBuilding != null)
                            {
                                // Load building interior
                                var buildingVM = CreateBuildingInteriorViewModel(savedBuilding);
                                CurrentViewModel = buildingVM;
                                try { AudioService.PlayBuildingTheme(savedBuilding.Type); } catch { }
                                AddLog($"You are in {savedBuilding.Name}.");
                                return; // Exit early to avoid loading map view
                            }
                        }
                    }
                }
                
                var mapVM = CreateMapViewModel();
                CurrentViewModel = mapVM;
                
                // Update music based on loaded time of day
                try { AudioService.UpdateMusicForTime(); } catch { }
                
                AddLog("Welcome back!");
            }
        }

        private MapViewModel CreateMapViewModel()
        {
            MapViewModel mapVM;
            
            // If a region is selected, create MapViewModel with region-specific content
            if (_currentRegion != null)
            {
                mapVM = new MapViewModel(GlobalLog, CurrentPlayer, _currentRegion);
                AddLog($"You are now in {_currentRegion.Name}.");
            }
            else
            {
                // Use default constructor (legacy behavior)
                mapVM = new MapViewModel(GlobalLog, CurrentPlayer);
            }
            
            // Subscribe to battle events
            mapVM.OnStartBattle += async location =>
            {
                var battleVM = new BattleViewModel(GlobalLog, CurrentPlayer, location);
                battleVM.BattleEnded += async result =>
                {
                    AddLog($"Battle ended with result: {result}");
                    // TODO: Later, implement rewards and experience points after victory
                    await Task.Delay(1000);
                    ShowMap();
                };
                CurrentViewModel = battleVM;
                try { AudioService.PlayBattleTheme(); } catch { }
                AddLog($"Entering battle at {location}");
                // TODO: Later - fade transition and music change between map and battle
            };
            
            // Subscribe to shop events
            mapVM.OnOpenShop += () =>
            {
                var shopVM = new ShopViewModel(CurrentPlayer, GlobalLog);
                shopVM.ExitShopCommand = new RelayCommand(_ => ShowMap());
                CurrentViewModel = shopVM;
                AddLog("You enter the shop.");
            };
            
            // Subscribe to quest board events
            mapVM.OnOpenQuestBoard += () =>
            {
                var questBoardVM = new QuestBoardViewModel(CurrentPlayer);
                questBoardVM.ExitBoardCommand = new RelayCommand(_ => ShowMap());
                CurrentViewModel = questBoardVM;
                AddLog("You approach the quest board.");
                // TODO: Add NPCs offering quests directly later
            };
            
            // Subscribe to TalkToNPC events
            mapVM.OnTalkToNPC += selectedNPC =>
            {
                var dialogueVM = new DialogueViewModel(selectedNPC, CurrentPlayer, GlobalLog);
                dialogueVM.OnDialogueExit += () => ShowMap();
                CurrentViewModel = dialogueVM;
                AddLog($"You approach {selectedNPC.Name}.");
            };
            
            // Subscribe to EnterBuilding events
            mapVM.OnEnterBuilding += selectedBuilding =>
            {
                var buildingVM = CreateBuildingInteriorViewModel(selectedBuilding);
                CurrentViewModel = buildingVM;
                try { AudioService.PlayBuildingTheme(selectedBuilding.Type); } catch { }
                
                // Save player state when entering building
                CurrentPlayer.LastBuildingName = selectedBuilding.Name;
                SaveLoadService.SavePlayer(CurrentPlayer);
                
                AddLog($"You enter {selectedBuilding.Name}.");
                // TODO: Add animated fade transition between outdoor and indoor views
            };
            
            // Subscribe to OpenWorldMap events
            mapVM.OnOpenWorldMap += () =>
            {
                var worldMapVM = new WorldMapViewModel(CurrentPlayer);
                
                // Subscribe to random encounter event
                worldMapVM.OnRandomEncounter += regionName =>
                {
                    // Store the target region for after battle
                    var regions = WorldMapService.GetRegions();
                    _targetRegion = regions.FirstOrDefault(r => r.Name == regionName);
                    
                    // Create battle with region-specific enemy
                    var battleVM = new BattleViewModel(GlobalLog, CurrentPlayer, regionName);
                    battleVM.BattleEnded += async result =>
                    {
                        AddLog($"Battle ended with result: {result}");
                        await Task.Delay(1000);
                        
                        // After battle ends, proceed to target region
                        if (_targetRegion != null)
                        {
                            _currentRegion = _targetRegion;
                            CurrentPlayer.LastRegionName = _targetRegion.Name;
                            SaveLoadService.SavePlayer(CurrentPlayer);
                            AddLog($"Continuing journey to {_targetRegion.Name}...");
                            _targetRegion = null; // Clear target region
                        }
                        
                        ShowMap();
                    };
                    
                    CurrentViewModel = battleVM;
                    try { AudioService.PlayBattleTheme(); } catch { }
                    AddLog("A wild enemy appears during travel!");
                    // TODO: Add encounter animations and travel interruption visuals
                };
                
                worldMapVM.OnRegionSelected += selectedRegion =>
                {
                    _currentRegion = selectedRegion;
                    
                    // Save the region name to player and trigger auto-save
                    CurrentPlayer.LastRegionName = selectedRegion.Name;
                    SaveLoadService.SavePlayer(CurrentPlayer);
                    
                    AddLog($"Traveling to {selectedRegion.Name}...");
                    // TODO: Add animated fade-out/fade-in transitions between regions
                    // TODO: Add music change and environment effect system
                    
                    // Update music based on time of day when entering region
                    try { AudioService.UpdateMusicForTime(); } catch { }
                    
                    ShowMap();
                };
                worldMapVM.OnExitWorldMap += () => ShowMap();
                CurrentViewModel = worldMapVM;
                AddLog("You open the world map.");
            };
            
            // Subscribe to OpenFastTravel events
            mapVM.OnOpenFastTravel += () =>
            {
                // TODO: Replace temporary menu with town gate NPC or airship terminal UI
                var worldMapVM = new WorldMapViewModel(CurrentPlayer);
                
                // Subscribe to random encounter event
                worldMapVM.OnRandomEncounter += regionName =>
                {
                    // Store the target region for after battle
                    var regions = WorldMapService.GetRegions();
                    _targetRegion = regions.FirstOrDefault(r => r.Name == regionName);
                    
                    // Create battle with region-specific enemy
                    var battleVM = new BattleViewModel(GlobalLog, CurrentPlayer, regionName);
                    battleVM.BattleEnded += async result =>
                    {
                        AddLog($"Battle ended with result: {result}");
                        await Task.Delay(1000);
                        
                        // After battle ends, proceed to target region
                        if (_targetRegion != null)
                        {
                            _currentRegion = _targetRegion;
                            CurrentPlayer.LastRegionName = _targetRegion.Name;
                            SaveLoadService.SavePlayer(CurrentPlayer);
                            AddLog($"Continuing journey to {_targetRegion.Name}...");
                            _targetRegion = null; // Clear target region
                        }
                        
                        ShowMap();
                    };
                    
                    CurrentViewModel = battleVM;
                    try { AudioService.PlayBattleTheme(); } catch { }
                    AddLog("A wild enemy appears during travel!");
                    // TODO: Add encounter animations and travel interruption visuals
                };
                
                worldMapVM.OnRegionSelected += selectedRegion =>
                {
                    _currentRegion = selectedRegion;
                    
                    // Save the region name to player and trigger auto-save
                    CurrentPlayer.LastRegionName = selectedRegion.Name;
                    SaveLoadService.SavePlayer(CurrentPlayer);
                    
                    AddLog($"Traveling to {selectedRegion.Name}...");
                    // TODO: Add animated fade-out/fade-in transitions between regions
                    // TODO: Add music change and environment effect system
                    
                    // Update music based on time of day when entering region
                    try { AudioService.UpdateMusicForTime(); } catch { }
                    
                    ShowMap();
                };
                worldMapVM.OnExitWorldMap += () => ShowMap();
                CurrentViewModel = worldMapVM;
                AddLog("You open the fast travel menu.");
            };
            
            // Subscribe to fast travel events
            mapVM.OnFastTravel += regionName =>
            {
                var regions = WorldMapService.GetRegions();
                var targetRegion = regions.FirstOrDefault(r => r.Name == regionName);
                
                if (targetRegion != null && FastTravelService.IsUnlocked(regionName))
                {
                    _currentRegion = targetRegion;
                    CurrentPlayer.LastRegionName = targetRegion.Name;
                    SaveLoadService.SavePlayer(CurrentPlayer);
                    
                    // Update music based on time of day when entering region
                    try { AudioService.UpdateMusicForTime(); } catch { }
                    
                    AddLog($"Fast traveled to {targetRegion.Name}!");
                    ShowMap();
                }
                else
                {
                    AddLog("Cannot fast travel to that region.");
                }
            };
            
            // Subscribe to OnOpenSkillTree events
            mapVM.OnOpenSkillTree += () =>
            {
                var skillTreeVM = new SkillTreeViewModel(CurrentPlayer);
                skillTreeVM.OnExitSkillTree += () =>
                {
                    ShowMap();
                    AddLog("Returned from Skill Tree.");
                };
                CurrentViewModel = skillTreeVM;
                AddLog("Opened Skill Tree.");
            };
            
            return mapVM;
        }

        private BuildingInteriorViewModel CreateBuildingInteriorViewModel(Building selectedBuilding)
        {
            var buildingVM = new BuildingInteriorViewModel(selectedBuilding, CurrentPlayer);
            
            // Subscribe to talk to NPC inside building
            buildingVM.OnTalkToNPC += npc =>
            {
                var dialogueVM = new DialogueViewModel(npc, CurrentPlayer, GlobalLog);
                dialogueVM.OnDialogueExit += () =>
                {
                    // Return to building interior after dialogue
                    CurrentViewModel = new BuildingInteriorViewModel(selectedBuilding, CurrentPlayer);
                    AddLog($"Returned to {selectedBuilding.Name}.");
                    
                    // Re-subscribe to events
                    var resubscribedBuildingVM = CurrentViewModel as BuildingInteriorViewModel;
                    if (resubscribedBuildingVM != null)
                    {
                        resubscribedBuildingVM.OnTalkToNPC += npc =>
                        {
                            var innerDialogueVM = new DialogueViewModel(npc, CurrentPlayer, GlobalLog);
                            innerDialogueVM.OnDialogueExit += () =>
                            {
                                // Clear building when exiting via dialogue
                                CurrentPlayer.LastBuildingName = null;
                                SaveLoadService.SavePlayer(CurrentPlayer);
                                
                                ShowMap();
                                try { AudioService.PlayMapTheme(); } catch { }
                            };
                            CurrentViewModel = innerDialogueVM;
                            AddLog($"You talk to {npc.Name}.");
                        };
                        resubscribedBuildingVM.OnExitBuilding += () =>
                        {
                            // Clear building when exiting
                            CurrentPlayer.LastBuildingName = null;
                            SaveLoadService.SavePlayer(CurrentPlayer);
                            
                            ShowMap();
                            try { AudioService.PlayMapTheme(); } catch { }
                        };
                    }
                };
                CurrentViewModel = dialogueVM;
                AddLog($"You talk to {npc.Name}.");
            };
            
            // Subscribe to exit building event
            buildingVM.OnExitBuilding += () =>
            {
                // Clear building when exiting
                CurrentPlayer.LastBuildingName = null;
                SaveLoadService.SavePlayer(CurrentPlayer);
                
                ShowMap();
                try { AudioService.PlayMapTheme(); } catch { }
            };
            
            return buildingVM;
        }

        public void AddLog(string message)
        {
            GlobalLog.Add(message);
            OnPropertyChanged(nameof(GlobalLog));
            OnPropertyChanged(nameof(CombinedLog));
        }

        public string CombinedLog => string.Join("\n", GlobalLog);

        private void ShowMap()
        {
            var mapVM = CreateMapViewModel();
            CurrentViewModel = mapVM;
            try { AudioService.PlayMapTheme(); } catch { }
            AddLog("Switched to MapView");
            // Future: Insert transition/animation logic here for MapView
        }
        // TODO: Add currency, inventory, and gear tabs next
        // TODO: Add shop icons on map and custom merchant types later
        // TODO: Add NPC proximity detection and movement system later

        private void ShowBattle()
        {
            var battleVM = new BattleViewModel(GlobalLog, CurrentPlayer);
            battleVM.BattleEnded += async result =>
            {
                AddLog($"Battle ended with result: {result}");
                // TODO: Later, implement rewards and experience points after victory
                await Task.Delay(1000);
                ShowMap();
            };
            CurrentViewModel = battleVM;
            try { AudioService.PlayBattleTheme(); } catch { }
            AddLog("Switched to BattleView");
            // Future: Insert transition/animation logic here for BattleView
        }
        // TODO: Add intro cutscene for new game start
        // TODO: Add currency, inventory, and gear tabs next
        // TODO: Later - add save/load player data to file system
        // TODO: Later - trigger auto-save after each battle or major event
        // TODO: Replace with smooth fade transitions between tracks
        // TODO: Add animated fade-out/fade-in transitions between regions
        // TODO: Add music change and environment effect system
        // TODO: Add encounter animations and travel interruption visuals
    }
}
