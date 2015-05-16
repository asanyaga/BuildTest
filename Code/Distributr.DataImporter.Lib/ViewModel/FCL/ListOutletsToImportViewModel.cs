using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Utility;
using Distributr.DataImporter.Lib.ImportService;
using Distributr.DataImporter.Lib.Utils;
using Distributr.DataImporter.Lib.ImportEntity;
using Distributr.DataImporter.Lib.ImportService.Outlets;
using Distributr.WPF.Lib.ViewModels.Admin;

namespace Distributr.DataImporter.Lib.ViewModel.FCL
{
    public class ListOutletsToImportViewModel : FCLListingBaseViewModel
    {
        public ObservableCollection<OutletImportItem> OutletImportList { get; set; }
        internal IPagenatedList<OutletImportItem> PagedList;
        internal IEnumerable<OutletImport> ImportItems;

        public ListOutletsToImportViewModel()
        {
            OutletImportList=new ObservableCollection<OutletImportItem>();
        }

        protected override void Load(bool isFirstLoad = false)
        {
            if (isFirstLoad)
                SetUp();
            Reload();
        }

        private void SetUp()
        {
            OutletImportList.Clear();
            SelectedPath = string.Concat(FileUtility.GetWorkingDirectory("importpath"), @"\Customer.txt");
            LoadedImportItem = "outlets";
            PageTitle = "Outlets";
            UploadStatusMessage = "";
            IsUploadSuccess = false;
        }

        protected override void Reload()
        {
            if (!FileUtility.ValidateFile(SelectedPath)) return;
            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    () =>
                        {
                            using (var c = NestedContainer)
                            {
                                try
                                {
                                    ImportItems = Using<IOutletImportService>(c).Import(SelectedPath);
                                    var imports = ImportItems as List<OutletImport> ?? ImportItems.ToList();
                                    if (imports.Any())
                                    {
                                        var importready = imports.Select((n, i) => new OutletImportItem()
                                                                                       {
                                                                                           SequenceNo = i + 1,
                                                                                           DistributorCode =
                                                                                               n.DistributorCode,
                                                                                           Name = n.Name,
                                                                                           IsChecked = false,
                                                                                           Address = n.Address,
                                                                                           ContactPerson =
                                                                                               n.ContactPerson,
                                                                                           Credit = n.Credit,
                                                                                           Discount = n.Discount,
                                                                                           DiscountGroupCode =
                                                                                               n.DiscountGroupCode,
                                                                                           OutletCategoryName =
                                                                                               n.OutletCategoryName,
                                                                                           OutletCode = n.OutletCode,
                                                                                           OutletTypeName =
                                                                                               n.OutletTypeName,
                                                                                           PhoneNo = n.PhoneNo,
                                                                                           RouteCode = n.RouteCode,
                                                                                           SpecialPrice = n.SpecialPrice,
                                                                                           TierCode = n.TierCode,
                                                                                           VatClass = n.VatClass
                                                                                       }).ToList();
                                        OutletImportList.Clear();
                                        PagedList = new PagenatedList<OutletImportItem>(importready.AsQueryable(),
                                                                                              CurrentPage,
                                                                                              ItemsPerPage,
                                                                                              importready.Count());
                                        PagedList.ToList().ForEach(OutletImportList.Add);

                                        UpdatePagenationControl();
                                       
                                        
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Error getting data from file\nDetails=>" + ex.Message);

                                }
                            }
                        }));
        }
       
        protected override void UploadAll()
        {
           Task.Run(() =>
            {
                using (var c = NestedContainer)
                {
                    var importService = Using<IOutletImportService>(c);
                    UploadStatusMessage = "Reading files...Please wait";
                    ImportItems = importService.Import(SelectedPath);

                    if (ImportItems.Any())
                    {

                        IList<ImportValidationResultInfo> result =
                            new List<ImportValidationResultInfo>();
                        UploadStatusMessage = "Validation started...Please wait";

                        int batchSize = 1000;
                        int processed = 0;
                        var outlets =
                            ImportItems.OrderBy(p => p.OutletCode).Batch(batchSize).Select(
                                x => x.ToList()).ToList();
                        while (outlets.Any())
                        {
                            var current = outlets.FirstOrDefault();
                            if (current != null)
                            {
                                var tempresult = importService.ValidateAndSave(current);
                                tempresult.ToList().ForEach(result.Add);
                                outlets.Remove(current);
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


        public const string SelectedOutletImportPropertyName = "SelectedOutletImport";
        private DistributorSalesmanImportItem _selectedOutletImport = null;
        public DistributorSalesmanImportItem SelectedOutletImport
        {
            get { return _selectedOutletImport; }

            set
            {
                if (_selectedOutletImport == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedOutletImportPropertyName);
                _selectedOutletImport = value;
                RaisePropertyChanged(SelectedOutletImportPropertyName);
            }
        }
    }

    public class OutletImportItem
    {
        public string OutletCode { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string PhoneNo { get; set; }

        public string ContactPerson { get; set; }

        public string DiscountGroupCode { get; set; }

        public string RouteCode { get; set; }

        public string TierCode { get; set; }
        public string SpecialPrice { get; set; }

        public string Discount { get; set; }

        public string VatClass { get; set; }

        public string Credit { get; set; }

        public string DistributorCode { get; set; }

        public string OutletCategoryName { get; set; }

        public string OutletTypeName { get; set; }
        public int SequenceNo { get; set; }
        public bool IsChecked { get; set; }
    }
}
