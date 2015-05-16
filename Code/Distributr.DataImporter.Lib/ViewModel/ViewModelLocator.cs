
using Distributr.DataImporter.Lib.ViewModel.FCL;
using Distributr.WPF.Lib.ViewModels.Sync;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;

namespace Distributr.DataImporter.Lib.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ValidationResultViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<FclMainWindowViewModel>();
            SimpleIoc.Default.Register<ListProductsToImportViewModel>();
            SimpleIoc.Default.Register<ListExportOrdersViewModel>();
            SimpleIoc.Default.Register<ListSalesmenToImportViewModel>();
            SimpleIoc.Default.Register<ListOutletsToImportViewModel>();
            SimpleIoc.Default.Register<ListProductPricingToImportViewModel>();
            SimpleIoc.Default.Register<ListShipToAddressesViewModel>();
            SimpleIoc.Default.Register<ImportStockLineViewModel>();
            SimpleIoc.Default.Register<ListProductDiscountGroupToImportViewModel>();
            SimpleIoc.Default.Register<CommandsUploadActivityViewModel>();
            SimpleIoc.Default.Register<TransactionsExportViewModel>();
         
            RegisterMessages();
           
        }

        void RegisterMessages()
        {
            var exportActivity = SimpleIoc.Default.GetInstance<ListExportOrdersViewModel>();
            Messenger.Default.Register<string>(exportActivity, exportActivity.ReceiveMessage);

            var stocklineActivity = SimpleIoc.Default.GetInstance<ImportStockLineViewModel>();
            Messenger.Default.Register<string>(stocklineActivity, stocklineActivity.ReceiveMessage);

            var commandsActivity = SimpleIoc.Default.GetInstance<CommandsUploadActivityViewModel>();
            Messenger.Default.Register<string>(commandsActivity, commandsActivity.ReceiveMessage);

            var transActivity = SimpleIoc.Default.GetInstance<TransactionsExportViewModel>();
            Messenger.Default.Register<string>(transActivity, transActivity.ReceiveMessage);

            
            
        }
        public TransactionsExportViewModel TransactionsExportViewModel
        {
            get { return ServiceLocator.Current.GetInstance<TransactionsExportViewModel>(); }
        }

        public FclMainWindowViewModel FclMainWindowViewModel
        {
            get { return ServiceLocator.Current.GetInstance<FclMainWindowViewModel>(); }
        }
        public CommandsUploadActivityViewModel CommandsUploadActivityViewModel
        {
            get { return ServiceLocator.Current.GetInstance<CommandsUploadActivityViewModel>(); }
        }

        public ImportStockLineViewModel ImportStockLineViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ImportStockLineViewModel>(); }
        }

        public LoginViewModel LoginViewModel
        {
            get { return ServiceLocator.Current.GetInstance<LoginViewModel>(); }
        }
        public ListProductPricingToImportViewModel ListProductPricingToImportViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ListProductPricingToImportViewModel>(); }
        }
        public ListProductDiscountGroupToImportViewModel ListProductDiscountGroupToImportViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ListProductDiscountGroupToImportViewModel>(); }
        }
        

        public ListExportOrdersViewModel ListExportOrdersViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ListExportOrdersViewModel>(); }
        }
        public ListShipToAddressesViewModel ListShipToAddressesViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ListShipToAddressesViewModel>(); }
        }
        public ListOutletsToImportViewModel ListOutletsToImportViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ListOutletsToImportViewModel>(); }
        }

        public ListProductsToImportViewModel ListProductsToImportViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ListProductsToImportViewModel>(); }
        }

        public ListSalesmenToImportViewModel ListSalesmenToImportViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ListSalesmenToImportViewModel>(); }
        }


        public MainViewModel MainViewModel
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        public ValidationResultViewModel ValidationResultViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ValidationResultViewModel>(); }
        }
       
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}