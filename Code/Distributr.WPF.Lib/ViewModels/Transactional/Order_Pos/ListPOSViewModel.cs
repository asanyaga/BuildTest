using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Distributr.Core.ClientApp;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Order_Pos
{
    public class ListPOSViewModel : OrdersListingBaseViewModel
    {
        public ListPOSViewModel()
        {
            NewSaleCommand = new RelayCommand(DoASale);
            ClearCommand = new RelayCommand(ClearViewModel);
            ViewLineItemCommand = new RelayCommand<OrderItemSummary>(ViewSelectedLineItem);
            ReceivePaymentsCommand = new RelayCommand<OrderItemSummary>(ReceivePayment);
            ViewPrintableOrderCommand = new RelayCommand<OrderItemSummary>(ViewPrintableOrder);

        }

        #region Properties

        public RelayCommand LoadSalesCommand { get; set; }
        public RelayCommand ListPOSSalesPageLoadedCommand { get; set; }
        public RelayCommand NewSaleCommand { get; set; }
        public RelayCommand<OrderItemSummary> ViewLineItemCommand { get; set; }
        public RelayCommand<OrderItemSummary> ViewPrintableOrderCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }

        public const string CanCreateSalesPropertyName = "CanCreateSales";
        private bool _canCreateSales;
        public bool CanCreateSales
        {
            get
            {
                return _canCreateSales;
            }

            set
            {
                if (_canCreateSales == value)
                {
                    return;
                }

                var oldValue = _canCreateSales;
                _canCreateSales = value;
                RaisePropertyChanged(CanCreateSalesPropertyName);
            }
        }



        #endregion

        #region methods

        private void ReceivePayment(OrderItemSummary selectedItem)
        {
            if (selectedItem.OutstandingAmount < 0)
            {
                MessageBox.Show( /*"Gross amount should be greater than zero"*/
                    GetLocalText("sl.pos.receivepayments.messagebox.grossamountiszero")
                    , "!" + GetLocalText("sl.payment.title") /*"Distributr: Payment Module"*/
                    , MessageBoxButton.OK);
                return;
            }
           
            Submitpayments(selectedItem);
        }
        protected override void ContinueSelectedOrder(OrderItemSummary obj)
        {
            const string uri = "/views/Order_Pos/addpos.xaml";
            Messenger.Default.Send(new SaleOrderContinueMessage { Id = obj.OrderId,IsUnConfirmed = true});
            NavigateCommand.Execute(uri);
        }
        private void ViewSelectedLineItem(OrderItemSummary selectedItem)
        {
            const string uri = "/views/Order_Pos/ViewPOS.xaml";
            Messenger.Default.Send<ViewModelMessage>(new ViewModelMessage {Id = selectedItem.OrderId});
            NavigateCommand.Execute(uri);
        }

        private void ClearViewModel()
        {
            SearchText = string.Empty;
            CanCreateSales = false;
        }

        private void DoASale()
        {
            var uri = "/views/Order_Pos/AddPOS.xaml";
            NavigateCommand.Execute(uri);
        }

        private void ViewPrintableOrder(OrderItemSummary item)
        {
            using (var c = NestedContainer)
            {
                Using<IPrintableDocumentViewer>(c).ViewDocument(item.OrderId, DocumentType.Order);
            }
        }

        private void Submitpayments(OrderItemSummary orderSummary)
        {
            PaymentInfoList.Clear();
            try
            {
                using (var container = NestedContainer)
                {
                    BasicConfig config = container.GetInstance<IConfigService>().Load();
                    var invoice = Using<IInvoiceRepository>(container).GetInvoiceByOrderId(orderSummary.OrderId);
                    string incoicedocref = invoice != null ? invoice.DocumentReference : "";

                    var payInfo =
                        Using<IPaymentPopup>(container).GetPayments(orderSummary.OutstandingAmount,
                                                                    orderSummary.OrderReference, incoicedocref);
                    foreach (var paymentInfo in payInfo)
                    {
                        if (!PaymentInfoList.Contains(paymentInfo))
                            PaymentInfoList.Add(paymentInfo);
                    }
                    var order = Using<IMainOrderRepository>(container).GetById(orderSummary.OrderId);
                    order.ChangeccId(GetConfigParams().CostCentreApplicationId);
                    var invoiceId = Using<IInvoiceRepository>(container).GetInvoiceByOrderId(order.Id).Id;
                    var posWorkflow = Using<IOrderPosWorkflow>(container);
                    if (order != null)
                    {
                        foreach (var paymentInfo in PaymentInfoList.Where(p => p.PaymentModeUsed != PaymentMode.Credit))
                        {
                            paymentInfo.InvoiceId = invoiceId;
                            order.AddOrderPaymentInfoLineItem(paymentInfo);
                        }
                        posWorkflow.Submit(order,config);
                        Load();
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override void ClearSearchText()
        {
            base.ClearSearchText();
            ListOrders();
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
            using (IContainer cont = NestedContainer)
            {
                CanCreateSales = Using<IConfigService>(cont).ViewModelParameters.CurrentUserRights.CanManagePOSSales;
                CanReceivePayments = Using<IConfigService>(cont).ViewModelParameters.CurrentUserRights.CanManagePOSSales;
                PageTitle = "POS Sales";
                SelectedOrderStatus = DocumentStatus.Closed;
                SelectedOrderType = OrderType.DistributorPOS;
                ItemsPerPage = 10;
            }
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
            switch (selectedTab.Name)
            {
                case "PendingConfirmationTabItem":
                    SelectedOrderStatus = DocumentStatus.New;
                    isUnconfirmedTab = true;
                    break;
                case "CompleteSalesTabItem":
                    SelectedOrderStatus = DocumentStatus.Closed;
                    break;
                case "IncompleteTabItem":
                    SelectedOrderStatus = DocumentStatus.Confirmed;
                    break;
                case "OutstandingTabItem":
                    SelectedOrderStatus = DocumentStatus.Outstanding;
                    break;
                default:
                    SelectedOrderStatus = DocumentStatus.New;
                    break;

            }
            Load();
        }

        #endregion

    }
}
