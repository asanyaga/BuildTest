using System.Windows;
using Distributr_Middleware.WPF.Lib.ViewModels;

namespace SAPClient.Views
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
            _vm = DataContext as MiddlewareLoginViewModel;

            if(System.Diagnostics.Debugger.IsAttached)
            {
                _vm.Username = "john1";
                _vm.Password = "12345678";
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
          var isSuccess= _vm.Login(this);
            if(isSuccess)
            {
                var mainWindow = new MainWindow();
                this.Hide();
                mainWindow.Show();
                

            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _vm.Username = "";
            _vm.Password = "";
            txtPassword.Password = "";
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _vm.Password = txtPassword.Password;
        }

        
    }
}
