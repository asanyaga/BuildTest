using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core;
using Distributr.WPF.Lib.Impl.Model.Utility;
using Distributr.WPF.Lib.Impl.Repository.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;

namespace Distributr.WPF.Lib.Impl.Services.Utility
{
    public class AgrimanagrConfigService : ConfigService , IConfigService
    {
        IConfigRepository _configRepository;
        public AgrimanagrConfigService(IConfigRepository configRepsository)
            : base(configRepsository)
        {
            _configRepository = configRepsository;
        }

        public Config Load()
        {
            var c = _configRepository.Get(VirtualCityApp.Agrimanagr);
            if (c == null)
            {

                Config c1 = new Config()
                {
                    CostCentreId = Guid.Empty,
                    CostCentreApplicationId = Guid.Empty,
                    CostCentreApplicationDescription = "",
                    DateInitialized = DateTime.Now,//DateTime.MinValue,
                    IsApplicationInitialized = false,
                    WebServiceUrl = "",
                    ApplicationStatus = 0,
                    AppId = VirtualCityApp.Agrimanagr
                };
                Save(c1);
                c = _configRepository.Get(VirtualCityApp.Agrimanagr);

            }
            return new Config
            {
                CostCentreId = c.CostCenterId,
                DateInitialized = c.DateInitialized,
                IsApplicationInitialized = c.IsApplicationInitialized,
                CostCentreApplicationDescription = c.CostCentreApplicationDescription,
                CostCentreApplicationId = c.CostCentreApplicationId,
                WebServiceUrl = c.WebServiceUrl,
                ApplicationStatus = c.ApplicationStatus,
                AppId = (VirtualCityApp)c.VirtualCityApp,

            };
        }


        public string WCFRemoteAddress
        {
            get
            {
                string remoteAddressFormat = "{0}DistributorServices.svc";
                return string.Format(remoteAddressFormat, this.Load().WebServiceUrl.Trim());
            }
        }

        public List<ClientApplication> GetClientApplications()
        {
            return _configRepository.GetClientApplication(VirtualCityApp.Agrimanagr).Select(s => new ClientApplication
            {
                AppId = (VirtualCityApp)s.AppTypeId,
                CanSync = s.CanSync,
                DateInitialized = s.DateInitialized,
                HostName = s.HostName,
                Id = s.ClientAppId
            }).ToList();
        }

        public void SaveClientApplication(ClientApplication application)
        {
            ClientApplicationLocal applicationLocal = new ClientApplicationLocal
            {
                AppTypeId = (int)VirtualCityApp.Agrimanagr,
                CanSync = application.CanSync,
                DateInitialized = application.DateInitialized,
                HostName = application.HostName,
                ClientAppId = application.Id
            };
            _configRepository.SaveClientApplication(applicationLocal);
         
        }

        //public bool CanSync()
        //{
        //    Guid clientAppId = GetClientAppId();
        //    bool exist = GetClientApplications().Any(c => c.CanSync);
        //    if (!exist)
        //    {
        //        ClientApplication application = GetClientApplications().FirstOrDefault(s => s.Id == clientAppId);
        //        if (application == null)
        //            return false;
        //        application.CanSync = true;
        //        SaveClientApplication(application);
        //        return true;
        //    }
        //    else
        //    {
        //        return GetClientApplications().Any(c => c.CanSync && c.Id == clientAppId);
        //    }
        //}
    }
}
