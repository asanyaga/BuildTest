using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.WorkFlow.Discount;
using Distributr.WPF.Lib.UI.Hierarchy;
using Distributr.WPF.Lib.ViewModels.Transactional.Purchasing;
using Distributr.WPF.Lib.ViewModels.Transactional.SalesmanOrders;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Linq;

namespace Distributr.WPF.Lib.ViewModels.Transactional.POS
{
    public class POSLineItemViewModel : DistributrViewModelBase
    {
        public List<OrderLineItemProductLookupItem> AddedLineItems;

        public POSLineItemViewModel()
        {
            AddedLineItems = new List<OrderLineItemProductLookupItem>();
            ProductSelected = new RelayCommand(RunProductSelected);
            ClearAndSetupCommand = new RelayCommand(RunClearAndSetup);
            LineItems = new ObservableCollection<LineItem>();
            MultipleProduct = new List<MultipleAddProduct>();
            LoadedProducts = new List<Product>();
            Products = new ObservableCollection<POLineItemProductLookupItem>();
            AvailableInventory = new List<Inventory>();
        }

        public RelayCommand ProductSelected { get; set; }
        public RelayCommand SellInBulkSelected { get; set; }
        public RelayCommand SellInUnitsSelected { get; set; }
        public Dictionary<Guid, decimal> ReturnableIn { get; set; }
        public LineItem SaleProduct;
        public LineItem BulkSaleContainer;//e.g crate
        public LineItem SaleProductReturnable; //e.g bottle
        public LineItem UiProduct; //presentational

        public decimal MyAvailableInvAtEdit = 0;

        public void LoadForEdit(Guid selectedProductId, decimal unitPrice,
                                decimal lineItemVatValue, decimal totalPrice, decimal vatAmount, int sequenceNo,
                                decimal quantity)
        {
            Setup(true, selectedProductId);
            SelectedProduct = Products.First(n => n.ProductId == selectedProductId);
            _productPackagingSummaryService.ClearBuffer();
            var product = GetEntityById(typeof (Product), selectedProductId) as Product;
            Inventory productInv;
            var ccId = GetConfigParams().CostCentreId;
            using (StructureMap.IContainer cont = NestedContainer)
            {
                productInv = Using<IInventoryRepository>(cont).GetByProductIdAndWarehouseId(SelectedProduct.ProductId, ccId);
            }
            AvailableProductInv = productInv == null ? 0 : productInv.Balance;
            MyAvailableInvAtEdit = productInv == null ? 0 : productInv.Balance;
            AvailableUnits = AvailableProductInv;
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
    
        void BindTree(Product product)
        {
            if (product is ConsolidatedProduct)
            {
                ProductTree = ConsolidatedProductHierarchyHelper.GetConsolidatedProductForTreeview((ConsolidatedProduct)product, Qty);
            }
            else
                ProductTree = null;
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
                    UnitVat += (productUnitVat*productUnitPrice);
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
            UpdateAvailableProductInv(productId, quantity);
          
        }

        public void PriceCalc(Product product)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                UnitPrice =Using<IDiscountProWorkflow>(cont).GetUnitPrice(product, SelectedOutlet.OutletProductPricingTier);

                LineItemVatValue = Using<IDiscountProWorkflow>(cont).GetVATRate(product, SelectedOutlet);

                RecalcTotal();
                BindTree(product);
            }
        }

        public decimal ProductLineItemTypeVatCalc(Product product)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                return Using<IDiscountProWorkflow>(cont).GetVATRate(product, SelectedOutlet);
            }
        }

        public decimal ProductPriceCalc(Product product)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                return Using<IDiscountProWorkflow>(cont).GetUnitPrice(product, SelectedOutlet.OutletProductPricingTier);
            }
        }

        public decimal ProductVatCalc(Product product)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                return Using<IDiscountProWorkflow>(cont).GetVATRate(product, SelectedOutlet);
            }
        }

        public RelayCommand ClearAndSetupCommand { get; set; }

        public void ClearViewModel()
        {
            Qty = 1;
            UnitPrice = 0;
            UnitVat = 0;
            VatAmount = 0;
            TotalPrice = 0;
            UnitVat = 0;
            LiTotalNet = 0;
            SaleProduct = null;
            BulkSaleContainer = null; //e.g crate
            SaleProductReturnable = null; //e.g bottle
            UiProduct = null;
            ProductTree = null;
            AvailableProductInv = 0;
            AvailableUnits = 0;
            OriginalQty = 0;
            LineItemType = LineItemType.Unit;
            SellFreeOfCharge = false;
            mesCn = 0;
            _productPackagingSummaryService.ClearBuffer();

            foreach (var item in AddedLineItems)
            {
                Inventory inv = AvailableInventory.FirstOrDefault(n => n.Product.Id == item.ProductId);
                if (inv != null)
                {
                    if ((inv.Balance - item.LineItemQty) <= 0)
                    {
                        var product = Products.FirstOrDefault(n => n.ProductId == item.ProductId);
                        Products.Remove(product);
                    }
                }
            }

            if (ReturnableIn != null)
            {
                foreach (var item in ReturnableIn)
                {
                    if (item.Value <= 0)
                    {
                        var product = Products.FirstOrDefault(n => n.ProductId == item.Key);
                        Products.Remove(product);
                    }
                }
            }
        }

        public void RunClearAndSetup()
        {
            RunClear();
            Setup();
        }

        public void RunClear()
        {
            Qty = 1;
            UnitPrice = 0;
            UnitVat = 0;
            VatAmount = 0;
            TotalPrice = 0;
            UnitVat = 0;
            LiTotalNet = 0;
            SaleProduct = null;
            BulkSaleContainer = null; //e.g crate
            SaleProductReturnable = null; //e.g bottle
            UiProduct = null;
            ProductTree = null;
            AvailableProductInv = 0;
            AvailableUnits = 0;
            OriginalQty = 0;
            LineItemType = LineItemType.Unit;
            SellFreeOfCharge = false;
            mesCn = 0;
            Products.Clear();
            _productPackagingSummaryService.ClearBuffer();
        }


        public void RunProductSelected()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                Inventory productInv = null;
                Product product = null;
                var ccId = Using<IConfigService>(cont).Load().CostCentreId;

                if (SelectedProduct != null && SelectedProduct.ProductId != Guid.Empty)
                {
                    //product = _productService.GetByMasterId(SelectedProduct.ProductId);
                    product = LoadedProducts.FirstOrDefault(n => n.Id == SelectedProduct.ProductId);
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
                    //cn : override for receive returnables
                    if (product is ReturnableProduct)
                    {
                        if (((ReturnableProduct) product).Capacity > 1)
                        {
                            LineItemType = LineItemType.Unit;
                            IsEnabled = false;
                        }
                        else
                            IsEnabled = true;
                    }
                }

                if (!ReceiveReturnables)
                {
                    //var product = LoadedProducts.FirstOrDefault(n => n.Id == SelectedProduct.ProductId);
                    if (SelectedProduct != null && SelectedProduct.ProductId != Guid.Empty)
                        //productInv = _inventoryService.GetByProductId(SelectedProduct.ProductId, ccId);
                        productInv = AvailableInventory.FirstOrDefault(n => n.Product.Id == SelectedProduct.ProductId);

                    AvailableProductInv = productInv == null ? 0 : productInv.Balance;

                    if (SelectedProduct != null && SelectedProduct.ProductId != Guid.Empty && AddedLineItems != null &&
                        AddedLineItems.Count > 0)
                    {
                        try
                        {
                            var item = AddedLineItems.First(n => n.ProductId == SelectedProduct.ProductId);
                            if (ProductTypeToLoad != ProducTypeToLoad.Returnables)
                                AvailableProductInv -= item.LineItemQty;
                        }
                        catch (Exception)
                        {
                        }
                    }
                    var qty = LineItems.Where(n => n.ProductId == SelectedProduct.ProductId).Sum(n => n.Qty);
                    if (!IsNew)
                        AvailableProductInv = MyAvailableInvAtEdit - qty;
                    else
                        AvailableProductInv -= qty;
                }
                else
                {
                    AvailableProductInv = ReturnableIn.FirstOrDefault(n => n.Key == SelectedProduct.ProductId).Value;
                }

                AvailableUnits = AvailableProductInv;

                PriceCalc(product);
                CanSellInBulk = !(product is ConsolidatedProduct);
                if (!CanSellInBulk)
                    LineItemType = UI.Hierarchy.LineItemType.Unit;
                ChangeAvailableQty();
            }
        }

        public void ChangeAvailableQty()
        {
            if (LineItemType == UI.Hierarchy.LineItemType.Unit)
                AvailableProductInv = AvailableUnits;
            else if (LineItemType == UI.Hierarchy.LineItemType.Bulk)
            {
                GetCrateCapacity();
                AvailableProductInv = (int) (AvailableUnits/CrateCapacity);
            }
        }
        void UpdateAvailableProductInv(Guid productId, decimal qty)
        {
            if (ReceiveReturnables)
            {
                Guid id = ReturnableIn.FirstOrDefault(n => n.Key == productId).Key;
                var newQty = ReturnableIn.FirstOrDefault(n => n.Key == productId).Value - qty;
                ReturnableIn.Remove(id);
                ReturnableIn.Add(id, newQty);
            }
            else
            {
                var prod = LoadedProducts.FirstOrDefault(n => n.Id == productId);
                if (prod != null)
                {
                    //AvailableInventory.FirstOrDefault(n => n.Product.Id == productId).Balance -= qty;
                }
                var existingAddedLineItem = AddedLineItems.FirstOrDefault(n => n.ProductId == productId);
                if (existingAddedLineItem != null)
                {
                    existingAddedLineItem.LineItemQty += qty;
                }
                else
                {
                    AddedLineItems.Add(new OrderLineItemProductLookupItem
                                           {
                                               ProductId = productId,
                                               LineItemQty = qty
                                           });
                }
            }
        }

        void GetCrateCapacity()
        {
            CrateCapacity = 1;
            if (SelectedProduct != null)
            {
                if (SelectedProduct.ProductId != Guid.Empty)
                {
                    CrateCapacity = GetMyReturnable().Capacity;
                }
            }
        }

        private ReturnableProduct GetMyReturnable()
        {
            var returnable =
                _productPackagingSummaryService.GetProductBulkReturnable(
                    LoadedProducts.FirstOrDefault(n => n.Id == SelectedProduct.ProductId));
            return returnable;
        }

        private int mesCn = 0;

        public void Setup(bool loadForEditing = false, Guid? productId = null)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                //SellInUnits = true;
                Products.Clear();
                AvailableInventory.Clear();
                var product = new POLineItemProductLookupItem
                                  {
                                      ProductId = Guid.Empty,
                                      ProductDesc = "--Please Select a Product--"
                                  };
                Products.Add(product);
                SelectedProduct = product;
                if (loadForEditing)
                {
                    var editproduct = Using<IProductRepository>(cont).GetById(productId.Value);
                    LoadedProducts = new List<Product> {editproduct};
                    Products.Add(new POLineItemProductLookupItem
                                     {
                                         ProductId = editproduct.Id,
                                         ProductDesc = editproduct.Description,
                                         ProductCode = editproduct.ProductCode
                                     });
                }
                else
                {
                    switch (ProductTypeToLoad)
                    {
                        case ProducTypeToLoad.AllProducts:
                            LoadedProducts.Clear();
                            AvailableInventory = Using<IInventoryRepository>(cont).GetByWareHouseId(Using<IConfigService>(cont).Load().CostCentreId)
                                                                  .Where(n => n.Balance > 0).ToList();

                            AvailableInventory.OrderBy(n => n.Product.Description).ToList()
                                              .ForEach(n => LoadedProducts.Add(n.Product));
                            LoadedProducts.Where(n => !Using<IDiscountProWorkflow>(cont).IsProductFreeOfCharge(n.Id))
                                          .ToList()
                                          .ForEach(n => Products.Add(new POLineItemProductLookupItem
                                                                         {
                                                                             ProductId = n.Id,
                                                                             ProductDesc = n.Description,
                                                                             ProductCode = n.ProductCode
                                                                         }));
                            break;
                        case ProducTypeToLoad.NonReturnables:
                            LoadedProducts.Clear();

                            AvailableInventory = Using<IInventoryRepository>(cont).GetByWareHouseId(Using<IConfigService>(cont).Load().CostCentreId)
                                                                  .Where(
                                                                      n =>
                                                                      n.Product.GetType() != typeof (ReturnableProduct))
                                                                  .Where(n => n.Balance > 0)
                                                                  .OrderBy(n => n.Product.Description).ToList();
                            AvailableInventory.ForEach(n => LoadedProducts.Add(n.Product));
                            LoadedProducts.Where(n => !Using<IDiscountProWorkflow>(cont).IsProductFreeOfCharge(n.Id))
                                          .ToList()
                                          .ForEach(n => Products.Add(new POLineItemProductLookupItem
                                                                         {
                                                                             ProductId = n.Id,
                                                                             ProductDesc = n.Description,
                                                                             ProductCode = n.ProductCode
                                                                         }));
                            break;
                        case ProducTypeToLoad.Returnables:
                            LoadedProducts.Clear();
                            Using<IProductRepository>(cont).GetAll()
                                           .OfType<ReturnableProduct>()
                                           .Where(p => ReturnableIn.ContainsKey(p.Id))
                                           .OrderBy(n => n.Description).ToList().ForEach(n => LoadedProducts.Add(n));
                            LoadedProducts.ToList().ForEach(n => Products.Add(new POLineItemProductLookupItem
                                                                                  {
                                                                                      ProductId = n.Id,
                                                                                      ProductDesc = n.Description,
                                                                                      ProductCode = n.ProductCode
                                                                                  }));
                            break;
                    }
                }
                if (ProductTypeToLoad == ProducTypeToLoad.Returnables)
                    ModalCrumbs = "Distributr: Receive Returnable Products from " + Salesman.Username + "";
                else if (ProductTypeToLoad == ProducTypeToLoad.NonReturnables)
                {
                    if (Products.Count == 1 && mesCn == 0)
                    {
                        MessageBox.Show("Adjust product inventory to load Product");
                        mesCn += 1;
                    }
                }

                CapacityValueApplied = false;
            }
        }

        public void ToggleFreeOfChargeProducts(bool loadFreeOfCharge)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                if (loadFreeOfCharge)
                {
                    LoadedProducts = Using<IDiscountProWorkflow>(cont).ReturnFreeOfChargeProducts(LoadedProducts);
                    var toRemove =
                        Products.Where(
                            n => n.ProductId != Guid.Empty && !LoadedProducts.Select(p => p.Id).Contains(n.ProductId))
                                .ToList();

                    toRemove.ForEach(n => Products.Remove(n));
                    LoadedProducts.OrderBy(n => n.Description)
                                  .ToList()
                                  .ForEach(n => Products.Add(new POLineItemProductLookupItem
                                                                 {
                                                                     ProductId = n.Id,
                                                                     ProductDesc = n.Description,
                                                                     ProductCode = n.ProductCode
                                                                 }));
                }
                else
                {
                    Setup();
                }
            }
        }

        public ObservableCollection<LineItem> LineItems { get; set; }
        public List<MultipleAddProduct> MultipleProduct { set; get; }

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

        public List<Product> LoadedProducts { get; set; }
        public List<Inventory> AvailableInventory { get; set; }
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

        /// <summary>
        /// The <see cref="AvailableUnits" /> property's name.
        /// </summary>
        public const string AvailableUnitsPropertyName = "AvailableUnits";
        private decimal _availableUnits;
        /// <summary>
        /// Gets the AvailableUnits property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public decimal AvailableUnits
        {
            get
            {
                return _availableUnits;
            }

            set
            {
                if (_availableUnits == value)
                {
                    return;
                }

                var oldValue = _availableUnits;
                _availableUnits = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(AvailableUnitsPropertyName);
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

        public const string ProductTreePropertyName = "ProductTree";
        private IEnumerable<HierarchyNode<ProductViewer>> _productTree;
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
                using (StructureMap.IContainer cont = NestedContainer)
                {
                    return Using<ICostCentreRepository>(cont).GetById(SelectedOutletId) as Outlet;
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
        //        //CalcLostSale();
        //        //RecalcTotal(_backOrder);
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
        //        //RecalcTotal(_lostSale);
        //        BindTree(SelectedProduct.ProductId);
        //    }
        //}

        public const string OriginalQtyPropertyName = "OriginalQty";
        private decimal _originalQty = 0;
        public decimal OriginalQty
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
        //private decimal _approve = 0;
        //public decimal Approve
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
        //        //CalcLostSale();
        //        //RecalcTotal(_awaitingStock);
        //        //CalcAwaitingStcok();
        //        BindTree(SelectedProduct.ProductId);
        //    }
        //}

        //public const string SellInUnitsPropertyName = "SellInUnits";
        //private bool _sellInUnits = true;
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
        //        _sellInBulk = !_sellInUnits;
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

        public const string SelectedBulkSaleLevelPropertyName = "SelectedBulkSaleLevel";
        private int _selectedBulkSaleLevel = 1;
        public int SelectedBulkSaleLevel
        {
            get
            {
                return _selectedBulkSaleLevel;
            }

            set
            {
                if (_selectedBulkSaleLevel == value)
                {
                    return;
                }

                var oldValue = _selectedBulkSaleLevel;
                _selectedBulkSaleLevel = value;
                RaisePropertyChanged(SelectedBulkSaleLevelPropertyName);
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
    }

    #region Helper Classes

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
        public bool SellInBulk { get; set; }
    }
    #endregion
}
