using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PlaidSoft.MvvmMicro
{
    /// <summary>
    /// A basic implementation of <see cref="IMessenger"/> with the following characteristics:
    /// <list type="bullet">
    ///     <item>
    ///         <description>
    ///             Delivers messages synchronously.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             The callbacks are kept as weak references. Subscribers should keep a strong reference
    ///             to their callback to avoid it being garbage collected.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             Provides a convenient static instance of itself via the <see cref="Default"/> property.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             All public methods are thread-safe.
    ///         </description>
    ///     </item>
    /// </list>
    /// </summary>
    public class SimpleMessenger : IMessenger
    {
        private readonly Dictionary<Type, Channel> _registry = new Dictionary<Type, Channel>();

        /// <summary>
        /// Gets the default instance of the <see cref="SimpleMessenger" />.
        /// </summary>
        public static IMessenger Default { get; } = new SimpleMessenger();

        /// <inheritdoc />
        public void Publish<T>(T message)
        {
            var type = typeof(T);
            var callbacks = new List<Delegate>();

            lock (_registry)
            {
                foreach (KeyValuePair<Type, Channel> entry in _registry)
                {
                    if (entry.Key.IsAssignableFrom(type))
                    {
                        callbacks.AddRange(entry.Value.GetCallbacks());
                    }
                }
            }

            foreach (Action<T> callback in callbacks)
            {
                callback(message);
            }
        }

        /// <inheritdoc />
        public void Subscribe<T>(Action<T> callback)
        {
            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            var type = typeof(T);
            lock (_registry)
            {
                if (!_registry.TryGetValue(type, out Channel channel))
                {
                    _registry.Add(type, channel = new Channel());
                }

                channel.Subscribe(callback);
            }
        }

        /// <inheritdoc />
        public void Unsubscribe(object subscriber)
        {
            if (subscriber is null)
            {
                throw new ArgumentNullException(nameof(subscriber));
            }

            lock (_registry)
            {
                foreach (Channel channel in _registry.Values)
                {
                    channel.Unsubscribe(subscriber);
                }
            }
        }

        private class Channel
        {
            private readonly List<WeakReference<object>> _subscribers =
                new List<WeakReference<object>>();

            private readonly ConditionalWeakTable<object, List<Delegate>> _callbacks =
                new ConditionalWeakTable<object, List<Delegate>>();

            public void Subscribe(Delegate callback)
            {
                if (!_callbacks.TryGetValue(callback.Target, out List<Delegate> callbacks))
                {
                    _subscribers.Add(new WeakReference<object>(callback.Target));
                    _callbacks.Add(callback.Target, callbacks = new List<Delegate>());
                }

                callbacks.Add(callback);
            }

            public void Unsubscribe(object subscriber)
            {
                if (_callbacks.Remove(subscriber))
                {
                    for (int i = 0; i < _subscribers.Count; i++)
                    {
                        if (_subscribers[i].TryGetTarget(out object target) && ReferenceEquals(target, subscriber))
                        {
                            _subscribers.RemoveAt(i);
                            break;
                        }
                    }
                }
            }

            public IEnumerable<Delegate> GetCallbacks()
            {
                foreach (WeakReference<object> reference in _subscribers)
                {
                    if (reference.TryGetTarget(out object subscriber) &&
                        _callbacks.TryGetValue(subscriber, out List<Delegate> callbacks))
                    {
                        foreach (Delegate callback in callbacks)
                        {
                            yield return callback;
                        }
                    }
                }
            }
        }
    }
}
