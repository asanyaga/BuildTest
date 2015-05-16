using System;
using System.Windows;
using System.Windows.Controls;
using Distributr.WPF.Lib.ViewModels.IntialSetup;

namespace Agrimanagr.WPF.UI.Views.LoginViews
{
    public partial class AgriConfiguration : Page
    {
        private ConfigurationViewModel _vm;
        public AgriConfiguration()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Configuration_Loaded);
        }

        void Configuration_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = this.DataContext as ConfigurationViewModel;
            _vm.SyncStatusInfo = "";
            _vm.CostCentreApplicationId = Guid.Empty;
            _vm.GetApplicationIdCommand.Execute(null);
        }
    }
}
