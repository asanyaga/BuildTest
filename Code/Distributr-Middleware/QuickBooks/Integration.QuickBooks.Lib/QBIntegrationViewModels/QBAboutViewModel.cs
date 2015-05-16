using System;
using System.Reflection;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr_Middleware.WPF.Lib.Utils;
using Distributr_Middleware.WPF.Lib.ViewModels;
using GalaSoft.MvvmLight.Command;

namespace Integration.QuickBooks.Lib.QBIntegrationViewModels
{
    public class QBAboutViewModel : MiddleWareViewModelBase
    {
        public const string ProductNamePropertyName = "ProductName";
        private string _productName = "Quick Books Integration Module";
        public string ProductName
        {
            get
            {
                return _productName;
            }

            set
            {
                if (_productName == value)
                {
                    return;
                }

                RaisePropertyChanging(ProductNamePropertyName);
                _productName = value;
                RaisePropertyChanged(ProductNamePropertyName);
            }
        }
         
        public const string VersionPropertyName = "Version";
        private string _version = "1.0.0.1";
        public string Version
        {
            get
            {
                return _version;
            }

            set
            {
                if (_version == value)
                {
                    return;
                }

                RaisePropertyChanging(VersionPropertyName);
                _version = value;
                RaisePropertyChanged(VersionPropertyName);
            }
        } 

        public const string LoggedInUserPropertyName = "LoggedInUser";
        private string _loggedInUser = "";
        public string LoggedInUser
        {
            get
            {
                return _loggedInUser;
            }

            set
            {
                if (_loggedInUser == value)
                {
                    return;
                }

                RaisePropertyChanging(LoggedInUserPropertyName);
                _loggedInUser = value;
                RaisePropertyChanged(LoggedInUserPropertyName);
            }
        }

        private RelayCommand _loadPageCommand = null;
        public new RelayCommand LoadPageCommand
        {
            get { return _loadPageCommand ?? (_loadPageCommand = new RelayCommand(Load)); }
        }

        protected void Load()
        {
            SetProductInfo();
        }

        private void SetProductInfo()
        {
            LoggedInUser = "Logged in as " + CredentialsManager.GetUserName();

            ProductName = "Product \t\t:: Quick Books Integration Module";
            Version = "Version \t\t:: " + ParseVersionNumber(Assembly.GetEntryAssembly()); 
        }

        private static Version ParseVersionNumber(Assembly assembly)
        {
            AssemblyName assemblyName = new AssemblyName(assembly.FullName);
            return assemblyName.Version;
        }
    }
}
