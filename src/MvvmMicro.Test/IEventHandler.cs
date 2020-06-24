using System;

namespace MvvmMicro.Test
{
    public interface IEventHandler
    {
        void EventHandler(object sender, EventArgs e);
    }
}
