using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.CreditNoteRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.PaymentNoteRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service;
using Distributr.WPF.Lib.Services.Service.Payment;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Transactional.Order_Pos;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Payments
{
    public class ListOutStandingPaymentsViewModel : OrdersListingBaseViewModel
    {
        private bool _isInitialized;
        public ListOutStandingPaymentsViewModel()
        {
            OutstandingPaymentList=new ObservableCollection<OutstandingPaymentItem>();
            UnConfirmedPaymentList=new ObservableCollection<UnconfirmedPaymentItem>();
            
            ReceivePaymentCommand= new RelayCommand<OutstandingPaymentItem>(ReceivePayment);
            PageLoadedCommand= new RelayCommand(PageLoaded);
            LoadForSelectedDatesCommand= new RelayCommand(Load);
            ViewSelectedInvoiceCommand = new RelayCommand<Guid>(ViewSelectedInvoice);
            ConfirmPaymentCommand =new RelayCommand<UnconfirmedPaymentItem>(ConfirmPayment);
            PaymentDetailsCommand=new RelayCommand<UnconfirmedPaymentItem>(ViewPaymentDetails);
          
            PaymentInfoList =new ObservableCollection<PaymentInfo>();
            SetupCommand=new RelayCommand(SetUp);
            _isInitialized = true;

        }

        public ObservableCollection<OutstandingPaymentItem> OutstandingPaymentList { get; set; }
        public ObservableCollection<UnconfirmedPaymentItem> UnConfirmedPaymentList { get; set; } 
        public RelayCommand<OutstandingPaymentItem> ReceivePaymentCommand { get; set; }
        public RelayCommand PageLoadedCommand { get; set; }
        public RelayCommand LoadForSelectedDatesCommand { get; set; }
        public RelayCommand<Guid> ViewSelectedInvoiceCommand { get; set; }
        public RelayCommand<UnconfirmedPaymentItem> ConfirmPaymentCommand { get; set; }
        public RelayCommand<UnconfirmedPaymentItem> PaymentDetailsCommand { get; set; }
        
       

      
        private void PageLoaded()
        {
            SetUp();
            Load();

        }
        protected  override void SetUp()
        {
            if (!_isInitialized) return;
            OutstandingPaymentList.Clear();
            PaymentInfoList.Clear();
            DocumentStatus = DocumentStatus.Outstanding;
            using (StructureMap.IContainer c = NestedContainer)
            {

                var recordsPerPageSetting = Using<IGeneralSettingRepository>(c).GetByKey(GeneralSettingKey.RecordsPerPage);
                ItemsPerPage = recordsPerPageSetting != null ? Convert.ToInt32(recordsPerPageSetting.SettingValue) : 10;
                CurrentPage = 1;
                SearchText = "";
                StartDate = DateTime.Now.AddDays(-2);
                EndDate = DateTime.Now;
            }
        }

   
        protected override void ClearSearchText()
        {
            base.ClearSearchText();
            Load();
           
           
        }


        private void Load()
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(delegate
                               {
                                   using (var container = NestedContainer)
                                   {
                                       var orders = Using<IMainOrderRepository>(container)
                                           .PagedDocumentList(CurrentPage,ItemsPerPage,StartDate, EndDate, 0, DocumentStatus, SearchText);

                                       OrdersSummaryList.Clear();
                                       if (orders != null && orders.Any())
                                       {
                                           PagedDocumentList = new PagenatedList<MainOrderSummary>(
                                               orders.AsQueryable(), CurrentPage,
                                               ItemsPerPage, orders.TotalItemCount,true);

                                           UpdatePagenationControl();

                                           if (DocumentStatus == DocumentStatus.Outstanding)
                                               MapOutstandingPayments(PagedDocumentList);
                                           else
                                           {
                                               MapUnConfirmedPayments(PagedDocumentList);
                                           }

                                       }

                                   }
                               }));
        }

        protected override async void ListOrders()
        {
            await Task.Factory.StartNew(() =>
            {
                Application.Current.Dispatcher.BeginInvoke(
                    new Action(delegate
                    {
                        using (var container = NestedContainer)
                        {
                            var orders = Using<IMainOrderRepository>(container)
                                .PagedDocumentList(CurrentPage, ItemsPerPage, StartDate, EndDate, 0, DocumentStatus,
                                    SearchText);

                            OrdersSummaryList.Clear();
                            if (orders != null && orders.Any())
                            {
                                PagedDocumentList = new PagenatedList<MainOrderSummary>(
                                    orders.AsQueryable(), CurrentPage,
                                    ItemsPerPage, orders.TotalItemCount, true);

                                UpdatePagenationControl();

                                if (DocumentStatus == DocumentStatus.Outstanding)
                                    MapOutstandingPayments(PagedDocumentList);
                                else
                                {
                                    MapUnConfirmedPayments(PagedDocumentList);
                                }

                            }

                        }
                    }));
            });

        }
        private void ViewSelectedInvoice(Guid orderId)
        {

            const string uri = "/views/invoicedocument/invoicedocument.xaml";
            Messenger.Default.Send<ViewModelMessage>(new ViewModelMessage { Id = orderId });
            NavigateCommand.Execute(uri);

        }
       

        private void MapOutstandingPayments(IEnumerable<MainOrderSummary> orders)
        {
            using (var c = NestedContainer)
            {
            if (OutstandingPaymentList.Any())
                OutstandingPaymentList.Clear();
            int i = 0;
            foreach (var order in orders)
            {
               var invoice= Using<IInvoiceRepository>(c).GetInvoiceByOrderId(order.OrderId);
                    
                    OutstandingPaymentList.Add(new OutstandingPaymentItem()
                                                   {
                                                       SequenceId = i + ((((CurrentPage - 1) * ItemsPerPage) + 1)),
                                                       OrderId = order.OrderId,
                                                       OrderDocRef = order.OrderReference,
                                                       TotalGross = order.GrossAmount,
                                                       TotalNet = order.NetAmount,
                                                       AmountDue =FormatOutstandingAmount(order.OutstandingAmount),
                                                       TotalVat = order.TotalVat,
                                                       TotalPaid = order.PaidAmount,
                                                       InvoiceOrderId =invoice==null?Guid.Empty:invoice.Id,
                                                       InvoiceDocumentRef=invoice==null?"":invoice.DocumentReference,
                                                           
                                                   });
                    i++;
                }
            }
        }
        
        protected override void TabSelectionChanged(SelectionChangedEventArgs eventArgs)
        {
            CurrentPage = 1;
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(delegate
            {
                if (eventArgs.Source.GetType() != typeof(TabControl))
                    return;

                TabItem tabItem = eventArgs.AddedItems[0] as TabItem;
                LoadSelectedTab(tabItem);
                eventArgs.Handled = true;

            }));
        }

        protected override void LoadSelectedTab(TabItem selectedTab)
        {
            if (selectedTab.Name.Equals("tiInvoiceList"))
            {
                DocumentStatus = DocumentStatus.Outstanding;
                
            }
            else if (selectedTab.Name.Equals("tiUnconfirmedReceiptPayments"))
            {
                DocumentStatus=DocumentStatus.UnconfirmedReceiptPayment;
            
            }
            Load();
        }

    
        private void MapUnConfirmedPayments(IEnumerable<MainOrderSummary> orders)
        {
           using (var c = NestedContainer)
            {
                if (!orders.Any()) return;
                UnConfirmedPaymentList.Clear();
                 
               foreach (var ordersummary in orders)
                    {
                        var order = Using<IMainOrderRepository>(c).GetById(ordersummary.OrderId);
                        if(order.GetPayments.Any(p=>!p.IsConfirmed))
                        {
                            var payments = order.GetPayments.Where(p=>!p.IsConfirmed).Select((info, i) =>
                                                                    new UnconfirmedPaymentItem()
                                                                        {
                                                                            SequenceId = i + 1,
                                                                            OrderId = order.Id,
                                                                            OrderDocRef = order.DocumentReference,
                                                                            UnconfirmedAmount =(info.Amount- info.ConfirmedAmount),
                                                                            PaymentModeUsed = info.PaymentModeUsed.ToString(),
                                                                            LineItemId = info.Id,
                                                                            Description =info.Description,
                                                                           
                                                                        }).ToList();
                            payments.ForEach(UnConfirmedPaymentList.Add);
                        }
                    }
            }
        }
    

        private void ReceivePayment(OutstandingPaymentItem summary)
        {
            if(summary.AmountDue.StartsWith("("))
            {
                MessageBox.Show("This item has been overpaid");
                return;
            }
            if (decimal.Parse(summary.AmountDue) <= 0)
            {
                MessageBox.Show( /*"Gross amount should be greater than zero"*/
                GetLocalText("sl.pos.receivepayments.messagebox.grossamountiszero")
               , "!" + GetLocalText("sl.payment.title") /*"Distributr: Payment Module"*/
               , MessageBoxButton.OK);
                return;
            }
            Submitpayments(summary);

        }
       

        protected void Submitpayments(OutstandingPaymentItem orderSummary)
        {
            PaymentInfoList.Clear();
            try
            {
                using (var container = NestedContainer)
                {
                    var payInfo = Using<IPaymentPopup>(container).GetPayments(decimal.Parse(orderSummary.AmountDue), orderSummary.OrderId);
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
                    var posWorkflow = Using<IOrderPosWorkflow>(container);
                    Config config = Using<IConfigService>(container).Load();
                    if (order != null)
                    {
                        order.ChangeccId(GetConfigParams().CostCentreApplicationId);
                        foreach (var paymentInfo in PaymentInfoList.Where(p => p.PaymentModeUsed != PaymentMode.Credit))
                        {
                            paymentInfo.InvoiceId = invoiceId;
                            order.AddOrderPaymentInfoLineItem(paymentInfo);
                        }
                        if (order.DocumentReference.ToLower().StartsWith("sale"))
                             posWorkflow.Submit(order,config);
                            
                        else
                        {
                            orderWorkflow.Submit(order,config);
                        }
                        Load();
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void ViewPaymentDetails(UnconfirmedPaymentItem item)
        {
            using (var c =NestedContainer)
            {
                var payment = Using<IMainOrderRepository>(c).GetById(item.OrderId).GetPayments.FirstOrDefault(p => p.Id == item.LineItemId);
                
                if(payment !=null)
                    Using<IPaymentUtils>(c).GetPaymentDetails(payment.PaymentRefId, payment.PaymentModeUsed.ToString());             
                
            }
        }

        private async void ConfirmPayment(UnconfirmedPaymentItem paymentInfoItem)
        {
            using (var c = NestedContainer)
            {
                var order = Using<IMainOrderRepository>(c).GetById(paymentInfoItem.OrderId);
                var paymentbridgeService = Using<IPaymentGateWayBridge>(c);

                if (order != null && order.GetPayments.Count>0)
                {
                    var payments=order.GetPayments;
                    if (payments.Any(p => p.Id == paymentInfoItem.LineItemId && p.IsConfirmed))
                    {
                        MessageBox.Show("This payment has already been confirmed.", "Distributr: Payment Module",
                                        MessageBoxButton.OK);
                        return;
                    }
                    try
                    {
                        var paymentInfo = payments.FirstOrDefault(p => p.Id == paymentInfoItem.LineItemId);
                        var responce = await paymentbridgeService.GetNotification(paymentInfo);
                        if (responce !=null  && responce.PaymentNotificationDetails.Any(s=>!s.IsUsed) )
                        {
                            foreach (var notItem in responce.PaymentNotificationDetails)
                            {
                                var payment = new PaymentInfo()
                                {
                                    Id = new Guid(responce.TransactionRefId),
                                    Amount = (decimal)notItem.PaidAmount,
                                    ConfirmedAmount = (decimal) notItem.PaidAmount,
                                    PaymentRefId = responce.SDPReferenceId,
                                    MMoneyPaymentType = paymentInfo.MMoneyPaymentType,
                                    NotificationId = notItem.Id.ToString(),
                                    PaymentModeUsed = paymentInfo.PaymentModeUsed,
                                    IsConfirmed = true,
                                    IsProcessed = false

                                };
                                paymentbridgeService.ConfirmNotification(notItem.Id);
                                order.AddOrderPaymentInfoLineItem(payment);
                                    
                            }
                            Config config = Using<IConfigService>(c).Load();
                            if (order.OrderType==OrderType.DistributorPOS)
                                Using<IOrderPosWorkflow>(c).Submit(order,config);

                            else
                            {
                                
                 
                                Using<IOrderWorkflow>(c).Submit(order,config);
                            }
                            Load();

                            MessageBox.Show("This payment confirmed.", "Distributr: Payment Module",
                                       MessageBoxButton.OK);
                        }
                        //if(Using<IPaymentUtils>(c).ConfirmPayment(paymentInfo))
                        //{
                        //    order.AddOrderPaymentInfoLineItem(paymentInfo);

                        //    if (order.DocumentReference.ToLower().StartsWith("sale"))
                        //        Using<IOrderPosWorkflow>(c).Submit(order);

                        //    else
                        //    {
                        //        Using<IOrderWorkflow>(c).Submit(order);
                        //    }
                        //    Load();

                        //    MessageBox.Show("This payment confirmed.", "Distributr: Payment Module",
                        //               MessageBoxButton.OK);
                     
                        //}

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
               
            }
        }

       
        public const string DocumentStatusPropertyName = "DocumentStatus";
        private DocumentStatus _documentStatus=DocumentStatus.Outstanding;
        public DocumentStatus DocumentStatus
        {
            get
            {
                return _documentStatus;
            }

            set
            {
                if (_documentStatus == value)
                {
                    return;
                }

                _documentStatus = value;
                // Update bindings, no broadcast
               RaisePropertyChanged(DocumentStatusPropertyName);
            }
        }
      
    }

    public class OutstandingPaymentItem : ViewModelBase
    {
        public int SequenceId { get; set; }
        public const string OrderIdPropertyName = "OrderId";
        private Guid _orderId = Guid.Empty;
        public Guid OrderId
        {
            get
            {
                return _orderId;
            }

            set
            {
                if (_orderId == value)
                {
                    return;
                }

                var oldValue = _orderId;
                _orderId = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(OrderIdPropertyName);
            }
        }

        public const string InvoiceDocumentRefPropertyName = "InvoiceDocumentRef";
        private string _documentRef = null;
        public string InvoiceDocumentRef
        {
            get
            {
                return _documentRef;
            }

            set
            {
                if (_documentRef == value)
                {
                    return;
                }

                var oldValue = _documentRef;
                _documentRef = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(InvoiceDocumentRefPropertyName);
            }
        }

        public const string TotalGrossPropertyName = "TotalGross";
        private decimal _totalGross;
        public decimal TotalGross
        {
            get
            {
                return _totalGross;
            }

            set
            {
                if (_totalGross == value)
                {
                    return;
                }

                var oldValue = _totalGross;
                _totalGross = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(TotalGrossPropertyName);
            }
        }

        public const string TotalNetPropertyName = "TotalNet";
        private decimal _totalNet;
        public decimal TotalNet
        {
            get
            {
                return _totalNet;
            }

            set
            {
                if (_totalNet == value)
                {
                    return;
                }

                var oldValue = _totalNet;
                _totalNet = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(TotalNetPropertyName);
            }
        }

        public const string TotalVatPropertyName = "TotalVat";
        private decimal _totalVat;
        public decimal TotalVat
        {
            get
            {
                return _totalVat;
            }

            set
            {
                if (_totalVat == value)
                {
                    return;
                }

                var oldValue = _totalVat;
                _totalVat = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(TotalVatPropertyName);
            }
        }

        public const string TotalPaidPropertyName = "TotalPaid";
        private decimal _totalPaid;
        public decimal TotalPaid
        {
            get
            {
                return _totalPaid;
            }

            set
            {
                if (_totalPaid == value)
                {
                    return;
                }

                var oldValue = _totalPaid;
                _totalPaid = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(TotalPaidPropertyName);
            }
        }

        public const string InvoiceDatePropertyName = "InvoiceDate";
        private string _invoiceDate;
        public string InvoiceDate
        {
            get
            {
                return _invoiceDate;
            }

            set
            {
                if (_invoiceDate == value)
                {
                    return;
                }

                var oldValue = _invoiceDate;
                _invoiceDate = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(InvoiceDatePropertyName);

                // Update bindings and broadcast change using GalaSoft.MvvmLight.Messenging
                RaisePropertyChanged(InvoiceDatePropertyName, oldValue, value, true);
            }
        }

        public const string InvoiceOrderIdPropertyName = "InvoiceOrderId";
        private Guid _InvoiceOrderId;
        public Guid InvoiceOrderId
        {
            get
            {
                return _InvoiceOrderId;
            }

            set
            {
                if (_InvoiceOrderId == value)
                {
                    return;
                }

                var oldValue = _InvoiceOrderId;
                _InvoiceOrderId = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(InvoiceOrderIdPropertyName);

                // Update bindings and broadcast change using GalaSoft.MvvmLight.Messenging
                RaisePropertyChanged(InvoiceOrderIdPropertyName, oldValue, value, true);
            }
        }

        public const string OrderDocRefPropertyName = "OrderDocRef";
        private string _orderDocRef = "";
        public string OrderDocRef
        {
            get
            {
                return _orderDocRef;
            }

            set
            {
                if (_orderDocRef == value)
                {
                    return;
                }

                var oldValue = _orderDocRef;
                _orderDocRef = value;
                RaisePropertyChanged(OrderDocRefPropertyName);
            }
        }

        public const string AmountDuePropertyName = "AmountDue";
        private string _amountDue ="0";
        public string AmountDue
        {
            get
            {
                return _amountDue;
            }

            set
            {
                if (_amountDue == value)
                {
                    return;
                }

                var oldValue = _amountDue;
                _amountDue = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(AmountDuePropertyName);


            }
        }
       
        public bool CanIssueCreditNote { get; set; }



    }
    public class UnconfirmedPaymentItem
    {
        public int SequenceId { get; set; }
        public Guid LineItemId { get; set; }
        public decimal UnconfirmedAmount { get; set; }
        public string InvoiceDocReference { get; set; }
        public Guid InvoiceId { get; set; }
        public string PaymentModeUsed { get; set; }
        public Guid OrderId { get; set; }
        public string OrderDocRef { get; set; }
        public string Description { get; set; }
    }

}