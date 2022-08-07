// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;

namespace MvvmMicro.Helpers;

internal static class ReflectionCache<T>
{
    public static readonly Type Type = typeof(T);
}
