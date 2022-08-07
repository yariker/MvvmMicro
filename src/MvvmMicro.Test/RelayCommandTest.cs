// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using Moq;
using Xunit;

namespace MvvmMicro.Test;

public class RelayCommandTest
{
    [Fact]
    public void Ctor_Should_Verify_Arguments()
    {
        Assert.Throws<ArgumentNullException>("execute", () => new RelayCommand(null));
    }

    [Fact]
    public void Ctor_Should_Provide_Default_CanExecute_Callback()
    {
        var handler = new Mock<ICommandHandler>();
        var command = new RelayCommand(handler.Object.Execute);
        Assert.True(command.CanExecute(null));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CanExecute_Should_Invoke_CanExecute_Callback(bool canExecute)
    {
        var handler = new Mock<ICommandHandler>();
        handler.Setup(h => h.CanExecute()).Returns(canExecute);

        var command = new RelayCommand(handler.Object.Execute, handler.Object.CanExecute);
        Assert.Equal(canExecute, command.CanExecute(null));

        handler.Verify(h => h.CanExecute(), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Execute_Should_Invoke_Execute_Callback(bool canExecute)
    {
        var handler = new Mock<ICommandHandler>();
        handler.Setup(h => h.CanExecute()).Returns(canExecute);

        var command = new RelayCommand(handler.Object.Execute, handler.Object.CanExecute);
        command.Execute(null);

        handler.Verify(h => h.Execute(), canExecute ? Times.Once() : Times.Never());
    }

    [Fact]
    public void RaiseCanExecuteChanged_Should_Raise_CanExecuteChanged()
    {
        var handler = new Mock<IEventHandler>();
        var command = new RelayCommand(Mock.Of<Action>());
        command.CanExecuteChanged += handler.Object.EventHandler;

        command.RaiseCanExecuteChanged();

        handler.Verify(h => h.EventHandler(command, EventArgs.Empty), Times.Once);
    }
}
