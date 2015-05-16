using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Utility;
using Distributr.DataImporter.Lib.ImportService;
using Distributr.DataImporter.Lib.Utils;
using Distributr.DataImporter.Lib.ImportEntity;
using Distributr.DataImporter.Lib.ImportService.Products;
using Distributr.WPF.Lib.ViewModels.Admin;

namespace Distributr.DataImporter.Lib.ViewModel.FCL
{
    public class ListProductsToImportViewModel : FCLListingBaseViewModel
    {
        public ObservableCollection<ProductImportItem> ProductImportList { get; set; }
        internal IPagenatedList<ProductImportItem> PagedList;
        internal IEnumerable<ProductImport> ImportItems;
        public ListProductsToImportViewModel()
        {
            ProductImportList = new ObservableCollection<ProductImportItem>();
        }

        protected override void Load(bool isFirstLoad = false)
        {
            if (isFirstLoad)
                SetUp();
            LoadProductsToImport();
        }

        private void LoadProductsToImport()
        {
           if(!FileUtility.ValidateFile(SelectedPath))return;
            using (var c=NestedContainer)
            {

                try
                {
                    ImportItems = Using<IProductImportService>(c).Import(SelectedPath);

                    var productImports = ImportItems as List<ProductImport> ?? ImportItems.ToList();
                    if (productImports.Any())
                    {
                        var items = productImports.Select((n, i) => new ProductImportItem()
                                                                        {
                                                                            SequenceNo = i + 1,
                                                                            BrandCode = n.BrandCode,
                                                                            Description = n.Description,
                                                                            DiscountGroup = n.DiscountGroup,
                                                                            ExFactoryPrice = n.ExFactoryPrice,
                                                                            PackagingCode = n.PackagingCode,
                                                                            PackagingTypeCode = n.PackagingTypeCode,
                                                                            ProductCode = n.ProductCode,
                                                                            ProductFlavourCode = n.ProductFlavourCode,
                                                                            ProductTypeCode = n.ProductTypeCode,
                                                                            VATClass = n.VATClass,
                                                                            CustomerDiscount = n.CustomerDiscount,
                                                                            IsChecked = false
                                                                        }).AsQueryable();
                        
                        PagedList = new PagenatedList<ProductImportItem>(items, CurrentPage, ItemsPerPage,items.Count());
                        
                            ProductImportList.Clear();
                        PagedList.ToList().ForEach(ProductImportList.Add);

                        UpdatePagenationControl();

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error getting data from file\nDetails=>" + ex.Message);

                }
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
                                 var importService = Using<IProductImportService>(c);
                                 UploadStatusMessage = "Reading files...Please wait";
                                 ImportItems = importService.Import(SelectedPath);

                                 if (ImportItems.Any())
                                 {

                                     IList<ImportValidationResultInfo> result =
                                         new List<ImportValidationResultInfo>();
                                     UploadStatusMessage = "Validation started...Please wait";

                                     int batchSize = 500;
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
                                     UploadStatusMessage = string.Format("Done updating {0} of {1}", processed,
                                                                            ImportItems.Count());
                                     
                                 }
                             }
                         });
        }

       protected override void EditSelected()
        {
            throw new NotImplementedException();
        }

        protected override void ActivateSelected()
        {
            throw new NotImplementedException();
        }

        protected override void DeleteSelected()
        {
            throw new NotImplementedException();
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
        

        private void SetUp()
        {
            ProductImportList.Clear();
            SelectedPath = string.Concat(FileUtility.GetWorkingDirectory("importpath"), @"\item.txt");
            LoadedImportItem = "products";
            PageTitle = "Products";
            UploadStatusMessage = "";
            IsUploadSuccess = false;
        }


        

        public const string SelectedProductImportPropertyName = "SelectedProductImport";
        private ProductImportItem _selectedProductImport = null;
        public ProductImportItem SelectedProductImport
        {
            get { return _selectedProductImport; }

            set
            {
                if (_selectedProductImport == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedProductImportPropertyName);
                _selectedProductImport = value;
                RaisePropertyChanged(SelectedProductImportPropertyName);
            }
        }
    }

    public class ProductImportItem
    {
       public string ProductCode { get; set; }
       public string Description { get; set; }
       public decimal ExFactoryPrice { get; set; }
       public string PackagingTypeCode { get; set; }
       public string DiscountGroup { get; set; }
       public string CustomerDiscount { get; set; }
       public string VATClass { get; set; }
       public string BrandCode { get; set; }
       public string Weight { get; set; }
       public string PackagingCode { get; set; }
       public string ProductTypeCode { get; set; }
       public string ProductFlavourCode { get; set; }
       public int SequenceNo { get; set; }
       public bool IsChecked { get; set; }
       
    }
}
