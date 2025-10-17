using System;
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

        private ObservableCollection<Quest> _availableQuests = new ObservableCollection<Quest>();
        public ObservableCollection<Quest> AvailableQuests
        {
            get => _availableQuests;
            set
            {
                _availableQuests = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Quest> ActiveQuests => _player?.ActiveQuests ?? new ObservableCollection<Quest>();

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
        public ICommand ExitBoardCommand { get; set; } = null!;

        public QuestBoardViewModel(Player player)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));

            // Initialize AcceptQuestCommand before loading quests
            AcceptQuestCommand = new RelayCommand(_ => AcceptQuest(), _ => CanAcceptQuest());

            // Load available quests from QuestService
            var allQuests = QuestService.GetAvailableQuests();

            // Only show quests that aren't already active or completed
            foreach (var quest in allQuests)
            {
                bool isAlreadyActive = false;
                bool isAlreadyCompleted = false;

                if (_player.ActiveQuests != null)
                {
                    foreach (var activeQuest in _player.ActiveQuests)
                    {
                        if (activeQuest != null && activeQuest.Title == quest.Title)
                        {
                            isAlreadyActive = true;
                            break;
                        }
                    }
                }

                if (!isAlreadyActive && _player.CompletedQuests != null)
                {
                    foreach (var completedQuest in _player.CompletedQuests)
                    {
                        if (completedQuest != null && completedQuest.Title == quest.Title)
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
        }

        private bool CanAcceptQuest()
        {
            return SelectedQuest != null;
        }

        private void AcceptQuest()
        {
            if (SelectedQuest != null && _player != null && AvailableQuests != null && _player.ActiveQuests != null)
            {
                var questToAccept = SelectedQuest;
                // Add quest to player's active quests
                _player.AddQuest(questToAccept);
                // Remove from available quests
                AvailableQuests.Remove(questToAccept);
                // Log the acceptance
                Debug.WriteLine($"You accepted the quest: {questToAccept.Title}");
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