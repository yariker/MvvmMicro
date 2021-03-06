using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Xunit;

namespace MvvmMicro.Test
{
    public class ViewModelBaseTest : ViewModelBase
    {
        public ViewModelBaseTest(IMessenger messenger = null)
            : base(messenger)
        {
        }

        [Fact]
        public void Ctor_Should_Use_SimpleMessenger_By_Default()
        {
            var viewModel = new ViewModelBaseTest(null);
            Assert.Same(MvvmMicro.Messenger.Default, viewModel.Messenger);
        }        
        
        [Fact]
        public void Ctor_Should_Accept_Custom_Messenger()
        {
            var messenger = Mock.Of<IMessenger>();
            var viewModel = new ViewModelBaseTest(messenger);
            Assert.Same(messenger, viewModel.Messenger);
        }
    }
}
