// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmMicro;

/// <summary>
/// An <see cref="ICommand"/> with an asynchronous <see cref="Task"/>-based delegate, which prevents
/// a reentrant execution.
/// </summary>
public class AsyncRelayCommand : AsyncCommandBase, IAsyncCommand
{
    private readonly Func<CancellationToken, Task> _execute;
    private readonly Func<bool> _canExecute;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
    /// </summary>
    /// <param name="execute">A delegate to execute when <see cref="ICommand.Execute(object)"/> is called on the command.</param>
    /// <param name="canExecute">A delegate to execute when <see cref="ICommand.CanExecute(object)"/> is called on the command.</param>
    /// <exception cref="ArgumentNullException"><paramref name="execute"/> is <c>null</c>.</exception>
    public AsyncRelayCommand(Func<CancellationToken, Task> execute, Func<bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute ?? (() => true);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
    /// </summary>
    /// <param name="execute">A delegate to execute when <see cref="ICommand.Execute(object)"/> is called on the command.</param>
    /// <param name="canExecute">A delegate to execute when <see cref="ICommand.CanExecute(object)"/> is called on the command.</param>
    /// <exception cref="ArgumentNullException"><paramref name="execute"/> is <c>null</c>.</exception>
    public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute = null)
    {
        if (execute == null)
        {
            throw new ArgumentNullException(nameof(execute));
        }

        _execute = _ => execute();
        _canExecute = canExecute ?? (() => true);
    }

    /// <inheritdoc cref="ICommand.CanExecute" />
    public bool CanExecute() => _canExecute();

    /// <inheritdoc />
    public Task ExecuteAsync() => ExecuteAsync(null);

    /// <inheritdoc />
    protected override bool CanExecute(object parameter) => CanExecute();

    /// <inheritdoc />
    protected override Task ExecuteAsync(object parameter, CancellationToken cancellationToken)
    {
        return cancellationToken.IsCancellationRequested
            ? Task.FromCanceled(cancellationToken)
            : _execute(cancellationToken);
    }
}
