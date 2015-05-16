using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.WPF.Lib.UI.Pages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Transactional.InventoryTransfer
{
    public class InventoryLevelViewModel : DistributrViewModelBase
    {

        public ObservableCollection<InventoryLevelLineItemViewModel> LineItem { get; set; }
        public RelayCommand LoadCommand { get; set; }
        public RelayCommand SelectWarehouseCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }
        public RelayCommand ViewInventoryCommand { get; set; }
        public InventoryLevelViewModel()
        {
            LineItem = new ObservableCollection<InventoryLevelLineItemViewModel>();
            LoadCommand = new RelayCommand(Load);
            SelectWarehouseCommand = new RelayCommand(SelectWarehouse);
            ViewInventoryCommand = new RelayCommand(ViewInventory);
            ClearCommand = new RelayCommand(Clear);
        }

        private void ViewInventory()
        {
           
            if(SelectedWarehouse.Id == Guid.Empty)
            {
                MessageBox.Show("Select warehouse to proceed");
                return;
            }
            LineItem.Clear();
            using (var c = NestedContainer)
            {
                var Items = Using<ISourcingInventoryRepository>(c).GetByWareHouseId(SelectedWarehouse.Id);
                foreach (SourcingInventory item in Items)
                {
                    LineItem.Add(new InventoryLevelLineItemViewModel
                    {
                        Balance = item.Balance,
                        Commodity = item.Commodity,
                        CommodityGrade = item.Grade,
                        Warehouse = item.Warehouse
                    });
                }
            }
        }

        private void Clear()
        {
            Load();
        }

        private void SelectWarehouse()
        {
            
            using (var container = NestedContainer)
            {
                var selected = Using<IAgriItemsLookUp>(container).SelectWarehouse();

                SelectedWarehouse = selected;
                if (selected == null)
                {
                    SelectedWarehouse = new Store(Guid.Empty) { Name = "--Select Warehouse---" };
                }
            }
        }

        private void Load()
        {
            SelectedWarehouse = new Store(Guid.Empty) { Name = "--Select Warehouse---" };
            LineItem.Clear();
            using (var c = NestedContainer)
            {
                var Items = Using<ISourcingInventoryRepository>(c).GetAll();
                foreach (SourcingInventory item in Items)
                {
                    LineItem.Add(new InventoryLevelLineItemViewModel
                                     {
                                         Balance=item.Balance,
                                         Commodity=item.Commodity,
                                         CommodityGrade=item.Grade,
                                         Warehouse=item.Warehouse
                                     } );
                }
            }
        }

       
        public const string SelectedWarehousePropertyName = "SelectedWarehouse";
        private Warehouse _selecetdWarehouse = null;
        public Warehouse SelectedWarehouse
        {
            get
            {
                return _selecetdWarehouse;
            }

            set
            {
                if (_selecetdWarehouse == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedWarehousePropertyName);
                _selecetdWarehouse = value;
                RaisePropertyChanged(SelectedWarehousePropertyName);
            }
        }
    }
    public class InventoryLevelLineItemViewModel : ViewModelBase
    {
       
        public const string CommodityPropertyName = "Commodity";
        private Commodity _commodity = null;
        public Commodity Commodity
        {
            get
            {
                return _commodity;
            }

            set
            {
                if (_commodity == value)
                {
                    return;
                }

                RaisePropertyChanging(CommodityPropertyName);
                _commodity = value;
                RaisePropertyChanged(CommodityPropertyName);
            }
        }

       
        public const string WarehousePropertyName = "Warehouse";
        private Warehouse _warehouse = null;
        public Warehouse Warehouse
        {
            get
            {
                return _warehouse;
            }

            set
            {
                if (_warehouse == value)
                {
                    return;
                }

                RaisePropertyChanging(WarehousePropertyName);
                _warehouse = value;
                RaisePropertyChanged(WarehousePropertyName);
            }
        }
       
        public const string CommodityGradePropertyName = "CommodityGrade";
        private CommodityGrade _grade = null;
        public CommodityGrade CommodityGrade
        {
            get
            {
                return _grade;
            }

            set
            {
                if (_grade == value)
                {
                    return;
                }

                RaisePropertyChanging(CommodityGradePropertyName);
                _grade = value;
                RaisePropertyChanged(CommodityGradePropertyName);
            }
        }

       
        public const string BalancePropertyName = "Balance";
        private decimal _balance = 0;
        public decimal Balance
        {
            get
            {
                return _balance;
            }

            set
            {
                if (_balance == value)
                {
                    return;
                }

                RaisePropertyChanging(BalancePropertyName);
                _balance = value;
                RaisePropertyChanged(BalancePropertyName);
            }
        }
    } 
}