using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Master.Util;
using Integration.Cussons.WPF.Lib.ImportEntities;
using Integration.Cussons.WPF.Lib.ImportService;
using Integration.Cussons.WPF.Lib.ImportService.DistributrSalesmen;
using Integration.Cussons.WPF.Lib.Utils;

namespace Integration.Cussons.WPF.Lib.CussonsIntegrationViewModels.MasterData
{
    public class DistributrsalesmanImportListViewModel : MasterDataImportListingsBase
    {
        internal IEnumerable<DistributorSalesmanImport> ImportItems;
        public ObservableCollection<DistributrsalesmanImportVM> ImportVmList { get; set; }

        public DistributrsalesmanImportListViewModel()
        {
            ImportVmList=new ObservableCollection<DistributrsalesmanImportVM>();
        }

        protected override void Load(bool isFirstLoad = false)
        {
            if (isFirstLoad)
                Setup();
            LoadImportsFromFile();
        }

        private async void LoadImportsFromFile()
        {
            if (FileUtility.ValidateFile(SelectedPath))
            {
                using (var c = NestedContainer)
                {

                    try
                    {
                        ImportVmList.Clear();
                        ImportItems = await Using<IDistributorSalesmanImportService>(c).Import(SelectedPath);

                        var productImports = ImportItems as List<DistributorSalesmanImport> ?? ImportItems.ToList();
                        if (productImports.Any())
                        {
                            var items = productImports.Select((n, i) => new DistributrsalesmanImportVM()
                            {
                                SequenceNo = i + 1,
                                Code = n.Code,
                                Description = n.Description,
                                Name = n.Name,
                                IsChecked = false
                            }).AsQueryable();

                            PagedList = new PagenatedList<ImportItemVM>(items, CurrentPage, ItemsPerPage, items.Count());


                            PagedList.ToList().ForEach(n => ImportVmList.Add((DistributrsalesmanImportVM)n));
                            UpdatePagenationControl();

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error getting data from file\nDetails=>" + ex.Message);

                    }
                }
            }

        }
        protected override async void UploadAll()
        {
            if (ImportVmList.Any())
            {
                var items = ImportItems.Select(n => new DistributorSalesmanImport()
                {
                    Code = n.Code,
                    Description = n.Description,
                    Name = n.Name,
                }).ToList();
                Import(items);
            }
            else
            {
                MessageBox.Show("There is nothing to import");
            }
        }
        protected override async void UploadSelected()
        {
            var selected = ImportVmList.Where(o => o.IsChecked).ToList();
            if (selected.Any())
            {
                var items = selected.Select(n => new DistributorSalesmanImport()
                {
                    Code = n.Code,
                    Description = n.Description,
                    Name = n.Name,
                }).ToList();
                Import(items);
            }
            else
            {
                MessageBox.Show("There is nothing to import");
            }
        }
        internal async void Import(List<DistributorSalesmanImport> importItems)
        {
            using (var c = NestedContainer)
            {
                var importService = Using<IDistributorSalesmanImportService>(c);
                MainViewModel.GlobalStatus = "Validating...Please wait";
                using (var trans = new TransactionScope(TransactionScopeOption.Required))
                {
                    var result = await importService.ValidateAsync(importItems);
                    if (result.All(p => p.IsValid))
                    {
                        MainViewModel.GlobalStatus = "Validating...Users...Please wait";


                        var items = result.Select(o => o.Entity).OfType<DistributorSalesman>().ToList();
                        MainViewModel.GlobalStatus = "Validation Success..Saving entities";
                        bool isSuccess = await importService.SaveAsync(items);

                        var usersResult = await importService.ValidateUsers(importItems);
                        if (usersResult.All(p => p.IsValid))
                        {
                            isSuccess =
                                await importService.SaveAsync(usersResult.Select(p => p.Entity).OfType<User>().ToList());
                        }
                        else
                        {
                            ShowValidationErrors(usersResult);
                            return;
                        }
                        if (isSuccess)
                        {
                            Application.Current.Dispatcher.BeginInvoke(
                                new Action(() =>
                                               {
                                                   int i = 0;
                                                   foreach (var item in items)
                                                   {
                                                       var remove =
                                                           ImportVmList.FirstOrDefault(
                                                               o => o.Name == item.Name);
                                                       if (remove != null)
                                                           ImportVmList.Remove(remove);
                                                       i++;
                                                   }
                                                   MainViewModel.GlobalStatus =
                                                       string.Format("Successfully uploaded {0} Users", i);
                                               }));

                        }
                    }
                    else
                    {
                        ShowValidationErrors(result);
                    }   
                    trans.Complete();
                   try
                   {
                       trans.Dispose();
                   }catch
                   {
                   }

                }
            }
        }

        private void ShowValidationErrors(IEnumerable<ImportValidationResultInfo> result)
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

        private void Setup()
        {
            ImportVmList.Clear();
            UploadStatusMessage = "";
            SelectedPath = string.IsNullOrEmpty(MainViewModel.Filepath) ? "" : Path.Combine(MainViewModel.Filepath, @"users.txt");
            if (string.IsNullOrEmpty(SelectedPath) && System.Diagnostics.Debugger.IsAttached)
                SelectedPath = Path.Combine(FileUtility.GetDefaultDirectory(), @"users.txt");
            PageTitle = "Distributr Salesmen";
            MainViewModel.GlobalStatus = string.Empty;
        }
    }

    public class DistributrsalesmanImportVM : ImportItemVM
    {
    }
}
