using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Utility;
using Distributr.WSAPI.Lib.Integrations;
using Distributr_Middleware.WPF.Lib.MiddlewareServices;
using Distributr_Middleware.WPF.Lib.Utils;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;

namespace Distributr_Middleware.WPF.Lib.ViewModels
{
   public class InventoryTransferViewModel:MiddleWareViewModelBase
    {
       private Dictionary<string, List<InventoryLineItemDto>> StocksPerSalesman { get; set; }
       private List<FileInfo> Files; 
       private  InventoryTransferDTO dto;

       public InventoryTransferViewModel()
       {
           dto=new InventoryTransferDTO();
           StocksPerSalesman = new Dictionary<string, List<InventoryLineItemDto>>();
           Files = new List<FileInfo>();
           SetUp();
       }
       private void SetUp()
       {
           MasterDataFilepath = FileUtility.GetFilePath("stocklinepath");
           LoadDistributorInventoryFiles();
           FileUtility.CreateImportedStockLineFile();
       }
     private void LoadDistributorInventoryFiles()
       {
           if (string.IsNullOrEmpty(MasterDataFilepath))
           {
               MasterDataFilepath = FileUtility.GetFilePath("stocklinepath");
           }
           Files = FileUtility.GetStockLines(new DirectoryInfo(MasterDataFilepath)).Distinct().ToList();
           Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + "Generating import files");

           foreach (var fileInfo in Files)
           {
              //format=>stocline-distributorcode
               var highenSeparator = new[] { '-' };
               var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName);
               if (fileNameWithoutExtension != null)
               {
                   Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format(" Extracting file =>{0}", fileNameWithoutExtension));
                   var temp = fileNameWithoutExtension.Split(highenSeparator, StringSplitOptions.RemoveEmptyEntries).ToList();
                  
                   //only distributor
                   if (!string.IsNullOrEmpty(temp.ElementAtOrDefault(1)))
                   {
                       dto.DistributorCode = temp.ElementAt(1);
                   }
               }
              using(var c =NestedContainer)
               {
                   var inventory = Using<IInventoryTransferService>(c).Import(fileInfo.FullName);

                   if (inventory != null && inventory.Any())
                   {
                       var grouped = inventory.GroupBy(p => p.SalesmanCode);
                       var dicties = new Dictionary<string, List<InventoryLineItemDto>>();
                       foreach (var group in grouped)
                       {
                           var listofLineItems = new List<InventoryLineItemDto>();
                           foreach (var itemModel in group)
                           {
                              var qntity = 1m;
                               try
                               {
                                   qntity = Convert.ToDecimal(itemModel.Quantity);
                               }catch
                               {
                                   qntity = 1m;
                               }
                               listofLineItems.Add(new InventoryLineItemDto()
                                                       {
                                                           ProductCode = itemModel.ProductCode,
                                                           Quantity = qntity

                                                       });

                           }
                           if(group.Key=="default")
                               dto.DistributorInventory.AddRange(listofLineItems);
                           else
                           {
                               dicties.Add(group.Key, listofLineItems);
                               dto.SalesmanInventoryList.Add(dicties);
                           }
                           
                       }
                   }
                 
                       
               }
               
              
           }
       }

     public async void UploadFiles()
     {
         if ((dto.DistributorInventory == null || !dto.DistributorInventory.Any()) &&
             (dto.SalesmanInventoryList == null || !dto.SalesmanInventoryList.Any()))
             return;
         using (var c = NestedContainer)
         {
             var response = await Using<IInventoryTransferService>(c).UploadInventory(dto);
             if(response.Result=="Error")
             {
                 MessageBox.Show(response.ResultInfo, "Integration Response", MessageBoxButton.OK, MessageBoxImage.Error);
             }else if(response.Result=="Success")
             {
                 await DampExported(Files.Select(n => n.FullName).ToList());
             }
         }
     }
     Task DampExported(List<string> files)
     {
         return Task.Run(() =>
         {
             try
             {
                 var basefile = FileUtility.CreateImportedStockLineFile().FullName;
                 foreach (string file in files.Where(File.Exists))
                 {
                     string destPath = Path.Combine(basefile, Path.GetFileName(file));
                     if (File.Exists(destPath))
                         File.Delete(destPath);
                     File.Move(file, destPath);
                 }

             }
             catch
             {
             }
         });

     }


    }
}
