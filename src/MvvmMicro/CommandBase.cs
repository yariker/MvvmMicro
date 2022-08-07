// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.Windows.Input;

namespace MvvmMicro;

/// <summary>
/// The base class for a command.
/// </summary>
public abstract class CommandBase : ObservableObject, ICommand
{
#if NETFRAMEWORK || NET5_0
    /// <inheritdoc cref="ICommand.CanExecuteChanged" />
    public event EventHandler CanExecuteChanged
    {
        add => System.Windows.Input.CommandManager.RequerySuggested += value;
        remove => System.Windows.Input.CommandManager.RequerySuggested -= value;
    }
#else
    /// <inheritdoc cref="ICommand.CanExecuteChanged" />
    public event EventHandler CanExecuteChanged;
#endif

    /// <inheritdoc />
    bool ICommand.CanExecute(object parameter) => CanExecute(parameter);

    /// <inheritdoc />
    void ICommand.Execute(object parameter) => Execute(parameter);

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

    /// <summary>
    /// A helper method to typecast the command parameter.
    /// </summary>
    /// <param name="parameter">The data used by the command.</param>
    /// <typeparam name="T">The command parameter type.</typeparam>
    /// <returns>A converted value; or default value if <paramref name="parameter"/> is <c>null</c>.</returns>
    protected static T CastParameter<T>(object parameter) => parameter == null ? default : (T)parameter;

    /// <summary>
    /// When implemented by a derived class, invokes the command's <see cref="ICommand.CanExecute"/> handler.
    /// </summary>
    /// <param name="parameter">The data used by the command.</param>
    /// <returns><c>true</c> if the command can be executed; otherwise, <c>false</c>.</returns>
    protected abstract bool CanExecute(object parameter);

    /// <summary>
    /// When implemented by a derived class, invokes the command's <see cref="ICommand.Execute"/> handler.
    /// </summary>
    /// <param name="parameter">The data used by the command.</param>
    protected abstract void Execute(object parameter);
}
