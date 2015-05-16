using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr_Middleware.WPF.Lib.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using SAPUtilityLib.Masterdata.Impl;

namespace SAPUtilityLib.ViewModels
{
    public class MainWindowVM : MiddlewareMainWindowViewModel
    {

        protected override async void LoadMenuItem(MenuItem item)
        {
            string itemname = item.Name;
            if (string.IsNullOrEmpty(itemname)) return;

            if (itemname == "sap")
            {
                var service = new PullMasterdataService();
                bool fileImported = true;
                foreach (var masterdata in GenerateMasterDataList)
                {
                    if (fileImported)
                        fileImported = service.Import(masterdata).Status;
                }
                if (fileImported)
                    Messenger.Default.Send("Ready to import");
                MessageBox.Show(fileImported ? "Done...!" : "Completed with errors...see logs for details!");
            }
            if (itemname == "ordes")
            {
               var done= await new ExportTransactionsService().ExportToSap(OrderType.OutletToDistributor);

            }
            if (itemname == "sales")
            {
                await new ExportTransactionsService().ExportToSap(OrderType.DistributorPOS);

            }
            if (itemname == "inventory")
            {
                new ExportTransactionsService().PullInventory();
            }
            if (itemname == "settings")
            {
                Navigate(@"/views/settings.xaml");
                
            }
            if (itemname == "Sync")
            {
                Navigate(@"/views/sync.xaml");

            }
        }

        private IEnumerable<string> GenerateMasterDataList
        {
            get
            {
                return new List<string>
                           {
                               MasterDataCollective.VatClass.ToString(),
                               MasterDataCollective.Outlet.ToString(),
                               MasterDataCollective.SaleProduct.ToString(),
                               MasterDataCollective.PricingTier.ToString(),
                               MasterDataCollective.Supplier.ToString(),
                               MasterDataCollective.Pricing.ToString(),
                               MasterDataCollective.Route.ToString(),
                               MasterDataCollective.Bank.ToString(),
                               MasterDataCollective.Region.ToString(),
                               MasterDataCollective.Distributor.ToString(),
                               MasterDataCollective.Country.ToString(),
                               MasterDataCollective.DistributorSalesman.ToString(),
                               MasterDataCollective.ProductBrand.ToString(),
                               "shipto"
                           };


            }
        } 
      
    }
    
}
