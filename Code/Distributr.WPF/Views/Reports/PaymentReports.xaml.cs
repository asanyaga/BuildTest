using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Distributr.WPF.Lib.Impl.Services.Utility;
using Distributr.WPF.Lib.ViewModels.Reports;

namespace Distributr.WPF.UI.Views.Reports
{
    public partial class PaymentReports : Page
    {
        private PaymentReportsViewModel _vm;
        public PaymentReports()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(PaymentReports_Loaded);
            Unloaded += new RoutedEventHandler(PaymentReports_Unloaded);
            double newLayoutRootWidth = OtherUtilities.ContentFrameWidth;
            newLayoutRootWidth = (newLayoutRootWidth * 0.95);

            double newLayoutRootHeight = OtherUtilities.ContentFrameHeight;
            newLayoutRootHeight = (newLayoutRootHeight * 0.95);

            double tabControlHeight = (newLayoutRootHeight * 0.800);

            LayoutRoot.Width = newLayoutRootWidth;
            LayoutRoot.Height = newLayoutRootHeight;
            tabReport.Height = tabControlHeight;
        }

        void PaymentReports_Unloaded(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as PaymentReportsViewModel;
            _vm.Cleanup();
        }

        void PaymentReports_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as PaymentReportsViewModel;
            _vm.SetUp();
            _vm.Load();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            _vm.CurrentPage = 1;
            _vm.ListPaymentExceptionReportItems.Clear();
            _vm.Filter();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            cmbOutlets.SelectedIndex = 0;
            cmbSalesmen.SelectedIndex = 0;
            _vm.SearchText = "";
            _vm.ListPaymentExceptionReportItems.Clear();
            _vm.Filter();
        }

        private void txtSearchText_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void tabReport_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmdLoad_Click(object sender, RoutedEventArgs e)
        {
            //dgExceptionReport.ClearValue(DataGrid.ItemsSourceProperty);
            _vm.ListPaymentExceptionReportItems.Clear();
            _vm.Cleanup();
            _vm.Load();
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            _vm.CurrentPage = 1;
            _vm.ListPaymentExceptionReportItems.Clear();
            _vm.Filter();
        }

        private void dgExceptionReport_LoadingRow(object sender, DataGridRowEventArgs e)
        {

        }

    }
}
