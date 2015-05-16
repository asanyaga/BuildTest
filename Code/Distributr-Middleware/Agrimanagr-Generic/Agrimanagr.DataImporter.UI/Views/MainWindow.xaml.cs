using System;
using System.Windows;
using Agrimanagr.DataImporter.Lib.ViewModels;
using GalaSoft.MvvmLight.Messaging;

namespace Agrimanagr.DataImporter.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<Uri>(this, "NavigationRequest", (uri) => this.ContentFrame.Navigate(uri));
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to quit this application?"
                                , "Masterdata importer: Exit Application",
                                MessageBoxButton.YesNo)
                == MessageBoxResult.No)
                e.Cancel = true;
            else
                Application.Current.Shutdown();
        }
    }
}