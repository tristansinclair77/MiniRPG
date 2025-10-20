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
                OnPropertyChanged(nameof(VisibleOccupants));
                OnPropertyChanged(nameof(IsInn));
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

        private bool _isSleeping;
        public bool IsSleeping
        {
            get => _isSleeping;
            set
            {
                _isSleeping = value;
                OnPropertyChanged();
            }
        }

        public string BuildingName => CurrentBuilding?.Name ?? "Unknown Building";
        public string BuildingDescription => CurrentBuilding?.Description ?? "No description available.";
        
        /// <summary>
        /// Returns occupants filtered by time - NPCs at home appear at night, others during their work hours.
        /// </summary>
        public ObservableCollection<NPC> VisibleOccupants
        {
            get
            {
                if (CurrentBuilding == null || CurrentBuilding.Occupants == null)
                    return new ObservableCollection<NPC>();

                // If this is a home building, show NPCs when they're NOT available outside (i.e., at home time)
                if (CurrentBuilding.IsHome)
                {
                    // NPCs are home when they're outside their work hours
                    return new ObservableCollection<NPC>(
                        CurrentBuilding.Occupants.Where(npc => !npc.IsAvailableNow())
                    );
                }
                else
                {
                    // For non-home buildings (shops, inns), show NPCs during their work hours
                    return new ObservableCollection<NPC>(
                        CurrentBuilding.Occupants.Where(npc => npc.IsAvailableNow())
                    );
                }
            }
        }
        
        public bool IsInn => CurrentBuilding?.Type == "Inn";

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
        public ICommand? StayAtInnCommand { get; }
        public ICommand? OpenCraftingCommand { get; }

        // Event callbacks
        public event Action<NPC>? OnTalkToNPC;
        public event Action? OnExitBuilding;
        public event Action? OnOpenCrafting;

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
            
            // Subscribe to time changes to update visible NPCs
            TimeService.OnHourChanged += OnHourChanged;
            
            TalkToNPCCommand = new RelayCommand(
                param => TalkToNPC(param as NPC),
                _ => SelectedNPC != null
            );
            
            ExitBuildingCommand = new RelayCommand(_ => ExitBuilding());
            
            // Add StayAtInnCommand if this is an inn
            if (CurrentBuilding.Type == "Inn")
            {
                StayAtInnCommand = new RelayCommand(_ => StayAtInn());
            }
            
            // Add OpenCraftingCommand if this is a Blacksmith or Workshop
            if (CurrentBuilding.Type == "Blacksmith" || CurrentBuilding.Type == "Workshop")
            {
                OpenCraftingCommand = new RelayCommand(_ => OpenCrafting());
            }
            
            // Add room-based navigation and building-specific events
            // TODO: Add NPC bedtime dialogues and idle animations
        }

        /// <summary>
        /// Handles hour changes and refreshes visible occupants.
        /// </summary>
        private void OnHourChanged(object? sender, int newHour)
        {
            OnPropertyChanged(nameof(VisibleOccupants));
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
            // Unsubscribe from time changes
            TimeService.OnHourChanged -= OnHourChanged;
            // TODO: Add fade-to-black transition between scenes
            OnExitBuilding?.Invoke();
        }

        private void OpenCrafting()
        {
            OnOpenCrafting?.Invoke();
        }

        private async void StayAtInn()
        {
            const int cost = 20;
            if (Player.Gold >= cost)
            {
                Player.SpendGold(cost);
                
                // Play sleep sound effect
                try { AudioService.PlaySleep(); } catch { }
                
                // Display "Sleeping..." overlay for 2 seconds
                IsSleeping = true;
                await Task.Delay(2000);
                
                // After delay, restore HP, advance time, and hide overlay
                Player.HP = Player.MaxHP;
                TimeService.AdvanceHours(8);
                EnvironmentService.UpdateLighting();
                IsSleeping = false;
                
                Debug.WriteLine("You rest at the inn and feel refreshed.");
                
                // Update music for new time of day after resting
                try { AudioService.UpdateMusicForTime(); } catch { }
                
                // Refresh visible occupants after time change
                OnPropertyChanged(nameof(VisibleOccupants));
            }
            else
            {
                Debug.WriteLine("You can't afford a room.");
            }
        }

        // TODO: Add choice of standard/luxury rooms and longer rest durations
    }
}
