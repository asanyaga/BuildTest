using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.EquipmentViewModel;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.SettingsViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.SettingsViewModelBuilder
{
   public interface IAgrimanagrSettingsViewModelBuilder
   {
       AgrimanagrSettingsViewModel GetSettings();
       void Save(AgrimanagrSettingsViewModel model);
       void SetInactive(SettingsKeys key);
       void SetActive(SettingsKeys key);
       void SetAsDeleted(SettingsKeys key);
       void SaveReportSettings(AgrimanagrReportSettingViewModel setting);
       void SaveDocumentReferenceSettings(SettingsDocReferenceViewModel setting);
       void SaveNotificationSettings(NotificationSettingsBaseViewModel setting);
       void SaveCallProtocalSettings(CallProtocalSettingsViewModel setting);
        void SaveMapUriSettings(MapUriSettingsViewModel  setting);
       
      
       

       

   }
}
