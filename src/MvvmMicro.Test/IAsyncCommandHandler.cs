// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System.Threading.Tasks;

namespace MvvmMicro.Test;

public interface IAsyncCommandHandler
{
    bool CanExecute();

    Task ExecuteAsync();
}
