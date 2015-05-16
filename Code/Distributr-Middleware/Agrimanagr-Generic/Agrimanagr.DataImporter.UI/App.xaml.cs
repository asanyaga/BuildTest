using System.Windows;
using System.Windows.Threading;
using Agrimanagr.DataImporter.Lib.IoC;
using Agrimanagr.DataImporter.UI.Views;
using Distributr_Middleware.WPF.Lib.Utils;
using GalaSoft.MvvmLight.Threading;
using StructureMap;
using log4net;

namespace Agrimanagr.DataImporter.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 
    public partial class App : Application
    {
        private ILog _logger = LogManager.GetLogger("App");
        protected override void OnStartup(StartupEventArgs e)
        {
            Initializer.Init();
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            DispatcherHelper.Initialize();
            ObjectFactory.Configure(x => x.For<IImportValidationPopUp>().Use<ValidationResultsPopUp>());
            
        }

       private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.Error("Unhandled exception ", e.Exception);
            e.Handled = true;
           var msg = e.Exception.Message +
                     (e.Exception.InnerException == null ? "" : "\n" + e.Exception.InnerException.ToString());
           FileUtility.LogError(msg);
            MessageBox.Show(msg, "Distributr: Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }
}
