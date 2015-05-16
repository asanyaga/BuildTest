using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Distributr.Core.Domain.Transactional.DocumentEntities;


namespace Distributr.WPF.UI.Views.Reports
{
    public partial class SalesManOrdersReport : Page
    {
       
        public SalesManOrdersReport()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected  void OnNavigatedTo(NavigationEventArgs e)
        {
            vm = this.DataContext as ListSalesmanOrdersViewModel;
            vm.LoadOrderTypesCommand.Execute(null);
            vm.LoadOrderStatusCommand.Execute(null);
            vm.LoadOrdersCommand.Execute(null);
        }

        private void cmdGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Convert.ToDateTime(StartDateDP.Text) <= Convert.ToDateTime(EndDateDP.Text))
                {
                    vm.LoadOrdersCommand.Execute(null);
                    dg1.ItemsSource = vm.Report;
                }
                else
                    MessageBox.Show("Start Date should not be greater than end date");                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void cmbOrderStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                vm.OrderStatus = (DocumentStatus)cmbOrderStatus.SelectedItem;
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cmdOrderType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                vm.Ordertype = (OrderType)cmdOrderType.SelectedItem;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
