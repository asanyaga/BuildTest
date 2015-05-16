using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr_Middleware.WPF.Lib.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Integration.QuickBooks.Lib.QBIntegrationViewModels;
using Microsoft.Practices.ServiceLocation;

namespace Integration.QuickBooks.Lib.IOC
{
    public class QBViewModelLocator
    {
        static QBViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MiddlewareLoginViewModel>();
            SimpleIoc.Default.Register<QBListTransactionsViewModel>();
            SimpleIoc.Default.Register<QBMainWindowViewModel>();
            SimpleIoc.Default.Register<QBEditAccountViewModel>();
            SimpleIoc.Default.Register<QBAboutViewModel>();

            SimpleIoc.Default.Register<QBListTransactionsNewViewModel>();

            SimpleIoc.Default.Register<VmImportMasterdata>();
            
            var vm1 = SimpleIoc.Default.GetInstance<QBMainWindowViewModel>();
            Messenger.Default.Register<string>(vm1, vm1.SetGlobalStatus);
        }

        #region QBLoginViewModel

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MiddlewareLoginViewModel MiddlewareLoginViewModel
        {
            get { return ServiceLocator.Current.GetInstance<MiddlewareLoginViewModel>(); }
        }

        #endregion

        #region QBListTransactionsViewModel

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public QBListTransactionsViewModel QbListTransactionsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<QBListTransactionsViewModel>(); }
        }

        #endregion

        #region QBListTransactionsNewViewModel

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public QBListTransactionsNewViewModel QbListTransactionsNewViewModel
        {
            get { return ServiceLocator.Current.GetInstance<QBListTransactionsNewViewModel>(); }
        }

        #endregion


        public VmImportMasterdata VmImportMasterdata
        {
            get { return ServiceLocator.Current.GetInstance<VmImportMasterdata>(); }
        }


      
        #region QBMainWindowViewModel

        /// <summary>
        /// Gets the QBMainWindowViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public QBMainWindowViewModel QBMainWindowViewModel
        {
            get { return ServiceLocator.Current.GetInstance<QBMainWindowViewModel>(); }
        }

        #endregion

        #region QBEditAccountViewModel

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public QBEditAccountViewModel QBEditAccountViewModel
        {
            get { return ServiceLocator.Current.GetInstance<QBEditAccountViewModel>(); }
        }

        #endregion

        #region QBAboutViewModel

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public QBAboutViewModel QBAboutViewModel
        {
            get { return ServiceLocator.Current.GetInstance<QBAboutViewModel>(); }
        }

        #endregion


    }
}
