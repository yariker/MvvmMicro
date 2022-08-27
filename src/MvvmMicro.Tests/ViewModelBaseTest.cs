// Copyright (c) Yaroslav Bugaria. All rights reserved.

using Moq;
using Xunit;

namespace MvvmMicro.Test;

public class ViewModelBaseTest
{
    [Fact]
    public void Ctor_UsesSimpleMessengerByDefault()
    {
        var viewModel = new ViewModelBaseShim();
        Assert.Same(Messenger.Default, viewModel.Messenger);
    }

    [Fact]
    public void Ctor_AcceptsCustomMessenger()
    {
        var messenger = Mock.Of<IMessenger>();
        var viewModel = new ViewModelBaseShim(messenger);
        Assert.Same(messenger, viewModel.Messenger);
    }

    private class ViewModelBaseShim : ViewModelBase
    {
        public ViewModelBaseShim(IMessenger messenger = null)
            : base(messenger)
        {
        }

        public new IMessenger Messenger => base.Messenger;
    }
}
