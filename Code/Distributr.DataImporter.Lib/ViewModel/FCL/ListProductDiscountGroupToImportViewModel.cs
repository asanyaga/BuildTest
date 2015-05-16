using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility;
using Distributr.DataImporter.Lib.ImportEntity;
using Distributr.DataImporter.Lib.ImportService;
using Distributr.DataImporter.Lib.ImportService.DiscountGroups;
using Distributr.DataImporter.Lib.ImportService.Products;
using Distributr.DataImporter.Lib.Utils;
using Distributr.WPF.Lib.ViewModels.Admin;

namespace Distributr.DataImporter.Lib.ViewModel.FCL
{
   public class ListProductDiscountGroupToImportViewModel : FCLListingBaseViewModel
    {
       public ObservableCollection<DiscountGroupImportItem> DiscountGroupImportList { get; set; }
       internal IPagenatedList<DiscountGroupImportItem> PagedList;
       internal IEnumerable<ProductDiscountGroupImport> ImportItems; 

       public ListProductDiscountGroupToImportViewModel()
       {
           DiscountGroupImportList = new ObservableCollection<DiscountGroupImportItem>();
       }

       protected override void Load(bool isFirstLoad = false)
       {
           if (isFirstLoad)
               SetUp();
           LoadDiscountGroupsToImport();
       }
       private void SetUp()
       {
           DiscountGroupImportList.Clear();
           SelectedPath = string.Concat(FileUtility.GetWorkingDirectory("importpath"), @"\DiscountGroup.txt");
           LoadedImportItem = "discountGroup";
           PageTitle = "Discount Group";
           UploadStatusMessage = "";
           IsUploadSuccess = false;
       }

       private void LoadDiscountGroupsToImport()
       {
           if (!FileUtility.ValidateFile(SelectedPath)) return;
           using (var c = NestedContainer)
           {
               try
               {
                   ImportItems = Using<IProductDiscountGroupImportService>(c).Import(SelectedPath);
                   var imports = ImportItems as List<ProductDiscountGroupImport> ?? ImportItems.ToList();
                   if (imports.Any())
                   {
                       var importready = imports.Select((n, i) => new DiscountGroupImportItem()
                       {
                           SequenceNo = i + 1,
                           IsChecked = false,
                           ProductCode = n.ProductCode,
                           DiscontGroupCode = n.DiscontGroupCode,
                           DiscountValue = n.DiscountValue

                       }).AsQueryable();

                       PagedList = new PagenatedList<DiscountGroupImportItem>(importready.AsQueryable(),
                                                                                              CurrentPage,
                                                                                              ItemsPerPage,
                                                                                              importready.Count());
                       DiscountGroupImportList.Clear();
                       PagedList.ToList().ForEach(DiscountGroupImportList.Add);

                       UpdatePagenationControl();


                   }
               }
               catch (Exception ex)
               {
                   MessageBox.Show("Error getting data from file\nDetails=>" + ex.Message);

               }
           }
       }

       protected override void UploadSelected()
       {
           var selectedproductDiscounts = DiscountGroupImportList.Where(o => o.IsChecked).ToList();
           if (selectedproductDiscounts.Any())
               Import(selectedproductDiscounts.Select(n=>new ProductDiscountGroupImport()
                                                             {
                                                                 ProductCode = n.ProductCode,
                                                                 DiscontGroupCode = n.DiscontGroupCode,
                                                                 DiscountValue = n.DiscountValue
                                                             }).ToList());
           else
           {
               MessageBox.Show("No import item", "Importer Info", MessageBoxButton.OK);
           }
       }
       protected override void UploadCurrentPage()
       {

           var selected = DiscountGroupImportList.ToList();
           if (selected.Any())
               Import(selected.Select(n => new ProductDiscountGroupImport()
               {
                   ProductCode = n.ProductCode,
                   DiscontGroupCode = n.DiscontGroupCode,
                   DiscountValue = n.DiscountValue
               }).ToList());
           else
           {
               MessageBox.Show("No import item", "Importer Info", MessageBoxButton.OK);

           }
       }
       protected override void UploadAll()
       {
           ImportAll();
       }
       internal void ImportAll()
       {
           Task.Run(() =>
                        {
                            using (var c = NestedContainer)
                            {
                                var importService = Using<IProductDiscountGroupImportService>(c);
                                UploadStatusMessage = "Reading files...Please wait";
                                ImportItems = importService.Import(SelectedPath);

                                if (!ImportItems.Any())
                                {
                                    UploadStatusMessage = "No files for upload";
                                    return;
                                }
                                IList<ImportValidationResultInfo> result =
                                    new List<ImportValidationResultInfo>();
                                UploadStatusMessage = "Validation started...Please wait";

                                int batchSize = 1000;
                                int processed = 0;
                                var products =
                                    ImportItems.OrderBy(p => p.ProductCode).Batch(batchSize).Select(
                                        x => x.ToList()).ToList();
                                while (products.Any())
                                {
                                    var current = products.FirstOrDefault();
                                    if (current != null)
                                    {
                                        var tempresult = importService.ValidateAndSave(current);
                                        tempresult.ToList().ForEach(result.Add);
                                        products.Remove(current);
                                        processed += current.Count;
                                        UploadStatusMessage =
                                            string.Format("{0} of {1} validation & upload Completed...Please wait",
                                                          processed, ImportItems.Count());
                                    }
                                }

                                if (result.Any(p => !p.IsValid))
                                {
                                    ShowValidationErrors(result);
                                    IsUploadSuccess = false;
                                }
                                else
                                {
                                    UploadStatusMessage = string.Format("Done updating {0} of {1}", processed,
                                                                        ImportItems.Count());
                                    ShowSkippedItems(importService.GetNonExistingProductCodes());

                                }

                            }
                        });
       }
        [Obsolete("Depriciated")]
       private async void Import(List<ProductDiscountGroupImport> importItems)
       {
           await Task.Run(() =>
                              {
                                  using (var c = NestedContainer)
                                  {

                                      var importService = Using<IProductDiscountGroupImportService>(c);

                                      MainWindowViewModel.GlobalStatus = "Validating...Please wait";
                                      IList<ImportValidationResultInfo> result = new List<ImportValidationResultInfo>();
                                      MainWindowViewModel.GlobalStatus = "Validation started...Please wait";
                                      UploadStatusMessage = "Validation started...Please wait";

                                      int batchSize = 100;
                                      int processed = 0;
                                      var productDiscountGroups =
                                          importItems.OrderBy(p => p.DiscontGroupCode).Batch(batchSize).Select(
                                              x => x.ToList()).ToList();
                                      while (productDiscountGroups.Any())
                                      {
                                          var current = productDiscountGroups.FirstOrDefault();
                                          if (current == null) continue;
                                          var tempresult = importService.Validate(current);
                                          tempresult.ToList().ForEach(result.Add);
                                          productDiscountGroups.Remove(current);
                                          processed += current.Count;
                                          UploadStatusMessage =
                                              string.Format("{0} of {1} validation Completed...Please wait",
                                                            processed, importItems.Count);
                                      }
                                      productDiscountGroups.Clear();
                                      processed = 0;

                                      
                                      if (result.All(p => p.IsValid))
                                      {
                                          var items =
                                              result.Select(o => o.Entity).OfType<ProductGroupDiscount>().ToList();
                                          MainWindowViewModel.GlobalStatus = "Validation Success..Saving entities";
                                          UploadStatusMessage = "Validation Success..Saving entities";
                                          var tempresult = new List<string>();
                                          var savedProductDiscounts =
                                              items.Batch(batchSize).Select(x => x.ToList()).ToList();
                                          while (savedProductDiscounts.Any())
                                          {
                                              var current = savedProductDiscounts.FirstOrDefault();
                                              if (current != null)
                                              {
                                                  importService.Save(current);
                                                  tempresult.Add(true.ToString().ToLower());
                                                  savedProductDiscounts.Remove(current);
                                                  processed += current.Count;
                                                  UploadStatusMessage =
                                                      string.Format("Updated {0} of {1} ...Please wait",
                                                                    processed, importItems.Count);
                                              }
                                          }
                                          if (tempresult.All(n => n.Equals("true")))
                                          {
                                              IsUploadSuccess = true;
                                              if (processed == 0)
                                                  processed = importService.GetUpdatedItems();
                                              UploadStatusMessage = string.Format(
                                                  "Successfully uploaded {0} product discounts", processed);
                                          }
                                         ShowSkippedItems(importService.GetNonExistingProductCodes());
                                      }
                                      else
                                      {
                                          Using<IImportValidationPopUp>(c).ShowPopUp(
                                              result.Where(o => !o.IsValid).ToList());
                                          IsUploadSuccess = false;
                                      }
                                  }
                              });
       }

       private void ShowSkippedItems(List<string> productsNotFound)
       {
           
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
                           error.AppendLine(string.Format("Product codes=>{0} ||{1}",
                                                          current[0] ?? "",
                                                          current.Count > 1 ? current[1] : ""));
                           batches.Remove(current);
                       }
                   }
                   MessageBox.Show(error.ToString(), "Distributr Error", MessageBoxButton.OK);
                   FileUtility.LogError(error.ToString());
               }
           
       }

       protected override void GoToPage(PageDestinations page)
       {
           GoToPageBase(page, PagedList.PageCount);
       }

       protected override void ComboPageSizesSelectionChanged(int take)
       {
           ItemsPerPage = take;
           Load();
       }

       protected override void UpdatePagenationControl()
       {
           UpdatePagenationControlBase(CurrentPage, PagedList.PageCount, PagedList.TotalItemCount, PagedList.IsFirstPage,
                                       PagedList.IsLastPage);
       }


    }

    public class DiscountGroupImportItem
    {
        public string DiscontGroupCode { get; set; }
        public string ProductCode { get; set; }
        public decimal DiscountValue { get; set; }
        public int SequenceNo { get; set; }
        public bool IsChecked { get; set; }
    }
}
