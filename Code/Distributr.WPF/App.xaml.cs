using System;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using Distributr.Core.Data.EF;
using Distributr.Core.Data.Script;
using Distributr.Core.Domain.Master.EagcLogin;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.WPF.Lib.Data.EF;
using Distributr.WPF.Lib.Data.IOC;
using Distributr.WPF.Lib.Data.Migrations;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.UI.UI_Utillity.Converters;
using Distributr.WPF.UI.Views.DocumentReports;
using Distributr.WPF.UI.Views.GRN;
using Distributr.WPF.UI.Views.Order_Pos;
using Distributr.WPF.UI.Views.Orders;
using Distributr.WPF.UI.Views.RN;
using Distributr.WPF.UI.Views.Utils;
using StructureMap;
using log4net;
using System.Linq;
using Distributr.WPF.Lib.Data.Setup;

namespace Distributr.WPF.UI
{

    public partial class App : Application
    {
        private ILog _logger = LogManager.GetLogger("App");
        protected override void OnStartup(StartupEventArgs e)
        {
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);

            // Database.SetInitializer(new CreateDatabaseIfNotExists<DistributrLocalContext>());
            // string localConnectionString = ConfigurationManager.ConnectionStrings["DistributrLocalContext"].ConnectionString;

            Task.Factory.StartNew(() =>
                                      {
                                          //go=>loading EF metadata on app start speeds up user activity on app loaded.
                                          using (
                                              var ctx =
                                                  new CokeDataContext(
                                                      ConfigurationManager.AppSettings["cokeconnectionstring"]))
                                          {
                                              ctx.tblCostCentre.ToList();
                                          }
                                      }, TaskCreationOptions.AttachedToParent);


            _logger.Info("Application starting");
            Initializer.Init();
            ObjectFactory.Configure(x => x.For<IComboPopUp>().Use<ComboPopUp>());
            ObjectFactory.Configure(x => x.For<IOrderProductPage>().Use<ProductLookUp>());
            ObjectFactory.Configure(x => x.For<IDistributrMessageBox>().Use<CustomMessageBox>());
            ObjectFactory.Configure(x => x.For<IPaymentPopup>().Use<PaymentModeModalPopup>());
            ObjectFactory.Configure(x => x.For<ICustomReportViewer>().Use<CustomReportViewer>());
            ObjectFactory.Configure(x => x.For<IChangeSalesmanToDeliveryPopUp>().Use<ChangeSalesmanToDeliveryPopUp>());
            ObjectFactory.Configure(x => x.For<IGrnPopUp>().Use<GRNItemModal>());
            ObjectFactory.Configure(x => x.For<IPrintableDocumentViewer>().Use<DocumentReportViewer>());
            ObjectFactory.Configure(x => x.For<IItemsLookUp>().Use<ItemLookUp>());
            ObjectFactory.Configure(x => x.For<IItemsEnumLookUp>().Use<ItemEnumLookUp>());
            ObjectFactory.Configure(x => x.For<IUnderBankingPopUp>().Use<UnderBanking>());
            ObjectFactory.Configure(x => x.For<IUnderBankingConfirmationPopUp>().Use<UnderBankingPayment>());

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DistributrLocalContextAuto, ConfigurationMigritation>());

            DistributrDataHelper.Migrate();



            AppIdSetup();


            CultureInfo ci = CultureInfo.CreateSpecificCulture("en-GB");
            ci.DateTimeFormat.ShortDatePattern = "dd-MMM-yyyy";
            Thread.CurrentThread.CurrentCulture = ci;
            _logger.Info("Startup complete");
            //clear integration docs

        }


        private static void AppIdSetup()
        {
            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                ISettingsRepository settings = c.GetInstance<ISettingsRepository>();
                IConfigService configService = c.GetInstance<IConfigService>();
                SetupHub.AppIdSetup(settings, configService);
            }
        }



        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {

            e.Handled = true;
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
            MessageBox.Show(e.Exception.Message + (e.Exception.InnerException == null ? "" : "\n" + e.Exception.InnerException.ToString()), "Distributr: Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }



    }
}
