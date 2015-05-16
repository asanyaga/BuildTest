using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClientTestHarness.GenerateCommands;
using ClientTestHarness.GenerateCommands2;
using Distributr.Core;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.WPF.Lib.Data.EF;
using Distributr.WPF.Lib.Data.Migrations;
//using Distributr.WPF.Lib.Services.Service.Config;
using Distributr.WPF.Lib.Services.Service.Sync;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;

namespace ClientTestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            int noDocuments = 1;
            if (args.Any())
            {
                noDocuments = Int32.Parse(args[0]);
            }

            Trace.Write("Starting Client Harness");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            for (int i = 0; i < noDocuments; i++)
            {
                Console.WriteLine("Begin document {0}", i);
                var documentCommands = new GenerateCommandsFromSeed().BuildMainOrderCommandsFromSeed();
                foreach (var command in documentCommands.OrderBy(n => n.Item1))
                {
                    Console.WriteLine("sending command {0} commandtype {1} ", command.Item2.CommandId,command.Item2.CommandTypeRef);
                    bool result = new SendCommand().SendDocumentCommand(command.Item2);
                }
            }

            return;

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DistributrLocalContextAuto, ConfigurationMigritation>());
            Services.Setup();

            AppIdSetup();

            bool canlogin = LoginProcess.Login();
            if (!canlogin)
            {
                Console.WriteLine("Unable to login");
                return;
            }

            if (!Initialise())
            {
                Console.WriteLine("Unable to initialise");
                return;
            }
            bool masterDataSync = SyncMasterData().Result;

            Console.WriteLine("Adding {0} documents", noDocuments);
            for (int i = 0; i < noDocuments;i++ )
                GenerateIAN.Run();

            Console.WriteLine("Begin sync process");
            SyncProcess.RunUploadCommands(noDocuments * 4);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Trace.Write(e.ExceptionObject.ToString());
        }

        private static void AppIdSetup()
        {
                IConfigService configService = Services.Using<IConfigService>();
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
        }

        public static bool Initialise()
        {
            IConfigService configService = Services.Using<IConfigService>();
            ISetupApplication setupApplication = Services.Using<ISetupApplication>();
            Config config = configService.Load();
            CreateCostCentreApplicationResponse response = setupApplication.CreateCostCentreApplication(config.CostCentreId, "Test");
            if (response != null && response.CostCentreApplicationId != Guid.Empty)
            {
                Guid costCentreApplicationId = response.CostCentreApplicationId;
                config.CostCentreApplicationId = costCentreApplicationId;
                config.DateInitialized = DateTime.Now;
                config.IsApplicationInitialized = true;
                config.ApplicationStatus = 1;
                configService.Save(config);
                return true;
            }
            return false;
        }

        public static async Task<bool> SyncMasterData()
        {
            IConfigService configService = Services.Using<IConfigService>();
            IUpdateMasterDataService updateMasterDataService = Services.Using<IUpdateMasterDataService>();
            Config config = configService.Load();
            Guid appId = config.CostCentreApplicationId;
            VirtualCityApp vcAppId = VirtualCityApp.Ditributr;

            foreach (MasterDataCollective masterData in SyncMasterDataCollective.GetMasterDataCollective(vcAppId))
            {
                string entity = masterData.ToString();

                bool success = await updateMasterDataService.GetByBatchAndUpdateEntityMasterDataAsync(appId, entity);
                    if (success)
                        Console.WriteLine(string.Format("==>  Successful update of {0}", entity));
                    else
                    {

                        Console.WriteLine(string.Format("....Failed to update {0}", entity));
                        break;
                    }
            }
            return true;
        }
    }
}
