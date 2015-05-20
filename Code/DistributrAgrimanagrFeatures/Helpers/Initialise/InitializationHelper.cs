using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Data.Setup;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.Services.Service.Sync;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using Newtonsoft.Json;

namespace DistributrAgrimanagrFeatures.Helpers.Initialise
{
    /// <summary>
    /// Do all initial hub initialization work and validate that setup 
    /// was successful
    /// </summary>
    public class InitializationHelper
    {
        private IConfigService _configService;
        private ISetupApplication _setupApplication;
        private ISettingsRepository _settingsRepository;
        private ISyncService _syncService;
        private IWebApiProxy _webApiProxy;
        private string section = "InitializationHelper";
        public InitializationHelper(IConfigService configService, ISetupApplication setupApplication, ISettingsRepository settingsRepository, ISyncService syncService, IWebApiProxy webApiProxy)
        {
            _configService = configService;
            _setupApplication = setupApplication;
            _settingsRepository = settingsRepository;
            _syncService = syncService;
            _webApiProxy = webApiProxy;
        }

        public string DumpObject(string name, object o)
        {
            string s = JsonConvert.SerializeObject(o);
            TI.trace("Serialised " + name,  s);
            return s;
        }

        /// <summary>
        /// #1
        /// </summary>
        /// <returns></returns>
        public ClientApplication AppIdSetup()
        {
            return SetupHub.AppIdSetup(_settingsRepository, _configService);
        }
        /// <summary>
        /// #2
        /// </summary>
        /// <param name="url"></param>
        public void SetConfigWebserviceUrl(string url)
        {
            Config config = _configService.Load();
            config.WebServiceUrl = url;
            _configService.Save(config);
        }
        /// <summary>
        /// #3 Login on server and sets up cost centre id in config
        /// </summary>
        public CostCentreLoginResponse InitialLogin(string username, string password, UserType userType)
        {
            CostCentreLoginResponse response = _setupApplication.LoginOnServer(username, password, userType);
            return response;
        }

        /// <summary>
        /// #4 Setup up the cost centre application id and initialise application
        /// </summary>
        /// <returns></returns>
        public async Task<Guid> GetCostCentreApplicationIdAndAppInitialise()
        {
            Config config = _configService.Load();
            Guid ccid = config.CostCentreId;
            string appDescription = "Distributr-MDC-WPF-APP-" + DateTime.Now.ToString("yyyyMMddhhmmss") + "-";

            CreateCostCentreApplicationResponse response = await _webApiProxy.CreateCostCentreApplicationAsync(config.CostCentreId, appDescription);
            Guid ccappid = response.CostCentreApplicationId;
            _syncService.AppInitialize(ccappid, config.CostCentreId, config.WebServiceUrl);

            return ccappid;
        }

        //#5 call can sync
        public bool CanSync()
        {
            return _syncService.CanSync();
        }

        private string SyncStatus = "";

        private void ReportProgress(string progress)
        {
            TI.trace(section + "[ProgressReporter]", progress.Replace('\n', ' '));
            SyncStatus += progress;
        }

        //#6 call get master data
        public async Task<bool> SyncMasterData()
        {
            var progress = new Progress<string>(ReportProgress);
            bool result = await  _syncService.UpdateMasterData(progress);
            return result;
        }

        //#7 call get inventory 
        public async Task<bool> SyncInventory()
        {
            var progress = new Progress<string>(ReportProgress);
            bool result = await _syncService.UpdateInventory(progress);
            return result;
        }

        //#8 call get payments




    }
}
