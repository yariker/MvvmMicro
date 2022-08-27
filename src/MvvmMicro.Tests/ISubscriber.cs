// Copyright (c) Yaroslav Bugaria. All rights reserved.

namespace MvvmMicro.Test;

public interface ISubscriber<in T>
{
    void Callback(T message);
}
