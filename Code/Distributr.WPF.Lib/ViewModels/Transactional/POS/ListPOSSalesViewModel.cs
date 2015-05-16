using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Config;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.ViewModels.Transactional.SalesmanOrders;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Transactional.POS
{
    public class ListPOSSalesViewModel : DistributrViewModelBase
    {
        private List<Guid> UnpaidOrderIds = new List<Guid>();
        private ListSalesmanOrdersViewModel _vm;
        public enum ReportType
        {
            Complete = 1,
            Incomplete = 2,
            BackOrders = 3,
            OutstandingPayment = 4,
            LostSales = 5
        }

        public ListPOSSalesViewModel()
        {
            _vm = new ListSalesmanOrdersViewModel();
            LoadSalesBySearchTextCommand = new RelayCommand(DoLoadBySearchText);
            AddNewSale = new RelayCommand(DoAddNewSale);
            SelectViewerAndGoCommand = new RelayCommand(DoSelectViewerAndGo);
            Sales = new ObservableCollection<POSSaleItem>();
            using (StructureMap.IContainer cont = NestedContainer)
            {
                CanCreateSales = Using<IConfigService>(cont).ViewModelParameters.CurrentUserRights.CanManagePOSSales;
                    //create and edit..
                CanReceivePayments = Using<IConfigService>(cont).ViewModelParameters.CurrentUserRights.CanManagePOSSales;
            }
        }

        #region Properties
        //public PagedCollectionView POSSales { get; set; }
        public RelayCommand AddNewSale { get; set; }
        public RelayCommand LoadSalesBySearchTextCommand { get; set; }
        public RelayCommand SelectViewerAndGoCommand { get; set; }
        public ObservableCollection<POSSaleItem> Sales { get; set; }

        public const string SearchTextPropertyName = "SearchText";
        private string _searchTect = "";
        public string SearchText
        {
            get
            {
                return _searchTect;
            }

            set
            {
                if (_searchTect == value)
                {
                    return;
                }

                var oldValue = _searchTect;
                _searchTect = value;
                RaisePropertyChanged(SearchTextPropertyName);
            }
        }

        public const string PendingSalesPropertyName = "PendingSales";
        private bool _pendingSales = true;
        public bool PendingSales
        {
            get
            {
                return _pendingSales;
            }

            set
            {
                if (_pendingSales == value)
                {
                    return;
                }

                var oldValue = _pendingSales;
                _pendingSales = value;
                RaisePropertyChanged(PendingSalesPropertyName);
            }
        }

        public const string OrderIdLookupPropertyName = "OrderIdLookup";
        private Guid _orderIdLookup = Guid.Empty;
        public Guid OrderIdLookup
        {
            get
            {
                return _orderIdLookup;
            }

            set
            {
                if (_orderIdLookup == value)
                {
                    return;
                }

                var oldValue = _orderIdLookup;
                _orderIdLookup = value;
                RaisePropertyChanged(OrderIdLookupPropertyName);
            }
        }

        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "List of Pending Sales";
        public string PageTitle
        {
            get
            {
                return _pageTitle;
            }

            set
            {
                if (_pageTitle == value)
                {
                    return;
                }

                var oldValue = _pageTitle;
                _pageTitle = value;
                RaisePropertyChanged(PageTitlePropertyName);
            }
        }

        public const string ReportTypePropertyName = "reportType";
        private ReportType _reportType;
        public ReportType reportType
        {
            get
            {
                return _reportType;
            }

            set
            {
                if (_reportType == value)
                {
                    return;
                }

                var oldValue = _reportType;
                _reportType = value;
                RaisePropertyChanged(ReportTypePropertyName);
            }
        }

        public const string CanCreateSalesPropertyName = "CanCreateSales";
        private bool _canCreateSales = false;
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

        public const string SalesCountPropertyName = "SalesCount";
        private int _salesCount = 0;
        public int SalesCount
        {
            get
            {
                return _salesCount;
            }

            set
            {
                if (_salesCount == value)
                {
                    return;
                }

                var oldValue = _salesCount;
                _salesCount = value;
                RaisePropertyChanged(SalesCountPropertyName);
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
        private DateTime _startDate = DateTime.Now.AddDays(-2);
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }

            set
            {
                if (_startDate == value)
                {
                    return;
                }

                var oldValue = _startDate;
                _startDate = value;
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
         
        public const string PageProgressBarPropertyName = "PageProgressBar";
        private string _pageProgressBar = "0";
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
        #endregion

        #region Methods

        public void ClearAndSetup()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                // AppSettings recordsPerPageSetting = Using<ISettingsRepository>(cont).GetByKey(SettingsKeys.RecordsPerPage);
                GeneralSetting recordsPerPageSetting = Using<IGeneralSettingRepository>(cont).GetByKey(GeneralSettingKey.RecordsPerPage);
                ItemsPerPage = recordsPerPageSetting != null ? Convert.ToInt32(recordsPerPageSetting.SettingValue) : 10;
                CurrentPage = 1;
                SearchText = "";
                StartDate = DateTime.Now.AddDays(-2);
                EndDate = DateTime.Now;
                UnpaidOrderIds.Clear();
            }
        }

        public void LoadSales()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
               // List<Order> sales = null;
                PageTitle = "List of Pending Sales";
               // SalesCount = Using<IOrderRepository>(cont).GetSaleCount(StartDate, EndDate);

               // sales = GetSales(sales);

                DumpAndDeploy(GetSales(null));
            }
        }

        public void DoLoadBySearchText()
        {
            //List<Order> sales = null;

           var sales = GetSales(null);

            sales = sales.Where(n =>
                                n.DocumentReference.ToLower().Contains(SearchText.ToLower()) ||
                                n.Status.ToString().ToLower().Contains(SearchText.ToLower()) ||
                                n.DateRequired.ToString().ToLower().Contains(SearchText.ToLower())).ToList();
            DumpAndDeploy(sales);
        }

        //private List<Order> CachedSales;
        private List<Order> GetSales(List<Order> sales)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                switch (reportType)
                {
                    case ReportType.Complete:
                        SalesCount = Using<IOrderRepository>(cont).GetCountByDocumentStatus((int) DocumentStatus.Closed,
                                                                            (int) OrderType.DistributorPOS, StartDate,
                                                                            EndDate);

                        sales = Using<IOrderRepository>(cont).GetByDocumentStatusPagenated(CurrentPage, ItemsPerPage,
                                                                           (int) DocumentStatus.Closed,
                                                                           (int) OrderType.DistributorPOS, StartDate,
                                                                           EndDate, SearchText);
                        //CachedSales = sales;

                        PageCount = (int) Math.Ceiling(Math.Round(((double) SalesCount)/ItemsPerPage, 1));
                        break;
                    case ReportType.Incomplete:
                        SalesCount = Using<IOrderRepository>(cont).GetCountByDocumentStatus((int)DocumentStatus.Closed,
                                                                            false, (int) DocumentStatus.Rejected, false,
                                                                            (int) OrderType.DistributorPOS, StartDate,
                                                                            EndDate);

                        sales = Using<IOrderRepository>(cont).GetByDocumentStatusPagenated(CurrentPage, ItemsPerPage,
                                                                           (int) DocumentStatus.Closed,
                                                                           false, (int) DocumentStatus.Rejected, false,
                                                                           (int) OrderType.DistributorPOS, StartDate,
                                                                           EndDate, SearchText);
                        //CachedSales = CachedSales.Union(sales).ToList();
                        PageCount = (int) Math.Ceiling(Math.Round(((double) SalesCount)/ItemsPerPage, 1));
                        break;
                    case ReportType.BackOrders:
                        SalesCount = Using<IOrderRepository>(cont).GetAll(OrderType.DistributorPOS, StartDate, EndDate)
                                                  .Where(n =>n.LineItems.Any(li => li.LineItemType == OrderLineItemType.BackOrder)).Count();

                        sales = Using<IOrderRepository>(cont).GetAll(OrderType.DistributorPOS, StartDate, EndDate, SearchText.Trim())
                                             .Where(
                                                 n =>
                                                 n.LineItems.Any(li => li.LineItemType == OrderLineItemType.BackOrder))
                                             .ToList();

                        PageCount = (int) Math.Ceiling(Math.Round(((double) SalesCount)/ItemsPerPage, 1));
                        break;
                    case ReportType.LostSales:
                        SalesCount = Using<IOrderRepository>(cont).GetAll(OrderType.DistributorPOS, StartDate, EndDate)
                                                  .Where(n =>n.LineItems.Any(li => li.LineItemType == OrderLineItemType.LostSale)).Count();

                        sales = Using<IOrderRepository>(cont).GetAll(OrderType.DistributorPOS, StartDate, EndDate, SearchText.Trim())
                                             .Where(n =>n.LineItems.Any(li => li.LineItemType == OrderLineItemType.LostSale)).ToList();

                        PageCount = (int) Math.Ceiling(Math.Round(((double) SalesCount)/ItemsPerPage, 1));
                        break;
                    case ReportType.OutstandingPayment:
                        {
                            var unpaidOrders = UpaidOrders();
                            sales = unpaidOrders.OrderByDescending(n => n.DocumentDateIssued).ToList();
                            PageCount = (int) Math.Ceiling(Math.Round(((double) SalesCount)/ItemsPerPage, 1));
                            break;
                        }
                    default:
                        sales = sales
                            .Where(n => (n.OrderType == OrderType.DistributorPOS && n.Status == DocumentStatus.Closed))
                            .ToList();
                        PageCount = (int) Math.Ceiling(Math.Round(((double) SalesCount)/ItemsPerPage, 1));
                        break;
                }
                return sales;
            }
        }

        void DumpAndDeploy(List<Order> sales)
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    delegate
                    {
                if (sales == null) return;
                var salesItems = sales.Select((n, i) =>
                {//cn..
                    InvoicePaymentInfo paymentInfo = null;
                    if (_vm.OrderPaymentInfos != null)
                        paymentInfo = _vm.OrderPaymentInfos.FirstOrDefault(p => p.OrderId == n.Id);

                    var sale = new POSSaleItem
                    {
                        SequenceId = i + ((((CurrentPage - 1) * ItemsPerPage) + 1)),
                        SalesId = n.DocumentReference,
                        TotalVat = n.TotalVat,
                        TotalNet = n.TotalNet,
                        TotalGross = n.TotalGross,
                        Id = n.Id,
                        SalesDate = n.DateRequired,
                        Status = n.Status.ToString(),
                        AmountPaid = paymentInfo != null ? paymentInfo.AmountPaid : 0,
                        InvoiceNo = paymentInfo != null ? paymentInfo.InvoiceId : Guid.Empty,
                        AmountDue = paymentInfo != null ? paymentInfo.AmountDue : 0
                    };
                    return sale;

                });

                Sales.Clear();
                salesItems.ToList().ForEach(s => Sales.Add(s));
            }));
        }

        private List<Order> UpaidOrders()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                List<Order> orders = new List<Order>();
                _vm.LoadAllPaymentInfos(null, StartDate, EndDate);

                var orderIds = _vm.OrderPaymentInfos
                                  .OrderByDescending(n => n.InvoiceDate)
                                  .Where(n => n.AmountDue > 0 && n.OrderId !=Guid.Empty).Select(n => n.OrderId).ToList();

                orderIds.Distinct().ToList().ForEach(UnpaidOrderIds.Add);
                orderIds = orderIds.Distinct().ToList();

                if (SearchText.Trim() == "")
                {
                    orderIds.ForEach(n =>
                                         {
                                             var ord = Using<IOrderRepository>(cont).GetById(n) as Order;
                                             if (ord.OrderType == OrderType.DistributorPOS)
                                                 orders.Add(ord);
                                         });
                    SalesCount = orders.Count;
                    orders = orders.Skip((CurrentPage - 1)*ItemsPerPage).Take(ItemsPerPage).ToList();
                }
                else
                {
                    orders = orderIds
                        .Select(n => Using<IOrderRepository>(cont).GetById(n) as Order )
                        .Where(n => n.DocumentReference.ToLower().Contains(SearchText)).ToList();

                    SalesCount = orders.Count;

                    orders = orders.Skip((CurrentPage - 1)*ItemsPerPage).Take(ItemsPerPage).ToList();
                }

                return orders;
            }
        }

        void DoAddNewSale()
        {
            SendNavigationRequestMessage(new Uri("/views/pos/addpossale.xaml", UriKind.Relative));
        }

        private void DoSelectViewerAndGo()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                //if (PendingSales)
                var loadforviewing = true;
                if (Using<IOrderRepository>(cont).GetById(OrderIdLookup).Status == DocumentStatus.New)
                    loadforviewing = false;
                if (Using<IOrderRepository>(cont).GetById(OrderIdLookup).Status == DocumentStatus.Confirmed)
                    loadforviewing = false;

                if (!loadforviewing)
                    SendNavigationRequestMessage(new Uri("/views/pos/addpossale.xaml?orderid=" + OrderIdLookup,
                                                         UriKind.Relative));
                else
                    SendNavigationRequestMessage(
                        new Uri(
                            "/views/pos/addpossale.xaml?orderid=" + OrderIdLookup + "&loadforviewing=" + loadforviewing,
                            UriKind.Relative));
                //else
                //    SendNavigationRequestMessage(new Uri("/salesmanorder/approvesalemanorders?orderid=" + OrderIdLookup +"&loadforprocessing=" + false, UriKind.Relative));
            }
        }

        #endregion
    }

    #region Helper Classes
    public class POSSaleItem : ViewModelBase
    {
        public int SequenceId { get; set; }
        public Guid Id { get; set; }
        public string SalesId { get; set; } //OrderId
        public DateTime SalesDate { get; set; } //date of order approval
        public Decimal TotalNet { get; set; } //Net amount
        public Decimal TotalVat { get; set; }
        public Decimal TotalGross { get; set; } //Amount paid //TODO: Diff: Amount paid against the gross amount
        public string Status { get; set; }
        public Decimal AmountPaid { get; set; }
        public Guid InvoiceNo { get; set; }
        public bool IsEditable { get { return (Status == "New" || Status == "Confirmed"); } }
        public Decimal AmountDue { get; set; }
    }
    #endregion
}