using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.MasterDataAllocationRepositories;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.RouteViewModel;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;


namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.RouteViewBuilder.Impl
{
    public class AdminRouteViewModelBuilder : IAdminRouteViewModelBuilder
    {
        
        IRouteRepository _routeRepository;
        IHubRepository _hubRepository;
        IRouteFactory _routeFactory;
        private IRegionRepository _regionRepository;
        
        private IMasterDataAllocationRepository _masterDataAllocationRepository;
        private IMasterDataUsage _masterDataUsage;
        public AdminRouteViewModelBuilder
            (
             IRouteRepository routeRepository,
             IHubRepository hubRepository,
             IRouteFactory routeFactory, IRegionRepository regionRepository, IMasterDataAllocationRepository masterDataAllocationRepository, IMasterDataUsage masterDataUsage)
        {
            _routeRepository = routeRepository;
            _hubRepository = hubRepository;
            _routeFactory = routeFactory;
            _regionRepository = regionRepository;
            _masterDataAllocationRepository = masterDataAllocationRepository;
            _masterDataUsage = masterDataUsage;
        }

        public IList<AdminRouteViewModel> GetAll(bool inactive = false)
        {
            var route = _routeRepository.GetAll(inactive);
            return route
                .Select(n =>
                            {
                                Hub hub = _hubRepository.GetByRegionId(n.Region.Id).FirstOrDefault() as Hub;
                                AdminRouteViewModel vm = new AdminRouteViewModel
                                                             {
                                                                 Id = n.Id,
                                                                 Name = n.Name,
                                                                 Code = n.Code,
                                                                 isActive = n._Status == EntityStatus.Active ? true : false,
                                                                 RegionId = n.Region.Id,
                                                                 RegionName = n.Region.Name
                                                             };
                                if (hub != null)
                                {
                                    vm.HubName = hub.Name;
                                    vm.HubId = hub.Id;
                                }
                                return vm;
                            })
                .ToList();
        }

        public AdminRouteViewModel Get(Guid id)
        {
            Route route = _routeRepository.GetById(id);
            if (route == null) return null;
            
            return Map(route);

        }

        public void Save(AdminRouteViewModel vm)
        {
            Region region = _regionRepository.GetById(vm.RegionId);
            if (region == null)
            {
                throw new ValidationException("Selected region is not valid.");
            }
            Route route = _routeRepository.GetById(vm.Id);
            if (route == null)
            {
                route = new Route(Guid.NewGuid());
            }
            route.Name = vm.Name;
            route.Code = vm.Code;
            route.Region = region;
            MasterDataAllocation allocation = new MasterDataAllocation(Guid.NewGuid())
                                                  {
                                                      AllocationType = MasterDataAllocationType.RouteRegionAllocation,
                                                      EntityAId = route.Id,
                                                      EntityBId = vm.Id,
                                                  };
            _routeRepository.Save(route);
            _masterDataAllocationRepository.Save(allocation);
        }

        public void SetInactive(Guid id)
        {
            Route route = _routeRepository.GetById(id);
            if (_masterDataUsage.CheckAgriRouteIsUsed(route, EntityStatus.Inactive))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot deactivate route since it has dependencies. Remove dependencies first to continue");
            }
            _routeRepository.SetInactive(route);
        }

        public void SetActive(Guid id)
        {
            Route route = _routeRepository.GetById(id);
            _routeRepository.SetActive(route);
        }

        AdminRouteViewModel Map(Route route)
        {
            Hub hub = _hubRepository.GetByRegionId(route.Region.Id).FirstOrDefault() as Hub;
            AdminRouteViewModel vm = new AdminRouteViewModel
            {
                Id = route.Id,
                Name = route.Name,
                Code = route.Code,
                isActive = route._Status == EntityStatus.Active ? true : false, 
                 RegionId = route.Region.Id,
                RegionName = route.Region.Name,
                
            };
            if (hub != null)
            {
                vm.HubName = hub.Name;
                vm.HubId = hub.Id;
            }
            return vm;

        }

        public Dictionary<Guid, string> Distributor()
        {

            return _hubRepository.GetAll()
                .Select(s => s as Hub)
                .ToList().ToDictionary(d => d.Id, d => d.Name);
        }

        public Dictionary<Guid, string> Regions()
        {
            return _regionRepository.GetAll()
                .ToDictionary(n => n.Id, n => n.Name);
        }

        public Dictionary<Guid, string> Hubs()
        {
            return _hubRepository.GetAll()
                .ToDictionary(n => n.Id, n => n.Name);
        }

        public QueryResult<AdminRouteViewModel> Query(QueryStandard query)
        {
            var queryResults = _routeRepository.Query(query);
            var result = new QueryResult<AdminRouteViewModel>();
            result.Data = queryResults.Data.Select(Map).ToList();
            result.Count = queryResults.Count;
            return result;

        }

        public IList<AdminRouteViewModel> Querylist(QueryResult result)
        {
            return result.Data.OfType<Route>().ToList().Select(Map).ToList();
        }

        public List<AdminRouteViewModel> GetByDistributor(Guid regionId)
        {
            return _routeRepository.GetAll().ToList().OrderBy(n => n.Name).Where(n => n.Region.Id == regionId).Select(n => Map(n)).ToList();
        }

        public List<AdminRouteViewModel> GetDefaultRoute()
        {
            var defaultRoute=_routeRepository.GetAll().Select(n=>Map(n)).ToList();
            return defaultRoute;
        }

        public void SetAsDeleted(Guid id)
        {
            Route route = _routeRepository.GetById(id);
            if (_masterDataUsage.CheckAgriRouteIsUsed(route, EntityStatus.Inactive))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot deactivate route since it has dependencies. Remove dependencies first to continue");
            }
            _routeRepository.SetAsDeleted(route);
        }
    }


}

