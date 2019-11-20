using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Takesoft.MvvmMicro
{
    /// <summary>
    /// An <see cref="ICommand"/> with an asynchronous <see cref="Task"/>-based delegate, which prevents
    /// a reentrant execution.
    /// </summary>
    public class AsyncRelayCommand : CommandBase
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
        private bool _isExecuting;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
        /// </summary>
        /// <param name="execute">A delegate to execute when <see cref="ICommand.Execute(object)"/> is called on the command.</param>
        /// <param name="canExecute">A delegate to execute when <see cref="ICommand.CanExecute(object)"/> is called on the command.</param>
        /// <exception cref="ArgumentNullException"><paramref name="execute"/> is <c>null</c>.</exception>
        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? new Func<bool>(() => true);
        }

        /// <inheritdoc />
        public override bool CanExecute(object parameter)
        {
            return !_isExecuting && _canExecute();
        }

        /// <inheritdoc />
        public override async void Execute(object parameter)
        {
            if (!_isExecuting && _canExecute())
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();

                try
                {
                    await _execute();
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