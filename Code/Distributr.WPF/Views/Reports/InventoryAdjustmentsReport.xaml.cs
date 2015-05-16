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
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.IAN;
using StructureMap;

namespace Distributr.WPF.UI.Views.Reports
{
    /// <summary>
    /// Interaction logic for InventoryAdjustmentsReport.xaml
    /// </summary>
    public partial class InventoryAdjustmentsReport : Page
    {
        ListIANViewModel _vm = null;
        public InventoryAdjustmentsReport()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(InventoryAdjustmentsReport_Loaded);
            LabelControls();
        }

        void InventoryAdjustmentsReport_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as ListIANViewModel;
            _vm.Id = "";
            string _current = NavigationService.Source.OriginalString.ParseQueryString("current");
            
            _current = PresentationUtility.ParseIdFromUrl(NavigationService.CurrentSource).ToString();
            if (!string.IsNullOrEmpty(_current))
            {
                _vm.Id = _current;
            }
            _vm.RunLoadIANs();
        }

        void LabelControls()
        {
            IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();

            lblTitle.Content = messageResolver.GetText("sl.viewAdjustmentReports.title_lbl");
            lblStartDate.Content = messageResolver.GetText("sl.viewAdjustmentReports.startDate_lbl");
            lblEndDate.Content = messageResolver.GetText("sl.viewAdjustmentReports.endDate_lbl");
            cmdGenerate.Content = messageResolver.GetText("sl.viewAdjustmentReports.runReport_btn");
        }

        private void cmdGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToDateTime(StartDateDP.Text) <= Convert.ToDateTime(EndDateDP.Text))
            {
                _vm.LoadIANs.Execute(null);
                IANGrid.ItemsSource = _vm.IANList;
            }
            else
                MessageBox.Show("Start Date should not be greater than end date");
        }
    }
}
