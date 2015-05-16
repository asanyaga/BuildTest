using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Printerutilis;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Distributr.WPF.Lib.ViewModels.Transactional.InventoryViewModels
{
    public class ListInventoryViewModel : DistributrViewModelBase
    {
       
        public ObservableCollection<ListInventoryItemViewModel> InventoryList { get; set; }
        public ObservableCollection<ListInventoryItemViewModel> LineItems { get; set; }
        public ObservableCollection<Product> Products { get; set; }
        public RelayCommand LoadInventoryList { get; set; }

        public RelayCommand LoadAllInventoryCommand { get; set; }
        public RelayCommand LoadGenericInventoryList { get; set; }
        public RelayCommand LoadGetGenericProduct { get; set; }
        public RelayCommand<ListInventoryItemViewModel> ReconcileCommand { get; set; }
        public RelayCommand ReconcilePageLoadedCommand { get; set; }
        public RelayCommand GenericProductListingPageLoadedCommand { get; set; }
        public RelayCommand LoadReturnableProducts { get; set; }
        public RelayCommand ConfirmCommand { get; set; }
        public RelayCommand CleanUpCommand { get; set; }
        public RelayCommand<object> PrintReportCommand { get; set; }
        public RelayCommand LoadWarehouseCommand { get; set; }

        public ListInventoryViewModel()
        {
            LineItems = new ObservableCollection<ListInventoryItemViewModel>();
           
            LoadInventoryList = new RelayCommand(RunLoadInventoryList);
            LoadGenericInventoryList = new RelayCommand(RunLoadGenericInventoryList);
            LoadGetGenericProduct = new RelayCommand(RunGetGenericProduct);
            LoadReturnableProducts = new RelayCommand(RunReturnableProducts);
            ReconcileCommand= new RelayCommand<ListInventoryItemViewModel>(Reconcile);
            ConfirmCommand = new RelayCommand(ConfirmReconciliation);
            CleanUpCommand = new RelayCommand(CleanUp);
            ReconcilePageLoadedCommand= new RelayCommand(ReconCilePageLoaded);
            GenericProductListingPageLoadedCommand=new RelayCommand(ListGenericPageLoaded);
            InventoryList = new ObservableCollection<ListInventoryItemViewModel>();
            Products = new ObservableCollection<Product>();
            PrintReportCommand = new RelayCommand<object>(PrintReport);
            LoadWarehouseCommand = new RelayCommand(LoadWarehouse);
            LoadAllInventoryCommand = new RelayCommand(LoadAllInventory);
        }

        private void LoadAllInventory()
        {
            SelectedWarehouse = null;
            RunLoadInventoryList();
        }

        private void LoadWarehouse()
        {
            using (var container = NestedContainer)
            {
                var parentCostCentreId = Using<IConfigService>(container).Load().CostCentreId;

                SelectedWarehouse = Using<IItemsLookUp>(container).SelectDistribtrWarehouse(parentCostCentreId);//??default;
            
            }
        }

        #region Properties

        
        public const string SelectedWarehousePropertyName = "SelectedWarehouse";
        private Warehouse _selectedWarehouse = null;
        public Warehouse SelectedWarehouse
        {
            get
            {
                return _selectedWarehouse;
            }

            set
            {
                if (_selectedWarehouse == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedWarehousePropertyName);
                _selectedWarehouse = value;
                RaisePropertyChanged(SelectedWarehousePropertyName);
            }
        }



        /// <summary>
        /// The <see cref="ProductId" /> property's name.
        /// </summary>
        public const string ProductIdPropertyName = "ProductId";
        private Guid _ProductId = Guid.Empty;
        /// <summary>
        /// Gets the ProductId property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public Guid ProductId
        {
            get
            {
                return _ProductId;
            }

            set
            {
                if (_ProductId == value)
                {
                    return;
                }

                var oldValue = _ProductId;
                _ProductId = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(ProductIdPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ProductDescription" /> property's name.
        /// </summary>
        public const string ProductDescriptionPropertyName = "ProductDescription";
        private string _ProductDescription = null;
        /// <summary>
        /// Gets the ProductDescription property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string ProductDescription
        {
            get
            {
                return _ProductDescription;
            }

            set
            {
                if (_ProductDescription == value)
                {
                    return;
                }

                var oldValue = _ProductDescription;
                _ProductDescription = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(ProductDescriptionPropertyName);
            }
        }

        
        public const string GenericQuantityPropertyName = "GenericQuantity";
        private decimal _GenericQuantity = 0;
        public decimal GenericQuantity
        {
            get
            {
                return _GenericQuantity;
            }

            set
            {
                if (_GenericQuantity == value)
                {
                    return;
                }

                var oldValue = _GenericQuantity;
                _GenericQuantity = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(GenericQuantityPropertyName);
            }
        }
        public const string ReturnableQuantityPropertyName = "ReturnableQuantity";
        private int _returnableQuantity = 0;
        public int ReturnableQuantity
        {
            get
            {
                return _returnableQuantity;
            }

            set
            {
                if (_returnableQuantity == value)
                {
                    return;
                }

                _returnableQuantity = value;
             RaisePropertyChanged(ReturnableQuantityPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="SelectedProduct" /> property's name.
        /// </summary>
        public const string SelectedProductPropertyName = "SelectedProduct";
        private Product _SelectedProduct = null;
        /// <summary>
        /// Gets the SelectedProduct property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public Product SelectedProduct
        {
            get
            {
                return _SelectedProduct;
            }

            set
            {
                if (_SelectedProduct == value)
                {
                    return;
                }

                var oldValue = _SelectedProduct;
                _SelectedProduct = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(SelectedProductPropertyName);
            }
        }

        public const string OriginalGenericQuantityPropertyName = "OriginalGenericQuantity";
        private decimal _OriginalGenericQuantity = 0;
        public decimal OriginalGenericQuantity
        {
            get
            {
                return _OriginalGenericQuantity;
            }

            set
            {
                if (_OriginalGenericQuantity == value)
                {
                    return;
                }

                var oldValue = _OriginalGenericQuantity;
                _OriginalGenericQuantity = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(OriginalGenericQuantityPropertyName);
            }
        }
        #endregion

        private void ListGenericPageLoaded()
        {
           RunLoadGenericInventoryList();
        }

        void RunLoadInventoryList()
        {
            List<Inventory> inventories = null;
            using (IContainer container = NestedContainer)
            {
              
                if (SelectedWarehouse != null && SelectedWarehouse.Id != Guid.Empty)
                {
                    inventories =  Using<IInventoryRepository>(container).Query(SelectedWarehouse.Id);
                }
                else
                {
                    inventories = Using<IInventoryRepository>(container).Query(null);
                }
             
            }
            InventoryList.Clear();
            inventories.ForEach(n => InventoryList.Add(new ListInventoryItemViewModel
            {
                Quantity = n.Balance,
                ProductDescription = n.Product.Description,
                Id = n.Product.Id,
                InventoryOwner = n.Warehouse.Name
            }));
        }

        void RunGetGenericProduct()
        {
            
             using (IContainer container = NestedContainer)
             {
                 var inventoryService = Using<IInventoryRepository>(container);
                 IConfigService configService = Using<IConfigService>(container);
                 Inventory inv = inventoryService.GetByProductIdAndWarehouseId(ProductId, configService.Load().CostCentreId);
                 if (inv != null)
                 {
                     ProductDescription = inv.Product.Description;
                     OriginalGenericQuantity = GenericQuantity = inv.Balance;
                 }
             }
        }

        public void RunReturnableProducts()
        {
             using (IContainer container = NestedContainer)
             {
                 IProductRepository productService = Using<IProductRepository>(container);
                
            Products.Clear();
            Product product = productService.GetById(ProductId);
            //Load only returnable products that are not generics and that have the same packaging type and packaging as the selected generic product
            //if (product.Packaging == null || product.PackagingType == null)
            //    return;
            if (product.PackagingType == null)
                return;
            //productService.GetAll()
            //    .Where(n => n.ReturnableType == ReturnableType.Returnable
                            
            //                && n.PackagingType.Id == product.PackagingType.Id)
            //    .ToList()
            //    .ForEach(n => Products.Add(n));
                 Products.Clear();
                var resturnables=  Using<IProductRepository>(container).GetAll().OfType<ReturnableProduct>().Where(p=>p.ReturnableType !=ReturnableType.GenericReturnable)
                     .OrderBy(o => o.Description).ToList();
                 resturnables.ForEach(Products.Add);
                 

             }
        }

        void RunLoadGenericInventoryList()
        {
            using (IContainer container = NestedContainer)
            {
                IInventoryRepository inventoryService = Using<IInventoryRepository>(container);
                List<Inventory> inventoryservice = inventoryService.GetAll()
                    .Where(n => n.Product.ReturnableType == ReturnableType.GenericReturnable).ToList();
                InventoryList.Clear();
                inventoryservice.ForEach(n => InventoryList.Add(new ListInventoryItemViewModel
                                                                    {
                                                                        Quantity = n.Balance == null ? 0 : n.Balance,
                                                                        ProductDescription = n.Product.Description,
                                                                        Id = n.Product.Id
                                                                    }));
            }
        }

        void ConfirmReconciliation()
        {
            using (IContainer container = NestedContainer)
            {
                IInventoryRepository inventoryService = Using<IInventoryRepository>(container);
                ICostCentreRepository costCentreService = Using<ICostCentreRepository>(container);
                IConfigService configService = Using<IConfigService>(container);
                Config config = configService.Load();
                IProductRepository productService = Using<IProductRepository>(container);
                IUserRepository userService = Using<IUserRepository>(container);
                IInventoryAdjustmentNoteWfManager confirmInventoryAdjustmentNoteWFManager = Using<IInventoryAdjustmentNoteWfManager>(container);
                var cc = costCentreService.GetById(config.CostCentreId);
                Guid appId = config.CostCentreApplicationId;
                var ian = Using<IInventoryAdjustmentNoteFactory>(container)
                    .Create(cc, appId, cc, userService.GetById(configService.ViewModelParameters.CurrentUserId),
                            "Reconcile Generics", InventoryAdjustmentNoteType.Available, Guid.Empty);
                ian.Confirm();
                ian.DocumentParentId = ian.Id;
                foreach (ListInventoryItemViewModel item in LineItems)
                {
                    var inventory = inventoryService.GetByProductIdAndWarehouseId(item.Id, ian.DocumentIssuerCostCentre.Id);
                    decimal Expected = 0;
                    if (inventory != null)
                        Expected = inventory.Balance;
                    var li = Using<IInventoryAdjustmentNoteFactory>(container)
                        .CreateLineItem(item.Quantity + Expected, item.Id, Expected, 0, "Reconcile Generics");
                    ian.AddLineItem(li);
                }
                confirmInventoryAdjustmentNoteWFManager.SubmitChanges(ian,config);
                LineItems.Clear();
                Cleanup();
                ListGenericPageLoaded();
            }
        }

        public void AddLineItem(Guid productId, string productdescription, int qty)
        {
            if (LineItems.Any(p => p.Id == productId))
            {
                var liupdate = LineItems.First(p => p.Id == productId);
                liupdate.Quantity = qty;
            }
            else
            {
                ListInventoryItemViewModel li = new ListInventoryItemViewModel
                {
                    Quantity = qty,
                    Id = productId,
                    ProductDescription = productdescription
                };
                LineItems.Add(li);
            }
        }

        public void RemoveLineItem(Guid productId)
        {
            if (LineItems.Any(p => p.Id == productId))
            {
                var liupdate = LineItems.First(p => p.Id == productId);
                GenericQuantity += liupdate.Quantity;
                LineItems.Remove(liupdate);
            }
        }
        
        private void Reconcile(ListInventoryItemViewModel model)
        {
            const string uri = "/views/ReconcileGenerics/ReconcileGenerics.xaml";
            ProductId = model.Id;
            NavigateCommand.Execute(uri);
        }
        private void PrintReport(object param)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == false)
                return;

            
            string documentTitle =string.Format("Available Inventory List-{0}",DateTime.Now.ToLocalTime());
            Size pageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);

            var grid= param as DataGrid;
            var removed = grid.Columns.FirstOrDefault(n => n.GetType() == typeof(DataGridTemplateColumn));//go:remove action links
            if (removed != null)
                grid.Columns.Remove(removed);
            var paginator = new CustomDataGridDocumentPaginator(grid, documentTitle, pageSize, new Thickness(30, 20, 30, 20));

            printDialog.PrintDocument(paginator, "Grid");
        }

        private void ReconCilePageLoaded()
        {
            if(ProductId !=Guid.Empty)
            {
                GenericQuantity = 0;
                OriginalGenericQuantity = 0;
                RunGetGenericProduct();
                RunReturnableProducts();
            }
            
        }

        void CleanUp()
        {
            GenericQuantity = 0;
            OriginalGenericQuantity = 0;
            SelectedProduct = null;
            ProductDescription = null;
            ProductId = Guid.Empty;
            LineItems.Clear();
            Products.Clear();
            InventoryList.Clear();
        }
    }

    public class ListInventoryItemViewModel : ViewModelBase
    {
        public const string InventoryOwnerPropertyName = "InventoryOwner";
        private string _InventoryOwner = null;
        public string InventoryOwner
        {
            get
            {
                return _InventoryOwner;
            }

            set
            {
                if (_InventoryOwner == value)
                {
                    return;
                }

                var oldValue = _InventoryOwner;
                _InventoryOwner = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(InventoryOwnerPropertyName);
            }
        }

        public const string ProductDescriptionPropertyName = "ProductDescription";
        private string _ProductDescription = null;
        public string ProductDescription
        {
            get
            {
                return _ProductDescription;
            }

            set
            {
                if (_ProductDescription == value)
                {
                    return;
                }

                var oldValue = _ProductDescription;
                _ProductDescription = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(ProductDescriptionPropertyName);
            }
        }

        public const string QuantityPropertyName = "Quantity";
        private decimal _Quantity = 0;
        public decimal Quantity
        {
            get
            {
                return _Quantity;
            }

            set
            {
                if (_Quantity == value)
                {
                    return;
                }

                var oldValue = _Quantity;
                _Quantity = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(QuantityPropertyName);
            }
        }

        public const string IdPropertyName = "Id";
        private Guid _Id = Guid.Empty;
        public Guid Id
        {
            get
            {
                return _Id;
            }

            set
            {
                if (_Id == value)
                {
                    return;
                }

                var oldValue = _Id;
                _Id = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(IdPropertyName);
            }
        }
    }
}
