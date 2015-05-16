using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.DataImporter.Lib.Utils;
using Distributr.DataImporter.Lib.ImportEntity;
using Distributr.DataImporter.Lib.ImportService.Salesman;
using Distributr.WPF.Lib.ViewModels.Admin;

namespace Distributr.DataImporter.Lib.ViewModel.FCL
{
    public class ListSalesmenToImportViewModel : FCLListingBaseViewModel
    {
        public ObservableCollection<DistributorSalesmanImportItem> SalesmanImportList { get; set; }
        internal IPagenatedList<DistributorSalesmanImportItem> PagedList;
        internal IEnumerable<DistributorSalesmanImport> ImportItems;
        public ListSalesmenToImportViewModel()
        {
            SalesmanImportList = new ObservableCollection<DistributorSalesmanImportItem>();
        }

        protected override void Load(bool isFirstLoad = false)
        {
            if (isFirstLoad)
                SetUp();
                Reload();
        }

        private void SetUp()
        {
            SalesmanImportList.Clear();
            SelectedPath = string.Concat(FileUtility.GetWorkingDirectory("importpath"), @"\Staff.txt");
            LoadedImportItem = "salesmen";
            PageTitle = "Distributor Salesmen";
            UploadStatusMessage = "";
            IsUploadSuccess = false;
            
        }
     

        protected override void Reload()
        {
            if (!FileUtility.ValidateFile(SelectedPath)) return;
             using (var c = NestedContainer)
             {

                 MainWindowViewModel.GlobalStatus = string.Format("Loading: {0}", LoadedImportItem);
                 try
                 {

                     ImportItems = Using<IDistributorSalesmanImportService>(c).Import(SelectedPath);
                     var imports = ImportItems as List<DistributorSalesmanImport> ?? ImportItems.ToList();
                     if (imports.Any())
                     {
                         var importready = imports.Select((n, i) => new DistributorSalesmanImportItem()
                                                                        {
                                                                            SequenceNo = i + 1,
                                                                            DistributorCode = n.DistributorCode,
                                                                            Name = n.Name,
                                                                            PayRollNumber = n.PayRollNumber,
                                                                            IsChecked = false,
                                                                            SalesmanCode = n.SalesmanCode,
                                                                          //  SalesmanPhoneNumber = n.SalesmanPhoneNumber
                                                                        }).ToList();
                         SalesmanImportList.Clear();
                        PagedList = new PagenatedList<DistributorSalesmanImportItem>(importready.AsQueryable(),
                                                                                            CurrentPage,
                                                                                            ItemsPerPage,
                                                                                            importready.Count());
                         PagedList.ToList().ForEach(SalesmanImportList.Add);

                         UpdatePagenationControl();
                         MainWindowViewModel.GlobalStatus = string.Format("Loaded:{0}", LoadedImportItem);
                     }
                 }
                 catch (Exception ex)
                 {
                     MessageBox.Show("Error getting data from file\nDetails=>" + ex.Message);
                     MainWindowViewModel.GlobalStatus = string.Format("Error loading {0}", LoadedImportItem);
                 }
             }

        }

      protected override void UploadAll()
        {
            if (ImportItems.Any())
            {
                var salesmen = ImportItems.Select(n => new DistributorSalesmanImport
                    {
                        SalesmanCode = n.SalesmanCode,
                        Name = n.Name,
                        PayRollNumber = n.PayRollNumber,
                        DistributorCode = n.DistributorCode,
                        //SalesmanPhoneNumber = n.SalesmanPhoneNumber
                    }).ToList();
                Import(salesmen);
            }
            
            else
            {
                MessageBox.Show("No import item","Importer Info",MessageBoxButton.OK);
            }
        }
        internal async void Import(IEnumerable<DistributorSalesmanImport> importItems )
        {
            await Task.Run(() =>
                               {
                                   using (var c = NestedContainer)
                                   {
                                       var importService = Using<IDistributorSalesmanImportService>(c);
                                       UploadStatusMessage = "Please wait ...validating your data";
                                       var result = importService.Validate(importItems.ToList());
                                      if (result.All(s => s.IsValid))
                                       {
                                           UploadStatusMessage = "Uploading data...";
                                           using (var trans = new TransactionScope())
                                           {
                                               importService.Save(
                                                   result.Select(s => s.Entity).OfType<DistributorSalesman>().ToList());

                                               var validUsers = importService.ValidateUsers(importItems.ToList());

                                               importService.Save(
                                                   validUsers.Select(p => p.Entity).OfType<User>().ToList());
                                               IsUploadSuccess = true;
                                               if (IsUploadSuccess)
                                               {
                                                   Application.Current.Dispatcher.BeginInvoke(
                                                       new Action(() =>
                                                                      {
                                                                          int i = 0;
                                                                          foreach (
                                                                              var item in
                                                                                  result.Select(s => s.Entity).OfType
                                                                                      <DistributorSalesman>().
                                                                                      ToList())
                                                                          {
                                                                              SalesmanImportList.Remove(
                                                                                  SalesmanImportList.FirstOrDefault(
                                                                                      s =>
                                                                                      s.SalesmanCode ==
                                                                                      item.CostCentreCode));
                                                                              i++;
                                                                          }
                                                                          UploadStatusMessage = string.Format(
                                                                              "Successfully uploaded {0} salesmen", i);
                                                                      }));

                                               }

                                               else
                                               {
                                                   ShowValidationErrors(result.Where(o => !o.IsValid).ToList());
                                                   IsUploadSuccess = false;
                                               }
                                               trans.Complete();
                                               try
                                               {
                                                   trans.Dispose();
                                               }
                                               catch(Exception ex)
                                               {
                                                   FileUtility.LogError(ex.Message);
                                               }
                                           }
                                       }
                                      else
                                      {
                                          ShowValidationErrors(result.Where(o => !o.IsValid).ToList());
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

        public const string SelectedSalesmanImportItemPropertyName = "SelectedSalesmanImportItem";
        private DistributorSalesmanImportItem _selectedSalesmanImportItem = null;
        public DistributorSalesmanImportItem SelectedSalesmanImportItem
        {
            get { return _selectedSalesmanImportItem; }

            set
            {
                if (_selectedSalesmanImportItem == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedSalesmanImportItemPropertyName);
                _selectedSalesmanImportItem = value;
                RaisePropertyChanged(SelectedSalesmanImportItemPropertyName);
            }
        }
    }

    public class DistributorSalesmanImportItem
    {
        public int SequenceNo { get; set; }
        public string SalesmanCode { get; set; }
        public string Name { get; set; }
        public string PayRollNumber { get; set; }
        public string DistributorCode { get; set; }
        //public string SalesmanPhoneNumber { get; set; }
        public bool IsChecked { get; set; }
    }
}
