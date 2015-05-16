using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr_Middleware.WPF.Lib.ViewModels;
using GalaSoft.MvvmLight.Command;
using Integration.QuickBooks.Lib.QBIntegrationCore;

namespace Integration.QuickBooks.Lib.QBIntegrationViewModels
{
    public class VmImportMasterdata : ImportMasterDataViewModel
    {


        private void Close()
        {
            var confirm = MessageBox.Show("Are you sure you want to navigate away from masterdata page?","Warning",MessageBoxButton.YesNo,MessageBoxImage.Warning);
            if(confirm==MessageBoxResult.No)return;
            Navigate(@"/Views/ListSalesPage.xaml");
        }


        protected override void Sync()
        {
           var canConnectToQuickBooks = QBFC_Core.CanConnect();
             if(!canConnectToQuickBooks)
             {
                 Alert("Cannot connect to quickbooks company file");
                return; 
             }

             try
             {
                 QBFC_Core.GenerateMasterData();
                 Alert("Done..!");
             }catch(Exception ex)
             {
                 Alert("Quickbooks has thrown an error \n" + ex.Message);
               
             }
            
             
        }

        private void UpdateInventory()
        {
            var canConnectToQuickBooks = QBFC_Core.CanConnect();
            if (!canConnectToQuickBooks)
            {
                Alert("Cannot connect to quickbooks company file");
                return;
            }
            try
            {
                QBFC_Core.PullInventory();
                Alert("Inventory Updated!");
            }
            catch (Exception ex)
            {
                Alert("Quickbooks has thrown an error \n" + ex.Message);

            }
        }
        void Alert(string msg)
        {
            MessageBox.Show(msg, "Alert");
        }


        private RelayCommand _closeCommand;
        public RelayCommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new RelayCommand(Close)); }
        }

        private RelayCommand _updateInventoryCommand;
        public RelayCommand UpdateInventoryCommand
        {
            get { return _updateInventoryCommand ?? (new RelayCommand(UpdateInventory)); }
        }
    }
}
