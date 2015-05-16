using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.MasterDataAllocationRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Admin;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Transactional.CommodityPurchase
{
    public class ListFarmersViewModel : ListingsViewModelBase
    {
        private PagenatedList<CommodityOwner> _pagedCommodityOwners;
        public ListFarmersViewModel()
        {
            FarmersList = new ObservableCollection<CommodityOwner>();
            RouteList = new ObservableCollection<Route>();
            CommodityList = new ObservableCollection<Commodity>();
            BuyingCentreList = new ObservableCollection<Centre>();
            FilterFarmersCommand = new RelayCommand<string>((s) => FilterFarmersInfo(s));
            PurchaseCommodityCommand = new RelayCommand<CommodityOwner>(PurchaseCommodity);
            ViewLastTransactionCommand =new RelayCommand<CommodityOwner>(ViewLastTransaction); 

        }

       

       

        #region Properties

        public ObservableCollection<CommodityOwner> FarmersList { get; set; }

        public ObservableCollection<Route> RouteList { get; set; }

        public ObservableCollection<Commodity> CommodityList { get; set; }

        public ObservableCollection<Centre> BuyingCentreList { get; set; }

        public RelayCommand<string> FilterFarmersCommand { get; set; }
        public RelayCommand<CommodityOwner> PurchaseCommodityCommand { get; set; }
        public RelayCommand<CommodityOwner> ViewLastTransactionCommand { get; set; }

        public const string SelectedBuyingCentrePropertyName = "SelectedBuyingCentre";
        private Centre _selectedBuyingCentre = null;
        public Centre SelectedBuyingCentre
        {
            get
            {
                return _selectedBuyingCentre;
            }

            set
            {
                if (_selectedBuyingCentre == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedBuyingCentrePropertyName);
                _selectedBuyingCentre = value;
                RaisePropertyChanged(SelectedBuyingCentrePropertyName);
            }
        }

        public CommodityPurchaseNote LastTransaction
        {
            get
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    return Using<ICommodityPurchaseRepository>(c)
                        .GetLastTransactionByFarmerId(SelectedFarmer.Id);

                }
            }
        }

        public const string SelectedRoutePropertyName = "SelectedRoute";
        private Route _selectedRoute = null;
        public Route SelectedRoute
        {
            get
            {
                return _selectedRoute;
            }

            set
            {
                if (_selectedRoute == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedRoutePropertyName);
                _selectedRoute = value;
                RaisePropertyChanged(SelectedRoutePropertyName);

                if(SelectedRoute !=null && SelectedRoute.Id !=Guid.Empty)
                    LoadBuyingCentres();
            }
        }
         
        public const string SelectedCommodityPropertyName = "SelectedCommodity";
        private Commodity _selectedCommodity = null;
        public Commodity SelectedCommodity
        {
            get
            {
                return _selectedCommodity;
            }

            set
            {
                if (_selectedCommodity == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCommodityPropertyName);
                _selectedCommodity = value;
                RaisePropertyChanged(SelectedCommodityPropertyName);
            }
        }
         
        public const string TodayTotalCommodityPurchasePropertyName = "TodayTotalCommodityPurchase";
        private decimal _todayTotalCommodityPurchase = 0m;
        public decimal TodayTotalCommodityPurchase
        {
            get
            {
                return _todayTotalCommodityPurchase;
            }

            set
            {
                if (_todayTotalCommodityPurchase == value)
                {
                    return;
                }

                RaisePropertyChanging(TodayTotalCommodityPurchasePropertyName);
                _todayTotalCommodityPurchase = value;
                RaisePropertyChanged(TodayTotalCommodityPurchasePropertyName);
            }
        }
         
        public const string MonthTotalCommodityPurchasePropertyName = "MonthTotalCommodityPurchase";
        private decimal _monthTotalCommodityPurchase = 0m;
        public decimal MonthTotalCommodityPurchase
        {
            get
            {
                return _monthTotalCommodityPurchase;
            }

            set
            {
                if (_monthTotalCommodityPurchase == value)
                {
                    return;
                }

                RaisePropertyChanging(MonthTotalCommodityPurchasePropertyName);
                _monthTotalCommodityPurchase = value;
                RaisePropertyChanged(MonthTotalCommodityPurchasePropertyName);
            }
        }

        public const string SelectedFarmerPropertyName = "SelectedFarmer";
        private CommodityOwner _selectedFarmer = null;
        public CommodityOwner SelectedFarmer
        {
            get
            {
                return _selectedFarmer;
            }

            set
            {
                if (_selectedFarmer == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedFarmerPropertyName);
                _selectedFarmer = value;
                RaisePropertyChanged(SelectedFarmerPropertyName);
            }
        }

        Route _defaultRoute = null;
        private Route DefaultRoute
        {
            get { return _defaultRoute ?? (_defaultRoute = new Route(Guid.Empty) {Name = "--Select a route--"}); }
        }

        Centre _defaultBuyingCentre = null;
        private Centre DefaultBuyingCentre
        {
            get {
                return _defaultBuyingCentre ??
                       (_defaultBuyingCentre = new Centre(Guid.Empty) {Name = "--Select buying centre--"});
            }
        }

        Commodity _defaultCommodity = null;

        private Commodity DefaultCommodity
        {
            get {
                return _defaultCommodity ??
                       (_defaultCommodity = new Commodity(Guid.Empty) {Name = "--Select commodity--"});
            }
        }

        #endregion

        #region Methods


        private void PurchaseCommodity(CommodityOwner obj)
        {
            Messenger.Default.Send<ViewModelMessage>(new ViewModelMessage { Id = obj.Id});
            //var navHelper = UserControlNavigationHelper.FindParentByType<>();
            const string uri = "/views/CommodityPurchase/EditPurchase.xaml";
            NavigateCommand.Execute(uri);
        }

        public void Clear()
        {
            FarmersList.Clear();
            RouteList.Clear();
            CommodityList.Clear();
            BuyingCentreList.Clear();
            TodayTotalCommodityPurchase = 0m;
            MonthTotalCommodityPurchase = 0m;
            SelectedBuyingCentre = null;
            SelectedCommodity = null;
            SelectedRoute = null;
        }

        protected override void Load(bool isFirstLoad = false)
        {
            if (isFirstLoad)
                Setup();
            using (var c = NestedContainer)
            {
                var farmersRepository = Using<ICommodityOwnerRepository>(c);

                var farmers =
                    farmersRepository.GetAll().OrderBy(n => n.FirstName).ThenBy(n => n.Surname).ToList
                        ();
                var thefarmers = new List<CommodityOwner>();
                if (SelectedBuyingCentre != null && SelectedBuyingCentre.Id != Guid.Empty)
                {
                    var selectedCentreId = SelectedBuyingCentre.Id;

                    var farmsByCentre =
                        Using<IMasterDataAllocationRepository>(c).GetByAllocationType(
                            MasterDataAllocationType.CommodityProducerCentreAllocation)
                            .Where(n => n.EntityBId == selectedCentreId)
                            .Select(n => n.EntityAId);

                    var farmRepos = Using<ICommodityProducerRepository>(c);
                    
                    foreach (var farm in farmsByCentre)
                    {
                        var theFarm = farmRepos.GetById(farm);
                        var supplier = theFarm.CommoditySupplier;
                        
                        thefarmers.AddRange(farmers.Where(p=>p.CommoditySupplier.Id==supplier.Id).ToList());
                    }

                    _pagedCommodityOwners = new PagenatedList<CommodityOwner>(thefarmers.AsQueryable(),
                                                                              CurrentPage,
                                                                              ItemsPerPage,
                                                                              farmers.Count());
                   

                }
                else
                {
                    _pagedCommodityOwners = new PagenatedList<CommodityOwner>(farmers.AsQueryable(),
                                                                          CurrentPage,
                                                                          ItemsPerPage,
                                                                          farmers.Count());
                    
                }
                FarmersList.Clear();
                _pagedCommodityOwners.ToList().ForEach(n => FarmersList.Add(n));

                UpdatePagenationControl();
               
            }
        }

        public void LoadData()
        {
            LoadDefaultSelected();
            LoadFarmers();
        }

        public void LoadDefaultSelected(string entity = "")
        {
            if(entity == "" || entity == "route")
            {
                RouteList.Clear();
                RouteList.Add(DefaultRoute);
                SelectedRoute = DefaultRoute;
            }
            if (entity == "" || entity == "centre")
            {
                BuyingCentreList.Clear();
                BuyingCentreList.Add(DefaultBuyingCentre);
                SelectedBuyingCentre = DefaultBuyingCentre;
            }
            if (entity == "" || entity == "commodity")
            {
                CommodityList.Clear();
                CommodityList.Add(DefaultCommodity);
                SelectedCommodity = DefaultCommodity;
            }
        }

        public void LoadFarmers()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                var farmers = Using<ICommodityOwnerRepository>(c).GetAll().OrderBy(n => n.FirstName).ThenBy(n => n.Surname).ToList();
                farmers.ForEach(n => { if (FarmersList.Select(q => q.Id).All(q => q != n.Id)) FarmersList.Add(n); });
            }
        }
        private void FilterFarmersInfo(string s)
        {
            FarmersList.Clear();
             using (StructureMap.IContainer c = NestedContainer)
             {
                 s = s.ToLower();
                 var farmers =
                     Using<ICommodityOwnerRepository>(c).GetAll().Where(
                         p =>p.FullName.ToLower().Contains(s) ||p.IdNo.ToLower().Contains(s)||p.PinNo.Contains(s)||p.Code.ToLower().Contains(s)).ToList();

                 _pagedCommodityOwners = new PagenatedList<CommodityOwner>(farmers.AsQueryable(),
                                                                         CurrentPage,
                                                                         ItemsPerPage,
                                                                         farmers.Count());

                 FarmersList.Clear();
                 _pagedCommodityOwners.ToList().ForEach(n => FarmersList.Add(n));
                 UpdatePagenationControl();
             }

        }
        public void LoadRoutes()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                var routes = Using<IRouteRepository>(c).GetAll().OrderBy(n => n.Name).ThenBy(n => n.Code).ToList();
                routes.ForEach(n => { if (RouteList.Select(q => q.Id).All(q => q != n.Id)) RouteList.Add(n); });
            }
        }

         private void ViewLastTransaction(CommodityOwner farmer)
        {
            using (var c = NestedContainer)
            {
                var commodityPurchaseNote =
                    Using<ICommodityPurchaseRepository>(c).GetLastTransactionByFarmerId(farmer.Id);
                if (commodityPurchaseNote == null)
                {
                    MessageBox.Show(farmer.FullName+ " has no transactions to show", "Agrimanagr Info", MessageBoxButton.OK);
                    
                }
                else
                {
                    Using<IReceiptDocumentPopUp>(c).ShowReceipt(commodityPurchaseNote);
                }
            }
        }
        public void LoadBuyingCentres()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                var bcs = Using<ICentreRepository>(c).GetAll()
                    .OrderBy(n => n.Name).ThenBy(n => n.Code)
                    .ToList();
                foreach (var centre in bcs)
                {
                    if(centre.Route!=null)
                    {
                       bcs=bcs.Where(p => p.Route.Id == SelectedRoute.Id).ToList();
                       bcs.ForEach(n => { if (BuyingCentreList.Select(q => q.Id).All(q => q != n.Id)) BuyingCentreList.Add(n); });
                    }
                   
                }
               
            }
        }

        public void LoadCommodity()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                var commodity = Using<ICommodityRepository>(c).GetAll().OrderBy(n => n.Name).ThenBy(n => n.Code).ToList();
                commodity.ForEach(n => { if (CommodityList.Select(q => q.Id).All(p => p != n.Id)) CommodityList.Add(n); });
            }
        }
        #endregion

        #region util methods

        

        private void Setup()
        {
            Clear();
            LoadDefaultSelected();
        }
        #region unused inheritance
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
        #endregion

        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, _pagedCommodityOwners.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            Load();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(_pagedCommodityOwners.PageNumber, _pagedCommodityOwners.PageCount, _pagedCommodityOwners.TotalItemCount,
                                      _pagedCommodityOwners.IsFirstPage, _pagedCommodityOwners.IsLastPage);
        }
        #endregion
    }
}
