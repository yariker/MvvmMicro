using System.Windows;
using MvvmMicro.Sample.Wpf.ViewModel;

namespace MvvmMicro.Sample.Wpf.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Messenger.Default.Subscribe<string>(this, message =>
        {
            if (message == Notifications.CloseWindow)
            {
                Close();
            }
        });
    }
}
