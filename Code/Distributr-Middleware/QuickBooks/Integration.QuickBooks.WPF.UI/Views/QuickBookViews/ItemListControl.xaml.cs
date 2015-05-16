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
using Integration.QuickBooks.Lib.QBIntegrationViewModels;

namespace Integration.QuickBooks.WPF.UI.Views.QuickBookViews
{
    /// <summary>
    /// Interaction logic for ItemListControl.xaml
    /// </summary>
    public partial class ItemListControl : UserControl
    {
        private QBListTransactionsNewViewModel _vm;
        public ItemListControl()
        {
            InitializeComponent();
            _vm = DataContext as QBListTransactionsNewViewModel;
        }

        private void OnContentChanged(object sender, RoutedEventArgs e)
        {
           // busy.IsBusy = true;
        }
    }
}
