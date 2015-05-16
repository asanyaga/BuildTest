using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Repository.Master;
using Distributr.Core.Domain.Master;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility.Caching;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Data.Utility;
using log4net;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
    internal class AreaRepository : RepositoryMasterBase<Area>, IAreaRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        IRegionRepository _regionRepository;

        public AreaRepository(CokeDataContext ctx, ICacheProvider cacheProvider,IRegionRepository regionRepository)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _regionRepository = regionRepository;
            _log.Debug("Region Repository Constructor Bootstrap");
        }

        public Guid Save(Area entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                _log.Debug("Failed to save invalid Area");
                throw new DomainValidationException(vri, "Failed to save invalid Area");
            }
            tblArea area =  _ctx.tblArea.FirstOrDefault(n => n.id == entity.Id);
            DateTime date = DateTime.Now;
            if (area == null)
            {
                area = new tblArea
                           {
                               IM_DateCreated = date,
                               id = entity.Id,
                    IM_Status =(int) EntityStatus.Active
                };
                _ctx.tblArea.AddObject(area);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (area.IM_Status != (int)entityStatus)
                area.IM_Status = (int)entity._Status;

            area.Name = entity.Name;
            area.Description = entity.Description;
            area.Region = entity.region.Id;

            area.IM_DateLastUpdated = date;

            _log.Debug("Saving Area");
            _ctx.SaveChanges();
            _log.Debug("Invalidating cache");
            _cacheProvider.Put(_cacheListKey, _ctx.tblArea.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, area.id));
            _log.DebugFormat("Successfully saved item id:{0}", area.id);
            return area.id;
        }

        public void SetInactive(Area entity)
        {
            _log.Debug("Inactivating area");
            //dependency Exists on Distributor and asm
            bool dependenciesPresent = false; 

            string failureReason = "";
            if (dependenciesPresent)
            {
                failureReason = "DEPENDENCIES FOUND:";//populate with ids
                throw new ArgumentException(failureReason);
            }
            else
            {
                tblArea area = _ctx.tblArea.FirstOrDefault(n => n.id == entity.Id);
                if (area == null || area.IM_Status!=(int)EntityStatus.Active)
                {//not existing or already deactivated.
                    return;
                }
                area.IM_Status = (int)EntityStatus.Inactive;
                area.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblArea.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, area.id));
            }

        }

        public void SetActive(Area entity)
        {
            tblArea area = _ctx.tblArea.FirstOrDefault(n => n.id == entity.Id);
            if (area != null)
            {
                area.IM_Status = (int) EntityStatus.Active;
                area.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblArea.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, area.id));
            }

        }

        public void SetAsDeleted(Area entity)
        {
            _log.Debug("Deleting area");
            //dependency Exists on Distributor and asm
            bool dependenciesPresent = false;

            string failureReason = "";
            if (dependenciesPresent)
            {
                failureReason = "DEPENDENCIES FOUND:";//populate with ids
                throw new ArgumentException(failureReason);
            }
            else
            {
                tblArea area = _ctx.tblArea.FirstOrDefault(n => n.id == entity.Id);
                if (area == null || area.IM_Status != (int)EntityStatus.Active)
                {//not existing or already deactivated.
                    return;
                }
                area.IM_Status = (int)EntityStatus.Deleted;
                area.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblArea.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, area.id));
            }

        }

        public Area GetById(Guid Id, bool includeDeactivated = false)
        {
            _log.DebugFormat("Getting Area by ID: {0}", Id);

            Area entity = (Area)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblArea.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public ValidationResultInfo Validate(Area itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            bool hasDuplicate = GetAll(true).Where(s => s.Id != itemToValidate.Id).Any(n => n.Name == itemToValidate.Name);
            if (hasDuplicate)
            {
                vri.Results.Add(new ValidationResult("Duplicate Name found"));
            }
            return vri;
        }


        protected override string _cacheKey
        {
            get { return "Area-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "AreaList"; }
        }

        public override IEnumerable<Area> GetAll(bool includeDeactivated = false)
        {
            _log.Debug("Get all Areas");
            IList<Area> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<Area>(ids.Count);
                foreach (Guid id in ids)
                {
                    Area entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblArea.Where(n=>n.IM_Status!=(int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (Area p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                //entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList().OfType<Area>().Select(Map).ToList();
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }
        public  Area Map(tblArea area)
        {
            Area retArea = new Area(area.id)
            {
                //_Status = region.Active.Value,
                Name = area.Name,
                Description = area.Description,
                region=_regionRepository.GetById(area.Region)
            };
            retArea._SetDateCreated(area.IM_DateCreated);
            retArea._SetDateLastUpdated(area.IM_DateLastUpdated);
            retArea._SetStatus((EntityStatus)area.IM_Status);

            return retArea;
        }
    }
}
