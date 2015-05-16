using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Distributr.WPF.Lib.Data.IOC;
using Integration.Cussons.WPF.Lib.IOC;
using Integration.Cussons.WPF.Lib.Utils;
using Integration.Cussons.WPF.UI.Pages;
using Integration.Cussons.WPF.UI.Pages.UtilityControls;
using StructureMap;
using log4net;

namespace Integration.Cussons.WPF.UI
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
            PzInitializer.Init();
            ObjectFactory.Configure(x => x.For<IAdjustInventoryWindow>().Use<AdjustInventory>());
            ObjectFactory.Configure(x => x.For<IAbout>().Use<About>());
            _logger.Info("Startup complete");
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.Error("Unhandled exception ", e.Exception);
            e.Handled = true;
           MessageBox.Show(
                e.Exception.Message
                 + (e.Exception.InnerException == null ? "" : "\n" + e.Exception.InnerException.ToString())
                ,
                "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
           FileUtility.LogError(e.Exception.InnerException == null ? "" : "\n" + e.Exception.InnerException.ToString());
           

            if (e.Exception is ReflectionTypeLoadException)
            {
                var typeLoadException =e.Exception as ReflectionTypeLoadException;
                if(typeLoadException !=null)
                {
                    var loaderExceptions = typeLoadException.LoaderExceptions;
                    foreach (var loaderException in loaderExceptions)
                    {
                       FileUtility.LogError("================");
                       FileUtility.LogError(loaderException.Message);
                       FileUtility.LogError("================");
                    }

                  
                }
                
            }

        }
        
       
    }
}
       