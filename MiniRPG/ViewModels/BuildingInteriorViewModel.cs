using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using MiniRPG.Models;

namespace MiniRPG.ViewModels
{
    /// <summary>
    /// ViewModel for BuildingInteriorView. Manages building interior interactions.
    /// </summary>
    public class BuildingInteriorViewModel : BaseViewModel
    {
        private Building _currentBuilding;
        public Building CurrentBuilding
        {
            get => _currentBuilding;
            set
            {
                _currentBuilding = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BuildingName));
                OnPropertyChanged(nameof(BuildingDescription));
                OnPropertyChanged(nameof(Occupants));
            }
        }

        private Player _player;
        public Player Player
        {
            get => _player;
            set
            {
                _player = value;
                OnPropertyChanged();
            }
        }

        public string BuildingName => CurrentBuilding?.Name ?? "Unknown Building";
        public string BuildingDescription => CurrentBuilding?.Description ?? "No description available.";
        public ObservableCollection<NPC> Occupants => CurrentBuilding?.Occupants ?? new ObservableCollection<NPC>();

        private NPC? _selectedNPC;
        public NPC? SelectedNPC
        {
            get => _selectedNPC;
            set
            {
                _selectedNPC = value;
                OnPropertyChanged();
                (TalkToNPCCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand TalkToNPCCommand { get; }
        public ICommand ExitBuildingCommand { get; }

        // Event callbacks
        public event Action<NPC>? OnTalkToNPC;
        public event Action? OnExitBuilding;

        /// <summary>
        /// Constructor for BuildingInteriorViewModel.
        /// </summary>
        /// <param name="building">The building to display interior for</param>
        /// <param name="player">The player character</param>
        public BuildingInteriorViewModel(Building building, Player player)
        {
            CurrentBuilding = building;
            Player = player;
            
            Debug.WriteLine($"Entered {building.Name}.");
            
            TalkToNPCCommand = new RelayCommand(
                param => TalkToNPC(param as NPC),
                _ => SelectedNPC != null
            );
            
            ExitBuildingCommand = new RelayCommand(_ => ExitBuilding());
            
            // Add room-based navigation and building-specific events
        }

        private void TalkToNPC(NPC? npc)
        {
            if (npc != null)
            {
                OnTalkToNPC?.Invoke(npc);
            }
        }

        private void ExitBuilding()
        {
            // TODO: Add fade-to-black transition between scenes
            OnExitBuilding?.Invoke();
        }
    }
}
