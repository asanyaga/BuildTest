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

namespace Agrimanagr.WPF.UI.Views
{
    public partial class InventoryHome : UserControl
    {
        public InventoryHome()
        {
            InitializeComponent();
        }

        private void LoadDefaultPage()
        {
        }

        private void InventoryTransferTabChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source.GetType() != typeof (TabControl))
                return;
            TabItem tabItem = e.AddedItems[0] as TabItem;
            Uri url = null;
            switch (tabItem.Name)
            {
                case "In_Storage_TabItem":
                    url = new Uri("/views/CommodityReception/AwaitingReception.xaml", UriKind.Relative);
                    break;
                case "Transfered_TabItem":
                    url = new Uri("/views/CommodityReception/AwaitingReception.xaml", UriKind.Relative);
                    break;
                default:
                    url = new Uri("/views/CommodityReception/AwaitingReception.xaml", UriKind.Relative);
                    break;
            }
        }
    }
}
