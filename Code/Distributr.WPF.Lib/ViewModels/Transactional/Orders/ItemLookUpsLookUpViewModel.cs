using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.ViewModels.Admin.SalesmanTargets;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Orders
{
    public class ItemLookUpsLookUpViewModel : DistributrViewModelBase
    {
        public object Param1 { set; get; }
        public object Param2 { set; get; }
        public string LoadedEntity { get; set; }
        public event EventHandler RequestClose = (s, e) => { };

        public ItemLookUpsLookUpViewModel()
        {
            DefaultItemsCollection = new ObservableCollection<ComboPopUpItem>();
            DefaultEnumItemsCollection = new ObservableCollection<ComboPopUpEnumItem>();
        }

        #region Commands and Collections

        public ObservableCollection<ComboPopUpItem> DefaultItemsCollection { get; set; }
        public ObservableCollection<ComboPopUpEnumItem> DefaultEnumItemsCollection { get; set; }

        private RelayCommand _cancelCommand;

        public RelayCommand CancelCommand

        #endregion


        #region Properties

        {
            get { return _cancelCommand ?? (_cancelCommand = new RelayCommand(Cancel)); }
        }

        private RelayCommand _searchCommand;

        public RelayCommand SearchCommand
        {
            get { return _searchCommand ?? (_searchCommand = new RelayCommand(Search)); }
        }



        public const string SelectedItemPropertyName = "SelectedItem";
        private ComboPopUpItem _selectedItem = null;

        public ComboPopUpItem SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                if (_selectedItem == value)
                {
                    return;
                }

                _selectedItem = value;

                RaisePropertyChanged(SelectedItemPropertyName);
            }
        }

        public const string SelectedEnumItemPropertyName = "SelectedEnumItem";
        private ComboPopUpEnumItem _selectedEnumItem = null;

        public ComboPopUpEnumItem SelectedEnumItem
        {
            get { return _selectedEnumItem; }

            set
            {
                if (_selectedEnumItem == value)
                {
                    return;
                }

                _selectedEnumItem = value;

                RaisePropertyChanged(SelectedEnumItemPropertyName);
            }
        }

        public const string CodeVisiblePropertyName = "CodeVisible";
        private bool _codeVisible = false;

        public bool CodeVisible
        {
            get { return _codeVisible; }

            set
            {
                if (_codeVisible == value)
                {
                    return;
                }

                _codeVisible = value;

                RaisePropertyChanged(CodeVisiblePropertyName);
            }
        }

        public const string SearchTextPropertyName = "SearchText";
        private string _searchText = "";

        public string SearchText
        {
            get { return _searchText; }

            set
            {
                if (_searchText == value)
                {
                    return;
                }

                _searchText = value;
                RaisePropertyChanged(SearchTextPropertyName);
            }
        }

        public const string IsFreeOfChargeSalePropertyName = "IsFreeOfChargeSale";
        private bool _isfree = false;

        public bool IsFreeOfChargeSale
        {
            get { return _isfree; }

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

        #endregion


        #region Methods

        private void Cancel()
        {
            SelectedItem = null;
            RequestClose(this, EventArgs.Empty);
        }

        private void Search()
        {
            if (!string.IsNullOrEmpty(SearchText))
                SearchText = SearchText.ToLower();

            LoadComboItems(defaultTake);

        }

        public void SetUp()
        {
            DefaultItemsCollection.Clear();
            SearchText = "";

        }


        private void Loadproducts()
        {
            Guid? salesmanId = Param1 as Guid?;
            using (var c = NestedContainer)
            {
                DefaultItemsCollection.Clear();

                if (issueInventoryProducts)
                {
                    var products =
                        Using<IInventoryRepository>(c).GetByWareHouseId(Using<IConfigService>(c).Load().CostCentreId)
                            .Where(n => n.Balance > 0 && n.Product._Status == EntityStatus.Active).Select(n => n.Product)
                            .AsQueryable();

                    if (!string.IsNullOrEmpty(SearchText))
                    {
                        SearchText = SearchText.ToLower();
                        products =
                            products.Where(
                                p =>

                                p.ProductCode.ToLower().Contains(SearchText) ||
                                p.Description.ToLower().Contains(SearchText));
                    }
                    if (salesmanId.HasValue)
                    {
                        var suppliersid =
                            Using<CokeDataContext>(c).tblSalesmanSupplier.Where(s => s.CostCentreId == salesmanId.Value).Select(s=>s.SupplierId).ToList();
                        if (suppliersid.Any())
                        {
                            products = products.Where(s => suppliersid.Contains(s.Brand.Supplier.Id));
                        }

                    }
                    products.ToList().Select(
                        n => new ComboPopUpItem() { Name = n.Description, Code = n.ProductCode, Id = n.Id }).
                        ToList().ForEach(DefaultItemsCollection.Add);
                    return;
                }

                if (string.IsNullOrEmpty(SearchText))
                    enumerable =
                        Using<CokeDataContext>(c).tblProduct.Where(p => p.IM_Status == (int)EntityStatus.Active).
                            OrderBy(p => p.ProductCode).ThenBy(p => p.Description).Take(defaultTake).ToList();
                else
                {
                    SearchText = SearchText.ToLower();
                    enumerable =
                        Using<CokeDataContext>(c).tblProduct.Where(p => p.IM_Status == (int)EntityStatus.Active)
                            .Where(
                                p =>
                                p.Description.ToLower().Contains(SearchText) ||
                                p.ProductCode.ToLower().Contains(SearchText))
                            .OrderBy(p => p.ProductCode).ThenBy(p => p.Description).Take(defaultTake).ToList();
                }

                enumerable.Cast<tblProduct>().Select(
                    n => new ComboPopUpItem() { Name = n.Description, Code = n.ProductCode, Id = n.id }).
                    ToList().ForEach(DefaultItemsCollection.Add);
            }
        }

        private IEnumerable<object> enumerable;
        private int defaultTake = 20;
        public bool issueInventoryProducts = false;

        public void LoadComboItems(int? take)
        {
            DefaultItemsCollection.Clear();

            if (take.HasValue)
                defaultTake = take.Value;
            switch (LoadedEntity)
            {
                case "User":
                    break;
                case "Product":
                    Loadproducts();
                    break;
                case "UnderbankingCostCentre":
                    LoadUnderbankingCostCentre((Guid)Param1);
                    break;
                case "DistributrSalesman":
                    LoadDistributrSalesman();
                    break;
                case "OutletToMapToSupplier":
                    LoadOutletToMapToSupplier();
                    break;
                case "Route":
                    LoadDistributrSalesmanRoute((Guid)Param1);
                    break;
                case "Outlet":
                    LoadOutetsInRoute((Guid)Param1);
                    break;
                case "OutletBySalesman":
                    LoadOutletsForSalesman((Guid)Param1);
                    break;
                case "Warehouse":
                    LoadWarehouse();
                    break;
                case "StockistDistributrSalesman":
                    LoadStockistDistributrSalesman();
                    break;
                case "LoadAllRoutes":
                    LoadAllRoutes();
                    break;
                case "CommodityOwner":
                    LoadCommodityOwner();
                    break;
                case "Commodity":
                    LoadCommodity();
                    break;
                case "CommodityGrade":
                    LoadCommodityGrade((Guid)Param1);
                    break;
                case "AllPeriods":
                    LoadAllPeriods();
                    break;
                case "Salesman":
                    LoadSalesMen();
                    break;
                case "AllContactsTypes":
                    LoadAllContactTypes();
                    break;
                case "AllContactsOwners":
                    LoadAllContactOwners((int)Param1, (Guid)Param2);
                    break;
                case "AllContactOwnerTypes":
                    LoadAllContactOwnerTypes();
                    break;
                case "Store":
                    LoadStore();
                    break;
                case "CommoditySupplier":
                    LoadCommoditySupplier();
                    break;
                case "CommodityOwnersBySupplier":
                    LoadCommodityOwnersBySupplier((Guid)Param1);
                    break;



                    //Enum Cases

                case "AllMaritalStatus":
                    LoadAllMaritalStatus();
                    break;



                case "DistribtrWarehouse":
                    LoadAllDistributrWarehouses((Guid)Param1);
                    break;
                case "DistribtrSuppliers":
                    LoadDistribtrSuppliers();
                    break;
                    

            }

        }

        public void LoadUnderbankingCostCentre(Guid salesmanid)
        {
            DefaultItemsCollection.Clear();
            using (var c = NestedContainer)
            {
                var ctx = Using<CokeDataContext>(c);
                ctx.tblCostCentre.Where(s => s.Id == salesmanid).ToList()
                    .Select(n => new ComboPopUpItem() { Name = n.Name, Code = n.Cost_Centre_Code, Id = n.Id }).
                    ToList().ForEach(DefaultItemsCollection.Add);
                var salesmanroutesId =
                    ctx.tblSalemanRoute.Where(s => s.SalemanId == salesmanid).Select(s => s.RouteId).ToList();
                IQueryable<tblCostCentre> query = ctx.tblCostCentre.AsQueryable();
                query = query.Where(s => s.CostCentreType == (int)CostCentreType.Outlet);
                query = query.Where(s => salesmanroutesId.Contains(s.RouteId.Value));
                if (!string.IsNullOrEmpty(SearchText))
                {
                    query =
                        query.Where(
                            s =>
                            s.Name.ToLower().Contains(SearchText.ToLower()) ||
                            s.Cost_Centre_Code.ToLower().Contains(SearchText.ToLower()));

                }
                query = query.OrderBy(s => s.Name);
                query.ToList().Select(n => new ComboPopUpItem() { Name = n.Name, Code = n.Cost_Centre_Code, Id = n.Id }).
                    ToList().ForEach(DefaultItemsCollection.Add);


            }

        }

        public void LoadOutletShipto(Guid outletId)
        {
            DefaultItemsCollection.Clear();
            using (var c = NestedContainer)
            {
                var ctx = Using<CokeDataContext>(c);
                IQueryable<tblShipToAddress> query =
                    ctx.tblShipToAddress.AsQueryable().Where(s => s.CostCentreId == outletId);

                if (!string.IsNullOrEmpty(SearchText))
                {
                    query =
                        query.Where(
                            s =>
                            s.Name.ToLower().Contains(SearchText.ToLower()) ||
                            s.PhysicalAddress.Contains(SearchText.ToLower()) || s.PostalAddress.Contains(SearchText));
                }
                query = query.OrderBy(s => s.Name);
                query.ToList().Select(n => new ComboPopUpItem() { Name = n.Name, Code = n.PhysicalAddress, Id = n.Id }).
                    ToList().ForEach(DefaultItemsCollection.Add);


            }

        }

        public void LoadOutetsInRoute(Guid routeId)
        {
            DefaultItemsCollection.Clear();
            using (var c = NestedContainer)
            {
                var ctx = Using<CokeDataContext>(c);
                IQueryable<tblCostCentre> query =
                    ctx.tblCostCentre.AsQueryable().Where(
                        s => s.CostCentreType == (int)CostCentreType.Outlet && s.RouteId == routeId);

                if (!string.IsNullOrEmpty(SearchText))
                {
                    query =
                        query.Where(
                            s =>
                            s.Name.ToLower().Contains(SearchText.ToLower()) ||
                            s.Cost_Centre_Code.ToLower().Contains(SearchText.ToLower()));
                }
                query = query.OrderBy(s => s.Name);
                query.ToList().Select(n => new ComboPopUpItem() { Name = n.Name, Code = n.Cost_Centre_Code, Id = n.Id }).
                    ToList().ForEach(DefaultItemsCollection.Add);


            }

        }

        public void LoadOutletsForSalesman(Guid salesmanId)
        {
            DefaultItemsCollection.Clear();
            using (var c = NestedContainer)
            {
                var outlets = new List<tblCostCentre>();
                var ctx = Using<CokeDataContext>(c);
                var routes = ctx.tblSalemanRoute.Where(n => n.SalemanId == salesmanId).Select(n => n.RouteId).ToList();

                foreach (var route in routes)
                {
                    Guid routeId = route;
                    IEnumerable<tblCostCentre> outlet =
                        ctx.tblCostCentre.AsQueryable().Where(
                            n => n.CostCentreType == (int)CostCentreType.Outlet && n.RouteId == routeId).ToList();

                    if (outlet != null)
                        outlets.AddRange(outlet);

                }
                IOrderedEnumerable<tblCostCentre> orderdOutlets = outlets.Distinct().ToList().OrderBy(s => s.Name);
                IQueryable<tblCostCentre> tblCostCentres = orderdOutlets.AsQueryable();

                if (!string.IsNullOrEmpty(SearchText))
                {
                    tblCostCentres =
                        tblCostCentres.Where(
                            s =>
                            s.Name.ToLower().Contains(SearchText.ToLower()) ||
                            s.Cost_Centre_Code.ToLower().Contains(SearchText.ToLower()));

                }

                //IOrderedEnumerable<tblCostCentre> tblCostCentres = outlets.OrderBy(s => s.Name);
                tblCostCentres.ToList().Select(
                    n => new ComboPopUpItem() { Name = n.Name, Code = n.Cost_Centre_Code, Id = n.Id }).
                    ToList().ForEach(DefaultItemsCollection.Add);
            }

        }

        public void LoadDistributrSalesmanRoute(Guid salesmanId)
        {
            DefaultItemsCollection.Clear();
            using (var c = NestedContainer)
            {
                var ctx = Using<CokeDataContext>(c);
                var routeIds =
                    ctx.tblSalemanRoute.Where(n => n.SalemanId == salesmanId && n.IM_Status == (int)EntityStatus.Active)
                        .Select(n => n.RouteId).ToList();

                if (!routeIds.Any()) return;

                IList<tblRoutes> routes =
                    routeIds.Select(routeId => ctx.tblRoutes.FirstOrDefault(p => p.RouteID == routeId)).ToList();


                if (!string.IsNullOrEmpty(SearchText))
                {
                    routes =
                        routes.Where(
                            s =>
                            s.Name.ToLower().Contains(SearchText.ToLower()) || s.Code.Contains(SearchText.ToLower())).
                            ToList();
                }
                routes.OrderBy(s => s.Name).ToList().Select(
                    n => new ComboPopUpItem() { Name = n.Name, Code = n.Code, Id = n.RouteID }).
                    ToList().ForEach(DefaultItemsCollection.Add);
            }

        }

        public void LoadAllRoutes()
        {
            DefaultItemsCollection.Clear();
            using (var c = NestedContainer)
            {
                var ctx = Using<CokeDataContext>(c);
                //var routeIds =
                //    ctx.tblSalemanRoute.Where(n=> n.IM_Status == (int)EntityStatus.Active).Select(n => n.RouteId).ToList();

                //if (!routeIds.Any()) return;

                IList<tblRoutes> routes = ctx.tblRoutes.ToList();


                if (!string.IsNullOrEmpty(SearchText))
                {
                    routes = routes.Where(
                            s =>
                            s.Name.ToLower().Contains(SearchText.ToLower()) || s.Code.Contains(SearchText.ToLower())).
                            ToList();
                }
                routes.ToList().Select(
                    n => new ComboPopUpItem() { Name = n.Name, Code = n.Code, Id = n.RouteID }).
                    ToList().ForEach(DefaultItemsCollection.Add);
            }

        }


        public void LoadAllContactOwners(int contactOwnerType, Guid costCenId)
        {
            DefaultItemsCollection.Clear();
            using (var c = NestedContainer)
            {
            
             
                var ctx = Using<CokeDataContext>(c);


               IEnumerable<tblCostCentre> costCen = ctx.tblCostCentre.ToList();

               
                switch (contactOwnerType)
                {

                    case 0:

                        costCen = costCen.Where(
                    p => p.CostCentreType == (int)CostCentreType.None && p.Id == costCenId).ToList();

                        break;
                    case 1:
                        costCen = costCen.Where(
                    p => p.CostCentreType == (int)CostCentreType.Distributor && p.Id == costCenId).ToList();
                        break;
                    
                    case 3:
                        costCen = costCen.Where(
              p => p.CostCentreType == (int)CostCentreType.Outlet ).ToList();
                        break;
                    case 4:
                        costCen = costCen.Where(
              p => p.CostCentreType == (int)CostCentreType.CommoditySupplier && p.Id == costCenId).ToList();
                        break;
              //      default:
              //          costCen = costCen.Where(
              //p => p.CostCentreType == (int)CostCentreType.DistributorSalesman ).ToList();
              //          break;
                    case 2:
                        costCen = costCen.Where(
              p => p.CostCentreType != null || p.CostCentreType == (int)CostCentreType.CommoditySupplier ).ToList();
                        break;

                }



                if (!string.IsNullOrEmpty(SearchText))
                {
                    costCen =
                        costCen.Where(
                            s =>
                            s.Name.ToLower().Contains(SearchText.ToLower())).
                            ToList();
                }
                costCen.OrderBy(s => s.Name).ToList().Select(
                    n => new ComboPopUpItem() { Name = n.Name, Id = n.Id }).
                    ToList().ForEach(DefaultItemsCollection.Add);
            }

        }

        public void LoadAllContactOwnerTypes()
        {
            DefaultEnumItemsCollection.Clear();
            var contactOwnerTypeList = Enum.GetValues(typeof(ContactOwnerType)).Cast<ContactOwnerType>().ToList();

              

            contactOwnerTypeList.OrderBy(n => n.ToString()).ToList().Select(
                n => new ComboPopUpEnumItem() { Name = n.ToString(), Id = (int)n }).
                ToList().ForEach(DefaultEnumItemsCollection.Add);

        }

        public void LoadAllMaritalStatus()
        {
            DefaultEnumItemsCollection.Clear();
            var maritalStatus = Enum.GetValues(typeof(MaritalStatas)).Cast<MaritalStatas>().ToList();

            maritalStatus.OrderBy(n => n.ToString()).ToList().Select(
                n => new ComboPopUpEnumItem() { Name = n.ToString(), Id = (int)n }).
                ToList().ForEach(DefaultEnumItemsCollection.Add);

        }







        public void LoadDistributrSalesman()
        {
            DefaultItemsCollection.Clear();
            using (var c = NestedContainer)
            {
                var ctx = Using<CokeDataContext>(c);
                IQueryable<tblCostCentre> query = ctx.tblCostCentre.AsQueryable();
                query = query.Where(s => s.CostCentreType == (int)CostCentreType.DistributorSalesman && s.IM_Status == (int)EntityStatus.Active);

                if (!string.IsNullOrEmpty(SearchText))
                {
                    query =
                        query.Where(
                            s =>
                            s.Name.ToLower().Contains(SearchText.ToLower()) ||
                            s.Cost_Centre_Code.ToLower().Contains(SearchText.ToLower()));
                }
                query = query.OrderBy(s => s.Name);
                query.ToList().Select(n => new ComboPopUpItem() { Name = n.Name, Code = n.Cost_Centre_Code, Id = n.Id }).
                    ToList().ForEach(DefaultItemsCollection.Add);


            }

        }

        public void LoadStockistDistributrSalesman()
        {
            DefaultItemsCollection.Clear();
            using (var c = NestedContainer)
            {
                var ctx = Using<CokeDataContext>(c);
                IQueryable<tblCostCentre> query = ctx.tblCostCentre.AsQueryable();
                query =
                    query.Where(
                        s =>
                        s.CostCentreType == (int)CostCentreType.DistributorSalesman &&
                        s.CostCentreType2 == (int)DistributorSalesmanType.Stockist);

                if (!string.IsNullOrEmpty(SearchText))
                {
                    query =
                        query.Where(
                            s =>
                            s.Name.ToLower().Contains(SearchText.ToLower()) ||
                            s.Cost_Centre_Code.ToLower().Contains(SearchText.ToLower()));
                }
                query = query.OrderBy(s => s.Name);
                query.ToList().Select(n => new ComboPopUpItem() { Name = n.Name, Code = n.Cost_Centre_Code, Id = n.Id }).
                    ToList().ForEach(DefaultItemsCollection.Add);


            }

        }

        public ComboPopUpItem ReturnSelected()
        {
            return SelectedItem;
        }

        public ComboPopUpEnumItem ReturnSelectedEnum()
        {
            return SelectedEnumItem;
        }

        public void LoadOutletToMapToSupplier()
        {
            DefaultItemsCollection.Clear();
            using (var c = NestedContainer)
            {
                var ctx = Using<CokeDataContext>(c);
                IQueryable<tblCostCentre> query = ctx.tblCostCentre.AsQueryable();
                var alreadyMapped = ctx.tblCostCentreMapping.Select(s => s.MapToCostCentreId).ToList();
                query =
                    query.Where(s => s.CostCentreType == (int)CostCentreType.Outlet && !alreadyMapped.Contains(s.Id));

                if (!string.IsNullOrEmpty(SearchText))
                {
                    query =
                        query.Where(
                            s =>
                            s.Name.ToLower().Contains(SearchText.ToLower()) ||
                            s.Cost_Centre_Code.ToLower().Contains(SearchText.ToLower()));
                }
                query = query.OrderBy(s => s.Name);
                query.ToList().Select(n => new ComboPopUpItem() { Name = n.Name, Code = n.Cost_Centre_Code, Id = n.Id }).
                    ToList().ForEach(DefaultItemsCollection.Add);


            }
        }


        public void LoadWarehouse()
        {
            DefaultItemsCollection.Clear();
            using (var c = NestedContainer)
            {
                var ctx = Using<CokeDataContext>(c);
                IQueryable<tblCostCentre> query = ctx.tblCostCentre.AsQueryable();
                query =
                    query.Where(
                        s =>
                        s.CostCentreType == (int)CostCentreType.Producer ||
                        s.CostCentreType == (int)CostCentreType.Store || s.CostCentreType == (int)CostCentreType.Hub);

                if (!string.IsNullOrEmpty(SearchText))
                {
                    query =
                        query.Where(
                            s =>
                            s.Name.ToLower().Contains(SearchText.ToLower()) ||
                            s.Cost_Centre_Code.ToLower().Contains(SearchText.ToLower()));
                }
                query = query.OrderBy(s => s.Name);
                query.ToList().Select(n => new ComboPopUpItem() { Name = n.Name, Code = n.Cost_Centre_Code, Id = n.Id }).
                    ToList().ForEach(DefaultItemsCollection.Add);


            }
        }

        public void LoadStore()
        {
            DefaultItemsCollection.Clear();
            using (var c = NestedContainer)
            {
                var ctx = Using<CokeDataContext>(c);
                IQueryable<tblCostCentre> query = ctx.tblCostCentre.AsQueryable();
                query =
                    query.Where(
                        s =>
                        s.CostCentreType == (int)CostCentreType.Store );

                if (!string.IsNullOrEmpty(SearchText))
                {
                    query =
                        query.Where(
                            s =>
                            s.Name.ToLower().Contains(SearchText.ToLower()) ||
                            s.Cost_Centre_Code.ToLower().Contains(SearchText.ToLower()));
                }
                
                query = query.OrderBy(s => s.Name);
                query.ToList().Select(n => new ComboPopUpItem() { Name = n.Name, Code = n.Cost_Centre_Code, Id = n.Id }).
                    ToList().ForEach(DefaultItemsCollection.Add);


            }
        }

        public void LoadCommodity()
        {
            DefaultItemsCollection.Clear();
            using (var c = NestedContainer)
            {
                var ctx = Using<CokeDataContext>(c);
                IQueryable<tblCommodity> query = ctx.tblCommodity.AsQueryable();
                //query = query.Where(s => s.CostCentreType == (int)CostCentreType.Producer || s.CostCentreType == (int)CostCentreType.Store || s.CostCentreType == (int)CostCentreType.Hub);

                if (!string.IsNullOrEmpty(SearchText))
                {
                    query =
                        query.Where(
                            s =>
                            s.Name.ToLower().Contains(SearchText.ToLower()) ||
                            s.Code.ToLower().Contains(SearchText.ToLower()));
                }
                query = query.OrderBy(s => s.Name);
                query.ToList().Select(n => new ComboPopUpItem() { Name = n.Name, Code = n.Code, Id = n.Id }).
                    ToList().ForEach(DefaultItemsCollection.Add);


            }
        }

        public void LoadCommodityOwner()
        {
            DefaultItemsCollection.Clear();
            using (var c = NestedContainer)
            {
                var ctx = Using<CokeDataContext>(c);
                IQueryable<tblCommodityOwner> query = ctx.tblCommodityOwner.AsQueryable();
                //query = query.Where(s => s.CostCentreType == (int)CostCentreType.Producer || s.CostCentreType == (int)CostCentreType.Store || s.CostCentreType == (int)CostCentreType.Hub);

                if (!string.IsNullOrEmpty(SearchText))
                {
                    query =
                        query.Where(
                            s =>
                            s.FirstName.ToLower().Contains(SearchText.ToLower()) ||
                            s.Code.ToLower().Contains(SearchText.ToLower()));
                }
                query = query.OrderBy(s => s.FirstName);
                query.ToList().Select(n => new ComboPopUpItem() {Name = n.FirstName+" "+n.LastName+" "+n.Surname, Code = n.Code, Id = n.Id}).
                    ToList().ForEach(DefaultItemsCollection.Add);


            }
        }

        public void LoadCommodityOwnersBySupplier(Guid supplierId)
        {
            DefaultItemsCollection.Clear();
            using (var c = NestedContainer)
            {
                var ctx = Using<CokeDataContext>(c);
                IQueryable<tblCommodityOwner> query = ctx.tblCommodityOwner.Where(s=>s.CostCentreId==supplierId).AsQueryable();
                //query = query.Where(s => s.CostCentreType == (int)CostCentreType.Producer || s.CostCentreType == (int)CostCentreType.Store || s.CostCentreType == (int)CostCentreType.Hub);

                if (!string.IsNullOrEmpty(SearchText))
                {
                    query =
                        query.Where(
                            s =>s.FirstName.ToLower().Contains(SearchText.ToLower()) ||
                            s.Code.ToLower().Contains(SearchText.ToLower()));
                }
                query = query.OrderBy(s => s.FirstName);
                query.ToList().Select(n => new ComboPopUpItem() { Name = n.FirstName + " " + n.LastName + " " + n.Surname, Code = n.Code, Id = n.Id }).
                    ToList().ForEach(DefaultItemsCollection.Add);


            }
        }
        public void LoadCommodityGrade(Guid commodityId)
        {
            DefaultItemsCollection.Clear();
            using (var c = NestedContainer)
            {
                var ctx = Using<CokeDataContext>(c);
                IQueryable<tblCommodityGrade> query = ctx.tblCommodityGrade.AsQueryable();
                query = query.Where(s => s.tblCommodity.Id == commodityId);

                if (!string.IsNullOrEmpty(SearchText))
                {
                    query =
                        query.Where(
                            s =>
                            s.Name.ToLower().Contains(SearchText.ToLower()) ||
                            s.Code.ToLower().Contains(SearchText.ToLower()));
                }
                query = query.OrderBy(s => s.Name);
                query.ToList().Select(n => new ComboPopUpItem() {Name = n.Name, Code = n.Code, Id = n.Id}).
                    ToList().ForEach(DefaultItemsCollection.Add);


            }
        }

        //public void LoadCommodityGrade(Guid commodityId)
        //{
        //    DefaultItemsCollection.Clear();
        //    using (var c = NestedContainer)
        //    {
        //        var ctx = Using<CokeDataContext>(c);
        //        IQueryable<tblCommodityGrade> query = ctx.tblCommodityGrade.AsQueryable();
        //        query = query.Where(s => s.tblCommodity.Id == commodityId);

        //        if (!string.IsNullOrEmpty(SearchText))
        //        {
        //            query =
        //                query.Where(
        //                    s =>
        //                    s.Name.ToLower().Contains(SearchText.ToLower()) ||
        //                    s.Code.ToLower().Contains(SearchText.ToLower()));
        //        }
        //        query = query.OrderBy(s => s.Name);
        //        query.ToList().Select(n => new ComboPopUpItem() { Name = n.Name, Code = n.Code, Id = n.Id }).
        //            ToList().ForEach(DefaultItemsCollection.Add);
        //    }
        //}


        public void LoadSalesMen()
        {
            DefaultItemsCollection.Clear();
            using (var c = NestedContainer)
            {

                var ctx = Using<CokeDataContext>(c);
                IQueryable<tblUsers> query = ctx.tblUsers.AsQueryable();

                query = query.Where(s => s.UserType == (int)UserType.DistributorSalesman);

                if (!string.IsNullOrEmpty(SearchText))
                {
                    query =
                        query.Where(
                            s =>
                            s.UserName.ToLower().Contains(SearchText.ToLower()) ||
                            s.Code.ToLower().Contains(SearchText.ToLower()));
                }
                query = query.OrderBy(s => s.FirstName);
                query.ToList().Select(n => new ComboPopUpItem() { Name = n.UserName, Code = n.Code, Id = n.Id }).
                    ToList().ForEach(DefaultItemsCollection.Add);
            }
        }




        public void LoadAllContactTypes()
        {
            DefaultItemsCollection.Clear();
            using (var c = NestedContainer)
            {
                var ctx = Using<CokeDataContext>(c);

                // IList<tblContactType> contactstype = ctx.tblContactType.ToList();
                IQueryable<tblContactType> query = ctx.tblContactType.AsQueryable();
                // query = contactstype.AsQueryable();
                if (!string.IsNullOrEmpty(SearchText))
                {
                    query =
                         query.Where(
                             s =>
                             s.Name.ToLower().Contains(SearchText.ToLower()) || s.Code.ToLower().Contains(SearchText.ToLower()));
                }
                query = query.OrderBy(s => s.Name);
                query.ToList().Select(n => new ComboPopUpItem() { Name = n.Name, Code = n.Code, Id = n.id }).
                  ToList().ForEach(DefaultItemsCollection.Add);
               
            }
        }

        public void LoadAllPeriods()
        {
            DefaultItemsCollection.Clear();
            using (var c = NestedContainer)
            {

                var ctx = Using<CokeDataContext>(c);
                IQueryable<tblTargetPeriod> query = ctx.tblTargetPeriod.AsQueryable();

                if (!string.IsNullOrEmpty(SearchText))
                {
                    query =
                        query.Where(
                            s =>
                            s.Name.ToLower().Contains(SearchText.ToLower()));
                }
                query = query.OrderBy(s => s.Name);
                query.ToList().Select(n => new ComboPopUpItem() { Name = n.Name, Id = n.Id }).
                    ToList().ForEach(DefaultItemsCollection.Add);
            }
        }

        public void LoadCommoditySupplier()
        {
            DefaultItemsCollection.Clear();
            using (var c = NestedContainer)
            {
                var ctx = Using<CokeDataContext>(c);
                IQueryable<tblCostCentre> query = ctx.tblCostCentre.AsQueryable();
                query =
                    query.Where(
                        s =>
                        s.CostCentreType == (int)CostCentreType.CommoditySupplier);

                if (!string.IsNullOrEmpty(SearchText))
                {
                    query =
                        query.Where(
                            s =>
                            s.Name.ToLower().Contains(SearchText.ToLower()) ||
                            s.Cost_Centre_Code.ToLower().Contains(SearchText.ToLower()));
                }
                
                query = query.OrderBy(s => s.Name);
                query.ToList().Select(n => new ComboPopUpItem() { Name = n.Name, Code = n.Cost_Centre_Code, Id = n.Id }).
                    ToList().ForEach(DefaultItemsCollection.Add);


            }
        }


        public void LoadAllDistributrWarehouses(Guid? parentCostCentreId)
        {
            DefaultItemsCollection.Clear();
            using (var c = NestedContainer)
            {
                var ctx = Using<CokeDataContext>(c);
                IQueryable<tblCostCentre> query = ctx.tblCostCentre.AsQueryable();
                if (parentCostCentreId.HasValue)
                {
                    query =
                    query.Where(
                        s =>
                        s.Id==parentCostCentreId.Value||
                        s.ParentCostCentreId == parentCostCentreId.Value &&
                        (s.CostCentreType == (int)CostCentreType.Distributor ||
                        s.CostCentreType == (int)CostCentreType.DistributorSalesman || s.CostCentreType == (int)CostCentreType.DistributorPendingDispatchWarehouse));
                }
                else
                {
                    query =
                    query.Where(
                        s =>
                        s.CostCentreType == (int)CostCentreType.Distributor ||
                        s.CostCentreType == (int)CostCentreType.DistributorSalesman || s.CostCentreType == (int)CostCentreType.DistributorPendingDispatchWarehouse);
                }
                

                if (!string.IsNullOrEmpty(SearchText))
                {
                    query =
                        query.Where(
                            s =>
                            s.Name.ToLower().Contains(SearchText.ToLower()) ||
                            s.Cost_Centre_Code.ToLower().Contains(SearchText.ToLower()));
                }
                query = query.OrderBy(s => s.Name);
                query.ToList().Select(n => new ComboPopUpItem() { Name = n.Name, Code = n.Cost_Centre_Code, Id = n.Id }).
                    ToList().ForEach(DefaultItemsCollection.Add);


            }
        }

        public void LoadDistribtrSuppliers()
        {
            DefaultItemsCollection.Clear();
            using (var c = NestedContainer)
            {
                var ctx = Using<CokeDataContext>(c);
                IQueryable<tblSupplier> query = ctx.tblSupplier.AsQueryable();
               

                if (!string.IsNullOrEmpty(SearchText))
                {
                    query =
                        query.Where(
                            s =>
                            s.Name.ToLower().Contains(SearchText.ToLower()) ||
                            s.Code.ToLower().Contains(SearchText.ToLower()));
                }

                query = query.OrderBy(s => s.Name);
                query.ToList().Select(n => new ComboPopUpItem() { Name = n.Name, Code = n.Code, Id = n.id }).
                    ToList().ForEach(DefaultItemsCollection.Add);


            }
        }
    }
}

        #endregion