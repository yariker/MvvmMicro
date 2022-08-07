// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.Windows.Input;

namespace MvvmMicro;

/// <summary>
/// Provides the abstract base class for a command.
/// </summary>
public abstract class CommandBase : ICommand
{
#if NETFRAMEWORK || NET5_0
    /// <inheritdoc />
    public event EventHandler CanExecuteChanged
    {
        add => System.Windows.Input.CommandManager.RequerySuggested += value;
        remove => System.Windows.Input.CommandManager.RequerySuggested -= value;
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
#if NETFRAMEWORK || NET5_0
        System.Windows.Input.CommandManager.InvalidateRequerySuggested();
#else
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
#endif
    }
}
