using System;
using System.Collections.Generic;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.HQ.Lib.ViewModels.Admin.SettingsViewModel;


namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.SettingsViewBuilder
{
    public interface ISettingsViewModelBuilder
    {
        IList<SettingsViewModel> GetAll(bool inactive = false);
        List<SettingsViewModel> Search(string srchParam, bool inactive = false);
        SettingsViewModel GetById(Guid Id);
        void Save(SettingsViewModel settingsViewModel);
       
        void SetInactive(Guid id);
        Dictionary<int, string> sk();
        void SetActive(Guid Id);
        void SetDeleted(Guid id);
        void SaveSetting(Guid id, SettingsKeys sk, string value);
        bool HasKey(SettingsKeys sk);
        SettingsViewModel GetIdFromKey(SettingsKeys sk);
    }
}
