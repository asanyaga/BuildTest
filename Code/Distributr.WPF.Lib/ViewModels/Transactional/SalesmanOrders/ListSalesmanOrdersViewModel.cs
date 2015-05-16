
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.CreditNoteRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.DispatchRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Config;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.WorkFlow.Orders;
using Distributr.WPF.Lib.WorkFlow.Orders;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;

namespace Distributr.WPF.Lib.ViewModels.Transactional.SalesmanOrders
{
    public class ListSalesmanOrdersViewModel : DistributrViewModelBase
    {
        private List<CostCentre> _allCostCentres = null;
       
        private List<User> _allSalesmen = null;
        private Guid CostCentreId = Guid.Empty;

        public ListSalesmanOrdersViewModel()
        {
            
            _allCostCentres = new List<CostCentre>();
            _allSalesmen = new List<User>();
          
            AddSalesmanOrder = new RelayCommand(RunAddOrders);
            LoadPendingOrdersCommand = new RelayCommand(RunLoadPendingOrdersCommand);
            LoadOrdersCommand = new RelayCommand(LoadOrders);
            LoadOrderStatusCommand = new RelayCommand(LoadOrderStatus);
            LoadOrderTypesCommand = new RelayCommand(LoadOrderTypes);
            SetupForDispatchCommand = new RelayCommand(RunSetupForDispatch);
            DispatchCommand = new RelayCommand(RunDispatchCommandRevised);
            LoadSalesmansPendingOrdersCommand = new RelayCommand(RunLoadSalesmansPendingOrder);
            LoadSalesmansPendingOrdersForDispatchCommand = new RelayCommand(RunLoadOrderForDispatch);

            SetRecipientCommand = new RelayCommand(RunSetRecipientCommand);
            UnsetRecipientCommand = new RelayCommand(RunUnsetRecipientCommand);
            RouteChangedCommand = new RelayCommand(RunRouteChangedCommand);
            SelectViewerAndGoCommand = new RelayCommand(RunSelectViewerAndGo);
            SelectAllCommand = new RelayCommand(RunSelectAllCommand);
            UnSelectAllCommand = new RelayCommand(RunUnSelectAllCommand);
            Routes = new ObservableCollection<DistributorRoute>();
            Salesmen = new ObservableCollection<Salesman>();
            try
            {
                using (var container = NestedContainer)
                {
                    IConfigService _configService = Using<IConfigService>(container);
                    CanCreateOrders = _configService.ViewModelParameters.CurrentUserRights.CanCreateOrder;
                    CanDispatchOrders = _configService.ViewModelParameters.CurrentUserRights.CanDispatchOrder;
                    CanReceivePayments = _configService.ViewModelParameters.CurrentUserRights.CanCreateOrder;
                    CanApproveOrder = _configService.ViewModelParameters.CurrentUserRights.CanApproveOrders;
                }
            }catch
            {
                //silence _configService.ViewModelParameters NullReference when not logged in.
            }

            DeliveredOrdersCache = new List<Order>();
            PartiallyPaidDeliveriesCache = new List<Order>();
            OrdersPendingDispatchCache = new List<Order>();
            FullyPaidDeliveriesCache = new List<Order>();
            BackOrdersCache = new List<Order>();
            LostSalesCache = new List<Order>();
            Orders = new ObservableCollection<ListOrderViewModelItem>();
            Report = new ObservableCollection<ReportsListViewModel>();
            OrderedSalesmen = new List<Salesman>();
            
        }

        #region Enums
        public enum EnumOrdersToLoad
        {
            PendingApproval = 1,
            PendingDispatch = 2,
            DispatchedOrders = 3,
            Incomplete = 4,
            Delivered = 5,
            PartiallyPaidDeliveries = 6,
            FullyPaidDeliveries = 7,
            BackOrders = 8,
            Rejected = 9,
            LostSales = 10
        }


        public enum EnumSelectedDispatchMode
        {
            DispatchFulfilled = 1,
            ProcessFulfillableAndDispatchFulfilled = 2,
            ProcessAndDispatchAll = 3,
            DispatchWithPartialDispatch = 4,
        }
        #endregion
        
        #region Properties

        #region RelayCommands And Lists
        public RelayCommand LoadPendingOrdersCommand { get; set; } //all orders
        public RelayCommand AddSalesmanOrder { get; set; }
        public RelayCommand LoadOrdersCommand { get; set; }// Load Orders Based On Selected Order Status & Date
        public RelayCommand LoadOrderStatusCommand { get; set; }
        public RelayCommand LoadOrderTypesCommand { get; set; }
        public ObservableCollection<ListOrderViewModelItem> Orders { get; set; }
        public ObservableCollection<ReportsListViewModel> Report { get; set; }
         public ObservableCollection<Salesman> Salesmen { get; set; }
        public ObservableCollection<DistributorRoute> Routes { get; set; }
        public ObservableCollection<DocumentStatus> OrderStatusItems { get; set; }
        public RelayCommand SetupForDispatchCommand { get; set; }
        public RelayCommand DispatchCommand { get; set; }
        public RelayCommand LoadSalesmansPendingOrdersCommand { get; set; }
        public RelayCommand LoadSalesmansPendingOrdersForDispatchCommand { get; set; }
        public RelayCommand SetRecipientCommand { get; set; }
        public RelayCommand UnsetRecipientCommand { get; set; }
        public RelayCommand RouteChangedCommand { get; set; }
        public RelayCommand SelectViewerAndGoCommand { get; set; }
        public RelayCommand OrderIsSelectedCommand { get; set; }
        public RelayCommand SelectAllCommand { get; set; }
        public RelayCommand UnSelectAllCommand { get; set; }
        public RelayCommand ValidateSelectedOrderForDispatch { get; set; }
        public ObservableCollection<OrderType> OrderTypes { get; set; }
        public List<ListOrderViewModelItem> FulfillableOrdersWithBackOrder { get; set; }
        public List<ListOrderViewModelItem> FulfilledOrders { get; set; }
        public List<ListOrderViewModelItem> UnFulfillableOrdersWithBackOrder { get; set; }
        public List<ListOrderViewModelItem> OrdersWithBackOrder { get; set; }
        public List<ListOrderViewModelItem> PartiallyDispatchableOrders { get; set; }
        public EnumOrdersToLoad OrdersToLoad { get; set; }
        #endregion

        #region mvvminpc

        public const string ProgressBarValuePropertyName = "ProgressBarValue";
        private int _progresbar = 0;
        public int ProgressBarValue
        {
            get
            {
                return _progresbar;
            }

            set
            {
                if (_progresbar == value)
                {
                    return;
                }

                var oldValue = _progresbar;
                _progresbar = value;



                // Update bindings, no broadcast
                RaisePropertyChanged(ProgressBarValuePropertyName);

            }
        }

        public const string SearchTextPropertyName = "SearchText";
        private string _searchText = "";
        public string SearchText
        {
            get
            {
                return _searchText.ToLower();
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

        public const string StartDatePropertyName = "StartDate";
        private DateTime _StartDate = DateTime.Now.AddDays(-2);
        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }

            set
            {
                if (_StartDate == value)
                {
                    return;
                }

                var oldValue = _StartDate;
                _StartDate = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(StartDatePropertyName);
            }
        }

        public const string EndDatePropertyName = "EndDate";
        private DateTime _EndDate = DateTime.Now;
        public DateTime EndDate
        {
            get
            {
                return _EndDate;
            }

            set
            {
                if (_EndDate == value)
                {
                    return;
                }

                var oldValue = _EndDate;
                _EndDate = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(EndDatePropertyName);
            }
        }

        public const string OrderStatusPropertyName = "OrderStatus";
        private DocumentStatus _OrderStatus = DocumentStatus.New;
        public DocumentStatus OrderStatus
        {
            get
            {
                return _OrderStatus;
            }

            set
            {
                if (_OrderStatus == value)
                {
                    return;
                }

                var oldValue = _OrderStatus;
                _OrderStatus = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(OrderStatusPropertyName);
            }
        }

        public const string OrdertypePropertyName = "Ordertype";
        private OrderType _Ordertype = OrderType.OutletToDistributor;
        public OrderType Ordertype
        {
            get
            {
                return _Ordertype;
            }

            set
            {
                if (_Ordertype == value)
                {
                    return;
                }

                var oldValue = _Ordertype;
                _Ordertype = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(OrdertypePropertyName);
            }
        }

        public const string SelectedSalesmanPropertyName = "SelectedSalesman";
        private Salesman _selectedSalesman = null;
        public Salesman SelectedSalesman
        {
            get
            {
                return _selectedSalesman;
            }

            set
            {
                if (_selectedSalesman == value)
                {
                    return;
                }

                var oldValue = _selectedSalesman;
                _selectedSalesman = value;
                RaisePropertyChanged(SelectedSalesmanPropertyName);
            }
        }

        public const string ForDispatchPropertyName = "ForDispatch";
        private bool _forDispatch = false;
        public bool ForDispatch
        {
            get
            {
                return _forDispatch;
            }

            set
            {
                if (_forDispatch == value)
                {
                    return;
                }

                var oldValue = _forDispatch;
                _forDispatch = value;
                RaisePropertyChanged(ForDispatchPropertyName);
            }
        }

        public const string ClosedPropertyName = "Closed";
        private bool _closed = false;
        public bool Closed
        {
            get
            {
                return _closed;
            }

            set
            {
                if (_closed == value)
                {
                    return;
                }

                var oldValue = _closed;
                _closed = value;
                RaisePropertyChanged(ClosedPropertyName);
            }
        }

        public const string AwaitingStockPropertyName = "AwaitingStock";
        private bool _awaitingStock = false;
        public bool AwaitingStock
        {
            get
            {
                return _awaitingStock;
            }

            set
            {
                if (_awaitingStock == value)
                {
                    return;
                }

                var oldValue = _awaitingStock;
                _awaitingStock = value;
                RaisePropertyChanged(AwaitingStockPropertyName);
            }
        }

        public const string BackOrdersPropertyName = "BackOrders";
        private bool _backOrders = false;
        public bool BackOrders
        {
            get
            {
                return _backOrders;
            }

            set
            {
                if (_backOrders == value)
                {
                    return;
                }

                var oldValue = _backOrders;
                _backOrders = value;
                RaisePropertyChanged(BackOrdersPropertyName);
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

        public const string AssignOverallRecipientPropertyName = "AssignOverallRecipient";
        private bool _assignOverallRecipient = false;
        public bool AssignOverallRecipient
        {
            get
            {
                return _assignOverallRecipient;
            }

            set
            {
                if (_assignOverallRecipient == value)
                {
                    return;
                }

                var oldValue = _assignOverallRecipient;
                _assignOverallRecipient = value;
                RaisePropertyChanged(AssignOverallRecipientPropertyName);
            }
        }

        public const string OverallRecipientPropertyName = "OverallRecipient";
        private Salesman _overallRecipient = null;
        public Salesman OverallRecipient
        {
            get
            {
                return _overallRecipient;
            }

            set
            {
                if (_overallRecipient == value)
                {
                    return;
                }

                var oldValue = _overallRecipient;
                _overallRecipient = value;
                RaisePropertyChanged(OverallRecipientPropertyName);
            }
        }

        public const string SelectedRoutePropertyName = "SelectedRoute";
        private DistributorRoute _selectedRoute = null;
        public DistributorRoute SelectedRoute
        {
            get
            {
                return _selectedRoute;
            }

            set
            {
                if (_selectedRoute == value)
                {
                    return;
                }

                var oldValue = _selectedRoute;
                _selectedRoute = value;
                RaisePropertyChanged(SelectedRoutePropertyName);
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

        public const string OrderIsFulfillablePropertyName = "OrderIsFulfillable";
        private bool _orderIsFulfillable = false;
        public bool OrderIsFulfillable
        {
            get
            {
                return _orderIsFulfillable;
            }

            set
            {
                if (_orderIsFulfillable == value)
                {
                    return;
                }

                var oldValue = _orderIsFulfillable;
                _orderIsFulfillable = value;
                RaisePropertyChanged(OrderIsFulfillablePropertyName);
            }
        }

        public const string DispatchAnywayPropertyName = "DispatchAnyway";
        private bool _dispatchAnyway = false;
        public bool DispatchAnyway
        {
            get
            {
                return _dispatchAnyway;
            }

            set
            {
                if (_dispatchAnyway == value)
                {
                    return;
                }

                var oldValue = _dispatchAnyway;
                _dispatchAnyway = value;
                RaisePropertyChanged(DispatchAnywayPropertyName);
            }
        }

        public const string ProcessAndDispatchPropertyName = "ProcessAndDispatch";
        private bool _processAndDispatch = false;
        public bool ProcessAndDispatch
        {
            get
            {
                return _processAndDispatch;
            }

            set
            {
                if (_processAndDispatch == value)
                {
                    return;
                }

                var oldValue = _processAndDispatch;
                _processAndDispatch = value;
                RaisePropertyChanged(ProcessAndDispatchPropertyName);
            }
        }

        public const string ProcessBackOrderPropertyName = "ProcessBackOrder";
        private bool _processBackOrder = false;
        public bool ProcessBackOrder
        {
            get
            {
                return _processBackOrder;
            }

            set
            {
                if (_processBackOrder == value)
                {
                    return;
                }

                var oldValue = _processBackOrder;
                _processBackOrder = value;
                RaisePropertyChanged(ProcessBackOrderPropertyName);
            }
        }

        public const string PageProgressBarPropertyName = "PageProgressBar";
        private string  _pageProgressBar = "";
        public string  PageProgressBar
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
         
        public const string ProgressPropertyName = "Progress";
        private int _progress = 0;
        public int Progress
        {
            get
            {
                return _progress;
            }

            set
            {
                if (_progress == value)
                {
                    return;
                }

                var oldValue = _progress;
                _progress = value;
                RaisePropertyChanged(ProgressPropertyName);
            }
        }

        public const string ReadyToLoadPropertyName = "ReadyToLoad";
        private bool _readyToLoad = false;
        public bool ReadyToLoad
        {
            get
            {
                return _readyToLoad;
            }

            set
            {
                if (_readyToLoad == value)
                {
                    return;
                }

                var oldValue = _readyToLoad;
                _readyToLoad = value;
                RaisePropertyChanged(ReadyToLoadPropertyName);
            }
        }

        public const string SelectedDispatchModePropertyName = "SelectedDispatchMode";
        private EnumSelectedDispatchMode _selectedDispatchMode = EnumSelectedDispatchMode.DispatchFulfilled;
        public EnumSelectedDispatchMode SelectedDispatchMode
        {
            get
            {
                return _selectedDispatchMode;
            }

            set
            {
                if (_selectedDispatchMode == value)
                {
                    return;
                }

                var oldValue = _selectedDispatchMode;
                _selectedDispatchMode = value;
                RaisePropertyChanged(SelectedDispatchModePropertyName);
            }
        }

        public const string ShowProgressBarPropertyName = "ShowProgressBar";
        private bool _showProgressBar = false;
        public bool ShowProgressBar
        {
            get
            {
                return _showProgressBar;
            }

            set
            {
                if (_showProgressBar == value)
                {
                    return;
                }

                var oldValue = _showProgressBar;
                _showProgressBar = value;
                RaisePropertyChanged(ShowProgressBarPropertyName);
            }
        }

        public const string CanCreateOrdersPropertyName = "CanCreateOrders";
        private bool _canCreateOrders = false;
        public bool CanCreateOrders
        {
            get
            {
                return _canCreateOrders;
            }

            set
            {
                if (_canCreateOrders == value)
                {
                    return;
                }

                var oldValue = _canCreateOrders;
                _canCreateOrders = value;
                RaisePropertyChanged(CanCreateOrdersPropertyName);
            }
        }

        public const string CanDispatchOrdersPropertyName = "CanDispatchOrders";
        private bool _canDispatchOrders = false;
        public bool CanDispatchOrders
        {
            get
            {
                return _canDispatchOrders;
            }

            set
            {
                if (_canDispatchOrders == value)
                {
                    return;
                }

                var oldValue = _canDispatchOrders;
                _canDispatchOrders = value;
                RaisePropertyChanged(CanDispatchOrdersPropertyName);
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

        public const string CanApproveOrderPropertyName = "CanApproveOrder";
        private bool _canApproveOrder = false;
        public bool CanApproveOrder
        {
            get
            {
                return _canApproveOrder;
            }

            set
            {
                if (_canApproveOrder == value)
                {
                    return;
                }

                var oldValue = _canApproveOrder;
                _canApproveOrder = value;
                RaisePropertyChanged(CanApproveOrderPropertyName);
            }
        }

        public const string OrdersCountPropertyName = "OrdersCount";
        private int _ordersCount = 0;
        public int OrdersCount
        {
            get
            {
                return _ordersCount;
            }

            set
            {
                if (_ordersCount == value)
                {
                    return;
                }

                var oldValue = _ordersCount;
                _ordersCount = value;
                RaisePropertyChanged(OrdersCountPropertyName);
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

        #endregion

        #endregion

        #region Listings Methods
        
       

        public void LoadOrderStatus()
        {
            //get the type
            Type enumType = typeof(DocumentStatus);

            //set up new collection
            OrderStatusItems = new ObservableCollection<DocumentStatus>();

            //retrieve the info for the type
            FieldInfo[] infos = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

            //Add each proper enum val to collection
            foreach (FieldInfo fi in infos)
                OrderStatusItems.Add((DocumentStatus)Enum.Parse(enumType, fi.Name, true));
        }

        public void LoadOrderTypes()
        {

            //get the type
            Type enumType = typeof(OrderType);

            //set up new collection
            OrderTypes = new ObservableCollection<OrderType>();

            //retrieve the info for the type
            FieldInfo[] infos = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

            //Add each proper enum val to collection
            foreach (FieldInfo fi in infos)
                OrderTypes.Add((OrderType)Enum.Parse(enumType, fi.Name, true));
        }

        void LoadOrders()
        {
            using (var container = NestedContainer)
            {
                IOrderRepository _orderService = Using<IOrderRepository>(container);
                try
                {
                    List<Order> orders = null;
                    Report.Clear();
                    orders = _orderService.GetByOrderTypeAndDocumentStatus(Ordertype, OrderStatus)
                        .Where(n => n.DateRequired >= StartDate && n.DateRequired <= EndDate)
                        .ToList();

                    var orderItems = orders.Select(n => new ReportsListViewModel
                                                            {
                                                                CreatedBy =
                                                                    n.DocumentIssuerUser == null
                                                                        ? "---"
                                                                        : n.DocumentIssuerUser.Username,
                                                                DateRequired = n.DateRequired.ToString("dd-MMM-yyyy"),
                                                                DocumentRef = n.DocumentReference,
                                                                TotalGross = n.TotalGross,
                                                                TotalNet = n.TotalNet,
                                                                TotalVat = n.TotalVat,
                                                                DocIssuerInfo = GetDocIssuerInfo(n),
                                                                OrderId = n.Id,
                                                                Status = n.Status.ToString()
                                                            });

                    orderItems.ToList().ForEach(f => Report.Add(f));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        void RunLoadPendingOrdersCommand()
        {
            //if(!bw.IsBusy)
            //bw.RunWorkerAsync();
            LoadPendingOrders();
        }

        public void SetUp()
        {
            try
            {
                using (var container = NestedContainer)
                {
                   

                    IConfigService _configService = Using<IConfigService>(container);
                    GeneralSetting recordsPerPageSetting = Using<IGeneralSettingRepository>(container).GetByKey(GeneralSettingKey.RecordsPerPage);
                   
                    ItemsPerPage = recordsPerPageSetting != null ? Convert.ToInt32(recordsPerPageSetting.SettingValue) : 10;

                    CanCreateOrders = _configService.ViewModelParameters.CurrentUserRights.CanCreateOrder;
                    CanDispatchOrders = _configService.ViewModelParameters.CurrentUserRights.CanDispatchOrder;
                    CanReceivePayments = _configService.ViewModelParameters.CurrentUserRights.CanCreateOrder;
                    CanApproveOrder = _configService.ViewModelParameters.CurrentUserRights.CanApproveOrders;
                }
            }
            catch
            {
               
            }
        }

        public void ClearViewModel()
        {
            PageCount = 1;
            OrdersCount = 0;
            SearchText = "";
            StartDate = DateTime.Now.AddDays(-2);
            EndDate = DateTime.Now;

            if (DeliveredOrdersCache != null) DeliveredOrdersCache.Clear();
            if (OrdersPendingDispatchCache != null) OrdersPendingDispatchCache.Clear();
            if (PartiallyPaidDeliveriesCache != null) PartiallyPaidDeliveriesCache.Clear();
            if (FullyPaidDeliveriesCache != null) FullyPaidDeliveriesCache.Clear();
            if (BackOrdersCache != null) BackOrdersCache.Clear();
            if (LostSalesCache != null) LostSalesCache.Clear();
        }

        void LoadPendingOrders()
        {
            LoadAllPaymentInfos(null, StartDate, EndDate);
            using (var container = NestedContainer)
            {
                IOrderRepository _orderService = Using<IOrderRepository>(container);
                //IConfigService _configService = Using<IConfigService>(container);
                List<Order> orders = null;
                PageTitle = "Order Summary";
                switch (OrdersToLoad)
                {
                    case EnumOrdersToLoad.PendingApproval:
                        OrdersCount = _orderService.GetCountByDocumentStatus((int) DocumentStatus.Confirmed,
                                                                             (int) OrderType.OutletToDistributor,
                                                                             StartDate,
                                                                             EndDate);
                        orders = _orderService.GetByDocumentStatusPagenated(CurrentPage, ItemsPerPage,
                                                                            (int) DocumentStatus.Confirmed,
                                                                            (int) OrderType.OutletToDistributor,
                                                                            StartDate,
                                                                            EndDate, SearchText);
                        orders = FilterForDeactivatedOutletsOrRoutesOrSalesmen(orders);

                        break;
                    case EnumOrdersToLoad.PendingDispatch:
                        orders = LoadOrdersPendingDispatch();
                        break;
                    case EnumOrdersToLoad.DispatchedOrders:
                        OrdersCount = _orderService.GetCountByDocumentStatus(
                            (int) DocumentStatus.OrderDispatchedToPhone,
                            (int) OrderType.OutletToDistributor, StartDate,
                            EndDate);

                        orders = _orderService.GetByDocumentStatusPagenated(CurrentPage, ItemsPerPage,
                                                                            (int) DocumentStatus.OrderDispatchedToPhone,
                                                                            (int) OrderType.OutletToDistributor,
                                                                            StartDate,
                                                                            EndDate, SearchText);
                        orders = FilterForDeactivatedOutletsOrRoutesOrSalesmen(orders);

                        break;
                    case EnumOrdersToLoad.Incomplete:
                        OrdersCount = _orderService.GetCountByDocumentStatus((int) DocumentStatus.New,
                                                                             (int) OrderType.OutletToDistributor,
                                                                             StartDate,
                                                                             EndDate);

                        orders = _orderService.GetByDocumentStatusPagenated(CurrentPage, ItemsPerPage,
                                                                            (int) DocumentStatus.New,
                                                                            (int) OrderType.OutletToDistributor,
                                                                            StartDate,
                                                                            EndDate, SearchText);
                        orders = FilterForDeactivatedOutletsOrRoutesOrSalesmen(orders);
                        break;
                    case EnumOrdersToLoad.Delivered:
                        orders = LoadDeliveredOrders();
                        break;
                    case EnumOrdersToLoad.PartiallyPaidDeliveries:
                        orders = LoadPartiallyPaidDeliveries();
                        break;
                    case EnumOrdersToLoad.FullyPaidDeliveries:
                        orders = LoadFullyPaidDeliveries();
                        break;
                    case EnumOrdersToLoad.BackOrders:
                        orders = LoadBackOrders();
                        break;
                    case EnumOrdersToLoad.Rejected:
                        OrdersCount = _orderService.GetCountByDocumentStatus((int) DocumentStatus.Rejected, true,
                                                                             (int) DocumentStatus.Cancelled, true,
                                                                             (int) OrderType.OutletToDistributor,
                                                                             StartDate,
                                                                             EndDate);

                        orders = _orderService.GetByDocumentStatusPagenated(CurrentPage, ItemsPerPage,
                                                                            (int) DocumentStatus.Rejected, true,
                                                                            (int) DocumentStatus.Cancelled, true,
                                                                            (int) OrderType.OutletToDistributor,
                                                                            StartDate,
                                                                            EndDate, SearchText);

                        orders = FilterForDeactivatedOutletsOrRoutesOrSalesmen(orders);
                        break;
                    case EnumOrdersToLoad.LostSales:
                        orders = LoadLostSales();
                        break;
                }

                PageCount = (int) Math.Ceiling(Math.Round(((double) OrdersCount)/ItemsPerPage, 1));

                DumpAndDeploy(orders);
            }
        }

        List<Order> Search(List<Order> orders)
        {
            SearchText = SearchText.ToLower();
            orders = orders.Where(n =>
                                  (n.DocumentReference.ToLower()).Contains(SearchText) ||
                                  (n.DocumentIssuerUser.Username.ToLower()).Contains(SearchText) ||
                                  (n.Status.ToString().ToLower()).Contains(SearchText) ||
                                  (n.DocumentDateIssued.ToString()).Contains(SearchText) ||
                                  (n.DateRequired.ToString()).Contains(SearchText)
                ).ToList();

            return orders;
        }

        void DumpAndDeploy(List<Order> orders)
        {
            /*
            if(orders !=null)
            {
                Orders.Clear();
                ListOrderViewModelItem lineItem = null;
                int i = 1;
                foreach (var order in orders)
                {
                    lineItem = new ListOrderViewModelItem
                                   {
                                       SequenceNo = i + ((((CurrentPage - 1)*ItemsPerPage) + 1)),
                                       CreatedBy =
                                           order.DocumentIssuerUser == null
                                               ? "---"
                                               : order.DocumentIssuerUser.Username,
                                       DateRequired = order.DateRequired.ToString("dd-MMM-yyyy"),
                                       DocumentRef = order.DocumentReference,
                                       TotalGross = order.TotalGross.ToString("#,0.00;(#,0.00);0"),
                                       //"#,###,###.00"
                                       TotalNet = order.TotalNet.ToString("#,0.00;(#,0.00);0"),
                                       TotalVat = order.TotalVat.ToString("#,0.00;(#,0.00);0"),
                                       //ToString("G"),
                                       DocIssuerInfo = GetDocIssuerInfo(order),
                                       OrderId = order.Id,
                                       Status = GetStatus(order),
                                       HasBackOrder = HasUnprocessedBackOrder(order),
                                       TotalPaid = GetTotalPaid(order.Id),
                                       TotalDue = GetAmountDue(order.Id)
                                   };
                    i++;
                    Orders.Add(lineItem);
                }
            }*/
        
            Application.Current.Dispatcher.BeginInvoke(
                new Action(delegate
                               {
                                   if (orders == null) return;
                                   var orderItems =
                                       orders.OrderByDescending(n => n.DocumentDateIssued).Select(
                                           (n, j) => new ListOrderViewModelItem
                                                         {
                                                             SequenceNo = j + ((((CurrentPage - 1)*ItemsPerPage) + 1)),
                                                             CreatedBy =
                                                                 n.DocumentIssuerUser == null
                                                                     ? "---"
                                                                     : n.DocumentIssuerUser.Username,
                                                             DateRequired = n.DateRequired.ToString("dd-MMM-yyyy"),
                                                             DocumentRef = n.DocumentReference,
                                                             TotalGross = n.TotalGross.ToString("#,0.00;(#,0.00);0"),
                                                             //"#,###,###.00"
                                                             TotalNet = n.TotalNet.ToString("#,0.00;(#,0.00);0"),
                                                             TotalVat = n.TotalVat.ToString("#,0.00;(#,0.00);0"),
                                                             //ToString("G"),
                                                             DocIssuerInfo = GetDocIssuerInfo(n),
                                                             OrderId = n.Id,
                                                             Status = GetStatus(n),
                                                             HasBackOrder = HasUnprocessedBackOrder(n),
                                                             TotalPaid = GetTotalPaid(n.Id),
                                                             TotalDue = GetAmountDue(n.Id)
                                                         }
                                           );

                                   Orders.Clear();
                                   orderItems.ToList().ForEach(f => Orders.Add(f));
                               }));
            

        }

        string GetStatus(Order order)
        {
            using (var container = NestedContainer)
            {
                var _dispatchNoteService = Using<IDispatchNoteRepository>(container);
                string retStatus = order.Status.ToString();
                if (order.Status == DocumentStatus.OrderDispatchedToPhone && OrdersToLoad == EnumOrdersToLoad.Delivered)
                    return "Partially Delivered";

                if (OrdersToLoad == EnumOrdersToLoad.PartiallyPaidDeliveries)
                {
                    if (order.Status == DocumentStatus.OrderDispatchedToPhone &&
                        _dispatchNoteService.GetAll().OfType<DispatchNote>().Any(
                            n => n.OrderId == order.Id && n.DispatchType == DispatchNoteType.Delivery))
                        return "Partially Delivered";
                }

                if (retStatus != "Closed" && retStatus != "New" && retStatus != "Confirmed")
                    return Using<IOtherUtilities>(container).BreakStringByUpperCB(retStatus);

                return retStatus;
            }
        }

        public string GetTotalPaid(Guid orderId)
        {
            try
            {
                return OrderPaymentInfos.FirstOrDefault(o => o.OrderId == orderId).AmountPaid.ToString("#,0.00;(#,0.00);0");
            }
            catch
            {
                return "0.00";
            }
        }

        public string GetAmountDue(Guid orderId)
        {
            string retVal = "0.00";
            try
            {
                decimal amnt = OrderPaymentInfos.FirstOrDefault(o => o.OrderId == orderId).AmountDue;
                //if (amnt < 0) amnt = -amnt; //cn: WTF
                retVal = amnt.ToString("#,0.00;(#,0.00);0");
            }
            catch
            {
                return retVal;
            }
            return retVal;
        }

        public bool HasUnprocessedBackOrder(Order order)
        {
            int unprocessedLiCnt = 0;
            bool hasUnprocessed = false;
            if (order.LineItems.Any(n => n.LineItemType == OrderLineItemType.BackOrder))
            {
                foreach (var item in order.LineItems.Where(n => n.LineItemType == OrderLineItemType.BackOrder))
                {
                    decimal totalProcessedBackOrder = 0;
                    try
                    {
                        totalProcessedBackOrder =
                            order.LineItems.Where(
                                n =>
                                n.LineItemType == OrderLineItemType.ProcessedBackOrder &&
                                n.Product.Id == item.Product.Id).Sum(n => n.Qty);
                    }
                    catch
                    {
                    }
                    if ((item.Qty - totalProcessedBackOrder) > 0)
                    {
                        unprocessedLiCnt += 1;
                        hasUnprocessed = true;
                    }
                }
            }
            if (unprocessedLiCnt > 0)
                hasUnprocessed = true;
            else if (unprocessedLiCnt == 0)
                hasUnprocessed = false;

            return hasUnprocessed;
        }

        List<Invoice> InvoicesList = null;
        List<Invoice> AllInvoices = null;
        List<CreditNote> InvoiceCreditNotes = null;

        List<InvoicePaymentInfo> OutstandingOrderpaymentInfos = null;
        public List<InvoicePaymentInfo> OrderPaymentInfos = null;

        void LoadUnpaidInvoices()
        {
            using (var container = NestedContainer)
            {
                IReceiptRepository _receiptService = Using<IReceiptRepository>(container);
                IInvoiceRepository _invoiceService = Using<IInvoiceRepository>(container);
                List<Invoice> invoices = _invoiceService.GetAll().OfType<Invoice>().ToList();
                InvoicesList = new List<Invoice>();
                OutstandingOrderpaymentInfos = new List<InvoicePaymentInfo>();
                foreach (var inv in invoices)
                {
                    //List<Receipt> irs = _receiptService.GetAll().Where(n => n.LineItems.Any(li => li.InvoiceId == inv.Id)).ToList();
                    List<Receipt> irs = _receiptService.GetByInvoiceId(inv.Id);
                    var irsTotals = new decimal();
                    foreach (Receipt item in irs)
                        irsTotals = irsTotals + item.Total;
                    if (inv.TotalGross > irsTotals)
                    {
                        InvoicesList.Add(inv);

                        OutstandingOrderpaymentInfos.Add(new InvoicePaymentInfo
                                                             {
                                                                 InvoiceId = inv.Id,
                                                                 OrderId = inv.OrderId,
                                                                 InvoiceAmount = inv.TotalGross,
                                                                 AmountPaid = irsTotals,
                                                                 AmountDue = inv.TotalGross - irsTotals
                                                             });
                    }
                }
            }
        }

        void LoadFullyPaidInvoices()
        {
            using (var container = NestedContainer)
            {
                IReceiptRepository _receiptService = Using<IReceiptRepository>(container);
                IInvoiceRepository _invoiceService = Using<IInvoiceRepository>(container);
                List<Invoice> invoices = _invoiceService.GetAll().OfType<Invoice>().ToList();
                InvoicesList = new List<Invoice>();
                foreach (var inv in invoices)
                {
                    List<Receipt> irs = _receiptService.GetByInvoiceId(inv.Id);
                    var irsTotals = new decimal();
                    foreach (Receipt item in irs)
                        irsTotals = irsTotals + item.Total;
                    if (inv.TotalGross == irsTotals)
                    {
                        InvoicesList.Add(inv);
                    }
                }
            }
        }

        public void LoadAllPaymentInfos(List<Order> orders = null, DateTime startDate = new DateTime(), DateTime endDate = new DateTime())
        {
            using (var container = NestedContainer)
            {
                var _receiptService = Using<IReceiptRepository>(container);
                var _invoiceService = Using<IInvoiceRepository>(container);
                ICreditNoteRepository _creditNoteService = Using<ICreditNoteRepository>(container);
                
                OrderPaymentInfos = new List<InvoicePaymentInfo>();
                AllInvoices = new List<Invoice>();
                InvoiceCreditNotes = new List<CreditNote>();

                OrderPaymentInfos.Clear();
                AllInvoices.Clear();
                InvoiceCreditNotes.Clear();

                if (startDate.Equals(new DateTime()))
                {
                    if (orders == null)
                    {
                        AllInvoices = _invoiceService.GetAll().OfType<Invoice>()
                            .Where(n => n.Status != DocumentStatus.Rejected).ToList();
                    }
                    else
                    {
                        AllInvoices = orders.Select(n => _invoiceService.GetInvoiceByOrderId(n.Id)).ToList();
                    }
                }
                else
                {
                    if (orders == null)
                    {
                        AllInvoices = _invoiceService.GetAll(startDate, DateTime.Now).OfType<Invoice>()
                            .Where(n => n.Status != DocumentStatus.Rejected).ToList();
                    }
                    else
                    {
                        AllInvoices =
                            orders.Where(n => n.DocumentDateIssued >= startDate && n.DocumentDateIssued <= endDate)
                                .Select(n => _invoiceService.GetInvoiceByOrderId(n.Id)).ToList();
                    }
                }


                //InvoiceCreditNotes = _creditNoteService.GetAll();

                foreach (var inv in AllInvoices)
                {
                    var invoiceReceipts = _receiptService.GetByInvoiceId(inv.Id);
                    var receiptsTotal = new decimal();
                    if (invoiceReceipts != null)
                    {
                        foreach (var r in invoiceReceipts)
                        {
                            receiptsTotal = receiptsTotal + r.Total;
                        }
                    }

                    //var invoiceCreditNotes = InvoiceCreditNotes.Where(n => n.InvoiceId == inv.Id);                
                    var invoiceCreditNotes = _creditNoteService.GetCreditNotesByInvoiceId(inv.Id);
                    var creditNotesTotals = new decimal();
                    if (invoiceCreditNotes != null)
                    {
                        foreach (var cn in invoiceCreditNotes)
                        {
                            creditNotesTotals = creditNotesTotals + cn.Total;
                        }
                    }

                    OrderPaymentInfos.Add(new InvoicePaymentInfo
                                              {
                                                  InvoiceId = inv.Id,
                                                  OrderId = inv.OrderId,
                                                  InvoiceAmount = inv.TotalGross,
                                                  AmountPaid = receiptsTotal,
                                                  CreditNoteAmount = creditNotesTotals,
                                                  AmountDue = (inv.TotalGross - creditNotesTotals) - receiptsTotal,
                                                  InvoiceDate = inv.DocumentDateIssued
                                              });
                }
            }
        }

        private List<Order> OrdersPendingDispatchCache;
        public List<Order> LoadOrdersPendingDispatch()
        {
            using (var container = NestedContainer)
            {
                IOrderRepository _orderService = Using<IOrderRepository>(container);
               

                List<Order> orders;
                List<Order> retOrders = new List<Order>();
                if (CurrentPage == 1 && SearchText.Trim() == "") //reload all otherwise get from cache
                {
                    OrdersPendingDispatchCache.Clear();
                    orders = _orderService.GetByOrderTypeAndDocumentStatus(OrderType.OutletToDistributor,
                                                                           DocumentStatus.OrderPendingDispatch, true,
                                                                           StartDate, EndDate);

                    var partiallyDispatched = LoadPartiallyDispatchedOrders(true).ToList();
                    partiallyDispatched.ForEach(orders.Add);
                    orders.OrderByDescending(n => n.DocumentDateIssued).ToList().ForEach(OrdersPendingDispatchCache.Add);

                }
                else if (CurrentPage == 1 && SearchText.Trim() != "")
                {
                    OrdersPendingDispatchCache = Search(OrdersPendingDispatchCache);
                }

                OrdersCount = OrdersPendingDispatchCache.Count;

                retOrders = OrdersPendingDispatchCache.Skip((CurrentPage - 1)*ItemsPerPage).Take(ItemsPerPage).ToList();

                return retOrders;
            }
        }

        private List<Order> DeliveredOrdersCache;
        private List<Order> LoadDeliveredOrders()
        {
            using (var container = NestedContainer)
            {
                IOrderRepository _orderService = Using<IOrderRepository>(container);

                List<Order> orders;
                List<Order> retOrders = new List<Order>();
                if (CurrentPage == 1 && SearchText.Trim() == "") //reload all otherwise get from cache
                {
                    DeliveredOrdersCache.Clear();
                    orders =
                        _orderService.GetByOrderTypeAndDocumentStatus(OrderType.OutletToDistributor,
                                                                      DocumentStatus.Closed,
                                                                      true, StartDate, EndDate, SearchText)
                            .Union(LoadPartiallyDeliveredOrders()).ToList();
                    orders = FilterForDeactivatedOutletsOrRoutesOrSalesmen(orders);
                    orders.ForEach(DeliveredOrdersCache.Add);

                }
                else if (CurrentPage == 1 && SearchText.Trim() != "")
                    DeliveredOrdersCache = Search(DeliveredOrdersCache);

                OrdersCount = DeliveredOrdersCache.Count;

                retOrders = DeliveredOrdersCache.Skip((CurrentPage - 1)*ItemsPerPage).Take(ItemsPerPage).ToList();

                return retOrders;
            }
        }

        IEnumerable<Order> LoadPartiallyDeliveredOrders()
        {
            using (var container = NestedContainer)
            {
                IOrderRepository _orderService = Using<IOrderRepository>(container);
                var _dispatchNoteService = Using<IDispatchNoteRepository>(container);
                var partiallyDeliveredOrders = new List<Order>();
                var dispatchNoteOrderIds = _dispatchNoteService.GetAll().OfType<DispatchNote>().Where(
                    n => n.DispatchType == DispatchNoteType.Delivery)
                    .Where(n => n.DocumentDateIssued >= StartDate).Select(n => n.OrderId);

                foreach (var id in dispatchNoteOrderIds.Distinct())
                {
                    var order = _orderService.GetById(id) as Order;
                    if (order == null)
                        continue; //shida hapa!!??????????? 
                    if (order.Status == DocumentStatus.OrderDispatchedToPhone)
                    {
                        partiallyDeliveredOrders.Add(order);
                    }
                }
                if (SearchText.Trim() != "")
                {
                    SearchText = SearchText.ToLower();
                    partiallyDeliveredOrders = partiallyDeliveredOrders
                        .Where(n =>
                               n.DocumentIssuerUser.Username.ToLower().Contains(SearchText)
                               || n.DocumentReference.ToLower().Contains(SearchText)
                               || n.DocumentDateIssued.ToString().ToLower().Contains(SearchText))
                        .ToList();
                }
                return partiallyDeliveredOrders;
            }
        }

        private List<Order> PartiallyPaidDeliveriesCache;
        List<Order> LoadPartiallyPaidDeliveries()
        {
            using (var container = NestedContainer)
            {
                IOrderRepository _orderService = Using<IOrderRepository>(container);
                //IDispatchNoteService _dispatchNoteService = Using<IDispatchNoteService>(container);
                var orders = new List<Order>();

                if (CurrentPage == 1 && SearchText.Trim() == "")
                {
                    PartiallyPaidDeliveriesCache.Clear();
                    var orderIds = OrderPaymentInfos.Where(n => n.AmountDue > 0).Select(n => n.OrderId).ToList();
                    orderIds.Distinct().ToList().ForEach(n =>
                                                             {
                                                                 Order o = null;
                                                                 o = _orderService.GetById(n) as Order;
                                                                 if (o != null &&
                                                                     o.OrderType == OrderType.OutletToDistributor)
                                                                     orders.Add(o);
                                                             });
                    orders = FilterForDeactivatedOutletsOrRoutesOrSalesmen(orders);

                    orders.ForEach(PartiallyPaidDeliveriesCache.Add);


                }
                else if (CurrentPage == 1 && SearchText.Trim() != "")
                    PartiallyPaidDeliveriesCache = Search(PartiallyPaidDeliveriesCache);

                OrdersCount = PartiallyPaidDeliveriesCache.Count;

                orders = PartiallyPaidDeliveriesCache.Skip((CurrentPage - 1)*ItemsPerPage).Take(ItemsPerPage).ToList();

                return orders;
            }
        }

        private List<Order> FullyPaidDeliveriesCache;
        List<Order> LoadFullyPaidDeliveries()
        {
            using (var container = NestedContainer)
            {
                IOrderRepository _orderService = Using<IOrderRepository>(container);
                var _dispatchNoteService = Using<IDispatchNoteRepository>(container);
                var orders = new List<Order>();
                if (CurrentPage == 1 && SearchText.Trim() == "")
                {
                    FullyPaidDeliveriesCache.Clear();
                    var orderIds2 = OrderPaymentInfos.Where(n => n.AmountDue <= 0).Select(n => n.OrderId).ToList();
                    orderIds2.Distinct().ToList().ForEach(n =>
                                                              {
                                                                  Order o = _orderService.GetById(n) as Order;
                                                                  if (o != null &&
                                                                      o.OrderType == OrderType.OutletToDistributor)
                                                                      orders.Add(o);
                                                              });
                    orders = FilterForDeactivatedOutletsOrRoutesOrSalesmen(orders);
                    orders.ForEach(FullyPaidDeliveriesCache.Add);

                }
                else if (CurrentPage == 1 && SearchText.Trim() != "")
                    FullyPaidDeliveriesCache = Search(FullyPaidDeliveriesCache);

                OrdersCount = FullyPaidDeliveriesCache.Count;

                orders = FullyPaidDeliveriesCache.Skip((CurrentPage - 1)*ItemsPerPage).Take(ItemsPerPage).ToList();

                return orders;
            }
        }

        private List<Order> BackOrdersCache;
        List<Order> LoadBackOrders()
        {
            using (var container = NestedContainer)
            {
                IOrderRepository _orderService = Using<IOrderRepository>(container);
                var _dispatchNoteService = Using<IDispatchNoteRepository>(container);
                List<Order> orders = new List<Order>();

                if (CurrentPage == 1 && SearchText.Trim() == "")
                {
                    BackOrdersCache.Clear();
                    var ordersWithBackOrder =
                        _orderService.GetByOrderTypeAndDocumentStatus(OrderType.OutletToDistributor, DocumentStatus.New,
                                                                      false, StartDate, EndDate)
                            .Where(n => n.LineItems.Any(l => l.LineItemType == OrderLineItemType.BackOrder)).ToList();

                    List<Order> fullyProcessedBOs = new List<Order>();
                    if (
                        ordersWithBackOrder.Any(
                            n =>
                            n.LineItems.Any(
                                l =>
                                l.LineItemType == OrderLineItemType.ProcessedBackOrder ||
                                l.LineItemType == OrderLineItemType.LostSale)))
                    {
                        foreach (var orderWBO in ordersWithBackOrder)
                        {
                            if (CheckOrderIsFullyProcessed(orderWBO))
                                fullyProcessedBOs.Add(orderWBO);
                        }
                    }
                    fullyProcessedBOs.ForEach(n => ordersWithBackOrder.Remove(n));
                    ordersWithBackOrder = FilterForDeactivatedOutletsOrRoutesOrSalesmen(ordersWithBackOrder);

                    ordersWithBackOrder.ForEach(BackOrdersCache.Add);
                }
                else if (CurrentPage == 1 && SearchText.Trim() != "")
                    BackOrdersCache = Search(BackOrdersCache);

                OrdersCount = BackOrdersCache.Count;

                orders = BackOrdersCache.Skip((CurrentPage - 1)*ItemsPerPage).Take(ItemsPerPage).ToList();

                return orders;
            }
        }

        private List<Order> LostSalesCache;
        List<Order> LoadLostSales()
        {
            using (var container = NestedContainer)
            {
                IOrderRepository _orderService = Using<IOrderRepository>(container);
                var _dispatchNoteService = Using<IDispatchNoteRepository>(container);
                List<Order> orders = new List<Order>();

                if (CurrentPage == 1 && SearchText.Trim() == "")
                {
                    LostSalesCache.Clear();
                    orders =
                        _orderService.GetByOrderTypeAndDocumentStatus(OrderType.OutletToDistributor,
                                                                      DocumentStatus.OrderPendingDispatch, true,
                                                                      StartDate, EndDate).Union(
                                                                          _orderService.GetByOrderTypeAndDocumentStatus(
                                                                              OrderType.OutletToDistributor,
                                                                              DocumentStatus.OrderDispatchedToPhone,
                                                                              true, StartDate, EndDate)).Union(
                                                                                  _orderService.
                                                                                      GetByOrderTypeAndDocumentStatus(
                                                                                          OrderType.OutletToDistributor,
                                                                                          DocumentStatus.Closed, true,
                                                                                          StartDate, EndDate))
                            .Where(n => n.LineItems.Any(l => l.LineItemType == OrderLineItemType.LostSale))
                            .ToList();

                    orders = FilterForDeactivatedOutletsOrRoutesOrSalesmen(orders);
                    orders.ForEach(LostSalesCache.Add);
                }
                else if (CurrentPage == 1 && SearchText.Trim() != "")
                    LostSalesCache = Search(LostSalesCache);

                OrdersCount = LostSalesCache.Count;

                orders = LostSalesCache.Skip((CurrentPage - 1)*ItemsPerPage).Take(ItemsPerPage).ToList();

                return orders;
            }
        }

        static bool CheckOrderIsFullyProcessed(Order orderWithBackOrder)
        {
            int unfullyProcesedLiCnt = 0;
            bool isFullyProcessed = true;
            var backOrderLineItems = orderWithBackOrder.LineItems.Where(n => n.LineItemType == OrderLineItemType.BackOrder);
            foreach (var li in backOrderLineItems)
            {
                if (orderWithBackOrder.LineItems.Any(
                    l =>
                    l.Description == li.Description
                     && (l.LineItemType == OrderLineItemType.ProcessedBackOrder
                    || l.LineItemType == OrderLineItemType.LostSale)))
                {
                    decimal totalProcessedBOQty =
                        orderWithBackOrder.LineItems.Where(l =>
                            l.Description == li.Description && //has original LI Id
                            //l.Product.Id == li.Product.Id &&
                            l.LineItemType == OrderLineItemType.ProcessedBackOrder).Sum(n => n.Qty);

                    decimal totalLostSale = orderWithBackOrder.LineItems.Where(n =>
                            n.Description == li.Description
                            && n.LineItemType == OrderLineItemType.LostSale)
                            .Sum(n => n.Qty);

                    if ((li.Qty - totalProcessedBOQty - totalLostSale) > 0)//here back order is constant
                        //if (li.Qty > 0) //here back order is modified whenever it is processed
                        unfullyProcesedLiCnt += 1;
                }
                else
                {
                    if (li.Qty > 0)//back order was not converted to lost sale.?????
                        unfullyProcesedLiCnt += 1;
                }

            }
            if (unfullyProcesedLiCnt == 0)
                isFullyProcessed = true;
            else if (unfullyProcesedLiCnt > 0)
                isFullyProcessed = false;

            return isFullyProcessed;
        }

        string GetDocIssuerInfo(Document o)
        {
            using (var container = NestedContainer)
            {
                var _costCentreService = Using<ICostCentreRepository>(container);
                IUserRepository _userService = Using<IUserRepository>(container);
                CostCentre cc = _costCentreService.GetById(o.DocumentIssuerCostCentre.Id);
                User u = _userService.GetById(o.DocumentIssuerUser == null ? Guid.Empty : o.DocumentIssuerUser.Id);
                return u != null ? string.Format("{0} ({1})", u.Username, cc.Name) : string.Empty;
            }
        }

        private void RunAddOrders()
        {
            SendNavigationRequestMessage(new Uri("/views/salesmanorders/editsalesmanorder.xaml?orderid=" + Guid.Empty,
                                                 UriKind.Relative));
        }

        #endregion

        #region Dispatch

        private List<Salesman> OrderedSalesmen;
        private List<Order> orderFailedToLoad;
        void RunLoadOrderForDispatch()
        {
            using (var container = NestedContainer)
            {
                IOrderRepository _orderService = Using<IOrderRepository>(container);

                orderFailedToLoad = new List<Order>();
                ShowProgressBar = true;
                List<Order> orders = null;
                PageTitle = "Dispatch Pending Orders";
                if (SelectedRoute != null && SelectedRoute.Id != Guid.Empty &&
                    (SelectedSalesman == null || SelectedSalesman.Id == Guid.Empty))
                {
                    //load orders in that route
                    try
                    {
                        orders =
                            LoadPartiallyDispatchedOrders().Union(
                                _orderService.GetByOrderTypeAndDocumentStatus(OrderType.OutletToDistributor,
                                                                              DocumentStatus.OrderPendingDispatch)).
                                Where(
                                    n =>
                                    ((Outlet)
                                     n.IssuedOnBehalfOf).Route.Id ==
                                    SelectedRoute.Id).ToList();
                        //filter deactivated route n/or outlet
                        orders = FilterForDeactivatedOutletsOrRoutesOrSalesmen(orders);
                        orders.ForEach(GetOrderRouteSalesmen);

                        OrderedSalesmen.Clear();
                        Salesmen.Where(n => n.Id != Guid.Empty).OrderBy(n => n.Username).ToList()
                            .ForEach(n =>
                                         {
                                             if (OrderedSalesmen.All(q => q.Id != n.Id))
                                                 OrderedSalesmen.Add(n);
                                         });
                        Salesmen.Where(n => n.Id != Guid.Empty).ToList().ForEach(n => Salesmen.Remove(n));
                        //OrderedSalesmen;
                        OrderedSalesmen.ToList().ForEach(n => Salesmen.Add(n));
                    }
                    catch
                    {
                        MessageBox.Show(
                            "An error was encountered while filtering orders by route.\nOne or more of the orders may have route = null",
                            "Distributr: Fetch Orders For Dispatch", MessageBoxButton.OK);
                    }
                }
                else if (SelectedSalesman != null && SelectedSalesman.Id != Guid.Empty)
                {
                    //LoadRoutesSalesmen();

                    orders =
                        LoadPartiallyDispatchedOrders().Union(
                            _orderService.GetByOrderTypeAndDocumentStatus(OrderType.OutletToDistributor,
                                                                          DocumentStatus.OrderPendingDispatch)).Where(
                                                                              n =>
                                                                              n.DocumentIssuerUser.Id ==
                                                                              SelectedSalesman.Id).ToList();

                    //filter deactivated route n/or outlet
                    orders = FilterForDeactivatedOutletsOrRoutesOrSalesmen(orders);
                }
                else
                {
                    orders =
                        LoadPartiallyDispatchedOrders().Union(
                            _orderService.GetByOrderTypeAndDocumentStatus(OrderType.OutletToDistributor,
                                                                          DocumentStatus.OrderPendingDispatch)).ToList();

                    //filter deactivated route n/or outlet
                    orders = FilterForDeactivatedOutletsOrRoutesOrSalesmen(orders);
                    orders.ForEach(GetOrderRouteSalesmen);

                    OrderedSalesmen.Clear();
                    Salesmen.Where(n => n.Id != Guid.Empty).OrderBy(n => n.Username).ToList()
                        .ForEach(n =>
                                     {
                                         if (OrderedSalesmen.All(q => q.Id != n.Id))
                                             OrderedSalesmen.Add(n);
                                     });
                    Salesmen.Where(n => n.Id != Guid.Empty).ToList().ForEach(n => Salesmen.Remove(n));
                    OrderedSalesmen.ToList().ForEach(n => Salesmen.Add(n));
                }

                try
                {
                    var orderItems = new List<ListOrderViewModelItem>();
                    int seqNo = 1;
                    foreach (var n in orders.OrderByDescending(n => n.DocumentDateIssued))
                    {
                        try
                        {
                            var li = new ListOrderViewModelItem
                                         {
                                             SequenceNo = seqNo,
                                             CreatedBy =
                                                 n.DocumentIssuerUser == null
                                                     ? "---"
                                                     : n.DocumentIssuerUser.Username,
                                             DateRequired = n.DateRequired.ToString("dd-MMM-yyyy hh:mm tt"),
                                             DocumentRef = n.DocumentReference,
                                             TotalGross = n.TotalGross.ToString("#,###,###.00"),
                                             //TotalVat = n.TotalVat.ToString("G"),
                                             TotalNet = n.TotalNet.ToString("#,###,###.00"),
                                             TotalVat = n.TotalVat.ToString("#,###,###.00"),
                                             DocIssuerInfo = GetDocIssuerInfo(n),
                                             OrderId = n.Id,
                                             Status = n.Status.ToString(),
                                             Recipients = Salesmen,
                                             SelectedRecipient = Salesmen.First(s => s.Id == n.DocumentIssuerUser.Id),
                                             HasBackOrder = HasUnprocessedBackOrder(n),
                                             chkDispatchContent =
                                                 n.Status == DocumentStatus.OrderDispatchedToPhone ? "*" : ""
                                         };
                            orderItems.Add(li);
                            seqNo += 1;
                        }
                        catch
                        {
                            orderFailedToLoad.Add(n);
                        }
                    }

                    Orders.Clear();
                    //orderItems.ToList().ForEach(f => Orders.Add(f));
                    foreach (var item in orderItems)
                    {
                        Orders.Add(item);
                    }
                }
                catch
                {
                    MessageBox.Show("An error was encountered while loading orders for dispatch to grid.",
                                    "Distributr: Fetch Orders For Dispatch", MessageBoxButton.OK);
                }
                ShowProgressBar = false;
            }
        }

        List<Order> FilterForDeactivatedOutletsOrRoutesOrSalesmen(List<Order> orders)
        {
            if (orders == null)
                return null;

            var toRemove = new List<Order>();
            foreach (Order order in orders)
            {
                if (((Outlet)order.IssuedOnBehalfOf).Route._Status != EntityStatus.Active)
                    toRemove.Add(order);
            }

            toRemove.ForEach(n => orders.Remove(n));
            toRemove.Clear();

            foreach (Order order in orders)
            {
                if (((Outlet)order.IssuedOnBehalfOf)._Status !=EntityStatus.Active)
                    toRemove.Add(order);
            }

            toRemove.ForEach(n => orders.Remove(n));
            toRemove.Clear();

            //foreach (Order order in orders)
            //{
            //    if (!order.DocumentIssuerUser._IsActive)
            //        toRemove.Add(order);
            //}

            //toRemove.ForEach(n => orders.Remove(n));
            //toRemove.Clear();

            return orders;
        }


        public IEnumerable<Order> LoadPartiallyDispatchedOrders(bool forOrdersSummary = false)
        {
        //partially dispatched orders
        //order is Dispatched
        //has back order
        //back order is processed
            using (var container = NestedContainer)
            {
                IOrderRepository _orderService = Using<IOrderRepository>(container);
                var _dispatchNoteService = Using<IDispatchNoteRepository>(container);

                List<Order> orders = new List<Order>();
                if (forOrdersSummary)
                    orders = _orderService.GetByOrderTypeAndDocumentStatus(OrderType.OutletToDistributor,
                                                                           DocumentStatus.OrderDispatchedToPhone
                                                                           , true, StartDate, EndDate)
                        .Where(n => n.LineItems.Any(l => l.LineItemType == OrderLineItemType.ProcessedBackOrder)).ToList
                        ();
                else
                {
                    orders = _orderService.GetByOrderTypeAndDocumentStatus(OrderType.OutletToDistributor,
                                                                           DocumentStatus.OrderDispatchedToPhone
                                                                           , true)
                        .Where(n => n.LineItems.Any(l => l.LineItemType == OrderLineItemType.ProcessedBackOrder)).ToList
                        ();
                }

                List<Order> withUndispatched = new List<Order>();
                foreach (var ord in orders)
                {
                    List<DispatchNote> dns = new List<DispatchNote>();
                    dns = _dispatchNoteService.GetAll().OfType<DispatchNote>()
                        .Where(n => n.OrderId == ord.Id && n.DispatchType == DispatchNoteType.DispatchToPhone).ToList();


                    if (forOrdersSummary)
                        dns = _dispatchNoteService.GetByOrderId(ord.Id);
                    else
                        dns = _dispatchNoteService.GetByOrderId(ord.Id);

                    if (hasItemsToBeDispatched(ord, dns))
                    {
                        withUndispatched.Add(ord);
                    }
                }

                return withUndispatched;
            }
        }

        public IEnumerable<Order> LoadPartiallyDispatchedOrders(List<Order> orders)
        {
            using (var container = NestedContainer)
            {
                
                var _dispatchNoteService = Using<IDispatchNoteRepository>(container);

                orders =
                    orders.Where(
                        n =>
                        n.OrderType == OrderType.OutletToDistributor &&
                        n.Status == DocumentStatus.OrderDispatchedToPhone).ToList();
                orders =
                    orders.Where(n => n.LineItems.Any(l => l.LineItemType == OrderLineItemType.ProcessedBackOrder)).
                        ToList();

                List<Order> withUndispatched = new List<Order>();
                foreach (var ord in orders)
                {
                    var dns =
                        _dispatchNoteService.GetAll().OfType<DispatchNote>().Where(
                            n => n.OrderId == ord.Id && n.DispatchType == DispatchNoteType.DispatchToPhone).ToList();

                    if (hasItemsToBeDispatched(ord, dns))
                    {
                        withUndispatched.Add(ord);
                    }
                }

                return withUndispatched;
            }
        }

        bool hasItemsToBeDispatched(Order ord, List<DispatchNote> dns)
        {
            bool has = false;
            foreach (var procesdBO in ord.LineItems.Where(n => n.LineItemType == OrderLineItemType.ProcessedBackOrder))
            {
                //get oli's confirmed line item
                var confirmed = ord.LineItems.FirstOrDefault(n => n.Description == procesdBO.Description
                                                                  && n.LineItemType == OrderLineItemType.PostConfirmation);
                if(confirmed == null)
                {
                    confirmed = ord.LineItems.FirstOrDefault(n => n.Id.ToString() == procesdBO.Description && n.LineItemType == OrderLineItemType.Discount);
                }

                var liBo = ord.LineItems.FirstOrDefault(n => n.Description == procesdBO.Description && n.LineItemType == OrderLineItemType.BackOrder);

                decimal liDispatchedQty = dns.SelectMany(n => n.LineItems.Where(l => l.Description== confirmed.Description)).Sum(s => s.Qty);
                if (liDispatchedQty < (confirmed.Qty - (liBo.Qty - procesdBO.Qty)))
                {
                    has = true;
                }

            }

            return has;
        }

        void GetOrderRouteSalesmen(Order order)
        {
            try
            {
                if (((Outlet)order.IssuedOnBehalfOf).Route != null)//filter deactivated routes
                    LoadRoutesSalesmen(((Outlet)order.IssuedOnBehalfOf).Route, order.DocumentIssuerUser);
            }
            catch (Exception ex)
            {
                //weka attached salesman kisha bla .. bla ..
                if (!Salesmen.Any(n => n.Id == order.DocumentIssuerUser.Id))
                {
                    Salesmen.Add(new Salesman { Id = order.DocumentIssuerUser.Id, Username = order.DocumentIssuerUser.Username });
                }
                MessageBox.Show("Order " + order.DocumentReference + " has the following issue.\n" + ex.Message,
                                "Distributr: Fetch Orders For Dispatch", MessageBoxButton.OK);
            }
        }

        public bool changedFromCode = false;
        void RunRouteChangedCommand()
        {
            using (var container = NestedContainer)
            {
                ISalesmanRouteRepository _salesmanRouteService = Using<ISalesmanRouteRepository>(container);
               

                Salesmen.Clear();
                var salesman = new Salesman {Id = Guid.Empty, Username = "--Please Select a Salesman--"};
                Salesmen.Add(salesman);

                SelectedSalesman = salesman;
                OverallRecipient = salesman;

                if (SelectedRoute != null && SelectedRoute.Id != Guid.Empty)
                {
                    string[] salesmenwarehouses =
                        _salesmanRouteService.GetAll().Where(n => n.Route != null && n.Route.Id == SelectedRoute.Id).
                            Select(
                                c => c.DistributorSalesmanRef.Id.ToString()).ToArray();
                    _allSalesmen.Where(u => salesmenwarehouses.Contains(u.CostCentre.ToString())).ToList().ForEach(
                        n => Salesmen.Add(new Salesman {Id = n.Id, Username = n.Username}));

                }
                else
                {
                    if (OrderedSalesmen != null)
                        OrderedSalesmen.ForEach(n => Salesmen.Add(n));
                }
            }
        }
        
        public void LoadDefaultSalesman()
        {
            using (var container = NestedContainer)
            {
                ISalesmanRouteRepository _salesmanRouteService = Using<ISalesmanRouteRepository>(container);
                IUserRepository _userService = Using<IUserRepository>(container);
                Salesmen.Clear();
                var salesman = new Salesman
                                   {
                                       Id = Guid.Empty,
                                       Username = GetLocalText("sl.dispatchorders.salesman.default")
                                       /*"--Please Select a Salesman--"*/
                                   };
                Salesmen.Add(salesman);
                SelectedSalesman = salesman;
                OverallRecipient = salesman;

                string[] salesmenwarehouses =
                    _salesmanRouteService.GetAll().Where(n => n.Route.Id == SelectedRoute.Id).Select(
                        c => c.DistributorSalesmanRef.Id.ToString()).ToArray();
                _userService.GetAll().Where(u => salesmenwarehouses.Contains(u.CostCentre.ToString())).ToList().ForEach(
                    n => Salesmen.Add(new Salesman {Id = n.Id, Username = n.Username}));
            }

        }

        void LoadData()
        {
            using (var container = NestedContainer)
            {
                IRouteRepository _routeService = Using<IRouteRepository>(container);
                IUserRepository _userService = Using<IUserRepository>(container);
                var _costCentreService = Using<ICostCentreRepository>(container);

                Routes.Clear();
                var route = new DistributorRoute
                                {
                                    Id = Guid.Empty,
                                    Name = GetLocalText("sl.dispatchorders.route.default")
                                    /*"--Please Select a Route--"*/
                                };
                Routes.Add(route);
                SelectedRoute = route;
                _routeService.GetAll().ToList().ForEach(n => Routes.Add(new DistributorRoute {Id = n.Id, Name = n.Name}));

                _allCostCentres = _costCentreService.GetAll().ToList();
                _allSalesmen = _userService.GetAll().Where(n => n.UserType == UserType.DistributorSalesman).ToList();
            }


        }

        void LoadAllSalesmen()
        {
            using (var container = NestedContainer)
            {

                IUserRepository _userService = Using<IUserRepository>(container);
               

                Salesmen.Clear();
                var salesman = new Salesman {Id = Guid.Empty, Username = "--Please Select a Salesman--"};
                Salesmen.Add(salesman);
                SelectedSalesman = salesman;
                OverallRecipient = salesman;
                _userService.GetAll().Where(n => n.UserType == UserType.DistributorSalesman).ToList().ForEach(
                    n => Salesmen.Add(new Salesman {Id = n.Id, Username = n.Username}));
            }
        }

        void LoadRoutesSalesmen()
        {
            using (var container = NestedContainer)
            {
                ISalesmanRouteRepository _salesmanRouteService = Using<ISalesmanRouteRepository>(container);
               
                if (SelectedRoute != null)
                {
                    if (SelectedRoute.Id != Guid.Empty)
                    {
                        Salesmen.Clear();
                        string[] salesmenwarehouses =
                            _salesmanRouteService.GetAll().Where(n => n.Route.Id == SelectedRoute.Id).Select(
                                c => c.DistributorSalesmanRef.Id.ToString()).ToArray();
                        _allSalesmen.Where(u => salesmenwarehouses.Contains(u.CostCentre.ToString())).OrderBy(
                            n => n.Username).ToList().ForEach(
                                n => Salesmen.Add(new Salesman {Id = n.Id, Username = n.Username}));
                    }
                }
            }
        }

        void LoadRoutesSalesmen(Route route, User user)
        {
            using (var container = NestedContainer)
            {
                ISalesmanRouteRepository _salesmanRouteService = Using<ISalesmanRouteRepository>(container);

                if (route != null)
                {
                    string[] salesmenwarehouses =
                        _salesmanRouteService.GetAll().Where(n => n.Route != null && n.Route.Id == route.Id).Select(
                            c => c.DistributorSalesmanRef.Id.ToString()).ToArray();
                    var orderRouteSalesmen =
                        _allSalesmen.Where(u => salesmenwarehouses.Contains(u.CostCentre.ToString())).ToList();
                    if (orderRouteSalesmen != null)
                    {
                        if (orderRouteSalesmen.Count > 0)
                        {
                            orderRouteSalesmen.Where(n => (!Salesmen.Select(x => x.Id).Contains(n.Id))).ToList().ForEach
                                (
                                    n => Salesmen.Add(new Salesman {Id = n.Id, Username = n.Username}));
                        }
                        else
                        {
                            if (!Salesmen.Any(n => n.Id == user.Id))
                            {
                                Salesmen.Add(new Salesman {Id = user.Id, Username = user.Username});
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Order route is null.");
                }
                Salesmen.Where(n => n.Id != Guid.Empty);
            }
        }


        private void RunSetupForDispatch()
        {
            DispatchAnyway = false;
            ProcessAndDispatch = false;
            ProcessBackOrder = false;
            ForDispatch = true;
            LoadData();
            //LoadPendingOrders();
        }

        void RunLoadSalesmansPendingOrder()
        {
            LoadPendingOrders();
        }

        public void RunValidateBackOrdersForDispatch()
        {
            using (var container = NestedContainer)
            {
                IConfigService _configService = Using<IConfigService>(container);

                OrdersWithBackOrder = new List<ListOrderViewModelItem>();
                FulfilledOrders = new List<ListOrderViewModelItem>();
                FulfillableOrdersWithBackOrder = new List<ListOrderViewModelItem>();
                PartiallyDispatchableOrders = new List<ListOrderViewModelItem>();

                CostCentreId = _configService.Load().CostCentreId;
                var orders = (from ListOrderViewModelItem item in Orders
                              where item.Dispatch
                              select item).ToList();

                //run check for each to see if they have back order and if the back order can be met
                FulfilledOrders = orders.Where(n => !n.HasBackOrder).ToList();
                OrdersWithBackOrder = orders.Where(n => n.HasBackOrder).ToList();
                FulfillableOrdersWithBackOrder = ValidateBackOrdersForDispatch(OrdersWithBackOrder).ToList();
            }
        }

        //revision
        ///1. Dipatch Fullfilled - dispatch orders which do not have back order,
            ///2. Process Fullfillable And Dispatch - should process the back order which can be fulfilled 
            /// - order status is pending dispatch ,
            /// - process the back order
            /// -- modify the order line items without first changing the order status
            /// - join the order without back order with those whose back order has been processed
            /// - dispatch all  
            ///3. Process And Dispatch All, 4. Cancel
            /// - process fulfillable back orders
            /// - dispatch all selected orders with partial dispatch for those wich cannot be processed
            /// 
        void RunDispatchCommandRevised()
        {
                var orders = (from ListOrderViewModelItem item in Orders
                              where item.Dispatch
                              select item).ToList();

                //var ord = orders.Select(n => new
                //                                 {
                //                                     id = n.OrderId,
                //                                     Desc = n.DocumentRef
                //                                 }); // note : wow list

            switch(SelectedDispatchMode)
            {
                case EnumSelectedDispatchMode.DispatchFulfilled: //1.
                    DispatchOrders(orders.Where(n => n.HasBackOrder == false));
                    break;
                case EnumSelectedDispatchMode.ProcessFulfillableAndDispatchFulfilled://2.
                    if (FulfillableOrdersWithBackOrder.Count() > 0)
                        ProcessBackOrders(FulfillableOrdersWithBackOrder);
                    var fulfilledOrders =
                        FulfillableOrdersWithBackOrder.Union(orders.Where(n => !n.HasBackOrder));
                    DispatchOrders(fulfilledOrders);
                    break;
                case EnumSelectedDispatchMode.ProcessAndDispatchAll: //3
                    if (FulfillableOrdersWithBackOrder.Count() > 0)
                        ProcessBackOrders(FulfillableOrdersWithBackOrder);
                    var fulfilledOrders2 = FulfillableOrdersWithBackOrder.Union(orders.Where(n => !n.HasBackOrder));
                    if (UnFulfillableOrdersWithBackOrder.Count() > 0)
                        fulfilledOrders2 = fulfilledOrders2.Union(UnFulfillableOrdersWithBackOrder);
                    DispatchOrders(fulfilledOrders2, true);
                    break;
                case EnumSelectedDispatchMode.DispatchWithPartialDispatch:
                    DispatchOrders(orders, true);
                    break;
            }
        }

        void RunDispatchCommand()
        {
            var orders = (from ListOrderViewModelItem item in Orders
                          where item.Dispatch
                          select item).ToList();
            //options
            //1. dispatch orders without back order only (Dispatch Anyway)
            //2. dispatch orders without back order + those with back order and can be satisfied =>back order will be processed (Quick Process And Dispatch)
            //3. Process back orders first => Navigate to BackOrders List (Process Back Orders)
            //
            //1.
            if (DispatchAnyway)
            {
                DispatchOrders(orders.Where(n => n.HasBackOrder == false));
            }
            //2.
            else if (ProcessAndDispatch)
            {
                //process back order items
                ProcessBackOrders(FulfillableOrdersWithBackOrder);
                DispatchOrders(orders);//cn: .Where(n => n.HasBackOrder == false) can dispatch available qty => will perform partial dispatch
            }
            //3.
            else if (ProcessBackOrder)
            {
                SendNavigationRequestMessage(new Uri("/salesmanorder/listsalesmanorder?BackOrders",
                                                     UriKind.Relative));
            }

            //LoadPendingOrders();
        }

        public int dispatchCount = 0;
        void DispatchOrders(IEnumerable<ListOrderViewModelItem> orders, bool withPartialDispatch = false)
        {
            using (var container = NestedContainer)
            {
                IOrderRepository _orderService = Using<IOrderRepository>(container);
                IUserRepository _userService = Using<IUserRepository>(container);
                var _costCentreService = Using<ICostCentreRepository>(container);
                dispatchCount = 0;
                List<Order> list = new List<Order>();
                OrdersThatCannotBeDispatched = new List<Order>();

                if (withPartialDispatch)
                {
                    foreach (var item in orders.Where(n => n.HasBackOrder))
                    {
                        var order = _orderService.GetById(item.OrderId) as Order;
                        if (CanDispatchThisOrder(order))
                        {
                            continue;
                        }
                    }

                    if (OrdersThatCannotBeDispatched.Count > 0) //filter ...
                    {
                        orders = orders.Where(o => (!OrdersThatCannotBeDispatched.Select(x => x.Id).Contains(o.OrderId)));
                    }
                }

                //validate for passed order but with 0 qty to deliver

                foreach (var item in orders)
                {
                    //get order

                    var order = _orderService.GetById(item.OrderId) as Order;

                    if (!ShouldDispatchThisOrder(order))
                    {
                        OrdersThatCannotBeDispatched.Add(order);
                        continue;
                    }

                    //reset recipient

                    User salesman = _allSalesmen.First(n => n.Id == item.SelectedRecipient.Id) ??
                                    _userService.GetById(item.SelectedRecipient.Id);

                    if (order.DocumentRecipientCostCentre.Id != salesman.CostCentre)
                    {
                        var newCC = _allCostCentres.First(n => n.Id == salesman.CostCentre) ??
                                    _costCentreService.GetById(salesman.CostCentre);

                        order.DocumentRecipientCostCentre = newCC;

                        _orderService.Save(order);

                        order = _orderService.GetById(order.Id) as Order;
                    }


                    var orderToremove = Orders.First(p => p.OrderId == order.Id);
                    Orders.Remove(orderToremove);
                    list.Add(order);
                    dispatchCount += 1;
                }
                foreach (Order order in list)
                {
                    Using<IApproveSalesmanOrderWFManager>(container).DispatchOrder(order);
                }
            }
        }

        

        void RunSetRecipientCommand()
        {
            if (Orders != null)
            {
                var allOrders = (from ListOrderViewModelItem item in Orders
                                 select item).ToList();

                foreach (var item in allOrders.Where(item => item.Dispatch))
                {
                    item.SelectedRecipient = Salesmen.FirstOrDefault(n => n.Id == OverallRecipient.Id);
                }
                Orders.Clear();
                allOrders.ToList().ForEach(f => Orders.Add(f));
            }
        }

        void RunUnsetRecipientCommand()
        {
            LoadPendingOrders();
        }

        void RunSelectViewerAndGo()
        {
            using (var container = NestedContainer)
            {
                IOrderRepository _orderService = Using<IOrderRepository>(container);

                var orderDocumentStatus = _orderService.GetById(OrderIdLookup).Status;
                if (orderDocumentStatus == DocumentStatus.Confirmed || orderDocumentStatus == DocumentStatus.New)
                    //view with EditViewModel
                {
                    SendNavigationRequestMessage(new Uri(
                                                     "/views/salesmanorders/editsalesmanorder.xaml?orderid=" +
                                                     OrderIdLookup +
                                                     "&loadforviewing=" + true,
                                                     UriKind.Relative));
                }
                else if (orderDocumentStatus == DocumentStatus.Approved ||
                         orderDocumentStatus == DocumentStatus.OrderPendingDispatch ||
                         orderDocumentStatus == DocumentStatus.OrderDispatchedToPhone ||
                         orderDocumentStatus == DocumentStatus.Cancelled ||
                         orderDocumentStatus == DocumentStatus.Rejected ||
                         orderDocumentStatus == DocumentStatus.Closed)
                {
                    SendNavigationRequestMessage(
                        new Uri(
                            "/views/salesmanorders/approvesalemanorders.xaml?orderid=" + OrderIdLookup +
                            "&loadforprocessing=" +
                            false, UriKind.Relative));
                }
            }
        }

        public bool RunOrderIsSelected(ListOrderViewModelItem selectedOrder)
        {
            if (selectedOrder == null)
                return false; ;

            if (!CheckRecipientIsActive(selectedOrder.SelectedRecipient))
            {
                MessageBox.Show(
                    "Recipient is deactivated. Please reassign the order to active recipient then dispatch.",
                    "Distributr: Cannot Dispatch Order to " + selectedOrder.SelectedRecipient.Username,
                    MessageBoxButton.OK);
                return false;
            }
            selectedOrder.Dispatch = true;
            return true;
        }

        public void RunOrderIsUnSelected(ListOrderViewModelItem selectedOrder)
        {

            selectedOrder.Dispatch = false;
        }

        bool CheckRecipientIsActive(Salesman salesman)
        {
            using (var container = NestedContainer)
            {
                IUserRepository _userService = Using<IUserRepository>(container);

                var salesmanUser = _userService.GetById(salesman.Id, true);
                if (salesmanUser != null && salesmanUser._Status == EntityStatus.Active)
                    return true;
                return false;
            }
        }

        void RunSelectAllCommand()
        {
            foreach (var item in Orders)
            {
                item.Dispatch = true;
            }
        }

        void RunUnSelectAllCommand()
        {
            foreach (var item in Orders)
            {
                RunOrderIsUnSelected(item);
            }
        }

        public void ProcessBackOrders(List<ListOrderViewModelItem> fulfillableOrdersWithBackOrder)
        {
            using (var container = NestedContainer)
            {
                IOrderRepository _orderService = Using<IOrderRepository>(container);
                IApproveSalesmanOrderWFManager _approveSalesmanOrderWFManager = Using<IApproveSalesmanOrderWFManager>(container);
                

                foreach (var item in fulfillableOrdersWithBackOrder)
                {
                    var order = _orderService.GetById(item.OrderId) as Order;
                    foreach (var lineItem in order.LineItems.Where(n => n.LineItemType == OrderLineItemType.BackOrder))
                    {
                        //create or update
                        var existingProcessedBackOrder =
                            order.LineItems.FirstOrDefault(
                                n =>
                                n.Product.Id == lineItem.Product.Id &&
                                n.LineItemType == OrderLineItemType.ProcessedBackOrder);
                        if (existingProcessedBackOrder != null && existingProcessedBackOrder.Qty == lineItem.Qty)
                        {
                            continue;
                        }
                        else if (existingProcessedBackOrder != null && existingProcessedBackOrder.Qty != lineItem.Qty)
                        {
                            existingProcessedBackOrder.Qty += lineItem.Qty;
                        }
                        else if (existingProcessedBackOrder == null)
                        {
                            order.CreateLineItem(new OrderLineItem(Guid.NewGuid())
                                                     {
                                                         Description = lineItem.Description,
                                                         IsNew = true,
                                                         LineItemSequenceNo = order.LineItems.Count() + 1,
                                                         LineItemType = OrderLineItemType.ProcessedBackOrder,
                                                         LineItemVatValue = lineItem.LineItemVatValue,
                                                         Product = lineItem.Product,
                                                         Qty = lineItem.Qty,
                                                         Value = lineItem.Value,
                                                     });
                        }
                    }
                    _approveSalesmanOrderWFManager.ProcessBackOrder(order);
                }
            }
        }

        public IEnumerable<ListOrderViewModelItem> ValidateBackOrdersForDispatch(IEnumerable<ListOrderViewModelItem> orders)
        {
            using (var container = NestedContainer)
            {
                IOrderRepository _orderService = Using<IOrderRepository>(container);

                var fulfillableOrders = new List<ListOrderViewModelItem>();
                UnFulfillableOrdersWithBackOrder = new List<ListOrderViewModelItem>();

                foreach (var item in orders)
                {
                    int unfulfillableLiCnt = 0;
                    int dispatchableLiCnt = 0;

                    //check if the back order can be covered
                    var order = _orderService.GetById(item.OrderId) as Order;

                    bool orderIsFulfillable = false;
                    _productInventoryHistory = new Dictionary<Guid, decimal>();
                    foreach (var li in order.LineItems)
                    {
                        if (li.LineItemType == OrderLineItemType.BackOrder)
                        {
                            if (ValidateMyBackOrder(li.Product.Id, li.Qty))
                            {
                            }
                            else
                            {
                                unfulfillableLiCnt += 1;
                            }
                        }

                        OrderLineItem liBackOrder = null;
                        OrderLineItem liProcessedBackOrder = null;

                        try
                        {
                            liBackOrder =
                                order.LineItems.First(
                                    n =>
                                    n.Description == li.Id.ToString() && n.LineItemType == OrderLineItemType.BackOrder);
                        }
                        catch
                        {
                        }
                        try
                        {
                            liProcessedBackOrder =
                                order.LineItems.First(
                                    n =>
                                    n.Description == li.Id.ToString() &&
                                    n.LineItemType == OrderLineItemType.ProcessedBackOrder);
                        }
                        catch
                        {
                        }

                        var boQty = liBackOrder != null ? liBackOrder.Qty : 0;
                        var processedBoQty = liProcessedBackOrder != null ? liProcessedBackOrder.Qty : 0;

                        if ((li.Qty - (boQty - processedBoQty)) > 0)
                        {
                            dispatchableLiCnt += 1;
                        }

                        if (unfulfillableLiCnt == 0)
                            orderIsFulfillable = true;
                        else
                            orderIsFulfillable = false;
                    }

                    if (orderIsFulfillable)
                    {
                        fulfillableOrders.Add(item);
                    }
                    else
                    {
                        UnFulfillableOrdersWithBackOrder.Add(item);
                    }
                    if (dispatchableLiCnt > 0)
                    {
                        PartiallyDispatchableOrders.Add(item);
                    }
                }

                return fulfillableOrders;
            }
        }

        private Dictionary<Guid, decimal> _productInventoryHistory;
        bool ValidateMyBackOrder(Guid productId, decimal backOrderQty)
        {
            using (var container = NestedContainer)
            {
                var _inventoryService = Using<IInventoryRepository>(container);

                int invalidCnt = 0;
                bool valid = false;
                //get inventory level for the product
                if (_productInventoryHistory.Any(i => i.Key == productId))
                {
                    var tempInv = _productInventoryHistory.First(n => n.Key == productId).Value;
                    valid = backOrderQty <= _productInventoryHistory.First(n => n.Key == productId).Value;

                    //update dict
                    _productInventoryHistory.Remove(productId);
                    _productInventoryHistory.Add(productId, (tempInv - backOrderQty));

                    if (!valid)
                        invalidCnt += 1;
                }
                else
                {
                    var availableInv = _inventoryService.GetByProductIdAndWarehouseId(productId, CostCentreId);
                    valid = backOrderQty <= (availableInv != null ? availableInv.Balance : 0);
                    //insert in dict
                    _productInventoryHistory.Add(productId,
                                                 (availableInv != null ? availableInv.Balance : 0 - backOrderQty));

                    if (!valid)
                        invalidCnt += 1;
                }
                if (invalidCnt > 0)
                    valid = false;
                if (invalidCnt == 0)
                    valid = true;

                return valid;
            }
        }

        public List<Order> OrdersThatCannotBeDispatched = null;
        bool CanDispatchThisOrder(Order order)//validate orders with unprocessed bo
        {
            int itemCanBeDispatchedCount = 0;
            bool canDispatch = false;

            foreach (var item in order.LineItems.Where(
                                n =>
                                n.LineItemType != OrderLineItemType.LostSale
                                && n.LineItemType != OrderLineItemType.BackOrder
                                && n.LineItemType != OrderLineItemType.ProcessedBackOrder
                                //&& n.Product.GetType() != typeof(ReturnableProduct)
                                ))
            {
                OrderLineItem itemBackOrder = null;
                OrderLineItem itemProcessedBackOrder = null;
                try
                {
                    itemBackOrder =
                        order.LineItems.First(
                            n => n.Description == (item.LineItemType == OrderLineItemType.Discount ? item.Id.ToString() : item.Description) && n.LineItemType == OrderLineItemType.BackOrder);
                }
                catch
                {
                }
                try
                {
                    itemProcessedBackOrder =
                        order.LineItems.First(
                            n =>
                            n.Description == (item.LineItemType == OrderLineItemType.Discount ? item.Id.ToString() : item.Description) && n.LineItemType == OrderLineItemType.ProcessedBackOrder);
                }
                catch
                {
                }

                var itemBackOrderQty = itemBackOrder != null ? itemBackOrder.Qty : 0;
                var itemProcessedBackOrderQty = itemProcessedBackOrder != null ? itemProcessedBackOrder.Qty : 0;
                if (item.Qty - (itemBackOrderQty - itemProcessedBackOrderQty) > 0)
                {
                    itemCanBeDispatchedCount += 1;
                }
            }

            if (itemCanBeDispatchedCount > 0)
                canDispatch = true;
            else if (itemCanBeDispatchedCount == 0)
                canDispatch = false;

            if (!canDispatch)
                OrdersThatCannotBeDispatched.Add(order);

            return canDispatch;
        }

        bool ShouldDispatchThisOrder(Order order)
        {
            int dispatchableLineItemsCnt = 0;
            bool dispatch = false;
            foreach (var li in order.LineItems.Where(
                                n =>
                                n.LineItemType != OrderLineItemType.LostSale
                                && n.LineItemType != OrderLineItemType.BackOrder
                                && n.LineItemType != OrderLineItemType.ProcessedBackOrder
                                ))
            {
                decimal liBackOrderQty = 0, liLostSaleQty = 0, liProcessedBackOrderQty = 0;
                OrderLineItem liBackOrder = null;
                try
                {
                    liBackOrder =
                        order.LineItems.First(
                        n => n.Description == (li.LineItemType == OrderLineItemType.Discount ? li.Id.ToString() : li.Description) && n.LineItemType == OrderLineItemType.BackOrder);
                }
                catch { }
                OrderLineItem liLostSale = null;
                try
                {
                    liLostSale =
                    order.LineItems.First(
                        n => n.Description == (li.LineItemType == OrderLineItemType.Discount ? li.Id.ToString() : li.Description) && n.LineItemType == OrderLineItemType.LostSale);
                }
                catch { }

                OrderLineItem liProcessedBackOrder = null;
                try
                {
                    liProcessedBackOrder =
                    order.LineItems.First(
                        n => n.Description == (li.LineItemType == OrderLineItemType.Discount ? li.Id.ToString() : li.Description) && n.LineItemType == OrderLineItemType.ProcessedBackOrder);
                }
                catch { }

                if (liBackOrder != null)
                    liBackOrderQty = liBackOrder.Qty;
                if (liLostSale != null)
                    liLostSaleQty = liLostSale.Qty;
                if (liProcessedBackOrder != null)
                    liProcessedBackOrderQty = liProcessedBackOrder.Qty;

                if (li.Qty - (liBackOrderQty - liProcessedBackOrderQty) > 0)
                    dispatchableLineItemsCnt += 1;
            }

            if (dispatchableLineItemsCnt > 0)
                dispatch = true;

            return dispatch;
        }

        public Guid getInvoiceId(Guid orderId)
        {
            using (var container = NestedContainer)
            {
                IInvoiceRepository _invoiceService = Using<IInvoiceRepository>(container);

                try
                {
                    return InvoicesList.First(n => n.OrderId == orderId).Id;
                }
                catch //if no matcher
                {
                    return _invoiceService.GetInvoiceByOrderId(orderId).Id;
                }
            }
        }

        #endregion

        #region Helper Classes

        public class ReportsListViewModel : ViewModelBase
        {
            public Guid OrderId { get; set; }
            public string SequenceNo { get; set; }
            public const string DocumentRefPropertyName = "DocumentRef";
            public string DocumentRef { get; set; }
            public string DateRequired { get; set; }
            public string CreatedBy { get; set; }
            public string DocIssuerInfo { get; set; }
            public string Status { get; set; }
            public Decimal TotalNet { get; set; }
            public Decimal TotalVat { get; set; }
            public Decimal TotalGross { get; set; }
        }

        public class ListOrderViewModelItem : ViewModelBase
        {
            public Guid OrderId { get; set; }
            public int SequenceNo { get; set; }
            public const string DocumentRefPropertyName = "DocumentRef";
            public string DocumentRef { get; set; }
            public string DateRequired { get; set; }
            public string CreatedBy { get; set; }
            public string DocIssuerInfo { get; set; }

            public const string StatusPropertyName = "Status";
            private string _status = "";
            public string Status
            {
                get
                {
                    return _status == "Closed" ? "Delivered" : _status;
                }

                set
                {
                    if (_status == value)
                    {
                        return;
                    }

                    var oldValue = _status;
                    _status = value;
                    RaisePropertyChanged(StatusPropertyName);
                }
            }

            public string TotalNet { get; set; }
            public string TotalVat { get; set; }
            public string TotalGross { get; set; }
            public string TotalPaid { get; set; }
            public string TotalDue { get; set; }
            public bool IsEditable { get { return (Status == "New"); } }// || Status == "Confirmed"
            public bool IsProcessable { get { return (Status == "Confirmed" || Status == "New"); } }
            public bool IsApproved { get; set; }
            
            public const string DispatchPropertyName = "Dispatch";
            private bool _dispatch = false;
            public bool Dispatch
            {
                get
                {
                    return _dispatch;
                }

                set
                {
                    if (_dispatch == value)
                    {
                        return;
                    }

                    _dispatch = value;
                    RaisePropertyChanged(DispatchPropertyName);
                }
            }

            public string chkDispatchContent{get; set;}
            public bool HasBackOrder { get; set; }
            public ObservableCollection<Salesman> Recipients { get; set; }
            public Salesman SelectedRecipient { get; set; }
            public string SalesmanUsername { get { return SelectedRecipient.Username; }}

            public const string HlkProcessContentPropertyName = "HlkProcessContent";
            private string _hlkProcessContent = "Process";
            public string HlkProcessContent
            {
                get
                {
                    if(Status == "Confirmed")
                        _hlkProcessContent = "Process";
                    if(Status == "New")
                        _hlkProcessContent = "Confirm";

                    return _hlkProcessContent;
                }

                set
                {
                    if (_hlkProcessContent == value)
                    {
                        return;
                    }

                    var oldValue = _hlkProcessContent;
                    _hlkProcessContent = value;

                    RaisePropertyChanged(HlkProcessContentPropertyName);
                }
            }

            public const string HlkViewContentPropertyName = "HlkViewContent";
            private string _hlkViewContent = "View";
            public string HlkViewContent
            {
                get
                {
                  
                    return _hlkViewContent;
                }

                set
                {
                    if (_hlkViewContent == value)
                    {
                        return;
                    }

                    var oldValue = _hlkViewContent;
                    _hlkViewContent = value;
                    RaisePropertyChanged(HlkViewContentPropertyName);
                }
            }

            public string EmptyString { get { return ""; } }
        }

        public class Salesman
        {
            public Guid Id { get; set; }
            public string Username { get; set; }
        }

        public class DistributorRoute
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class UnfulfillableOrder
        {
            public Guid OrderId { get; set; }
            string OrderReference { get; set; }
        }

        public class UnfulfillableOrderLineItem
        {
            public Guid ProdcutId { get; set; }
            public int RequiredQty { get; set; }
            public int AvailableQty { get; set; }
            public string ProductDesc { get; set; }
        }
        #endregion
    }
}