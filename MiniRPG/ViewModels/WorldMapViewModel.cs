using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using MiniRPG.Models;
using MiniRPG.Services;

namespace MiniRPG.ViewModels
{
    /// <summary>
    /// ViewModel for WorldMapView. Manages region selection and travel.
    /// </summary>
    public class WorldMapViewModel : BaseViewModel
    {
        private ObservableCollection<Region> _regions;
        public ObservableCollection<Region> Regions
        {
            get => _regions;
            set
            {
                _regions = value;
                OnPropertyChanged();
            }
        }

        private Region? _selectedRegion;
        public Region? SelectedRegion
        {
            get => _selectedRegion;
            set
            {
                _selectedRegion = value;
                OnPropertyChanged();
                (TravelCommand as RelayCommand)?.RaiseCanExecuteChanged();
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

        public ICommand TravelCommand { get; }
        public ICommand ExitWorldMapCommand { get; }

        /// <summary>
        /// Event triggered when a region is selected for travel.
        /// </summary>
        public event Action<Region>? OnRegionSelected;

        /// <summary>
        /// Event triggered when exiting the world map.
        /// </summary>
        public event Action? OnExitWorldMap;

        /// <summary>
        /// Event triggered when a random encounter occurs during travel.
        /// </summary>
        public event Action<string>? OnRandomEncounter;

        /// <summary>
        /// Constructor for WorldMapViewModel.
        /// </summary>
        /// <param name="player">The player character</param>
        public WorldMapViewModel(Player player)
        {
            Player = player;
            
            // If no regions are unlocked yet, automatically unlock "Greenfield Town"
            if (FastTravelService.UnlockedRegions.Count == 0)
            {
                FastTravelService.UnlockRegion("Greenfield Town");
            }
            
            // Filter Regions list so only unlocked regions are visible
            Regions = new ObservableCollection<Region>(WorldMapService.GetRegions()
                .Where(r => FastTravelService.IsUnlocked(r.Name)));

            TravelCommand = new RelayCommand(param => Travel(param as Region), _ => CanTravel());
            ExitWorldMapCommand = new RelayCommand(_ => ExitWorldMap());
        }

        /// <summary>
        /// Determines if the player can travel to a region.
        /// </summary>
        private bool CanTravel()
        {
            // For now, always allow travel. Later add cost and unlock checks.
            return true;
        }

        /// <summary>
        /// Travel to the selected region.
        /// </summary>
        private void Travel(Region? region)
        {
            if (region != null)
            {
                SelectedRegion = region;
                Debug.WriteLine($"Traveling to {region.Name}...");
                
                // After player visits a new region, unlock it for fast travel
                FastTravelService.UnlockRegion(region.Name);
                
                // Check if a random encounter should occur
                if (EncounterService.ShouldTriggerEncounter())
                {
                    // Trigger random encounter event with region name
                    OnRandomEncounter?.Invoke(region.Name);
                }
                else
                {
                    // No encounter, proceed to region normally
                    OnRegionSelected?.Invoke(region);
                }
            }
        }

        /// <summary>
        /// Exit the world map.
        /// </summary>
        private void ExitWorldMap()
        {
            OnExitWorldMap?.Invoke();
        }

        // TODO: Add travel cost, animation, and random encounter system
        // TODO: Add unlocking through quests or story events
    }
}
