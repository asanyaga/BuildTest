using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Distributr.HQ.Lib.ViewModels.Admin.SettingsViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.SettingsViewBuilder
{
   public interface IRetireSettingViewModelBuilder
   {
       List<SelectListItem> RetireType();
       RetireSettingViewModel GetSettings();
       void Save(RetireSettingViewModel settings);
       void SetActive(Guid Id);
       void SetInactive(Guid Id);
       void SetDeleted(Guid id);
    }
}
