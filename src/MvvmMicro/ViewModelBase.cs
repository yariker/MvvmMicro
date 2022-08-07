// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;

namespace MvvmMicro;

/// <summary>
/// Provides the base class for a view model.
/// </summary>
public class ViewModelBase : ObservableObject
{
    private static readonly Lazy<bool> IsInDesignModeLazy = new(GetIsInDesignMode);

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
    /// </summary>
    /// <param name="messenger">
    /// A custom messenger to be used by this view model. If <c>null</c>, then <see cref="MvvmMicro.Messenger.Default"/>
    /// will be used instead.
    /// </param>
    public ViewModelBase(IMessenger messenger = null)
    {
        Messenger = messenger ?? MvvmMicro.Messenger.Default;
    }

    /// <summary>
    /// Gets a value indicating whether the application is running in the context of a designer.
    /// </summary>
    public static bool IsInDesignMode => IsInDesignModeLazy.Value;

    /// <summary>
    /// Gets the messenger associated with this view model.
    /// </summary>
    protected IMessenger Messenger { get; }

    private static bool GetIsInDesignMode()
    {
#if NETFRAMEWORK || NET5_0
        // WPF.
        var descriptor = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(
            System.ComponentModel.DesignerProperties.IsInDesignModeProperty, typeof(System.Windows.FrameworkElement));
        return (bool)descriptor.Metadata.DefaultValue;
#elif WINDOWS_UWP
        // UWP.
        return Windows.ApplicationModel.DesignMode.DesignModeEnabled;
#else
        return false;
#endif
    }
}
