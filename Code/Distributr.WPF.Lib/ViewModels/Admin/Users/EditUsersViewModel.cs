using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.Core.Domain.Master.UserEntities;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using System;
using System.Reflection;
using System.Windows;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.MasterDataDTO.DataContracts;
using Distributr.Core.Workflow.Impl.AuditLogs;
using StructureMap;
using UserRole = Distributr.Core.Domain.Master.UserEntities.UserRole;

namespace Distributr.WPF.Lib.ViewModels.Admin.Users
{
    public class EditUsersViewModel : DistributrViewModelBase
    {
       public EditUsersViewModel()
        {
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(CancelAll);
            Load = new RelayCommand(DoLoad);
            userRoles = new ObservableCollection<string>();
            UserTypes = new ObservableCollection<UserType>();
           SalesmanTypes= new ObservableCollection<DistributorSalesmanType>();
        }

        #region Properties
        public ObservableCollection<UserType> UserTypes { get; set; }
        public ObservableCollection<DistributorSalesmanType> SalesmanTypes { get; set; }
        public ObservableCollection<string> userRoles;
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand Load { get; set; }

        public const string SelectedUserTypePropertyName = "SelectedUserType";
        private UserType _userType;
        public UserType SelectedUserType
        {
            get
            {
                return _userType;
            }

            set
            {
                if (_userType == value)
                {
                    return;
                }

                var oldValue = _userType;
                _userType = value;
                CanEditUserCode = _userType == UserType.DistributorSalesman;
                CanSetSalesmanType = _userType == UserType.DistributorSalesman;

                RaisePropertyChanged(SelectedUserTypePropertyName);
            }
        }

        public const string SelectedSalesmanTypePropertyName = "SelectedSalesmanType";
        private DistributorSalesmanType _salesmanType;
        public DistributorSalesmanType SelectedSalesmanType
        {
            get
            {
                return _salesmanType;
            }

            set
            {
                if (_salesmanType == value)
                {
                    return;
                }

                var oldValue = _salesmanType;
                _salesmanType = value;


                RaisePropertyChanged(SelectedSalesmanTypePropertyName);
            }
        }

        public const string UserNamePropertyName = "UserName";
        private string _userName = "";
        [Required(ErrorMessage = "User name is required.")]
        public string UserName
        {
            get
            {
                return _userName;
            }

            set
            {
                if (_userName == value)
                {
                    return;
                }

                var oldValue = _userName;
                _userName = value;

                RaisePropertyChanged(UserNamePropertyName);
            }
        }

        public const string CodePropertyName = "Code";
        private string _code = "";
        public string Code
        {
            get
            {
                return _code;
            }

            set
            {
                if (_code == value)
                {
                    return;
                }

                var oldValue = _code;
                _code = value;
                RaisePropertyChanged(CodePropertyName);
            }
        }

        public const string IdPropertyName = "Id";
        private Guid _id = Guid.Empty;
        public Guid Id
        {
            get
            {
                return _id;
            }

            set
            {
                if (_id == value)
                {
                    return;
                }

                var oldValue = _id;
                _id = value;
                CanEditUserType = Id == Guid.Empty;

                RaisePropertyChanged(IdPropertyName);
            }
        }

        public const string PINPropertyName = "PIN";
        private string _PIN = "";
        public string PIN
        {
            get
            {
                return _PIN;
            }

            set
            {
                if (_PIN == value)
                {
                    return;
                }

                var oldValue = _PIN;
                _PIN = value;

                RaisePropertyChanged(PINPropertyName);
            }
        }

        public const string MobilePropertyName = "Mobile";
        private string _mobile = "";
       //  [RegularExpression(mobileNumberRegex, ErrorMessage = "Invalid telephone number")]
        public string Mobile
        {
            get
            {
                return _mobile;
            }

            set
            {
                if (_mobile == value)
                {
                    return;
                }

                var oldValue = _mobile;
                _mobile = value;

                RaisePropertyChanged(MobilePropertyName);
            }
        }

        public const string PasswordPropertyName = "Password";
        private string _password = "";
        public string Password
        {
            get
            {
                return _password;
            }

            set
            {
                if (_password == value)
                {
                    return;
                }

                var oldValue = _password;
                _password = value;

                RaisePropertyChanged(PasswordPropertyName);
            }
        }

        public const string OldPasswordPropertyName = "OldPassword";
        private string _oldPassword = "";
        [Required(ErrorMessage = "Old Password is required")]
        public string OldPassword
        {
            get
            {
                return _oldPassword;
            }

            set
            {
                if (_oldPassword == value)
                {
                    return;
                }

                var oldValue = _oldPassword;
                _oldPassword = value;

                RaisePropertyChanged(OldPasswordPropertyName);
            }
        }

        public const string NewPasswordPropertyName = "NewPassword";
        private string _newPassword = "";
        [Required(ErrorMessage = "New password is required.")]
        public string NewPassword
        {
            get
            {
                return _newPassword;
            }

            set
            {
                if (_newPassword == value)
                {
                    return;
                }

                var oldValue = _newPassword;
                _newPassword = value;

                RaisePropertyChanged(NewPasswordPropertyName);
            }
        }

        public const string ConfirmPasswordPropertyName = "ConfirmPassword";
        private string _confirmPassword = "";
        [Required(ErrorMessage = "Confirm password.")]
        public string ConfirmPassword
        {
            get
            {
                return _confirmPassword;
            }

            set
            {
                if (_confirmPassword == value)
                {
                    return;
                }

                var oldValue = _confirmPassword;
                _confirmPassword = value;

                RaisePropertyChanged(ConfirmPasswordPropertyName);
            }
        }

        public const string CostCenterIDPropertyName = "CostCenterID";
        private Guid _costCenterID = Guid.Empty;
        public Guid CostCenterID
        {
            get
            {
                return _costCenterID;
            }

            set
            {
                if (_costCenterID == value)
                {
                    return;
                }

                var oldValue = _costCenterID;
                _costCenterID = value;

                RaisePropertyChanged(CostCenterIDPropertyName);
            }
        }

        public const string UserRolesPropertyName = "UserRoles";
        private UserRole _userRole;
        public UserRole UserRoles
        {
            get
            {
                return _userRole;
            }

            set
            {
                if (_userRole == value)
                {
                    return;
                }

                var oldValue = _userRole;
                _userRole = value;

                RaisePropertyChanged(UserRolesPropertyName);
            }
        }


        public const string IsAdministratorPropertyName = "IsAdministrator";
        private bool _isAdministrator = false;
        public bool IsAdministrator
        {
            get
            {
                return _isAdministrator;
            }

            set
            {
                if (_isAdministrator == value)
                {
                    return;
                }

                var oldValue = _isAdministrator;
                _isAdministrator = value;
                if (value)
                {
                    if (!userRoles.Contains(UserRole.Administrator.ToString()))
                        userRoles.Add(UserRole.Administrator.ToString());
                }
                else
                {
                    if (userRoles.Contains(UserRole.Administrator.ToString()))
                        userRoles.Remove(UserRole.Administrator.ToString());
                }

                RaisePropertyChanged(IsAdministratorPropertyName);
            }
        }

        public const string IsFinanceHandlerPropertyName = "IsFinanceHandler";
        private bool _isFinanceHandler = false;
        public bool IsFinanceHandler
        {
            get
            {
                return _isFinanceHandler;
            }

            set
            {
                if (_isFinanceHandler == value)
                {
                    return;
                }

                var oldValue = _isFinanceHandler;
                _isFinanceHandler = value;
                if (value)
                {
                    if (!userRoles.Contains(UserRole.FinanceHandler.ToString()))
                        userRoles.Add(UserRole.FinanceHandler.ToString());
                }
                else
                {
                    if (userRoles.Contains(UserRole.FinanceHandler.ToString()))
                        userRoles.Remove(UserRole.FinanceHandler.ToString());
                }
                RaisePropertyChanged(IsFinanceHandlerPropertyName);
            }
        }

        public const string IsInventoryHandlerPropertyName = "IsInventoryHandler";
        private bool _isInventoryHandler = false;
        public bool IsInventoryHandler
        {
            get
            {
                return _isInventoryHandler;
            }

            set
            {
                if (_isInventoryHandler == value)
                {
                    return;
                }

                var oldValue = _isInventoryHandler;
                _isInventoryHandler = value;
                if (value)
                {
                    if (!userRoles.Contains(UserRole.InventoryHandler.ToString()))
                        userRoles.Add(UserRole.InventoryHandler.ToString());
                }
                else
                {
                    if (userRoles.Contains(UserRole.InventoryHandler.ToString()))
                        userRoles.Remove(UserRole.InventoryHandler.ToString());
                }

                RaisePropertyChanged(IsInventoryHandlerPropertyName);
            }
        }

        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "";
        public string PageTitle
        {
            get
            {
                return _pageTitle;
            }

            set
            {
                if (_pageTitle == value)
                {
                    return;
                }

                var oldValue = _pageTitle;
                _pageTitle = value;
                RaisePropertyChanged(PageTitlePropertyName);
            }
        }

        public const string CanEditUserTypePropertyName = "CanEditUserType";
        private bool _canEditUserType = false;
        public bool CanEditUserType
        {
            get
            {
                return _canEditUserType;
            }

            set
            {
                if (_canEditUserType == value)
                {
                    return;
                }

                RaisePropertyChanging(CanEditUserTypePropertyName);
                _canEditUserType = value;
                RaisePropertyChanged(CanEditUserTypePropertyName);
            }
        }

        public const string CanEditUserCodePropertyName = "CanEditUserCode";
        private bool _canEditUserCode;
        public bool CanEditUserCode
        {
            get
            {
                return _canEditUserCode;
            }

            set
            {
                if (_canEditUserCode == value)
                {
                    return;
                }

                var oldValue = _canEditUserCode;
                _canEditUserCode = value;

                RaisePropertyChanged(CanEditUserCodePropertyName);
            }
        }

        public const string CanSetSalesmanTypePropertyName = "CanSetSalesmanType";
        private bool _canSetSalesmanType;
        public bool CanSetSalesmanType
        {
            get
            {
                return _canSetSalesmanType;
            }

            set
            {
                if (_canSetSalesmanType == value)
                {
                    return;
                }

                var oldValue = _canSetSalesmanType;
                _canSetSalesmanType = value;

                RaisePropertyChanged(CanSetSalesmanTypePropertyName);
            }
        }

        public const string TillNumberPropertyName = "TillNumber";
        private string _tillNumber = string.Empty;
        public string TillNumber
        {
            get
            {
                return _tillNumber;
            }

            set
            {
                if (_tillNumber == value)
                {
                    return;
                }

                var oldValue = _tillNumber;
                _tillNumber = value;
                RaisePropertyChanged(TillNumberPropertyName);
            }
        }

        public const string CanManageUserPropertyName = "CanManageUser";
        private bool _canManageUser = false;
        public bool CanManageUser
        {
            get
            {
                return _canManageUser;
            }

            set
            {
                if (_canManageUser == value)
                {
                    return;
                }

                var oldValue = _canManageUser;
                _canManageUser = value;
                RaisePropertyChanged(CanManageUserPropertyName);
            }
        }


        public const string BntCancelContentPropertyName = "BntCancelContent";
        private string _btnCancelContent = "Cancel";
        public string BntCancelContent
        {
            get
            {
                return _btnCancelContent;
            }

            set
            {
                if (_btnCancelContent == value)
                {
                    return;
                }

                var oldValue = _btnCancelContent;
                _btnCancelContent = value;
                RaisePropertyChanged(BntCancelContentPropertyName);
            }
        }

        bool showActionResultMsg = false;

        private string AuditLogEntry { get; set; }
        #endregion

        #region Methods

        public async void Save()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                ResponseBool response = null;
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                IConfigService _configService = Using<IConfigService>(c);
                if(!ChangingPassword)
                if (Id != Guid.Empty && !IsValid())
                    return;
                if (SelectedUserType == null || SelectedUserType == UserType.None)
                {
                    MessageBox.Show(GetLocalText("sl.user.edit.validate.usertype") /*"Select user type"*/,
                                    "Distributr: Invalid Field(s)", MessageBoxButton.OK);
                    return;
                }
                showActionResultMsg = true;

                #region New User

                var _userRoles =  userRoles.Select(n =>n).ToList();
                if (Id == Guid.Empty)
                {
                    NewPassword = "12345678";
                    var newUser = new UserItem
                                      {
                                          MasterId = Guid.NewGuid(),
                                          Username = UserName,
                                          PIN = PIN,
                                          Mobile = Mobile,
                                          Password = Using<IOtherUtilities>(c).MD5Hash(NewPassword),
                                          UserType = (int) SelectedUserType,
                                          UserRoles = _userRoles,
                                          StatusId = (int) EntityStatus.Active,
                                          DateCreated = DateTime.Now,
                                          DateLastUpdated = DateTime.Now,
                                          TillNumber = TillNumber,
                                          CostCenterID = _configService.Load().CostCentreId,
                                          CostCentreCode = Code,
                                           SalesmanType =(int) SelectedSalesmanType,
                                      };
                    response = await proxy.UserAddAsync(newUser);
                    AuditLogEntry = string.Format("Created New User: {0}; Code: {1}; And User Type", UserName,
                                                  SelectedUserType);
                    Using<IAuditLogWFManager>(c).AuditLogEntry("User Administration", AuditLogEntry);
                }

                    #endregion

                #region Update Existing

                else //updating existing
                {
                    var newUser = new UserItem
                                      {
                                          Username = UserName,
                                          PIN = PIN,
                                          Mobile = Mobile,
                                          MasterId = Id,
                                          UserType = (int) SelectedUserType,
                                          UserRoles = _userRoles,
                                          DateLastUpdated = DateTime.Now,
                                          CostCenterID = CostCenterID,
                                          StatusId = (int) EntityStatus.Active,
                                          TillNumber = TillNumber,
                                          CostCentreCode = Code,
                                          SalesmanType =(int) SelectedSalesmanType,
                                          PasswordChanged = 1,
                                          
                                      };

                    if (ChangingPassword)
                    {
                        var user = Using<IUserRepository>(c).GetById(Id);
                      if(ConfirmPassword.ToLower()==NewPassword.ToLower())
                      {
                          if (user != null && user.Password == Using<IOtherUtilities>(c).MD5Hash(OldPassword))
                          {
                              newUser.Password = Using<IOtherUtilities>(c).MD5Hash(NewPassword);
                          }
                          else
                          {
                              OldPassword = string.Empty;
                              NewPassword = string.Empty;
                              ConfirmPassword = string.Empty;
                              MessageBox.Show("Old password mismatch", "Distributr Error");
                              return;
                          }
                      }
                      else
                      {
                          OldPassword = string.Empty;
                          NewPassword = string.Empty;
                          ConfirmPassword = string.Empty;
                          MessageBox.Show("Confirm password and new password mismatch", "Distributr Error");
                          return;
                      }
                    }
                   
                    else
                        newUser.Password = Password;
                    response = await proxy.UserUpdateAsync(newUser);

                    AuditLogEntry = string.Format("Updated User: {0}; Code: {1};", UserName, SelectedUserType);
                    Using<IAuditLogWFManager>(c).AuditLogEntry("User Administration", AuditLogEntry);
                }

                MessageBox.Show(response.ErrorInfo, "Distributr: Manage Users", MessageBoxButton.OK, MessageBoxImage.Information);

                if (response.Success)
                {
                    ConfirmNavigatingAway = false;
                    SendNavigationRequestMessage(new Uri(@"\views\CommodityReception\AwaitingReception.xaml", UriKind.Relative));
                }

                #endregion
            }
        }

        void DoLoad()
        {
            SelectedUserType=UserType.None;
            EnumToList();
            LoadSalesmanTypes();
        }

        public void LoadByID(Guid id)
        {
            using (var c = NestedContainer)
            {
                if (ChangingPassword)
                    id = Using<IConfigService>(c).ViewModelParameters.CurrentUserId;

                if (id == Guid.Empty)
                {
                    InitializeBlank();
                    PageTitle = GetLocalText("sl.user.edit.new"); // "Create New User";
                }
                else
                {
                    PageTitle = GetLocalText("sl.user.edit.edit"); // "Edit User";
                    var item = Using<IUserRepository>(c).GetById(id);

                    if (item == null)
                        InitializeBlank();
                    else
                    {
                        Id = item.Id;
                        UserName = item.Username;
                        Password = item.Password;

                        if (!ChangingPassword)
                        {
                            NewPassword = item.Password;
                            OldPassword = item.Password;
                            ConfirmPassword = item.Password;
                        }
                        else
                        {
                            NewPassword = string.Empty;
                            OldPassword = string.Empty;
                            ConfirmPassword = string.Empty;
                        }

                        PIN = item.PIN;
                        Mobile = item.Mobile;
                        TillNumber = item.TillNumber;
                        EnumToList();
                       
                       
                        SelectedUserType = item.UserType;
                       
                        CostCenterID = item.CostCentre;

                        var cc = Using<ICostCentreRepository>(c).GetById(CostCenterID);
                        Code = cc != null ? cc.CostCentreCode : "";
                         if(item.UserType==UserType.DistributorSalesman && cc is DistributorSalesman)
                        {
                           
                            LoadSalesmanTypes();
                            SelectedSalesmanType = (cc as DistributorSalesman).Type;
                        }
                    }
                }
            }
        }

        public void ResetUserPassword()
        {
            NewPassword = "12345678";
            OldPassword = NewPassword;
            ConfirmPassword = NewPassword;
            ChangingPassword = true;
            Save();
        }

        public bool ChangingPassword = false;
        public void ChangeLocalPassword()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                User user = Using<IUserRepository>(c).GetById(Id);
                if (user != null)
                {
                    user.Password = Using<IOtherUtilities>(c).MD5Hash(NewPassword);
                    user._SetDateLastUpdated(DateTime.Now);
                    Using<IUserRepository>(c).Save(user);
                }

                ChangingPassword = false;
            }
        }

        public void SetUp()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                ClearAll();
                CanManageUser = Using<IConfigService>(c).ViewModelParameters.CurrentUserRights.CanManageUsers;

                if (Id == Using<IConfigService>(c).ViewModelParameters.CurrentUserId) // a user can edit his / her own details
                    CanManageUser = true;

                if (CanManageUser)
                    BntCancelContent = GetLocalText("sl.user.edit.cancel"); // "Cancel";
                else
                    BntCancelContent = GetLocalText("sl.user.edit.back"); // "Back";
            }
        }

        void InitializeBlank()
        {
            
            CanEditUserType = true;
            SelectedUserType = UserType.None;
            DoLoad();
        }

        void ClearAll()
        {
            Id = Guid.Empty;
            UserName = string.Empty;
            Password = string.Empty;
            OldPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
            PIN = string.Empty;
            Mobile = string.Empty;
            Code = string.Empty;
            TillNumber = string.Empty;
            CanManageUser = false;
            BntCancelContent = "Cancel";
        }

        public void CancelAll()
        {
            if (
                MessageBox.Show(
                    GetLocalText("sl.user.edit.cancel.messagebox.prompt")
                    // "Unsaved changes will be lost. Cancel creating user anyway?"
                    , GetLocalText("sl.user.edit.cancel.messagebox.caption") //"Distributr: Create User"
                    , MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ConfirmNavigatingAway = false;
            //    SendNavigationRequestMessage(new Uri("/views/MainViews/MainWindow.xaml", UriKind.Relative));
            }
        }

        public string GetHashedPassword(string psw)
        {
            string RetVal = "";
            RetVal = ObjectFactory.GetInstance<IOtherUtilities>().MD5Hash(psw);
            return RetVal;
        }

        public void EnumToList()
        {
            //get the type
            Type _enumType = typeof(UserType);

            //set up new collection
            UserTypes.Clear();
            UserTypes.Add(UserType.DistributorSalesman);

            ////retrieve the info for the type
            //FieldInfo[] infos;
            //infos = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

            ////Add each proper enum val to collection
            //foreach (FieldInfo fi in infos.Where(n => n.Name != UserType.HQAdmin.ToString()))
            //    UserTypes.Add((UserType)Enum.Parse(_enumType, fi.Name, true));
            SelectedUserType = UserTypes.FirstOrDefault();

        }
        public void LoadSalesmanTypes()
        {
            //get the type
            Type _enumType = typeof(DistributorSalesmanType);

            //set up new collection
            SalesmanTypes.Clear();

            //retrieve the info for the type
            FieldInfo[] infos;
            infos = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

            //Add each proper enum val to collection
            foreach (FieldInfo fi in infos)
                SalesmanTypes.Add((DistributorSalesmanType)Enum.Parse(_enumType, fi.Name, true));
            SelectedSalesmanType = DistributorSalesmanType.Salesman;

        }
        #endregion
    }
}