using System;
using System.Reflection;
using System.Windows;
using Distributr.DataImporter.Lib.ViewModel;
using Distributr.DataImporter.Lib.ViewModel.FCL;
using GalaSoft.MvvmLight.Messaging;

namespace FCLDataImporter.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FclMainWindowViewModel _vm;
        public MainWindow()
        {
            InitializeComponent();
            _vm = DataContext as FclMainWindowViewModel;

            Messenger.Default.Register<Uri>(this, "NavigationRequest", (uri) => this.ContentFrame.Navigate(uri));
            _vm.ExitApplicationEventHandler += (s, e) => this.Close();
              Closing += MainWindow_Closing;
              _vm.TitleWithVesion = string.Format("FCL-Integration. Version :{0}", GetVersion());
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to quit this application?"
                                , "Importer: Exit Application",
                                MessageBoxButton.YesNo)
                == MessageBoxResult.No)
                e.Cancel = true;
            else
            {
                ViewModelLocator.Cleanup();
                Application.Current.Shutdown();
            }
            
        }
        private string GetVersion()
        {

            var assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = new AssemblyName(assembly.FullName);
            return assemblyName.Version.ToString();
        }
    }

  

}