using System;
using System.Windows;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;

namespace Agrimanagr.WPF.UI.Views.MainViews
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<Uri>(this, "NavigationRequest", (uri) =>
                                                                           {
                                                                               this.ContentFrame.Navigate(uri);
                                                                           });
            ContentFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to quit this application?"
                                , "Agrimanagr: Exit Application",
                                MessageBoxButton.YesNo)
                == MessageBoxResult.No)
                e.Cancel = true;
            else
                Application.Current.Shutdown();
        }
    }
}
