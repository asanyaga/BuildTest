using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Integration.Cussons.WPF.Lib.ImportEntities;
using Integration.Cussons.WPF.Lib.ImportService;
using Integration.Cussons.WPF.Lib.ImportService.Shipping;
using Integration.Cussons.WPF.Lib.MasterDataImportService;
using Integration.Cussons.WPF.Lib.Utils;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.Util;

namespace Integration.Cussons.WPF.Lib.CussonsIntegrationViewModels.MasterData
{
    public class ShipToAddressImportsViewModel : MasterDataImportListingsBase
    {
        internal IEnumerable<ShipToAddressImport> ImportItems;
        public ObservableCollection<ShipToAddressImportVM> ImportVmList { get; set; }

        public ShipToAddressImportsViewModel()
        {
            ImportVmList=new ObservableCollection<ShipToAddressImportVM>();
  
        }

        protected override void Load(bool isFirstLoad = false)
        {
            if (isFirstLoad)
                Setup();
            LoadImportFromFile();
        }

        private async void LoadImportFromFile()
        {
            if (FileUtility.ValidateFile(SelectedPath))
            {
                using (var c = NestedContainer)
                {

                    try
                    {
                        ImportVmList.Clear();
                        ImportItems = await Using<IShipToAddressImportService>(c).Import(SelectedPath);

                        var productImports = ImportItems as List<ShipToAddressImport> ?? ImportItems.ToList();
                        if (productImports.Any())
                        {
                            var items = productImports.Select((n, i) => new ShipToAddressImportVM()
                            {
                                SequenceNo = i + 1,
                                OutletCode=n.OutletCode,
                                OutletName=n.OutletName,
                                PostalAddress=n.PostalAddress,
                                ShipToCode=n.ShipToCode,
                                ShipToName=n.ShipToName,
                                IsChecked = false
                            }).AsQueryable();

                            PagedList = new PagenatedList<ImportItemVM>(items, CurrentPage, ItemsPerPage, items.Count());


                            PagedList.ToList().ForEach(n => ImportVmList.Add((ShipToAddressImportVM)n));
                            UpdatePagenationControl();

                        }
                    }catch(Exception ex)
                    {
                        MessageBox.Show("Error getting data from file\nDetails=>" + ex.Message);
                        FileUtility.LogError(ex.Message);
                    }
                }
            }
        }

        internal async void Import(List<ShipToAddressImport> imports)
        {
            using (var c = NestedContainer)
            {
                var importService = Using<IShipToAddressImportService>(c);
                MainViewModel.GlobalStatus = "Validating...Please wait";
                IList<ImportValidationResultInfo> result = new List<ImportValidationResultInfo>();
                int batchSize = 100;
                int processed = 0;
                var outlets = imports.OrderBy(p => p.OutletCode).Batch(batchSize).Select(x => x.ToList()).ToList();
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
                                                                   processed, imports.Count);
                    }
                }
                outlets.Clear();
                bool success = false;
                if (result.All(p => p.IsValid))
                {
                    //var items = result.Select(o => o.Entity).OfType<>().ToList();
                    MainViewModel.GlobalStatus = "Validation Success..Saving entities";
                    success = await importService.SaveAsync();

                }

                if (success)
                {
                    Application.Current.Dispatcher.BeginInvoke(
                        new Action(() =>
                                       {
                                           MainViewModel.GlobalStatus =
                                               string.Format("Successfully uploaded {0} Ship to Addresses", processed);
                                       }));

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

        protected override async void UploadAll()
        {
            if (ImportVmList.Any())
            {
                var items = ImportItems.Select(n => new ShipToAddressImport()
                {
                   OutletCode=n.OutletCode,
                   OutletName=n.OutletName,
                   PostalAddress=n.PostalAddress,
                   ShipToCode=n.ShipToCode,
                   ShipToName=n.ShipToName
                   
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
                var items = selected.Select(n => new ShipToAddressImport()
                {
                    OutletCode = n.OutletCode,
                    OutletName = n.OutletName,
                    PostalAddress = n.PostalAddress,
                    ShipToCode = n.ShipToCode,
                    ShipToName = n.ShipToName
                }).ToList();
                Import(items);
            }
            else
            {
                MessageBox.Show("There is nothing to import");
            }
        }

        private void Setup()
        {
            ImportVmList.Clear();
            UploadStatusMessage = "";
            SelectedPath = string.IsNullOrEmpty(MainViewModel.Filepath)
                               ? ""
                               :Path.Combine(MainViewModel.Filepath, @"shipto.txt");
            if (string.IsNullOrEmpty(SelectedPath) && System.Diagnostics.Debugger.IsAttached)
                SelectedPath = Path.Combine(FileUtility.GetDefaultDirectory(), @"shipto.txt");
            PageTitle = "Shipto to Addresses";
            MainViewModel.GlobalStatus = string.Empty;
           
        }
    }

    public class ShipToAddressImportVM:ImportItemVM
    {
        
        public string OutletCode { get; set; }

       
        public string OutletName { get; set; }

        
        public string ShipToCode { get; set; }


        public string ShipToName { get; set; }

       
        public string PostalAddress { get; set; }
    }
}
