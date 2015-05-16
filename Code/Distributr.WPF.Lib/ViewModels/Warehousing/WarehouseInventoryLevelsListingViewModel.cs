using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WPF.Lib.Messages;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Admin;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Distributr.WPF.Lib.ViewModels.Warehousing
{
    public class WarehouseInventoryLevelsListingViewModel :ListingsViewModelBase
    {
        private PagenatedList<CommoditySupplierInventoryLevel> _pagedCommoditySupplierInventoryLevel;
        public WarehouseInventoryLevelsListingViewModel()
        {
            WarehouseInventoryLevelsList = new ObservableCollection<WarehouseInventoryLevelsDocumentListItem>();
            CommoditySuppliersList=new ObservableCollection<CommoditySupplier>();
            StoresList=new ObservableCollection<Store>();

            LoadListingPageCommand = new RelayCommand(LoadPage);
            CommoditySupplierChangedCommand=new RelayCommand(CommmoditySupplierChanged);
            StoreChangedCommand=new RelayCommand(StoreChanged);
            ClearCommand = new RelayCommand(ClearFilters);
            RowNumber=0;
            QueryCommoditySupplierInventoryLevel = null;
            IsSet = false;
        }

       

        #region Members
        public ObservableCollection<WarehouseInventoryLevelsDocumentListItem> WarehouseInventoryLevelsList { get; set; }
        public ObservableCollection<CommoditySupplier> CommoditySuppliersList { get; set; }
        public ObservableCollection<Store>StoresList { get; set; }
        public RelayCommand LoadListingPageCommand { get; set; }

        private bool IsSet { get; set; }


        protected override void Load(bool isFirstLoad = false)
        {
            LoadCommoditySuppliers();
            LoadStores();
            LoadCommoditySupplierInventoryList(IsSet);
        }

        protected override void EditSelected()
        {
            throw new NotImplementedException();
        }

        protected override void ActivateSelected()
        {
            throw new NotImplementedException();
        }

        protected override void DeleteSelected()
        {
            throw new NotImplementedException();
        }

        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, _pagedCommoditySupplierInventoryLevel.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            LoadPage();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(_pagedCommoditySupplierInventoryLevel.PageNumber, _pagedCommoditySupplierInventoryLevel.PageCount, _pagedCommoditySupplierInventoryLevel.TotalItemCount,
                                      _pagedCommoditySupplierInventoryLevel.IsFirstPage, _pagedCommoditySupplierInventoryLevel.IsLastPage);
        }

        public RelayCommand CommoditySupplierChangedCommand { get; set; }
        public RelayCommand StoreChangedCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }
        private QueryCommoditySupplierInventory QueryCommoditySupplierInventoryLevel { get; set; }
        public int RowNumber;

        #endregion

        #region Methods

        private void LoadPage()
        {
            LoadCommoditySuppliers();
            LoadStores();
            LoadCommoditySupplierInventoryList(IsSet);
            ClearFilters();
        }

        private void ClearFilters()
        {
            IsSet = false;
            LoadCommoditySupplierInventoryList(IsSet);

            SelectedCommoditySupplier = new CommoditySupplier(Guid.Empty) { Name = "--Select Commodity Supplier---" };
            SelectedCommoditySupplierName = "--Select Commdity Supplier---";

            SelectedStore = new Store(Guid.Empty) { Name = "--Select Store---" };
            SelectedStoreName = "--Select Store---";

            UpdatePagenationControl();

        }

        private void LoadCommoditySupplierInventoryList(bool isSet)
        {
            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                var query = new QueryCommoditySupplierInventory();
                query.Skip = ItemsPerPage * (CurrentPage - 1);
                query.Take = ItemsPerPage;
                if(isSet)
                {
                    query = QueryCommoditySupplierInventoryLevel;
                }
                //else
                //{
                //    query.Skip = 0;
                //    query.Take = 5;
                //}




                var rawList = c.GetInstance<ICommoditySupplierInventoryRepository>().Query(query);
                var data = rawList.Data.OfType<CommoditySupplierInventoryLevel>();
                WarehouseInventoryLevelsList.Clear();
                RowNumber=1;
                //rawList.ForEach(n => WarehouseInventoryLevelsList.Add(Map(n)));

                _pagedCommoditySupplierInventoryLevel = new PagenatedList<CommoditySupplierInventoryLevel>(data.AsQueryable(),
                                                                                         CurrentPage,
                                                                                         ItemsPerPage,
                                                                                         rawList.Count, true);

                _pagedCommoditySupplierInventoryLevel.ForEach(n => WarehouseInventoryLevelsList.Add(Map(n)));
                UpdatePagenationControl();
               
            }    
        }

        private void LoadCommoditySuppliers()
        {
            CommoditySuppliersList.Clear();
             using (StructureMap.IContainer c = NestedContainer)
            {

                var commoditySuppliers = Using<ICommoditySupplierRepository>(c).GetAll().OfType<CommoditySupplier>().OrderBy(n => n.Name).ThenBy(n => n.CostCentreCode).ToList();
                commoditySuppliers.ForEach(n => { if (CommoditySuppliersList.Select(q => q.Id).All(p => p != n.Id)) CommoditySuppliersList.Add(n); });

                if (CommoditySuppliersList.Count(p => p.Id != Guid.Empty) == 1 )
                {
                    SelectedCommoditySupplier = CommoditySuppliersList.FirstOrDefault(p => p.Id != Guid.Empty);
                    
                }

            }
        }

        private void LoadStores()
        {
            StoresList.Clear();
            using (StructureMap.IContainer c = NestedContainer)
            {

                var stores = Using<IStoreRepository>(c).GetAll().OfType<Store>().OrderBy(n => n.Name).ThenBy(n => n.CostCentreCode).ToList();
                stores.ForEach(n => { if (StoresList.Select(q => q.Id).All(p => p != n.Id)) StoresList.Add(n); });

                if (StoresList.Count(p => p.Id != Guid.Empty) == 1)
                {
                    SelectedStore = StoresList.FirstOrDefault(p => p.Id != Guid.Empty);

                }

            }
        }

        private void CommmoditySupplierChanged()
        {
            using (var container = NestedContainer)
            {
                var selected = Using<IAgriItemsLookUp>(container).SelectCommoditySupplier();

                SelectedCommoditySupplier = selected;
                if (selected == null)
                {
                    SelectedCommoditySupplier = new CommoditySupplier(Guid.Empty) { Name = "--Select Commodity Supplier---" };
                    SelectedCommoditySupplierName = "--Select Commdity Supplier---";

                    var query = new QueryCommoditySupplierInventory();
                    
                    if (SelectedStore.Id != Guid.Empty)
                    {
                        query.StoreId = SelectedStore.Id;
                    }
                    QueryCommoditySupplierInventoryLevel = query;
                    IsSet = true;
                    LoadCommoditySupplierInventoryList(IsSet);
                }
                else
                {

                    SelectedCommoditySupplierName = SelectedCommoditySupplier.Name;

                    var query = new QueryCommoditySupplierInventory();
                    query.CommoditySupplierId = selected.Id;
                    if(SelectedStore.Id!=Guid.Empty)
                    {
                        query.StoreId = SelectedStore.Id;
                    }
                    QueryCommoditySupplierInventoryLevel = query;
                    IsSet = true;
                    LoadCommoditySupplierInventoryList(IsSet);
                }
            }
        }

        private void StoreChanged()
        {
            using (var container = NestedContainer)
            {
                var selected = Using<IAgriItemsLookUp>(container).SelectStore();

                SelectedStore = selected;
                if (selected == null)
                {
                    SelectedStore = new Store(Guid.Empty) { Name = "--Select Store---" };
                    SelectedStoreName = "--Select Store---";

                    var query = new QueryCommoditySupplierInventory();
                   
                    if (SelectedCommoditySupplier.Id != Guid.Empty)
                    {
                        query.CommoditySupplierId = SelectedCommoditySupplier.Id;
                    }
                    QueryCommoditySupplierInventoryLevel = query;
                    IsSet = true;
                    LoadCommoditySupplierInventoryList(IsSet);
                }
                else
                {

                    SelectedStoreName = SelectedStore.Name;

                    var query = new QueryCommoditySupplierInventory();
                    query.StoreId = selected.Id;

                    if (SelectedCommoditySupplier.Id != Guid.Empty)
                    {
                        query.CommoditySupplierId = SelectedCommoditySupplier.Id;
                    }
                    QueryCommoditySupplierInventoryLevel = query;
                    IsSet = true;
                    LoadCommoditySupplierInventoryList(IsSet);
                }
            }
        }

        private WarehouseInventoryLevelsDocumentListItem Map(CommoditySupplierInventoryLevel document)
        {


            var item = new WarehouseInventoryLevelsDocumentListItem();
            item.Id = document.Id;
            item.RowNumber = RowNumber;
            item.CommoditySupplier = document.CommoditySupplier;
            item.Store = document.Warehouse;
            item.Balance = document.Balance;
            item.Commodity = document.Commodity;
            item.CommodityGrade = document.Grade;
          
            //using(var c = ObjectFactory.Container.GetNestedContainer())
            //{
            //    var commodityOwner = c.GetInstance<ICommodityOwnerRepository>().GetById(document.CommodityOwnerId);
            //    if(commodityOwner!=null)
            //    {
            //        item.CommodityOwner = commodityOwner.FullName;
            //    } 
            //}

            RowNumber++;
            return item;
        }


       
        #endregion

        #region Properties
        public const string SelectedInventoryItemPropertyName = "SelectedWarehouseListing";
        private WarehouseInventoryLevelsDocumentListItem _selectedInventoryItem = null;
        public WarehouseInventoryLevelsDocumentListItem SelectedInventoryItem
        {
            get
            {
                return _selectedInventoryItem;
            }

            set
            {
                if (_selectedInventoryItem == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedInventoryItemPropertyName);
                _selectedInventoryItem = value;
                RaisePropertyChanged(SelectedInventoryItemPropertyName);
            }
        }

        public const string SelectedCommoditySupplierNamePropertyName = "SelectedCommoditySupplierName";
        private string _selectedCommoditySupplierName = "--Select Commodity Supplier---";
        public string SelectedCommoditySupplierName
        {
            get
            {
                return _selectedCommoditySupplierName;
            }

            set
            {
                if (_selectedCommoditySupplierName == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCommoditySupplierNamePropertyName);
                _selectedCommoditySupplierName = value;
                RaisePropertyChanged(SelectedCommoditySupplierNamePropertyName);
            }
        }

        public const string SelectedCommoditySupplierPropertyName = "SelectedCommoditySupplier";
        private CommoditySupplier _selectedCommoditySupplier = new CommoditySupplier(Guid.Empty) { Name = "--Select Commodity Supplier---" };
        public CommoditySupplier SelectedCommoditySupplier
        {
            get
            {
                return _selectedCommoditySupplier;
            }

            set
            {
                if (_selectedCommoditySupplier == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCommoditySupplierPropertyName);
                _selectedCommoditySupplier = value;
                RaisePropertyChanged(SelectedCommoditySupplierPropertyName);
            }
        }

        public const string SelectedStoreNamePropertyName = "SelectedStoreName";
        private string _selectedStoreName = "--Select Store---";
        public string SelectedStoreName
        {
            get
            {
                return _selectedStoreName;
            }

            set
            {
                if (_selectedStoreName == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedStoreNamePropertyName);
                _selectedStoreName = value;
                RaisePropertyChanged(SelectedStoreNamePropertyName);
            }
        }

        public const string SelectedStorePropertyName = "SelectedStore";
        private Store _selectedStore = new Store(Guid.Empty) { Name = "--Select Store---" };
        public Store SelectedStore
        {
            get
            {
                return _selectedStore;
            }

            set
            {
                if (_selectedStore == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedStorePropertyName);
                _selectedStore = value;
                RaisePropertyChanged(SelectedStorePropertyName);
            }
        }
        #endregion

    }

    public class WarehouseInventoryLevelsDocumentListItem
    {
        public int RowNumber { get; set; }
        public Guid Id { get; set; }
        public string CommoditySupplier { get; set; }
        public string Store{ get; set; }
        public string Commodity { get; set; }
        public string CommodityGrade { get; set; }
        public string Balance { get; set; }

    }

   
}
