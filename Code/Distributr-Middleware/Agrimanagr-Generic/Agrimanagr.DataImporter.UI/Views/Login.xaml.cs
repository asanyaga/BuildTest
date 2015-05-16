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
using Agrimanagr.DataImporter.Lib.IoC;
using Agrimanagr.DataImporter.Lib.ViewModels;
using Distributr_Middleware.WPF.Lib.ViewModels;

namespace Agrimanagr.DataImporter.UI.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
       private MiddlewareLoginViewModel _vm; 
        public Login()
        {
            InitializeComponent();
            _vm = DataContext as  MiddlewareLoginViewModel;

        }


        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _vm.Password = txtPassword.Password;


        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.Login(this))
            {
                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to quit this application?"
                                , "Importer: Exit Application",
                                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _vm.Username = "";
                _vm.Password = "";
                txtPassword.Password = "";
                this.Close();
                Application.Current.Shutdown();

            }
        }

        

    }
}
