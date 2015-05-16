using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Agrimanagr.WPF.UI.Views.CommodityPurchase;
using Agrimanagr.WPF.UI.Views.CommodityReception;
using GalaSoft.MvvmLight.Messaging;

namespace Agrimanagr.WPF.UI.Views
{
    public partial class CommodityHome : UserControl
    {
        private ListCommodityReception _home;
        public CommodityHome()
        {
            InitializeComponent();
        }

        private void LoadDefaultPage()
        {
            if (_home == null)
                _home = new ListCommodityReception();
            ctrlCommodity.Content = _home;
        }
        private void tbCommodityReception_Selected(object sender, RoutedEventArgs e)
        {
            ReceptionSubTab.SelectedIndex = 0;
            Uri url = new Uri("/views/CommodityReception/AwaitingReception.xaml", UriKind.Relative);
            Messenger.Default.Send<Uri>(url, "NavigationRequest");
        }

        private void tbCommodityPurchase_Selected(object sender, RoutedEventArgs e)
        {
            Uri url = new Uri("/views/CommodityPurchase/ListFarmers.xaml", UriKind.Relative);
            Messenger.Default.Send<Uri>(url, "NavigationRequest");

        
        }

        private void tbCommodityDispatch_Selected(object sender, RoutedEventArgs e)
        {

            //AssetPossession assetPossession = new AssetPossession();
            //ctrlDispatch.Content = assetPossession;
        }

        private void ReceptionSubTabChanged(object sender, SelectionChangedEventArgs e)
        {if(e.Source.GetType() !=typeof(TabControl))
                return;

           TabItem tabItem = e.AddedItems[0] as TabItem;
           if (!(((TabControl)tabItem.Parent).Name == "ReceptionSubTab" || ((TabControl)tabItem.Parent).Name == "InventorySubTab"))
               return;
            Uri url = null;
           switch (tabItem.Name)
           {
               case "AwaitingReceptionTabItem":
                   url = new Uri("/views/CommodityReception/AwaitingReception.xaml", UriKind.Relative);
                   break;
               case "AwaitingStorageTabItem":
                   url = new Uri("/views/CommodityReception/AwaitingStorage.xaml", UriKind.Relative);
                   break;
               case "CompletedAndStoredTabItem":
                   url = new Uri("/views/CommodityReception/CompletedAndStored.xaml", UriKind.Relative);
                   break;
               case "IntraTransferTabItem":
                   url = new Uri("/views/InventoryTransfer/IntraStoreTransfer.xaml", UriKind.Relative);
                   break;
               case "hqTransferTabItem":
                   url = new Uri("/views/InventoryTransfer/ToHqInventoryTransfer.xaml", UriKind.Relative);
                   break;
               case "InventoryLevelTabItem":
                   url = new Uri("/views/InventoryTransfer/InventoryLevel.xaml", UriKind.Relative);
                   break;
               case "CommodityReleaseTabItem":
                   url = new Uri("/views/InventoryTransfer/CommodityRelease.xaml", UriKind.Relative);
                   break;
              
           }
           
           Messenger.Default.Send<Uri>(url, "NavigationRequest");
        }

        private void tbCommodityInventory_Selected(object sender, RoutedEventArgs e)
        {
            Uri url = new Uri("/views/InventoryTransfer/InventoryLevel.xaml", UriKind.Relative);
            Messenger.Default.Send<Uri>(url, "NavigationRequest");   
        }
    }
}
