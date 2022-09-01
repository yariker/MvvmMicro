// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmMicro;

/// <summary>
/// The base class for an asynchronous command.
/// </summary>
public abstract class AsyncCommandBase : CommandBase
{
    private CancellationTokenSource _cancellationTokenSource;
    private bool _isExecuting;
    private int _executeCounter;

    /// <inheritdoc cref="IAsyncCommand.IsExecuting"/> />
    public bool IsExecuting
    {
        get => _isExecuting;
        private set
        {
            if (Set(ref _isExecuting, value))
            {
                RaiseCanExecuteChanged();
            }
        }
    }

    /// <inheritdoc cref="IAsyncCommand.Cancel" />
    public void Cancel()
    {
        _cancellationTokenSource?.Cancel();
    }

    /// <inheritdoc />
    protected sealed override async void Execute(object parameter)
    {
        try
        {
            await ExecuteAsync(parameter);
        }
        catch (OperationCanceledException)
        {
            // Canceled.
        }
    }

    /// <summary>
    /// An asynchronous equivalent of the <see cref="ICommand.Execute"/> method.
    /// </summary>
    /// <param name="parameter">The data used by the command.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected Task ExecuteAsync(object parameter)
    {
        return CanExecute(parameter) ? ExecuteAsyncInternal() : Task.CompletedTask;

        async Task ExecuteAsyncInternal()
        {
            if (_executeCounter++ == 0)
            {
                IsExecuting = true;
            }

            using var cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource = cancellationTokenSource;

            try
            {
                await ExecuteAsync(parameter, cancellationTokenSource.Token);
            }
            finally
            {
                if (_cancellationTokenSource == cancellationTokenSource)
                {
                    _cancellationTokenSource = null;
                }

                if (--_executeCounter == 0)
                {
                    IsExecuting = false;
                }
            }
        }
    }

    /// <summary>
    /// When implemented by a derived class, invokes the command's <see cref="ICommand.Execute"/> handler
    /// asynchronously.
    /// </summary>
    /// <param name="parameter">The data used by the command.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected abstract Task ExecuteAsync(object parameter, CancellationToken cancellationToken);
}
