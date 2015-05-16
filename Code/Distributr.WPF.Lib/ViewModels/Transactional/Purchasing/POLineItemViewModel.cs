using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.WorkFlow.Discount;
using Distributr.WPF.Lib.UI.Hierarchy;
using GalaSoft.MvvmLight.Command;
using System.Linq;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Purchasing
{
    public class POLineItemViewModel : DistributrViewModelBase
    {
        
        public POLineItemViewModel()
        {
            
            ProductSelected = new RelayCommand(RunProductSelected);
            ClearAndSetup = new RelayCommand(RunClearAndSetup);
            Setup = new RelayCommand(RunSetup);
            MultipleProduct = new List<MultipleAddProduct>();
            ProductSummaryCommand = new RelayCommand(ProductSummaryView);
           
            _ProductAddSummaries = new List<ProductAddSummary>();
            Products = new ObservableCollection<POLineItemProductLookupItem>();
           
            
        }

        #region Properties


        public const string ProductAddSummariesPropertyName = "ProductAddSummaries";

        private List<ProductAddSummary> _ProductAddSummaries;

        public RelayCommand Setup { get; set; }
        public RelayCommand ProductSelected { get; set; }
        public RelayCommand ClearAndSetup { get; set; }
        public RelayCommand ProductSummaryCommand { get; set; }

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

              

                // Update bindings, no broadcast
                RaisePropertyChanged(IsAddPropertyName);

              
            }
        }

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

              

                // Update bindings, no broadcast
                RaisePropertyChanged(ButtonNameTextPropertyName);

                
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
                    return;
                var oldValue = _selectedProduct;
                _selectedProduct = value;
                RaisePropertyChanged(SelectedProductPropertyName);
            }
        }

        public ObservableCollection<POLineItemProductLookupItem> Products { get; set; }
        public List<MultipleAddProduct> MultipleProduct { set; get; }
        public const string QtyPropertyName = "Qty";
        private decimal _qty = 1;
        [Range(1,99999999)]
        public decimal Qty
        {
            get
            {
                return _qty;
            }

            set
            {
                try
                {
                    if (_qty == value)
                    {
                        return;
                    }
                    var oldValue = _qty;
                    _qty = value;

                    RaisePropertyChanged(QtyPropertyName);
                    RecalcTotal();
                    if (SelectedProduct != null)
                        BindTree(SelectedProduct.ProductId);
                }
                catch
                {
                    MessageBox.Show("Value was too large.", "Distribut: Order Module", MessageBoxButton.OK);
                    Qty = 0;
                }
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

        public const string VatPropertyName = "Vat";
        private decimal _vat = 0m;
        public decimal Vat
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

        public const string ModalTitlePropertyName = "ModalTitle";
        private string _ModalTitle = "Add Purchase Order Product";
        public string ModalTitle
        {
            get
            {
                return _ModalTitle;
            }

            set
            {
                if (_ModalTitle == value)
                {
                    return;
                }

                var oldValue = _ModalTitle;
                _ModalTitle = value;



                // Update bindings, no broadcast
                RaisePropertyChanged(ModalTitlePropertyName);


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



                // Update bindings, no broadcast
                RaisePropertyChanged(LineItemTypePropertyName);

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



                // Update bindings, no broadcast
                RaisePropertyChanged(IsEnabledPropertyName);


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


        #endregion

        #region Methods

        public void AddProduct(MultipleAddProduct product)
        {
            MultipleProduct.Add(product);
            AddProductSummary(product);
        }

        private void ProductSummaryView()
        {
            string msg = "";
            string group = "";
            if (ProductAddSummaries.Count == 0)
            {
                return;
            }
            foreach (ProductAddSummary ps in ProductAddSummaries.OrderBy(p => p.LineItemType).ToList())
            {
                //if (group != ps.LineItemType.ToString())
                //{
                //    group = ps.LineItemType.ToString();
                //    msg += ps.LineItemType.ToString() + "\n"; 
                //}
                string temp = string.Format("\t{0} of {1} @ {2} = {3}\n", ps.Quantity, ps.ProductName, ps.UnitPrice,
                                            ps.TotalPrice);
                msg += temp;
            }

            MessageBox.Show(msg, "Distributr: Purchase Order add summary", MessageBoxButton.OK);
        }

        void RunSetup()
        {
            using (var container = NestedContainer)
            {
                IProductRepository _productService = Using<IProductRepository>(container);
               
                Products.Clear();
                var Allproduct =
                    _productService.GetAll().Where(n => n is ConsolidatedProduct || n is SaleProduct).OrderBy(
                        o => o.Description);
                Allproduct.ToList().ForEach(n => Products.Add(new POLineItemProductLookupItem
                                                                  {
                                                                      ProductId = n.Id,
                                                                      ProductDesc = n.Description,
                                                                      ProductCode = n.ProductCode
                                                                  }));
            }
        }

        void RunProductSelected()
        {
            using (var container = NestedContainer)
            {
                IProductRepository _productService = Using<IProductRepository>(container);
                
                if (SelectedProduct != null)
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

        public void PriceCalc(Product product)
        {
            //TODO: find out how tier is resolved
            // ProductPricingTier tier = _productPricingService.GetAll().FirstOrDefault().Tier;

            if (product is ConsolidatedProduct)
                try
                {
                    UnitPrice = product.ExFactoryPrice;//((ConsolidatedProduct) product).ProductPrice(tier);
                }
                catch
                {
                    UnitPrice = 0m;
                }
            else
                try
                {
                    UnitPrice = product.ExFactoryPrice;//product.ProductPricings.First(n => n.Tier.Id == tier.Id).CurrentSellingPrice;
                }
                catch
                {
                    UnitPrice = 0m;
                }

            //if (product is ConsolidatedProduct)
            //    LineItemVatValue = ((ConsolidatedProduct)product).ProductVATRate(tier);
            //else
            //{
            if (product.VATClass != null)
                LineItemVatValue = product.VATClass.CurrentRate;
            else
                LineItemVatValue = 0;
            //}
            RecalcTotal();
            BindTree(product);
        }

        public decimal ProductVatCalc(Product product)
        {
            if (product is ReturnableProduct)
                return 0;
            //if (product is ConsolidatedProduct)
            //    return ((ConsolidatedProduct) product).ProductVATRate(_productPricingService.GetAll().FirstOrDefault().Tier);
            //else
            //{
            if (product.VATClass != null)
                return product.VATClass.CurrentRate;
            //}
            return 0;
        }

        public decimal ProductPriceCalc(Product product)
        {
            using (var container = NestedContainer)
            {
                
                var _productPricingService = Using<IProductPricingRepository>(container);
                ProductPricingTier tier = _productPricingService.GetAll().FirstOrDefault().Tier;
                decimal prodprice = 0;
                if (product is ConsolidatedProduct)
                    try
                    {
                        prodprice = product.ExFactoryPrice; // ((ConsolidatedProduct)product).TotalExFactoryValue(tier);
                    }
                    catch
                    {
                        prodprice = 0m;
                    }
                else
                    try
                    {
                        prodprice = product.ExFactoryPrice;
                            // product.TotalExFactoryValue(tier);//.First(n => n.Tier.Id == tier.Id).CurrentExFactory;
                    }
                    catch
                    {
                        prodprice = 0m;
                    }


                return prodprice;
            }
        }

        void BindTree(Guid productId)
        {
            using (var container = NestedContainer)
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

        private void RunClearAndSetup()
        {
            Qty = 1;
            UnitPrice = 0;
            VatAmount = 0;
            TotalPrice = 0;
            ProductTree = null;
            RunSetup();
            _productPackagingSummaryService.ClearBuffer();
        }

        public void RecalcTotal()
        {
           // UnitPrice = 0; WE NEED NOT INCREMENT UNIT PRICE
            VatAmount = 0;
            TotalPrice = 0;
            if (SelectedProduct != null)
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
                   // UnitPrice += productCost;
                    VatAmount += productVat;
                    TotalPrice = productCost + VatAmount;
                }

            }
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
                ProductAddSummaries.FirstOrDefault(x => x.ProductId == productId );
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

        public void LoadForEdit(Guid selectedProductId, decimal unitPrice, decimal lineItemVatValue, decimal totalPrice,
                                decimal vatAmount, int sequenceNo, decimal quantity)
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

        #endregion
    }

    #region Other Classes
    public class POLineItemProductLookupItem
    {
        public Guid ProductId { get; set; }
        public string ProductDesc { get; set; }
        public int ProductType { get; set; }
        public string ProductCode { get; set; }
    }

      public class MultipleAddProduct
      {

          public POLineItemProductLookupItem Product { get; set; }
          public decimal Quantity { get; set; }
          public decimal UnitPrice { get; set; }
          public decimal Vat { get; set; }
          public decimal TotalPrice { get; set; }
          public decimal VatAmount { get; set; }
          public decimal LineItemVatValue { get; set; }
          public LineItemType LineItemType { get; set; }
      }

      public class ProductAddSummary
      {
          public Guid ProductId { get; set; }
          public string ProductName { get; set; }
          public decimal UnitPrice { get; set; }
          public decimal TotalPrice { get; set; }
          public decimal Quantity { get; set; }
          public LineItemType LineItemType { get; set; }
          public Guid ParentProductId { get; set; }
          public bool IsEditable { get; set; }
          public bool IsFreeOfCharge { get; set; }
      }
    #endregion
}
