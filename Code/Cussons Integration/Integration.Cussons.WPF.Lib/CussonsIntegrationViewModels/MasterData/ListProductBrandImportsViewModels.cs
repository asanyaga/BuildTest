using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.Util;
using Integration.Cussons.WPF.Lib.ImportEntities;
using Integration.Cussons.WPF.Lib.ImportService.Products;
using Integration.Cussons.WPF.Lib.Utils;

namespace Integration.Cussons.WPF.Lib.CussonsIntegrationViewModels.MasterData
{
    public class ListProductBrandImportsViewModels : MasterDataImportListingsBase
    {
        internal IEnumerable<ProductBrandImport> ImportItems;
        public ObservableCollection<ProductBrandImportVM> ImportVmList { get; set; }
        public ListProductBrandImportsViewModels()
        {
            ImportVmList = new ObservableCollection<ProductBrandImportVM>();
        }

        protected override void Load(bool isFirstLoad = false)
        {
            if (isFirstLoad)
                Setup();
            LoadProductBrandImportFromImportFile();
        }

        private async void LoadProductBrandImportFromImportFile()
        {
            if(FileUtility.ValidateFile(SelectedPath))
            {
               using (var c = NestedContainer)
                {

                    try
                    {
                        ImportVmList.Clear();
                        ImportItems = await Using<IProductBrandImportService>(c).Import(SelectedPath);

                        var productImports = ImportItems as List<ProductBrandImport> ?? ImportItems.ToList();
                        if (productImports.Any())
                        {
                            var items = productImports.Select((n, i) => new ProductBrandImportVM()
                            {
                                SequenceNo = i + 1,
                                Code = n.Code,
                                Description = n.Description,
                                Name = n.Name,
                                SupplierCode = n.SupplierCode,
                                IsChecked = false
                            }).AsQueryable();

                            PagedList = new PagenatedList<ImportItemVM>(items, CurrentPage, ItemsPerPage, items.Count());


                            PagedList.ToList().ForEach(n => ImportVmList.Add((ProductBrandImportVM)n));
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
                var items = ImportItems.Select(n => new ProductBrandImport()
                {
                    Code = n.Code,
                    Description = n.Description,
                    Name = n.Name,
                    SupplierCode = n.SupplierCode
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
                var items = selected.Select(n => new ProductBrandImport()
                {
                    Code = n.Code,
                    Description = n.Description,
                    Name = n.Name,
                    SupplierCode = n.SupplierCode
                }).ToList();
                Import(items);
            }
            else
            {
                MessageBox.Show("There is nothing to import");
            }
        }

        internal async void Import(List<ProductBrandImport> commodityTypeImportItems)
        {
            using (var c = NestedContainer)
            {
                var importService = Using<IProductBrandImportService>(c);
                MainViewModel.GlobalStatus = "Validating...Please wait";
                var result = await importService.ValidateAsync(commodityTypeImportItems);
                if (result.All(p => p.IsValid))
                {
                    var items = result.Select(o => o.Entity).OfType<ProductBrand>().ToList();
                    MainViewModel.GlobalStatus = "Validation Success..Saving entities";
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
                                       ImportVmList.FirstOrDefault(
                                           o => o.Name == item.Name);
                                   if (remove != null)
                                       ImportVmList.Remove(remove);
                                   i++;
                               }
                               MainViewModel.GlobalStatus = string.Format("Successfully uploaded {0} Brands", i);
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
            SelectedPath = string.IsNullOrEmpty(MainViewModel.Filepath) ? "" : Path.Combine(MainViewModel.Filepath, @"brands.txt");
            if(string.IsNullOrEmpty(SelectedPath) &&System.Diagnostics.Debugger.IsAttached)
                SelectedPath = Path.Combine(FileUtility.GetDefaultDirectory(), @"Brands.txt");
            PageTitle = "Product Brands";
            MainViewModel.GlobalStatus = string.Empty;    
        }
        
    }
    public class ProductBrandImportVM : ImportItemVM
    {
        public string SupplierCode { get; set; }
    }
}
