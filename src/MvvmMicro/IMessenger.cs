// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;

namespace MvvmMicro
{
    /// <summary>
    /// Defines an interface for a publish-subscribe message broker.
    /// </summary>
    public interface IMessenger
    {
        /// <summary>
        /// Sends a message of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="message">The message to route to subscribers.</param>
        void Publish<T>(T message);

        /// <summary>
        /// Registers a callback to invoke when a message of the given type is received.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the message to listen, including the types derived from <typeparamref name="T"/>.
        /// </typeparam>
        /// <param name="subscriber">The subscriber to register.</param>
        /// <param name="callback">The callback to invoke when a message is delivered.</param>
        void Subscribe<T>(object subscriber, Action<T> callback);

        /// <summary>
        /// Removes all callbacks registered by the given subscriber.
        /// </summary>
        /// <param name="subscriber">The subscriber to unregister.</param>
        void Unsubscribe(object subscriber);
    }
}
