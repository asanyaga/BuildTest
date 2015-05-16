using System;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Threading;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.CreditNoteRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using System.Linq;
using Distributr.Core.ClientApp;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Transactional.CN
{
    public class ListInvoicesViewModel : DistributrViewModelBase
    {
        public ObservableCollection<ListInvoiceItemsViewModel> InvoicesList { get; set; }
        public List<ListInvoiceItemsViewModel> InvoicesListCached { get; set; }
        public List<Receipt> ReceiptsOfUnpaidInvoices { get; set; }
        public ObservableCollection<PaymentInfo> PaymentInfoList { get; set; }
        public ObservableCollection<UnconfirmedReceiptPayment> UnconfirmedReceiptPayments { get; set; }
        public RelayCommand LoadInvoices { get; set; }
        public RelayCommand LoadUpaidInvoicesCommand { get; set; }
        public RelayCommand LoadGetInvoiceAmountsCommand { get; set; }
        public RelayCommand LoadUnpaidInvoicesbySearchTextCommand { get; set; }
        public RelayCommand<ListInvoiceItemsViewModel> ViewSelectedInvoiceCommand { get; set; }
        public RelayCommand PageLoadedCommand { get; set; }
        public RelayCommand LoadForSelectedDatesCommand { get; set; }
        public RelayCommand ClearSearchtextBoxCommand { get; set; }
        public RelayCommand<SelectionChangedEventArgs> TabSelectionChangedCommand { get; set; }
        public RelayCommand<ListInvoiceItemsViewModel> ReceivePaymentsCommand { get; set; }
        public RelayCommand<UnconfirmedReceiptPayment> ViewReceiptCommand { get; set; }
        private List<Order> _orders;
        

        public ListInvoicesViewModel()
        {
            LoadInvoices = new RelayCommand(RunLoadInvoices);
            LoadUpaidInvoicesCommand = new RelayCommand(RunLoadUpaidInvoices);
            LoadGetInvoiceAmountsCommand = new RelayCommand(GetInvoiceAmounts);
            ViewSelectedInvoiceCommand = new RelayCommand<ListInvoiceItemsViewModel>(ViewSelectedInvoice);
            PageLoadedCommand = new RelayCommand(PageLoaded);
            LoadForSelectedDatesCommand= new RelayCommand(RunLoadInvoices);
            ClearSearchtextBoxCommand= new RelayCommand(ClearSearchBox);
            TabSelectionChangedCommand= new RelayCommand<SelectionChangedEventArgs>(TabSelectionChanged);
            ReceivePaymentsCommand = new RelayCommand<ListInvoiceItemsViewModel>(ReceivePayments);
            ViewReceiptCommand = new RelayCommand<UnconfirmedReceiptPayment>(ViewReceipt);

            InvoicesList = new ObservableCollection<ListInvoiceItemsViewModel>();
            InvoicesListCached = new List<ListInvoiceItemsViewModel>();
            UnconfirmedReceiptPayments = new ObservableCollection<UnconfirmedReceiptPayment>();
            UnconfirmedReceiptPaymentsCache = new List<UnconfirmedReceiptPayment>();
            PaymentInfoList = new ObservableCollection<PaymentInfo>();
            ReceiptsOfUnpaidInvoices = new List<Receipt>();
            _orders = new List<Order>();
            using (StructureMap.IContainer c = NestedContainer)
            {
                CanReceivePayments = Using<IConfigService>(c).ViewModelParameters.CurrentUserRights.CanReceivePayments;
            }
        }

       

      
        #region Properties

        public const string OrderIdPropertyName = "OrderId";
        private Guid _OrderId = Guid.Empty;
        public Guid OrderId
        {
            get
            {
                return _OrderId;
            }

            set
            {
                if (_OrderId == value)
                {
                    return;
                }

                var oldValue = _OrderId;
                _OrderId = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(OrderIdPropertyName);
            }
        }

        public const string InvoiceNoPropertyName = "InvoiceNo";
        private string _InvoiceNo = null;
        public string InvoiceNo
        {
            get
            {
                return _InvoiceNo;
            }

            set
            {
                if (_InvoiceNo == value)
                {
                    return;
                }

                var oldValue = _InvoiceNo;
                _InvoiceNo = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(InvoiceNoPropertyName);
            }
        }

        public const string TotalPaidPropertyName = "TotalPaid";
        private decimal _TotalPaid = 0;
        public decimal TotalPaid
        {
            get
            {
                return _TotalPaid;
            }

            set
            {
                if (_TotalPaid == value)
                {
                    return;
                }

                var oldValue = _TotalPaid;
                _TotalPaid = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(TotalPaidPropertyName);
            }
        }


        public const string GrossTotalPropertyName = "GrossTotal";
        private decimal _GrossTotal = 0;
        public decimal GrossTotal
        {
            get
            {
                return _GrossTotal;
            }

            set
            {
                if (_GrossTotal == value)
                {
                    return;
                }

                var oldValue = _GrossTotal;
                _GrossTotal = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(GrossTotalPropertyName);
            }
        }

        public const string InvoiceRefPropertyName = "InvoiceRef";
        private string _InvoiceRef = null;
        public string InvoiceRef
        {
            get
            {
                return _InvoiceRef;
            }

            set
            {
                if (_InvoiceRef == value)
                {
                    return;
                }

                var oldValue = _InvoiceRef;
                _InvoiceRef = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(InvoiceRefPropertyName);
            }
        }

        public const string SearchTextPropertyName = "SearchText";
        private string _searchText = "";
        public string SearchText
        {
            get
            {
                return _searchText;
            }

            set
            {
                if (_searchText == value)
                {
                    return;
                }

                var oldValue = _searchText;
                _searchText = value;
                RaisePropertyChanged(SearchTextPropertyName);
            }
        }

        public const string PageProgressBarPropertyName = "PageProgressBar";
        private string _pageProgressBar = "";
        public string PageProgressBar
        {
            get
            {
                return _pageProgressBar;
            }

            set
            {
                if (_pageProgressBar == value)
                {
                    return;
                }

                var oldValue = _pageProgressBar;
                _pageProgressBar = value;
                RaisePropertyChanged(PageProgressBarPropertyName);
            }
        }

        public const string CanReceivePaymentsPropertyName = "CanReceivePayments";
        private bool _canReceivePayments = false;
        public bool CanReceivePayments
        {
            get
            {
                return _canReceivePayments;
            }

            set
            {
                if (_canReceivePayments == value)
                {
                    return;
                }

                var oldValue = _canReceivePayments;
                _canReceivePayments = value;
                RaisePropertyChanged(CanReceivePaymentsPropertyName);
            }
        }
         
        public const string CurrentPagePropertyName = "CurrentPage";
        private int _currentPage = 1;
        public int CurrentPage
        {
            get
            {
                return _currentPage;
            }

            set
            {
                if (_currentPage == value)
                {
                    return;
                }

                var oldValue = _currentPage;
                _currentPage = value;
                RaisePropertyChanged(CurrentPagePropertyName);
            }
        }
         
        public const string PageCountPropertyName = "PageCount";
        private int _pageCount = 1;
        public int PageCount
        {
            get
            {
                return _pageCount;
            }

            set
            {
                if (_pageCount == value)
                {
                    return;
                }

                var oldValue = _pageCount;
                if (value == 0)
                    value = 1;
                _pageCount = value;
                RaisePropertyChanged(PageCountPropertyName);
            }
        }
         
        public const string RecordsCountPropertyName = "RecordsCount";
        private int _recordsCount = 0;
        public int RecordsCount
        {
            get
            {
                return _recordsCount;
            }

            set
            {
                if (_recordsCount == value)
                {
                    return;
                }

                var oldValue = _recordsCount;
                _recordsCount = value;
                RaisePropertyChanged(RecordsCountPropertyName);
            }
        }
         
        public const string ItemsPerPagePropertyName = "ItemsPerPage";
        private int _itemsPerPage = 10;
        public int ItemsPerPage
        {
            get
            {
                return _itemsPerPage;
            }

            set
            {
                if (_itemsPerPage == value)
                {
                    return;
                }

                var oldValue = _itemsPerPage;
                _itemsPerPage = value;
                RaisePropertyChanged(ItemsPerPagePropertyName);
            }
        }
         
        public const string StartDatePropertyName = "StartDate";
        private DateTime _startDare = DateTime.Now.AddDays(-2);
        public DateTime StartDate
        {
            get
            {
                return _startDare;
            }

            set
            {
                if (_startDare == value)
                {
                    return;
                }

                var oldValue = _startDare;
                _startDare = value;
                RaisePropertyChanged(StartDatePropertyName);
            }
        }

        public const string EndDatePropertyName = "EndDate";
        private DateTime _endDate = DateTime.Now;
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }

            set
            {
                if (_endDate == value)
                {
                    return;
                }

                var oldValue = _endDate;
                _endDate = value;
                RaisePropertyChanged(EndDatePropertyName);
            }
        }
        #endregion
        private void PageLoaded()
        {
           ClearAndSetup();
           RunLoadInvoices();
        }
        private void ClearSearchBox()
        {
            SearchText = string.Empty;
        }
       void ClearAndSetup()
        {
            PaymentInfoList.Clear();
            InvoicesList.Clear();
            InvoicesListCached.Clear();
            UnconfirmedReceiptPayments.Clear();
            ReceiptsOfUnpaidInvoices.Clear();
           UnconfirmedReceiptPaymentsCache .Clear();
            using (StructureMap.IContainer c = NestedContainer)
            {
                 
                GeneralSetting recordsPerPageSetting = Using<IGeneralSettingRepository>(c).GetByKey(GeneralSettingKey.RecordsPerPage);
                ItemsPerPage = recordsPerPageSetting != null ? Convert.ToInt32(recordsPerPageSetting.SettingValue) : 10;
                CurrentPage = 1;
                SearchText = "";
                StartDate = DateTime.Now.AddDays(-2);
                _endDate = DateTime.Now;
            }
          

        }
        private void TabSelectionChanged(SelectionChangedEventArgs e)
        {
          Dispatcher.CurrentDispatcher.BeginInvoke(new Action(delegate
            {
                if (e.Source.GetType() != typeof(TabControl))
                    return;

                TabItem tabItem = e.AddedItems[0] as TabItem;
                LoadSelectedTab(tabItem);
                e.Handled = true;

            }));

        }
        void LoadSelectedTab(TabItem selectedTab)
        {
            if (selectedTab.Name.Equals("tiInvoiceList"))
            {
                RunLoadInvoices();
            }
            else if (selectedTab.Name.Equals("tiUnconfirmedReceiptPayments"))
            {
                RunLoadUpaidInvoices();
            }
        }

        private void ViewReceipt(UnconfirmedReceiptPayment obj)
        {
            const string uri = "/views/receiptdocuments/receiptdocument.xaml";
            Messenger.Default.Send<ViewModelMessage>(new ViewModelMessage { Id = obj.ReceiptId });
            NavigateCommand.Execute(uri);
        }

        private void ViewSelectedInvoice(ListInvoiceItemsViewModel selectedItem)
        {
            const string uri = "/views/invoicedocument/invoicedocument.xaml";
            Messenger.Default.Send<ViewModelMessage>(new ViewModelMessage { Id = selectedItem.InvoiceOrderId });
            NavigateCommand.Execute(uri);
        }
        private void ReceivePayments(ListInvoiceItemsViewModel invoiceItem)
        {
            if (!CanReceivePayments)
            {
                MessageBox.Show("Sorry, you do not have sufficient rights to perform this action.", "Distributr: Payments Module",
                                 MessageBoxButton.OK);
                return;
            }
            else
            {
                if (invoiceItem.HasUnconfirmedPayment)
                {
                    MessageBox.Show("NOTE: \nThere are unconfirmed payments for this invoice.", "Distributr: Payments Module",
                                MessageBoxButton.OK);
                }
                Submitpayments(invoiceItem);
            }
        }

        void Submitpayments(ListInvoiceItemsViewModel invoiceItem)
        {
            PaymentInfoList.Clear();
            try
            {
                using (var container = NestedContainer)
                {
                    BasicConfig config = container.GetInstance<IConfigService>().Load();
                    var payInfo = Using<IPaymentPopup>(container).GetPayments(invoiceItem.AmountDue, invoiceItem.InvoiceOrderId);
                    foreach (var paymentInfo in payInfo)
                    {
                        if (!PaymentInfoList.Contains(paymentInfo))
                            PaymentInfoList.Add(paymentInfo);
                    }
                    var order = Using<IMainOrderRepository>(container).GetById(invoiceItem.InvoiceOrderId);
                    var posWorkflow = Using<IOrderPosWorkflow>(container);
                    if (order != null)
                    {
                        foreach (var paymentInfo in PaymentInfoList.Where(p => p.PaymentModeUsed != PaymentMode.Credit))
                        {
                            paymentInfo.InvoiceId = invoiceItem.Id;
                            order.AddPayment(paymentInfo);
                        }
                        posWorkflow.Submit(order,config);
                        RunLoadInvoices();
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       void Load(int currentPage, int itemsPerPage, DateTime startDate, DateTime endDate, OrderType orderType, DocumentStatus orderStatus, string searchText)
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(delegate
                {
                   using (var container = NestedContainer)
                    {
                        var orders = Using<IMainOrderRepository>(container)
                            .PagedDocumentList(currentPage, itemsPerPage, startDate, endDate,
                              orderType, orderStatus, searchText);
                       MapOutstandingPayments(orders.ToList());
                        
                    }
                }));
        }

        private void MapOutstandingPayments(List<MainOrderSummary> orders)
        {
            if (InvoicesList.Any())
                InvoicesList.Clear();
            foreach (var order in orders)
            {
                InvoicesList.Add(new ListInvoiceItemsViewModel()
                                     {
                                         Id = order.OrderId,
                                         DocumentRef = order.OrderReference,
                                         TotalGross = order.GrossAmount,
                                         TotalNet = order.NetAmount,
                                         AmountDue = order.OutstandingAmount,
                                         TotalVat = order.TotalVat,
                                         TotalPaid = order.PaidAmount,
                                         OrderDocRef = order.OrderReference,
                                         
                                     });
            }
        }

        void RunLoadInvoices()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                List<ListInvoiceItemsViewModel> listItems = new List<ListInvoiceItemsViewModel>();

                if (CurrentPage == 1 && SearchText.Trim() == "")
                {
                    List<Invoice> invoices = Using<IInvoiceRepository>(c).GetAll(StartDate, EndDate).OfType<Invoice>().ToList();
                    InvoicesListCached.Clear();
                    int i = 0;
                    InvoicesList.Clear();
                    invoices.OrderByDescending(n => n.DocumentDateIssued).ToList()
                            .ForEach(n =>
                                {
                                    //var item = 
                                    List<CreditNote> cns = Using<ICreditNoteRepository>(c).GetCreditNotesByInvoiceId(n.Id).ToList(); //cn:
                                    //if cn totals is >= invoice total gross ? cannot issue another credit note
                                    //                                         if(cns.Sum(cn => cn.Total) >= n.TotalGross)
                                    decimal cnsTotal = cns.Sum(cn => cn.Total);

                                    InvoicesListCached.Add(new ListInvoiceItemsViewModel
                                        {
                                            SequenceId = i + ((((CurrentPage - 1)*ItemsPerPage) + 1)),
                                            Id = n.Id,
                                            DocumentRef = n.DocumentReference,
                                            InvoiceDate = n.DocumentDateIssued.ToString("dd-MMM-yyyy"),
                                            TotalGross = n.TotalGross,
                                            TotalNet = n.TotalNet,
                                            TotalVat = n.TotalVat,
                                            InvoiceOrderId = n.OrderId,
                                            CanIssueCreditNote = (cnsTotal < n.TotalGross)
                                        });
                                    i++;
                                }
                        );
                }
                else if (CurrentPage == 1 && SearchText.Trim() != "")
                {
                    InvoicesListCached = Search(InvoicesListCached);
                }

                RecordsCount = InvoicesListCached.Count;
                PageCount = (int) Math.Ceiling(Math.Round(((double) RecordsCount)/ItemsPerPage, 1));
                listItems = InvoicesListCached.Skip((CurrentPage - 1)*ItemsPerPage).Take(ItemsPerPage).ToList();

                DumpAndDeployInvoices(listItems);
            }
        }

        void GetInvoiceAmounts()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                Invoice invoice = Using<IInvoiceRepository>(c).GetById(new Guid(InvoiceNo)) as Invoice ;
                GrossTotal = 0m;
                TotalPaid = 0m;
                if (invoice != null)
                {
                    List<Receipt> receipts = Using<IReceiptRepository>(c).GetByInvoiceId(invoice.Id);
                    OrderId = invoice.OrderId;
                   
                    var invoiceCreditNotes = Using<ICreditNoteRepository>(c).GetCreditNotesByInvoiceId(invoice.Id);
                    var creditNotesTotals = new decimal();
                    if (invoiceCreditNotes != null)
                    {
                        foreach (var cn in invoiceCreditNotes)
                        {
                            creditNotesTotals = creditNotesTotals + cn.Total;
                        }
                    }
                    TotalPaid = receipts.Sum(n => n.Total);
                    //Get the unpaid balance as the gross amount

                    GrossTotal = (invoice.TotalGross - creditNotesTotals) - TotalPaid;
                    InvoiceRef = invoice.DocumentReference;
                }
                else
                {
                    TotalPaid = 0;
                    GrossTotal = 0;
                }
            }
        }

        void RunLoadUpaidInvoices()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                List<ListInvoiceItemsViewModel> listItems = new List<ListInvoiceItemsViewModel>();

                if (CurrentPage == 1 && SearchText.Trim() == "")
                {
                    List<Invoice> invoices = Using<IInvoiceRepository>(c) .GetAll(StartDate, EndDate).OfType<Invoice>()
                                                            .Where(n => n.Status != DocumentStatus.Rejected).ToList();
                    _orders.Clear();
                    ReceiptsOfUnpaidInvoices.Clear();
                    InvoicesListCached.Clear();
                    int i = 0;
                    foreach (var inv in invoices.OrderByDescending(n => n.DocumentDateIssued))
                    {
                        List<Receipt> irs = Using<IReceiptRepository>(c) .GetByInvoiceId(inv.Id);
                        irs.ForEach(ReceiptsOfUnpaidInvoices.Add); //cache

                        var invoiceCreditNotes = Using<ICreditNoteRepository>(c).GetCreditNotesByInvoiceId(inv.Id);
                        decimal crTotals = invoiceCreditNotes.Aggregate(new decimal(),
                                                                        (current, item) => current + item.Total);

                        var irsTotals = irs.Aggregate(new decimal(), (current, item) => current + item.Total);
                        if ((inv.TotalGross - crTotals) > irsTotals)
                        {
                            try
                            {
                                var order = Using<IOrderRepository>(c) .GetById(inv.OrderId) as Order;
                                _orders.Add(order);
                                InvoicesListCached.Add(new ListInvoiceItemsViewModel
                                    {
                                        SequenceId = i + ((((CurrentPage - 1)*ItemsPerPage) + 1)),
                                        DocumentRef = inv.DocumentReference,
                                        TotalGross = inv.TotalGross,
                                        Id = inv.Id,
                                        TotalNet = inv.TotalNet,
                                        TotalVat = inv.TotalVat,
                                        TotalPaid = irsTotals,
                                        InvoiceDate = inv.DocumentDateIssued.ToString("dd-MMM-yyyy"),
                                        InvoiceOrderId = inv.OrderId,
                                        AmountDue = inv.TotalGross - irsTotals,
                                        HasUnconfirmedPayment = CheckHasUnconfirmedPayment(irs),
                                        OrderDocRef = order.DocumentReference
                                    });
                                i++;
                            }
                            catch
                            {
                                MessageBox.Show("An error occurred while loading invoice " + inv.DocumentReference +
                                                " details.");
                            }
                        }
                    }
                }
                else if (CurrentPage == 1 && SearchText.Trim() != "")
                {
                    InvoicesListCached = Search(InvoicesListCached);
                }

                RecordsCount = InvoicesListCached.Count;
                PageCount = (int) Math.Ceiling(Math.Round(((double) RecordsCount)/ItemsPerPage, 1));
                listItems = InvoicesListCached.Skip((CurrentPage - 1)*ItemsPerPage).Take(ItemsPerPage).ToList();

                DumpAndDeployInvoices(listItems);
            }
        }
        void DumpAndDeployInvoices(List<ListInvoiceItemsViewModel> listItems)
        {
           Application.Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                InvoicesList.Clear();
                listItems.ForEach(InvoicesList.Add);
            }));
          
        }

        bool CheckHasUnconfirmedPayment(List<Receipt> receipts)
        {
            bool retVal = false;

            var unconfirmed = GetUnconfirmedLineItems(receipts);
            if (unconfirmed.Count() > 0)
                retVal = true;

            return retVal;
        }

        List<ReceiptLineItem> GetUnconfirmedLineItems(List<Receipt> receipts)
        {
            List<Receipt> unconfirmed = receipts.Where(n => n.LineItems.Any(l => l.LineItemType == OrderLineItemType.DuringConfirmation)).ToList();
            List<ReceiptLineItem> unconfirmedLis = receipts.SelectMany(n => n.LineItems.Where(l => l.LineItemType == OrderLineItemType.DuringConfirmation)).ToList();

            foreach (var rct in unconfirmed)
            {
                var childRecs = receipts.Where(n => n.PaymentDocId == rct.Id).ToList();
                foreach (var li in rct.LineItems.Where(n => n.LineItemType == OrderLineItemType.DuringConfirmation))
                {
                    var childLis = childRecs.SelectMany(n => n.LineItems.Where(l => l.PaymentDocLineItemId == li.Id)).ToList();
                    var totalConfirmed = childLis.Sum(n => n.Value);
                    if (li.Value - totalConfirmed <= 0)
                    {
                        var targt = unconfirmedLis.FirstOrDefault(n => n.Id == li.Id);
                        if (targt != null)
                            unconfirmedLis.Remove(li);
                    }
                }
            }

            return unconfirmedLis;
        }

        List<UnconfirmedReceiptPayment> UnconfirmedReceiptPaymentsCache = new List<UnconfirmedReceiptPayment>();
        public void LoadUnconfirmedReceiptItems(Guid invoiceId)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                if (InvoicesList != null && InvoicesList.Count == 0)
                    return;

                List<Receipt> selectedReceipts = ReceiptsOfUnpaidInvoices;
                List<UnconfirmedReceiptPayment> listItems = new List<UnconfirmedReceiptPayment>();

                if (CurrentPage == 1 && SearchText.Trim() == "")
                {
                    if (invoiceId != Guid.Empty)
                        selectedReceipts = ReceiptsOfUnpaidInvoices.Where(n => n.InvoiceId == invoiceId).ToList();
                    int i = 0;
                    UnconfirmedReceiptPaymentsCache.Clear();
                    foreach (var rec in selectedReceipts /*.OrderByDescending(n => n.DocumentDateIssued)*/)
                    {
                        decimal unconfirmedAmnt = 0m;
                        foreach (var item in rec.LineItems)
                        {
                            if (! Using<ITransactionalViewmodelRefactoring>(c).LineItemIsConfirmed(rec, item.Id, out unconfirmedAmnt))
                            {
                                try
                                {
                                    if (unconfirmedAmnt <= 0)
                                        continue;
                                    var recInvoice = InvoicesListCached.FirstOrDefault(inv => inv.Id == rec.InvoiceId);
                                    if (recInvoice == null)
                                        continue;
                                    var order = _orders.FirstOrDefault(n => n.Id == recInvoice.InvoiceOrderId);

                                    if (order == null)
                                        order =
                                           Using<IOrderRepository>(c).GetById(
                                                InvoicesListCached.FirstOrDefault(n => n.Id == rec.InvoiceId)
                                                                  .InvoiceOrderId) as Order;

                                    UnconfirmedReceiptPaymentsCache.Add(Map(item, rec, order, unconfirmedAmnt, i));
                                    i++;
                                }
                                catch
                                {
                                    MessageBox.Show("Error loading unconfirmed receipt payments.");
                                }
                            }
                        }
                    }
                }
                else if (CurrentPage == 1 && SearchText.Trim() != "")
                {
                    UnconfirmedReceiptPaymentsCache = Search(UnconfirmedReceiptPaymentsCache);
                }

                RecordsCount = UnconfirmedReceiptPaymentsCache.Count;
                PageCount = (int) Math.Ceiling(Math.Round(((double) RecordsCount)/ItemsPerPage, 1));

                listItems =
                    UnconfirmedReceiptPaymentsCache.Skip((CurrentPage - 1)*ItemsPerPage).Take(ItemsPerPage).ToList();

                DumpAndDeployUnconfirmedReceipt(listItems);
            }
        }

        void DumpAndDeployUnconfirmedReceipt(List<UnconfirmedReceiptPayment> selectedReceipts)
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(delegate
                               {
                                   UnconfirmedReceiptPayments.Clear();
                                   selectedReceipts.ForEach(UnconfirmedReceiptPayments.Add);
                               }));

        }

        UnconfirmedReceiptPayment Map(ReceiptLineItem li, Receipt rec, Order order, decimal unconfirmedAmt, int count)
        {
            return new UnconfirmedReceiptPayment
                       {
                           SequenceId = count + ((((CurrentPage - 1)*ItemsPerPage) + 1)),
                           Amount = unconfirmedAmt,//li.Value,
                           LineItemId = li.Id,
                           ReceiptId = rec.Id,
                           ReceiptDocReference = rec.DocumentReference,
                           InvoiceDocReference = InvoicesListCached.FirstOrDefault(n => n.Id == rec.InvoiceId).DocumentRef,
                           InvoiceId = InvoicesListCached.FirstOrDefault(n => n.Id == rec.InvoiceId).Id,
                           OrderId = InvoicesListCached.FirstOrDefault(n => n.Id == rec.InvoiceId).InvoiceOrderId,
                           PaymentType = li.MMoneyPaymentType,
                           OrderDocRef = order.DocumentReference,
                           Description = li.Description
                       };
        }

        List<ListInvoiceItemsViewModel> Search(List<ListInvoiceItemsViewModel> listItems)
        {
            var searchText = SearchText.ToLower();
            var temp = new List<ListInvoiceItemsViewModel>();
            listItems.Where(n =>
                                        n.DocumentRef.ToLower().Contains(searchText)
                                        || n.InvoiceDate.ToString().ToLower().Contains(searchText)
                                        || n.OrderDocRef.ToLower().Contains(searchText)
                                        || n.DocumentRef.ToLower().Contains(searchText)
                ).ToList().ForEach(temp.Add);
            listItems.Clear();
            int i = 0;
            temp.ForEach(n =>
                             {
                                 n.SequenceId = i + ((((CurrentPage - 1)*ItemsPerPage) + 1));
                                 listItems.Add(n);
                                 i++;
                             });

            return listItems;
        }

        List<UnconfirmedReceiptPayment> Search(List<UnconfirmedReceiptPayment> listItems)
        {
            var searchText = SearchText.ToLower();
            var temp = new List<UnconfirmedReceiptPayment>();
            listItems.Where(n =>
                            n.Description.ToLower().Contains(searchText)
                            || n.InvoiceDocReference.ToString().ToLower().Contains(searchText)
                            || n.OrderDocRef.ToLower().Contains(searchText)
                            || n.PaymentType.ToLower().Contains(searchText)
                            || n.ReceiptDocReference.ToLower().Contains(searchText)
                ).ToList().ForEach(temp.Add);

            int i = 0;
            temp.ForEach(n =>
            {
                n.SequenceId = i + ((((CurrentPage - 1) * ItemsPerPage) + 1));
                listItems.Add(n);
                i++;
            });

            return listItems;
        }

        #region HelperClasses
        public class ListInvoiceItemsViewModel : ViewModelBase
        {
            public int SequenceId { get; set; }
            public const string IdPropertyName = "Id";
            private Guid _id = Guid.Empty;
            public Guid Id
            {
                get
                {
                    return _id;
                }

                set
                {
                    if (_id == value)
                    {
                        return;
                    }

                    var oldValue = _id;
                    _id = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(IdPropertyName);
                }
            }

            public const string DocumentRefPropertyName = "DocumentRef";
            private string _documentRef = null;
            public string DocumentRef
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
                    RaisePropertyChanged(DocumentRefPropertyName);
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
            private decimal _amountDue = 0;
            public decimal AmountDue
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

            public bool HasUnconfirmedPayment { get; set; }

            public bool CanIssueCreditNote { get; set; }
        }

        public class ListReceiptViewModel : ViewModelBase
        {
            public const string IdPropertyName = "Id";
            private Guid _Id = Guid.Empty;
            public Guid Id
            {
                get
                {
                    return _Id;
                }

                set
                {
                    if (_Id == value)
                    {
                        return;
                    }

                    var oldValue = _Id;
                    _Id = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(IdPropertyName);
                }
            }

            public const string ReceiptTotalPropertyName = "ReceiptTotal";
            private decimal _ReceiptTotal = 0;
            public decimal ReceiptTotal
            {
                get
                {
                    return _ReceiptTotal;
                }

                set
                {
                    if (_ReceiptTotal == value)
                    {
                        return;
                    }

                    var oldValue = _ReceiptTotal;
                    _ReceiptTotal = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(ReceiptTotalPropertyName);
                }
            }

            public const string InvoiceIdPropertyName = "InvoiceId";
            private Guid _InvoiceId = Guid.Empty;
            public Guid InvoiceId
            {
                get
                {
                    return _InvoiceId;
                }

                set
                {
                    if (_InvoiceId == value)
                    {
                        return;
                    }

                    var oldValue = _InvoiceId;
                    _InvoiceId = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(InvoiceIdPropertyName);
                }
            }
        }

        public class UnconfirmedReceiptPayment
        {
            public int SequenceId{get; set;}
            public Guid LineItemId{get; set;}
            public decimal Amount{get; set;}
            public string ReceiptDocReference{get; set;}
            public string InvoiceDocReference{get; set;}
            public Guid ReceiptId{get; set;}
            public Guid InvoiceId { get; set; }
            public string PaymentType { get; set; }
            public Guid OrderId { get; set; }
            public string OrderDocRef { get; set; }
            public string Description { get; set; }
        }
        #endregion

        
    }
}
