using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MiniRPG.Models;
using MiniRPG.Services;

namespace MiniRPG.ViewModels
{
    /// <summary>
    /// ViewModel for MapView. Holds battle locations and command to start battle.
    /// </summary>
    public class MapViewModel : BaseViewModel
    {
        private ObservableCollection<string> _locations;
        public ObservableCollection<string> Locations
        {
            get => _locations;
            set { _locations = value; OnPropertyChanged(); }
        }

        private string? _selectedLocation;
        public string? SelectedLocation
        {
            get => _selectedLocation;
            set
            {
                _selectedLocation = value;
                OnPropertyChanged();
                (StartBattleCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private Item? _selectedInventoryItem;
        public Item? SelectedInventoryItem
        {
            get => _selectedInventoryItem;
            set { _selectedInventoryItem = value; OnPropertyChanged(); }
        }

        private ObservableCollection<NPC> _nearbyNPCs;
        public ObservableCollection<NPC> NearbyNPCs
        {
            get => _nearbyNPCs;
            set { _nearbyNPCs = value; OnPropertyChanged(); }
        }

        private string _regionName;
        public string RegionName
        {
            get => _regionName;
            set { _regionName = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets the current day from TimeService for UI binding.
        /// </summary>
        public int CurrentDay => TimeService.Day;

        /// <summary>
        /// Gets the current time of day from TimeService for UI binding.
        /// </summary>
        public string TimeOfDay => TimeService.GetTimeOfDay();

        private ObservableCollection<string> _localEnemies;
        public ObservableCollection<string> LocalEnemies
        {
            get => _localEnemies;
            set { _localEnemies = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Quest> _regionQuests;
        public ObservableCollection<Quest> RegionQuests
        {
            get => _regionQuests;
            set { _regionQuests = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Building> _regionBuildings;
        public ObservableCollection<Building> RegionBuildings
        {
            get => _regionBuildings;
            set { _regionBuildings = value; OnPropertyChanged(); }
        }

        private Building? _selectedBuilding;
        public Building? SelectedBuilding
        {
            get => _selectedBuilding;
            set
            {
                _selectedBuilding = value;
                OnPropertyChanged();
                (EnterBuildingCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private string? _selectedFastTravelRegion;
        public string? SelectedFastTravelRegion
        {
            get => _selectedFastTravelRegion;
            set
            {
                _selectedFastTravelRegion = value;
                OnPropertyChanged();
                (FastTravelCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<string> UnlockedRegions => FastTravelService.UnlockedRegions;

        public ICommand StartBattleCommand { get; }
        public ICommand RestCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand UseItemCommand { get; }
        public ICommand EquipItemCommand { get; }
        public ICommand OpenShopCommand { get; }
        public ICommand OpenQuestBoardCommand { get; }
        public ICommand TalkToNPCCommand { get; }
        public ICommand OpenWorldMapCommand { get; }
        public ICommand OpenFastTravelCommand { get; }
        public ICommand FastTravelCommand { get; }
        public ICommand EnterBuildingCommand { get; }
        private ObservableCollection<string> _globalLog;
        
        // Store the full region reference to filter NPCs
        private Region? _region;
        
        // Store the last hour to detect significant time changes
        private int _lastHour;

        private bool _isSaveConfirmed;
        public bool IsSaveConfirmed
        {
            get => _isSaveConfirmed;
            set { _isSaveConfirmed = value; OnPropertyChanged(); }
        }

        public Player Player { get; }

        // Event/callback for starting battle
        public event Action<string>? OnStartBattle;
        
        // Event/callback for opening shop
        public event Action? OnOpenShop;
        
        // Event/callback for opening quest board
        public event Action? OnOpenQuestBoard;
        
        // Event/callback for talking to NPC
        public event Action<NPC>? OnTalkToNPC;
        
        // Event/callback for opening world map
        public event Action? OnOpenWorldMap;

        // Event/callback for opening fast travel
        public event Action? OnOpenFastTravel;

        // Event/callback for fast travel
        public event Action<string>? OnFastTravel;

        // Event/callback for entering building
        public event Action<Building>? OnEnterBuilding;

        /// <summary>
        /// Constructor for MapViewModel (legacy - uses default data).
        /// </summary>
        public MapViewModel(ObservableCollection<string> globalLog, Player player)
        {
            _globalLog = globalLog;
            Player = player;
            RegionName = "Unknown Region";
            Locations = new ObservableCollection<string>
            {
                "Forest",
                "Cave",
                "Ruins"
            };
            
            // Populate NearbyNPCs with all NPCs from DialogueService (filtered by availability)
            var allNPCs = DialogueService.GetAllNPCs();
            NearbyNPCs = new ObservableCollection<NPC>(allNPCs.Where(npc => npc.IsAvailableNow()));
            
            LocalEnemies = new ObservableCollection<string>();
            RegionQuests = new ObservableCollection<Quest>();
            RegionBuildings = new ObservableCollection<Building>();
            
            _lastHour = TimeService.Hour;
            
            StartBattleCommand = new RelayCommand(_ => StartBattle(), _ => !string.IsNullOrEmpty(this.SelectedLocation));
            RestCommand = new RelayCommand(_ => Rest());
            SaveCommand = new RelayCommand(async _ => await SaveGame());
            UseItemCommand = new RelayCommand(param => UseItem(param as Item));
            EquipItemCommand = new RelayCommand(param => EquipItem(param as Item));
            OpenShopCommand = new RelayCommand(_ => OpenShop());
            OpenQuestBoardCommand = new RelayCommand(_ => OpenQuestBoard());
            TalkToNPCCommand = new RelayCommand(param => TalkToNPC(param as NPC));
            OpenWorldMapCommand = new RelayCommand(_ => OpenWorldMap());
            OpenFastTravelCommand = new RelayCommand(_ => OpenFastTravel());
            FastTravelCommand = new RelayCommand(param => FastTravel(param as string), _ => !string.IsNullOrEmpty(SelectedFastTravelRegion));
            EnterBuildingCommand = new RelayCommand(param => EnterBuilding(param as Building));
        }

        /// <summary>
        /// Constructor for MapViewModel with region-specific content.
        /// </summary>
        /// <param name="globalLog">The global log for game messages</param>
        /// <param name="player">The player character</param>
        /// <param name="region">The region to load content from</param>
        public MapViewModel(ObservableCollection<string> globalLog, Player player, Region region)
        {
            _globalLog = globalLog;
            Player = player;
            RegionName = region.Name;
            _region = region;
            
            // Load region-specific content with NPC filtering by availability
            NearbyNPCs = new ObservableCollection<NPC>(region.NPCs.Where(npc => npc.IsAvailableNow()));
            LocalEnemies = region.AvailableEnemies;
            RegionQuests = region.LocalQuests;
            RegionBuildings = region.Buildings;
            
            _lastHour = TimeService.Hour;
            
            // Use region enemies as battle locations, fallback to default if none
            if (region.AvailableEnemies != null && region.AvailableEnemies.Any())
            {
                Locations = new ObservableCollection<string>(region.AvailableEnemies);
            }
            else
            {
                Locations = new ObservableCollection<string>
                {
                    "Forest",
                    "Cave",
                    "Ruins"
                };
            }
            
            StartBattleCommand = new RelayCommand(_ => StartBattle(), _ => !string.IsNullOrEmpty(this.SelectedLocation));
            RestCommand = new RelayCommand(_ => Rest());
            SaveCommand = new RelayCommand(async _ => await SaveGame());
            UseItemCommand = new RelayCommand(param => UseItem(param as Item));
            EquipItemCommand = new RelayCommand(param => EquipItem(param as Item));
            OpenShopCommand = new RelayCommand(_ => OpenShop());
            OpenQuestBoardCommand = new RelayCommand(_ => OpenQuestBoard());
            TalkToNPCCommand = new RelayCommand(param => TalkToNPC(param as NPC));
            OpenWorldMapCommand = new RelayCommand(_ => OpenWorldMap());
            OpenFastTravelCommand = new RelayCommand(_ => OpenFastTravel());
            FastTravelCommand = new RelayCommand(param => FastTravel(param as string), _ => !string.IsNullOrEmpty(SelectedFastTravelRegion));
            EnterBuildingCommand = new RelayCommand(param => EnterBuilding(param as Building));
            
            // TODO: Add visual background per region
            // TODO: Add weather or time-of-day changes
            // TODO: Add time-based events and NPC schedules
            // TODO: Add NPC pathfinding or animated map icons later
        }

        /// <summary>
        /// Refreshes the time-related properties to update the UI after time changes.
        /// </summary>
        public void RefreshTimeDisplay()
        {
            OnPropertyChanged(nameof(CurrentDay));
            OnPropertyChanged(nameof(TimeOfDay));
            
            // Check if hour has changed significantly and refresh NPCs
            if (TimeService.Hour != _lastHour)
            {
                RefreshNearbyNPCs();
                _lastHour = TimeService.Hour;
            }
        }
        
        /// <summary>
        /// Refreshes the NearbyNPCs list based on current time, adding log messages for changes.
        /// </summary>
        private void RefreshNearbyNPCs()
        {
            if (_region == null)
            {
                // Legacy mode: filter all NPCs from DialogueService
                var allNPCs = DialogueService.GetAllNPCs();
                var previousNPCs = NearbyNPCs.ToList();
                var availableNPCs = allNPCs.Where(npc => npc.IsAvailableNow()).ToList();
                
                // Find NPCs that disappeared
                var disappeared = previousNPCs.Where(prev => !availableNPCs.Any(curr => curr.Name == prev.Name)).ToList();
                foreach (var npc in disappeared)
                {
                    _globalLog?.Add(GetNPCDisappearanceMessage(npc));
                }
                
                // Find NPCs that appeared
                var appeared = availableNPCs.Where(curr => !previousNPCs.Any(prev => prev.Name == curr.Name)).ToList();
                foreach (var npc in appeared)
                {
                    _globalLog?.Add(GetNPCAppearanceMessage(npc));
                }
                
                NearbyNPCs = new ObservableCollection<NPC>(availableNPCs);
            }
            else
            {
                // Region mode: filter NPCs from the region
                var previousNPCs = NearbyNPCs.ToList();
                var availableNPCs = _region.NPCs.Where(npc => npc.IsAvailableNow()).ToList();
                
                // Find NPCs that disappeared
                var disappeared = previousNPCs.Where(prev => !availableNPCs.Any(curr => curr.Name == prev.Name)).ToList();
                foreach (var npc in disappeared)
                {
                    _globalLog?.Add(GetNPCDisappearanceMessage(npc));
                }
                
                // Find NPCs that appeared
                var appeared = availableNPCs.Where(curr => !previousNPCs.Any(prev => prev.Name == curr.Name)).ToList();
                foreach (var npc in appeared)
                {
                    _globalLog?.Add(GetNPCAppearanceMessage(npc));
                }
                
                NearbyNPCs = new ObservableCollection<NPC>(availableNPCs);
            }
        }
        
        /// <summary>
        /// Generates an appropriate log message when an NPC disappears.
        /// </summary>
        private string GetNPCDisappearanceMessage(NPC npc)
        {
            // Generate context-aware messages based on NPC role and time of day
            if (npc.Role == "Merchant" || npc.Role == "Shopkeeper")
            {
                return $"{npc.Name} has closed shop for the day.";
            }
            else if (TimeService.Hour >= 20 || TimeService.Hour < 6)
            {
                return $"{npc.Name} has gone home for the night.";
            }
            else if (TimeService.Hour >= 6 && TimeService.Hour < 12)
            {
                return $"{npc.Name} has left for the morning.";
            }
            else
            {
                return $"{npc.Name} is no longer around.";
            }
        }
        
        /// <summary>
        /// Generates an appropriate log message when an NPC appears.
        /// </summary>
        private string GetNPCAppearanceMessage(NPC npc)
        {
            // Generate context-aware messages based on NPC role and time of day
            if (npc.Role == "Merchant" || npc.Role == "Shopkeeper")
            {
                return $"{npc.Name} has opened for the morning.";
            }
            else if (TimeService.Hour >= 6 && TimeService.Hour < 12)
            {
                return $"{npc.Name} has arrived for the morning.";
            }
            else if (TimeService.Hour >= 12 && TimeService.Hour < 18)
            {
                return $"{npc.Name} has appeared for the afternoon.";
            }
            else
            {
                return $"{npc.Name} has arrived.";
            }
        }

        private void StartBattle()
        {
            var msg = $"Starting battle at [{SelectedLocation}]";
            Debug.WriteLine(msg);
            _globalLog.Add(msg);
            
            // Advance time by 1 hour for battle
            TimeService.AdvanceHours(1);
            RefreshTimeDisplay();
            
            // Pass the region name to the battle system for region-aware enemy selection
            OnStartBattle?.Invoke(RegionName ?? "Unknown");
            // TODO: In future, connect to BattleViewModel and load enemy data
        }

        private void OpenShop()
        {
            OnOpenShop?.Invoke();
        }

        private void OpenQuestBoard()
        {
            OnOpenQuestBoard?.Invoke();
        }

        private void TalkToNPC(NPC? npc)
        {
            if (npc != null)
            {
                OnTalkToNPC?.Invoke(npc);
            }
        }

        private void OpenWorldMap()
        {
            OnOpenWorldMap?.Invoke();
        }

        private void OpenFastTravel()
        {
            _globalLog?.Add("Opening fast travel menu...");
            // TODO: Replace temporary menu with town gate NPC or airship terminal UI
            OnOpenFastTravel?.Invoke();
        }

        private void FastTravel(string? regionName)
        {
            if (!string.IsNullOrEmpty(regionName))
            {
                // Advance time by 2 hours for travel
                TimeService.AdvanceHours(2);
                RefreshTimeDisplay();
                
                _globalLog?.Add($"Fast traveling to {regionName}... It is now Day {CurrentDay}, {TimeOfDay}.");
                OnFastTravel?.Invoke(regionName);
            }
        }

        private void EnterBuilding(Building? building)
        {
            if (building != null)
            {
                OnEnterBuilding?.Invoke(building);
            }
        }

        private async void Rest()
        {
            // Advance time by 8 hours for resting
            TimeService.AdvanceHours(8);
            RefreshTimeDisplay();
            
            Player.HP = Player.MaxHP;
            OnPropertyChanged(nameof(Player));
            _globalLog?.Add($"You rest and recover all HP. It is now Day {CurrentDay}, {TimeOfDay}.");
            IsSaveConfirmed = true;
            await HideSaveConfirmation();
            // TODO: Replace with inn scene and cost-based healing later
        }

        private async Task SaveGame()
        {
            SaveLoadService.SavePlayer(Player);
            IsSaveConfirmed = true;
            await HideSaveConfirmation();
            // TODO: Replace with pixel-art popup animation for future version
        }

        private async Task HideSaveConfirmation()
        {
            await Task.Delay(2000);
            IsSaveConfirmed = false;
            // Later - use animation system for this message
        }

        private async void UseItem(Item? item)
        {
            if (item == null)
            {
                _globalLog.Add("No item selected.");
                return;
            }
            if (item.Type == "Consumable" && item.Name == "Potion")
            {
                int oldHP = Player.HP;
                Player.HP = Math.Min(Player.MaxHP, Player.HP + 10);
                Player.RemoveItem(item);
                OnPropertyChanged(nameof(Player));
                _globalLog.Add("You used a Potion and recovered 10 HP.");
                SaveLoadService.SavePlayer(Player);
                IsSaveConfirmed = true;
                await HideSaveConfirmation();
                // TODO: // Add autosave toggle option later
            }
            else
            {
                _globalLog.Add("That item cannot be used now.");
            }
            // TODO: Add targeting and status effects later
        }

        private void EquipItem(Item? item)
        {
            if (item == null)
            {
                _globalLog.Add("No item selected.");
                return;
            }

            if (item.IsEquippable)
            {
                bool equipped = Player.EquipItem(item);
                if (equipped)
                {
                    _globalLog.Add($"You equipped {item.Name}.");
                }
                else
                {
                    _globalLog.Add("Cannot equip this item.");
                }
            }
            else
            {
                _globalLog.Add("That item is not equippable.");
            }

            // After equipping, save the player data
            SaveLoadService.SavePlayer(Player);
            // TODO: // Add equipment change sound effect later
        }
    }
}
