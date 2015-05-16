using System;
using System.Configuration;
using Distributr.Core;
using Distributr.WPF.Lib.Impl.Model.Utility;
using Distributr.WPF.Lib.Impl.Repository.Utility;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.ViewModels;

namespace Distributr.WPF.Lib.Impl.Services.Utility
{
    public class ConfigService
    {
        IConfigRepository _configRepository;
        private ViewModelParameters _viewModelParameters;
        public ConfigService(IConfigRepository configRepsository)
        {
            _configRepository = configRepsository;
            _viewModelParameters = new ViewModelParameters();
        }

        public void Save(Config config)
        {
            var c = _configRepository.Get(config.AppId);
            if (c == null)
                c = new ConfigLocal();
            c.CostCenterId = config.CostCentreId;
            c.DateInitialized = config.DateInitialized;
            c.IsApplicationInitialized = config.IsApplicationInitialized;
            c.CostCentreApplicationDescription = config.CostCentreApplicationDescription;
            c.CostCentreApplicationId = config.CostCentreApplicationId;
            c.WebServiceUrl = config.WebServiceUrl;
            c.ApplicationStatus = config.ApplicationStatus;
            c.VirtualCityApp = (int)config.AppId;
            _configRepository.Save(c);
            
        }

        public void CleanLocalDB()
        {
            _viewModelParameters.CurrentUserId = Guid.Empty;
            _viewModelParameters.IsLogin = false;
            _configRepository.CleanLocalDB();
        }

        public ViewModelParameters ViewModelParameters
        {
            get { return _viewModelParameters; }
        }


        public string WCFEndpointConfigurationName
        {
            get { return "CustomBinding_DistributorServices"; }
        }
        public Guid GetClientAppId()
        {
            string appIdKey = "ClientApplicationId";
            Guid appId = Guid.Empty;
            Guid.TryParse(ConfigurationSettings.AppSettings[appIdKey], out appId);
            if (appId == Guid.Empty)
            {
                appId = Guid.NewGuid();
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings.Add(new KeyValueConfigurationElement(appIdKey, appId.ToString()));
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
            }
            return appId;

        }

    }
}
