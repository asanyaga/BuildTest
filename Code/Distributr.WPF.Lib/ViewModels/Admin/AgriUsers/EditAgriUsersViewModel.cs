using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Factory.Master.Impl;
using Distributr.Core.MasterDataDTO.DataContracts;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Workflow.Impl.AuditLogs;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.ViewModels.Admin.Contacts;
using Distributr.WPF.Lib.ViewModels.Admin.Routes;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Admin.AgriUsers
{
    public class EditAgriUsersViewModel : MasterEntityContactUtilsViewModel
    {
        IDistributorServiceProxy _proxy;
        IEnumerable<PurchasingClerkRoute> _assignedRoutes;
      
        public EditAgriUsersViewModel()
        {
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(CancelAll);
            AssignSelectedCommand = new RelayCommand(AssignRoutes);
            AssignAllCommand = new RelayCommand(AssignAllRoutes);
            UnassignSelectedCommand = new RelayCommand(UnassignRoutes);
            UnassignAllCommand = new RelayCommand(UnassignAllRoutes);

            AssignedRoutesList = new ObservableCollection<VMRouteItem>();
            UnassignedRoutesList = new ObservableCollection<VMRouteItem>();
            AllRoutesCache = new ObservableCollection<VMRouteItem>();
            UserTypeList = new ObservableCollection<UserType>();
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(CancelAll);
            ContactsList = new ObservableCollection<VMContactItem>();
        }

        #region properties

        public ObservableCollection<VMRouteItem> AssignedRoutesList { get; set; }
        public ObservableCollection<VMRouteItem> UnassignedRoutesList { get; set; }
        public ObservableCollection<VMRouteItem> AllRoutesCache { get; set; }
        public ObservableCollection<UserType> UserTypeList { get; set; }

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand AssignSelectedCommand { get; set; }
        public RelayCommand AssignAllCommand { get; set; }
        public RelayCommand UnassignSelectedCommand { get; set; }
        public RelayCommand UnassignAllCommand { get; set; }

        private string AuditLogEntry { get; set; }
        public const string UserPropertyName = "User";
        private User _user = null;
        public User User
        {
            get { return _user; }

            set
            {
                if (_user == value)
                {
                    return;
                }

                var oldValue = _user;
                _user = value;
                RaisePropertyChanged(UserPropertyName);
            }
        }

        public const string CodePropertyName = "Code";
        private string _code = null;
        public string Code
        {
            get { return _code; }

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

        public const string SelectedUserTypePropertyName = "SelectedUserType";
        private UserType _userType;
        [Required(ErrorMessage = "User type is required")]
        public UserType SelectedUserType
        {
            get { return _userType; }

            set
            {
                //if (_userType == value)
                //{
                //    return;
                //}

                var oldValue = _userType;
                _userType = value;
                CanEditUserCode = _userType == UserType.PurchasingClerk;
                AssignRouteVisibility = _userType == UserType.PurchasingClerk ? Visibility.Visible : Visibility.Collapsed;

                RaisePropertyChanged(SelectedUserTypePropertyName);
            }
        }

        public const string AssignRouteVisibilityPropertyName = "AssignRouteVisibility";
        private Visibility _assignRouteVisibility = Visibility.Collapsed;
        public Visibility AssignRouteVisibility
        {
            get { return _assignRouteVisibility; }

            set
            {
                if (_assignRouteVisibility == value)
                {
                    return;
                }

                var oldValue = _assignRouteVisibility;
                _assignRouteVisibility = value;

                RaisePropertyChanged(AssignRouteVisibilityPropertyName);
            }
        }

        public const string CanEditUserCodePropertyName = "CanEditUserCode";
        private bool _canEditUserCode;
        public bool CanEditUserCode
        {
            get { return _canEditUserCode; }

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

                RaisePropertyChanged(PasswordPropertyName);
            }
        }

        public const string OldPasswordPropertyName = "OldPassword";
        private string _oldPassword = "";
        [Required(ErrorMessage = "Old Password is required")]
        public string OldPassword
        {
            get { return _oldPassword; }

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
            get { return _newPassword; }

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
            get { return _confirmPassword; }

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

        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "Edit User";    
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

                RaisePropertyChanging(PageTitlePropertyName);
                _pageTitle = value;
                RaisePropertyChanged(PageTitlePropertyName);
            }
        }

        public bool CreateDriver { get; set; }

        #endregion

        #region methods

        public void Setup()
        {
            LoadUserTypes();
            LoadEntityContacts(User);
            LoadRouteAssignment();
        }

        private void LoadRouteAssignment()
        {
            using (var c = NestedContainer)
            {
                AssignedRoutesList.Clear();
                UnassignedRoutesList.Clear();
                AllRoutesCache.Clear();

                var query = new QueryPurchasingClerkRoute();
                query.ShowInactive = false;

                _assignedRoutes = Using<IPurchasingClerkRouteRepository>(c).Query(query).Where(
                   n => n.PurchasingClerkRef.Id == User.CostCentre);

                //_assignedRoutes = Using<IPurchasingClerkRouteRepository>(c).GetAll().Where(
                //    n => n.PurchasingClerkRef.Id == User.CostCentre);
                _assignedRoutes.OrderBy(n => n.Route.Name).ToList().ForEach(
                    n => AssignedRoutesList.Add(new VMRouteItem() {IsSelected = false, Route = n.Route}));

                var allRoutesCache =
                    Using<IRouteRepository>(c).GetAll().OrderBy(n => n.Name)
                        .ToList();
                allRoutesCache.ForEach(n => AllRoutesCache.Add(new VMRouteItem {Route = n, IsSelected = false}));

                var unassignedRoutes =
                    AllRoutesCache.Where(n => AssignedRoutesList.All(p => p.Route.Id != n.Route.Id)).ToList();
                unassignedRoutes.ForEach(UnassignedRoutesList.Add);
            }
        }

        public void Load(Guid userId)
        {
            ContactsList.Clear();
            using (var c = NestedContainer)
            {
                if (userId == Guid.Empty)
                {
                    PageTitle = "Create User";

                    User = new User(Guid.NewGuid());
                    Code = string.Empty;
                    User.Password = Using<IOtherUtilities>(c).MD5Hash("12345678");
                    User.CostCentre = GetConfigParams().CostCentreId;
                }
                else
                {
                    var user = Using<IUserRepository>(c).GetById(userId);
                    User = user.Clone<User>();
                    PageTitle = "Edit User";
                    if (User == null)
                    {
                        MessageBox.Show("Sorry, there is no record of user with id " + userId +
                                        " in your database. Try syncing master data", "Agrimanager: Error Editing User",
                                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                    Code = Using<ICostCentreRepository>(c).GetById(User.CostCentre).CostCentreCode;
                    Password = User.Password;
                }
                NewPassword = User.Password;
            }

            Setup();
            if (User._Status != EntityStatus.New)
                SelectedUserType = User.UserType;

            if (CreateDriver)
            {
                PageTitle = "Create Driver";
                SelectedUserType = UserType.Driver;
            }
        }

        private async void Save()
        {
            SetValues();
            if (!IsValid(User))
                return;
            using (var c = NestedContainer)
            {
                _proxy = Using<IDistributorServiceProxy>(c);
                ResponseBool response = null;
                if (SelectedUserType == UserType.None)
                {
                    MessageBox.Show(GetLocalText("sl.user.edit.validate.usertype") /*"Select user type"*/,
                                    "Distributr: Invalid Field(s)", MessageBoxButton.OK);
                    return;
                }

                if (User.UserType == UserType.PurchasingClerk)
                {
                    response = await SavePurchasingClerkCostCentre();
                    if (!response.Success) return;
                }
                else
                {
                    var userItem = CreateUserItem(GetConfigParams().CostCentreId);

                    response = await _proxy.UserAddAsync(userItem);
                    AuditLogEntry = string.Format("Created New User: {0}; Code: {1}; And User Type", userItem.Username,
                                                  SelectedUserType);
                    Using<IAuditLogWFManager>(c).AuditLogEntry("User Administration", AuditLogEntry);

                    MessageBox.Show(response.ErrorInfo, "Agrimanagr: Manage Users", MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
                if (response.Success)
                {
                    if (await SaveContacts(User))
                    {
                        ConfirmNavigatingAway = false;
                        SendNavigationRequestMessage(new Uri(@"\views\admin\users\listusers.xaml",
                                                                            UriKind.Relative));
                    }
                }
            }
        }

        void SetValues()
        {
            User.UserType = SelectedUserType;
        }

        UserItem CreateUserItem(Guid costCentreId)
        {
            UserItem userItem = new UserItem()
            {
                MasterId = User.Id,
                Username = User.Username,
                UserType = (int)User.UserType,
                TillNumber = User.TillNumber,
                Mobile = User.Mobile,
                PIN = User.PIN,
                CostCenterID = costCentreId,
                StatusId = (int)EntityStatus.Active,
                DateCreated = DateTime.Now,
                DateLastUpdated = DateTime.Now,
                Password = User.Password, 
                CostCentreCode = Code,
            };
            return userItem;
        }

        private async Task<ResponseBool> SavePurchasingClerkCostCentre()
        {
            ResponseBool response = new ResponseBool {Success = false};
            using (var c = NestedContainer)
            {
                _proxy = Using<IDistributorServiceProxy>(c);

                Guid ccId = User._Status == EntityStatus.New ? Guid.NewGuid() : User.CostCentre;
                PurchasingClerk pClerk =
                    Using<CostCentreFactory>(c).CreateCostCentre(ccId, CostCentreType.PurchasingClerk,
                                                                 Using<ICostCentreRepository>(c).GetById(
                                                                     GetConfigParams().CostCentreId)) as PurchasingClerk;
                pClerk.Name = User.Username;
                pClerk.CostCentreCode = Code;


                User.CostCentre = pClerk.Id;
                pClerk.User = User;
                pClerk.PurchasingClerkRoutes = GetRoutesAssigned(pClerk);

                PurchasingClerkItem pClerkItem = new PurchasingClerkItem()
                {
                    MasterId = pClerk.Id,
                    Name = pClerk.Name,
                    ParentCostCentreId = pClerk.ParentCostCentre.Id,
                    StatusId = (int)EntityStatus.Active,
                    CostCentreTypeId = (int)pClerk.CostCentreType,
                    CostCentreCode = pClerk.CostCentreCode
                };
                pClerkItem.UserItem = CreateUserItem(pClerk.Id);
                pClerkItem.PurchasingClerkRouteItems = pClerk.PurchasingClerkRoutes.Select(Map).ToList();

                AuditLogEntry = string.Format("Created purchasing clerk costcentre for user: {0};", User.Username);
                Using<IAuditLogWFManager>(c).AuditLogEntry("User Administration", AuditLogEntry);
                response = await _proxy.PurchasingCerkAddAsync(pClerkItem);
                if(response.Success)
                {
                   ChangeAllocation(pClerkItem);
                }
                MessageBox.Show(response.ErrorInfo, "Distributr: Manage purchasing clerk", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            return response;
        }

        private void ChangeAllocation(PurchasingClerkItem purchasingClerkItem)
        {
            using (var container=NestedContainer)
            {
               // Route route = null;
                var _purchasingClerkRouteRepository = Using<IPurchasingClerkRouteRepository>(container);
               // var _routeRepository = Using<IRouteRepository>(container);
                //var 
                //var purchasingClerk = _costCentreFactory.CreateCostCentre(purchasingClerkItem.MasterId, CostCentreType.PurchasingClerk, hub)
                //        as PurchasingClerk;

                var existingAssignedRoutes = _purchasingClerkRouteRepository.GetAll(true)
                           .Where(n => n.PurchasingClerkRef.Id == purchasingClerkItem.MasterId);
                var deletedRouteAssignments = existingAssignedRoutes
                    .Where(n => purchasingClerkItem.PurchasingClerkRouteItems.Select(x => x.RouteId).All(x => x != n.Route.Id));
                //foreach (var item in purchasingClerkItem.PurchasingClerkRouteItems)
                //{
                //    route = _routeRepository.GetById(item.RouteId);
                //    var pcr = new PurchasingClerkRoute(item.MasterId)
                //    {
                //        Route = route,
                //        PurchasingClerkRef = new CostCentreRef { Id = purchasingClerkItem.MasterId }
                //    };
                //    _routeRepository.Save(route);
                //    purchasingClerk.PurchasingClerkRoutes.Add(pcr);
                //}

                foreach (var item in deletedRouteAssignments)
                {
                    _purchasingClerkRouteRepository.SetAsDeleted(item);
                }
            }
        }

        PurchasingClerkRouteItem Map(PurchasingClerkRoute pcRoute)
        {
            PurchasingClerkRouteItem mapped = new PurchasingClerkRouteItem
                                                  {
                                                      MasterId = pcRoute.Id,
                                                      PurchasingClerkCostCentreId = pcRoute.PurchasingClerkRef.Id,
                                                      RouteId = pcRoute.Route.Id,
                                                      StatusId = (int) EntityStatus.Active,
                                                  };
            return mapped;
        }

        private List<PurchasingClerkRoute> GetRoutesAssigned(PurchasingClerk purchasingClerk)
        {
            using (var c = NestedContainer)
            {
                var assignedRouteItems = new List<PurchasingClerkRoute>();
                foreach (VMRouteItem item in AssignedRoutesList)
                {
                    PurchasingClerkRoute pcrItem = null;
                    var existing = _assignedRoutes.FirstOrDefault(n => n.Route.Id == item.Route.Id && n.PurchasingClerkRef.Id == purchasingClerk.Id);

                    pcrItem = new PurchasingClerkRoute(Guid.NewGuid())
                                  {
                                      Id = existing == null ? Guid.NewGuid() : existing.Id,
                                      Route = item.Route,
                                      _Status = EntityStatus.Active,
                                      PurchasingClerkRef = new CostCentreRef {Id = purchasingClerk.Id}
                                  };

                    assignedRouteItems.Add(pcrItem);
                    AuditLogEntry = string.Format("Assigned Route: {0}; To Costcentre: {1};", item.Route.Name,
                                                  purchasingClerk.Id);
                    Using<IAuditLogWFManager>(c).AuditLogEntry("User Administration", AuditLogEntry);
                }

                return assignedRouteItems;
            }
        }

        private void AssignRoutes()
        {
            List<VMRouteItem> selected = new List<VMRouteItem>();
            UnassignedRoutesList.Where(n => n.IsSelected).ToList().ForEach(selected.Add);
            foreach (var item in selected)
            {
                if(AssignedRoutesList.All(n => n.Route.Id != item.Route.Id))
                {
                    AssignedRoutesList.Add(item);
                    UnassignedRoutesList.Remove(item);
                }
            }
        }

        private void AssignAllRoutes()
        {
            List<VMRouteItem> selected = new List<VMRouteItem>();
            UnassignedRoutesList.ToList().ForEach(selected.Add);
            foreach (var item in selected)
            {
                if (AssignedRoutesList.All(n => n.Route.Id != item.Route.Id))
                {
                    AssignedRoutesList.Add(item);
                    UnassignedRoutesList.Remove(item);
                }
            }
        }

        private void UnassignRoutes()
        {
            List<VMRouteItem> selected = new List<VMRouteItem>();
            AssignedRoutesList.Where(n => n.IsSelected).ToList().ForEach(selected.Add);
            foreach (var item in selected)
            {
                if (UnassignedRoutesList.All(n => n.Route.Id != item.Route.Id))
                {
                    UnassignedRoutesList.Add(item);
                    AssignedRoutesList.Remove(item);
                }
            }
        }

        private void UnassignAllRoutes()
        {
            List<VMRouteItem> selected = new List<VMRouteItem>();
            AssignedRoutesList.ToList().ForEach(selected.Add);
            foreach (var item in selected)
            {
                if (UnassignedRoutesList.All(n => n.Route.Id != item.Route.Id))
                {
                    UnassignedRoutesList.Add(item);
                    AssignedRoutesList.Remove(item);
                }
            }
        }

        private void LoadUserTypes()
        {
           
            Type _enumType = typeof (UserType);
            UserTypeList.Clear();
            FieldInfo[] infos = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo fi in infos.Where(n =>
                /* n.Name == UserType.AgriHQAdmin.ToString() || WE DON'T WANT TO ADD HQ ADMIN USER HERE*/
                                    n.Name == UserType.None.ToString() ||
                                    n.Name == UserType.HubManager.ToString() ||
                                    n.Name == UserType.Clerk.ToString() ||
                                    n.Name == UserType.PurchasingClerk.ToString() ||
                                    n.Name == UserType.Driver.ToString()
                ))
                UserTypeList.Add((UserType) Enum.Parse(_enumType, fi.Name, true));
            SelectedUserType = UserType.None;
        }

        private void CancelAll()
        {
            if(MessageBox.Show("Unsaved changes will be lost. Do you want to continue?", "Agrimanagr: Edit user details", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                SendNavigationRequestMessage(new Uri(@"\views\admin\users\listusers.xaml",
                                                                            UriKind.Relative));
            }
        }

        protected override void AddOrEditContact(Button btnAdd)
        {
            base.AddOrEditContact(btnAdd, User);
        }

        protected override void EditContact(VMContactItem contactItem)
        {
            base.EditContact(contactItem, User);
        }

        #endregion
    }

    #region Helper classes

    

    public class VMRouteItem
    {
        public Route Route { get; set; }
        public bool IsSelected { get; set; }
    }

    #endregion
}
