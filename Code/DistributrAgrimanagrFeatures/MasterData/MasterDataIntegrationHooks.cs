using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributrAgrimanagrFeatures.Helpers.DB;
using DistributrAgrimanagrFeatures.Helpers.IOC;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using StructureMap;
using TechTalk.SpecFlow;

namespace DistributrAgrimanagrFeatures.MasterData
{
    [Binding]
    public class MasterDataIntegrationHooks
    {
        [BeforeFeature("masterdatahook")]
        public static void BeforeMasterDataFeature()
        {
            string section = "BeforeFeature_@masterdatahook";
            TI.trace(section,"master data hook before feature >>>>>>>>");
            FeatureContext fc = FeatureContext.Current;

            TI.trace(section,"======================= MasterData Setup =======================================");
            TI.trace(section, "Complete data clean ============================ ");
            //check that sql databases exists
            DB_TestingHelper dbh = DefaultDbTestingHelper.GetDefaultDbTestingHelper();
            if (!dbh.CheckDBsExist())
                throw new Exception("Failed to find all required databases");

            //check that mongo service is available
            if (!dbh.CanConnectToMongo())
                throw new Exception("Failed to connect to Mongo");
            //setup all databases
            Autofac.IContainer c = IOCHelper.InitializeServerWithAutofacContainer(dbh.Server_DistributrExmxConnection, dbh.MongoConnectionString, "win", dbh.MongoAuditingConnectionString).Build();
            IOCHelper.InitialiseHubSliceWithStructurmapContainer(dbh.Hub_DistributrEdmxConnection, dbh.Hub_RoutingConnectionString, "");
            DbSetupHelper.SetupAllDatabases(dbh, c, ObjectFactory.Container);

            //reset container
            ObjectFactory.Initialize(x => { });
            TI.trace(section, "Begin feature >>>>>>");

        }

        [BeforeScenario("masterdatahook")]
        public static void BeforeMasterDataScenario()
        {
            TI.trace("<><><><><><> Scenario start <><><><><><><><> ");
            string section = "BeforeScenario_@masterdatahook";
            TI.trace(section, "Master data hook before scenario >>>>");
            DB_TestingHelper dbTestingHelper = DefaultDbTestingHelper.GetDefaultDbTestingHelper();
            //AUTOFAC
            Autofac.IContainer container =  IOCHelper.InitializeServerWithAutofacContainer(dbTestingHelper.Server_DistributrExmxConnection, dbTestingHelper.MongoConnectionString, "win", dbTestingHelper.MongoAuditingConnectionString).Build();
            //STRUCTURE MAP
            IOCHelper.InitialiseHubSliceWithStructurmapContainer(dbTestingHelper.Hub_DistributrEdmxConnection, dbTestingHelper.Hub_RoutingConnectionString, "");
            TI.trace(section, "Only clean hub distributr and hub local database ===================================");
            DbSetupHelper.SetupHubDistributr(dbTestingHelper, ObjectFactory.Container);
            DbSetupHelper.SetupHubLocal(dbTestingHelper);
            ScenarioContext sc = ScenarioContext.Current;
            TI.trace("Begin scenario >>>");
        }

        [AfterScenario("masterdatahook")]
        private static void AfterMasterDataScenario()
        {
            ObjectFactory.Initialize(x => { });
            TI.trace("Master data hook after scenario <<<<");
        }

        [AfterFeature("masterdatahook")]
        public static void AfterMasterDataFeature()
        {
            TI.trace("master data hook before feature <<<<<<<<");
        }



    }
}
