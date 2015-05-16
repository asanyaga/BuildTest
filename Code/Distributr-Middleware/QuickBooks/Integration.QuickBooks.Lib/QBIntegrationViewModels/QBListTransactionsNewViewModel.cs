using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Intergration;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr.Core.Utility;
using Distributr.WSAPI.Lib.Integrations;
using Distributr_Middleware.WPF.Lib.Utils;
using Distributr_Middleware.WPF.Lib.ViewModels;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Integration.QuickBooks.Lib.EF.Entities;
using Integration.QuickBooks.Lib.QBIntegrationCore;
using Integration.QuickBooks.Lib.Repository;
using Integration.QuickBooks.Lib.Repository.Impl;
using Integration.QuickBooks.Lib.Services;
using Integration.QuickBooks.Lib.Services.Impl;
using Newtonsoft.Json;
using QBFC13Lib;
using StructureMap;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace Integration.QuickBooks.Lib.QBIntegrationViewModels
{
    public class QBListTransactionsNewViewModel : MiddleWareViewModelBase
    {


        internal List<QuickBooksOrderDocumentDto> PagedList;
        internal List<QuickBooksOrderDocumentDto> UnClosedOrdersList;
        internal List<QuickBooksReturnInventoryDocumentDto> ReturnsList; 
        private Page _currentPage;
        private TabItem tabItem;
        internal List<string> NewOutlets;
        internal List<string> NewProducts;
        internal List<string> ExportAudits;

        public QBListTransactionsNewViewModel()
        {
            SalesOrdersList = new ObservableCollection<SaleOrderListItem>();
            ExportSelectedCommand = new RelayCommand(ExportSelected);
            ExportAllCommand = new RelayCommand(ExportAll);
            NewOutlets = new List<string>();
            NewProducts = new List<string>();
            ExportAudits=new List<string>();
            PagedList=new List<QuickBooksOrderDocumentDto>();
            UnClosedOrdersList = new List<QuickBooksOrderDocumentDto>();
            ReturnsList=new List<QuickBooksReturnInventoryDocumentDto>();

            COGSAccountList = new ObservableCollection<QBAccount>();
            IncomeAccountList = new ObservableCollection<QBAccount>();
            AssetAccountList = new ObservableCollection<QBAccount>();
            TradeReceivableAccountList=new ObservableCollection<QBAccount>();
            OrderTypeToLoad=OrderType.OutletToDistributor;
            IncludeReceiptsAndInvoice = true; 
            ReturnInventoryItemList = new ObservableCollection<ReturnInventoryItem>();
        }

        

        #region properties

        public ObservableCollection<QBAccount> AssetAccountList { get; set; }
        public ObservableCollection<QBAccount> IncomeAccountList { get; set; }
        public ObservableCollection<QBAccount> COGSAccountList { get; set; }
        public ObservableCollection<QBAccount> TradeReceivableAccountList { get; set; }

        public ObservableCollection<SaleOrderListItem> SalesOrdersList { get; set; }

        public ObservableCollection<ReturnInventoryItem> ReturnInventoryItemList { get; set; }

        internal OrderType OrderTypeToLoad;

        public RelayCommand ExportSelectedCommand { get; set; }
        public RelayCommand ExportAllCommand { get; set; }

        private RelayCommand<SelectionChangedEventArgs> _tabSelectionChangedCommand;


        public RelayCommand<SelectionChangedEventArgs> TabSelectionChangedCommand
        {
            get
            {
                return _tabSelectionChangedCommand ?? (_tabSelectionChangedCommand = new RelayCommand<SelectionChangedEventArgs>(TabSelectionChanged));
            }
        }
        private RelayCommand<Page> _loadPageCommand = null;
        public RelayCommand<Page> LoadPageCommand
        {
            get { return _loadPageCommand ?? (_loadPageCommand = new RelayCommand<Page>(LoadPage)); }
        }

        public const string SelectedSalePropertyName = "SelectedSale";
        private SaleOrderListItem _selectedSale = null;
        public SaleOrderListItem SelectedSale
        {
            get { return _selectedSale; }

            set
            {
                if (_selectedSale == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedSalePropertyName);
                _selectedSale = value;
                RaisePropertyChanged(SelectedSalePropertyName);
            }
        }

        public const string StartDatePropertyName = "StartDate";
        private DateTime _startDate = DateTime.Now;
        public DateTime StartDate
        {
            get { return _startDate; }

            set
            {
                if (_startDate == value)
                {
                    return;
                }

                RaisePropertyChanging(StartDatePropertyName);
                _startDate = value;
                RaisePropertyChanged(StartDatePropertyName);
            }
        }

        public const string EndDatePropertyName = "EndDate";
        private DateTime _endDate = DateTime.Now;
        public DateTime EndDate
        {
            get { return _endDate; }

            set
            {
                if (_endDate == value)
                {
                    return;
                }

                RaisePropertyChanging(EndDatePropertyName);
                _endDate = value;
                RaisePropertyChanged(EndDatePropertyName);
            }
        }

        protected QBMainWindowViewModel HomeViewModel
        {
            get { return SimpleIoc.Default.GetInstance<QBMainWindowViewModel>(); }
        }
        


        private RelayCommand<bool?> _includeReceiptsCommand;
        public RelayCommand<bool?> ToggleInvoiceReceiptsCommand
        {
            get { return _includeReceiptsCommand ?? (_includeReceiptsCommand = new RelayCommand<bool?>(IncludeReceipts)); }
        }

        private void IncludeReceipts(bool? obj)
        {
            IncludeReceiptsAndInvoice = obj.HasValue && obj.Value;
        }

        private ITransactionsDownloadService _transactionsDownloadService;
        protected ITransactionsDownloadService TransactionsDownloadService
        {
            get
            {
                return _transactionsDownloadService ?? (_transactionsDownloadService = new TransactionsDownloadService());
            }
        }

        private ITransactionRepository _transactionRepository;
        protected ITransactionRepository TransactionRepository
        {
            get
            {
                return _transactionRepository ?? (_transactionRepository = new TransactionRepository());
            }
        }

        private IOrderImportRepository _orderImportRepository;
        protected IOrderImportRepository OrderImportRepository
        {
            get
            {
                return _orderImportRepository ?? (_orderImportRepository = new OrderImportRepository());
            }
        }

        private IInvoiceImportRepository _invoiceImportRepository;
        protected IInvoiceImportRepository InvoiceImportRepository
        {
            get
            {
                return _invoiceImportRepository ?? (_invoiceImportRepository = new InvoiceImportRepository());
            }
        }

      

        public const string IncludeReceiptsAndInvoicePropertyName = "IncludeReceiptsAndInvoice";
        private bool _includeReceipts = true;
        public bool IncludeReceiptsAndInvoice
        {
            get { return _includeReceipts; }

            set
            {
                if (_includeReceipts == value)
                {
                    return;
                }

                RaisePropertyChanging(IncludeReceiptsAndInvoicePropertyName);
                _includeReceipts = value;
                RaisePropertyChanged(IncludeReceiptsAndInvoicePropertyName);
            }
        }

        public const string SelectedCOGSAccountPropertyName = "SelectedCOGSAccount";
        private QBAccount _selectedCOGSAccount = null;
        public QBAccount SelectedCOGSAccount
        {
            get { return _selectedCOGSAccount; }

            set
            {
                if (_selectedCOGSAccount == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCOGSAccountPropertyName);
                _selectedCOGSAccount = value;
                RaisePropertyChanged(SelectedCOGSAccountPropertyName);
            }
        }

        public const string SelectedIncomeAccountPropertyName = "SelectedIncomeAccount";
        private QBAccount _selectedIncomeAccount = null;

        public QBAccount SelectedIncomeAccount
        {
            get { return _selectedIncomeAccount; }

            set
            {
                if (_selectedIncomeAccount == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedIncomeAccountPropertyName);
                _selectedIncomeAccount = value;
                RaisePropertyChanged(SelectedIncomeAccountPropertyName);
            }
        }

        public const string SelectedAssetAccountPropertyName = "SelectedAssetAccount";
        private QBAccount _selectedAssetAccount = null;

        public QBAccount SelectedAssetAccount
        {
            get { return _selectedAssetAccount; }

            set
            {
                if (_selectedAssetAccount == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedAssetAccountPropertyName);
                _selectedAssetAccount = value;
                RaisePropertyChanged(SelectedAssetAccountPropertyName);
            }
        }


        public const string SelectedTradeReceivableAccountPropertyName = "SelectedTradeReceivableAccount";
        private QBAccount _selectedTradeReceivableAccount = null;

        public QBAccount SelectedTradeReceivableAccount
        {
            get { return _selectedTradeReceivableAccount; }

            set
            {
                if (_selectedTradeReceivableAccount == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedTradeReceivableAccountPropertyName);
                _selectedTradeReceivableAccount = value;
                RaisePropertyChanged(SelectedTradeReceivableAccountPropertyName);
            }
        }

        private QBAccount _defaultAccount;

        public QBAccount DefaultAccount
        {
            get
            {
                return _defaultAccount ??
                       (_defaultAccount = new QBAccount { QBAccountId = "", AccountName = "--Select Account--" });
            }
        }



        #endregion

        #region Methods

        void ShowBusy(bool isBusy)
        {
            Dispatcher.CurrentDispatcher.Invoke((Action)(() =>
            {
                var control = _currentPage.FindName("listSales") as UserControl ?? _currentPage.FindName("listOrders") as UserControl;

                if (control != null)
                {
                    var busy = control.FindName("busy") as BusyIndicator;
                    if (busy != null)
                    {
                        busy.BusyContent = "Processing Please wait...";
                        busy.IsBusy = isBusy;
                        if (isBusy)
                            busy.Focus();
                    }
                }
            }));
        }

        private void Setup()
        {
            LoadAccountsLists();

            LoadClosedSalesOrders();

        }

        private  void LoadAccountsLists()
        {

            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    delegate
                    {
                        HomeViewModel.GlobalStatus =
                            string.Format("Connecting to Quickbooks....");
                        HomeViewModel.CanConnectToQuickBooks = QBFC_Core.CanConnect();

                        if (HomeViewModel.CanConnectToQuickBooks)
                        {
                            HomeViewModel.GlobalStatus = string.Format("Done....");
                            LoadAssetAccountsLists();
                            LoadCOGSAccountsLists();
                            LoadIncomeAccountsLists();
                            LoadTradeReceivableAccountsLists();
                        }
                        else
                        {
                            HomeViewModel.GlobalStatus =
                               string.Format("Connection to Quickbooks failed....");
                        }
                    }));

        }

        private void LoadAssetAccountsLists()
        {
            HomeViewModel.GlobalStatus =
                string.Format("Downloading {0}accounts from Quick Books ...", "");
            AssetAccountList.Clear();
            AssetAccountList.Add(DefaultAccount);
            SelectedAssetAccount = DefaultAccount;
            IAccountRetList accountRetList = QBIntegrationMethods.GetAssetAccounts();
            if (accountRetList != null && accountRetList.Count != 0)
            {
                for (int i = 0; i <= accountRetList.Count - 1; i++)
                {
                    HomeViewModel.GlobalStatus =
                        string.Format("Downloaded {0} account from Quick Books ...", i);
                    IAccountRet accountRet = accountRetList.GetAt(i);
                    ENAccountType accntType = accountRet.AccountType.GetValue();
                    QBAccount account = new QBAccount
                    {
                        AccountName = accountRet.Name.GetValue(),
                        AccountNumber = accountRet.AccountNumber.GetValue(),
                        QBAccountId = accountRet.ListID.GetValue(),
                        AccountType = QBAccountType.CurrentAssetsAccount
                    };
                    AssetAccountList.Add(account);
                }
            }
            SimpleIoc.Default.GetInstance<QBMainWindowViewModel>().GlobalStatus =
                string.Format("Ready.");
        }

        private void LoadCOGSAccountsLists()
        {
            HomeViewModel.GlobalStatus =
                string.Format("Downloading {0}accounts from Quick Books ...", "");
            COGSAccountList.Clear();
            COGSAccountList.Add(DefaultAccount);
            SelectedCOGSAccount = DefaultAccount;
            IAccountRetList accountRetList = QBIntegrationMethods.GetCOGSAccounts();
            if (accountRetList != null && accountRetList.Count != 0)
            {
                for (int i = 0; i <= accountRetList.Count - 1; i++)
                {
                    HomeViewModel.GlobalStatus =
                        string.Format("Downloaded {0} account from Quick Books ...", i);
                    IAccountRet accountRet = accountRetList.GetAt(i);
                    ENAccountType accntType = accountRet.AccountType.GetValue();
                    QBAccount account = new QBAccount
                    {
                        AccountName = accountRet.Name.GetValue(),
                        AccountNumber = accountRet.AccountNumber.GetValue(),
                        QBAccountId = accountRet.ListID.GetValue(),
                        AccountType = QBAccountType.COGSAccount
                    };
                    COGSAccountList.Add(account);
                }
            }
            SimpleIoc.Default.GetInstance<QBMainWindowViewModel>().GlobalStatus =
                string.Format("Ready.");
        }

        private void LoadIncomeAccountsLists()
        {
            SimpleIoc.Default.GetInstance<QBMainWindowViewModel>().GlobalStatus =
                string.Format("Downloading {0}accounts from Quick Books ...", "");
            IncomeAccountList.Clear();
            IncomeAccountList.Add(DefaultAccount);
            SelectedIncomeAccount = DefaultAccount;
            IAccountRetList accountRetList = QBIntegrationMethods.GetIncomeAccounts();
            if (accountRetList != null && accountRetList.Count != 0)
            {
                for (int i = 0; i <= accountRetList.Count - 1; i++)
                {
                    SimpleIoc.Default.GetInstance<QBMainWindowViewModel>().GlobalStatus =
                        string.Format("Downloaded {0} account from Quick Books ...", i);
                    IAccountRet accountRet = accountRetList.GetAt(i);
                    ENAccountType accntType = accountRet.AccountType.GetValue();
                    QBAccount account = new QBAccount
                    {
                        AccountName = accountRet.Name.GetValue(),
                        AccountNumber = accountRet.AccountNumber.GetValue(),
                        QBAccountId = accountRet.ListID.GetValue(),
                        AccountType = QBAccountType.IncomeAccount
                    };
                    IncomeAccountList.Add(account);
                }
            }
            SimpleIoc.Default.GetInstance<QBMainWindowViewModel>().GlobalStatus =
                string.Format("Ready.");
        }

        private void LoadTradeReceivableAccountsLists()
        {
            SimpleIoc.Default.GetInstance<QBMainWindowViewModel>().GlobalStatus =
                string.Format("Downloading {0}accounts from Quick Books ...", "");
            TradeReceivableAccountList.Clear();
            TradeReceivableAccountList.Add(DefaultAccount);
            SelectedTradeReceivableAccount = DefaultAccount;
            IAccountRetList accountRetList = QBIntegrationMethods.GetReceivableAccounts();
            if (accountRetList != null && accountRetList.Count != 0)
            {
                for (int i = 0; i <= accountRetList.Count - 1; i++)
                {
                    SimpleIoc.Default.GetInstance<QBMainWindowViewModel>().GlobalStatus =
                        string.Format("Downloaded {0} account from Quick Books ...", i);
                    IAccountRet accountRet = accountRetList.GetAt(i);
                    ENAccountType accntType = accountRet.AccountType.GetValue();
                    QBAccount account = new QBAccount
                    {
                        AccountName = accountRet.Name.GetValue(),
                        AccountNumber = accountRet.AccountNumber.GetValue(),
                        QBAccountId = accountRet.ListID.GetValue(),
                        AccountType = QBAccountType.ReceivableAccount
                    };

                    TradeReceivableAccountList.Add(account);
                }
            }
            SimpleIoc.Default.GetInstance<QBMainWindowViewModel>().GlobalStatus =
                string.Format("Ready.");
        }

        public void LoadClosedSalesOrders()
        {
            if (PagedList.Any())
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                {
                    {
                        ShowBusy(true);
                        HomeViewModel.GlobalStatus = string.Format("Loading....");
                    }
                }));
                LoadExisting();
                //LoadFromLocalDB();
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                {
                    {
                        ShowBusy(true);
                        HomeViewModel.GlobalStatus =
                            string.Format(
                                "Downloading from HQ..please wait...."
                                )
                            ;
                    }
                }));
                DownloadFromHq();
                //LoadFromLocalDB();
            }
        }


        private void LoadExisting()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                {
                    if (PagedList == null) return;

                    SalesOrdersList.Clear();
                    if (OrderTypeToLoad ==
                        OrderType.DistributorPOS)
                    {
                       // 
                        var data =
                            PagedList.Where(
                                p =>
                                p.OrderType == OrderTypeToLoad)
                                .
                                ToList();
                        if (data.Any())
                        {
                            data.Select(MapSelector).ToList().
                                ForEach(
                                    SalesOrdersList.Add);

                            var tabitem =
                                _currentPage.FindName(
                                    "TabSales") as
                                TabItem;
                            if (tabitem != null)
                                tabitem.Focus();
                        }
                        else
                        {
                            DownloadFromHq();
                        }
                    }
                    else
                    {
                        SalesOrdersList.Clear();
                        var data =
                            PagedList.Where(
                                p =>
                                p.OrderType == OrderTypeToLoad)
                                .
                                ToList();
                        //data = PagedList.ToList();
                        if (data.Any())
                        {
                            data.Select(MapSelector).ToList().
                                ForEach(
                                    SalesOrdersList.Add);
                            var tabitem =
                                _currentPage.FindName(
                                    "TabOrders") as
                                TabItem;
                            if (tabitem != null)
                            {
                                tabitem.Focus();
                            }

                        }
                    }
                }
            }));

        }

       private void LoadFromLocalDB()
       {
           var tabName = "";
           if(OrderTypeToLoad ==OrderType.DistributorPOS)
           {
               LoadSalesOrders();
               tabName = "TabSales";
           }

           if (OrderTypeToLoad == OrderType.OutletToDistributor)
           {
               LoadUnclosedOrders();
               tabName = "TabOrders";
           }
           SalesOrdersList.Clear();
           if(PagedList.Any())
           {
               PagedList.Select(MapSelector).ToList().ForEach(SalesOrdersList.Add);
               var tabitem =_currentPage.FindName(tabName) as TabItem;
               if (tabitem != null)
               {
                   tabitem.Focus();
               }
           }
           else
           {
               DownloadFromHq();
           }

               
       }

        private void LoadSalesOrders()
        {
            //var orderRepository = TransactionRepository;
            //var orders = orderRepository.LoadFromDB(TransactionType.DistributorPOS);
            var orderRepository = OrderImportRepository;
            var saleOrders = orderRepository.LoadFromDB(OrderType.DistributorPOS);
            PagedList.Clear();
            PagedList = MapOrder(saleOrders, OrderType.DistributorPOS);
        }

        private List<QuickBooksOrderDocumentDto> MapOrder(List<OrderExportDocument> orders, OrderType orderTypeToLoad)
        {
           List<QuickBooksOrderDocumentDto> doc=new List<QuickBooksOrderDocumentDto>();
            foreach (var order in orders)
            {
                var quickBooksOrderDocDto = new QuickBooksOrderDocumentDto();
                quickBooksOrderDocDto.GenericReference = order.OrderRef;
                quickBooksOrderDocDto.OrderType = orderTypeToLoad;
                quickBooksOrderDocDto.OrderDateRequired = order.OrderDueDate.ToShortTimeString();
                quickBooksOrderDocDto.DocumentDateIssued = order.OrderDate.ToString();
                quickBooksOrderDocDto.OutletName = order.OutletName;
                quickBooksOrderDocDto.OutletCode = order.OutletCode;
                quickBooksOrderDocDto.SalesmanCode = order.SalesmanCode;
                quickBooksOrderDocDto.TotalGross = order.TotalGross;
                quickBooksOrderDocDto.TotalDiscount = order.TotalDiscount;
                quickBooksOrderDocDto.TotalNet = order.TotalNet;
                quickBooksOrderDocDto.TotalVat = order.TotalVat;

                doc.Add(quickBooksOrderDocDto);
            }
            return doc;
        }

        private void LoadUnclosedOrders()
        {
            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                //var orderRepository = TransactionRepository;
                //var orders = orderRepository.LoadFromDB(TransactionType.OutletToDistributor);
                var orderRepository = OrderImportRepository;
                var orders = orderRepository.LoadFromDB(OrderType.OutletToDistributor);
                PagedList.Clear();
                PagedList = MapOrder(orders,OrderType.OutletToDistributor);//.AsEnumerable().Select(Map).ToList();
            }
        }

        private List<QuickBooksOrderDocumentDto> Map(List<TransactionImport> orders,OrderType orderType)
        {
            var doc = orders.GroupBy(s => s.ExternalRef).Select(s=>MapGroup(s,orderType)).ToList();//.ToList();//.SelectMany(MapGroup).ToList();//.Select(MapGroup).ToList();
            return doc;
        }

        private QuickBooksOrderDocumentDto MapGroup(IEnumerable<TransactionImport> transactionImports,OrderType orderType)
        {
            var doc = new QuickBooksOrderDocumentDto();

            var transactionImportList = transactionImports as List<TransactionImport> ?? transactionImports.ToList();
            var firstOrDefault = transactionImportList.FirstOrDefault();
            if (firstOrDefault != null)
            {
                doc.GenericReference = firstOrDefault.GenericRef;
                doc.ExternalReference = firstOrDefault.ExternalRef;
                doc.DocumentDateIssued = firstOrDefault.TransactionIssueDate.ToString();
                doc.OrderDateRequired = firstOrDefault.TransactionDueDate.ToString();
                doc.OutletCode = firstOrDefault.OutletCode;
                doc.OutletName = firstOrDefault.OutletName;
                doc.SalesmanCode = firstOrDefault.SalesmanCode;
                doc.TotalNet = firstOrDefault.TotalNet;
                doc.OrderType = orderType;
                doc.DocumentType = GetDocumentType(firstOrDefault.TransactionType);
            }

            foreach (var transactionImport in transactionImportList)
            {
                var lineItem = new QuickBooksOrderDocLineItem();
                lineItem.ProductCode = transactionImport.ProductCode;
                lineItem.Quantity = transactionImport.Quantity;
                lineItem.LineItemValue = transactionImport.LineItemValue;
                lineItem.VATClass = transactionImport.VatClass;
                lineItem.TotalNet = transactionImport.TotalNet;
                lineItem.TotalVat = transactionImport.TotalVat;

                doc.LineItems.Add(lineItem);
            }
            return doc;
        }

        private DocumentType GetDocumentType(int transactionType)
        {
            if(transactionType==(int)TransactionType.DistributorPOS || transactionType==(int)TransactionType.OutletToDistributor)
                return DocumentType.Order;
            if (transactionType == (int)TransactionType.Invoice)
                return DocumentType.Invoice;

            return DocumentType.Receipt;
        }


        //private IEnumerable<QuickBooksOrderDocumentDto> MapGroup(IEnumerable<TransactionImport> @group)
        //{
        //    throw new NotImplementedException();
        //}

        //private QuickBooksOrderDocumentDto MapGroup(IGrouping<string, TransactionImport> @group)
        //{
        //    throw new NotImplementedException();
        //}


        private void DownloadFromHq()
        {
            Task.Run(() =>
                         {
                             SetStatus();
                             ExportSaleOrders();
                             //ExportUnclosedOrders();
                             ExportInvoices();
                             //ExportReceipts();
                             //ExportInventoryReturns();
                             //HomeViewModel.GlobalStatus =string.Format("Please wait downloading data..from HQ!");

                         });
           
        }

       

        private void SetStatus()
        {
            HomeViewModel.GlobalStatus = string.Format("Please wait downloading data,from HQ!..This might take a while..");
        }

        private void ExportInventoryReturns()
        {
            throw new NotImplementedException();
        }

        private  void ExportUnclosedOrders()
        {
            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                
                // var response = TransactionsDownloadService.DownloadAllAsync(SearchText, IncludeReceiptsAndInvoice);
                var response = TransactionsDownloadService.GetNextOrder(OrderType.OutletToDistributor,DocumentStatus.Confirmed);
                response.Wait();
                if (response.Result.Status)
                {
                    var doc = JsonConvert.DeserializeObject<OrderExportDocument>(response.Result.TransactionData);
                    //var orderExportRepository = TransactionRepository;
                    //var result = orderExportRepository.SaveToLocal(doc, TransactionType.OutletToDistributor);
                    var orderExportRepository = OrderImportRepository;
                    var result = orderExportRepository.SaveToLocal(doc, OrderType.OutletToDistributor);

                    if (result!=null && result.Count >= 1)
                    {
                        TransactionsDownloadService.MarkAsExported(doc.ExternalRef);
                        ExportUnclosedOrders();
                    }
                }
                if (!response.Result.Status)
                {
                    var info = "";
                    if (response.Result.Info.Contains("No Orders to import"))
                    {
                        info = response.Result.Info;
                    }
                    else
                    {
                        info = "An error occured please check log file";
                    }

                    HomeViewModel.GlobalStatus = string.Format(info);
                }
            }
        }

        private void ExportSaleOrders()
        {
            using(var c=ObjectFactory.Container.GetNestedContainer())
            {
                var response = TransactionsDownloadService.GetNextOrder(OrderType.DistributorPOS, DocumentStatus.Closed);
                response.Wait();
                if (response.Result.Status)
                {
                    var doc = JsonConvert.DeserializeObject<OrderExportDocument>(response.Result.TransactionData);
                    var orderExportRepository = OrderImportRepository;
                    var result = orderExportRepository.SaveToLocal(doc, OrderType.DistributorPOS);

                    if(result!=null && result.Count>=1)
                    {
                        TransactionsDownloadService.MarkAsExported(doc.ExternalRef);
                        ExportSaleOrders();
                    }
                }
                if(!response.Result.Success)
                {
                    string info;
                    if (response.Result.Info.Contains("No Sales Orders to import"))
                    {
                        info = response.Result.Info;
                    }
                    else
                    {
                        info = "An error occured please check log file";
                    }
                        
                    HomeViewModel.GlobalStatus = string.Format(info);
                }
            }
            
        }

        private void ExportInvoices()
        {
            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                var response = TransactionsDownloadService.GetNextOrder(OrderType.DistributorPOS, DocumentStatus.Closed);
                response.Wait();
                if (response.Result.Status)
                {
                    var doc = JsonConvert.DeserializeObject<OrderExportDocument>(response.Result.TransactionData);
                    var orderExportRepository = OrderImportRepository;
                    var result = orderExportRepository.SaveToLocal(doc, OrderType.DistributorPOS);

                    if (result != null && result.Count >= 1)
                    {
                        TransactionsDownloadService.MarkAsExported(doc.ExternalRef);
                        ExportInvoices();
                    }
                }
                if (!response.Result.Success)
                {
                    string info;
                    if (response.Result.Info.Contains("No Sales Orders to import"))
                    {
                        info = response.Result.Info;
                    }
                    else
                    {
                        info = "An error occured please check log file";
                    }

                    HomeViewModel.GlobalStatus = string.Format(info);
                }
            }

        }

        private void ExportClosedInvoice(string orderDocExternalRef)
        {
            using (var c = ObjectFactory.Container.GetNestedContainer())
            {

                // var response = TransactionsDownloadService.DownloadAllAsync(SearchText, IncludeReceiptsAndInvoice);
                var response = TransactionsDownloadService.GetNextInvoice();//.GetChildDocument(orderDocExternalRef, DocumentType.Invoice);//.GetNextOrder(OrderType.OutletToDistributor,DocumentStatus.Confirmed);
                response.Wait();
                if (response.Result.Status)
                {
                    var doc = JsonConvert.DeserializeObject<OrderExportDocument>(response.Result.TransactionData);
                    var orderExportRepository = c.GetInstance<ITransactionRepository>(); //TransactionRepository;//c.GetInstance<ITransactionRepository>();
                    var result = orderExportRepository.SaveToLocal(doc, TransactionType.Invoice);

                    if (result != null && result.Count >= 1)
                    {
                        TransactionsDownloadService.MarkAsExported(doc.ExternalRef);
                        ExportClosedInvoice(orderDocExternalRef);
                        //ExportReceipt(doc.ExternalRef);
                    }
                }
                if (!response.Result.Status)
                {
                    var info = "";
                    if (response.Result.Info.Contains("No Orders to import"))
                    {
                        info = response.Result.Info;
                    }
                    else
                    {
                        info = "An error occured please check log file";
                    }

                    HomeViewModel.GlobalStatus = string.Format(info);
                }
            }
        }

        private void ExportReceipts()
        {
            using (var c = ObjectFactory.Container.GetNestedContainer())
            {

                // var response = TransactionsDownloadService.DownloadAllAsync(SearchText, IncludeReceiptsAndInvoice);
                var response = TransactionsDownloadService.GetNextReceipt();//.GetChildDocument(orderDocExternalRef, DocumentType.Receipt);//.GetNextOrder(OrderType.OutletToDistributor,DocumentStatus.Confirmed);
                response.Wait();
                if (response.Result.Status)
                {
                    var doc = JsonConvert.DeserializeObject<OrderExportDocument>(response.Result.TransactionData);
                    var orderExportRepository = TransactionRepository;//c.GetInstance<ITransactionRepository>();
                    var result = orderExportRepository.SaveToLocal(doc, TransactionType.OutletToDistributor);

                    if (result != null && result.Count >= 1)
                    {
                        TransactionsDownloadService.MarkAsExported(doc.ExternalRef);
                        ExportReceipts();
                    }
                }
                if (!response.Result.Status)
                {
                    var info = "";
                    if (response.Result.Info.Contains("No Orders to import"))
                    {
                        info = response.Result.Info;
                    }
                    else
                    {
                        info = "An error occured please check log file";
                    }

                    HomeViewModel.GlobalStatus = string.Format(info);
                }
            }
        }

        private async void AcknowledgeAsync()
        {

            if (!ExportAudits.Any()) return;
            var service = new TransactionsDownloadService();
            HomeViewModel.GlobalStatus =string.Format("Pushing {0} acknowledgments to HQ", ExportAudits.Count);
            var done =await service.AcknowledgeDocuments(ExportAudits);
            if (done)
            {
                HomeViewModel.GlobalStatus = "Done..";

                #region Commented Code
                //Application.Current.Dispatcher.BeginInvoke(
                // new Action(
                //     delegate
                //     {

                //         foreach (var orderRef in ExportAudits)
                //         {
                //             var found = SalesOrdersList.FirstOrDefault(p => p.GenericReference == orderRef);
                //             var order = PagedList.FirstOrDefault(p => p.GenericReference == orderRef);
                //             if (found != null)
                //             {
                //                 SalesOrdersList.Remove(found);
                //                 PagedList.Remove(order);
                //                 LoadClosedSalesOrders();
                //             }
                //         }

                //         foreach (var itemRef in ExportAudits)
                //         {
                //             var found = ReturnInventoryItemList.FirstOrDefault(p => p.GenericReference == itemRef);
                //             var documentItem = ReturnsList.FirstOrDefault(p => p.GenericReference == itemRef);
                //             if (found != null)
                //             {
                //                 ReturnsList.Remove(documentItem);
                //                 ReturnInventoryItemList.Remove(found);
                //             }
                //         }

                //     }));
                #endregion

            }

        }


        private  void ExportSelected()
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    delegate
                    {
                        try
                        {
                            //string _appName = "Distributr QB Intergration";
                            //string qdbpath = ConfigurationSettings.AppSettings["QuickBooksCompanyFilePath"];

                            //QBSessionManager sessionManager = new QBSessionManager();
                            //sessionManager.OpenConnection("", _appName);
                            //sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);

                            HomeViewModel.GlobalStatus =string.Format("Exporting {0} sales to Quick Books ...", SalesOrdersList.Count(n => n.IsSelected));
                            int cnt = 0;
                            Random random = new Random();
                            if (tabItem.Name == "TabSales")
                            {
                                foreach (var sale in SalesOrdersList.Where(n => n.IsSelected))
                                {

                                    var order =PagedList.FirstOrDefault(p => p.GenericReference == sale.GenericReference);
                                    if (order != null && ExportAudits.All(p => !p.Equals(order.GenericReference)))
                                    {
                                        var lineItems = OrderImportRepository.GetLineItems(order.ExternalReference);
                                        order.LineItems.AddRange(lineItems);
                                        string externalOrderRef = order.ExternalReference;
                                        
                                        var qbOrder = QBIntegrationMethods.AddOrder(order, externalOrderRef);
                                        
                                        if (qbOrder != null)
                                        {
                                             var quickBooksId = qbOrder.TxnID.GetValue();
                                            OrderImportRepository.MarkExportedLocal(externalOrderRef,quickBooksId);
                                            //TransactionRepository.MarkExportedLocal(externalOrderRef);
                                            
                                            ExportAudits.Add(order.GenericReference);
                                           

                                            string externalInvoiceRef =GetExternalOrderRef(random);
                                            var invoice =PagedList.FirstOrDefault(p =>p.DocumentType == DocumentType.Invoice &&p.ExternalReference ==order.ExternalReference);
                                            if (invoice != null)
                                            {
                                                var account = SelectedTradeReceivableAccount.QBAccountId.ToString(CultureInfo.InvariantCulture);

                                                var inv =QBIntegrationMethods.AddInvoice(
                                                        invoice, quickBooksId,
                                                        order.OutletName,
                                                        externalInvoiceRef, account);


                                                if (inv != null)
                                                {
                                                    var receipt = PagedList.FirstOrDefault(
                                                        p => p.DocumentType == DocumentType.Receipt &&
                                                             p.ExternalReference == order.ExternalReference);
                                                    if (receipt != null)
                                                    {
                                                        var invoiceId = inv.TxnID.GetValue();
                                                        var accountRef = inv.ARAccountRef.ListID.GetValue();

                                                        // var rec = QBIntegrationMethods.AddSaleReceipt(receipt);
                                                        var count = receipt.LineItems.Count;
                                                        var references = new List<string>();

                                                        if (count > 1)
                                                        {
                                                            for (int y = 0; y < count; y++)
                                                            {
                                                                references.Add(GetExternalOrderRef(random));
                                                            }

                                                        }
                                                        var rec = QBIntegrationMethods.AddPayment(receipt, invoiceId, accountRef, references);

                                                    }
                                                }
                                            }

                                        }

                                    }

                                }
                            }
                            //else if (tabItem.Name == "TabReturns")
                            //{
                            //    foreach (var returnItem in ReturnInventoryItemList.Where(n => n.IsSelected))
                            //    {
                            //        ReturnInventoryItem item = returnItem;

                            //        var retItem =
                            //       ReturnsList.FirstOrDefault(
                            //           p =>
                            //           p.GenericReference == returnItem.GenericReference);
                            //        if (retItem != null && ExportAudits.All(p => !p.Equals(retItem.GenericReference)))
                            //        {
                            //            var rec = QBIntegrationMethods.ReturnInventory(retItem);

                            //            if (rec != null)
                            //            {

                            //                ExportAudits.Add(retItem.GenericReference);
                            //            }
                            //        }

                            //    }
                            //}
                            //else if (tabItem.Name == "TabOrders")
                            //{
                            //    foreach (
                            //        var sale in SalesOrdersList.Where(n => n.IsSelected))
                            //    {

                            //        var order =
                            //            PagedList.FirstOrDefault(
                            //                p =>
                            //                p.GenericReference == sale.GenericReference);
                            //        if (order != null && ExportAudits.All(p => !p.Equals(order.GenericReference)))
                            //        {

                            //            string externalOrderRef = order.ExternalReference;
                            //            //GetExternalOrderRef(random);
                            //            var qbOrder = QBIntegrationMethods.AddOrder(order, externalOrderRef);
                            //            if (qbOrder != null)
                            //            {
                            //                ExportAudits.Add(order.GenericReference);
                            //            }
                            //        }
                            //    }

                            //}
                            AcknowledgeAsync();
                            HomeViewModel.GlobalStatus = string.Format("Done..!");
                            if (tabItem.Name == "TabSales" || tabItem.Name == "TabOrders")
                            {
                                LoadClosedSalesOrders();
                            }
                            //else if (tabItem.Name == "TabReturns")
                            //{
                            //    ShowReturns();
                            //}


                            //sessionManager.EndSession();
                            //// boolSessionBegun = false;
                            //sessionManager.CloseConnection();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Quick Books Message", MessageBoxButton.OKCancel,
                                            MessageBoxImage.Information);
                        }

                    }));

        }

        private void ExportAll()
        {
            if (tabItem.Name == "TabSales")
            {
                foreach (var sale in SalesOrdersList)
                {
                    if (sale.OrderType == OrderType.DistributorPOS)
                        sale.IsSelected = true;
                }
            }

            else if (tabItem.Name == "TabReturns")
            {
                foreach (var returnItem in ReturnInventoryItemList)
                {
                    returnItem.IsSelected = true;
                }
            }

            else if (tabItem.Name == "TabOrders")
            {
                foreach (var sale in SalesOrdersList)
                {
                    if (sale.OrderType == OrderType.OutletToDistributor)
                        sale.IsSelected = true;
                }
            }
            ExportSelected();

        }

        private void TabSelectionChanged(SelectionChangedEventArgs e)
        {
            if (e.Source.GetType() != typeof(TabControl))
                return;

            tabItem = e.AddedItems[0] as TabItem;
            if (((TabControl)tabItem.Parent).Name != "tcMainPage")
                return;

            LoadSelectedTab();
            LoadExisting();
        }

        //private void LoadSelectedTab()
        //{

        //    switch (tabItem.Name)
        //    {
        //        case "TabSales":
        //            OrderTypeToLoad = OrderType.DistributorPOS;
        //            LoadClosedSalesOrders();
        //            break;
        //        case "TabOrders":
        //            OrderTypeToLoad = OrderType.OutletToDistributor;
        //            LoadClosedSalesOrders();
        //            break;
        //        //case "TabReturns":
        //        //    ShowReturns();
        //        //    break;
        //    }
        //}

        private void LoadSelectedTab()
        {

            switch (tabItem.Name)
            {
                case "TabSales":
                    OrderTypeToLoad = OrderType.DistributorPOS;
                    LoadSalesOrders();//LoadClosedSalesOrders();
                    break;
                case "TabOrders":
                    OrderTypeToLoad = OrderType.OutletToDistributor;
                    LoadUnclosedOrders();//LoadClosedSalesOrders();
                    break;
                //case "TabReturns":
                //    ShowReturns();
                //    break;
            }
        }
        protected void LoadPage(Page page)
        {
            _currentPage = page;
            Setup();

        }

        string GetExternalOrderRef(Random random)
        {
            return StringUtils.GenerateRandomString(8, 8, random);
        }

        private static SaleOrderListItem MapSelector(QuickBooksOrderDocumentDto n)
        {
            return new SaleOrderListItem()
            {
                DocumentDateIssued = DateTime.Parse(n.DocumentDateIssued),
                GenericReference = n.GenericReference ?? n.ExternalReference,
                ExternalReference = n.ExternalReference ?? n.GenericReference,
                OrderDateRequired = DateTime.Parse(n.OrderDateRequired),
                OutletName = n.OutletName,
                TotalDiscount = n.TotalDiscount,
                TotalGross = n.TotalGross,
                TotalNet = n.TotalNet,
                TotalVAT = n.TotalVat,
                OrderType = n.OrderType
            };
        }
        #endregion
    }
}
