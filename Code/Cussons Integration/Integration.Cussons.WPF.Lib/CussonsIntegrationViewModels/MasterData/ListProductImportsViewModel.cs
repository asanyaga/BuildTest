using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Utility;
using Integration.Cussons.WPF.Lib.ImportService;
using Integration.Cussons.WPF.Lib.ImportService.Products;
using Integration.Cussons.WPF.Lib.MasterDataImportService;
using Integration.Cussons.WPF.Lib.Utils;

namespace Integration.Cussons.WPF.Lib.CussonsIntegrationViewModels.MasterData
{
    public class ListProductImportsViewModel : MasterDataImportListingsBase
    {
        internal IEnumerable<ProductImport> ImportItems;
        public ObservableCollection<ProductImportVM> ImportVmList { get; set; }

        public ListProductImportsViewModel()
        {
            ImportVmList = new ObservableCollection<ProductImportVM>();
        }

        protected override void Load(bool isFirstLoad = false)
        {
            if (isFirstLoad)
                Setup();
            LoadProductImportFromImportFile();
        }

        private async void LoadProductImportFromImportFile()
        {
            if (FileUtility.ValidateFile(SelectedPath))
            {
                using (var c = NestedContainer)
                {

                    try
                    {
                        ImportVmList.Clear();
                        ImportItems = await Using<IProductImportService>(c).Import(SelectedPath);

                        var productImports = ImportItems as List<ProductImport> ?? ImportItems.ToList();
                        if (productImports.Any())
                        {
                            var items = productImports.Select((n, i) => new ProductImportVM()
                                                                            {
                                                                                SequenceNo = i + 1,
                                                                                BrandCode = n.BrandCode,
                                                                                Code = n.ProductCode,
                                                                                Description = n.Description,
                                                                                Status = n.Status,
                                                                                SellingPrice = n.SellingPrice,
                                                                                ExFactoryPrice = n.ExFactoryPrice,
                                                                                IsChecked = false
                                                                            }).AsQueryable();

                            PagedList = new PagenatedList<ImportItemVM>(items, CurrentPage, ItemsPerPage, items.Count());


                            PagedList.ToList().ForEach(n => ImportVmList.Add((ProductImportVM) n));
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

        private void RemoveDuplicates()
        {
            var duplicates = ImportItems.OrderBy(p => p.ProductCode).Distinct();
        }

        protected override async void UploadAll()
        {
            if (ImportVmList.Any())
            {
                var items = ImportItems.Select(n => new ProductImport()
                                                         {
                                                             ProductCode=n.ProductCode,
                                                             Description = n.Description,
                                                             BrandCode = n.BrandCode,
                                                             ExFactoryPrice = n.ExFactoryPrice,
                                                             SellingPrice = n.SellingPrice,
                                                             Status = n.Status,
                                                             
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
                var items = selected.Select(n => new ProductImport()
                                                     {
                                                         ProductCode = n.Code,
                                                         Description = n.Description,
                                                         BrandCode = n.BrandCode,
                                                         ExFactoryPrice = n.ExFactoryPrice,
                                                         SellingPrice = n.SellingPrice,
                                                         Status = n.Status,
                                                         
                                                         
                                                     }).ToList();
               Import(items);
            }
            else
            {
                MessageBox.Show("There is nothing to import");
            }
        }

        internal async void Import(List<ProductImport> importItems)
        {
            using (var c = NestedContainer)
            {
                var importService = Using<IProductImportService>(c);
                MainViewModel.GlobalStatus = "Validating...Please wait";
                IList<ImportValidationResultInfo> result = new List<ImportValidationResultInfo>();
                MainViewModel.GlobalStatus = "Validation started...Please wait";

                int batchSize = 100;
                int processed = 0;
                importItems = importItems.OrderBy(p => p.ProductCode).Distinct().ToList();
                var products = importItems.OrderBy(p => p.BrandCode).Batch(batchSize).Select(x => x.ToList()).ToList();
                var passedValidations = new List<ProductImport>();
                while (products.Any())
                {
                    var current = products.FirstOrDefault();
                    if (current != null)
                    {
                        IList<ImportValidationResultInfo> tempresult = await importService.ValidateAsync(current);
                        tempresult.ToList().ForEach(result.Add);
                        products.Remove(current);
                        passedValidations.AddRange(current);
                        processed += current.Count;
                        MainViewModel.GlobalStatus = string.Format("{0} of {1} validation Completed...Please wait",
                                                                   processed, importItems.Count);
                    }
                }
                products.Clear();
                batchSize = 100;
                 processed = 0;
                 if (result.All(p => p.IsValid))
                 {
                     var items = result.Select(o => o.Entity).OfType<Product>().ToList();
                     MainViewModel.GlobalStatus = "Validation Success..Saving entities";

                     var tempresult = new List<string>();
                     var productsToSave =
                         items.OrderBy(p => p.Brand.Code).Batch(batchSize).Select(x => x.ToList()).ToList();

                     while (productsToSave.Any())
                     {
                         var current = productsToSave.FirstOrDefault();
                         if (current != null)
                         {
                             var isSuccess = await importService.SaveAsync(current);
                             tempresult.Add(isSuccess.ToString().ToLower());
                             productsToSave.Remove(current);
                             processed += current.Count;
                             MainViewModel.GlobalStatus =
                                 string.Format("{0} of {1} product updates Completed...Please wait",
                                               processed, items.Count);
                         }
                     }
                     MainViewModel.GlobalStatus =
                         string.Format("Started updating pricing...please wait");
                     var savedProducts =
                         passedValidations.OrderBy(p => p.ProductCode).Batch(batchSize).Select(x => x.ToList()).ToList();
                     processed = 0;
                     var pricingresult = new List<ImportValidationResultInfo>();
                     while (savedProducts.Any())
                     {
                         var current = savedProducts.FirstOrDefault();
                         if (current != null)
                         {
                             var temp = await importService.ValidatePricingAsync(current);
                             temp.ToList().ForEach(pricingresult.Add);
                             savedProducts.Remove(current);
                             processed += current.Count;
                             MainViewModel.GlobalStatus = string.Format("{0} of {1} validation Completed...Please wait",
                                                                        processed, passedValidations.Count);
                         }
                     }
                     var productsNotFound = importService.GetNonExistingProductCodes();
                     if (productsNotFound.Any())
                     {
                         var error = new StringBuilder();
                         error.AppendLine("Non existing products-Not imported");
                         var batches = productsNotFound.Batch(2).Select(x => x.ToList()).ToList();
                         while (batches.Any())
                         {
                             var current = batches.FirstOrDefault();
                             if (current != null)
                             {
                                 error.AppendLine(string.Format("Product codes=>{0} ||{1}", current[0] ?? "", current.Count > 1 ? current[1] : ""));
                                 batches.Remove(current);
                             }
                         }
                         MessageBox.Show(error.ToString(), "Distributr Error", MessageBoxButton.OK);
                         FileUtility.LogError(error.ToString());
                     }

                     if (pricingresult.All(p => p.IsValid))
                     {
                         processed = 0;
                         var pricings =
                             pricingresult.Select(o => o.Entity).OfType<ProductPricing>().ToList();
                         var batches = pricings.Batch(batchSize).Select(x => x.ToList()).ToList();

                         while (batches.Any())
                         {
                             var current = batches.FirstOrDefault();
                             if (current != null)
                             {
                                 var temp = await importService.SaveAsync(current);
                                 //temp.ToList().ForEach(pricingresult.Add);
                                 batches.Remove(current);
                                 processed += current.Count;
                                 MainViewModel.GlobalStatus = string.Format(
                                     "{0} of {1} Pricing Completed...Please wait",
                                     processed, pricings.Count);
                             }
                         }

                     }
                     else
                     {
                         ShowValidationErrors(pricingresult);
                     }

                     if (tempresult.All(n => n.Equals("true")))
                     {
                         Application.Current.Dispatcher.BeginInvoke(
                             new Action(() =>
                                            {
                                                int i = 0;
                                                foreach (var item in items)
                                                {
                                                    var remove =
                                                        ImportVmList.FirstOrDefault(
                                                            o => o.Description == item.Description);
                                                    if (remove != null)
                                                        ImportVmList.Remove(remove);
                                                    i++;
                                                }
                                                MainViewModel.GlobalStatus =
                                                    string.Format("Successfully uploaded {0} products", i);
                                            }));
                     }
                 }
                 else
                 {
                     ShowValidationErrors(result);
                 }
                
            }
        }

        private void ShowValidationErrors(IList<ImportValidationResultInfo> result)
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
            SelectedPath = string.IsNullOrEmpty(MainViewModel.Filepath)
                               ? ""
                               : Path.Combine(MainViewModel.Filepath, @"products.txt");
            if (string.IsNullOrEmpty(SelectedPath) && System.Diagnostics.Debugger.IsAttached)
                SelectedPath = Path.Combine(FileUtility.GetDefaultDirectory(), @"products.txt");
            PageTitle = "Products";
            MainViewModel.GlobalStatus = string.Empty;
        }

    }

    public class ProductImportVM : ImportItemVM
    {
        public string BrandCode { get; set; }
        public string Status { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal ExFactoryPrice { get; set; }
    }
}
