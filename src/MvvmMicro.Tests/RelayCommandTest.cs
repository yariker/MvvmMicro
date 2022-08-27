// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using Moq;
using Xunit;

namespace MvvmMicro.Test;

public class RelayCommandTest
{
    [Fact]
    public void Ctor_VerifiesArguments()
    {
        Assert.Throws<ArgumentNullException>("execute", () => new RelayCommand(null));
    }

    [Fact]
    public void Ctor_ProvidesDefaultCanExecuteCallback()
    {
        var handler = new Mock<ICommandHandler>();
        var command = new RelayCommand(handler.Object.Execute);
        Assert.True(command.CanExecute());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CanExecute_InvokesCanExecuteCallback(bool canExecute)
    {
        var handler = new Mock<ICommandHandler>();
        handler.Setup(h => h.CanExecute()).Returns(canExecute);

        var command = new RelayCommand(handler.Object.Execute, handler.Object.CanExecute);
        Assert.Equal(canExecute, command.CanExecute());

        handler.Verify(h => h.CanExecute(), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Execute_InvokesExecuteCallback(bool canExecute)
    {
        var handler = new Mock<ICommandHandler>();
        handler.Setup(h => h.CanExecute()).Returns(canExecute);

        var command = new RelayCommand(handler.Object.Execute, handler.Object.CanExecute);
        command.Execute();

        handler.Verify(h => h.Execute(), canExecute ? Times.Once() : Times.Never());
    }

    [Fact]
    public void RaiseCanExecuteChanged_RaisesCanExecuteChanged()
    {
        var handler = new Mock<IEventHandler>();
        var command = new RelayCommand(Mock.Of<Action>());
        command.CanExecuteChanged += handler.Object.EventHandler;

        command.RaiseCanExecuteChanged();

#if NETFRAMEWORK
        DispatcherHelper.DoEvents();
#endif

        handler.Verify(h => h.EventHandler(It.IsAny<object>(), EventArgs.Empty), Times.Once);
    }
}
