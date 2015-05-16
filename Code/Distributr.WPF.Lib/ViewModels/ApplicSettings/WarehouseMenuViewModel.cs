using System.Windows.Documents;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.ApplicSettings
{
    public class WarehouseMenuViewModel : DistributrViewModelBase
    {
        public RelayCommand<string> MenuCommand { get; set; }

        public WarehouseMenuViewModel()
        {
            MenuCommand = new RelayCommand<string>(Load);
        }

        private void Load(string param)
        {
            string url;
            switch (param)
            {

                case "AddNew":
                    url = @"/views/warehousing/AddWarehouseEntryFormPage.xaml";
                    break;
                case "EntryListing":
                    url = @"/views/warehousing/WarehouseEntryListingPage.xaml";
                    break;
                case "ExitListing":
                    url = @"/views/warehousing/WarehouseExitListingPage.xaml";
                    break;
                case "PendingStorage":
                    url = @"/views/warehousing/WarehousePendingStorageListingPage.xaml";
                    break;
                case "InventoryLevels":
                    url = @"/views/warehousing/WarehouseInventoryLevelsListingPage.xaml";
                    break;
                case "AddDepositor":
                    url = @"/views/warehousing/WarehousedepositorFormPage.xaml";
                    break;
                case "AddReceipt":
                    url = @"/views/warehousing/WarehouseReceiptFormPage.xaml";
                    break;
                case "ListDepositors":
                    url = @"/views/warehousing/WarehouseListDepositorsPage.xaml";
                    break;
                case "ListReceipts":
                    url = @"/views/warehousing/WarehouseListReceiptsPage.xaml";
                    break;

                default:
                    url = @"/views/warehousing/WarehouseEntryListingPage.xaml";
                    break;
            }
            NavigateCommand.Execute(url);
        }
    }
}
