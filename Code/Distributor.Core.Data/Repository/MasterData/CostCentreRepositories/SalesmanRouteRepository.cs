using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
    internal class SalesmanRouteRepository : RepositoryMasterBase<SalesmanRoute>, ISalesmanRouteRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        private IRouteRepository _routeRepository;
        private IDistributorSalesmanRepository _distributorSalesmanRepository;


        public SalesmanRouteRepository(CokeDataContext ctx, ICacheProvider cacheProvider, IRouteRepository routeRepository, IDistributorSalesmanRepository distributorSalesmanRepository)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _routeRepository = routeRepository;
            _distributorSalesmanRepository = distributorSalesmanRepository;
        }


        protected override string _cacheKey
        {
            get { return "SalesmanRoute-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "SalesmanRouteList"; }
        }

        public override IEnumerable<SalesmanRoute> GetAll(bool includeDeactivated = false)
        {
            _log.Debug("Get all SalesmanRoute");
            IList<SalesmanRoute> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<SalesmanRoute>(ids.Count);
                foreach (Guid id in ids)
                {
                    SalesmanRoute entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblSalemanRoute.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (SalesmanRoute p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        public void Delete(Guid id)
        {
            tblSalemanRoute delObj = _ctx.tblSalemanRoute.FirstOrDefault(p => p.Id == id);
            if (delObj != null)
                _ctx.tblSalemanRoute.DeleteObject(delObj);
            _ctx.SaveChanges();
        }

        public SalesmanRoute Map(tblSalemanRoute salemanRoute)
        {
            SalesmanRoute salemanroute1 = new SalesmanRoute(salemanRoute.Id)
            {
                //_Status = region.Active.Value,
                Route = _routeRepository.GetById(salemanRoute.RouteId, true),
                DistributorSalesmanRef= new CostCentreRef{Id=salemanRoute.SalemanId},
                
            };
            salemanroute1._SetDateCreated(salemanRoute.IM_DateCreated);
            salemanroute1._SetDateLastUpdated(salemanRoute.IM_DateLastUpdated);
            salemanroute1._SetStatus((EntityStatus)salemanRoute.IM_Status);

            return salemanroute1;
        }

        public Guid Save(SalesmanRoute entity, bool? isSync = null)
        {
            _log.Debug("Saving/Updating SalesmanRoute");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                _log.Debug("Failed to save invalid SalesmanRoute");
                throw new DomainValidationException(vri, "Failed to save invalid SalesmanRoute");
            }
            tblSalemanRoute salemanroute = _ctx.tblSalemanRoute.FirstOrDefault(n => n.Id == entity.Id);
            DateTime date = DateTime.Now;
            if (salemanroute == null)
            {
                salemanroute = new tblSalemanRoute
                {
                    IM_DateCreated = date,
                    IM_Status =(int)EntityStatus.Active,// true,
                    Id= entity.Id
                };
                _ctx.tblSalemanRoute.AddObject(salemanroute);
                salemanroute.IM_DateLastUpdated = date;
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (salemanroute.IM_Status != (int)entityStatus)
                salemanroute.IM_Status = (int)entity._Status;
            salemanroute.RouteId = entity.Route.Id;
            salemanroute.SalemanId = entity.DistributorSalesmanRef.Id;
            _log.Debug("Saving SalesmanRoute");
            _ctx.SaveChanges();
            _log.Debug("Invalidating cache");
            _cacheProvider.Put(_cacheListKey, _ctx.tblSalemanRoute.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, salemanroute.Id));
            _log.DebugFormat("Successfully saved item id:{0}", salemanroute.Id);
            return salemanroute.Id;
        }

        public void SetActive(SalesmanRoute entity)
        {
            _log.Debug("Activating SalesmanRoute Id " + entity.Id);
            ValidationResultInfo vri = Validate(entity);
            if (!vri.IsValid)
            {
                _log.Debug("Failed to activate invalid salesman route");
                throw new DomainValidationException(vri, "Failed to save invalid SalesmanRoute");
            }

            tblSalemanRoute tblSr = _ctx.tblSalemanRoute.FirstOrDefault(n => n.Id == entity.Id);
            if (tblSr != null)
            {
                tblSr.IM_Status = (int) EntityStatus.Active;
                tblSr.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblSalemanRoute.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblSr.Id));
            }
        }

        public void SetInactive(SalesmanRoute entity)
        {
            _log.Debug("Inactivating SalesmanRoute");
            
            bool dependenciesPresent = false;

            string failureReason = "";
            if (dependenciesPresent)
            {
                failureReason = "DEPENDENCIES FOUND:";//populate with ids
                throw new ArgumentException(failureReason);
            }
            else
            {
                tblSalemanRoute sales = _ctx.tblSalemanRoute.First(n => n.Id == entity.Id);
                if (sales == null || sales.IM_Status==(int)EntityStatus.Inactive)
                {//not existing or already deactivated.
                    return;
                }
                sales.IM_Status = (int)EntityStatus.Inactive;// false;
                sales.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblSalemanRoute.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, sales.Id));
            }
        }

        public void SetAsDeleted(SalesmanRoute entity)
        {
          _log.Debug("Deleted SalesmanRoute Id: "+ entity.Id);

            bool dependenciesPresent = false;

            string failureReason = "";
            if (dependenciesPresent)
            {
                failureReason = "DEPENDENCIES FOUND:";//populate with ids
                throw new ArgumentException(failureReason);
            }
            else
            {
                tblSalemanRoute sales = _ctx.tblSalemanRoute.First(n => n.Id == entity.Id);
                if (sales == null || sales.IM_Status == (int)EntityStatus.Deleted)
                {//not existing or already deactivated.
                    return;
                }
                sales.IM_Status = (int)EntityStatus.Deleted;// false;
                sales.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                var val = _ctx.tblSalemanRoute.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList();
                _cacheProvider.Put(_cacheListKey, val);
                _cacheProvider.Remove(string.Format(_cacheKey, sales.Id));
            }
        }

        public SalesmanRoute GetById(Guid Id, bool includeDeactivated = false)
        {
            _log.DebugFormat("Getting SalesmanRoute by ID: {0}", Id);
            SalesmanRoute entity = (SalesmanRoute)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblSalemanRoute.FirstOrDefault(s => s.Id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public ValidationResultInfo Validate(SalesmanRoute itemToValidate)
        {
            bool hasDuplicateSalesmanRoute = false;
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            if (itemToValidate.Route != null && itemToValidate.DistributorSalesmanRef != null)
            {
                hasDuplicateSalesmanRoute = _ctx.tblSalemanRoute
                    .Where(n => n.IM_Status != (int)EntityStatus.Deleted && n.Id != itemToValidate.Id)
                    .Any(n => n.RouteId == itemToValidate.Route.Id && n.SalemanId == itemToValidate.DistributorSalesmanRef.Id);
            }
            if (hasDuplicateSalesmanRoute)
            {
                vri.Results.Add(new ValidationResult("Duplicate Distributor saleman Route found"));
            }
            return vri;
        }
     
    }
}
