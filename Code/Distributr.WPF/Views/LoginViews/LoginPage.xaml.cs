using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.ViewModels.IntialSetup;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StructureMap;

namespace Distributr.WPF.UI.Views.LoginViews
{
    public partial class LoginPage : UserControl
    {
        private LoginViewModel _vm;
        public LoginPage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(LoginPage_Loaded);
        }

        void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = this.DataContext as LoginViewModel;
            txtusername.Focus();
            txtusername.SelectAll();
            string user = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToLower();
            if (user == @"virtualcity\goyoo" || user == @"juve-pc\gitau")
            {
                _vm.Username = "kameme";
                txtpassword.Password = "12345678";
                // Login1_Click(this, null);
            }
            else if (user == @"juve-pc\gitau")
            {
                _vm.Username = "kameme";
                txtpassword.Password = "12345678";
               // Login1_Click(this, null);
            }
           
        }

        private void Login1_Click(object sender, RoutedEventArgs e)
        {
            _vm.Password = txtpassword.Password;
            _vm.LoginCommand.Execute(null);
        }
    }
}
