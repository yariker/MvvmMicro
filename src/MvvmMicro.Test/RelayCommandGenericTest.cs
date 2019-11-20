using System;
using Moq;
using Xunit;

namespace Takesoft.MvvmMicro.Test
{
    public class RelayCommandGenericTest
    {
        [Fact]
        public void Ctor_Should_Verify_Arguments()
        {
            Assert.Throws<ArgumentNullException>("execute", () => new RelayCommand<string>(null));
        }

        [Fact]
        public void Ctor_Should_Provide_Default_CanExecute_Callback()
        {
            var handler = new Mock<ICommandHandler<string>>();
            var command = new RelayCommand<string>(handler.Object.Execute);
            Assert.True(command.CanExecute("Test"));
        }

        [Theory]
        [InlineData("Test1", true)]
        [InlineData("Test2", false)]
        public void CanExecute_Should_Invoke_CanExecute_Callback(string parameter, bool canExecute)
        {
            var handler = new Mock<ICommandHandler<string>>();
            handler.Setup(h => h.CanExecute(parameter)).Returns(canExecute);

            var command = new RelayCommand<string>(handler.Object.Execute, handler.Object.CanExecute);
            Assert.Equal(canExecute, command.CanExecute(parameter));

            handler.Verify(h => h.CanExecute(parameter), Times.Once);
        }

        [Theory]
        [InlineData("Test1", true)]
        [InlineData("Test2", false)]
        public void Execute_Should_Invoke_Execute_Callback(string parameter, bool canExecute)
        {
            var handler = new Mock<ICommandHandler<string>>();
            handler.Setup(h => h.CanExecute(parameter)).Returns(canExecute);

            var command = new RelayCommand<string>(handler.Object.Execute, handler.Object.CanExecute);
            command.Execute(parameter);

            handler.Verify(h => h.Execute(parameter), canExecute ? Times.Once() : Times.Never());
        }

        [Fact]
        public void RaiseCanExecuteChanged_Should_Raise_CanExecuteChanged()
        {
            var handler = new Mock<IEventHandler>();
            var command = new RelayCommand<string>(Mock.Of<Action<string>>());
            command.CanExecuteChanged += handler.Object.EventHandler;

            command.RaiseCanExecuteChanged();

            handler.Verify(h => h.EventHandler(command, EventArgs.Empty), Times.Once);
        }
    }
}
