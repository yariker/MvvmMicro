// Copyright (c) Yaroslav Bugaria. All rights reserved.

namespace MvvmMicro.Test;

public interface ICommandHandler<in T>
{
    bool CanExecute(T parameter);

    void Execute(T parameter);
}
