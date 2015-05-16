using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.WorkFlow.Discount;
using Distributr.WPF.Lib.UI.Hierarchy;
using Distributr.WPF.Lib.ViewModels.Transactional.Purchasing;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Linq;
using Distributr.Core.Domain.Master.ProductEntities;
using StructureMap;

namespace Distributr.WPF.Lib.ViewModels.Transactional.SalesmanOrders
{
    public class SOLineItemViewModel : DistributrViewModelBase
    {
       
        
        public List<LineItem> AddedLineItems = null;
        
        public SOLineItemViewModel()
        {
           

            LineItems = new ObservableCollection<LineItem>();
            MultipleProduct = new List<MultipleAddProduct>();
            AllProducts = new List<Product>();
            Products = new ObservableCollection<POLineItemProductLookupItem>();
        }

        #region Declarations
        //public RelayCommand txtQtyGotFocusCommand { get; set; }
        //public RelayCommand txtBackOrderGotFocusCommand { get; set; }
        //public RelayCommand txtLostSaleGotFocusCommand { get; set; }
        //public RelayCommand txtApproveGotFocusCommand { get; set; }
        public ObservableCollection<LineItem> LineItems { get; set; }

        public RelayCommand SellInBulkSelected { get; set; }
        public RelayCommand SellInUnitsSelected { get; set; }

        public LineItem SaleProduct = null;
        public LineItem BulkSaleContainer = null;//e.g crate
        public LineItem SaleProductReturnable = null; //e.g bottle
        public LineItem UIProduct = null; //presentational
        private ProductPricingTier tier = null;
        public List<MultipleAddProduct> MultipleProduct { set; get; }

        public bool _atApproval = false;
        
        #endregion

        #region Methods New
        public void LoadForEdit(Guid selectedProductId, decimal unitPrice,
           decimal lineItemVatValue, decimal totalPrice, decimal vatAmount, int sequenceNo, decimal quantity)
        {  
                _productPackagingSummaryService.ClearBuffer();
                var product = GetEntityById(typeof(Product), selectedProductId) as Product;
                SelectedProduct = Products.First(n => n.ProductId == selectedProductId);
                UnitPrice = unitPrice;
                LineItemVatValue = lineItemVatValue;
                TotalPrice = totalPrice;
                VatAmount = vatAmount;
                SequenceNo = sequenceNo;
                Qty = quantity;
                IsNew = false;
                IsAdd = false;
                RecalcTotal();
                IsEnabled = false;
                BindTree(product); 
        }

        public void RecalcTotal()
        {
            UnitPrice = 0;
            VatAmount = 0;
            TotalPrice = 0;
            LiTotalNet = 0;
            UnitVat = 0;
            decimal x = 0;
            if (SelectedProduct != null && SelectedProduct.ProductId != Guid.Empty)
            {
                decimal qt = Qty;

                if (LineItemType == LineItemType.Bulk)
                    qt = _productPackagingSummaryService.GetProductQuantityInBulk(SelectedProduct.ProductId)*Qty;
                List<PackagingSummary> summarypro =
                    _productPackagingSummaryService.GetProductSummaryByProduct(SelectedProduct.ProductId, qt, false);
                foreach (PackagingSummary cal in summarypro)
                {
                    decimal productUnitPrice = ProductPriceCalc(cal.Product);
                    decimal productUnitVat = ProductVatCalc(cal.Product);
                    decimal productCost = productUnitPrice*cal.Quantity;
                    decimal productVat = productUnitVat*productCost;

                    x += productCost;
                    UnitPrice += productUnitPrice;
                    UnitVat += productUnitVat;
                    VatAmount += productVat;
                    LiTotalNet += productCost;
                    TotalPrice = x + VatAmount;
                }
            }
        }

        public void AddProduct(MultipleAddProduct product)
        {
            MultipleProduct.Add(product);
            AddProductSummary(product);
        }

        public void AddProductSummary(MultipleAddProduct mp)
        {
            decimal qt = Qty;

            if (mp.LineItemType == LineItemType.Bulk)
                qt = _productPackagingSummaryService.GetProductQuantityInBulk(SelectedProduct.ProductId)*Qty;
            _productPackagingSummaryService.AddProduct(mp.Product.ProductId, qt, false, !IsAdd, false);
            List<PackagingSummary> summarypro = _productPackagingSummaryService.GetProductSummary();
            foreach (PackagingSummary item in summarypro)
            {
                decimal productUnitPrice = ProductPriceCalc(item.Product);
                decimal productUnitVat = ProductVatCalc(item.Product);
                decimal productCost = productUnitPrice*item.Quantity;
                decimal productVat = productUnitVat*productCost;

                CalculateProductSummary(item.Product.Id, item.Product.Description, item.Quantity, productUnitPrice,
                                        productCost + productVat, productUnitVat, productVat, item.ParentProductId,
                                        item.IsEditable);
            }
        }

        private void CalculateProductSummary(Guid productId, string ProductName, decimal quantity, decimal unitPrice, decimal totalPrice, decimal untiVat, decimal totalvat, Guid prarentId, bool isEditable)
        {
            ProductAddSummary summary =
                ProductAddSummaries.FirstOrDefault(x => x.ProductId == productId);
            if (summary == null)
            {
                summary = new ProductAddSummary();
                ProductAddSummaries.Add(summary);
                summary.Quantity = quantity;
                summary.ParentProductId = prarentId;
                summary.IsEditable = isEditable;

            }
            else
            {
                summary.Quantity += quantity;
            }
            summary.LineItemType = LineItemType;
            summary.ProductId = productId;
            summary.ProductName = ProductName;
            summary.UnitPrice = unitPrice;
            summary.TotalPrice = totalPrice;


            ButtonNameText = string.Format("{0} Product(s)", ProductAddSummaries.Count);
        }

        public void PriceCalc(Product product)
        {
            using (IContainer container = NestedContainer)
            {
                IDiscountProWorkflow _discountProService =Using<IDiscountProWorkflow>(container);


                UnitPrice = _discountProService.GetUnitPrice(product, SelectedOutlet.OutletProductPricingTier);

                LineItemVatValue = _discountProService.GetVATRate(product, SelectedOutlet);

                RecalcTotal();
                BindTree(product);
            }
        }

        public decimal ProductLineItemTypeVatCalc(Product product)
        {
            using (IContainer container = NestedContainer)
            {
                IDiscountProWorkflow _discountProService = Using<IDiscountProWorkflow>(container);
                return _discountProService.GetVATRate(product, SelectedOutlet);
            }
        }

        public decimal ProductPriceCalc(Product product)
        {
            using (IContainer container = NestedContainer)
            {
                IDiscountProWorkflow _discountProService = Using<IDiscountProWorkflow>(container);
                return _discountProService.GetUnitPrice(product, SelectedOutlet.OutletProductPricingTier);
            }
        }

        public decimal ProductVatCalc(Product product)
        {
            using (IContainer container = NestedContainer)
            {
                IDiscountProWorkflow _discountProService = Using<IDiscountProWorkflow>(container);
                return _discountProService.GetVATRate(product, SelectedOutlet);
            }
        }

        public void RunClearAndSetup()
        {
            Qty = 1;
            UnitPrice = 0;
            VatAmount = 0;
            TotalPrice = 0;
            LiTotalNet = 0;
            ProductTree = null;
            SellFreeOfCharge = false;
            LineItemType = LineItemType.Unit;
            RunSetup();
            _productPackagingSummaryService.ClearBuffer();
        }

        void RunSetup()
        {
            using (IContainer container = NestedContainer)
            {
                IDiscountProWorkflow _discountProService = Using<IDiscountProWorkflow>(container);

                IProductRepository _productService = Using<IProductRepository>(container);
                AllProducts.Clear();
                Products.Clear();
                var product = new POLineItemProductLookupItem()
                                  {
                                      ProductId = Guid.Empty,
                                      ProductDesc =GetLocalText("sl.order.addlineitems.modal.selectProduct"),
                                      /*"--Please Select a Product--",*/
                                      ProductCode = " "
                                  };
                Products.Add(product);
                SelectedProduct = product;

                AllProducts = _productService.GetAll().ToList();
                var allproduct =
                    AllProducts.Where(n => n is ConsolidatedProduct || n is SaleProduct).OrderBy(o => o.Description);
                allproduct.Where(n => !_discountProService.IsProductFreeOfCharge(n.Id)).ToList().ForEach(
                    n => Products.Add(new POLineItemProductLookupItem
                                          {
                                              ProductId = n.Id,
                                              ProductDesc = n.Description,
                                              ProductCode = n.ProductCode
                                          }));
            }
        }

        public void RunProductSelected()
        {
            using (IContainer container = NestedContainer)
            {

                IProductRepository _productService = Using<IProductRepository>(container);

                if (SelectedProduct != null && SelectedProduct.ProductId != Guid.Empty)
                {
                    var product = _productService.GetById(SelectedProduct.ProductId);
                    PriceCalc(product);
                    if (product is ConsolidatedProduct)
                    {
                        LineItemType = LineItemType.Unit;
                        IsEnabled = false;
                    }
                    else
                    {
                        IsEnabled = true;
                    }
                }
            }
        }

        public void BindTree(Guid productId)
        {
            using (IContainer container = NestedContainer)
            {

                IProductRepository _productService = Using<IProductRepository>(container);
                Product p = _productService.GetById(productId);
                BindTree(p);
            }
        }

        void BindTree(Product product)
        {
            if (product is ConsolidatedProduct)
            {
                ProductTree = ConsolidatedProductHierarchyHelper.GetConsolidatedProductForTreeview((ConsolidatedProduct)product, Qty);
            }
            else
                ProductTree = null;
        }

        public void ToggleFreeOfChargeProducts(bool loadFreeOfCharge)
        {
            using (IContainer container = NestedContainer)
            {

                IDiscountProWorkflow _discountProService = Using<IDiscountProWorkflow>(container);
                if (loadFreeOfCharge)
                {
                    AllProducts = _discountProService.ReturnFreeOfChargeProducts(AllProducts);
                    var toRemove =
                        Products.Where(
                            n => n.ProductId != Guid.Empty && !AllProducts.Select(p => p.Id).Contains(n.ProductId)).
                            ToList();

                    toRemove.ForEach(n => Products.Remove(n));
                    AllProducts.OrderBy(n => n.Description).ToList().ForEach(
                        n => Products.Add(new POLineItemProductLookupItem
                                              {
                                                  ProductId = n.Id,
                                                  ProductDesc = n.Description,
                                                  ProductCode = n.ProductCode
                                              }));
                }
                else
                {
                    RunSetup();
                }
            }
        }
        #endregion

        #region Properties
        public const string ButtonNameTextPropertyName = "ButtonNameText";
        private string _ButtonNameText = "";
        public string ButtonNameText
        {
            get
            {
                return _ButtonNameText;
            }

            set
            {
                if (_ButtonNameText == value)
                {
                    return;
                }

                var oldValue = _ButtonNameText;
                _ButtonNameText = value; 
                RaisePropertyChanged(ButtonNameTextPropertyName); 
            }
        }

        public const string ProductAddSummariesPropertyName = "ProductAddSummaries";
        private List<ProductAddSummary> _ProductAddSummaries = new List<ProductAddSummary>();
        public List<ProductAddSummary> ProductAddSummaries
        {
            get
            {
                return _ProductAddSummaries;
            }

            set
            {
                if (_ProductAddSummaries == value)
                {
                    return;
                }

                var oldValue = _ProductAddSummaries;
                _ProductAddSummaries = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(ProductAddSummariesPropertyName);
    }
        }

        public const string LineItemTypePropertyName = "LineItemType";
        private LineItemType _lineItemType = LineItemType.Unit;
        public LineItemType LineItemType
        {
            get
            {
                return _lineItemType;
            }

            set
            {
                if (_lineItemType == value)
                {
                    return;
                }

                var oldValue = _lineItemType;
                _lineItemType = value;
                RaisePropertyChanged(LineItemTypePropertyName);
            }
        }

        public const string IsAddPropertyName = "IsAdd";
        private bool _isAdd = true;
        public bool IsAdd
        {
            get
            {
                return _isAdd;
            }

            set
            {
                if (_isAdd == value)
                {
                    return;
                }

                var oldValue = _isAdd;
                _isAdd = value;
                RaisePropertyChanged(IsAddPropertyName);


            }
        }

        public const string IsEnabledPropertyName = "IsEnabled";
        private bool _IsEnabled = true;
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }

            set
            {
                if (_IsEnabled == value)
                {
                    return;
                }

                var oldValue = _IsEnabled;
                _IsEnabled = value;
                RaisePropertyChanged(IsEnabledPropertyName);
            }
        }

        public const string SelectedProductPropertyName = "SelectedProduct";
        private POLineItemProductLookupItem _selectedProduct = null;
        public POLineItemProductLookupItem SelectedProduct
        {
            get
            {
                return _selectedProduct;
            }

            set
            {
                if (_selectedProduct == value)
                {
                    return;
                }
                var oldValue = _selectedProduct;
                _selectedProduct = value;
                RaisePropertyChanged(SelectedProductPropertyName);
            }
        }

        public ObservableCollection<POLineItemProductLookupItem> Products { get; set; }
        public List<Product> AllProducts { get; set; }
        public const string QtyPropertyName = "Qty";
        private decimal _qty = 1;
        [Range(1, 99999999)]
        public decimal Qty
        {
            get
            {
                return _qty;
            }

            set
            {
                if (_qty == value)
                {
                    return;
                }
                var oldValue = _qty;
                _qty = value;

                RaisePropertyChanged(QtyPropertyName);
                //RecalcTotal();
                //BindTree(SelectedProduct.ProductId);
            }
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
                    return;
                var oldValue = _unitPrice;
                _unitPrice = value;
                RaisePropertyChanged(UnitPricePropertyName);
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
                    return;

                var oldValue = _vatAmount;
                _vatAmount = value;
                RaisePropertyChanged(VatAmountPropertyName);
            }
        }

        public const string TotalPricePropertyName = "TotalPrice";
        private decimal _totalPrice = 0;
        public decimal TotalPrice
        {
            get
            {
                return _totalPrice;
            }

            set
            {
                if (_totalPrice == value)
                {
                    return;
                }
                var oldValue = _totalPrice;
                _totalPrice = value;
                RaisePropertyChanged(TotalPricePropertyName);
            }
        }

        public const string IsNewPropertyName = "IsNew";
        private bool _myProperty = true;
        public bool IsNew
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
                RaisePropertyChanged(IsNewPropertyName);
            }
        }

        public const string IsUpdatedPropertyName = "IsUpdated";
        private bool _isUpdated = false;
        public bool IsUpdated
        {
            get
            {
                return _isUpdated;
            }

            set
            {
                if (_isUpdated == value)
                {
                    return;
                }

                var oldValue = _isUpdated;
                _isUpdated = value;
                RaisePropertyChanged(IsUpdatedPropertyName);
            }
        }

        public const string IsDeletedPropertyName = "IsDeleted";
        private bool _isDeleted = false;
        public bool IsDeleted
        {
            get
            {
                return _isDeleted;
            }

            set
            {
                if (_isDeleted == value)
                {
                    return;
                }

                var oldValue = _isDeleted;
                _isDeleted = value;
                RaisePropertyChanged(IsDeletedPropertyName);
            }
        }

        public const string SequenceNoPropertyName = "SequenceNo";
        private int _sequenceNo = 0;
        public int SequenceNo
        {
            get
            {
                return _sequenceNo;
            }

            set
            {
                if (_sequenceNo == value)
                    return;
                var oldValue = _sequenceNo;
                _sequenceNo = value;
                RaisePropertyChanged(SequenceNoPropertyName);
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

        public const string AvailableProductInvPropertyName = "AvailableProductInv";
        private int _availableProductInv = 0;
        public int AvailableProductInv
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

        public const string ProductTreePropertyName = "ProductTree";
        private IEnumerable<HierarchyNode<ProductViewer>> _productTree = null;
        public IEnumerable<HierarchyNode<ProductViewer>> ProductTree
        {
            get
            {
                return _productTree;
            }

            set
            {
                if (_productTree == value)
                    return;
                var oldValue = _productTree;
                _productTree = value;
                RaisePropertyChanged(ProductTreePropertyName);
            }
        }

        #region enum
        public enum ProducTypeToLoad
        {
            AllProducts = 1,
            Returnables = 2,
            NonReturnables = 3
        }
        #endregion

        public const string ProductTypeToLoadPropertyName = "ProductTypeToLoad";
        private ProducTypeToLoad _productTypeToLoad = ProducTypeToLoad.AllProducts;
        public ProducTypeToLoad ProductTypeToLoad
        {
            get
            {
                return _productTypeToLoad;
            }

            set
            {
                if (_productTypeToLoad == value)
                {
                    return;
                }

                var oldValue = _productTypeToLoad;
                _productTypeToLoad = value;
                RaisePropertyChanged(ProductTypeToLoadPropertyName);
            }
        }

        public const string SalesmanPropertyName = "Salesman";
        private User _salesman = null;
        public User Salesman
        {
            get
            {
                return _salesman;
            }

            set
            {
                if (_salesman == value)
                {
                    return;
                }

                var oldValue = _salesman;
                _salesman = value;
                RaisePropertyChanged(SalesmanPropertyName);
            }
        }

        public const string SelectedOutletIdPropertyName = "SelectedOutletId";
        private Guid _selectedOutletId = Guid.Empty;
        public Guid SelectedOutletId
        {
            get
            {
                return _selectedOutletId;
            }

            set
            {
                if (_selectedOutletId == value)
                {
                    return;
                }

                var oldValue = _selectedOutletId;
                _selectedOutletId = value;
                RaisePropertyChanged(SelectedOutletIdPropertyName);
            }
        }

        public const string SelectedOutletPropertyName = "SelectedOutlet";
        private Outlet _selectedOutlet = null;
        public Outlet SelectedOutlet
        {
            get
            {
                if (_selectedOutlet != null)
                    if (_selectedOutlet.Id == SelectedOutletId)
                        return _selectedOutlet;
                using (IContainer container = NestedContainer)
                {
                    var _costCentreService = Using<ICostCentreRepository>(container);

                    return _costCentreService.GetById(SelectedOutletId) as Outlet;
                }
            }
        }

        public const string ModalCrumbsPropertyName = "ModalCrumbs";
        private string _modalCrumbs = "";
        public string ModalCrumbs
        {
            get
            {
                return _modalCrumbs;
            }

            set
            {
                if (_modalCrumbs == value)
                {
                    return;
                }

                var oldValue = _modalCrumbs;
                _modalCrumbs = value;
                RaisePropertyChanged(ModalCrumbsPropertyName);
            }
        }

        public const string UnitVatPropertyName = "UnitVat";
        private decimal _unitVat = 0m;
        public decimal UnitVat
        {
            get
            {
                return _unitVat;
            }

            set
            {
                if (_unitVat == value)
                {
                    return;
                }

                var oldValue = _unitVat;
                _unitVat = value;
                RaisePropertyChanged(UnitVatPropertyName);
            }
        }

        public const string LiTotalNetPropertyName = "LiTotalNet";
        private decimal _liTotalNet = 0m;
        public decimal LiTotalNet
        {
            get
            {
                return _liTotalNet;
            }

            set
            {
                if (_liTotalNet == value)
                {
                    return;
                }

                var oldValue = _liTotalNet;
                _liTotalNet = value;
                RaisePropertyChanged(LiTotalNetPropertyName);
            }
        }

        public const string IsEditingPropertyName = "IsEditing";
        private bool _IsEditing = false;
        public bool IsEditing
        {
            get
            {
                return _IsEditing;
            }

            set
            {
                if (_IsEditing == value)
                {
                    return;
                }

                var oldValue = _IsEditing;
                _IsEditing = value;
                RaisePropertyChanged(IsEditingPropertyName);
            }
        }

        public const string SelectedActionDetailsPropertyName = "SelectedActionDetails";
        private string _selectedOptionDetalails = "";
        public string SelectedActionDetails
        {
            get
            {
                return _selectedOptionDetalails;
            }

            set
            {
                if (_selectedOptionDetalails == value)
                {
                    return;
                }

                var oldValue = _selectedOptionDetalails;
                _selectedOptionDetalails = value;
                RaisePropertyChanged(SelectedActionDetailsPropertyName);
            }
        }

        //public const string AwaitingStockPropertyName = "AwaitingStock";
        //private int _awaitingStock = 0;
        //public int AwaitingStock
        //{
        //    get
        //    {
        //        return _awaitingStock;
        //    }

        //    set
        //    {
        //        if (_awaitingStock == value)
        //        {
        //            return;
        //        }

        //        var oldValue = _awaitingStock;
        //        _awaitingStock = value;
        //        RaisePropertyChanged(AwaitingStockPropertyName);
        //        //RecalcQuantities();
        //        //RecalcTotal(_awaitingStock);
        //        BindTree(SelectedProduct.ProductId);
        //    }
        //}

        //public const string BackOrderPropertyName = "BackOrder";
        //private int _backOrder = 0;
        //public int BackOrder
        //{
        //    get
        //    {
        //        return _backOrder;
        //    }

        //    set
        //    {
        //        if (_backOrder == value)
        //        {
        //            return;
        //        }

        //        var oldValue = _backOrder;
        //        _backOrder = value;
        //        RaisePropertyChanged(BackOrderPropertyName);
        //        CalcLostSale();
        //        RecalcTotal(_backOrder);
        //        BindTree(SelectedProduct.ProductId);
        //    }
        //}

        //public const string LostSalePropertyName = "LostSale";
        //private int _lostSale = 0;
        //public int LostSale
        //{
        //    get
        //    {
        //        return _lostSale;
        //    }

        //    set
        //    {
        //        if (_lostSale == value)
        //        {
        //            return;
        //        }

        //        var oldValue = _lostSale;
        //        _lostSale = value;
        //        RaisePropertyChanged(LostSalePropertyName);
        //        RecalcTotal(_lostSale);
        //        BindTree(SelectedProduct.ProductId);
        //    }
        //}

        public const string OriginalQtyPropertyName = "OriginalQty";
        private int _originalQty = 0;
        public int OriginalQty
        {
            get
            {
                return _originalQty;
            }

            set
            {
                if (_originalQty == value)
                {
                    return;
                }

                var oldValue = _originalQty;
                _originalQty = value;
                RaisePropertyChanged(OriginalQtyPropertyName);
            }
        }

        //public const string ApprovePropertyName = "Approve";
        //private int _approve = 0;
        //public int Approve
        //{
        //    get
        //    {
        //        return _approve;
        //    }

        //    set
        //    {
        //        if (_approve == value)
        //        {
        //            return;
        //        }

        //        var oldValue = _approve;
        //        _approve = value;
        //        RaisePropertyChanged(ApprovePropertyName);
        //        CalcLostSale();
        //        CalcBackOrder();
        //        BindTree(SelectedProduct.ProductId);
        //    }
        //}

        public const string POSPropertyName = "POS";
        private bool _pos = false;
        public bool POS
        {
            get
            {
                return _pos;
            }

            set
            {
                if (_pos == value)
                {
                    return;
                }

                var oldValue = _pos;
                _pos = value;
                RaisePropertyChanged(POSPropertyName);
            }
        }

        public const string ProcessingBackOrderPropertyName = "ProcessingBackOrder";
        private bool _processingBackOrder = false;
        public bool ProcessingBackOrder
        {
            get
            {
                return _processingBackOrder;
            }

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

        public const string CanSellInBulkPropertyName = "CanSellInBulk";
        private bool _canSellInBulk = false;
        public bool CanSellInBulk
        {
            get
            {
                return _canSellInBulk;
            }

            set
            {
                if (_canSellInBulk == value)
                {
                    return;
                }

                var oldValue = _canSellInBulk;
                _canSellInBulk = value;
                RaisePropertyChanged(CanSellInBulkPropertyName);
            }
        }

        //public const string SellInUnitsPropertyName = "SellInUnits";
        //private bool _sellInUnits = false;
        //public bool SellInUnits
        //{
        //    get
        //    {
        //        return _sellInUnits;
        //    }

        //    set
        //    {
        //        if (_sellInUnits == value)
        //        {
        //            return;
        //        }

        //        var oldValue = _sellInUnits;
        //        _sellInUnits = value;
        //        RaisePropertyChanged(SellInUnitsPropertyName);
        //    }
        //}

        //public const string SellInBulkPropertyName = "SellInBulk";
        //private bool _sellInBulk = false;
        //public bool SellInBulk
        //{
        //    get
        //    {
        //        return _sellInBulk;
        //    }

        //    set
        //    {
        //        if (_sellInBulk == value)
        //        {
        //            return;
        //        }

        //        var oldValue = _sellInBulk;
        //        _sellInBulk = value;
        //        RaisePropertyChanged(SellInBulkPropertyName);
        //    }
        //}

        public const string SellInUnitsPropertyName = "SellInUnits";
        private bool _sellInUnits = true;
        public bool SellInUnits
        {
            get
            {
                return _sellInUnits;
            }

            set
            {
                if (_sellInUnits == value)
                {
                    return;
                }

                var oldValue = _sellInUnits;
                _sellInUnits = value;
                //_sellInBulk = !_sellInUnits;
                RaisePropertyChanged(SellInUnitsPropertyName);
            }
        }

        public const string SellInBulkPropertyName = "SellInBulk";
        private bool _sellInBulk = false;
        public bool SellInBulk
        {
            get
            {
                return _sellInBulk;
            }

            set
            {
                if (_sellInBulk == value)
                {
                    return;
                }

                var oldValue = _sellInBulk;
                _sellInBulk = value;
                RaisePropertyChanged(SellInBulkPropertyName);
            }
        }

        public const string CapacityValueAppliedPropertyName = "CapacityValueApplied";
        private bool _capacityValueApplied = false;
        public bool CapacityValueApplied
        {
            get
            {
                return _capacityValueApplied;
            }

            set
            {
                if (_capacityValueApplied == value)
                {
                    return;
                }

                var oldValue = _capacityValueApplied;
                _capacityValueApplied = value;
                RaisePropertyChanged(CapacityValueAppliedPropertyName);
            }
        }

        public const string CrateCapacityPropertyName = "CrateCapacity";
        private int _crateCapacity = 1;
        public int CrateCapacity
        {
            get
            {
                return _crateCapacity;
            }

            set
            {
                if (_crateCapacity == value)
                {
                    return;
                }

                var oldValue = _crateCapacity;
                _crateCapacity = value;
                RaisePropertyChanged(CrateCapacityPropertyName);
            }
        }

        public const string RequiredQtyPropertyName = "RequiredQty";
        private decimal _requiredQty = 1;
        public decimal RequiredQty
        {
            get
            {
                return _requiredQty;
            }

            set
            {
                if (_requiredQty == value)
                {
                    return;
                }

                var oldValue = _requiredQty;
                _requiredQty = value;
                RaisePropertyChanged(RequiredQtyPropertyName);
            }
        }

        public const string ReturnableIdPropertyName = "ReturnableId";
        private Guid _returnableId = Guid.Empty;
        public Guid ReturnableId
        {
            get
            {
                return _returnableId;
            }

            set
            {
                if (_returnableId == value)
                {
                    return;
                }

                var oldValue = _returnableId;
                _returnableId = value;
                RaisePropertyChanged(ReturnableIdPropertyName);
            }
        }

        public const string ReceiveReturnablesPropertyName = "ReceiveReturnables";
        private bool _receiveReturnables = false;
        public bool ReceiveReturnables
        {
            get
            {
                return _receiveReturnables;
            }

            set
            {
                if (_receiveReturnables == value)
                {
                    return;
                }

                var oldValue = _receiveReturnables;
                _receiveReturnables = value;
                RaisePropertyChanged(ReceiveReturnablesPropertyName);
            }
        }

        public const string SellFreeOfChargePropertyName = "SellFreeOfCharge";
        private bool _sellFreeOfCharfe = false;
        public bool SellFreeOfCharge
        {
            get
            {
                return _sellFreeOfCharfe;
            }

            set
            {
                if (_sellFreeOfCharfe == value)
                {
                    return;
                }

                var oldValue = _sellFreeOfCharfe;
                _sellFreeOfCharfe = value;
                RaisePropertyChanged(SellFreeOfChargePropertyName);
            }
        }
        #endregion
    }

        #region Helper Classes
        public class OrderLineItemProductLookupItem
        {
            public Guid ProductId { get; set; }
            public string ProductDesc { get; set; }
            public string ProductCode { get; set; }
            public Guid PackagingId { get; set; }
            public Guid PackagingTypeId { get; set; }
            public string ProductType { get; set; }
            public decimal LineItemQty { get; set; }
            public bool HasReturnable { get; set; }
            public Guid MyReturnableProducrId { get; set; }
            public decimal AvailableInventory { get; set; }
        }

        public class LineItem
        {
            public Guid ProductId { get; set; }
            public string ProductDesc { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal LineItemVatValue { get; set; }
            public decimal LiTotalNet { get; set; }
            public decimal VatAmount { get; set; }
            public decimal Vat { get; set; }
            public decimal Qty { get; set; }
            public decimal TotalPrice { get; set; }
            public string ProductType { get; set; }
            public bool IsNew { get; set; }
            public Guid ReturnableId { get; set; }
            public int SequenceNo { get; set; }
            public int Capacity { get; set; }

            public decimal Approve { get; set; }
            public decimal BackOrder { get; set; }
            public decimal LostSale { get; set; }
        }
        #endregion
}