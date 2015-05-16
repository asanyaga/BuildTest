using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master;
using Distributr.Core.Domain.Master.ChannelPackagings;
using Distributr.Core.Repository.Master.ChannelPackagings;
using Distributr.Core.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.ChannelPackagings
{
   internal class ChannelPackagingRepository:RepositoryMasterBase<ChannelPackaging>,IChannelPackagingRepository
    {
       ICacheProvider _cacheProvider;
       CokeDataContext _ctx;
       IOutletTypeRepository _outletTypeRepository;
       IProductPackagingRepository  _productPackagingRepository;
       public ChannelPackagingRepository(ICacheProvider cacheProvider,CokeDataContext ctx,IProductPackagingRepository productPackagingRepository,IOutletTypeRepository outletTypeRepository)
       {
           _ctx = ctx;
           _cacheProvider = cacheProvider;
           _productPackagingRepository=productPackagingRepository;
           _outletTypeRepository=outletTypeRepository;
       }


       protected override string _cacheKey
       {
           get { return "ChannelPackaging-{0}"; }
       }

       protected override string _cacheListKey
       {
           get { return "ChannelPackagingList"; }
       }

       public override IEnumerable<ChannelPackaging> GetAll(bool includeDeactivated = false)
        {
            IList<ChannelPackaging> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<ChannelPackaging>(ids.Count);
                foreach (Guid id in ids)
                {
                    ChannelPackaging entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblChannelPackaging.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (ChannelPackaging p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

       public IEnumerable<ChannelPackaging> Query(QueryStandard q)
       {
           IQueryable<tblChannelPackaging> cpQuery;
           if (q.ShowInactive)
               cpQuery = _ctx.tblChannelPackaging.Where(k => k.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
           else
               cpQuery = _ctx.tblChannelPackaging.Where(h => h.IM_Status == (int) EntityStatus.Active).AsQueryable();

           if (!string.IsNullOrEmpty(q.Name))
               cpQuery = cpQuery.Where(h => h.tblProductPackaging.Name.ToLower().Contains(q.Name.ToLower()));

          cpQuery = cpQuery.OrderBy(j => j.tblProductPackaging.Name);

           if (q.Skip.HasValue && q.Take.HasValue)
               cpQuery = cpQuery.Skip(q.Skip.Value).Take(q.Take.Value);

           var queryResult = cpQuery.Select(k=>Map(k)).ToList();

           q.ShowInactive = false;
           return queryResult;
       }

       public Guid Save(ChannelPackaging entity, bool? isSync = null)
        {
            _log.DebugFormat("Saving/Updating");
            ValidationResultInfo vri=Validate(entity);
            if (!vri.IsValid)
            { 
            throw new DomainValidationException(vri,"Failed to validate channel packaging");
            }
            DateTime dt = DateTime.Now;
            tblChannelPackaging tblCPacks =  _ctx.tblChannelPackaging.FirstOrDefault(n => n.id == entity.Id);
            if (tblCPacks == null)
            {
                tblCPacks = new tblChannelPackaging();
                tblCPacks.id = entity.Id;
                tblCPacks.IM_Status = (int)EntityStatus.Active;// true;
                tblCPacks.IM_DateCreated = dt;
                _ctx.tblChannelPackaging.AddObject(tblCPacks);
            }

            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (tblCPacks.IM_Status != (int)entityStatus)
                tblCPacks.IM_Status = (int)entity._Status;
            tblCPacks.PackagingId = entity.Packaging.Id;
            tblCPacks.OutletTypeId = entity.OutletType.Id;
            tblCPacks.IsChecked = entity.IsChecked;
            tblCPacks.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblChannelPackaging.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, tblCPacks.id));
            return tblCPacks.id;
        }

        public void SetInactive(ChannelPackaging entity)
        {
            _log.Debug("InActivating Channel Packaging");
            tblChannelPackaging cPackaging = _ctx.tblChannelPackaging.First(n => n.id == entity.Id);
            if (cPackaging != null)
            {
                cPackaging.IM_Status = (int)EntityStatus.Inactive;// false;
                cPackaging.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblChannelPackaging.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, cPackaging.id));
            }
            
        }

       public void SetActive(ChannelPackaging entity)
       {
           _log.Debug("Activating Channel Packaging");
           tblChannelPackaging cPackaging = _ctx.tblChannelPackaging.First(n => n.id == entity.Id);
           if (cPackaging != null)
           {
               cPackaging.IM_Status = (int)EntityStatus.Active;
               cPackaging.IM_DateLastUpdated = DateTime.Now;
               _ctx.SaveChanges();
               _cacheProvider.Put(_cacheListKey, _ctx.tblChannelPackaging.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
               _cacheProvider.Remove(string.Format(_cacheKey, cPackaging.id));
           }
            
       }

       public void SetAsDeleted(ChannelPackaging entity)
       {
           _log.Debug("Deleting Channel Packaging");
           tblChannelPackaging cPackaging = _ctx.tblChannelPackaging.First(n => n.id == entity.Id);
           if (cPackaging != null)
           {
               cPackaging.IM_Status = (int)EntityStatus.Deleted;
               cPackaging.IM_DateLastUpdated = DateTime.Now;
               _ctx.SaveChanges();
               _cacheProvider.Put(_cacheListKey, _ctx.tblChannelPackaging.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
               _cacheProvider.Remove(string.Format(_cacheKey, cPackaging.id));
           }
       }

       protected ChannelPackaging Map(tblChannelPackaging channelPacks)
        {
            ChannelPackaging cPacks = new ChannelPackaging(channelPacks.id) 
            {
             OutletType=_outletTypeRepository.GetById(channelPacks.OutletTypeId),
              Packaging=_productPackagingRepository.GetById(channelPacks.PackagingId),
              IsChecked=channelPacks.IsChecked
            };
            cPacks._SetStatus((EntityStatus)channelPacks.IM_Status);
            cPacks._SetDateCreated(channelPacks.IM_DateCreated);
            cPacks._SetDateLastUpdated(channelPacks.IM_DateLastUpdated);
            return cPacks;
        }
        public ChannelPackaging GetById(Guid Id, bool includeDeactivated = false)
        {
            ChannelPackaging entity = (ChannelPackaging)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblChannelPackaging.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
       }

        public ValidationResultInfo Validate(ChannelPackaging itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            return vri;
        }
      
    }
}
