using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Windows;
using Distributr.Core;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.WPF.Lib.Services.Service.Sync;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.Services.UI;
using Distributr.WPF.Lib.ViewModels.MainPage;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json.Linq;
using StructureMap;

namespace Distributr.WPF.Lib.ViewModels.IntialSetup
{

    public class LoginViewModel : DistributrViewModelBase
    {
        private IAutoSyncService _autoSyncService;
        private static string _userName;
       
       
        public LoginViewModel()
        {
           
            //Login = new RelayCommand(LoginLocal);
            LoginCommand = new RelayCommand(DoLogin);
            CleanDBCommand = new RelayCommand(CleanDB);
            Init();
            if (System.Diagnostics.Debugger.IsAttached)
            {
                SetPasswordAndUsername();
            }
            _autoSyncService = ObjectFactory.GetInstance<IAutoSyncService>();

        }

        #region Properties
        public const string UsernamePropertyName = "Username";
        private string _username = "";
        public string Username
        {
            get { return _username; }

            set
            {
                if (_username == value)
                {
                    return;
                }

                var oldValue = _username;
                _username = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(UsernamePropertyName);
            }
        }

        public const string PasswordPropertyName = "Password";
        private string _password = "";
        public string Password
        {
            get { return _password; }

            set
            {
                if (_password == value)
                {
                    return;
                }

                var oldValue = _password;
                _password = value;



                // Update bindings, no broadcast
                RaisePropertyChanged(PasswordPropertyName);



            }
        }

        public RelayCommand Login { get; set; }
        
        public RelayCommand CleanDBCommand { get; set; }
        
        public const string UrlPropertyName = "Url";
        private string _url ="http://localhost:57200/"; 
        public string Url
        {
            get { return _url; }

            set
            {
                if (_url == value)
                {
                    return;
                }

                var oldValue = _url;
                _url = value;
                RaisePropertyChanged(UrlPropertyName);
            }
        }
        public RelayCommand LoginCommand { get; set; }
        public RelayCommand TestUrlCommand { get; set; }

        

        public const string IsInitializedPropertyName = "IsInitialized";
        private bool _isInitialized = false;
        public bool IsInitialized
        {
            get
            {
                using (NestedServices n = GetNestedServices())
                {
                    return n.ConfigService.Load().IsApplicationInitialized;
                }
            }
        }

        #endregion

        #region Methods
        private void Init()
        {
            if (IsInitialized)
                using (NestedServices n = GetNestedServices())
                {
                    Url = n.ConfigService.Load().WebServiceUrl;
                }
        }

        public void StopTimers()
        {
           
             _autoSyncService.StopAutomaticSync();
            
        }

        [Conditional("DEBUG")]
        private void SetPasswordAndUsername()
        {
            using (NestedServices n = GetNestedServices())
            {
                if ((n.ConfigService.Load().AppId == VirtualCityApp.Agrimanagr))
                    Username = "HubManager";
                else
                    Username = "kameme";
            }
           // Password = "12345678";
        }

        internal void DoLogin()
    {
            using (NestedServices n = GetNestedServices())
            {
               if (Username == "")
                {
                    MessageBox.Show("Enter Username  ");
                    return;
                }
                else if (Password == "")
                {
                    MessageBox.Show("Enter Password  ");
                    return;
                }
                if (IsInitialized)
                {
                    LoginLocal();
                }
                else if (CanConnectToServer(Url))
                {
                    ServerUrlRequest();
                }
                else
                {

                    MessageBox.Show("Connection failed, check the server URL", "Distributr Configuration",
                                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        private bool CanConnectToServer(string url)
        {
            bool canconnecto = false;
            try
            {
                string _url = url + "Test/gettestcostcentre";
                Uri uri = new Uri(_url, UriKind.Absolute);
                WebClient wc = new WebClient();
                string result = wc.DownloadString(uri);
                JObject jo = JObject.Parse(result);
                string costCentreId = (string) jo["costCentreId"];
                canconnecto = true;
                //MessageBox.Show("Connected Successfully", "Distributr Configuration", MessageBoxButton.OK);
            }
            catch(Exception ex)
            {
                //MessageBox.Show("Connection failed, check the server URL", "Distributr Configuration", MessageBoxButton.OK);
                canconnecto = false;
            }

            return canconnecto;
        }

        private void ServerUrlRequest()
        {
            SaveUrl();
            LoginFromServer();
        }

        private void SaveUrl()
        {
            using (NestedServices n = GetNestedServices())
            {
                Config config = n.ConfigService.Load();
                config.WebServiceUrl = Url;
                n.ConfigService.Save(config);
            }
        }

        private void LoginFromServer()
        {
            VirtualCityApp vcApp = GetConfigParams().AppId;
            using (NestedServices n = GetNestedServices())
            {
                UserType userType = vcApp == VirtualCityApp.Agrimanagr ? UserType.HubManager : UserType.WarehouseManager;
                CostCentreLoginResponse response = n.SetupApplication.LoginOnServer(Username, Password,
                                                                                             userType);
                //());

                if (response.ErrorInfo != null && response.ErrorInfo.Equals("Success"))
                {
                    SendLoggedInRequestMessage(new bool[] {false, true});

                    if (vcApp == Core.VirtualCityApp.Agrimanagr)
                        SendNavigationRequestMessage(new Uri("/Views/LoginViews/AgriConfiguration.xaml",
                                                             UriKind.Relative));
                    else
                        SendNavigationRequestMessage(new Uri("/Views/LoginViews/Configuration.xaml", UriKind.Relative));
                }
                else
                    MessageBox.Show(response.ErrorInfo);
            }
        }

   
        public void LoginLocal()
        {
            bool cansync = false;
            
            using (NestedServices n = GetNestedServices())
            {
                
                Config con = n.ConfigService.Load();
                if (!con.IsApplicationInitialized)
                {

                }
                if (Username == "")
                {
                    MessageBox.Show("Enter Username ");
                    return;
                }
                else if (Password == "")
                {
                    MessageBox.Show("Enter Password ");
                    return;
                }
                else
                {
                    User user = null;
                    user = n.UserRepository.Login(Username, n.OtherUtilities.MD5Hash(Password));
                    bool allowed = false;
                    if (GetConfigParams().AppId == Core.VirtualCityApp.Agrimanagr && user != null
                        && (user.UserType == UserType.HubManager || user.UserType == UserType.Clerk || user.UserType == UserType.PurchasingClerk))
                    {
                        allowed = true;
                    }
                    else if (GetConfigParams().AppId == Core.VirtualCityApp.Ditributr && user != null
                        && (user.UserType == UserType.OutletManager || user.UserType == UserType.DistributorSalesman || user.UserType == UserType.WarehouseManager))
                    {
                        allowed = true;
                        //user = Using<IUserRepository>(c).Login(Username, Using<IOtherUtilities>(c).MD5Hash(Password));
                    }
                    
                    if (user == null || !allowed)
                    {
                        MessageBox.Show("Invalid username and Password");
                        SendLoggedInRequestMessage(new bool[] {false, false});
                        return;
                    }
                    else
                    {
                        n.ConfigService.ViewModelParameters.CurrentUserId = user.Id;
                        n.ConfigService.ViewModelParameters.CurrentUsername = user.Username;
                        _userName = user.Username;
                        n.ConfigService.ViewModelParameters.IsLogin = true;
                        n.ConfigService.ViewModelParameters.CurrentUserRights = MapUserRights(user);
                        n.OtherUtilities.LoadMenu = true;
                        cansync = true;

                    }
                }
            }
            if (cansync)
            {
                _autoSyncService.StartAutomaticSync();
                SendLoggedInRequestMessage(new bool[] {true, true});

                if (GetConfigParams().AppId == Core.VirtualCityApp.Agrimanagr)
                {
                    
                     SimpleIoc.Default.GetInstance<AgrimanagrMainPageViewModel>().IsUserLoggedIn=true;
                    var rights = GetConfigViewModelParameters().CurrentUserRights;
                    if (rights.CanViewCommodity)
                    {
                        using (NestedServices n = GetNestedServices())
                        {
                             var name = _userName;
                           var loggedUser =  n.UserRepository.GetUser(name);
                            var isPassChangeEnabled =n.SettingsRepository.GetByKey(SettingsKeys.EnforcePasswordChange);
                    
                        if (isPassChangeEnabled != null && (loggedUser.PassChange == 0 && isPassChangeEnabled.Value == "True"))
                        {
                            SendNavigationRequestMessage(new Uri("/Views/LoginViews/ChangeUserPassword.xaml", UriKind.Relative));

                        }
                        else
                        {
                            SendNavigationRequestMessage(new Uri("/Views/CommodityReception/AwaitingReception.xaml", UriKind.Relative));

                        }
                        }
                       // SendNavigationRequestMessage(new Uri("/Views/CommodityReception/AwaitingReception.xaml", UriKind.Relative));
                    }
                    else
                    {
                        SendNavigationRequestMessage(new Uri("/Views/Reports/MainReport.xaml", UriKind.Relative));
                    }
                    SimpleIoc.Default.GetInstance<AgrimanagrMainPageViewModel>().ViewableModulesCommand.Execute(rights);

                }
               
                else
                    SendNavigationRequestMessage(new Uri("/Views/HomeViews/Home.xaml", UriKind.Relative));

                AddLogEntry("Login", "Successfully logged in as " + Username);
            }
        }

        public bool CheckIntialSetup()
        {
            using (NestedServices n = GetNestedServices())
            {
                Config con = n.ConfigService.Load();
                return con.IsApplicationInitialized;
            }
        }

        public UserRights MapUserRights(User user)
        {
            UserRights userRights = new UserRights();
            userRights.IsAdministrator = user.UserRoles.Contains(((int)UserRole.Administrator).ToString());

            //Overrides
            if (userRights.IsAdministrator)
                return IsAdministrator();

            userRights.IsFinanceHandler = user.UserRoles.Contains(((int)UserRole.FinanceHandler).ToString());

            userRights.IsInventoryHandler = user.UserRoles.Contains(((int)UserRole.InventoryHandler).ToString());

            //master data //outlets, routes, salesman route assigment,
            userRights.CanAddmasterData = user.UserRoles.Contains(((int)UserRole.RoleAddMasterData).ToString());
            userRights.CanViewMasterData = user.UserRoles.Contains(((int)UserRole.RoleViewMasterData).ToString());
            userRights.CanModifyMasterData = user.UserRoles.Contains(((int)UserRole.RoleModifyMasterData).ToString());

            //routes
            userRights.CanViewRoutes = user.UserRoles.Contains(((int)UserRole.RoleViewRoutes).ToString());
            userRights.CanManageRoutes = user.UserRoles.Contains(((int)UserRole.RoleCreateRoutes).ToString());

            //outlets
            userRights.CanViewOutlets = user.UserRoles.Contains(((int)UserRole.RoleViewOutlet).ToString());
            userRights.CanManageOutlet = user.UserRoles.Contains(((int)UserRole.RoleAddOutlet).ToString());

            //users
            userRights.CanViewUsers = user.UserRoles.Contains(((int)UserRole.RoleViewUser).ToString());
            userRights.CanManageUsers = user.UserRoles.Contains(((int)UserRole.RoleAddUser).ToString());

            //SalesmanRoute
            userRights.CanViewSalesmanRoutes = user.UserRoles.Contains(((int)UserRole.RoleViewSalesmanRoute).ToString());
            userRights.CanManageSalesmanRoutes = user.UserRoles.Contains(((int)UserRole.RoleCreateSalesmanRoute).ToString());

            //Contacts
            userRights.CanViewContacts = user.UserRoles.Contains(((int)UserRole.RoleViewContacts).ToString());
            userRights.CanManageContacts = user.UserRoles.Contains(((int)UserRole.RoleCreateContacts).ToString());

            //POS
            userRights.CanViewPOSSales = user.UserRoles.Contains(((int)UserRole.RoleCreatePOSSale).ToString());
            userRights.CanManagePOSSales = user.UserRoles.Contains(((int)UserRole.RoleCreatePOSSale).ToString());

            //Orders
            userRights.CanViewOrders = user.UserRoles.Contains(((int)UserRole.RoleViewOrder).ToString());
            userRights.CanCreateOrder = user.UserRoles.Contains(((int)UserRole.RoleCreateOrder).ToString());
            userRights.CanEditOrder = user.UserRoles.Contains(((int)UserRole.RoleEditOrder).ToString());
            userRights.CanApproveOrders = user.UserRoles.Contains(((int)UserRole.RoleApproveOrder).ToString());
            userRights.CanDispatchOrder = user.UserRoles.Contains(((int)UserRole.RoleDispatchOrder).ToString());

            //PurchaseOrders
            userRights.CanViewPurchaseOrders = user.UserRoles.Contains(((int)UserRole.RoleViewPurchaseOrder).ToString());
            userRights.CanCreatPurchaseOrders = user.UserRoles.Contains(((int)UserRole.RoleCreatePurchaseOrder).ToString());

            //Inventory - Inventory handler has all these rights
            userRights.CanViewInventory = user.UserRoles.Contains(((int)UserRole.RoleViewInventory).ToString());
            userRights.CanAdjustInventory = user.UserRoles.Contains(((int)UserRole.RoleAdjustInventory).ToString());
            userRights.CanIssueInventory = user.UserRoles.Contains(((int)UserRole.RoleIssueInventory).ToString());
            userRights.CanReceiveInventory = user.UserRoles.Contains(((int)UserRole.RoleReceiveInventory).ToString());
            userRights.CanReceiveReturnables = user.UserRoles.Contains(((int)UserRole.RoleReceiveReturnables).ToString());
            userRights.CanDispatchProducts = user.UserRoles.Contains(((int)UserRole.RoleDispatchProducts).ToString());
            userRights.CanReturnInventory = user.UserRoles.Contains(((int)UserRole.RoleReturnInventory).ToString());
            userRights.CanReconcileGenericReturnables = user.UserRoles.Contains(((int)UserRole.RoleReconcileGenericReturnables).ToString());
            userRights.CanViewReturnsList = user.UserRoles.Contains(((int)UserRole.RoleViewReturnsList).ToString());
            userRights.CanDoStockTake = user.UserRoles.Contains(((int)UserRole.RoleStockTake).ToString());

            //Payments - Finance guy has all these rights
            userRights.CanViewOutstandingPayments = user.UserRoles.Contains(((int)UserRole.RoleViewOutstandingPayments).ToString());
            userRights.CanIssueCreditNote = user.UserRoles.Contains(((int)UserRole.RoleIssueCreditNote).ToString());
            userRights.CanReceivePayments = user.UserRoles.Contains(((int)UserRole.RoleReceivePayments).ToString());

            //Reports
            userRights.CanViewReports = user.UserRoles.Contains(((int)UserRole.RoleViewReportsMenu).ToString());
            userRights.CanViewOrdersReports = user.UserRoles.Contains(((int)UserRole.RoleViewOrdersReports).ToString()); //Orders
            userRights.CanViewFinancialReports = user.UserRoles.Contains(((int)UserRole.RoleViewFinancialReports).ToString()); //Finance
            userRights.CanViewInventoryLevels = user.UserRoles.Contains(((int)UserRole.RoleViewInventoryLevels).ToString()); //Inventory
            userRights.CanViewInventoryIssues = user.UserRoles.Contains(((int)UserRole.RoleViewInventoryIssues).ToString()); //Inventory
            userRights.CanViewInventoryAdjustmentReports =
                user.UserRoles.Contains(((int)UserRole.RoleViewInventoryAdjustmentReports).ToString()); //Inventory
            userRights.CanViewAuditLog = user.UserRoles.Contains(((int)UserRole.RoleViewAuditLog).ToString());

            //Sync
            userRights.CanViewSyncMenu = user.UserRoles.Contains(((int)UserRole.RoleViewSyncMenu).ToString());
            userRights.CanViewSettings = user.UserRoles.Contains(((int)UserRole.RoleViewSettings).ToString());

            //Stockist
            userRights.CanCreateStockistOrder = user.UserRoles.Contains(((int)UserRole.RoleCreateStockistOrder).ToString());
            userRights.CanViewStockistOrder = user.UserRoles.Contains(((int)UserRole.RoleViewStockistOrder).ToString());

            //Banking
            userRights.CanViewUnderbankinglist = user.UserRoles.Contains(((int)UserRole.RoleViewUnderbankinglist).ToString());

            //OutletVisitDays 

            userRights.CanCreateOutletVisitDays = user.UserRoles.Contains(((int)UserRole.RoleCreateOutletVisitDays).ToString());

            //OutletPriority
            userRights.CanViewOutletPriority = user.UserRoles.Contains(((int)UserRole.RoleViewOutletPriority).ToString());

            //OutletTargets 
            userRights.CanViewOutletTargets = user.UserRoles.Contains(((int)UserRole.RoleViewOutletTargets).ToString());

            //SalesManTarget 
            userRights.CanViewSalesManTarget = user.UserRoles.Contains(((int)UserRole.RoleViewSalesManTarget).ToString());

            //Overrides
            if (userRights.IsFinanceHandler)
            {
                userRights = IsFinanceHander(userRights);
            }

            if (userRights.IsInventoryHandler)
            {
                userRights = IsInventoryHandler(userRights);
            }

            //userRights.CanViewAdminMenu = 

            #region AgrimanagrUserRights&Settings

            //var settings = ObjectFactory.GetInstance<ISettingsRepository>();

            //var canviewActivitiesSetting = settings.GetByKey(SettingsKeys.ShowFarmActivities)!=null ? settings.GetByKey(SettingsKeys.ShowFarmActivities).Value:null;
            //var canViewWarehouse = settings.GetByKey(SettingsKeys.ShowWarehouseReceipt)!=null?settings.GetByKey(SettingsKeys.ShowWarehouseReceipt).Value:null;

            //var canviewActivitesRights = user.UserRoles.Contains(((int)AgriUserRole.RoleViewActivities).ToString());

            //var canviewWarehouseRights = user.UserRoles.Contains(((int)AgriUserRole.RoleViewWarehouse).ToString());

            //userRights.CanViewActivities = canviewActivitesRights;
            //userRights.CanViewWarehouse = canviewWarehouseRights;

            userRights.CanViewActivities = ViewAgriModules(user, AgriUserRole.RoleViewActivities,SettingsKeys.ShowFarmActivities);
            userRights.CanViewWarehouse = ViewAgriModules(user, AgriUserRole.RoleViewWarehouse, SettingsKeys.ShowWarehouseReceipt);



            userRights.CanViewAdmin = user.UserRoles.Contains(((int)AgriUserRole.RoleViewAdmin).ToString());
            userRights.CanViewCommodity = user.UserRoles.Contains(((int)AgriUserRole.RoleViewCommodity).ToString());
            #endregion

            return userRights;
        }

        private UserRights IsAdministrator()
        {
            UserRights userRights = null;
            if (GetConfigParams().AppId == Core.VirtualCityApp.Agrimanagr)
            {
                var settings = ObjectFactory.GetInstance<ISettingsRepository>();

                userRights = new UserRights
                {
                    CanViewActivities = true,
                    CanViewAdmin = true,
                    CanViewCommodity = true,
                    CanViewWarehouse = true,
                };
            }
            if (GetConfigParams().AppId == Core.VirtualCityApp.Ditributr)
            {
                userRights = new UserRights
                {
                    IsAdministrator = true,
                    IsFinanceHandler = true,
                    IsInventoryHandler = true,
                    CanAddmasterData = true,
                    CanAdjustInventory = true,
                    CanApproveOrders = true,
                    CanCreateOrder = true,
                    CanCreatPurchaseOrders = true,
                    CanDispatchOrder = true,
                    CanDispatchProducts = true,
                    CanEditOrder = true,
                    CanIssueCreditNote = true,
                    CanIssueInventory = true,
                    CanManageContacts = true,
                    CanManageOutlet = true,
                    CanManagePOSSales = true,
                    CanManageRoutes = true,
                    CanManageSalesmanRoutes = true,
                    CanManageUsers = true,
                    CanModifyMasterData = true,
                    CanReceiveInventory = true,
                    CanReceiveReturnables = true,
                    CanReturnInventory = true,
                    CanViewAdminMenu = true,
                    CanViewAuditLog = true,
                    CanViewContacts = true,
                    CanViewFinancialReports = true,
                    CanViewInventory = true,
                    CanViewInventoryAdjustmentReports = true,
                    CanViewInventoryIssues = true,
                    CanViewInventoryLevels = true,
                    CanDoStockTake = true,
                    CanViewMasterData = true,
                    CanViewOrders = true,
                    CanViewOrdersReports = true,
                    CanViewOutlets = true,
                    CanViewOutstandingPayments = true,
                    CanViewPOSSales = true,
                    CanViewPurchaseOrders = true,
                    CanViewReports = true,
                    CanViewRoutes = true,
                    CanViewSalesmanRoutes = true,
                    CanViewUsers = true,
                    CanReconcileGenericReturnables = true,
                    CanViewReturnsList = true,
                    CanViewSettings = true,
                    CanViewSyncMenu = true,
                    CanReceivePayments = true,
                };
            }
            return userRights;
        }

        private UserRights IsFinanceHander(UserRights userRights)
        {
            userRights.CanViewOutstandingPayments = true;
            userRights.CanIssueCreditNote = true;
            userRights.CanViewFinancialReports = true;
            userRights.CanReceivePayments = true;

            return userRights;
        }

        private UserRights IsInventoryHandler(UserRights userRights)
        {
            userRights.CanViewInventory = true;
            userRights.CanAdjustInventory = true;
            userRights.CanIssueInventory = true;
            userRights.CanReceiveInventory = true;
            userRights.CanReceiveReturnables = true;
            userRights.CanDispatchProducts = true;
            userRights.CanReturnInventory = true;
            userRights.CanReconcileGenericReturnables = true;
            userRights.CanViewReturnsList = true;
            userRights.CanViewInventoryLevels = true;
            userRights.CanViewInventoryIssues = true;
            userRights.CanViewInventoryAdjustmentReports = true;
            userRights.CanDoStockTake = true;

            return userRights;
        }


        private bool ViewAgriModules(User user,AgriUserRole role,SettingsKeys settingKey)
        {
            var settings = ObjectFactory.GetInstance<ISettingsRepository>();

            bool result = false;

            var canViewModuleSetting = settings.GetByKey(settingKey) != null ? settings.GetByKey(SettingsKeys.ShowFarmActivities).Value : null;
            var canViewModuleRight = user.UserRoles.Contains(((int)role).ToString());

            if (canViewModuleSetting == null || Convert.ToBoolean(canViewModuleSetting)==false)
            {
                result = false;
                //return result;
            }
            if (canViewModuleSetting != null && Convert.ToBoolean(canViewModuleSetting))
            {   
                result = true;
                if (!canViewModuleRight)
                    result = false;

            }



            return result;


        }
        private void CleanDB()
        {
            using (NestedServices n = GetNestedServices())
            {
             
             n.ConfigService.CleanLocalDB();
                MessageBox.Show("DB Cleaned");
            }
            _autoSyncService.StopAutomaticSync();
        }

        #endregion
    }
}