// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.Reflection;

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
    /// <value><c>true</c> if a designer context was detected; otherwise, <c>false</c>.</value>
    /// <remarks>
    /// The following UI frameworks are supported: WPF, UWP, Xamarin.Forms, and Avalonia.
    /// </remarks>
    public static bool IsInDesignMode => IsInDesignModeLazy.Value;

    /// <summary>
    /// Gets the messenger associated with this view model.
    /// </summary>
    protected IMessenger Messenger { get; }

    private static bool GetIsInDesignMode()
    {
        return GetIsInDesignMode_WPF() ??
               GetIsInDesignMode_UWP() ??
               GetIsInDesignMode_XamarinForms() ??
               GetIsInDesignMode_Avalonia() ??
               false;
    }

    private static bool? GetIsInDesignMode_WPF()
    {
#if NETFRAMEWORK || NET8_0
        var descriptor = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(
            System.ComponentModel.DesignerProperties.IsInDesignModeProperty, typeof(System.Windows.FrameworkElement));

        return (bool)descriptor.Metadata.DefaultValue;
#else
        return null;
#endif
    }

    private static bool? GetIsInDesignMode_UWP()
    {
#if WINDOWS_UWP
        return Windows.ApplicationModel.DesignMode.DesignModeEnabled;
#else
        return null;
#endif
    }

    private static bool? GetIsInDesignMode_XamarinForms()
    {
        var design = Type.GetType("Xamarin.Forms.DesignMode, Xamarin.Forms.Core")?.GetProperty(
            "IsDesignModeEnabled", BindingFlags.Static | BindingFlags.Public)?.GetValue(null);

        return design is bool value ? value : null;
    }

    private static bool? GetIsInDesignMode_Avalonia()
    {
        var design = Type.GetType("Avalonia.Controls.Design, Avalonia.Controls")?.GetProperty(
            "IsDesignMode", BindingFlags.Static | BindingFlags.Public)?.GetValue(null);

        return design is bool value ? value : null;
    }
}
