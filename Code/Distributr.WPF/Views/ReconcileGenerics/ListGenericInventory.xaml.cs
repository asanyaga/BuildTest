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
using Distributr.WPF.Lib.ViewModels.Transactional.InventoryViewModels;

namespace Distributr.WPF.UI.Views.ReconcileGenerics
{
    /// <summary>
    /// Interaction logic for ListGenericInventory.xaml
    /// </summary>
    public partial class ListGenericInventory : Page
    {
        ListInventoryViewModel vm = null;
        public ListGenericInventory()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected void OnNavigatedTo(NavigationEventArgs e)
        {
            vm = DataContext as ListInventoryViewModel;
            vm.LoadGenericInventoryList.Execute(null);
        }
    }
}
