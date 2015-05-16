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

namespace Agrimanagr.DataImporter.Lib.ViewModels
{
    public class CommodityImportViewModel : ImporterListingsViewModelBase
    {
        public CommodityImportViewModel()
        {
            CommodityImportVmList = new ObservableCollection<CommodityImportVM>();
        }

        protected override void Load(bool isFirstLoad = false)
        {
            if (isFirstLoad)
                Setup();
            LoadCommodities();
        }

        private async void LoadCommodities()
        {
            using (var c = NestedContainer)
            {
                if (string.IsNullOrEmpty(SelectedPath) || !File.Exists(SelectedPath))
                    SelectedPath = FileUtility.OpenImportDirectoryPath();

                var importService = Using<ICsvHandlerService>(c);
                try
                {
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

        private void Map(IEnumerable<MasterImportEntity> importItems)
        {
            if (!importItems.Any()) return;
            if (!Positions.Any())
            using (var c = NestedContainer)
            {
                Positions = Using<IMapEntityColumnPosition>(c).GetEntityMapping(new ImportEntity(){EntityName = "Commodity"});
            }
           var paged= importItems.Select((row, index) => new CommodityImportVM()
                                                   {
                                                       Code = GetColumn(row, GetIndex("code")),
                                                       Name = GetColumn(row, GetIndex("name")),
                                                       Description = GetColumn(row, GetIndex("description")),
                                                       CommodityTypeCode =GetColumn(row, GetIndex("commoditytypecode")),
                                                       IsChecked = false,
                                                       SequenceNo = index + 1
                                                   }).AsQueryable();
           PagedList = new PagenatedList<ImportItemVM>(paged, CurrentPage, ItemsPerPage, paged.Count());
            CommodityImportVmList.Clear();
            PagedList.ToList().ForEach(n => CommodityImportVmList.Add((CommodityImportVM)n));
        }

        private void Setup()
        {
            CommodityImportVmList.Clear();
            UploadStatusMessage = "";
            SelectedPath = Path.Combine(MainViewModel.Filepath, @"commodity.csv");
            PageTitle = "Commodities";
            MainViewModel.ProsessMessage = string.Empty;

        }
        protected override async void UploadAll()
        {
            if (CommodityImportVmList.Any())
            {
                var items = CommodityImportVmList.Select(n => new CommodityImport()
                {
                    Code = n.Code,
                    Description = n.Description,
                    Name = n.Name,
                    CommodityTypeCode = n.CommodityTypeCode
                }).ToList();
                Import(items);
            }
        }
        protected override async void UploadSelected()
        {
            var selected = CommodityImportVmList.Where(o => o.IsChecked).ToList();
            if (selected.Any())
            {
                var items = selected.Select(n => new CommodityImport()
                {
                    Code = n.Code,
                    Description = n.Description,
                    Name = n.Name,
                    CommodityTypeCode = n.CommodityTypeCode
                }).ToList();
                Import(items);
            }
        }

        async void Import(List<CommodityImport> commodityImportItems)
        {
            using (var c = NestedContainer)
            {
                var importService = Using<ICommodityImportService>(c);
                MainViewModel.ProsessMessage = "Validating...Please wait";
                var result = await importService.ValidateAsync(commodityImportItems);
                if (result.All(p => p.IsValid))
                {
                    var items = result.Select(o => o.Entity).OfType<Commodity>().ToList();
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
                                       CommodityImportVmList.FirstOrDefault(
                                           o => o.Code == item.Code);
                                   if (remove != null)
                                       CommodityImportVmList.Remove(remove);
                                   i++;
                               }
                               MainViewModel.ProsessMessage = string.Format("Successfully uploaded {0} Commodities", i);
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

        public ObservableCollection<CommodityImportVM> CommodityImportVmList { get; set; }
    }
    public class CommodityImportVM : ImportItemVM
    {
        public string CommodityTypeCode { get; set; }
    }
}
