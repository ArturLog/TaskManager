using System.CodeDom.Compiler;
using System.Windows.Input;

namespace Task_manager.Commands
{
    public class RelayCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
            : this(
                param => execute(),
                canExecute != null ? new Func<object?, bool>(_ => canExecute()) : null
            )
        {
        }
        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
