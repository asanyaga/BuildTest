using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using Distributr.WPF.Lib.ViewModels.Printerutilis;
using Distributr.WPF.Lib.ViewModels.Transactional.InventoryViewModels;

namespace Distributr.WPF.UI.Views.Reports
{
    public partial class InventoryListing : Page
    {
        ListInventoryViewModel vm = null;
        public InventoryListing()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(InventoryListing_Loaded);
        }

        void InventoryListing_Loaded(object sender, RoutedEventArgs e)
        {
            vm = DataContext as ListInventoryViewModel;
            vm.LoadInventoryList.Execute(null);
        }

        private void hlView_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = sender as Hyperlink;
            Guid id = new Guid(hl.Tag.ToString());
            Uri uri = new Uri("/views/Reports/DocumentsReport.xaml?ProductId="+id, UriKind.Relative);
            NavigationService.Navigate(uri);
        }

       

    }
}
