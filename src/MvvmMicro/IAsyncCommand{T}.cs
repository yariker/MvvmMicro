// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System.Threading.Tasks;

namespace MvvmMicro;

/// <summary>
/// Defines an asynchronous command with a strongly typed parameter.
/// </summary>
/// <typeparam name="T">The type of the command parameter.</typeparam>
public interface IAsyncCommand<in T> : IAsyncCommand, ICommand<T>
{
    /// <summary>
    /// Invokes the command asynchronously.
    /// </summary>
    /// <param name="parameter">The data used by the command.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ExecuteAsync(T parameter);
}
