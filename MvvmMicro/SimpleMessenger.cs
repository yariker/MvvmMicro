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
    ///             The subscribers are kept as weak references, which allows them to be garbage collected
    ///             even without an explicit <see cref="Unsubscribe(object)"/> call.
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
                        entry.Value.GetCallbacks(callbacks);
                    }
                }
            }

            foreach (Action<T> callback in callbacks)
            {
                callback(message);
            }
        }

        /// <inheritdoc />
        public void Subscribe<T>(object subscriber, Action<T> callback)
        {
            if (subscriber is null)
            {
                throw new ArgumentNullException(nameof(subscriber));
            }
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

                channel.Subscribe(subscriber, callback);
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

            public void Subscribe(object subscriber, Delegate callback)
            {
                if (!_callbacks.TryGetValue(subscriber, out List<Delegate> callbacks))
                {
                    _subscribers.Add(new WeakReference<object>(subscriber));
                    _callbacks.Add(subscriber, callbacks = new List<Delegate>());
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

            public void GetCallbacks(List<Delegate> list)
            {
                for (int i = _subscribers.Count - 1; i >= 0; i--)
                {
                    if (_subscribers[i].TryGetTarget(out object subscriber) && 
                        _callbacks.TryGetValue(subscriber, out List<Delegate> callbacks))
                    {
                        list.AddRange(callbacks);
                    }
                    else
                    {
                        _subscribers.RemoveAt(i);
                    }
                }
            }
        }
    }
}
