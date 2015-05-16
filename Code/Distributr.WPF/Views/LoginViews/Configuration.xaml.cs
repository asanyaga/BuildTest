using System;
using System.Collections.Generic;
using System.Linq;
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
using Distributr.WPF.Lib.ViewModels.IntialSetup;

namespace Distributr.WPF.UI.Views.LoginViews
{
    /// <summary>
    /// Interaction logic for Configuration.xaml
    /// </summary>
    public partial class Configuration : UserControl
    {
        private ConfigurationViewModel _configurationViewModel;
        public Configuration()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Configuration_Loaded);
        }

        void Configuration_Loaded(object sender, RoutedEventArgs e)
        {
            _configurationViewModel = this.DataContext as ConfigurationViewModel;
            _configurationViewModel.SyncStatusInfo = "";
            _configurationViewModel.CostCentreApplicationId = Guid.Empty;
            _configurationViewModel.GetApplicationIdCommand.Execute(null);
        }

        private void btgetAppId_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            _configurationViewModel.SaveSettingCommand.Execute(null);
        }

        private void btSysnc_Click(object sender, RoutedEventArgs e)
        {
            _configurationViewModel.SyncCommand.Execute(null);
        }

        private void btLogin_Click(object sender, RoutedEventArgs e)
        {
            _configurationViewModel.FinishCommand.Execute(null);
        }
    }
}
