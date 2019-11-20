using System;

namespace Takesoft.MvvmMicro.Test
{
    public interface IEventHandler
    {
        void EventHandler(object sender, EventArgs e);
    }
}
