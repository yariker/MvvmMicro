// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.Windows.Input;

namespace MvvmMicro
{
    /// <summary>
    /// Provides the abstract base class for a command.
    /// </summary>
    public abstract class CommandBase : ICommand
    {
#if NETFRAMEWORK || NET5_0_WINDOWS
        /// <inheritdoc />
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
#else
        /// <inheritdoc />
        public event EventHandler CanExecuteChanged;
#endif

        /// <inheritdoc />
        public virtual bool CanExecute(object parameter) => true;

        /// <inheritdoc />
        public abstract void Execute(object parameter);

        /// <summary>
        /// Forces the command to raise its <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
#if NETFRAMEWORK || NET5_0_WINDOWS
            CommandManager.InvalidateRequerySuggested();
#else
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
#endif
        }
    }
}
