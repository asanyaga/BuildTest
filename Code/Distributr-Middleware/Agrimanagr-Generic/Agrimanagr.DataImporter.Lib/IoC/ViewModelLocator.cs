
using Agrimanagr.DataImporter.Lib.ViewModels;
using Distributr.WPF.Lib.ViewModels.IntialSetup;
using Distributr_Middleware.WPF.Lib.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;

namespace Agrimanagr.DataImporter.Lib.IoC
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            
            SimpleIoc.Default.Register<ValidationResultViewModel>();
            SimpleIoc.Default.Register<MiddlewareLoginViewModel>();
            SimpleIoc.Default.Register<MasterDataImportVm>();


            var transActivity = SimpleIoc.Default.GetInstance<MasterDataImportVm>();
            Messenger.Default.Register<string>(transActivity, transActivity.ReceiveMessage);
        }

       
        public MiddlewareLoginViewModel MiddlewareLoginViewModel
        {
            get { return ServiceLocator.Current.GetInstance<MiddlewareLoginViewModel>(); }
        }
        public MasterDataImportVm MasterDataImportVm
        {
            get { return ServiceLocator.Current.GetInstance<MasterDataImportVm>(); }
        }

       
        public ValidationResultViewModel ValidationResultViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ValidationResultViewModel>(); }
        }

    }
}