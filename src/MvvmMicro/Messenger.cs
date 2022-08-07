// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MvvmMicro.Helpers;

namespace MvvmMicro;

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
public class Messenger : IMessenger
{
    private readonly ActionInvoker _actionInvoker = new();
    private readonly Dictionary<Type, Channel> _registry = new();

    /// <summary>
    /// Gets the default instance of the <see cref="Messenger" />.
    /// </summary>
    public static IMessenger Default { get; } = new Messenger();

    /// <inheritdoc />
    public void Publish<T>(T message)
    {
        var type = ReflectionCache<T>.Type;
        var callbackLists = new List<List<Delegate>>();

        lock (_registry)
        {
            foreach (KeyValuePair<Type, Channel> entry in _registry)
            {
                if (entry.Key.IsAssignableFrom(type))
                {
                    entry.Value.GetCallbacks(callbackLists);
                }
            }
        }

        foreach (List<Delegate> callbacks in callbackLists)
        {
            foreach (Delegate callback in callbacks)
            {
                _actionInvoker.Invoke(callback, message);
            }
        }
    }

    /// <inheritdoc />
    public void Subscribe<T>(object subscriber, Action<T> callback)
    {
        if (subscriber == null)
        {
            throw new ArgumentNullException(nameof(subscriber));
        }

        if (callback == null)
        {
            throw new ArgumentNullException(nameof(callback));
        }

        var type = ReflectionCache<T>.Type;

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
        if (subscriber == null)
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
        private readonly List<WeakReference<object>> _subscribers = new();
        private readonly ConditionalWeakTable<object, List<Delegate>> _callbacks = new();

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

        public void GetCallbacks(ICollection<List<Delegate>> lists)
        {
            for (int i = _subscribers.Count - 1; i >= 0; i--)
            {
                if (_subscribers[i].TryGetTarget(out object subscriber) &&
                    _callbacks.TryGetValue(subscriber, out List<Delegate> callbacks))
                {
                    lists.Add(callbacks);
                }
                else
                {
                    _subscribers.RemoveAt(i);
                }
            }
        }
    }
}
