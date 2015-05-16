using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Workflow;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Transactional.POS.PostOrderRefactor
{
    public class EditPOSViewModel : DistributrViewModelBase
    {
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

        #region Order Properties

        public Order OrderDocument { get; set; }

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

        #endregion

        #region Methods
        #endregion
    }
}
