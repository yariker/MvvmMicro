using System;
using Autofac;
using MvvmMicro.Sample.Wpf.Services;

namespace MvvmMicro.Sample.Wpf.ViewModel
{
    public sealed class ViewModelLocator : IDisposable
    {
        private readonly IContainer _container;

        public ViewModelLocator()
        {
            var containerBuilder = new ContainerBuilder();

            if (ViewModelBase.IsInDesignMode)
            {
                // Design-time services.
                containerBuilder.RegisterType<Design.CatFactFeed>().As<ICatFactFeed>().SingleInstance();
            }
            else
            {
                // Run-time services.
                containerBuilder.RegisterType<CatFactFeed>().As<ICatFactFeed>().SingleInstance();
            }

            containerBuilder.RegisterInstance(Messenger.Default).As<IMessenger>();
            containerBuilder.RegisterType<MainViewModel>().AsSelf().SingleInstance();

            _container = containerBuilder.Build();
        }

        public MainViewModel Main => _container.Resolve<MainViewModel>();

        /// <inheritdoc />
        public void Dispose()
        {
            _container.Dispose();
        }
    }
}