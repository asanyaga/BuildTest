using System;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Hierarchy;
using Distributr.WPF.Lib.UI.Pages;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using Distributr.Core.Domain.InventoryEntities;

namespace Distributr.WPF.Lib.ViewModels.Transactional.IAN
{

    public class IANLineItemViewModel : DistributrViewModelBase
    {
        public bool StockTake = false;
        public IANLineItemViewModel()
        {
            ClearAndSetup = new RelayCommand(RunClearAndSetup);
            SetInventoryDetails = new RelayCommand(SetProductInventory);
            LoadModalWindowCommand =new RelayCommand(LoadWindow);

            
            using (StructureMap.IContainer c = NestedContainer)
            {
                InventoryCC = Using<IConfigService>(c).Load().CostCentreId;
            }
           
        }

        private void AdjustModeChanged(string mode)
        {
            LineItemType = (LineItemType)Enum.Parse(typeof(LineItemType), mode);
        }

        void SetProductInventory()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                if (SelectedProduct == null)
                    return;
                Inventory inv = Using<IInventoryRepository>(c).GetByProductIdAndWarehouseId(SelectedProduct.ProductId, InventoryCC);
                if (inv != null)
                {
                    Expected = inv.Balance;
                    //Actual = 0;
                }
                else
                {
                    //Actual = 0;
                    Expected = 0;
                }
            }
        }

        private void LoadWindow()
        {
            
            if(SelectedProduct.ProductId !=Guid.Empty)
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                   
                }
            }
        }

        public void Setup()
        {

            TitlePage = "Distributr: "
                        +
                        (StockTake
                             ? GetLocalText("sl.inventory.stocktake.title")
                             : GetLocalText("sl.inventory.adjust.title"))
                        + " Add" + GetLocalText("sl.inventory.adjust.modal.product");

            var defaultP = new IANLineItemProductLookupItem { ProductId = Guid.Empty, ProductDesc = "--Select product--" };
           SelectedProduct = defaultP;
           
        }

        void RunClearAndSetup()
        {
            Actual = 0;
            Expected = 0;
            Reason = "";
            Variance = 0;
            IsEdit = false;
            IsUnit = true;
            IsBulk = false;
            RadioEdit = false;
            SelectedProduct = null;
            Setup();
        }

        public void LoadForEdit(Guid selectedProductId, decimal actual, decimal expected, string reason,
                                decimal variance,LineItemType lineItemType)
        {
            //bool isBulk = lineItemType == LineItemType.Bulk;
            using (StructureMap.IContainer c = NestedContainer)
            {
                //var bulkQuantity = Using<IProductPackagingSummaryService>(c).GetProductQuantityInBulk(selectedProductId);
                //actual = isBulk ? actual / bulkQuantity : actual;

                var prod = Using<IProductRepository>(c).GetById(selectedProductId);
                SelectedProduct = new IANLineItemProductLookupItem()
                                      {
                                          ProductCode = prod.ProductCode,
                                          ProductId = prod.Id,
                                          ProductDesc = prod.Description
                                      };
                Actual = actual;
                Expected = expected;
                Reason = reason;
                Variance = variance;
                IsEdit = true;
                LineItemType = lineItemType;

            }
        }

        #region Properties
        public RelayCommand ClearAndSetup { get; set; }
        
        public RelayCommand SetInventoryDetails { get; set; }

        public RelayCommand LoadModalWindowCommand { get; set; }

        private RelayCommand _productDropDownOpened;
        public RelayCommand ProductDropDownOpenedCommand
        {
            get { return _productDropDownOpened ?? (new RelayCommand(ProductDropDownOpened)); }
        }

        private RelayCommand<string> _adjustModeChanged;
        public RelayCommand<string> AdjustModeChangedCommand 
        {
            get { return _adjustModeChanged ?? (new RelayCommand<string>(AdjustModeChanged)); }
        }
        private void ProductDropDownOpened()
        {
            using (var c = NestedContainer)
            {
                var selectedId = Using<IItemsLookUp>(c).ShowDlg(typeof(Product));
                if(selectedId !=Guid.Empty)
                {
                    var product = Using<IProductRepository>(c).GetById(selectedId);
                    if (product != null)
                    {
                        SelectedProduct = new IANLineItemProductLookupItem()
                                              {
                                                  ProductId = product.Id,
                                                  ProductCode = product.ProductCode,
                                                  ProductDesc = product.Description

                                              };
                    }
                    
                }


            }
        }
        
      
        
        public const string TitlePagePropertyName = "TitlePage";
        private string _titlePage = "Distributr: Inventory Adjustment, Add products";
        public string TitlePage
        {
            get
            {
                return _titlePage;
            }

            set
            {
                if (_titlePage == value)
                {
                    return;
                }

                var oldValue = _titlePage;
                _titlePage = value;

              

                // Update bindings, no broadcast
                RaisePropertyChanged(TitlePagePropertyName);

               
            }
        }

        public const string ActualPropertyName = "Actual";
        private decimal _actual = 0;
        public decimal Actual
        {
            get
            {
                return _actual;
            }

            set
            {
                if (_actual == value)
                {
                    return;
                }

                var oldValue = _actual;
                _actual = value;



                // Update bindings, no broadcast
                RaisePropertyChanged(ActualPropertyName);


            }
        }

        public const string ExpectedPropertyName = "Expected";
        private decimal _expected = 0;
        public decimal Expected
        {
            get
            {
                return _expected;
            }

            set
            {
                if (_expected == value)
                {
                    return;
                }

                var oldValue = _expected;
                _expected = value;



                // Update bindings, no broadcast
                RaisePropertyChanged(ExpectedPropertyName);


            }
        }

        public const string SelectedProductPropertyName = "SelectedProduct";
        private IANLineItemProductLookupItem _selectedProduct = null;
        public IANLineItemProductLookupItem SelectedProduct
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



                // Update bindings, no broadcast
                RaisePropertyChanged(SelectedProductPropertyName);


            }
        }

        public const string InventoryCCPropertyName = "InventoryCC";
        private Guid _InventoryCC = Guid.Empty;
        public Guid InventoryCC
        {
            get
            {
                return _InventoryCC;
            }

            set
            {
                if (_InventoryCC == value)
                {
                    return;
                }

                var oldValue = _InventoryCC;
                _InventoryCC = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(InventoryCCPropertyName);
            }
        }
       
        public const string ReasonPropertyName = "Reason";
        private string _reason = "";
        public string Reason
        {
            get
            {
                return _reason;
            }

            set
            {
                if (_reason == value)
                {
                    return;
                }

                var oldValue = _reason;
                _reason = value;
                

                // Update bindings, no broadcast
                RaisePropertyChanged(ReasonPropertyName);

               
            }
        }

       
        public const string VariancePropertyName = "Variance";
        private decimal _Variance = 0;
        public decimal Variance
        {
            get
            {
                return _Variance;
            }

            set
            {
                if (_Variance == value)
                {
                    return;
                }

                var oldValue = _Variance;
                _Variance = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(VariancePropertyName);

             
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

        public const string RadioEditPropertyName = "RadioEdit";
        private bool _RadioEdit = true;
        public bool RadioEdit
        {
            get
            {
                return _RadioEdit;
            }

            set
            {
                if (_RadioEdit == value)
                {
                    return;
                }

                var oldValue = _RadioEdit;
                _RadioEdit = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(RadioEditPropertyName);
            }
        }

        public const string IsUnitPropertyName = "IsUnit";
        private bool _isUnit = true;
        public bool IsUnit
        {
            get
            {
                return _isUnit;
            }

            set
            {
                if (_isUnit == value)
                {
                    return;
                }

                RaisePropertyChanging(IsUnitPropertyName);
                _isUnit = value;
                RaisePropertyChanged(IsUnitPropertyName);
            }
        }

        public const string IsBulkPropertyName = "IsBulk";
        private bool _isBulk = false;
        public bool IsBulk
        {
            get
            {
                return _isBulk;
            }

            set
            {
                if (_isBulk == value)
                {
                    return;
                }

                RaisePropertyChanging(IsBulkPropertyName);
                _isBulk = value;
                RaisePropertyChanged(IsBulkPropertyName);
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
        #endregion
    }

    public class IANLineItemProductLookupItem
    {
        public Guid ProductId { get; set; }
        public string ProductDesc { get; set; }
        public string ProductCode { get; set; }
    }
}