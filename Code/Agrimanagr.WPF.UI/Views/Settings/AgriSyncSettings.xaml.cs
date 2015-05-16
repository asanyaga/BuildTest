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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.ViewModels.ApplicSettings;

namespace Agrimanagr.WPF.UI.Views.Settings
{
    /// <summary>
    /// Interaction logic for AgriSyncSettings.xaml
    /// </summary>
    
    public partial class AgriSyncSettings : UserControl
    {
        private GeneralSettingsViewModel _vm = null;
            private bool isInitialized =false;
        public AgriSyncSettings()
        {
            InitializeComponent();
            isInitialized = true;
            _vm = DataContext as GeneralSettingsViewModel;
        }

      private void SetSyncApp_Loaded(object sender, RoutedEventArgs e)
        {
           if(_vm ==null)
               _vm = DataContext as GeneralSettingsViewModel;

          _vm.LoadCurrentAppsCommand.Execute(null);
        }

   
    }
}
