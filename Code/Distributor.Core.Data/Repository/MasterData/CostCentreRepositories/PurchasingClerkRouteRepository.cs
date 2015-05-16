using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
    internal class PurchasingClerkRouteRepository : RepositoryMasterBase<PurchasingClerkRoute>, IPurchasingClerkRouteRepository
    {

        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        private IRouteRepository _routeRepository;
      
        public PurchasingClerkRouteRepository(CokeDataContext ctx, ICacheProvider cacheProvider, IRouteRepository routeRepository)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _routeRepository = routeRepository;
        }

        protected override string _cacheKey
        {
            get { return "PurchasingClerkRoute-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "PurchasingClerkRouteList"; }
        }

        public Guid Save(PurchasingClerkRoute entity, bool? isSync = null)
        {
            _log.Debug("Saving/Updating PurchasingClerkRoute");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                _log.Debug("Failed to save invalid PurchasingClerkRoute");
                throw new DomainValidationException(vri, "Failed to save invalid PurchasingClerkRoute");
            }
            tblPurchasingClerkRoute purchasingClerkRoute = _ctx.tblPurchasingClerkRoute.FirstOrDefault(n => n.Id == entity.Id);
            DateTime date = DateTime.Now;
            if (purchasingClerkRoute == null)
            {
                purchasingClerkRoute = new tblPurchasingClerkRoute
                {
                    IM_DateCreated = date,
                    IM_Status = (int)EntityStatus.Active,
                    Id = entity.Id
                };
                _ctx.tblPurchasingClerkRoute.AddObject(purchasingClerkRoute);
            }
            var entityStatus = (entity._Status == null || entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (purchasingClerkRoute.IM_Status != (int)entityStatus)
                purchasingClerkRoute.IM_Status = (int)entity._Status;
            purchasingClerkRoute.RouteId = entity.Route.Id;
            purchasingClerkRoute.PurchasingClerkId = entity.PurchasingClerkRef.Id;
            purchasingClerkRoute.IM_DateLastUpdated = date;
            _log.Debug("Saving PurchasingClerkRoute");
            _ctx.SaveChanges();
            _log.Debug("Invalidating cache");
            _cacheProvider.Put(_cacheListKey, _ctx.tblSalemanRoute.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, purchasingClerkRoute.Id));
            _log.DebugFormat("Successfully saved item id:{0}", purchasingClerkRoute.Id);
            return purchasingClerkRoute.Id;
        }

        public void SetInactive(PurchasingClerkRoute entity)
        {
            _log.Debug("Inactivating PurchasingClerkRoute");

                tblPurchasingClerkRoute sales = _ctx.tblPurchasingClerkRoute.First(n => n.Id == entity.Id);
                if (sales == null || sales.IM_Status == (int)EntityStatus.Inactive)
                {//not existing or already deactivated.
                    return;
                }
                sales.IM_Status = (int)EntityStatus.Inactive;
                sales.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblSalemanRoute.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, sales.Id));
            
        }

        public void SetActive(PurchasingClerkRoute entity)
        {
            _log.Debug("Activating PurchasingClerkRoute Id " + entity.Id);
            ValidationResultInfo vri = Validate(entity);
            if (!vri.IsValid)
            {
                _log.Debug("Failed to activate invalid PurchasingClerkRoute route");
                throw new DomainValidationException(vri, "Failed to save invalid PurchasingClerkRoute");
            }

            tblPurchasingClerkRoute tblSr = _ctx.tblPurchasingClerkRoute.FirstOrDefault(n => n.Id == entity.Id);
            if (tblSr != null)
            {
                tblSr.IM_Status = (int)EntityStatus.Active;
                tblSr.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblSalemanRoute.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblSr.Id));
            }
        }

        public void SetAsDeleted(PurchasingClerkRoute entity)
        {
            _log.Debug("Deleted PurchasingClerkRoute Id: " + entity.Id);

                tblPurchasingClerkRoute sales = _ctx.tblPurchasingClerkRoute.First(n => n.Id == entity.Id);
                if (sales == null || sales.IM_Status == (int)EntityStatus.Deleted)
                {//not existing or already deactivated.
                    return;
                }
                sales.IM_Status = (int)EntityStatus.Deleted;
                sales.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                var val = _ctx.tblSalemanRoute.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList();
                _cacheProvider.Put(_cacheListKey, val);
                _cacheProvider.Remove(string.Format(_cacheKey, sales.Id));
            
        }

        public PurchasingClerkRoute GetById(Guid Id, bool includeDeactivated = false)
        {
            _log.DebugFormat("Getting SalesmanRoute by ID: {0}", Id);
            PurchasingClerkRoute entity = (PurchasingClerkRoute)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblPurchasingClerkRoute.FirstOrDefault(s => s.Id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public override IEnumerable<PurchasingClerkRoute> GetAll(bool includeDeactivated = false)
        {
            _log.Debug("Get all PurchasingClerkRoute");
            IList<PurchasingClerkRoute> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<PurchasingClerkRoute>(ids.Count);
                foreach (Guid id in ids)
                {
                    PurchasingClerkRoute entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblPurchasingClerkRoute.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); 
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (PurchasingClerkRoute p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        public IEnumerable<PurchasingClerkRoute> Query(QueryPurchasingClerkRoute query)
        {
            var q = query as QueryPurchasingClerkRoute;
             IQueryable<tblPurchasingClerkRoute> purchasingClerkRouteAllocation;

             if (q.ShowInactive)
             {
                 purchasingClerkRouteAllocation = _ctx.tblPurchasingClerkRoute.Where(p=>p.IM_Status!=(int)EntityStatus.Deleted).AsQueryable();
                    
             }
             else
             {
                 purchasingClerkRouteAllocation = _ctx.tblPurchasingClerkRoute.Where(p=>p.IM_Status==(int)EntityStatus.Active).AsQueryable();//.Where(s => s.CostCentreId == q.SupplierId).AsQueryable();
             }


            var result = purchasingClerkRouteAllocation.ToList();
              var data = result.Select(Map).ToList();
               
            return data;
        }

        public void Delete(Guid id)
        {
            tblPurchasingClerkRoute delObj = _ctx.tblPurchasingClerkRoute.FirstOrDefault(p => p.Id == id);
            if (delObj != null)
                _ctx.tblPurchasingClerkRoute.DeleteObject(delObj);
            _ctx.SaveChanges();
        }

       

        public ValidationResultInfo Validate(PurchasingClerkRoute itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));

            bool hasDuplicate = _ctx.tblPurchasingClerkRoute.Where(n => n.IM_Status != (int)EntityStatus.Deleted && n.Id != itemToValidate.Id)
                .Any(n => n.RouteId == itemToValidate.Route.Id && n.PurchasingClerkId == itemToValidate.PurchasingClerkRef.Id);
            if (hasDuplicate)
            {
                vri.Results.Add(new ValidationResult("Duplicate Distributor saleman Route found"));
            }
            return vri;
        }
       
        public PurchasingClerkRoute Map(tblPurchasingClerkRoute salemanRoute)
        {
            PurchasingClerkRoute salemanroute1 = new PurchasingClerkRoute(salemanRoute.Id)
            {
                Route = _routeRepository.GetById(salemanRoute.RouteId, true),
                PurchasingClerkRef = new CostCentreRef { Id = salemanRoute.PurchasingClerkId },

            };
            salemanroute1._SetDateCreated(salemanRoute.IM_DateCreated);
            salemanroute1._SetDateLastUpdated(salemanRoute.IM_DateLastUpdated);
            salemanroute1._SetStatus((EntityStatus)salemanRoute.IM_Status);

            return salemanroute1;
        }

    }
}
