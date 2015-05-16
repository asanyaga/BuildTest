using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility;
using Distributr.WPF.Lib.Service.Utility;
using System.Linq;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.UI.Pages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ContactOwnerType = Distributr.Core.Domain.Master.CostCentreEntities.ContactOwnerType;

namespace Distributr.WPF.Lib.ViewModels.Admin.Contacts
{
    public class ListContactViewModel : DistributrViewModelBase
    {/*
        private Guid _myCostCentreId;

        public ListContactViewModel()
        {
            ContactOwnerTypeSelectionChangedCommand = new RelayCommand(RunContactOwnerTypeSelectionChanged);
            ContactOwnerSelectionChangedCommand = new RelayCommand(RunContactOwnerSelectionChanged);
            GetContactsCommand = new RelayCommand(GetContacts);
            ListContactsLoadedCommand = new RelayCommand(RunClearAndSetUp);
            ContactChangedCommand = new RelayCommand(ContactChanged);
            ContactOwnerTypeDropDownOpenedCommand = new RelayCommand(ContactOwnerTypeDropDownOpened);
            //ContactDropDownOpenedCommand = new RelayCommand(ContactDropDownOpened);
          /*  ContactOwnerTypeTestCommand = new RelayCommand(ContactOwnerTypeTest);#1#
            using (StructureMap.IContainer c = NestedContainer)
            {
                CanEdit = Using<IConfigService>(c).ViewModelParameters.CurrentUserRights.CanManageContacts;
            }
            Contacts = new ObservableCollection<ContactLineItemViewmodel>();
            ContactOwners = new ObservableCollection<EditContactViewModel.ContactOwnerLookUp>();
            ContactOwnerTypes = new ObservableCollection<ContactOwnerType>();
      
            ClearContacts = new RelayCommand(ClearViewModel);
          
        }

        
#region Methods


    
        private void ContactOwnerTypeDropDownOpened()
        {
            using (var container = NestedContainer)
            {
               

                SelectedContactOwnerType = Using<IItemsEnumLookUp>(container).SelectContactOwnerType(); //??default;
            }

            GetContacts();
        }
        private void ContactChanged()
        {
            SelectedContact = DefaultContact;
        }
        protected Contact DefaultContact
        {
            get
            {
                return
                    new Contact(Guid.Empty) { Firstname = "Select Contact--" };
            }
        }
      /*  private void ContactDropDownOpened()
        {
            using (var container = NestedContainer)
            {
                SelectedContactOwner = Using<IItemsLookUp>(container).SelectContact((int)SelectedContactOwnerType );//??default;
              
            }
            GetContacts();
          //  RunClearAndSetUp();
           

        }#1#

      public void GetContacts()
        {
            if (SelectedContactOwner != null )
            if (SelectedContactOwner.Id == Guid.Empty)
                return;

            Loading = true;
            LoadStatus = "Loading Contacts... \nPlease wait.";
           LoadContactsFromLocalDb();
           
        }

      void LoadContactsFromLocalDb()
      {
          using (StructureMap.IContainer c = NestedContainer)
          {
              var contactRepository= Using<IContactRepository>(c);

              var contacts = contactRepository.Query(new QueryStandard()).Data.ToList();
            if(SelectedContactOwner!=null)
          {
                   contacts =
                  Using<IContactRepository>(c).GetByContactsOwnerId(SelectedContactOwner.Id, ShowInactive).Where(
                      p => p.ContactOwnerType == SelectedContactOwnerType).ToList();

       }
             
              foreach (var item in contacts)
              {
                  
                  Contacts.Add(Map(item));
                 
                 

              }
          }

      }

        private ContactLineItemViewmodel Map(Contact item)
        {
            var contact = new ContactLineItemViewmodel();
            contact.Name = item.Firstname;
            contact.RowNumber = 0;
            contact.MaritalStats = item.MStatus;
            if (item.DateOfBirth != null) contact.DoB = (DateTime) item.DateOfBirth;
            return contact;
        }

        public void ClearViewModel()
        {
           if (Contacts != null) Contacts.Clear();
            
            if (ContactOwners != null) ContactOwners.Clear();
            
            if (ContactOwnerTypes != null) ContactOwnerTypes.Clear();
            ContactOwnerId = Guid.Empty;
            SelectedContactOwnerType = ContactOwnerType.None;
            ContactTypeName = "";
          
          //  SelectedContacttype = new ContactType( Guid.Empty){Name = "--- Select Contact Type---"};
        }

       /* public void SetUp()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
               
                RunContactOwnerTypeSelectionChanged();

                if (SelectedContactOwner == null)
                {
                    var contactOwner = new EditContactViewModel.ContactOwnerLookUp(Guid.Empty)
                                           {

                                              // Name = GetLocalText("sl.contacts.edit.contactOwner.default")
                                               Name = SelectedContactOwner.Name 
                                               /*"--Please Select Contact Owner--"#2#
                                           };

                    ContactOwners.Add(contactOwner);
               //    SelectedContactOwner.Firstname  = contactOwner;
                }
            }

        }#1#


        void RunContactOwnerTypeSelectionChanged()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                if (_myCostCentreId == Guid.Empty)
                    _myCostCentreId = Using<IConfigService>(c).Load().CostCentreId;
                ContactOwners.Clear();
                var contactOwner = new EditContactViewModel.ContactOwnerLookUp(Guid.Empty)
                    {
                        Name = GetLocalText("sl.contacts.edit.contactOwner.default")
                        /*"--Please Select Contact Owner--"#1#
                    };
                ContactOwners.Add(contactOwner);
               // SelectedContactOwner = contactOwner;

                switch (SelectedContactOwnerType)
                {
#if(KEMSA)
                case ContactOwnerType.HealthFacility:
#else
                    case ContactOwnerType.Outlet:
#endif
                        Using<IOutletRepository>(c).GetByDistributor(_myCostCentreId)
                                          .ForEach(
                                              n => ContactOwners.Add(new EditContactViewModel.ContactOwnerLookUp(n.Id)
                                                  {

                                                      Name = n.Name,
                                                      Type = typeof (Outlet)
                                                  }));
                        break;
                    case ContactOwnerType.User:
                        //get all salesmen ccs whose cc has myCC as parent 
                        var mySalesmenCCIds =
                            Using<ICostCentreRepository>(c).GetAll().OfType<DistributorSalesman>().Where(
                                n => ((DistributorSalesman) n).ParentCostCentre.Id == _myCostCentreId)
                                              .Select(n => n.Id)
                                              .ToList();

                        List<Guid> ccIds = mySalesmenCCIds;
                        ccIds.Add(_myCostCentreId);

                        List<User> users = new List<User>();

                        foreach (var ccId in ccIds)
                        {
                            var usrs = Using<IUserRepository>(c).GetByCostCentre(ccId);
                            usrs.ForEach(users.Add);
                        }

                        users.ToList().ForEach(n => ContactOwners.Add(new EditContactViewModel.ContactOwnerLookUp(n.CostCentre)
                            {
                                Name = n.Username,
                                Type = typeof (User)
                            }));
                        break;
                    case ContactOwnerType.Distributor:
                        //ContactOwners.Remove(contactOwner);
                        var cc = Using<ICostCentreRepository>(c).GetById(_myCostCentreId);
                        ContactOwners.Add(new EditContactViewModel.ContactOwnerLookUp(cc.Id)
                            {
                                Name = cc.Name,
                                Type = typeof (Distributor)
                            });
                        //SelectedContactOwner = ContactOwners.First();
                        break;

                }
            }
        }

        void RunContactOwnerSelectionChanged()
        {
            if (SelectedContactOwner != null)
            {
               ContactOwnerId = SelectedContactOwner.Id;
            }
            
            GetContacts();
        }

        void RunClearAndSetUp()
        {
            ClearViewModel();
            
            //SetUp();
        }

        public void LoadContactOwnerTypes()
        {
            ContactOwnerTypes.Clear();
            SelectedContactOwnerType = ContactOwnerType.Distributor;
            Type _enumType = typeof(ContactOwnerType);

            
            FieldInfo[] infos = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

            //Add each proper enum val to collection
            foreach (FieldInfo fi in infos)
                ContactOwnerTypes.Add((ContactOwnerType)Enum.Parse(_enumType, fi.Name, true));
          
            
           
        }

        public async void DeactivateConctact()
        {using (StructureMap.IContainer c = NestedContainer)
            {
                ResponseBool response = null;
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                response = await proxy.ContactDeactivateAsync (ContactId);
                if (SelectedContactOwner != null)
                    ContactOwnerId = SelectedContactOwner.Id;

                MessageBox.Show(response.ErrorInfo, "Distributr: Manage Contacts", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        public async void DeleteContact()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                ResponseBool response = null;
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                response = await proxy.ContactDeleteAsync(ContactId);
                if (SelectedContactOwner != null)
                    ContactOwnerId = SelectedContactOwner.Id;
                if (response.Success)
                {
                    var contact = Using<IContactRepository>(c).GetById(ContactId);
                    if (contact != null) Using<IContactRepository>(c).SetAsDeleted(contact);
                }
                MessageBox.Show(response.ErrorInfo, "Distributr: Manage Contacts", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        public async void ActivateContact()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                ResponseBool response = null;
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                response = await proxy.ContactActivateAsync(ContactId);
                if (SelectedContactOwner != null)
                    ContactOwnerId = SelectedContactOwner.Id;

                MessageBox.Show(response.ErrorInfo, "Distributr: Manage Contacts", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        #endregion
        #region Properties

        public ObservableCollection<Distributr.Core.Domain.Master.CostCentreEntities.ContactOwnerType> ContactOwnerTypes { get; set; }
        public ObservableCollection<EditContactViewModel.ContactOwnerLookUp> ContactOwners { get; set; }
       // public ObservableCollection<ContactOwnerType> ContactOwnerTypes_ { get; set; }

        
        public RelayCommand ContactOwnerTypeDropDownOpenedCommand { get; set; }
        public RelayCommand ClearContacts {get; set; }
        public RelayCommand ContactDropDownOpenedCommand { get; set; }
        public RelayCommand ContactTypeDropDownOpenedCommand { get; set; }
        public RelayCommand ListContactsLoadedCommand { get; set; }
        public RelayCommand ContactChangedCommand { get; set; }
        public RelayCommand ContactOwnerTypeSelectionChangedCommand { get; set; }
        public RelayCommand ContactOwnerSelectionChangedCommand { get; set; }
        public RelayCommand GetContactsCommand { get; set; }
       // public RelayCommand LoadCommand { get; set; }
        public ObservableCollection<ContactLineItemViewmodel> Contacts { get; set; }



        /// <summary>
        /// The <see cref="ContactTypeName" /> property's name.
        /// </summary>
        public const string ContactTypeNamePropertyName = "ContactTypeName";

        private string _contactTypeName= "";
        public string ContactTypeName
        {
            get
            {
                return _contactTypeName;
            }

            set
            {
                if (_contactTypeName == value)
                {
                    return;
                }

                RaisePropertyChanging(ContactTypeNamePropertyName);
                _contactTypeName = value;
                RaisePropertyChanged(ContactTypeNamePropertyName);
            }
        }
     
        public const string SelectedContactPropertyName = "SelectedContact ";
        private Contact _selectedContact  = new Contact( Guid.Empty ){Firstname  = "---Select Contact Name---"};
        public Contact SelectedContact 
        {
            get
            {
                return _selectedContact ;
            }

            set
            {
                if (_selectedContact  == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedContactPropertyName);
                _selectedContact  = value;
                RaisePropertyChanged(SelectedContactPropertyName);
            }
        }
        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "Contact List";
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

        public const string ContactOwnerIdPropertyName = "ContactOwnerId";
        private Guid _contactOwnerId = Guid.Empty;
        public Guid ContactOwnerId
        {
            get
            {
                return _contactOwnerId;
            }

            set
            {
                if (_contactOwnerId == value)
                {
                    return;
                }

                var oldValue = _contactOwnerId;
                _contactOwnerId = value;
                RaisePropertyChanged(ContactOwnerIdPropertyName);
            }
        }

        public const string SelectedContactOwnerPropertyName = "SelectedContactOwner";
        private CostCentre  _selectedContactOwner = null /*new CostCentre(Guid.Empty) { Firstname = "--Select Contact Owner--" }#1#;
        public CostCentre SelectedContactOwner
        {
            get
            {
                return _selectedContactOwner;
            }

            set
            {
                if (_selectedContactOwner == value)
                {
                    return;
                }

                var oldValue = _selectedContactOwner;
                _selectedContactOwner = value;
                RaisePropertyChanged(SelectedContactOwnerPropertyName);
            }
        }

        public const string SelectedContactOwnerTypePropertyName = "SelectedContactOwnerType";
        private ContactOwnerType _selectedContactOwnerType = ContactOwnerType.Distributor;
        public  ContactOwnerType SelectedContactOwnerType
        {
            get
            {
                return _selectedContactOwnerType;
            }

            set
            {
                if (_selectedContactOwnerType == value)
                {
                    return;
                }

                var oldValue = _selectedContactOwnerType;
                _selectedContactOwnerType = value;
                RaisePropertyChanged(SelectedContactOwnerTypePropertyName);
            }
        }



        public const string CanEditPropertyName = "CanEdit";
        private bool _canEdit = false;
        public bool CanEdit
        {
            get
            {
                return _canEdit;
            }

            set
            {
                if (_canEdit == value)
                {
                    return;
                }

                var oldValue = _canEdit;
                _canEdit = value;
                RaisePropertyChanged(CanEditPropertyName);
            }
        }

        public const string LoadStatusPropertyName = "LoadStatus";
        private string _loadStatus = "Loading";
        public string LoadStatus
        {
            get
            {
                return _loadStatus;
            }

            set
            {
                if (_loadStatus == value)
                {
                    return;
                }

                var oldValue = _loadStatus;
                _loadStatus = value;

                RaisePropertyChanged(LoadStatusPropertyName);
            }
        }

        public const string LoadingPropertyName = "Loading";
        private bool _loading = false;
        public bool Loading
        {
            get
            {
                return _loading;
            }

            set
            {
                if (_loading == value)
                {
                    return;
                }

                var oldValue = _loading;
                _loading = value;

                RaisePropertyChanged(LoadingPropertyName);
                RaisePropertyChanged(LoadingPropertyName, oldValue, value, true);
            }
        }
         
        public const string ContactIdPropertyName = "ContactId";
        private Guid _contactId = Guid.Empty;
        public Guid ContactId
        {
            get
            {
                return _contactId;
            }

            set
            {
                if (_contactId == value)
                {
                    return;
                }

                var oldValue = _contactId;
                _contactId = value;
                RaisePropertyChanged(ContactIdPropertyName);
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

        

        #region Helper Classes
       
        public class ContactLineItemViewmodel:ViewModelBase
        {
            
            public const string RowNumberPropertyName = "RowNumber";
            private int _rowNumber = 0;
            public int RowNumber
            {
                get
                {
                    return _rowNumber;
                }

                set
                {
                    if (_rowNumber == value)
                    {
                        return;
                    }

                    RaisePropertyChanging(RowNumberPropertyName);
                    _rowNumber = value;
                    RaisePropertyChanged(RowNumberPropertyName);
                }
            }

            
           

           
            public const string NamePropertyName = "Name";
            private string  _name = "";
            public string  Name
            {
                get
                {
                    return _name;
                }

                set
                {
                    if (_name == value)
                    {
                        return;
                    }

                    RaisePropertyChanging(NamePropertyName);
                    _name = value;
                    RaisePropertyChanged(NamePropertyName);
                }
            }

            public const string MaritalStatsPropertyName = "MaritalStats";
            private MaritalStatas _maritalStatas = MaritalStatas.Unknown;
            public MaritalStatas MaritalStats
            {
                get
                {
                    return _maritalStatas;
                }

                set
                {
                    if (_maritalStatas == value)
                    {
                        return;
                    }

                    RaisePropertyChanging(MaritalStatsPropertyName);
                    _maritalStatas = value;
                    RaisePropertyChanged(MaritalStatsPropertyName);
                }
            }

          
            public const string DoBPropertyName = "DoB";

            private DateTime  _dob = DateTime.Now;
            public DateTime  DoB
            {
                get
                {
                    return _dob;
                }

                set
                {
                    if (_dob == value)
                    {
                        return;
                    }

                    RaisePropertyChanging(DoBPropertyName);
                    _dob = value;
                    RaisePropertyChanged(DoBPropertyName);
                }
            }
        

            //public int RowNumber { get; set; }
            public Guid Id { get; set; }
           // public string Name { get; set; }
            public MaritalStatas MaritalStatus { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string JobTitle { get; set; }
            public string Company { get; set; }
            public string Address { get; set; }
            public string MobilePhone { get; set; }
            public string BusinessPhone { get; set; }
            public string Email { get; set; }
            public string ContactClassification { get; set; }
            public string HlkEditContent { get; set; }
            public string HlkDeactivateContent { get; set; }
            public int EntityStatus { get; set; }
        }
        #endregion*/

        public class ContactLineItem
        {
            public int RowNumber { get; set; }
            public Guid Id { get; set; }
            public string Name { get; set; }
            public MaritalStatas MaritalStatus { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string JobTitle { get; set; }
            public string Company { get; set; }
            public string Address { get; set; }
            public string MobilePhone { get; set; }
            public string BusinessPhone { get; set; }
            public string Email { get; set; }
            public string ContactClassification { get; set; }
            public string HlkEditContent { get; set; }
            public string HlkDeactivateContent { get; set; }
            public int EntityStatus { get; set; }
        }
    }
   
}