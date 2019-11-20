using System.Windows;
using PlaidSoft.MvvmMicro.Sample.NetFx.ViewModel;

namespace PlaidSoft.MvvmMicro.Sample.NetFx
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
