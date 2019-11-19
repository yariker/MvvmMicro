using System;
using Autofac;
using PlaidSoft.MvvmMicro.Sample.NetFx.Services;

namespace PlaidSoft.MvvmMicro.Sample.NetFx.ViewModel
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