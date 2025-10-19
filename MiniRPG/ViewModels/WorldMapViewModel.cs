using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        /// Constructor for WorldMapViewModel.
        /// </summary>
        /// <param name="player">The player character</param>
        public WorldMapViewModel(Player player)
        {
            Player = player;
            
            // Load regions from WorldMapService
            Regions = new ObservableCollection<Region>(WorldMapService.GetRegions());

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
                OnRegionSelected?.Invoke(region);
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
    }
}
