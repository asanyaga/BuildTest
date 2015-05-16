using System;
using System.Diagnostics.CodeAnalysis;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.ProductRepositories;

using Distributr.WPF.Lib.ViewModels.Admin.Contacts;
using Distributr.WPF.Lib.ViewModels.Admin.Outlets;
using Distributr.WPF.Lib.ViewModels.Admin.ReorderLevel;
using Distributr.WPF.Lib.ViewModels.Admin.Routes;

using Distributr.WPF.Lib.ViewModels.Admin.SalesmanSuppliers;
using Distributr.WPF.Lib.ViewModels.Admin.SalesmanTargets;
using Distributr.WPF.Lib.ViewModels.Admin.Users;
using Distributr.WPF.Lib.ViewModels.ApplicSettings;
using Distributr.WPF.Lib.ViewModels.IntialSetup;
using Distributr.WPF.Lib.ViewModels.MainPage;
using Distributr.WPF.Lib.ViewModels.PrintableDocuments;
using Distributr.WPF.Lib.ViewModels.Reports;
using Distributr.WPF.Lib.ViewModels.Sync;
using Distributr.WPF.Lib.ViewModels.Test;
using Distributr.WPF.Lib.ViewModels.TestFrames;
using Distributr.WPF.Lib.ViewModels.Transactional.AuditLogs;
using Distributr.WPF.Lib.ViewModels.Transactional.CN;
using Distributr.WPF.Lib.ViewModels.Transactional.CommodityPurchase;
using Distributr.WPF.Lib.ViewModels.Transactional.DisbursementNotes;
using Distributr.WPF.Lib.ViewModels.Transactional.DispatchProducts;
using Distributr.WPF.Lib.ViewModels.Transactional.GRN;
using Distributr.WPF.Lib.ViewModels.Transactional.IAN;
using Distributr.WPF.Lib.ViewModels.Transactional.ITN;
using Distributr.WPF.Lib.ViewModels.Transactional.InventoryViewModels;
using Distributr.WPF.Lib.ViewModels.Transactional.InvoiceDocument;
using Distributr.WPF.Lib.ViewModels.Transactional.Order_Pos;
using Distributr.WPF.Lib.ViewModels.Transactional.Order_Purchase;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders.OrderProduct;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders_Stockist;

using Distributr.WPF.Lib.ViewModels.Transactional.Payments;

using Distributr.WPF.Lib.ViewModels.Transactional.RN;
using Distributr.WPF.Lib.ViewModels.Transactional.ReceiptDocument;

using Distributr.WPF.Lib.ViewModels.Transactional.TransactionSatement;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using StructureMap;
using System.Linq;

namespace Distributr.WPF.Lib.Data.IOC
{
    public class ViewModelLocator
    {

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<OrderFormViewModel>();
            SimpleIoc.Default.Register<AddEditContactViewModel>();
            SimpleIoc.Default.Register<SalesmanOrderListingViewModel>();
            SimpleIoc.Default.Register<ViewOrderViewModel>();
            SimpleIoc.Default.Register<ProductMainOrderLineItem>();
            SimpleIoc.Default.Register<CustomMessageBoxViewModel>();
            SimpleIoc.Default.Register<OrderApprovalViewModel>();
            SimpleIoc.Default.Register<PurchaseOrderFormViewModel>();
            SimpleIoc.Default.Register<PurchaseOrderListingViewModel>();
            SimpleIoc.Default.Register<ViewPurchaseOrderViewModel>();
            SimpleIoc.Default.Register<ListPOSViewModel>();
            SimpleIoc.Default.Register<ViewOrderPOSViewModel>();
            SimpleIoc.Default.Register<AddPOSViewModel>();
            SimpleIoc.Default.Register<PaymentModePopUpViewModel>();
            SimpleIoc.Default.Register <InvoiceDocumentViewModel>();
            SimpleIoc.Default.Register<OrderDispatchViewModel>();
            SimpleIoc.Default.Register<ReceiptDocumentViewModel>();
            SimpleIoc.Default.Register<AddGRNViewModel>();
            SimpleIoc.Default.Register<ListOutStandingPaymentsViewModel>();
            SimpleIoc.Default.Register<ChangeDeliveryPersonViewModel>();
            SimpleIoc.Default.Register<DocumentReportViewerViewModel>();
            SimpleIoc.Default.Register<EditSalesmanTargetsViewModel>();
            SimpleIoc.Default.Register<SyncViewModel>();
            SimpleIoc.Default.Register < ItemLookUpsLookUpViewModel>();
            SimpleIoc.Default.Register<UnderBankingViewModel>();
            SimpleIoc.Default.Register<UnderBankingListViewModel>();
            SimpleIoc.Default.Register<UnderBankingCollectionViewModel>();
            SimpleIoc.Default.Register<SalesmanOrderSummaryListingViewModel>();
            SimpleIoc.Default.Register<MainPageViewModel>();
            SimpleIoc.Default.Register<ReceiptVm>();
            SimpleIoc.Default.Register<StockistPurchaseOrderFormViewModel>();
            SimpleIoc.Default.Register<StockistPurchaseOrderListingViewModel>();
            SimpleIoc.Default.Register<ViewStockistPurchaseOrderViewModel>();
            SimpleIoc.Default.Register<StockistPurchaseOrderApprovalFormViewModel>();
            SimpleIoc.Default.Register<NewListContactViewModel>();
            SimpleIoc.Default.Register<ImportSalesmanInventoryViewModel>();
            SimpleIoc.Default.Register<SalesmanSupplierViewModel>();
            SimpleIoc.Default.Register<ListRoutesViewModel>();
            
            


            
            
            
            RegisterViewModelMessage();
        }

        private static void RegisterViewModelMessage()
        {
            var viewOrder = SimpleIoc.Default.GetInstance<ViewOrderViewModel>();
            Messenger.Default.Register<ViewModelMessage>(viewOrder, viewOrder.GetId);

            var createOrder = SimpleIoc.Default.GetInstance<OrderFormViewModel>();
            Messenger.Default.Register<OrderContinueMessage>(createOrder, createOrder.Continue);
             var createsaleOrder = SimpleIoc.Default.GetInstance<AddPOSViewModel>();
             Messenger.Default.Register<SaleOrderContinueMessage>(createsaleOrder, createsaleOrder.Continue);
             var createpurchaseOrder = SimpleIoc.Default.GetInstance<PurchaseOrderFormViewModel>();
             Messenger.Default.Register<PurchaseOrderContinueMessage>(createpurchaseOrder, createpurchaseOrder.Continue);

             var createstockistpurchaseOrder = SimpleIoc.Default.GetInstance<StockistPurchaseOrderFormViewModel>();
             Messenger.Default.Register<StockistPurchaseOrderContinueMessage>(createstockistpurchaseOrder, createstockistpurchaseOrder.Continue);
            

            var viewPOS = SimpleIoc.Default.GetInstance<ViewOrderPOSViewModel>();
            Messenger.Default.Register<ViewModelMessage>(viewPOS, viewPOS.GetId);

            var viewPurchase = SimpleIoc.Default.GetInstance<ViewPurchaseOrderViewModel>();
            Messenger.Default.Register<ViewModelMessage>(viewPurchase, viewPurchase.GetId);

            var viewstockistPurchase = SimpleIoc.Default.GetInstance<ViewStockistPurchaseOrderViewModel>();
            Messenger.Default.Register<ViewModelMessage>(viewstockistPurchase, viewstockistPurchase.GetId);

            var orderApproval = SimpleIoc.Default.GetInstance<OrderApprovalViewModel>();
            Messenger.Default.Register<Guid>(viewOrder, orderApproval.SetId);
            var invoicedoc = SimpleIoc.Default.GetInstance<InvoiceDocumentViewModel>();
            Messenger.Default.Register<ViewModelMessage>(invoicedoc, invoicedoc.SetId);
            var receiptdoc = SimpleIoc.Default.GetInstance<ReceiptDocumentViewModel>();
            Messenger.Default.Register<ViewModelMessage>(receiptdoc, receiptdoc.SetId);

            var rcpt = SimpleIoc.Default.GetInstance<ReceiptVm>();
            Messenger.Default.Register<ViewModelMessage>(rcpt, rcpt.SetId);

            var sync = SimpleIoc.Default.GetInstance<SyncViewModel>();
            Messenger.Default.Register<string>(sync, sync.ReceiveMessage);

            var underBankingViewModel = SimpleIoc.Default.GetInstance<UnderBankingViewModel>();
            Messenger.Default.Register<UnderBankingSetup>(underBankingViewModel, underBankingViewModel.SetSalesman);

            var underBankingCollectionViewModel = SimpleIoc.Default.GetInstance<UnderBankingCollectionViewModel>();
            Messenger.Default.Register<Guid>(underBankingCollectionViewModel, underBankingCollectionViewModel.SetUnderBankingId);
            var approvestocktakeOrder = SimpleIoc.Default.GetInstance<StockistPurchaseOrderApprovalFormViewModel>();
            Messenger.Default.Register<ApproveStockistPurchaseOrderMessage>(approvestocktakeOrder, approvestocktakeOrder.SetOrderToApprove);

            
            
        }

        public ListRoutesViewModel ListRoutesViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ListRoutesViewModel>();
            }
        }
        public ImportSalesmanInventoryViewModel ImportSalesmanInventoryViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ImportSalesmanInventoryViewModel>();
            }
        }
        public ReceiptVm ReceiptVm
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ReceiptVm>();
            }
        }

        public StockistPurchaseOrderApprovalFormViewModel StockistPurchaseOrderApprovalFormViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<StockistPurchaseOrderApprovalFormViewModel>();
            }
        }
         public StockistPurchaseOrderFormViewModel StockistPurchaseOrderFormViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<StockistPurchaseOrderFormViewModel>();
            }
        }
        public StockistPurchaseOrderListingViewModel StockistPurchaseOrderListingViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<StockistPurchaseOrderListingViewModel>();
            }
        }
        public ViewStockistPurchaseOrderViewModel ViewStockistPurchaseOrderViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewStockistPurchaseOrderViewModel>();
            }
        }
        



        /// <summary>
        /// Gets the UnderBankingCollectionViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SalesmanOrderSummaryListingViewModel SalesmanOrderSummaryListingViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SalesmanOrderSummaryListingViewModel>();
            }
        }
        /// <summary>
        /// Gets the UnderBankingCollectionViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public UnderBankingCollectionViewModel UnderBankingCollectionViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<UnderBankingCollectionViewModel>();
            }
        }

        /// <summary>
        /// Gets the UnderBankingViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public UnderBankingListViewModel UnderBankingListViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<UnderBankingListViewModel>();
            }
        }

        /// <summary>
        /// Gets the UnderBankingViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public UnderBankingViewModel UnderBankingViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<UnderBankingViewModel>();
            }
        }

        /// <summary>
        /// Gets the MainPageViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainPageViewModel MainPageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainPageViewModel>();

            }
        }


        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ItemLookUpsLookUpViewModel ItemLookUpsLookUpViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ItemLookUpsLookUpViewModel>();
            }
        }

        #region AddEditContactViewModel

        /// <summary>
        /// Gets the ChangeDeliveryPersonViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ChangeDeliveryPersonViewModel ChangeDeliveryPersonViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ChangeDeliveryPersonViewModel>(); }
        }

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public AddEditContactViewModel AddEditContactViewModel
        {
            get { return ServiceLocator.Current.GetInstance<AddEditContactViewModel>(); }
        }

        #endregion

        #region EditSalesmanTargetsViewModel
        [SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public EditSalesmanTargetsViewModel EditSalesmanTargetsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<EditSalesmanTargetsViewModel>(); }
        }
        #endregion

        #region OrderDispatchViewModel

        /// <summary>
        /// Gets the OrderDispatchViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public OrderDispatchViewModel OrderDispatchViewModel
        {
            get { return ServiceLocator.Current.GetInstance<OrderDispatchViewModel>(); }
        }

        #endregion

        #region ListOutStandingPaymentsViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public ListOutStandingPaymentsViewModel ListOutStandingPaymentsViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ListOutStandingPaymentsViewModel>();
            }
        }
        #endregion
        
        #region PaymentModePopUpViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public PaymentModePopUpViewModel PaymentModePopUpViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PaymentModePopUpViewModel>();
            }
        }
        #endregion
        
        #region POS

        #region ViewOrderPOSViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public ViewOrderPOSViewModel ViewOrderPOSViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewOrderPOSViewModel>();
            }
        }
        #endregion

        #region AddPOSViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public AddPOSViewModel AddPOSViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AddPOSViewModel>();
            }
        }


        #endregion

        #region ListPOSViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public ListPOSViewModel  ListPOSViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ListPOSViewModel>();
            }
        }


        #endregion

        #endregion

        #region OrderApprovalViewModel

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public OrderApprovalViewModel OrderApprovalViewModel
        {
            get { return ServiceLocator.Current.GetInstance<OrderApprovalViewModel>(); }
        }

        #endregion

        #region AddGRNViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public AddGRNViewModel AddGRNViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AddGRNViewModel>();
            }
        }


        #endregion

        #region ReceiptDocumentViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ReceiptDocumentViewModel ReceiptDocumentViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ReceiptDocumentViewModel>();
            }
        }
        #endregion

        #region InvoiceDocumentViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public InvoiceDocumentViewModel InvoiceDocumentViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<InvoiceDocumentViewModel>();
            }
        }
        #endregion

        #region CustomMessageBoxViewModel

        /// <summary>
        /// Gets the CustomMessageBoxViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public CustomMessageBoxViewModel CustomMessageBoxViewModel
        {
            get { return ServiceLocator.Current.GetInstance<CustomMessageBoxViewModel>(); }
        }

        #endregion

        #region ProductMainOrderLineItem

        /// <summary>
        /// Gets the ProductMainOrderLineItem property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ProductMainOrderLineItem ProductMainOrderLineItem
        {
            get { return ServiceLocator.Current.GetInstance<ProductMainOrderLineItem>(); }
        }

        #endregion

        #region TestViewModel

        private static TestViewModel _testViewModel;
        private static ProductPackagingTestViewModel _productPackagingTestViewModel;

        /// <summary>
        /// Gets the TestViewModel property.
        /// </summary>
        public static TestViewModel TestViewModelStatic
        {
            get
            {
                if (_testViewModel == null)
                {
                    CreateTestViewModel();
                }

                return _testViewModel;
            }
        }

        /// <summary>
        /// Gets the TestViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public TestViewModel TestViewModel
        {
            get
            {
                return TestViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the TestViewModel property.
        /// </summary>
        public static void ClearTestViewModel()
        {
            _testViewModel.Cleanup();
            _testViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the TestViewModel property.
        /// </summary>
        public static void CreateTestViewModel()
        {
            if (_testViewModel == null)
            {
                //trigger an add to productbrand for testing
                IProductBrandRepository pbr = ObjectFactory.GetInstance<IProductBrandRepository>();
                if (pbr.GetAll().Count() == 0)
                {
                    //ProductBrandLocal l1 = new ProductBrandLocal
                    //{
                    //    Code = "code1",
                    //    DateCreated = DateTime.Now,
                    //    DateLastUpdated = DateTime.Now,
                    //    Description = "desc1",
                    //    StatusId = (int)EntityStatus.Active,
                    //    MasterId = Guid.NewGuid(),
                    //    Name = "name1",
                    //};
                   // pbr.AddOrReplace(l1);
                    //ProductBrandLocal l2 = new ProductBrandLocal
                    //{
                    //    Code = "code2",
                    //    DateCreated = DateTime.Now,
                    //    DateLastUpdated = DateTime.Now,
                    //    Description = "desc2",
                    //    StatusId =(int)EntityStatus.Active ,
                    //    MasterId = Guid.NewGuid(),
                    //    Name = "name2",
                    //};
                    //pbr.AddOrReplace(l2);
                }
                _testViewModel = new TestViewModel();


            }
        }

        #endregion

        #region ProductPackagingTestViewModel

        /// <summary>
        /// Provides a deterministic way to delete the ProductPackagingTestViewModel property.
        /// </summary>
        public static ProductPackagingTestViewModel ProductPackagingTestStatic
        {
            get
            {
                if (_productPackagingTestViewModel == null)
                {
                    CreateProductPackagingTestViewModel();
                }
                return _productPackagingTestViewModel;
            }
        }
        /// <summary>
        /// Gets the TestViewModel property.
        /// </summary>

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ProductPackagingTestViewModel ProductPackagingViewModel
        {
            get
            {
                return ProductPackagingTestStatic;
            }
        }
        /// <summary>
        /// Provides a deterministic way to delete the ProductPackagingViewModel property.
        /// </summary>
        public static void ClearProductPackagingViewModel()
        {
            _productPackagingTestViewModel.Cleanup();
            _productPackagingTestViewModel = null;
        }
        /// <summary>
        /// Provides a deterministic way to create the ProductPackagingViewModel property.
        /// </summary>
        public static void CreateProductPackagingTestViewModel()
        {
            if (_productPackagingTestViewModel == null)
            {
                //Trigger to add productpackaging for testing
                //if (ppr.GetAll().Count() == 0)
                //{
                //    ProductPackagingLocal ppl = new ProductPackagingLocal
                //    {
                //        DateCreated = DateTime.Now,
                //        MasterId = Guid.NewGuid(),
                //        Description = "300ml RGBx24",
                //        Name = "Coke 300ml",
                //        StatusId =(int)EntityStatus.Active ,
                //        DateLastUpdated = DateTime.Now
                //    };
                //    //ppr.AddOrReplace(ppl);
                //    ProductPackagingLocal ppl2 = new ProductPackagingLocal
                //    {
                //        DateCreated = DateTime.Now,
                //        MasterId = Guid.NewGuid(),
                //        Description = "300ml RGBx24",
                //        Name = "Coke 300ml",
                //        StatusId =(int)EntityStatus.Active ,
                //        DateLastUpdated = DateTime.Now
                //    };
                //   // ppr.AddOrReplace(ppl2);
                //}
                _productPackagingTestViewModel = new ProductPackagingTestViewModel();
            }
        }

        #endregion

        #region SyncView
        //private static SyncViewModel _syncViewModel;

        ///// <summary>
        ///// Gets the SyncViewModel property.
        ///// </summary>
        //public static SyncViewModel SyncViewModelStatic
        //{
        //    get
        //    {
        //        if (_syncViewModel == null)
        //        {
        //            CreateSyncViewModel();
        //        }

        //        return _syncViewModel;
        //    }
        //}

        ///// <summary>
        ///// Gets the SyncViewModel property.
        ///// </summary>
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        //    "CA1822:MarkMembersAsStatic",
        //    Justification = "This non-static member is needed for data binding purposes.")]
        //public SyncViewModel SyncViewModel
        //{
        //    get
        //    {
        //        return SyncViewModelStatic;
        //    }
        //}

        ///// <summary>
        ///// Provides a deterministic way to delete the SyncViewModel property.
        ///// </summary>
        //public static void ClearSyncViewModel()
        //{
        //    _syncViewModel.Cleanup();
        //    _syncViewModel = null;
        //}

        ///// <summary>
        ///// Provides a deterministic way to create the SyncViewModel property.
        ///// </summary>
        //public static void CreateSyncViewModel()
        //{
        //    if (_syncViewModel == null)
        //    {
        //        _syncViewModel = new SyncViewModel();
        //    }
        //}

        #region SyncViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SyncViewModel SyncViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SyncViewModel>();
            }
        }
        #endregion 
        #endregion
        
        #region ListOutlets

        private static ListOutletsViewModel _listOutletsViewModel;

        /// <summary>
        /// Gets the ListOutletsViewModel property.
        /// </summary>
        public static ListOutletsViewModel ListOutletsViewModelStatic
        {
            get
            {
                if (_listOutletsViewModel == null)
                {
                    CreateListOutletsViewModel();
                }

                return _listOutletsViewModel;
            }
        }

        /// <summary>
        /// Gets the ListOutletsViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListOutletsViewModel ListOutletsViewModel
        {
            get
            {
                return ListOutletsViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ListOutletsViewModel property.
        /// </summary>
        public static void ClearListOutletsViewModel()
        {
            _listOutletsViewModel.Cleanup();
            _listOutletsViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ListOutletsViewModel property.
        /// </summary>
        public static void CreateListOutletsViewModel()
        {
            if (_listOutletsViewModel == null)
            {
                _listOutletsViewModel = new ListOutletsViewModel();
            }
        }

        #endregion

        #region EditOutletViewModel

        private static EditOutletViewModel _editOutletViewModel;

        /// <summary>
        /// Gets the EditOutletViewModel property.
        /// </summary>
        public static EditOutletViewModel EditOutletViewModelStatic
        {
            get
            {
                if (_editOutletViewModel == null)
                {
                    CreateEditOutletViewModel();
                }

                return _editOutletViewModel;
            }
        }

        /// <summary>
        /// Gets the EditOutletViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditOutletViewModel EditOutletViewModel
        {
            get
            {
                return EditOutletViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the EditOutletViewModel property.
        /// </summary>
        public static void ClearEditOutletViewModel()
        {
            _editOutletViewModel.Cleanup();
            _editOutletViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the EditOutletViewModel property.
        /// </summary>
        public static void CreateEditOutletViewModel()
        {
            if (_editOutletViewModel == null)
            {
                _editOutletViewModel = new EditOutletViewModel();
            }
        }


        #endregion

        #region ListUsers
        private static ListUsersViewModel _listUsersViewModel;

        /// <summary>
        /// Gets the ListUsersViewModel property.
        /// </summary>
        public static ListUsersViewModel ListUsersViewModelStatic
        {
            get
            {
                if (_listUsersViewModel == null)
                {
                    CreateListUsersViewModel();
                }

                return _listUsersViewModel;
            }
        }

        /// <summary>
        /// Gets the ListUsersViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListUsersViewModel ListUsersViewModel
        {
            get
            {
                return ListUsersViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ListUsersViewModel property.
        /// </summary>
        public static void ClearListUsersViewModel()
        {
            _listUsersViewModel.Cleanup();
            _listUsersViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ListUsersViewModel property.
        /// </summary>
        public static void CreateListUsersViewModel()
        {
            if (_listUsersViewModel == null)
            {
                _listUsersViewModel = new ListUsersViewModel();
            }
        }

        #endregion

        #region EditUsers
        private static EditUsersViewModel _editUsersViewModel;

        /// <summary>
        /// Gets the EditUsersViewModel property.
        /// </summary>
        public static EditUsersViewModel EditUsersViewModelStatic
        {
            get
            {
                if (_editUsersViewModel == null)
                {
                    CreateEditUsersViewModel();
                }

                return _editUsersViewModel;
            }
        }

        /// <summary>
        /// Gets the EditUsersViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditUsersViewModel EditUsersViewModel
        {
            get
            {
                return EditUsersViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the EditUsersViewModel property.
        /// </summary>
        public static void ClearEditUsersViewModel()
        {
            _editUsersViewModel.Cleanup();
            _editUsersViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the EditUsersViewModel property.
        /// </summary>
        public static void CreateEditUsersViewModel()
        {
            if (_editUsersViewModel == null)
            {
                _editUsersViewModel = new EditUsersViewModel();
            }
        }
        #endregion

        #region ListRoutes

      

        #region Edit Route ViewModel
        private static EditRouteViewModel _editRouteViewModel;

        /// <summary>
        /// Gets the ViewModelPropertyName property.
        /// </summary>
        public static EditRouteViewModel EditRouteViewModelStatic
        {
            get
            {
                if (_editRouteViewModel == null)
                {
                    CreateEditRouteViewModel();
                }

                return _editRouteViewModel;
            }
        }

        /// <summary>
        /// Gets the ViewModelPropertyName property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditRouteViewModel EditRouteViewModel
        {
            get
            {
                return EditRouteViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ViewModelPropertyName property.
        /// </summary>
        public static void ClearEditRouteViewModel()
        {
            _editRouteViewModel.Cleanup();
            _editRouteViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ViewModelPropertyName property.
        /// </summary>
        public static void CreateEditRouteViewModel()
        {
            if (_editRouteViewModel == null)
            {
                _editRouteViewModel = new EditRouteViewModel();
            }
        }


        #endregion

        


        #endregion

    
      
        #region _ListITNViewModel
        private static ListITNViewModel _ListITNViewModelPropertyName;

        /// <summary>
        /// Gets the ListITNViewModel property.
        /// </summary>
        public static ListITNViewModel ListITNViewModelStatic
        {
            get
            {
                if (_ListITNViewModelPropertyName == null)
                {
                    CreateListITNViewModel();
                }

                return _ListITNViewModelPropertyName;
            }
        }

        /// <summary>
        /// Gets the ListITNViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListITNViewModel ListITNViewModel
        {
            get
            {
                return ListITNViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ListITNViewModel property.
        /// </summary>
        public static void ClearListITNViewModel()
        {
            _ListITNViewModelPropertyName.Cleanup();
            _ListITNViewModelPropertyName = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ListITNViewModel property.
        /// </summary>
        public static void CreateListITNViewModel()
        {
            if (_ListITNViewModelPropertyName == null)
            {
                _ListITNViewModelPropertyName = new ListITNViewModel();
            }
        }
        #endregion

        #region ListInventoryViewModel
        private static ListInventoryViewModel _ListInventoryViewModel;

        /// <summary>
        /// Gets the ListInventoryViewModel property.
        /// </summary>
        public static ListInventoryViewModel ListInventoryViewModelStatic
        {
            get
            {
                if (_ListInventoryViewModel == null)
                {
                    CreateListInventoryViewModel();
                }

                return _ListInventoryViewModel;
            }
        }

        /// <summary>
        /// Gets the ListInventoryViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListInventoryViewModel ListInventoryViewModel
        {
            get
            {
                return ListInventoryViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ListInventoryViewModel property.
        /// </summary>
        public static void ClearListInventoryViewModel()
        {
            _ListInventoryViewModel.Cleanup();
            _ListInventoryViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ListInventoryViewModel property.
        /// </summary>
        public static void CreateListInventoryViewModel()
        {
            if (_ListInventoryViewModel == null)
            {
                _ListInventoryViewModel =new ListInventoryViewModel();
            }
        }
        #endregion

        #region ListGRNViewModel

        private static ListGRNViewModel _listGRNViewModel;

        /// <summary>
        /// Gets the ListGRNViewModel property.
        /// </summary>
        public static ListGRNViewModel ListGRNViewModelStatic
        {
            get
            {
                if (_listGRNViewModel == null)
                {
                    CreateListGRNViewModel();
                }

                return _listGRNViewModel;
            }
        }

        /// <summary>
        /// Gets the ListGRNViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListGRNViewModel ListGRNViewModel
        {
            get
            {
                return ListGRNViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ListGRNViewModel property.
        /// </summary>
        public static void ClearListGRNViewModel()
        {
            _listGRNViewModel.Cleanup();
            _listGRNViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ListGRNViewModel property.
        /// </summary>
        public static void CreateListGRNViewModel()
        {
            if (_listGRNViewModel == null)
            {
                _listGRNViewModel = new ListGRNViewModel(); 
            }
        }

        #endregion

        #region ListIANViewModel
        private static ListIANViewModel _ListIANViewModel;

        /// <summary>
        /// Gets the ListIANViewModel property.
        /// </summary>
        public static ListIANViewModel ListIANViewModelStatic
        {
            get
            {
                if (_ListIANViewModel == null)
                {
                    CreateListIANViewModel();
                }

                return _ListIANViewModel;
            }
        }

        /// <summary>
        /// Gets the ListIANViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListIANViewModel ListIANViewModel
        {
            get
            {
                return ListIANViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ListIANViewModel property.
        /// </summary>
        public static void ClearListIANViewModel()
        {
            _ListIANViewModel.Cleanup();
            _ListIANViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ListIANViewModel property.
        /// </summary>
        public static void CreateListIANViewModel()
        {
            if (_ListIANViewModel == null)
            {
                _ListIANViewModel = new ListIANViewModel();
            }
        }

        #endregion

        
        #region GRNItemModalViewModel

        private static GRNItemModalViewModel _grnItemModalViewModel;

        /// <summary>
        /// Gets the GRNItemModalViewModel property.
        /// </summary>
        public static GRNItemModalViewModel GRNItemModalViewModelStatic
        {
            get
            {
                if (_grnItemModalViewModel == null)
                {
                    CreateGRNItemModalViewModel();
                }

                return _grnItemModalViewModel;
            }
        }

        /// <summary>
        /// Gets the GRNItemModalViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public GRNItemModalViewModel GRNItemModalViewModel
        {
            get
            {
                return GRNItemModalViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the GRNItemModalViewModel property.
        /// </summary>
        public static void ClearGRNItemModalViewModel()
        {
            _grnItemModalViewModel.Cleanup();
            _grnItemModalViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the GRNItemModalViewModel property.
        /// </summary>
        public static void CreateGRNItemModalViewModel()
        {
            if (_grnItemModalViewModel == null)
            {
                _grnItemModalViewModel = new GRNItemModalViewModel();
            }
        }


        #endregion

    

     

   

        #region ITNLineItemViewModel
        private static ITNLineItemViewModel _ITNLineItemViewModel;

        /// <summary>
        /// Gets the ITNLineItemViewModel property.
        /// </summary>
        public static ITNLineItemViewModel ITNLineItemViewModelStatic
        {
            get
            {
                if (_ITNLineItemViewModel == null)
                {
                    CreateITNLineItemViewModel();
                }

                return _ITNLineItemViewModel;
            }
        }

        /// <summary>
        /// Gets the ITNLineItemViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ITNLineItemViewModel ITNLineItemViewModel
        {
            get
            {
                return ITNLineItemViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ITNLineItemViewModel property.
        /// </summary>
        public static void ClearITNLineItemViewModel()
        {
            _ITNLineItemViewModel.Cleanup();
            _ITNLineItemViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ITNLineItemViewModel property.
        /// </summary>
        public static void CreateITNLineItemViewModel()
        {
            if (_ITNLineItemViewModel == null)
            {
                _ITNLineItemViewModel = new ITNLineItemViewModel();
            }
        }

        ///// <summary>
        ///// Cleans up all the resources.
        ///// </summary>
        //public static void Cleanup()
        //{
        //    ClearITNLineItemViewModel();
        //}
        #endregion

        #region     _IANLineItemViewModel
        private static IANLineItemViewModel _ianLineItemViewModel;

        /// <summary>
        /// Gets the IANLineItemViewModel property.
        /// </summary>
        public static IANLineItemViewModel IANLineItemViewModelStatic
        {
            get
            {
                if (_ianLineItemViewModel == null)
                {
                    CreateIANLineItemViewModel();
                }

                return _ianLineItemViewModel;
            }
        }

        /// <summary>
        /// Gets the IANLineItemViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public IANLineItemViewModel IANLineItemViewModel
        {
            get
            {
                return IANLineItemViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the IANLineItemViewModel property.
        /// </summary>
        public static void ClearIANLineItemViewModel()
        {
            _ianLineItemViewModel.Cleanup();
            _ianLineItemViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the IANLineItemViewModel property.
        /// </summary>
        public static void CreateIANLineItemViewModel()
        {
            if (_ianLineItemViewModel == null)
            {
                _ianLineItemViewModel = new IANLineItemViewModel();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>

        private static IANLineItemViewModel _IANLineItemViewModel;

        /// <summary>
        /// Gets the ViewModelPropertyName property.
        /// </summary>
        public static IANLineItemViewModel ViewModelPropertyNameStatic
        {
            get
            {
                if (_IANLineItemViewModel == null)
                {
                    CreateViewModelPropertyName();
                }

                return _IANLineItemViewModel;
            }
        }

        /// <summary>
        /// Gets the ViewModelPropertyName property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public IANLineItemViewModel ViewModelPropertyName
        {
            get
            {
                return ViewModelPropertyNameStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ViewModelPropertyName property.
        /// </summary>
        public static void ClearViewModelPropertyName()
        {
            _IANLineItemViewModel.Cleanup();
            _IANLineItemViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ViewModelPropertyName property.
        /// </summary>
        public static void CreateViewModelPropertyName()
        {
            if (_IANLineItemViewModel == null)
            {
                _IANLineItemViewModel = new IANLineItemViewModel(); ;
            }
        }

        #endregion

        #region EditITNViewModel
        private static EditITNViewModel _EditITNViewModel;

        /// <summary>
        /// Gets the EditITNViewModel property.
        /// </summary>
        public static EditITNViewModel EditITNViewModelStatic
        {
            get
            {
                if (_EditITNViewModel == null)
                {
                    CreateEditITNViewModel();
                }

                return _EditITNViewModel;
            }
        }

        /// <summary>
        /// Gets the EditITNViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditITNViewModel EditITNViewModel
        {
            get
            {
                return EditITNViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the EditITNViewModel property.
        /// </summary>
        public static void ClearEditITNViewModel()
        {
            _EditITNViewModel.Cleanup();
            _EditITNViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the EditITNViewModel property.
        /// </summary>
        public static void CreateEditITNViewModel()
        {
            if (_EditITNViewModel == null)
            {
                _EditITNViewModel = new EditITNViewModel();
            }
        }
        #endregion

        #region EditIANViewModel
        private static EditIANViewModel _EditIANViewModel;

        /// <summary>
        /// Gets the EditIANViewModel property.
        /// </summary>
        public static EditIANViewModel EditIANViewModelStatic
        {
            get
            {
                if (_EditIANViewModel == null)
                {
                    CreateEditIANViewModel();
                }

                return _EditIANViewModel;
            }
        }

        /// <summary>
        /// Gets the EditIANViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditIANViewModel EditIANViewModel
        {
            get
            {
                return EditIANViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the EditIANViewModel property.
        /// </summary>
        public static void ClearEditIANViewModel()
        {
            _EditIANViewModel.Cleanup();
            _EditIANViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the EditIANViewModel property.
        /// </summary>
        public static void CreateEditIANViewModel()
        {
            if (_EditIANViewModel == null)
            {
                _EditIANViewModel = new EditIANViewModel();
            }
        }

       

        #endregion

       
       
        /// <summary>
        /// Provides a deterministic way to delete the EditPOSOutletSaleViewModelPropertyName property.
        /// </summary>
       
        

        #region SelectApprovedOrderViewModel
        
        private static SelectApprovedOrdersViewModel _selectApprovedOrdersViewModel;

        /// <summary>
        /// Gets the SelectApprovedOrdersViewModel property.
        /// </summary>
        public static SelectApprovedOrdersViewModel SelectApprovedOrdersViewModelStatic
        {
            get
            {
                if (_selectApprovedOrdersViewModel == null)
                {
                    CreateSelectApprovedOrdersViewModel();
                }

                return _selectApprovedOrdersViewModel;
            }
        }

        /// <summary>
        /// Gets the SelectApprovedOrdersViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SelectApprovedOrdersViewModel SelectApprovedOrdersViewModel
        {
            get
            {
                return SelectApprovedOrdersViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the SelectApprovedOrdersViewModel property.
        /// </summary>
        public static void ClearSelectApprovedOrdersViewModel()
        {
            _selectApprovedOrdersViewModel.Cleanup();
            _selectApprovedOrdersViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the SelectApprovedOrdersViewModel property.
        /// </summary>
        public static void CreateSelectApprovedOrdersViewModel()
        {
            if (_selectApprovedOrdersViewModel == null)
            {
                _selectApprovedOrdersViewModel = new SelectApprovedOrdersViewModel();
            }
        }

        


        #endregion

       
      

        
        
        #region ListInvoicesViewModel
        private static ListInvoicesViewModel _ListInvoicesViewModel;
        /// <summary>
        /// Gets the ListInvoicesViewModel property.
        /// </summary>
        public static ListInvoicesViewModel ListInvoicesViewModelStatic
        {
            get
            {
                if (_ListInvoicesViewModel == null)
                {
                    CreateListInvoicesViewModel();
                }

                return _ListInvoicesViewModel;
            }
        }
        /// <summary>
        /// Gets the ListInvoicesViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListInvoicesViewModel ListInvoicesViewModel
        {
            get
            {
                return ListInvoicesViewModelStatic;
            }
        }
        /// <summary>
        /// Provides a deterministic way to delete the ListInvoicesViewModel property.
        /// </summary>
        public static void ClearListInvoicesViewModel()
        {
            _ListInvoicesViewModel.Cleanup();
            _ListInvoicesViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ListInvoicesViewModel property.
        /// </summary>
        public static void CreateListInvoicesViewModel()
        {
            if (_ListInvoicesViewModel == null)
            {
                _ListInvoicesViewModel = new ListInvoicesViewModel();
            }
        }
        
        #endregion

        #region ProductTransactionsViewModel
        private static ProductTransactionsViewModel _ProductTransactionsViewModel;

        /// <summary>
        /// Gets the ProductTransactionsViewModel property.
        /// </summary>
        public static ProductTransactionsViewModel ProductTransactionsViewModelStatic
        {
            get
            {
                if (_ProductTransactionsViewModel == null)
                {
                    CreateProductTransactionsViewModel();
                }

                return _ProductTransactionsViewModel;
            }
        }

        /// <summary>
        /// Gets the ProductTransactionsViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ProductTransactionsViewModel ProductTransactionsViewModel
        {
            get
            {
                return ProductTransactionsViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ProductTransactionsViewModel property.
        /// </summary>
        public static void ClearProductTransactionsViewModel()
        {
            _ProductTransactionsViewModel.Cleanup();
            _ProductTransactionsViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ProductTransactionsViewModel property.
        /// </summary>
        public static void CreateProductTransactionsViewModel()
        {
            if (_ProductTransactionsViewModel == null)
            {
                _ProductTransactionsViewModel = new ProductTransactionsViewModel();
            }
        }
        #endregion

        #region ConfigurationViewModel
        private static ConfigurationViewModel _ConfigurationViewModel;
        public static ConfigurationViewModel ConfigurationViewModelStatic
        {
            get
            {
                if (_ConfigurationViewModel == null)
                {
                    CreateConfigurationViewModel();
                }

                return _ConfigurationViewModel;
            }
        }

        /// <summary>
        /// Gets the ConfigurationViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ConfigurationViewModel ConfigurationViewModel
        {
            get
            {
                return ConfigurationViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ConfigurationViewModel property.
        /// </summary>
        public static void ClearConfigurationViewModel()
        {
            _ConfigurationViewModel.Cleanup();
            _ConfigurationViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ConfigurationViewModel property.
        /// </summary>
        public static void CreateConfigurationViewModel()
        {
            if (_ConfigurationViewModel == null)
            {
                _ConfigurationViewModel = new ConfigurationViewModel();
            }
        }
        #endregion

        
        

        #region FinancialsViewModel
        private static FinancialsViewModel _FinancialsViewModel;

        /// <summary>
        /// Gets the FinancialsViewModel property.
        /// </summary>
        public static FinancialsViewModel FinancialsViewModelStatic
        {
            get
            {
                if (_FinancialsViewModel == null)
                {
                    CreateFinancialsViewModel();
                }

                return _FinancialsViewModel;
            }
        }

        /// <summary>
        /// Gets the FinancialsViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public FinancialsViewModel FinancialsViewModel
        {
            get
            {
                return FinancialsViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the FinancialsViewModel property.
        /// </summary>
        public static void ClearFinancialsViewModel()
        {
            _FinancialsViewModel.Cleanup();
            _FinancialsViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the FinancialsViewModel property.
        /// </summary>
        public static void CreateFinancialsViewModel()
        {
            if (_FinancialsViewModel == null)
            {
                _FinancialsViewModel = new FinancialsViewModel();
            }
        }
        #endregion

        #region EditCNViewModel
        private static EditCNViewModel _EditCNViewModel;

        /// <summary>
        /// Gets the EditCNViewModel property.
        /// </summary>
        public static EditCNViewModel EditCNViewModelStatic
        {
            get
            {
                if (_EditCNViewModel == null)
                {
                    CreateEditCNViewModel();
                }

                return _EditCNViewModel;
            }
        }

        /// <summary>
        /// Gets the EditCNViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditCNViewModel EditCNViewModel
        {
            get
            {
                return EditCNViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the EditCNViewModel property.
        /// </summary>
        public static void ClearEditCNViewModel()
        {
            _EditCNViewModel.Cleanup();
            _EditCNViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the EditCNViewModel property.
        /// </summary>
        public static void CreateEditCNViewModel()
        {
            if (_EditCNViewModel == null)
            {
                _EditCNViewModel = new EditCNViewModel();
            }
        }

        #endregion

        #region RecieveReturnableLineItemViewModel
        private static RecieveReturnableLineItemViewModel _RecieveReturnableLineItemViewModel;

        /// <summary>
        /// Gets the RecieveReturnableLineItemViewModel property.
        /// </summary>
        public static RecieveReturnableLineItemViewModel RecieveReturnableLineItemViewModelStatic
        {
            get
            {
                if (_RecieveReturnableLineItemViewModel == null)
                {
                    CreateRecieveReturnableLineItemViewModel();
                }

                return _RecieveReturnableLineItemViewModel;
            }
        }

        /// <summary>
        /// Gets the RecieveReturnableLineItemViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public RecieveReturnableLineItemViewModel RecieveReturnableLineItemViewModel
        {
            get
            {
                return RecieveReturnableLineItemViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the RecieveReturnableLineItemViewModel property.
        /// </summary>
        public static void ClearRecieveReturnableLineItemViewModel()
        {
            _RecieveReturnableLineItemViewModel.Cleanup();
            _RecieveReturnableLineItemViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the RecieveReturnableLineItemViewModel property.
        /// </summary>
        public static void CreateRecieveReturnableLineItemViewModel()
        {
            if (_RecieveReturnableLineItemViewModel == null)
            {
                _RecieveReturnableLineItemViewModel = new RecieveReturnableLineItemViewModel();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
       
        #endregion

        #region RecieveReturnableViewModel
        private static RecieveReturnableViewModel _RecieveReturnableViewModel;

        /// <summary>
        /// Gets the RecieveReturnableViewModel property.
        /// </summary>
        public static RecieveReturnableViewModel RecieveReturnableViewModelStatic
        {
            get
            {
                if (_RecieveReturnableViewModel == null)
                {
                    CreateRecieveReturnableViewModel();
                }

                return _RecieveReturnableViewModel;
            }
        }

        /// <summary>
        /// Gets the RecieveReturnableViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public RecieveReturnableViewModel RecieveReturnableViewModel
        {
            get
            {
                return RecieveReturnableViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the RecieveReturnableViewModel property.
        /// </summary>
        public static void ClearRecieveReturnableViewModel()
        {
            _RecieveReturnableViewModel.Cleanup();
            _RecieveReturnableViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the RecieveReturnableViewModel property.
        /// </summary>
        public static void CreateRecieveReturnableViewModel()
        {
            if (_RecieveReturnableViewModel == null)
            {
                _RecieveReturnableViewModel = new RecieveReturnableViewModel();
            }
        }

      
        #endregion

        #region TestGridViewModel
        private static TestGridViewModel _TestGridViewModel;

        /// <summary>
        /// Gets the TestGridViewModel property.
        /// </summary>
        public static TestGridViewModel TestGridViewModelStatic
        {
            get
            {
                if (_TestGridViewModel == null)
                {
                    CreateTestGridViewModel();
                }

                return _TestGridViewModel;
            }
        }

        /// <summary>
        /// Gets the TestGridViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public TestGridViewModel TestGridViewModel
        {
            get
            {
                return TestGridViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the TestGridViewModel property.
        /// </summary>
        public static void ClearTestGridViewModel()
        {
            _TestGridViewModel.Cleanup();
            _TestGridViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the TestGridViewModel property.
        /// </summary>
        public static void CreateTestGridViewModel()
        {
            if (_TestGridViewModel == null)
            {
                _TestGridViewModel = new TestGridViewModel();
            }
        }

       
        #endregion

        #region SalesmanRoutesViewModel
        private static SalesmanRoutesViewModel _SalesmanRoutesViewModel;

        /// <summary>
        /// Gets the SalesmanRoutesViewModel property.
        /// </summary>
        public static SalesmanRoutesViewModel SalesmanRoutesViewModelStatic
        {
            get
            {
                if (_SalesmanRoutesViewModel == null)
                {
                    CreateSalesmanRoutesViewModel();
                }

                return _SalesmanRoutesViewModel;
            }
        }

        /// <summary>
        /// Gets the SalesmanRoutesViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SalesmanRoutesViewModel SalesmanRoutesViewModel
        {
            get
            {
                return SalesmanRoutesViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the SalesmanRoutesViewModel property.
        /// </summary>
        public static void ClearSalesmanRoutesViewModel()
        {
            _SalesmanRoutesViewModel.Cleanup();
            _SalesmanRoutesViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the SalesmanRoutesViewModel property.
        /// </summary>
        public static void CreateSalesmanRoutesViewModel()
        {
            if (_SalesmanRoutesViewModel == null)
            {
                _SalesmanRoutesViewModel = new SalesmanRoutesViewModel();
            }
        }

       
        #endregion

        #region SalesmanRouteItemViewModel
        private static SalesmanRouteItemViewModel _SalesmanRouteItemViewModel;

        public static SalesmanRouteItemViewModel SalesmanRouteItemViewModelStatic
        {
            get
            {
                if (_SalesmanRouteItemViewModel == null)
                {
                    CreateSalesmanRouteItemViewModel();
                }

                return _SalesmanRouteItemViewModel;
            }
        }

        /// <summary>
        /// Gets the SalesmanRouteItemViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SalesmanRouteItemViewModel SalesmanRouteItemViewModel
        {
            get
            {
                return SalesmanRouteItemViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the SalesmanRouteItemViewModel property.
        /// </summary>
        public static void ClearSalesmanRouteItemViewModel()
        {
            _SalesmanRouteItemViewModel.Cleanup();
            _SalesmanRouteItemViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the SalesmanRouteItemViewModel property.
        /// </summary>
        public static void CreateSalesmanRouteItemViewModel()
        {
            if (_SalesmanRouteItemViewModel == null)
            {
                _SalesmanRouteItemViewModel = new  SalesmanRouteItemViewModel();
            }
        }

        
        #endregion

        #region ReportsViewModel
        private static ReportsViewModel _ReportsViewModel;

        /// <summary>
        /// Gets the ReportsViewModel property.
        /// </summary>
        public static ReportsViewModel ReportsViewModelStatic
        {
            get
            {
                if (_ReportsViewModel == null)
                {
                    CreateReportsViewModel();
                }

                return _ReportsViewModel;
            }
        }

        /// <summary>
        /// Gets the ReportsViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ReportsViewModel ReportsViewModel
        {
            get
            {
                return ReportsViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ReportsViewModel property.
        /// </summary>
        public static void ClearReportsViewModel()
        {
            _ReportsViewModel.Cleanup();
            _ReportsViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ReportsViewModel property.
        /// </summary>
        public static void CreateReportsViewModel()
        {
            if (_ReportsViewModel == null)
            {
                _ReportsViewModel = new ReportsViewModel();
            }
        }

        #endregion

      

        #region DistributrMessageBoxViewModel
        private static DistributrMessageBoxViewModel _distributrMessageBoxViewModelPropertyName;

        /// <summary>
        /// Gets the DistributrMessageBoxViewModel property.
        /// </summary>
        public static DistributrMessageBoxViewModel DistributrMessageBoxViewModelStatic
        {
            get
            {
                if (_distributrMessageBoxViewModelPropertyName == null)
                {
                    CreateDistributrMessageBoxViewModel();
                }

                return _distributrMessageBoxViewModelPropertyName;
            }
        }

        /// <summary>
        /// Gets the DistributrMessageBoxViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public DistributrMessageBoxViewModel DistributrMessageBoxViewModel
        {
            get
            {
                return DistributrMessageBoxViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the DistributrMessageBoxViewModel property.
        /// </summary>
        public static void ClearDistributrMessageBoxViewModel()
        {
            _distributrMessageBoxViewModelPropertyName.Cleanup();
            _distributrMessageBoxViewModelPropertyName = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the DistributrMessageBoxViewModel property.
        /// </summary>
        public static void CreateDistributrMessageBoxViewModel()
        {
            if (_distributrMessageBoxViewModelPropertyName == null)
            {
                _distributrMessageBoxViewModelPropertyName = new DistributrMessageBoxViewModel();
            }
        }
        #endregion

        #region AuditLogViewModel
        private static AuditLogViewModel _AuditLogViewModel;

        /// <summary>
        /// Gets the AuditLogViewModel property.
        /// </summary>
        public static AuditLogViewModel AuditLogViewModelStatic
        {
            get
            {
                if (_AuditLogViewModel == null)
                {
                    CreateAuditLogViewModel();
                }

                return _AuditLogViewModel;
            }
        }

        /// <summary>
        /// Gets the AuditLogViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public AuditLogViewModel AuditLogViewModel
        {
            get
            {
                return AuditLogViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the AuditLogViewModel property.
        /// </summary>
        public static void ClearAuditLogViewModel()
        {
            _AuditLogViewModel.Cleanup();
            _AuditLogViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the AuditLogViewModel property.
        /// </summary>
        public static void CreateAuditLogViewModel()
        {
            if (_AuditLogViewModel == null)
            {
                _AuditLogViewModel = new AuditLogViewModel();
            }
        }
        #endregion

        #region ListReturnsViewModel
        private static ListReturnsViewModel _ListReturnsViewModel;

        /// <summary>
        /// Gets the ListReturnsViewModel property.
        /// </summary>
        public static ListReturnsViewModel ListReturnsViewModelStatic
        {
            get
            {
                if (_ListReturnsViewModel == null)
                {
                    CreateListReturnsViewModel();
                }

                return _ListReturnsViewModel;
            }
        }

        /// <summary>
        /// Gets the ListReturnsViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListReturnsViewModel ListReturnsViewModel
        {
            get
            {
                return ListReturnsViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ListReturnsViewModel property.
        /// </summary>
        public static void ClearListReturnsViewModel()
        {
            _ListReturnsViewModel.Cleanup();
            _ListReturnsViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ListReturnsViewModel property.
        /// </summary>
        public static void CreateListReturnsViewModel()
        {
            if (_ListReturnsViewModel == null)
            {
                _ListReturnsViewModel = new ListReturnsViewModel();
            }
        }
        #endregion

     
        #region SLCacheViewModel
        private static SLCacheViewModel _SLCacheViewModel;

        /// <summary>
        /// Gets the SLCacheViewModel property.
        /// </summary>
        public static SLCacheViewModel SLCacheViewModelStatic
        {
            get
            {
                if (_SLCacheViewModel == null)
                {
                    CreateSLCacheViewModel();
                }

                return _SLCacheViewModel;
            }
        }

        /// <summary>
        /// Gets the SLCacheViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SLCacheViewModel SLCacheViewModel
        {
            get
            {
                return SLCacheViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the SLCacheViewModel property.
        /// </summary>
        public static void ClearSLCacheViewModel()
        {
            _SLCacheViewModel.Cleanup();
            _SLCacheViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the SLCacheViewModel property.
        /// </summary>
        public static void CreateSLCacheViewModel()
        {
            if (_SLCacheViewModel == null)
            {
                _SLCacheViewModel = new SLCacheViewModel();
            }
        }
        #endregion

        #region EditContactViewModel
        private static EditContactViewModel _editContactViewModelPropertyName;

        /// <summary>
        /// Gets the EditContactViewModel property.
        /// </summary>
        public static EditContactViewModel EditContactViewModelStatic
        {
            get
            {
                if (_editContactViewModelPropertyName == null)
                {
                    CreateEditContactViewModel();
                }

                return _editContactViewModelPropertyName;
            }
        }

        /// <summary>
        /// Gets the EditContactViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditContactViewModel EditContactViewModel
        {
            get
            {
                return EditContactViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the EditContactViewModel property.
        /// </summary>
        public static void ClearEditContactViewModel()
        {
            _editContactViewModelPropertyName.Cleanup();
            _editContactViewModelPropertyName = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the EditContactViewModel property.
        /// </summary>
        public static void CreateEditContactViewModel()
        {
            if (_editContactViewModelPropertyName == null)
            {
                _editContactViewModelPropertyName = new EditContactViewModel();
            }
        }

        #endregion

        #region ListContactsViewModel   
        private static ListContactViewModel _listContactViewModelPropertyName;

        /// <summary>
        /// Gets the ListContactViewModel property.
        /// </summary>
        public static ListContactViewModel ListContactViewModelStatic
        {
            get
            {
                if (_listContactViewModelPropertyName == null)
                {
                    CreateListContactViewModel();
                }

                return _listContactViewModelPropertyName;
            }
        }

        /// <summary>
        /// Gets the ListContactViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListContactViewModel ListContactViewModel
        {
            get
            {
                return ListContactViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ListContactViewModel property.
        /// </summary>
        public static void ClearListContactViewModel()
        {
            _listContactViewModelPropertyName.Cleanup();
            _listContactViewModelPropertyName = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ListContactViewModel property.
        /// </summary>
        public static void CreateListContactViewModel()
        {
            if (_listContactViewModelPropertyName == null)
            {
                _listContactViewModelPropertyName = new ListContactViewModel();
            }
        }

        #endregion


        #region SalesmanSupplierViewModel
        private static SalesmanSupplierViewModel _salesmanSupplierViewModelPropertyName;
        public static SalesmanSupplierViewModel SalesmanSupplierViewModelStatic
        { 
            get
            {
                if (_salesmanSupplierViewModelPropertyName == null)
                {
                    CreateSalesmanSupplierViewModel();
                }
                return _salesmanSupplierViewModelPropertyName;
            }
        }

       [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SalesmanSupplierViewModel SalesmanSupplierViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SalesmanSupplierViewModel>();
            }
        }

        public static void ClearSalesmanSupplierViewModel()
        {
            _salesmanSupplierViewModelPropertyName.Cleanup();
            _salesmanSupplierViewModelPropertyName = null;
        }

        public static void CreateSalesmanSupplierViewModel()
        {
            if (_salesmanSupplierViewModelPropertyName == null)
            {
                _salesmanSupplierViewModelPropertyName = new SalesmanSupplierViewModel();
            }
        }

        #endregion





        #region NewListContactsViewModel

        private static NewListContactViewModel _newlistContactViewModelPropertyName;
        public static NewListContactViewModel NewListContactViewModelStatic
        {
            get
            {
                if (_newlistContactViewModelPropertyName == null)
                {
                    CreateNewListContactViewModel();
                }

                return _newlistContactViewModelPropertyName;
            }
        }

        /// <summary>
        /// Gets the ListContactViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public NewListContactViewModel NewListContactViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<NewListContactViewModel>();
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ListContactViewModel property.
        /// </summary>
        public static void ClearNewListContactViewModel()
        {
            _newlistContactViewModelPropertyName.Cleanup();
            _newlistContactViewModelPropertyName = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ListContactViewModel property.
        /// </summary>
        public static void CreateNewListContactViewModel()
        {
            if (_newlistContactViewModelPropertyName == null)
            {
                _newlistContactViewModelPropertyName = new NewListContactViewModel();
            }
        }

        #endregion

        #region  GeneralSettingsViewModel
        private static GeneralSettingsViewModel _GeneralSettingsViewModel;

        /// <summary>
        /// Gets the GeneralSettingsViewModel property.
        /// </summary>
        public static GeneralSettingsViewModel GeneralSettingsViewModelStatic
        {
            get
            {
                if (_GeneralSettingsViewModel == null)
                {
                    CreateGeneralSettingsViewModel();
                }

                return _GeneralSettingsViewModel;
            }
        }

        /// <summary>
        /// Gets the GeneralSettingsViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public GeneralSettingsViewModel GeneralSettingsViewModel
        {
            get
            {
                return GeneralSettingsViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the GeneralSettingsViewModel property.
        /// </summary>
        public static void ClearGeneralSettingsViewModel()
        {
            _GeneralSettingsViewModel.Cleanup();
            _GeneralSettingsViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the GeneralSettingsViewModel property.
        /// </summary>
        public static void CreateGeneralSettingsViewModel()
        {
            if (_GeneralSettingsViewModel == null)
            {
                _GeneralSettingsViewModel = new GeneralSettingsViewModel();
            }
        }

      
        #endregion

        #region TransactionStatementViewModel
        private static TransactionStatementViewModel _transactionStatementViewModelPropertyName;

        /// <summary>
        /// Gets the TransactionStatementViewModel property.
        /// </summary>
        public static TransactionStatementViewModel TransactionStatementViewModelStatic
        {
            get
            {
                if (_transactionStatementViewModelPropertyName == null)
                {
                    CreateTransactionStatementViewModel();
                }

                return _transactionStatementViewModelPropertyName;
            }
        }

        /// <summary>
        /// Gets the TransactionStatementViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public TransactionStatementViewModel TransactionStatementViewModel
        {
            get
            {
                return TransactionStatementViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the TransactionStatementViewModel property.
        /// </summary>
        public static void ClearTransactionStatementViewModel()
        {
            _transactionStatementViewModelPropertyName.Cleanup();
            _transactionStatementViewModelPropertyName = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the TransactionStatementViewModel property.
        /// </summary>
        public static void CreateTransactionStatementViewModel()
        {
            if (_transactionStatementViewModelPropertyName == null)
            {
                _transactionStatementViewModelPropertyName = new TransactionStatementViewModel();
            }
        }
        #endregion

        #region AddCreditNoteModel
        private static AddCreditNoteModel _addCreditNote;

        /// <summary>
        /// Gets the AddCreditNoteModel property.
        /// </summary>
        public static AddCreditNoteModel AddCreditNoteModelStatic
        {
            get
            {
                if (_addCreditNote == null)
                {
                    CreateAddCreditNoteModel();
                }

                return _addCreditNote;
            }
        }

        /// <summary>
        /// Gets the AddCreditNoteModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public AddCreditNoteModel AddCreditNoteModel
        {
            get
            {
                return AddCreditNoteModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the AddCreditNoteModel property.
        /// </summary>
        public static void ClearAddCreditNoteModel()
        {
            _addCreditNote.Cleanup();
            _addCreditNote = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the AddCreditNoteModel property.
        /// </summary>
        public static void CreateAddCreditNoteModel()
        {
            if (_addCreditNote == null)
            {
                _addCreditNote =new AddCreditNoteModel();
            }
        }

        
        #endregion

        #region AddCreditNoteLineViewModel
        private static AddCreditNoteLineViewModel _AddCreditNoteLineViewModel;

        /// <summary>
        /// Gets the AddCreditNoteLineViewModel property.
        /// </summary>
        public static AddCreditNoteLineViewModel AddCreditNoteLineViewModelStatic
        {
            get
            {
                if (_AddCreditNoteLineViewModel == null)
                {
                    CreateAddCreditNoteLineViewModel();
                }

                return _AddCreditNoteLineViewModel;
            }
        }

        /// <summary>
        /// Gets the AddCreditNoteLineViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public AddCreditNoteLineViewModel AddCreditNoteLineViewModel
        {
            get
            {
                return AddCreditNoteLineViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the AddCreditNoteLineViewModel property.
        /// </summary>
        public static void ClearAddCreditNoteLineViewModel()
        {
            _AddCreditNoteLineViewModel.Cleanup();
            _AddCreditNoteLineViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the AddCreditNoteLineViewModel property.
        /// </summary>
        public static void CreateAddCreditNoteLineViewModel()
        {
            if (_AddCreditNoteLineViewModel == null)
            {
                _AddCreditNoteLineViewModel =new AddCreditNoteLineViewModel();
            }
        }

       
        #endregion

        #region EditOutletVistDayViewModel
        private static EditOutletVistDayViewModel _EditOutletVistDayViewModel;

        /// <summary>
        /// Gets the EditOutletVistDayViewModel property.
        /// </summary>
        public static EditOutletVistDayViewModel EditOutletVistDayViewModelStatic
        {
            get
            {
                if (_EditOutletVistDayViewModel == null)
                {
                    CreateEditOutletVistDayViewModel();
                }

                return _EditOutletVistDayViewModel;
            }
        }

        /// <summary>
        /// Gets the EditOutletVistDayViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditOutletVistDayViewModel EditOutletVistDayViewModel
        {
            get
            {
                return EditOutletVistDayViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the EditOutletVistDayViewModel property.
        /// </summary>
        public static void ClearEditOutletVistDayViewModel()
        {
            _EditOutletVistDayViewModel.Cleanup();
            _EditOutletVistDayViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the EditOutletVistDayViewModel property.
        /// </summary>
        public static void CreateEditOutletVistDayViewModel()
        {
            if (_EditOutletVistDayViewModel == null)
            {
                _EditOutletVistDayViewModel = new EditOutletVistDayViewModel();
            }
        }

       
        #endregion

        #region EditOutletPrioritizationViewModel
        private static EditOutletPriorityViewModel _editOutletPriorityViewModel;

        /// <summary>
        /// Gets the EditOutletPriorityViewModel property.
        /// </summary>
        public static EditOutletPriorityViewModel EditOutletPriorityViewModelStatic
        {
            get
            {
                if (_editOutletPriorityViewModel == null)
                {
                    CreateEditOutletPriorityViewModel();
                }

                return _editOutletPriorityViewModel;
            }
        }

        /// <summary>
        /// Gets the EditOutletPriorityViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditOutletPriorityViewModel EditOutletPriorityViewModel
        {
            get
            {
                return EditOutletPriorityViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the EditOutletPriorityViewModel property.
        /// </summary>
        public static void ClearEditOutletPriorityViewModel()
        {
            _editOutletPriorityViewModel.Cleanup();
            _editOutletPriorityViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the EditOutletPriorityViewModel property.
        /// </summary>
        public static void CreateEditOutletPriorityViewModel()
        {
            if (_editOutletPriorityViewModel == null)
            {
                _editOutletPriorityViewModel = new EditOutletPriorityViewModel();
            }
        }
        #endregion

        #region EditOutletTargetsViewModel
        private static EditOutletTargetsViewModel _editOutletTargetsViewModel;

        /// <summary>
        /// Gets the EditOutletTargetsViewModel property.
        /// </summary>
        public static EditOutletTargetsViewModel EditOutletTargetsViewModelStatic
        {
            get
            {
                if (_editOutletTargetsViewModel == null)
                {
                    CreateEditOutletTargetsViewModel();
                }

                return _editOutletTargetsViewModel;
            }
        }

        /// <summary>
        /// Gets the EditOutletTargetsViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditOutletTargetsViewModel EditOutletTargetsViewModel
        {
            get
            {
                return EditOutletTargetsViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the EditOutletTargetsViewModel property.
        /// </summary>
        public static void ClearEditOutletTargetsViewModel()
        {
            _editOutletTargetsViewModel.Cleanup();
            _editOutletTargetsViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the EditOutletTargetsViewModel property.
        /// </summary>
        public static void CreateEditOutletTargetsViewModel()
        {
            if (_editOutletTargetsViewModel == null)
            {
                _editOutletTargetsViewModel = new EditOutletTargetsViewModel();
            }
        }

        #endregion

        #region ReorderLevelViewModel
        private static ReorderLevelViewModel _reorderLevelViewModel;

        /// <summary>
        /// Gets the ReorderLevelViewModel property.
        /// </summary>
        public static ReorderLevelViewModel ReorderLevelViewModelStatic
        {
            get
            {
                if (_reorderLevelViewModel == null)
                {
                    CreateReorderLevelViewModel();
                }

                return _reorderLevelViewModel;
            }
        }

        /// <summary>
        /// Gets the ReorderLevelViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ReorderLevelViewModel ReorderLevelViewModel
        {
            get
            {
                return ReorderLevelViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ReorderLevelViewModel property.
        /// </summary>
        public static void ClearReorderLevelViewModel()
        {
            _reorderLevelViewModel.Cleanup();
            _reorderLevelViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ReorderLevelViewModel property.
        /// </summary>
        public static void CreateReorderLevelViewModel()
        {
            if (_reorderLevelViewModel == null)
            {
                _reorderLevelViewModel = new ReorderLevelViewModel();
            }
        }
        #endregion

        #region DispatchProductsViewModel

        private static DispatchProductsViewModel _DispatchProductsViewModel;

        /// <summary>
        /// Gets the DispatchProductsViewModel property.
        /// </summary>
        public static DispatchProductsViewModel DispatchProductsViewModelStatic
        {
            get
            {
                if (_DispatchProductsViewModel == null)
                {
                    CreateDispatchProductsViewModel();
                }

                return _DispatchProductsViewModel;
            }
        }

        /// <summary>
        /// Gets the DispatchProductsViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public DispatchProductsViewModel DispatchProductsViewModel
        {
            get
            {
                return DispatchProductsViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the DispatchProductsViewModel property.
        /// </summary>
        public static void ClearDispatchProductsViewModel()
        {
            _DispatchProductsViewModel.Cleanup();
            _DispatchProductsViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the DispatchProductsViewModel property.
        /// </summary>
        public static void CreateDispatchProductsViewModel()
        {
            if (_DispatchProductsViewModel == null)
            {
                _DispatchProductsViewModel =new DispatchProductsViewModel();
            }
        }
        #endregion

        #region DPLineItemViewModel
        private static DPLineItemViewModel _dPLineItemViewModel;

        /// <summary>
        /// Gets the DPLineItemViewModel property.
        /// </summary>
        public static DPLineItemViewModel DPLineItemViewModelStatic
        {
            get
            {
                if (_dPLineItemViewModel == null)
                {
                    CreateDPLineItemViewModel();
                }

                return _dPLineItemViewModel;
            }
        }

        /// <summary>
        /// Gets the DPLineItemViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public DPLineItemViewModel DPLineItemViewModel
        {
            get
            {
                return DPLineItemViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the DPLineItemViewModel property.
        /// </summary>
        public static void ClearDPLineItemViewModel()
        {
            _dPLineItemViewModel.Cleanup();
            _dPLineItemViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the DPLineItemViewModel property.
        /// </summary>
        public static void CreateDPLineItemViewModel()
        {
            if (_dPLineItemViewModel == null)
            {
                _dPLineItemViewModel = new DPLineItemViewModel();
            }
        }
        #endregion

        #region ArchiveViewModel
        private static ArchiveViewModel _ArchiveViewModel;

        /// <summary>
        /// Gets the ArchiveViewModel property.
        /// </summary>
        public static ArchiveViewModel ArchiveViewModelStatic
        {
            get
            {
                if (_ArchiveViewModel == null)
                {
                    CreateArchiveViewModel();
                }

                return _ArchiveViewModel;
            }
        }

        /// <summary>
        /// Gets the ArchiveViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ArchiveViewModel ArchiveViewModel
        {
            get
            {
                return ArchiveViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ArchiveViewModel property.
        /// </summary>
        public static void ClearArchiveViewModel()
        {
            _ArchiveViewModel.Cleanup();
            _ArchiveViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ArchiveViewModel property.
        /// </summary>
        public static void CreateArchiveViewModel()
        {
            if (_ArchiveViewModel == null)
            {
                _ArchiveViewModel = new ArchiveViewModel(); 
            }
        }

       
      
        #endregion

        #region Test1ViewModel
        private static Test1ViewModel _Test1ViewModel;

        public static Test1ViewModel Test1ViewModelStatic
        {
            get
            {
                if (_Test1ViewModel == null)
                {
                    CreateTest11ViewModel();
                }

                return _Test1ViewModel;
            }
        }

       
        public Test1ViewModel Test1ViewModel
        {
            get
            {
                return Test1ViewModelStatic;
            }
        }

        public static void ClearTest1ViewModel()
        {
            _Test1ViewModel.Cleanup();
            _Test1ViewModel = null;
        }
        public static void CreateTest11ViewModel()
        {
            if (_Test1ViewModel == null)
            {
                _Test1ViewModel = new Test1ViewModel();
            }
        }



        #endregion

        #region Test2ViewModel
        private static Test2ViewModel _Test2ViewModel;

        public static Test2ViewModel Test2ViewModelStatic
        {
            get
            {
                if (_Test2ViewModel == null)
                {
                    CreateTest21ViewModel();
                }

                return _Test2ViewModel;
            }
        }


        public Test2ViewModel Test2ViewModel
        {
            get
            {
                return Test2ViewModelStatic;
            }
        }

        public static void ClearTest2ViewModel()
        {
            _Test2ViewModel.Cleanup();
            _Test2ViewModel = null;
        }
        public static void CreateTest21ViewModel()
        {
            if (_Test2ViewModel == null)
            {
                _Test2ViewModel = new Test2ViewModel();
            }
        }



        #endregion

    

        #region ReturnItemViewModel
        private static ReturnItemViewModel _ReturnItemViewModel;

       
        public static ReturnItemViewModel ReturnItemViewModelStatic
        {
            get
            {
                if (_ReturnItemViewModel == null)
                {
                    CreateReturnItemViewModel();
                }

                return _ReturnItemViewModel;
            }
        }

        /// <summary>
        /// Gets the ReturnItemViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ReturnItemViewModel ReturnItemViewModel
        {
            get
            {
                return ReturnItemViewModelStatic;
            }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ReturnItemViewModel property.
        /// </summary>
        public static void ClearReturnItemViewModel()
        {
            _ReturnItemViewModel.Cleanup();
            _ReturnItemViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ReturnItemViewModel property.
        /// </summary>
        public static void CreateReturnItemViewModel()
        {
            if (_ReturnItemViewModel == null)
            {
                _ReturnItemViewModel = new ReturnItemViewModel();
            }
        }

     
        #endregion

        #region ComboPopUpViewModel
        private static ComboPopUpViewModel _comboPopUpViewModelPropertyName;

        public static ComboPopUpViewModel ComboPopUpViewModelStatic
        {
            get
            {
                if (_comboPopUpViewModelPropertyName == null)
                {
                    CreateComboPopUpViewModel();
                }

                return _comboPopUpViewModelPropertyName;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ComboPopUpViewModel ComboPopUpViewModel
        {
            get
            {
                return ComboPopUpViewModelStatic;
            }
        }
         
        

        public static void ClearComboPopUpViewModel()
        {
            _comboPopUpViewModelPropertyName.Cleanup();
            _comboPopUpViewModelPropertyName = null;
        }

        public static void CreateComboPopUpViewModel()
        {
            if (_comboPopUpViewModelPropertyName == null)
            {
                _comboPopUpViewModelPropertyName = new ComboPopUpViewModel();
            }
        }
        #endregion

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public OrderFormViewModel OrderFormViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<OrderFormViewModel>();
            }
        }

        #region SalesmanOrderListingViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SalesmanOrderListingViewModel SalesmanOrderListingViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SalesmanOrderListingViewModel>();
            }
        }
        #endregion 

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public PurchaseOrderFormViewModel PurchaseOrderFormViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PurchaseOrderFormViewModel>();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public PurchaseOrderListingViewModel PurchaseOrderListingViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PurchaseOrderListingViewModel>();
            }
        }

        #region ViewOrderViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ViewOrderViewModel ViewOrderViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewOrderViewModel>();
            }
        }
        #endregion

        #region ViewPurchaseOrderViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ViewPurchaseOrderViewModel ViewPurchaseOrderViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewPurchaseOrderViewModel>();
            }
        }
        #endregion

        #region LoginViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public LoginViewModel LoginViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LoginViewModel>();
            }
        }


        #endregion


        #region DocumentReportViewerViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public DocumentReportViewerViewModel DocumentReportViewerViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DocumentReportViewerViewModel>();
            }
        }


        #endregion
        

        public static void Cleanup()
        {
            ClearTest1ViewModel();
            ClearTest2ViewModel();
            ClearReturnItemViewModel();
            ClearArchiveViewModel();
            ClearEditOutletVistDayViewModel();
            ClearAddCreditNoteLineViewModel();
            ClearAddCreditNoteModel();
            ClearGeneralSettingsViewModel();
            ClearSalesmanRouteItemViewModel();
            ClearSalesmanRoutesViewModel();
            ClearTestGridViewModel();
            ClearRecieveReturnableLineItemViewModel();
           ClearRecieveReturnableViewModel();
            //ClearLoginModalViewModel();
            ClearConfigurationViewModel();
            ClearListITNViewModel();
            ClearListIANViewModel();
            ClearListInventoryViewModel();
          //ClearIntialLoginViewModel();
            ClearSelectApprovedOrdersViewModel();
            ClearEditITNViewModel();
            ClearITNLineItemViewModel();
            ClearEditIANViewModel();
            ClearGRNItemModalViewModel();
            ClearListGRNViewModel();
            
            //ClearEditOrderViewModel();
            //ClearOrderLineItemViewModel();
            //ClearListOrdersViewModel();
           // ClearSyncViewModel();
            ClearTestViewModel();
            ClearListOutletsViewModel();
            ClearEditOutletViewModel();
            
            ClearEditRouteViewModel();
            ClearListUsersViewModel();
          
           
           ClearProductTransactionsViewModel();
            ClearFinancialsViewModel();
            ClearListInvoicesViewModel();
            ClearEditCNViewModel();
            ClearReportsViewModel();
          
            ClearDistributrMessageBoxViewModel();
            ClearAuditLogViewModel();
            ClearListReturnsViewModel();
           
            ClearSLCacheViewModel();
            ClearEditContactViewModel();
            ClearListContactViewModel();
            ClearTransactionStatementViewModel();
          
            ClearEditOutletPriorityViewModel();
            ClearEditOutletTargetsViewModel();
            ClearReorderLevelViewModel();
            ClearDispatchProductsViewModel();
            ClearDPLineItemViewModel();
         
            ClearComboPopUpViewModel();
        }
    }
}
