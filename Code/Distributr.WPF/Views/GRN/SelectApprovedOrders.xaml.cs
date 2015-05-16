using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.ViewModels.Transactional.GRN;

using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Distributr.WPF.UI.Views.GRN
{
    public partial class SelectApprovedOrders : Page
    {
        public SelectApprovedOrders()
        {
            InitializeComponent();
            this.Loaded += SelectApprovedOrders_Loaded;
        }
        SelectApprovedOrdersViewModel _vm;
        void SelectApprovedOrders_Loaded(object sender, RoutedEventArgs e)
        {
            LocalizeLabels();
            _vm = this.DataContext as SelectApprovedOrdersViewModel;
            _vm.LoadOrders.Execute(null);
        }

        private void LocalizeLabels()
        {
            IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            lblPageHeader.Content = messageResolver.GetText("sl.inventory.receive.select.title");
            colorderref.Header = messageResolver.GetText("sl.inventory.receive.select.grid.col.orderref");
            colorderdate.Header = messageResolver.GetText("sl.inventory.receive.select.grid.col.orderdate");
            colordervalue.Header = messageResolver.GetText("sl.inventory.receive.select.grid.col.ordervalue");
            colaction.Header = messageResolver.GetText("sl.inventory.receive.select.grid.col.action");

        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SelectOrder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void chkSelectOrder_Checked(object sender, RoutedEventArgs e)
        {
            _vm.SelectOrder();
        }

        private void chkSelectOrder_Unchecked(object sender, RoutedEventArgs e)
        {
            _vm.UnSelectOrder();
        }
    }
}
