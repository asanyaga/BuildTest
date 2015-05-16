using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Transactional.ITN;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Transactional.DispatchProducts
{
    public class DPLineItemViewModel : DistributrViewModelBase
    {
        public DPLineItemViewModel()
        {

            Products = new ObservableCollection<ProductLookupItem>();
            Reasons = new ObservableCollection<ReasonLookUpItem>();
            LoadedProducts = new List<Product>();
            AvailableInventory = new List<Inventory>();
        }

        public ObservableCollection<ProductLookupItem> Products { get; set; }
        public ObservableCollection<ReasonLookUpItem> Reasons { get; set; }

        private RelayCommand _selectedproductChanged;
        public RelayCommand SelectedProductChangedCommand
        {
            get { return _selectedproductChanged ?? (new RelayCommand(ProductSelectedChanged)); }
        }

        private RelayCommand _productDropDownOpened;
        public RelayCommand ProductDropDownOpenedCommand
        {
            get { return _productDropDownOpened ?? (new RelayCommand(ProductDropDownOpened)); }
        }

      
        public List<Product> LoadedProducts { get; set; }
        public List<Inventory> AvailableInventory { get; set; }
         
        public const string SelectedProductPropertyName = "SelectedProduct";
        private ProductLookupItem _selectedProduct = null;
        public ProductLookupItem SelectedProduct
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
         
        public const string SelectedReasonPropertyName = "SelectedReason";
        private ReasonLookUpItem _selectedReason = null;
        public ReasonLookUpItem SelectedReason
        {
            get
            {
                return _selectedReason;
            }

            set
            {
                if (_selectedReason == value)
                {
                    return;
                }

                var oldValue = _selectedReason;
                _selectedReason = value;
                RaisePropertyChanged(SelectedReasonPropertyName);
            }
        }
         
        public const string AvailableQtyPropertyName = "AvailableQty";
        private decimal _availableQty = 0;
        public decimal AvailableQty
        {
            get
            {
                return _availableQty;
            }

            set
            {
                if (_availableQty == value)
                {
                    return;
                }

                var oldValue = _availableQty;
                _availableQty = value;
                RaisePropertyChanged(AvailableQtyPropertyName);
            }
        }
         
        public const string QtyPropertyName = "Qty";
        private decimal _qty = 0;
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
         
        public const string OtherReasonPropertyName = "OtherReason";
        private string _otherReason = "";
        public string OtherReason
        {
            get
            {
                return _otherReason;
            }

            set
            {
                if (_otherReason == value)
                {
                    return;
                }

                var oldValue = _otherReason;
                _otherReason = value;
                RaisePropertyChanged(OtherReasonPropertyName);
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

                var oldValue = _isEdit;
                _isEdit = value;
                RaisePropertyChanged(IsEditPropertyName);
            }
        }

        public void ClearAndSetUp()
        {
            Clear();
            SetUp();
        }

        void Clear()
        {
            IsEdit = false;
            AvailableQty = 0m;
            Qty = 0m;
            OtherReason = string.Empty;
        }

        void SetUp(bool loadForEditing = false, Guid? productId = null)
        {
            LoadProducts(loadForEditing, productId);
            LoadReasons();
        }

        private void LoadProducts(bool loadForEditing = false, Guid? productId = null)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                Products.Clear();
                AvailableInventory.Clear();
                LoadedProducts.Clear();

                var product = new ProductLookupItem
                    {
                        Id = Guid.Empty,
                        Description = "--Please Select a Product--"
                    };
                Products.Add(product);
                SelectedProduct = product;
                if (loadForEditing)
                {
                    var editproduct = Using<IProductRepository>(c).GetById(productId.Value);
                    AvailableInventory.Add(
                        Using<IInventoryRepository>(c) .GetByWareHouseId(Using<IConfigService>(c) .Load().CostCentreId)
                                         .FirstOrDefault(n => n.Product.Id == productId.Value));

                    LoadedProducts.Add(editproduct);
                    Products.Add(new ProductLookupItem
                        {
                            Id = editproduct.Id,
                            Description = editproduct.Description,
                            Code = editproduct.ProductCode
                        });
                }
                else
                {
                    AvailableInventory = Using<IInventoryRepository>(c) .GetByWareHouseId(Using<IConfigService>(c)  .Load().CostCentreId)
                                                          .Where(n => n.Balance > 0)
                                                          .OrderBy(n => n.Product.Description)
                                                          .ToList();
                    AvailableInventory.ForEach(n => LoadedProducts.Add(n.Product));

                    LoadedProducts.ToList().ForEach(n => Products.Add(new ProductLookupItem
                        {
                            Id = n.Id,
                            Description = n.Description,
                        }));
                }
            }
        }

        void LoadReasons()
        {
            Reasons.Clear();
            var reason = new ReasonLookUpItem {Id = Guid.Empty, Reason = "--Please Select A Reason--"};
            Reasons.Add(reason);
            SelectedReason = reason;

            //to load from repo in  future
            Reasons.Add(new ReasonLookUpItem { Id = Guid.NewGuid(), Reason = "Expiry" });
            Reasons.Add(new ReasonLookUpItem { Id = Guid.NewGuid(), Reason = "Non-conforming" });
            Reasons.Add(new ReasonLookUpItem { Id = Guid.NewGuid(), Reason = "Stolen" });
            Reasons.Add(new ReasonLookUpItem { Id = Guid.NewGuid(), Reason = "Underfills" });
            Reasons.Add(new ReasonLookUpItem { Id = Guid.NewGuid(), Reason = "Others" });
        }

        void ProductSelectedChanged()
        {
            if (SelectedProduct != null && SelectedProduct.Id != Guid.Empty)
            {
                var inventory = AvailableInventory.FirstOrDefault(n => n.Product.Id == SelectedProduct.Id);
                if (inventory != null)
                    AvailableQty = inventory.Balance;
            }
        }
        private void ProductDropDownOpened()
        {
            using (var c = NestedContainer)
            {
                var selectedId = Using<IItemsLookUp>(c).ShowDlg(typeof(Product));
                if (selectedId != Guid.Empty)
                {
                    var product = Using<IProductRepository>(c).GetById(selectedId);
                    if (product != null)
                    {
                        SelectedProduct = new ProductLookupItem()
                        {
                            Id = product.Id,
                            Code = product.ProductCode,
                            Description = product.Description
                        };
                    }

                }


            }
        }

        public void LoadForEdit(Guid productId, string reason, string otherReason, decimal qty)
        {
            IsEdit = true;
            SelectedProduct = Products.FirstOrDefault(n => n.Id == productId);
            Qty = qty;
            SelectedReason = Reasons.FirstOrDefault(n => n.Reason == reason);
            OtherReason = otherReason;
            AvailableQty -= qty;
        }
    }

    public class ProductLookupItem
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public decimal Available { get; set; }
    }

    public class ReasonLookUpItem
    {
        public Guid Id { get; set; }
        public string Reason { get; set; }
    }
}
