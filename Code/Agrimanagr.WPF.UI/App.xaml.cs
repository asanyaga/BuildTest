using System;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Agrimanagr.WPF.UI.Views.Admin.CommoditySuppliers;
using Agrimanagr.WPF.UI.Views.Admin.Contacts;
using Agrimanagr.WPF.UI.Views.CommodityReception;
using Agrimanagr.WPF.UI.Views.CommodityRelease;
using Agrimanagr.WPF.UI.Views.InventoryTransfer;
using Agrimanagr.WPF.UI.Views.ReceiptView;
using Agrimanagr.WPF.UI.Views.UtilityViews;
using Agrimanagr.WPF.UI.Views.Warehousing;
using Distributr.Core.Data.Script;
using Distributr.WPF.Lib.Data.EF;
using Distributr.WPF.Lib.Data.IOC;
using Distributr.WPF.Lib.Data.Migrations;
using Distributr.WPF.Lib.Impl.Services.Utility;
using Distributr.WPF.Lib.Services.Service.Sync;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using StructureMap;
using log4net;

namespace Agrimanagr.WPF.UI
{
    public partial class App : Application
    {
        private ILog _logger = LogManager.GetLogger("App");
        protected override void OnStartup(StartupEventArgs e)
        { 
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            _logger.Info("Application starting");
           
            Initializer.Init();
            

           
            ObjectFactory.Configure(x => x.For<IEditContactModal>().Use<AddUserContactsModal>());
            ObjectFactory.Configure(x => x.For<IAgrimanagrComboPopUp>().Use<ComboPopUp>());
            ObjectFactory.Configure(x => x.For<IReceiptDocumentPopUp>().Use<ReceiptDocument>());
            ObjectFactory.Configure(x => x.For<IReleaseDocumentPopUp>().Use<ReleaseDocument>());
            ObjectFactory.Configure(x => x.For<IWeighAndReceivePopUp>().Use<WeighAndReceivePopup>());
            ObjectFactory.Configure(x => x.For<IStoreCommodityPopUp>().Use<StoreCommodity>());
            ObjectFactory.Configure(x => x.For<IEditCommodityProducerModal>().Use<EditCommodityProducerModal>());
            ObjectFactory.Configure(x => x.For<IEditCommodityOwnerModal>().Use<EditCommodityOwnerModal>());
            ObjectFactory.Configure(x => x.For<IFarmerOutletMapping>().Use<FarmerOutletMapping>());
            ObjectFactory.Configure(x => x.For<IAgriItemsLookUp>().Use<AgriItemLookUpView>());
            ObjectFactory.Configure(x => x.For<ILoginPopup>().Use<EagcLogin>());
            //IStoreCommodityPopUp
            
           
            

           
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DistributrLocalContextAuto, ConfigurationMigritation>());

            DistributrDataHelper.Migrate();

            AppIdSetup();

            CultureInfo ci = CultureInfo.CreateSpecificCulture("en-GB");
             ci.DateTimeFormat.ShortDatePattern = "dd-MMM-yyyy";
            Thread.CurrentThread.CurrentCulture = ci;
            _logger.Info("Startup complete");
        }

        private static void AppIdSetup()
        
        {
            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                IConfigService configService = c.GetInstance<IConfigService>();
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
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if(e.Exception is AutoSyncException)
            {
                var ex = e.Exception as AutoSyncException;
                ShowMessage(ex);
                e.Handled = true;
                return;
            }
            if (e.Exception is ReflectionTypeLoadException)
            {
                _logger.Error("Unhandled ReflectionTypeLoadException ", e.Exception);
                var typeLoadException = e.Exception as ReflectionTypeLoadException;
                var loaderExceptions = typeLoadException.LoaderExceptions;
                string messages = "";
                foreach (var loaderException in loaderExceptions)
                {
                    messages += loaderException.Message + "\n";
                }
                MessageBox.Show(messages);
            }
            _logger.Error("Unhandled exception ", e.Exception);
            e.Handled = true;
            MessageBox.Show(e.Exception.Message + (e.Exception.InnerException == null ? "" : "\n" + e.Exception.InnerException.ToString()),"Agrimangr: Error",MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        private void ShowMessage(Exception ex)
        {
            var autoSyncService = ObjectFactory.GetInstance<IAutoSyncService>();
            if (autoSyncService.ShowMessageBox)
            {
                var result = MessageBox.Show(ex.Message + "\n\n Do you want to show this message again? ", " Auto Sync Service", MessageBoxButton.YesNo);
                autoSyncService.StartAutomaticSync();
                if (result == MessageBoxResult.No)
                    autoSyncService.SetShowMessageBox(false);
            }
        }

    }
}
