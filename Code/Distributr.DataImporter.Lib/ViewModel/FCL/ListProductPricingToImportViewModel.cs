using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Utility;
using Distributr.DataImporter.Lib.ImportService;
using Distributr.DataImporter.Lib.Utils;
using Distributr.DataImporter.Lib.ImportEntity;
using Distributr.DataImporter.Lib.ImportService.PriceGroups;
using Distributr.WPF.Lib.ViewModels.Admin;

namespace Distributr.DataImporter.Lib.ViewModel.FCL
{
    public class ListProductPricingToImportViewModel : FCLListingBaseViewModel
    {
        public ObservableCollection<PricingImportItem> PricingImportList { get; set; }
        internal IPagenatedList<PricingImportItem> PagedList;
        internal IEnumerable<PricingImport> ImportItems;

        public ListProductPricingToImportViewModel()
        {
            PricingImportList = new ObservableCollection<PricingImportItem>();
        }

        protected override void Load(bool isFirstLoad = false)
        {
            if (isFirstLoad)
                SetUp();
            Reload();
        }

        private void SetUp()
        {
            PricingImportList.Clear();
            SelectedPath = string.Concat(FileUtility.GetWorkingDirectory("importpath"), @"\PriceGroup.txt");
            LoadedImportItem = "productPricing";
            PageTitle = "Product Pricing";
            UploadStatusMessage = "";
            IsUploadSuccess = false;
        }

        protected override void Reload()
        {
            if (!FileUtility.ValidateFile(SelectedPath)) return;
            using (var c = NestedContainer)
            {
                try
                {
                    ImportItems = Using<IPricingImportService>(c).Import(SelectedPath);
                    var imports = ImportItems as List<PricingImport> ?? ImportItems.ToList();
                    if (imports.Any())
                    {
                        var importready = imports.Select((n, i) => new PricingImportItem()
                                                                       {
                                                                           SequenceNo = i + 1,
                                                                           IsChecked = false,
                                                                           ProductCode = n.ProductCode,
                                                                           SellingPrice = n.SellingPrice,
                                                                           ExFactoryRate = n.ExFactoryRate,
                                                                           PricingTireCode = n.PricingTireCode

                                                                       }).AsQueryable();

                        PagedList = new PagenatedList<PricingImportItem>(importready.AsQueryable(),
                                                                                               CurrentPage,
                                                                                               ItemsPerPage,
                                                                                               importready.Count());
                        PricingImportList.Clear();
                        PagedList.ToList().ForEach(PricingImportList.Add);
                       
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
                    var importService = Using<IPricingImportService>(c);
                    UploadStatusMessage = "Reading files...Please wait";
                    ImportItems = importService.Import(SelectedPath);

                    if (ImportItems.Any())
                    {

                        IList<ImportValidationResultInfo> result =
                            new List<ImportValidationResultInfo>();
                        UploadStatusMessage = "Validation started...Please wait";

                        int batchSize = 1000;
                        int processed = 0;
                        var pricings =
                            ImportItems.OrderBy(p => p.ProductCode).Batch(batchSize).Select(
                                x => x.ToList()).ToList();
                        while (pricings.Any())
                        {
                            var current = pricings.FirstOrDefault();
                            if (current != null)
                            {
                                var tempresult = importService.ValidateAndSave(current);
                                tempresult.ToList().ForEach(result.Add);
                                pricings.Remove(current);
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
                        ShowSkippedItems(importService.GetNonExistingProductCodes());
                        UploadStatusMessage = string.Format("Done updating {0} of {1}", processed,
                                                            ImportItems.Count());
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

        public const string SelectedPricingImportItemPropertyName = "SelectedPricingImportItem";
        private PricingImportItem _selectedPricingImport = null;
        public PricingImportItem SelectedOutletImport
        {
            get { return _selectedPricingImport; }

            set
            {
                if (_selectedPricingImport == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedPricingImportItemPropertyName);
                _selectedPricingImport = value;
                RaisePropertyChanged(SelectedPricingImportItemPropertyName);
            }
        }
    }

    public class PricingImportItem
    {
        public string ProductCode { get; set; }
        public string PricingTireCode { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal ExFactoryRate { get; set; }
        public int SequenceNo { get; set; }
        public bool IsChecked { get; set; }
    }


}
