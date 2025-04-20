using System;
using Microsoft.Extensions.DependencyInjection;
using MvvmMicro.Sample.Wpf.Services;

namespace MvvmMicro.Sample.Wpf.ViewModel;

public sealed class ViewModelLocator : IDisposable
{
    private readonly ServiceProvider _container;

    public ViewModelLocator()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddHttpClient(CatFactFeed.HttpClientName)
                         .AddStandardResilienceHandler();

        serviceCollection.AddTransient<ICatFactFeed, CatFactFeed>();

        if (ViewModelBase.IsInDesignMode)
        {
            // Design-time services.
            serviceCollection.AddTransient<ICatFactFeed, Design.CatFactFeed>();
        }
        else
        {
            // Run-time services.
            serviceCollection.AddTransient<ICatFactFeed, CatFactFeed>();
        }

        serviceCollection.AddSingleton(Messenger.Default);
        serviceCollection.AddSingleton<MainViewModel>();

        _container = serviceCollection.BuildServiceProvider();
    }

    public MainViewModel Main => _container.GetRequiredService<MainViewModel>();

    /// <inheritdoc />
    public void Dispose()
    {
        _container.Dispose();
    }
}
