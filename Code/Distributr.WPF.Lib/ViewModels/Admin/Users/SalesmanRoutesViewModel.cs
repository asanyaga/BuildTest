using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.MasterDataDTO.DataContracts;
using Distributr.Core.Workflow.Impl.AuditLogs;

namespace Distributr.WPF.Lib.ViewModels.Admin.Users
{

    public class SalesmanRoutesViewModel : DistributrViewModelBase
    {
        //BusyWindow _isBusy = null;
        private List<User> CachedSalesmen;
        public SalesmanRoutesViewModel()
        {
            ClearAndSetupCommand = new RelayCommand(RunClearAndSetup);
           // UserLookupList = new ObservableCollection<Users.UserLookup>();
            RouteLookupList = new ObservableCollection<RouteLookup>();
            DistributorSalemanRoute = new ObservableCollection<DistributorSalemanRoute>();
            UserLookupList = new ObservableCollection<UserLookup>();

            GetRoutesCommand = new RelayCommand(GetDistributorSalemanRoute);
            SaveRouteAssignmentCommand = new RelayCommand(Save);
            GetRoutesNotAssignedCommand = new RelayCommand(GetRoutesNotAssigned);
        }

        #region Properties
        public RelayCommand GetRoutesCommand { get; set; }
        public RelayCommand SaveRouteAssignmentCommand { get; set; }
        public RelayCommand GetRoutesNotAssignedCommand { get; set; }
        public RelayCommand ClearAndSetupCommand { get; set; }

        public ObservableCollection<RouteLookup> RouteLookupList { get; set; }
        public ObservableCollection<DistributorSalemanRoute> DistributorSalemanRoute { get; set; }
        public ObservableCollection<UserLookup> UserLookupList { get; set; }
        private int counter = 0;

        public const string UserLookupPropertyName = "UserLookup";
        private UserLookup _UserLookup = null;
        private string AuditLogEntry { get; set; }
        public UserLookup UserLookup
        {
            get
            {
                return _UserLookup;
            }

            set
            {
                if (_UserLookup == value)
                {
                    return;
                }

                var oldValue = _UserLookup;
                _UserLookup = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(UserLookupPropertyName);


            }
        }

        public const string CanAssignRoutePropertyName = "CanAssignRoute";
        private bool _canAssignRoute = false;
        public bool CanAssignRoute
        {
            get
            {
                return _canAssignRoute;
            }

            set
            {
                if (_canAssignRoute == value)
                {
                    return;
                }

                var oldValue = _canAssignRoute;
                _canAssignRoute = value;
                RaisePropertyChanged(CanAssignRoutePropertyName);
            }
        }
         
        public const string ShowInactivePropertyName = "ShowInactive";
        private bool _showIactive = false;
        public bool ShowInactive
        {
            get
            {
                return _showIactive;
            }

            set
            {
                if (_showIactive == value)
                {
                    return;
                }

                var oldValue = _showIactive;
                _showIactive = value;
                RaisePropertyChanged(ShowInactivePropertyName);
            }
        }

        private Guid id;
        #endregion

        #region Methods
        private void RunClearAndSetup()
        {
            CachedSalesmen = new List<User>();
            Setup();
        }

        public async void Deactivate(Guid salemanRouteId)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                id = salemanRouteId;
                this.id = id;
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                ResponseBool response = null;
                //_isBusy = new BusyWindow();
                //_isBusy.lblWhatsUp.Content = "Activating salesman route.";
                //_isBusy.Show();
                response = await proxy.SalesmanRouteDeactivateAsync(salemanRouteId);
                MessageBox.Show(response.ErrorInfo, "Distributr: Manage Salesman Routes", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        public async void Activate(Guid id)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                this.id = id;
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                ResponseBool response = null;
                //_isBusy = new BusyWindow();
                //_isBusy.lblWhatsUp.Content = "Activating salesman route.";
                //_isBusy.Show();
                response = await proxy.SalesmanRouteActivateAsync(id);
                MessageBox.Show(response.ErrorInfo, "Distributr: Manage Salesman Routes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public async void Delete(Guid id)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                this.id = id;
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                ResponseBool response = null;
             
                response = await proxy.SalesmanRouteDeleteAsync(id);
                if(response.Success)
                {
                    var exists = Using<ISalesmanRouteRepository>(c).GetById(id);
                    if (exists!=null)
                        Using<ISalesmanRouteRepository>(c).SetAsDeleted(exists);
                    GetDistributorSalemanRoute();
                }

                MessageBox.Show(response.ErrorInfo, "Distributr: Manage Salesman Routes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void Save()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                //_isBusy = new BusyWindow();
                //_isBusy.lblWhatsUp.Content = "Saving salesman route.";
                //_isBusy.Show(); ResponseBool response = null;
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
               var salesmanRepository  = Using<ISalesmanRouteRepository>(c);
                ResponseBool response = null;
                if (DistributorSalemanRoute.Count > 0)
                {

                    var items = new ObservableCollection<DistributorSalesmanRouteItem>();
                    foreach (DistributorSalemanRoute r in DistributorSalemanRoute)
                    {
                        DistributorSalesmanRouteItem sr = new DistributorSalesmanRouteItem
                                                              {
                                                                  CostCentreMasterId = r.CostCentreId,
                                                                  RouteMasterId = r.RouteId,
                                                                  MasterId = r.MasterId
                                                              };
                        items.Add(sr);
                        AuditLogEntry = string.Format("Assigned Route: {0}; To Costcentre: {1};", r.RouteName,
                                                      r.CostCentreId);
                        Using<IAuditLogWFManager>(c).AuditLogEntry("User Administration", AuditLogEntry);
                    }

                    response = await proxy.SalesmanRoutesUpdateAsync(items.ToList());
                   
                   
                    if(
                    MessageBox.Show(response.ErrorInfo, "Distributr: Manage Salesman Routes", MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK)
                    {

                        RunClearAndSetup(); 
                    }
                }
                else
                {
                    MessageBox.Show("Please add routes first");
                }
            }
        }

        private void Setup()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                UserLookupList.Clear();
                CachedSalesmen.Clear();
               var salesman = new UserLookup
                    {
                        Id = Guid.Empty,
                        Username = GetLocalText("sl.userroute.salesman.default") /* "---Select Salesman---" */
                    };
                UserLookupList.Add(salesman);
                Using<IUserRepository>(c).GetAll(true).Where(o => o.UserType == UserType.DistributorSalesman).ToList()
                            .ForEach(n => CachedSalesmen.Add(n));
               
                CachedSalesmen.Where(n => n._Status == EntityStatus.Active).OrderBy(n => n.Username).ToList()
                              .ForEach(n => UserLookupList.Add(
                                  new UserLookup {Id = n.Id, Username = n.Username, CostCentreId = n.CostCentre}));

                UserLookup = salesman;
              
               
                CanAssignRoute = Using<IConfigService>(c).ViewModelParameters.CurrentUserRights.CanManageSalesmanRoutes;
            }
        }

        private void GetRoutesNotAssigned()
        {
            RouteLookupList.Clear();
            try
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    string[] RouteAvailable = DistributorSalemanRoute.Select(s => s.RouteId.ToString()).ToArray();
                    Using<IRouteRepository>(c).GetAll()
                                 .Where(w => !RouteAvailable.Contains(w.Id.ToString())).OrderBy(n => n.Name)
                                 .ToList()
                                 .ForEach(n => RouteLookupList.Add(new RouteLookup
                                     {
                                         Id = n.Id,
                                         RouteName = n.Name
                                     }));
                }
            }
            catch
            {
                throw new Exception("An error occured while loading Rotes Available list.");
            }
        }

        private void GetDistributorSalemanRoute()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                DistributorSalemanRoute.Clear();
                
                List<User> userList = Using<IUserRepository>(c).GetAll(true).ToList();
                if (UserLookup == null)
                    UserLookup = UserLookupList.FirstOrDefault(n => n.Id == Guid.Empty);
                if(UserLookup==null)
                {
                  UserLookup = new UserLookup
                    {
                        Id = Guid.Empty,
                        Username = GetLocalText("sl.userroute.salesman.default") /* "---Select Salesman---" */
                    };
                }

                if (UserLookup.Id == Guid.Empty)
                {
                    var salesmanRoutes = Using<ISalesmanRouteRepository>(c).GetAll(ShowInactive).ToList();
                    salesmanRoutes.Where(n => n.Route != null).ToList()
                                  .ForEach(p =>
                                      {
                                          try
                                          {
                                              var item = new DistributorSalemanRoute
                                                  {
                                                      RouteId = p.Route.Id,
                                                      RouteName = p.Route.Name,
                                                      MasterId = p.Id,
                                                      CostCentreId = p.DistributorSalesmanRef.Id,

                                                      SalesmanNames = userList.Where(
                                                          w => w.CostCentre == p.DistributorSalesmanRef.Id)
                                                                              .First() != null
                                                                          ? userList
                                                                                .Where(
                                                                                    w =>
                                                                                    w.CostCentre ==
                                                                                    p.DistributorSalesmanRef.Id)
                                                                                .First().Username
                                                                          : "",

                                                      EntityStatus = (int) p._Status
                                                  };
                                              if (item.EntityStatus == (int) EntityStatus.Active)
                                                  item.HlkDeactivateContent =
                                                      GetLocalText("sl.userroute.grid.col.deactivate");
                                              else if (item.EntityStatus == (int) EntityStatus.Inactive)
                                                  item.HlkDeactivateContent =
                                                      GetLocalText("sl.userroute.grid.col.activate");

                                              DistributorSalemanRoute.Add(item);
                                          }
                                          catch
                                          {
                                          }
                                      });
                }
                else
                {
                    var salesmanRoutes = Using<ISalesmanRouteRepository>(c).GetAll(ShowInactive).ToList();
                    salesmanRoutes.Where(n => n.Route != null).OrderBy(o => o.Route.Name)
                                  .Where(n => n.DistributorSalesmanRef.Id == UserLookup.CostCentreId)
                                  .ToList()
                                  .ForEach(p =>
                                      {
                                          var item = new DistributorSalemanRoute
                                              {
                                                  RouteId = p.Route.Id,
                                                  RouteName = p.Route.Name,
                                                  MasterId = p.Id,
                                                  CostCentreId = p.DistributorSalesmanRef.Id,
                                                  SalesmanNames =
                                                      userList.Where(w => w.CostCentre == p.DistributorSalesmanRef.Id)
                                                              .First()
                                                              .Username,
                                                  EntityStatus = (int) p._Status
                                              };

                                          if (item.EntityStatus == (int) EntityStatus.Active)
                                              item.HlkDeactivateContent =
                                                  GetLocalText("sl.userroute.grid.col.deactivate");
                                          else if (item.EntityStatus == (int) EntityStatus.Inactive)
                                              item.HlkDeactivateContent =
                                                  GetLocalText("sl.userroute.grid.col.activate");

                                          DistributorSalemanRoute.Add(item);
                                      });
                }
            }
        }


        public bool SalesmanRouteCheck(Guid salesmanId)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                var salesmanRoutes = Using<ISalesmanRouteRepository>(c).GetAll(ShowInactive).ToList();
                if (salesmanRoutes!= null)
                {
                int unalocatedRouteSalesman = salesmanRoutes.Where(r => r.DistributorSalesmanRef.Id == salesmanId).Count();
                if (unalocatedRouteSalesman == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void ReloadSalesmen()
        {
            if (ShowInactive)
            {
                CachedSalesmen.Where(n => n._Status == EntityStatus.Inactive).ToList()
                    .ForEach(n => UserLookupList.Add(new UserLookup
                                                                   {
                                                                       Id = n.Id,
                                                                       CostCentreId = n.CostCentre,
                                                                       Username = n.Username
                                                                   }));
            }
            else
            {
                var inactive = CachedSalesmen.Where(n => n._Status == EntityStatus.Inactive).ToList();
                foreach(var item in inactive)
                {
                    var user = UserLookupList.FirstOrDefault(n => n.Id == item.Id);
                    if (user != null)
                        UserLookupList.Remove(user);
                }
            }
        }

        public void AddRoute(RouteLookup routeLookup)
        {
            if (routeLookup != null)
            {
                DistributorSalemanRoute dsr = new DistributorSalemanRoute()
                                                  {
                                                      CostCentreId = UserLookup.CostCentreId,
                                                      MasterId = Guid.NewGuid(),
                                                      RouteId = routeLookup.Id,
                                                      RouteName = routeLookup.RouteName
                                                  };
                DistributorSalemanRoute.Add(dsr);
            }
        }
        #endregion
    }

    #region Helper Classes
    public class SalesmanRouteItemViewModel : ViewModelBase
    {
        public ObservableCollection<RouteLookup> RouteLookupList { get; set; }
        public SalesmanRouteItemViewModel()
        {
            RouteLookupList = new ObservableCollection<RouteLookup>();
        }
        public const string RouteLookupPropertyName = "RouteLookup";
        private RouteLookup _RouteLookup = null;
        public RouteLookup RouteLookup
        {
            get
            {
                return _RouteLookup;
            }

            set
            {
                if (_RouteLookup == value)
                {
                    return;
                }

                var oldValue = _RouteLookup;
                _RouteLookup = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(RouteLookupPropertyName);


            }
        }
    }

    public class DistributorSalemanRoute
    {
        public Guid MasterId { get; set; }
        public string RouteName { set; get; }
        public Guid RouteId { set; get; }
        public Guid CostCentreId { set; get; }
        public string SalesmanNames { get; set; }
        public string HlkDeactivateContent { get; set; }
        public int EntityStatus { get; set; }
    }

    public class UserLookup
    {
        public Guid Id { get; set; }
        public string Username { set; get; }
        public Guid CostCentreId { set; get; }
    }

    public class RouteLookup
    {
        public Guid Id { get; set; }
        public string RouteName { set; get; }

    }
    #endregion
}