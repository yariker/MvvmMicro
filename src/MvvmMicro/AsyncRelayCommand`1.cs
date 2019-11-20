using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Takesoft.MvvmMicro
{
    /// <summary>
    /// An <see cref="ICommand"/> with an asynchronous parametrized <see cref="Task"/>-based delegate, which
    /// prevents a reentrant execution.
    /// </summary>
    public class AsyncRelayCommand<T> : CommandBase
    {
        private readonly Func<T, Task> _execute;
        private readonly Func<T, bool> _canExecute;
        private bool _isExecuting;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
        /// </summary>
        /// <param name="execute">A delegate to execute when <see cref="ICommand.Execute(object)"/> is called on the command.</param>
        /// <param name="canExecute">A delegate to execute when <see cref="ICommand.CanExecute(object)"/> is called on the command.</param>
        /// <exception cref="ArgumentNullException"><paramref name="execute"/> is <c>null</c>.</exception>
        public AsyncRelayCommand(Func<T, Task> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? new Func<T, bool>(_ => true);
        }

        /// <inheritdoc />
        public override bool CanExecute(object parameter)
        {
            return !_isExecuting && _canExecute((T)parameter);
        }

        /// <inheritdoc />
        public override async void Execute(object parameter)
        {
            var typedParameter = (T)parameter;
            if (!_isExecuting && _canExecute(typedParameter))
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();

                try
                {
                    await _execute(typedParameter);
                }
                finally
                {
                    _isExecuting = false;
                    RaiseCanExecuteChanged();
                }
            }
        }
    }
}