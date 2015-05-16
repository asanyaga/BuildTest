using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
    internal class RouteRepository : RepositoryMasterBase<Route>, IRouteRepository
    {

        CokeDataContext _ctx;
        private IRegionRepository _regionRepository;
        ICacheProvider _cacheProvider;
        public RouteRepository(CokeDataContext ctx, ICacheProvider cacheProvider, IRegionRepository regionRepository)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _regionRepository = regionRepository;
        }

        public Guid Save(Route entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            _log.Debug("Save route");
            if (!vri.IsValid)
            {
                _log.Debug("Route not valid");
                throw new DomainValidationException(vri, "Route not valid");
            }

            DateTime dt = DateTime.Now;
            tblRoutes tblRoutes = _ctx.tblRoutes.FirstOrDefault(n => n.RouteID == entity.Id);
            if (tblRoutes == null)
            {
                tblRoutes = new tblRoutes();
                tblRoutes.IM_DateCreated = dt;
                tblRoutes.IM_Status = (int)EntityStatus.Active;// true;
                tblRoutes.RouteID = entity.Id;
                _ctx.tblRoutes.AddObject(tblRoutes);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (tblRoutes.IM_Status != (int)entityStatus)
                tblRoutes.IM_Status = (int)entity._Status;
            tblRoutes.Name = entity.Name;
            tblRoutes.Code = entity.Code;
            tblRoutes.RegionId = entity.Region.Id;
            tblRoutes.IM_DateLastUpdated = dt;

            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblRoutes.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.RouteID).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, tblRoutes.RouteID));
            _log.Debug("Route saved");
            return tblRoutes.RouteID;
        }

        private Route Map(tblRoutes route)
        {
            if (route == null)
                return null;
            Route r = new Route(route.RouteID)
            {
                Name = route.Name,
                Code = route.Code,
                Region = _regionRepository.GetById(route.RegionId)
            };
            r._SetDateCreated(route.IM_DateCreated);
            r._SetDateLastUpdated(route.IM_DateLastUpdated);
            r._SetStatus((EntityStatus)route.IM_Status);
            return r;
        }

        public void SetInactive(Route entity)
        {
            _log.Debug("Deactivating Route " + entity.Name);
            var validationResultInfo = entity.BasicValidation();
            var hasDependency = _ctx.tblSalemanRoute.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Any(n => n.RouteId == entity.Id);
            if (hasDependency)
            {
                throw new DomainValidationException(validationResultInfo, "Cannot deactivate - route has dependency");
            }
            tblRoutes route = _ctx.tblRoutes.FirstOrDefault(p => p.RouteID == entity.Id);
            if (route != null)
            {
                route.IM_Status = (int)EntityStatus.Inactive; //false;
                route.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey,
                                    _ctx.tblRoutes.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(
                                        s => s.RouteID).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, route.RouteID));
            }
        }

        public void SetActive(Route entity)
        {
            tblRoutes route = _ctx.tblRoutes.FirstOrDefault(p => p.RouteID == entity.Id);
            if (route != null)
            {
                route.IM_Status = (int)EntityStatus.Active;
                route.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblRoutes.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.RouteID).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, route.RouteID));
            }
        }

        public void SetAsDeleted(Route entity)
        {
            var validationResultInfo = entity.BasicValidation();
            var hasSalesmanRouteDependency = _ctx.tblSalemanRoute
                .Where(n => n.IM_Status != (int)EntityStatus.Deleted)
                .Any(n => n.RouteId == entity.Id);
            if (hasSalesmanRouteDependency)
            {
                throw new DomainValidationException(validationResultInfo, "Cannot delete - a salesman route has dependency");
            }
            var hasOutletDependency = _ctx.tblCostCentre
                    .Where(n => n.IM_Status != (int)EntityStatus.Deleted)
                    .Where(n => n.CostCentreType == (int)CostCentreType.Outlet)
                    .Any(n => n.RouteId == entity.Id);
            if (hasOutletDependency)
            {
                throw new DomainValidationException(validationResultInfo, "Cannot delete - an outlet has dependency");
            }
            var hasOutletPriorityDependency = _ctx.tblOutletPriority
                    .Where(n => n.IM_Status != (int)EntityStatus.Deleted)
                    .Any(n => n.RouteId == entity.Id);
            if (hasOutletPriorityDependency)
            {
                throw new DomainValidationException(validationResultInfo, "Cannot delete - an outlet priority has dependency");
            }
            var hasPurchasingClerkDependency = _ctx.tblPurchasingClerkRoute
                .Where(n => n.IM_Status != (int)EntityStatus.Deleted)
                .Any(n => n.RouteId == entity.Id);
            if (hasPurchasingClerkDependency)
            {
                throw new DomainValidationException(validationResultInfo, "Cannot delete - a purchasing clerk route has dependency");
            }
            var hasCentreDependency = _ctx.tblCentre
                .Where(n => n.IM_Status != (int)EntityStatus.Deleted)
                .Any(n => n.RouteId == entity.Id);
            if (hasCentreDependency)
            {
                throw new DomainValidationException(validationResultInfo, "Cannot delete - a centre has dependency");
            }
            _log.Debug("Deleting Route " + entity.Name);
            tblRoutes route = _ctx.tblRoutes.FirstOrDefault(p => p.RouteID == entity.Id);
            if (route != null)
            {
                route.IM_Status = (int)EntityStatus.Deleted;
                route.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblRoutes.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.RouteID).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, route.RouteID));
            }
        }

        public Route GetById(Guid Id, bool includeDeactivated = false)
        {
            Route entity = (Route)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblRoutes.FirstOrDefault(s => s.RouteID == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;

        }

        protected override string _cacheKey
        {
            get { return "Route-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "RouteList"; }
        }

        public override IEnumerable<Route> GetAll(bool includeDeactivated = false)
        {

            IList<Route> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<Route>(ids.Count);
                foreach (Guid id in ids)
                {
                    Route entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblRoutes.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(Map).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (Route p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }
            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;

        }

        
        public ValidationResultInfo Validate(Route itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            bool hasDuplicateCode = GetAll(true)
                .Where(r => r.Id != itemToValidate.Id)
                .Any(r => r.Code == itemToValidate.Code);

            bool hasDuplicateName = GetAll(true)
               .Where(s => s.Id != itemToValidate.Id)
               .Any(p => p.Name == itemToValidate.Name);

            if (hasDuplicateCode)
                vri.Results.Add(new ValidationResult("Duplicate Code found"));
            if (hasDuplicateName)
                vri.Results.Add(new ValidationResult("Duplicate Name found"));

            Region region = _regionRepository.GetById(itemToValidate.Region.Id);
            if (region == null)
            {
                vri.Results.Add(new ValidationResult("Invalid region assigned to route."));
            }
            return vri;

        }




        public QueryResult<Route> Query(QueryStandard query)
        {
            IQueryable<tblRoutes> routesQuery;
            if (query.ShowInactive)

                routesQuery = _ctx.tblRoutes.Where(p => p.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
            else
                routesQuery = _ctx.tblRoutes.Where(p => p.IM_Status == (int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<Route>();

            //excecute search filter
            if (!string.IsNullOrEmpty(query.Name))
            {
                routesQuery = routesQuery.Where(p => p.Name.ToLower().Contains(query.Name.ToLower())
                                                     || p.Code.ToLower().Contains(query.Name.ToLower())
                                                     || p.tblRegion.Name.ToLower().Contains(query.Name.ToLower()));
                
               }
            
            queryResult.Count = routesQuery.Count();

            //order costCentreQuery
            routesQuery = routesQuery.OrderBy(p => p.Name).ThenBy(p => p.Code).ThenBy(p => p.RegionId);
            
           //method paging
            if (query.Skip.HasValue && query.Take.HasValue)
            {
                routesQuery = routesQuery.Skip(query.Skip.Value).Take(query.Take.Value);

            }
            queryResult.Data = routesQuery.Select(Map).OfType<Route>().ToList();
            return queryResult;
        }

    }
}
