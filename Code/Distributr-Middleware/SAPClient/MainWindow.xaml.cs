﻿using System;
using System.ComponentModel;
using System.Windows;
using Distributr_Middleware.WPF.Lib.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using SAPUtilityLib.ViewModels;

namespace SAPClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowVM _vm;
        public MainWindow()
        {
            InitializeComponent();
            _vm = DataContext as MainWindowVM;
            Messenger.Default.Register<Uri>(this, "NavigationRequest", (uri) => this.ContentFrame.Navigate(uri));
            _vm.ExitApplicationEventHandler += (s, e) => this.Close();
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to quit this application?"
                               , "Integration: Exit Application",
                               MessageBoxButton.YesNo)
               == MessageBoxResult.No)
                e.Cancel = true;
            else
                Application.Current.Shutdown();
        }
    }
}