using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.WPF.Lib.Reporting;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using Microsoft.Reporting.WinForms;
using StructureMap;

namespace Distributr.WPF.UI.Views.Orders
{
    /// <summary>
    /// Interaction logic for OrderSummaryWithExport.xaml
    /// </summary>
    public partial class OrderSummaryWithExport : Page
    {
        private SalesmanOrderSummaryListingViewModel _vm;
        public OrderSummaryWithExport()
        {
            InitializeComponent();
            
            _vm = DataContext as SalesmanOrderSummaryListingViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PendingwindowsFormsHost.Child = _vm.ViewPendingOrders();
        }

        private void Approved_Click(object sender, RoutedEventArgs e)
        {
            ApprovedwindowsFormsHost.Child = _vm.ViewApprovedOrders();
        }
    }
}
