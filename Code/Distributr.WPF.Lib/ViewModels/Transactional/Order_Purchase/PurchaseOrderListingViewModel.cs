using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.WPF.Lib.Services.Service;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Admin;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Order_Purchase
{
    public class PurchaseOrderListingViewModel : OrdersListingBaseViewModel
    {
        public PurchaseOrderListingViewModel()
        {
            ViewSelectedOrderCommand = new RelayCommand<OrderItemSummary>(ViewSelectedOrder);
            CreateOrderCommand = new RelayCommand(CreateOrder);
            ViewPrintableOrderCommand = new RelayCommand<OrderItemSummary>(ViewPrintableOrder);
        }

        #region methods

        protected override void ContinueSelectedOrder(OrderItemSummary obj)
        {
            var uri = "/views/Orders_Purchase/CreatePurchaseOrder.xaml";
            Messenger.Default.Send(new PurchaseOrderContinueMessage { Id = obj.OrderId, IsUnConfirmed = true });
            NavigateCommand.Execute(uri);
        }

        private void CreateOrder()
        {
            var uri = "/Views/Orders_Purchase/CreatePurchaseOrder.xaml";
            NavigateCommand.Execute(uri);
        }

        private void ViewSelectedOrder(OrderItemSummary summary)
        {
            var uri = "/Views/Orders_Purchase/ViewPurchaseOrder.xaml";
            Messenger.Default.Send(new ViewModelMessage {Id = summary.OrderId});
            NavigateCommand.Execute(uri);
        }


        protected override void Load(bool isFirstLoad = false)
        {
            if (isFirstLoad)
            {
                SetUp();
                isUnconfirmedTab = true;
            }
            ListOrders();
        }

        protected override void SetUp()
        {
            OrdersSummaryList.Clear();
            try
            {
                using (var container = NestedContainer)
                {
                    var _configService = Using<IConfigService>(container);
                    GeneralSetting recordsPerPageSetting =
                        Using<IGeneralSettingRepository>(container).GetByKey(GeneralSettingKey.RecordsPerPage);
                    ItemsPerPage = recordsPerPageSetting != null
                                       ? Convert.ToInt32(recordsPerPageSetting.SettingValue)
                                       : 10;
                    CanCreateOrders = _configService.ViewModelParameters.CurrentUserRights.CanCreateOrder;
                    CanDispatchOrders = _configService.ViewModelParameters.CurrentUserRights.CanDispatchOrder;
                    CanReceivePayments = _configService.ViewModelParameters.CurrentUserRights.CanCreateOrder;
                    CanApproveOrder = _configService.ViewModelParameters.CurrentUserRights.CanApproveOrders;
                }
                SelectedOrderType = OrderType.DistributorToProducer;
                SelectedOrderStatus = DocumentStatus.New;
                ListOrders();
            }
            catch
            {

            }
        }

        protected override void ClearSearchText()
        {
            base.ClearSearchText();
            ListOrders();
        }

        protected override void TabSelectionChanged(SelectionChangedEventArgs e)
        {
            CurrentPage = 1;
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(delegate
                                                                    {
                                                                        if (e.Source.GetType() != typeof (TabControl))
                                                                            return;

                                                                        TabItem tabItem = e.AddedItems[0] as TabItem;
                                                                        LoadSelectedTab(tabItem);
                                                                        e.Handled = true;
                                                                    }));
        }

        protected override void LoadSelectedTab(TabItem selectedTab)
        {
            isUnconfirmedTab = false;
            CurrentPage = 1;
            switch (selectedTab.Name)
            {
                case "PendingApprovalTab":
                    SelectedOrderStatus = DocumentStatus.Confirmed;
                    break;
                case "ApprovedTab":
                    SelectedOrderStatus = DocumentStatus.Approved;
                    break;
                case "IncompleteTab":
                    SelectedOrderStatus = DocumentStatus.New;
                    isUnconfirmedTab = true;
                    break;

                case "RejectedOrdersTab":
                    SelectedOrderStatus = DocumentStatus.Rejected;
                    break;
            }
            ListOrders();
          
        }

        private void ViewPrintableOrder(OrderItemSummary item)
        {
            using (var c = NestedContainer)
            {
                Using<IPrintableDocumentViewer>(c).ViewDocument(item.OrderId, DocumentType.Order);
            }
        }

        #endregion

        #region Commands

        public RelayCommand DispatchOrderCommand { get; set; }
        public RelayCommand CreateOrderCommand { get; set; }
        public RelayCommand<OrderItemSummary> ViewSelectedOrderCommand { get; set; }
        public RelayCommand<OrderItemSummary> ViewPrintableOrderCommand { get; set; }

        #endregion

       
    }
}
