using System;
using System.Collections.Generic;
using System.Windows;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Hierarchy;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Transactional.IAN;
using GalaSoft.MvvmLight;
using Distributr.Core.Domain.InventoryEntities;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Transactional.ITN
{
    public class ITNLineItemViewModel : DistributrViewModelBase
    {
         


        public ITNLineItemViewModel()
        {
            ClearAndSetup = new RelayCommand(RunClearAndSetup);
            SetInventoryDetails = new RelayCommand(SetProductInventory);
            Products = new ObservableCollection<ItnLineItemProductLookupItem>();
            ProductSerialNumbersList = new ObservableCollection<ProductSerialNumbers>();
           
           
        }

        #region Properties
        public RelayCommand ClearAndSetup { get; set; }
        public RelayCommand SetInventoryDetails { get; set; }
        private RelayCommand _productDropDownOpened;
        public RelayCommand ProductDropDownOpenedCommand
        {
            get { return _productDropDownOpened ?? (new RelayCommand(ProductDropDownOpened)); }
        }
        private RelayCommand<string> _unitOfMeasureModeChangedCommand;
        public RelayCommand<string> UnitOfMeasureModeChangedCommand
        {
            get { return _unitOfMeasureModeChangedCommand ?? (new RelayCommand<string>(UnitOfMeasureModeChanged)); }
        }

        private void UnitOfMeasureModeChanged(string uom)
        {
            LineItemType = (LineItemType)Enum.Parse(typeof(LineItemType), uom);
        }


        public ObservableCollection<ItnLineItemProductLookupItem> Products { get; set; }
        public ObservableCollection<ProductSerialNumbers> ProductSerialNumbersList { get; set; }


        public const string SalesmanIdPropertyName = "SalesmanId";
        private Guid _salesmanid;
        public Guid SalesmanId
        {
            get
            {
                return _salesmanid;
            }

            set
            {
                if (_salesmanid == value)
                {
                    return;
                }

                var oldValue = _salesmanid;
                _salesmanid = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(SalesmanIdPropertyName);
            }
        }
        public const string QtyPropertyName = "Qty";
        private decimal _qty;
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

                // Update bindings, no broadcast
                RaisePropertyChanged(QtyPropertyName);
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

        public const string InStockPropertyName = "InStock";
        private decimal _inStock;
        public decimal InStock
        {
            get
            {
                return _inStock;
            }

            set
            {
                if (_inStock == value)
                {
                    return;
                }

                var oldValue = _inStock;
                _inStock = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(InStockPropertyName);
            }
        }

        public const string ProductPropertyName = "Product";
        private ItnLineItemProductLookupItem _product;
        public ItnLineItemProductLookupItem Product
        {
            get
            {
                return _product;
            }

            set
            {
                if (_product == value)
                {
                    return;
                }

                var oldValue = _product;
                _product = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(ProductPropertyName);
            }
        }
       
        public const string IsEditPropertyName = "IsEdit";
        private bool _IsEdit = false;
        public bool IsEdit
        {
            get
            {
                return _IsEdit;
            }

            set
            {
                if (_IsEdit == value)
                {
                    return;
                }

                var oldValue = _IsEdit;
                _IsEdit = value;

             

                // Update bindings, no broadcast
                RaisePropertyChanged(IsEditPropertyName);

              
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

        //public const string AllowBarCodeInputPropertyName = "AllowBarCodeInput";

        //private bool allowBarCodeInput = false;

        //public bool AllowBarCodeInput
        //{
        //    get
        //    {
        //        return allowBarCodeInput;
        //    }

        //    set
        //    {
        //        if (allowBarCodeInput == value)
        //        {
        //            return;
        //        }

        //        var oldValue = allowBarCodeInput;
        //        allowBarCodeInput = value;
                
        //        // Update bindings, no broadcast
        //        RaisePropertyChanged(AllowBarCodeInputPropertyName);
        //    }
        //}   

        
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

        #region Methods
        public void SetProductInventory()
        {
            using (var container = NestedContainer)
            {
                
                IInventoryRepository _inventoryService = Using<IInventoryRepository>(container);
                ;
                IConfigService _configService = Using<IConfigService>(container);
               
                if (Product != null)
                {
                    Inventory inv = _inventoryService.GetByProductIdAndWarehouseId(Product.ProductId,
                                                                     _configService.Load().CostCentreId);
                    InStock = inv != null ? inv.Balance : 0;
                }
            }
        }
        private void ProductDropDownOpened()
        {
            using (var c = NestedContainer)
            {
               var selectedId = Using<IItemsLookUp>(c).IssueInventory(SalesmanId);
                if (selectedId != Guid.Empty)
                {
                    var product = Using<IProductRepository>(c).GetById(selectedId);
                    if (product != null)
                    {
                        Product = new ItnLineItemProductLookupItem()
                        {
                            ProductId = product.Id,
                            ProductCode = product.ProductCode,
                            ProductDesc = product.Description
                        };
                    }

                }


            }
        }

        public bool AddProduct()
        {
            var actualQuantity = Qty;
            if (LineItemType == LineItemType.Bulk)
            {
               decimal bulkQuantity = _productPackagingSummaryService.GetProductQuantityInBulk(Product.ProductId);
                actualQuantity = bulkQuantity*Qty;
            }
            _productPackagingSummaryService.AddProduct(Product.ProductId, actualQuantity, false, IsEdit, false);
            return true;
        }

        public List<PackagingSummary> GetProducts()
        {
            return _productPackagingSummaryService.GetProductSummary();
        }

        public void Setup()
        {
            using (var container = NestedContainer)
            {
                
                var _inventoryService = Using<IInventoryRepository>(container);
                ;
                IConfigService _configService = Using<IConfigService>(container);
               
                Products.Clear();
                var def = new ItnLineItemProductLookupItem
                              {ProductId = Guid.Empty, ProductDesc = "--Please select a product--"};
                Products.Add(def);
                Product = def;
                var tempL = new ObservableCollection<ItnLineItemProductLookupItem>();
                _inventoryService.GetByWareHouseId(_configService.Load().CostCentreId)
                    .Where(n => n.Balance > 0)
                    .ToList()
                    .ForEach(n => tempL.Add(new ItnLineItemProductLookupItem
                                                {
                                                    ProductId = n.Product.Id,
                                                    ProductDesc = n.Product.Description,
                                                    ProductCode = n.Product.ProductCode
                                                }));

                tempL.OrderBy(n => n.ProductDesc).ToList().ForEach(n => Products.Add(n));
            }
        }

        private void RunClearAndSetup()

        {
            SalesmanId = Guid.Empty;
            Qty = 0;
            InStock = 0;
            Setup();
            _productPackagingSummaryService.ClearBuffer();
            IsEdit = false;
            LineItemType=LineItemType.Unit;
            ;
            //RunSetup();
            ProductSerialNumbersList.Clear();
            AppSettings allowBarCodeInput;
            using (var container = NestedContainer)
            {

                ISettingsRepository _appSettingsService = Using<ISettingsRepository>(container);
                allowBarCodeInput = _appSettingsService.GetByKey(SettingsKeys.AllowBarcodeInput);
            }
            if (allowBarCodeInput != null)
            {
                AllowBarCodeInput = allowBarCodeInput.Value == "1" ? true : false;
            }
        }

        void RunSetup()
        {
            using (var container = NestedContainer)
            {

                IProductRepository _productService = Using<IProductRepository>(container);
               
                Products.Clear();
                _productService.GetAll()
                    .OrderBy(n => n.Description)
                    .ToList()
                    .ForEach(n => Products.Add(new ItnLineItemProductLookupItem
                                                   {
                                                       ProductId = n.Id,
                                                       ProductDesc = n.Description,
                                                       ProductCode = n.ProductCode
                                                   }));
            }
        }

        public void LoadForEdit(Guid selectedProductId, decimal qty, List<ProductSerialNumbers> productSerialNums)
        {
            Product = Products.First(n => n.ProductId == selectedProductId);
            Qty = qty;
            IsEdit = true;
            productSerialNums.ForEach(n => ProductSerialNumbersList.Add(n));
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
                ProductId = Product.ProductId
            };
            if (SelectedSerialId != Guid.Empty)
            {
                ProductSerialNumbers serials = ProductSerialNumbersList.FirstOrDefault(n => n.SerialsId == SelectedSerialId);
                item.SerialsId = serials.SerialsId;
                ProductSerialNumbersList.Remove(serials);
            }
            var list = ProductSerialNumbersList;
            list.Add(item);
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
    }

    #region Helper Classes
    public class ItnLineItemProductLookupItem
    {
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductDesc { get; set; }
    }

    public class ItnWareHouseLookupItem
    {
        public Guid WareHouseId { get; set; }
        public string CostCenterName { get; set; }
    }

    public class ProductSerialNumbers
    {
        public Guid SerialsId { get; set; }
        public Guid ProductId { get; set; }
        public Guid LineItemId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
    #endregion
}
