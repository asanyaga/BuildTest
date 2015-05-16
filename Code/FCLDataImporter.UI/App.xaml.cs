using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Distributr.DataImporter.Lib.IoC;
using Distributr.DataImporter.Lib.Utils;
using Distributr.DataImporter.Lib.ViewModel;
using FCLDataImporter.UI.Views;
using StructureMap;
using log4net;

namespace FCLDataImporter.UI
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
            FileUtility.InitLogFile();
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            ObjectFactory.Configure(x => x.For<IImportValidationPopUp>().Use<ValidationResultsPopUp>());
            ObjectFactory.Configure(x => x.For<IShowWorkingFolderPopUp>().Use<ShowWorkingFolderPopUp>());
           
        }

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                FileUtility.LogError("----Loader exception-----");
                 var assemblyName = args.Name;
            if(!string.IsNullOrEmpty(assemblyName))
                FileUtility.LogError(assemblyName);
                var requestingAssembly = args.RequestingAssembly;
                if(requestingAssembly !=null)
                {
                   var assembly = requestingAssembly.GetName();
                   FileUtility.LogError(assembly.FullName);

                   FileUtility.LogError(args.RequestingAssembly.ToString());

                   FileUtility.LogError("------end------");

                    
                }
            }
            catch
            {

            }
            return null;

        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.Error("Unhandled exception ", e.Exception);
            e.Handled = true;
            MessageBox.Show("Something bad has happened.See logs for details", "Distributr: Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            FileUtility.LogError(e.Exception.Message + (e.Exception.InnerException == null ? "" : "\n" + e.Exception.InnerException.ToString()));


            if (e.Exception is ReflectionTypeLoadException)
            {
                var typeLoadException = e.Exception as ReflectionTypeLoadException;
                if (typeLoadException != null)
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
