﻿using PlaidSoft.MvvmMicro.Sample.NetFx.ViewModel;
using System.Windows;

namespace PlaidSoft.MvvmMicro.Sample.NetFx.View
{
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
            SimpleMessenger.Default.Subscribe<string>(message =>
            {
                if (message == Notifications.CloseWindow)
                {
                    Close();
                }
            });
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            SimpleMessenger.Default.Unsubscribe(this);
        }
    }
}
