using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Agrimanagr.DataImporter.Lib.ImportEntities;
using Agrimanagr.DataImporter.Lib.ImportServices;
using Agrimanagr.DataImporter.Lib.ImportServices.Commodities;
using Agrimanagr.DataImporter.Lib.Utils;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.Util;
using GalaSoft.MvvmLight;

namespace Agrimanagr.DataImporter.Lib.ViewModels
{
   public class ListCommodityTypesViewModel: ImporterListingsViewModelBase
    {
       
       public ListCommodityTypesViewModel()
       {
           CommodityTypeImportVmList=new ObservableCollection<CommodityTypeImportVM>();
       }

       protected override void Load(bool isFirstLoad = false)
       {
           if (isFirstLoad)
               Setup();
           LoadCommodityTypes();
       }

       private async void LoadCommodityTypes()
       {
           using (var c = NestedContainer)
           {
               var importService = Using<ICsvHandlerService>(c);
               try
               {
                   if (string.IsNullOrEmpty(SelectedPath) || !File.Exists(SelectedPath))
                       SelectedPath = FileUtility.OpenImportDirectoryPath();
                   UploadStatusMessage = "Please Wait......!";
                  var importItems = await importService.ReadFromCsVFileAsync(SelectedPath);
                  Map(importItems);
               }
               catch (Exception ex)
               {
                   MessageBox.Show("Error Importing orders\nDetails:" + ex.Message);

               }

           }
       }

       private void Setup()
       {
           CommodityTypeImportVmList.Clear();
           UploadStatusMessage = "";
           SelectedPath =Path.Combine(MainViewModel.Filepath,@"commoditytype.csv");
           PageTitle = "Commodity Types";
           MainViewModel.ProsessMessage = string.Empty;
       }
       void Map(IEnumerable<MasterImportEntity> items)
       {
           if(!items.Any())return;
           if (!Positions.Any())
           using (var c = NestedContainer)
           {
               Positions = Using<IMapEntityColumnPosition>(c).GetEntityMapping(new ImportEntity() { EntityName = "commoditytype" });
           }
           if (!Positions.Any())
           {
               GotoHomePage();
               return;
           }
           var paged = items.Select((row, index) => new CommodityTypeImportVM()
                                                              {
                                                                  Code = GetColumn(row, GetIndex("code")),
                                                                  Name = GetColumn(row, GetIndex("name")),
                                                                  Description = GetColumn(row, GetIndex("description")),
                                                                  IsChecked = false,
                                                                  SequenceNo = index + 1
                                                              }).AsQueryable();
           PagedList = new PagenatedList<ImportItemVM>(paged, CurrentPage, ItemsPerPage, paged.Count());
           CommodityTypeImportVmList.Clear();
           PagedList.ToList().ForEach(n => CommodityTypeImportVmList.Add((CommodityTypeImportVM)n));
       }

       protected override async void UploadAll()
       {
           if (CommodityTypeImportVmList.Any())
           {
               var items = CommodityTypeImportVmList.Select(n => new CommodityTypeImport()
                                                                     {
                                                                         Code = n.Code,
                                                                         Description = n.Description,
                                                                         Name = n.Name
                                                                     }).ToList();
               Import(items);
           }
       }
      protected override async void UploadSelected()
      {
          var selected = CommodityTypeImportVmList.Where(o => o.IsChecked).ToList();
          if(selected.Any())
          {
              var items = selected.Select(n => new CommodityTypeImport()
              {
                  Code = n.Code,
                  Description = n.Description,
                  Name = n.Name
              }).ToList();
              Import(items);
          }
      }

       internal async void Import(List<CommodityTypeImport> commodityTypeImportItems)
       {
           using (var c = NestedContainer)
           {
               var importService = Using<ICommodityTypeImportService>(c);
               MainViewModel.ProsessMessage = "Validating...Please wait";
               var result = await importService.ValidateAsync(commodityTypeImportItems);
               if (result.All(p => p.IsValid))
               {
                   var items = result.Select(o => o.Entity).OfType<CommodityType>().ToList();
                   MainViewModel.ProsessMessage = "Validation Success..Saving entities";
                    bool isSuccess= await importService.SaveAsync(items);
                    if (isSuccess)
                    {
                        Application.Current.Dispatcher.BeginInvoke(
                           new Action(() =>
                           {
                               int i = 0;
                               foreach (var item in items)
                               {
                                   var remove =
                                       CommodityTypeImportVmList.FirstOrDefault(
                                           o => o.Name == item.Name);
                                   if (remove != null)
                                       CommodityTypeImportVmList.Remove(remove);
                                   i++;
                               }
                               MainViewModel.ProsessMessage = string.Format("Successfully uploaded {0} Commodity Types", i);
                           }));
                    }
               }
               else
               {
                   Application.Current.Dispatcher.BeginInvoke(
                         new Action(
                             delegate
                             {
                                 using (var cont = NestedContainer)
                                 {
                                     Using<IImportValidationPopUp>(cont).ShowPopUp(
                                         result.Where(o => !o.IsValid).ToList());
                                 }
                             }));
               }
           }
       }
       public ObservableCollection<CommodityTypeImportVM> CommodityTypeImportVmList { get; set; }
    }

   public class CommodityTypeImportVM : ImportItemVM
    {
        
    }
}
