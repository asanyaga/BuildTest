using System;
using System.Net;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.WPF.Lib.Services.Service.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using log4net;

namespace Distributr.WPF.Lib.Services.Service.WSProxies.Impl
{
    public class SetupApplication : ISetupApplication
    {
        private IConfigService _configService;
        private IOtherUtilities _otherUtilities;
        private ILog _log;

        public SetupApplication(IConfigService configService, IOtherUtilities otherUtilities)
        {
            _configService = configService;
            _otherUtilities = otherUtilities;
            _log = LogManager.GetLogger("SetupApplication");
        }


        public CostCentreLoginResponse LoginOnServer(string userName, string password, UserType userType)
        
        {
          var config = _configService.Load();
          string _url = config.WebServiceUrl + "api/Login/LoginGet?Username={0}&Password={1}&UserType={2}";

            string url = string.Format(_url, userName, _otherUtilities.MD5Hash(password), userType.ToString());
            _log.Info("Attempting logon on server --> " + url);
            Uri uri = new Uri(url, UriKind.Absolute);
            WebClient wc = new WebClient();

            
            string response = wc.DownloadString(uri);
            CostCentreLoginResponse _response = JsonConvert.DeserializeObject<CostCentreLoginResponse>(response, new IsoDateTimeConverter());
            if (_response.ErrorInfo != null && _response.ErrorInfo.Equals("Success"))
            {
                _log.InfoFormat("Remote login success. Saving CC {0} ", _response.CostCentreId);
                config.CostCentreId = _response.CostCentreId;
                _configService.Save(config);
            }

            return _response;
        }

        public CreateCostCentreApplicationResponse CreateCostCentreApplication(Guid costCentreId, string appDescription)
        {
            var config = _configService.Load();
            string _url = _configService.Load().WebServiceUrl + "CostCentreApplication/createcostcentreapplication?costcentreid={0}&applicationdescription={1}";
            string url = string.Format(_url, config.CostCentreId, appDescription);
            Uri uri = new Uri(url, UriKind.Absolute);
            WebClient wc = new WebClient();
         
            CreateCostCentreApplicationResponse r = null;
            try
            {
                string response = wc.DownloadString(uri);
                r = JsonConvert.DeserializeObject<CreateCostCentreApplicationResponse>(response, new IsoDateTimeConverter());
                if (r.CostCentreApplicationId != Guid.Empty)
                {
                    _configService.CleanLocalDB();
                    config.CostCentreApplicationId = r.CostCentreApplicationId;
                    config.DateInitialized = DateTime.Now;
                    config.CostCentreId = costCentreId;
                    config.IsApplicationInitialized = true;
                    config.ApplicationStatus = 1;
                    _configService.Save(config);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return r;

        }
    }
}
