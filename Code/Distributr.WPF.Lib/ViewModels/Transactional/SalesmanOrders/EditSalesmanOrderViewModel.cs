using System.Windows;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.WorkFlow.AuditLogs;
using Distributr.WPF.Lib.Services.WorkFlow.Discount;
using Distributr.WPF.Lib.Services.WorkFlow.GetDocumentReferences;
using Distributr.WPF.Lib.Services.WorkFlow.Orders;
using Distributr.WPF.Lib.UI.Hierarchy;
using Distributr.WPF.Lib.ViewModels.Transactional.Purchasing;
using Distributr.WPF.Lib.WorkFlow.Orders;
using GalaSoft.MvvmLight.Command;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using System.Collections.ObjectModel;
using Distributr.Core.Domain.Master.CostCentreEntities;
using System.Linq;
using System;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Master.UserEntities;
using System.Collections.Generic;
using StructureMap;

namespace Distributr.WPF.Lib.ViewModels.Transactional.SalesmanOrders
{
    public class EditSalesmanOrderViewModel : DistributrViewModelBase
    {
        #region Declarations

       

        public bool editing = false;
        private ApproveSalesmanOrderViewModel _vm;
        private Order _order = null;
        public bool FireSalesmanChangedCmd = true;
        public bool FireRouteChangedCmd = true;
        public bool FireOutletChangeCmd = true;
        public List<EditSalesmanOrderItem> RemovedLineItems;
        IProductPackagingSummaryService _productPackagingSummaryService;
        #endregion

        public EditSalesmanOrderViewModel()
        {
            
            _vm = new ApproveSalesmanOrderViewModel();
            _productPackagingSummaryService = ObjectFactory.GetInstance<IProductPackagingSummaryService>();

            LoadOrderCommand = new RelayCommand(DoLoad);
            RouteChangedCommand = new RelayCommand(RouteChanged);
            LineItems = new ObservableCollection<EditSalesmanOrderItem>();
            RemovedLineItems = new List<EditSalesmanOrderItem>();
            CancelCommand = new RelayCommand(Cancel);
            ConfirmCommand = new RelayCommand(ConfirmOrder);
            ConfirmAndApproveCommand = new RelayCommand(RunConfirmAndApproveCommand);
            SalesmanChangedCommand = new RelayCommand(SalesmanChanged);
            ValidateOrderForApprovalCommand = new RelayCommand(RunValidateOrderForApproval);
            ApproveOrderCommand = new RelayCommand(RunApproveOrderCommand);
            CreateInvalidOrdersMessageCommand = new RelayCommand(RunCreateInvalidOrdersMessageCommand);
            CreateBackOrderAndApproveCommand = new RelayCommand(RunCreateBackOrderAndApproveCommand);
            CancelOrderCommand = new RelayCommand(RunCancelOrderCommand);

            Salesmen = new ObservableCollection<User>();
            DistributorRoutes = new ObservableCollection<Route>();
            RouteOutlets = new ObservableCollection<Outlet>();
        }

        #region Relay Cmds N Collections
        public RelayCommand LoadOrderCommand { get; set; }
        public RelayCommand RouteChangedCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand ConfirmCommand { get; set; }
        public RelayCommand ConfirmAndApproveCommand { get; set; }
        public RelayCommand SalesmanChangedCommand { get; set; }
        public RelayCommand ValidateOrderForApprovalCommand { get; set; }
        public RelayCommand ApproveOrderCommand { get; set; }
        public RelayCommand CreateInvalidOrdersMessageCommand { get; set; }
        public RelayCommand CreateBackOrderAndApproveCommand { get; set; }
        public RelayCommand CancelOrderCommand { get; set; }

        //public ObservableCollection<EditSalesmanOrderItem> LineItems { get; set; }
        public ObservableCollection<Outlet> RouteOutlets { get; set; }
        public ObservableCollection<Route> DistributorRoutes { get; set; }
        public ObservableCollection<EditSalesmanOrderItem> LineItems { get; set; }
        //public ObservableCollection<EditSalesmanOrderItem> _LineItems { get; set; }
        public ObservableCollection<User> Salesmen { get; set; }
        public ObservableCollection<Outlet> DistributorOutlets { get; set; }
        #endregion

        #region Properties

        public const string SelectedRoutePropertyName = "SelectedRoute";
        private Route _selectedRoute = null;
        [MasterDataDropDownValidation]
        public Route SelectedRoute
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
                //SelectedRouteName = _selectedRoute == null ? "" : _selectedRoute.Name;

                // Update bindings, no broadcast
                RaisePropertyChanged(SelectedRoutePropertyName);
            }
        }

        public const string SelectedSalesmanPropertyName = "SelectedSalesman";
        private User _selectedSalesman = null;
        [MasterDataDropDownValidation]
        public User SelectedSalesman
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
                SelectedSalesmanUserName = _selectedSalesman == null ? "" : _selectedSalesman.Username;

                RaisePropertyChanged(SelectedSalesmanPropertyName);
            }
        }

        public const string SelectedOutletPropertyName = "SelectedOutlet";
        private Outlet _selectedOutlet = null;
        [MasterDataDropDownValidation]
        public Outlet SelectedOutlet
        {
            get
            {
                return _selectedOutlet;
            }

            set
            {
                if (_selectedOutlet == value)
                {
                    return;
                }

                var oldValue = _selectedOutlet;
                _selectedOutlet = value;
                //SelectedOutletName = _selectedOutlet == null ? "" : _selectedOutlet.Name;

                // Update bindings, no broadcast
                RaisePropertyChanged(SelectedOutletPropertyName);
            }
        }

        public const string DateRequiredPropertyName = "DateRequired";
        private DateTime _dateRequired = DateTime.Now;
        public DateTime DateRequired
        {
            get
            {
                return _dateRequired;
            }

            set
            {
                if (_dateRequired == value)
                {
                    return;
                }
                var oldValue = _dateRequired;
                if (LoadForViewing)
                    _dateRequired = value;
                else if (LoadForEditing)
                {
                    _dateRequired = value < DateTime.Now
                                        ? DateTime.Now
                                        : (value > DateTime.Now.AddMonths(3) ? DateTime.Now : value);
                }
                // Update bindings, no broadcast
                RaisePropertyChanged(DateRequiredPropertyName);
            }
        }

        public const string NowPropertyName = "Now";
        private DateTime _now = DateTime.Now;
        public DateTime Now
        {
            get
            {
                return _now;
            }

            set
            {
                if (_now == value)
                {
                    return;
                }

                var oldValue = _now;
                _now = value;
                RaisePropertyChanged(NowPropertyName);
            }
        }

        public const string TotalNetPropertyName = "TotalNet";
        private decimal _totalNet = 0;
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
                RaisePropertyChanged(TotalNetPropertyName);
            }
        }

        public const string TotalVatPropertyName = "TotalVat";
        private decimal _totalVat = 0;
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
                RaisePropertyChanged(TotalVatPropertyName);
            }
        }

        public const string TotalGrossPropertyName = "TotalGross";
        private decimal _totalGross = 0;
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
                RaisePropertyChanged(TotalGrossPropertyName);
            }
        }

        public const string DocumentRefPropertyName = "DocumentRef";
        private string _documentRef = "";
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
                RaisePropertyChanged(DocumentRefPropertyName);
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

        public const string OrderIdPropertyName = "OrderId";
        private string _orderId = "";
        public string OrderId
        {
            get
            {
                return _orderId;
            }

            set
            {
                if (_orderId == value)
                    return;
                var oldValue = _orderId;
                _orderId = value;
                RaisePropertyChanged(OrderIdPropertyName);
            }
        }

        public const string StatusPropertyName = "Status";
        private string _status = "";
        public string Status
        {
            get
            {
                return _status;
            }

            set
            {
                if (_status == value)
                    return;

                var oldValue = _status;
                _status = value;
                RaisePropertyChanged(StatusPropertyName);
            }
        }

        public const string CreatedByUserPropertyName = "CreatedByUser";
        private string _createdByUser = "";
        public string CreatedByUser
        {
            get
            {
                return _createdByUser;
            }

            set
            {
                if (_createdByUser == value)
                    return;

                var oldValue = _createdByUser;
                _createdByUser = value;
                RaisePropertyChanged(CreatedByUserPropertyName);
            }
        }

        public const string IsEditablePropertyName = "IsEditable";
        private bool _myProperty = true;
        public bool IsEditable
        {
            get
            {
                return _myProperty;
            }

            set
            {
                if (_myProperty == value)
                    return;
                var oldValue = _myProperty;
                _myProperty = value;
                RaisePropertyChanged(IsEditablePropertyName);
            }
        }

        public const string DateSubmittedPropertyName = "DateSubmitted";
        private DateTime _dateSubmitted = DateTime.MaxValue;
        public DateTime DateSubmitted
        {
            get
            {
                return _dateSubmitted;
            }

            set
            {
                if (_dateSubmitted == value)
                    return;
                var oldValue = _dateSubmitted;
                _dateSubmitted = value;
                RaisePropertyChanged(DateSubmittedPropertyName);
            }
        }

        public const string SelectedSalesmanUserNamePropertyName = "SelectedSalesmanUserName";
        private string _selectedSalesmanUsername = "";
        public string SelectedSalesmanUserName
        {
            get
            {
                return _selectedSalesmanUsername;
            }

            set
            {
                if (_selectedSalesmanUsername == value)
                {
                    return;
                }

                var oldValue = _selectedSalesmanUsername;
                _selectedSalesmanUsername = value;

                RaisePropertyChanged(SelectedSalesmanUserNamePropertyName);
            }
        }

        public const string SelectedRouteNamePropertyName = "SelectedRouteName";
        private string _selectedRouteName = "";
        public string SelectedRouteName
        {
            get
            {
                return _selectedRouteName;
            }

            set
            {
                if (_selectedRouteName == value)
                {
                    return;
                }

                var oldValue = _selectedRouteName;
                _selectedRouteName = value;

                RaisePropertyChanged(SelectedRouteNamePropertyName);
            }
        }

        public const string SelectedOutletNamePropertyName = "SelectedOutletName";
        private string _selectedOutletName = "";
        public string SelectedOutletName
        {
            get
            {
                return _selectedOutletName;
            }

            set
            {
                if (_selectedOutletName == value)
                {
                    return;
                }

                var oldValue = _selectedOutletName;
                _selectedOutletName = value;

                RaisePropertyChanged(SelectedOutletNamePropertyName);
            }
        }

        public const string LoadForViewingPropertyName = "LoadForViewing";
        private bool _loadForViewing = false;
        public bool LoadForViewing
        {
            get
            {
                return _loadForViewing;
            }

            set
            {
                if (_loadForViewing == value)
                {
                    return;
                }

                var oldValue = _loadForViewing;
                _loadForViewing = value;
                RaisePropertyChanged(LoadForViewingPropertyName);
            }
        }

        public const string LoadForEditingPropertyName = "LoadForEditing";
        private bool _loadForEditing = true;
        public bool LoadForEditing
        {
            get
            {
                return _loadForEditing;
            }

            set
            {
                if (_loadForEditing == value)
                {
                    return;
                }

                var oldValue = _loadForEditing;
                _loadForEditing = value;
                RaisePropertyChanged(LoadForEditingPropertyName);
            }
        }

        public const string CancelButtonContentPropertyName = "CancelButtonContent";
        private string _cancelButtonContent = "Cancel";
        public string CancelButtonContent
        {
            get
            {
                return _cancelButtonContent;
            }

            set
            {
                if (_cancelButtonContent == value)
                {
                    return;
                }

                var oldValue = _cancelButtonContent;
                _cancelButtonContent = value;
                RaisePropertyChanged(CancelButtonContentPropertyName);
            }
        }

        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "";
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

        public const string IsEditingPropertyName = "IsEditing";
        private bool _isEditing = false;
        public bool IsEditing
        {
            get
            {
                return _isEditing;
            }

            set
            {
                if (_isEditing == value)
                {
                    return;
                }

                var oldValue = _isEditing;
                _isEditing = value;
                RaisePropertyChanged(IsEditingPropertyName);
            }
        }

        public const string setSelectedIdPropertyName = "setSelectedId";
        private Guid _setSelectedId = Guid.Empty;
        public Guid setSelectedId
        {
            get
            {
                return _setSelectedId;
            }

            set
            {
                if (_setSelectedId == value)
                {
                    return;
                }

                var oldValue = _setSelectedId;
                _setSelectedId = value;
                RaisePropertyChanged(setSelectedIdPropertyName);
            }
        }

        public const string PostConfirmVisiblePropertyName = "PostConfirmVisible";
        private bool _postConfirmVisible = false;
        public bool PostConfirmVisible
        {
            get
            {
                return _postConfirmVisible;
            }

            set
            {
                if (_postConfirmVisible == value)
                {
                    return;
                }

                var oldValue = _postConfirmVisible;
                _postConfirmVisible = value;
                RaisePropertyChanged(PostConfirmVisiblePropertyName);
            }
        }

        public const string ReceiveReturnablePropertyName = "ReceiveReturnable";
        private bool _receiveReturnable = false;
        public bool ReceiveReturnable
        {
            get
            {
                return _receiveReturnable;
            }

            set
            {
                if (_receiveReturnable == value)
                {
                    return;
                }

                var oldValue = _receiveReturnable;
                _receiveReturnable = value;
                RaisePropertyChanged(ReceiveReturnablePropertyName);
            }
        }

        public bool showConfirmMsg;

        public const string OrderIsValidForApprovalPropertyName = "OrderIsValidForApproval";
        private bool _orderIsValidForApproval = false;
        public bool OrderIsValidForApproval
        {
            get
            {
                return _orderIsValidForApproval;
            }

            set
            {
                if (_orderIsValidForApproval == value)
                {
                    return;
                }

                var oldValue = _orderIsValidForApproval;
                _orderIsValidForApproval = value;
                RaisePropertyChanged(OrderIsValidForApprovalPropertyName);
            }
        }

        public const string MessagePropertyName = "Message";
        private string _message = "";
        public string Message
        {
            get
            {
                return _message;
            }

            set
            {
                if (_message == value)
                {
                    return;
                }

                var oldValue = _message;
                _message = value;
                RaisePropertyChanged(MessagePropertyName);
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

        public const string LoadSalesmenProgressBarPropertyName = "LoadSalesmenProgressBar";
        private string _loadSalesmenProgressBar = "";
        public string LoadSalesmenProgressBar
        {
            get
            {
                return _loadSalesmenProgressBar;
            }

            set
            {
                if (_loadSalesmenProgressBar == value)
                {
                    return;
                }

                var oldValue = _loadSalesmenProgressBar;
                _loadSalesmenProgressBar = value;
                RaisePropertyChanged(LoadSalesmenProgressBarPropertyName);
            }
        }

        public const string ShowProgressPropertyName = "ShowProgress";
        private bool _showProgress = false;
        public bool ShowProgress
        {
            get
            {
                return _showProgress;
            }

            set
            {
                if (_showProgress == value)
                {
                    return;
                }

                var oldValue = _showProgress;
                _showProgress = value;
                RaisePropertyChanged(ShowProgressPropertyName);
            }
        }

        public const string ShowLoadSalesmenProgressPropertyName = "ShowLoadSalesmenProgress";
        private bool _showLoadSalesmenProgress = false;
        public bool ShowLoadSalesmenProgress
        {
            get
            {
                return _showLoadSalesmenProgress;
            }

            set
            {
                if (_showLoadSalesmenProgress == value)
                {
                    return;
                }

                var oldValue = _showLoadSalesmenProgress;
                _showLoadSalesmenProgress = value;
                RaisePropertyChanged(ShowLoadSalesmenProgressPropertyName);
            }
        }

        public const string HasDiscountPropertyName = "HasDiscount";
        private bool _hasDiscount = false;
        public bool HasDiscount
        {
            get
            {
                return _hasDiscount;
            }

            set
            {
                if (_hasDiscount == value)
                {
                    return;
                }

                var oldValue = _hasDiscount;
                _hasDiscount = value;
                RaisePropertyChanged(HasDiscountPropertyName);
            }
        }
         
        public const string SaleDiscountPropertyName = "SaleDiscount";
        private decimal _saleDiscount = 0m;
        public decimal SaleDiscount
        {
            get
            {
                return _saleDiscount;
            }

            set
            {
                if (_saleDiscount == value)
                {
                    return;
                }

                var oldValue = _saleDiscount;
                _saleDiscount = value;
                RaisePropertyChanged(SaleDiscountPropertyName);
            }
        }

        public const string TotalDiscountPropertyName = "TotalProductDiscount";
        private decimal _totalProductDiscount = 0m;
        public decimal TotalProductDiscount
        {
            get
            {
                return _totalProductDiscount;
            }

            set
            {
                if (_totalProductDiscount == value)
                {
                    return;
                }

                var oldValue = _totalProductDiscount;
                _totalProductDiscount = value;
                RaisePropertyChanged(TotalDiscountPropertyName);
            }
        }
         
        public const string DiscountLookUpPropertyName = "DiscountLookUp";
        private Guid _discountLookUp = Guid.Empty;
        public Guid DiscountLookUp
        {
            get
            {
                return _discountLookUp;
            }

            set
            {
                if (_discountLookUp == value)
                {
                    return;
                }

                var oldValue = _discountLookUp;
                _discountLookUp = value;
                RaisePropertyChanged(DiscountLookUpPropertyName);
            }
        }

        #endregion

        #region Methods

        void ConfirmOrder()
        {
            using (var container = NestedContainer)
            {
                ISalesmanOrderWFManager _orderWFManager = Using<ISalesmanOrderWFManager>(container);

                try
                {
                    _order = CreateOrUpdateOrderInMemory();
                    if (_order != null)
                    {
                        _order.Confirm();
                        _orderWFManager.SubmitChanges(_order);
                    }

                    if (showConfirmMsg)
                        MessageBox.Show("Order " + _vm.OrderId + " has been successfully confirmed.",
                                        "Distriburt: Order on Behalf of " + SelectedSalesman.Username,
                                        MessageBoxButton.OK);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "An error was encountered while confirming Order " + OrderId + "\nError Details:\n" + ex + ".",
                        "Distriburt: Order on Behalf of " + SelectedSalesman.Username, MessageBoxButton.OK);
                }
            }
        }

        void RunCancelOrderCommand()
        {
            using (var container = NestedContainer)
            {
                ISalesmanOrderWFManager _orderWFManager = Using<ISalesmanOrderWFManager>(container);
                IOrderRepository _orderService = Using<IOrderRepository>(container);
                IAuditLogWFManager _auditLogWFManager = Using<IAuditLogWFManager>(container);
                _orderService.CancelDocument(OrderIdLookup);
                _auditLogWFManager.AuditLogEntry("Outlet Orders",
                                                 string.Format("Cancelled order: {0}", OrderId));
            }
        }

        void RunConfirmAndApproveCommand()
        {
            showConfirmMsg = false;
            try
            {
                ConfirmOrder();
                _vm.ShowSuccessMessage = false;
                _vm.OrderIdLookup = OrderIdLookup;
                _vm.LoadOrderCommand.Execute(null);
                _vm.ApproveCommand.Execute(null);

                string msg ="";
                if (_vm.IsApproved)
                    msg = "Order " + _vm.OrderId + " has been successfully confirmed and approved.";
                else
                    msg = "Order " + _vm.OrderId + " has been successfully confirmed.";
                MessageBox.Show(msg,
                                "Distriburt: Order on Behalf of " + SelectedSalesman.Username, MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error was encountered while performing confirm and approve of Order " + OrderId + "\nError Details:\n" + ex + ".",
                   "Distriburt: Order on Behalf of " + SelectedSalesman.Username, MessageBoxButton.OK);
            }
            showConfirmMsg = true;
        }

        void RunValidateOrderForApproval()
        {
            _vm.OrderIdLookup = OrderIdLookup;
            _vm.LoadOrderCommand.Execute(null);
            OrderIsValidForApproval = false;
            _vm.ValidateOrderForApprovalCommand.Execute(null);
            OrderIsValidForApproval = _vm.Validated;
        }

        void RunApproveOrderCommand()
        {
            if (OrderIsValidForApproval)
            {
                _vm.OrderIdLookup = OrderIdLookup;
                _vm.LoadOrderCommand.Execute(null);
                _vm.ProcessingBackOrder = false;
                _vm.ApproveCommand.Execute(null);
            }
        }

        void RunCreateInvalidOrdersMessageCommand()
        {
            _vm.CreateInvalidOrdersMessageCommand.Execute(null);
            Message = _vm.Message;
        }

        void RunCreateBackOrderAndApproveCommand()
        {
            _vm.CreateBackOrderAndApproveCommand.Execute(null);
        }

        private void LoadData()
        {
            using (var container = NestedContainer)
            {
                IUserRepository _userService = Using<IUserRepository>(container);
                Salesmen.Clear();
                var salesman = new User(Guid.Empty)
                                   {
                                       Username = GetLocalText("sl.pos.selectsalesman")
                                       /*"--Please Select Salesman --"*/
                                   };
                Salesmen.Add(salesman);
                SelectedSalesman = salesman;
                _userService.GetByUserType(UserType.DistributorSalesman)
                            .OrderBy(n => n.Username).ToList()
                            .ForEach(n => Salesmen.Add(n));
            }
        }

        void DoLoad()
        {
                _productPackagingSummaryService.ClearBuffer();
                ClearViewModel();
                //check if order has orderId
                if (OrderIdLookup == Guid.Empty)
                {
                    PageTitle = "Add Order On Behalf Of Salesman";
                    OrderId = "New Order";
                    editing = false;

                    ShowProgress = true;
                    ShowProgress = false;
                    Status = DocumentStatus.New.ToString();
                    IsEditable = true;

                    ShowLoadSalesmenProgress = true;
                    LoadData();
                    ShowLoadSalesmenProgress = false;
                }
                else //load order for edit
                {
                    IsEditing = true;
                    PageTitle = "Edit Salesman's Order";
                    editing = true;
                    _order = GetEntityById(typeof(Order), OrderIdLookup) as Order;
                    IsEditable = false;
                    if (_order.Status == DocumentStatus.New)
                        IsEditable = true;
                    Status = _order.Status.ToString();
                    OrderId = _order.DocumentReference;
                    SaleDiscount = _order.SaleDiscount;
                    DateRequired = _order.DateRequired;
                    DateSubmitted = _order.DocumentDateIssued;
                    CreatedByUser = _order.DocumentIssuerUser == null
                                        ? string.Empty
                                        : _order.DocumentIssuerUser.Username;
                    LineItems.Clear();

                    foreach (var item in _order.LineItems)
                    {
                        bool isEditable = false;
                        if (item.LineItemType != OrderLineItemType.Discount &&
                            (item.Product is SaleProduct || item.Product is ConsolidatedProduct))
                        {
                            _productPackagingSummaryService.AddProduct(item.Product.Id, item.Qty, false, false, true);
                            isEditable = true;
                        }

                        AddLineItem(item.Product.Id, item.Product.Description, item.Value,
                                    item.LineItemVatValue, item.LineItemVatTotal, item.LineItemVatValue,
                                    item.Qty, item.LineItemTotal, IsEditable, item.Id,
                                    item.Product.GetType().ToString().Split('.').Last(),
                                    item.LineItemType, item.ProductDiscount, item.DiscountType);
                    }

                    //RefreshList();
                    using (StructureMap.IContainer container = NestedContainer)
                    {
                        IUserRepository _userService = Using<IUserRepository>(container);
                        ICostCentreRepository _costCentreService = Using<ICostCentreRepository>(container);
                        IRouteRepository _routeService = Using<IRouteRepository>(container);
                        IConfigService _configService = Using<IConfigService>(container);
                        Salesmen.Clear();
                        _userService.GetByUserType(UserType.DistributorSalesman).ForEach(n => Salesmen.Add(n));
                        if (_order.DocumentIssuerUser != null)
                        {
                            try
                            {
                                SelectedSalesman = Salesmen.First(n => n.Id == _order.DocumentIssuerUser.Id);
                            }
                            catch
                            {
                                ;
                            } //hush
                        }
                        //this order outlet
                        var outlets11 = _costCentreService.GetAll()
                                                          .OfType<Outlet>();
                        var myOutlet = outlets11.FirstOrDefault(n => n.Id == _order.IssuedOnBehalfOf.Id);

                        //Load routes
                        DistributorRoutes.Clear();

                        _routeService.GetAll().Where(
                            n =>
                            n.Region.Id ==
                            ((Distributor)
                             Using<ICostCentreRepository>(container).GetById(_configService.Load().CostCentreId)).Region.Id)
                             .ToList().ForEach(n => DistributorRoutes.Add(n));

                        SelectedRoute = DistributorRoutes.FirstOrDefault(n => n.Id == myOutlet.Route.Id);
                        SelectedRouteName = SelectedRoute != null ? SelectedRoute.Name : "";

                        RouteOutlets.Clear();
                        var outlets = _costCentreService.GetAll()
                                                        .OfType<Outlet>();
                        var fileteredOutlets =
                            outlets.Where(n => n.Route != null).Where(n => n.Route.Id == SelectedRoute.Id).ToList();
                        fileteredOutlets.ForEach(
                            n => RouteOutlets.Add(n));
                        SelectedOutlet = RouteOutlets.FirstOrDefault(n => n.Id == myOutlet.Id);
                        setSelectedId = SelectedOutlet.Id;
                        SelectedOutletName = SelectedOutlet.Name;

                        FireSalesmanChangedCmd = false;
                        FireRouteChangedCmd = false;
                        FireOutletChangeCmd = false;
                    }
                }
                RemovedLineItems.Clear();
        }

        private void GetOrderId()
        {
            using (var container = NestedContainer)
            {
                IGetDocumentReference _getDocumentReference = Using<IGetDocumentReference>(container);
                try
                {
                   // OrderId = _getDocumentReference.GetDocReference("SO", SelectedSalesman.Username,
                  //                                                  SelectedOutlet.CostCentreCode);
                }
                catch
                {
                    OrderId = "";
                }
            }
        }

        void SalesmanChanged()
        {
            using (var container = NestedContainer)
            {
                ISalesmanRouteRepository _salesmanRouteService = Using<ISalesmanRouteRepository>(container);

                if (FireSalesmanChangedCmd)
                {
                    //load ur routes only
                    DistributorRoutes.Clear();
                    var route = new Route(Guid.Empty)
                                    {
                                        Name = GetLocalText("sl.pos.selectroute")
                                        /*"--Please Select a Route--"*/
                                    };
                    DistributorRoutes.Add(route);
                    SelectedRoute = route;

                    if (SelectedSalesman != null)
                    {
                        if (SelectedSalesman.Id != Guid.Empty)
                        {
                            _salesmanRouteService.GetAll()
                                .Where(
                                    n => n.DistributorSalesmanRef.Id == SelectedSalesman.CostCentre && n.Route != null).
                                Select(s => s.Route).OrderBy(n => n.Name).ToList().ForEach(n => DistributorRoutes.Add(n));
                        }
                    }
                    RouteChanged();
                }
            }
        }

        void RouteChanged()
        {
            using (var container = NestedContainer)
            {
                

                if (FireRouteChangedCmd)
                {
                    try
                    {
                        if (SelectedRoute == null)
                        {
                            SelectedRoute = DistributorRoutes.First(n => n.Id == Guid.Empty);
                        }
                    }
                    catch
                    {
                    }
                    RouteOutlets.Clear();
                    var outlet = new Outlet(Guid.Empty)
                                     {
                                         CostCentreCode = " ",
                                         Name =GetLocalText("sl.pos.selectoutlet")
                                         /*"--Please Select An Outlet"--"*/
                                     };
                    RouteOutlets.Add(outlet);
                    SelectedOutlet = outlet;
                    if (SelectedRoute != null && SelectedRoute.Id != Guid.Empty)
                    {
                        var outlets = Using<IOutletRepository>(container).GetAll().OfType<Outlet>().Where(n => n._Status == EntityStatus.Active);
                        List<Outlet> filteredOutlets =
                            outlets.OrderBy(n => n.Name).Where(item => item.Route != null).Where(
                                item => item.Route.Id == SelectedRoute.Id).ToList();

                        filteredOutlets.ForEach(n => RouteOutlets.Add(n));
                    }
                }
            }
        }

        public void OutletChanged()
        {
            if (FireOutletChangeCmd)
            {
                GetOrderId();
            }
        }

        public void SaveToContinue()
        {
            using (var container = NestedContainer)
            {
                IOrderRepository _orderService = Using<IOrderRepository>(container);
                _order = CreateOrUpdateOrderInMemory();
                _orderService.Save(_order);
            }
        }

        Order CreateOrUpdateOrderInMemory()
        {
            using (var container = NestedContainer)
            {
                IOrderRepository _orderService = Using<IOrderRepository>(container);

                Order order = null;
                bool updateOrder = false;
                if (OrderIdLookup == Guid.Empty)
                {
                    order = CreateNewOrderSaveInMemory();
                }
                else
                {
                    //order = _orderService.GetByDocumentId(OrderIdLookup);
                    order = _order;
                    updateOrder = true;
                }
                ClearOrderLineItems(order);
                foreach (var item in LineItems)
                {
                    OrderLineItem existingLi = null;

                    existingLi =
                        order.LineItems.FirstOrDefault(
                            n => n.Product.Id == item.ProductId && n.LineItemType == item.OrderLineItemType);

                    if (existingLi == null)
                    {
                        order.AddLineItem(CreateNewLineItem(item, order));
                        continue;
                    }
                    else if (existingLi != null && existingLi.Qty == item.Qty)
                    {
                        continue;
                    }
                    else if (existingLi != null && existingLi.Qty != item.Qty)
                    {
                        if (item.Qty == 0)
                        {
                            _orderService.DeleteLineItem(existingLi);
                            order = _orderService.GetById(order.Id) as Order;
                            updateOrder = true;
                        }
                        order.ChangeLineItemQty(existingLi.Id, item.Qty);
                    }
                }


                if (updateOrder)
                {
                    order = UpdateOrderHeader(order);
                }
                return order;
            }
        }

        Order CreateNewOrderSaveInMemory()
        {
            using (var container = NestedContainer)
            {
                ICostCentreRepository _costCentreService = Using<ICostCentreRepository>(container);
                Config config = GetConfigParams();
                Order newOrder = null;
                CostCentre docIssuerCC = _costCentreService.GetById(config.CostCentreId);
                CostCentre docRecipientCC = _costCentreService.GetById(SelectedSalesman.CostCentre);

                newOrder = Using<IOrderFactory>(container).Create(docIssuerCC,
                                                                  config.CostCentreApplicationId,
                                                                  docRecipientCC, 
                                                                  SelectedSalesman,
                                                                  SelectedOutlet, 
                                                                  OrderType.OutletToDistributor,
                                                                  OrderId, 
                                                                  Guid.Empty, 
                                                                  DateRequired);
                newOrder.SaleDiscount = SaleDiscount;
                OrderIdLookup = newOrder.Id;
                return newOrder;
            }
        }

        Order UpdateOrderHeader(Order order)
        {
            using (var container = NestedContainer)
            {
               
                ICostCentreRepository _costCentreService = Using<ICostCentreRepository>(container);
                IConfigService _configService = Using<IConfigService>(container);

                CostCentre docIssuerCC = _costCentreService.GetById(_configService.Load().CostCentreId);
                order.DateRequired = DateRequired;
                order.DocumentIssuerCostCentre = docIssuerCC;
                order.IssuedOnBehalfOf = _costCentreService.GetById(SelectedOutlet.Id);
                order.DocumentIssuerUser = SelectedSalesman;
                order.DocumentRecipientCostCentre = _costCentreService.GetById(docIssuerCC.ParentCostCentre.Id);
                order.DocumentIssuerCostCentre = docIssuerCC;
                order.DocumentReference = OrderId;
                order.SaleDiscount = SaleDiscount;
                return order;
            }
        }

        private void ClearOrderLineItems(Order order)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {

                foreach (OrderLineItem item in order._allLineItems())
                {
                    Using<IOrderRepository>(cont).DeleteLineItem(item);
                }
                order.ClearLineItems();
            }
        }

        OrderLineItem CreateNewLineItem(EditSalesmanOrderItem item, Order order)
        {
            using (var container = NestedContainer)
            {
                IProductRepository _productService = Using<IProductRepository>(container);
                ISalesmanOrderWFManager _orderWFManager = Using<ISalesmanOrderWFManager>(container);
                //IConfigService _configService = Using<IConfigService>(container);
                var p = new OrderLineItemFactoryParameters
                            {
                                Description = item.Product,
                                Product = _productService.GetById(item.ProductId),
                                Qty = item.Qty
                            };
                var oli = _orderWFManager.PendingDocumentLineItemFactory(order, p);
                item.LineItemId = oli.Id;
                oli.Qty = item.Qty;
                oli.Value = item.UnitPrice;
                oli.LineItemVatValue = item.Vat;
                oli.LineItemType = OrderLineItemType.DuringConfirmation;
                if (item.OrderLineItemType == OrderLineItemType.Discount) //overrite
                    oli.LineItemType = OrderLineItemType.Discount;
                oli.ProductDiscount = item.ProductDiscount;
                oli.DiscountType = item.LineItemDiscountType;
                return oli;
            }
        }

        public void UpdateLineItem(Product product, decimal quantity, bool isEditable, Guid parentProductId, LineItemType lineItemType)
        {
            using (var container = NestedContainer)
            {

                IDiscountProWorkflow _discountProService = Using<IDiscountProWorkflow>(container);

                LineItemPricingInfo pi = _discountProService.GetLineItemPricing(new PackagingSummary
                                                                                    {
                                                                                        Product = product,
                                                                                        Quantity = quantity,
                                                                                    },
                                                                                SelectedOutlet.Id
                    );

                decimal unitPrice = pi.UnitPrice;
                decimal vatValue = pi.VatValue;
                decimal totalVatAmount = pi.TotalVatAmount;
                decimal totalPrice = pi.TotalPrice;
                decimal totalProdDisc = pi.TotalProductDiscount;

                EditSalesmanOrderItem li;
                if (
                    LineItems.Any(
                        p =>
                        p.ProductId == product.Id && p.LineItemType == lineItemType &&
                        p.OrderLineItemType != OrderLineItemType.Discount))
                {
                    li = LineItems.First(p => p.ProductId == product.Id && p.LineItemType == lineItemType);
                    li.Qty = quantity;
                    li.LineItemUnitVatValue = vatValue;
                    li.TotalLineItemVatAmount = totalVatAmount;
                    li.TotalPrice = totalPrice;
                    li.ProductDiscount = totalProdDisc;
                }
                CalcTotals();
            }
        }

        public void UpdateLineItem(int sequenceNo, decimal qty)
        {
            EditSalesmanOrderItem item = LineItems.First(n => n.SequenceNo == sequenceNo);
            item.Qty = qty;
            CalcTotals();
        }

        void Cancel()
        {
            ClearViewModel();
        }

        void ClearViewModel()
        {
            DocumentRef = "";
            Now = DateTime.Now;
            DateRequired = DateTime.Now;
            LineItems.Clear();
            DiscountLookUp = Guid.Empty;
            TotalGross = 0;
            TotalNet = 0;
            TotalVat = 0;
            TotalProductDiscount = 0m;
            SaleDiscount = 0m;
            SelectedOutlet = null;
            SelectedRoute = null;
            _order = null;
        }

        public void AddLineItem(Guid productId, string productDesc, decimal unitPrice, decimal vatValue, decimal vatAmount, decimal vat, decimal qty, decimal totalPrice, bool isEditable, Guid? lineItemId, string productType, OrderLineItemType orderLineItemType, decimal productDiscount, DiscountType liDiscountType)
        {
            using (var container = NestedContainer)
            {
                IOtherUtilities _otherUtilities = Using<IOtherUtilities>(container);
                EditSalesmanOrderItem li = null;
                int sequenceNo = 1;
                if (LineItems.Count > 0)
                {
                    sequenceNo = LineItems.Max(n => n.SequenceNo) + 1;
                }
                //Check the line item exists by product
                if (LineItems.Any(n => n.ProductId == productId && n.OrderLineItemType == orderLineItemType))
                {
                    li = LineItems.FirstOrDefault(n => n.ProductId == productId);
                    li.Qty += qty;
                    li.TotalLineItemVatAmount += vatAmount;
                    li.NetAmount += (li.Qty*unitPrice);
                    li.Vat = vat;
                    li.TotalPrice += totalPrice;
                    li.LineItemTotalProductDiscount += (productDiscount*qty);
                }
                else
                {
                    #region New LineItem

                    li = new EditSalesmanOrderItem(_otherUtilities)
                             {
                                 SequenceNo = sequenceNo,
                                 ProductId = productId,
                                 Product = productDesc,
                                 UnitPrice = LoadForViewing ? (unitPrice < 0 ? -unitPrice : unitPrice) : unitPrice,
                                 LineItemUnitVatValue = vatValue,
                                 TotalLineItemVatAmount = vatAmount,
                                 Vat = vat,
                                 Qty = qty,
                                 NetAmount = (qty*unitPrice),
                                 TotalPrice = totalPrice,
                                 IsEditable = isEditable,
                                 LineItemId = (Guid) lineItemId,
                                 //item.LineItemId = oli.Id;,
                                 CanEditLineItem = true,
                                 CanRemoveLineItem = true,
                                 ProductType = productType,
                                 OrderLineItemType = orderLineItemType,
                                 ProductDiscount = productDiscount,
                                 LineItemTotalProductDiscount = (productDiscount*qty),
                                 LineItemDiscountType = liDiscountType
                             };
                    if (productType == "ReturnableProduct") li.CanEditLineItem = false;
                    if (productType == "ReturnableProduct") li.CanRemoveLineItem = false;

                    if (li.OrderLineItemType == OrderLineItemType.Discount)
                    {
                        li.CanEditLineItem = false;
                        li.CanRemoveLineItem = false;

                        if (li.LineItemDiscountType == DiscountType.FreeOfChargeDiscount)
                        {
                            li.CanEditLineItem = true;
                            li.CanRemoveLineItem = true;
                        }
                    }

                    if (Status != "New" && Status != "Confirmed")
                    {
                        li.CanEditLineItem = false;
                        li.CanRemoveLineItem = false;
                    }

                    if (LoadForViewing)
                    {
                        li.CanEditLineItem = false;
                        li.CanRemoveLineItem = false;
                    }
                    LineItems.Add(li);

                    //reorder 
                    //1.consolidated
                    //2.sale products
                    //3.returnable products

                    #endregion
                }

                //CalcDiscounts(li);
                CalcTotals();
            }
        }


        public void UpdateOrAddLineItemFromPoductSummary(List<ProductAddSummary> productsummariies, bool IsNew)
        {
            foreach (ProductAddSummary product in productsummariies)
            {

                if (product.IsEditable)
                    _productPackagingSummaryService.AddProduct(product.ProductId, product.Quantity, false, !IsNew,
                                                               true);

            }
            RefreshList();
        }

        private void RefreshList()
        {
            LineItems.Clear();
            List<PackagingSummary> summarypro = _productPackagingSummaryService.GetProductSummary();
            ClearOrderLineItems();
            foreach (PackagingSummary item in summarypro)
            {
                if (IsEditable)
                    UpdateOrAddLineItem(item.Product, item.Quantity, item.IsEditable, item.ParentProductId,
                                        LineItemType.Unit);
                else
                    UpdateOrAddLineItem(item.Product, item.Quantity, false, item.ParentProductId, LineItemType.Unit);
            }
            ProcessDiscounts();
        }

        void ClearOrderLineItems()
        {
            using (var container = NestedContainer)
            {
                IOrderRepository _orderService =Using<IOrderRepository>(container);

                if (_order == null)
                    return;

                foreach (OrderLineItem item in _order.LineItems)
                {
                    _orderService.DeleteLineItem(item);
                }
                _order._allLineItems().Clear();
            }
        }

        private void UpdateOrAddLineItem(Product product, decimal quantity, bool isEditable, Guid parentProductId, LineItemType lineItemType)
        {


                LineItemPricingInfo pi = GetLineItemPricing(new PackagingSummary
                                                                                    {
                                                                                        Product = product,
                                                                                        Quantity = quantity,
                                                                                    },
                                                                                SelectedOutlet.Id
                    );

                decimal unitPrice = pi.UnitPrice;
                decimal vatValue = pi.VatValue;
                decimal totalVatAmount = pi.TotalVatAmount;
                decimal totalNet = pi.TotalNetPrice;
                decimal totalPrice = pi.TotalPrice;
                decimal productDiscount = pi.ProductDiscount;
                decimal totalProductDiscount = pi.TotalProductDiscount;

                int sequenceNo = 1;
                if (LineItems.Count() > 0)
                {
                    sequenceNo = LineItems.Max(n => n.SequenceNo) + 1;
                }
                EditSalesmanOrderItem li;
                if (
                    LineItems.Any(
                        p =>
                        p.ProductId == product.Id && p.LineItemType == lineItemType &&
                        p.OrderLineItemType != OrderLineItemType.Discount))
                {
                    li = LineItems.First(p => p.ProductId == product.Id && p.LineItemType == lineItemType);
                    li.Qty = li.Qty + quantity;
                    //li.LineItemUnitVatValue = li.LineItemUnitVatValue + VatValue;
                    li.TotalLineItemVatAmount = li.TotalLineItemVatAmount + totalVatAmount;
                    li.NetAmount += totalNet;
                    li.TotalPrice = li.TotalPrice + totalPrice;
                    li.ProductDiscount = productDiscount;
                    li.LineItemTotalProductDiscount += totalProductDiscount;
                }
                else if (
                    LineItems.Any(
                        p =>
                        p.ProductId == product.Id && p.LineItemType == lineItemType &&
                        p.OrderLineItemType != OrderLineItemType.Discount))
                {
                    li = LineItems.First(p => p.ProductId == product.Id && p.LineItemType == lineItemType);
                    li.Qty = quantity;
                    li.LineItemUnitVatValue = vatValue;
                    li.TotalLineItemVatAmount = totalVatAmount;
                    li.NetAmount = totalNet;
                    li.TotalPrice = totalPrice;
                    li.ProductDiscount = productDiscount;
                    li.LineItemTotalProductDiscount = totalProductDiscount;
                }
                else
                {
                    using (var container = NestedContainer)
                    {
                        IDiscountProWorkflow _discountProService = Using<IDiscountProWorkflow>(container);
                        IOtherUtilities _otherUtilities = Using<IOtherUtilities>(container);
                        li = new EditSalesmanOrderItem(_otherUtilities);
                        li.Qty = quantity;
                        li.LineItemUnitVatValue = vatValue;
                        li.TotalLineItemVatAmount = totalVatAmount;
                        li.NetAmount = totalNet;
                        li.TotalPrice = totalPrice;
                        li.SequenceNo = sequenceNo;
                        li.ProductId = product.Id;
                        li.Product = product.Description;
                        li.UnitPrice = unitPrice;
                        li.IsEditable = isEditable;
                        li.Vat = vatValue;
                        li.ParentProductId = parentProductId;
                        li.LineItemType = lineItemType;
                        li.CanEditLineItem = li.IsEditable;
                        li.CanRemoveLineItem = li.IsEditable;
                        li.ProductDiscount = productDiscount;
                        li.LineItemTotalProductDiscount = totalProductDiscount;
                        li.ProductType = product.GetType().ToString().Split('.').Last();

                        if (_discountProService.IsProductFreeOfCharge(product.Id))
                        {
                            li.OrderLineItemType = OrderLineItemType.Discount;
                            li.LineItemDiscountType = DiscountType.FreeOfChargeDiscount;
                        }

                        if (li.OrderLineItemType == OrderLineItemType.Discount)
                        {
                            li.CanEditLineItem = false;
                            li.CanRemoveLineItem = false;

                            if (li.LineItemDiscountType == DiscountType.FreeOfChargeDiscount)
                            {
                                li.CanEditLineItem = true;
                                li.CanRemoveLineItem = true;
                            }
                        }
                        LineItems.Add(li);
                        List<OrderLineItemBase> lineItems = LineItems.Select(n => n as OrderLineItemBase).ToList();
                        LineItems.Clear();
                        _productPackagingSummaryService.OrderLineItems(lineItems)
                                                       .Select(n => n as EditSalesmanOrderItem)
                                                       .ToList().ForEach(LineItems.Add);
                    }
                }

                CalcTotals();
        }

        decimal PriceCalc(Product product)
        {
            using (var container = NestedContainer)
            {
                var _productPricingService = Using<IProductPricingRepository>(container);

                decimal UnitPrice = 0m;
                ProductPricingTier tier = _productPricingService.GetAll().FirstOrDefault().Tier;

                if (product is ConsolidatedProduct)
                    try
                    {
                        UnitPrice = ((ConsolidatedProduct) product).ProductPrice(tier);
                    }
                    catch
                    {
                        UnitPrice = 0m;
                    }
                else
                    try
                    {
                        UnitPrice = product.ProductPrice(tier);
                    }
                    catch
                    {
                        UnitPrice = 0m;
                    }
                return UnitPrice;
            }
        }

        decimal VatCalc(Product product)
        {
            decimal vat = 0m;
            if (product is ReturnableProduct)
                return 0;
            if (product.VATClass != null)
            {
                vat = product.VATClass.CurrentRate;
            }
            return vat;
        }

        void CalcTotals()
        {
            TotalNet = LineItems.Sum(n => (n.Qty * (n.TotalPrice < 0 ? -n.UnitPrice : n.UnitPrice)));
            TotalProductDiscount = LineItems.Sum(n => n.LineItemTotalProductDiscount);
            TotalNet += TotalProductDiscount;
            TotalVat = LineItems.Sum(n => n.TotalLineItemVatAmount);
            TotalGross = LineItems.Sum(n => n.TotalPrice);
            //AddSaleDiscount();
            TotalGross -= SaleDiscount;
        }

        #region Process Discounts
        void ProcessDiscounts()
        {
            SaleDiscount = 0;
            List<EditSalesmanOrderItem> lineItems =
                LineItems.Where(
                    n => n.OrderLineItemType != OrderLineItemType.Discount && n.ProductType != "ReturnableProduct").
                    ToList();
            foreach (EditSalesmanOrderItem lineItem in lineItems)
            {
                //2. add free of charge (Certain Product Quantity Certain Product)
                AddCertainProductQuantityCertainProductDiscount(lineItem);
            }

            //1. add product discount
            //2. add free of charge (Certain Product Quantity Certain Product)
            //AddCertainProductQuantityCertainProductDiscount(lineItem);
            //3. add free of charge (Certain Sale Value Certain Product)
            AddCertainSaleValueCertainProductDiscount();

            ProcessDiscountMixedPackReturnables();
            //4. add sale discount
            AddSaleDiscount();
        }

        void AddCertainSaleValueCertainProductDiscount()
        {
            using (var container = NestedContainer)
            {
                IDiscountProWorkflow _discountProService = Using<IDiscountProWorkflow>(container);

                CalcTotals();
                ProductAsDiscount productAsDiscount = _discountProService.GetFOCCertainValue(TotalGross);
                if (productAsDiscount != null)
                    DisplayProductAsDiscountAndAddToDiscountLineItems(productAsDiscount);
            }
        }

        void AddCertainProductQuantityCertainProductDiscount(EditSalesmanOrderItem lineItem)
        {
            using (var container = NestedContainer)
            {
                IDiscountProWorkflow _discountProService = Using<IDiscountProWorkflow>(container);

                List<ProductAsDiscount> productAsDiscounts = _discountProService.GetFOCCertainProduct(
                    lineItem.ProductId, lineItem.Qty);

                foreach (ProductAsDiscount productAsDiscount in productAsDiscounts)
                {
                    if (productAsDiscount != null)
                        DisplayProductAsDiscountAndAddToDiscountLineItems(productAsDiscount);
                }
            }
        }

        void AddSaleDiscount()
        {
            using (var container = NestedContainer)
            {
                IDiscountProWorkflow _discountProService = Using<IDiscountProWorkflow>(container);

                SaleDiscount = 0;
                if (SelectedOutlet == null || SelectedOutlet.Id == Guid.Empty)
                {
                    SaleDiscount = 0m;
                }
                else
                {
                    CalcTotals();
                    SaleDiscount = _discountProService.GetSalevalue(TotalGross, SelectedOutlet.Id);
                }
            }
        }

        void DisplayProductAsDiscountAndAddToDiscountLineItems(ProductAsDiscount productAsDiscount)
        {
            using (var container = NestedContainer)
            {
                IProductRepository _productService = Using<IProductRepository>(container);

                Product product = _productService.GetById(productAsDiscount.ProductId);

                AddOrUpdateLineItemFromDiscount(productAsDiscount, product);
            }
        }
        List<ReturnableProduct> discProdReturnables = null;
        void AddOrUpdateLineItemFromDiscount(ProductAsDiscount productAsDiscount, Product product)
        {
                List<Product> discountProducts = new List<Product>();
                discountProducts.Add(product);
                discProdReturnables = new List<ReturnableProduct>();
                discProdReturnables = _productPackagingSummaryService.GetProductReturnables(product,
                                                                                            productAsDiscount.Quantity);
                if (discProdReturnables != null && discProdReturnables.Count > 0)
                {
                    discProdReturnables.ForEach(discountProducts.Add);
                }
                _productPackagingSummaryService.ClearProductReturnables();

                foreach (Product prod in discountProducts)
                {
                    decimal unitPrice = 0m;
                    decimal vatValue = 0m;
                    decimal totalVatAmount = 0m;
                    decimal totalNet = 0m;
                    decimal totalPrice = 0m;
                    decimal productDiscount = 0m;
                    decimal totalProductDiscount = 0m;
                    decimal qty = productAsDiscount.Quantity;

                    EditSalesmanOrderItem existingItem = null;
                    if (prod.Id == product.Id) //is the discount product
                    {
                        existingItem = LineItems.FirstOrDefault(n =>
                                                                n.OrderLineItemType == OrderLineItemType.Discount &&
                                                                n.ProductId == prod.Id &&
                                                                n.LineItemDiscountType ==
                                                                productAsDiscount.DiscountType);
                    }
                    else
                    {
                        existingItem =
                            LineItems.FirstOrDefault(
                                n => n.ProductId == prod.Id && n.OrderLineItemType != OrderLineItemType.Discount);
                    }

                    if (prod.Id != product.Id && ((ReturnableProduct) prod).Capacity > 1)
                    {
                        qty = (int) (productAsDiscount.Quantity/(prod as ReturnableProduct).Capacity);
                    }

                    if (prod.Id != product.Id)
                    {
                        LineItemPricingInfo pi = GetLineItemPricing(new PackagingSummary
                                                                        {
                                                                            Product = prod,
                                                                            Quantity = qty
                                                                        },
                                                                    SelectedOutlet.Id);
                        unitPrice = pi.UnitPrice;
                        vatValue = pi.VatValue;
                        totalVatAmount = pi.TotalVatAmount;
                        totalNet += pi.TotalNetPrice;
                        totalPrice = pi.TotalPrice;
                        productDiscount = pi.ProductDiscount;
                        totalProductDiscount = pi.TotalProductDiscount;
                    }

                    if (existingItem != null)
                    {
                        if (prod.Id != product.Id) //returnable product
                        {
                            if (((ReturnableProduct) prod).Capacity > 1) //returnable bulk container
                                existingItem.Qty += qty;
                            else //sale product returnable
                                existingItem.Qty += qty;
                        }
                        else //sale product
                        {
                            existingItem.Qty += qty;
                        }

                        existingItem.TotalLineItemVatAmount += totalVatAmount;
                        existingItem.TotalPrice += totalPrice;
                        existingItem.NetAmount += totalNet;
                        existingItem.ProductDiscount = productDiscount;
                        existingItem.LineItemTotalProductDiscount += totalProductDiscount;
                    }
                    else
                    {
                        using (var container = NestedContainer)
                        {
                            IOtherUtilities _otherUtilities = Using<IOtherUtilities>(container);
                            existingItem = new EditSalesmanOrderItem(_otherUtilities)
                                               {
                                                   SequenceNo = LineItems.Count() + 1,
                                                   ProductDiscount = productDiscount,
                                                   LineItemTotalProductDiscount = totalProductDiscount,
                                                   IsEditable = false,
                                                   LineItemId = Guid.Empty,
                                                   ProductId = prod.Id,
                                                   Product = prod.Description,
                                                   ParentProductId = product.Id,
                                                   Qty = productAsDiscount.Quantity,
                                                   UnitPrice = unitPrice,
                                                   Vat = vatValue,
                                                   TotalLineItemVatAmount = totalVatAmount,
                                                   LineItemUnitVatValue = vatValue,
                                                   NetAmount = totalNet,
                                                   TotalPrice = totalPrice,
                                                   ProductType = prod.GetType().ToString().Split('.').Last(),
                                                   CanEditLineItem = false,
                                                   CanRemoveLineItem = false,
                                                   LineItemType = LineItemType.Unit,
                                               };
                        }

                        if (prod.Id == product.Id) //is the discount product
                        {
                            existingItem.LineItemDiscountType = productAsDiscount.DiscountType;
                            existingItem.OrderLineItemType = OrderLineItemType.Discount;

                            if (existingItem.LineItemDiscountType == DiscountType.FreeOfChargeDiscount)
                            {
                                existingItem.CanEditLineItem = true;
                                existingItem.CanRemoveLineItem = true;
                            }
                        }

                        LineItems.Add(existingItem);
                        List<OrderLineItemBase> lineItems = LineItems.Select(n => n as OrderLineItemBase).ToList();
                        LineItems.Clear();
                        _productPackagingSummaryService.OrderLineItems(lineItems).Select(n => n as EditSalesmanOrderItem)
                            .ToList().ForEach(LineItems.Add);
                    }
            }
        }

        private void ProcessDiscountMixedPackReturnables()
        {
            List<PackagingSummary> mixdPackReturns =
                _productPackagingSummaryService.GetMixedPackContainers(
                    LineItems.Where(n => n.ProductType == "ReturnableProduct")
                             .Select(n => new PackagingSummary
                                              {
                                                  Product = GetEntityById(typeof (Product), n.ProductId) as Product,
                                                  Quantity = n.Qty,
                                              }).ToList());

            foreach (PackagingSummary ps in mixdPackReturns)
            {
                EditSalesmanOrderItem existing = LineItems.FirstOrDefault(n => n.ProductId == ps.Product.Id);

                if (existing == null)
                {
                    UpdateOrAddLineItem(ps.Product, ps.Quantity, false, ps.ParentProductId, LineItemType.Unit);
                    continue;
                }
                if (existing.Qty != ps.Quantity)
                {
                    UpdateLineItem(ps.Product, ps.Quantity, false, ps.ParentProductId, LineItemType.Unit);
                }
            }

            _productPackagingSummaryService.ClearMixedPackReturnables();
        }

        #endregion

        public void RemoveLineItem(int sequenceNo)
        {
            var litoremove = LineItems.First(n => n.SequenceNo == sequenceNo);
            LineItems.Remove(litoremove);
            CalcTotals();
            RemovedLineItems.Add(litoremove);
        }

        public void RemoveLineItemByProductId(Guid productId)
        {
            var litoremove = LineItems.First(n => n.ProductId == productId);
            LineItems.Remove(litoremove);
            CalcTotals();
            RemovedLineItems.Add(litoremove);
        }

        public void RemoveLineItem(Guid productId, LineItemType lit)
        {
            var delProduct =
                _productPackagingSummaryService.GetProductSummary().FirstOrDefault(
                    p => p.Product.Id == productId && p.IsEditable);
            string msg = "";
            foreach (
                PackagingSummary delitem in
                    _productPackagingSummaryService.GetProductSummaryByProduct(productId, delProduct.Quantity))
            {
                msg += string.Format("\n\t{0} "
                                     + GetLocalText("sl.approveOrder.lineItems.delete.messageBox.productDetails1")
                                     /*"of"*/
                                     + " {1} "
                                     + GetLocalText("sl.approveOrder.lineItems.delete.messageBox.productDetails2")
                                     /*"will be deleted"*/
                                     , delitem.Quantity, delitem.Product.Description);
            }
            MessageBoxResult isConfirmed = MessageBox.Show( /*"Are sure you want to delete the following product(s)" */
                GetLocalText("sl.approveOrder.lineItems.delete.messageBox.text")
                + msg,
                GetLocalText("sl.approveOrder.lineItems.delete.messageBox.title") /*"Delete Order Line item"*/
                , MessageBoxButton.OKCancel);

            if (isConfirmed == MessageBoxResult.OK)
            {
                _productPackagingSummaryService.RemoveProduct(productId);
            }
            RefreshList();
            CalcTotals();
        }

        public void RunClearAndSetup()
        {
            OrderIdLookup = Guid.Empty;
            DiscountLookUp = Guid.Empty;
            HasDiscount = false;
            Status = "";
            OrderId = "";
            DateRequired = DateTime.Now;
            DateSubmitted = DateTime.MaxValue;
            CreatedByUser = "";
            LineItems.Clear();
            CalcTotals();
        }
        #endregion
    }

    #region SupportClasses

    public class EditSalesmanOrderItem : OrderLineItemBase
    {
        private IOtherUtilities _otherUtilities;

        public EditSalesmanOrderItem(IOtherUtilities otherUtilities)
        {
            _otherUtilities = otherUtilities;
        }

        public const string UnitPricePropertyName = "UnitPrice";
        private decimal _unitPrice = 0;
        public decimal UnitPrice
        {
            get
            {
                return _unitPrice;
            }
            set
            {
                if (_unitPrice == value)
                {
                    return;
                }
                var oldValue = _unitPrice;
                _unitPrice = value;
                RaisePropertyChanged(UnitPricePropertyName);
            }
        }

        public const string NetAmountPropertyName = "NetAmount";
        private decimal _netAmount = 0m;
        public decimal NetAmount
        {
            get
            {
                return _netAmount;
            }

            set
            {
                if (_netAmount == value)
                {
                    return;
                }

                var oldValue = _netAmount;
                _netAmount = value;
                RaisePropertyChanged(NetAmountPropertyName);
            }
        }

        public const string LineItemVatValuePropertyName = "LineItemUnitVatValue";
        private decimal _lineUnitItemVatValue = 0;
        public decimal LineItemUnitVatValue
        {
            get
            {
                return _lineUnitItemVatValue;
            }

            set
            {
                if (_lineUnitItemVatValue == value)
                    return;
                var oldValue = _lineUnitItemVatValue;
                _lineUnitItemVatValue = value;
                RaisePropertyChanged(LineItemVatValuePropertyName);
            }
        }

        public const string VatAmountPropertyName = "TotalLineItemVatAmount";
        private decimal _totalLineItemVatAmount = 0;
        public decimal TotalLineItemVatAmount
        {
            get
            {
                return _totalLineItemVatAmount;
            }

            set
            {
                if (_totalLineItemVatAmount == value)
                {
                    return;
                }
                var oldValue = _totalLineItemVatAmount;
                _totalLineItemVatAmount = value;
                RaisePropertyChanged(VatAmountPropertyName);
            }
        }

        public const string VatPropertyName = "Vat";
        private decimal  _vat = 0m;
        public decimal  Vat
        {
            get
            {
                return _vat;
            }

            set
            {
                if (_vat == value)
                {
                    return;
                }

                var oldValue = _vat;
                _vat = value;
                RaisePropertyChanged(VatPropertyName);
            }
        }

        public const string IsEditablePropertyName = "IsEditable";
        private bool _myProperty = true;
        public bool IsEditable
        {
            get
            {
                return _myProperty;
            }

            set
            {
                if (_myProperty == value)
                    return;
                var oldValue = _myProperty;
                _myProperty = value;
                RaisePropertyChanged(IsEditablePropertyName);
            }
        }

        public const string CanEditLineItemPropertyName = "CanEditLineItem";
        private bool _canEditLineItem = false;
        public bool CanEditLineItem
        {
            get
            {
                return _canEditLineItem;
            }

            set
            {
                if (_canEditLineItem == value)
                {
                    return;
                }

                var oldValue = _canEditLineItem;
                _canEditLineItem = value;
                RaisePropertyChanged(CanEditLineItemPropertyName);
            }
        }

        public const string CanRemoveLineItemPropertyName = "CanRemoveLineItem";
        private bool _canRemoveLineItem = false;
        public bool CanRemoveLineItem
        {
            get
            {
                return _canRemoveLineItem;
            }

            set
            {
                if (_canRemoveLineItem == value)
                {
                    return;
                }

                var oldValue = _canRemoveLineItem;
                _canRemoveLineItem = value;
                RaisePropertyChanged(CanRemoveLineItemPropertyName);
            }
        }

        public string Product_Type
        {
            get
            {
                string productType = ProductType.Remove(ProductType.LastIndexOf("Product"));
                return _otherUtilities.BreakStringByUpperCB(productType +
                                                     ((int) LineItemDiscountType > 0
                                                          ? " (" + LineItemDiscountType.ToString() + ")"
                                                          : ""));
            }
        }

        public LineItemType LineItemType { get; set; }
        public Guid ParentProductId { get; set; }

        public const string LineItemDiscountTypePropertyName = "LineItemDiscountType";
        private DiscountType _lineItemDiscountType = 0;
        public DiscountType LineItemDiscountType
        {
            get
            {
                return _lineItemDiscountType;
            }

            set
            {
                if (_lineItemDiscountType == value)
                {
                    return;
                }

                var oldValue = _lineItemDiscountType;
                _lineItemDiscountType = value;
                RaisePropertyChanged(LineItemDiscountTypePropertyName);
            }
        }

        public const string ProductDiscountPropertyName = "ProductDiscount";
        private decimal _productDiscount = 0m;
        public decimal ProductDiscount
        {
            get
            {
                return _productDiscount;
            }

            set
            {
                if (_productDiscount == value)
                {
                    return;
                }

                var oldValue = _productDiscount;
                _productDiscount = value;
                RaisePropertyChanged(ProductDiscountPropertyName);
            }
        }
         
        public const string LineItemTotalProductDiscountPropertyName = "LineItemTotalProductDiscount";
        private decimal _lineItemTotalProductDiscount = 0m;
        public decimal LineItemTotalProductDiscount
        {
            get
            {
                return _lineItemTotalProductDiscount;
            }

            set
            {
                if (_lineItemTotalProductDiscount == value)
                {
                    return;
                }

                var oldValue = _lineItemTotalProductDiscount;
                _lineItemTotalProductDiscount = value;
                RaisePropertyChanged(LineItemTotalProductDiscountPropertyName);
            }
        }

    }
    #endregion
}