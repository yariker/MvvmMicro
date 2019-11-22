using System;
using Autofac;
using Takesoft.MvvmMicro.Sample.Wpf.Services;

namespace Takesoft.MvvmMicro.Sample.Wpf.ViewModel
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

            containerBuilder.RegisterInstance(SimpleMessenger.Default).As<IMessenger>();
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