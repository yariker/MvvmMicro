using System;
using System.Windows.Input;

namespace MvvmMicro
{
    /// <summary>
    /// An <see cref="ICommand"/> whose delegates take a strongly-typed parameter.
    /// </summary>
    /// <typeparam name="T">The parameter type.</typeparam>
    public class RelayCommand<T> : CommandBase
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
        /// </summary>
        /// <param name="execute">A delegate to execute when <see cref="ICommand.Execute(object)"/> is called on the command.</param>
        /// <param name="canExecute">A delegate to execute when <see cref="ICommand.CanExecute(object)"/> is called on the command.</param>
        /// <exception cref="ArgumentNullException"><paramref name="execute"/> is <c>null</c>.</exception>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? new Func<T, bool>(_ => true);
        }

        /// <inheritdoc />
        public override bool CanExecute(object parameter)
        {
            return _canExecute((T)parameter);
        }

        /// <inheritdoc />
        public override void Execute(object parameter)
        {
            var typedParameter = (T)parameter;
            if (_canExecute(typedParameter))
            {
                _execute(typedParameter);
            }
        }
    }
}