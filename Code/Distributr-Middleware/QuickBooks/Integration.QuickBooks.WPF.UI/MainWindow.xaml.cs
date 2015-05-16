using System;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Integration.QuickBooks.Lib.QBIntegrationViewModels;

namespace Integration.QuickBooks.WPF.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private QBMainWindowViewModel _vm;
        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<Uri>(this, "NavigationRequest", (uri) =>
            {
                this.ContentFrame.Navigate(uri);
            });
            _vm = DataContext as QBMainWindowViewModel;
            _vm.ExitApplicationEventHandler += (s, e) => this.Close();
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to quit this application?"
                                , "QuickBooks Integration: Exit Application",
                                MessageBoxButton.YesNo)
                == MessageBoxResult.No)
                e.Cancel = true;
            else
                Application.Current.Shutdown();
        }
    }
}
