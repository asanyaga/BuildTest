using Distributr_Middleware.WPF.Lib.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using SAPUtilityLib.ViewModels;

namespace SAPUtilityLib.IoC
{
    public class VMLocator
    {
        static VMLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainWindowVM>();
            SimpleIoc.Default.Register<VmImportMasterdata>();
            SimpleIoc.Default.Register<MiddlewareLoginViewModel>();
            SimpleIoc.Default.Register<ValidationResultViewModel>();
            SimpleIoc.Default.Register<SapSettingsViewModel>();
            SimpleIoc.Default.Register<SyncViewModel>();


            var vm = SimpleIoc.Default.GetInstance<VmImportMasterdata>();
            Messenger.Default.Register<string>(vm, vm.ReceiveMessage);
        }

        public SyncViewModel SyncViewModel
        {
            get { return ServiceLocator.Current.GetInstance<SyncViewModel>(); }
        }
        public MainWindowVM MainWindowVM
        {
            get { return ServiceLocator.Current.GetInstance<MainWindowVM>(); }
        }

        public SapSettingsViewModel SapSettingsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<SapSettingsViewModel>(); }
        }

        public ValidationResultViewModel ValidationResultViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ValidationResultViewModel>(); }
        }
        public VmImportMasterdata VmImportMasterdata
        {
            get { return ServiceLocator.Current.GetInstance<VmImportMasterdata>(); }
        }

        public MiddlewareLoginViewModel MiddlewareLoginViewModel
        {
            get { return ServiceLocator.Current.GetInstance<MiddlewareLoginViewModel>(); }
        }
    }
}
