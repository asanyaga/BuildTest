using System.Windows;
using System.Windows.Controls;
using SAPUtilityLib.ViewModels;

namespace Sage.Integrations.UI.Views
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class Settings : Page
    {
        private SapSettingsViewModel _vm;
        public Settings()
        {
            InitializeComponent();
            _vm = DataContext as SapSettingsViewModel;
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var sorce = sender as PasswordBox;
            if (sorce.Name == "txtPassword")
            _vm.Password = txtPassword.Password;
            if (sorce.Name == "dbPassword")
                _vm.DbPassword = dbPassword.Password;
        }
    }
}
