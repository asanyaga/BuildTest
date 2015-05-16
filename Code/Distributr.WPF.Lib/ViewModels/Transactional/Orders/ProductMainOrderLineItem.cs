using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.SuppliersEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Hierarchy;
using Distributr.WPF.Lib.UI.Pages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Orders
{
    public class ProductMainOrderLineItem : ProductMainOrderLineItemBase
    {
        public bool allowQuantityChangeEvent;
        public RelayCommand ProductMainOrderLineItemLoadCommand { get; set; }
        public RelayCommand IsFreeOfChargeCheckedCommand { get; set; }
        public RelayCommand<TextCompositionEventArgs> ValidNumericInputCommand { get; set; }
        public RelayCommand<TextChangedEventArgs> ValidQuantityCommand { get; set; }
        
        public RelayCommand<object> ProductDropDownOpenedCommand { get; set; }
        public RelayCommand ProductLookUpOpenedCommand { get; set; }
        public RelayCommand CalculateSummaryCommand { get; set; }
        public RelayCommand<string> SellModeChangedCommand { get; set; }
        public ObservableCollection<Product> ProductLookup { get; set; }
        private List<ProductPopUpItem> addedProduct;
        private List<ProductPopUpItem> Existing;
        private Dictionary<Product, int> returns;
        public RelayCommand<object> AddProductCommand { get; set; }
        public RelayCommand DoneAddingCommand { get; set; }
        public RelayCommand ShowAddedProductsCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public event EventHandler RequestClose = (s, e) => { };
        public ProductMainOrderLineItem()
        {
            ProductMainOrderLineItemLoadCommand = new RelayCommand(Load);
            IsFreeOfChargeCheckedCommand = new RelayCommand(IsFreeOfChargeChecked);
            ProductLookup = new ObservableCollection<Product>();
            ProductDropDownOpenedCommand = new RelayCommand<object>(ProductDropDownOpened);
            
            ValidNumericInputCommand = new RelayCommand<TextCompositionEventArgs>(ValidNumericInput);
            CalculateSummaryCommand = new RelayCommand(CalculateSummary);
            addedProduct = new List<ProductPopUpItem>();
            AddProductCommand = new RelayCommand<object>(AddProduct);
            DoneAddingCommand = new RelayCommand(DoneAdding);
            CancelCommand = new RelayCommand(Cancel);
            SellModeChangedCommand = new RelayCommand<string>(SellModeChanged);
            ShowAddedProductsCommand=new RelayCommand(ShowAddedProductsSummary);
            ProductLookUpOpenedCommand=new RelayCommand(ProductLookUpOpened);
            Existing= new List<ProductPopUpItem>();
            returns = new Dictionary<Product, int>();
            ValidQuantityCommand = new RelayCommand<TextChangedEventArgs>(ValidQuantity);
           
        }

       

        private void ShowAddedProductsSummary()
        {
            if(addedProduct.Any())
            {
                var message = new StringBuilder();
                int count = 1;
                  
                foreach (var product in addedProduct)
                {
                    var total = GetQuickAggretarePriceForAddedProduct(product.Product);
                    message.AppendLine("(" + count + ".)" + product.Product.Description + "\tQuantity:" +
                                       product.Quantity +"\t"+ "Total Price:"+ (total*product.Quantity).ToString("0.00"));
                    message.AppendLine("---------------------------------------------------------------");
                    count++;
                }
                MessageBox.Show(message.ToString(), "Added products", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                

            }

        }


        private void Cancel()
        {
            RequestClose(this, EventArgs.Empty);
        }
        public override void Cleanup()
        {
            SimpleIoc.Default.Unregister <ProductMainOrderLineItem>();
            SimpleIoc.Default.Register<ProductMainOrderLineItem>();
            base.Cleanup();
        }
        private void SellModeChanged(string obj)
        {
            LineItemType = (LineItemType)Enum.Parse(typeof(LineItemType), obj);
            CalculateSummary();
        }

        private void DoneAdding()
        {
            string message = String.Empty;
            if (ValidateAddProduct(out message) && ValidateAvailabilty(out message))
            {
                AddProduct(null);
            }
            this.RequestClose(this, EventArgs.Empty);
        }
        
        private void AddProduct(object obj)
        {
            string message = String.Empty;
            if (!ValidateAddProduct(out message))
            {
                MessageBox.Show(message);
                return;
            }
            if (!ValidateAvailabilty(out message))
            {
                MessageBox.Show(message);
                return;
            }
           
            decimal bulkQuantity = 1;
            if (LineItemType == LineItemType.Bulk)
            {
                using (var container = NestedContainer)
                {
                    bulkQuantity = Using<IProductPackagingSummaryService>(container).GetProductQuantityInBulk(SelectedProduct.Id);
                }
               
            }
            bool isBulk = LineItemType == LineItemType.Bulk;
            if (addedProduct.Any(s => s.Product.Id == SelectedProduct.Id))
            {
                var item = addedProduct.First(s => s.Product.Id == SelectedProduct.Id);
                item.Quantity = isBulk? Quantity*bulkQuantity:Quantity;
                item.IsFreeOfCharge = IsFreeOfChargeSale;

            }
            else
            {
                addedProduct.Add(new ProductPopUpItem
                                     {
                                         Quantity = isBulk ? Quantity*bulkQuantity : Quantity,
                                         Product = SelectedProduct,
                                         IsFreeOfCharge = IsFreeOfChargeSale
                                     });
            }
            ShowAddedProductsLink=Visibility.Visible;
            RefreshViewModel();
            if(obj is TextBox)
            {
                allowQuantityChangeEvent = false;
                ((TextBox) obj).Text = "1";
            }

        }
        private bool ValidateAddProduct(out string message)
        {
            string msg = String.Empty;
            if (_selectedProduct == null || SelectedProduct.Id == Guid.Empty)
                msg += "\tProduct is required !\n";
            if (Quantity<=0)
                msg += "\tValid Quantity is required !\n";
            if (msg != String.Empty)
            {
                msg+="Provide \n" + msg;
                message = msg;
                return false;
            }
            message = msg;
            return true;
        }
        private void ValidQuantity(TextChangedEventArgs e)
        {
            if (SelectedProduct == null || SelectedProduct.Id == Guid.Empty)
            {
                MessageBox.Show("Select product to continue....");
                e.Handled = false;
            }

            else
            {
               
                    CalculateSummary();
              
            }
        }
        private void ValidNumericInput(TextCompositionEventArgs e)
        {
            if (SelectedProduct == null || SelectedProduct.Id == Guid.Empty)
            {
                MessageBox.Show("Select product to continue....");
                e.Handled = false;
            }
            
            else
            {
               
                TextBox box = e.OriginalSource as TextBox;
               
                e.Handled = !IsTextAllowed(box.Text);
            }
           
        }

        public static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex(@"^(?=.*[1-9])(\d{0,18})?(?:\.\d{0,2})?$"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        decimal GetQuickAggretarePriceForAddedProduct( Product product)
        {
            using (var container = NestedContainer)
            {
                var summaryService = Using<IProductPackagingSummaryService>(container);
                var pricingService = Using<IDiscountProWorkflow>(container);

                decimal quantity = Quantity;
                decimal bulkQuantity = 1;
                if (LineItemType == LineItemType.Bulk)
                {
                    bulkQuantity = summaryService.GetProductQuantityInBulk(product.Id);
                    quantity = bulkQuantity * quantity;
                }
                List<PackagingSummary> summary = summaryService.GetProductSummaryByProduct(product.Id, quantity);

                foreach (var s in summary)
                {
                    LineItemPricingInfo info;
                    bool isBulk = LineItemType == LineItemType.Bulk && s.Quantity != Quantity;
                    if (SelectedOutlet != null)
                    {
                        info = pricingService.GetLineItemPricing(s, SelectedOutlet.Id);
                    }
                    else
                    {
                        info = pricingService.GetPurchaseLineItemPricing(s);
                    }
                    UnitPrice += isBulk ? info.UnitPrice * bulkQuantity : info.UnitPrice;

                    UnitVAT += isBulk ? info.VatValue * bulkQuantity : info.VatValue; ;
                    TotalAmount += info.TotalPrice;
                    TotalNet += info.TotalNetPrice;
                    TotalVAT += info.TotalVatAmount;
                    GrossAmount += info.TotalPrice;
                    UnitDiscount += isBulk ? info.ProductDiscount * bulkQuantity : info.ProductDiscount;
                    TotalProductDiscount += info.TotalProductDiscount;

                    
                }
                return GrossAmount;
            }
        }

        private void CalculateSummary()
        {
          
            ClearViewModelAmount();
            if (SelectedProduct == null || SelectedProduct.Id == Guid.Empty)
                return;
            string msg;
            if (!ValidateAvailabilty(out msg))
            {
                MessageBox.Show(msg);
                return;
            }
            using (var container = NestedContainer)
            {
               


                var summaryService = Using<IProductPackagingSummaryService>(container);
                var pricingService = Using<IDiscountProWorkflow>(container);
               
                decimal quantity = Quantity;
                decimal bulkQuantity = 1;
                if (LineItemType == LineItemType.Bulk)
                {
                    bulkQuantity = summaryService.GetProductQuantityInBulk(SelectedProduct.Id);
                    quantity=bulkQuantity*quantity;
                }
                List<PackagingSummary> summary = summaryService.GetProductSummaryByProduct(SelectedProduct.Id, quantity);
              
                foreach(var s in summary)
                {
                    LineItemPricingInfo info;
                    bool isBulk = LineItemType == LineItemType.Bulk && s.Quantity!=Quantity;
                    if (SelectedOutlet != null)
                    {
                        info = pricingService.GetLineItemPricing(s, SelectedOutlet.Id);
                    }
                    else
                    {
                        info = pricingService.GetPurchaseLineItemPricing(s);
                    }
                    UnitPrice += isBulk ? info.UnitPrice * bulkQuantity : info.UnitPrice;
                   
                    UnitVAT += isBulk ? info.VatValue * bulkQuantity : info.VatValue;;
                    TotalAmount += info.TotalPrice;
                    TotalNet += info.TotalNetPrice;
                    TotalVAT += info.TotalVatAmount;
                    GrossAmount += info.TotalPrice;
                    UnitDiscount += isBulk ? info.ProductDiscount * bulkQuantity : info.ProductDiscount;
                    TotalProductDiscount += info.TotalProductDiscount;
                }
              
            }
        }

        private bool ValidateAvailabilty(out string  msg)
        {
            msg = "";
            if (OrderType==OrderType.DistributorPOS)
            {
                using (var container = NestedContainer)
                {
                    Guid costcentreId = Using<IConfigService>(container).Load().CostCentreId;
                    var availableInv =
                        Using<IInventoryRepository>(container).GetByProductIdAndWarehouseId(SelectedProduct.Id,
                                                                                            costcentreId);
                    Available = availableInv != null ? availableInv.Balance-Existing.Where(s=>s.Product.Id==SelectedProduct.Id).Sum(s=>s.Quantity) : 0;
                    if (Available < Quantity)
                    {
                        msg = "Required Quantity cant be more than available Quantity";
                        return false;
                    }
                }
            }
            return true;
        }

        void ProductLookUpOpened()
        {
            using (var c = NestedContainer)
            {
                var selectedId = Using<IItemsLookUp>(c).ShowDlg(typeof (Product));
                SelectedProduct = Using<IProductRepository>(c).GetById(selectedId);
                CalculateSummary();
            }
        }

        private void ProductDropDownOpened(object sender)
        {
            using (var container = NestedContainer)
            {
             
                SelectedProduct = Using<IComboPopUp>(container).ShowDlg(ProductLookup) as Product;
                
                if (SelectedProduct is ReturnableProduct)
                {
                    AvailableLabel = "Expected:";
                    Available = returns.Where(p => p.Key.Id == SelectedProduct.Id).Sum(q=>q.Value);
                }
                
            }
        }

        private void IsFreeOfChargeChecked()
        {
            LoadProducts();
            RefreshViewModel();
        }

        private void RefreshViewModel()
        {
            Quantity = 1;
            SelectedProduct = ProductLookup.FirstOrDefault();
            ClearViewModelAmount();
            var addedproductscount = addedProduct.Count;

            AddedProductsCount = string.Format("AddedProducts(" + addedproductscount + ")");

        }

        private void ClearViewModelAmount()
        {
            TotalAmount = 0;
            TotalNet = 0;
            TotalVAT = 0;
            GrossAmount = 0;
            UnitPrice = 0;
            UnitDiscount = 0;
            UnitVAT = 0;
            TotalProductDiscount = 0;
           
        }

        private void Load()
        {
            addedProduct.Clear();
            IsFreeOfChargeSale = false;
           
            LineItemType = LineItemType.Unit;

            SelectedSupplier = null;
        }

        private void LoadProducts()
        {
            ProductLookup.Clear();
            ProductLookup.Add(new SaleProduct(Guid.Empty) {Description = "---Select Product-----"});
            using (var container = NestedContainer)
            {
                var distributrId = Using<IConfigService>(container).Load().CostCentreId;
                List<SaleProduct> products;
                if (OrderType == OrderType.DistributorPOS)
                {
                    if (IsFreeOfChargeSale)
                    {
                        var productsRef =
                            Using<IFreeOfChargeDiscountRepository>(container).GetAll().Where(p=>p.StartDate.Date <= DateTime.Now.Date && p.EndDate.Date >= DateTime.Now.Date).Select(s => s.ProductRef).ToList();
                        foreach (var p in productsRef)
                        {

                            var inventory =
                                Using<IInventoryRepository>(container).GetByProductIdAndWarehouseId(p.ProductId,
                                                                                                    distributrId);
                            if (inventory != null && inventory.Product != null && inventory.Balance > 0)
                            {
                                Product fp = inventory.Product;
                                if (fp._Status == EntityStatus.Active)
                                {
                                    ProductLookup.Add(fp);
                                }
                            }
                        }
                    }
                    else
                    {
                        products =
                            Using<IInventoryRepository>(container).GetByWareHouseId(distributrId).Where(
                                p => p.Balance > 0).Select(p => p.Product).Where(s=>s._Status==EntityStatus.Active)
                                .OfType<SaleProduct>().ToList();
                        products.ForEach(f => ProductLookup.Add(f));
                    }
                }

                else
                {
                    if (IsFreeOfChargeSale)
                    {
                        var productsRef =
                            Using<IFreeOfChargeDiscountRepository>(container).GetAll().Where(p => p.StartDate.Date <= DateTime.Now.Date && p.EndDate.Date >= DateTime.Now.Date && p._Status == EntityStatus.Active).Select(s => s.ProductRef).ToList();
                        foreach (var p in productsRef)
                        {
                            
                            Product fp = Using<IProductRepository>(container).GetById(p.ProductId);
                            if(fp._Status!=EntityStatus.Active)
                                continue;
                            ProductLookup.Add(fp);
                        }
                    }
                    else
                    {
                        products = Using<IProductRepository>(container).GetAll().OfType<SaleProduct>().Where(s=>s._Status==EntityStatus.Active).ToList();
                        products.ForEach(f => ProductLookup.Add(f));
                    }


                }

            }
            if (!IsEdit)
                SelectedProduct = ProductLookup.FirstOrDefault();

            if (ProductLookup.Any())
            {
                var ordered = ProductLookup.OrderBy(p => p.ProductCode).ThenBy(p => p.Description).ToList();
                ProductLookup.Clear();
                foreach (var product in ordered)
                {
                    if (ProductLookup.All(p => p.ProductCode != product.ProductCode))
                        ProductLookup.Add(product);
                }
                
            }

            if (SelectedSupplier != null && SelectedSupplier.Id != Guid.Empty)
            {
             var supplierproduct=   ProductLookup.Where(s=>s.Id !=Guid.Empty &&s.Brand.Supplier.Id==SelectedSupplier.Id).ToList();
             ProductLookup.Clear();
             ProductLookup.Add(new SaleProduct(Guid.Empty) { Description = "---Select Product-----" });
                supplierproduct.ForEach(f => ProductLookup.Add(f));
            }

        }

        public void SetUp(Outlet outlet, OrderType orderType)
        {
            SelectedOutlet = outlet;
            CanChange = true;
            OrderType = orderType;
            LoadProducts();
            AddedProductsCount = "AddedProducts(0)";
            ShowAddedProductsLink=Visibility.Collapsed;
            if(OrderType==OrderType.DistributorPOS)
            {
                IsAvailableVisible = true;
                IsFreeOfChargeVisible = true;
                PackageLabel = "Sell By";
            }
            else if (OrderType ==OrderType.OutletToDistributor)
            {
                IsAvailableVisible = false;
                IsFreeOfChargeVisible = true;
                PackageLabel = "Order By";
            }else
            {
                IsAvailableVisible = false;
                IsFreeOfChargeVisible = false;
                PackageLabel = "Purchase By";
            }
        }
        public void SetUpWithSupplier(Outlet outlet, OrderType orderType,Supplier supplier)
        {
            SelectedOutlet = outlet;
            CanChange = true;
            OrderType = orderType;
            SelectedSupplier = supplier;
            LoadProducts();
            AddedProductsCount = "AddedProducts(0)";
            ShowAddedProductsLink = Visibility.Collapsed;
            if (OrderType == OrderType.DistributorPOS)
            {
                IsAvailableVisible = true;
                IsFreeOfChargeVisible = true;
                PackageLabel = "Sell By";
            }
            else if (OrderType == OrderType.OutletToDistributor)
            {
                IsAvailableVisible = false;
                IsFreeOfChargeVisible = true;
                PackageLabel = "Order By";
            }
            else
            {
                IsAvailableVisible = false;
                IsFreeOfChargeVisible = false;
                PackageLabel = "Purchase By";
            }
        }
        public void SetUpPOS(Outlet outlet, OrderType orderType,List<ProductPopUpItem> existing )
        {
            Existing = existing;
            SelectedOutlet = outlet;
            CanChange = true;
            OrderType = orderType;
            LoadProducts();
            AddedProductsCount = "AddedProducts(0)";
            ShowAddedProductsLink = Visibility.Collapsed;
            if (OrderType == OrderType.DistributorPOS)
            {
                IsAvailableVisible = true;
                IsFreeOfChargeVisible = true;
                PackageLabel = "Sell By";
            }
            else if (OrderType == OrderType.OutletToDistributor)
            {
                IsAvailableVisible = false;
                IsFreeOfChargeVisible = true;
                PackageLabel = "Order By";
            }
            else
            {
                IsAvailableVisible = false;
                IsFreeOfChargeVisible = false;
                PackageLabel = "Purchase By";
            }
        }
        public void LoadForEdit(Guid productId, decimal quantity)
        {
            using(var c =NestedContainer)
            {
                SelectedProduct = Using<IProductRepository>(c).GetById(productId);
                string msg;
                if (!ValidateAvailabilty(out msg))
                {
                    MessageBox.Show(msg);
                    return;
                }
                Quantity = quantity;
                CanChange = false;
                IsEdit = true;
                IsAddButtonVisible = false;
               
            }
           
        }
        public List<ProductPopUpItem> GetProductsLineItem()
        {
            return addedProduct;
        }

     
        public void SetReturnables(Outlet outlet,Dictionary<Guid,int> returnables)
        {
            ProductLookup.Clear();
            ProductLookup.Add(new SaleProduct(Guid.Empty) { Description = "---Select Returnable-----" });
            using (var c = NestedContainer)
            {
               
                   
                foreach (var returnableId in returnables.Select(p=>p.Key))
                {
                    Product p = Using<IProductRepository>(c).GetById(returnableId);
                    ProductLookup.Add(p);

                    returns.Add(p, returnables.FirstOrDefault(q => q.Key == returnableId).Value);
                }
            }
            if (!IsEdit)
                SelectedProduct = ProductLookup.FirstOrDefault();
        }
       

        public const string ShowAddedProductsLinkPropertyName = "ShowAddedProductsLink";
        private Visibility _showAddedProductsLink =Visibility.Collapsed;
        public Visibility ShowAddedProductsLink
        {
            get
            {
                return _showAddedProductsLink;
            }

            set
            {
                if (_showAddedProductsLink == value)
                {
                    return;
                }

                RaisePropertyChanging(ShowAddedProductsLinkPropertyName);
                _showAddedProductsLink = value;
                RaisePropertyChanged(ShowAddedProductsLinkPropertyName);
            }
        }


        public const string SelectedOutletPropertyName = "SelectedOutlet";
        private Outlet _outlet = null;
        public Outlet SelectedOutlet
        {
            get
            {
                return _outlet;
            }

            set
            {
                if (_outlet == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedOutletPropertyName);
                _outlet = value;
                RaisePropertyChanged(SelectedOutletPropertyName);
            }
        }

        public const string SelectedSupplierPropertyName = "SelectedSupplier";
        private Supplier _supplier = null;
        public Supplier SelectedSupplier
        {
            get
            {
                return _supplier;
            }

            set
            {
                if (_supplier == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedOutletPropertyName);
                _supplier = value;
                RaisePropertyChanged(SelectedOutletPropertyName);
            }
        }


        public const string SelectedProductPropertyName = "SelectedProduct";
        private Product _selectedProduct = null;
        public Product SelectedProduct
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

                RaisePropertyChanging(SelectedProductPropertyName);
                _selectedProduct = value;
                RaisePropertyChanged(SelectedProductPropertyName);
            }
        }

        public const string IsFreeOfChargeSalePropertyName = "IsFreeOfChargeSale";
        private bool _isfree = false;
        public bool IsFreeOfChargeSale
        {
            get
            {
                return _isfree;
            }

            set
            {
                if (_isfree == value)
                {
                    return;
                }

                RaisePropertyChanging(IsFreeOfChargeSalePropertyName);
                _isfree = value;
                RaisePropertyChanged(IsFreeOfChargeSalePropertyName);
            }
        }

       
        public const string LineItemTypePropertyName = "LineItemType";
        private LineItemType _type = LineItemType.Unit;
        public LineItemType LineItemType
        {
            get
            {
                return _type;
            }

            set
            {
                if (_type == value)
                {
                    return;
                }

                RaisePropertyChanging(LineItemTypePropertyName);
                _type = value;
                RaisePropertyChanged(LineItemTypePropertyName);
            }
        }

        
        public const string IsEditPropertyName = "IsEdit";
        private bool _isEdit = false;
        public bool IsEdit
        {
            get
            {
                return _isEdit;
            }

            set
            {
                if (_isEdit == value)
                {
                    return;
                }

                RaisePropertyChanging(IsEditPropertyName);
                _isEdit = value;
                RaisePropertyChanged(IsEditPropertyName);
            }
        }

        
        public const string OrderTypePropertyName = "OrderType";
        private OrderType _orderType = OrderType.OutletToDistributor;
        public OrderType OrderType
        {
            get
            {
                return _orderType;
            }

            set
            {
                if (_orderType == value)
                {
                    return;
                }

                RaisePropertyChanging(OrderTypePropertyName);
                _orderType = value;
                RaisePropertyChanged(OrderTypePropertyName);
            }
        }

        
        public const string IsFreeOfChargeVisiblePropertyName = "IsFreeOfChargeVisible";
        private bool _isfreeOfChargeVisible = true;
        public bool IsFreeOfChargeVisible
        {
            get
            {
                return _isfreeOfChargeVisible;
            }

            set
            {
                if (_isfreeOfChargeVisible == value)
                {
                    return;
                }

                RaisePropertyChanging(IsFreeOfChargeVisiblePropertyName);
                _isfreeOfChargeVisible = value;
                RaisePropertyChanged(IsFreeOfChargeVisiblePropertyName);
            }
        }

        
        public const string IsAvailableVisiblePropertyName = "IsAvailableVisible";
        private bool _myProperty = true;
        public bool IsAvailableVisible
        {
            get
            {
                return _myProperty;
            }

            set
            {
                if (_myProperty == value)
                {
                    return;
                }

                RaisePropertyChanging(IsAvailableVisiblePropertyName);
                _myProperty = value;
                RaisePropertyChanged(IsAvailableVisiblePropertyName);
            }
        }
        public const string IsAddButtonVisiblePropertyName = "IsAddButtonVisible";
        private bool _isAddButtonVisible = true;
        public bool IsAddButtonVisible
        {
            get
            {
                return _isAddButtonVisible;
            }

            set
            {
                if (_isAddButtonVisible == value)
                {
                    return;
                }

                RaisePropertyChanging(IsAddButtonVisiblePropertyName);
                _isAddButtonVisible = value;
                RaisePropertyChanged(IsAddButtonVisiblePropertyName);
            }
        }
       
        public const string PackageLabelPropertyName = "PackageLabel";
        private string _packagelable = "Order By";
        public string PackageLabel
        {
            get
            {
                return _packagelable;
            }

            set
            {
                if (_packagelable == value)
                {
                    return;
                }

                RaisePropertyChanging(PackageLabelPropertyName);
                _packagelable = value;
                RaisePropertyChanged(PackageLabelPropertyName);
            }
        }
       
    }
}