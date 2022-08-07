// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace MvvmMicro.Helpers;

/// <summary>
/// A fast replacement for the slower <see cref="Delegate.DynamicInvoke"/> for <see cref="Action{T}"/>
/// with the support of contravariant invocation.
/// </summary>
internal class ActionInvoker
{
    private static readonly Type ActionGenericType = typeof(Action<>);

    private readonly Dictionary<ValueTuple<Type, MethodInfo>, Delegate> _invokerCache = new();

    public void Invoke<T>(Delegate method, T arg)
    {
        if (method is Action<T> action)
        {
            action(arg);
        }
        else
        {
            Action<Delegate, T> invoker;

            lock (_invokerCache)
            {
                var argType = ReflectionCache<T>.Type;
                var key = new ValueTuple<Type, MethodInfo>(argType, method.Method);

                if (_invokerCache.TryGetValue(key, out Delegate cache))
                {
                    invoker = (Action<Delegate, T>)cache;
                }
                else
                {
                    _invokerCache.Add(key, invoker = Compile<T>(method));
                }
            }

            invoker(method, arg);
        }
    }

    private static Action<Delegate, T> Compile<T>(Delegate method)
    {
        var callbackParameterType = method.Method.GetParameters()[0].ParameterType;
        var methodType = ActionGenericType.MakeGenericType(callbackParameterType);

        var methodParameter = Expression.Parameter(ReflectionCache<Delegate>.Type);
        var messageParameter = Expression.Parameter(ReflectionCache<T>.Type);

        var lambda = Expression.Lambda<Action<Delegate, T>>(
            Expression.Invoke(
                Expression.Convert(methodParameter, methodType),
                Expression.Convert(messageParameter, callbackParameterType)),
            methodParameter,
            messageParameter);

        return lambda.Compile();
    }
}
