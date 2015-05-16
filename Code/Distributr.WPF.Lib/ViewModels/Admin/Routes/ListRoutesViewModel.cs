using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.ViewModels.Admin.Outlets;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using StructureMap;

namespace Distributr.WPF.Lib.ViewModels.Admin.Routes
{
    public class ListRoutesViewModel :ListingsViewModelBase
    {
        private List<Outlet> _routeOutlets;
        public List<Outlet> RouteOutletsWithOutstandingPayments;
        private bool showMsg = false;
        private EntityUsage _entityUsage;

        public ListRoutesViewModel()
        {
            Routes = new ObservableCollection<ListRouteItem>();
            PageTitle = GetLocalText("sl.routes.list.title"); //"List Routes";
        }

        #region Propertied

        public ObservableCollection<ListRouteItem> Routes { get; set; }

        public const string RouteIdPropertyName = "RouteId";
        private Guid _routeId = Guid.Empty;
        public Guid RouteId
        {
            get { return _routeId; }

            set
            {
                if (_routeId == value)
                {
                    return;
                }

                var oldValue = _routeId;
                _routeId = value;
                RaisePropertyChanged(RouteIdPropertyName);
            }
        }
        
        public const string LoadingStatusPropertyName = "LoadingStatus";
        private string _loadingStatus = "Loading Routes ... \nPlease wait";
        public string LoadingStatus
        {
            get { return _loadingStatus; }

            set
            {
                if (_loadingStatus == value)
                {
                    return;
                }

                var oldValue = _loadingStatus;
                _loadingStatus = value;
                RaisePropertyChanged(LoadingStatusPropertyName);
            }
        }

        public const string LoadingPropertyName = "Loading";
        private bool _loading = false;
        public bool Loading
        {
            get { return _loading; }

            set
            {
                if (_loading == value)
                {
                    return;
                }

                var oldValue = _loading;
                _loading = value;
                RaisePropertyChanged(LoadingPropertyName);
            }
        }

        public const string SelectedRoutePropertyName = "SelectedRoute";
        private ListRouteItem _selectedRoute = null;
        public ListRouteItem SelectedRoute
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
            }
        }

        #endregion

        #region Methods

        public void RunGetRoutes(bool showInactive = false)
        {
            ShowInactive = showInactive;
            Loading = true;
            LoadRoutes();
        }

        protected override void Load(bool isFirstLoad = false)
        {
            LoadPagedRoutes();
            UpdatePagenationControl();
        }

        private IPagenatedList<Route> _pagedroutes;
        void LoadPagedRoutes()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                _pagedroutes = Using<IRouteRepository>(c).GetAll(CurrentPage, (int) ItemsPerPage, SearchText, ShowInactive);
                var pagedRouteItems = _pagedroutes.Select(Map).ToList();

                Routes.Clear();
                foreach (var item in pagedRouteItems)
                {
                    if (item.EntityStatus == (int)EntityStatus.Active)
                        item.HlkDeactivateContent = GetLocalText("sl.users.grid.col.deactivate"); //Deactivate
                    else if (item.EntityStatus == (int)EntityStatus.Inactive)
                        item.HlkDeactivateContent = GetLocalText("sl.users.grid.col.activate"); //Activate
                    Routes.Add(item);
                    double percent = ((double)Routes.Count * 100) / (double)_pagedroutes.Count();
                    LoadingStatus = "Loading Routes ..." + (int)percent + "%\nPlease wait";
                }
                Loading = false;
            }
        }

        public void LoadRoutes()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                var routes =
                    Using<IRouteRepository>(c).GetAll(ShowInactive)
                        .Where(
                            N =>
                            N.Region.Id ==
                            ((Distributor)
                             Using<ICostCentreRepository>(c).GetById(Using<IConfigService>(c).Load().CostCentreId)).
                                Region.Id)
                        .ToList();
                var list = routes.OrderBy(n => n.Name).Select(Map);

                Routes.Clear();
                foreach (var item in list)
                {
                    if (item.EntityStatus == (int)EntityStatus.Active)
                        item.HlkDeactivateContent = GetLocalText("sl.users.grid.col.deactivate");
                    //Deactivate
                    else if (item.EntityStatus == (int)EntityStatus.Inactive)
                        item.HlkDeactivateContent = GetLocalText("sl.users.grid.col.activate"); //Activate
                    Routes.Add(item);
                    double percent = ((double)Routes.Count * 100) / (double)list.Count();
                    LoadingStatus = "Loading Routes ..." + (int)percent + "%\nPlease wait";
                }
                Loading = false;
            }
        }

        ListRouteItem Map(Route route, int count)
        {
            return new ListRouteItem()
                                          {
                                              RowNumber = count + 1,
                                              Id = route.Id,
                                              Name = route.Name,
                                              Code = route.Code,
                                              EntityStatus = (int) route._Status
                                          };
        }

        public async void DeactivateRoute(Guid id)
        {
            if(!ConfirmDisable(id)) return;
            using (StructureMap.IContainer c = NestedContainer)
            {
                ResponseBool response = null;
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                showMsg = true;
                response = await proxy.RouteDeactivateAsync(id);
                MessageBox.Show(response.ErrorInfo, "Distributr:", MessageBoxButton.OK);
            }
        }

        public async void DeleteRoute(Guid id)
        {
            RouteId = id;
            if (!ConfirmDisable(id)) return;
            using (StructureMap.IContainer c = NestedContainer)
            {
                ResponseBool response = null;
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);

                response = await proxy.RouteDeleteAsync(id);
                if(response.Success)
                {
                    //GO:TODO=> we want to delete locally,somehow  doesn't unassign outlet priority before deleting in HQ
                  Using<IRouteRepository>(c).SetAsDeleted(Using<IRouteRepository>(c).GetById(RouteId));
                    LoadPagedRoutes();
                }

                MessageBox.Show(response.ErrorInfo, "Distributr:", MessageBoxButton.OK);
            }
        }

        bool ConfirmDisable(Guid routeId)
        {
            var app = GetConfigParams().AppId.ToString();
            bool delete = true;
            if (MessageBox.Show("Are you sure you want to delete this route?", GetConfigParams().AppId.ToString() +
                ": Delete Route", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                bool canDeactivate = true;
                string msg = CheckRouteUsed(routeId, out canDeactivate);
                if (!string.IsNullOrEmpty(msg)) //cn: route is used
                {
                    if (!canDeactivate)
                    {
                        MessageBox.Show(msg, app + ": Route is used.", MessageBoxButton.OK);
                        return false;
                    }
                    if (MessageBox.Show(msg, app +": Route is used.", MessageBoxButton.OKCancel) ==
                        MessageBoxResult.Cancel)
                    {
                        delete = false;
                    }
                    else
                    {
                        if (AnyRouteOutletHasPendingPayments())
                        {
                            string outletNames = RouteOutletsWithOutstandingPayments.Aggregate("",
                                                                                                   (current, outlet) =>
                                                                                                   current +
                                                                                                   ("\t- " + outlet.Name));

                            MessageBox.Show(
                                "The following route outlets have orders with outstanding payments and therefore cannot be deactivates\n" +
                                outletNames,
                                app + ": Outlets With Outstanding Payments Cannot Be Deactivated",
                                MessageBoxButton.OK);

                            MessageBox.Show("Route was not deactivated.", app + ": Deactivate Route Failed",
                                            MessageBoxButton.OK);
                            return false;
                        }
                        else
                        {
                            DeactivateRouteOutlets();
                        }
                    }
                }
            }
            return delete;
        }

        public async void ActivateRoute(Guid id)
        {
            var app = GetConfigParams().AppId.ToString();
            if (
                    MessageBox.Show("Are you sure you want to activate this route?",
                                    app +": Activate Route", MessageBoxButton.OKCancel) ==
                    MessageBoxResult.Cancel) return;
            using (StructureMap.IContainer c = NestedContainer)
            {
                ResponseBool response = null;
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                response = await proxy.RouteActivateAsync(id);

                MessageBox.Show(response.ErrorInfo, app +": ", MessageBoxButton.OK);

            }
        }

        public string CheckRouteUsed(Guid routeId, out bool canDeactivate)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                string info = "";
                _entityUsage = ObjectFactory.GetInstance<EntityUsage>();
                bool can = _entityUsage.CheckRouteHasOutlets(routeId, out _routeOutlets) == "";

                info += _entityUsage.CheckRouteHasOrders(routeId);

                canDeactivate = can;
                return info;
            }
        }
        bool OutletOrdersOutstandingPayments(Guid outletId)
        {
            using (var c = NestedContainer)
            {
                return Using<IMainOrderRepository>(c).GetAll().Any(p => p.IssuedOnBehalfOf != null && p.IssuedOnBehalfOf.Id == outletId && p.OutstandingAmount > 0);

            }
        }
        public bool AnyRouteOutletHasPendingPayments()
        {
                bool retVal = false;
                RouteOutletsWithOutstandingPayments = new List<Outlet>();

                foreach (Outlet outlet in _routeOutlets)
                {
                    if (OutletOrdersOutstandingPayments(outlet.Id))
                        RouteOutletsWithOutstandingPayments.Add(outlet);
                }

                if (RouteOutletsWithOutstandingPayments.Count > 0)
                    retVal = true;

                return retVal;
            
        }

        public void DeactivateRouteOutlets()
        {
            DeactivateRouteOutlets(_routeOutlets);
        }

        private async void DeactivateRouteOutlets(IEnumerable<Outlet> routeOutlets)
        {
            using (var c = NestedContainer)
            {

                if(routeOutlets.Any())
                {
                     var ids= routeOutlets.Select(n => n.Id).ToList();
             
              
                   var responses = new List<ResponseBool>();
                    var proxy = Using<IDistributorServiceProxy>(c);
                    foreach (var id in ids)
                    {
                       var response = await proxy.OutletDeactivateAsync(id);
                        responses.Add(response);
                        
                    }
                    if(!responses.Any(p=>p.Success))
                    {
                        var failed = responses.Where(p => !p.Success).Select(n => n.ErrorInfo).ToList();
                        var errors = new StringBuilder();
                        failed.ForEach(n=>errors.AppendLine(n));
                        MessageBox.Show(errors.ToString(), "Distributr:", MessageBoxButton.OK);
                    }
                    else
                    {
                        var firstOrDefault   = responses.FirstOrDefault();
                        if (firstOrDefault != null)
                            MessageBox.Show(firstOrDefault.ErrorInfo, "Distributr:", MessageBoxButton.OK);
                    }
                }
            }
        }

        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, _pagedroutes.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            Load();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(_pagedroutes.PageNumber, _pagedroutes.PageCount, _pagedroutes.TotalItemCount,
                                        _pagedroutes.IsFirstPage, _pagedroutes.IsLastPage);
        }

        protected override void EditSelected()
        {
            if (SelectedRoute == null) return;
            var app = GetConfigParams().AppId;
            if (app == Core.VirtualCityApp.Agrimanagr)
                SendNavigationRequestMessage(new Uri("/views/admin/routes/editroute.xaml?" + SelectedRoute.Id,
                                                                    UriKind.Relative));
        }

        protected override void DeleteSelected()
        {
            if (SelectedRoute == null) return;
            using (var c = NestedContainer)
            {
                if (Using<IMasterDataUsage>(c).CheckAgriRouteIsUsed(
                    Using<IRouteRepository>(c).GetById(SelectedRoute.Id), EntityStatus.Deleted))
                {
                    MessageBox.Show(
                        "Route " + SelectedRoute.Name +
                        " has been used in either a cost centre, outlet priority and or centre. Deactivate or delete dependencies to continue.",
                        "Agrimanagr: Delete Route", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }
            DeleteRoute(SelectedRoute.Id);
        }

        protected override void ActivateSelected()
        {
            if (SelectedRoute == null) return;
            if(SelectedRoute.EntityStatus == (int)EntityStatus.Active)
            {
                using (var c = NestedContainer)
                {
                    if (Using<IMasterDataUsage>(c).CheckAgriRouteIsUsed(Using<IRouteRepository>(c).GetById(SelectedRoute.Id), EntityStatus.Inactive))
                    {
                        MessageBox.Show(
                            "Route " + SelectedRoute.Name +
                            " has been used in either a cost centre, outlet priority and or centre. Deactivate or delete dependencies to continue.",
                            "Agrimanagr: Deactivate Route", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                    DeactivateRoute(SelectedRoute.Id);
                }
            }
            else if(SelectedRoute.EntityStatus == (int)EntityStatus.Inactive)
            {
                ActivateRoute(SelectedRoute.Id);
            }
        }
        #endregion

        #region helper classes

        public class ListRouteItem : ViewModelBase
        {
            public Guid Id { get; set; }
            public int RowNumber { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public string EmptyString { get; set; }

            public const string HlkDeactivateContentPropertyName = "HlkDeactivateContent";
            private string _hlkDeactivateContent = "Deactivate";

            public string HlkDeactivateContent
            {
                get { return _hlkDeactivateContent; }

                set
                {
                    if (_hlkDeactivateContent == value)
                    {
                        return;
                    }

                    _hlkDeactivateContent = value;
                    RaisePropertyChanged(HlkDeactivateContentPropertyName);
                }
            }

            public int EntityStatus { get; set; }
        }

        #endregion
    }
}
