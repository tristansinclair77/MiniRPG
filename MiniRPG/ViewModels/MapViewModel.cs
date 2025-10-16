using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
            set { _selectedLocation = value; OnPropertyChanged(); }
        }

        private Item? _selectedInventoryItem;
        public Item? SelectedInventoryItem
        {
            get => _selectedInventoryItem;
            set { _selectedInventoryItem = value; OnPropertyChanged(); }
        }

        public ICommand StartBattleCommand { get; }
        public ICommand RestCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand UseItemCommand { get; }
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

        public MapViewModel(ObservableCollection<string> globalLog, Player player)
        {
            _globalLog = globalLog;
            Player = player;
            Locations = new ObservableCollection<string>
            {
                "Forest",
                "Cave",
                "Ruins"
            };
            StartBattleCommand = new RelayCommand(_ => StartBattle());
            RestCommand = new RelayCommand(_ => Rest());
            SaveCommand = new RelayCommand(async _ => await SaveGame());
            UseItemCommand = new RelayCommand(param => UseItem(param as Item));
        }

        private void StartBattle()
        {
            var msg = $"Starting battle at [{SelectedLocation}]";
            Debug.WriteLine(msg);
            _globalLog.Add(msg);
            OnStartBattle?.Invoke(SelectedLocation ?? "Unknown");
            // TODO: In future, connect to BattleViewModel and load enemy data
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
                Player.Inventory.Remove(item);
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
    }
}
