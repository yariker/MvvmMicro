// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System.Threading.Tasks;

namespace MvvmMicro.Test
{
    public interface IAsyncCommandHandler<in T>
    {
        bool CanExecute(T parameter);

        Task ExecuteAsync(T parameter);
    }
}
