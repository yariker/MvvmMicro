// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System.Windows.Input;

namespace MvvmMicro;

/// <summary>
/// Defines a command with a strongly typed parameter.
/// </summary>
/// <typeparam name="T">The type of the command parameter.</typeparam>
public interface ICommand<in T> : ICommand
{
    /// <summary>
    /// Defines the method that determines whether the command can execute in its current state.</summary>
    /// <param name="parameter">The data used by the command.</param>
    /// <returns><c>true</c> if the command can be executed; otherwise, <c>false</c>.</returns>
    bool CanExecute(T parameter);

    /// <summary>
    /// Defines the method to be called when the command is invoked.
    /// </summary>
    /// <param name="parameter">The data used by the command.</param>
    void Execute(T parameter);
}
