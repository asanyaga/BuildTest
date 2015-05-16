using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.CreditNoteRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.DispatchRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.WorkFlow.Discount;
using Distributr.WPF.Lib.Services.WorkFlow.Orders;
using Distributr.WPF.Lib.UI.Hierarchy;
using Distributr.WPF.Lib.ViewModels.Transactional.Purchasing;
using Distributr.WPF.Lib.WorkFlow.Orders;
using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using System.Linq;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.InventoryEntities;
using System.Windows;
using System.Collections.Generic;

namespace Distributr.WPF.Lib.ViewModels.Transactional.SalesmanOrders
{
    public class ApproveSalesmanOrderViewModel : DistributrViewModelBase
    {
        private Order _order;

        public ApproveSalesmanOrderViewModel()
        {


            LoadOrderCommand = new RelayCommand(DoLoad);
            RejectCommand = new RelayCommand(RunRejectCommand);
            CancelCommand = new RelayCommand(Cancel);
            LineItemsRemoved = new ObservableCollection<Guid>();
            ProcessBackOrdersCommand = new RelayCommand(RunProcessBackOrderCommand);
            ValidateProcessBackOrdersCommand = new RelayCommand(RunValidateProcessBackOrder);
            ViewInvoiceCommand = new RelayCommand(RunViewInvoice);
            ViewReceiptCommand = new RelayCommand(RunViewReceipt);

            //cn:   : keep it simple stupid
            ApproveCommand = new RelayCommand(RunApproveCommand);
            ValidateOrderForApprovalCommand = new RelayCommand(RunValidateOrderForApproval);
            CreateBackOrderAndApproveCommand = new RelayCommand(RunCreateBackOrderAndApprove);
            CreateInvalidOrdersMessageCommand = new RelayCommand(RunCreateMessageOfInvalidOrders);
            LineItems = new ObservableCollection<ApproveSalesmanOrderItem>();
            InvoiceReceipts = new ObservableCollection<Receipt>();

        }

      

        #region Properties

        #region RelayCommandsCollections

        public ObservableCollection<ApproveSalesmanOrderItem> LineItems { get; set; }
        public ObservableCollection<Receipt> InvoiceReceipts { get; set; }
        public Dictionary<int, decimal> OriginalLineItems;
        public ObservableCollection<Guid> LineItemsRemoved { get; set; }
        public RelayCommand LoadOrderCommand { get; set; }
        public RelayCommand RejectCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand AddLineItemBeforeConfirmCommand { get; set; }
        public RelayCommand ProcessBackOrdersCommand { get; set; }
        public RelayCommand ValidateProcessBackOrdersCommand { get; set; }
        //cn:   : keep it simple stupid
        public RelayCommand ApproveCommand { get; set; }
        public RelayCommand ValidateOrderForApprovalCommand { get; set; }
        public RelayCommand CreateBackOrderAndApproveCommand { get; set; }
        public RelayCommand CreateInvalidOrdersMessageCommand { get; set; }
        public RelayCommand ViewInvoiceCommand { get; set; }
        public RelayCommand ViewReceiptCommand { get; set; }
        public List<OrderLineItemInventoryInfo> LineItemsInventoryInfoList { get; set; }

        #endregion

        #region mvvminpc

        public const string OrderIdPropertyName = "OrderId";
        private string _orderId = "";

        public string OrderId
        {
            get { return _orderId; }

            set
            {
                if (_orderId == value)
                    return;
                var oldValue = _orderId;
                _orderId = value;
                RaisePropertyChanged(OrderIdPropertyName);
            }
        }

        public const string OrderIdLookupPropertyName = "OrderIdLookup";
        private Guid _orderIdLookup = Guid.Empty;

        public Guid OrderIdLookup
        {
            get { return _orderIdLookup; }

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

        public const string StatusPropertyName = "Status";
        private string _status = "";

        public string Status
        {
            get { return _status; }

            set
            {
                if (_status == value)
                    return;

                var oldValue = _status;
                _status = value;
                RaisePropertyChanged(StatusPropertyName);
            }
        }

        public const string DateRequiredPropertyName = "DateRequired";
        private DateTime _dateRequired = DateTime.Now;

        public DateTime DateRequired
        {
            get { return _dateRequired; }

            set
            {
                if (_dateRequired == value)
                {
                    return;
                }
                var oldValue = _dateRequired;
                _dateRequired = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(DateRequiredPropertyName);
            }
        }

        public const string DateSubmittedPropertyName = "DateSubmitted";
        private DateTime _dateSubmitted = DateTime.MaxValue;

        public DateTime DateSubmitted
        {
            get { return _dateSubmitted; }

            set
            {
                if (_dateSubmitted == value)
                    return;
                var oldValue = _dateSubmitted;
                _dateSubmitted = value;
                RaisePropertyChanged(DateSubmittedPropertyName);
            }
        }

        public const string CreatedByUserPropertyName = "CreatedByUser";
        private string _createdByUser = "";

        public string CreatedByUser
        {
            get { return _createdByUser; }

            set
            {
                if (_createdByUser == value)
                    return;

                var oldValue = _createdByUser;
                _createdByUser = value;
                RaisePropertyChanged(CreatedByUserPropertyName);
            }
        }

        public const string TotalNetPropertyName = "TotalNet";
        private decimal _totalNet = 0;

        public decimal TotalNet
        {
            get { return _totalNet; }

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
            get { return _totalVat; }

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
            get { return _totalGross; }

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

        public const string IsEditablePropertyName = "IsEditable";
        private bool _myProperty = true;

        public bool IsEditable
        {
            get { return _myProperty; }

            set
            {
                if (_myProperty == value)
                    return;
                var oldValue = _myProperty;
                _myProperty = value;
                RaisePropertyChanged(IsEditablePropertyName);
            }
        }

        public const string SalesmanUsernamePropertyName = "SalesmanUsername";
        private string _salesmanUsername = "";

        public string SalesmanUsername
        {
            get { return _salesmanUsername; }

            set
            {
                if (_salesmanUsername == value)
                {
                    return;
                }

                var oldValue = _salesmanUsername;
                _salesmanUsername = value;
                RaisePropertyChanged(SalesmanUsernamePropertyName);
            }
        }

        public const string RouteNamePropertyName = "RouteName";
        private string _routeName = "";

        public string RouteName
        {
            get { return _routeName; }

            set
            {
                if (_routeName == value)
                {
                    return;
                }

                var oldValue = _routeName;
                _routeName = value;
                RaisePropertyChanged(RouteNamePropertyName);
            }
        }

        public const string OutletNamePropertyName = "OutletName";
        private string _outletName = "";

        public string OutletName
        {
            get { return _outletName; }

            set
            {
                if (_outletName == value)
                {
                    return;
                }

                var oldValue = _outletName;
                _outletName = value;
                RaisePropertyChanged(OutletNamePropertyName);
            }
        }

        public const string ValidatedPropertyName = "Validated";
        private bool _validated = false;

        public bool Validated
        {
            get { return _removedLineItem; }

            set
            {
                if (_removedLineItem == value)
                {
                    return;
                }

                var oldValue = _removedLineItem;
                _removedLineItem = value;
                RaisePropertyChanged(ValidatedPropertyName);
            }
        }

        public const string AddedNewLineItemPropertyName = "AddedNewLineItem";
        private bool _addedNewLineItem = false;

        public bool AddedNewLineItem
        {
            get { return _removedLineItem; }

            set
            {
                if (_removedLineItem == value)
                {
                    return;
                }

                var oldValue = _removedLineItem;
                _removedLineItem = value;
                RaisePropertyChanged(RemovedLineItemPropertyName);
            }
        }

        public const string RemovedLineItemPropertyName = "RemovedLineItem";
        private bool _removedLineItem = false;

        public bool RemovedLineItem
        {
            get { return _removedLineItem; }

            set
            {
                if (_removedLineItem == value)
                {
                    return;
                }

                var oldValue = _removedLineItem;
                _removedLineItem = value;
                RaisePropertyChanged(RemovedLineItemPropertyName);
            }
        }

        public const string UpdatedLineItemPropertyName = "UpdatedLineItem";
        private bool _updatedLineItem = false;

        public bool UpdatedLineItem
        {
            get { return _updatedLineItem; }

            set
            {
                if (_updatedLineItem == value)
                {
                    return;
                }

                var oldValue = _updatedLineItem;
                _updatedLineItem = value;
                RaisePropertyChanged(UpdatedLineItemPropertyName);
            }
        }

        public const string IsApprovedPropertyName = "IsApproved";
        private bool _isApproved = false;

        public bool IsApproved
        {
            get { return _isApproved; }

            set
            {
                if (_isApproved == value)
                {
                    return;
                }

                var oldValue = _isApproved;
                _isApproved = value;
                RaisePropertyChanged(IsApprovedPropertyName);
            }
        }

        public const string IsRejectedPropertyName = "IsRejected";
        private bool _isRejected = false;

        public bool IsRejected
        {
            get { return _isRejected; }

            set
            {
                if (_isRejected == value)
                {
                    return;
                }

                var oldValue = _isRejected;
                _isRejected = value;
                RaisePropertyChanged(IsRejectedPropertyName);
            }
        }

        public const string OutletIdPropertyName = "OutletId";
        private Guid _outletId = Guid.Empty;

        public Guid OutletId
        {
            get { return _outletId; }

            set
            {
                if (_outletId == value)
                {
                    return;
                }

                var oldValue = _outletId;
                _outletId = value;
                RaisePropertyChanged(OutletIdPropertyName);
            }
        }

        public const string LoadForProcessingPropertyName = "LoadForProcessing";
        private bool _loadForProcessing = false;

        public bool LoadForProcessing
        {
            get { return _loadForProcessing; }

            set
            {
                if (_loadForProcessing == value)
                {
                    return;
                }

                var oldValue = _loadForProcessing;
                _loadForProcessing = value;
                RaisePropertyChanged(LoadForProcessingPropertyName);
            }
        }

        public const string LoadForViewingPropertyName = "LoadForViewing";
        private bool _loadForViewing = false;

        public bool LoadForViewing
        {
            get { return _loadForViewing; }

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

        public const string LoadFullGridPropertyName = "LoadFullGrid";
        private bool _loadFullGrid = false;

        public bool LoadFullGrid
        {
            get { return _loadFullGrid; }

            set
            {
                if (_loadFullGrid == value)
                {
                    return;
                }

                var oldValue = _loadFullGrid;
                _loadFullGrid = value;
                RaisePropertyChanged(LoadFullGridPropertyName);
            }
        }

        //public const string CancelButtonContentPropertyName = "CancelButtonContent";
        //private string _cancelButtonContent = "Cancel";
        //public string CancelButtonContent
        //{
        //    get
        //    {
        //        return _cancelButtonContent;
        //    }

        //    set
        //    {
        //        if (_cancelButtonContent == value)
        //        {
        //            return;
        //        }

        //        var oldValue = _cancelButtonContent;
        //        _cancelButtonContent = value;
        //        RaisePropertyChanged(CancelButtonContentPropertyName);
        //    }
        //}

        public const string CanProcessBackOrderPropertyName = "CanProcessBackOrder";
        private bool _canProcessBackOrder = false;

        public bool CanProcessBackOrder
        {
            get { return _canProcessBackOrder; }

            set
            {
                if (_canProcessBackOrder == value)
                {
                    return;
                }

                var oldValue = _canProcessBackOrder;
                _canProcessBackOrder = value;
                RaisePropertyChanged(CanProcessBackOrderPropertyName);
            }
        }

        public const string CanProcessAwaitingStockPropertyName = "CanProcessAwaitingStock";
        private bool _canProcessAwaitingStock = false;

        public bool CanProcessAwaitingStock
        {
            get { return _canProcessAwaitingStock; }

            set
            {
                if (_canProcessAwaitingStock == value)
                {
                    return;
                }

                var oldValue = _canProcessAwaitingStock;
                _canProcessAwaitingStock = value;
                RaisePropertyChanged(CanProcessAwaitingStockPropertyName);
            }
        }

        public const string ProcessingBackOrderPropertyName = "ProcessingBackOrder";
        private bool _processingBackOrder = false;

        public bool ProcessingBackOrder
        {
            get { return _processingBackOrder; }

            set
            {
                if (_processingBackOrder == value)
                {
                    return;
                }

                var oldValue = _processingBackOrder;
                _processingBackOrder = value;
                RaisePropertyChanged(ProcessingBackOrderPropertyName);
            }
        }

        public const string ShowSuccessMessagePropertyName = "ShowSuccessMessage";
        private bool _showSuccessMessage = false;

        public bool ShowSuccessMessage
        {
            get { return _showSuccessMessage; }

            set
            {
                if (_showSuccessMessage == value)
                {
                    return;
                }

                var oldValue = _showSuccessMessage;
                _showSuccessMessage = value;
                RaisePropertyChanged(ShowSuccessMessagePropertyName);
            }
        }

        public const string IsBusyPropertyName = "IsBusy";
        private bool _isBusy = false;

        public bool IsBusy
        {
            get { return _isBusy; }

            set
            {
                if (_isBusy == value)
                {
                    return;
                }

                var oldValue = _isBusy;
                _isBusy = value;
                RaisePropertyChanged(IsBusyPropertyName);
            }
        }

        public const string MessagePropertyName = "Message";
        private string _message = "";

        public string Message
        {
            get { return _message; }

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

        public const string RejectReasonPropertyName = "RejectReason";
        private string _rejectReason = "";

        public string RejectReason
        {
            get { return _rejectReason; }

            set
            {
                if (_rejectReason == value)
                {
                    return;
                }

                var oldValue = _rejectReason;
                _rejectReason = value;
                RaisePropertyChanged(RejectReasonPropertyName);
            }
        }

        public const string SelectedReceiptPropertyName = "SelectedReceipt";
        private Receipt _selectedReceipt = null;

        public Receipt SelectedReceipt
        {
            get { return _selectedReceipt; }

            set
            {
                if (_selectedReceipt == value)
                {
                    return;
                }

                var oldValue = _selectedReceipt;
                _selectedReceipt = value;
                RaisePropertyChanged(SelectedReceiptPropertyName);
            }
        }

        public const string ViewDispatchedPropertyName = "ViewDispatched";
        private bool _viewDispatched = false;

        public bool ViewDispatched
        {
            get { return _viewDispatched; }

            set
            {
                if (_viewDispatched == value)
                {
                    return;
                }

                var oldValue = _viewDispatched;
                _viewDispatched = value;
                RaisePropertyChanged(ViewDispatchedPropertyName);
            }
        }

        public const string SaleDiscountPropertyName = "SaleDiscount";
        private decimal _saleDiscount = 0m;

        public decimal SaleDiscount
        {
            get { return _saleDiscount; }

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

        public const string TotalProductDiscountPropertyName = "TotalProductDiscount";
        private decimal _totalProductDiscount = 0m;

        public decimal TotalProductDiscount
        {
            get { return _totalProductDiscount; }

            set
            {
                if (_totalProductDiscount == value)
                {
                    return;
                }

                var oldValue = _totalProductDiscount;
                _totalProductDiscount = value;
                RaisePropertyChanged(TotalProductDiscountPropertyName);
            }
        }

        #endregion

        #endregion

        #region Methods

        private void DoLoad()
        {

            _productPackagingSummaryService.ClearBuffer();
            ProcessingBackOrder = false;
            _order = GetEntityById(typeof (Order), OrderIdLookup) as Order;

            Status = GetOrderStatus(_order);
            OrderId = _order.DocumentReference;
            SaleDiscount = _order.SaleDiscount;
            DateRequired = _order.DateRequired;
            DateSubmitted = _order.DocumentDateIssued;
            CreatedByUser = _order.DocumentIssuerUser == null ? string.Empty : _order.DocumentIssuerUser.Username;
            SalesmanUsername = _order.DocumentIssuerUser.Username;
            OutletName = _order.IssuedOnBehalfOf.Name;
            RouteName = ((Outlet) _order.IssuedOnBehalfOf).Route.Name;
            OutletId = _order.IssuedOnBehalfOf.Id;
            LineItems.Clear();

            CanProcessBackOrder = false;
            IsRejected = Status == "Rejected";
            if (IsRejected)
                RejectReason = _order.Note;

            if (_loadBackOrder)
                return;
            if (LoadLostSale)
            {
                LoadOrderLostSale(_order);
                LoadCreditNoteInfo();
                return;
            }
            if (LoadDelivered)
            {
                LoadDeliveredLineItems(_order);
                LoadCreditNoteInfo();
                return;
            }
            if (!ViewDispatched)
            {
                LoadOrderLineItems(_order);
            }
            else
            {
                LoadDispatchedLineItems(_order);
            }

            if (_order.Status == DocumentStatus.Closed || _order.Status == DocumentStatus.OrderDispatchedToPhone)
            {
                LoadCreditNoteInfo();
            }
        }

        private string GetOrderStatus(Order order)
        {
            using (var container = NestedContainer)
            {
                IOtherUtilities _otherUtilities = Using<IOtherUtilities>(container);
                IDispatchNoteRepository _dispatchNoteService = Using<IDispatchNoteRepository>(container);

                var retVal = _otherUtilities.BreakStringByUpperCB(order.Status.ToString());
                if (order.Status == DocumentStatus.Closed)
                    return "Delivered";

                if (order.Status == DocumentStatus.OrderDispatchedToPhone)
                    if (
                        _dispatchNoteService.GetAll().OfType<DispatchNote>().Any(
                            n => n.DispatchType == DispatchNoteType.Delivery && n.OrderId == order.Id))
                        return "Partially Delivered";

                return retVal;
            }
        }

        private void LoadOrderLineItems(Order order)
        {
            //consolidated, then sale product then returnables
            List<OrderLineItem> itemsToLoad = new List<OrderLineItem>();
            order.LineItems.Where(n => n.LineItemType != OrderLineItemType.BackOrder &&
                                       n.LineItemType != OrderLineItemType.ProcessedBackOrder)
                 .OrderBy(n => n.LineItemType).Where(n => n.Product is ConsolidatedProduct).ToList()
                 .ForEach(itemsToLoad.Add);

            order.LineItems.Where(n => n.LineItemType != OrderLineItemType.BackOrder &&
                                       n.LineItemType != OrderLineItemType.ProcessedBackOrder)
                 .OrderBy(n => n.LineItemType).Where(n => n.Product is SaleProduct).ToList()
                 .ForEach(itemsToLoad.Add);

            order.LineItems.Where(n => n.LineItemType != OrderLineItemType.BackOrder &&
                                       n.LineItemType != OrderLineItemType.ProcessedBackOrder)
                 .OrderBy(n => n.LineItemType).Where(n => n.Product is ReturnableProduct).ToList()
                 .ForEach(itemsToLoad.Add);

            foreach (var item in itemsToLoad)
            {
                AddLineItem(item.Product.Id, item.Product.Description, item.Value, item.LineItemVatValue,
                            item.LineItemVatTotal, item.Qty, item.LineItemTotal,
                            IsEditable = (item.Product.GetType() != typeof (ReturnableType)),
                            item.Product.GetType().ToString().Split('.').Last(),
                            item.Id, item.ProductDiscount, item.LineItemType, item.DiscountType);

                bool isEditable = false;
                if (item.LineItemType != OrderLineItemType.Discount &&
                    (item.Product is SaleProduct || item.Product is ConsolidatedProduct))
                {
                    _productPackagingSummaryService.AddProduct(item.Product.Id, item.Qty, false, false, true);
                    isEditable = true;
                }
            }

            //load back order
            foreach (var item in order.LineItems.Where(n => n.LineItemType == OrderLineItemType.BackOrder))
            {
                InsertIntoLineItem(item);
            }
            //load processed backorder
            foreach (var item in order.LineItems.Where(n => n.LineItemType == OrderLineItemType.ProcessedBackOrder))
            {
                InsertIntoLineItem(item);

            }
        }

        private void LoadDispatchedLineItems(Order order)
        {
            using (var container = NestedContainer)
            {
                IDispatchNoteRepository _dispatchNoteService = Using<IDispatchNoteRepository>(container);

                var dispatchNote =
                    _dispatchNoteService.GetAll().OfType<DispatchNote>().Where(
                        n => n.OrderId == order.Id && n.DispatchType == DispatchNoteType.DispatchToPhone);
                var dnLis = dispatchNote.SelectMany(n => n.LineItems).ToList();
                dnLis.ForEach(n =>
                              AddLineItem(
                                  n.Product.Id,
                                  n.Product.Description,
                                  n.Value,
                                  order.LineItems.FirstOrDefault(l => l.Product.Id == n.Product.Id).LineItemVatValue,
                                  order.LineItems.FirstOrDefault(l => l.Product.Id == n.Product.Id).LineItemVatTotal,
                                  n.Qty,
                                  n.LineItemTotal,
                                  false,
                                  n.Product.GetType().ToString().Split('.').Last(),
                                  n.Id, n.ProductDiscount,
                                  n.LineItemType,
                                  n.DiscountType
                                  )
                    );
            }
        }

        public bool LoadDelivered = false;

        private void LoadDeliveredLineItems(Order order)
        {
            using (var container = NestedContainer)
            {
                IDispatchNoteRepository _dispatchNoteService = Using<IDispatchNoteRepository>(container);

                List<DispatchNote> dns =
                    _dispatchNoteService.GetAll().OfType<DispatchNote>().Where(
                        n => n.OrderId == order.Id && n.DispatchType == DispatchNoteType.Delivery).ToList();
                foreach (var dn in dns)
                {
                    foreach (var li in dn.LineItems)
                    {
                        AddLineItem(li);
                    }
                }
            }
        }

        public bool LoadLostSale = false;

        private void LoadOrderLostSale(Order order)
        {
            foreach (var item in order.LineItems.Where(n => n.LineItemType == OrderLineItemType.LostSale))
            {
                AddLineItem(item);
                CalcTotals();
            }
        }

        private void AddLineItem(OrderLineItem oli)
        {
            using (var container = NestedContainer)
            {
                IOtherUtilities _otherUtilities = Using<IOtherUtilities>(container);

                ApproveSalesmanOrderItem li = null;
                int sequenceNo = 1;
                if (LineItems.Count > 0)
                {
                    sequenceNo = LineItems.Max(n => n.SequenceNo) + 1;
                }
                if (LineItems.Any(n => n.ProductId == oli.Product.Id))
                {
                    li = LineItems.First(n => n.ProductId == oli.Product.Id);
                    li.Qty += oli.Qty;
                    li.TotalLineItemVatAmount += oli.LineItemVatTotal;
                    li.LineItemVatValue = oli.LineItemVatValue;
                    li.TotalPrice += oli.LineItemTotal;
                }
                else
                {
                    li = new ApproveSalesmanOrderItem(_otherUtilities)
                             {
                                 SequenceNo = sequenceNo,
                                 ProductId = oli.Product.Id,
                                 Product = oli.Product.Description,
                                 UnitPrice = oli.Value,
                                 LineItemVatValue = oli.LineItemVatValue,
                                 TotalLineItemVatAmount = oli.LineItemVatTotal,
                                 Qty = oli.Qty,
                                 TotalPrice = oli.LineItemTotal,
                                 IsEditable = false,
                                 LineItemId = oli.Id,
                                 CanEdit = false,
                                 CanRemove = false,
                                 ProductType = oli.Product.GetType().ToString().Split('.').Last(),
                                 HlnkDeleteContent =
                                     GetLocalText("sl.approveOrder.lineItemsGrid.deleteLineItem"),
                             };
                    LineItems.Add(li);
                }
                CalcTotals();
            }
        }

        private void AddLineItem(DispatchNoteLineItem dnLi)
        {
            using (var container = NestedContainer)
            {
                IOtherUtilities _otherUtilities = Using<IOtherUtilities>(container);

                var orderLi = _order.LineItems.First(n => n.Product.Id == dnLi.Product.Id);
                if (orderLi != null)
                    if (dnLi.Value != orderLi.Value)
                        dnLi.Value = _order.LineItems.First(n => n.Product.Id == dnLi.Product.Id).Value;

                ApproveSalesmanOrderItem li = null;
                int sequenceNo = 1;
                if (LineItems.Count > 0)
                {
                    sequenceNo = LineItems.Max(n => n.SequenceNo) + 1;
                }

                var dnLiVatValue =
                    _order.LineItems.FirstOrDefault(n => n.Product.Id == dnLi.Product.Id).LineItemVatValue;
                var dnLiVatTotal =
                    _order.LineItems.FirstOrDefault(n => n.Product.Id == dnLi.Product.Id).LineItemVatTotal;

                if (LineItems.Any(n => n.ProductId == dnLi.Product.Id
                                       && n.OrderLineItemType == dnLi.LineItemType
                                       && n.LineItemDiscountType == dnLi.DiscountType
                    ))
                {
                    li = LineItems.First(n => n.ProductId == dnLi.Product.Id);
                    li.Qty += dnLi.Qty;
                    li.TotalLineItemVatAmount += dnLiVatTotal;
                    li.LineItemVatValue = dnLiVatValue;
                    li.TotalPrice += dnLi.LineItemTotal;
                }
                else
                {
                    li = new ApproveSalesmanOrderItem(_otherUtilities)
                             {
                                 SequenceNo = sequenceNo,
                                 ProductId = dnLi.Product.Id,
                                 Product = dnLi.Product.Description,
                                 UnitPrice = dnLi.Value,
                                 LineItemVatValue = dnLiVatValue,
                                 TotalLineItemVatAmount = dnLiVatTotal,
                                 Qty = dnLi.Qty,
                                 TotalPrice = dnLi.LineItemTotal,
                                 IsEditable = false,
                                 LineItemId = dnLi.Id,
                                 CanEdit = false,
                                 CanRemove = false,
                                 ProductType = dnLi.Product.GetType().ToString().Split('.').Last(),
                                 HlnkDeleteContent =
                                     GetLocalText("sl.approveOrder.lineItemsGrid.deleteLineItem"),
                             };
                    LineItems.Add(li);
                }
                CalcTotals();
            }
        }

        private bool _loadBackOrder = false;

        private void LoadBackOrder()
        {
            ProcessingBackOrder = true;
            _loadBackOrder = true;
            DoLoad();
            ProcessingBackOrder = true;

            LineItems.Clear();
            foreach (var item in _order.LineItems.Where(n => n.LineItemType == OrderLineItemType.BackOrder))
            {
                LoadBackOrderlineItem(item.Product.Id, item.Product.Description, item.Value, item.LineItemVatValue,
                                      item.LineItemVatTotal, item.LineItemVatValue,
                                      item.Qty, item.LineItemTotal, IsEditable, item.Id,
                                      item.Product.GetType().ToString().Split('.').Last());
            }

            OriginalLineItems = new Dictionary<int, decimal>();
            LineItems.ToList().ForEach(n => OriginalLineItems.Add(n.SequenceNo, n.Qty));
            _loadBackOrder = false;
        }

        private void LoadCreditNoteInfo()
        {
            using (var container = NestedContainer)
            {
                IInvoiceRepository _invoiceService = Using<IInvoiceRepository>(container);
                var _creditNoteService = Using<ICreditNoteRepository>(container);
                var invoice = _invoiceService.GetInvoiceByOrderId(OrderIdLookup);
                if (invoice == null)
                    return;

                List<CreditNote> InvCreditNotes =
                    _creditNoteService.GetAll().OfType<CreditNote>().Where(
                        n => n.InvoiceId == invoice.Id && n.CreditNoteType != CreditNoteType.LostSale).ToList();
                var creditNoteLineItems = new List<CreditNoteLineItem>();

                if (InvCreditNotes.Count == 0)
                    return;
                InvCreditNotes.SelectMany(n => n.LineItems).ToList().ForEach(creditNoteLineItems.Add);
                creditNoteLineItems.ForEach(n =>
                                            AddReceivedReturnables(
                                                n.Product.Id,
                                                n.Product.Description,
                                                n.Value,
                                                n.LineItemVatValue,
                                                -n.LineItemVatTotal,
                                                n.Qty,
                                                -n.LineItemTotal,
                                                false,
                                                n.Product.GetType().ToString().Split('.').Last(),
                                                n.Id
                                                )
                    );
            }
        }

        private void RunValidateOrderForApproval()
        {
            IsApproved = false;
            LineItemsInventoryInfoList = new List<OrderLineItemInventoryInfo>();
            Validated = false;
            ValidateForApproval(false);
        }

        private void RunApproveCommand()
        {
            if (!ProcessingBackOrder)
            {
                if (Validated)
                    ApproveOrder();
            }
            else
            {
                ApproveBackOrder();
            }
        }

        public void RunCreateBackOrderAndApprove()
        {
            CreateBackOrders();
            ValidateForApproval(false);
            if (Validated)
                ApproveOrder();
        }

        private void RunCreateMessageOfInvalidOrders()
        {
            string msg = "";
            if (LineItemsInventoryInfoList.Count() > 0)
            {
                foreach (var item in LineItemsInventoryInfoList)
                {
                    msg += "\n" + item.ProductDesc + ": \n" + "\t" +
                           /*"Required:"*/
                           GetLocalText("sl.approveOrder.approve.messageBox.required") + " "
                           + item.RequiredInv + ",   " +
                           /*"Available:"*/
                           GetLocalText("sl.approveOrder.approve.messageBox.available") + " " +
                           item.AvailableInv + "\n";
                }
            }

            Message = msg;
        }

        private void RunRejectCommand()
        {
            if (!ProcessingBackOrder)
                RejectOrder();
            else
                CancelBackOrder();
        }

        private void ApproveOrder()
        {
            using (var container = NestedContainer)
            {
                IApproveSalesmanOrderWFManager _approveOrderWFManager = Using<IApproveSalesmanOrderWFManager>(container);
                //ICreditNoteService _creditNoteService = Using<ICreditNoteService>(container);

                IsApproved = false;
                //awaiting stock, back orders, lost sales
                if (childLineItems != null)
                    if (childLineItems.Count() > 0)
                    {
                        foreach (var item in childLineItems)
                        {
                            _order.CreateLineItem(item);
                        }
                    }
                for (int i = 0; i < LineItems.Count(); i++)
                {
                    if (LineItems[i].LineItemId == Guid.Empty)
                        LineItems[i].LineItemId = Guid.NewGuid();
                }
                foreach (var item in LineItems)
                {
                    var orderLineItem = _order.LineItems.FirstOrDefault(n => n.Id == item.LineItemId);
                    //is it the same
                    if (orderLineItem != null && orderLineItem.Qty == item.Qty)
                        continue;
                        //is it an update
                    else if (orderLineItem != null && orderLineItem.Qty != item.Qty)
                    {
                        _order.ChangeLineItemQty(item.LineItemId, item.Qty);
                        continue;
                    }
                        //is it new
                    else if (orderLineItem == null)
                    {
                        OrderLineItem oli = CreateNewLineItem(item.LineItemId, item);
                        oli.Description = oli.Id.ToString();
                        //helps identofy LIs added during approval; important for tracking back order, lost sale and dispatches
                        _order.AddLineItem(oli);
                        continue;
                    }
                }

                List<Guid> rolis = new List<Guid>(); //removed order line items
                foreach (var li in LineItemsRemoved)
                {
                    rolis.Add(li);
                    _order.RemoveLineItem(li);
                }
                _approveOrderWFManager.remvdLineItemIds = rolis;
                LineItemsRemoved.Clear();

                _approveOrderWFManager.SubmitChanges(_order);

                _approveOrderWFManager.remvdLineItemIds.Clear();

                IsApproved = true;
            }
        }

        private void CreateBackOrders()
        {
            foreach (var item in LineItemsInventoryInfoList)
            {
                var lineItem = LineItems.First(n => n.SequenceNo == item.LineItemSequenceNo);
                if (lineItem != null)
                {
                    decimal backOrder = lineItem.Qty - item.AvailableInv;
                    UpdateLineItem(lineItem.SequenceNo,
                                   lineItem.Qty,
                                   lineItem.LineItemVatValue,
                                   lineItem.TotalLineItemVatAmount,
                                   lineItem.TotalPrice,
                                   backOrder,
                                   0, true);
                }
            }
        }

        private void RejectOrder()
        {
            using (var container = NestedContainer)
            {
                IOrderRepository _orderService = Using<IOrderRepository>(container);
                IApproveSalesmanOrderWFManager _approveOrderWFManager = Using<IApproveSalesmanOrderWFManager>(container);
                IsRejected = false;
                var order = _orderService.GetById(OrderIdLookup) as Order;
                try
                {
                    order.Note = RejectReason;
                    _approveOrderWFManager.RejectOrder(order, RejectReason);

                    IsRejected = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "An error was encountered while rejecting Order " + order.DocumentReference +
                        "\nError Details:\n" + ex + ".",
                        "Distriburt: Order on Behalf of " + SalesmanUsername, MessageBoxButton.OK);
                }
            }
        }

        private void InsertIntoLineItem(OrderLineItem orderLineItem)
        {
            //get confirmed/discount line item
            OrderLineItem oli =
                _order.LineItems.FirstOrDefault(
                    n =>
                    n.Description == orderLineItem.Description && n.LineItemType == OrderLineItemType.PostConfirmation);
            if (oli == null)
                oli =
                    _order.LineItems.FirstOrDefault(
                        n =>
                        n.Id.ToString() == orderLineItem.Description && n.LineItemType == OrderLineItemType.Discount);

            if (oli == null)
            {
                throw new Exception("Main line item " + orderLineItem.Product.Description + " whose Id is " +
                                    orderLineItem.Description + " is missing in list.");
            }

            var lineItemToUpdate = LineItems.FirstOrDefault(n => n.LineItemId == oli.Id);

            if (lineItemToUpdate == null)
            {
                throw new Exception("Line item " + orderLineItem.Product.Description + " whose Id is " +
                                    orderLineItem.Description + " is missing in list.");
            }
            switch (orderLineItem.LineItemType)
            {
                case OrderLineItemType.BackOrder:
                    lineItemToUpdate.BackOrder += orderLineItem.Qty;
                    lineItemToUpdate.ProcessedQty = (lineItemToUpdate.Qty - lineItemToUpdate.BackOrder);
                    break;
                case OrderLineItemType.LostSale:
                    lineItemToUpdate.LostSale = orderLineItem.Qty;
                    break;
                case OrderLineItemType.ProcessedBackOrder:
                    lineItemToUpdate.BackOrder -= orderLineItem.Qty;
                    lineItemToUpdate.ProcessedQty = (lineItemToUpdate.Qty - lineItemToUpdate.BackOrder);
                    break;
            }

        }

        public void AddLineItem(Guid productId, string productDesc, decimal unitPrice, decimal vatValue,
                                decimal vatAmount, decimal qty, decimal totalPrice, bool isEditable, string liProdType,
                                Guid lineItemID, decimal productDiscount, OrderLineItemType orderLineItemType,
                                DiscountType discountType)
        {
            using (var container = NestedContainer)
            {
                IConfigService _configService = Using<IConfigService>(container);
                var _inventoryService = Using<IInventoryRepository>(container);
                IProductRepository _productService = Using<IProductRepository>(container);
                IOtherUtilities _otherUtilities = Using<IOtherUtilities>(container);

                ApproveSalesmanOrderItem li = null;
                Inventory availableInv = _inventoryService.GetByProductIdAndWarehouseId(productId, _configService.Load().CostCentreId);
                Product product = null;

                if (liProdType == "ReturnableProduct") //#$@$%#^!!
                {
                    if (availableInv != null)
                        if (availableInv.Balance < 0)
                            availableInv.Balance = 0;
                    product = _productService.GetById(productId);
                }


                int sequenceNo = 1;
                if (LineItems.Count > 0)
                {
                    sequenceNo = LineItems.Max(n => n.SequenceNo) + 1;
                }
                if (!LoadForProcessing)
                    isEditable = false;
                //Check the line item exists by productId and update it ..
                if (LineItems.Any(n => n.ProductId == productId && n.OrderLineItemType == orderLineItemType)) //
                {
                    li = LineItems.FirstOrDefault(n => n.ProductId == productId); //lineItem to update ..
                    if (_order.Status == DocumentStatus.Confirmed ||
                        (_order.Status == DocumentStatus.OrderPendingDispatch && ProcessingBackOrder))
                        // editing at confirm order and at process back order
                    {
                        if (!LoadForViewing)
                        {
                            li.Qty += qty;
                            li.TotalLineItemVatAmount += vatAmount;
                            li.LineItemVatValue = vatValue;
                            li.TotalPrice += totalPrice;
                            li.LineItemTotalProductDiscount += (productDiscount*qty);
                            li.ProcessedQty = (li.Qty - li.BackOrder);
                            li.ProcessedQty = li.ProcessedQty < 0 ? 0 : li.ProcessedQty;
                        }
                    }
                    else if (_order.Status == DocumentStatus.OrderPendingDispatch ||
                             _order.Status == DocumentStatus.OrderDispatchedToPhone ||
                             _order.Status == DocumentStatus.Closed ||
                             _order.Status == DocumentStatus.Cancelled ||
                             _order.Status == DocumentStatus.Rejected)
                        //load awaiting stock etc ...
                    {
                        if (!ProcessingBackOrder)
                        {
                            var item = _order.LineItems.FirstOrDefault(n => n.Id == lineItemID);
                            try
                            {
                                switch (item.LineItemType)
                                {
                                    case OrderLineItemType.BackOrder:
                                        li.BackOrder += qty;
                                        li.ProcessedQty = li.Qty - li.BackOrder;
                                        break;
                                    case OrderLineItemType.LostSale:
                                        li.LostSale = qty;
                                        break;
                                    case OrderLineItemType.ProcessedBackOrder:
                                        li.BackOrder -= qty;
                                        li.ProcessedQty = li.Qty - li.BackOrder;
                                        break;
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                else
                {
                    #region New LineItem

                    li = new ApproveSalesmanOrderItem(_otherUtilities)
                             {
                                 SequenceNo = sequenceNo,
                                 ProductId = productId,
                                 Product = productDesc,
                                 UnitPrice = unitPrice,
                                 LineItemVatValue = vatValue,
                                 TotalLineItemVatAmount = vatAmount,
                                 Qty = qty,
                                 TotalPrice = totalPrice,
                                 IsEditable = isEditable,
                                 LineItemId = (Guid) lineItemID,
                                 AvailableProductInv = availableInv == null ? 0 : availableInv.Balance,
                                 LiProductType = liProdType,
                                 CanEdit = isEditable,
                                 CanRemove = isEditable,
                                 ProductType = liProdType,
                                 OrderLineItemType = orderLineItemType,
                                 ProductDiscount = productDiscount,
                                 LineItemTotalProductDiscount = productDiscount*qty,
                                 LineItemDiscountType = discountType,
                                 HlnkDeleteContent = GetLocalText("sl.approveOrder.lineItemsGrid.deleteLineItem"),
                             };

                    if (liProdType == "ReturnableProduct") li.CanEdit = false;
                    if (liProdType == "ReturnableProduct") li.CanRemove = false;

                    if (liProdType == "ReturnableProduct" && ((ReturnableProduct) product).Capacity > 1)
                        //cn: containers to be edited independently
                    {
                        li.CanEdit = true;
                        li.CanRemove = true;
                    }

                    //overriding editing determiners!!
                    if (li.OrderLineItemType == OrderLineItemType.Discount)
                    {
                        li.CanEdit = false;
                        li.CanRemove = false;
                    }

                    if (Status != "New" && Status != "Confirmed")
                    {
                        li.CanEdit = false;
                        li.CanRemove = false;
                    }

                    if (LoadForViewing)
                    {
                        li.CanEdit = false;
                        li.CanRemove = false;
                    }
                    LineItems.Add(li);

                    #endregion
                }
                CanProcessBackOrder = li.BackOrder > 0;
                CalcTotals();
            }
        }

        public void AddReceivedReturnables(Guid productId, string productDesc, decimal unitPrice, decimal vatValue,
                                           decimal vatAmount, decimal qty, decimal totalPrice, bool isEditable,
                                           string liProdType, Guid lineItemID)
        {
            using (var container = NestedContainer)
            {
                IOtherUtilities _otherUtilities = Using<IOtherUtilities>(container);
                var _inventoryService = Using<IInventoryRepository>(container);
                IConfigService _configService = Using<IConfigService>(container);
                ApproveSalesmanOrderItem li = null;
                int sequenceNo = 0;
                Inventory availableInv = _inventoryService.GetByProductIdAndWarehouseId(productId, _configService.Load().CostCentreId);

                if (liProdType == "ReturnableProduct") //#$@$%#^!!
                    if (availableInv != null)
                        if (availableInv.Balance < 0)
                            availableInv.Balance = 0;

                if (_order.Status == DocumentStatus.Confirmed || _order.Status == DocumentStatus.OrderPendingDispatch)
                    // editing at confirm order and at process back order
                {
                    li.Qty += qty;
                    li.TotalLineItemVatAmount += vatAmount;
                    li.LineItemVatValue = vatValue;
                    li.TotalPrice += totalPrice;
                    li.ProcessedQty = (li.Qty - li.BackOrder);
                    li.ProcessedQty = li.ProcessedQty < 0 ? 0 : li.ProcessedQty;
                }
                else
                {
                    li = new ApproveSalesmanOrderItem(_otherUtilities)
                             {
                                 SequenceNo =
                                     LineItems.Count > 0 ? (LineItems.Max(n => n.SequenceNo) + 1) : (sequenceNo + 1),
                                 ProductId = productId,
                                 Product = productDesc,
                                 UnitPrice = unitPrice,
                                 LineItemVatValue = vatValue,
                                 TotalLineItemVatAmount = vatAmount,
                                 Qty = qty,
                                 TotalPrice = totalPrice,
                                 IsEditable = isEditable,
                                 LineItemId = (Guid) lineItemID,
                                 AvailableProductInv = availableInv == null ? 0 : availableInv.Balance,
                                 LiProductType = liProdType,
                                 CanEdit = isEditable,
                                 CanRemove = isEditable,
                                 ProductType = liProdType,
                                 HlnkDeleteContent =
                                     GetLocalText("sl.approveOrder.lineItemsGrid.deleteLineItem"),
                             };
                }
                LineItems.Add(li);
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
            List<PackagingSummary> summarypro = _productPackagingSummaryService.GetProductSummary();
            //items in LineItem but not in summarypro to be deleted
            List<ApproveSalesmanOrderItem> toDelete =
                LineItems //.Where(n => n.OrderLineItemType != OrderLineItemType.Discount)
                    .Where(n => !(summarypro.Select(s => s.Product.Id)).Contains(n.ProductId)).ToList();
            foreach (var item in toDelete)
            {
                RemoveLineItem(item.SequenceNo);
            }
            foreach (PackagingSummary item in summarypro)
            {
                AddOrUpdateLineItem(item);
            }

            //process discount here!!!!
            ProcessDiscounts();
        }

        private void AddOrUpdateLineItem(PackagingSummary packagingSummary)
        {
            using (var container = NestedContainer)
            {
                IDiscountProWorkflow _discountProService = Using<IDiscountProWorkflow>(container);

                LineItemPricingInfo pricingInfo = _discountProService.GetLineItemPricing(packagingSummary, OutletId);

                //if item in LineItems update it else insert
                ApproveSalesmanOrderItem li =
                    LineItems.FirstOrDefault(
                        n =>
                        n.ProductId == packagingSummary.Product.Id && n.OrderLineItemType != OrderLineItemType.Discount);

                if (li != null)
                {
                    UpdateLineItem(li.SequenceNo, packagingSummary.Quantity, pricingInfo.TotalVatAmount,
                                   pricingInfo.TotalPrice, pricingInfo.ProductDiscount, pricingInfo.TotalProductDiscount);
                }
                else
                {
                    AddLineItem(packagingSummary.Product.Id, packagingSummary.Product.Description, pricingInfo.UnitPrice,
                                pricingInfo.VatValue, pricingInfo.TotalVatAmount,
                                packagingSummary.Quantity, pricingInfo.TotalPrice, IsEditable,
                                packagingSummary.Product.GetType().ToString().Split('.').Last(),
                                Guid.Empty, pricingInfo.ProductDiscount, 0, 0);
                }
            }
        }

        public void AddAmmendedLineItem(Guid productId, decimal qty, int sequenceNo = 0)
        {
            using (var container = NestedContainer)
            {
                IDiscountProWorkflow _discountProService = Using<IDiscountProWorkflow>(container);
                IProductRepository _productService = Using<IProductRepository>(container);
                Product product = _productService.GetById(productId);
                LineItemPricingInfo pi = _discountProService.GetLineItemPricing(new PackagingSummary
                                                                                    {
                                                                                        Product = product,
                                                                                        Quantity = qty
                                                                                    }, OutletId);
                if (sequenceNo == 0)
                {
                    AddLineItem(product.Id, product.Description, pi.UnitPrice,
                                pi.VatValue, pi.TotalVatAmount,
                                qty, pi.TotalPrice, IsEditable,
                                product.GetType().ToString().Split('.').Last(),
                                Guid.Empty, pi.ProductDiscount, 0, 0);
                }
                else
                {
                    UpdateLineItem(sequenceNo, qty, pi.TotalVatAmount, pi.TotalPrice, pi.ProductDiscount,
                                   pi.TotalProductDiscount);
                }
            }
        }

        #region Process Discounts

        private List<ApproveSalesmanOrderItem> discountLineItems = null;

        public void ProcessDiscounts()
        {
            SaleDiscount = 0m;
            //1.process discount for each line item not a dicsount
            //2.add/update/delete discount Lineitems in the order.
            discountLineItems = new List<ApproveSalesmanOrderItem>();
            foreach (
                ApproveSalesmanOrderItem lineItem in
                    LineItems.Where(
                        n => n.OrderLineItemType != OrderLineItemType.Discount && n.ProductType != "ReturnableProduct"))
            {
                //1. add product discount
                //2. add free of charge (Certain Product Quantity Certain Product)
                AddCertainProductQuantityCertainProductDiscount(lineItem);
            }

            //3. add free of charge (Certain Sale Value Certain Product)
            AddCertainSaleValueCertainProductDiscount();
            //1.add this to LineItems
            //2.lineItemIds of items to delete from db
            foreach (ApproveSalesmanOrderItem item in discountLineItems)
            {
                ApproveSalesmanOrderItem existing =
                    LineItems.FirstOrDefault(
                        n =>
                        n.OrderLineItemType == OrderLineItemType.Discount &&
                        n.LineItemDiscountType == item.LineItemDiscountType && n.ProductId == item.ProductId);

                if (existing == null)
                {
                    LineItems.Add(item);
                    continue;
                }
                if (existing != null && existing.Qty != item.Qty)
                {
                    existing.Qty = item.Qty;
                }
            }

            foreach (OrderLineItem item in _order.LineItems.Where(n => n.LineItemType == OrderLineItemType.Discount))
            {
                //if item is in order.LineItems and not in discountLineItems, flag for delete
                if (!discountLineItems.Any(n => n.ProductId == item.Product.Id))
                {
                    LineItemsRemoved.Add(item.Id);
                }
            }

            ProcessDiscountMixedPackReturnables();

            //4. add sale value discount
            AddSaleDiscount();
            CalcTotals();

        }

        private void AddCertainSaleValueCertainProductDiscount()
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

        private void AddCertainProductQuantityCertainProductDiscount(ApproveSalesmanOrderItem lineItem)
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

        private void AddSaleDiscount()
        {
            using (var container = NestedContainer)
            {
                IDiscountProWorkflow _discountProService = Using<IDiscountProWorkflow>(container);


                SaleDiscount = 0;
                if (OutletId == Guid.Empty)
                {
                    SaleDiscount = 0m;
                }
                else
                {
                    CalcTotals();
                    SaleDiscount = _discountProService.GetSalevalue(TotalGross, OutletId);
                }
            }
        }

        private void DisplayProductAsDiscountAndAddToDiscountLineItems(ProductAsDiscount productAsDiscount)
        {
            using (var container = NestedContainer)
            {

                IProductRepository _productService = Using<IProductRepository>(container);

                Product product = _productService.GetById(productAsDiscount.ProductId);

                AddOrUpdateLineItemFromDiscount(productAsDiscount, product);
            }
        }

        private List<ReturnableProduct> discProdReturnables = null;

        private void AddOrUpdateLineItemFromDiscount(ProductAsDiscount productAsDiscount, Product product)
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
                decimal totalPrice = 0m;
                decimal productDiscount = 0m;
                decimal totalProductDiscount = 0m;
                decimal qty = productAsDiscount.Quantity;

                if (prod.Id != product.Id)
                {
                    if ((prod as ReturnableProduct).Capacity > 1) //returnable bulk container
                        qty = (int) (productAsDiscount.Quantity/(prod as ReturnableProduct).Capacity);
                }
                if (prod.Id != product.Id)
                {
                    LineItemPricingInfo pi = GetLineItemPricing(new PackagingSummary
                                                                    {
                                                                        Product = prod,
                                                                        Quantity = qty
                                                                    },
                                                                OutletId);
                    unitPrice = pi.UnitPrice;
                    vatValue = pi.VatValue;
                    totalVatAmount = pi.TotalVatAmount;
                    totalPrice = pi.TotalPrice;
                    productDiscount = pi.ProductDiscount;
                    totalProductDiscount = pi.TotalProductDiscount;
                }

                ApproveSalesmanOrderItem existingItem = null;
                if (prod.Id == product.Id) //is the discount product
                {
                    existingItem = discountLineItems.FirstOrDefault(n =>
                                                                    n.OrderLineItemType ==
                                                                    OrderLineItemType.Discount &&
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
                if (existingItem != null)
                {
                    if (prod.Id != product.Id)
                    {
                        if ((prod as ReturnableProduct).Capacity > 1) //returnable bulk container
                            existingItem.Qty += qty;
                        else //sale product returnable
                            existingItem.Qty += qty;
                    }
                    else //sale product
                        existingItem.Qty += qty;
                    existingItem.TotalLineItemVatAmount += totalVatAmount;
                    existingItem.TotalPrice += totalPrice;
                    existingItem.LineItemTotalProductDiscount += totalProductDiscount;
                }
                else
                {
                    using (var container = NestedContainer)
                    {
                        IOtherUtilities _otherUtilities = Using<IOtherUtilities>(container);
                        existingItem = new ApproveSalesmanOrderItem(_otherUtilities)
                                           {
                                               SequenceNo = LineItems.Count() + 1,
                                               ProductDiscount = productDiscount,
                                               LineItemTotalProductDiscount = totalProductDiscount,
                                               IsEditable = false,
                                               LineItemId = Guid.Empty,
                                               ProductId = prod.Id,
                                               Product = prod.Description,
                                               //ParentProductId            = product.Id,
                                               Qty = qty,
                                               UnitPrice = unitPrice,
                                               TotalLineItemVatAmount = totalVatAmount,
                                               LineItemVatValue = vatValue,
                                               TotalPrice = totalPrice,
                                               ProductType = prod.GetType().ToString().Split('.').Last(),
                                               CanEdit = false,
                                               CanRemove = false,
                                               HlnkDeleteContent =
                                                   GetLocalText("sl.approveOrder.lineItemsGrid.deleteLineItem"),
                                           };
                        if (prod.Id == product.Id) //is the discount product
                        {
                            existingItem.LineItemDiscountType = productAsDiscount.DiscountType;
                            existingItem.OrderLineItemType = OrderLineItemType.Discount;
                        }
                        if (existingItem.OrderLineItemType == OrderLineItemType.Discount)
                        {
                            discountLineItems.Add(existingItem);
                        }
                        else
                        {
                            LineItems.Add(existingItem);
                        }
                    }
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
                                                  Product = GetEntityById(typeof(Product), n.ProductId) as Product,
                                                  Quantity = n.Qty,
                                              }).ToList());

            foreach (PackagingSummary ps in mixdPackReturns)
            {
                AddOrUpdateLineItem(ps);
            }

            _productPackagingSummaryService.ClearMixedPackReturnables();
        }

        #endregion

        public void RemoveLineItem(Guid productId, LineItemType lit)
        { 
            var delProduct =
                _productPackagingSummaryService.GetProductSummary().FirstOrDefault(
                    p => p.Product.Id == productId && p.IsEditable);
            string msg = "";
            List<PackagingSummary> delItems = _productPackagingSummaryService.GetProductSummaryByProduct(productId,
                                                                                                         delProduct.
                                                                                                             Quantity);
            foreach (PackagingSummary delitem in delItems)
            {
                msg += string.Format("\n\t{0} of {1} will be deleted", delitem.Quantity, delitem.Product.Description);
            }
            MessageBoxResult isConfirmed =
                MessageBox.Show("Are sure you want to delete the following product(s)" + msg,
                                "Delete Order Line item", MessageBoxButton.OKCancel);

            if (isConfirmed == MessageBoxResult.OK)
            {
                _productPackagingSummaryService.RemoveProduct(productId);
            }
            RefreshList();
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
                        UnitPrice = ((ConsolidatedProduct) product).TotalExFactoryValue(tier);
                    }
                    catch
                    {
                        UnitPrice = 0m;
                    }
                else
                    try
                    {
                        UnitPrice = product.TotalExFactoryValue(tier);
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

        private List<OrderLineItem> childLineItems = new List<OrderLineItem>();

        void UpdateLineItem(int sequenceNo, decimal qty, decimal vatAmount, decimal totalPrice, decimal productDiscount, decimal totalDiscount)
        {
            ApproveSalesmanOrderItem li = LineItems.FirstOrDefault(n => n.SequenceNo == sequenceNo);
            li.Qty = qty;
            li.TotalLineItemVatAmount = vatAmount;
            li.TotalPrice = totalPrice;
            li.ProductDiscount = productDiscount;
            li.LineItemTotalProductDiscount = totalDiscount;
            CalcTotals();
        }
        
        public void UpdateLineItem(int sequenceNo, decimal qty, decimal lineItemVatValue, decimal vatAmount, decimal totalPrice, decimal backOrder, decimal lostSale, bool updatedLI = false)
        {
            using (var container = NestedContainer)
            {
                
                IOtherUtilities _otherUtilities = Using<IOtherUtilities>(container);

                if (childLineItems == null)
                    childLineItems = new List<OrderLineItem>();
                ApproveSalesmanOrderItem lineItem = LineItems.FirstOrDefault(n => n.SequenceNo == sequenceNo);
                OrderLineItem oli = _order.LineItems.FirstOrDefault(n => n.Id == lineItem.LineItemId);
                //BackOrder, LostSales
                lineItem.Qty = qty;
                //item.Approved = approve;
                lineItem.BackOrder = backOrder;
                lineItem.LostSale = lostSale;
                lineItem.LineItemVatValue = lineItemVatValue;
                lineItem.TotalLineItemVatAmount = vatAmount;
                lineItem.LineItemVatValue = vatAmount;
                lineItem.TotalPrice = totalPrice;
                lineItem.ProcessedQty = lineItem.Qty - lineItem.BackOrder;
                lineItem.ProcessedQty = lineItem.ProcessedQty < 0 ? 0 : lineItem.ProcessedQty;

                if (oli != null)
                {
                    OrderLineItem originalOLi =
                        _order._allLineItems().FirstOrDefault(n => n.Id.ToString() == oli.Description);
                    if (originalOLi == null)
                    {
                        if (oli.LineItemType != OrderLineItemType.PostConfirmation)
                            originalOLi = oli;
                    }

                    if (childLineItems.Any(n => n.Description == oli.Id.ToString())) //same as updating existing
                    {
                        var toRemove = childLineItems.Where(n => n.Description == oli.Id.ToString());
                        toRemove.ToList().ForEach(n => childLineItems.Remove(n));
                    }

                    if (backOrder > 0) //Create or update Back Orders
                    {
                        childLineItems.Add(new OrderLineItem(Guid.NewGuid())
                                               {
                                                   Description = originalOLi.Id.ToString(),
                                                   LineItemSequenceNo = _order.LineItems.Count() + 1,
                                                   LineItemVatValue = oli.LineItemVatValue,
                                                   Qty = lineItem.BackOrder,
                                                   Product = oli.Product,
                                                   IsNew = true,
                                                   LineItemType = OrderLineItemType.BackOrder,
                                                   Value = oli.Value
                                               });
                    }
                    if (lostSale > 0) //Lost Sale : create or update
                    {
                        childLineItems.Add(new OrderLineItem(Guid.NewGuid())
                                               {
                                                   Description = originalOLi.Id.ToString(),
                                                   LineItemSequenceNo = _order.LineItems.Count() + 1,
                                                   LineItemVatValue = oli.LineItemVatValue,
                                                   Qty = lineItem.LostSale,
                                                   Product = oli.Product,
                                                   IsNew = true,
                                                   LineItemType = OrderLineItemType.LostSale,
                                                   Value = oli.Value
                                               });
                    }
                }
                else //line item was added during approval
                {
                    var updatingLi = LineItems.First(n => n.SequenceNo == sequenceNo);
                    if (updatingLi.LineItemId == Guid.Empty)
                        updatingLi.LineItemId = Guid.NewGuid();

                    if (childLineItems.Any(n => n.Description == updatingLi.LineItemId.ToString())) //add/update
                    {
                        var toRemove = childLineItems.Where(n => n.Description == oli.Id.ToString());
                        toRemove.ToList().ForEach(n => childLineItems.Remove(n));
                    }
                    if (backOrder > 0) //Create or update Back Orders
                    {
                        var newLineItem = new ApproveSalesmanOrderItem(_otherUtilities)
                                              {
                                                  AvailableProductInv = updatingLi.AvailableProductInv,
                                                  BackOrder = updatingLi.BackOrder,
                                                  LineItemVatValue = updatingLi.LineItemVatValue,
                                                  LostSale = updatingLi.LostSale,
                                                  Product = updatingLi.Product,
                                                  ProcessedQty = updatingLi.ProcessedQty,
                                                  ProductId = updatingLi.ProductId,
                                                  Qty = updatingLi.BackOrder,
                                                  SequenceNo = LineItems.Count + 1,
                                                  TotalPrice = updatingLi.TotalPrice,
                                                  UnitPrice = updatingLi.UnitPrice,
                                                  TotalLineItemVatAmount = updatingLi.TotalLineItemVatAmount,
                                                  HlnkDeleteContent =
                                                      GetLocalText("sl.approveOrder.lineItemsGrid.deleteLineItem"),
                                              };
                        newLineItem.LineItemId = Guid.NewGuid();
                        var newOrderLineItem = CreateNewLineItem(newLineItem.LineItemId, newLineItem,
                                                                 OrderLineItemType.BackOrder);
                        newOrderLineItem.Description = updatingLi.LineItemId.ToString();

                        childLineItems.Add(newOrderLineItem);
                    }
                    if (lostSale > 0) //Lost Sale : create or update
                    {
                        var newLineItem = new ApproveSalesmanOrderItem(_otherUtilities)
                                              {
                                                  AvailableProductInv = updatingLi.AvailableProductInv,
                                                  BackOrder = updatingLi.BackOrder,
                                                  LineItemVatValue = updatingLi.LineItemVatValue,
                                                  LostSale = updatingLi.LostSale,
                                                  Product = updatingLi.Product,
                                                  ProcessedQty = updatingLi.ProcessedQty,
                                                  ProductId = updatingLi.ProductId,
                                                  Qty = updatingLi.LostSale,
                                                  SequenceNo = LineItems.Count + 1,
                                                  TotalPrice = updatingLi.TotalPrice,
                                                  UnitPrice = updatingLi.UnitPrice,
                                                  TotalLineItemVatAmount = updatingLi.TotalLineItemVatAmount,
                                                  HlnkDeleteContent =
                                                      GetLocalText("sl.approveOrder.lineItemsGrid.deleteLineItem"),
                                              };
                        newLineItem.LineItemId = Guid.NewGuid();
                        var newOrderLineItem = CreateNewLineItem(newLineItem.LineItemId, newLineItem,
                                                                 OrderLineItemType.LostSale);
                        newOrderLineItem.Description = updatingLi.LineItemId.ToString();
                        childLineItems.Add(newOrderLineItem);
                    }
                }
                CalcTotals();
            }
        }

        public void UpdateBackOrderLineItem(int sequenceNo, int qty, decimal vatValue, decimal vat, decimal totalPrice, int backOrder, int lostSale, bool updatedLI = false)
    {
        if (childLineItems == null)
            childLineItems = new List<OrderLineItem>();
            //Order order = _orderService.GetByDocumentId(OrderIdLookup);
            ApproveSalesmanOrderItem item = LineItems.First(n => n.SequenceNo == sequenceNo);
            var oli =
                _order.LineItems.FirstOrDefault(
                    n => n.Id == LineItems.FirstOrDefault(l => l.SequenceNo == sequenceNo).LineItemId);
            //BackOrder, LostSales
            item.Qty =qty;
            //item.Approved = approve;
            item.BackOrder = backOrder;
            item.LostSale = lostSale;
            item.LineItemVatValue = vatValue;
            item.TotalLineItemVatAmount = vat;
            item.TotalPrice = totalPrice;

            //remove existing
            if (childLineItems.Any(n => n.Product.Id == oli.Product.Id))
            {
                childLineItems.Where(n => n.Product.Id == oli.Product.Id);
            }

            if (backOrder > 0) //Create or update Back Orders
            {
                    childLineItems.Add(new OrderLineItem(Guid.NewGuid())
                                           {
                                               Description = oli.Description,
                                               LineItemSequenceNo = _order.LineItems.Count() + 1,
                                               LineItemVatValue = oli.LineItemVatValue,
                                               Qty = item.BackOrder,
                                               Product = oli.Product,
                                               IsNew = true,
                                               LineItemType = OrderLineItemType.BackOrder,
                                               Value = oli.Value
                                           });
            }
            if (lostSale > 0) //Lost Sale : create or update
            {
                childLineItems.Add(new OrderLineItem(Guid.NewGuid())
                                       {
                                           Description = oli.Description,
                                           LineItemSequenceNo = _order.LineItems.Count() + 1,
                                           LineItemVatValue = oli.LineItemVatValue,
                                           Qty = item.LostSale,
                                           Product = oli.Product,
                                           IsNew = true,
                                           LineItemType = OrderLineItemType.LostSale,
                                           Value = oli.Value
                                       });
            }
            CalcTotals();
        }

        public void LoadBackOrderlineItem(Guid productId, string productDesc, decimal unitPrice, decimal vatValue, decimal vatAmount, decimal vat, decimal qty, decimal totalPrice, bool isEditable, Guid? lineItemID, string liProdType)
        {
            using (var container = NestedContainer)
            {
               
                var _inventoryService = Using<IInventoryRepository>(container);
               
                IOtherUtilities _otherUtilities = Using<IOtherUtilities>(container);
                IConfigService _configService = Using<IConfigService>(container);

                ApproveSalesmanOrderItem li = null;
                //Inventory availableInv = _inventoryService.GetByProductId(productId, OutletId);
                Inventory availableInv = _inventoryService.GetByProductIdAndWarehouseId(productId, _configService.Load().CostCentreId);
                if (liProdType == "ReturnableProduct") //#$@$%#^!!
                    if (availableInv != null)
                        if (availableInv.Balance < 0)
                            availableInv.Balance = 0;

                decimal totalProcessedBackOrderQty = 0;
                try
                {
                    totalProcessedBackOrderQty =
                        _order.LineItems.Where(
                            n => n.Product.Id == productId && n.LineItemType == OrderLineItemType.ProcessedBackOrder)
                            .Sum(s => s.Qty);
                }
                catch
                {
                }

                int sequenceNo = 1;
                if (LineItems.Count > 0)
                {
                    sequenceNo = LineItems.Max(n => n.SequenceNo) + 1;
                }
                li = new ApproveSalesmanOrderItem(_otherUtilities)
                         {
                             SequenceNo = sequenceNo,
                             ProductId = productId,
                             Product = productDesc,
                             UnitPrice = unitPrice,
                             LineItemVatValue = vatValue,
                             TotalLineItemVatAmount = vatAmount,
                             Qty = qty - totalProcessedBackOrderQty,
                             //less the processed qty
                             TotalPrice = totalPrice,
                             IsEditable = isEditable,
                             LineItemId = (Guid) lineItemID,
                             ProcessedQty = 0,
                             //item.LineItemId   = oli.Id;
                             AvailableProductInv = availableInv == null ? 0 : availableInv.Balance,
                             CanEdit = true,
                             CanRemove = true,
                             HlnkDeleteContent = GetLocalText("sl.approveBackOrder.lineItemsGrid.rejectBackOrderItem"),
                             ProductType = liProdType
                         };
                if (liProdType == "ReturnableProduct") li.CanEdit = false;
                if (liProdType == "ReturnableProduct") li.CanRemove = false;

                if (LoadForViewing)
                {
                    li.CanEdit = false;
                    li.CanRemove = false;
                }
                LineItems.Add(li);
                //}
                CalcTotals();
            }
        }

        void CalcTotals()
        {
            TotalNet = LineItems.Sum(n => (n.Qty * (n.TotalPrice < 0 ? -n.UnitPrice : n.UnitPrice))) + (childLineItems == null ? 0 : childLineItems.Where(n => n.LineItemType == OrderLineItemType.ProcessedBackOrder).Sum(n => (n.Qty * n.Value)));
            TotalProductDiscount = LineItems.Sum(n => n.LineItemTotalProductDiscount);
            TotalNet += TotalProductDiscount;
            TotalVat = LineItems.Sum(n => n.TotalLineItemVatAmount) + (childLineItems == null? 0 : childLineItems.Where(n => n.LineItemType == OrderLineItemType.ProcessedBackOrder).Sum(n => (n.LineItemVatTotal)));
            TotalGross = (LineItems.Sum(n => n.TotalPrice) + (childLineItems == null? 0 : childLineItems.Where(n => n.LineItemType == OrderLineItemType.ProcessedBackOrder).Sum(n => n.LineItemTotal)));
            TotalGross -= SaleDiscount;
        }

        public void ValidateForApproval(bool includeBackOrder)
        {
            List<OrderLineItemInventoryInfo> groupItem = new List<OrderLineItemInventoryInfo>();
            int invalidCount = 0;
            if (!includeBackOrder)
            {
                foreach (var item in LineItems)//.Where(n => n.LiProductType != "ReturnableProduct")
                {
                    if (LineItems.Where(n => n.ProductId == item.ProductId).Count() > 1)
                    {
                        if (!groupItem.Any(n => n.ProductId == item.ProductId))
                            groupItem.Add(new OrderLineItemInventoryInfo
                                              {
                                                  ProductId = item.ProductId,
                                                  ProductDesc = item.Product,
                                                  AvailableInv = item.AvailableProductInv,
                                                  RequiredInv = LineItems.Where(n => n.ProductId == item.ProductId).Sum(n => n.Qty)
                                              });
                    }
                    OrderLineItemInventoryInfo itemInfo = new OrderLineItemInventoryInfo
                                                              {
                                                                  LineItemSequenceNo = item.SequenceNo,
                                                                  ProductId = item.ProductId,
                                                                  ProductDesc = item.Product,
                                                                  AvailableInv = item.AvailableProductInv,
                                                                  RequiredInv = item.Qty
                                                              };

                    if (groupItem.Any(n => n.ProductId == item.ProductId))
                    {
                        var group = groupItem.First(n => n.ProductId == item.ProductId);
                        itemInfo.AvailableInv = group.AvailableInv;
                        group.AvailableInv -= itemInfo.RequiredInv;
                        if (group.AvailableInv < 0)
                            group.AvailableInv = 0;
                    }

                    Validated = ((item.Qty - item.BackOrder) <= (itemInfo.AvailableInv));
                    if (!Validated)
                    {
                        LineItemsInventoryInfoList.Add(itemInfo);
                        invalidCount += 1;
                    }
                }
            }
            else
            {
                throw new Exception("Unxpected path followed validating inventory.");
                foreach (var item in LineItems)//.Where(n => n.LiProductType != "ReturnableProduct")
                {
                    Validated = item.Qty <= item.AvailableProductInv;
                    if (!Validated)
                    {
                        LineItemsInventoryInfoList.Add(new OrderLineItemInventoryInfo
                                                           {
                                                               LineItemSequenceNo = item.SequenceNo,
                                                               ProductId = item.ProductId,
                                                               ProductDesc = item.Product,
                                                               AvailableInv = item.AvailableProductInv,
                                                               RequiredInv = item.Qty
                                                           });
                        invalidCount += 1;
                    }
                }
            }

            if (invalidCount > 0)
                Validated = false;
            else if (invalidCount == 0)
                Validated = true;
        }

        OrderLineItem CreateNewLineItem(Guid lineItemId, ApproveSalesmanOrderItem item, OrderLineItemType orderLineItemType = OrderLineItemType.PostConfirmation)
        {
            using (var container = NestedContainer)
            {
                IProductRepository _productService = Using<IProductRepository>(container);
               

                lineItemId = lineItemId == Guid.Empty ? Guid.NewGuid() : lineItemId;
                var oli = new OrderLineItem(lineItemId)
                              {
                                  Description = item.Product,
                                  Product = _productService.GetById(item.ProductId),
                                  Qty = item.Qty,
                                  IsNew = true,
                                  LineItemSequenceNo = item.SequenceNo
                              };
                oli.Value = item.UnitPrice;
                oli.LineItemVatValue = item.LineItemVatValue;
                oli.LineItemType = orderLineItemType;
                oli.ProductDiscount = item.ProductDiscount;
                oli.LineItemType = orderLineItemType;
                oli.DiscountType = item.LineItemDiscountType;

                return oli;
            }
        }

        public void RunClearAndSetup()
        {
            childLineItems = new List<OrderLineItem>();
            OrderIdLookup = Guid.Empty;
            Status = "";
            OrderId = "";
            DateRequired = DateTime.Now;
            DateSubmitted = DateTime.MaxValue;
            CreatedByUser = "";
            LineItems.Clear();
            CalcTotals();
            LineItemsRemoved = new ObservableCollection<Guid>();
            ViewDispatched = false;
            LoadForProcessing = false;
            LoadForViewing = false;
            LoadFullGrid = false;
            ProcessingBackOrder = false;
        }

        void Cancel()
        {
            ClearViewModel();
        }

        void ClearViewModel()
        {
            DateRequired = DateTime.Now;
            LineItems.Clear();
            TotalGross = 0;
            TotalNet = 0;
            TotalVat = 0;
            TotalProductDiscount = 0m;
            SaleDiscount = 0m;
            OutletName = "";
            RouteName = "";
            SalesmanUsername = "";
            if (LineItemsRemoved != null)
                LineItemsRemoved.Clear();
            IsRejected = false;
            ViewDispatched = false;
        }

        public void RemoveLineItem(int sequenceNo)
        {
            var litoremove = LineItems.First(n => n.SequenceNo == sequenceNo);
            if (childLineItems != null)
            {//if had been edited and changes saved here.
                try
                {
                    var clis = childLineItems.Where(n => n.Product.Id == litoremove.ProductId);
                    clis.ToList().ForEach(n => childLineItems.Remove(n));
                }
                catch
                {
                }
            }
            LineItems.Remove(litoremove);
            if (litoremove.LineItemId != Guid.Empty && _order.LineItems.Any(n => n.Id == litoremove.LineItemId))//not empty and is in order.LineItems
                LineItemsRemoved.Add(litoremove.LineItemId);
            CalcTotals();
        }

        public void RemoveLineItemByProductId(Guid productId)
        {
            var litoremove = LineItems.First(n => n.ProductId == productId);
            LineItems.Remove(litoremove);
            if (litoremove.LineItemId != Guid.Empty && _order.LineItems.Any(n => n.Id == litoremove.LineItemId))//not empty and is in order.LineItems
                LineItemsRemoved.Add(litoremove.LineItemId);
            CalcTotals();
        }

        void RunProcessBackOrderCommand()
        {
            //Load back order with back order line items only
            LoadBackOrder();
        }

        void RunValidateProcessBackOrder()
        {
            int invalidCnt = 0;
            foreach (var item in LineItems)//.Where(n=>n.LiProductType != "ReturnableProduct")
            {
                Validated = item.BackOrder <= item.AvailableProductInv;
                if (!Validated)
                    invalidCnt += 1;
            }

            if (invalidCnt == 0)
                Validated = true;
            else if (invalidCnt > 0)
                Validated = false;
        }

        void ApproveBackOrder()
        {
            using (var container = NestedContainer)
            {
               
                IApproveSalesmanOrderWFManager _approveOrderWFManager = Using<IApproveSalesmanOrderWFManager>(container);

                List<Guid> rolis = new List<Guid>(); //removed order line items
                //any lost sales from the rejected items
                //create a lost sale line item for all removed line items
                if (LineItemsRemoved != null)
                {
                    if (LineItemsRemoved.Count > 0)
                    {
                        foreach (var lineItemId in LineItemsRemoved)
                        {
                            ApproveSalesmanOrderItem removedLineItem =
                                LineItems.FirstOrDefault(n => n.LineItemId == lineItemId);
                            OrderLineItem originalLi = _order.LineItems.FirstOrDefault(n => n.Id == lineItemId);
                            OrderLineItem lostSale = CreateNewLineItem(Guid.NewGuid(), removedLineItem,
                                                                       OrderLineItemType.LostSale);

                            lostSale.Description = originalLi.Description;
                            _order.CreateLineItem(lostSale);

                            //reduce original LI with lost sale
                            var existingOrderLi = _order.LineItems.First(n => n.Id == lineItemId);
                            _order.ChangeLineItemQty(existingOrderLi.Id, (existingOrderLi.Qty - lostSale.Qty));

                            //reduce existing back order with lost sale
                            var existingBackOrder = _order.LineItems.First(
                                //n => n.Product.Id == lineItemId && n.LineItemType == OrderLineItemType.BackOrder);
                                n =>
                                n.Description == existingOrderLi.Description &&
                                n.LineItemType == OrderLineItemType.BackOrder);
                            if (existingBackOrder != null)
                                _order.ChangeLineItemQty(existingBackOrder.Id,
                                                         (existingBackOrder.Qty - removedLineItem.Qty));

                            rolis.Add(lineItemId);
                        }
                    }
                }

                //create new line item type processed back order
                foreach (var lineItem in LineItems)
                {
                    var orderLineItem = _order.LineItems.FirstOrDefault(n => n.Id == lineItem.LineItemId);
                    if (orderLineItem == null) //is new
                    {
                        throw new Exception("To do");
                    }
                    //also subtract your back order in childLineItems
                    decimal backOrderQty = 0;
                    //if (childLineItems.Count > 0 && childLineItems.Any(n => n.Product.Id == lineItem.ProductId && n.LineItemType == OrderLineItemType.BackOrder))
                    if (childLineItems.Count > 0 &&
                        childLineItems.Any(
                            n =>
                            n.Description == lineItem.LineItemId.ToString() &&
                            n.LineItemType == OrderLineItemType.BackOrder))
                    {
                        backOrderQty =
                            childLineItems.Where(
                                //n => n.Product.Id == lineItem.ProductId && n.LineItemType == OrderLineItemType.BackOrder).
                                n =>
                                n.Description == orderLineItem.Description &&
                                n.LineItemType == OrderLineItemType.BackOrder).
                                Sum(n => n.Qty);
                    }

                    var existingProcessedLi =
                        _order.LineItems.FirstOrDefault(
                            //n => n.Product.Id == lineItem.ProductId && n.LineItemType == OrderLineItemType.ProcessedBackOrder);
                            n =>
                            n.Description == orderLineItem.Description &&
                            n.LineItemType == OrderLineItemType.ProcessedBackOrder);
                    //whose desc field matches this lineItem's desc, and is ProcessedBackOrder

                    if (existingProcessedLi != null && existingProcessedLi.Qty == lineItem.Qty)
                    {
                        continue;
                    }
                    else if (existingProcessedLi != null && existingProcessedLi.Qty != lineItem.Qty)
                    {
                        existingProcessedLi.Qty += lineItem.Qty;
                    }
                    else if (existingProcessedLi == null)
                    {
                        OrderLineItem oli = CreateNewLineItem(Guid.NewGuid(), lineItem,
                                                              OrderLineItemType.ProcessedBackOrder);
                        oli.Qty -= backOrderQty;
                        oli.LineItemType = OrderLineItemType.ProcessedBackOrder;
                        if (lineItem.LineItemId == Guid.Empty)
                            throw new Exception("Unexpected occurrence. LineItem Guid id is empty.");
                        oli.Description = orderLineItem.Description;
                        _order.CreateLineItem(oli);
                    }
                }

                if (childLineItems != null)
                    if (childLineItems.Count() > 0)
                    {
                        foreach (var item in childLineItems)
                        {
                            //create or update back orders or lost sales in the order
                            var existingLi =
                                _order.LineItems.FirstOrDefault(
                                    n => n.Description == item.Description && n.LineItemType == item.LineItemType);
                            if (existingLi != null && existingLi.Qty == item.Qty)
                            {
                                continue;
                            }
                            else if (existingLi != null && existingLi.Qty != item.Qty)
                            {
                                if (existingLi.LineItemType != OrderLineItemType.BackOrder)
                                {
                                    if (item.Qty > existingLi.Qty)
                                        existingLi.Qty = item.Qty;
                                }
                                if (existingLi.LineItemType == OrderLineItemType.LostSale)
                                {
                                    existingLi.Qty += item.Qty;
                                }
                            }
                            else if (existingLi == null)
                            {
                                _order.CreateLineItem(item);
                            }

                            if (item.LineItemType == OrderLineItemType.LostSale)
                            {
                                //reduce original LI with the lost sale
                                var confirmedOrderLi =
                                    _order.LineItems.First(
                                        n =>
                                        n.Description == item.Description &&
                                        n.LineItemType == OrderLineItemType.PostConfirmation);

                                if (confirmedOrderLi != null)
                                {
                                    _order.ChangeLineItemQty(confirmedOrderLi.Id, (confirmedOrderLi.Qty - item.Qty));
                                }

                                //reduce back order with the lost sale
                                var existingBackOrder =
                                    _order.LineItems.First(
                                        n =>
                                        n.Description == item.Description &&
                                        n.LineItemType == OrderLineItemType.BackOrder);

                                if (existingBackOrder != null)
                                {
                                    _order.ChangeLineItemQty(existingBackOrder.Id, (existingBackOrder.Qty - item.Qty));
                                }
                            }
                        }
                    }

                _approveOrderWFManager.remvdLineItemIds = rolis;
                LineItemsRemoved.Clear();

                _approveOrderWFManager.ProcessBackOrder(_order);

                _approveOrderWFManager.remvdLineItemIds.Clear();

                IsApproved = true;
                ProcessingBackOrder = false;
            }
        }

        void CancelBackOrder()
        {
            using (var container = NestedContainer)
            {
               
                IApproveSalesmanOrderWFManager _approveOrderWFManager = Using<IApproveSalesmanOrderWFManager>(container);
                IOrderRepository _orderService = Using<IOrderRepository>(container);
                _approveOrderWFManager.remvdLineItemIds = new List<Guid>();
                _order = _orderService.GetById(_order.Id) as Order; //refresh and discard changes on line items
                //send back order to lost sale
                foreach (var lineItem in LineItems)
                {
                    //find the original order line item
                    var oli =
                        _order.LineItems.First(
                            n =>
                            n.Product.Id == lineItem.ProductId && n.LineItemType == OrderLineItemType.PostConfirmation);

                    decimal lostSaleQty = 0;
                    var existingBackOrder =
                        _order.LineItems.FirstOrDefault(
                            n => n.Description == oli.Description && n.LineItemType == OrderLineItemType.BackOrder);
                    if (existingBackOrder != null)
                    {
                        existingBackOrder.Qty = 0;
                    }

                    var existingLostSale =
                        _order.LineItems.FirstOrDefault(
                            n => n.Id == lineItem.LineItemId && n.LineItemType == OrderLineItemType.LostSale);
                    if (existingLostSale != null)
                    {
                        existingLostSale.Qty += lineItem.Qty;
                        lostSaleQty = existingLostSale.Qty;
                    }
                    else
                    {
                        //create a new lost sale li
                        OrderLineItem lostSale = CreateNewLineItem(Guid.NewGuid(), lineItem);
                        lostSale.LineItemType = OrderLineItemType.LostSale;
                        _order.CreateLineItem(lostSale);
                        lostSaleQty = lostSale.Qty;
                    }

                    //update order
                    if (oli != null)
                    {
                        if (lostSaleQty > 0)
                            _order.ChangeLineItemQty(oli.Id, (oli.Qty - lostSaleQty));
                    }

                    //_approveOrderWFManager.remvdLineItemIds.Add(lineItem.LineItemId);
                }
                _approveOrderWFManager.CancelBackOrder(_order);

                ProcessingBackOrder = false;
            }
        }

        void RunViewInvoice()
        {
            SendNavigationRequestMessage(new Uri("views/invoicedocument/invoicedocument.xaml?orderid=" + OrderIdLookup,
                                                 UriKind.Relative));
        }

        public void LoadInvoiceAndReceipts()
        {
            using (var container = NestedContainer)
            {
              
                IInvoiceRepository _invoiceService = Using<IInvoiceRepository>(container);
                var _receiptService = Using<IReceiptRepository>(container);

                var invoice = _invoiceService.GetInvoiceByOrderId(OrderIdLookup);
                if (invoice != null)
                {
                    InvoiceReceipts.Clear();
                    var rec = new Receipt(Guid.Empty)
                                  {DocumentReference = GetLocalText("sl.approveOrder.selectReceipt")};
                    if(!InvoiceReceipts.Contains(rec))
                        InvoiceReceipts.Add(rec);
                    SelectedReceipt = rec;
                    _receiptService.GetByInvoiceId(invoice.Id).ForEach(n => InvoiceReceipts.Add(n));
                }
            }
        }

        void RunViewReceipt()
        {
            if (SelectedReceipt==null || SelectedReceipt.Id == Guid.Empty)
                MessageBox.Show("Select a receipt to view.", "Distributr: View Receipt", MessageBoxButton.OK);
            else
                SendNavigationRequestMessage(
                    new Uri(
                        "views/receiptdocument/receiptdocument.xaml?orderid=" + OrderIdLookup + "&receiptid=" + SelectedReceipt.Id,
                        UriKind.Relative));
        }

        #endregion

        #region Helper Classes
        public class ApproveSalesmanOrderItem : OrderLineItemBase
        {
            private IOtherUtilities _otherUtilities;

            public ApproveSalesmanOrderItem(IOtherUtilities otherUtilities)
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

            public decimal NetAmount { get { return UnitPrice * Qty; } }

            public const string LineItemVatValuePropertyName = "LineItemVatValue";
            private decimal _lineItemVatValue = 0;
            public decimal LineItemVatValue
            {
                get
                {
                    return _lineItemVatValue;
                }

                set
                {
                    if (_lineItemVatValue == value)
                        return;
                    var oldValue = _lineItemVatValue;
                    _lineItemVatValue = value;
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

            //public const string VatPropertyName = "Vat";
            //private decimal _vat = 0m;
            //public decimal Vat
            //{
            //    get
            //    {
            //        return _vat;
            //    }

            //    set
            //    {
            //        if (_vat == value)
            //        {
            //            return;
            //        }

            //        var oldValue = _vat;
            //        _vat = value;
            //        RaisePropertyChanged(VatPropertyName);
            //    }
            //}

            public const string ProcessedQtyPropertyName = "ProcessedQty";
            private decimal _processedQty = 0;
            public decimal ProcessedQty
            {
                get
                {
                    //_processedQty = Qty;
                    //if (BackOrder > 0)
                    //    _processedQty = Qty - BackOrder;
                    return _processedQty;
                }

                set
                {
                    if (_processedQty == value)
                    {
                        return;
                    }

                    var oldValue = _processedQty;
                    _processedQty = value;
                    RaisePropertyChanged(ProcessedQtyPropertyName);
                }
            }

            public const string AvailableProductInvPropertyName = "AvailableProductInv";
            private decimal _availableProductInv = 0;
            public decimal AvailableProductInv
            {
                get
                {
                    return _availableProductInv;
                }

                set
                {
                    if (_availableProductInv == value)
                    {
                        return;
                    }

                    var oldValue = _availableProductInv;
                    _availableProductInv = value;
                    RaisePropertyChanged(AvailableProductInvPropertyName);
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

            public const string BackOrderPropertyName = "BackOrder";
            private decimal _backOrder = 0;
            public decimal BackOrder
            {
                get
                {
                    return _backOrder;
                }

                set
                {
                    if (_backOrder == value)
                    {
                        return;
                    }

                    var oldValue = _backOrder;
                    _backOrder = value;
                    RaisePropertyChanged(BackOrderPropertyName);
                }
            }

            public const string LostSalePropertyName = "LostSale";
            private decimal _lostSale = 0;
            public decimal LostSale
            {
                get
                {
                    return _lostSale;
                }

                set
                {
                    if (_lostSale == value)
                    {
                        return;
                    }

                    var oldValue = _lostSale;
                    _lostSale = value;
                    RaisePropertyChanged(LostSalePropertyName);
                }
            }

            public const string ApprovedPropertyName = "Approved";
            private int _approved = 0;
            public int Approved
            {
                get
                {
                    return _approved;
                }

                set
                {
                    if (_approved == value)
                    {
                        return;
                    }

                    var oldValue = _approved;
                    _approved = value;
                    RaisePropertyChanged(ApprovedPropertyName);
                }
            }

            public const string CanEditPropertyName = "CanEdit";
            private bool _canEdit = false;
            public bool CanEdit
            {
                get
                {
                    return _canEdit;
                }

                set
                {
                    if (_canEdit == value)
                    {
                        return;
                    }

                    var oldValue = _canEdit;
                    _canEdit = value;
                    RaisePropertyChanged(CanEditPropertyName);
                }
            }

            public const string CanRemovePropertyName = "CanRemove";
            private bool _canRemove = false;
            public bool CanRemove
            {
                get
                {
                    return _canRemove;
                }

                set
                {
                    if (_canRemove == value)
                    {
                        return;
                    }

                    var oldValue = _canRemove;
                    _canRemove = value;
                    RaisePropertyChanged(CanRemovePropertyName);
                }
            }

            public const string LiProductTypePropertyName = "LiProductType";
            private string _liProdType = "";
            public string LiProductType
            {
                get
                {
                    return _liProdType;
                }

                set
                {
                    if (_liProdType == value)
                    {
                        return;
                    }

                    var oldValue = _liProdType;
                    _liProdType = value;
                    RaisePropertyChanged(LiProductTypePropertyName);
                }
            }

            public string Product_Type
            {
                get
                {
                    string productType = ProductType.Remove(ProductType.LastIndexOf("Product"));
                    return _otherUtilities.BreakStringByUpperCB(productType +
                                                         ((int)LineItemDiscountType > 0
                                                              ? " (" + LineItemDiscountType.ToString() + ")"
                                                              : ""));
                }
            }

            //public SalesmanOrders.EditSalesmanOrderItem.enumLineItemSaleType typeOfSale { get; set; }

            public const string HlnkDeleteContentPropertyName = "HlnkDeleteContent";
            private string _hlnkDeleteContent = "Delete";
            public string HlnkDeleteContent
            {
                get
                {
                    return _hlnkDeleteContent;
                }

                set
                {
                    if (_hlnkDeleteContent == value)
                    {
                        return;
                    }

                    var oldValue = _hlnkDeleteContent;
                    _hlnkDeleteContent = value;
                    RaisePropertyChanged(HlnkDeleteContentPropertyName);
                }
            }


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

        public class OrderLineItemInventoryInfo
        {
            public int LineItemSequenceNo { get; set; }
            public Guid ProductId { get; set; }
            public string ProductDesc { get; set; }
            public decimal AvailableInv { get; set; }
            public decimal RequiredInv { get; set; }
        }
        #endregion
    }
}