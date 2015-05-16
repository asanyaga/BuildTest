using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master.UserEntities;
using DistributrAgrimanagrFeatures.Helpers.Initialise;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
namespace DistributrAgrimanagrFeatures.Helpers.SFSteps
{
    public class StepHelper
    {
        private static string section = "StepHelper";

        public static string WSUrl
        {
            
            get { return "http://localhost:9443/"; }
        }

        public static string InitialLoginUserName
        {
            
            get { return "kameme"; }
        }

        public static string InitialLoginPassword
        {
            get { return "12345678"; }
        }

        public static void AppIdSetup(InitializationHelper helper)
        {
            TI.trace(section, "AppIdSetup");
            helper.AppIdSetup();
        }

        public static void SetConfigWebserviceUrl(InitializationHelper helper)
        {
            TI.trace(section, "SetConfigWebserviceUrl");

            helper.SetConfigWebserviceUrl(WSUrl);
        }

        public static CostCentreLoginResponse InitialLogin(InitializationHelper helper)
        {
            TI.trace(section, "InitialLogin");

            return helper.InitialLogin(InitialLoginUserName, InitialLoginPassword, UserType.WarehouseManager);
        }

        public static async Task<Guid> GetCostCentreAppicationIdAndAppInit(InitializationHelper helper)
        {
            TI.trace(section, "GetCostCentreAppicationIdAndAppInit");

            Guid ccAppId = await helper.GetCostCentreApplicationIdAndAppInitialise();
            return ccAppId;
        }

        public static void CanSync(InitializationHelper helper)
        {
            TI.trace(section, "CanSync");

            helper.CanSync();
        }

        public static async Task<bool> SyncMasterData(InitializationHelper helper)
        {
            TI.trace(section, "Sync master data");
            bool r = await helper.SyncMasterData();
            return r;
        }

        public static async Task<bool> DoCompleteSetup(InitializationHelper helper)
        {
            AppIdSetup(helper);
            SetConfigWebserviceUrl(helper);
            CostCentreLoginResponse r = InitialLogin(helper);
            Guid ccappId = await GetCostCentreAppicationIdAndAppInit(helper);
            CanSync(helper);
            bool masterDataSync = await SyncMasterData(helper);
            return true;
        }

    }
}
