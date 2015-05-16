using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Distributr_Middleware.WPF.Lib.ViewModels;
using Integration.QuickBooks.Lib.QBIntegrationViewModels;

namespace Integration.QuickBooks.WPF.UI.Views
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Window
    {
        private MiddlewareLoginViewModel _vm;
       
        public LoginPage()
        {
            InitializeComponent();
           _vm = this.DataContext as MiddlewareLoginViewModel;
            if(System.Diagnostics.Debugger.IsAttached)
            {
                _vm.Username = "john1";
                _vm.Password = "12345678";
            }
          
        }

        
        private void SetVersionNumber()
        {
            string version = "Version No.: " + ParseVersionNumber(Assembly.GetExecutingAssembly()).ToString();
            //TxtVersion.Text = version;
        }

        private static Version ParseVersionNumber(Assembly assembly)
        {
            AssemblyName assemblyName = new AssemblyName(assembly.FullName);
            return assemblyName.Version;
        }

        private void txtUserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            _vm.Username = txtUserName.Text.Trim();
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
            _vm.Username = "";
            _vm.Password = "";
            txtPassword.Password = "";
            this.Close();
        }
    }
}
