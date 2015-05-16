using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr.Core.Utility;
using Distributr_Middleware.WPF.Lib.ViewModels;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Integration.QuickBooks.Lib.Services;
using Interop.QBFC12;

namespace Integration.QuickBooks.Lib.QBIntegrationViewModels
{
    public abstract partial class QBSalesOrderViewModelBase : MiddleWareViewModelBase
    {
        

        internal List<QuickBooksOrderDocumentDto> PagedList;

        protected QBSalesOrderViewModelBase()
        {
            SalesOrdersList = new ObservableCollection<SaleOrderListItem>();
            ExportCommand = new RelayCommand(Export);
            ExportSelectedCommand = new RelayCommand(ExportSelected);
            ExportAllCommand = new RelayCommand(ExportAll);
            UploadedOutlets = new List<Outlet>();
            UploadedProducts = new List<Product>();

            COGSAccountList = new ObservableCollection<QBAccount>();
            IncomeAccountList = new ObservableCollection<QBAccount>();
            AssetAccountList = new ObservableCollection<QBAccount>();
        }

        #region properties

        public ObservableCollection<QBAccount> AssetAccountList { get; set; }
        public ObservableCollection<QBAccount> IncomeAccountList { get; set; }
        public ObservableCollection<QBAccount> COGSAccountList { get; set; }

        public ObservableCollection<SaleOrderListItem> SalesOrdersList { get; set; }

        public RelayCommand ExportCommand { get; set; }
        public RelayCommand ExportSelectedCommand { get; set; }
        public RelayCommand ExportAllCommand { get; set; }

         
       
        public const string SelectedSalePropertyName = "SelectedSale";
        private SaleOrderListItem _selectedSale = null;
        public SaleOrderListItem SelectedSale
        {
            get { return _selectedSale; }

            set
            {
                if (_selectedSale == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedSalePropertyName);
                _selectedSale = value;
                RaisePropertyChanged(SelectedSalePropertyName);
            }
        }

        public const string StartDatePropertyName = "StartDate";
        private DateTime _startDate = DateTime.Now;
        public DateTime StartDate
        {
            get { return _startDate; }

            set
            {
                if (_startDate == value)
                {
                    return;
                }

                RaisePropertyChanging(StartDatePropertyName);
                _startDate = value;
                RaisePropertyChanged(StartDatePropertyName);
            }
        }

        public const string EndDatePropertyName = "EndDate";
        private DateTime _endDate = DateTime.Now;
        public DateTime EndDate
        {
            get { return _endDate; }

            set
            {
                if (_endDate == value)
                {
                    return;
                }

                RaisePropertyChanging(EndDatePropertyName);
                _endDate = value;
                RaisePropertyChanged(EndDatePropertyName);
            }
        }

        protected QBMainWindowViewModel HomeViewModel
        {
            get { return SimpleIoc.Default.GetInstance<QBMainWindowViewModel>(); }
        }
        #endregion

        #region methods

       
        
        protected override void LoadPage()
        {
            Setup();
            LoadClosedSalesOrders();
        }

        private void LoadClosedSalesOrders()
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    delegate
                        {
                           if(!HomeViewModel.PagedList.Any())
                            TransactionsDownloadService.GetOrdersPendingExport(
                                                          SearchText, IncludeReceiptsAndInvoice);

                           PagedList =HomeViewModel.TransactionsDownloadService.
                                GetTransactions();
                            PagedList.Where(p=>p.OrderType==HomeViewModel.OrderTypeToLoad).Select(MapSelector).ToList().ForEach(SalesOrdersList.Add);

                        }));
        }

        private static SaleOrderListItem MapSelector(QuickBooksOrderDocumentDto n)
        {
            return new SaleOrderListItem()
                       {
                           DocumentDateIssued = n.DocumentDateIssued,
                           GenericReference = n.GenericReference??n.ExternalReference,
                           ExternalReference = n.ExternalReference??n.GenericReference,
                           OrderDateRequired = n.OrderDateRequired,
                           OutletName = n.OutletName,
                           TotalDiscount = n.TotalDiscount,
                           TotalGross = n.TotalGross,
                           TotalNet = n.TotalNet,
                           TotalVAT = n.TotalVat
                       };
        }


        private void Setup()
        {
            UploadedProducts.Clear();
            UploadedOutlets.Clear();
            LoadAccountsLists();
        }

        private void Export()
        {
            if (SelectedSale == null) return;
            SelectedSale.IsSelected = true;
            ExportSelected();
        }

        private void ExportSelected()
        {
            SimpleIoc.Default.GetInstance<QBMainWindowViewModel>().GlobalStatus =
                string.Format("Exporting {0} sales to Quick Books ...", "");
            using (var c = NestedContainer)
            {
               // var addedSaleOrderTemp = new List<SaleOrderListItem>();
                var addedInvoiceRetTemp = new List<IInvoiceRet>();

                SimpleIoc.Default.GetInstance<QBMainWindowViewModel>().GlobalStatus =
                    string.Format("Exporting {0} sales to Quick Books ...", SalesOrdersList.Count(n => n.IsSelected));
                int cnt = 0;
                /*  Random random = new Random();
                  foreach (var sale in SalesOrdersList.Where(n => n.IsSelected))
                  {
                      if (Using<IExportImportAuditRepository>(c).IsDocumentExported(sale.SaleOrder.OrderId, IntegrationModule.QuickBooks)) continue;

                      MainOrder order = Using<IMainOrderRepository>(c).GetById(sale.SaleOrder.OrderId);
                      Invoice invoice = Using<IInvoiceRepository>(c).GetInvoiceByOrderId(order.Id);
                      if (ProcessMasterDataForOrderToUpload(order))
                      {
                          var uploadedSale = Using<IExportImportAuditRepository>(c).GetExportedDocumentById(sale.SaleOrder.OrderReference,
                                                                                             IntegrationModule.QuickBooks
                                                                                             );
                          if (uploadedSale == null)
                          {
                              string externalOrderRef = GetExternalOrderRef(random);
                              var qbOrder = QBIntegrationMethods.AddOrder(order, externalOrderRef);
                              sale.QuickBooksID = qbOrder.TxnID.GetValue();
                              //Using<IExportImportAuditRepository>(c).SaveExportedDocument(sale.SaleOrder.OrderId,
                              //                                                            sale.SaleOrder.OrderReference,
                              //                                                            externalOrderRef,
                              //                                                            order.DocumentType,
                              //                                                            IntegrationModule.QuickBooks);
                          }
                          else sale.QuickBooksID = uploadedSale.ExternalDocumentRef;
                          addedSaleOrderTemp.Add(sale);

                          var uploadedInvoice = Using<IExportImportAuditRepository>(c).GetExportedDocumentById(
                              invoice.Id, IntegrationModule.QuickBooks, DocumentAuditStatus.Exported);
                          if (uploadedInvoice == null)
                          {
                              string externalInvoiceRef = GetExternalOrderRef(random);
                              IInvoiceRet invoiceRet = QBIntegrationMethods.AddInvoice(invoice, sale.QuickBooksID,
                                                                                       order.IssuedOnBehalfOf.Name, 
                                                                                       externalInvoiceRef);
                              addedInvoiceRetTemp.Add(invoiceRet);

                              Using<IExportImportAuditRepository>(c).SaveExportedDocument(invoice.Id, invoice.DocumentReference,
                                                                                            externalInvoiceRef,
                                                                                            order.DocumentType,
                                                                                            IntegrationModule.QuickBooks);
                          }
                          cnt++;

                          SimpleIoc.Default.GetInstance<QBMainWindowViewModel>().GlobalStatus =
                              string.Format("Exported {0} sales of {1} to Quick Books ...", cnt, SalesOrdersList.Count(n => n.IsSelected));
                      }
                  }*/
                LoadClosedSalesOrders();
            }
        }

        private void ExportAll()
        {
            foreach(var sale in SalesOrdersList)
            {
                sale.IsSelected = true;
            }
            ExportSelected();
        }


        string GetExternalOrderRef(Random random)
        {
            string orderRef = StringUtils.GenerateRandomString(8, 8, random);
            if (ExternalOrderRefIsValid(orderRef)) return orderRef;
            return GetExternalOrderRef(random);
        }

        bool ExternalOrderRefIsValid(string ordRef)
        {
            using (var c = NestedContainer)
            {
               // return Using<IExportImportAuditRepository>(c).OrderReferenceIsValid(ordRef);
            }
            return true;
        }

        

        

        #endregion

    }
    public class SaleOrderListItem
    {
        public bool IsSelected { get; set; }
        public string GenericReference { get; set; }
        public string ExternalReference { get; set; }
        public string OutletName { get; set; }
        public string OrderDateRequired { get; set; }
        public string DocumentDateIssued { get; set; }
        public decimal TotalVAT { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalNet { get; set; }
    }
   

}
