// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System.Collections.Generic;

namespace MvvmMicro.Helpers;

internal class ListPool<T>
{
    private readonly Queue<List<T>> _queue = new();

    public List<T> Get()
    {
        lock (_queue)
        {
            if (_queue.Count > 0)
            {
                return _queue.Dequeue();
            }
        }

        return new List<T>();
    }

    public void Return(List<T> list)
    {
        list.Clear();

        lock (_queue)
        {
            _queue.Enqueue(list);
        }
    }
}
