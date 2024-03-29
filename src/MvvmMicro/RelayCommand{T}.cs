﻿// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.Windows.Input;

namespace MvvmMicro;

/// <summary>
/// An <see cref="ICommand"/> whose delegates take a strongly-typed parameter.
/// </summary>
/// <typeparam name="T">The type of the command argument.</typeparam>
public class RelayCommand<T> : CommandBase, ICommand<T>
{
    private readonly Action<T> _execute;
    private readonly Func<T, bool> _canExecute;

    /// <summary>
    /// Initializes a new instance of the <see cref="RelayCommand{T}"/> class.
    /// </summary>
    /// <param name="execute">A delegate to execute when <see cref="ICommand.Execute(object)"/> is called on the command.</param>
    /// <param name="canExecute">A delegate to execute when <see cref="ICommand.CanExecute(object)"/> is called on the command.</param>
    /// <exception cref="ArgumentNullException"><paramref name="execute"/> is <c>null</c>.</exception>
    public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute ?? (_ => true);
    }

    /// <inheritdoc />
    public bool CanExecute(T parameter) => _canExecute(parameter);

    /// <inheritdoc />
    public void Execute(T parameter)
    {
        if (_canExecute(parameter))
        {
            _execute(parameter);
        }
    }

    /// <inheritdoc />
    protected override bool CanExecute(object parameter) => CanExecute(CastParameter<T>(parameter));

    /// <inheritdoc />
    protected override void Execute(object parameter) => Execute(CastParameter<T>(parameter));
}
