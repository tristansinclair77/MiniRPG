using System;
using System.Diagnostics;
using System.Windows.Input;
using MiniRPG.Models;

namespace MiniRPG.ViewModels
{
    /// <summary>
    /// ViewModel for managing NPC dialogue interactions.
    /// </summary>
    public class DialogueViewModel : BaseViewModel
    {
        private NPC _currentNPC;
        public NPC CurrentNPC
        {
            get => _currentNPC;
            set
            {
                _currentNPC = value;
                OnPropertyChanged();
            }
        }

        private string _currentLine;
        public string CurrentLine
        {
            get => _currentLine;
            set
            {
                _currentLine = value;
                OnPropertyChanged();
            }
        }

        private int _currentIndex;
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                _currentIndex = value;
                OnPropertyChanged();
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

        public ICommand NextCommand { get; }
        public ICommand AcceptQuestCommand { get; }
        public ICommand ExitDialogueCommand { get; }

        /// <summary>
        /// Event triggered when dialogue should exit.
        /// </summary>
        public event Action? OnDialogueExit;

        /// <summary>
        /// Constructor for DialogueViewModel.
        /// </summary>
        /// <param name="npc">The NPC to dialogue with</param>
        /// <param name="player">The player character</param>
        public DialogueViewModel(NPC npc, Player player)
        {
            CurrentNPC = npc;
            Player = player;
            CurrentLine = npc.Greeting;
            CurrentIndex = -1; // Start at -1 so first Next goes to index 0

            NextCommand = new RelayCommand(_ => NextDialogue());
            AcceptQuestCommand = new RelayCommand(_ => AcceptQuest(), _ => CanAcceptQuest());
            ExitDialogueCommand = new RelayCommand(_ => ExitDialogue());
        }

        /// <summary>
        /// Move through DialogueLines sequentially. When end is reached, show AcceptQuestCommand if OfferedQuest is available.
        /// </summary>
        private void NextDialogue()
        {
            CurrentIndex++;

            if (CurrentIndex < CurrentNPC.DialogueLines.Count)
            {
                CurrentLine = CurrentNPC.DialogueLines[CurrentIndex];
            }
            else
            {
                // End of dialogue reached
                if (CurrentNPC.OfferedQuest != null)
                {
                    CurrentLine = $"Would you accept the quest: {CurrentNPC.OfferedQuest.Title}?";
                }
                else
                {
                    CurrentLine = "That's all I have to say.";
                }
            }
        }

        /// <summary>
        /// Checks if the player can accept the quest.
        /// </summary>
        private bool CanAcceptQuest()
        {
            return CurrentNPC.OfferedQuest != null && CurrentIndex >= CurrentNPC.DialogueLines.Count;
        }

        /// <summary>
        /// Accept the quest offered by the NPC.
        /// </summary>
        private void AcceptQuest()
        {
            if (CurrentNPC.OfferedQuest != null)
            {
                Player.AddQuest(CurrentNPC.OfferedQuest);
                Debug.WriteLine($"You accepted {CurrentNPC.OfferedQuest.Title} from {CurrentNPC.Name}.");
                CurrentLine = $"Thank you! I'm counting on you.";
            }
        }

        /// <summary>
        /// Exit the dialogue and trigger the OnDialogueExit event.
        /// </summary>
        private void ExitDialogue()
        {
            OnDialogueExit?.Invoke();
        }

        // TODO: Add branching paths and player dialogue choices
        // TODO: Add emotion indicators and quest gating
    }
}
