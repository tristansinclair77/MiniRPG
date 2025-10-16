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
                var mapVM = new MapViewModel(GlobalLog, CurrentPlayer);
                mapVM.OnStartBattle += async location =>
                {
                    var battleVM = new BattleViewModel(GlobalLog, CurrentPlayer);
                    // Optionally pass location to BattleViewModel here
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
                CurrentViewModel = mapVM;
                try { AudioService.PlayMapTheme(); } catch { }
                AddLog("A new adventure begins!");
                // TODO: Add intro cutscene for new game start
            }
            else if (selection == "Continue")
            {
                var loadedPlayer = SaveLoadService.LoadPlayer();
                CurrentPlayer = loadedPlayer ?? new Player();
                var mapVM = new MapViewModel(GlobalLog, CurrentPlayer);
                mapVM.OnStartBattle += async location =>
                {
                    var battleVM = new BattleViewModel(GlobalLog, CurrentPlayer);
                    // Optionally pass location to BattleViewModel here
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
                CurrentViewModel = mapVM;
                try { AudioService.PlayMapTheme(); } catch { }
                AddLog("Welcome back!");
            }
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
            var mapVM = new MapViewModel(GlobalLog, CurrentPlayer);
            mapVM.OnStartBattle += async location =>
            {
                var battleVM = new BattleViewModel(GlobalLog, CurrentPlayer);
                // Optionally pass location to BattleViewModel here
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
            CurrentViewModel = mapVM;
            try { AudioService.PlayMapTheme(); } catch { }
            AddLog("Switched to MapView");
            // Future: Insert transition/animation logic here for MapView
        }
        // TODO: Add currency, inventory, and gear tabs next

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
    }
}
