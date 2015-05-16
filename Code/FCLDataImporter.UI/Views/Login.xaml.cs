using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Distributr.DataImporter.Lib.Utils;
using Distributr.DataImporter.Lib.ViewModel;

namespace FCLDataImporter.UI.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private LoginViewModel _vm;
        public Login()
        {
            InitializeComponent();
            _vm = DataContext as LoginViewModel;

            FileUtility.LogError("INFO=>LOGIN Page Initialized");

        }

       
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FileUtility.LogError("INFO=>LOGIN button clicked");
            if(_vm !=null)
            {
                _vm.LoginCommand.Execute(null);

                if (_vm.IsLoggedIn)
                {
                    var main = new MainWindow();
                    main.Show();
                    this.Close();
                }
            }
            else
            {
                FileUtility.LogError("Error=>VM is null");
            }
            
        }

        private void QuitApp(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to quit this application?"
                                , "Importer: Exit Application",
                                MessageBoxButton.YesNo)== MessageBoxResult.Yes)
               
            
            {
                ViewModelLocator.Cleanup();
                Application.Current.Shutdown();
               
            }
        }

    }
}
