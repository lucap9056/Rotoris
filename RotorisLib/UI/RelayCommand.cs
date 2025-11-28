namespace RotorisLib.UI
{
    public class RelayCommand<T>(Action<T?> execute, Predicate<T?>? canExecute = null) : System.Windows.Input.ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add => System.Windows.Input.CommandManager.RequerySuggested += value;
            remove => System.Windows.Input.CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter)
        {
            if (canExecute == null)
            {
                return true;
            }

            if (parameter is T tParam)
            {
                return canExecute(tParam);
            }

            if (parameter == null && (typeof(T).IsClass || (Nullable.GetUnderlyingType(typeof(T)) != null)))
            {
                return canExecute(default);
            }

            if (parameter != null && !typeof(T).IsAssignableFrom(parameter.GetType()))
            {
                return false;
            }

            return canExecute((T?)parameter);
        }
        public bool CanExecute(T parameter)
        {
            if (canExecute == null)
            {
                return true;
            }
            return canExecute(parameter);
        }
        public void Execute(object? parameter)
        {
            if (parameter is T tParam)
            {
                execute(tParam);
            }

            else if (parameter == null && (typeof(T).IsClass || (Nullable.GetUnderlyingType(typeof(T)) != null)))
            {
                execute(default);
            }
        }
        public void Execute(T parameter)
        {
            execute(parameter);
        }
    }

    public class RelayCommand : RelayCommand<object?>
    {
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        : base(execute, canExecute)
        {

        }

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
            : base(p => execute(), canExecute == null ? null : p => canExecute())
        {
            ArgumentNullException.ThrowIfNull(execute);
        }
    }
}
