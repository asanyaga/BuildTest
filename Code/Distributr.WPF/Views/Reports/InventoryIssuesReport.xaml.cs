using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Xml;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.ViewModels.Printerutilis;
using Distributr.WPF.Lib.ViewModels.Transactional.ITN;
using StructureMap;

namespace Distributr.WPF.UI.Views.Reports
{
    public partial class InventoryIssuesReport : Page
    {
        ListITNViewModel vm = null;
        public InventoryIssuesReport()
        {
            InitializeComponent();
            LabelControls();
            Loaded += new RoutedEventHandler(InventoryIssuesReport_Loaded);
        }

        void InventoryIssuesReport_Loaded(object sender, RoutedEventArgs e)
        {
            vm = DataContext as ListITNViewModel;
            vm.LoadWareHouses.Execute(null);
            cmbSalesmen.ItemsSource = vm.SalesMen;
            vm.LoadItNs.Execute(null);
        }

        void LabelControls()
        {
            IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();

            lblTitle.Content = messageResolver.GetText("sl.viewInventoryTransfers.title_lbl");
            lblSalesmen.Content = messageResolver.GetText("sl.viewInventoryTransfers.salesmen_lbl");
            lblStartDate.Content = messageResolver.GetText("sl.viewInventoryTransfers.startDate_lbl");
            lblEndDate.Content = messageResolver.GetText("sl.viewInventoryTransfers.endDate_lbl");
            cmdGenerate.Content = messageResolver.GetText("sl.viewInventoryTransfers.runReport_btn");
        }

        private void cmdGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToDateTime(StartDateDP.Text) <= Convert.ToDateTime(EndDateDP.Text))
            {
            vm.LoadItNs.Execute(null);
            ITNGrid.ItemsSource = vm.ItnList;
            }
            else
                MessageBox.Show("Start Date should not be greater than end date");
        }
        
    }
}
