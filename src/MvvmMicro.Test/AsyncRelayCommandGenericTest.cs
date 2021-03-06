using System;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace MvvmMicro.Test
{
    public class AsyncRelayCommandGenericTest
    {
        [Fact]
        public void Ctor_Should_Verify_Arguments()
        {
            Assert.Throws<ArgumentNullException>("execute", () => new AsyncRelayCommand<string>(null));
        }

        [Fact]
        public void Ctor_Should_Provide_Default_CanExecute_Callback()
        {
            var handler = new Mock<IAsyncCommandHandler<string>>();
            var command = new AsyncRelayCommand<string>(handler.Object.ExecuteAsync);
            Assert.True(command.CanExecute(null));
        }

        [Theory]
        [InlineData("Test1", true)]
        [InlineData("Test2", false)]
        public void CanExecute_Should_Invoke_CanExecute_Callback(string parameter, bool canExecute)
        {
            var handler = new Mock<IAsyncCommandHandler<string>>();
            handler.Setup(h => h.CanExecute(parameter)).Returns(canExecute);

            var command = new AsyncRelayCommand<string>(handler.Object.ExecuteAsync, handler.Object.CanExecute);
            Assert.Equal(canExecute, command.CanExecute(parameter));

            handler.Verify(h => h.CanExecute(parameter), Times.Once);
        }

        [Theory]
        [InlineData("Test1", true)]
        [InlineData("Test2", false)]
        public void Execute_Should_Invoke_Execute_Callback(string parameter, bool canExecute)
        {
            var handler = new Mock<IAsyncCommandHandler<string>>();
            handler.Setup(h => h.CanExecute(parameter)).Returns(canExecute);

            var command = new AsyncRelayCommand<string>(handler.Object.ExecuteAsync, handler.Object.CanExecute);
            command.Execute(parameter);

            handler.Verify(h => h.ExecuteAsync(parameter), canExecute ? Times.Once() : Times.Never());
        }

        [Fact]
        public void Execute_Should_Prevent_Reentrancy()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            var handler = new Mock<IAsyncCommandHandler<string>>();
            handler.Setup(h => h.ExecuteAsync(It.IsAny<string>())).Returns(taskCompletionSource.Task);

            var command = new AsyncRelayCommand<string>(handler.Object.ExecuteAsync);

            for (int i = 0; i < 4; i++)
            {
                command.Execute(null);
            }

            Assert.False(command.CanExecute(null));
            handler.Verify(h => h.ExecuteAsync(It.IsAny<string>()), Times.Once);

            taskCompletionSource.SetResult(true);

            Assert.True(command.CanExecute(null));
            command.Execute(null);

            handler.Verify(h => h.ExecuteAsync(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public void RaiseCanExecuteChanged_Should_Raise_CanExecuteChanged()
        {
            var handler = new Mock<IEventHandler>();
            var command = new AsyncRelayCommand<string>(Mock.Of<Func<string, Task>>());
            command.CanExecuteChanged += handler.Object.EventHandler;

            command.RaiseCanExecuteChanged();

            handler.Verify(h => h.EventHandler(command, EventArgs.Empty), Times.Once);
        }
    }
}
