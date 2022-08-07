// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmMicro;

/// <summary>
/// An <see cref="ICommand"/> with an asynchronous parametrized <see cref="Task"/>-based delegate, which
/// prevents a reentrant execution.
/// </summary>
/// <typeparam name="T">The type of the command argument.</typeparam>
public class AsyncRelayCommand<T> : AsyncCommandBase, IAsyncCommand<T>
{
    private readonly Func<T, CancellationToken, Task> _execute;
    private readonly Func<T, bool> _canExecute;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class.
    /// </summary>
    /// <param name="execute">A delegate to execute when <see cref="ICommand.Execute(object)"/> is called on the command.</param>
    /// <param name="canExecute">A delegate to execute when <see cref="ICommand.CanExecute(object)"/> is called on the command.</param>
    /// <exception cref="ArgumentNullException"><paramref name="execute"/> is <c>null</c>.</exception>
    public AsyncRelayCommand(Func<T, CancellationToken, Task> execute, Func<T, bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute ?? (_ => true);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class.
    /// </summary>
    /// <param name="execute">A delegate to execute when <see cref="ICommand.Execute(object)"/> is called on the command.</param>
    /// <param name="canExecute">A delegate to execute when <see cref="ICommand.CanExecute(object)"/> is called on the command.</param>
    /// <exception cref="ArgumentNullException"><paramref name="execute"/> is <c>null</c>.</exception>
    public AsyncRelayCommand(Func<T, Task> execute, Func<T, bool> canExecute = null)
    {
        if (execute == null)
        {
            throw new ArgumentNullException(nameof(execute));
        }

        _execute = (x, _) => execute(x);
        _canExecute = canExecute ?? (_ => true);
    }

    /// <inheritdoc />
    public bool CanExecute(T parameter) => _canExecute(parameter);

    /// <inheritdoc />
    public Task ExecuteAsync(T parameter) => ExecuteAsync((object)parameter);

    /// <inheritdoc />
    void ICommand<T>.Execute(T parameter) => Execute(parameter);

    /// <inheritdoc />
    Task IAsyncCommand.ExecuteAsync() => ExecuteAsync(null);

    /// <inheritdoc />
    protected override bool CanExecute(object parameter) => CanExecute(CastParameter<T>(parameter));

    /// <inheritdoc />
    protected override Task ExecuteAsync(object parameter, CancellationToken cancellationToken)
    {
        return cancellationToken.IsCancellationRequested
            ? Task.FromCanceled(cancellationToken)
            : _execute(CastParameter<T>(parameter), cancellationToken);
    }
}
