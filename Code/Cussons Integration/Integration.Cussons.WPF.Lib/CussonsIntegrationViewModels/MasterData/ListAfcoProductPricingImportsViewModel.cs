
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
using Integration.Cussons.WPF.Lib.ImportEntities;
using Integration.Cussons.WPF.Lib.ImportService;
using Integration.Cussons.WPF.Lib.ImportService.Products.Impl;
using Integration.Cussons.WPF.Lib.Utils;

namespace Integration.Cussons.WPF.Lib.CussonsIntegrationViewModels.MasterData
{
    public class ListAfcoProductPricingImportsViewModel : MasterDataImportListingsBase
    {
        internal IEnumerable<AfcoProductPricingImport> ImportItems;
        public ObservableCollection<AfcoPricingImportVM> ImportVmList { get; set; }

        public ListAfcoProductPricingImportsViewModel()
        {
            ImportVmList=new ObservableCollection<AfcoPricingImportVM>();
        }

        protected override void Load(bool isFirstLoad = false)
        {
            if (isFirstLoad)
                Setup();
            LoadFromImportFile();
        }
        

        private  async  void LoadFromImportFile()
        {
            if (FileUtility.ValidateFile(SelectedPath))
            {
                using (var c = NestedContainer)
                {

                    try
                    {
                        ImportVmList.Clear();
                        ImportItems = await Using<AfcoPricingImportService>(c).Import(SelectedPath);

                        var productImports = ImportItems as List<AfcoProductPricingImport> ?? ImportItems.ToList();
                        if (productImports.Any())
                        {
                            var items = productImports.Select((n, i) => new AfcoPricingImportVM()
                            {
                                SequenceNo = i + 1,
                                ProductBrandCode = n.ProductBrandCode,
                                ProductCode = n.ProductCode,
                                SellingPrice = n.SellingPrice,
                                IsChecked = false
                            }).AsQueryable();

                            PagedList = new PagenatedList<ImportItemVM>(items, CurrentPage, ItemsPerPage, items.Count());


                            PagedList.ToList().ForEach(n => ImportVmList.Add((AfcoPricingImportVM)n));
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
                var items = ImportItems.Select(n => new AfcoProductPricingImport()
                {
                    ProductCode = n.ProductCode,
                    SellingPrice = n.SellingPrice
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
                var items = selected.Select(n => new AfcoProductPricingImport()
                {
                    ProductCode = n.ProductCode,
                    SellingPrice = n.SellingPrice
                }).ToList();
                Import(items);
            }
            else
            {
                MessageBox.Show("There is nothing to import");
            }
        }
        internal async void  Import(List<AfcoProductPricingImport> imports)
        {
            using (var c = NestedContainer)
            {
                var importService = Using<AfcoPricingImportService>(c);
                MainViewModel.GlobalStatus = "Validating...Please wait";
                IList<ImportValidationResultInfo> result = new List<ImportValidationResultInfo>();
                MainViewModel.GlobalStatus = "Validation started...Please wait";

                int batchSize = 100;
                int processed = 0;
                var productPricings = imports.OrderBy(p => p.ProductCode).Batch(batchSize).Select(x => x.ToList()).ToList();
                while (productPricings.Any())
                {
                    var current = productPricings.FirstOrDefault();
                    if (current != null)
                    {
                        IList<ImportValidationResultInfo> tempresult = await importService.ValidateAsync(current);
                        tempresult.ToList().ForEach(result.Add);
                        //result.ToList().AddRange(tempresult);
                        productPricings.Remove(current);
                        processed += current.Count;
                        MainViewModel.GlobalStatus = string.Format("{0} of {1} validation Completed...Please wait",
                                                                   processed, imports.Count);
                    }
                }
                productPricings.Clear();
                batchSize = 100;
                processed = 0;
                var productsNotFound = importService.GetNonExistingProductCodes();
                if(productsNotFound.Any())
                {
                    var error = new StringBuilder();
                    error.AppendLine("Non existing products-Not imported");
                    foreach (var productCode in productsNotFound)
                    {
                        error.AppendLine(string.Format("Product code=>{0}", productCode));
                    }
                    MessageBox.Show(error.ToString(), "Distributr Error", MessageBoxButton.OK);
                    FileUtility.LogError(error.ToString());
                }

                if (result.All(p => p.IsValid))
                {
                    var items = result.Select(o => o.Entity).OfType<ProductPricing>().ToList();
                    MainViewModel.GlobalStatus = "Validation Success..Saving entities";

                    var tempresult = new List<string>();

                    var savedProductPricings =
                        items.Batch(batchSize).Select(x => x.ToList()).ToList();
                    while (savedProductPricings.Any())
                    {
                        var current = savedProductPricings.FirstOrDefault();
                        if (current != null)
                        {
                            var isSuccess = await importService.SaveAsync(current);
                            tempresult.Add(isSuccess.ToString().ToLower());
                            savedProductPricings.Remove(current);
                            processed += current.Count;
                            MainViewModel.GlobalStatus =
                                string.Format("{0} of {1} product pricing updates Completed...Please wait",
                                              processed, items.Count);
                        }
                    }
                    if (tempresult.All(n => n.Equals("true")))
                    {
                        Application.Current.Dispatcher.BeginInvoke(
                            new Action(() =>
                            {
                                int i = 0;
                                var removes = items.SelectMany(y => y.ProductPricingItems);
                                foreach (var item in removes)
                                {
                                    var remove =
                                        ImportVmList.FirstOrDefault(
                                            o => o.SellingPrice == item.SellingPrice);
                                    if (remove != null)
                                        ImportVmList.Remove(remove);
                                    i++;
                                }
                                MainViewModel.GlobalStatus =
                                    string.Format("Successfully uploaded {0} product pricing", processed);
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

        private void Setup()
        {
            ImportVmList.Clear();
            UploadStatusMessage = "";
            SelectedPath = string.IsNullOrEmpty(MainViewModel.Filepath)
                               ? ""
                               : Path.Combine(MainViewModel.Filepath, @"AfcoPricelist.txt");
            if (string.IsNullOrEmpty(SelectedPath) && System.Diagnostics.Debugger.IsAttached)
                SelectedPath = Path.Combine(FileUtility.GetDefaultDirectory(), @"AfcoPricelist.txt");
            PageTitle = "Afco Price List";
            MainViewModel.GlobalStatus = string.Empty;
        }
    }

    public class AfcoPricingImportVM:ImportItemVM
    {
        
        public string ProductBrandCode { get; set; }
       
        public string ProductCode { get; set; }
       
        public decimal SellingPrice { get; set; }
    }
}
