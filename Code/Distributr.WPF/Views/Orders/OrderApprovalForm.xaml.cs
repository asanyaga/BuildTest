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
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;

namespace Distributr.WPF.UI.Views.Orders
{
    /// <summary>
    /// Interaction logic for OrderApprovalForm.xaml
    /// </summary>
    public partial class OrderApprovalForm : Page
    {
        private OrderApprovalViewModel vm;
        public OrderApprovalForm()
        {
            InitializeComponent();
            vm = DataContext as OrderApprovalViewModel;
        }

        private void Edit_Click_1(object sender, RoutedEventArgs e)
        {
            var selectedRow = ((Hyperlink)sender).Tag;
            vm.EditProductCommand.Execute(selectedRow);
        }

        private void Delete_Click_1(object sender, RoutedEventArgs e)
        {
            var selectedRow = ((Hyperlink)sender).Tag;
            vm.DeleteProductCommand.Execute(selectedRow);
        }
    }
}
