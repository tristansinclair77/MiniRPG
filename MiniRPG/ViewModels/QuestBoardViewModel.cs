using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using MiniRPG.Models;
using MiniRPG.Services;

namespace MiniRPG.ViewModels
{
    /// <summary>
    /// ViewModel for the Quest Board, managing available and active quests.
    /// </summary>
    public class QuestBoardViewModel : BaseViewModel
    {
        private Player _player;

        public Player Player => _player;

        private ObservableCollection<Quest> _availableQuests;
        public ObservableCollection<Quest> AvailableQuests
        {
            get => _availableQuests;
            set
            {
                _availableQuests = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Quest> ActiveQuests => _player.ActiveQuests;

        private Quest? _selectedQuest;
        public Quest? SelectedQuest
        {
            get => _selectedQuest;
            set
            {
                _selectedQuest = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(QuestTitle));
                OnPropertyChanged(nameof(QuestDescription));
            }
        }

        public string QuestTitle => SelectedQuest?.Title ?? "Select a quest";
        public string QuestDescription => SelectedQuest?.Description ?? "Choose a quest from the available quests list.";

        public ICommand AcceptQuestCommand { get; }
        public ICommand ExitBoardCommand { get; set; }

        public QuestBoardViewModel(Player player)
        {
            _player = player;

            // Load available quests from QuestService
            var allQuests = QuestService.GetAvailableQuests();
            AvailableQuests = new ObservableCollection<Quest>();

            // Only show quests that aren't already active or completed
            foreach (var quest in allQuests)
            {
                bool isAlreadyActive = false;
                bool isAlreadyCompleted = false;

                foreach (var activeQuest in _player.ActiveQuests)
                {
                    if (activeQuest.Title == quest.Title)
                    {
                        isAlreadyActive = true;
                        break;
                    }
                }

                if (!isAlreadyActive)
                {
                    foreach (var completedQuest in _player.CompletedQuests)
                    {
                        if (completedQuest.Title == quest.Title)
                        {
                            isAlreadyCompleted = true;
                            break;
                        }
                    }
                }

                if (!isAlreadyActive && !isAlreadyCompleted)
                {
                    AvailableQuests.Add(quest);
                }
            }

            AcceptQuestCommand = new RelayCommand(_ => AcceptQuest(), _ => CanAcceptQuest());
        }

        private bool CanAcceptQuest()
        {
            return SelectedQuest != null;
        }

        private void AcceptQuest()
        {
            if (SelectedQuest != null)
            {
                // Add quest to player's active quests
                _player.AddQuest(SelectedQuest);
                
                // Remove from available quests
                AvailableQuests.Remove(SelectedQuest);
                
                // Log the acceptance
                Debug.WriteLine($"You accepted the quest: {SelectedQuest.Title}");
                
                // Clear selection
                SelectedQuest = null;
                
                // Notify UI of changes
                OnPropertyChanged(nameof(ActiveQuests));
            }
        }

        // Add quest filtering by type and region
        // Add quest completion check refresh
    }
}