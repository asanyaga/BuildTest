using System.Diagnostics.CodeAnalysis;
using Distributr_Middleware.WPF.Lib.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;

namespace Distributr_Middleware.WPF.Lib.IOC
{
    public class MiddlewareViewModelLocator
    {
        static MiddlewareViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MiddlewareLoginViewModel>();
            SimpleIoc.Default.Register<MiddlewareMainWindowViewModel>();
            SimpleIoc.Default.Register<ImportMasterDataViewModel>();
            SimpleIoc.Default.Register<ValidationResultViewModel>();
            SimpleIoc.Default.Register<SageOrdersSalesexportViewModel>();
            SimpleIoc.Default.Register<AppSettingsViewModel>();
            SimpleIoc.Default.Register<InventoryTransferViewModel>();
           
            

            

            var transActivity = SimpleIoc.Default.GetInstance<SageOrdersSalesexportViewModel>();
            Messenger.Default.Register<string>(transActivity, transActivity.ReceiveMessage);
        }

          

    #region MainWindowViewModel

        public MiddlewareMainWindowViewModel MiddlewareMainWindowViewModel
        {
            get { return ServiceLocator.Current.GetInstance<MiddlewareMainWindowViewModel>(); }
        }

        #endregion

        
        public MiddlewareLoginViewModel MiddlewareLoginViewModel
        {
            get { return ServiceLocator.Current.GetInstance<MiddlewareLoginViewModel>(); }
        }

        public AppSettingsViewModel AppSettingsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<AppSettingsViewModel>(); }
        }

        public SageOrdersSalesexportViewModel SageOrdersSalesexportViewModel
        {
            get { return ServiceLocator.Current.GetInstance<SageOrdersSalesexportViewModel>(); }
        }

        public InventoryTransferViewModel InventoryTransferViewModel
        {
            get { return ServiceLocator.Current.GetInstance<InventoryTransferViewModel>(); }
        }

        

        public ValidationResultViewModel ValidationResultViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ValidationResultViewModel>(); }
        }

        public ImportMasterDataViewModel ImportMasterDataViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ImportMasterDataViewModel>(); }
        }
       
        

    }
}
