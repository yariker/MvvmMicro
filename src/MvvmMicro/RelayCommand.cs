// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.Windows.Input;

namespace MvvmMicro
{
    /// <summary>
    /// An <see cref="ICommand"/> whose delegates do not take any parameters for
    /// <see cref="ICommand.Execute(object)"/> and <see cref="ICommand.CanExecute(object)"/>.
    /// </summary>
    public class RelayCommand : CommandBase
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">A delegate to execute when <see cref="ICommand.Execute(object)"/> is called on the command.</param>
        /// <param name="canExecute">A delegate to execute when <see cref="ICommand.CanExecute(object)"/> is called on the command.</param>
        /// <exception cref="ArgumentNullException"><paramref name="execute"/> is <c>null</c>.</exception>
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? (() => true);
        }

        /// <inheritdoc />
        public override bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        /// <inheritdoc />
        public override void Execute(object parameter)
        {
            if (_canExecute())
            {
                _execute();
            }
        }
    }
}
