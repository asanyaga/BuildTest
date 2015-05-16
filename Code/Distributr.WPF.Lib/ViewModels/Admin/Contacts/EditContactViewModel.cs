using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Windows;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.MasterDataDTO.DataContracts;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility.Validation;
using Distributr.Core.Workflow.Impl.AuditLogs;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using GalaSoft.MvvmLight.Command;
using ContactOwnerType = Distributr.Core.Domain.Master.CostCentreEntities.ContactOwnerType;

namespace Distributr.WPF.Lib.ViewModels.Admin.Contacts
{
    public class EditContactViewModel : DistributrViewModelBase
    {
        private const string TelNumberRegex = @"^\d{3}-\d{3}-\d{4}$";
       
        private Guid _myCostCentreId;
        private int saveMsgCnt = 0;

        public EditContactViewModel()
        {

            LoadCommand = new RelayCommand(Load);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(CancelAll);
            ContactOwnerTypeSelectionChangedCommand = new RelayCommand(RunContactOwnerTypeSelectionChanged);

            ContactOwnerTypes = new ObservableCollection<ContactOwnerType>();
            ContactOwners = new ObservableCollection<EditContactViewModel.ContactOwnerLookUp>();
            ContactClassifications = new ObservableCollection<ContactClassification>();
            ContactTypes = new ObservableCollection<ContactType>();
            MaritalStatuses = new ObservableCollection<MaritalStatas>();
            using (StructureMap.IContainer c = NestedContainer)
            {
                CanEdit = Using<IConfigService>(c).ViewModelParameters.CurrentUserRights.CanManageContacts;
            }
        }
        

        #region Properties
        public RelayCommand LoadCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand ClearViewModelCommand { get; set; }
        public RelayCommand ClearAndSetupCommand { get; set; }
        public RelayCommand ContactOwnerTypeSelectionChangedCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        public ObservableCollection<ContactOwnerType> ContactOwnerTypes { get; set; }
        public ObservableCollection<ContactOwnerLookUp> ContactOwners { get; set; }
        public ObservableCollection<MaritalStatas> MaritalStatuses { get; set; }
        public ObservableCollection<ContactType> ContactTypes { get; set; }
        public ObservableCollection<ContactClassification> ContactClassifications { get; set; }


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
                RaisePropertyChanged(IdPropertyName);
            }
        }

        public const string SelectedContactOwnerTypePropertyName = "SelectedContactOwnerType";
        private ContactOwnerType _contact_owner_type = ContactOwnerType.Distributor;
        public ContactOwnerType SelectedContactOwnerType
        {
            get
            {
                return _contact_owner_type;
            }

            set
            {
                if (_contact_owner_type == value)
                {
                    return;
                }

                _contact_owner_type = value;
                RaisePropertyChanged(SelectedContactOwnerTypePropertyName);
            }
        }

        public const string SelectedContactOwnerPropertyName = "SelectedContactOwner";
        private ContactOwnerLookUp _selestedContactOwner = null;
        [MasterDataDropDownValidation]
        public ContactOwnerLookUp SelectedContactOwner
        {
            get
            {
                return _selestedContactOwner;
            }

            set
            {
                if (_selestedContactOwner == value)
                {
                    return;
                }

                var oldValue = _selestedContactOwner;
                _selestedContactOwner = value;
                RaisePropertyChanged(SelectedContactOwnerPropertyName);
            }
        }

        public const string FirstNamePropertyName = "FirstName";
        private string _firstName = "";
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName
        {
            get
            {
                return _firstName;
            }

            set
            {
                if (_firstName == value)
                {
                    return;
                }

                var oldValue = _firstName;
                _firstName = value;
                RaisePropertyChanged(FirstNamePropertyName);
            }
        }

        public const string LastNamePropertyName = "LastName";
        private string _lastName = "";
        [Required(ErrorMessage = "Last name is required.")]
        public string LastName
        {
            get
            {
                return _lastName;
            }

            set
            {
                if (_lastName == value)
                {
                    return;
                }

                var oldValue = _lastName;
                _lastName = value;
                RaisePropertyChanged(LastNamePropertyName);
            }
        }

        public const string DateOfBirthPropertyName = "DateOfBirth";
        private DateTime _dateOfBirth = new DateTime(1900, 01, 01);
         public DateTime DateOfBirth
        {
            get
            {
                return _dateOfBirth;
            }

            set
            {
                if (_dateOfBirth == value)
                {
                    return;
                }

                var oldValue = _dateOfBirth;
                _dateOfBirth = value;
                RaisePropertyChanged(DateOfBirthPropertyName);
            }
        }

        public const string SelectedMaritalStatusPropertyName = "SelectedMaritalStatus";
        private MaritalStatas _selectedMaritalStatus = MaritalStatas.Unknown;
        public MaritalStatas SelectedMaritalStatus
        {
            get
            {
                return _selectedMaritalStatus;
            }

            set
            {
                if (_selectedMaritalStatus == value)
                {
                    return;
                }

                _selectedMaritalStatus = value;
                RaisePropertyChanged(SelectedMaritalStatusPropertyName);
            }
        }

        public const string SpouseNamePropertyName = "SpouseName";
        private string _spouseName = "";
        public string SpouseName
        {
            get
            {
                return _spouseName;
            }

            set
            {
                if (_spouseName == value)
                {
                    return;
                }

                var oldValue = _spouseName;
                _spouseName = value;
                RaisePropertyChanged(SpouseNamePropertyName);
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

                var oldValue = _company;
                _company = value;
                RaisePropertyChanged(CompanyPropertyName);
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

                var oldValue = _jobTitle;
                _jobTitle = value;
                RaisePropertyChanged(JobTitlePropertyName);
            }
        }

        public const string MobilePhonePropertyName = "MobilePhone";
        private string _mobilePhone = "";
        [Required(ErrorMessage = "Mobile phone is required.")]
      // [RegularExpression(TelNumberRegex, ErrorMessage = "Invalid telephone number")]
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

                var oldValue = _mobilePhone;
                _mobilePhone = value;
                RaisePropertyChanged(MobilePhonePropertyName);
            }
        }

        [Required(ErrorMessage = "Business phone is required.")]
        public const string BusinessPhonePropertyName = "BusinessPhone";
        private string _businessPhone = "";
      //  [RegularExpression(mobileNumberRegex, ErrorMessage = "Invalid telephone number")]
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

                var oldValue = _businessPhone;
                _businessPhone = value;
                RaisePropertyChanged(BusinessPhonePropertyName);
            }
        }

        public const string HomePhonePropertyName = "HomePhone";
        private string _homePhone = "";
       // [RegularExpression(mobileNumberRegex, ErrorMessage = "Invalid telephone number")]
        public string HomePhone
        {
            get
            {
                return _homePhone;
            }

            set
            {
                if (_homePhone == value)
                {
                    return;
                }

                var oldValue = _homePhone;
                _homePhone = value;
                RaisePropertyChanged(HomePhonePropertyName);
            }
        }

        public const string WorkExtensionPhonePropertyName = "WorkExtensionPhone";
        private string _workExtensionPhone = "";
       // [RegularExpression(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$", ErrorMessage = "Invalid telephone number")]
        public string WorkExtensionPhone
        {
            get
            {
                return _workExtensionPhone;
            }

            set
            {
                if (_workExtensionPhone == value)
                {
                    return;
                }

                var oldValue = _workExtensionPhone;
                _workExtensionPhone = value;
                RaisePropertyChanged(WorkExtensionPhonePropertyName);
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

                var oldValue = _email;
                _email = value;
                RaisePropertyChanged(EmailPropertyName);
            }
        }

        public const string FaxPropertyName = "Fax";
        private string _fax = "";
        public string Fax
        {
            get
            {
                return _fax;
            }

            set
            {
                if (_fax == value)
                {
                    return;
                }

                var oldValue = _fax;
                _fax = value;
                RaisePropertyChanged(FaxPropertyName);
            }
        }

        public const string PhysicalAddressPropertyName = "PhysicalAddress";
        private string _physicalAddress = "";
        public string PhysicalAddress
        {
            get
            {
                return _physicalAddress;
            }

            set
            {
                if (_physicalAddress == value)
                {
                    return;
                }

                var oldValue = _physicalAddress;
                _physicalAddress = value;
                RaisePropertyChanged(PhysicalAddressPropertyName);
            }
        }

        public const string PostalAddressPropertyName = "PostalAddress";
        private string _postalAddress = "";
        public string PostalAddress
        {
            get
            {
                return _postalAddress;
            }

            set
            {
                if (_postalAddress == value)
                {
                    return;
                }

                var oldValue = _postalAddress;
                _postalAddress = value;
                RaisePropertyChanged(PostalAddressPropertyName);
            }
        }

        public const string HomeTownPropertyName = "HomeTown";
        private string _homeTown = "";
        public string HomeTown
        {
            get
            {
                return _homeTown;
            }

            set
            {
                if (_homeTown == value)
                {
                    return;
                }

                var oldValue = _homeTown;
                _homeTown = value;
                RaisePropertyChanged(HomeTownPropertyName);
            }
        }

        public const string CityPropertyName = "City";
        private string _city = "";
        public string City
        {
            get
            {
                return _city;
            }

            set
            {
                if (_city == value)
                {
                    return;
                }

                var oldValue = _city;
                _city = value;
                RaisePropertyChanged(CityPropertyName);
            }
        }

        public const string ChildrenNamesPropertyName = "ChildrenNames";
        private string _childrenNames = "";
        public string ChildrenNames
        {
            get
            {
                return _childrenNames;
            }

            set
            {
                if (_childrenNames == value)
                {
                    return;
                }

                var oldValue = _childrenNames;
                _childrenNames = value;
                RaisePropertyChanged(ChildrenNamesPropertyName);
            }
        }

        public const string SelectedContactTypePropertyName = "SelectedContactType";
        private ContactType _selectedContactType = null;
        public ContactType SelectedContactType
        {
            get
            {
                return _selectedContactType;
            }

            set
            {
                if (_selectedContactType == value)
                {
                    return;
                }

                var oldValue = _selectedContactType;
                _selectedContactType = value;
                RaisePropertyChanged(SelectedContactTypePropertyName);
            }
        }

        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "Edit Contact";
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

        public const string SelectedContactClassificationPropertyName = "SelectedContactClassification";
        private ContactClassification _selectedContactClassification = ContactClassification.SecondaryContact;
        public ContactClassification SelectedContactClassification
        {
            get
            {
                return _selectedContactClassification;
            }

            set
            {
                if (_selectedContactClassification == value)
                {
                    return;
                }
                if (value == ContactClassification.PrimaryContact)
                {
                    if (setAsPrimContact)
                    {
                        _selectedContactClassification = value;
                    }
                    else return;
                }
                else
                {
                    var oldValue = _selectedContactClassification;
                    _selectedContactClassification = value;
                    RaisePropertyChanged(SelectedContactClassificationPropertyName); 
                }
            }
        }

        public const string AuditLogEntryPropertyName = "AuditLogEntry";
        private string _AuditLogEntry = null;
        public string AuditLogEntry
        {
            get
            {
                return _AuditLogEntry;
            }

            set
            {
                if (_AuditLogEntry == value)
                {
                    return;
                }

                var oldValue = _AuditLogEntry;
                _AuditLogEntry = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(AuditLogEntryPropertyName);
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

        #endregion

        #region Methods
        public string BtnCancelContent
        {
            get { return CanEdit ? "Cancel" : "Back"; }
        }

        async void Save()
        {
            if (!IsValid())
                return;

            using (StructureMap.IContainer c = NestedContainer)
            {

                ResponseBool response = null;
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);

                saveMsgCnt = 0;
                ContactItem contactItem = null;
                var contactItems = new ObservableCollection<ContactItem>();

                if (Id == Guid.Empty)
                {
                    #region contactItem = new ContactItem

                    contactItem = new ContactItem
                        {
                            MasterId = Guid.NewGuid(),
                            DateOfBirth = DateOfBirth,
                            MaritalStatusMasterId = (int)SelectedMaritalStatus,
                            BusinessPhone = BusinessPhone,
                            ChildrenNames = ChildrenNames,
                            City = City,
                            Company = Company,
                            ContactClassification = (int) SelectedContactClassification,
                            ContactOwnerType = SelectedContactOwnerType,
                            ContactTypeMasterId = SelectedContactType.Id,
                            ContactOwnerMasterId = SelectedContactOwner.Id,
                            DateCreated = DateTime.Now,
                            Email = Email,
                            Fax = Fax,
                            Firstname = FirstName,
                            HomePhone = HomePhone,
                            HomeTown = HomeTown,
                            JobTitle = JobTitle,
                            Lastname = LastName,
                            MobilePhone = MobilePhone,
                            PhysicalAddress = PhysicalAddress,
                            PostalAddress = PostalAddress,
                            SpouseName = SpouseName,
                            WorkExtPhone = WorkExtensionPhone,
                            DateLastUpdated = DateTime.Now,
                            StatusId = (int) EntityStatus.Active,
                            IsNew = true
                        };

                    #endregion

                    contactItems.Add(contactItem);
                    AuditLogEntry = string.Format("Created New Contact: {0}; Contact Owner: ",
                                                  FirstName + " " + LastName);
                    Using<IAuditLogWFManager>(c).AuditLogEntry("Contacts Administration", AuditLogEntry);
                }
                else
                {
                    #region contactItem = new ContactItem

                    contactItem = new ContactItem
                        {
                            MasterId = Id,
                            DateOfBirth = DateOfBirth,
                            MaritalStatusMasterId =(int)SelectedMaritalStatus,
                            BusinessPhone = BusinessPhone,
                            ChildrenNames = ChildrenNames,
                            City = City,
                            Company = Company,
                            ContactClassification = (int) SelectedContactClassification,
                            ContactOwnerType =  SelectedContactOwnerType,
                            ContactTypeMasterId = SelectedContactType.Id,
                            ContactOwnerMasterId = SelectedContactOwner.Id,
                            DateCreated = DateTime.Now,
                            Email = Email,
                            Fax = Fax,
                            Firstname = FirstName,
                            HomePhone = HomePhone,
                            HomeTown = HomeTown,
                            JobTitle = JobTitle,
                            Lastname = LastName,
                            MobilePhone =MobilePhone,
                            PhysicalAddress = PhysicalAddress,
                            PostalAddress = PostalAddress,
                            SpouseName = SpouseName,
                            WorkExtPhone = WorkExtensionPhone,
                            DateLastUpdated = DateTime.Now,
                            StatusId = (int) EntityStatus.Active,
                            IsNew = false
                        };

                    #endregion

                    contactItems.Add(contactItem);

                    AuditLogEntry = string.Format("Updated Contact: {0}; Contact Owner: ", FirstName + " " + LastName);
                    Using<IAuditLogWFManager>(c).AuditLogEntry("Contacts Administration", AuditLogEntry);
                }


                if (contactItem.ContactClassification == (int) ContactClassification.PrimaryContact)
                {
                    var existingPrimConts =
                        Using<IContactRepository>(c).GetByContactsOwnerId(SelectedContactOwner.Id).Where(
                            n => n.ContactClassification == ContactClassification.PrimaryContact);
                    foreach (var item in existingPrimConts)
                    {
                        #region var existing = new ContactItem

                        var existing = new ContactItem
                            {
                                MasterId = item.Id,
                                DateOfBirth = item.DateOfBirth,
                                MaritalStatusMasterId =(int)SelectedMaritalStatus,
                                BusinessPhone = item.BusinessPhone,
                                ChildrenNames = item.ChildrenNames,
                                City = item.City,
                                Company = item.Company,
                                ContactClassification = (int) ContactClassification.SecondaryContact,
                                DateCreated = item._DateCreated,
                                Email = item.Email,
                                Fax = item.Fax,
                                Firstname = item.Firstname,
                                HomePhone = item.HomePhone,
                                HomeTown = item.HomeTown,
                                JobTitle = item.JobTitle,
                                Lastname = item.Lastname,
                                MobilePhone = item.MobilePhone,
                                PhysicalAddress = item.PhysicalAddress,
                                PostalAddress = item.PostalAddress,
                                SpouseName = item.SpouseName,
                                WorkExtPhone = item.WorkExtPhone,
                                DateLastUpdated = DateTime.Now,
                                StatusId = (int) EntityStatus.Active,
                                ContactOwnerMasterId = item.ContactOwnerMasterId,
                                ContactOwnerType = item.ContactOwnerType,
                                ContactTypeMasterId = item.ContactType != null ? item.ContactType.Id : Guid.Empty,
                                IsNew = false
                            };

                        #endregion

                        AuditLogEntry = string.Format("Updated Contact: {0}; Contact Owner: ",
                                                      FirstName + " " + LastName);
                        Using<IAuditLogWFManager>(c).AuditLogEntry("Contacts Administration", AuditLogEntry);

                        contactItems.Add(existing);
                    }
                }

                response = await proxy.ContactsAddAsync(contactItems.ToList());
                MessageBox.Show(response.ErrorInfo, "Distributr: Manage contacts", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                if (response.Success)
                {
                    ConfirmNavigatingAway = false;
                    SendNavigationRequestMessage(new Uri("/views/administration/contacts/listcontacts.xaml",
                                                         UriKind.Relative));
                }
            }
        }

        public void Load()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                CanEdit = Using<IConfigService>(c).ViewModelParameters.CurrentUserRights.CanManageContacts;
                _myCostCentreId = Using<IConfigService>(c).Load().CostCentreId;
                if (Id == Guid.Empty)
                {
                    InitializeBlank();
                   
                }
                else
                {
                    SetUp();
                    LoadById();
                }
            }
        }

        void LoadById()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                PageTitle = GetLocalText("sl.contacts.edit.title.edit"); // "Edit Contact";


                var contact = Using<IContactRepository>(c).GetById(Id);
                FirstName = contact.Firstname;
                LastName = contact.Lastname;
                DateOfBirth = contact.DateOfBirth == null ? DateTime.Now : (DateTime) contact.DateOfBirth;
                SpouseName = contact.SpouseName;
                Company = contact.Company;
                JobTitle = contact.JobTitle;
                MobilePhone = contact.MobilePhone;
                BusinessPhone = contact.BusinessPhone;
                HomePhone = contact.HomePhone;
                WorkExtensionPhone = contact.WorkExtPhone;
                Email = contact.Email;
                Fax = contact.Fax;
                PhysicalAddress = contact.PhysicalAddress;
                HomeTown = contact.HomeTown;
                City = contact.City;
                ChildrenNames = contact.ChildrenNames;
                SelectedMaritalStatus = contact.MStatus;
               
                try
                {
                    SelectedContactOwnerType = ContactOwnerTypes.First(n => n == contact.ContactOwnerType);
                }
                catch
                {
                    SelectedContactOwnerType = ContactOwnerTypes.FirstOrDefault();
                }
                LoadContactOwners();
                if(contact.ContactOwnerMasterId != Guid.Empty)
                {
                    var costcentre = Using<ICostCentreRepository>(c).GetById(contact.ContactOwnerMasterId);
                    SelectedContactOwner = costcentre!=null ? ContactOwners.FirstOrDefault(n => n.Name == costcentre.Name)
                                           : null;
                }
                

               
               

                LoadContactTypes();
                if (contact.ContactType != null)
                    SelectedContactType = ContactTypes.First(n => n.Id == contact.ContactType.Id);
                if (contact.ContactClassification == ContactClassification.PrimaryContact)
                    setAsPrimContact = true;
                SelectedContactClassification = contact.ContactClassification != 0
                                                    ? contact.ContactClassification
                                                    : ContactClassification.SecondaryContact;
            }
        }

        void InitializeBlank()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                PageTitle = GetLocalText("sl.contacts.edit.title.new"); //"Create Contact";
                ClearViewModel();

                _myCostCentreId = Using<IConfigService>(c).Load().CostCentreId;
                var contactOwner = new EditContactViewModel.ContactOwnerLookUp(Guid.Empty)
                    {
                        //Id = Guid.Empty,
                        Name = GetLocalText("sl.contacts.edit.contactOwner.default")
                        /*"--Please Select Contact Owner--"*/
                    };
                ContactOwners.Clear();
                ContactOwners.Add(contactOwner);
                SelectedContactOwner = contactOwner;


                if (
                    !Using<IContactRepository>(c).GetByContactsOwnerId(SelectedContactOwner.Id)
                                    .Any(n => n.ContactClassification == ContactClassification.PrimaryContact))
                {
                    setAsPrimContact = true;
                    SelectedContactClassification = ContactClassification.PrimaryContact;
                }
            }
        }

        void SetUp()
        {
            LoadMaritalStatuses();
            LoadContactOwnerTypes();
            SelectedContactOwnerType = ContactOwnerTypes.FirstOrDefault();
            LoadMaritalStatuses();
            SelectedMaritalStatus = MaritalStatuses.FirstOrDefault();
            LoadContactClassifications();
            SelectedContactClassification = ContactClassifications.FirstOrDefault();
            LoadContactTypes();
        }

        void LoadContactOwners()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                _myCostCentreId = Using<IConfigService>(c).Load().CostCentreId;
                ContactOwners.Clear();

                switch (SelectedContactOwnerType)
                {
#if(KEMSA)
                case ContactOwnerType.HealthFacility:
#else
                    case ContactOwnerType.Outlet:
#endif
                        Using<IOutletRepository>(c).GetByDistributor(_myCostCentreId)
                                          .ForEach(n => ContactOwners.Add(new ContactOwnerLookUp(Id = n.Id)
                                              {
                                                  Name = n.Name,
                                                  Type = typeof (Outlet)
                                              }));
                        break;
                    case ContactOwnerType.User:
                        //get all users whose cc have myCc as parent cc
                        var ccs = Using<ICostCentreRepository>(c).GetAll().Where(n => n.ParentCostCentre != null);
                        ccs.Where(n => n.ParentCostCentre.Id == _myCostCentreId).ToList();
                        List<Guid> ccIds = ccs.Select(n => n.Id).ToList();
                        ccIds.Add(_myCostCentreId);

                        var users = ccIds.ToList().SelectMany(n => Using<IUserRepository>(c).GetByCostCentre(n));
                        users.ToList().ForEach(n => ContactOwners.Add(new ContactOwnerLookUp(n.Id)
                            {
                                Name = n.Username,
                                Type = typeof (User)
                            }));
                        break;
                    case ContactOwnerType.Distributor:
                       var defaultCc = ContactOwners.FirstOrDefault(n => n.Id == Guid.Empty);
                            if (defaultCc != null)
                                ContactOwners.Remove(defaultCc);
                     

                        var cc = Using<ICostCentreRepository>(c).GetById(_myCostCentreId);
                        ContactOwners.Add(new ContactOwnerLookUp(cc.Id)
                            {
                                Name = cc.Name,
                                Type = typeof (Distributor)
                            });
                        break;

                }
            }
        }

        void RunContactOwnerTypeSelectionChanged()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                _myCostCentreId = Using<IConfigService>(c).Load().CostCentreId;
                ContactOwners.Clear();
                var contactOwner = new EditContactViewModel.ContactOwnerLookUp(Guid.Empty)
                    {
                        Name = GetLocalText("sl.contacts.edit.contactOwner.default")
                        /*"--Please Select Contact Owner--"*/
                    };
                ContactOwners.Add(contactOwner);
                SelectedContactOwner = contactOwner;

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
                        //SelectedContactOwner = ContactOwners.FirstOrDefault();
                        break;

                }
            }
        }

        void LoadContactTypes()
        {
            ContactTypes.Clear();
            var contactType = new ContactType(Guid.Empty)
                                  {
                                      Name = GetLocalText("sl.contacts.edit.contactType.default") /*"--Please Select Contact Type--"*/
                                  };
            ContactTypes.Add(contactType);
            SelectedContactType = contactType;
            using (StructureMap.IContainer c = NestedContainer)
            {
                Using<IContactTypeRepository>(c).GetAll().ToList().ForEach(n => ContactTypes.Add(n));
            }
        }

        private void LoadContactOwnerTypes()
        {
            //get the type
            Type _enumType = typeof(ContactOwnerType);

            //set up new collection
            ContactOwnerTypes.Clear();

            //retrieve the info for the type
            FieldInfo[] infos;
            infos = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

            //Add each proper enum val to collection
            foreach (FieldInfo fi in infos)
                ContactOwnerTypes.Add((ContactOwnerType)Enum.Parse(_enumType, fi.Name, true));

        }

        private void LoadContactClassifications()
        {
            //get the type
            Type _enumType = typeof(ContactClassification);
            FieldInfo[] infos;
            infos = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            ContactClassifications.Clear();
            foreach (FieldInfo fi in infos)
                ContactClassifications.Add((ContactClassification)Enum.Parse(_enumType, fi.Name, true));
        }

        private void LoadMaritalStatuses()
        {
           Type _enumType = typeof(MaritalStatas);
            FieldInfo[] infos;
            infos = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            MaritalStatuses.Clear();
            foreach (FieldInfo fi in infos)
                MaritalStatuses.Add((MaritalStatas)Enum.Parse(_enumType, fi.Name, true));

            SelectedMaritalStatus = MaritalStatas.Unknown;
        
    }

        void ClearViewModel()
        {
            FirstName = "";
            LastName = "";
            DateOfBirth = DateTime.Now;
            SpouseName = "";
            Company = "";
            JobTitle = "";
            MobilePhone = "";
            WorkExtensionPhone = "";
            Email = "";
            Fax = "";
            PhysicalAddress = "";
            PostalAddress = "";
            HomePhone = "";
            BusinessPhone = "";
            City = "";
            ChildrenNames = "";
            HomeTown = "";

            SelectedContactOwnerType = ContactOwnerType.Distributor;
            SelectedMaritalStatus = MaritalStatas.Unknown;
            SelectedContactType = null;
            SelectedContactOwner = null;
        }

        void CancelAll()
        {
            if (
                MessageBox.Show(/*"Are you sure you want to move away from this page?\n" +
                                "Unsaved changes will be lost"*/
                GetLocalText("sl.contacts.edit.cancel.messagebox.propmt"),
                                GetLocalText("sl.contacts.edit.navigateaway.messagebox.caption")/*"Distributr: Confirm Navigating Away"*/
                            , MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ClearViewModel();
                ConfirmNavigatingAway = false;
                SendNavigationRequestMessage(new Uri("/views/administration/contacts/listcontacts.xaml", UriKind.Relative));
            }
        }

        bool setAsPrimContact = false;
        public bool PrimaryContactSelected()
        {
            bool setPrimContact = false;

            if (!PrimaryContactIsSet())
            {
                setPrimContact = true;
            }
            else
            {
                if (MessageBox.Show("Primary Contact ia already set. Do you want to set this as the primary contact?", "Distributr: Setting Primary Contact", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    setPrimContact = true;
                }
                else
                {
                    SelectedContactClassification = ContactClassification.SecondaryContact;
                }
            }

            setAsPrimContact = setPrimContact;
            return setPrimContact;
        }

        public bool PrimaryContactIsSet()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                if (SelectedContactOwner == null)
                    return false;

                if (Id != Guid.Empty)
                {
                    var all = Using<IContactRepository>(c).GetByContactsOwnerId(SelectedContactOwner.Id).Where(n => n.Id != Id).ToList();
                    if (all.Any(n => n.ContactClassification == ContactClassification.PrimaryContact))
                    {
                        return true;
                    }
                }
                else
                {
                    var myConts = Using<IContactRepository>(c).GetByContactsOwnerId(SelectedContactOwner.Id);
                    if (myConts.Any(n => n.ContactClassification == ContactClassification.PrimaryContact))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        //if this user has a primary contact ... set this contact as default esle set as primary
        public void SetDefaultContactClassification()
        {
            if (Id == Guid.Empty)
            {
                if (!PrimaryContactIsSet())
                {
                    SelectedContactClassification = ContactClassification.PrimaryContact;
                }
                else
                {
                    SelectedContactClassification = ContactClassification.SecondaryContact;
                }
            }
        }

        public async void DeactivateConctact()
        {
            saveMsgCnt = 0;
            using (StructureMap.IContainer c = NestedContainer)
            {
                ResponseBool response = null;
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                response = await proxy.ContactDeactivateAsync(Id);

                MessageBox.Show(response.ErrorInfo, "Distributr: Manage contacts", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                if (response.Success)
                {
                    ConfirmNavigatingAway = false;
                    SendNavigationRequestMessage(new Uri("/views/administration/contacts/listcontacts.xaml",
                                                         UriKind.Relative));
                }
            }
        }

        #endregion

        #region Classes
        public class ContactOwnerLookUp : MasterEntity
        {

            // public Guid Id { get; set; }
            public ContactOwnerLookUp(Guid id)
                : base(id)
            {
            }

            public string Name { get; set; }
            public object Type { get; set; }
        }
        
        
        #endregion
    }
}