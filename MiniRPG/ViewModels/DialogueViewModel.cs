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

        private ObservableCollection<string> _globalLog;

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
        /// <param name="globalLog">The global log for game messages (optional)</param>
        public DialogueViewModel(NPC npc, Player player, ObservableCollection<string>? globalLog = null)
        {
            CurrentNPC = npc;
            Player = player;
            _globalLog = globalLog ?? new ObservableCollection<string>();
            
            // Check if quest was completed by this player
            if (npc.OfferedQuest != null && player.CompletedQuests.Any(q => q.Title == npc.OfferedQuest.Title))
            {
                CurrentLine = GetCompletedQuestDialogue(npc);
            }
            else
            {
                CurrentLine = npc.Greeting;
            }
            
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
            // Check if quest was completed
            if (CurrentNPC.OfferedQuest != null && Player.CompletedQuests.Any(q => q.Title == CurrentNPC.OfferedQuest.Title))
            {
                CurrentLine = GetCompletedQuestDialogue(CurrentNPC);
                return;
            }
            
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
            if (CurrentNPC.OfferedQuest == null)
                return false;
            
            // Don't show accept button if quest is already completed
            if (Player.CompletedQuests.Any(q => q.Title == CurrentNPC.OfferedQuest.Title))
                return false;
            
            return CurrentIndex >= CurrentNPC.DialogueLines.Count;
        }

        /// <summary>
        /// Accept the quest offered by the NPC.
        /// </summary>
        private void AcceptQuest()
        {
            if (CurrentNPC.OfferedQuest != null)
            {
                // Check if quest is not already in Player.ActiveQuests
                if (!Player.ActiveQuests.Any(q => q.Title == CurrentNPC.OfferedQuest.Title))
                {
                    Player.AddQuest(CurrentNPC.OfferedQuest);
                    _globalLog.Add($"Quest accepted: {CurrentNPC.OfferedQuest.Title}");
                    SaveLoadService.SavePlayer(Player);
                    // Add voiced confirmation and quest acceptance animation later
                    Debug.WriteLine($"You accepted {CurrentNPC.OfferedQuest.Title} from {CurrentNPC.Name}.");
                    CurrentLine = $"Thank you! I'm counting on you.";
                }
                else
                {
                    CurrentLine = $"You already have this quest!";
                }
            }
        }

        /// <summary>
        /// Gets the dialogue for when the player has completed the NPC's quest.
        /// </summary>
        private string GetCompletedQuestDialogue(NPC npc)
        {
            // Customize based on NPC name
            if (npc.Name == "Mira")
            {
                return "Thank you so much for dealing with those slimes! Our village is safe again.";
            }
            
            // Default completion dialogue
            return "Thank you for your help, adventurer!";
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
