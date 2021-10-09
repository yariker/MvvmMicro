// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;

namespace MvvmMicro.Test
{
    public interface IEventHandler
    {
        void EventHandler(object sender, EventArgs e);
    }
}
