using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.MasterDataDTO.DataContracts;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.Users
{
    public class ListUsersViewModel : ListingsViewModelBase
    {
       public ListUsersViewModel()
        {
            Users = new ObservableCollection<ListUserItem>();
            Title = "List Users";
           CreateNewUserCommand=new RelayCommand(AddUser);
           ResetUserPasswordCommand=new RelayCommand(ResetUserPassword);
        }

        #region properties
        internal IPagenatedList<User> PagedList;
        public ObservableCollection<ListUserItem> Users { get; set; }
        public RelayCommand CreateNewUserCommand { get; set; }
        public RelayCommand ResetUserPasswordCommand { get; set; }
        public const string SelectedUserPropertyName = "SelectedUser";
        private ListUserItem _selecteduser = null;
        public ListUserItem SelectedUser
        {
            get { return _selecteduser; }

            set
            {
                if (_selecteduser == value)
                {
                    return;
                }

                var oldValue = _canAddUser;
                _selecteduser = value;
                RaisePropertyChanged(SelectedUserPropertyName);
            }
        }

        public const string CanAddUserPropertyName = "CanAddUser";
        private bool _canAddUser = false;

        public bool CanAddUser
        {
            get { return _canAddUser; }

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

        public const string UserIdPropertyName = "UserId";
        private Guid _userId = Guid.Empty;

        public Guid UserId
        {
            get { return _userId; }

            set
            {
                if (_userId == value)
                {
                    return;
                }

                var oldValue = _userId;
                _userId = value;
                RaisePropertyChanged(UserIdPropertyName);
            }
        }

        public const string TitlePropertyName = "Title";
        private string _title = "";

        public string Title
        {
            get { return _title; }

            set
            {
                if (_title == value)
                {
                    return;
                }

                var oldValue = _title;
                _title = value;
                RaisePropertyChanged(TitlePropertyName);
            }
        }

        public const string ShowInactivePropertyName = "ShowInactive";
        private bool _showInactive = false;

        public bool ShowInactive
        {
            get { return _showInactive; }

            set
            {
                if (_showInactive == value)
                {
                    return;
                }

                var oldValue = _showInactive;
                _showInactive = value;
                RaisePropertyChanged(ShowInactivePropertyName);
            }
        }

        

        #endregion
        
        #region Methods
        protected override void Load(bool isFirstLoad = false)
        {
           if(isFirstLoad)
               SetUp();
            LoadUsers();
        }

        void AddUser()
        {
            string url = url = "views/administration/users/edituser.xaml?" + Guid.Empty;
            Navigate(url);
        }

        protected override void EditSelected()
        {
            if (SelectedUser == null) return;
            string url = url = "views/administration/users/edituser.xaml?" + SelectedUser.Id;
            Navigate(url);
        }

        protected override void ActivateSelected()
        {
            if(SelectedUser ==null)return;

            if(SelectedUser.EntityStatus==(int)EntityStatus.New)
            ActivateUser();
            else
            {
                DeactivateUser();
            }
        }

        protected override void DeleteSelected()
        {
            if (SelectedUser == null) return;
            DeleteUser();
        }

        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, PagedList.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            Load();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(PagedList.PageNumber, PagedList.PageCount,
                                        PagedList.TotalItemCount,
                                        PagedList.IsFirstPage, PagedList.IsLastPage);
        }

        public void SetUp()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                ShowInactive = false;
                CanAddUser = Using<IConfigService>(c).ViewModelParameters.CurrentUserRights.CanManageUsers;
            }
        }

      

        void LoadUsers()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                Users.Clear();
                var myCostCentreId = Using<IConfigService>(c).Load().CostCentreId;
                var ccs = Using<ICostCentreRepository>(c).GetAll().Where(n => n.ParentCostCentre != null);
                ccs=ccs.Where(n => n.ParentCostCentre.Id == myCostCentreId).ToList();
                List<Guid> ccIds = ccs.Select(n => n.Id).ToList();
                ccIds.Add(myCostCentreId);

                var userRepository = Using<IUserRepository>(c);
             
                var users = ccIds.ToList().SelectMany(n => userRepository.GetByCostCentre(n, ShowInactive)).OrderByDescending(p => p.Username);

              PagedList=new PagenatedList<User>(users.AsQueryable(),CurrentPage,ItemsPerPage,users.Count());
                PagedList.Select(MapUser).ToList().ForEach(n => Users.Add(n));
                UpdatePagenationControl();

            }
        }
        ListUserItem MapUser(User item, int count)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                var user = new ListUserItem
                {
                    RowNumber = count + 1,
                    Id = item.Id,
                    UserName = item.Username,
                    Password = item.Password,
                    Mobile = !string.IsNullOrEmpty(item.Mobile) ? item.Mobile : "",
                    UserType = ((UserType)item.UserType).ToString(),
                    PIN = !string.IsNullOrEmpty(item.PIN) ? item.PIN : "",
                    TillNumber = !string.IsNullOrEmpty(item.TillNumber) ? item.TillNumber : "",
                    CanManage = (item.Id != Using<IConfigService>(c).ViewModelParameters.CurrentUserId),
                    EntityStatus = (int)item._Status,
                };

                if (item._Status == EntityStatus.Active)
                    user.HlkDeactivateContent = GetLocalText("sl.users.grid.col.deactivate");
                //Deactivate
                else if (item._Status == EntityStatus.Inactive)
                    user.HlkDeactivateContent = GetLocalText("sl.users.grid.col.activate"); //Activate

                //ui.UserRoles = DelimitedRoles(ui.UserRoles.Select(x => x.ToString()).ToArray());
                var cc = Using<ICostCentreRepository>(c).GetById(item.CostCentre);
                if (cc != null)
                {
                    user.CostCenterId = cc.Id;
                    user.CostCenter = cc.Name;
                    user.Code = cc.CostCentreCode;
                }
                else
                {
                    user.Code = "";
                }
                return user;
            }
        }

        string ConcatRoles(string[] roles)
        {
            return string.Join(",", roles);
        }

        void DeactivateUser()
        {
            if (SelectedUser == null || SelectedUser.Id==Guid.Empty)
                return;
            if (!CanAddUser)
            {
                MessageBox.Show("Sorry, you do not have sufficient rights to perform this action", "Distributr: Deactivate User", MessageBoxButton.OK);
                return;
            }
            if (MessageBox.Show(GetLocalText("sl.users.deactivate.messagebox.prompt")//"Are you sure you want to deactivate this user?"
                , GetLocalText("sl.users.deactivate.messagebox.caption")//"Confirm Deactivation"
                , MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                bool deactivate = true;
                string msg = SalesmanUsage();

                if (!string.IsNullOrEmpty(msg))
                {
                    if (
                        MessageBox.Show(GetLocalText("sl.users.deactivate.info.messagebox.part1")//"User usage info." 
                        + "\n" + msg + "\n"
                        + GetLocalText("sl.users.deactivate.info.messagebox.part2")//"Do you want to continue deactivation?"
                        , GetLocalText("sl.users.deactivate.info.messagebox.caption")//"!Distributr: Deactivate Used User"
                        , MessageBoxButton.OKCancel) ==
                        MessageBoxResult.Cancel)
                    {
                        deactivate = false;
                    }
                }

                //cn .. if has inventory cannot be deactivated till inv is returned
                if (HasInventory(SelectedUser.CostCenterId))
                {
                    MessageBox.Show(SelectedUser.UserName + " "
                        + GetLocalText("sl.users.deactivate.info.hasinventory.messagebox.text")
                        //"has inventory and therefore cannot be deactivated unless the invetory is returned."
                        , GetLocalText("sl.users.deactivate.info.hasinventory.messagebox.caption")//"!Distributr: Cannot Deactivate User"
                        + " " + SelectedUser.UserName, MessageBoxButton.OK);
                    return;//toka hapa!!!!
                }

                if (deactivate)
                {
                    DoDeactivateUser(SelectedUser.Id);
                }
            }

        }

        async void DoDeactivateUser(Guid id)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                ResponseBool response = null;
                response = await proxy.UserDeactivateAsync(id);
                MessageBox.Show(response.ErrorInfo, "Distributr: Manage User", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        void DeleteUser()
        {
            if (SelectedUser == null || SelectedUser.Id == Guid.Empty)
                return;
            if (!CanAddUser)
            {
                MessageBox.Show("Sorry, you do not have sufficient rights to perform this action", "Distributr: Deactivate User", MessageBoxButton.OK);
                return;
            }
            if (MessageBox.Show(GetLocalText("sl.users.delete.messagebox.prompt")//"Are you sure you want to delete this user?"
                , GetLocalText("sl.users.deactivate.messagebox.caption")//"Confirm Deactivation"
                , MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                bool delete = true;
                string msg = SalesmanUsage();

                if (!string.IsNullOrEmpty(msg))
                {
                    if (
                        MessageBox.Show(GetLocalText("sl.users.delete.info.messagebox.part1")//"User usage info." 
                        + "\n" + msg + "\n"
                        + GetLocalText("sl.users.delete.info.messagebox.part2")//"Do you want to continue deletion?"
                        , GetLocalText("sl.users.delete.info.messagebox.caption")//"!Distributr: Delete Used User"
                        , MessageBoxButton.OKCancel) ==
                        MessageBoxResult.Cancel)
                    {
                        delete = false;
                    }
                }

                //cn .. if has inventory cannot be deactivated till inv is returned
                if (HasInventory(SelectedUser.CostCenterId))
                {
                    MessageBox.Show(SelectedUser.UserName + " "
                        + GetLocalText("sl.users.delete.info.hasinventory.messagebox.text")
                        //"has inventory and therefore cannot be deleted unless the invetory is returned."
                        , GetLocalText("sl.users.delete.info.hasinventory.messagebox.caption")//"!Distributr: Cannot Delete User"
                        + " " + SelectedUser.UserName, MessageBoxButton.OK);
                    return;//toka hapa!!!!
                }

                if (delete)
                {
                    DoDeleteUser(SelectedUser.Id);
                }
            }
        }

       async void DoDeleteUser(Guid id)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                ResponseBool response = null;
                response = await proxy.UserDeleteAsync(id);
                if(response.Success)
                {
                    var user = Using<IUserRepository>(c).GetById(id);
                    if (user != null)
                        Using<IUserRepository>(c).SetAsDeleted(user);
                }
                MessageBox.Show(response.ErrorInfo, "Distributr: Manage User", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

       void ActivateUser()
        {
            if (!CanAddUser)
            {
                MessageBox.Show("Sorry, you do not have sufficient rights to perform this action", "Distributr: Activate User", MessageBoxButton.OK);
                return;
            }
            if (MessageBox.Show(GetLocalText("sl.users.activate.messagebox.prompt")//"Are you sure you want to activate this user?"
                , GetLocalText("sl.users.activate.messagebox.caption")//"Confirm Deactivation"
                , MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DoActivateUser();
            }
        }

        private async void DoActivateUser()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                ResponseBool response = null;
                response = await proxy.UserActivateAsync(SelectedUser.Id);
                MessageBox.Show(response.ErrorInfo, "Distributr: Manage User", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        public string SalesmanUsage()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                if (SelectedUser.UserType != UserType.DistributorSalesman.ToString())
                    return "";

                string msg = "";

                if (HasUndispatchedOrders())
                {
                    msg +=
                        "  - Selected salesman has undispatched order which will have to be reassigned to a different salesman inorder to dispatch them.";
                }
                return msg;
            }
        }

        async void ResetUserPassword()
        {
            if (!CanAddUser)
            {
                MessageBox.Show("Sorry, you do not have sufficient rights to perform this action", "Distributr: Reset User Password", MessageBoxButton.OK);
                return;
            }
            if (MessageBox.Show(GetLocalText("sl.users.resetpwd.messagebox.prompt")//"Are you sure you want to reset this user's password?"
                , GetLocalText("sl.users.resetpwd.messagebox.caption")//"Confirm Reset Password"
                , MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;
            using (StructureMap.IContainer c = NestedContainer)
            {
                string newPassword = "12345678";
                User user = PagedList.FirstOrDefault(p=>p.Id==SelectedUser.Id);
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
                                          UserType = (int) user.UserType,
                                          UserRoles = user.UserRoles,
                                          DateLastUpdated = DateTime.Now,
                                          CostCenterID = user.CostCentre,
                                          StatusId = (int) EntityStatus.Active,
                                          TillNumber = user.TillNumber,
                                          CostCentreCode = Using<ICostCentreRepository>(c).GetById(user.CostCentre).CostCentreCode,
                                          Password = pwd
                                      };

                    response = await proxy.UserUpdateAsync(newUser);
                    MessageBox.Show(response.ErrorInfo, "Distributr: Manage Users", MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
            }
        }

        bool HasUndispatchedOrders()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                if (SelectedUser.UserType != UserType.DistributorSalesman.ToString())
                    return false;

                return Using<IMainOrderRepository>(c).HasOrdersPendingDispatch(UserId);//.Where(n => n.DocumentIssuerUser.Id == UserId)
                
            }
        }


        public bool HasInventory(Guid sCcId)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                if (SelectedUser.UserType!= UserType.DistributorSalesman.ToString())
                    return false;

                var salesmanInv = Using<IInventoryRepository>(c).GetByWareHouseId(sCcId);

                if (salesmanInv.Sum(n => n.Balance) > 0)
                    return true;

                return false;
            }
        }
        #endregion

        #region Helper Classes
        public class ListUserItem : ViewModelBase
        {
            public int RowNumber { get; set; }
            public Guid Id { get; set; }
            public string Code { get; set; }
            public string UserName {get;set;}
            public string Password {get;set;}
            public string UserType {get;set;}
            public string Mobile { get; set; }
            public Guid CostCenterId { get; set; }
            public string CostCenter { get; set; }
            public string PIN { get; set; }
            public string UserRoles { get; set; } //comma separated list of roles
            public string TillNumber { get; set; }
          
            public const string CanManagePropertyName = "CanManage";
            private bool _canManage = false;
            public bool CanManage
            {
                get
                {
                    return _canManage;
                }

                set
                {
                    if (_canManage == value)
                    {
                        return;
                    }

                    _canManage = value;
                    RaisePropertyChanged(CanManagePropertyName);
                }
            }

            public string HlkDeactivateContent { get; set; }
            public int EntityStatus { get; set; }
        }
        #endregion
    }
}