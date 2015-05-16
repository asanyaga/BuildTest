using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.DataImporter.Lib.ImportService;
using Distributr.DataImporter.Lib.Utils;
using Distributr.Core.Utility;
using Distributr.DataImporter.Lib.ImportEntity;
using Distributr.DataImporter.Lib.ImportService.Shipping;
using Distributr.WPF.Lib.ViewModels.Admin;

namespace Distributr.DataImporter.Lib.ViewModel.FCL
{
    public class ListShipToAddressesViewModel : FCLListingBaseViewModel
    {
        public ObservableCollection<ShipToAddressImportItem> ShipToAddressesImportList { get; set; }
        internal IPagenatedList<ShipToAddressImportItem> PagedList;
        internal IEnumerable<ShipToAddressImport> ImportItems;

        public ListShipToAddressesViewModel()
        {
            ShipToAddressesImportList = new ObservableCollection<ShipToAddressImportItem>();
        }

        protected override void Load(bool isFirstLoad = false)
        {
            if (isFirstLoad)
                SetUp();
            Reload();
        }

        private void SetUp()
        {
            ShipToAddressesImportList.Clear();
            SelectedPath = string.Concat(FileUtility.GetWorkingDirectory("importpath"), @"\CustomerShipTo.txt");
            LoadedImportItem = "shiptoAddress";
            PageTitle = "Outlet Shipping Addresses";
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
                    ImportItems = Using<IShipToAddressImportService>(c).Import(SelectedPath);
                    var imports = ImportItems as List<ShipToAddressImport> ?? ImportItems.ToList();
                    if (imports.Any())
                    {
                        var importready = imports.Select((n, i) => new ShipToAddressImportItem()
                                                                       {
                                                                           SequenceNo = i + 1,
                                                                           IsChecked = false,
                                                                           Name = n.Code,
                                                                           Description = n.Description,
                                                                           OutletCode = n.OutletCode,
                                                                           PhysicalAddress = n.PhysicalAddress,
                                                                           PostalAddress = n.PostalAddress,
                                                                           Latitude = n.Latitude,
                                                                           Longitude = n.Longitude
                                                                       }).ToList();
                        ShipToAddressesImportList.Clear();
                        PagedList = new PagenatedList<ShipToAddressImportItem>(importready.AsQueryable(),
                                                                                             CurrentPage,
                                                                                             ItemsPerPage,
                                                                                             importready.Count());
                        PagedList.ToList().ForEach(ShipToAddressesImportList.Add);

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
            if (ImportItems.Any())
            {
                Import();
            }

        }

        private async void Import( )
        {
            await Task.Run(() =>
                               {
                                   using (var c = NestedContainer)
                                   {
                                       var importService = Using<IShipToAddressImportService>(c);
                                       UploadStatusMessage = "Please wait ...validating your data";

                                       IList<ImportValidationResultInfo> result =
                           new List<ImportValidationResultInfo>();
                                       UploadStatusMessage = "Validation started...Please wait";

                                       int batchSize = 500;
                                       int processed = 0;
                                       var outletAddress =
                                           ImportItems.OrderBy(p => p.OutletCode).Batch(batchSize).Select(
                                               x => x.ToList()).ToList();
                                       while (outletAddress.Any())
                                       {
                                           var current = outletAddress.FirstOrDefault();
                                           if (current != null)
                                           {
                                               var tempresult = importService.Validate(current);
                                               tempresult.ToList().ForEach(result.Add);
                                               outletAddress.Remove(current);
                                               processed += current.Count;
                                               UploadStatusMessage =
                                                   string.Format("{0} of {1} validation Completed...Please wait",
                                                                 processed, ImportItems.Count());
                                           }
                                       }
                                       

                                       if (result.All(p => p.IsValid))
                                       {
                                           UploadStatusMessage = "Uploading data...";

                                          processed = 0;
                                           var validoutletAddress = result.Select(p => (Outlet) p.Entity).ToList();
                                           var batched =
                                               validoutletAddress.OrderBy(p => p.CostCentreCode).Batch(batchSize).Select
                                                   (x => x.ToList()).ToList();
                                           while (batched.Any())
                                           {
                                               var current = batched.FirstOrDefault();
                                               if (current != null)
                                               {
                                                    importService.Save(current);
                                                    batched.Remove(current);
                                                   processed += current.Count;
                                                   UploadStatusMessage =
                                                       string.Format("{0} of {1} Uploaded...Please wait",
                                                                     processed, ImportItems.Count());
                                               }
                                           }
                                     IsUploadSuccess = true;
                                           if (IsUploadSuccess)
                                           {
                                               var productsNotFound = importService.GetNonExistingOutletCodes();
                                               if (productsNotFound.Any())
                                               {
                                                   var error = new StringBuilder();
                                                   error.AppendLine("Non existing Outlets- shipping addresss not imported");
                                                   var batches = productsNotFound.Batch(2).Select(x => x.ToList()).ToList();
                                                   while (batches.Any())
                                                   {
                                                       var current = batches.FirstOrDefault();
                                                       if (current != null)
                                                       {
                                                           error.AppendLine(string.Format("Outlet codes=>{0} ||{1} || {2}", current[0] ?? "", current.Count > 1 ? current[1] : "", current.Count > 2 ? current[2] : ""));
                                                           batches.Remove(current);
                                                       }
                                                   }
                                                   MessageBox.Show(error.ToString(), "Distributr Error", MessageBoxButton.OK);
                                                   FileUtility.LogError(error.ToString());
                                                   UploadStatusMessage ="Done processing all addresses";
                                               }
                                             
                                           }
                                       }
                                       else
                                       {
                                           ShowValidationErrors(result.Where(o => !o.IsValid).ToList());
                                           IsUploadSuccess = false;
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

        public const string SelectedShipToAddressImportPropertyName = "SelectedShipToAddressImport";
        private ShipToAddressImportItem _selectedShipToAddressImport = null;
        public ShipToAddressImportItem SelectedShipToAddressImport
        {
            get { return _selectedShipToAddressImport; }

            set
            {
                if (_selectedShipToAddressImport == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedShipToAddressImportPropertyName);
                _selectedShipToAddressImport = value;
                RaisePropertyChanged(SelectedShipToAddressImportPropertyName);
            }
        }


    }

    public class ShipToAddressImportItem
    {
       public string OutletCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PostalAddress { get; set; }
        public string PhysicalAddress { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public int SequenceNo { get; set; }
        public bool IsChecked { get; set; }
    }
}
