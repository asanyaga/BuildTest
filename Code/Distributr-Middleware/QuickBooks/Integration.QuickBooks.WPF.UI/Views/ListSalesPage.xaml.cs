﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using Integration.QuickBooks.Lib.QBIntegrationViewModels;
using Timer = System.Timers.Timer;

namespace Integration.QuickBooks.WPF.UI.Views
{
    /// <summary>
    /// Interaction logic for ListSalesPage.xaml
    /// </summary>
    public partial class ListSalesPage : Page
    {
        private QBListTransactionsViewModel _vm;
       
        public ListSalesPage()
        {
            InitializeComponent();
            _vm = DataContext as QBListTransactionsViewModel;
          tcMainPage.SelectedIndex = 1;
            //  tcMainPage.SelectedIndex = 0;

        }



        
    }
}
