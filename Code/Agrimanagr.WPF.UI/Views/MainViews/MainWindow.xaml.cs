using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Distributr.WPF.Lib.ViewModels.MainPage;
using GalaSoft.MvvmLight.Messaging;

namespace Agrimanagr.WPF.UI.Views.MainViews
{
    public partial class MainWindow : Window
    {
        private AgrimanagrMainPageViewModel _vm;
        bool firstLoad = false;
        public MainWindow()
        {
            
            InitializeComponent();
            _vm = DataContext as AgrimanagrMainPageViewModel;

            
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

        public void SetSelectedTab()
        {
            tcMainPage.SelectedIndex = 0;
        }
        private void TcSettings_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (firstLoad)
            {
                _vm.TabSelectionChanged(e);
                //tcMainPage.SelectedIndex = 0;
            }

            firstLoad = true;
        }
    }
}
