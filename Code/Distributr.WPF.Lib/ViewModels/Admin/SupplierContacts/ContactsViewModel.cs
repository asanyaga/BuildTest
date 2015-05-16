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
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DataContracts;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.WPF.Lib.Messages;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.SupplierContacts
{
    public class ContactsViewModel:DistributrViewModelBase
    {
        
        public ContactsViewModel()
        {
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            LoadListingPageCommand = new RelayCommand(Load);
            ContactTypeList = new ObservableCollection<ContactType>();
            ContactClassificationList = new ObservableCollection<ContactClassification>();
        }

        

        #region Class Members

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand LoadListingPageCommand { get; set; }
        public ObservableCollection<ContactType> ContactTypeList { get; set; }
        public ObservableCollection<ContactClassification> ContactClassificationList { get; set; }

        #endregion


        #region Methods


        public void AddContact(AddContactMessage messageTo)
        {
            SupplierId = messageTo.SupplierId;
        }

        public void EditContact(EditContactMessage messageTo)
        {
            SupplierId = messageTo.SupplierId;
            Id = messageTo.ContactId;
            IsEdit = true;
        }

        private void Load()
        {
            LoadContactOwnerType();
            LoadContactClassification();

            ClearViewModel();
            if(IsEdit)
            {
                PageTitle = "Edit Contact";
                LoadForEdit();
                IsEdit = false;
            }
            else
            {
                PageTitle = "Add Contact";
               DefaultData();
            }
            
        }

        private void LoadForEdit()
        {
            using(var c=NestedContainer)
            {
                var contact = Using<IContactRepository>(c).GetById(Id);

                FirstName = contact.Firstname;
                LastName = contact.Lastname;
                PhysicalAddress =contact.PhysicalAddress;
                PostalAddress = contact.PostalAddress;
                Email = contact.Email;
                MobileNumber = contact.MobilePhone;
                BusinessNumber = contact.BusinessPhone;
                FaxNumber = contact.Fax;
                OfficeNumber =contact.HomePhone;
                SelectedContactClassification = ContactClassification.None;
                SelectedContactClassification =ContactClassificationList.FirstOrDefault(l => l == contact.ContactClassification);
                SelectedContactType = ContactTypeList.FirstOrDefault(l => l == contact.ContactType) ?? ContactTypeList.FirstOrDefault(n=>n.Name=="--Select Contact Type--");

            }
        }

        private void LoadContactClassification()
        {
            ContactClassificationList.Clear();

            Type _enumType = typeof(ContactClassification);
            FieldInfo[] infos;
            infos = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo fi in infos)
                ContactClassificationList.Add((ContactClassification)Enum.Parse(_enumType, fi.Name, true));
            ContactClassificationList.Remove(ContactClassification.None);
        }

        private void LoadContactOwnerType()
        {
            ContactTypeList.Clear();
            ContactTypeList.Add(new ContactType(Guid.Empty) { Name = "--Select Contact Type--" });
            using (var c = NestedContainer)
            {
                var contactOwnerList = Using<IContactTypeRepository>(c).GetAll().ToList();
                foreach (var n in contactOwnerList)
                {
                    ContactTypeList.Add(n);
                }
            }
        }

        private void DefaultData()
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Random rand = new Random();
                FirstName = "Michael";
                LastName = "Muthee";
                PhysicalAddress = "Marekani";
                PostalAddress = "P.O Box 5654";
                Email = "mm@gmail.com";
                MobileNumber = rand.Next(1000000, 9999999).ToString();
                BusinessNumber = rand.Next(1000000, 9999999).ToString();
                FaxNumber = rand.Next(1000000, 9999999).ToString();
                OfficeNumber = rand.Next(1000000, 9999999).ToString();
                
            }
        }

        private void Cancel()
        {
            if (MessageBox.Show("Unsaved Changes will be lost. Do you want to continue?", "Agrimangr: " + PageTitle,
                                MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                ClearViewModel();
                SendNavigationRequestMessage(new Uri("/Views/Admin/SupplierContacts/ListingSupplierContacts.xaml", UriKind.Relative));
            }
                
        }

        private void ClearViewModel()
        {
            FirstName = "";
            LastName = "";
            PostalAddress = "";
            Email = "";
            MobileNumber = "";
            BusinessNumber = "";
            FaxNumber = "";
            PhysicalAddress = "";
            OfficeNumber = "";
            Description = "";
        }

        private async void Save()
        {
            var contactItems = new List<ContactItem>();
            var contactItem = new ContactItem();
            string responseMsg = string.Empty;
            var response = new ResponseBool { Success = false };


            using(var c=NestedContainer)
            {
                var contact = Using<IContactRepository>(c).GetById(Id);
                if(contact==null)
                {
                    Id = Guid.NewGuid();
                   contact=new Contact(Id);
                }
            
            
                contactItem.MasterId =Id;
                contactItem.BusinessPhone = BusinessNumber;
                contactItem.ContactClassification = (int) SelectedContactClassification;
                contactItem.ContactOwnerMasterId = SupplierId;
                contactItem.ContactTypeMasterId = SelectedContactType!=null?SelectedContactType.Id:Guid.Empty;
                contactItem.Email = Email;
                contactItem.Fax = FaxNumber;
                contactItem.Firstname = FirstName;
                contactItem.HomePhone = OfficeNumber;
                contactItem.Lastname = LastName;
                contactItem.MobilePhone = MobileNumber;
                contactItem.PhysicalAddress = PhysicalAddress;
                contactItem.PostalAddress = PostalAddress;
                contactItem.StatusId = (int) EntityStatus.Active;
                contactItem.IsNew = true;
            }
            
            contactItems.Add(contactItem);

           using(var c=NestedContainer)
           {
               response = await Using<IDistributorServiceProxy>(c).ContactsAddAsync(contactItems.ToList());
               MessageBox.Show(response.ErrorInfo, "Agrimanagr: Manage contacts", MessageBoxButton.OK,
                               MessageBoxImage.Information);
           }
           if(response.Success)
               SendNavigationRequestMessage(new Uri("/Views/Admin/SupplierContacts/ListingSupplierContacts.xaml", UriKind.Relative));
        }

        #endregion


        #region Properties


        
        public const string SupplierIdPropertyName = "SupplierId";
        private Guid _supplierId = Guid.Empty;
        public Guid SupplierId
        {
            get
            {
                return _supplierId;
            }

            set
            {
                if (_supplierId == value)
                {
                    return;
                }

                RaisePropertyChanging(SupplierIdPropertyName);
                _supplierId = value;
                RaisePropertyChanged(SupplierIdPropertyName);
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

                RaisePropertyChanging(IdPropertyName);
                _id = value;
                RaisePropertyChanged(IdPropertyName);
            }
        }

       
        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "Add Contact";
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
        
        public const string FirstNamePropertyName = "FirstName";
        private string _firstName = "";
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

                RaisePropertyChanging(FirstNamePropertyName);
                _firstName = value;
                RaisePropertyChanged(FirstNamePropertyName);
            }
        }

        
        public const string LastNamePropertyName = "LastName";
        private string _lastName = "";
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

                RaisePropertyChanging(LastNamePropertyName);
                _lastName = value;
                RaisePropertyChanged(LastNamePropertyName);
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

                RaisePropertyChanging(PostalAddressPropertyName);
                _postalAddress = value;
                RaisePropertyChanged(PostalAddressPropertyName);
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

        public const string MobileNumberPropertyName = "MobileNumber";
        private string _mobileNumber = "";
        public string MobileNumber
        {
            get
            {
                return _mobileNumber;
            }

            set
            {
                if (_mobileNumber == value)
                {
                    return;
                }

                RaisePropertyChanging(MobileNumberPropertyName);
                _mobileNumber = value;
                RaisePropertyChanged(MobileNumberPropertyName);
            }
        }

        
        public const string BusinessNumberPropertyName = "BusinessNumber";
        private string _businessNumber = "";
        public string BusinessNumber
        {
            get
            {
                return _businessNumber;
            }

            set
            {
                if (_businessNumber == value)
                {
                    return;
                }

                RaisePropertyChanging(BusinessNumberPropertyName);
                _businessNumber = value;
                RaisePropertyChanged(BusinessNumberPropertyName);
            }
        }

        
        public const string FaxNumberPropertyName = "FaxNumber";
        private string _faxNumber = "";
        public string FaxNumber
        {
            get
            {
                return _faxNumber;
            }

            set
            {
                if (_faxNumber == value)
                {
                    return;
                }

                RaisePropertyChanging(FaxNumberPropertyName);
                _faxNumber = value;
                RaisePropertyChanged(FaxNumberPropertyName);
            }
        }

        
        public const string OfficeNumberPropertyName = "OfficeNumber";
        private string _officeNumber = "";
        public string OfficeNumber
        {
            get
            {
                return _officeNumber;
            }

            set
            {
                if (_officeNumber == value)
                {
                    return;
                }

                RaisePropertyChanging(OfficeNumberPropertyName);
                _officeNumber = value;
                RaisePropertyChanged(OfficeNumberPropertyName);
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

                RaisePropertyChanging(PhysicalAddressPropertyName);
                _physicalAddress = value;
                RaisePropertyChanged(PhysicalAddressPropertyName);
            }
        }

        
        public const string DescriptionPropertyName = "Description";
        private string _description = "";
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                if (_description == value)
                {
                    return;
                }

                RaisePropertyChanging(DescriptionPropertyName);
                _description = value;
                RaisePropertyChanged(DescriptionPropertyName);
            }
        }

        
        public const string SelectedContactTypePropertyName = "SelectedContactType";
        private ContactType _selecteContactType = new ContactType(Guid.NewGuid());
        public ContactType SelectedContactType
        {
            get
            {
                return _selecteContactType;
            }

            set
            {
                if (_selecteContactType == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedContactTypePropertyName);
                _selecteContactType = value;
                RaisePropertyChanged(SelectedContactTypePropertyName);
            }
        }


        public const string SelectedContactClassificationPropertyName = "SelectedContactClassification";
        private ContactClassification _selectedContactClassification = ContactClassification.None;
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

                RaisePropertyChanging(SelectedContactClassificationPropertyName);
                _selectedContactClassification = value;
                RaisePropertyChanged(SelectedContactClassificationPropertyName);
            }
        }


        public const string IsEditPropertyName = "IsEdit";
        private bool _isEdit = false;
        public bool IsEdit
        {
            get
            {
                return _isEdit;
            }

            set
            {
                if (_isEdit == value)
                {
                    return;
                }

                RaisePropertyChanging(IsEditPropertyName);
                _isEdit = value;
                RaisePropertyChanged(IsEditPropertyName);
            }
        }

        #endregion





        
    }
}
