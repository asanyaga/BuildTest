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
using Integration.Cussons.WPF.Lib.CussonsIntegrationViewModels;

namespace Integration.Cussons.WPF.UI.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Window
    {
        private CussonsLoginViewModel _vm;
        bool isInitialized = false;
        public LoginPage()
        {
            InitializeComponent();
            Loaded += LoginPage_Loaded;
            _vm = this.DataContext as CussonsLoginViewModel;
            isInitialized = true;
        }

        void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            SetVersionNumber();
            string name = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToLower();

            if (name == @"goblet\owner" || name == @"VIRTUALCITY\cndirangu" || name == @"VIRTUALCITY\goyoo"
                || name == @"jakanudo-pc\jakanudo")
            {
                _vm.Username = "kameme";
                txtPassword.Password = "12345678";
                btnLogin_Click(this, null);
            }
        }

        private void SetVersionNumber()
        {
            string version = "Version No.: " + ParseVersionNumber(Assembly.GetExecutingAssembly()).ToString();
            TxtVersion.Text = version;
        }

        private static Version ParseVersionNumber(Assembly assembly)
        {
            AssemblyName assemblyName = new AssemblyName(assembly.FullName);
            return assemblyName.Version;
        }

        private void txtUserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isInitialized) return;
            _vm.Username = txtUserName.Text.Trim();
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!isInitialized) return;
            _vm.Password = txtPassword.Password;


        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.Login())
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
