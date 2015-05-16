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
    public class CommoditySuppliersListViewModel : ImporterListingsViewModelBase
    {
        public ObservableCollection<CommoditySupplierImportVM> CommoditySupplierImportVMList { get; set; }
        public CommoditySuppliersListViewModel()
        {
            CommoditySupplierImportVMList = new ObservableCollection<CommoditySupplierImportVM>();
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
        void Map(IEnumerable<MasterImportEntity> items)
        {
            if (!items.Any()) return;
            if(!Positions.Any())
            using (var c = NestedContainer)
            {
                Positions = Using<IMapEntityColumnPosition>(c).GetEntityMapping(new ImportEntity() { EntityName = "commoditysupplier" });
            }
            if (!Positions.Any())
            {
                GotoHomePage();
                return;
            }
            
            var paged = items.Select((row, index) => new CommoditySupplierImportVM()
                                                               {
                                                                   Code =GetColumn(row,GetIndex("code")),
                                                                   Name =GetColumn(row,GetIndex("name")),
                                                                   Description =GetColumn(row,GetIndex("description")),
                                                                   AccountNo =GetColumn(row,GetIndex("AccountNo")),
                                                                   PinNo =GetColumn(row,GetIndex("PinNo")),
                                                                   JoinDate =DateTime.Parse(GetColumn(row, GetIndex("JoinDate"),handleDateTime:true)),
                                                                   BankBranchName =GetColumn(row,GetIndex("BankBranchName")),
                                                                   BankName =GetColumn(row,GetIndex("BankName")),
                                                                   CommoditySupplierType =Int32.Parse(GetColumn(row, GetIndex("CommoditySupplierType"),handleEnum:true)),
                                                                   IsChecked = false,
                                                                   SequenceNo = index + 1
                                                               }).AsQueryable();
            PagedList = new PagenatedList<ImportItemVM>(paged, CurrentPage, ItemsPerPage, paged.Count());
            CommoditySupplierImportVMList.Clear();
            PagedList.ToList().ForEach(n => CommoditySupplierImportVMList.Add((CommoditySupplierImportVM)n));
        }

        

        private void Setup()
        {
            CommoditySupplierImportVMList.Clear();
            UploadStatusMessage = "";
            SelectedPath = Path.Combine(MainViewModel.Filepath, @"commoditySupplier.csv");
            PageTitle = "Commodity Suppliers";
            MainViewModel.ProsessMessage = string.Empty;
        }

        protected override async void UploadAll()
        {
            if (CommoditySupplierImportVMList.Any())
            {
                var items = CommoditySupplierImportVMList.Select(n => new CommoditySupplierImport()
                {
                    Code = n.Code,
                    Description = n.Description,
                    Name = n.Name,
                    AccountNo = n.AccountNo,
                    PinNo = n.PinNo,
                    BankBranchName = n.BankBranchName,
                    BankName = n.BankName,
                    CommoditySupplierType = n.CommoditySupplierType,
                    JoinDate = n.JoinDate

                }).ToList();
                await Task.Factory.StartNew(() => Import(items));
            }
        }
        protected override async void UploadSelected()
        {
            var selected = CommoditySupplierImportVMList.Where(o => o.IsChecked).ToList();
            if (selected.Any())
            {
                var items = selected.Select(n => new CommoditySupplierImport()
                {
                    Code = n.Code,
                    Description = n.Description,
                    Name = n.Name,
                    AccountNo = n.AccountNo,
                    PinNo = n.PinNo,
                    BankBranchName = n.BankBranchName,
                    BankName = n.BankName,
                    CommoditySupplierType = n.CommoditySupplierType,
                    JoinDate = n.JoinDate
                }).ToList();
                await Task.Factory.StartNew(() => Import(items));
            }
        }

        internal async void Import(List<CommoditySupplierImport> commodityTypeImportItems)
        {
            using (var c = NestedContainer)
            {
                var importService = Using<ICommoditySupplierImportService>(c);
                MainViewModel.ProsessMessage = "Validating...Please wait";
                var result = await importService.ValidateAsync(commodityTypeImportItems);
                if (result.All(p => p.IsValid))
                {
                    var items = result.Select(o => o.Entity).OfType<CommoditySupplier>().ToList();
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
                                       CommoditySupplierImportVMList.FirstOrDefault(o=>o.Name==item.Name);
                                   if (remove != null)
                                       CommoditySupplierImportVMList.Remove(remove);
                                   i++;
                               }
                               MainViewModel.ProsessMessage = string.Format("Successfully uploaded {0} Commodity Suppliers", i);
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

    public class CommoditySupplierImportVM : ImportItemVM
    {
        public int CommoditySupplierType { get; set; }
        public DateTime JoinDate { get; set; }
        public string AccountNo { get; set; }
        public string PinNo { get; set; }
        public string BankName { get; set; }
        public string BankBranchName { get; set; }
    }
}
