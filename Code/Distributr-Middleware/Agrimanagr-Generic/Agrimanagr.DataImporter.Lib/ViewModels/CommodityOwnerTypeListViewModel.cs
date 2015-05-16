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
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.Util;

namespace Agrimanagr.DataImporter.Lib.ViewModels
{
   public class CommodityOwnerTypeListViewModel:ImporterListingsViewModelBase
    {
       public ObservableCollection<CommodityOwnerTypeImportVM> CommodityOwnerTypeImportVMList { get; set; }
       public CommodityOwnerTypeListViewModel()
       {
           CommodityOwnerTypeImportVMList =new ObservableCollection<CommodityOwnerTypeImportVM>();
       }
       protected override void Load(bool isFirstLoad = false)
       {
           if (isFirstLoad)
               Setup();
           LoadCommodityOwnerTypes();
       }

       private async void LoadCommodityOwnerTypes()
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
           CommodityOwnerTypeImportVMList.Clear();
           UploadStatusMessage = "";
           SelectedPath = Path.Combine(MainViewModel.Filepath, @"commodityownertype.csv");
           PageTitle = "Commodity owner Types";
           MainViewModel.ProsessMessage = string.Empty;

       }
       void Map(IEnumerable<MasterImportEntity> items)
       {
           if (!items.Any()) return;
           if(!Positions.Any())
           using (var c = NestedContainer)
           {
               Positions = Using<IMapEntityColumnPosition>(c).GetEntityMapping(new ImportEntity() { EntityName = "commodityownertype" });
           }
           if (!Positions.Any())
           {  GotoHomePage();
               return;
           }
           var paged = items.Select((row, index) => new CommodityOwnerTypeImportVM()
                                                              {
                                                                  Code = GetColumn(row, GetIndex("code")),
                                                                  Name = GetColumn(row, GetIndex("name")),
                                                                  Description = GetColumn(row, GetIndex("description")),
                                                                  IsChecked = false,
                                                                  SequenceNo = index + 1
                                                              }).AsQueryable();

           PagedList = new PagenatedList<ImportItemVM>(paged, CurrentPage, ItemsPerPage, paged.Count());
           CommodityOwnerTypeImportVMList.Clear();
           PagedList.ToList().ForEach(n => CommodityOwnerTypeImportVMList.Add((CommodityOwnerTypeImportVM)n));
       }

       protected override async void UploadAll()
       {
           if (CommodityOwnerTypeImportVMList.Any())
           {
               var items = CommodityOwnerTypeImportVMList.Select(n => new CommodityOwnerTypeImport()
               {
                   Code = n.Code,
                   Description = n.Description,
                   Name = n.Name
               }).ToList();
               await Task.Factory.StartNew(() => Import(items));
           }
       }
       protected override async void UploadSelected()
       {
           var selected = CommodityOwnerTypeImportVMList.Where(o => o.IsChecked).ToList();
           if (selected.Any())
           {
               var items = selected.Select(n => new CommodityOwnerTypeImport()
               {
                   Code = n.Code,
                   Description = n.Description,
                   Name = n.Name
               }).ToList();
              await Task.Factory.StartNew(() => Import(items)); 
           }
       }

       internal async void Import(List<CommodityOwnerTypeImport> commodityTypeImportItems)
       {
           using (var c = NestedContainer)
           {
               var importService = Using<ICommodityOwnerTypeImportService>(c);
               MainViewModel.ProsessMessage = "Validating...Please wait";
               var result = await importService.ValidateAsync(commodityTypeImportItems);
               if (result.All(p => p.IsValid))
               {
                   var items = result.Select(o => o.Entity).OfType<CommodityOwnerType>().ToList();
                   MainViewModel.ProsessMessage = "Validation Success..Saving entities";
                   bool isSuccess = await importService.SaveAsync(items);
                   if (isSuccess)
                   {
                       Application.Current.Dispatcher.BeginInvoke(
                           new Action(() =>
                                          {
                                              int i = 0;
                                              foreach (var item in items)
                                              {
                                                  var remove =
                                                      CommodityOwnerTypeImportVMList.FirstOrDefault(
                                                          o => o.Code == item.Code);
                                                  if (remove != null)
                                                      CommodityOwnerTypeImportVMList.Remove(remove);
                                                  i++;
                                              }
                                              MainViewModel.ProsessMessage =
                                                  string.Format("Successfully uploaded {0} Commodity Owner Types", i);
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

    }
   public class CommodityOwnerTypeImportVM : ImportItemVM
   {

   }
}
