using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.ThirdPartyIntegrationRepository;
using Distributr.DataImporter.Lib.ImportEntity;
using Distributr.DataImporter.Lib.ImportService.Orders;
using Distributr.DataImporter.Lib.Utils;
using Distributr.WPF.Lib.ViewModels.Admin;
using GalaSoft.MvvmLight;

namespace Distributr.DataImporter.Lib.ViewModel.FCL
{
    public class ListImportOrdersViewModel : FCLListingBaseViewModel
    {
        internal IPagenatedList<ImportOrderItemSummary> PagedList;
        public ObservableCollection<ImportOrderItemSummary> ImportOrderItemsSummaryList { get; set; }
        internal IEnumerable<ApprovedOrderImport> ImportItems;
        List<string> successfulimportItems;
        List<ImportOrderItemSummary> SelectedImports;

        public ListImportOrdersViewModel()
        {
            ImportOrderItemsSummaryList = new ObservableCollection<ImportOrderItemSummary>();
            successfulimportItems=new List<string>();
            SelectedImports = new List<ImportOrderItemSummary>();
        }

        protected override void Load(bool isFirstLoad = false)
        {
            if (isFirstLoad)
                Setup();
            LoadImportOrders();
        }

        private void Setup()
        {
            ImportOrderItemsSummaryList.Clear();
            UploadStatusMessage = "";
            IsUploadSuccess = false;
           
                LoadedImportItem = "orders";
                PageTitle = "Import Orders";
               
            
        }

        protected override void UploadCurrentPage()
        {
            SelectedImports = ImportOrderItemsSummaryList.ToList();
            Upload();
        }
        private void Upload()
        {
            if (SelectedImports.Any())
            {
                var readyForApproval = SelectedImports.Select(n => new ApprovedOrderImport()
                {
                    OutletCode = n.OutletCode,
                    ProductCode = n.ProductCode,
                    ApprovedQuantity = n.ApprovedQuantity,
                    SalesmanCode = n.SalesmanCode,
                    DistributrCode = n.DistributrCode,
                    OrderReference = string.IsNullOrEmpty(n.DistributrCode) ? GetDistributorCode(n.OrderReference) : ""
                }).AsEnumerable();
                using (var c = NestedContainer)
                {
                    successfulimportItems = Using<IApprovedOrderImportService>(c).Approve(readyForApproval);
                }
                 //importService.Approve(readyForApproval);
                ClearImportedFiles(successfulimportItems);
                UploadStatusMessage = string.Format("{0} orders exported", successfulimportItems.Count);
            }
            else 
            {
                MessageBox.Show("Select atleast one order to import");
                return;
            }
        }
        protected override void UploadSelected()
        {
            SelectedImports = ImportOrderItemsSummaryList.Where(p => p.IsChecked).ToList();
            Upload();
        }

      
        protected override void UploadAll()
        {
            if(!ImportOrderItemsSummaryList.Any())return;
            var uploadItems = ImportItems.Select(n => new ApprovedOrderImport()
                                                          {
                                                              OutletCode = n.OutletCode,
                                                              ProductCode = n.ProductCode,
                                                              ApprovedQuantity = n.ApprovedQuantity,
                                                              SalesmanCode = n.SalesmanCode,
                                                              OrderReference = n.OrderReference,
                                                              DistributrCode = string.IsNullOrEmpty(n.DistributrCode) ? GetDistributorCode(n.OrderReference) : ""
                                                        }).AsEnumerable();
            
            using (var c =NestedContainer)
            {
              successfulimportItems=Using<IApprovedOrderImportService>(c).Approve(uploadItems);
            }
            UploadStatusMessage = string.Format("{0} orders exported", successfulimportItems.Count());
           ClearImportedFiles(successfulimportItems);
        }
        private void ClearImportedFiles(List<string> successimports)
        {
            foreach (var successimport in successimports)
            {
                var exist = ImportOrderItemsSummaryList.FirstOrDefault(p => p.OrderReference == successimport);
                if (exist != null)
                    ImportOrderItemsSummaryList.Remove(exist);
            }
           
        }

        internal void LoadImportOrders()
        {
            UploadStatusMessage = "Getting Approved Orders...Please Wait.....!";
            Application.Current.Dispatcher.BeginInvoke(
                new Action(() =>
                               {
                                   using (var c = NestedContainer)
                                   {
                                       SelectedPath = FileUtility.OpenImportDirectoryPath(); // TODO=>We need a naming convention here..  Path.Combine(FileUtility.GetWorkingDirectory("importpath"),@"approvedorders.csv");

                                       if (FileUtility.ValidateFile(SelectedPath))
                                       {
                                           var importService = Using<IApprovedOrderImportService>(c);
                                           try
                                           {
                                               ImportItems = importService.Import(SelectedPath).ToList();
                                               if (ImportItems.Any())
                                               {
                                                   Load(ImportItems);
                                                   UploadStatusMessage = "";
                                               }
                                           }
                                           catch (Exception ex)
                                           {
                                               MessageBox.Show("Error Importing orders\nDetails:" + ex.Message);

                                           }
                                       }
                                   }
                               }));

        }
        private string GetDistributorCode(string orderreference)
        {
            string code = "";
            using (var c = NestedContainer)
            {
                var repo = Using<IMainOrderRepository>(c);
                var order = repo.GetByDocumentReference(orderreference);
                if(order !=null)
                {
                  var issuer=order.DocumentIssuerCostCentre;
                    code = Using<ICostCentreRepository>(c).GetById(issuer.ParentCostCentre.Id).CostCentreCode;
                }
            }
            return code;
        }

        private void Load(IEnumerable<ApprovedOrderImport> imports)
        {
            using (var c = NestedContainer)
            {
                var importsAuditRepository = Using<IExportImportAuditRepository>(c);
                var validImportsItems =
                    imports.Where(
                        approvedOrderImport => !importsAuditRepository.IsImported(approvedOrderImport.OrderReference)).
                        ToList();
               
                var items = validImportsItems.Select((n, i) => new ImportOrderItemSummary()
                                                         {
                                                             SequenceNo = i + 1,
                                                             DistributrCode =string.IsNullOrEmpty(n.DistributrCode)?GetDistributorCode(n.OrderReference):"",
                                                             OrderReference = n.OrderReference,
                                                             OutletCode = n.OutletCode,
                                                             ProductCode = n.ProductCode,
                                                             OrderDate=n.OrderDate,
                                                            SalesmanCode = n.SalesmanCode,
                                                             ApprovedQuantity = n.ApprovedQuantity
                                                         }).AsQueryable();

                PagedList = new PagenatedList<ImportOrderItemSummary>(items, CurrentPage, ItemsPerPage,
                                                                      items.Count());

                PagedList.ToList().ForEach(ImportOrderItemsSummaryList.Add);

                UpdatePagenationControl();
                
                if (!validImportsItems.Any())
                    UploadStatusMessage = "All orders in file already imported";
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
            UpdatePagenationControlBase(PagedList.PageNumber, PagedList.PageCount, PagedList.TotalItemCount,
                                        PagedList.IsFirstPage, PagedList.IsLastPage);

        }
    }


    public class ImportOrderItemSummary:ViewModelBase
    {
        public int SequenceNo { get; set; }
        public string OrderReference { get; set; }
    
        public string DistributrCode { get; set; }
     
        public string SalesmanCode { get; set; }
       
        public string OutletCode { get; set; }
        
        public string ProductCode { get; set; }
       
        public decimal ApprovedQuantity { get; set; }

        public string OrderDate { get; set; }
       
       // public string ShipToAddress { get; set; }
        
        public const string IsCheckedPropertyName = "IsChecked";
        private bool _isChecked ;
        public bool IsChecked
        {
            get { return _isChecked; }

            set
            {
                if (_isChecked == value)
                {
                    return;
                }

                RaisePropertyChanging(IsCheckedPropertyName);
                _isChecked = value;
                RaisePropertyChanged(IsCheckedPropertyName);
            }
        }
    }
}
