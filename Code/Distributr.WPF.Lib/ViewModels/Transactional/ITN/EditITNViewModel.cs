using System;
using System.Windows;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Utility;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using System.Linq;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using System.Collections.Generic;
using Distributr.Core.ClientApp;
using GalaSoft.MvvmLight.Command;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.InventoryEntities;

namespace Distributr.WPF.Lib.ViewModels.Transactional.ITN
{
    public class EditITNViewModel : DistributrViewModelBase
    {
        public ObservableCollection<EditItnViewModelInventoryTransfer> InventoryTransferProducts { get; set; }
        InventoryTransferNote note = null;
        public ObservableCollection<EditItnViewModelInventoryTransfer> LineItems { set; get; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand ConfirmCommand { get; set; }
        public RelayCommand LoadSalesMenCommand { get; set; }
        public RelayCommand PickSelectedSalesmanCommand { get; set; }
        public ObservableCollection<User> SalesMen { get; set; }
        
        private List<ProductSerialNumbers> _inventorySerialNumbersList;
        

        public EditITNViewModel()
        {
            LineItems = new ObservableCollection<EditItnViewModelInventoryTransfer>();
            
            CancelCommand = new RelayCommand(CancelITN);
            SaveCommand = new RelayCommand(RunSaveITNCommand);
            ConfirmCommand = new RelayCommand(ConfirmCommandITN);
            LoadSalesMenCommand = new RelayCommand(LoadSalesMen);
           
            _inventorySerialNumbersList = new List<ProductSerialNumbers>();
           
            SalesMen = new ObservableCollection<User>();
            
            
        }


        public void ClearViewModel()
        {
            LineItems.Clear();
            SelectedSaleMan = null;
            IsEdit = false;
            _productPackagingSummaryService.ClearBuffer();
        }

        public void ClearBuffer()
        {
            _productPackagingSummaryService.ClearBuffer();
        }

        public void LoadSalesMen()
        {
            using (var container = NestedContainer)
            {

                IUserRepository _userService = Using<IUserRepository>(container);

                var temp = SelectedSaleMan;
                SelectedSaleMan = null;
                SalesMen.Clear();
                var salesman = new User(Guid.Empty)
                                   {
                                       Username =
                                           "--Please Select a " + GetLocalText("sl.issueInventory.salesman_lbl") + "--"
                                   };
                    //Salesman
                SalesMen.Add(salesman);
                _userService.GetAll()
                  .Where(n => n.UserType == UserType.DistributorSalesman)
                    .OrderBy(n => n.Username)
                    .ToList()
                    .ForEach(n => SalesMen.Add(n));
                SelectedSaleMan = temp ?? salesman;
            }
        }
        
        void RunSaveITNCommand()
        {
            SaveITN();
        }

        private bool SaveITN()
        {
            using (var container = NestedContainer)
            {

                ICostCentreRepository _costCentreService = Using<ICostCentreRepository>(container);
               
                IConfigService _configService = Using<IConfigService>(container);
                IProductRepository _productService = Using<IProductRepository>(container);
                IInventorySerialsWorkFlow _inventorySerialsService = Using<IInventorySerialsWorkFlow>(container);
                IInventoryRepository _inventoryService = Using<IInventoryRepository>(container);
                IInventoryTransferNoteFactory _inventoryTransferNoteFactory = Using<IInventoryTransferNoteFactory>(container);
                if (LineItems.Count <= 0)
                {
                    MessageBox.Show("Add products to issue first");
                    return false;
                }
                string msg = "";
                var transferTo = _costCentreService.GetById(SelectedSaleMan.CostCentre);
                var transferFrom = _costCentreService.GetById(_configService.Load().CostCentreId);
                note = null;
                foreach (var item in LineItems)
                {
                    var inventory = _inventoryService.GetByProductIdAndWarehouseId(item.ProductId, transferFrom.Id);
                    if (inventory == null)
                    {
                        msg += "Product : " + item.ProductName + " Required(" + item.Qty +
                               ") inventory is not available(0)\n";
                    }
                    else if (inventory != null && inventory.Balance < item.Qty)
                    {
                        msg += "Product : " + item.ProductName + " Required(" + item.Qty +
                               ") inventory is not available(" + inventory.Balance + ")\n";
                    }
                }
                if (msg != "")
                {
                    MessageBox.Show("Make sure you have this inventory\n" + msg, "Inventory Transfer",
                                    MessageBoxButton.OK);
                    return false;
                }
                var cc = _costCentreService.GetById(_configService.Load().CostCentreId);
                note = _inventoryTransferNoteFactory.Create(cc,
                                                            _configService.Load().CostCentreApplicationId,
                                                            SelectedSaleMan, transferTo, cc, "");

                List<InventoryTransferNoteLineItem> ListitnLineitem = new List<InventoryTransferNoteLineItem>();
                foreach (EditItnViewModelInventoryTransfer item in LineItems)
                {
                    InventoryTransferNoteLineItem itnLineitem = _inventoryTransferNoteFactory.CreateLineItem(item.ProductId, item.Qty, 0, 0, "");
                    ListitnLineitem.Add(itnLineitem);
                }

                foreach (var i in ListitnLineitem)
                {
                    note.AddLineItem(i);
                }
                //note._SetLineItems(ListitnLineitem);
                var list = _inventorySerialNumbersList
                    .Select(n => new InventorySerials(n.SerialsId)
                                     {
                                         CostCentreRef = new CostCentreRef {Id = note.DocumentRecipientCostCentre.Id},
                                         DocumentId = note.Id,
                                         ProductRef = new ProductRef {ProductId = n.ProductId,},
                                         From = n.From,
                                         To = n.To
                                     }).ToList();
                _inventorySerialsService.SubmitInventorySerials(list);
                return true;
            }
        }

        public void CancelITN()
        {
            if (MessageBox.Show("Are you sure you want to cancel?", "Inventory Transfer", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ClearViewModel();
                ConfirmNavigatingAway = false;
                SendNavigationRequestMessage(new Uri("/views/Reports/InventoryIssuesReport.xaml", UriKind.Relative));
            }
        }

        public void ConfirmCommandITN()
        {
            using (var container = NestedContainer)
            {
                BasicConfig config = container.GetInstance<IConfigService>().Load();
                IConfirmInventoryTransferNoteWFManager _confirmInventoryTransferNoteWFManager =
                    Using<IConfirmInventoryTransferNoteWFManager>(container);
               
                if (SaveITN())
                    if (note != null && note.LineItems.Any())
                    {
                        note.Confirm();
                        _confirmInventoryTransferNoteWFManager.SubmitChanges(note,config);
                        note = null;
                        ClearViewModel();
                        MessageBox.Show("Successfully Confirmed", "Information", MessageBoxButton.OK);
                        ConfirmNavigatingAway = false;
                        SendNavigationRequestMessage(new Uri("/views/Reports/InventoryIssuesReport.xaml",
                                                             UriKind.Relative));
                    }
                    else
                    {
                        throw new Exception("Inventory wasn't tranfered ");
                    }
            }
        }

        public void AddLineItem(Guid productId, string productdescription, int qty)
        {
            using (var container = NestedContainer)
            {
               
                IConfigService _configService = Using<IConfigService>(container);
               
                IInventoryRepository _inventoryService = Using<IInventoryRepository>(container);
                if (LineItems.Any(p => p.ProductId == productId))
                {
                    var liupdate = LineItems.First(p => p.ProductId == productId);
                    if (!IsEdit)
                    {
                        Inventory inv = _inventoryService.GetByProductIdAndWarehouseId(productId, _configService.Load().CostCentreId);
                        if (inv != null)
                        {
                            if ((liupdate.Qty + qty) > inv.Balance)
                                MessageBox.Show("You dont have enough inventory");
                            else
                                liupdate.Qty += qty;
                        }
                    }
                    else
                        liupdate.Qty = qty;
                    IsEdit = false;
                }
                else
                {
                    EditItnViewModelInventoryTransfer li = new EditItnViewModelInventoryTransfer
                                                               {
                                                                   Qty = qty,
                                                                   ProductId = productId,
                                                                   ProductName = productdescription
                                                               };
                    LineItems.Add(li);
                }
            }
        }

        public void Reload(List<PackagingSummary> lineitems, bool isNew, List<ProductSerialNumbers> inventorySerials)
        {
            foreach (PackagingSummary ps in lineitems)
            {

                if (ps.IsEditable)
                {
                    _productPackagingSummaryService.AddProduct(ps.Product.Id, ps.Quantity, false, isNew, true);
                }

            }
            RefreshList();
            inventorySerials.ForEach(_inventorySerialNumbersList.Add);
        }

        private void RefreshList()
        {
            List<PackagingSummary> currentList = _productPackagingSummaryService.GetProductSummary();
            LineItems.Clear();

            foreach (var item in currentList)
            {

                //check if auto added items are in inventory
                decimal invBalance = 0m;
                decimal lineItemsQty = LineItems.Where(n => n.ProductId == item.Product.Id).Sum(n => n.Qty);
                decimal balanceAfterLineItems = 0m;

                bool inStock = _productPackagingSummaryService.IsProductInStock(
                    GetConfigParams().CostCentreId,
                    item.Product.Id,
                    lineItemsQty,
                    item.Quantity,
                    out invBalance);
                if (!inStock)
                {
                    MessageBox.Show(
                        "Product " + item.Product.Description +
                        " is out of stock.\nThe product will not be added.\nThe required quantity is " +
                        item.Quantity,
                        "Distributr: Issue Inventory Module", MessageBoxButton.OK);
                }
                else
                {
                    balanceAfterLineItems = invBalance - lineItemsQty;
                    if (balanceAfterLineItems < item.Quantity)
                    {
                        MessageBox.Show(
                            "The available inventory of " + balanceAfterLineItems + " units cannot cover the " +
                            item.Quantity + " units for product " + item.Product.Description + ".\n" +
                            "" + balanceAfterLineItems + " units will be added.",
                            "Distributr: Issue Inventory Module", MessageBoxButton.OK);
                        item.Quantity = balanceAfterLineItems;
                    }
                    if (item.Quantity > 0)
                    {
                        LineItems
                            .Add(new EditItnViewModelInventoryTransfer
                                     {
                                         Qty = item.Quantity,
                                         ProductId = item.Product.Id,
                                         ProductName = item.Product.Description,
                                         Editable = item.IsEditable
                                     });
                    }
                }
            }
        }

        public List<ProductSerialNumbers> GetProductSerials(Guid productId)
        {
            var items = _inventorySerialNumbersList.Where(n => n.ProductId == productId).ToList();
            return items;
        }

        public void RemoveLineItem(Guid productId)
        {
            if (
                MessageBox.Show("Are you sure you want to delete the selected product?", "Inventory Transfer",
                                MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {

                _productPackagingSummaryService.RemoveProduct(productId);
                RefreshList();
            }
        }

        public const string SelectedSaleManPropertyName = "SelectedSaleMan";
        private User _SelectedSaleMan;
        public User SelectedSaleMan
        {
            get
            {
                return _SelectedSaleMan;
            }

            set
            {
                if (_SelectedSaleMan == value)
                {
                    return;
                }

                var oldValue = _SelectedSaleMan;
                _SelectedSaleMan = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(SelectedSaleManPropertyName);
            }
        }
        
        public const string IsEditPropertyName = "IsEdit";
        private bool _IsEdit;
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

        public class EditItnViewModelInventoryTransfer : ViewModelBase
        {
            public const string ProductIdPropertyName = "ProductId";
            private Guid _productId= Guid.Empty;
            public Guid ProductId
            {
                get
                {
                    return _productId;
                }

                set
                {
                    if (_productId == value)
                    {
                        return;
                    }

                    var oldValue = _productId;
                    _productId = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(ProductIdPropertyName);
                }
            }

            public const string ProductNamePropertyName = "ProductName";
            private string _productName = "";
            public string ProductName
            {
                get
                {
                    return _productName;
                }

                set
                {
                    if (_productName == value)
                    {
                        return;
                    }

                    var oldValue = _productName;
                    _productName = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(ProductNamePropertyName);
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

                    // Update bindings, no broadcast
                    RaisePropertyChanged(QtyPropertyName);
                }
            }
            
            public const string EditablePropertyName = "Editable";
            private bool _editable;
            public bool Editable
            {
                get
                {
                    return _editable;
                }

                set
                {
                    if (_editable == value)
                    {
                        return;
                    }

                    var oldValue = _editable;
                    _editable = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(EditablePropertyName);

                   
                }
            }
           
        }
    }

    public class ITNSalesManLookupItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
