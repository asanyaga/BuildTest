using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using Distributr_Middleware.WPF.Lib.MiddlewareServices;
using Distributr_Middleware.WPF.Lib.Utils;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;

namespace Distributr_Middleware.WPF.Lib.ViewModels
{
   public class MiddlewareMainWindowViewModel:MiddleWareViewModelBase
   {
       public MiddlewareMainWindowViewModel()
       {
           MasterDataFilepath = FileUtility.GetFilePath("masterdataimportpath");
       }
       private RelayCommand<MenuItem> _loadmenuItem;
       public RelayCommand<MenuItem> LoadMenuItemCommand
       {
           get { return _loadmenuItem ?? (new RelayCommand<MenuItem>(LoadMenuItem)); }
       }

       

       protected  virtual void LoadMenuItem(MenuItem item)
       {
           string itemname = item.Name;
           if(string.IsNullOrEmpty(itemname))return;
           string url = "";
           switch (itemname.ToLower())
           {
               case "masterdata":
                   url = @"views/MasterData.xaml";
                   break;
               case "settings":
                   url = @"views/MasterData.xaml";
                   break;
               case "transactions":
                   url = @"views/Transactions.xaml";
                   break;
               case "inventory":
                   ExportInventory();
                   break;
               case "Sync":
                   url = @"views/sync.xaml";
                   break;
             
           }
           if(string.IsNullOrEmpty(url))return;
           NavigateCommand.Execute(url);
       }

       private async void ExportInventory()
       {
           await Task.Run(() =>
                              {
                                  var vm = SimpleIoc.Default.GetInstance<InventoryTransferViewModel>();
                                  if (vm != null)
                                  {
                                      vm.UploadFiles();
                                  }
                              });

       }

       
 
     
       private void ExitApplication()
       {
           ExitApplicationEventHandler(this, null);
       }
       public EventHandler ExitApplicationEventHandler = (s, e) => { };

       
    }
}
