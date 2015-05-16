using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.UI;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Linq;

namespace Distributr.WPF.Lib.ViewModels.MainPage
{
    public class AgrimanagrMainPageViewModel : DistributrViewModelBase
    {
        public AgrimanagrMainPageViewModel()
        {
            TabSelectionChangedCommand = new RelayCommand<SelectionChangedEventArgs>(TabSelectionChanged);
            LogoutCommand = new RelayCommand(LogOut);
            ViewableModulesCommand=new RelayCommand<UserRights>(ViewableModules);
        }

        #region properties

        public RelayCommand<SelectionChangedEventArgs> TabSelectionChangedCommand { get; set; }
        public RelayCommand LogoutCommand { get; set; }

        public RelayCommand<UserRights> ViewableModulesCommand { get; set; }


        public const string ViewAdminMainMenuPropertyName = "ViewAdminMainMenu";
        private string _viewAdminMainMenu = "Collapsed";

        public string ViewAdminMainMenu
        {
            get { return _viewAdminMainMenu; }

            set
            {
                if (_viewAdminMainMenu == value)
                {
                    return;
                }
                _viewAdminMainMenu = value;
                RaisePropertyChanged(ViewAdminMainMenuPropertyName);
            }
        }

        public const string ViewCommodityMainMenuPropertyName = "ViewCommodityMainMenu";
        private string _viewCommodityMainMenu = "Visible";

        public string ViewCommodityMainMenu
        {
            get { return _viewCommodityMainMenu; }

            set
            {
                if (_viewCommodityMainMenu == value)
                {
                    return;
                }
                _viewCommodityMainMenu = value;
                RaisePropertyChanged(ViewCommodityMainMenuPropertyName);
            }
        }

        public const string ViewWarehouseMainMenuPropertyName = "ViewWarehouseMainMenu";
        private string _viewWarehouseMainMenu = "Collapsed";

        public string ViewWarehouseMainMenu
        {
            get { return _viewWarehouseMainMenu; }

            set
            {
                if (_viewWarehouseMainMenu == value)
                {
                    return;
                }
                _viewWarehouseMainMenu = value;
                RaisePropertyChanged(ViewWarehouseMainMenuPropertyName);
            }
        }

        public const string ViewActivityMainMenuPropertyName = "ViewActivityMainMenu";
        private string _viewActivityMainMenu = "Collapsed";

        public string ViewActivityMainMenu
        {
            get { return _viewActivityMainMenu; }

            set
            {
                if (_viewActivityMainMenu == value)
                {
                    return;
                }
                _viewActivityMainMenu = value;
                RaisePropertyChanged(ViewActivityMainMenuPropertyName);
            }
        }


        public const string ShowAdminMainMenuPropertyName = "ShowAdminMainMenu";
        private bool _showAdminMainMenu;

        public bool ShowAdminMainMenu
        {
            get { return _showAdminMainMenu; }

            set
            {
                if (_showAdminMainMenu == value)
                {
                    return;
                }
                _showAdminMainMenu = value;
                RaisePropertyChanged(ShowAdminMainMenuPropertyName);
            }
        }

        public const string ShowShowWarehouseMenuPropertyName = "ShowWarehouseMenu";
        private bool _showShowWarehouseMenu;

        public bool ShowWarehouseMenu
        {
            get { return _showShowWarehouseMenu; }

            set
            {
                if (_showShowWarehouseMenu == value)
                {
                    return;
                }
                _showShowWarehouseMenu = value;
                RaisePropertyChanged(ShowShowWarehouseMenuPropertyName);
            }
        }
        public const string IsUserLoggedInPropertyName = "IsUserLoggedIn";
        private bool _isUserLoggedIn;

        public bool IsUserLoggedIn
        {
            get { return _isUserLoggedIn; }

            set
            {
                if (_isUserLoggedIn == value)
                {
                    return;
                }
                if (value) SetProductInfo(); else ClearProductInfo();
                _isUserLoggedIn = value;
                RaisePropertyChanged(IsUserLoggedInPropertyName);
            }
        }

        public const string CanAccessPropertyName = "CanAccess";
        private bool _canAccess;

        public bool CanAccess
        {
            get { return _canAccess; }

            set
            {
                if (_canAccess == value)
                {
                    return;
                }
                _canAccess = value;
                RaisePropertyChanged(CanAccessPropertyName);
            }
        }

        public const string ReceptionTabSelectedPropertyName = "ReceptionTabSelected";
        private bool _receptionTabSelected;
        public bool ReceptionTabSelected
        {
            get { return _receptionTabSelected; }

            set
            {
                if (_receptionTabSelected == value)
                {
                    return;
                }
                _receptionTabSelected = value;
                RaisePropertyChanged(ReceptionTabSelectedPropertyName);
            }
        }
         
        public const string HubNamePropertyName = "HubName";
        private string _hubName = "";
        public string HubName
        {
            get
            {
                return _hubName;
            }

            set
            {
                if (_hubName == value)
                {
                    return;
                }

                RaisePropertyChanging(HubNamePropertyName);
                _hubName = value;
                RaisePropertyChanged(HubNamePropertyName);
            }
        } 

        public const string ProductNamePropertyName = "ProductName";
        private string _productName = "";
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
        private string _version = "";
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

        public const string WebServicePropertyName = "WebServiceUrl";
        private string _webService = "";
        public string WebServiceUrl
        {
            get
            {
                return _webService;
            }

            set
            {
                if (_webService == value)
                {
                    return;
                }

                RaisePropertyChanging(WebServicePropertyName);
                _webService = value;
                RaisePropertyChanged(WebServicePropertyName);
            }
        }
         
        public const string ApStatusPropertyName = "AppStatus";
        private string _appStatus = "";
        public string AppStatus
        {
            get
            {
                return _appStatus;
            }

            set
            {
                if (_appStatus == value)
                {
                    return;
                }

                RaisePropertyChanging(ApStatusPropertyName);
                _appStatus = value;
                RaisePropertyChanged(ApStatusPropertyName);
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

        #endregion

        #region methods

        private void LogOut()
        {
            MessageBoxResult boxResult = MessageBox.Show("Do you want to log out", "Log Out", MessageBoxButton.YesNo,
                                                         MessageBoxImage.Information);
            if (boxResult == MessageBoxResult.Yes)
            {
                using (var c = NestedContainer)
                {
                    var configParameters = Using<IConfigService>(c).ViewModelParameters;
                    configParameters.CurrentUserId = Guid.Empty;
                    configParameters.CurrentUsername = "";
                    configParameters.IsLogin = false;
                    configParameters.CurrentUserRights = null;
                    IsUserLoggedIn = false;
                    ShowAdminMainMenu = false;
                    ShowWarehouseMenu = false;
                }
                NavigateCommand.Execute(@"/views/LoginViews/LoginPage.xaml");
            }
        }

        
        public void TabSelectionChanged(SelectionChangedEventArgs e)
        {
            if (e.Source.GetType() != typeof (TabControl))
                return;

            TabItem tabItem = e.AddedItems[0] as TabItem;
            if (((TabControl) tabItem.Parent).Name != "tcMainPage")
                return;

            LoadSelectedTab(tabItem);
        }

        private void LoadSelectedTab(TabItem tabItem)
        {
            ReceptionTabSelected = false;
            ShowWarehouseMenu = false;
            string url = "";
            switch (tabItem.Name)
            {
                case "tbAdmin":
                    CanAccess = Permission();
                    ShowAdminMainMenu = CanAccess;
                    url = @"/views/admin/adminhome.xaml";
                    break;
                case "tbCommodity":
                    ShowAdminMainMenu = false;
                    ReceptionTabSelected = true;
                    CanAccess = true;
                    url = @"/views/CommodityReception/AwaitingReception.xaml";
                    break;
                case "tbSettings":
                    ShowAdminMainMenu = false;
                    ReceptionTabSelected = true;
                    CanAccess = true;
                    url = "/views/settings/agrisettingsmainpage.xaml";
                    break;
                case "tbReports":
                    ShowAdminMainMenu = false;
                    ReceptionTabSelected = true;
                    CanAccess = true;
                    url = "/views/reports/MainReport.xaml";
                    break;
                case "tbInventory":
                    ShowAdminMainMenu = true;
                    CanAccess = true;
                    url = @"/views/InventoryTransfer/InventoryInStorage.xaml";
                    break;
                case "tbActivity":
                    ShowAdminMainMenu = false;
                    ReceptionTabSelected = true;
                    CanAccess = true;
                    url = @"/views/activities/activitylisting.xaml";
                    break;
                case "tbWarehousing":
                    ShowAdminMainMenu = false;
                    ReceptionTabSelected = true;
                    ShowWarehouseMenu = true;
                    CanAccess = true;
                    url = @"/views/warehousing/WarehouseEntryListingPage.xaml";
                    break;
                default:
                    ShowAdminMainMenu = false;
                    url = @"/views/admin/adminhome.xaml";
                    break;
            }
           // NavigateCommand.Execute(url);
            NavigateWithPermission(url,CanAccess);
        }

        public void NavigateWithPermission(string url,bool canAccess)
        {
           if (!canAccess)
            {
                MessageBox.Show("\n \tAccess Denied !!!! \n \tYou don't have permission access this module.\n \tPlease Contact the Administrator", "User Permissions",MessageBoxButton.OKCancel);

                url = @"/views/CommodityReception/AwaitingReception.xaml";
                
                
                NavigateCommand.Execute(url);
                
                
            }

            NavigateCommand.Execute(url);
        }

        public bool Permission()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                User user = Using<IUserRepository>(c).GetById(Using<IConfigService>(c).ViewModelParameters.CurrentUserId);
                if (user != null && user.Group != null)
                {
                    bool canAccess = Using<IUserGroupRolesRepository>(c)
                              .GetByGroup(user.Group.Id)
                              .Any(s => s.CanAccess);
                    return canAccess;
                }
            }
            return false;
        }

        private void SetProductInfo()
        {
            using (var c = NestedContainer)
            {
                Config config = GetConfigParams();
                ViewModelParameters vmParams = GetConfigViewModelParameters();
                var hub = Using<ICostCentreRepository>(c).GetById(config.CostCentreId);
                HubName = "Hub : " + (hub == null ? "" : hub.Name);
                LoggedInUser = "Logged in as " + vmParams.CurrentUsername;
                
                ProductName = "Product \t\t:: Agrimanagr";
                Version = "Version \t\t:: " + ParseVersionNumber(Assembly.GetEntryAssembly()).ToString(); ;
                WebServiceUrl = "Web Service \t:: " + config.WebServiceUrl;
                AppStatus = "App Status \t:: Active";
          
            }
        }

        private void ClearProductInfo()
        {
            HubName = string.Empty;
            Version = string.Empty;
            WebServiceUrl = string.Empty;
            AppStatus = string.Empty;
            LoggedInUser = string.Empty;
        }

        private static Version ParseVersionNumber(Assembly assembly)
        {
            AssemblyName assemblyName = new AssemblyName(assembly.FullName);
            return assemblyName.Version;
        }

        private bool CanView(AgriUserRole role)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                User user = Using<IUserRepository>(c).GetById(Using<IConfigService>(c).ViewModelParameters.CurrentUserId);
                if (user != null && user.Group != null)
                {
                    bool canAccess = Using<IUserGroupRolesRepository>(c)
                        .GetByGroup(user.Group.Id)
                        .Any(s => s.UserRole == (int) role);
                              
                    return canAccess;
                }
            }
            return false;
        }
        public void ViewableModules(UserRights userRights)
        {
            
            ViewActivityMainMenu = userRights.CanViewActivities? "Visible":"Collapsed";
            ViewWarehouseMainMenu = userRights.CanViewWarehouse ? "Visible" : "Collapsed";
            ViewCommodityMainMenu = userRights.CanViewCommodity ? "Visible" : "Collapsed";
            ViewAdminMainMenu = userRights.CanViewAdmin ? "Visible" : "Collapsed";

        }

        #endregion


      
    }
}