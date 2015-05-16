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
using Distributr.Core.Utility;
using Integration.Cussons.WPF.Lib.ImportEntities;
using Integration.Cussons.WPF.Lib.ImportService;
using Integration.Cussons.WPF.Lib.ImportService.CostCentres;
using Integration.Cussons.WPF.Lib.MasterDataImportService;
using Integration.Cussons.WPF.Lib.Utils;

namespace Integration.Cussons.WPF.Lib.CussonsIntegrationViewModels.MasterData
{
    public class OutletImportsListViewModel : MasterDataImportListingsBase
    {
        internal IEnumerable<OutletImport> ImportItems;
        public ObservableCollection<OutletImportVm> ImportVmList { get; set; }

        public OutletImportsListViewModel()
        {
            ImportVmList=new ObservableCollection<OutletImportVm>();
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
                        ImportItems = await Using<IOutletImportService>(c).Import(SelectedPath);

                        var productImports = ImportItems as List<OutletImport> ?? ImportItems.ToList();
                        if (productImports.Any())
                        {
                            var items = productImports.Select((n, i) => new OutletImportVm()
                            {
                                SequenceNo = i + 1,
                                Code = n.OutletCode,
                                Name = n.Name,
                                PinNo = n.PinNo,
                                PostalAddress = n.PostalAddress,
                                PhysicalAddress = n.PhysicalAddress,
                                Status = n.Status,
                                Tax = n.Tax,
                                Currency =n.Currency,
                                SalesmanCode = n.SalesmanCode,
                                RouteName = n.RouteName,
                                IsChecked = false
                            }).AsQueryable();

                            PagedList = new PagenatedList<ImportItemVM>(items, CurrentPage, ItemsPerPage, items.Count());


                            PagedList.ToList().ForEach(n => ImportVmList.Add((OutletImportVm)n));
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
                var items = ImportItems.Select(n => new OutletImport()
                {
                    OutletCode = n.OutletCode,
                    Name = n.Name,
                    PinNo = n.PinNo,
                    PostalAddress = n.PostalAddress,
                    PhysicalAddress = n.PhysicalAddress,
                    Status = n.Status,
                    Tax = n.Tax,
                    Currency = n.Currency,
                    SalesmanCode = n.SalesmanCode,
                    RouteName = n.RouteName,
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
                var items = selected.Select(n => new OutletImport()
                {
                    OutletCode = n.Code,
                    Name = n.Name,
                    PinNo = n.PinNo,
                    PostalAddress = n.PostalAddress,
                    PhysicalAddress = n.PhysicalAddress,
                    Status = n.Status,
                    Tax = n.Tax,
                    Currency = n.Currency,
                    SalesmanCode = n.SalesmanCode,
                    RouteName = n.RouteName,
                }).ToList();
                Import(items);
            }
            else
            {
                MessageBox.Show("There is nothing to import");
            }
        }
        internal async void Import(List<OutletImport> importItems)
        {
            using (var c = NestedContainer)
            {
                var importService = Using<IOutletImportService>(c);
                IList<ImportValidationResultInfo> result = new List<ImportValidationResultInfo>();
                MainViewModel.GlobalStatus = "Validation started...Please wait";

                int batchSize = 100;
                int processed = 0;
                var outlets = importItems.OrderBy(p => p.OutletCode).Batch(batchSize).Select(x => x.ToList()).ToList();
                while (outlets.Any())
                {
                    var current = outlets.FirstOrDefault();
                    if (current != null)
                    {
                        IList<ImportValidationResultInfo> tempresult = await importService.ValidateAsync(current);
                        tempresult.ToList().ForEach(result.Add);
                       outlets.Remove(current);
                        processed += current.Count;
                        MainViewModel.GlobalStatus = string.Format("{0} of {1} validation Completed...Please wait",
                                                                   processed, importItems.Count);
                    }
                }
                 outlets.Clear();
                 batchSize = 100;
                 processed = 0;
                if (result.All(p => p.IsValid))
                    {
                      var items = result.Select(o => o.Entity).OfType<Outlet>().ToList();
                        MainViewModel.GlobalStatus = "Validation Success..Saving entities";
                        
                        var tempresult = new List<string>();
                        var savedoutlets = items.Batch(batchSize).Select(x => x.ToList()).ToList();
                        while (savedoutlets.Any())
                        {
                            var current = savedoutlets.FirstOrDefault();
                            if (current != null)
                            {
                               bool isSuccess = await importService.SaveAsync(current);
                                tempresult.Add(isSuccess.ToString().ToLower());
                                savedoutlets.Remove(current);
                                processed += current.Count;
                                MainViewModel.GlobalStatus = string.Format("{0} of {1} Outlet updates Completed...Please wait",
                                                                           processed, items.Count);
                            }
                        }

                        if (tempresult.All(n=>n.Equals("true")))
                        {
                            Application.Current.Dispatcher.BeginInvoke(
                                new Action(() =>
                                {
                                    int i = 0;
                                    foreach (var item in items)
                                    {
                                        ImportVmList.Remove(
                                            ImportVmList.FirstOrDefault(
                                                o => o.Name == item.Name));
                                        i++;
                                    }
                                    MainViewModel.GlobalStatus =
                                        string.Format("Successfully uploaded {0} Outlets", items.Count);
                                }));

                        }
                    }
                    else
                    {
                        ShowValidationErrors(result);
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
            SelectedPath = string.IsNullOrEmpty(MainViewModel.Filepath) ? "" : Path.Combine(MainViewModel.Filepath, @"customer.txt");
            if (string.IsNullOrEmpty(SelectedPath) && System.Diagnostics.Debugger.IsAttached)
                SelectedPath = Path.Combine(FileUtility.GetDefaultDirectory(), @"customer.txt");
            PageTitle = "Outlets Imports";
            MainViewModel.GlobalStatus = string.Empty;
        }

    }
    public class OutletImportVm:ImportItemVM
    {
        public string PinNo { get; set; }
        public string PostalAddress { get; set; }
        public string PhysicalAddress { get; set; }
        public string Status { get; set; }
        public string Tax { get; set; }
        public string Currency { get; set; }
        //public decimal CreditLimit { get; set; }
        public string SalesmanCode { get; set; }
        public string RouteName { get; set; }
        
    }
}
