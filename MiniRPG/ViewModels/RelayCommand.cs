using System;
using System.Windows.Input;

namespace MiniRPG.ViewModels
{
    /// <summary>
    /// RelayCommand for MVVM button bindings. Supports future parameter expansion.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        /// <summary>
        /// Creates a new RelayCommand.
        /// </summary>
        /// <param name="execute">Action to execute. Cannot be null.</param>
        /// <param name="canExecute">Optional predicate to determine if command can execute.</param>
        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
            // Future: Add support for additional parameters or context if needed
        }

        /// <summary>
        /// Determines whether the command can execute.
        /// </summary>
        public bool CanExecute(object? parameter)
        {
            // Future: Expand parameter handling for more complex scenarios
            return _canExecute?.Invoke(parameter) ?? true;
        }

        /// <summary>
        /// Executes the command action.
        /// </summary>
        public void Execute(object? parameter)
        {
            // Future: Expand parameter handling for more complex scenarios
            _execute(parameter);
        }

        /// <summary>
        /// Raised when CanExecute changes.
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
