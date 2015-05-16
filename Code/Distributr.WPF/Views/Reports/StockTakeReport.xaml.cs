using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.Impl.Services.Utility;
using Distributr.WPF.Lib.ViewModels.Transactional.IAN;
using StructureMap;

namespace Distributr.WPF.UI.Views.Reports
{
    public partial class StockTakeReport : Page
    {
        ListIANViewModel _vm = null;
        public StockTakeReport()
        {
            InitializeComponent();
            LabelControls();
            Loaded += StockTakeReport_Loaded;
            double newLayoutRootWidth = OtherUtilities.ContentFrameWidth;
            newLayoutRootWidth = (newLayoutRootWidth * 0.95);

            double newLayoutRootHeight = OtherUtilities.ContentFrameHeight;
            newLayoutRootHeight = (newLayoutRootHeight * 0.95);
            double dataGridHeight = (newLayoutRootHeight * 0.88);

            LayoutRoot.Width = newLayoutRootWidth;
            LayoutRoot.Height = newLayoutRootHeight;
            dgStockTakeList.Height = dataGridHeight;
        }

        void StockTakeReport_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = this.DataContext as ListIANViewModel;
            _vm.LoadStockTakeReport();
        }

        void LabelControls()
        {
            IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();

            lblStartDate.Content = messageResolver.GetText("sl.viewAdjustmentReports.startDate_lbl");
            lblEndDate.Content = messageResolver.GetText("sl.viewAdjustmentReports.endDate_lbl");
            btnGenerate.Content = messageResolver.GetText("sl.viewAdjustmentReports.runReport_btn");
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToDateTime(dpStartDate.Text) <= Convert.ToDateTime(dpEndDate.Text))
            {
                _vm.LoadStockTakeReport();
                //IANGrid.ItemsSource = _vm.IANList;
            }
            else
                MessageBox.Show("Start Date should not be greater than end date");
        }

    }
}
