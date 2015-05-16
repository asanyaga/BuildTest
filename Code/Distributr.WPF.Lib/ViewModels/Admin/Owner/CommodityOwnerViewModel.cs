using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DataContracts;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Mapping;
using Distributr.Core.Workflow.Impl.AuditLogs;
using Distributr.WPF.Lib.Messages;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using GalaSoft.MvvmLight.Command;
using StructureMap;

namespace Distributr.WPF.Lib.ViewModels.Admin.Owner
{
    public class CommodityOwnerViewModel:DistributrViewModelBase
    {
        private IDistributorServiceProxy _proxy;

        public CommodityOwnerViewModel()
        {
            LoadListingPageCommand=new RelayCommand(LoadPage);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            MaritalStatusList = new ObservableCollection<MaritalStatas>();
            CommodityOwnerTypesList = new ObservableCollection<CommodityOwnerType>();

        }

        #region Class Members

        public RelayCommand LoadListingPageCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public ObservableCollection<MaritalStatas> MaritalStatusList { get; set; }
        public ObservableCollection<CommodityOwnerType> CommodityOwnerTypesList { get; set; }
        #endregion

        #region Methods
        private void LoadContactType()
        {
            using (var c = NestedContainer)
            {
                CommodityOwnerTypesList.Clear();
                CommodityOwnerTypesList.Add(new CommodityOwnerType(Guid.Empty) { Name = "--Select Commodity Owner Type--" });

                var commodityOwnerType = Using<ICommodityOwnerTypeRepository>(c).GetAll();
                commodityOwnerType.ToList().ForEach(n => CommodityOwnerTypesList.Add(n));

                SelectedCommodityOwnerType = CommodityOwnerTypesList.FirstOrDefault();
            }
        }


        private void LoadMaritalStatuses()
        {
            MaritalStatusList.Clear();
            Type _enumType = typeof(MaritalStatas);
            FieldInfo[] infos;
            infos = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            // MaritalStatusList.Clear();

            foreach (FieldInfo fi in infos)
                MaritalStatusList.Add((MaritalStatas)Enum.Parse(_enumType, fi.Name, true));
            MaritalStatusList.Remove(MaritalStatas.Default);

        }

        private void Cancel()
        {
            if (MessageBox.Show("Unsaved Changes will be lost. Do you want to continue?", "Agrimangr: " + PageTitle,
                                MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                ClearViewModel();
                SendNavigationRequestMessage(new Uri("Views/Admin/Owner/ListingMemberCommodityOwner.xaml", UriKind.Relative));
            }
                
        }

        private async void Save()
        {
            CommodityOwner commodityOwner;
            string responseMsg = string.Empty;
            var response = new ResponseBool {Success = false};


            if (DateTime.Now.Year - DateOfBirth.Year < 18)
            {
                MessageBox.Show("Farmer must be over 18 years old.", "Agrimanagr: Farmer Management",
                                MessageBoxButton.OK);
                return;
            }

            if (string.IsNullOrEmpty(Surname) || string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(IdNumber) || string.IsNullOrEmpty(PinNumber) || string.IsNullOrEmpty(SelectedCommodityOwnerTypePropertyName) || string.IsNullOrEmpty(PostalAddress) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrEmpty(BusinessNumber) || string.IsNullOrEmpty(PhysicalAddress))
            {
                MessageBox.Show("Please fill in all the mandatory fields.", "Agrimanagr: Farmer Management",
                                MessageBoxButton.OK);
                return;
            }
                using (var c = NestedContainer)
                {
                    commodityOwner = Using<ICommodityOwnerRepository>(c).GetById(Id);
                    if(commodityOwner==null)
                    {
                        commodityOwner = new CommodityOwner(Id);
                    }
                    commodityOwner.Surname = Surname;
                    commodityOwner.FirstName = FirstName;
                    commodityOwner.Code = Code;
                    commodityOwner.IdNo = IdNumber;
                    commodityOwner.PinNo = PinNumber;
                    commodityOwner.MaritalStatus = SelectedMaritalStatus;
                    commodityOwner.CommodityOwnerType = SelectedCommodityOwnerType;
                    commodityOwner.PhysicalAddress = PhysicalAddress;
                    commodityOwner.PhoneNumber = PhoneNumber;
                    commodityOwner.BusinessNumber = BusinessNumber;
                    commodityOwner.OfficeNumber = OfficeNumber;
                    commodityOwner.FaxNumber = FaxNumber;
                    commodityOwner.Email = Email;
                   
                    commodityOwner.DateOfBirth = DateOfBirth;
                    commodityOwner.Description = Description;
                    commodityOwner.PostalAddress = PostalAddress;
                    commodityOwner._Status = EntityStatus.Active;
                
                var commoditySupplier = Using<ICommoditySupplierRepository>(c).GetById(SupplierId) as CommoditySupplier;
                if (commoditySupplier != null)
                {
                    commodityOwner.CommoditySupplier = commoditySupplier;
                }


                //if (IsValid(commodityOwner))
                //{
                    _proxy = Using<IDistributorServiceProxy>(c);
                    response = await SaveCommodityOwner(commodityOwner);
                    if (response.ErrorInfo != "")
                        responseMsg += response.ErrorInfo + "\n";
                //}

                if (response.Success&& string.IsNullOrEmpty(response.ErrorInfo))
                {
                     responseMsg = "Farmer Added Successfully!";
                    MessageBox.Show(responseMsg, "Agrimanager :" + PageTitle, MessageBoxButton.OK,
                               MessageBoxImage.Information);
                    SendNavigationRequestMessage(new Uri("Views/Admin/Owner/ListingMemberCommodityOwner.xaml", UriKind.Relative));
                }
                else
                {
                    MessageBox.Show(responseMsg, "Agrimanager :" + PageTitle, MessageBoxButton.OK,
                               MessageBoxImage.Information);
                }
                   
            }
        }

        private async Task<ResponseBool> SaveCommodityOwner(CommodityOwner commodityOwner)
        {
            //if (commodityOwner == null) throw new ArgumentNullException("commodityOwner");

            var response = new ResponseBool { Success = false };
            var logs = new List<string>();

           // CommodityOwnerItem itemToSave = Map(commodityOwner);
            string log = string.Format("Created farmer: {0}; Code: {1}; In Account {2}",
                                       commodityOwner.FullName,
                                       commodityOwner.Code, commodityOwner.CommoditySupplier.Name);
            logs.Add(log);
            using (var c = NestedContainer)
            {
                var mapper = Using<IMasterDataToDTOMapping>(c);
                var commodityOwnerDto = mapper.Map(commodityOwner);
                response = await _proxy.CommodityOwnerAddAsync(commodityOwnerDto);
                if (response.Success)
                    logs.ForEach(n => Using<IAuditLogWFManager>(c).AuditLogEntry("Farmer Management", n));
            }
            return response;
        }

        private CommodityOwnerItem Map(CommodityOwner co)
        {
            var item = new CommodityOwnerItem
                {
                    BusinessNumber = co.BusinessNumber,
                    Code = co.Code,
                    CommodityOwnerTypeMasterId = co.CommodityOwnerType.Id,
                    CommoditySupplierMasterId = co.CommoditySupplier.Id,
                    DateCreated = co._DateCreated,
                    DateLastUpdated = co._DateLastUpdated,
                    DateOfBirth = co.DateOfBirth,
                    Description = co.Description,
                    Email = co.Email,
                    FaxNumber = co.FaxNumber,
                    FirstName = co.FirstName,
                    GenderId = (int)co.Gender,
                    IdNo = co.IdNo,
                    LastName = co.LastName,
                    MaritalStatasMasterId = (int)co.MaritalStatus,
                    MasterId = co.Id,
                    OfficeNumber = co.OfficeNumber,
                    PhoneNumber = co.PhoneNumber,
                    PhysicalAddress = co.PhysicalAddress,
                    PinNo = co.PinNo,
                    PostalAddress = co.PostalAddress,
                    StatusId = (int)co._Status,
                    Surname = co.Surname
                };
            return item;
        }

        private void LoadPage()
        {
            LoadContactType();
            LoadMaritalStatuses();

            ClearViewModel();

            if(IsEdit)
            {
                PageTitle = "Edit Farmer";
                LoadForEdit();
                IsEdit = false;
            }
            else
            {
                Id = Guid.NewGuid();
                PageTitle = "Add Farmer";
                //DefaultData();
            }
            

        }

        private void ClearViewModel()
        {
            FirstName = "";
            Surname = "";
            LastName = "";
            Code = "";
            IdNumber = "";
            PinNumber ="";
            PhysicalAddress = "";
            PostalAddress = "";
            Email = "";
            PhoneNumber = "";
            BusinessNumber = "";
            FaxNumber = "";
            OfficeNumber = "";
            Description = "";
            SelectedMaritalStatus = MaritalStatas.Single;
        }

        private void LoadForEdit()
        {
            using(var c=NestedContainer)
            {
                var commodityOwner = Using<ICommodityOwnerRepository>(c).GetById(Id);

                FirstName = commodityOwner.FirstName;
                Code = commodityOwner.Code;
                Surname = commodityOwner.Surname;
                LastName = commodityOwner.LastName;
                IdNumber = commodityOwner.IdNo;
                PinNumber = commodityOwner.PinNo;
                PhysicalAddress =commodityOwner.PhysicalAddress;
                PostalAddress = commodityOwner.PostalAddress;
                Email = commodityOwner.Email;
                PhoneNumber =commodityOwner.PhoneNumber;
                BusinessNumber = commodityOwner.BusinessNumber;
                FaxNumber = commodityOwner.FaxNumber;
                OfficeNumber =commodityOwner.OfficeNumber;
                DateOfBirth = commodityOwner.DateOfBirth;
                Description =commodityOwner.Description;
                SelectedMaritalStatus = MaritalStatas.Default;
                SelectedMaritalStatus = MaritalStatusList.FirstOrDefault(m => m == commodityOwner.MaritalStatus);
                SelectedCommodityOwnerType =CommodityOwnerTypesList.FirstOrDefault(co => co.Id == commodityOwner.CommodityOwnerType.Id);// commodityOwner.CommodityOwnerType;

            }

            
        }

        private void DefaultData()
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Random rand = new Random();
                
                FirstName = "Michael";
                Surname = "Wainaina";
                LastName = "Muthee";
                Code = System.IO.Path.GetRandomFileName();
                IdNumber = rand.Next(1000000, 9999999).ToString();
                PinNumber = rand.Next(1000000, 9999999).ToString();
                PhysicalAddress = "Marekani";
                PostalAddress = "P.O Box 5654";
                Email = "mm@gmail.com";
                PhoneNumber = rand.Next(1000000, 9999999).ToString();
                BusinessNumber = rand.Next(1000000, 9999999).ToString();
                FaxNumber = rand.Next(1000000, 9999999).ToString();
                OfficeNumber = rand.Next(1000000, 9999999).ToString();
                Description = "Farmer to watch";
                
            }
        }

         public void AddOwner(AddCommodityOwnerMessage messageTo)
         {
             SupplierId = messageTo.SupplierId;
         }
        public void EditOwner(EditCommodityOwnerMessage messageTo)
        {
            SupplierId = messageTo.SupplierId;
            Id = messageTo.CommodityOwnerId;
            IsEdit = true;
        }
        #endregion

        #region Properties
         public const string SupplierIdPropertyName = "SupplierId";
         private Guid _supplierId = Guid.NewGuid();
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

         
         public const string SurnamePropertyName = "Surname";
         private string _surname = "";
         public string Surname
         {
             get
             {
                 return _surname;
             }

             set
             {
                 if (_surname == value)
                 {
                     return;
                 }

                 RaisePropertyChanging(SurnamePropertyName);
                 _surname = value;
                 RaisePropertyChanged(SurnamePropertyName);
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

         
         public const string IdNumberPropertyName = "IdNumber";
         private string _idNumber = "";
         public string IdNumber
         {
             get
             {
                 return _idNumber;
             }

             set
             {
                 if (_idNumber == value)
                 {
                     return;
                 }

                 RaisePropertyChanging(IdNumberPropertyName);
                 _idNumber = value;
                 RaisePropertyChanged(IdNumberPropertyName);
             }
         }

         
         public const string PinNumberPropertyName = "PinNumber";
         private string _pinNumber = "";
         public string PinNumber
         {
             get
             {
                 return _pinNumber;
             }

             set
             {
                 if (_pinNumber == value)
                 {
                     return;
                 }

                 RaisePropertyChanging(PinNumberPropertyName);
                 _pinNumber = value;
                 RaisePropertyChanged(PinNumberPropertyName);
             }
         }

         
         public const string DateOfBirthPropertyName = "DateOfBirth";
         private DateTime _dateOfBirth = DateTime.Now;
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

                 RaisePropertyChanging(DateOfBirthPropertyName);
                 _dateOfBirth = value;
                 RaisePropertyChanged(DateOfBirthPropertyName);
             }
         }

         
         public const string SelectedMaritalStatusPropertyName = "SelectedMaritalStatus";
         private MaritalStatas _selectedMariatalStatus = MaritalStatas.Single;
         public MaritalStatas SelectedMaritalStatus
         {
             get
             {
                 return _selectedMariatalStatus;
             }

             set
             {
                 if (_selectedMariatalStatus == value)
                 {
                     return;
                 }

                 RaisePropertyChanging(SelectedMaritalStatusPropertyName);
                 _selectedMariatalStatus = value;
                 RaisePropertyChanged(SelectedMaritalStatusPropertyName);
             }
         }

         
         public const string SelectedCommodityOwnerTypePropertyName = "SelectedCommodityOwnerType";
         private CommodityOwnerType _selectedCommodityOwnerType = new CommodityOwnerType(Guid.Empty);
         public CommodityOwnerType SelectedCommodityOwnerType
         {
             get
             {                  
                 return _selectedCommodityOwnerType;
             }

             set
             {
                 if (_selectedCommodityOwnerType == value)
                 {
                     return;
                 }

                 RaisePropertyChanging(SelectedCommodityOwnerTypePropertyName);
                 _selectedCommodityOwnerType = value;
                 RaisePropertyChanged(SelectedCommodityOwnerTypePropertyName);
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
         #endregion
    }

   
}
