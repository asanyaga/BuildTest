using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Repository.Payment;
using Distributr.WPF.Lib.Services.Service.Payment;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.WorkFlow.AuditLogs;
using Distributr.WPF.Lib.Services.WorkFlow.Discount;
using Distributr.WPF.Lib.Services.WorkFlow.GetDocumentReferences;
using Distributr.WPF.Lib.Services.WorkFlow.Invoices;
using Distributr.WPF.Lib.Services.WorkFlow.Orders;
using Distributr.WPF.Lib.Services.WorkFlow.POS;
using Distributr.WPF.Lib.UI.Hierarchy;
using Distributr.WPF.Lib.ViewModels.Transactional.CN;
using Distributr.WPF.Lib.ViewModels.Transactional.Purchasing;
using Distributr.WPF.Lib.ViewModels.Utils.Payment;
using Distributr.WPF.Lib.WorkFlow.Orders;
using GalaSoft.MvvmLight.Command;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Workflow.InventoryWorkflow;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Request;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;
using PaymentGateway.WSApi.Lib.MessageResults;

namespace Distributr.WPF.Lib.ViewModels.Transactional.POS
{
    public class EditPOSOutletSaleViewModel : DistributrViewModelBase
    {
        public bool Editing;
        public bool FireSalesmanChangedCmd = true;
        public bool FireRouteChangedCmd = true;
        public bool FireOutletChangeCmd = true;
        bool _paymentNotificationCompleted = true;
        private List<ClientRequestResponseBase> _clientRequestResponses;

        public EditPOSOutletSaleViewModel()
        {
            InvoiceReceipts = new ObservableCollection<Receipt>();
            LineItems = new ObservableCollection<EditPOSSaleLineItem>();
            
            CancelCommand = new RelayCommand(Cancel);
            SalesManChangedCommand = new RelayCommand(SalesManChanged);
            RouteChangedCommand = new RelayCommand(RouteChanged);
            LoadOrderCommand = new RelayCommand(DoLoad);
            ConfirmCommand = new RelayCommand(ConfirmOrder);
            ViewInvoice = new RelayCommand(DoViewInvoice);
            ViewReceipt = new RelayCommand(DoViewReceipt);
            CloseOrderCommand = new RelayCommand(CloseOrder);
            SaveOrderCommand = new RelayCommand(SaveOrderToContinue);
            ConfirmPaymentCommand = new RelayCommand(ConfirmOrderPayment);//Used by Receiving payment module
            DistributorRoutes = new ObservableCollection<Route>();
            RouteOutlets = new ObservableCollection<Outlet>();
            Salesmen = new ObservableCollection<User>();
            InvoiceReceipts = new ObservableCollection<Receipt>();

            PaymentInfoList = new ObservableCollection<PaymentInfo>();
            _clientRequestResponses = new List<ClientRequestResponseBase>();
            _paymentNotifs = new List<PaymentNotificationResponse>();
        }

        #region Properties
        #region Commands n Collections
        public ObservableCollection<PaymentInfo> PaymentInfoList { get; set; }
        public ObservableCollection<Route> DistributorRoutes { get; set; }
        public ObservableCollection<Outlet> RouteOutlets { get; set; }
        public ObservableCollection<Receipt> InvoiceReceipts { get; set; }
        public ObservableCollection<User> Salesmen { get; set; }
        public ObservableCollection<EditPOSSaleLineItem> LineItems { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand RouteChangedCommand { get; set; }
        public RelayCommand SalesManChangedCommand { get; set; }
        public RelayCommand LoadOrderCommand { get; set; }
        public RelayCommand ConfirmCommand { get; set; }
        public RelayCommand CloseOrderCommand { get; set; }
        public RelayCommand SaveOrderCommand { get; set; }
        public RelayCommand ConfirmPaymentCommand { get; set; }//Used by Receiving payment module
        public RelayCommand ViewInvoice { get; set; }
        public RelayCommand ViewReceipt { get; set; }

        #endregion

        #region Properties

        #region Order Properties

        public const string OrderDocumentPropertyName = "OrderDocument";
        private Order _orderDocument = null;
        public Order OrderDocument
        {
            get
            {
                return _orderDocument;
            }

            set
            {
                //if (_orderDocument == value)
                //{
                //    return;
                //}

                var oldValue = _orderDocument;
                _orderDocument = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(OrderDocumentPropertyName);
            }
        }

        public const string InvoiceDocumentPropertyName = "InvoiceDocument";
        private Invoice _invoiceDocument = null;
        public Invoice InvoiceDocument
        {
            get
            {
                return _invoiceDocument;
            }

            set
            {
                var oldValue = _invoiceDocument;
                _invoiceDocument = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(InvoiceDocumentPropertyName);
            }
        }

        public const string ReceiptsPropertyName = "Receipts";
        private List<Receipt> _receipts = null;
        public List<Receipt> Receipts
        {
            get
            {
                return _receipts;
            }

            set
            {
                if (_receipts == value)
                {
                    return;
                }

                var oldValue = _receipts;
                _receipts = value;
                RaisePropertyChanged(ReceiptsPropertyName);
            }
        }

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
                SelectedRouteName = _selectedRoute == null ? "" : _selectedRoute.Name;

                // Update bindings, no broadcast
                RaisePropertyChanged(SelectedRoutePropertyName);
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

        public const string SelectedSalesmanUserNamePropertyName = "SelectedSalesmanUserName";
        private string _selectedSalesmanUserName = "";
        public string SelectedSalesmanUserName
        {
            get
            {
                return _selectedSalesmanUserName;
            }

            set
            {
                if (_selectedSalesmanUserName == value)
                {
                    return;
                }

                var oldValue = _selectedSalesmanUserName;
                _selectedSalesmanUserName = value;
                RaisePropertyChanged(SelectedSalesmanUserNamePropertyName);
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
                SelectedOutletName = _selectedOutlet == null ? "" : _selectedOutlet.Name;

                // Update bindings, no broadcast
                RaisePropertyChanged(SelectedOutletPropertyName);
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
                if (value < DateTime.Now)
                    _dateRequired = DateTime.Now;
                else
                    _dateRequired = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(DateRequiredPropertyName);
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

        public const string TotalProductDiscountPropertyName = "TotalProductDiscount";
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
                RaisePropertyChanged(TotalProductDiscountPropertyName);
            }
        }

        public const string SetSelectedIdPropertyName = "SetSelectedId";
        private Guid _setSelectedId = Guid.Empty;
        public Guid SetSelectedId
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
                RaisePropertyChanged(SetSelectedIdPropertyName);
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
                if (value)
                    CanSaveToContinue = false;
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
                //if (value)

                    RaisePropertyChanged(LoadForEditingPropertyName);
            }
        }
         
        public const string CanSaveToContinuePropertyName = "CanSaveToContinue";
        private bool _canSaveToContinue = false;
        public bool CanSaveToContinue
        {
            get
            {
                return _canSaveToContinue;
            }

            set
            {
                if (_canSaveToContinue == value)
                {
                    return;
                }

                RaisePropertyChanging(CanSaveToContinuePropertyName);
                _canSaveToContinue = value;
                RaisePropertyChanged(CanSaveToContinuePropertyName);
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

        public const string SaleValuePropertyName = "SaleValue";
        private decimal _saleValue;
        public decimal SaleValue
        {
            get
            {
                return _saleValue;
            }

            set
            {
                if (_saleValue == value)
                {
                    return;
                }

                var oldValue = _saleValue;
                _saleValue = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(SaleValuePropertyName);
            }
        }

        public const string ReturnableValuePropertyName = "ReturnableValue";
        private decimal _returnableValue;
        public decimal ReturnableValue
        {
            get
            {
                return _returnableValue;
            }

            set
            {
                if (_returnableValue == value)
                {
                    return;
                }

                var oldValue = _returnableValue;
                _returnableValue = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(ReturnableValuePropertyName);
            }
        }
        

        #endregion

        //payment info ...
        #region payment
        
        public const string PaymentModeUsedPropertyName = "PaymentModeUsed";
        private PaymentMode _paymentModeUsed = PaymentMode.Cash;
        public PaymentMode PaymentModeUsed
        {
            get
            {
                return _paymentModeUsed;
            }

            set
            {
                if (_paymentModeUsed == value)
                {
                    return;
                }

                var oldValue = _paymentModeUsed;
                _paymentModeUsed = value;
                RaisePropertyChanged(PaymentModeUsedPropertyName);
            }
        }

        public const string AmountPaidPropertyName = "AmountPaid";
        private decimal _amountPaid = 0m;
        public decimal AmountPaid
        {
            get
            {
                return _amountPaid;
            }

            set
            {
                if (_amountPaid == value)
                {
                    return;
                }

                var oldValue = _amountPaid;
                _amountPaid = value;
                RaisePropertyChanged(AmountPaidPropertyName);
            }
        }

        public const string CashAmountPropertyName = "CashAmount";
        private decimal _cashAmount = 0m;
        public decimal CashAmount
        {
            get
            {
                return _cashAmount;
            }

            set
            {
                if (_cashAmount == value)
                {
                    return;
                }

                var oldValue = _cashAmount;
                _cashAmount = value;
                //CalcAmountPaid();
                RaisePropertyChanged(CashAmountPropertyName);
            }
        }

        public const string MMoneyAmountPropertyName = "MMoneyAmount";
        private decimal _mMoneyAmount = 0m;
        public decimal MMoneyAmount
        {
            get
            {
                return _mMoneyAmount;
            }

            set
            {
                if (_mMoneyAmount == value)
                {
                    return;
                }

                var oldValue = _mMoneyAmount;
                _mMoneyAmount = value;
                //CalcAmountPaid();
                RaisePropertyChanged(MMoneyAmountPropertyName);
            }
        }

        public const string ChequeAmountPropertyName = "ChequeAmount";
        private decimal _chequeAmount = 0m;
        public decimal ChequeAmount
        {
            get
            {
                return _chequeAmount;
            }

            set
            {
                if (_chequeAmount == value)
                {
                    return;
                }

                var oldValue = _chequeAmount;
                _chequeAmount = value;
                //CalcAmountPaid();
                RaisePropertyChanged(ChequeAmountPropertyName);
            }
        }

        public const string CreditAmountPropertyName = "CreditAmount";
        private decimal _creditAmount = 0m;
        public decimal CreditAmount
        {
            get
            {
                return _creditAmount;
            }

            set
            {
                if (_creditAmount == value)
                {
                    return;
                }

                var oldValue = _creditAmount;
                _creditAmount = value;
                //CalcAmountPaid();
                RaisePropertyChanged(CreditAmountPropertyName);
            }
        }
        
        public const string MMoneyRefNoPropertyName = "MMoneyRefNo";
        private string _mMoneyRefNo = "";
        public string MMoneyRefNo
        {
            get
            {
                return _mMoneyRefNo;
            }

            set
            {
                if (_mMoneyRefNo == value)
                {
                    return;
                }

                var oldValue = _mMoneyRefNo;
                _mMoneyRefNo = value;
                RaisePropertyChanged(MMoneyRefNoPropertyName);
            }
        }

        public const string MMoneyOptionPropertyName = "MMoneyOption";
        private string _mMoneyOption = "";
        public string MMoneyOption
        {
            get
            {
                return _mMoneyOption;
            }

            set
            {
                if (_mMoneyOption == value)
                {
                    return;
                }

                var oldValue = _mMoneyOption;
                _mMoneyOption = value;
                RaisePropertyChanged(MMoneyOptionPropertyName);
            }
        }

        public const string ChequeNoPropertyName = "ChequeNo";
        private string _chequeNo = "";
        public string ChequeNo
        {
            get
            {
                return _chequeNo;
            }

            set
            {
                if (_chequeNo == value)
                {
                    return;
                }

                var oldValue = _chequeNo;
                _chequeNo = value;
                RaisePropertyChanged(ChequeNoPropertyName);
            }
        }

        public const string BankBranchPropertyName = "bankBranch";
        private BankBranch _bankBranch = null;
        public BankBranch bankBranch
        {
            get
            {
                return _bankBranch;
            }

            set
            {
                if (_bankBranch == value)
                {
                    return;
                }

                var oldValue = _bankBranch;
                _bankBranch = value;
                RaisePropertyChanged(BankBranchPropertyName);
            }
        }
         
        public const string MMoneyIsApprovedPropertyName = "MMoneyIsApproved";
        private bool _mMoneyIsApproved = false;
        public bool MMoneyIsApproved
        {
            get
            {
                return _mMoneyIsApproved;
            }

            set
            {
                if (_mMoneyIsApproved == value)
                {
                    return;
                }

                var oldValue = _mMoneyIsApproved;
                _mMoneyIsApproved = value;
                RaisePropertyChanged(MMoneyIsApprovedPropertyName);
            }
        }
         
        public const string PaymentTransactionRefIdPropertyName = "PaymentTransactionRefId";
        private Guid _paymentTransactionRefId = Guid.Empty;
        public Guid PaymentTransactionRefId
        {
            get
            {
                return _paymentTransactionRefId;
            }

            set
            {
                if (_paymentTransactionRefId == value)
                {
                    return;
                }

                var oldValue = _paymentTransactionRefId;
                _paymentTransactionRefId = value;
                RaisePropertyChanged(PaymentTransactionRefIdPropertyName);
            }
        }

        #endregion

        //invoice
        #region Invoice

        public const string InvoiceIdLookUpPropertyName = "InvoiceIdLookUp";
        private Guid _invoiceIdLookup = Guid.Empty;
        public Guid InvoiceIdLookUp
        {
            get
            {
                return _invoiceIdLookup;
            }

            set
            {
                if (_invoiceIdLookup == value)
                {
                    return;
                }

                var oldValue = _invoiceIdLookup;
                _invoiceIdLookup = value;
                RaisePropertyChanged(InvoiceIdLookUpPropertyName);
            }
        }

        public const string InvoiceIdPropertyName = "InvoiceId";
        private string _invoiceId = "";
        public string InvoiceId
        {
            get
            {
                return _invoiceId;
            }

            set
            {
                if (_invoiceId == value)
                {
                    return;
                }

                var oldValue = _invoiceId;
                _invoiceId = value;
                RaisePropertyChanged(InvoiceIdPropertyName);
            }
        }

        public const string MyNewOrderIdPropertyName = "MyNewOrderId";
        private Guid _myNewOrderId = Guid.Empty;
        public Guid MyNewOrderId
        {
            get
            {
                return _myNewOrderId;
            }

            set
            {
                if (_myNewOrderId == value)
                {
                    return;
                }

                var oldValue = _myNewOrderId;
                _myNewOrderId = value;
                RaisePropertyChanged(MyNewOrderIdPropertyName);
            }
        }

        public const string HasReceiptPropertyName = "HasReceipt";
        private bool _hasreceipt = false;
        public bool HasReceipt
        {
            get
            {
                return _hasreceipt;
            }

            set
            {
                if (_hasreceipt == value)
                {
                    return;
                }

                var oldValue = _hasreceipt;
                _hasreceipt = value;
                RaisePropertyChanged(HasReceiptPropertyName);
            }
        }

        public const string ChangePropertyName = "Change";
        private decimal _change = 0m;
        public decimal Change
        {
            get
            {
                return _change;
            }

            set
            {
                if (_change == value)
                {
                    return;
                }

                var oldValue = _change;
                _change = value;
                RaisePropertyChanged(ChangePropertyName);
            }
        }

        public const string TheInvoicePropertyName = "TheInvoice";
        private Invoice _theInvoice = null;
        public Invoice TheInvoice
        {
            get
            {
                return _theInvoice;
            }

            set
            {
                if (_theInvoice == value)
                {
                    return;
                }

                var oldValue = _theInvoice;
                _theInvoice = value;
                RaisePropertyChanged(TheInvoicePropertyName);
            }
        }

        public const string SelectedReceiptPropertyName = "SelectedReceipt";
        private Receipt _selectedReceipt = null;
        public Receipt SelectedReceipt
        {
            get
            {
                return _selectedReceipt;
            }

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

        #endregion
        #endregion

        #endregion

        #region Load Order

        public void LoadPayments()
        {
            using (StructureMap.IContainer container = NestedContainer)
            {
                Receipts = new List<Receipt>();
               
                DateTime start = DateTime.Now;
                InvoiceDocument = Using<IInvoiceRepository>(container).GetInvoiceByOrderId(OrderIdLookup);
                if (InvoiceDocument == null) return;
                InvoiceIdLookUp = InvoiceDocument.Id;
                start = DateTime.Now;

                var receipts = Using<IReceiptRepository>(container).GetByInvoiceId(InvoiceDocument.Id);

                receipts.ForEach(Receipts.Add); //cache

                InvoiceReceipts.Clear();
                var rec = new Receipt(Guid.Empty) {DocumentReference = "--Select A Receipt to View--"};
                SelectedReceipt = rec;
                InvoiceReceipts.Add(rec);
                receipts.Where(n => n.Total > 0).ToList().ForEach(n => InvoiceReceipts.Add(n));
                start = DateTime.Now;
                var paymentInfo = new List<PaymentInfo>();
                receipts.ToList()
                        .ForEach(n => n.LineItems
                                       .ForEach(x =>
                                                    {
                                                        decimal unconfirmedAmnt = 0m;
                                                        bool isConfirmed = Using<ITransactionalViewmodelRefactoring>(container).LineItemIsConfirmed(n, x.Id,out unconfirmedAmnt);
                                                        if (isConfirmed &&
                                                            x.LineItemType == OrderLineItemType.DuringConfirmation)
                                                            //cn: a fully confirmed payment
                                                            return;
                                                        var info = new PaymentInfo
                                                                       {
                                                                           Id = x.Id,
                                                                           Amount =
                                                                               x.LineItemType ==
                                                                               OrderLineItemType.DuringConfirmation
                                                                                   ? unconfirmedAmnt
                                                                                   : x.Value,
                                                                           PaymentModeUsed = (PaymentMode) x.PaymentType,
                                                                           IsNew = false,
                                                                           IsConfirmed = isConfirmed,
                                                                           PaymentRefId = x.PaymentRefId,
                                                                           MMoneyPaymentType = x.MMoneyPaymentType,
                                                                           PaymentTypeDisplayer =
                                                                               x.PaymentType == PaymentMode.MMoney
                                                                                   ? x.MMoneyPaymentType
                                                                                   : x.PaymentType.ToString()
                                                                       };
                                                        paymentInfo.Add(info);
                                                    }
                                          ));

                paymentInfo.ForEach(PaymentInfoList.Add);

                RecalcAmountPaid();
            }
        }

        private void LoadSalesmen()
        {
            using (StructureMap.IContainer container = NestedContainer)
            {
                Salesmen.Clear();
                var salesman = new User(Guid.Empty)
                                   {
                                       Username = GetLocalText("sl.pos.selectsalesman")
                                       /*"--Please Select Salesman --"*/
                                   };
                Salesmen.Add(salesman);
                SelectedSalesman = salesman;
                Using<IUserRepository>(container).GetAll().Where(n => n.UserType == UserType.DistributorSalesman)
                            .OrderBy(n => n.Username).ToList()
                            .ForEach(n => Salesmen.Add(n));
                //bw.RunWorkerAsync();
                //SalesManChanged();   
            }
        }

        private void SalesManChanged()
        {
            using (StructureMap.IContainer container = NestedContainer)
            {
                if (FireSalesmanChangedCmd)
                {
                    DistributorRoutes.Clear();
                    var route = new Route(Guid.Empty)
                                    {
                                        Name = GetLocalText("sl.pos.selectroute")
                                        /*"--Please Select a Route--"*/
                                    };
                    DistributorRoutes.Add(route);
                    SelectedRoute = route;
                    if (SelectedSalesman == null)
                        return;
                    List<SalesmanRoute> sroutes = Using<ISalesmanRouteRepository>(container).GetAll().ToList();
                    sroutes =
                        sroutes.Where(n => n.Route != null && n.DistributorSalesmanRef.Id == SelectedSalesman.CostCentre)
                               .OrderBy(n => n.Route.Name).ToList();
                    sroutes.ForEach(n => DistributorRoutes.Add(n.Route));
                    //_routeService.GetAll().ForEach(n => DistributorRoutes.Add(n));
                    //RouteChanged();
                }
            }
        }

        private void RouteChanged()
        {
            using (StructureMap.IContainer container = NestedContainer)
            {
                if (FireRouteChangedCmd)
                {
                    RouteOutlets.Clear();
                    var outlet = new Outlet(Guid.Empty)
                                     {
                                         Name = GetLocalText("sl.pos.selectoutlet"),
                                         /*"--Please Select An Outlet"--"*/
                                         CostCentreCode = ""
                                     };
                    RouteOutlets.Add(outlet);
                    SelectedOutlet = outlet;
                    if (SelectedRoute != null)
                    {
                        var outlets =
                            Using<IOutletRepository>(container).GetAll().OfType<Outlet>()
                                .Where(n => n._Status == EntityStatus.Active);
                        var filteredOutlets =
                            outlets.Where(n => n.Route != null).Where(n => n.Route.Id == SelectedRoute.Id)
                                   .OrderBy(n => n.Name).ToList();
                        filteredOutlets.ForEach(n => RouteOutlets.Add(n));
                    }
                }
            }
        }

        private bool loadingFromDb = false;

        private void DoLoad()
        {
            _productPackagingSummaryService.ClearBuffer();
            //check if order has orderId
            if (OrderIdLookup == Guid.Empty)
            {
                PageTitle = "Create New Sale";
                Editing = false;
                //create new order
                OrderId = //NewOrderId();
                    Status = DocumentStatus.New.ToString();
                using (StructureMap.IContainer container = NestedContainer)
                {
                    User salesmanUserName =
                        Using<IUserRepository>(container)
                            .GetById(Using<IConfigService>(container).ViewModelParameters.CurrentUserId);
                    CreatedByUser = salesmanUserName.Username;
                }
                IsEditable = true;
                IsEditing = false;
                LoadSalesmen();
                CanSaveToContinue = true;
            }
            else //load order for edit
            {
                DateTime start = DateTime.Now;
                loadingFromDb = true;
                IsEditing = true;
                PageTitle = "Edit Sale";
                Editing = true;
                OrderDocument = GetEntityById(typeof (Order), OrderIdLookup) as Order;
                LoadPayments();
                AmountPaid = InvoiceReceipts.Sum(s => s.Total);
                IsEditable = false;
                if (OrderDocument.Status == DocumentStatus.New)
                    IsEditable = true;
                Status = OrderDocument.Status.ToString();
                OrderId = OrderDocument.DocumentReference;
                SaleDiscount = OrderDocument.SaleDiscount;
                DateRequired = OrderDocument.DateRequired;
                DateSubmitted = OrderDocument.DocumentDateIssued;
                CreatedByUser = OrderDocument.DocumentIssuerUser == null
                                    ? string.Empty
                                    : OrderDocument.DocumentIssuerUser.Username;
                LineItems.Clear();

                //Load Salesmen
                foreach (var item in OrderDocument.LineItems)
                {
                    AddLineItem(item.Product.Id, item.Product.Description, item.Value, item.LineItemVatValue,
                                item.LineItemVatTotal, item.Qty, item.LineItemTotal, false, IsEditable, item.Id,
                                Guid.Empty, item.LineItemType, item.DiscountType, item.ProductDiscount,
                                item.Product.GetType().ToString().Split('.').Last());
                    if (item.LineItemType != OrderLineItemType.Discount &&
                        (item.Product is SaleProduct || item.Product is ConsolidatedProduct))
                    {
                        _productPackagingSummaryService.AddProduct(item.Product.Id, item.Qty, false, false, true);
                    }
                }
                LoadSalesmen();

                FireSalesmanChangedCmd = false;
                FireRouteChangedCmd = false;
                FireOutletChangeCmd = false;

                List<Outlet> outlets;
                Outlet myOutlet;
                using (StructureMap.IContainer container = NestedContainer)
                {
                    SelectedSalesman = Salesmen.FirstOrDefault(n => n.Id == OrderDocument.DocumentIssuerUser.Id);
                    if (SelectedSalesman != null)
                        SelectedSalesmanUserName = CreatedByUser = SelectedSalesman.Username;
                    //this order outlet
                    outlets = Using<IOutletRepository>(container).GetAll().OfType<Outlet>().ToList();
                    myOutlet = outlets.FirstOrDefault(n => n.Id == OrderDocument.IssuedOnBehalfOf.Id);

                    //Load routes
                    DistributorRoutes.Clear();
                    Using<IRouteRepository>(container).GetAll(true).ToList().ForEach(n => DistributorRoutes.Add(n));
                    SelectedRoute = DistributorRoutes.FirstOrDefault(n => n.Id == myOutlet.Route.Id);
                }
                RouteOutlets.Clear();
                var fileteredOutlets =
                    outlets.Where(n => n.Route != null).Where(n => n.Route.Id == SelectedRoute.Id).ToList();
                fileteredOutlets.ForEach(n => RouteOutlets.Add(n));
                SelectedOutlet = RouteOutlets.FirstOrDefault(n => n.Id == myOutlet.Id);
                SetSelectedId = SelectedOutlet.Id;
                loadingFromDb = false;
                CalcSaleValue();

                if (SelectedRoute._Status == EntityStatus.Inactive || myOutlet._Status == EntityStatus.Inactive)
                {
                    if (IsEditable)
                    {
                        MessageBox.Show("Sale cannot be edited because of deactivated route and outlet.");
                        IsEditable = false;
                    }
                }

                if (OrderDocument.Status == DocumentStatus.New)
                    OrderDocument.EnableAddCommands();
            }
        }

        #endregion

        #region Creating & Confirm Order Operations

        public void GenerateSaleId()
        {
            NewOrderId();
        }

        private void NewOrderId()
        {
            using (StructureMap.IContainer container = NestedContainer)
            {
                try
                {
                //    OrderId = Using<IGetDocumentReference>(container)
                //        .GetDocReference("Sale", SelectedSalesman.Username, SelectedOutlet.CostCentreCode);
                }
                catch
                {
                    MessageBox.Show("Error getting new sale id");
                }
            }
        }

        public Dictionary<Guid, decimal> GetReturnableIn()
        {
            using (StructureMap.IContainer container = NestedContainer)
            {
                Dictionary<Guid, decimal> s = new Dictionary<Guid, decimal>();
                foreach (var item in LineItems.Where(n => !n.IsReceivedReturnable))
                {
                    Product p = Using<IProductRepository>(container).GetById(item.ProductId);
                    if (p is ReturnableProduct && item.UnitPrice > 0)
                    {
                        decimal receivedQty =
                            LineItems.Where(n => n.IsReceivedReturnable && n.ProductId == item.ProductId)
                                     .Sum(n => n.Qty);
                        if ((item.Qty - receivedQty) > 0)
                            s.Add(p.Id, item.Qty - receivedQty);
                    }
                }
                return s;
            }
        }

        public bool IsReturnableRequired()
        {
            using (StructureMap.IContainer container = NestedContainer)
            {
                bool IsReturnable = false;
                foreach (var item in LineItems)
                {
                    Product product = Using<IProductRepository>(container).GetById(item.ProductId);
                    if (product is ReturnableProduct)
                    {
                        decimal amt = LineItems.Where(n => n.ProductId == item.ProductId).Sum(s => s.TotalPrice);
                        if (amt > 0)
                            return true;
                    }
                }

                return IsReturnable;
            }
        }

        public bool IsPaymentRequired()
       {
           bool IsPaymentRequired = false;
           if(PaymentInfoList.Count==0)
           {
               return true;
           }
           return IsPaymentRequired;
       }

        public decimal GetChangeRequired()
        {
            var change = AmountPaid - TotalGross;
            return change;
        }

        public void SaveOrderToContinue()
        {
            CommitOrderChanges();
        }

        private void CommitOrderChanges(int bwswitch = 2)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                AddLineItemsToDocuments();

                if (bwswitch == 3)
                {
                    OrderDocument.Confirm();

                    InvoiceDocument.Confirm();

                    Using<IPOSSalesWFManager>(cont).SubmitChanges(OrderDocument);
                    Using<IConfirmInvoiceWorkFlowManager>(cont).SubmitChanges(InvoiceDocument);

                    Using<IPOSSalesWFManager>(cont).CloseOrder(OrderDocument);
                    //Using<IConfirmDispatchNoteWFManager>(cont).Save(GetDispatchNote(OrderDocument));
                }

                if (AmountPaid > 0)
                    SubmitPaymentInfo(OrderDocument, PaymentInfoList, InvoiceDocument.Id,
                                      InvoiceDocument.DocumentReference);

                switch (bwswitch)
                {
                    case 1:
                        {
                            LoadPayments();
                        }
                        break;
                    case 2:
                        {
                            Using<IOrderRepository>(cont).Save(OrderDocument);
                        }
                        break;
                }
            }
        }

        public void ConfirmOrder()
        {
            if(InvoiceDocument == null)
            {
                RaiseInvoice();
            }
            InvoiceDocument.SaleDiscount = SaleDiscount;
            CommitOrderChanges(3);
        }

        DispatchNote GetDispatchNote(Order order)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                var dispatchNote = Using<IDispatchNoteFactory>(c).Create(order.DocumentIssuerCostCentre,
                                                                     order.DocumentIssuerCostCentreApplicationId,
                                                                     order.DocumentRecipientCostCentre,
                                                                     order.DocumentIssuerUser,
                                                                     order.IssuedOnBehalfOf,
                                                                     DispatchNoteType.Delivery,
                                                                     GetDocumentReference("DN", order.DocumentReference),
                                                                     order.Id,
                                                                     order.Id);

                int seq = 1;
                foreach(var item in order.LineItems.Where(n => n.Value > 0))
                {
                    var returned = order.LineItems.FirstOrDefault(q => q.Product.Id == item.Product.Id && q.Value < 0);
                    decimal netQty = item.Qty;
                    if (returned != null)
                    {
                        netQty = item.Qty - returned.Qty;
                    }
                    if (netQty > 0)
                    {
                        var li = Using<IDispatchNoteFactory>(c)
                            .CreateLineItem(item.Product.Id,
                                            netQty,
                                            item.Value,
                                            item.Id.ToString(),
                                            seq,
                                            item.LineItemVatValue,
                                            item.ProductDiscount,
                                            item.DiscountType);
                        dispatchNote.AddLineItem(li);
                        seq += 1;
                    }
                }

                return dispatchNote;
            }
        }

        private void CloseOrder()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                if (OrderIdLookup == Guid.Empty) return;
                Using<IOrderRepository>(cont).ChangeStatus(OrderDocument.Id, DocumentStatus.Closed);
                Using<ICloseOrderWFManager>(cont).SubmitChanges(OrderDocument);
            }
        }

        public void CreateNewOrder()
        {
            if (OrderIdLookup == Guid.Empty)
            {
                OrderDocument = CreateNewSalesOrder();
            }
        }

        public void RaiseInvoice()
        {
            if (InvoiceDocument == null)
                InvoiceDocument = CreateNewInvoice(OrderDocument);
        }

        void CalcTotals()
        {
            TotalNet = LineItems.Sum(n => n.Qty * (n.TotalPrice < 0 ? -n.UnitPrice : n.UnitPrice));
            TotalVat = LineItems.Sum(n => n.VatAmount);
            TotalProductDiscount = LineItems.Sum(n => n.LineItemTotalProductDiscount);
            TotalNet += TotalProductDiscount;
            TotalGross = LineItems.Sum(n => n.TotalPrice);
            //AddSaleDiscount();
            TotalGross -= SaleDiscount;
            ReturnableValue = LineItems.Where(n => n.IsReceivedReturnable).Sum(n => n.TotalPrice);
            SaleValue = LineItems.Where(n => n.UnitPrice > 0).Sum(n => n.TotalPrice);
        }

        public void CalcSaleValue()
        {
            SaleValue = LineItems.Where(n => n.UnitPrice > 0).Sum(n => n.TotalPrice);
            ReturnableValue = LineItems.Where(n => n.IsReceivedReturnable).Sum(n => n.TotalPrice);
        }

        public bool ValidateOrderAmounts()
        {
            bool retVal = true;
            if (TotalGross < 0)
                retVal = false;
            if (TotalNet < 0)
                retVal = false;
            return retVal;
        }

        private Order CreateNewSalesOrder()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                Order newOrder = null;
                Config config = GetConfigParams();
                CostCentre docIssuerCc = Using<ICostCentreRepository>(cont).GetById(config.CostCentreId);
                var appId = config.CostCentreApplicationId;
                CostCentre recipientCc = SelectedOutlet; //also as issued on behalf of
                ////OrderIdLookup = Guid.NewGuid();
                ////newOrder = new Order(OrderIdLookup)
                ////               {
                ////                   DateRequired = DateRequired,
                ////                   DocumentDateIssued = DateTime.Now > DateRequired ? DateRequired : DateTime.Now,
                ////                   DocumentIssuerCostCentre = docIssuerCc,
                ////                   DocumentIssuerCostCentreApplicationId = appId,
                ////                   DocumentIssuerUser = SelectedSalesman,
                ////                   //issuerUser,
                ////                   DocumentRecipientCostCentre = recipientCc,
                ////                   DocumentReference = OrderId,
                ////                   DocumentType = DocumentType.Order,
                ////                   IssuedOnBehalfOf = recipientCc,
                ////                   OrderType = OrderType.DistributorPOS,
                ////                   Status = DocumentStatus.New,
                ////                   DocumentParentId = OrderIdLookup
                ////               };
                newOrder = Using<IOrderFactory>(cont)
                    .Create(docIssuerCc,
                            appId,
                            recipientCc,
                            SelectedSalesman,
                            recipientCc,
                            OrderType.DistributorPOS,
                            OrderId,
                            Guid.Empty,
                            DateRequired);
                OrderIdLookup = newOrder.Id;

                return newOrder;
            }
        }

        
         #endregion

        #region LineItem Operations

        public void AddLineItem(Guid productId, string productDesc, decimal unitPrice, decimal vatValue,
                                decimal vatAmount, decimal qty,
                                decimal totalPrice, bool sellInBulk, bool isEditable, Guid lineItemId,
                                Guid parentProductId, OrderLineItemType orderLineItemType, DiscountType discounType,
                                decimal productDiscount, string productType = "")
        {
            using(StructureMap.IContainer cont = NestedContainer)
            {
                if (loadingFromDb)
                {
                    ReceiveReturnable = unitPrice < 0;
                }
                //are we receiving returnables ...
                if (!loadingFromDb)
                {
                    if (ReceiveReturnable)
                    {
                        vatAmount = -vatAmount;
                        totalPrice = -totalPrice;
                        vatValue = -vatValue;
                    }
                }

                int sequenceNo = 1;
                if (LineItems.Count > 0)
                {
                    sequenceNo = LineItems.Max(n => n.SequenceNo) + 1;
                }

                bool isReturnableProduct = false;
                if (!LoadForViewing)
                {
                    isReturnableProduct = Using<IProductRepository>(cont).GetById(productId).GetType() ==
                                          typeof (ReturnableProduct);
                }

                //Check the line item exists by product
                EditPOSSaleLineItem li = null;
                if (ReceiveReturnable)
                {
                    if (LineItems.Any(n => n.ProductId == productId && n.IsReceivedReturnable == true))
                    {
                        li = LineItems.FirstOrDefault(n => n.ProductId == productId && n.IsReceivedReturnable == true);
                    }
                }
                else
                {
                    if (
                        LineItems.Any(
                            n =>
                            n.ProductId == productId && !n.IsReceivedReturnable &&
                            n.OrderLineItemType == orderLineItemType))
                    {
                        li =
                            LineItems.FirstOrDefault(
                                n =>
                                n.ProductId == productId && !n.IsReceivedReturnable &&
                                n.OrderLineItemType == orderLineItemType);
                    }
                }

                //if found update it
                if (li != null)
                {
                    li.Qty += qty;
                    li.VatAmount += vatAmount;
                    li.LineItemVatValue += vatValue;
                    li.TotalPrice += totalPrice;
                    li.ProductDiscount = productDiscount;
                    li.LineItemTotalProductDiscount += (productDiscount*qty);
                }
                else
                {
                    li = new EditPOSSaleLineItem(Using<IOtherUtilities>(cont))
                             {
                                 SequenceNo = sequenceNo,
                                 ProductId = productId,
                                 Product = productDesc,
                                 UnitPrice = unitPrice < 0 ? -unitPrice : unitPrice,
                                 //display +ve value for received returnables
                                 LineItemVatValue = vatValue,
                                 VatAmount = vatAmount,
                                 Qty = qty,
                                 TotalPrice = totalPrice,
                                 LineItemId = (Guid) lineItemId,
                                 IsReceivedReturnable = ReceiveReturnable,
                                 CanEditLineItem = true,
                                 CanRemoveLineItem = true,
                                 ProductType = productType,
                                 OrderLineItemType = orderLineItemType,
                                 LineItemDiscountType = discounType,
                                 ProductDiscount = productDiscount,
                                 LineItemTotalProductDiscount = (productDiscount*qty),
                             };
                    if (li.OrderLineItemType == OrderLineItemType.Discount)
                    {
                        li.CanEditLineItem = false;
                        li.CanRemoveLineItem = false;
                    }

                    if (!ReceiveReturnable)
                    {
                        if (isReturnableProduct && li.TotalPrice >= 0) li.CanEditLineItem = false;
                        if (isReturnableProduct && li.TotalPrice >= 0) li.CanRemoveLineItem = false;

                        if (li.LineItemDiscountType == DiscountType.FreeOfChargeDiscount)
                        {
                            li.CanEditLineItem = true;
                            li.CanRemoveLineItem = true;
                        }
                    }

                    if (Status != "New")
                    {
                        li.CanEditLineItem = false;
                        li.CanRemoveLineItem = false;
                    }
                    LineItems.Add(li);
                }
                CalcTotals();
            }
        }

        public void UpdateLineItem(Guid productId, int sequenceNo, decimal qty, decimal vatValue, decimal vatAmount, decimal totalPrice,
            decimal unitPrice, decimal totalProdDisc)
        {
            EditPOSSaleLineItem item = LineItems.First(n => n.SequenceNo == sequenceNo);
            item.Qty = qty;
            item.LineItemVatValue = vatValue;
            item.VatAmount = vatAmount;
            item.TotalPrice = totalPrice;
            item.LineItemTotalProductDiscount = totalProdDisc;
            CalcTotals();
            //AddOrderLineItem(item);
        }

        public void UpdateReturnableLineItem(PackagingSummary item)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                LineItemPricingInfo pi = GetLineItemPricing(item, SelectedOutlet.Id);
                decimal totalVatAmount = pi.TotalVatAmount;
                decimal totalPrice = pi.TotalPrice;
                decimal totalProductDiscount = pi.TotalProductDiscount;

                EditPOSSaleLineItem li = LineItems.First(n => n.ProductId == item.Product.Id && n.TotalPrice < 0);
                li.Qty = item.Quantity;
                li.VatAmount = -totalVatAmount;
                li.TotalPrice = -totalPrice;
                li.ProductDiscount = -totalProductDiscount;
                CalcTotals();
                //AddOrderLineItem(li);
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

        public void ReceiveReturnableLineItem(List<ProductAddSummary> productsummariies, bool IsNew)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                var receivables = GetReturnableIn();
                foreach (ProductAddSummary product in productsummariies)
                {
                    if (!receivables.Any(n => n.Key == product.ProductId))
                        continue;
                    var receivableItem = receivables.FirstOrDefault(n => n.Key == product.ProductId);

                    var receivableQty = receivableItem.Value;

                    if (receivableItem.Value >= product.Quantity)
                        receivableQty = product.Quantity;

                    if (receivableItem.Value < product.Quantity)
                        receivableQty = receivableItem.Value;

                    if (receivableQty == 0)
                        return;

                    PackagingSummary ps = new PackagingSummary
                                              {
                                                  Product = Using<IProductRepository>(cont).GetById(product.ProductId),
                                                  Quantity = receivableQty,
                                              };
                    if (IsNew)
                        AddLineItem(ps);
                    else
                        UpdateReturnableLineItem(ps);
                }

                RefreshList();
            }
        }

        private void RefreshList()
        {
            ReceiveReturnable = false;
            // 1. Clear LineItems and CalcTotals
            // 2. Add Line Items
            // 3. Process discounts
            // 4. Clear Saved Order LineItems

            var receivedReturnables = new List<EditPOSSaleLineItem>(); 
            LineItems.Where(n => n.IsReceivedReturnable).ToList().ForEach(receivedReturnables.Add);

            LineItems.Clear();
            ClearOrderAndInvoiceLineItems();

            List<PackagingSummary> summarypro = _productPackagingSummaryService.GetProductSummary();
            foreach (PackagingSummary item in summarypro)
            {
                //check if auto added items are in inventory
                decimal invBalance = 0m;
                decimal lineItemsQty = LineItems.Where(n => n.ProductId == item.Product.Id).Sum(n => n.Qty);
                decimal balanceAfterLineItems = 0m;

                    Guid costCentreId = Guid.Empty;
                using (StructureMap.IContainer cont = NestedContainer)
                {
                    costCentreId = Using<IConfigService>(cont).Load().CostCentreId;
                }

                bool inStock = _productPackagingSummaryService.IsProductInStock(
                                                            costCentreId,
                                                            item.Product.Id,
                                                            lineItemsQty,
                                                            item.Quantity,
                                                            out invBalance);

                if (!inStock)
                {
                    MessageBox.Show("Product " + item.Product.Description + " is out of stock.\nThe product will not be added.\nThe required quantity is " + item.Quantity,
                                    "Distributr: Sales Module", MessageBoxButton.OK);
                }
                else
                {
                    balanceAfterLineItems = invBalance - lineItemsQty;
                    if (balanceAfterLineItems < item.Quantity)
                    {
                        MessageBox.Show(
                            "The available inventory of "+balanceAfterLineItems+" units cannot cover the " + item.Quantity + " units for product " + item.Product.Description+".\n"+
                            "" + balanceAfterLineItems + " units will be added.",
                            "Distributr: Sales Module", MessageBoxButton.OK);
                        item.Quantity = balanceAfterLineItems;
                    }
                    if (item.Quantity > 0)
                        AddLineItem(item);
                }
            }

            foreach (var item in receivedReturnables)
            {
                LineItems.Add(item);
                //ordering
                List<OrderLineItemBase> lineItems = LineItems.Select(n => n as OrderLineItemBase).ToList();
                LineItems.Clear();
                _productPackagingSummaryService.OrderLineItems(lineItems).Select(n => n as EditPOSSaleLineItem).ToList().ForEach(LineItems.Add);
            }

            ProcessDiscounts();
            CalcTotals();
        }

        private void ClearOrderAndInvoiceLineItems()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {

                foreach (OrderLineItem item in OrderDocument.LineItems)
                {
                    Using<IOrderRepository>(cont).DeleteLineItem(item);
                }
                OrderDocument.ClearLineItems();
                if (InvoiceDocument != null)
                {
                    foreach (InvoiceLineItem item in InvoiceDocument.LineItems)
                    {
                        Using<IInvoiceRepository>(cont).RemoveLineItem(item);
                    }

                    InvoiceDocument.ClearLineItems();
                }
            }
        }

        private void AddLineItem(PackagingSummary item)
        {
            LineItemPricingInfo pricingInfo;
            DiscountType discountType = 0;
            OrderLineItemType orderLineItemType = 0;
            decimal unitPrice;
            decimal vatValue;
            decimal totalNet;
            decimal totalVatAmount;
            decimal totalPrice;
            decimal productDiscount;
            decimal totalProductDiscount;
                pricingInfo = GetLineItemPricing(item, SelectedOutlet.Id);
                unitPrice = pricingInfo.UnitPrice;
                vatValue = pricingInfo.VatValue;
                totalNet = pricingInfo.TotalNetPrice;
                totalVatAmount = pricingInfo.TotalVatAmount;
                totalPrice = pricingInfo.TotalPrice;
                productDiscount = pricingInfo.ProductDiscount;
                totalProductDiscount = pricingInfo.TotalProductDiscount;

            using (StructureMap.IContainer cont = NestedContainer)
            {
                if (Using<IDiscountProWorkflow>(cont).IsProductFreeOfCharge(item.Product.Id))
                {
                    unitPrice = 0m;
                    vatValue = 0m;
                    totalNet = 0m;
                    totalVatAmount = 0m;
                    totalPrice = 0m;
                    totalProductDiscount = 0m;
                    discountType = DiscountType.FreeOfChargeDiscount;
                    orderLineItemType = OrderLineItemType.Discount;
                }
            }


            if (ReceiveReturnable)
                pricingInfo.UnitPrice = -pricingInfo.UnitPrice;

            AddLineItem(item.Product.Id, item.Product.Description, unitPrice, vatValue, totalVatAmount,
                        item.Quantity, totalPrice, false, IsEditable, Guid.Empty, item.Product.Id, orderLineItemType,
                        discountType, productDiscount, item.Product.GetType().ToString().Split('.').Last());

            List<OrderLineItemBase> lineItems = LineItems.Select(n => n as OrderLineItemBase).ToList();
            LineItems.Clear();
            _productPackagingSummaryService.OrderLineItems(lineItems)
                                           .Select(n => n as EditPOSSaleLineItem)
                                           .ToList()
                                           .ForEach(LineItems.Add);
        }

        public void RemoveLineItem(int sequenceNo)
        {
            var litoremove = LineItems.First(n => n.SequenceNo == sequenceNo);
            string msg = "";
            msg += string.Format("\n\t{0} of {1} will be deleted", litoremove.Qty, litoremove.Product);
            MessageBoxResult isConfirmed = MessageBox.Show(
                "Are sure you want to delete the following product(s)" + msg, "Delete POS Order Line item",
                MessageBoxButton.OKCancel);
            if (isConfirmed == MessageBoxResult.OK)
            {
                LineItems.Remove(litoremove);
                RefreshList();
            }
        }

        public void RemoveLineItem(Guid productId, LineItemType lit, bool isReceivedReturnable)
        {
            PackagingSummary delProduct;
            List<PackagingSummary> itemsToDelete;
            MessageBoxResult isConfirmed;
            delProduct = _productPackagingSummaryService.GetProductSummary().FirstOrDefault(
                p => p.Product.Id == productId && p.IsEditable);
            itemsToDelete = _productPackagingSummaryService.GetProductSummaryByProduct(
                productId, delProduct.Quantity);

            string msg = "";
            foreach (PackagingSummary delitem in itemsToDelete)
            {
                msg += string.Format("\n\t{0} of {1} will be deleted", delitem.Quantity, delitem.Product.Description);
            }

            isConfirmed = MessageBox.Show(
                "Are sure you want to delete the following product(s)" + msg, "Delete POS Order Line item",
                MessageBoxButton.OKCancel);

            if (isConfirmed == MessageBoxResult.OK)
            {
                _productPackagingSummaryService.RemoveProduct(productId);
                RefreshList();
            }
        }

        void AddLineItemsToDocuments()
        {
            ClearOrderAndInvoiceLineItems();
            foreach (EditPOSSaleLineItem item in LineItems)
            {
                OrderLineItem oli = CreateNewLineItem(item, item.IsReceivedReturnable);
                OrderDocument.AddLineItem(oli);
                if (InvoiceDocument != null)
                {
                    InvoiceLineItem ili = CreateNewInvoiceLineItem(item);
                    InvoiceDocument.AddLineItem(ili);
                }
            }
        }

        private OrderLineItem CreateNewLineItem(EditPOSSaleLineItem item, bool isReceivedReturnable)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                OrderLineItem li = null;
                var oli = Using<IOrderFactory>(cont)
                    .CreateLineItem(item.ProductId,
                                    item.Qty,
                                    (item.IsReceivedReturnable ? -item.UnitPrice : item.UnitPrice),
                                    item.Product,
                                    1,
                                    item.LineItemVatValue,
                                    (item.OrderLineItemType == OrderLineItemType.Discount
                                         ? OrderLineItemType.Discount
                                         : OrderLineItemType.DuringConfirmation),
                                    item.ProductDiscount, item.LineItemDiscountType);

                item.LineItemId = oli.Id;
                return oli;
            }
        }

        #endregion
        
        #region Process Discounts

        void ProcessDiscounts()
        {
            //reset discount items to 0
            SaleDiscount = 0;
            ////List<EditPOSSaleLineItem> discountLineItems = null;
            ////discountLineItems = new List<EditPOSSaleLineItem>();
            ////LineItems.Where(n => n.OrderLineItemType == OrderLineItemType.Discount).ToList().ForEach(
            ////    discountLineItems.Add);
            ////discountLineItems.ForEach(n => LineItems.Remove(n));

            //1.process discount for each line item not a dicsount
            //2.add/update/delete discount Lineitems in the order.

            List<EditPOSSaleLineItem> lineItems =
                LineItems.Where(
                    n => n.OrderLineItemType != OrderLineItemType.Discount && n.ProductType != "ReturnableProduct").
                    ToList();
            foreach (EditPOSSaleLineItem lineItem in lineItems)
            {
                AddCertainProductQuantityCertainProductDiscount(lineItem);
            }
            //3. add free of charge (Certain Sale Value Certain Product)
            AddCertainSaleValueCertainProductDiscount();

            ProcessDiscountMixedPackReturnables();
            //4 add sale discount
            AddSaleDiscount();
        }

        private void AddCertainSaleValueCertainProductDiscount()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                CalcTotals();
                ProductAsDiscount productAsDiscount = Using<IDiscountProWorkflow>(cont).GetFOCCertainValue(TotalGross);
                if (productAsDiscount != null)
                    DisplayProductAsDiscountAndAddToDiscountLineItems(productAsDiscount);
            }
        }

        private void AddCertainProductQuantityCertainProductDiscount(EditPOSSaleLineItem lineItem)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                List<ProductAsDiscount> productAsDiscounts = Using<IDiscountProWorkflow>(cont).GetFOCCertainProduct(
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
            using (StructureMap.IContainer cont = NestedContainer)
            {
                SaleDiscount = 0;
                if (SelectedOutlet == null || SelectedOutlet.Id == Guid.Empty)
                {
                    SaleDiscount = 0m;
                }
                else
                {
                    CalcTotals();
                    SaleDiscount = Using<IDiscountProWorkflow>(cont).GetSalevalue(TotalGross, SelectedOutlet.Id);
                    OrderDocument.SaleDiscount = SaleDiscount;
                    if (InvoiceDocument != null) InvoiceDocument.SaleDiscount = SaleDiscount;
                }
            }
        }

        private void DisplayProductAsDiscountAndAddToDiscountLineItems(ProductAsDiscount productAsDiscount)
        {
            Product product = GetEntityById(typeof(Product),productAsDiscount.ProductId) as Product;

            AddOrUpdateLineItemFromDiscount(productAsDiscount, product);
        }

        List<ReturnableProduct> discProdReturnables = null;

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
                decimal unitVatRate = 0m;
                decimal totalNetPrice = 0m;
                decimal vatValue = 0m;
                decimal totalVatAmount = 0m;
                decimal totalPrice = 0m;
                decimal productDiscount = 0m;
                decimal totalProductDiscount = 0m;
                decimal qty = productAsDiscount.Quantity;
                OrderLineItemType orderLineItemType = OrderLineItemType.Discount;
                DiscountType discountType = productAsDiscount.DiscountType;

                if (prod.Id != product.Id)
                {
                    if (((ReturnableProduct) prod).Capacity > 1) //returnable bulk container
                        qty = (int) (productAsDiscount.Quantity/((ReturnableProduct) prod).Capacity);
                    else //sale product returnable
                        qty = productAsDiscount.Quantity;
                    orderLineItemType = 0;
                    discountType = 0;
                }

                if (prod.Id != product.Id)
                {
                    LineItemPricingInfo pi = GetLineItemPricing(
                        new PackagingSummary {Product = prod, Quantity = qty},
                        SelectedOutlet.Id);

                    unitPrice = pi.UnitPrice;
                    unitVatRate = pi.VatValue;
                    totalNetPrice = pi.TotalNetPrice;
                    vatValue = unitPrice*unitVatRate;
                    totalVatAmount = pi.TotalVatAmount;
                    totalPrice = pi.TotalPrice;
                    productDiscount = pi.ProductDiscount;
                    totalProductDiscount = pi.TotalProductDiscount;
                }

                decimal invBalance = 0m;
                decimal lineItemsQty = LineItems.Where(n => n.ProductId == prod.Id).Sum(n => n.Qty);
                decimal balanceAfterLineItems = 0m;

                Guid ccId = GetConfigParams().CostCentreId;
                
                bool inStock = _productPackagingSummaryService.IsProductInStock(
                    ccId,
                    prod.Id,
                    lineItemsQty,
                    qty,
                    out invBalance);

                if (!inStock)
                {
                    MessageBox.Show(
                        "Discount product " + prod.Description +
                        " is out of stock.\nThe product will not be added.\nThe required discount quantity is " +
                        qty,
                        "Distributr: Sales Module", MessageBoxButton.OK);
                }
                else
                {
                    balanceAfterLineItems = invBalance - lineItemsQty;
                    if (balanceAfterLineItems < qty)
                    {
                        MessageBox.Show(
                            "The available inventory of " + balanceAfterLineItems + " units cannot cover the " + qty +
                            " units awarded as discount for product " + prod.Description + ".\n" +
                            "Please give " + balanceAfterLineItems + " units.",
                            "Distributr: Sales Module", MessageBoxButton.OK);
                        qty = balanceAfterLineItems;
                    }
                    if (qty > 0)
                        AddLineItem(prod.Id, prod.Description, unitPrice, vatValue, totalVatAmount, qty, totalPrice,
                                    false, false,
                                    Guid.Empty, product.Id, orderLineItemType, discountType, productDiscount,
                                    prod.GetType().ToString().Split('.').Last());
                    //ordering
                    List<OrderLineItemBase> lineItems = LineItems.Select(n => n as OrderLineItemBase).ToList();
                    LineItems.Clear();
                    _productPackagingSummaryService.OrderLineItems(lineItems)
                                                   .Select(n => n as EditPOSSaleLineItem)
                                                   .ToList()
                                                   .ForEach(LineItems.Add);
                }
            }
        }

        private void ProcessDiscountMixedPackReturnables()
        {
            List<PackagingSummary> mixdPackReturns =
                    _productPackagingSummaryService.GetMixedPackContainers(
                        LineItems.Where(n => !n.IsReceivedReturnable && n.ProductType == "ReturnableProduct")
                                 .Select(n => new PackagingSummary
                                                  {
                                                      Product = GetEntityById(typeof(Product), n.ProductId) as Product,
                                                      Quantity = n.Qty,
                                                  }).ToList());

                foreach (PackagingSummary ps in mixdPackReturns)
                {
                    LineItemPricingInfo pi = GetLineItemPricing(ps, SelectedOutlet.Id);
                    decimal unitPrice = pi.UnitPrice;
                    decimal vatValue = pi.VatValue;
                    decimal totalVatAmount = pi.TotalVatAmount;
                    decimal totalPrice = pi.TotalPrice;
                    decimal prodDisc = pi.ProductDiscount;
                    decimal totalProdDisc = pi.TotalProductDiscount;
                    EditPOSSaleLineItem existing = LineItems.FirstOrDefault(n => n.ProductId == ps.Product.Id);

                    //check if auto added items are in inventory
                    decimal invBalance = 0m;
                    decimal lineItemsQty = LineItems.Where(n => n.ProductId == ps.Product.Id).Sum(n => n.Qty);
                    decimal balanceAfterLineItems = 0m;

                    bool inStock = _productPackagingSummaryService.IsProductInStock(
                        GetConfigParams().CostCentreId,
                        ps.Product.Id,
                        lineItemsQty,
                        ps.Quantity,
                        out invBalance);

                    if (!inStock)
                    {
                        MessageBox.Show(
                            "Product " + ps.Product.Description +
                            " is out of stock.\nThe product will not be added.\nThe required quantity is " + ps.Quantity,
                            "Distributr: Sales Module", MessageBoxButton.OK);
                    }
                    else
                    {
                        balanceAfterLineItems = invBalance - lineItemsQty;
                        if (balanceAfterLineItems < ps.Quantity)
                        {
                            MessageBox.Show(
                                "The available inventory of " + balanceAfterLineItems + " units cannot cover the " +
                                ps.Quantity + " units for product " + ps.Product.Description + ".\n" +
                                "" + balanceAfterLineItems + " units will be added.",
                                "Distributr: Sales Module", MessageBoxButton.OK);
                            ps.Quantity = balanceAfterLineItems;
                        }
                        if (ps.Quantity <= 0)
                            continue;
                        if (existing == null)
                        {
                            AddLineItem(ps.Product.Id, ps.Product.Description, unitPrice, vatValue, totalVatAmount,
                                        ps.Quantity,
                                        totalPrice, false, false,
                                        Guid.Empty, ps.Product.Id, 0, 0, prodDisc,
                                        "ReturnableProduct");
                            //ordering
                            List<OrderLineItemBase> lineItems = LineItems.Select(n => n as OrderLineItemBase).ToList();
                            LineItems.Clear();
                            _productPackagingSummaryService.OrderLineItems(lineItems)
                                                           .Select(n => n as EditPOSSaleLineItem)
                                                           .ToList()
                                                           .ForEach(LineItems.Add);
                            continue;
                        }
                        if (existing.Qty < ps.Quantity)
                        {
                            UpdateLineItem(ps.Product.Id, existing.SequenceNo, ps.Quantity, vatValue, totalVatAmount,
                                           totalPrice,
                                           unitPrice, totalProdDisc);
                            continue;
                        }
                    }
                }

                _productPackagingSummaryService.ClearMixedPackReturnables();
        }

        #endregion

        #region Invoice Document

        private string NewInvoiceId()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                try
                {
                    return Using<IGetDocumentReference>(cont).GetDocReference("Inv", OrderId);
                }
                catch
                {
                    MessageBox.Show("Error getting new invoice id");
                    return "";
                }
            }
        }

        Invoice CreateNewInvoice(Order orderDoc)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                Invoice invoice = Using<IInvoiceFactory>(cont).Create(orderDoc.DocumentIssuerCostCentre,
                                                                  orderDoc.DocumentIssuerCostCentreApplicationId,
                                                                  orderDoc.DocumentRecipientCostCentre,
                                                                  SelectedSalesman, NewInvoiceId(),
                                                                  orderDoc.DocumentParentId, OrderIdLookup);
                InvoiceIdLookUp = invoice.Id;
                    
                return invoice;
            }
            
        }

        private InvoiceLineItem CreateNewInvoiceLineItem(EditPOSSaleLineItem item)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                InvoiceLineItem invli = null;
                Guid id;
                if (invli == null)
                    id = Guid.NewGuid();
                else
                {
                    var li = Using<IInvoiceRepository>(cont).GetLineItemById(invli.Id);
                    id = li == null ? Guid.NewGuid() : li.Id;
                }

                var ili = Using<IInvoiceFactory>(cont).CreateLineItem(
                    item.ProductId,
                    item.Qty,
                    item.IsReceivedReturnable ? -item.UnitPrice : item.UnitPrice,
                    item.Product,
                    1,
                    item.LineItemVatValue,
                    item.ProductDiscount,
                    item.LineItemDiscountType
                    );
                    
                if (item.OrderLineItemType == OrderLineItemType.Discount)
                    ili.LineItemType = OrderLineItemType.Discount;
                return ili;
            }
        }

        public void LoadInvoiceAndReceipts()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                //TheInvoice = Using<IInvoiceRepository>(cont).GetAll().OfType<Invoice>().FirstOrDefault(i => i.OrderId == OrderIdLookup);
                TheInvoice = Using<IInvoiceRepository>(cont).GetInvoiceByOrderId(OrderIdLookup);
                if (TheInvoice != null)
                {
                    InvoiceReceipts.Clear();
                    var rec = new Receipt(Guid.Empty)
                                  {
                                      DocumentReference = GetLocalText("sl.pos.selectreceipt")
                                      /*"--Select Receipt To View--"*/
                                  };
                    InvoiceReceipts.Add(rec);
                    SelectedReceipt = rec;
                    Using<IReceiptRepository>(cont).GetByInvoiceId(TheInvoice.Id).ForEach(n => InvoiceReceipts.Add(n));
                }
            }
        }

        void DoViewInvoice()
        {
            ConfirmNavigatingAway = false;
            SendNavigationRequestMessage(new Uri("/views/invoicedocument/invoicedocument.xaml?orderid=" + OrderIdLookup, UriKind.Relative));
        }

        void DoViewReceipt()
        {
            
            if (SelectedReceipt.Id == Guid.Empty)
                MessageBox.Show("Select a receipt to view.", "Distributr: View Receipt", MessageBoxButton.OK);
            else
            {
                ConfirmNavigatingAway = false;
                SendNavigationRequestMessage(
                    new Uri(
                        "/views/receiptdocuments/receiptdocument.xaml?orderid=" + OrderIdLookup + "&receiptid=" +
                        SelectedReceipt.Id,
                        UriKind.Relative));
            }
        }

        #endregion

        #region Clear & Setup

        public void RunClearAndSetup()
        {
            ConfirmNavigatingAway = true;
            OrderIdLookup = Guid.Empty;
            Status = "";
            OrderId = "";
            DateRequired = DateTime.Now;
            DateSubmitted = DateTime.MaxValue;
            CreatedByUser = "";
            AmountPaid = 0m;
            SaleDiscount = 0m;
            TotalProductDiscount = 0m;
            LineItems.Clear();
            PaymentInfoList.Clear();
            _clientRequestResponses.Clear();
            CalcTotals();
            ClearViewModel();
            OrderDocument = null;
            InvoiceDocument = null;
        }

        void ClearViewModel()
        {
            DocumentRef = "";
            DateRequired = DateTime.Now;
            LineItems.Clear();
            TotalGross = 0;
            TotalNet = 0;
            TotalVat = 0;
            CashAmount = 0;
            ChequeAmount = 0;
            SaleDiscount = 0m;
            TotalProductDiscount = 0m;
            ChequeNo = null;
            MMoneyAmount = 0;
            MMoneyRefNo = null;
            AmountPaid = 0;
            SaleValue = 0m;
            ReturnableValue = 0m;
            //SelectedSalesman = null;
            //SelectedOutlet = null;
            //SelectedRoute = null;
            OrderDocument = null;
            if (Receipts != null)
                Receipts.Clear();

            PaymentTransactionRefId = Guid.Empty;
            SelectedReceipt = new Receipt(Guid.Empty);
        }

        private void Cancel()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                if (OrderDocument != null)
                {
                    Using<IOrderRepository>(cont).ChangeStatus(OrderDocument.Id, DocumentStatus.Rejected);
                    if (InvoiceDocument != null)
                        Using<IInvoiceRepository>(cont).ChangeStatus(InvoiceDocument.Id, DocumentStatus.Rejected);
                    if (Using<IOrderRepository>(cont).GetById(OrderDocument.Id) != null)
                    {
                        Using<IPOSSalesWFManager>(cont).RejectOrder(OrderDocument);
                    }
                }
                ClearViewModel();
            }
        }

        #endregion

        #region Payment

        private List<PaymentNotificationResponse> _paymentNotifs;

        public void AddPaymentInfo(decimal cashAmnt, decimal creditAmnt, decimal mMoneyAmnt, decimal chequeAmnt,
                                   decimal amountPaid, string mMoneyReferenceNo, string chequeNo, decimal grossAmount,
                                   decimal change, Bank bank, BankBranch bBranch, string mMoneyOption,
                                   bool mMoneyIsApproved, Guid mMoneyTransactionRefId, string mMoneyAccountId,
                                   string mMoneySubscriberId, string mMoneyTillNumber, string currency,
                                   PaymentNotificationResponse paymentNotif,
                                   PaymentResponse paymentResponse)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                CashAmount = cashAmnt - change;
                bankBranch = bBranch;
                TotalGross = grossAmount + AmountPaid;

                CreditAmount = creditAmnt;
                MMoneyAmount = mMoneyAmnt;
                ChequeAmount = chequeAmnt;
                MMoneyRefNo = mMoneyReferenceNo;
                ChequeNo = chequeNo + " - " + (bank != null ? bank.Name : "");
                AmountPaid = amountPaid + AmountPaid;
                MMoneyOption = mMoneyOption;

                string desc = "";
              
                #region Cash

                if (cashAmnt > 0)
                {
                    var existing = PaymentInfoList.FirstOrDefault(n => n.PaymentModeUsed == PaymentMode.Cash && n.IsNew);
                    if (existing == null)
                    {
                        existing = new PaymentInfo
                                       {
                                           Id = Guid.NewGuid(),
                                           Amount = cashAmnt - change, //??
                                           PaymentModeUsed = PaymentMode.Cash,
                                           IsNew = true,
                                           IsConfirmed = true,
                                           PaymentRefId = "Cash",
                                           MMoneyPaymentType = "",
                                           PaymentTypeDisplayer = "Cash",
                                           Description = "", 
                                       };
                        PaymentInfoList.Add(existing);
                    }
                    else
                        existing.Amount += cashAmnt;

                    desc = GetLocalText("sl.payment.notifitcation.desc.inpaymentof") /*"In payment of" */
                           + " " + currency + " " + existing.Amount + ".";
                    existing.Description = desc;
                }

                #endregion

                #region Cheq

                if (chequeAmnt > 0)
                {
                    var existing =
                        PaymentInfoList.FirstOrDefault(n => n.PaymentModeUsed == PaymentMode.Cheque && n.IsNew);
                    if (existing == null)
                    {
                        existing = new PaymentInfo
                                       {
                                           Id = Guid.NewGuid(),
                                           Amount = chequeAmnt,
                                           PaymentModeUsed = PaymentMode.Cheque,
                                           PaymentRefId = chequeNo + " - " + (bank != null ? bank.Name : ""),
                                           IsNew = true,
                                           IsConfirmed = true,
                                           MMoneyPaymentType = "",
                                           PaymentTypeDisplayer =
                                               "Cheque " + chequeNo + " - " + (bank != null ? bank.Name : ""),
                                           Description = ""
                                       };
                        PaymentInfoList.Add(existing);
                    }
                    else
                        existing.Amount += chequeAmnt;

                    desc = GetLocalText("sl.payment.notifitcation.desc.inpaymentof") /*"I payment of"*/
                           + " " + currency + " " + existing.Amount + " "
                           + GetLocalText("sl.payment.notifitcation.desc.tobank") /*"to bank"*/
                           + " " + (bank != null ? bank.Name : "") + " "
                           + GetLocalText("sl.payment.notifitcation.desc.chequenumber") /*"cheque number"*/
                           + " " + chequeNo + ".";
                    existing.Description = desc;
                }

                #endregion

                #region M-Money

                if (mMoneyAmnt > 0)
                {
                    if (mMoneyTransactionRefId == Guid.Empty)
                        throw new Exception("Transaction reference id not set.\nAddPaymentInfo()");

                    //cn: Add or replace a notification.
                    var existingNotif = _paymentNotifs.FirstOrDefault(n => n.Id == paymentNotif.Id);
                    if (existingNotif != null)
                        _paymentNotifs.Remove(existingNotif);

                    if (paymentNotif != null)
                        _paymentNotifs.Add(paymentNotif);

                    var mmPayment = new PaymentInfo
                                        {
                                            Id = mMoneyTransactionRefId,
                                            Amount = mMoneyAmnt,
                                            PaymentModeUsed = PaymentMode.MMoney,
                                            MMoneyPaymentType = mMoneyOption,
                                            IsNew = true,
                                            IsConfirmed = mMoneyIsApproved,
                                            PaymentRefId = mMoneyReferenceNo,
                                            PaymentTypeDisplayer = mMoneyOption,
                                            Description = desc
                                        };
                    PaymentInfoList.Add(mmPayment);

                    if (mmPayment.IsConfirmed)
                        desc = MMoneyDescription(mmPayment.Amount, currency, mMoneySubscriberId, mMoneyAccountId,
                                                 mMoneyTillNumber, MMoneyRefNo);
                    else
                        desc = paymentResponse.LongDescription != ""
                                   ? paymentResponse.LongDescription
                                   : paymentResponse.ShortDescription;

                    mmPayment.Description = desc;
                }

                #endregion

                #region Credit

                var credit = PaymentInfoList.FirstOrDefault(n => n.PaymentModeUsed == PaymentMode.Credit && n.IsNew);
                if (credit == null)
                {
                    credit = new PaymentInfo
                                 {
                                     Id = Guid.NewGuid(),
                                     Amount = creditAmnt,
                                     PaymentModeUsed = PaymentMode.Credit,
                                     IsNew = true,
                                     IsConfirmed = true,
                                     MMoneyPaymentType = "",
                                     PaymentRefId = "",
                                     PaymentTypeDisplayer = "Credit"
                                 };

                    PaymentInfoList.Add(credit);
                }
                else
                    credit.Amount = creditAmnt;

                if (credit.Amount == 0)
                    PaymentInfoList.Remove(credit);

                #endregion

                RecalcAmountPaid();
                CanSaveToContinue = false;
            }
        }

        private string MMoneyDescription(decimal amount, string currency, string mMoneySubscriberId,
                                         string mMoneyAccountId, string mMoneyTillNumber, string mMoneyRefNo)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                string desc = "";
                desc = GetLocalText("sl.payment.notifitcation.desc.inpaymentof") /*"In payment of"*/
                       + " " + currency + " " + Math.Round(amount, 2);
                if (mMoneySubscriberId != "")
                    desc += GetLocalText("sl.payment.notifitcation.desc.bysubscriber") /*"by subscriber"*/
                            + " " + mMoneySubscriberId + " ";
                if (mMoneyAccountId != "")
                    desc += GetLocalText("sl.payment.notifitcation.desc.toaccount") /*"to account"*/
                            + " " + mMoneyAccountId + ". ";
                if (mMoneyRefNo != "")
                    desc += GetLocalText("sl.payment.notifitcation.desc.paymentreference")
                            /*"Payment reference"*/
                            + " #: " + mMoneyRefNo + (mMoneyTillNumber != ""
                                                          ? "; "
                                                            +
                                                            GetLocalText("sl.payment.bgnotifitcation.desc.tillnumber")
                                                            /*"Buy Goods Till"*/
                                                            + " #:" + " " + mMoneyTillNumber
                                                          : "");
                return desc;
            }
        }

        void RecalcAmountPaid()
        {
            AmountPaid = PaymentInfoList.Where(n => n.PaymentModeUsed != PaymentMode.Credit /*&& n.IsConfirmed*/).Sum(n => n.Amount);
        }

        public void UpdatePaymentInfo(Guid itemId, decimal amount, decimal creditAmnt)
        {
            var credit = PaymentInfoList.FirstOrDefault(n => n.PaymentModeUsed == PaymentMode.Credit);
            if (credit != null)
            {
                if (creditAmnt == 0)
                    PaymentInfoList.Remove(credit);
                else
                    credit.Amount = creditAmnt;
            }
            else
            {
                if (creditAmnt > 0)
                {
                    var newItem = new PaymentInfo
                                      {
                                          Id                = Guid.NewGuid(),
                                          Amount            = creditAmnt,
                                          PaymentModeUsed   = PaymentMode.Credit,
                                          IsNew             = true,
                                          IsConfirmed        = true,
                                          MMoneyPaymentType = "",
                                          PaymentRefId      = ""
                                      };
                    PaymentInfoList.Add(newItem);
                }
            }
            RecalcAmountPaid();
        }

        public void RemovePaymentIfo(Guid Id)
        {
            var existing = PaymentInfoList.FirstOrDefault(n => n.Id == Id);
            if (existing != null)
            {
                PaymentInfoList.Remove(existing);
                AmountPaid -= existing.Amount;

                var credit = PaymentInfoList.FirstOrDefault(n => n.PaymentModeUsed == PaymentMode.Credit);
                if (credit != null)
                {
                    credit.Amount += existing.Amount;
                }
                else
                {
                    var newItem = new PaymentInfo
                                      {
                                          Id = Guid.NewGuid(),
                                          Amount = existing.Amount,
                                          PaymentModeUsed = PaymentMode.Credit,
                                          IsNew = true,
                                          IsConfirmed = true,
                                          MMoneyPaymentType = "",
                                          PaymentRefId = ""
                                      };
                    PaymentInfoList.Add(newItem);
                }

                if (_paymentNotifs != null && _paymentNotifs.Count > 0)
                {
                    var myNotif = _paymentNotifs.FirstOrDefault(n => n.TransactionRefId == Id.ToString());
                    if (myNotif != null)
                        _paymentNotifs.Remove(myNotif);
                }
            }

            //????????????????????????????????????????????????????????????????????????????????????????????????????????????
            if (PaymentInfoList.Count == 1 && PaymentInfoList.First().PaymentModeUsed == PaymentMode.Credit)
            {
                var rem = PaymentInfoList.First();
                PaymentInfoList.Remove(rem);
            }
            RecalcAmountPaid();
            if (PaymentInfoList.Count == 0)
                CanSaveToContinue = true;
        }

        private void SubmitPaymentInfo(Order order, IEnumerable<PaymentInfo> paymentInfo, Guid invoiceId,
                                       string invoiceRef)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                var confirmedM_Payments =
                    paymentInfo.Where(n => n.PaymentModeUsed == PaymentMode.MMoney && n.IsConfirmed);
                foreach (var item in confirmedM_Payments)
                {
                    var notif = _paymentNotifs.FirstOrDefault(n => n.TransactionRefId == item.Id.ToString());
                    if (notif != null)
                    {
                        string seed = notif.PaymentNotificationDetails.Aggregate("",
                                                                                 (current, detail) =>
                                                                                 current + (detail.Id + ";"));
                        item.NotificationId = seed;
                    }
                }
                var receipt = Using<IPOSSalesWFManager>(cont).SubmitPaymentChanges(order, paymentInfo.ToList(), invoiceId, invoiceRef);

                var mmPayments =
                    paymentInfo.Where(n => n.PaymentModeUsed == PaymentMode.MMoney && !n.IsConfirmed).ToList();

                if (mmPayments.Count() == 0)
                    return;
                SubmitUnConfirmedMMoneyPayments(order, receipt, mmPayments, invoiceId, invoiceRef);

            }
        }

        private void SubmitUnConfirmedMMoneyPayments(Order order, Receipt paymentDoc,
                                                     IEnumerable<PaymentInfo> mmPayments, Guid invoiceId,
                                                     string invoiceRef)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                foreach (var mmPayment in mmPayments)
                {
                    var newPaymentInfos = new List<PaymentInfo>();
                    //cn: if payment was made in partial check confirmation
                    if (!mmPayment.IsConfirmed) //cn: means sum of paymentNotif.PaidAmount < mmPayment.Amount
                    {
                        var paymentNotifs =
                            _paymentNotifs.Where(n => n.TransactionRefId == mmPayment.Id.ToString()).ToList();
                        if (paymentNotifs.Count > 0)
                        {
                            //cn: for each notification with a payment greater than 0, create line item
                            foreach (var paymentNotif in paymentNotifs)
                            {
                                foreach (
                                    var item in paymentNotif.PaymentNotificationDetails.Where(n => n.PaidAmount > 0))
                                {
                                    PaymentInfo info = new PaymentInfo
                                                           {
                                                               Amount = (decimal) item.PaidAmount,
                                                               Id = Guid.NewGuid(),
                                                               PaymentModeUsed = PaymentMode.MMoney,
                                                               IsConfirmed = true,
                                                               IsNew = true,
                                                               MMoneyPaymentType = mmPayment.MMoneyPaymentType,
                                                               PaymentRefId = mmPayment.PaymentRefId,
                                                               PaymentTypeDisplayer = mmPayment.PaymentTypeDisplayer,
                                                               NotificationId = item.Id + ";",
                                                               Description =
                                                                   MMoneyDescription((decimal) item.PaidAmount,
                                                                                     paymentNotif.Currency,
                                                                                     paymentNotif.SubscriberId,
                                                                                     paymentNotif.AccountId, "",
                                                                                     paymentNotif.SDPReferenceId)
                                                           };
                                    newPaymentInfos.Add(info);
                                }
                            }

                            Using<IPOSSalesWFManager>(cont).SubmitSecondarymMoneyPayment(order, paymentDoc, newPaymentInfos,
                                                                            invoiceId, invoiceRef);
                        }
                    }
                }
                _paymentNotifs.Clear();
            }
        }

        public void ConfirmOrderPayment() //Used by Receiving payment module
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                if (OrderIdLookup == Guid.Empty)
                    throw new Exception("specified order could not be found");
                Order order = Using<IOrderRepository>(cont).GetById(OrderIdLookup) as Order;
                SubmitPaymentInfo(order, PaymentInfoList, InvoiceIdLookUp, DocumentRef);
            }
        }

        private ListInvoicesViewModel.UnconfirmedReceiptPayment _payment;

        public void ConfirmThisPayment(ListInvoicesViewModel.UnconfirmedReceiptPayment payment, string recDocReference,
                                       string mMoneyPaymentType, string buyGoodsTransReference)
        {
            //isBusyWindow = new BusyWindow();
            //isBusyWindow.lblWhatsUp.Content = "Fetching payment notification.";
            //isBusyWindow.Show();
            using (StructureMap.IContainer cont = NestedContainer)
            {
                Using<IAuditLogWFManager>(cont).AuditLogEntry("PendingPayment",
                                                 "Attempted confirm payment to receipt line item Id :" +
                                                 payment.LineItemId +
                                                 " For receipt: " + recDocReference);

                _payment = payment;
                //_paymentNotifs.Clear();
                PaymentTransactionRefId = payment.LineItemId;
                ClientRequestResponseType type = ClientRequestResponseType.AsynchronousPaymentNotification;
                if (mMoneyPaymentType.ToLower() == "buy goods" || mMoneyPaymentType.ToLower() == "buy-goods" ||
                    mMoneyPaymentType.ToLower() == "buygoods")
                {
                    type = ClientRequestResponseType.BuyGoodsNotification;
                }
                string reqMsg = GetPaymentNotificationRequestJson(PaymentTransactionRefId, buyGoodsTransReference, type);

                GetPaymentNotification(reqMsg, type);
            }
        }

        private void ConfirmReceiptLineItem(Guid receiptLineItemId, string paymentRef, string description)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                PaymentInfo paymentInfo = null;

                if (PaymentInfoList != null)
                {
                    paymentInfo = PaymentInfoList.FirstOrDefault(n => n.Id == receiptLineItemId);
                }
                if (paymentInfo != null)
                {
                    List<PaymentInfo> temp = new List<PaymentInfo>();
                    PaymentInfoList.ToList().ForEach(temp.Add);
                    paymentInfo.IsConfirmed = true;

                    //attempt refresh to UI
                    PaymentInfoList.Clear();
                    temp.ForEach(PaymentInfoList.Add);
                }

                if (paymentInfo == null || paymentInfo.IsNew) return;

                Receipt receipt = null;
                if (Receipts != null)
                {
                    receipt = Receipts.FirstOrDefault(n => n.LineItems.Any(l => l.Id == receiptLineItemId));
                }
                if (receipt == null)
                {
                    receipt = Using<IReceiptRepository>(cont).GetAll().OfType<Receipt>().Where(n => n.LineItems.Any(x => x.Id == receiptLineItemId)).
                                              FirstOrDefault();
                }

                //cn: Mother of god!!!!
                if (receipt != null)
                {
                    ReceiptLineItem item = receipt.LineItems.FirstOrDefault(n => n.Id == receiptLineItemId);
                    if (_paymentNotifs.Count > 0)
                    {
                        List<PaymentInfo> paymentInfos = new List<PaymentInfo>();
                        var otherReceipts = Using<IReceiptRepository>(cont).GetByInvoiceId(_payment.InvoiceId);
                        foreach (var paymentNotif in _paymentNotifs)
                        {
                            foreach (var detail in paymentNotif.PaymentNotificationDetails.Where(n => n.PaidAmount > 0))
                            {
                                //cn: Check whether this amount had been confirmed
                                if (otherReceipts.Count > 0)
                                {
                                    var paymentDoc =
                                        otherReceipts.FirstOrDefault(
                                            n =>
                                            n.LineItems.Any(l => l.Id == receiptLineItemId) &&
                                            n.PaymentDocId == Guid.Empty);
                                    var childRecs = otherReceipts.Where(n => n.PaymentDocId == paymentDoc.Id).ToList();
                                    if (childRecs.Count > 0)
                                    {
                                        var notificationIds =
                                            childRecs.SelectMany(n => n.LineItems)
                                                     .SelectMany(n => n.NotificationId.Split(';'))
                                                     .Where(n => n.Length > 0);

                                        if (!notificationIds.Any(n => n.ToLower() == detail.Id.ToString().ToLower()))
                                        {
                                            var info = Map(detail, item, paymentNotif);
                                            paymentInfos.Add(info);
                                        }
                                    }
                                    else //cn: First confirmation.
                                    {
                                        var info = Map(detail, item, paymentNotif);
                                        paymentInfos.Add(info);
                                    }
                                }
                            }
                        }
                        var order = Using<IOrderRepository>(cont).GetById(OrderIdLookup) as Order;

                        Using<IPOSSalesWFManager>(cont).SubmitSecondarymMoneyPayment(order, receipt, paymentInfos, _payment.InvoiceId,
                                                                        _payment.InvoiceDocReference);
                    }
                }

                _paymentNotifs.Clear();
            }
        }

        PaymentInfo Map(PaymentNotificationListItem detail, ReceiptLineItem item, PaymentNotificationResponse paymentNotif)
        {
            var info = new PaymentInfo
                           {
                               Amount = (decimal) detail.PaidAmount,
                               Id = Guid.NewGuid(),
                               PaymentModeUsed = PaymentMode.MMoney,
                               IsConfirmed = true,
                               IsNew = true,
                               MMoneyPaymentType = item.MMoneyPaymentType,
                               PaymentRefId = item.PaymentRefId,
                               PaymentTypeDisplayer = item.MMoneyPaymentType,
                               NotificationId = detail.Id + ";",
                               Description = MMoneyDescription((decimal) detail.PaidAmount,
                                                               paymentNotif.Currency,
                                                               paymentNotif.SubscriberId,
                                                               paymentNotif.AccountId, "",
                                                               paymentNotif.SDPReferenceId)
                           };

            return info;
        }

        public void GetPaymentNotification(string requestMsg, ClientRequestResponseType type)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                _paymentNotificationCompleted = false;

                string url = Using<IPaymentService>(cont).GetPGWSUrl(type);
                Uri uri = new Uri(url, UriKind.Absolute);
                WebClient wc = new WebClient();
                wc.UploadStringCompleted +=wc_UploadGetPaymentNotificationJsonCompleted;
                wc.UploadStringAsync(uri, "POST", "jsonMessage=" + requestMsg);
            }
        }

        private void wc_UploadGetPaymentNotificationJsonCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                Thread.Sleep(1000);
                try
                {
                    if (e.Error == null)
                    {
                        if (_paymentNotificationCompleted)
                        {
                            return;
                        }

                        MMoneyIsApproved = false;
                        string jsonResult = e.Result;
                        if (!ValidateResponse(jsonResult))
                        {
                            ReportPaymentNotificationError("");
                            return;
                        }
                        PaymentNotificationResponse sapr = null;
                        BuyGoodsNotificationResponse bgr = null;
                        string msg = "";
                        string desc = "";
                        string reference = "";

                        JObject jo = JObject.Parse(jsonResult);
                        //JArray ja = JArray.Parse(jsonResult);
                        //var jo = ja.FirstOrDefault();
                        double totalPaid = 0.0;
                        double balance = Convert.ToDouble(MMoneyAmount);
                        string currency = "KES";

                        #region AsynchronousPaymentNotificationResponse

                        if ((int) (jo["ClientRequestResponseType"]) == 3)
                        {
                            MessageSerializer.CanDeserializeMessage(jsonResult, out sapr);

                            if (sapr == null)
                            {
                                msg = GetLocalText("sl.payment.notifitcation.pending");
                            }
                            else if ((sapr.PaymentNotificationDetails != null &&
                                      sapr.PaymentNotificationDetails.Count == 0) || sapr.StatusDetail == "Pending")
                            {
                                msg = GetLocalText("sl.payment.notifitcation.pending");
                                    /*"Specified payment is still pending. Please check again later.";*/
                            }
                            else
                            {
                                var existingNotif =
                                    _paymentNotifs.FirstOrDefault(n => n.TransactionRefId == sapr.TransactionRefId);
                                if (existingNotif != null)
                                    _paymentNotifs.Remove(existingNotif);
                                _paymentNotifs.Add(sapr);

                                int i = 1;
                                totalPaid = sapr.PaymentNotificationDetails.Sum(n => n.PaidAmount);
                                MMoneyIsApproved = totalPaid >= Convert.ToDouble(MMoneyAmount);
                                balance =
                                    sapr.PaymentNotificationDetails.OrderByDescending(n => n.TimeStamp)
                                        .FirstOrDefault()
                                        .BalanceDue;
                                currency = sapr.Currency;

                                msg = GetLocalText("sl.payment.notifitcation.response.paymentDetails")
                                      /*"Payment Details:"*/
                                      + "\tCount = " + sapr.PaymentNotificationDetails.Count + "\n\n";

                                msg +=
                                    GetLocalText("sl.payment.notifitcation.response.paymentStatus")
                                    /*"Payment Status:"*/+ " \t" + sapr.StatusDetail + ",\n"
                                    + GetLocalText("sl.payment.notifitcation.response.currency")
                                    /*"Currency:"*/+ " \t\t" + sapr.Currency + ",\n"
                                    + GetLocalText("sl.payment.notifitcation.response.reference")
                                    /*"Reference Id:"*/+ " \t" + sapr.SDPReferenceId + "\n";

                                double cumTotalPaid = 0;
                                foreach (var pnr in sapr.PaymentNotificationDetails.OrderBy(n => n.TimeStamp))
                                {
                                    cumTotalPaid += pnr.PaidAmount;
                                    var notif = new PaymentNotificationResponse
                                                    {
                                                        Id = Guid.NewGuid(),
                                                        TransactionRefId = sapr.TransactionRefId,
                                                        SDPTransactionRefId = sapr.SDPTransactionRefId,
                                                        SDPReferenceId = sapr.SDPReferenceId,
                                                        StatusCode = sapr.StatusCode,
                                                        StatusDetail = sapr.StatusDetail,
                                                        DateCreated = sapr.DateCreated,
                                                        Currency = sapr.Currency,
                                                        ClientRequestResponseType =
                                                            ClientRequestResponseType
                                                            .AsynchronousPaymentNotification,
                                                        DistributorCostCenterId = sapr.DistributorCostCenterId,
                                                        BalanceDue = pnr.BalanceDue,
                                                        PaidAmount = pnr.PaidAmount,
                                                        TimeStamp = pnr.TimeStamp,
                                                        TotalAmount = cumTotalPaid //pnr.TotalAmount 
                                                    };

                                    msg += "# " + i + ":\n";
                                    msg +=
                                        GetLocalText("sl.payment.notifitcation.response.amountPaid")
                                        /*"Amount Paid:"*/+ " \t" + pnr.PaidAmount + ",\n"
                                        + GetLocalText("sl.payment.notifitcation.response.balanceDue")
                                        /*"Balance Due:"*/+ " \t" + pnr.BalanceDue + ",\n"
                                        + GetLocalText("sl.payment.notifitcation.response.totalAmount")
                                        /*"Total Amount:"*/+ " \t" + cumTotalPaid /*pnr.TotalAmount*/+ ",\n"
                                        + GetLocalText("sl.payment.notifitcation.response.timeStamp")
                                        /*"Time Stamp:"*/+ " \t" + pnr.TimeStamp + "\n\n"
                                        + "Status:" + " \t" + pnr.Status + "\n\n"
                                        ;
                                    //_asynchronousPaymentNotificationResponseService.Save(notif);
                                    _clientRequestResponses.Add(notif);
                                    i++;
                                }

                                var p = sapr.PaymentNotificationDetails.First();
                                desc = GetLocalText("sl.payment.notifitcation.desc.inpaymentof")
                                       /*"In payment of"*/
                                       + " " + sapr.Currency + " " + p.PaidAmount
                                       + " " +
                                       GetLocalText("sl.payment.notifitcation.desc.bysubscriber")
                                       /*"by subscriber"*/
                                       + " " + sapr.SubscriberId + " " +
                                       GetLocalText("sl.payment.notifitcation.desc.toaccount")
                                       /*"to account"*/+
                                       " " + sapr.AccountId
                                       + " " +
                                       GetLocalText("sl.payment.notifitcation.desc.paymentreference")
                                       /*"Payment reference:"*/
                                       + " " + sapr.SDPReferenceId;

                                reference = sapr.SDPReferenceId;

                                //if (MMoneyIsApproved)
                                if (OrderDocument == null ||
                                    (OrderDocument != null && OrderDocument.Status != DocumentStatus.New))
                                    ConfirmReceiptLineItem(PaymentTransactionRefId, reference, desc);

                                else if (OrderDocument != null && OrderDocument.Status == DocumentStatus.New)
                                {
                                    if (PaymentInfoList != null)
                                    {
                                        var info =
                                            PaymentInfoList.FirstOrDefault(
                                                n => n.Id.ToString() == sapr.TransactionRefId);
                                        if (info != null)
                                            info.IsConfirmed = MMoneyIsApproved;
                                    }
                                }
                            }
                        }
                            #endregion

                            #region BuyGoodsNotificationResponse

                        else if ((int) (jo["ClientRequestResponseType"]) == 5)
                        {
                            MessageSerializer.CanDeserializeMessage(jsonResult, out bgr);

                            if (bgr == null || bgr.StatusDetail == "Pending")
                            {
                                msg = GetLocalText("sl.payment.notifitcation.pending");
                                    /*"Specified payment is still pending. Please check again later.";*/
                            }
                            else
                            {
                                totalPaid = bgr.PaidAmount;
                                balance = Convert.ToDouble(MMoneyAmount) - bgr.PaidAmount;
                                currency = bgr.Currency;
                                MMoneyIsApproved = totalPaid >= Convert.ToDouble(MMoneyAmount);
                                msg = GetLocalText("sl.payment.notifitcation.response.paymentDetails")
                                      /*"Payment Details:"*/
                                      + "\n\n";

                                PaymentRequest apr =
                                    Using<IAsynchronousPaymentRequestRepository>(cont).GetByTransactionRefId(bgr.TransactionRefId).
                                                                       LastOrDefault();

                                msg +=
                                    GetLocalText("sl.payment.bgnotifitcation.response.paymentStatus")
                                    /*"Payment Status:"*/+ " \t" + bgr.StatusDetail + ",\n"
                                    + GetLocalText("sl.payment.bgnotifitcation.response.amountPaid")
                                    /*"Amount Paid:"*/+ " \t" + bgr.PaidAmount + "\n"
                                    + GetLocalText("sl.payment.bgnotifitcation.response.receiptNumber")
                                    /*"Receipt Number:"*/+ " \t" + bgr.ReceiptNumber + "\n"
                                    + GetLocalText("sl.payment.bgnotifitcation.response.date") /*"Date:"*/+
                                    " \t" + bgr.Date.ToShortDateString() + "\n"
                                    + GetLocalText("sl.payment.bgnotifitcation.response.time") /*"Time:"*/+
                                    " \t" + bgr.Time.ToShortTimeString() + "\n"
                                    + GetLocalText("sl.payment.bgnotifitcation.response.currency")
                                    /*"Currency:"*/+ " \t" + bgr.Currency + "\n"
                                    + GetLocalText("sl.payment.bgnotifitcation.response.paidTo")
                                    /*"Paid To Subscriber:"*/+ " \t" + bgr.SubscriberName + "\n"
                                    + GetLocalText("sl.payment.bgnotifitcation.response.merchantBal")
                                    /*"Merchant Balance:"*/+ " \t" + bgr.MerchantBalance + "\n\n"
                                    ;
                                Using<IBuyGoodsNotificationResponseRepository>(cont).Save(bgr);
                                _clientRequestResponses.Add(bgr);

                                reference = bgr.ReceiptNumber;
                                desc =
                                    GetLocalText("sl.payment.notifitcation.desc.inpaymentof")
                                    /*"In payment of"*/+ " " + bgr.Currency + " " + bgr.PaidAmount
                                    + " " + GetLocalText("sl.payment.notifitcation.desc.bysubscriber")
                                    /*"by subscriber"*/
                                    + " " + (apr != null ? apr.SubscriberId : "") + " " +
                                    GetLocalText("sl.payment.notifitcation.desc.toaccount") /*"to account"*/+
                                    " " + (apr != null ? apr.AccountId : "")
                                    + " " + GetLocalText("sl.payment.notifitcation.desc.paymentreference")
                                    /*"Payment reference:"*/
                                    + " " + bgr.ReceiptNumber;

                                if (MMoneyIsApproved)
                                    ConfirmReceiptLineItem(PaymentTransactionRefId, reference, desc);
                                //else
                                //    MessageBox.Show(
                                //        GetLocalText("sl.payment.notification.notconfirmed")/*"Payment not confirmed due to outstanding balance of"*/
                                //            + " " + (Convert.ToDouble(MMoneyAmount) - totalPaid),
                                //        GetLocalText("sl.payment.title") /*"Distributr: Payment Module"*/,
                                //        MessageBoxButton.OK);
                            }
                        }
                            #endregion

                        else
                        {
                            ReportPaymentNotificationError("");
                            return;
                        }
                        MessageBox.Show(msg, GetLocalText("sl.payment.title")
                                        /*"Distributr: Payment Module"*/, MessageBoxButton.OK);

                        if (!MMoneyIsApproved)
                        {
                            MessageBox.Show(
                                GetLocalText("sl.payment.notification.notconfirmed")
                                /*"Payment not confirmed due to outstanding balance of"*/
                                + " " + currency + " " + balance /*(Convert.ToDouble(MMoneyAmount) - totalPaid)*/,
                                GetLocalText("sl.payment.title") /*"Distributr: Payment Module"*/,
                                MessageBoxButton.OK);
                        }
                        _paymentNotificationCompleted = true;
                        //if (isBusyWindow != null)
                        //    isBusyWindow.OKButton_Click(this, null);


                    }
                    else
                    {
                        ReportPaymentNotificationError(e.Error.Message);
                    }
                }
                catch (Exception ex)
                {
                    ReportPaymentNotificationError(ex.Message);
                }
            }
        }

        public void ViewPaymentDetails(string transactionRefId, string paymentType)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                PaymentResponse pr = null;
                string msg = "Details not available.";
                if (paymentType.ToLower() != "buy goods")
                {
                    pr = Using<IAsynchronousPaymentResponseRepository>(cont).GetByTransactionRef(transactionRefId).LastOrDefault();
                    if (pr != null)
                    {
                        msg = string.Format(
                            GetLocalText("sl.payment.paymentresponse.message.amountDue") /*"Amount due:"*/+
                            " \t{0};\n"
                            + GetLocalText("sl.payment.paymentresponse.message.businessnumber")
                            /*"Business Number:"*/+ " \t{1};\n"
                            + GetLocalText("sl.payment.paymentresponse.message.accountnumber")
                            /*"Account No:"*/+ " \t{2};\n"
                            + GetLocalText("sl.payment.paymentresponse.message.timestamp") /*"Time Stamp:"*/+
                            "\t {3};\n"
                            + GetLocalText("sl.payment.paymentresponse.message.statuscode")
                            /*"Status Code:"*/+ "\t{4};\n"
                            + GetLocalText("sl.payment.paymentresponse.message.statusdetail")
                            /*"Status Details:"*/+ "\t{5};\n",
                            //+ "Description:\t{6};\n"
                            pr.AmountDue,
                            pr.BusinessNumber,
                            pr.SDPReferenceId,
                            pr.TimeStamp,
                            pr.StatusCode,
                            pr.StatusDetail
                            //pr.LongDescription
                            );
                    }
                }

                MessageBox.Show(msg, GetLocalText("sl.payment.title"), MessageBoxButton.OK);
            }
        }

        bool ValidateResponse(string json)
        {
            bool isResponseValid = true;
            PGResponseBasic rb = null;
            if (MessageSerializer.CanDeserializeMessage(json, out rb))
            {
                if (rb.Result == "Invalid")
                    isResponseValid = false;
            }

            return isResponseValid;
        }

        private string GetPaymentNotificationRequestJson(Guid paymentTransactionRefId, string buyGoodsTransRef,
                                                         ClientRequestResponseType type)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                string json = "";
                string transRef = paymentTransactionRefId.ToString();
                if (type == ClientRequestResponseType.BuyGoodsNotification)
                {
                    transRef = buyGoodsTransRef;
                }
                ClientRequestResponseBase pnr = new ClientRequestResponseBase()
                                                    {
                                                        ClientRequestResponseType = type,
                                                        DateCreated = DateTime.Now,
                                                        Id = Guid.NewGuid(),
                                                        DistributorCostCenterId = Using<IConfigService>(cont).Load().CostCentreId,
                                                        TransactionRefId = transRef
                                                    };

                json = JsonConvert.SerializeObject(pnr, new IsoDateTimeConverter());
                _clientRequestResponses.Add(pnr);

                return json;
            }
        }

        private void ReportPaymentNotificationError(string msg)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                if (!_paymentNotificationCompleted)
                {
                    MessageBox.Show( /*"Unable to retrieve payment notification response.\n Please try again later or pay with any other means other than M-Money"*/
                        GetLocalText("sl.payment.notifitcation.fetcherror"),
                        GetLocalText("sl.payment.title") /*"Distributr: Payment Module"*/
                        , MessageBoxButton.OK);
                    _paymentNotificationCompleted = true;
                }
                //if (isBusyWindow != null)
                //    isBusyWindow.OKButton_Click(this, null);
            }
        }

        #endregion
    }

    #region SupportClasses

    #region LineItem Object
    public class EditPOSSaleLineItem : OrderLineItemBase
    {
        private IOtherUtilities _otherUtilities;

        public EditPOSSaleLineItem(IOtherUtilities otherUtilities)
        {
            _otherUtilities = otherUtilities;
        }

        public EditPOSSaleLineItem()
        {
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

        public const string VatAmountPropertyName = "VatAmount";
        private decimal _vatAmount = 0;
        public decimal VatAmount
        {
            get
            {
                return _vatAmount;
            }

            set
            {
                if (_vatAmount == value)
                {
                    return;
                }
                var oldValue = _vatAmount;
                _vatAmount = value;
                RaisePropertyChanged(VatAmountPropertyName);
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

        public const string IsReceivedReturnablePropertyName = "IsReceivedReturnable";
        private bool _isReceivedReturnable = false;
        public bool IsReceivedReturnable
        {
            get { return _isReceivedReturnable; }
            set
            {
                if (_isReceivedReturnable == value)
                {
                    return;
                }
                _isReceivedReturnable = value; 
                RaisePropertyChanged(IsReceivedReturnablePropertyName);
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

    #endregion
}