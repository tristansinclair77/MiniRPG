using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MiniRPG.Services;
using MiniRPG.Models;

namespace MiniRPG.ViewModels
{
    /// <summary>
    /// ViewModel for BattleView. Handles combat actions, HP, and combat log.
    /// </summary>
    public class BattleViewModel : BaseViewModel
    {
        public ObservableCollection<string> CombatLog { get; } = new();
        private ObservableCollection<string> _globalLog;

        public Player Player { get; set; }

        private int _enemyHP = 20;
        public int EnemyHP
        {
            get => _enemyHP;
            set { _enemyHP = value; OnPropertyChanged(); }
        }

        private string _currentEnemy;
        public string CurrentEnemy
        {
            get => _currentEnemy;
            set { _currentEnemy = value; OnPropertyChanged(); }
        }

        private bool _isBattleOver;
        public bool IsBattleOver
        {
            get => _isBattleOver;
            set { _isBattleOver = value; OnPropertyChanged(); }
        }

        private string _battleResult;
        public string BattleResult
        {
            get => _battleResult;
            set { _battleResult = value; OnPropertyChanged(); }
        }

        private bool _canAct = true;
        private bool _defendNext = false;

        public ICommand AttackCommand { get; }
        public ICommand DefendCommand { get; }
        public ICommand RunCommand { get; }
        public ICommand ReturnToMapCommand { get; }

        // Later: Replace integers with full Stat objects for scaling difficulty
        // TODO: Later - persist Player object between battles

        // Event for battle end
        public event Action<string>? BattleEnded;

        public BattleViewModel(ObservableCollection<string> globalLog, Player player)
        {
            _globalLog = globalLog;
            Player = player;
            CurrentEnemy = GameService.GetRandomEnemy();
            EnemyHP = 20;
            IsBattleOver = false;
            BattleResult = string.Empty;
            AttackCommand = new RelayCommand(_ => Attack(), _ => _canAct && !IsBattleOver);
            DefendCommand = new RelayCommand(_ => Defend(), _ => _canAct && !IsBattleOver);
            RunCommand = new RelayCommand(_ => Run(), _ => _canAct && !IsBattleOver);
            ReturnToMapCommand = new RelayCommand(_ => OnReturnToMap(), _ => IsBattleOver);
        }

        private void Attack()
        {
            int dmg = GameService.CalculateDamage();
            EnemyHP -= dmg;
            var msg = $"You strike {CurrentEnemy} for {dmg} damage! (Enemy HP: {EnemyHP})";
            CombatLog.Add(msg);
            _globalLog.Add(msg);
            if (EnemyHP <= 0)
            {
                CombatLog.Add("You defeated the enemy!");
                _globalLog.Add("You defeated the enemy!");
                // Experience and level-up logic
                int exp = new Random().Next(5, 15);
                bool leveledUp = Player.GainExperience(exp);
                CombatLog.Add($"You gained {exp} experience!");
                _globalLog.Add($"You gained {exp} experience!");
                if (leveledUp)
                {
                    CombatLog.Add($"You reached level {Player.Level}! Your stats increased.");
                    _globalLog.Add($"You reached level {Player.Level}! Your stats increased.");
                }
                // Loot logic
                var loot = GameService.GetRandomLoot();
                if (loot != null)
                {
                    Player.AddItem(loot);
                    CombatLog.Add($"You found {loot.Name}!");
                    _globalLog.Add($"You found {loot.Name}!");
                }
                else
                {
                    CombatLog.Add("No items found this time.");
                    _globalLog.Add("No items found this time.");
                }
                SaveLoadService.SavePlayer(Player);
                CombatLog.Add("Progress saved!");
                _globalLog.Add("Progress saved!");
                IsBattleOver = true;
                _canAct = false;
                BattleResult = "Victory";
                UpdateCommands();
                BattleEnded?.Invoke("Victory");
                // TODO: Later - include loot drops and enemy-specific EXP scaling
                // TODO: Later - add autosave indicator animation on screen
                // TODO: Display loot visually in post-battle summary later
            }
            else
            {
                EnemyAttack();
            }
        }

        private void Defend()
        {
            _defendNext = true;
            var msg = "You brace for the next attack!";
            CombatLog.Add(msg);
            _globalLog.Add(msg);
            EnemyAttack();
        }

        private void Run()
        {
            var msg = "You escaped safely.";
            CombatLog.Add(msg);
            _globalLog.Add(msg);
            IsBattleOver = true;
            _canAct = false;
            BattleResult = "Defeat";
            UpdateCommands();
            BattleEnded?.Invoke("Defeat"); // Treat run as defeat for now
        }

        private void EnemyAttack()
        {
            int dmg = GameService.CalculateDamage() / 2;
            if (_defendNext)
            {
                dmg /= 2;
                _defendNext = false;
            }
            Player.HP -= dmg;
            var msg = $"{CurrentEnemy} attacks you for {dmg} damage! (Your HP: {Player.HP})";
            CombatLog.Add(msg);
            _globalLog.Add(msg);
            if (Player.HP <= 0)
            {
                CombatLog.Add("You were defeated!");
                _globalLog.Add("You were defeated!");
                IsBattleOver = true;
                _canAct = false;
                BattleResult = "Defeat";
                UpdateCommands();
                BattleEnded?.Invoke("Defeat");
            }
        }

        private void OnReturnToMap()
        {
            CombatLog.Add("Returning to map...");
            _globalLog.Add("Returning to map...");
            IsBattleOver = true;
            BattleEnded?.Invoke("Return");
            // TODO: Hook this into save/load later so battle results persist
            // TODO: Replace with animated return transition to Map screen
        }

        private void UpdateCommands()
        {
            OnPropertyChanged(nameof(AttackCommand));
            OnPropertyChanged(nameof(DefendCommand));
            OnPropertyChanged(nameof(RunCommand));
        }
    }
}
