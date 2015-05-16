using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Distributr_Middleware.WPF.Lib.IOC;
using Distributr_Middleware.WPF.Lib.Utils;
using Distributr_Middleware.WPF.UI.Views;
using StructureMap;

namespace Distributr_Middleware.WPF.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Initializer.Init();
            this.DispatcherUnhandledException +=App_DispatcherUnhandledException;
            ObjectFactory.Configure(x => x.For<IImportValidationPopUp>().Use<ValidationResultsPopUp>());
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {

            e.Handled = true;
            if (e.Exception is ReflectionTypeLoadException)
            {
               var typeLoadException = e.Exception as ReflectionTypeLoadException;
                var loaderExceptions = typeLoadException.LoaderExceptions;
                string messages = loaderExceptions.Aggregate("", (current, loaderException) => current + (loaderException.Message + "\n"));
                MessageBox.Show(messages);
                return;
            }
            MessageBox.Show(e.Exception.Message + (e.Exception.InnerException == null ? "" : "\n" + e.Exception.InnerException.Message), "Distributr: Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
      
    }
}
