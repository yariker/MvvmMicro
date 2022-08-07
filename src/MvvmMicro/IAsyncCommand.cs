// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmMicro;

/// <summary>
/// Defines an asynchronous command.
/// </summary>
public interface IAsyncCommand : ICommand, INotifyPropertyChanged
{
    /// <summary>
    /// Gets a value indicating whether the command is executing.
    /// </summary>
    bool IsExecuting { get; }

    /// <summary>
    /// Invokes the command asynchronously.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ExecuteAsync();

    /// <summary>
    /// Communicates a request for cancellation.
    /// </summary>
    void Cancel();
}
