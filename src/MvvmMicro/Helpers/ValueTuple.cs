// Copyright (c) Yaroslav Bugaria. All rights reserved.

#if NETFRAMEWORK

using System;
using System.Collections.Generic;

namespace MvvmMicro.Helpers;

/// <summary>
/// Represents a value tuple with 2 components.
/// </summary>
/// <typeparam name="T1">Type of <see cref="Item1"/>.</typeparam>
/// <typeparam name="T2">Type of <see cref="Item2"/>.</typeparam>
internal struct ValueTuple<T1, T2>(T1 item1, T2 item2) : IEquatable<ValueTuple<T1, T2>>
{
    public T1 Item1 = item1;

    public T2 Item2 = item2;

    public static bool operator ==(ValueTuple<T1, T2> left, ValueTuple<T1, T2> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ValueTuple<T1, T2> left, ValueTuple<T1, T2> right)
    {
        return !left.Equals(right);
    }

    public bool Equals(ValueTuple<T1, T2> other)
    {
        return EqualityComparer<T1>.Default.Equals(Item1, other.Item1) &&
               EqualityComparer<T2>.Default.Equals(Item2, other.Item2);
    }

    public override bool Equals(object obj)
    {
        return obj is ValueTuple<T1, T2> other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (EqualityComparer<T1>.Default.GetHashCode(Item1) * 397) ^
                   EqualityComparer<T2>.Default.GetHashCode(Item2);
        }
    }
}

#endif
