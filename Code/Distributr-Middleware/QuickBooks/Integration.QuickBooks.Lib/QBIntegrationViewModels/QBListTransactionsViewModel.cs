﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr.Core.Utility;
using Distributr.WSAPI.Lib.Integrations;
using Distributr_Middleware.WPF.Lib.MiddlewareServices;
using Distributr_Middleware.WPF.Lib.Utils;
using Distributr_Middleware.WPF.Lib.ViewModels;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Integration.QuickBooks.Lib.QBIntegrationCore;
using Integration.QuickBooks.Lib.Services;
using Integration.QuickBooks.Lib.Services.Impl;
using Newtonsoft.Json;
using QBFC13Lib;
using Xceed.Wpf.Toolkit;
using ITransactionsDownloadService = Integration.QuickBooks.Lib.Services.ITransactionsDownloadService;
using MessageBox = System.Windows.MessageBox;
using System.ComponentModel;

namespace Integration.QuickBooks.Lib.QBIntegrationViewModels
{
    public class QBListTransactionsViewModel : MiddleWareViewModelBase
    {

        internal List<QuickBooksOrderDocumentDto> PagedList;
        internal List<QuickBooksOrderDocumentDto> UnClosedOrdersList;
        internal List<QuickBooksReturnInventoryDocumentDto> ReturnsList; 
        private Page _currentPage;
        private TabItem tabItem;
        internal List<string> NewOutlets;
        internal List<string> NewProducts;
        internal List<string> ExportAudits; 

        public QBListTransactionsViewModel()
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

       
        public ObservableCollection<QBAccount> AssetAccountList { get; set; }
        public ObservableCollection<QBAccount> IncomeAccountList { get; set; }
        public ObservableCollection<QBAccount> COGSAccountList { get; set; }
        public ObservableCollection<QBAccount> TradeReceivableAccountList { get; set; }

        public ObservableCollection<SaleOrderListItem> SalesOrdersList { get; set; }

        public ObservableCollection<ReturnInventoryItem> ReturnInventoryItemList { get; set; }

        internal OrderType OrderTypeToLoad;

        public RelayCommand ExportSelectedCommand { get; set; }
        public RelayCommand ExportAllCommand { get; set; }



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
        #endregion

        #region methods
    

        private void TabSelectionChanged(SelectionChangedEventArgs e)
        {
            if (e.Source.GetType() != typeof(TabControl))
                return;

            tabItem = e.AddedItems[0] as TabItem;
            if (((TabControl)tabItem.Parent).Name != "tcMainPage")
                return;

            LoadSelectedTab();
        }

        private void LoadSelectedTab()
        {
            
            switch (tabItem.Name)
            {
                case "TabSales":
                    OrderTypeToLoad = OrderType.DistributorPOS;
                    LoadClosedSalesOrders();
                    break;
                case "TabOrders":
                    OrderTypeToLoad = OrderType.OutletToDistributor;
                    LoadClosedSalesOrders();
                    break;
                case "TabReturns":
                    ShowReturns();
                    break;
            }
            
            
        }

        private void ShowReturns()
        {
            ReturnInventoryItemList.Clear();
            var data = ReturnsList.ToList();

            foreach (var returnNote in data)
            {
                  var returnInventory = new ReturnInventoryItem();
                    returnInventory.DocumentDateIssued =returnNote.DateOfIssue;
                    returnInventory.SalesmanName = returnNote.SalesmanName;
                    returnInventory.Quantity = returnNote.LineItems.Count;
                    returnInventory.GenericReference = returnNote.GenericReference;

                
                    ReturnInventoryItemList.Add(returnInventory);
               
            }
        }

        protected  void LoadPage(Page page)
        {
            _currentPage = page;
            Setup();
           
        }

        public  void LoadClosedSalesOrders()
        {
            if (PagedList.Any())
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                {
                    {
                        ShowBusy(true);
                        HomeViewModel.GlobalStatus =string.Format("Loading....");
                    }
                }));
                LoadExisting();
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
            }
        }

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

        async void DownloadFromHq()
        {
           Task.Run(() =>
                         {
                             HomeViewModel.GlobalStatus =
                                 string.Format("Please wait downloading data..from HQ!");
                            var response =TransactionsDownloadService.DownloadAllAsync(SearchText,IncludeReceiptsAndInvoice);
                             var newItes = new List<QuickBooksOrderDocumentDto>();
                             if (response != null)
                             {
                                 if (response.Result == "Error")
                                 {
                                     FileUtility.LogError(response.ResultInfo);
                                     MessageBox.Show(response.ErrorInfo, "Server Response",
                                                     MessageBoxButton.OK);
                                 }
                                 if (!string.IsNullOrEmpty(response.TransactionData))
                                 {
                                     newItes = JsonConvert.DeserializeObject
                                         <List<QuickBooksOrderDocumentDto>>(
                                             response.TransactionData);
                                     HomeViewModel.GlobalStatus =
                                         string.Format("downloaded {0} items!", newItes.Count);

                                 }
                             }

                             
                             foreach (var item in newItes)
                             {
                                 if (PagedList.All(p => p.GenericReference != item.GenericReference))
                                     PagedList.Add(item);
                             }
                             if (newItes.Any())
                             {
                                 LoadExisting();
                             }
                             var UnClosedOrders = TransactionsDownloadService.DownloadAllAsync(SearchText,
                                                                                               IncludeReceiptsAndInvoice,
                                                                                               DocumentStatus.Confirmed);

                             var unClosedNew = new List<QuickBooksOrderDocumentDto>();
                             if (UnClosedOrders != null)
                             {
                                 if (UnClosedOrders.Result == "Error")
                                 {
                                     FileUtility.LogError(UnClosedOrders.ResultInfo);
                                     MessageBox.Show(UnClosedOrders.ErrorInfo, "Server Response",
                                                     MessageBoxButton.OK);
                                 }
                                 if (!string.IsNullOrEmpty(UnClosedOrders.TransactionData))
                                 {
                                     unClosedNew = JsonConvert.DeserializeObject
                                         <List<QuickBooksOrderDocumentDto>>(
                                             UnClosedOrders.TransactionData);
                                     HomeViewModel.GlobalStatus =
                                         string.Format("downloaded {0} items!", unClosedNew.Count);

                                 }
                             }


                             foreach (var item in unClosedNew)
                             {
                                 //if (UnClosedOrdersList.All(p => p.GenericReference != item.GenericReference))
                                 //    UnClosedOrdersList.Add(item);

                                 if (PagedList.All(p => p.GenericReference != item.GenericReference))
                                     PagedList.Add(item);
                             }
                             if (unClosedNew.Any())
                             {
                                 LoadExisting();
                             }

                             var inventoryReturnsResponse = TransactionsDownloadService.DownloadReturnsAsync(SearchText);
                             var newReturnInventory = new List<QuickBooksReturnInventoryDocumentDto>();

                             if (inventoryReturnsResponse != null)
                             {
                                 if (inventoryReturnsResponse.Result == "Error")
                                 {
                                     FileUtility.LogError(inventoryReturnsResponse.ResultInfo);
                                     MessageBox.Show(response.ErrorInfo, "Server Response",
                                                     MessageBoxButton.OK);
                                 }
                                 if (!string.IsNullOrEmpty(inventoryReturnsResponse.TransactionData))
                                 {
                                     newReturnInventory = JsonConvert.DeserializeObject
                                         <List<QuickBooksReturnInventoryDocumentDto>>(
                                             inventoryReturnsResponse.TransactionData);
                                     HomeViewModel.GlobalStatus =
                                         string.Format("downloaded {0} items!", newItes.Count);

                                 }
                             }

                             foreach (var item in newReturnInventory)
                             {
                                // ReturnsList.Clear();
                                     //ReturnsList.All(
                                     //    p => p.GenericReference != item.GenericReference);
                                     ReturnsList.Add(item);
                             }
                             if (newReturnInventory.Any())
                             {
                                 LoadExisting();
                             }
                           
                         });
           // task.Wait(TimeSpan.FromMinutes(10).Seconds);

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
        /// <summary>
        /// Downloads outlets and products from HQ to quickbooks..this approach is being depreciated,instead
        /// all masterdata MUST explicitly come from QB and only transactions go to QB
        /// </summary>
        [Obsolete("All masterdata MUST explicitly come from QB and only transactions go to QB")]
        void SyncWithQB()
        {
            if (!PagedList.Any())return;
            Dispatcher.CurrentDispatcher.Invoke(() =>
                                                    {
                                                        HomeViewModel.GlobalStatus =
                                                            string.Format("Updating new outlets and products");
                                                       
                                                        
                                                    });
            var t1 = Task.Factory.StartNew(() => GetNewOutlets(PagedList.Select(p => p.OutletName).Distinct()));
            var t2 = Task.Factory.StartNew(() =>
                                               {
                                                   var productCodes =
                                                       PagedList.SelectMany(p => p.LineItems.Select(q => q.ProductCode).
                                                                                     ToList()).ToList();
                GetNewProducts(productCodes.Distinct());
                HomeViewModel.GlobalStatus = "Ready..!";
            });
            var tasks = new[] {t1, t2};
            Task.Factory.ContinueWhenAll(tasks.ToArray(),result => ShowBusy(false));
        }

        private void Setup()
        {
            LoadAccountsLists();
            
        }
       
       private async void ExportSelected()
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    delegate
                        {
                            try
                            {
                                string _appName = "Distributr QB Intergration";
                                string qdbpath = ConfigurationSettings.AppSettings["QuickBooksCompanyFilePath"];

                                QBSessionManager sessionManager = new QBSessionManager();
                                sessionManager.OpenConnection("", _appName);
                                sessionManager.BeginSession(qdbpath, ENOpenMode.omDontCare);

                              HomeViewModel.GlobalStatus =
                                    string.Format("Exporting {0} sales to Quick Books ...",
                                        SalesOrdersList.Count(n => n.IsSelected));
                                int cnt = 0;
                                Random random = new Random();
                                if (tabItem.Name=="TabSales")
                                {
                                   foreach (
                                        var sale in SalesOrdersList.Where(n => n.IsSelected))
                                    {

                                        var order =
                                            PagedList.FirstOrDefault(
                                                p =>
                                                p.GenericReference == sale.GenericReference);
                                        if (order != null && ExportAudits.All(p=>!p.Equals(order.GenericReference)))
                                        {

                                            string externalOrderRef = order.ExternalReference;
                                                //GetExternalOrderRef(random);
                                            var qbOrder = QBIntegrationMethods.AddOrder(
                                                order,externalOrderRef);

                                            if (qbOrder != null)
                                            {
                                            
                                                ExportAudits.Add(order.GenericReference);
                                                var quickBooksId = qbOrder.TxnID.GetValue();

                                                string externalInvoiceRef =
                                                    GetExternalOrderRef(random);
                                                var invoice =
                                                    PagedList.FirstOrDefault(
                                                        p =>
                                                        p.DocumentType ==
                                                        DocumentType.Invoice &&
                                                        p.ExternalReference ==
                                                        order.ExternalReference);
                                                if (invoice != null)
                                                {
                                                    var account=SelectedTradeReceivableAccount.QBAccountId.ToString(CultureInfo.InvariantCulture);

                                                    var inv=
                                                        QBIntegrationMethods.AddInvoice(
                                                            invoice, quickBooksId,
                                                            order.OutletName,
                                                            externalInvoiceRef, account);
                                               
                                              
                                                    if(inv!=null)
                                                    {
                                                        var receipt = PagedList.FirstOrDefault(
                                                            p => p.DocumentType == DocumentType.Receipt &&
                                                                 p.ExternalReference == order.ExternalReference);
                                                        if(receipt!=null)
                                                        {
                                                            var invoiceId = inv.TxnID.GetValue();
                                                            var accountRef = inv.ARAccountRef.ListID.GetValue();
                                                       
                                                                // var rec = QBIntegrationMethods.AddSaleReceipt(receipt);
                                                            var count = receipt.LineItems.Count;
                                                            var references = new List<string>();

                                                            if(count>1)
                                                            {
                                                                for(int y=0;y<count;y++)
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
                                else if (tabItem.Name == "TabReturns")
                                {
                                    foreach (var returnItem in ReturnInventoryItemList.Where(n => n.IsSelected))
                                        {
                                            ReturnInventoryItem item = returnItem;
                                           
                                             var retItem =
                                            ReturnsList.FirstOrDefault(
                                                p =>
                                                p.GenericReference == returnItem.GenericReference);
                                             if (retItem != null && ExportAudits.All(p => !p.Equals(retItem.GenericReference)))
                                             {
                                                 var rec = QBIntegrationMethods.ReturnInventory(retItem);

                                                 if (rec != null)
                                                 {

                                                     ExportAudits.Add(retItem.GenericReference);
                                                 }
                                             }
                                            
                                        }
                                }
                                else if (tabItem.Name == "TabOrders")
                                {
                                    foreach (
                                        var sale in SalesOrdersList.Where(n => n.IsSelected))
                                    {

                                        var order =
                                            PagedList.FirstOrDefault(
                                                p =>
                                                p.GenericReference == sale.GenericReference);
                                        if (order != null && ExportAudits.All(p => !p.Equals(order.GenericReference)))
                                        {

                                            string externalOrderRef = order.ExternalReference;
                                            //GetExternalOrderRef(random);
                                            var qbOrder = QBIntegrationMethods.AddOrder(order, externalOrderRef);
                                            if (qbOrder!=null)
                                            {
                                                ExportAudits.Add(order.GenericReference);
                                            }
                                        }
                                    }

                                }
                                AcknowledgeAsync();
                                HomeViewModel.GlobalStatus =string.Format("Done..!");
                                if (tabItem.Name == "TabSales" || tabItem.Name == "TabOrders")
                                {
                                    LoadClosedSalesOrders();
                                }
                                else if (tabItem.Name == "TabReturns")
                                {
                                    ShowReturns();
                                }


                                sessionManager.EndSession();
                               // boolSessionBegun = false;
                                sessionManager.CloseConnection();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Quick Books Message", MessageBoxButton.OKCancel,
                                                MessageBoxImage.Information);
                            }
                            
                        }));

        }

      
        private async void AcknowledgeAsync()
        {
            
            if(!ExportAudits.Any())return;
            var service = new TransactionsDownloadService();
            HomeViewModel.GlobalStatus =
                                 string.Format("Pushing {0} sacknowledgments to HQ", ExportAudits.Count);
            var done =
                await
                service.AcknowledgeDocuments(ExportAudits);
            if (done)
            {
                HomeViewModel.GlobalStatus ="Done..";
                Application.Current.Dispatcher.BeginInvoke(
                 new Action(
                     delegate
                         {
                             
                         foreach (var orderRef in ExportAudits)
                         {
                            var found= SalesOrdersList.FirstOrDefault(p => p.GenericReference == orderRef);
                             var order = PagedList.FirstOrDefault(p => p.GenericReference == orderRef);
                             if(found !=null)
                             {
                                 SalesOrdersList.Remove(found);
                                 PagedList.Remove(order);
                                 LoadClosedSalesOrders();
                             }
                         }

                         foreach (var itemRef in ExportAudits)
                         {
                             var found = ReturnInventoryItemList.FirstOrDefault(p => p.GenericReference == itemRef);
                             var documentItem = ReturnsList.FirstOrDefault(p => p.GenericReference == itemRef);
                             if (found != null)
                             {
                                 ReturnsList.Remove(documentItem);
                                 ReturnInventoryItemList.Remove(found);
                             }
                         }

                     }));
                
            }
           
        }

        private void ExportAll()
        {
            if (tabItem.Name == "TabSales")
            {
                foreach (var sale in SalesOrdersList)
                {
                    if(sale.OrderType==OrderType.DistributorPOS)
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


        string GetExternalOrderRef(Random random)
        {
            return StringUtils.GenerateRandomString(8, 8, random);
        }

       


        #endregion

        #region Base
        #region properties


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

        #region methods

        private void GetNewOutlets(IEnumerable<string> outletNames)
        {
            NewOutlets= outletNames.Where(outletname => !QBIntegrationMethods.CustomerExist(outletname)).ToList();
            HomeViewModel.CanSyncmasterData = NewOutlets.Any();
        }

        private void GetNewProducts(IEnumerable<string> productCodes)
        {
            NewProducts = productCodes.Where(product => !QBIntegrationMethods.ProductExist(product)).ToList();
            HomeViewModel.CanSyncmasterData = NewProducts.Any();
        }
        [Obsolete("This method's usage has been discontinued")]
        protected override async void Sync()
        {
            try
            {
                await ProcessMasterDataForOrderToUpload();
            }
                catch(AggregateException ex)
                {
                    
                }
            catch (Exception e)
            {

                MessageBox.Show("Error:" + e.Message);
                FileUtility.LogError(e.Message);
            }
           
        }

        
       async Task ProcessMasterDataForOrderToUpload( )
        {
            HomeViewModel.GlobalStatus = "Downloading new products and Outlets...";
            if ((SelectedAssetAccount == null || SelectedIncomeAccount == null || SelectedCOGSAccount==null) || SelectedAssetAccount.AccountName.Equals("--Select Account--") ||
                    SelectedIncomeAccount.AccountName.Equals("--Select Account--") ||
                    SelectedCOGSAccount.AccountName.Equals("--Select Account--"))
                {
                    MessageBox.Show("Select valid account from the drop down lists.");
                    return;
                }
           IMasterDataExportService _service =new QBMasterDataExportService();
           IEnumerable<ImportEntity> outlets=new List<ImportEntity>();
           IEnumerable<ImportEntity> products=new List<ImportEntity>();
           if (NewProducts.Any())
           {
               products =await
                   _service.DownloadMasterdata(MasterDataCollective.SaleProduct,
                                               NewProducts.Where(p => !string.IsNullOrEmpty(p)).
                                                   ToList());
           }
           if (NewOutlets.Any())
           {
               outlets = await _service.DownloadMasterdata(MasterDataCollective.Outlet,NewOutlets.Where(p =>!string.IsNullOrEmpty(p)).ToList());

           }
           Application.Current.Dispatcher.BeginInvoke(
               new Action(
                   delegate
                       {
                           if (products.Any() || outlets.Any())
                           {
                               HomeViewModel.GlobalStatus =
                                   string.Format(
                                       "Downloaded {0} new products and {1} Outlets...",
                                       products.Count(), outlets.Count());
                               HomeViewModel.GlobalStatus =
                                   string.Format("Updating quickbooks...");

                               var outletsAdded = new List<bool>();
                               foreach (var outlet in outlets)
                               {
                                   if (QBIntegrationMethods.AddOutlet(outlet))
                                       outletsAdded.Add(true);
                               }
                               HomeViewModel.GlobalStatus =
                                   string.Format("Updated {0} Outlets in quickbooks...",
                                                 outletsAdded.Count);
                               var productsAdded = new List<bool>();
                               foreach (var product in products)
                               {
                                   if (QBIntegrationMethods.AddProduct(product,
                                                                       SelectedCOGSAccount
                                                                           .AccountName,
                                                                       SelectedAssetAccount
                                                                           .AccountName,
                                                                       SelectedIncomeAccount
                                                                           .AccountName))
                                       productsAdded.Add(true);
                               }
                               HomeViewModel.GlobalStatus =
                                   string.Format("Updated {0} Product in quickbooks...",
                                                 productsAdded.Count);
                           }
                           else
                           {
                               HomeViewModel.GlobalStatus =
                                   string.Format("No data downloaded...");
                                                
                           }
                       }));

        }

        private async void LoadAccountsLists()
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
                                LoadClosedSalesOrders();
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

        #endregion
        #endregion

       

    }
    public class SaleOrderListItem
    {
        public bool IsSelected { get; set; }
        public string GenericReference { get; set; }
        public string ExternalReference { get; set; }
        public string OutletName { get; set; }
        public DateTime OrderDateRequired { get; set; }
        public DateTime DocumentDateIssued { get; set; }
        public decimal TotalVAT { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalNet { get; set; }
        public OrderType OrderType { get; set; }
    }

    public  class  ReturnInventoryItem
    {
        public bool IsSelected { get; set; }
        public Guid LineItemId { get; set; }
        public string SalesmanName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public DateTime OrderDateRequired { get; set; }
        public DateTime DocumentDateIssued { get; set; }
        public int Quantity { get; set; }
        public string GenericReference { get; set; }
    }
   

}