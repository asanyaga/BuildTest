using System;
using DistributrAgrimanagrFeatures.Helpers.DB;
using DistributrAgrimanagrFeatures.Helpers.Initialise;
using DistributrAgrimanagrFeatures.Helpers.IOC;
using DistributrAgrimanagrFeatures.Helpers.SFSteps;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using Microsoft.Owin.Hosting;
using StructureMap;

namespace DistributrAgrimanagrFeatures.DocumentFeatures
{
    public static class SetupHelper
    {
        public static void SetupDatabasesPullMasterdata()
        {
            TI.trace("Once per namespace. Doc features Setup");

            string section = "Setup-RunOncePerNameSpace_@DocumentSetup";
            TI.trace(section, "master data hook before feature >>>>>>>>");

            TI.trace(section, "======================= MasterData Setup =======================================");
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

            //Setup hub application and sync master data
            InitializationHelper helper = ObjectFactory.GetInstance<InitializationHelper>();
            using (var webApp = WebApp.Start<StartupServerAutofac>("http://localhost:9443/"))
            {
                bool setupOk = StepHelper.DoCompleteSetup(helper).Result;
                TI.trace("Completed Master Data Sync");
            }
            //reset container
            ObjectFactory.Initialize(x => { });
            TI.trace(section, "Begin feature >>>>>>");
        }
    }
}
