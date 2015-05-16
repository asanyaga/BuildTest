using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.UI_Utillity.Converters;

namespace Distributr.WPF.Lib.Data.Setup
{
    /// <summary>
    /// Methods used to setup hub
    /// </summary>
    public class SetupHub
    {
        /// <summary>
        /// Double dispatch
        /// </summary>
        /// <param name="settingsRepository"></param>
        /// <param name="configService"></param>
        public static ClientApplication AppIdSetup(ISettingsRepository settingsRepository,IConfigService configService)
        {
            var allowDecimal = settingsRepository.GetByKey(SettingsKeys.AllowDecimal);
            SetQuantityFormat(allowDecimal);
            Guid appId = configService.GetClientAppId();
            string hostname = Dns.GetHostName();

            ClientApplication clientApplication = configService.GetClientApplications().FirstOrDefault(s => s.Id == appId);
            if (clientApplication == null)
            {
                clientApplication = new ClientApplication();
                clientApplication.CanSync = false;
                clientApplication.DateInitialized = DateTime.Now;
            }
            clientApplication.HostName = hostname;
            clientApplication.Id = appId;
            configService.SaveClientApplication(clientApplication);
            return clientApplication;
        }
        private static void SetQuantityFormat(AppSettings allowDecimal)
        {
            bool allow = false;

            if (allowDecimal != null && bool.TryParse(allowDecimal.Value, out allow) && allow)
            {
                StringFormats.QuantityRegEx = @"^(?=.*[0-9])(\d{0,10})?(?:\.\d{0,2})?$";
            }
            else
            {
                StringFormats.QuantityRegEx = @"^(?=.*[0-9])(\d{0,10})$";
            }
        }
    }
}
