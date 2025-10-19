using System.Collections.ObjectModel;
using System.Windows.Input;
using MiniRPG.Models;
using MiniRPG.Services;

namespace MiniRPG.ViewModels
{
    /// <summary>
    /// ViewModel for the Skill Tree interface, handles skill unlocking.
    /// </summary>
    public class SkillTreeViewModel : BaseViewModel
    {
        public Player Player { get; }

        public ObservableCollection<Skill> AllSkills { get; }

        private Skill? _selectedSkill;
        public Skill? SelectedSkill
        {
            get => _selectedSkill;
            set
            {
                _selectedSkill = value;
                OnPropertyChanged();
                (UnlockSkillCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand UnlockSkillCommand { get; }
        public ICommand ExitSkillTreeCommand { get; }

        // Event for when the player wants to exit the skill tree
        public event System.Action? OnExitSkillTree;

        public SkillTreeViewModel(Player player)
        {
            Player = player;

            // Load AllSkills from SkillTreeService
            AllSkills = new ObservableCollection<Skill>(SkillTreeService.GetAllSkills());
            
            // Mark Player.AvailableSkills = AllSkills
            Player.AvailableSkills = AllSkills;

            UnlockSkillCommand = new RelayCommand(ExecuteUnlockSkill, CanUnlockSkill);
            ExitSkillTreeCommand = new RelayCommand(_ => OnExitSkillTree?.Invoke());
        }

        private bool CanUnlockSkill(object? parameter)
        {
            if (SelectedSkill == null) return false;
            if (SelectedSkill.IsUnlocked) return false;
            if (Player.SkillPoints < SelectedSkill.CostSP) return false;
            if (Player.Level < SelectedSkill.LevelRequirement) return false;
            return true;
        }

        private void ExecuteUnlockSkill(object? parameter)
        {
            if (SelectedSkill == null) return;

            // Calls Player.UnlockSkill(SelectedSkill)
            Player.UnlockSkill(SelectedSkill);

            // Refresh Player.SkillPoints display
            OnPropertyChanged(nameof(Player));
            OnPropertyChanged(nameof(Player.SkillPoints));
            (UnlockSkillCommand as RelayCommand)?.RaiseCanExecuteChanged();

            // Save progress
            SaveLoadService.SavePlayer(Player);
        }

        // TODO: Add skill preview popups and animations later
    }
}
