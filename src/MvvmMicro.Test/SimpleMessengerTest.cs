using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Takesoft.MvvmMicro.Test
{
    public class SimpleMessengerTest
    {
        [Fact]
        public void Subscribe_Should_Verify_Arguments()
        {
            var messenger = new SimpleMessenger();
            Assert.Throws<ArgumentNullException>("subscriber", () => messenger.Subscribe(null, Mock.Of<Action<string>>()));
            Assert.Throws<ArgumentNullException>("callback", () => messenger.Subscribe<string>(Mock.Of<object>(), null));
        }

        [Fact]
        public void Subscribe_Should_Register_Subscriber_Callback()
        {
            const string notification = "Message";
            var messenger = new SimpleMessenger();
            var subscriberMock = new Mock<ISubscriber<string>>();
            messenger.Subscribe<string>(subscriberMock.Object, subscriberMock.Object.Callback);
            messenger.Publish(notification);
            subscriberMock.Verify(s => s.Callback(notification), Times.Once);
        }

        [Fact]
        public void Subscribe_Should_Not_Hold_Strong_Reference_To_Subscriber()
        {
            var messenger = new SimpleMessenger();
            var (subscriberWeak, releaseSignal, releasedEvent) = GetGarbageCollectableInstance(() =>
            {
                var instance = Mock.Of<ISubscriber<string>>();
                messenger.Subscribe<string>(instance, instance.Callback);
                return instance;
            });

            releaseSignal.Set();
            releasedEvent.Wait();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.KeepAlive(messenger);

            Assert.False(subscriberWeak.IsAlive);
        }

        [Fact]
        public void Subscribe_Should_Keep_Callback_Alive()
        {
            var messenger = new SimpleMessenger();
            var subscriber = new object();

            var (callbackWeak, releaseSignal, releasedEvent) = GetGarbageCollectableInstance(() =>
            {
                var callback = Mock.Of<Action<string>>();
                messenger.Subscribe(subscriber, callback);
                return callback;
            });

            releaseSignal.Set();
            releasedEvent.Wait();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.KeepAlive(messenger);
            GC.KeepAlive(subscriber);

            Assert.True(callbackWeak.IsAlive);
        }

        [Fact]
        public void Publish_Should_Send_Message_To_Appropriate_Subscriber()
        {
            var messenger = new SimpleMessenger();

            var subscriber1 = new Mock<ISubscriber<object>>();
            var subscriber2 = new Mock<ISubscriber<string>>();
            var subscriber3 = new Mock<ISubscriber<int>>();

            messenger.Subscribe<object>(subscriber1.Object, subscriber1.Object.Callback);
            messenger.Subscribe<string>(subscriber2.Object, subscriber2.Object.Callback);
            messenger.Subscribe<int>(subscriber3.Object, subscriber3.Object.Callback);

            var message1 = new object();
            const string message2 = "Message";
            const int message3 = 42;

            messenger.Publish(message1);
            messenger.Publish(message2);
            messenger.Publish(message3);

            subscriber1.Verify(s => s.Callback(It.IsAny<object>()), Times.Exactly(3));
            subscriber1.Verify(s => s.Callback(message1), Times.Once);
            subscriber1.Verify(s => s.Callback(message2), Times.Once);
            subscriber1.Verify(s => s.Callback(message3), Times.Once);

            subscriber2.Verify(s => s.Callback(It.IsAny<string>()), Times.Once);
            subscriber2.Verify(s => s.Callback(message2), Times.Once);

            subscriber3.Verify(s => s.Callback(It.IsAny<int>()), Times.Once);
            subscriber3.Verify(s => s.Callback(message3), Times.Once);
        }

        [Fact]
        public void Unsubscribe_Shold_Verify_Arguments()
        {
            var messenger = new SimpleMessenger();
            Assert.Throws<ArgumentNullException>("subscriber", () => messenger.Unsubscribe(null));
        }

        [Fact]
        public void Unsubscribe_Shold_Unregister_Subscriber_Callbacks()
        {
            const string notification = "Message";
            var messenger = new SimpleMessenger();
            var subscriberMock = new Mock<ISubscriber<string>>();
            messenger.Subscribe<string>(subscriberMock.Object, subscriberMock.Object.Callback);
            messenger.Unsubscribe(subscriberMock.Object);
            messenger.Publish(notification);
            subscriberMock.Verify(s => s.Callback(notification), Times.Never);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static (WeakReference, ManualResetEventSlim, Task) GetGarbageCollectableInstance<T>(Func<T> factory)
            where T : class
        {
            var instance = factory();
            var weakReference = new WeakReference(instance);
            var releaseSignal = new ManualResetEventSlim();

            var keepAliveTask = Task.Run(() =>
            {
                releaseSignal.Wait();
                GC.KeepAlive(instance);
                instance = null;
            });

            return (weakReference, releaseSignal, keepAliveTask);
        }
    }
}
