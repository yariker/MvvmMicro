// Copyright (c) Yaroslav Bugaria. All rights reserved.

#if NETFRAMEWORK

using System.Windows.Threading;

namespace MvvmMicro.Test;

internal static class DispatcherHelper
{
    public static void DoEvents()
    {
        var frame = new DispatcherFrame();
        Dispatcher.CurrentDispatcher.BeginInvoke(() => frame.Continue = false, DispatcherPriority.Background);
        Dispatcher.PushFrame(frame);
    }
}

#endif
