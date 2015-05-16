using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Distributr.DataImporter.Lib.Experimental.Sync;
using Distributr.DataImporter.Lib.Utils;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Distributr.DataImporter.Lib.ViewModel.FCL
{
   public class FclMainWindowViewModel:MasterdataImportViewModelBase
    {
        public RelayCommand ChangeWorkingFolderCommand { get; set; }
        public RelayCommand ClosePopUpCommand { get; set; }
       public FclMainWindowViewModel()
       {
           FclImportCommand=new RelayCommand<MenuItem>(Import);
           ChangeWorkingFolderCommand = new RelayCommand(LoadSettings);
           ClosePopUpCommand = new RelayCommand(Cancel);
           GlobalStatus = "Idle";
           ImportStatusMessage = "Idle";
           Filepath = FileUtility.GetWorkingDirectory("importpath");
       }

       
       #region properties
      
       public RelayCommand<MenuItem> FclImportCommand { get; set; }
       public event EventHandler ExitApplicationEventHandler = (s, e) => { };
       private RelayCommand _exitCommand;
       public RelayCommand ExitCommand
       {
           get { return _exitCommand ?? (_exitCommand = new RelayCommand(ExitApplication)); }
       }
       private RelayCommand<MenuItem> _syncMasterDataCommand;
       public RelayCommand<MenuItem> SyncMasterDataCommand
       {
           get { return _syncMasterDataCommand ?? (_syncMasterDataCommand = new RelayCommand<MenuItem>(BeginInitialSync)); }
       }

       
       
      
       private RelayCommand _aboutCommand;
       public RelayCommand AboutCommand
       {
           get { return _aboutCommand ?? (_aboutCommand = new RelayCommand(ShowAbout)); }
       }

       private RelayCommand<MenuItem> _exportCommand;
       public RelayCommand<MenuItem> ExportCommand
       {
           get { return _exportCommand ?? (_exportCommand = new RelayCommand<MenuItem>(Export)); }
       }
       #endregion

       private void Import(MenuItem menuItem)
       {
           if (!string.IsNullOrEmpty(menuItem.Name))
           {
               String url = "";
             
               switch (menuItem.Name)
               {
                   case "products":
                       url = "/views/ListProducts.xaml";
                       break;
                   case "salesmen":
                       url = "/views/ListDistributorSalesmen.xaml";
                       break;
                   case "outlets":
                       url = "/views/ListOutlets.xaml";
                       break;
                   case "shiptoAddress":
                       url = "/views/ListShippingAddresses.xaml";
                       break;
                   case "productPricing":
                       url = "/views/ListProductPricing.xaml";
                       break;
                   case "discountGroup":
                       url = "/views/ListDiscountGroups.xaml";
                       break;
                   case "stockline":
                       url = "/views/ListImportOrders.xaml";
                       break;
                 
                 
               }
               NavigateCommand.Execute(url);
           }
       }

       private void Export(MenuItem menuItem)
       {
           if (!string.IsNullOrEmpty(menuItem.Name))
           {
               String url = "";
               switch (menuItem.Name)
               {
                   case "ordersexport":
                       url = "/views/ListExportOrders.xaml";
                       break;
               }
               NavigateCommand.Execute(url);
           }
           
       }
       private void ExitApplication()
       {
           ExitApplicationEventHandler(this, null);
       }
       private void ShowAbout()
       {
           throw new NotImplementedException();
       }
       private void BeginInitialSync(MenuItem menuItem)
       {
           try
           {
               if(!string.IsNullOrEmpty(menuItem.Name))
               {
                   var name = menuItem.Name.ToLower();
                   long result = 0;
                   switch (name)
                   {
                       case "productgroupdiscount":
                           result = ObjectFactory.GetInstance<ISyncProductGroupDiscount>().UpdateLocal();
                           break;
                       case "pricing":
                           result = ObjectFactory.GetInstance<ISyncProductPricing>().UpdateLocalDb();
                           break;
                   }
                   MessageBox.Show(string.Format("Process completed after:{0} minutes",(result*0.001)/60));
               }
               
              
           }
           catch (Exception ex)
           {

               MessageBox.Show("Error updating local database,Details\n" + ex.Message);
           }
           
       }
    }
}
