using System.Collections.ObjectModel;
using System.Windows.Input;
using MiniRPG.Models;

namespace MiniRPG.ViewModels
{
    /// <summary>
    /// ViewModel for the Reputation & Factions interface.
    /// </summary>
    public class ReputationViewModel : BaseViewModel
    {
        public Player Player { get; }

        public ObservableCollection<Faction> Factions { get; }

        public ICommand ExitReputationCommand { get; set; }

        // Event for when the player wants to exit the reputation view
        public event System.Action? OnExitReputation;

        public ReputationViewModel(Player player)
        {
            Player = player;
            Factions = new ObservableCollection<Faction>(Player.Factions.Values);
            ExitReputationCommand = new RelayCommand(_ => OnExitReputation?.Invoke());
            // TODO: Add sorting and filtering by friendliness later
        }
    }
}
