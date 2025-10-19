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

        public ICommand StartBattleCommand { get; }
        public ICommand RestCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand UseItemCommand { get; }
        public ICommand EquipItemCommand { get; }
        public ICommand OpenShopCommand { get; }
        public ICommand OpenQuestBoardCommand { get; }
        public ICommand TalkToNPCCommand { get; }
        public ICommand OpenWorldMapCommand { get; }
        private ObservableCollection<string> _globalLog;

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
            
            // Populate NearbyNPCs with all NPCs from DialogueService
            NearbyNPCs = new ObservableCollection<NPC>(DialogueService.GetAllNPCs());
            
            LocalEnemies = new ObservableCollection<string>();
            RegionQuests = new ObservableCollection<Quest>();
            
            StartBattleCommand = new RelayCommand(_ => StartBattle(), _ => !string.IsNullOrEmpty(this.SelectedLocation));
            RestCommand = new RelayCommand(_ => Rest());
            SaveCommand = new RelayCommand(async _ => await SaveGame());
            UseItemCommand = new RelayCommand(param => UseItem(param as Item));
            EquipItemCommand = new RelayCommand(param => EquipItem(param as Item));
            OpenShopCommand = new RelayCommand(_ => OpenShop());
            OpenQuestBoardCommand = new RelayCommand(_ => OpenQuestBoard());
            TalkToNPCCommand = new RelayCommand(param => TalkToNPC(param as NPC));
            OpenWorldMapCommand = new RelayCommand(_ => OpenWorldMap());
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
            
            // Load region-specific content
            NearbyNPCs = region.NPCs;
            LocalEnemies = region.AvailableEnemies;
            RegionQuests = region.LocalQuests;
            
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
            
            // TODO: Add visual background per region
            // TODO: Add weather or time-of-day changes
        }

        private void StartBattle()
        {
            var msg = $"Starting battle at [{SelectedLocation}]";
            Debug.WriteLine(msg);
            _globalLog.Add(msg);
            OnStartBattle?.Invoke(SelectedLocation ?? "Unknown");
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

        private async void Rest()
        {
            Player.HP = Player.MaxHP;
            OnPropertyChanged(nameof(Player));
            _globalLog?.Add("You rest and recover all HP.");
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
