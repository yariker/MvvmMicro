// Copyright (c) Yaroslav Bugaria. All rights reserved.

namespace MvvmMicro.Test
{
    public interface ICommandHandler
    {
        bool CanExecute();

        void Execute();
    }
}
