using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.SettingsRepositories
{
    internal class RetireDocumentSettingRepository : RepositoryMasterBase<RetireDocumentSetting>, IRetireDocumentSettingRepository
    {
         CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        public RetireDocumentSettingRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
            _ctx = ctx;
        }
        protected override string _cacheKey
        {
            get { return "RetireDocumentSetting-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "RetireDocumentSettingList"; }
        }

        public Guid Save(RetireDocumentSetting entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            } 
           
            if (!vri.IsValid)
            {
                _log.Debug("RetireDocumentSetting not valid");
                throw new DomainValidationException(vri, "RetireDocumentSetting Entity Not valid");
            }
            DateTime dt = DateTime.Now;
            //tblSettings settings = _ctx.tblSettings.FirstOrDefault(n => n.Id == entity.Id);
            tblRetireDocumentSetting settings = _ctx.tblRetireDocumentSetting.FirstOrDefault();
            if (settings == null)
            {
                settings = new tblRetireDocumentSetting();
                settings.IM_Status = (int)EntityStatus.Active;//true;
                settings.IM_DateCreated = dt;
                settings.Id = entity.Id;
                _ctx.tblRetireDocumentSetting.AddObject(settings);

            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (settings.IM_Status != (int)entityStatus)
                settings.IM_Status = (int)entity._Status;
            settings.RetireTypeId = (int)entity.RetireType;
            settings.Duration = entity.Duration;
            settings.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblRetireDocumentSetting.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, settings.Id));
            return settings.Id; 
        }

        public void SetInactive(RetireDocumentSetting entity)
        {
            _log.Debug("Deactivating ans RetireDocumentSetting ID: " + entity.Id);
            ValidationResultInfo vri = Validate(entity);
            tblRetireDocumentSetting retireDocumentSetting = _ctx.tblRetireDocumentSetting.FirstOrDefault(n => n.Id == entity.Id);
            if (retireDocumentSetting != null)
            {
                retireDocumentSetting.IM_Status = (int)EntityStatus.Inactive;
                retireDocumentSetting.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblRetireDocumentSetting.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, retireDocumentSetting.Id));

            }
        }

        public void SetActive(RetireDocumentSetting entity)
        {
            _log.Debug("Activating an RetireDocumentSetting ID: " + entity.Id);
            ValidationResultInfo vri = Validate(entity);
            tblRetireDocumentSetting retireDocumentSetting = _ctx.tblRetireDocumentSetting.FirstOrDefault(n => n.Id == entity.Id);
            if (retireDocumentSetting != null)
            {
                retireDocumentSetting.IM_Status = (int)EntityStatus.Active;
                retireDocumentSetting.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblRetireDocumentSetting.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, retireDocumentSetting.Id));

            }
        }

        public void SetAsDeleted(RetireDocumentSetting entity)
        {
            _log.Debug("Deleting ans RetireDocumentSetting ID: " + entity.Id);
            ValidationResultInfo vri = Validate(entity);
            tblRetireDocumentSetting retireDocumentSetting = _ctx.tblRetireDocumentSetting.FirstOrDefault(n => n.Id == entity.Id);
            if (retireDocumentSetting != null)
            {
                retireDocumentSetting.IM_Status = (int)EntityStatus.Deleted;
                retireDocumentSetting.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblRetireDocumentSetting.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, retireDocumentSetting.Id));

            }
        }

        public RetireDocumentSetting GetById(Guid id, bool includeDeactivated = false)
        {
            RetireDocumentSetting entity = (RetireDocumentSetting)_cacheProvider.Get(string.Format(_cacheKey, id));
            if (entity == null)
            {
                var tbl = _ctx.tblRetireDocumentSetting.FirstOrDefault(s => s.Id == id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }
            }
            return entity; 
        }

        private RetireDocumentSetting Map(tblRetireDocumentSetting settings)
        {
            RetireDocumentSetting setting = new RetireDocumentSetting(settings.Id)
            {
                RetireType = (RetireType)settings.RetireTypeId,
                Duration = settings.Duration,
            };
            setting._SetDateCreated(settings.IM_DateCreated);
            setting._SetDateLastUpdated(settings.IM_DateLastUpdated);
            setting._SetStatus((EntityStatus)settings.IM_Status);
            return setting;
        }

        public override IEnumerable<RetireDocumentSetting> GetAll(bool includeDeactivated =false)
        {
            IList<RetireDocumentSetting> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<RetireDocumentSetting>(ids.Count);
                foreach (Guid id in ids)
                {
                    RetireDocumentSetting entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblRetireDocumentSetting.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList();
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (RetireDocumentSetting p in entities)
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

        public RetireDocumentSetting GetSettings()
        {
            return GetAll().FirstOrDefault();
        }

        public ValidationResultInfo Validate(RetireDocumentSetting itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();

            return vri;
        }
    }
}
