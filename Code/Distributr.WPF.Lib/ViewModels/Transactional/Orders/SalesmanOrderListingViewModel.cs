using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.MainPage;
using Distributr.WPF.Lib.ViewModels.Transactional.Fiscalprinter;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Orders
{
    public class SalesmanOrderListingViewModel : OrdersListingBaseViewModel
    {
        private Page currentpage;
        public SalesmanOrderListingViewModel()
        {
            OrdersSummaryList = new ObservableCollection<OrderItemSummary>();
            ProcessSelectedOrderCommand = new RelayCommand<OrderItemSummary>(ProcessOrder);
            ViewSelectedOrderCommand = new RelayCommand<OrderItemSummary>(ViewSelectedOrder);
            CreateOrderCommand = new RelayCommand(CreateOrder);
            ReceivePaymentCommand = new RelayCommand<OrderItemSummary>(ReceivePayment);
            ViewInvoiceCommand = new RelayCommand<OrderItemSummary>(ViewInvoice);
            ViewPrintableDispatchNoteCommand = new RelayCommand<OrderItemSummary>(ViewPrintableDispatchNote);
            ViewPrintableOrderCommand = new RelayCommand<OrderItemSummary>(ViewPrintableOrder);
            ProcessSelectedOrdersCommand=new RelayCommand<Button>(ProcessSelectedOrders);
            SelectAllUnApprovedOrdersCommand = new RelayCommand<CheckBox>(SelectAllUnApprovedOrders);
            ProcessAndDispatchSelectedOrdersCommand = new RelayCommand<Button>(ApproveAndDispatch);
            SelectedAndApprovedOrderIds=new List<Guid>();
            ConfigureFiscalPrinter();
          
        }
        protected override void LoadPage(Page page)
        {
            currentpage = page;
            base.LoadPage(page);
        }
        protected  override void ContinueSelectedOrder(OrderItemSummary obj)
        {
            var uri = "/views/orders/CreateOrder.xaml";
            Messenger.Default.Send(new OrderContinueMessage { Id = obj.OrderId, IsUnConfirmed = true });
            NavigateCommand.Execute(uri);
        }

        #region Properties


        public OrderApprovalViewModel OrderApprovalViewModel
        {
            get {return SimpleIoc.Default.GetInstance<OrderApprovalViewModel>(); }
        }

        public OrderDispatchViewModel OrderDispatchViewModel
        {
            get { return SimpleIoc.Default.GetInstance<OrderDispatchViewModel>(); }
        }
        public RelayCommand DispatchOrderCommand { get; set; }
        public RelayCommand CreateOrderCommand { get; set; }
        public RelayCommand<OrderItemSummary> ReceivePaymentCommand { get; set; }
        public RelayCommand<OrderItemSummary> ViewInvoiceCommand { get; set; }

        public RelayCommand<OrderItemSummary> ProcessSelectedOrderCommand { get; set; }
        public RelayCommand<OrderItemSummary> ViewSelectedOrderCommand { get; set; }
        public RelayCommand<Button> ProcessSelectedOrdersCommand { get; set; }
        public RelayCommand<Button> ProcessAndDispatchSelectedOrdersCommand { get; set; }

        public RelayCommand<OrderItemSummary> ViewPrintableDispatchNoteCommand { get; set; }
        public RelayCommand<OrderItemSummary> ViewPrintableOrderCommand { get; set; }
        public RelayCommand<CheckBox> SelectAllUnApprovedOrdersCommand { get; set; }
        public List<Guid> SelectedAndApprovedOrderIds { get; set; }
      


      

        #endregion

        #region methods
        private FiscalPrinterUtility _printerUtility = null;
        public FiscalPrinterUtility PrinterUtility { get { return _printerUtility; } }
        void ConfigureFiscalPrinter()
        {

            int port = 2;
            int portSpeed = 115200;
            using (var c = NestedContainer)
            {
                var printerRepo = Using<IGeneralSettingRepository>(c);
                var printerEnabled = printerRepo.GetByKey(GeneralSettingKey.FiscalPrinterEnabled);
                if (printerEnabled != null && !string.IsNullOrEmpty(printerEnabled.SettingValue))
                {

                    bool enabled = Boolean.Parse(printerEnabled.SettingValue);
                    _printerUtility = null;
                    if (!enabled) return;

                    var portSetting = printerRepo.GetByKey(GeneralSettingKey.FiscalPrinterPort);
                    if (portSetting != null && !string.IsNullOrEmpty(portSetting.SettingValue))
                    {
                        port = Convert.ToInt32(portSetting.SettingValue);
                    }
                    var portSpeedSetting = printerRepo.GetByKey(GeneralSettingKey.FiscalPrinterPortSpeed);
                    if (portSpeedSetting != null && !string.IsNullOrEmpty(portSpeedSetting.SettingValue))
                    {
                        portSpeed = Convert.ToInt32(portSpeedSetting.SettingValue);
                    }
                    _printerUtility = new FiscalPrinterUtility(port, portSpeed) { IsEnabled = enabled };
                }
            }

        }
        private void ViewInvoice(OrderItemSummary summary)
        {
            const string uri = "/views/invoicedocument/invoicedocument.xaml";
            Messenger.Default.Send<ViewModelMessage>(new ViewModelMessage {Id = summary.OrderId});
            NavigateCommand.Execute(uri);
        }

        private void ReceivePayment(OrderItemSummary summary)
        {
            if (summary.OutstandingAmount < 0)
            {
                MessageBox.Show( /*"Gross amount should be greater than zero"*/
                    GetLocalText("sl.pos.receivepayments.messagebox.grossamountiszero")
                    , "!" + GetLocalText("sl.payment.title") /*"Distributr: Payment Module"*/
                    , MessageBoxButton.OK);
                return;
            }
           
            Submitpayments(summary);

        }

        private void Submitpayments(OrderItemSummary orderSummary)
        {
            PaymentInfoList.Clear();
            try
            {
                using (var container = NestedContainer)
                {
                    Config config = Using<IConfigService>(container).Load();
                    var payInfo =
                        Using<IPaymentPopup>(container).GetPayments(orderSummary.OutstandingAmount,
                                                                    orderSummary.OrderId);
                    if (payInfo != null)
                    {
                        foreach (var paymentInfo in payInfo)
                        {
                            if (!PaymentInfoList.Contains(paymentInfo))
                                PaymentInfoList.Add(paymentInfo);
                        }
                    }
                    var order = Using<IMainOrderRepository>(container).GetById(orderSummary.OrderId);
                    var invoiceId = Using<IInvoiceRepository>(container).GetInvoiceByOrderId(order.Id).Id;
                    var orderWorkflow = Using<IOrderWorkflow>(container);
                    if (order != null)
                    {
                        order.ChangeccId(GetConfigParams().CostCentreApplicationId);
                        foreach (var paymentInfo in PaymentInfoList.Where(p => p.PaymentModeUsed != PaymentMode.Credit))
                        {
                            paymentInfo.InvoiceId = invoiceId;
                           order.AddOrderPaymentInfoLineItem(paymentInfo);
                        }
                        orderWorkflow.Submit(order,config);
                        ListOrders();
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CreateOrder()
        {
            var uri = "/views/orders/CreateOrder.xaml";
            NavigateCommand.Execute(uri);
        }

        private void ViewSelectedOrder(OrderItemSummary summary)
        {
            var uri = "/views/orders/vieworder.xaml";
            Messenger.Default.Send<ViewModelMessage>(new ViewModelMessage {Id = summary.OrderId});
            NavigateCommand.Execute(uri);


        }

        #region Mass approval
       private bool _approveAndDispatch = false;
        private void SelectAllUnApprovedOrders(CheckBox checkBox)
        {
            if(checkBox==null)return;
            SelectAllOrdersChecked = checkBox.IsChecked.HasValue && checkBox.IsChecked.Value;
          //  PutFocusOnControl();
            foreach (var order in OrdersSummaryList)
            {
                order.IsChecked = SelectAllOrdersChecked;

            }
           
        }
        private void ApproveAndDispatch(Button button)
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(() =>
                               {
                                   _approveAndDispatch = true;
                                   ConfigureFiscalPrinter();
                                   ProcessSelectedOrders(button); //1. approve selected orders,dispatch 
                                   RefReshSummaryListing();
                               }));

        }
        

        private void RefReshSummaryListing()
        {

            if (_approveAndDispatch)
                RefreshPendingDispatchTab();
            else
            {
                RefreshPendingApprovalTab();
            }
           // MessageBox.Show(string.Format("Processing completed"));
        }

        private List<Guid> GetSelectedOrders()
        {
           return (SelectAllOrdersChecked
                        ? PagedDocumentList.Select(n => n.OrderId)
                        : OrdersSummaryList.Where(p => p.IsChecked).Select(n => n.OrderId)).ToList();
        }

        void ShowBusy(bool isBusy)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Loaded,
                                                     (System.Threading.ThreadStart)
                                                     (() =>{
                                                         var busy =currentpage.FindName("BusyIndicator") as BusyIndicator;
                                                                if (busy != null)
                                                                {
                                                                    busy.BusyContent ="Processing Please wait...";
                                                                    busy.IsBusy = isBusy;
                                                                }
                                                     }));
        }
        private void UpdateControl(Button button,bool status)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Loaded,
                                                     (System.Threading.ThreadStart) (() =>
                                                                                         {
                                                                                             Button control;
                                                                                             control = currentpage.FindName(button.Name) as Button;
                                                                                             if (control != null && control.IsEnabled !=status)
                                                                                             {
                                                                                                 control.IsEnabled = status;

                                                                                             }
                                                                                            
                                                                                         }));
        }
       
        private void RemoveProcessed(Guid orderId)
        {
            Application.Current.Dispatcher
                .BeginInvoke(new Action(delegate
                            {
                                var approved = OrdersSummaryList.FirstOrDefault(p => p.OrderId == orderId);
                                if (approved == null) return;
                                OrdersSummaryList.Remove(approved);
                                var removed = PagedDocumentList.FirstOrDefault(p => p.OrderId == approved.OrderId);
                                if (removed != null)
                                    PagedDocumentList.Remove(removed);
                            }));
        }
        private async void ProcessSelectedOrders(Button button)
        {
            UpdateControl(button, false); 
            ShowBusy(true);
              Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                {
                                    if (SelectedAndApprovedOrderIds.Any())
                                        SelectedAndApprovedOrderIds.Clear();
                                    var selected = GetSelectedOrders();
                                    if (selected.Any())
                                    {
                                      
                                        ConfigureFiscalPrinter();
                                        try
                                        {
                                            ShowBusy(true);
                                           Parallel.ForEach(selected, orderId =>
                                                                           {
                                                                               if (orderId == Guid.Empty) return;
                                                                               MainOrder order =ObjectFactory.GetInstance<IMainOrderRepository>().GetById(orderId);
                                                                               var mainOrderworkflow =ObjectFactory.GetInstance<IOrderWorkflow>();
                                                                               Config config =ObjectFactory.GetInstance<IConfigService>().Load();
                                                                               Guid costCentreApplicationid =config.CostCentreApplicationId;
                                                                               order.ChangeccId(costCentreApplicationid);
                                                                               foreach (var line in order.PendingApprovalLineItems)
                                                                               {
                                                                                   order.ApproveLineItem(line);
                                                                               }
                                                                               order.Approve();
                                                                               mainOrderworkflow.Submit(order,config);
                                                                               SelectedAndApprovedOrderIds.Add(orderId);
                                                                               using (var c = NestedContainer)
                                                                               {
                                                                                   if (PrinterUtility != null &&PrinterUtility.IsEnabled)
                                                                                   {
                                                                                       var approvedOrder =Using<IMainOrderRepository>(c).GetById(orderId);
                                                                                       if (approvedOrder != null)
                                                                                           PrinterUtility.PrinterOrderReceipt(approvedOrder);

                                                                                   }
                                                                               }
                                                                               RemoveProcessed(orderId);
                                                                           });
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.Message,
                                                            "Mass Order Approval", MessageBoxButton.OK, MessageBoxImage.Error);
                                        }
                                        finally
                                        {
                                            UpdateControl(button, true);
                                            ShowBusy(false);
                                        }
                                        if (SelectedAndApprovedOrderIds.Any() && _approveAndDispatch)
                                            DispatchBatches(SelectedAndApprovedOrderIds);

                                    }
                                    else
                                    {
                                        MessageBox.Show("Select at least one order to to approve");
                                        UpdateControl(button, true);
                                    }
                                    if (SelectedAndApprovedOrderIds.Any())
                                    {
                                        MessageBox.Show(string.Format("Successfully approved {0} orders", SelectedAndApprovedOrderIds.Count));
                                        SelectedAndApprovedOrderIds.Clear();
                                    }

                }));
            

        }
        

       
        async void DispatchBatches(List<Guid> approvedOrdersBatch)
        {
            await Task.Run(() =>
                               {
                                   if (!approvedOrdersBatch.Any()) return;
                                   using (var c = NestedContainer)
                                   {
                                       var mainOrderRepository = Using<IMainOrderRepository>(c);
                                       var mainOrderworkflow = Using<IOrderWorkflow>(c);
                                       foreach (var orderId in approvedOrdersBatch)
                                       {
                                           MainOrder order = mainOrderRepository.GetById(orderId);
                                           Config config = Using<IConfigService>(c).Load();
                                           Guid costCentreApplicationid = config.CostCentreApplicationId;
                                           order.ChangeccId(costCentreApplicationid);
                                           order.DispatchPendingLineItems();
                                           mainOrderworkflow.Submit(order,config);
                                       }

                                   }
                               });
        }






        private void RefreshPendingDispatchTab()
        {
            
                
                SelectAllOrdersChecked = false;
                OrderApprovalViewModel.ShowMessage = true;
                _approveAndDispatch = false;
            SelectedOrderStatus = DocumentStatus.Approved;
            Load();

        }
        private void RefreshPendingApprovalTab()
        {

            MessageBox.Show(string.Format("Processing completed"));

            OrderApprovalViewModel.ShowMessage = true;
            
            Dispatcher.CurrentDispatcher.InvokeAsync((delegate
                                                          {
                                                              TabItem tabItem = new TabItem
                                                                                    {
                                                                                        Name = "PendingApprovalTab"
                                                                                    };
                                                              SelectAllOrdersChecked = false;
                                                              LoadSelectedTab(tabItem);
                                                             
                                                              PutFocusOnControl(tabItem);
                                                          }));
        }
        
        #endregion
        private void ProcessOrder(OrderItemSummary summary)
        {
            var uri = "/views/orders/OrderApproval.xaml";
            Messenger.Default.Send(summary.OrderId);
            NavigateCommand.Execute(uri);
        }

        private void ViewPrintableDispatchNote(OrderItemSummary item)
        {
            using (var c = NestedContainer)
            {
                Using<IPrintableDocumentViewer>(c).ViewDocument(item.OrderId, DocumentType.DispatchNote);
            }
        }

        private void ViewPrintableOrder(OrderItemSummary item)
        {
            using (var c = NestedContainer)
            {
                Using<IPrintableDocumentViewer>(c).ViewDocument(item.OrderId, DocumentType.Order);
            }
        }

        protected override void SetUp()
        {
            OrdersSummaryList.Clear();
            SelectedAndApprovedOrderIds = new List<Guid>();
            using (var container = NestedContainer)
            {


                var configService = Using<IConfigService>(container);
                var recordsPerPageSetting =
                    Using<IGeneralSettingRepository>(container).GetByKey(GeneralSettingKey.RecordsPerPage);
                var approveAndDispatch = Using<ISettingsRepository>(container).GetByKey(SettingsKeys.ApproveAndDispatch);
                ShowApproveAndDispatchButton = (approveAndDispatch != null && approveAndDispatch.Value== "1")
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                

                ItemsPerPage = recordsPerPageSetting != null ? Convert.ToInt32(recordsPerPageSetting.SettingValue) : 10;
                CanCreateOrders = configService.ViewModelParameters.CurrentUserRights.CanCreateOrder;
                CanDispatchOrders = configService.ViewModelParameters.CurrentUserRights.CanDispatchOrder;
                CanReceivePayments = configService.ViewModelParameters.CurrentUserRights.CanCreateOrder;
                CanApproveOrder = configService.ViewModelParameters.CurrentUserRights.CanApproveOrders;
            }
            SelectedOrderStatus = DocumentStatus.Confirmed;
            SelectedOrderType = OrderType.OutletToDistributor;
           

        }

        

        protected override void ClearSearchText()
        {
            base.ClearSearchText();
            Load();
        }

        protected override void TabSelectionChanged(SelectionChangedEventArgs e)
        {
            CurrentPage = 1;
            Dispatcher.CurrentDispatcher.InvokeAsync((delegate
                                                          {
                                                              if (e.Source.GetType() != typeof (TabControl))
                                                                  return;

                                                              TabItem tabItem = e.AddedItems[0] as TabItem;
                                                              ResetDates();
                                                              LoadSelectedTab(tabItem);
                                                              e.Handled = true;

                                                          }));
        }

        
        protected override void LoadSelectedTab(TabItem selectedTab)
        {
            isUnconfirmedTab = false;
            if (SalesmenList.Any())
                SalesmenList.Clear();
                
            switch (selectedTab.Name)
            {
                case "PendingConfirmationTab":
                    SelectedOrderStatus = DocumentStatus.New;
                    ShowApproveSelectedButton = Visibility.Collapsed;
                    SelectAllOrdersChecked = false;
                    isUnconfirmedTab = true;
                    break;
                case "PendingApprovalTab":
                    SelectedOrderStatus = DocumentStatus.Confirmed;
                    ShowApproveSelectedButton=Visibility.Visible;
                    SelectAllOrdersChecked = false;
                    break;
                case "PendingDispatchTab":
                    SelectedOrderStatus = DocumentStatus.Approved;
                    break;
                case "DispatchedTab":
                    SelectedOrderStatus = DocumentStatus.Dispatched;
                    break;
                case "IncompleteTab":
                    SelectedOrderStatus = DocumentStatus.New;
                    break;
                case "DeliveredTab":
                    SelectedOrderStatus = DocumentStatus.Closed;
                    break;
                case "OutstandingPaymentsTab":
                    SelectedOrderStatus = DocumentStatus.Outstanding;
                    break;
                case "FullyPaidDeliveriesTab":

                    SelectedOrderStatus = DocumentStatus.FullyPaidOrders;
                    break;
                case "BackOrdersTab":
                    SelectedOrderStatus = DocumentStatus.OrderBackOrder;
                    break;
                case "LostSalesTab":
                    SelectedOrderStatus = DocumentStatus.OrderLossSale;

                    break;
                case "RejectedOrdersTab":
                    SelectedOrderStatus = DocumentStatus.Rejected;
                    break;
                case "ApprovedOrders":
                    ShowApproveSelectedButton = Visibility.Collapsed;
                    Initializesalemen();
                    break;
               

            }
            Load();
        }
        void ResetDates()
        {
            DateTime today = DateTime.Today;
            StartDate = new DateTime(today.Year, today.Month, 1);
            
            EndDate = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
           
        }

        void Initializesalemen()
        {
           if(SalesmenList.Any())
                SalesmenList.Clear();
            using (var c=NestedContainer)
            {
                SalesmenList.Add(new DistributorSalesman(Guid.Empty) { Name = "--All--" });
                var salesmen = Using<ICostCentreRepository>(c).GetAll().OfType<DistributorSalesman>();
                salesmen.OrderByDescending(s => s.Name).ToList().ForEach(f => SalesmenList.Add(f));
                SelectedSalesman = SalesmenList.FirstOrDefault();
            }
        }
      

        #endregion

       
    }
}
