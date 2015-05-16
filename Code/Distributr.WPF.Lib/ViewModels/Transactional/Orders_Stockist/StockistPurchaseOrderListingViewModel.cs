using System;
using System.Windows.Controls;
using System.Windows.Threading;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.WPF.Lib.Services.Service;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Orders_Stockist
{
    public class StockistPurchaseOrderListingViewModel : OrdersListingBaseViewModel
    {
        public StockistPurchaseOrderListingViewModel()
        {
            ViewSelectedOrderCommand = new RelayCommand<OrderItemSummary>(ViewSelectedOrder);
            CreateOrderCommand = new RelayCommand(CreateOrder);
            ViewPrintableOrderCommand = new RelayCommand<OrderItemSummary>(ViewPrintableOrder);
            ProcessOrderCommand = new RelayCommand<OrderItemSummary>(ProcessOrder);
        }

        private void ProcessOrder(OrderItemSummary summary)
        {
            var uri = "/Views/Orders_Stockist/ApproveStockistOrder.xaml";
            Messenger.Default.Send(new ApproveStockistPurchaseOrderMessage { Id = summary.OrderId });
            NavigateCommand.Execute(uri);
        }

        #region methods

        protected override void ContinueSelectedOrder(OrderItemSummary obj)
        {
            var uri = "/views/Orders_Stockist/CreateStockistOrder.xaml";
            Messenger.Default.Send(new StockistPurchaseOrderContinueMessage { Id = obj.OrderId, IsUnConfirmed = true });
            NavigateCommand.Execute(uri);
        }

        private void CreateOrder()
        {
            var uri = "/Views/Orders_Stockist/CreateStockistOrder.xaml";
            NavigateCommand.Execute(uri);
        }

        private void ViewSelectedOrder(OrderItemSummary summary)
        {
            var uri = "/Views/Orders_Stockist/ViewStockistPurchaseOrder.xaml";
            Messenger.Default.Send(new ViewModelMessage { Id = summary.OrderId });
            NavigateCommand.Execute(uri);
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
                SelectedOrderType = OrderType.SalesmanToDistributor;
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
                                                                        if (e.Source.GetType() != typeof(TabControl))
                                                                            return;

                                                                        TabItem tabItem = e.AddedItems[0] as TabItem;
                                                                        LoadSelectedTab(tabItem);
                                                                        e.Handled = true;
                                                                    }));
        }

        protected override void LoadSelectedTab(TabItem selectedTab)
        {
            isUnconfirmedTab = false;
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
        public RelayCommand<OrderItemSummary> ProcessOrderCommand { get; set; }
        public RelayCommand<OrderItemSummary> ViewSelectedOrderCommand { get; set; }
        public RelayCommand<OrderItemSummary> ViewPrintableOrderCommand { get; set; }

        #endregion


    }
}