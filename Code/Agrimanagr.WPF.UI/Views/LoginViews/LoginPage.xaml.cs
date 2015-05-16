using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Distributr.WPF.Lib.ViewModels.IntialSetup;

namespace Agrimanagr.WPF.UI.Views
{
    public partial class LoginPage : Page
    {
        private LoginViewModel _vm;
        
        public static bool closing = false;
        bool isInitialized = false;
        public LoginPage()
        {
            isInitialized = false;
            InitializeComponent();
            isInitialized = true;
            Loaded += LoginPage_Loaded;
            _vm = this.DataContext as LoginViewModel;
        }

        void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            SetVersionNumber();
            string user = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToLower();
            if (user == @"goblet\owner" || user == @"virtualcity\cndirangu" ||
                user == @"virtualcity\goyoo")
            {
                _vm.Username = "hubmanager";
                txtPassword.Password = "12345678";
                //btnLogin_Click(this, null);
            }
            else if (user == @"juve-pc\gitau")
            {
                _vm.Username = "hubmanager";
                txtPassword.Password = "12345678";
                btnLogin_Click(this, null);
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            _vm.LoginCommand.Execute(null);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _vm.Username = "";
            _vm.Password = "";
            txtPassword.Password = "";
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!isInitialized) return;
            _vm.Password = txtPassword.Password;
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

        private void txtServerUrl_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isInitialized) return;
            _vm.Url = txtServerUrl.Text.Trim();
        }
    }
}
