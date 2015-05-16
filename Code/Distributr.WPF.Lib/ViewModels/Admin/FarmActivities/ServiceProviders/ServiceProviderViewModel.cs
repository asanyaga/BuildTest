using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.MasterDataDTO.DataContracts;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.WPF.Lib.Messages;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.ViewModels.Admin.FarmActivities.Services;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.FarmActivities.ServiceProviders
{
    public class ServiceProviderViewModel : DistributrViewModelBase
    {
        public ServiceProviderViewModel()
        {
            LoadPageCommand = new RelayCommand(Load);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            SelectedBankChangedCommand = new RelayCommand(BankChanged);
            BankList = new ObservableCollection<Bank>();
            BankListOne = new ObservableCollection<Bank>();
            BankBranchList = new ObservableCollection<BankBranch>();
            GenderList = new ObservableCollection<Gender>();
        }



        #region Class Members
        public RelayCommand LoadPageCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand SelectedBankChangedCommand { get; set; }
        public ObservableCollection<Bank> BankList { get; set; }

        public ObservableCollection<Bank> BankListOne { get; set; }
        public ObservableCollection<BankBranch> BankBranchList { get; set; }
        public ObservableCollection<Gender> GenderList { get; set; }
        #endregion

        #region Methods

        public void EditServiceProvider(EditServiceProviderMessage messageFrom)
        {
            Id = messageFrom.ServiceProviderId;
            IsEdit = true;

        }
        private void Load()
        {

            LoadBanks();
            LoadGender();
            ClearViewModel();
            if (!IsEdit)
            {
                Id = Guid.NewGuid();
                ContactId = Guid.NewGuid();
                PageTitle = "Add Service Provider";
            }
            else
            {
                LoadForEdit();
                IsEdit = false;
            }
        }

        private void ClearViewModel()
        {
            Name = "";
            Code = "";
            IdNo = "";
            PinNo = "";
            MobileNumber = "";
            AccountName = "";
            AccountNumber = "";
            Description = "";
            SelectedGender = Gender.Unknown;
            SelectedBankOne = new Bank(Guid.NewGuid()) { Name = "--Select Bank--" };
            SelectedBankBranch = new BankBranch(Guid.NewGuid()) { Name = "--Select Bank Branch--" };

            PhysicalAddress = "";
            PostalAddress = "";
            Email = "";
            PhoneNumber = "";
            BusinessNumber = "";
            FaxNumber = "";

        }

        private void LoadForEdit()
        {
            PageTitle = "Edit Service Provider";
            using (var c = NestedContainer)
            {
                var serviceProvider = Using<IServiceProviderRepository>(c).GetById(Id);
                if (serviceProvider != null)
                {
                    Name = serviceProvider.Name;
                    Code = serviceProvider.Code;
                    IdNo = serviceProvider.IdNo;
                    PinNo = serviceProvider.PinNo;
                    MobileNumber = serviceProvider.MobileNumber;
                    AccountName = serviceProvider.AccountName;
                    AccountNumber = serviceProvider.AccountNumber;
                    Description = serviceProvider.Description;
                    SelectedGender = GenderList.FirstOrDefault(g => g == serviceProvider.Gender);
                    SelectedBankOne = BankListOne.FirstOrDefault(b => b.Id == serviceProvider.Bank.Id);
                    BankChanged();
                    SelectedBankBranch = BankBranchList.FirstOrDefault(b => b.Id == serviceProvider.BankBranch.Id);
                }

            }
        }

        private void LoadGender()
        {
            GenderList.Clear();
            Type _enumType = typeof(Gender);
            FieldInfo[] infos;
            infos = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (FieldInfo fi in infos)
                GenderList.Add((Gender)Enum.Parse(_enumType, fi.Name, true));

            GenderList.Remove(Gender.Unknown);
        }

        private void LoadBanks()
        {
            BankList.Clear();
            BankList.Add(new Bank(Guid.NewGuid()) { Name = "--Select Bank--" });
            using (var c = NestedContainer)
            {
                var allBanks = Using<IBankRepository>(c).GetAll();
                foreach (var bank in allBanks)
                {
                    BankList.Add(bank);
                }
            }

            BankListOne.Clear();
            BankListOne.Add(new Bank(Guid.NewGuid()) { Name = "--Select Bank--" });
            using (var c = NestedContainer)
            {
                var allBanks = Using<IBankRepository>(c).GetAll();
                foreach (var bank in allBanks)
                {
                    BankListOne.Add(bank);
                }
            }
        }

        private void BankChanged()
        {
            BankBranchList.Clear();
            BankBranchList.Add(new BankBranch(Guid.NewGuid()) { Name = "--Select Bank Branch--" });
            using (var c = NestedContainer)
            {
                if (SelectedBankOne != null)
                {
                    var allBankBranches = Using<IBankBranchRepository>(c).GetByBankMasterId(SelectedBankOne.Id);

                    foreach (var bankBranch in allBankBranches)
                    {
                        BankBranchList.Add(bankBranch);

                    }
                }

            }

        }




        private void Cancel()
        {
            if (MessageBox.Show("Unsaved Changes will be lost. Do you want to continue?", "Agrimangr: " + PageTitle,
                               MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                ClearViewModel();
                SendNavigationRequestMessage(new Uri("/Views/Admin/FarmActivities/ServiceProviders/ListServiceProviders.xaml", UriKind.Relative));
            }

        }

        private async void Save()
        {
            if (Validate())
            {
                MessageBox.Show("Please fill in the required fields",PageTitle,
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            using (var c = NestedContainer)
            {
                var serviceProvider = Using<IServiceProviderRepository>(c).GetById(Id);
                if (serviceProvider == null)
                {
                    serviceProvider = new ServiceProvider(Id);
                }
                serviceProvider.Code = Code;
                serviceProvider.Name = Name;
                serviceProvider.IdNo = IdNo;
                serviceProvider.PinNo = PinNo;
                serviceProvider.MobileNumber = MobileNumber;
                serviceProvider.AccountNumber = AccountNumber;
                serviceProvider.AccountName = AccountName;
                serviceProvider.Bank = SelectedBankOne;
                serviceProvider.BankBranch = SelectedBankBranch;
                serviceProvider.Gender = SelectedGender;
                serviceProvider._Status = EntityStatus.Active;

                var response = await Using<IDistributorServiceProxy>(c).ServiceProviderSaveAsync(serviceProvider);

                if (!response.Success)
                {
                    MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Service Provider ",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                }

                //var contact = Using<IContactRepository>(c).GetByContactOwnerId(Id);
                //if(contact==null)
                //{
                //    contact = new Contact(Guid.NewGuid());
                //}
                //contact.Firstname = Name;
                //contact.MobilePhone = MobileNumber;
                //contact.PhysicalAddress = PhysicalAddress;
                //contact.PostalAddress = PostalAddress;
                //contact.Email = Email;
                //contact.HomePhone = PhoneNumber;
                //contact.BusinessPhone = BusinessNumber;
                //contact.Fax = FaxNumber;
                //contact.ContactOwnerMasterId = Id;
                //var listContact = new List<ContactItem>();
                //listContact.Add(Map(contact));

                //var cresponse = await Using<IDistributorServiceProxy>(c).ContactsAddAsync(listContact);

                //if (!string.IsNullOrWhiteSpace(cresponse.ErrorInfo))
                //{
                //    MessageBox.Show(cresponse.ErrorInfo, "Agrimangr: Manage Service Provider ",
                //                    MessageBoxButton.OK, MessageBoxImage.Information);
                //}
                if (response.Success)
                {
                    MessageBox.Show("Service Provider Successfully Added", "Agrimangr: Manage Service Provider ",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    SendNavigationRequestMessage(new Uri("/Views/Admin/FarmActivities/ServiceProviders/ListServiceProviders.xaml", UriKind.Relative));
                }

            }
        }

        private bool Validate()
        {
            var result = Code == string.Empty || Name == string.Empty || IdNo == string.Empty || PinNo == string.Empty || MobileNumber == string.Empty;
            return result;
        }

        private ContactItem Map(Contact contact)
        {
            var item = new ContactItem
                {

                    MasterId = contact.Id,
                    Firstname = contact.Firstname,
                    BusinessPhone = contact.BusinessPhone,
                    Fax = contact.Fax,
                    PhysicalAddress = contact.PhysicalAddress,
                    PostalAddress = contact.PostalAddress,
                    HomePhone = contact.HomePhone,
                    MobilePhone = contact.MobilePhone,
                    Email = contact.Email,
                    IsNew = true,
                    ContactOwnerMasterId = contact.ContactOwnerMasterId,


                };
            return item;
        }

        #endregion

        #region Properties

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

                RaisePropertyChanging(ContactIdPropertyName);
                _contactId = value;
                RaisePropertyChanged(ContactIdPropertyName);
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

                RaisePropertyChanging(PageTitlePropertyName);
                _pageTitle = value;
                RaisePropertyChanged(PageTitlePropertyName);
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

                RaisePropertyChanging(CodePropertyName);
                _code = value;
                RaisePropertyChanged(CodePropertyName);
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

        public const string IdNoPropertyName = "IdNo";
        private string _idNo = "";
        public string IdNo
        {
            get
            {
                return _idNo;
            }

            set
            {
                if (_idNo == value)
                {
                    return;
                }

                RaisePropertyChanging(IdNoPropertyName);
                _idNo = value;
                RaisePropertyChanged(IdNoPropertyName);
            }
        }


        public const string PinNoPropertyName = "PinNo";
        private string _pinNo = "";
        public string PinNo
        {
            get
            {
                return _pinNo;
            }

            set
            {
                if (_pinNo == value)
                {
                    return;
                }

                RaisePropertyChanging(PinNoPropertyName);
                _pinNo = value;
                RaisePropertyChanged(PinNoPropertyName);
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


        public const string AccountNamePropertyName = "AccountName";
        private string _accountName = "";
        public string AccountName
        {
            get
            {
                return _accountName;
            }

            set
            {
                if (_accountName == value)
                {
                    return;
                }

                RaisePropertyChanging(AccountNamePropertyName);
                _accountName = value;
                RaisePropertyChanged(AccountNamePropertyName);
            }
        }


        public const string AccountNumberPropertyName = "AccountNumber";
        private string _accountNumber = "";
        public string AccountNumber
        {
            get
            {
                return _accountNumber;
            }

            set
            {
                if (_accountNumber == value)
                {
                    return;
                }

                RaisePropertyChanging(AccountNumberPropertyName);
                _accountNumber = value;
                RaisePropertyChanged(AccountNumberPropertyName);
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


      

        public const string SelectedBankOnePropertyName = "SelectedBankOne";
        private Bank _selectedBankOne = new Bank(Guid.Empty){Name = "--Select Bank--"};
        public Bank SelectedBankOne
        {
            get
            {
                return _selectedBankOne;
            }

            set
            {
                if (_selectedBankOne == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedBankOnePropertyName);
                _selectedBankOne = value;
                RaisePropertyChanged(SelectedBankOnePropertyName);
            }
        }

        public const string SelectedBankBranchPropertyName = "SelectedBankBranch";
        private BankBranch _selectedBankBranch = new BankBranch(Guid.Empty) { Name = "--Select Bank Branch--" };
        public BankBranch SelectedBankBranch
        {
            get
            {
                return _selectedBankBranch;
            }

            set
            {
                if (_selectedBankBranch == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedBankBranchPropertyName);
                _selectedBankBranch = value;
                RaisePropertyChanged(SelectedBankBranchPropertyName);
            }
        }


        public const string SelectedGenderPropertyName = "SelectedGender";
        private Gender _selectedGender = Gender.Male;
        public Gender SelectedGender
        {
            get
            {
                return _selectedGender;
            }

            set
            {
                if (_selectedGender == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedGenderPropertyName);
                _selectedGender = value;
                RaisePropertyChanged(SelectedGenderPropertyName);
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


        public const string PhoneNumberPropertyName = "PhoneNumber";
        private string _phoneNumber = "";
        public string PhoneNumber
        {
            get
            {
                return _phoneNumber;
            }

            set
            {
                if (_phoneNumber == value)
                {
                    return;
                }

                RaisePropertyChanging(PhoneNumberPropertyName);
                _phoneNumber = value;
                RaisePropertyChanged(PhoneNumberPropertyName);
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


        #endregion

    }
}
