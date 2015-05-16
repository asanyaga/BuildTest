using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Integration.Cussons.WPF.Lib.CussonsIntegrationViewModels;
using Integration.Cussons.WPF.Lib.CussonsIntegrationViewModels.MasterData;
using Microsoft.Practices.ServiceLocation;

namespace Integration.Cussons.WPF.Lib.IOC
{
    public class CussonsViewModelLocator
    {
        static CussonsViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<CussonsLoginViewModel>();
            SimpleIoc.Default.Register<CussonsMainWindowViewModel>();
            SimpleIoc.Default.Register<CussonsAboutViewModel>();
            SimpleIoc.Default.Register<ListProductBrandImportsViewModels>();
            SimpleIoc.Default.Register<ListProductImportsViewModel>();
            SimpleIoc.Default.Register<ValidationResultViewModel>();
            SimpleIoc.Default.Register<DistributrsalesmanImportListViewModel>();
            SimpleIoc.Default.Register<ShipToAddressImportsViewModel>();
            SimpleIoc.Default.Register<ListAfcoProductPricingImportsViewModel>();
            SimpleIoc.Default.Register<OutletImportsListViewModel>();
            SimpleIoc.Default.Register<TransactionsExportViewModel>();

            var transActivity = SimpleIoc.Default.GetInstance<TransactionsExportViewModel>();
            Messenger.Default.Register<string>(transActivity, transActivity.ReceiveMessage);
        }

          

        #region Master data
        [SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public OutletImportsListViewModel OutletImportsListViewModel
        {
            get { return ServiceLocator.Current.GetInstance<OutletImportsListViewModel>(); }
        }

        [SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public ListAfcoProductPricingImportsViewModel ListAfcoProductPricingImportsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ListAfcoProductPricingImportsViewModel>(); }
        }

        [SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public ShipToAddressImportsViewModel ShipToAddressImportsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ShipToAddressImportsViewModel>(); }
        }

        [SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public DistributrsalesmanImportListViewModel DistributrsalesmanImportListViewModel
        {
            get { return ServiceLocator.Current.GetInstance<DistributrsalesmanImportListViewModel>(); }
        }

        [SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public ValidationResultViewModel ValidationResultViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ValidationResultViewModel>(); }
        }

       
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListProductBrandImportsViewModels ListProductBrandImportsViewModels
        {
            get { return ServiceLocator.Current.GetInstance<ListProductBrandImportsViewModels>(); }
        }

        [SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public ListProductImportsViewModel ListProductImportsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ListProductImportsViewModel>(); }
        }

     
        #endregion
        [SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public TransactionsExportViewModel TransactionsExportViewModel
        {
            get { return ServiceLocator.Current.GetInstance<TransactionsExportViewModel>(); }
        }
        
        #region CussonsLoginViewModel

        /// <summary>
        /// Gets the CussonsLoginViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public CussonsLoginViewModel CussonsLoginViewModel
        {
            get { return ServiceLocator.Current.GetInstance<CussonsLoginViewModel>(); }
        }

        #endregion

        #region CussonsMainWindowViewModel

        /// <summary>
        /// Gets the CussonsMainWindowViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public CussonsMainWindowViewModel CussonsMainWindowViewModel
        {
            get { return ServiceLocator.Current.GetInstance<CussonsMainWindowViewModel>(); }
        }

        #endregion

        #region CussonsAboutViewModel

        /// <summary>
        /// Gets the CussonsAboutViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public CussonsAboutViewModel CussonsAboutViewModel
        {
            get { return ServiceLocator.Current.GetInstance<CussonsAboutViewModel>(); }
        }

        #endregion

    }
}
