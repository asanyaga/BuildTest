using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.HQ.Lib.Helper;
using Distributr.HQ.Lib.ViewModels.Admin.SettingsViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.SettingsViewBuilder.Impl
{
    public class RetireSettingViewModelBuilder : IRetireSettingViewModelBuilder
    {
        private IRetireDocumentSettingRepository _retireDocumentSettingRepository;

        public RetireSettingViewModelBuilder(IRetireDocumentSettingRepository retireDocumentSettingRepository)
        {
            _retireDocumentSettingRepository = retireDocumentSettingRepository;
        }

        public RetireSettingViewModel GetSettings()
        {
            var setting = _retireDocumentSettingRepository.GetSettings();
            return Map(setting);
        }

        private RetireSettingViewModel Map(RetireDocumentSetting setting)
        {
            if (setting == null) return null;
            else
                return new RetireSettingViewModel
                           {
                               Duration = setting.Duration,
                               Id = setting.Id,
                               IsActive = setting._Status == EntityStatus.Active ? true : false,
                               RetireTypeId = (int) setting.RetireType,
                               RetireTypeName=setting.RetireType.ToString(),
                           };
        }

        public void Save(RetireSettingViewModel settings)
        {
            RetireDocumentSetting item = new RetireDocumentSetting(settings.Id)
                                             {
                                                 RetireType = (RetireType) settings.RetireTypeId,
                                                 Duration = settings.Duration,
                                             };
            _retireDocumentSettingRepository.Save(item);
        }
        public List<SelectListItem> RetireType()
        {
            return EnumHelper.EnumToList<RetireType>().Select(s=> new SelectListItem{Text=s.ToString(),Value=((int)s).ToString()}).ToList();
        }

        public void SetInactive(Guid id)
        {
            RetireDocumentSetting retireDocumentSetting = new RetireDocumentSetting(id);
            _retireDocumentSettingRepository.SetInactive(retireDocumentSetting);
        }

        public void SetDeleted(Guid id)
        {
            RetireDocumentSetting retireDocumentSetting = new RetireDocumentSetting(id);
            _retireDocumentSettingRepository.SetAsDeleted(retireDocumentSetting);
        }

        public void SetActive(Guid id)
        {
            RetireDocumentSetting retireDocumentSetting = new RetireDocumentSetting(id);
            _retireDocumentSettingRepository.SetActive(retireDocumentSetting);
        }
    }
}
