using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.SettingsRepositories
{
    internal class SettingsRepository : RepositoryMasterBase<AppSettings>, ISettingsRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        public SettingsRepository(CokeDataContext ctx,ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
            _ctx = ctx;
        }
        public Guid Save(AppSettings entity, bool? isSync = null)
        {
            if (entity.Key == SettingsKeys.EnforceTransactionalWeightLimit)
            {
                ValidationResultInfo vri = Validate(entity);

                if (!vri.IsValid)
                {
                    _log.Debug("Settings not valid");
                    throw new DomainValidationException(vri, "Settings Entity Not valid");
                }
            }
            DateTime dt = DateTime.Now;
            //tblSettings settings = _ctx.tblSettings.FirstOrDefault(n => n.Id == entity.Id);
            tblSettings settings = _ctx.tblSettings.FirstOrDefault(n => n.Key == (int)entity.Key && n.IM_Status == (int)EntityStatus.Active);
            if (settings == null)
            {
                settings = new tblSettings();
                settings.IM_Status = (int)EntityStatus.Active;// true;
                settings.IM_DateCreated = dt;
                settings.Id = entity.Id;
                _ctx.tblSettings.AddObject(settings);
            }
            
          
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (settings.IM_Status != (int)entityStatus)
                settings.IM_Status = (int)entity._Status;
            settings.Key = (int) entity.Key;
            settings.Value = entity.Value;
            settings.IM_DateLastUpdated = dt;
            if(!string.IsNullOrEmpty(entity.VirtualCityAppName))
              settings.AppId=(int)((VirtualCityApp)Enum.Parse(typeof(VirtualCityApp), entity.VirtualCityAppName));
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblSettings.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, settings.Id));
            return settings.Id; 
        }
        
        public void SetInactive(AppSettings entity)
        {
            _log.Debug("Deactivating an Settings ID: " + entity.Id);
            ValidationResultInfo vri = Validate(entity);
            tblSettings settings = _ctx.tblSettings.FirstOrDefault(n => n.Id == entity.Id);
            if (settings != null)
            {
                settings.IM_Status = (int)EntityStatus.Inactive;
                settings.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblSettings.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, settings.Id));

            }
        }

        public void SetActive(AppSettings entity)
        {
            _log.Debug("Activating an Settings ID: " + entity.Id);
            ValidationResultInfo vri = Validate(entity);
            tblSettings settings = _ctx.tblSettings.FirstOrDefault(n => n.Id == entity.Id);
            if (settings != null)
            {
                settings.IM_Status = (int)EntityStatus.Active;
                settings.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblSettings.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, settings.Id));

            }
        }

        public void SetAsDeleted(AppSettings entity)
        {
            _log.Debug("Deleting an Settings ID: " + entity.Id);
            ValidationResultInfo vri = Validate(entity);
            tblSettings settings = _ctx.tblSettings.FirstOrDefault(n => n.Id == entity.Id);
            if (settings != null)
            {
                settings.IM_Status = (int)EntityStatus.Deleted;
                settings.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblSettings.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, settings.Id));

            }
        }

        public AppSettings GetById(Guid id, bool includeDeactivated = false)
        {
            AppSettings entity = (AppSettings)_cacheProvider.Get(string.Format(_cacheKey, id));
            if (entity == null)
            {
                var tbl = _ctx.tblSettings.FirstOrDefault(s => s.Id == id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }
            }
            return entity; 
        }

        public ValidationResultInfo Validate(AppSettings itemToValidate)
        {
            //cn: no validation
            ValidationResultInfo vri = itemToValidate.BasicValidation();

            if (itemToValidate.Key == SettingsKeys.NumberOfDecimalPlaces)
            {
                if (int.Parse(itemToValidate.Value) < 1)
                {
                    vri.Results.Add(new ValidationResult("Number Of DecimalPlaces cannot be less than One"));
                }
            }

            if (itemToValidate.Key == SettingsKeys.EnforceTransactionalWeightLimit)
            {

                var setting = this.GetByKey(SettingsKeys.EnforceTransactionalWeightLimit);
                var val = itemToValidate.Value;
                if (val.Trim().StartsWith("True"))
                {
                    StringCollection information = new StringCollection();
                    foreach (Match match in Regex.Matches(val, @"\(([^)]*)\)"))
                    {
                        information.Add(match.Value);
                    }
                    var minValue = information[0].Trim();
                    minValue = minValue.Replace("(", string.Empty).Replace(")", string.Empty);
                    string maxValue = information[1].Trim();
                    maxValue = maxValue.Replace("(", string.Empty).Replace(")", string.Empty);
                    double mn = double.Parse(minValue);
                    double mx = double.Parse(maxValue);
                    if (mn > mx)
                    {
                        vri.Results.Add(new ValidationResult("Error! Min Weight is Greater than Max Weight"));
                    }
                }
                
               
            }
            return vri;
        }

        protected override string _cacheKey
        {
            get { return "Settings-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "SettingsList"; }
        }

        public override IEnumerable<AppSettings> GetAll(bool includeDeactivated = false)
        {
            IList<AppSettings> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<AppSettings>(ids.Count);
                foreach (Guid id in ids)
                {
                    AppSettings entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblSettings.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); 
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (AppSettings p in entities)
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

        public AppSettings GetByKey(SettingsKeys key)
        {
            return GetAll().FirstOrDefault(s => s.Key == key);
        }

        public AppSettings Map(tblSettings settings)
        {
            AppSettings setting = new AppSettings(settings.Id)
                                   {
                                       Key = (SettingsKeys) settings.Key,
                                       Value = settings.Value,
                                       VirtualCityAppName = settings.AppId.HasValue ? Enum.GetName(typeof(VirtualCityApp),settings.AppId.Value) :"" 
                                   };
            setting._SetDateCreated(settings.IM_DateCreated);
            setting._SetDateLastUpdated(settings.IM_DateLastUpdated);
            setting._SetStatus((EntityStatus)settings.IM_Status);
            return setting;
        }

        private bool AgrimanagrKeyExists(AppSettings settings)
        {
            return
                GetAll(true).Any(p => p.Key.Equals(settings.Key) && p.VirtualCityAppName == settings.VirtualCityAppName);
        }
    }
}
