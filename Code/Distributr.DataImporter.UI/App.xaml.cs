using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Distributr.DataImporter.Lib.IoC;
using Distributr.DataImporter.Lib.ViewModel;
using StructureMap;
using log4net;

namespace Distributr.DataImporter.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ILog _logger = LogManager.GetLogger("App");
        protected override void OnStartup(StartupEventArgs e)
        {
            Initializer.Init();
            this.DispatcherUnhandledException +=App_DispatcherUnhandledException;
            ObjectFactory.Configure(x => x.For<IImportValidationPopUp>().Use<ImportValidationResultPopUp>());
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.Error("Unhandled exception ", e.Exception);
            e.Handled = true;
            MessageBox.Show(e.Exception.Message + (e.Exception.InnerException == null ? "" : "\n" + e.Exception.InnerException.ToString()), "Distributr: Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }
}
