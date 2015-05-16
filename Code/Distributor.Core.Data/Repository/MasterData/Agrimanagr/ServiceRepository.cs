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
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.Agrimanagr
{
    public class ServiceRepository:RepositoryMasterBase<CommodityProducerService>,IServiceRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;

        public ServiceRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
        }

        

        public Guid Save(CommodityProducerService entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }

            if (!vri.IsValid)
            {
                _log.Debug(CoreResourceHelper.GetText("Service.validation.error"));
                throw new DomainValidationException(vri, "");
            }
            DateTime dt = DateTime.Now;

            tblService service = _ctx.tblService.FirstOrDefault(n => n.id == entity.Id);
            if (service == null)
            {
                service = new tblService();
                service.id = entity.Id;
                service.IM_Status = (int)EntityStatus.Active;
                service.IM_DateCreated = dt;
                _ctx.tblService.AddObject(service);

            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (service.IM_Status != (int)entityStatus)
                service.IM_Status = (int)entity._Status;
            service.Code = entity.Code;
            service.Name = entity.Name;
            service.Cost = entity.Cost;
            service.Description = entity.Description; 
            service.IM_DateLastUpdated = dt;
           
            
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblService.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, service.id));
            return service.id; 
        }

        public void SetInactive(CommodityProducerService entity)
        {
            var service = _ctx.tblService.FirstOrDefault(n => n.id == entity.Id);
            if (service != null)
            {
                service.IM_Status = (int)EntityStatus.Inactive;
                service.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblService.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, service.id));
            }
        }

        public void SetActive(CommodityProducerService entity)
        {
            var service = _ctx.tblService.FirstOrDefault(n => n.id == entity.Id);
            if (service != null)
            {
                service.IM_Status = (int)EntityStatus.Active;
                service.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblService.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, service.id));
            }
        }

        public void SetAsDeleted(CommodityProducerService entity)
        {
            var service = _ctx.tblService.FirstOrDefault(n => n.id == entity.Id);
            if (service != null)
            {
                service.IM_Status = (int)EntityStatus.Deleted;
                service.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblService.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, service.id));
            }
        }

        public CommodityProducerService GetById(Guid Id, bool includeDeactivated = false)
        {
            CommodityProducerService entity = (CommodityProducerService)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblService.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public override IEnumerable<CommodityProducerService> GetAll(bool includeDeactivated = false)
        {
            IList<CommodityProducerService> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<CommodityProducerService>(ids.Count);
                foreach (Guid id in ids)
                {
                    CommodityProducerService entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblService.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(Map).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList();
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (CommodityProducerService p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }
            entities.ToList();
            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

     
        public QueryResult<CommodityProducerService> Query(QueryBase query)
        {
            var q = query as QueryCommodityProducerService;
            IQueryable<tblService> commodityProducerServiceQuery;
            if (q.ShowInactive)
                commodityProducerServiceQuery = _ctx.tblService.Where(p => p.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
            else
                commodityProducerServiceQuery = _ctx.tblService.Where(s => s.IM_Status == (int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<CommodityProducerService>();
            if (!string.IsNullOrWhiteSpace(q.Name))
            {
                commodityProducerServiceQuery = commodityProducerServiceQuery
                    .Where(s => s.Name.ToLower().Contains(q.Name.ToLower()) || s.Code.ToLower().Contains(q.Name.ToLower()));
            }

            queryResult.Count = commodityProducerServiceQuery.Count();
            commodityProducerServiceQuery = commodityProducerServiceQuery.OrderBy(s => s.Name).ThenBy(s => s.Code);
            if (q.Skip.HasValue && q.Take.HasValue)
                commodityProducerServiceQuery = commodityProducerServiceQuery.Skip(q.Skip.Value).Take(q.Take.Value);
            var result = commodityProducerServiceQuery.ToList();
            queryResult.Data = result.Select(Map).OfType<CommodityProducerService>().ToList();
            q.ShowInactive = false;
            return queryResult;
        }

        public ValidationResultInfo Validate(CommodityProducerService itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;

            var query = _ctx.tblService.Where(p => p.IM_Status != (int)EntityStatus.Deleted);
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));

            if (query.Any(s => s.id != itemToValidate.Id && !string.IsNullOrEmpty(s.Code) && s.Code == itemToValidate.Code))
                vri.Results.Add(new ValidationResult("Duplicate Code found"));

            if (query.Any(s => s.id != itemToValidate.Id && s.Name == itemToValidate.Name))
                vri.Results.Add(new ValidationResult("Duplicate Code found"));

            return vri;
        }

        public CommodityProducerService Map(tblService s)
        {
            var service = new CommodityProducerService(s.id)
            {
                Code = (s.Code ?? ""),
                Description = s.Description??"",
                Name = s.Name,
                Cost=s.Cost??0
                
            };
            service._SetDateCreated(s.IM_DateCreated);
            service._SetDateLastUpdated(s.IM_DateLastUpdated);
            service._SetStatus((EntityStatus)s.IM_Status);
            return service;
        }
        protected override string _cacheKey
        {
            get { return "Service-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "ServiceList"; }
        }
    }
}
