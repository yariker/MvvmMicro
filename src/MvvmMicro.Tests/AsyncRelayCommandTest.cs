// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace MvvmMicro.Test;

public class AsyncRelayCommandTest
{
    [Fact]
    public void Ctor_VerifiesArguments()
    {
        Assert.Throws<ArgumentNullException>(
            "execute", () => new AsyncRelayCommand((Func<Task>)null));

        Assert.Throws<ArgumentNullException>(
            "execute", () => new AsyncRelayCommand((Func<CancellationToken, Task>)null));
    }

    [Fact]
    public void Ctor_ProvidesDefaultCanExecuteCallback()
    {
        var handler = new Mock<IAsyncCommandHandler>();
        var command = new AsyncRelayCommand(handler.Object.ExecuteAsync);
        Assert.True(command.CanExecute());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CanExecute_InvokesCanExecuteCallback(bool canExecute)
    {
        var handler = new Mock<IAsyncCommandHandler>();
        handler.Setup(h => h.CanExecute()).Returns(canExecute);

        var command = new AsyncRelayCommand(handler.Object.ExecuteAsync, handler.Object.CanExecute);
        Assert.Equal(canExecute, command.CanExecute());

        handler.Verify(h => h.CanExecute(), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Execute_InvokesExecuteCallback(bool canExecute)
    {
        var handler = new Mock<IAsyncCommandHandler>();
        handler.Setup(h => h.CanExecute()).Returns(canExecute);

        var command = new AsyncRelayCommand(handler.Object.ExecuteAsync, handler.Object.CanExecute);
        await command.ExecuteAsync();

        handler.Verify(h => h.ExecuteAsync(), canExecute ? Times.Once : Times.Never);
    }

    [Fact]
    public void Execute_SupportsReentrancy()
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();

        var handler = new Mock<IAsyncCommandHandler>();
        handler.SetupSequence(h => h.ExecuteAsync())
               .Returns(taskCompletionSource.Task)
               .Returns(taskCompletionSource.Task);

        var command = new AsyncRelayCommand(handler.Object.ExecuteAsync);

        var task1 = command.ExecuteAsync();
        Assert.False(task1.IsCompleted);
        Assert.True(command.CanExecute());

        var task2 = command.ExecuteAsync();
        Assert.False(task2.IsCompleted);
        Assert.True(command.CanExecute());

        handler.Verify(h => h.ExecuteAsync(), Times.Exactly(2));
    }

    [Fact]
    public void RaiseCanExecuteChanged_RaisesCanExecuteChanged()
    {
        var handler = new Mock<IEventHandler>();
        var command = new AsyncRelayCommand(Mock.Of<Func<Task>>());
        command.CanExecuteChanged += handler.Object.EventHandler;

        command.RaiseCanExecuteChanged();

#if NETFRAMEWORK
        DispatcherHelper.DoEvents();
#endif

        handler.Verify(h => h.EventHandler(It.IsAny<object>(), EventArgs.Empty), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task IsExecuting_ReturnsCorrectStatus(int executes)
    {
        var taskSources = new List<TaskCompletionSource<bool>>();
        var tasks = new List<Task>();

        var command = new AsyncRelayCommand(() =>
        {
            var tcs = new TaskCompletionSource<bool>();
            taskSources.Add(tcs);
            return tcs.Task;
        });

        Assert.False(command.IsExecuting);

        for (int i = 0; i < executes; i++)
        {
            tasks.Add(command.ExecuteAsync());
            Assert.True(command.IsExecuting);
        }

        for (int i = 0; i < executes; i++)
        {
            taskSources[i].SetResult(true);
            await tasks[i];

            if (i < executes - 1)
            {
                Assert.True(command.IsExecuting);
            }
            else
            {
                Assert.False(command.IsExecuting);
            }
        }
    }
}
