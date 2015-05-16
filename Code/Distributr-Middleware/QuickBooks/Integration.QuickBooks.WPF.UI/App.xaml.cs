using System.Windows;
using System.Windows.Threading;
using Distributr.WPF.Lib.Data.IOC;
using Distributr_Middleware.WPF.Lib.Utils;
using Integration.QuickBooks.Lib.IOC;
using Integration.QuickBooks.WPF.UI.Views;
using StructureMap;
using log4net;
using IAbout = Integration.QuickBooks.Lib.UI.IAbout;

namespace Integration.QuickBooks.WPF.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ILog _logger = LogManager.GetLogger("App");

        protected override void OnStartup(StartupEventArgs e)
        {
            this.DispatcherUnhandledException +=
                new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            _logger.Info("Application starting");
            BootStrapper.Init();
            ObjectFactory.Configure(x => x.For<IAbout>().Use<About>());

            _logger.Info("Startup complete");
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.Error("Unhandled exception ", e.Exception);
            e.Handled = true;
            FileUtility.LogError(e.Exception.Message + "Inner:" + e.Exception.InnerException != null ? e.Exception.InnerException.Message : "");

            MessageBox.Show(
                e.Exception.Message
                // +(e.Exception.InnerException == null ? "" : "\n" + e.Exception.InnerException.ToString())
                ,
                "Quick Books Integration: Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }
}
