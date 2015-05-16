using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.MasterDataAllocationRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.CentreRepositories
{
    class CentreRepository : RepositoryMasterBase<Centre>, ICentreRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        ICentreTypeRepository _centreTypeRepository;
        IRouteRepository _routeRepository;
        private IMasterDataAllocationRepository _masterDataAllocationRepo;
        private ICostCentreRepository _costCentreRepository;

        public CentreRepository(CokeDataContext ctx, ICacheProvider cacheProvider, ICentreTypeRepository centreTypeRepository, IRouteRepository routeRepository, IMasterDataAllocationRepository masterDataAllocationRepo, ICostCentreRepository costCentreRepository)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _centreTypeRepository = centreTypeRepository;
            _routeRepository = routeRepository;
            _masterDataAllocationRepo = masterDataAllocationRepo;
            _costCentreRepository = costCentreRepository;
        }

        protected override string _cacheKey
        {
            get { return "Centre-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "CentreList"; }
        }

        public Guid Save(Centre entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                _log.Debug(CoreResourceHelper.GetText("centre.validation.error"));
                throw new DomainValidationException(vri, CoreResourceHelper.GetText("centre.validation.error"));
            }
            DateTime dt = DateTime.Now;

            tblCentre centre = _ctx.tblCentre.FirstOrDefault(n => n.Id == entity.Id);
            if (centre == null)
            {
                centre = new tblCentre();
                centre.Id = entity.Id;
                centre.IM_Status = (int)EntityStatus.Active;
                centre.IM_DateCreated = dt;
                centre.Id = entity.Id;
                centre.CentreTypeId = entity.CenterType.Id;
                _ctx.tblCentre.AddObject(centre);

            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (centre.IM_Status != (int)entityStatus)
                centre.IM_Status = (int)entity._Status;
            centre.Code = entity.Code;
            centre.Name = entity.Name;
            centre.Description = entity.Description;
            centre.HubId = entity.Hub.Id;
            centre.RouteId = entity.Route.Id;
            centre.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblCentre.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, centre.Id));
            return centre.Id; 
        }

        public void SetInactive(Centre entity)
        {
            tblCentre centre = _ctx.tblCentre.FirstOrDefault(n => n.Id == entity.Id);
            if (centre != null)
            {
                centre.IM_Status = (int)EntityStatus.Inactive;
                centre.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCentre.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, centre.Id));
            }
        }

        public void SetActive(Centre entity)
        {
            tblCentre centre = _ctx.tblCentre.FirstOrDefault(n => n.Id == entity.Id);
            if (centre != null)
            {
                centre.IM_Status = (int)EntityStatus.Active;
                centre.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCentre.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, centre.Id));
            }
        }

        public void SetAsDeleted(Centre entity)
        {
            tblCentre centre = _ctx.tblCentre.FirstOrDefault(n => n.Id == entity.Id);

            if (IsAllocated(entity.Id))
            {
                var vri = new ValidationResultInfo();
                vri.Results.Add(new ValidationResult("This center Cannot be deleted since it has been allocated"));

                _log.Debug(CoreResourceHelper.GetText("centre.validation.error"));
                throw new DomainValidationException(vri, "This center Cannot be deleted since it has been allocated");
            }

            if (centre != null)
            {
                centre.IM_Status = (int)EntityStatus.Deleted;
                centre.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCentre.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, centre.Id));
            }
        }

        public Centre GetById(Guid id, bool includeDeactivated = false)
        {
            Centre entity = (Centre)_cacheProvider.Get(string.Format(_cacheKey, id));
            if (entity == null)
            {
                var tbl =
                    _ctx.tblCentre.FirstOrDefault(
                        s =>
                        s.Id == id &&
                        (includeDeactivated
                             ? s.IM_Status != (int) EntityStatus.Deleted
                             : s.IM_Status == (int) EntityStatus.Active));
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public override IEnumerable<Centre> GetAll(bool includeDeactivated = false)
        {
            IList<Centre> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<Centre>(ids.Count);
                foreach (Guid id in ids)
                {
                    Centre entity = GetById(id, includeDeactivated);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblCentre.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList();
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (Centre p in entities)
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

        public List<Centre> GetByHubId(Guid hubId, bool includeDeactivated = false)
        {
            //IQueryable<tblCentre> centretbl = _ctx.tblCentre.Where(n => n.HubId == hubId);
            //var centres = centretbl.Select(Map).ToList();
            var centres = GetAll(includeDeactivated).Where(n => n.Hub.Id == hubId).ToList();
            return centres;
        }

        public QueryResult Query(QueryStandard q)
        {

            IQueryable<tblCentre> centerQuery;
            if (q.ShowInactive)
                centerQuery = _ctx.tblCentre.Where(s => s.IM_Status!=(int)EntityStatus.Deleted).AsQueryable();
            else
                centerQuery = _ctx.tblCentre.Where(s => s.IM_Status == (int)EntityStatus.Active).AsQueryable();

            //if (q.CommodityOwnerId.HasValue)
            //{
            //    centerQuery = centerQuery.Where(n => n.Id == q.CommodityOwnerId.Value);
            //}
            var queryResult = new QueryResult();
            if (!string.IsNullOrWhiteSpace(q.Name))
            {
                centerQuery = centerQuery
                    .Where(s => s.Name.ToLower().Contains(q.Name.ToLower()));
            }

            queryResult.Count = centerQuery.Count();
            centerQuery = centerQuery.OrderBy(s => s.Name).ThenBy(s => s.Code);
            if (q.Skip.HasValue && q.Take.HasValue)
                centerQuery = centerQuery.Skip(q.Skip.Value).Take(q.Take.Value);
            var result = centerQuery.ToList();
            queryResult.Data = result.Select(Map).OfType<MasterEntity>().ToList();
            q.ShowInactive = false;
            return queryResult;
        }

        public Centre Map(tblCentre centretbl)
        {
            Centre centre = new Centre(centretbl.Id)
                                {
                                    CenterType = _centreTypeRepository.GetById(centretbl.CentreTypeId),
                                    Code = centretbl.Code,
                                    Description = centretbl.Description,
                                    Name = centretbl.Name,
                                };
            centre.Hub = _costCentreRepository.GetById(centretbl.HubId) as Hub;
            if (centretbl.RouteId != null) centre.Route = _routeRepository.GetById(centretbl.RouteId.Value);
            centre._SetDateCreated(centretbl.IM_DateCreated);
            centre._SetDateLastUpdated(centretbl.IM_DateLastUpdated);
            centre._SetStatus((EntityStatus)centretbl.IM_Status);

            return centre;
        }

        public ValidationResultInfo Validate(Centre itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            if (itemToValidate.Route.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Route"));
            bool hasDuplicateName = GetAll(true)
                .Where(s => s.Id != itemToValidate.Id)
                .Any(p => p.Name == itemToValidate.Name);
            if (hasDuplicateName)
                vri.Results.Add(new ValidationResult("Duplicate Name found"));
            bool hasDuplicateCode = GetAll(true)
               .Where(s => s.Id != itemToValidate.Id)
               .Any(p => p.Code == itemToValidate.Code);
            if (hasDuplicateCode)
                vri.Results.Add(new ValidationResult("Duplicate Code found"));
            if (itemToValidate.Hub == null)
                vri.Results.Add(new ValidationResult("A centre must have a valid hub."));
           
            return vri;
        }

        public bool IsAllocated(Guid centerId)
        {
            var isAllocated = false;
             isAllocated =_ctx.tblMasterDataAllocation.Any(p => p.EntityBId == centerId);

            return isAllocated;

        }



    }
}
