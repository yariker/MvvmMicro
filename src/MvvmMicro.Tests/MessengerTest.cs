// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace MvvmMicro.Test;

public class MessengerTest
{
    [Fact]
    public void Register_VerifiesArguments()
    {
        var messenger = new Messenger();

        Assert.Throws<ArgumentNullException>(
            "subscriber",
            () => messenger.Register(null, Mock.Of<Action<string>>()));

        Assert.Throws<ArgumentNullException>(
            "callback",
            () => messenger.Register<string>(Mock.Of<object>(), null));
    }

    [Fact]
    public void Register_RegistersSubscriberCallback()
    {
        const string notification = "Message";
        var messenger = new Messenger();
        var subscriberMock = new Mock<ISubscriber<string>>();
        messenger.Register<string>(subscriberMock.Object, subscriberMock.Object.Callback);
        messenger.Send(notification);
        subscriberMock.Verify(s => s.Callback(notification), Times.Once);
    }

    [Fact]
    public void Register_DoesNotHoldStrongReferenceToSubscriber()
    {
        var messenger = new Messenger();
        var (subscriberWeak, releaseSignal, releasedEvent) = GetCollectableInstance(() =>
        {
            var instance = Mock.Of<ISubscriber<string>>();
            messenger.Register<string>(instance, instance.Callback);
            return instance;
        });

        releaseSignal.Set();
        releasedEvent.Wait();

        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
        GC.KeepAlive(messenger);

        Assert.False(subscriberWeak.IsAlive);
    }

    [Fact]
    public void Register_KeepsCallbackAlive()
    {
        var messenger = new Messenger();
        var subscriber = new object();

        var (callbackWeak, releaseSignal, releasedEvent) = GetCollectableInstance(() =>
        {
            var callback = Mock.Of<Action<string>>();
            messenger.Register(subscriber, callback);
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
    public void Send_DeliversMessageToAppropriateSubscriber()
    {
        var messenger = new Messenger();

        var subscriber1 = new Mock<ISubscriber<object>>();
        var subscriber2 = new Mock<ISubscriber<string>>();
        var subscriber3 = new Mock<ISubscriber<int>>();

        messenger.Register<object>(subscriber1.Object, subscriber1.Object.Callback);
        messenger.Register<string>(subscriber2.Object, subscriber2.Object.Callback);
        messenger.Register<int>(subscriber3.Object, subscriber3.Object.Callback);

        var message1 = new object();
        const string message2 = "Message";
        const int message3 = 42;

        messenger.Send(message1);
        messenger.Send(message2);
        messenger.Send(message3);

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
    public void Unregister_VerifiesArguments()
    {
        var messenger = new Messenger();
        Assert.Throws<ArgumentNullException>("subscriber", () => messenger.Unregister(null));
    }

    [Fact]
    public void Unregister_UnregistersSubscriberCallbacks()
    {
        const string notification = "Message";
        var messenger = new Messenger();
        var subscriberMock = new Mock<ISubscriber<string>>();
        messenger.Register<string>(subscriberMock.Object, subscriberMock.Object.Callback);
        messenger.Unregister(subscriberMock.Object);
        messenger.Send(notification);
        subscriberMock.Verify(s => s.Callback(notification), Times.Never);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static (WeakReference Callback, ManualResetEventSlim Releaser, Task WaitTask) GetCollectableInstance<T>(Func<T> factory)
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
