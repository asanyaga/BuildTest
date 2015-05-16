using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.UI.UI_Utillity;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Transactional.GRN
{
    public class GRNItemModalViewModel : DistributrViewModelBase
    {
        public GRNItemModalViewModel()
        {
            ProductSelectedChangedCommand = new RelayCommand(RunProductSelected);
            ClearAndSetup = new RelayCommand(RunClearAndSetup);
            ShowAddedProductsCommand = new RelayCommand(ShowAddedProductsSummary);
            Products = new ObservableCollection<GRNLineItemProductLookupItem>();
            ProductSerialNumbersList = new ObservableCollection<ProductSerialNumbers>();
            LineItems = new List<GRNLineItems>();
            ProductDropDownOpenedCommand=new RelayCommand<object>(DropDownOpened);
            EditSerialCommand = new RelayCommand(EditSelectedSerial);
            DeleteSelectedSerialCommand =new RelayCommand(DeleteSelectedSerial);
            ValidNumericInputCommand=new RelayCommand<object>(ValidateInput);
            DoneAddingCommand=new RelayCommand(DoneAdding);
            CancelCommand =new RelayCommand(Cancel);
            AddSerialsCommand  = new RelayCommand(AddSerialNumbers);
            
        }

        public RelayCommand ProductSelectedChangedCommand { get; set; }
        public RelayCommand ClearAndSetup { get; set; }
        public RelayCommand ShowAddedProductsCommand { get; set; }
        public RelayCommand <object> ProductDropDownOpenedCommand { get; set; }
        public RelayCommand EditSerialCommand { get; set; }
        public RelayCommand AddSerialsCommand { get; set; }
        public RelayCommand DeleteSelectedSerialCommand { get; set; }
        public RelayCommand<object> ValidNumericInputCommand { get; set; }
        public RelayCommand DoneAddingCommand { get; set; }
        public RelayCommand AddProductCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public event EventHandler RequestClose = (s, e) => { };

       

        public List<GRNLineItems> LineItems { get; set; }

        public ObservableCollection<ProductSerialNumbers> ProductSerialNumbersList { get; set; }
        private bool IsCancelled=false;
        #region Methods

        public GRNModalItems GetModalItems()
        {
            if (IsCancelled) return null;
             return new GRNModalItems
                                         {
                                             LineItems = LineItems,
                                             InventorySerials = ProductSerialNumbersList.ToList(),
                                             Reason = Reason
                                         };
            
        }
        private void DropDownOpened(object sender)
        {
            using (var c=NestedContainer)
            {
                SelectedProduct = Using<IComboPopUp>(c).ShowDlg(sender) as GRNLineItemProductLookupItem;
            }
           
        }

        private void ValidateInput(object sender)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                switch (textBox.Name)
                {
                    case "txtFrom":
                        WPFValidation.InvalidateSpecialCharactersOnKeyUp(sender);
                        break;

                    case "txtTo":
                       
                        break;

                }
            }
        }

        private void DoneAdding()
        {
            if (!ValidateBeforeAddProduct()) return;
            AddProducts();
            this.RequestClose(this, EventArgs.Empty);
        }


        private void Setup()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                Products.Clear();
                Using<IProductRepository>(c).GetAll() //.Where(p=>p is SaleProduct && p is ConsolidatedProduct)
                               .OrderBy(n => n.Description)
                               .ToList()
                               .ForEach(n => Products.Add(new GRNLineItemProductLookupItem
                                   {
                                       ProductId = n.Id,
                                       ProductCode = n.ProductCode,
                                       ProductDesc = n.Description
                                   }));
            }
        }
        void RunClearAndSetup()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                Qty = 1;
                UnitCost = 0;
                LineItemTotal = 0;
                Reason = null;
                LineItems.Clear();
                 Setup();
                Reason = string.Empty;
                ProductSerialNumbersList.Clear();
                AddedProductsCount = "AddedProducts(0)";
                var allowBarCodeInput = Using<ISettingsRepository>(c).GetByKey(SettingsKeys.AllowBarcodeInput);
                if (allowBarCodeInput != null)
                {
                    AllowBarCodeInput = allowBarCodeInput.Value == "1" ? true : false;
                }
            }
        }
        private void Cancel()
        {
            if (MessageBox.Show("Are you sure you want to cancel? \n Unsaved changes will be lost",
               "Receive Inventory", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                IsCancelled = true;
                RequestClose(this, EventArgs.Empty);
            }
            
        }
        private void ShowAddedProductsSummary()
        {
            using (var c = NestedContainer)
            {
                if (LineItems.Any())
                {
                    var message = new StringBuilder();
                    int count = 1;

                    foreach (var productItem in LineItems)
                    {
                        var product = Using<IProductRepository>(c).GetById(productItem.SelectedProduct.ProductId);
                        if (product == null) continue;
                        var unitprice = ProductPriceCalc(product);

                        message.AppendLine("(" + count + ".)" + product.Description + "\tQuantity:" +
                                           productItem.Qty + "\t" + "Total Price:" +
                                           (unitprice * productItem.Qty).ToString("0.00"));
                        message.AppendLine("---------------------------------------------------------------");
                        count++;

                    }
                    MessageBox.Show(message.ToString(), "Added products", MessageBoxButton.OK,
                                    MessageBoxImage.Information);


                }
            }

        }

        void RecalcTotal()
        {
            decimal net = UnitCost * Qty;
            LineItemTotal = net;
        }

        void PriceCalc(Product product)
        {
            try
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    ProductPricingTier tier = Using<IProductPricingTierRepository>(c).GetAll().First();
                    if (product is ConsolidatedProduct)
                        UnitCost = ((ConsolidatedProduct)product).TotalExFactoryValue(tier);
                    else
                    {
                        ProductPricing productpricing = product.ProductPricings.FirstOrDefault(n => n.Tier.Id == tier.Id);
                        if (productpricing != null)
                            UnitCost =
                                product.ProductPricings.FirstOrDefault(n => n.Tier.Id == tier.Id).CurrentExFactory;
                        else
                            UnitCost = product.ProductPricings.FirstOrDefault().CurrentExFactory;
                    }
                }
            }
            catch
            {
                UnitCost = 0;
            }
        }

        void RunProductSelected()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                if (SelectedProduct != null)
                {
                    var product = Using<IProductRepository>(c).GetById(SelectedProduct.ProductId);
                    if (product is ConsolidatedProduct)
                        isConsolidatedProduct = true;
                    else
                        isConsolidatedProduct = false;
                    PriceCalc(product);
                    RecalcTotal();
                }
            }
        }

        private bool ValidateBeforeAddProduct()
        {
            if (Qty <= 0)
            {
                MessageBox.Show("Quantity should be greater than zero");
                return false;
            }
            if (!IsNew && string.IsNullOrEmpty(Reason))
            {
               MessageBox.Show("Reason is required");
                    return false;
            }
            return true;
        }

        private void AddProducts()
        {
           
            if (SelectedProduct == null)
            {

                return;
            }
            var items = _productPackagingSummaryService.GetProductSummaryByProduct(SelectedProduct.ProductId, Qty);
            foreach (var item in items)
            {
                var li = LineItems.FirstOrDefault(n => n.SelectedProduct.ProductId == item.Product.Id);
                if (li != null)
                {
                    li.Qty = IsNew ? li.Qty += Qty : li.Qty = Qty;
                    li.LineItemTotal = li.Qty + li.UnitCost;
                    li.Expected = (int) Expected;
                }
                else
                {
                    var unitCost = ProductPriceCalc(item.Product);
                    //item.Product.ProductPricings.First().CurrentExFactory;
                    LineItems.Add(new GRNLineItems
                    {
                        SelectedProduct =
                            new GRNLineItemProductLookupItem
                            {
                                ProductDesc = item.Product.Description,
                                ProductCode = item.Product.ProductCode,
                                ProductId = item.Product.Id
                            },
                        isConsolidatedProduct = item.Product is ConsolidatedProduct ? true : false,
                        Qty = item.Quantity,
                        UnitCost = unitCost,
                        LineItemTotal = (item.Quantity * unitCost),
                        Reason = Reason,
                        LineItemId = Guid.NewGuid()
                    });

                }
            }
            ShowAddedProductsLink = Visibility.Visible;
            AddedProductsCount = string.Format("AddedProducts(" + LineItems.Count + ")");
            _productPackagingSummaryService.ClearBuffer();
        }

        public void LoadForEdit(AddGrnLineItemViewModel lineItem, List<ProductSerialNumbers> productSerials)
        {
            SelectedProduct = Products.First(n => n.ProductId == lineItem.ProductId);
            LineItemId = lineItem.LineItemId;
            LineItemTotal = lineItem.LineItemTotal;
            UnitCost = lineItem.UnitCost;
            Qty = lineItem.Qty;
            Expected = lineItem.Expected;
            isConsolidatedProduct = lineItem.isConsolidatedProduct;
            BreakBulk = lineItem.BreakBulk;
            Reason = lineItem.Reason;

            RecalcTotal();
            AddProducts();
            if (productSerials != null && productSerials.Any())
                productSerials.ForEach(n => ProductSerialNumbersList.Add(n));

            IsNew = false;
        }



        decimal ProductPriceCalc(Product product)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                //TODO: find out how tier is resolved
                ProductPricingTier tier = Using<IProductPricingTierRepository>(c).GetAll().First();
                decimal prodprice = 0;
                if (product is ConsolidatedProduct)
                    try
                    {
                        prodprice = ((ConsolidatedProduct)product).TotalExFactoryValue(tier);
                    }
                    catch
                    {
                        prodprice = 0m;
                    }
                else
                    try
                    {
                        prodprice = product.TotalExFactoryValue(tier);
                      
                    }
                    catch
                    {
                        prodprice = 0m;
                    }


                return prodprice;
            }
        }

        public void AddSerialNumbers()
        {
            if (string.IsNullOrEmpty(SerialFrom.Trim()) || string.IsNullOrEmpty(SerialTo.Trim()))
            {
                MessageBox.Show("You must enter the begininnig and last serial numbers." +
                    "\nFor a single serial number, enter it in both From and To fields.");
                return;
            }
            ProductSerialNumbers item = null;
            item = new ProductSerialNumbers
            {
                LineItemId = LineItemId,
                SerialsId = Guid.NewGuid(),
                From = SerialFrom.Trim(),
                To = SerialTo.Trim(),
                ProductId = SelectedProduct.ProductId
            };
            if (SelectedSerialId != Guid.Empty)
            {
                ProductSerialNumbers serials = ProductSerialNumbersList.FirstOrDefault(n => n.SerialsId == SelectedSerialId);
                item.SerialsId = serials.SerialsId;
                ProductSerialNumbersList.Remove(serials);
            }

            ProductSerialNumbersList.Add(item);
            SerialFrom = "";
            SerialTo = "";
            SelectedSerialNumbers = item;
            SelectedSerialId = Guid.Empty;
        }
        public void EditSelectedSerial()
        {
            SelectedSerialId = SelectedSerialNumbers.SerialsId;
            SerialFrom = SelectedSerialNumbers.From;
            SerialTo = SelectedSerialNumbers.To;
        }

        public void DeleteSelectedSerial()
        {
            if (MessageBox.Show("Are you sure you want to delete selected serial numbers?") == MessageBoxResult.Cancel)
                return;
            ProductSerialNumbersList.Remove(SelectedSerialNumbers);
            SelectedSerialNumbers = ProductSerialNumbersList.FirstOrDefault();
        }
#endregion
        #region Properties

        public const string LineItemIdPropertyName = "LineItemId";
        private Guid _lineItemId = Guid.Empty;
        public Guid LineItemId
        {
            get
            {
                return _lineItemId;
            }

            set
            {
                if (_lineItemId == value)
                    return;

                var oldValue = _lineItemId;
                _lineItemId = value;
                RaisePropertyChanged(LineItemIdPropertyName);
            }
        }

        public const string ShowAddedProductsLinkPropertyName = "ShowAddedProductsLink";
        private Visibility _showAddedProductsLink = Visibility.Collapsed;
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
        public const string AddedProductsCountPropertyName = "AddedProductsCount";
        private string _addedProductsCount = "Added products(0)";
        public string AddedProductsCount
        {
            get
            {
                return _addedProductsCount;
            }

            set
            {
                if (_addedProductsCount == value)
                {
                    return;
                }

                RaisePropertyChanging(AddedProductsCountPropertyName);
                _addedProductsCount = value;
                RaisePropertyChanged(AddedProductsCountPropertyName);
            }
        }

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
                RecalcTotal();

            }
        }

        public const string ExpectedPropertyName = "Expected";
        private decimal _expected;
        public decimal Expected
        {
            get
            {
                return _expected;
            }

            set
            {
                if (_expected == value)
                    return;
               _expected = value;
                RaisePropertyChanged(ExpectedPropertyName);
            }
        }

        public const string UnitCostPropertyName = "UnitCost";
        private decimal _unitCost;
        public decimal UnitCost
        {
            get
            {
                return _unitCost;
            }

            set
            {
                if (_unitCost == value)
                    return;
                var oldValue = _unitCost;
                _unitCost = value;
                RaisePropertyChanged(UnitCostPropertyName);
            }
        }

        public const string SelectedProductPropertyName = "SelectedProduct";
        private GRNLineItemProductLookupItem _selectedProduct;
        public GRNLineItemProductLookupItem SelectedProduct
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

        public ObservableCollection<GRNLineItemProductLookupItem> Products { get; set; }

        public const string LineItemTotalPropertyName = "LineItemTotal";
        private decimal _lineItemTotal;
        public decimal LineItemTotal
        {
            get
            {
                return _lineItemTotal;
            }

            set
            {
                if (_lineItemTotal == value)
                    return;
                var oldValue = _lineItemTotal;
                _lineItemTotal = value;
                RaisePropertyChanged(LineItemTotalPropertyName);
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

        public const string ReasonPropertyName = "Reason";
        private string _Reason;
        public string Reason
        {
            get
            {
                return _Reason;
            }

            set
            {
                if (_Reason == value)
                {
                    return;
                }

                var oldValue = _Reason;
                _Reason = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(ReasonPropertyName);
            }
        }

        public const string EditingPropertyName = "Editing";
        private bool _Editing;
        public bool Editing
        {
            get
            {
                return _Editing;
            }

            set
            {
                if (_Editing == value)
                {
                    return;
                }

                var oldValue = _Editing;
                _Editing = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(EditingPropertyName);
            }
        }

        public const string isConsolidatedProductPropertyName = "isConsolidatedProduct";
        private bool _isConsolidatedProduct;
        public bool isConsolidatedProduct
        {
            get
            {
                return _isConsolidatedProduct;
            }

            set
            {
                if (_isConsolidatedProduct == value)
                {
                    return;
                }

                var oldValue = _isConsolidatedProduct;
                _isConsolidatedProduct = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(isConsolidatedProductPropertyName);
            }
        }

        public const string BreakBulkPropertyName = "BreakBulk";
        private bool _BreakBulk;
        public bool BreakBulk
        {
            get
            {
                return _BreakBulk;
            }

            set
            {
                if (_BreakBulk == value)
                {
                    return;
                }

                var oldValue = _BreakBulk;
                _BreakBulk = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(BreakBulkPropertyName);
            }
        }
        
       

        public const string SerialFromPropertyName = "SerialFrom";
        private string _serialFrom = "";
        public string SerialFrom
        {
            get
            {
                return _serialFrom;
            }

            set
            {
                if (_serialFrom == value)
                {
                    return;
                }

                var oldValue = _serialFrom;
                _serialFrom = value;

                RaisePropertyChanged(SerialFromPropertyName);
            }
        } 

        public const string SerialToPropertyName = "SerialTo";
        private string _serialTo = "";
        public string SerialTo
        {
            get
            {
                return _serialTo;
            }

            set
            {
                if (_serialTo == value)
                {
                    return;
                }

                var oldValue = _serialTo;
                _serialTo = value;
                RaisePropertyChanged(SerialToPropertyName);
            }
        }
         
        public const string SelectedSerialNumbersPropertyName = "SelectedSerialNumbers";
        private ProductSerialNumbers _selectedSerialNumbers = null;
        public ProductSerialNumbers SelectedSerialNumbers
        {
            get
            {
                return _selectedSerialNumbers;
            }

            set
            {
                if (_selectedSerialNumbers == value)
                {
                    return;
                }

                var oldValue = _selectedSerialNumbers;
                _selectedSerialNumbers = value;
                RaisePropertyChanged(SelectedSerialNumbersPropertyName);
            }
        }

        public const string SelectedSerialIdPropertyName = "SelectedSerialId";
        private Guid _selectedSerialId = Guid.Empty;
        public Guid SelectedSerialId
        {
            get
            {
                return _selectedSerialId;
            }

            set
            {
                if (_selectedSerialId == value)
                {
                    return;
                }

                var oldValue = _selectedSerialId;
                _selectedSerialId = value;
                RaisePropertyChanged(SelectedSerialIdPropertyName);
            }
        }

        public const string AllowBarCodeInputPropertyName = "AllowBarCodeInput";

        private bool _allowBarCodeInput = false;

        public bool AllowBarCodeInput
        {
            get
            {
                return _allowBarCodeInput;
            }

            set
            {
                if (_allowBarCodeInput == value)
                {
                    return;
                }

                var oldValue = _allowBarCodeInput;
                _allowBarCodeInput = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(AllowBarCodeInputPropertyName);
            }
        }

#endregion
    }

    #region Helper Classes
    public class GRNLineItemProductLookupItem
    {
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductDesc { get; set; }
    }

    public class GRNLineItems : DistributrViewModelBase
    {
        public const string LineItemIdPropertyName = "LineItemId";
        private Guid _lineItemId = Guid.Empty;
        public Guid LineItemId
        {
            get
            {
                return _lineItemId;
            }

            set
            {
                if (_lineItemId == value)
                    return;

                var oldValue = _lineItemId;
                _lineItemId = value;
                RaisePropertyChanged(LineItemIdPropertyName);
            }
        }

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
            }
        }

        public const string ExpectedPropertyName = "Expected";
        private int _expected;
        public int Expected
        {
            get
            {
                return _expected;
            }

            set
            {
                if (_expected == value)
                    return;
                var oldValue = _expected;
                _expected = value;
                RaisePropertyChanged(ExpectedPropertyName);
            }
        }

        public const string UnitCostPropertyName = "UnitCost";
        private decimal _unitCost;
        public decimal UnitCost
        {
            get
            {
                return _unitCost;
            }

            set
            {
                if (_unitCost == value)
                    return;
                var oldValue = _unitCost;
                _unitCost = value;
                RaisePropertyChanged(UnitCostPropertyName);
            }
        }

        public const string SelectedProductPropertyName = "SelectedProduct";
        private GRNLineItemProductLookupItem _selectedProduct;
        public GRNLineItemProductLookupItem SelectedProduct
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

        public const string LineItemTotalPropertyName = "LineItemTotal";
        private decimal _lineItemTotal;
        public decimal LineItemTotal
        {
            get
            {
                return _lineItemTotal;
            }

            set
            {
                if (_lineItemTotal == value)
                    return;
                var oldValue = _lineItemTotal;
                _lineItemTotal = value;
                RaisePropertyChanged(LineItemTotalPropertyName);
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

        public const string ReasonPropertyName = "Reason";
        private string _Reason;
        public string Reason
        {
            get
            {
                return _Reason;
            }

            set
            {
                if (_Reason == value)
                {
                    return;
                }

                var oldValue = _Reason;
                _Reason = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(ReasonPropertyName);
            }
        }

        public const string EditingPropertyName = "Editing";
        private bool _Editing;
        public bool Editing
        {
            get
            {
                return _Editing;
            }

            set
            {
                if (_Editing == value)
                {
                    return;
                }

                var oldValue = _Editing;
                _Editing = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(EditingPropertyName);
            }
        }

        public const string isConsolidatedProductPropertyName = "isConsolidatedProduct";
        private bool _isConsolidatedProduct;
        public bool isConsolidatedProduct
        {
            get
            {
                return _isConsolidatedProduct;
            }

            set
            {
                if (_isConsolidatedProduct == value)
                {
                    return;
                }

                var oldValue = _isConsolidatedProduct;
                _isConsolidatedProduct = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(isConsolidatedProductPropertyName);
            }
        }

        public const string BreakBulkPropertyName = "BreakBulk";
        private bool _BreakBulk;
        public bool BreakBulk
        {
            get
            {
                return _BreakBulk;
            }

            set
            {
                if (_BreakBulk == value)
                {
                    return;
                }

                var oldValue = _BreakBulk;
                _BreakBulk = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(BreakBulkPropertyName);
            }
        }
       
    }

    public class GRNModalItems
    {
        public List<GRNLineItems> LineItems { get; set; }
        public string Reason { get; set; }
        public List<ProductSerialNumbers> InventorySerials { get; set; }
    }

      


    #endregion
}
