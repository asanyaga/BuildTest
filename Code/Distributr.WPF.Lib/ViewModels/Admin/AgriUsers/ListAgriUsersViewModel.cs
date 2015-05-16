using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.MasterDataDTO.DataContracts;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.AgriUsers
{
    public class ListAgriUsersViewModel : ListingsViewModelBase
    {

        IPagenatedList<User> _pagenatedUserList;
        public ListAgriUsersViewModel()
        {
            UsersList = new ObservableCollection<ListUserItem>();
            LoadUsersCommand = new RelayCommand(SetupAndLoad);
            DeActivateUserCommand = new RelayCommand(DoDeactivateUser);
            ResetPasswordCommand = new RelayCommand(DoResetUserUserword);
            AddNewUserCommand = new RelayCommand(AddNewUser);
            UserTypesList = new ObservableCollection<UserType>();
            SelectedUserTypeChangedCommand = new RelayCommand(SelectedUserTypeChanged);
        }

        #region Properties
        public RelayCommand LoadUsersCommand { get; set; }
        public RelayCommand DeActivateUserCommand { get; set; }
        public RelayCommand ResetPasswordCommand { get; set; }
        public RelayCommand AddNewUserCommand { get; set; }
        public RelayCommand SelectedUserTypeChangedCommand { get; set; }

        public ObservableCollection<ListUserItem> UsersList { get; set; }
        public ObservableCollection<UserType> UserTypesList { get; set; }

        public const string SelectedUserItemPropertyName = "SelectedUserItem";
        private ListUserItem _selectedUserItem = null;
        public ListUserItem SelectedUserItem
        {
            get
            {
                return _selectedUserItem;
            }

            set
            {
                if (_selectedUserItem == value)
                {
                    return;
                }

                var oldValue = _selectedUserItem;
                _selectedUserItem = value;
                RaisePropertyChanged(SelectedUserItemPropertyName);
            }
        }
         
        public const string SelectedUserTypePropertyName = "SelectedUserType";
        private UserType  _selectedUserType = UserType.None;
        public UserType  SelectedUserType
        {
            get
            {
                return _selectedUserType;
            }

            set
            {

                RaisePropertyChanging(SelectedUserTypePropertyName);
                _selectedUserType = value;
                RaisePropertyChanged(SelectedUserTypePropertyName);
            }
        }

        public const string CanAddUserPropertyName = "CanAddUser";
        private bool _canAddUser = false;
        public bool CanAddUser
        {
            get
            {
                return _canAddUser;
            }

            set
            {
                if (_canAddUser == value)
                {
                    return;
                }

                var oldValue = _canAddUser;
                _canAddUser = value;
                RaisePropertyChanged(CanAddUserPropertyName);
            }
        }
        #endregion

        #region Methods

        void SetupVm()
        {
            PageTitle = "Hub Users";
            if (SearchText != string.Empty) SearchText = "";//avoid events triggerring
            if (ShowInactive) ShowInactive = false;

            using (StructureMap.IContainer c = NestedContainer)
            {
                CanAddUser = Using<IConfigService>(c).ViewModelParameters.CurrentUserRights.CanManageUsers;
            }
            LoadUserTypes();
        }

        #region Load Methods

        protected override void Load(bool isFirstLoad = false)
        {
            if (isFirstLoad)
                SetupVm();
            LoadAgiUsers();
        }

        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, _pagenatedUserList.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            LoadAgiUsers();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(_pagenatedUserList.PageNumber, _pagenatedUserList.PageCount, _pagenatedUserList.TotalItemCount,
                                       _pagenatedUserList.IsFirstPage, _pagenatedUserList.IsLastPage);
        }

        private void SetupAndLoad()
        {
            SetupVm();
            LoadAgiUsers();
        }

        private void LoadAgiUsers()
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    delegate
                        {
                            using (StructureMap.IContainer c = NestedContainer)
                            {
                                UsersList.Clear();
                                var userList = Using<IUserRepository>(c).GetAll(ShowInactive);
                                if (SelectedUserType == UserType.None)
                                    userList = userList.Where(n =>
                                                              (n.UserType == UserType.AgriHQAdmin ||
                                                               n.UserType == UserType.HubManager ||
                                                               n.UserType == UserType.Clerk ||
                                                               n.UserType == UserType.PurchasingClerk ||
                                                               n.UserType == UserType.Driver)
                                                              && n.Username.ToLower().Contains(SearchText.ToLower())
                                        );
                                else
                                    userList = userList.Where(n => n.UserType == SelectedUserType);
                                userList = userList.OrderBy(n => n.CostCentre)
                                    .ThenBy(n => n.Username);

                                _pagenatedUserList = new PagenatedList<User>(userList.AsQueryable(), CurrentPage,
                                                                             (int) ItemsPerPage,
                                                                             userList.Count());
                                var mappedUserItems = _pagenatedUserList.Select(MapUser);

                                foreach (var user in mappedUserItems)
                                {
                                    if (user.EntityStatus == (int) EntityStatus.Active)
                                        user.HlkDeactivateContent = GetLocalText("sl.users.grid.col.deactivate");
                                            //Deactivate
                                    else if (user.EntityStatus == (int) EntityStatus.Inactive)
                                        user.HlkDeactivateContent = GetLocalText("sl.users.grid.col.activate");
                                    UsersList.Add(user);
                                }
                                UpdatePagenationControl();
                            }
                        }), null);
        }

        ListUserItem MapUser(User user, int i)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                ListUserItem item = new ListUserItem
                                        {
                                            RowNumber = i + 1,
                                            Id = user.Id,
                                            UserName = user.Username,
                                            Password = user.Password,
                                            Mobile = !string.IsNullOrEmpty(user.Mobile)
                                                         ? user.Mobile
                                                         : "",
                                            UserType = ((UserType)user.UserType).ToString(),
                                            PIN = !string.IsNullOrEmpty(user.PIN) ? user.PIN : "",
                                            CanManage =
                                                (user.Id != Using<IConfigService>(c)
                                                             .ViewModelParameters
                                                             .CurrentUserId),
                                            EntityStatus = (int)user._Status,
                                        };
                return item;
            }
        }

        private void SelectedUserTypeChanged()
        {
            LoadAgiUsers();
        }

        #endregion

        protected override void EditSelected()
        {
            if(SelectedUserItem != null)
            {
               string url = "views/admin/users/edituser.xaml?" + SelectedUserItem.Id;
               SendNavigationRequestMessage(new Uri(url,UriKind.Relative));
            }
        }

        protected override void ActivateSelected()
        {
            using (var c = NestedContainer)
            {
                if (SelectedUserItem == null) return;
                if (SelectedUserItem.EntityStatus == (int) EntityStatus.Active)
                {
                    if (Using<IMasterDataUsage>(c).CheckAgriUserIsUsed(Using<IUserRepository>(c).GetById(SelectedUserItem.Id)))
                    {
                        MessageBox.Show(
                            "User " + SelectedUserItem.UserName +
                            " has been used to create transactions and thus cannot be deleted",
                            "Agrimanagr: Deactivate User", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                    DoDeactivateUser();
                }
                else if (SelectedUserItem.EntityStatus == (int) EntityStatus.Inactive)
                    DoActivateUser();
            }
        }

        private async void DoResetUserUserword()
        {
            if (SelectedUserItem == null) return;
            using (StructureMap.IContainer c = NestedContainer)
            {
                string newPassword = "12345678";
                User user = Using<IUserRepository>(c).GetById(SelectedUserItem.Id);
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                ResponseBool response = null;
                if (user != null)
                {
                    string pwd = Using<IOtherUtilities>(c).MD5Hash(newPassword);

                    var newUser = new UserItem
                    {
                        Username = user.Username,
                        PIN = user.PIN,
                        Mobile = user.Mobile,
                        MasterId = user.Id,
                        UserType = (int)user.UserType,
                        UserRoles = user.UserRoles,
                        DateLastUpdated = DateTime.Now,
                        CostCenterID = user.CostCentre,
                        StatusId = (int)EntityStatus.Active,
                        TillNumber = user.TillNumber,
                        CostCentreCode = Using<ICostCentreRepository>(c).GetById(user.CostCentre).CostCentreCode,
                        Password = pwd
                    };

                    response = await proxy.UserUpdateAsync(newUser);
                    MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Users", MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
            }
        }

        private async void DoDeactivateUser()
        {
            if (!CanAddUser)
            {
                MessageBox.Show("Sorry, you do not have sufficient rights to perform this action",
                                "Agrimanagr: Deactivate User", MessageBoxButton.OK);
                return;
            }
            if (MessageBox.Show(
                GetLocalText("sl.users.deactivate.messagebox.prompt")
                //"Are you sure you want to deactivate this user?"
                , GetLocalText("sl.users.deactivate.messagebox.caption") //"Confirm Deactivation"
                , MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                    ResponseBool response = null;
                    response = await proxy.UserDeactivateAsync(SelectedUserItem.Id);
                    MessageBox.Show(response.ErrorInfo, "Distributr: Manage User", MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
            }
        }

        private async void DoActivateUser()
        {
            if (!CanAddUser)
            {
                MessageBox.Show("Sorry, you do not have sufficient rights to perform this action", "Agrimanagr: Activate User", MessageBoxButton.OK);
                return;
            }
            if (MessageBox.Show(GetLocalText("sl.users.activate.messagebox.prompt")//"Are you sure you want to activate this user?"
                , GetLocalText("sl.users.activate.messagebox.caption")//"Confirm Deactivation"
                , MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                    ResponseBool response = null;
                    response = await proxy.UserActivateAsync(SelectedUserItem.Id);
                    MessageBox.Show(response.ErrorInfo, "Distributr: Manage User", MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
            }
        }

        protected override void DeleteSelected()
        {

            if (SelectedUserItem == null) return;
            if (!CanAddUser)
            {
                MessageBox.Show("Sorry, you do not have sufficient rights to perform this action", "Distributr: Deactivate User", MessageBoxButton.OK);
                return;
            }
            using (var c = NestedContainer)
            {
                if (
                    Using<IMasterDataUsage>(c).CheckAgriUserIsUsed(Using<IUserRepository>(c).GetById(SelectedUserItem.Id)))
                {
                    MessageBox.Show(
                        "User " + SelectedUserItem.UserName +
                        " has been used to create transactions and thus cannot be deleted",
                        "Agrimanagr: Deactivate User", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }
            DoDeleteUser();
        }

        private async void DoDeleteUser()
        {
            if (MessageBox.Show(GetLocalText("sl.users.delete.messagebox.prompt")//"Are you sure you want to delete this user?"
                , GetLocalText("sl.users.deactivate.messagebox.caption")//"Confirm Deactivation"
                , MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                    ResponseBool response = null;
                    var item = Using<IUserRepository>(c).GetById(SelectedUserItem.Id);
                    response = await proxy.UserDeleteAsync(SelectedUserItem.Id);

                    if (response.Success)
                        Using<IUserRepository>(c).SetAsDeleted(item);

                    MessageBox.Show(response.ErrorInfo, "Distributr: Manage User", MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
            }
        }

        public void AddNewUser()
        {
            SendNavigationRequestMessage(new Uri(@"\views\admin\users\edituser.xaml",
                                                                               UriKind.Relative));
        }

        private void LoadUserTypes()
        {
            Type _enumType = typeof(UserType);
            UserTypesList.Clear();
            FieldInfo[] infos = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo fi in infos.Where(n =>
                  n.Name == UserType.AgriHQAdmin.ToString() ||
                                    n.Name == UserType.None.ToString() ||
                                    n.Name == UserType.HubManager.ToString() ||
                                    n.Name == UserType.Clerk.ToString() ||
                                    n.Name == UserType.PurchasingClerk.ToString() ||
                                    n.Name == UserType.Driver.ToString()
                ))
                UserTypesList.Add((UserType)Enum.Parse(_enumType, fi.Name, true));
            SelectedUserType = UserType.None;
        }

        #endregion

        #region Helper Classes
        public class ListUserItem
        {
            public int RowNumber { get; set; }
            public Guid Id { get; set; }
            public string Code { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public string UserType { get; set; }
            public string Mobile { get; set; }
            public Guid CostCenterId { get; set; }
            public string CostCenter { get; set; }
            public string PIN { get; set; }
            public bool CanManage { get; set; }
            public string UserRoles { get; set; } //comma separated list of roles
            public string HlkDeactivateContent { get; set; }
            public int EntityStatus { get; set; }
        }
        #endregion
    }
}
