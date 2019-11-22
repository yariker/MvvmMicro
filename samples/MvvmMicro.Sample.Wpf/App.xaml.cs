using System.Windows;
using Takesoft.MvvmMicro.Sample.Wpf.ViewModel;

namespace Takesoft.MvvmMicro.Sample.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnExit(object sender, ExitEventArgs e)
        {
            var locator = (ViewModelLocator)FindResource("Locator");
            locator?.Dispose();
        }
    }
}
