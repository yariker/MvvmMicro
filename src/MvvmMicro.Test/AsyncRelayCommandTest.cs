using System;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace MvvmMicro.Test
{
    public class AsyncRelayCommandTest
    {
        [Fact]
        public void Ctor_Should_Verify_Arguments()
        {
            Assert.Throws<ArgumentNullException>("execute", () => new AsyncRelayCommand(null));
        }

        [Fact]
        public void Ctor_Should_Provide_Default_CanExecute_Callback()
        {
            var handler = new Mock<IAsyncCommandHandler>();
            var command = new AsyncRelayCommand(handler.Object.ExecuteAsync);
            Assert.True(command.CanExecute(null));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanExecute_Should_Invoke_CanExecute_Callback(bool canExecute)
        {
            var handler = new Mock<IAsyncCommandHandler>();
            handler.Setup(h => h.CanExecute()).Returns(canExecute);

            var command = new AsyncRelayCommand(handler.Object.ExecuteAsync, handler.Object.CanExecute);
            Assert.Equal(canExecute, command.CanExecute(null));

            handler.Verify(h => h.CanExecute(), Times.Once);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Execute_Should_Invoke_Execute_Callback(bool canExecute)
        {
            var handler = new Mock<IAsyncCommandHandler>();
            handler.Setup(h => h.CanExecute()).Returns(canExecute);

            var command = new AsyncRelayCommand(handler.Object.ExecuteAsync, handler.Object.CanExecute);
            command.Execute(null);

            handler.Verify(h => h.ExecuteAsync(), canExecute ? Times.Once() : Times.Never());
        }        
        
        [Fact]
        public void Execute_Should_Prevent_Reentrancy()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            var handler = new Mock<IAsyncCommandHandler>();
            handler.Setup(h => h.ExecuteAsync()).Returns(taskCompletionSource.Task);

            var command = new AsyncRelayCommand(handler.Object.ExecuteAsync);

            for (int i = 0; i < 4; i++)
            {
                command.Execute(null);
            }

            Assert.False(command.CanExecute(null));
            handler.Verify(h => h.ExecuteAsync(), Times.Once);

            taskCompletionSource.SetResult(true);
            
            Assert.True(command.CanExecute(null));
            command.Execute(null);

            handler.Verify(h => h.ExecuteAsync(), Times.Exactly(2));
        }

        [Fact]
        public void RaiseCanExecuteChanged_Should_Raise_CanExecuteChanged()
        {
            var handler = new Mock<IEventHandler>();
            var command = new AsyncRelayCommand(Mock.Of<Func<Task>>());
            command.CanExecuteChanged += handler.Object.EventHandler;

            command.RaiseCanExecuteChanged();

            handler.Verify(h => h.EventHandler(command, EventArgs.Empty), Times.Once);
        }
    }    
}
