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
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.MasterDataAllocationRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.MasterDataAllocationRepositories
{
    internal class MasterDataAllocationRepository : RepositoryMasterBase<MasterDataAllocation>, IMasterDataAllocationRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        public MasterDataAllocationRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
            _ctx = ctx;
        }

        protected override string _cacheKey
        {
            get { return "MasterDataAllocation-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "MasterDataAllocationList"; }
        }

        public Guid Save(MasterDataAllocation entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            } 
           
            if (!vri.IsValid)
            {
                _log.Debug("MasterDataAllocation validation error.");
                throw new DomainValidationException(vri, CoreResourceHelper.GetText("MasterDataAllocation.validation.error"));
            }

            tblMasterDataAllocation allocation =
                _ctx.tblMasterDataAllocation.FirstOrDefault(
                    n =>
                    n.EntityAId == entity.EntityAId && n.EntityBId == entity.EntityBId &&
                    n.IM_Status == (int)EntityStatus.Active);
            DateTime now = DateTime.Now;
            if (allocation == null)
            {
                allocation = new tblMasterDataAllocation();
                allocation.Id = entity.Id;
                allocation.IM_DateCreated = now;
                allocation.IM_Status = (int)EntityStatus.Active;
                _ctx.tblMasterDataAllocation.AddObject(allocation);
            }

            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (allocation.IM_Status != (int)entityStatus)
                allocation.IM_Status = (int)entity._Status;

            allocation.IM_DateLastUpdated = now;
            allocation.EntityAId = entity.EntityAId;
            allocation.EntityBId = entity.EntityBId;
            allocation.AllocationType = (int)entity.AllocationType;


            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblMasterDataAllocation.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, allocation.Id));

            return allocation.Id;
        }

        public void SetInactive(MasterDataAllocation entity)
        {
            throw new Exception("Should only create and delete master data allocations.");
            _log.Debug("Deactivating MasterDataAllocation ID: " + entity.Id);

            tblMasterDataAllocation allocation = _ctx.tblMasterDataAllocation.FirstOrDefault(n => n.Id == entity.Id);
            if (allocation != null)
            {
                allocation.IM_Status = (int)EntityStatus.Inactive;
                allocation.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblMasterDataAllocation.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, allocation.Id));

            }
        }

        public void SetActive(MasterDataAllocation entity)
        {
            throw new Exception("Should only create and delete master data allocations.");
            _log.Debug("Activating MasterDataAllocation id : " + entity.Id);
            ValidationResultInfo vri = Validate(entity);

            if (!vri.IsValid)
            {
                _log.Debug("MasterDataAllocation validation error.");
                throw new DomainValidationException(vri, CoreResourceHelper.GetText("MasterDataAllocation.validation.error"));
            }
            tblMasterDataAllocation allocation = _ctx.tblMasterDataAllocation.FirstOrDefault(n => n.Id == entity.Id);
            if(allocation != null)
            {
                allocation.IM_Status = (int) EntityStatus.Active;
                allocation.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey,
                                   _ctx.tblMasterDataAllocation.Where(n => n.IM_Status != (int) EntityStatus.Deleted).
                                       Select(n => n.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, allocation.Id));
            }
            
        }

        public void SetAsDeleted(MasterDataAllocation entity)
        {
            throw new Exception("Should only create and delete master data allocations.");
            _log.Debug("Deleting MasterDataAllocation ID: " + entity.Id);
            tblMasterDataAllocation allocation = _ctx.tblMasterDataAllocation.FirstOrDefault(n => n.Id == entity.Id);
            if (allocation != null)
            {
                allocation.IM_Status = (int)EntityStatus.Deleted;
                allocation.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblMasterDataAllocation.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, allocation.Id));

            }
        }

        public MasterDataAllocation GetById(Guid Id, bool includeDeactivated = false)
        {
            MasterDataAllocation entity = (MasterDataAllocation)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblMasterDataAllocation.FirstOrDefault(s => s.Id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }
            }
            return entity; 
        }

        public override IEnumerable<MasterDataAllocation> GetAll(bool includeDeactivated = false)
        {
            IList<MasterDataAllocation> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<MasterDataAllocation>(ids.Count);
                foreach (Guid id in ids)
                {
                    MasterDataAllocation entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblMasterDataAllocation.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(Map).ToList();
                if (entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); 
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (MasterDataAllocation p in entities)
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

        public List<MasterDataAllocation> GetByAllocationType(MasterDataAllocationType allocationType, Guid entityAId = new Guid(), Guid entityBId = new Guid(), bool includeDeactivated = false)
        {
            IList<MasterDataAllocation> entities = GetAll(includeDeactivated)
                .Where(n => n.AllocationType == allocationType
                        && n._Status != EntityStatus.Deleted
                        && ((entityAId != Guid.Empty ? n.EntityAId == entityAId : n.EntityAId != Guid.Empty)
                        && (entityBId != Guid.Empty ? n.EntityBId == entityBId : n.EntityBId != Guid.Empty)
                        )
                        ).ToList();
            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities.ToList(); 
        }

        public bool DeleteAllocation(Guid allocationId)
        {
            _log.Debug("Deleting MasterDataAllocation ID: " + allocationId);
            tblMasterDataAllocation allocation = _ctx.tblMasterDataAllocation.FirstOrDefault(n => n.Id == allocationId);
            if(allocation != null)
            {
                _ctx.tblMasterDataAllocation.DeleteObject(allocation);
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblMasterDataAllocation.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, allocation.Id));
                return true;
            }
            return false;
        }

        public ValidationResultInfo Validate(MasterDataAllocation itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            return vri;
        }

        public MasterDataAllocation Map(tblMasterDataAllocation tblAlloc)
        {
            MasterDataAllocation allocation = new MasterDataAllocation(tblAlloc.Id)
                                                  {
                                                      EntityAId = tblAlloc.EntityAId,
                                                      EntityBId = tblAlloc.EntityBId,
                                                      AllocationType = (MasterDataAllocationType)tblAlloc.AllocationType
                                                  };
            allocation._SetDateCreated(tblAlloc.IM_DateCreated);
            allocation._SetDateLastUpdated(tblAlloc.IM_DateLastUpdated);
            allocation._SetStatus((EntityStatus)tblAlloc.IM_Status);
            return allocation;
        }
    }
}
