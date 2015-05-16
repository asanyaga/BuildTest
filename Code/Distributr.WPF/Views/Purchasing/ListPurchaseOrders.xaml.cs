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
using Distributr.WPF.Lib;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.Purchasing;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Distributr.WPF.UI.Views.Purchasing
{
    public partial class ListPurchaseOrders : Page
    {
        bool isInitialized = false;
        public ListPurchaseOrders()
        {
            isInitialized = false;
            InitializeComponent();
            isInitialized = true;
            this.Loaded += new RoutedEventHandler(ListPurchaseOrders_Loaded);
        }

        void ListPurchaseOrders_Loaded(object sender, RoutedEventArgs e)
        {
            string _tab = NavigationService.Source.OriginalString.ParseQueryString("tab");
            if (!string.IsNullOrEmpty(_tab))
            {
                string purchaseorderStatus = _tab;
                ListPurchaseOrderViewModel vm = DataContext as ListPurchaseOrderViewModel;
                vm.TabItemName = purchaseorderStatus;
                TabItem matchingItem = tabControlPurchaseOrder.Items
                    .Cast<TabItem>().FirstOrDefault(item => item.Name == purchaseorderStatus);
                if (matchingItem != null)
                    tabControlPurchaseOrder.SelectedItem = matchingItem;
            }

            string _approved = PresentationUtility.GetLastTokenFromUri(NavigationService.CurrentSource);
            if (!string.IsNullOrEmpty(_approved))
            {
                tabControlPurchaseOrder.SelectedIndex = tabControlPurchaseOrder.Items.IndexOf(tabItemApproved);
            }
            LocalizeLabels();
        }

        private void LocalizeLabels()
        {
            IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            txtpurchaseordersummary.Text = messageResolver.GetText("sl.po.summary.name");
            labelOrderRef.Content = messageResolver.GetText("sl.po.summary.search.orderref");
            tabItemIncomplete.Header = messageResolver.GetText("sl.po.summary.tab.incomplete.name");
            tabItemPendingAproval.Header = messageResolver.GetText("sl.po.summary.tab.pendingapproval.name");
            tabItemApproved.Header = messageResolver.GetText("sl.po.summary.tab.approved.name");
            tabItemRejected.Header = messageResolver.GetText("sl.po.summary.tab.rejected.name");
            //incomplete tab grid
            gridcolrefno.Header = messageResolver.GetText("sl.po.summary.incomplete.grid.col.refno");
            gridcolrequired.Header = messageResolver.GetText("sl.po.summary.incomplete.grid.col.required");
            gridcolissuedby.Header = messageResolver.GetText("sl.po.summary.incomplete.grid.col.issuedby");
            gridcolstatus.Header = messageResolver.GetText("sl.po.summary.incomplete.grid.col.status");
            gridcolnetamount.Header = messageResolver.GetText("sl.po.summary.incomplete.grid.col.netamount");
            gridcolvatamount.Header = messageResolver.GetText("sl.po.summary.incomplete.grid.col.vatamount");
            gridcolgrossamount.Header = messageResolver.GetText("sl.po.summary.incomplete.grid.col.grossamount");

            //pending Approval tab grid
            dg1_gridcolrefno.Header = messageResolver.GetText("sl.po.summary.pa.grid.col.refno");
            dg1_gridcolrequired.Header = messageResolver.GetText("sl.po.summary.pa.grid.col.required");
            dg1_gridcolissuedby.Header = messageResolver.GetText("sl.po.summary.pa.grid.col.issuedby");
            dg1_gridcolstatus.Header = messageResolver.GetText("sl.po.summary.pa.grid.col.status");
            dg1_gridcolnetamount.Header = messageResolver.GetText("sl.po.summary.pa.grid.col.netamount");
            dg1_gridcolvatamount.Header = messageResolver.GetText("sl.po.summary.pa.grid.col.vatamount");
            dg1_gridcolgrossamount.Header = messageResolver.GetText("sl.po.summary.pa.grid.col.grossamount");

            //Approved tab grid
            dgApproved_gridcolrefno.Header = messageResolver.GetText("sl.po.summary.approved.grid.col.refno");
            dgApproved_gridcolrequired.Header = messageResolver.GetText("sl.po.summary.approved.grid.col.required");
            dgApproved_gridcolissuedby.Header = messageResolver.GetText("sl.po.summary.approved.grid.col.issuedby");
            dgApproved_gridcolstatus.Header = messageResolver.GetText("sl.po.summary.approved.grid.col.status");
            dgApproved_gridcolnetamount.Header = messageResolver.GetText("sl.po.summary.approved.grid.col.netamount");
            dgApproved_gridcolvatamount.Header = messageResolver.GetText("sl.po.summary.approved.grid.col.vatamount");
            dgApproved_gridcolgrossamount.Header = messageResolver.GetText("sl.po.summary.approved.grid.col.grossamount");

            //Rejected tab grid
            gdReject_gridcolrefno.Header = messageResolver.GetText("sl.po.summary.rejected.grid.col.refno");
            gdReject_gridcolrequired.Header = messageResolver.GetText("sl.po.summary.rejected.grid.col.required");
            gdReject_gridcolissuedby.Header = messageResolver.GetText("sl.po.summary.rejected.grid.col.issuedby");
            gdReject_gridcolstatus.Header = messageResolver.GetText("sl.po.summary.rejected.grid.col.status");
            gdReject_gridcolnetamount.Header = messageResolver.GetText("sl.po.summary.rejected.grid.col.netamount");
            gdReject_gridcolvatamount.Header = messageResolver.GetText("sl.po.summary.rejected.grid.col.vatamount");
            gdReject_gridcolgrossamount.Header = messageResolver.GetText("sl.po.summary.rejected.grid.col.grossamount");



        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isInitialized)
                return;
            if (txtSearch.Text == "")
            {
                ListPurchaseOrderViewModel vm = DataContext as ListPurchaseOrderViewModel;
                vm.LoadOrders.Execute(null);
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialized)
                return;
            if (e.Source.GetType() != typeof(TabControl))
                return;

            TabControl tab = sender as TabControl;
            TabItem tabItem = tab.Items[tab.SelectedIndex] as TabItem;
            ListPurchaseOrderViewModel vm = DataContext as ListPurchaseOrderViewModel;
            vm.TabItemName = tabItem.Name;
            vm.LoadOrders.Execute(null);
        }

        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = sender as Hyperlink;
            Guid id = new Guid(hl.Tag.ToString());
            string url = "/views/purchasing/editpurchaseorder.xaml?orderid=" + id.ToString();
            NavigationService.Navigate(new Uri(url, UriKind.Relative));
        }
    }
}
