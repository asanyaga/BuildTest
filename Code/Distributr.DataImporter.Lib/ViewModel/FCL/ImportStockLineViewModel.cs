using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using Distributr.DataImporter.Lib.ImportEntity;
using Distributr.DataImporter.Lib.ImportService.Orders;
using Distributr.DataImporter.Lib.Utils;
using Distributr.Core.Utility;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.DataImporter.Lib.ViewModel.FCL
{
    public class ImportStockLineViewModel : MasterdataImportViewModelBase
    {
      internal IEnumerable<ImportInvetoryIssueToSalesman> ImportItems;
      private Dictionary<string, IEnumerable<ImportInvetoryIssueToSalesman>> StocksPerSalesman { get; set; }
      private Dictionary<string, List<string>> SalesmanStocklines { get; set; }
    
      private string Filematch { get; set; }

      public ImportStockLineViewModel()
      {
          StocksPerSalesman=new Dictionary<string, IEnumerable<ImportInvetoryIssueToSalesman>>();
          SalesmanStocklines=new Dictionary<string, List<string>>();
      }
      public void ReceiveMessage(string msg)
      {
          ExportActivityMessage += "\n" + msg;
          FileUtility.LogInventoryissue(msg);

      }

      
      protected override void Load(Page page)
      {
          SetUp();
         
          LoadItems();
      }
      

      protected async void LoadItems()
      {
          await Task.Run(async () =>
                                   {
                                       try
                                       {
                                           using (var c = NestedContainer)
                                           {
                                               var importService = Using<IInvetoryIssueToSalesmanImportService>(c);
                                               try
                                               {
                                                   base.BeginCommandsUpload();
                                                   foreach (var salesman in SalesmanStocklines.OrderBy(p => p.Key))
                                                   {
                                                       var imports =
                                                           await importService.ImportAsync(salesman.Value.ToArray());
                                                       if (imports == null || !imports.Any()) continue;
                                                       StocksPerSalesman.Add(salesman.Key, imports);
                                                       var errors =
                                                           await importService.IssueInventoryAsync(StocksPerSalesman);
                                                       Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                                                              string.Format(
                                                                                  "Moving files =>{0} to importedstock",
                                                                                  salesman.Key));
                                                       if (!errors.Any())
                                                           importService.DampExported(salesman.Value.ToList());
                                                       StocksPerSalesman.Clear();
                                                   }
                                                   base.BeginCommandsUpload();
                                                   Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") +
                                                                          " Task completed...");

                                               }
                                               catch (Exception ex)
                                               {
                                                   Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("Error: {0}", ex.Message));
                                               }
                                           }
                                       }catch(Exception ex)
                                       {
                                           
                                       }
                                      

                                   });
      }

      private void GetSalesmanFiles()
      {
          if(string.IsNullOrEmpty(Filepath))
          {
              Filepath = FileUtility.GetFilePath("stocklinepath");
          }
          var files = FileUtility.GetStockLines(new DirectoryInfo(Filepath)).Distinct().ToList();
          Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + "Generating import files");
          int active = 0;
          foreach (var fileInfo in files)
          {
              var currentKey = GetSalesmanCode(fileInfo);
              if(!string.IsNullOrEmpty(currentKey))
              {
                  var filePath = Path.GetFullPath(fileInfo.FullName);
                  SalesmanStocklines.AddToList(currentKey,filePath);
                  active++;
              }
              else
              {
                  Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("  Invalid file,salesman code cannot be extracted from file =>{0}", Path.GetFileNameWithoutExtension(fileInfo.FullName))); 
                  
              }
             
          }
          Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format(" {0} files detected matches date=>{1}", active,Filematch)); 
      }
      
     
    string GetSalesmanCode(FileInfo file)
    {
        
        //format=>stocline-salesmancode-orderdate-orderscount
        var highenSeparator = new[] { '-' };
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FullName);
        if (fileNameWithoutExtension != null)
        {
            Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format(" Extracting file =>{0}", fileNameWithoutExtension)); 
            var temp = fileNameWithoutExtension.Split(highenSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (temp.Length > 3 && !string.IsNullOrEmpty(temp[2]) && temp[2].Equals(Filematch))
                return temp[1];
            else
            {
                Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("  Invalid file =>{0}", fileNameWithoutExtension)); 
                
            }
        }
        return string.Empty;
    }
     

      private void SetUp()
      {
          Filepath = FileUtility.GetFilePath("stocklinepath");
          DateTime dat = DateTime.Now.Date;
          ExportActivityMessage = string.Empty;
          Filematch = string.Format("{0}{1}{2}", dat.Year, dat.Month.ToString("00"), dat.Day.ToString("00"));
          
          GetSalesmanFiles();
          FileUtility.CreateImportedStockLineFile();
      }
    }
}
