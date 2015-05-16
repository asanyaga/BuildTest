using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WPF.Lib.Messages;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.UI.Pages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PaymentGateway.WSApi.Lib.Paging.Impl;

namespace Distributr.WPF.Lib.ViewModels.Admin.Contacts
{
    public class NewListContactViewModel : ListingsViewModelBase
    {
        internal IPagenatedList<Contact> PagedList;
         

         

        public NewListContactViewModel()
        {
      
            ContactOwnerTypeDropDownOpenedCommand = new RelayCommand(ContactOwnerTypeDropDownOpened);
           // ClearContacts = new RelayCommand(ClearViewModel);
            ClearViewModel();
           
            ContactDropDownOpenedCommand = new RelayCommand(ContactDropDownOpened);
          //  GetContactsCommand = new RelayCommand(GetContacts);
            Contacts = new ObservableCollection<NewListContactViewModel>();
            load();
            

        }

        #region COMMANDS AND COLLECTIONS
      //  public ObservableCollection<Distributr.Core.Domain.Master.CostCentreEntities.ContactOwnerType> ContactOwnerTypes { get; set; }

        public RelayCommand ContactOwnerTypeSelectionChangedCommand { get; set; }

        public RelayCommand ContactOwnerTypeDropDownOpenedCommand { get; set; }
        public RelayCommand ClearContacts { get; set; }
        public RelayCommand ContactDropDownOpenedCommand { get; set; }
        // public RelayCommand ListContactsLoadedCommand { get; set; }
        public RelayCommand GetContactsCommand { get; set; }


        public ObservableCollection<NewListContactViewModel> Contacts { get; set; }

        #endregion




        #region PROPERTIES


        public const string UserIdPropertyName = "UserId";
        private User _userId = null;
        public User  UserId
        {
            get
            {
                return _userId;
            }

            set
            {
                if (_userId == value)
                {
                    return;
                }

                RaisePropertyChanging(UserIdPropertyName);
                _userId = value;
                RaisePropertyChanged(UserIdPropertyName);
            }
        }
        public const string SelectedContactOwnerTypePropertyName = "SelectedContactOwnerType";
        private ContactOwnerType _selectedContactOwnerType = ContactOwnerType.None;
        public ContactOwnerType SelectedContactOwnerType
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

        

             public const string SelectedContactPropertyName = "SelectedContact";
             private NewListContactViewModel _selectedContact = null;
          /*  new CostCentre(Guid.Empty) { Name = "--Select Contact Owner--" };*/
             public NewListContactViewModel SelectedContact
        {
            get
            {
                return _selectedContact;
            }

            set
            {
                if (_selectedContact == value)
                {
                    return;
                }

                var oldValue = _selectedContact;
                _selectedContact = value;
                RaisePropertyChanged(SelectedContactPropertyName);
            }
        }


        public const string SelectedContactOwnerPropertyName = "SelectedContactOwner";
        private CostCentre _selectedContactOwner = null;
          /*  new CostCentre(Guid.Empty) { Name = "--Select Contact Owner--" };*/
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

        public const string NamePropertyName = "Name";
        private string _name = "";
        public string Name
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
        private DateTime _dob = DateTime.Now;
        public DateTime DoB
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


        public const string JobTitlePropertyName = "JobTitle";
        private string _jobTitle = "";
        public string JobTitle
        {
            get
            {
                return _jobTitle;
            }

            set
            {
                if (_jobTitle == value)
                {
                    return;
                }

                RaisePropertyChanging(JobTitlePropertyName);
                _jobTitle = value;
                RaisePropertyChanged(JobTitlePropertyName);
            }
        }

        public const string CompanyPropertyName = "Company";
        private string _company = "";
        public string Company
        {
            get
            {
                return _company;
            }

            set
            {
                if (_company == value)
                {
                    return;
                }

                RaisePropertyChanging(CompanyPropertyName);
                _company = value;
                RaisePropertyChanged(CompanyPropertyName);
            }
        }

        public const string AddressPropertyName = "Address";
        private string _address = "";
        public string Address
        {
            get
            {
                return _address;
            }

            set
            {
                if (_address == value)
                {
                    return;
                }

                RaisePropertyChanging(AddressPropertyName);
                _address = value;
                RaisePropertyChanged(AddressPropertyName);
            }
        }

        public const string BusinessPhonePropertyName = "BusinessPhone";
        private string _businessPhone = "";
        public string BusinessPhone
        {
            get
            {
                return _businessPhone;
            }

            set
            {
                if (_businessPhone == value)
                {
                    return;
                }

                RaisePropertyChanging(BusinessPhonePropertyName);
                _businessPhone = value;
                RaisePropertyChanged(BusinessPhonePropertyName);
            }
        }


        public const string MobilePhonePropertyName = "MobilePhone";
        private string _mobilePhone = "";
        public string MobilePhone
        {
            get
            {
                return _mobilePhone;
            }

            set
            {
                if (_mobilePhone == value)
                {
                    return;
                }

                RaisePropertyChanging(MobilePhonePropertyName);
                _mobilePhone = value;
                RaisePropertyChanged(MobilePhonePropertyName);
            }
        }


        public const string EmailPropertyName = "Email";
        private string _email = "";
        public string Email
        {
            get
            {
                return _email;
            }

            set
            {
                if (_email == value)
                {
                    return;
                }

                RaisePropertyChanging(EmailPropertyName);
                _email = value;
                RaisePropertyChanged(EmailPropertyName);
            }
        }

        public const string ContactClassificationPropertyName = "ContactClassification";
        private ContactClassification _contactClassification = ContactClassification.None;
        public ContactClassification ContactClassification
        {
            get
            {
                return _contactClassification;
            }

            set
            {
                if (_contactClassification == value)
                {
                    return;
                }

                RaisePropertyChanging(ContactClassificationPropertyName);
                _contactClassification = value;
                RaisePropertyChanged(ContactClassificationPropertyName);
            }
        }


        public const string HlkDeactivateContentPropertyName = "HlkDeactivateContent";
        private string _hlkDeactivateContent = "";
        public string HlkDeactivateContent
        {
            get
            {
                return _hlkDeactivateContent;
            }

            set
            {
                if (_hlkDeactivateContent == value)
                {
                    return;
                }

                RaisePropertyChanging(HlkDeactivateContentPropertyName);
                _hlkDeactivateContent = value;
                RaisePropertyChanged(HlkDeactivateContentPropertyName);
            }
        }



        public const string HlkEditContentPropertyName = "HlkEditContent";
        private string _hlkEditContent = "";
        public string HlkEditContent
        {
            get
            {
                return _hlkEditContent;
            }

            set
            {
                if (_hlkEditContent == value)
                {
                    return;
                }

                RaisePropertyChanging(HlkEditContentPropertyName);
                _hlkEditContent = value;
                RaisePropertyChanged(HlkEditContentPropertyName);
            }
        }


        public const string IdPropertyName = "Id";
        private Guid _id;
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

                RaisePropertyChanging(IdPropertyName);
                _id = value;
                RaisePropertyChanged(IdPropertyName);
            }
        }


        public const string ContactIdPropertyName = "ContactId";
        private Guid _contactId;
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

                RaisePropertyChanging(ContactIdPropertyName);
                _contactId = value;
                RaisePropertyChanged(ContactIdPropertyName);
            }
        }




        #endregion




        #region METHODS


          


        public void load()
        {

            using (StructureMap.IContainer cont = NestedContainer)
            {
                IConfigService configService = Using<IConfigService>(cont);

                UserId = Using<IUserRepository>(cont).GetById(configService.ViewModelParameters.CurrentUserId);


            }
        }

        public void GetContacts()
        {

            


            if (SelectedContactOwner != null)
                if (SelectedContactOwner.Id == Guid.Empty)
                    return;
          
            LoadContactsFromLocalDb();

        }

        private void ContactOwnerTypeDropDownOpened()
        {

            using (var container = NestedContainer)
            {
                SelectedContactOwnerType = Using<IItemsEnumLookUp>(container).SelectContactOwnerType(); //??default;
            }
            // GetContacts();


        }

      

        private void ContactDropDownOpened()
        {

       using (var container = NestedContainer)
            {


                SelectedContactOwner = Using<IItemsLookUp>(container).SelectContactOwner((int)SelectedContactOwnerType , UserId.CostCentre  );//??default;
//                if (SelectedContactOwner != null) Id = SelectedContactOwner.Id;
            }
       if (SelectedContactOwner != null)
        GetContacts();


        }

        public void ClearViewModel()
        {
            if (Contacts != null) Contacts.Clear();


            //  if (ContactOwners != null) ContactOwners.Clear();

        //    if (ContactOwnerTypes != null) ContactOwnerTypes.Clear();
            ContactOwnerId = Guid.Empty;
            SelectedContactOwnerType = ContactOwnerType.None;

            //ContactTypeName = "";


        }

        void LoadContactsFromLocalDb()
        {
           

            using (StructureMap.IContainer c = NestedContainer)
            {
                var contactRepository = Using<IContactRepository>(c);

                var contacts = contactRepository.Query(new QueryStandard()).Data.ToList();
                if (SelectedContactOwner != null)
                {
                    contacts =
                   Using<IContactRepository>(c).GetByContactsOwnerId(SelectedContactOwner.Id, ShowInactive).ToList();
                

                    foreach (var item in contacts)
                    {

                        Contacts.Add(Map(item));

                   } }
                PagedList = new PagenatedList<Contact>(contacts, CurrentPage, ItemsPerPage, contacts.Count());
                Contacts.Clear();
                PagedList.Select(Map).ToList().ForEach(n => Contacts.Add(n));
                UpdatePagenationControl();
            }

        }



        private NewListContactViewModel Map(Contact item)
        {
            var contact = new NewListContactViewModel();
            contact.Name = item.Firstname;
            contact.RowNumber = 0;
            contact.MaritalStats = item.MStatus;
            if (item.DateOfBirth != null) contact.DoB = (DateTime)item.DateOfBirth;
            contact.JobTitle = item.JobTitle;
            contact.Company = item.Company;
            contact.Address = item.PostalAddress;
            contact.BusinessPhone = item.BusinessPhone;
            contact.MobilePhone = item.MobilePhone;
            contact.Email = item.Email;
            contact.ContactClassification = item.ContactClassification;
            contact.Id = item.Id;
            return contact;
        }

        public async void DeactivateConctact()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                ResponseBool response = null;
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                response = await proxy.ContactDeactivateAsync(ContactId);
                if (SelectedContactOwner != null)
                    ContactOwnerId = SelectedContactOwner.Id;

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
        public async void DeleteContact()
        {


            if (SelectedContact != null)
            {

               
               if(     MessageBox.Show("Do you want to Delete the selected Contact", "Distributr Contacts",
                                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                
                {
                    using (StructureMap.IContainer c = NestedContainer)
                    {
                        ResponseBool response = null;
                        IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                        response = await proxy.ContactDeleteAsync(SelectedContact.Id);

                        ContactOwnerId = SelectedContact.Id;
                        if (response.Success)
                        {
                            var contact = Using<IContactRepository>(c).GetById(ContactOwnerId);
                            if (contact != null) Using<IContactRepository>(c).SetAsDeleted(contact);
                        }
                        MessageBox.Show(response.ErrorInfo, "Distributr: Manage Contacts", MessageBoxButton.OK,
                                        MessageBoxImage.Information);
                    }
                    GetContacts();

                }
            }
           


        }

        protected override void DeleteSelected()
        {
            if (SelectedContact != null)
                DeleteContact();

        }



        protected override sealed void Load(bool isFirstLoad = false)
        {
            LoadContactsFromLocalDb();
        }

        protected override void EditSelected()
        {
            if (SelectedContact != null)
            {
                /*  using (var c = NestedContainer)
                  {
                      Messenger.Default.Send(new EditContactMessage
                                                 {
                                                     ContactId = SelectedContactOwner.Id
                                                 });*/

                var url = "views/administration/contacts/editcontact.xaml?" + SelectedContact.Id;
                Navigate(url);
                
                // }
            }
            else
            {
                MessageBox.Show("No outlet selected");
            }
            GetContacts();
        }

        protected override void ActivateSelected()
        {
            ActivateContact();
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

        #endregion






    }
}

