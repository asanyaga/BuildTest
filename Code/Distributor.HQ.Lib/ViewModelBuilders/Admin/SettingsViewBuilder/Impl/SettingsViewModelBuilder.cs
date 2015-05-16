using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.HQ.Lib.Helper;
using Distributr.HQ.Lib.ViewModels.Admin.SettingsViewModel;
using Distributr.Core.Domain.Master;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.SettingsViewBuilder.Impl
{
    public class SettingsViewModelBuilder : ISettingsViewModelBuilder
    {
        ISettingsRepository _settingsRepository;
        public SettingsViewModelBuilder(ISettingsRepository settingsRepository)
       {
           _settingsRepository = settingsRepository;
       }
        public IList<SettingsViewModel> GetAll(bool inactive = false)
        {
            var setting = _settingsRepository.GetAll(inactive);
            return setting.Select(n => new SettingsViewModel
            {
                Key = n.Key,
                Value = n.Value,
                Id = n.Id,
                IsActive = n._Status == EntityStatus.Active ? true : false
            }).ToList();
        }

        public List<SettingsViewModel> Search(string srchParam, bool inactive = false)
        {
            throw new NotImplementedException();
        }

        public SettingsViewModel GetById(Guid Id)
        {
            AppSettings setting;
            setting = _settingsRepository.GetById(Id);
            if (setting == null) return null;
            return Map(setting);
        }

        public bool HasKey(SettingsKeys sk)
        {
            return GetAll(true).Any(n => n.Key == sk);
        }

        public void Save(SettingsViewModel settingsViewModel)
        {
            AppSettings setting = new AppSettings(settingsViewModel.Id)
                                   {
                                       Key = (SettingsKeys)settingsViewModel.SettingKey,
                                       Value = settingsViewModel.Value
                                   };
            _settingsRepository.Save(setting);
        }

        public void SaveSetting(Guid id, SettingsKeys sk, string value)
        {
            AppSettings setting = new AppSettings(id)
            {
                Key = sk,
                Value = value
            };
            _settingsRepository.Save(setting);
        }

        public void SetInactive(Guid id)
        {
            AppSettings appSettings = new AppSettings(id);
            _settingsRepository.SetInactive(appSettings);
        }

        public SettingsViewModel Map(AppSettings s)
        {
            var Keys = from Enum e in Enum.GetValues(typeof(SettingsKeys))
                       select new { Key = e, Value = e.ToString() };
                           
            var settingViewModel=  new SettingsViewModel
            {

                SettingKey = (int) s.Key,
                Value = s.Value,
                Id = s.Id,
                IsActive = s._Status == EntityStatus.Active ? true : false,
            };
            //settingViewModel.KeyList = new SelectList(Keys, "key", "Value");
            return settingViewModel;
        }

        public void SetActive(Guid id)
        {
            AppSettings appSettings = new AppSettings(id);
            _settingsRepository.SetActive(appSettings);
        }

        public void SetDeleted(Guid id)
        {
            AppSettings appSettings = new AppSettings(id);
            _settingsRepository.SetAsDeleted(appSettings);
        }

        public Dictionary<int, string> sk()
        {
            return EnumHelper.EnumToList<SettingsKeys>()
                            .ToDictionary(n => (int)n, n => n.ToString());
        }

        public SettingsViewModel GetIdFromKey(SettingsKeys sk)
        {
            return GetAll(true)
                .Where(n => n.Key == sk)
                .Select(n => 
                    new SettingsViewModel
                    {
                        Key = n.Key,
                        Value = n.Value,
                        Id = n.Id,
                        IsActive = n.IsActive
                    }).First();
        }
    }
}
