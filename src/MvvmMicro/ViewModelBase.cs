using System;

namespace Takesoft.MvvmMicro
{
    /// <summary>
    /// Provides the base class for a view model.
    /// </summary>
    public class ViewModelBase : ObservableObject
    {
        private static readonly Lazy<bool> IsInDesignModeLazy = new Lazy<bool>(GetIsInDesignMode);

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        /// <param name="messenger">
        /// A custom messenger to be used by this view model. If <c>null</c>, then <see cref="SimpleMessenger.Default"/>
        /// will be used instead.
        /// </param>
        public ViewModelBase(IMessenger messenger = null)
        {
            Messenger = messenger ?? SimpleMessenger.Default;
        }

        /// <summary>
        /// Gets the messenger associated with this view model.
        /// </summary>
        protected IMessenger Messenger { get; }

        /// <summary>
        /// Gets the value determining whether the application is running in the context of a designer.
        /// </summary>
        public static bool IsInDesignMode => IsInDesignModeLazy.Value;

        private static bool GetIsInDesignMode()
        {
#if NETFRAMEWORK
            var descriptor = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(
                System.ComponentModel.DesignerProperties.IsInDesignModeProperty, typeof(System.Windows.FrameworkElement));

            return (bool)descriptor.Metadata.DefaultValue;
#else
            // UWP.
            var uwp = Type.GetType("Windows.ApplicationModel.DesignMode, Windows.Foundation.UniversalApiContract, ContentType=WindowsRuntime");
            if (uwp != null)
            {
                return (bool)uwp.GetProperty("DesignModeEnabled").GetValue(null);
            }

            // Xamarin.Forms.
            var xamarin = Type.GetType("Xamarin.Forms.DesignMode, Xamarin.Forms.Core");
            if (xamarin != null)
            {
                return (bool)uwp.GetProperty("IsDesignModeEnabled").GetValue(null);
            }

            return false;
#endif
        }
    }
}