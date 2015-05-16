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
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Factory.Master.Impl;
using Distributr.Core.MasterDataDTO.DataContracts;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Workflow.Impl.AuditLogs;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.ViewModels.Admin.CommodityProducers;
using Distributr.WPF.Lib.ViewModels.Admin.Contacts;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight.Command;
using StructureMap;

namespace Distributr.WPF.Lib.ViewModels.Admin.CommodityOwners
{
    public class EditIndividualFarmerViewModel : MasterEntityContactUtilsViewModel
    {
        private IDistributorServiceProxy _proxy;

        public EditIndividualFarmerViewModel()
        {

            ContactsList = new ObservableCollection<VMContactItem>();
            CommodityOwnerTypesList = new ObservableCollection<CommodityOwnerType>();
            MaritalStatusList = new ObservableCollection<MaritalStatas>();
            BankLookUpList = new ObservableCollection<Bank>();
            BankBranchLookUpList=new ObservableCollection<BankBranch>();

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);

            AssignSelectedCommand = new RelayCommand(AssignCentre);
            AssignAllCommand = new RelayCommand(AssignAllCentres);
            UnassignSelectedCommand = new RelayCommand(UnassignCentre);
            UnassignAllCommand = new RelayCommand(UnassignAllCentres);
            BankChangedCommand = new RelayCommand(BankChanged);

            AssignedCentresList = new ObservableCollection<VMCentreItem>();
            UnassignedCentresList = new ObservableCollection<VMCentreItem>();
            ContactOwnerTypesList= new ObservableCollection<ContactType>();
        }

       
        #region properties

        public ObservableCollection<CommodityOwnerType> CommodityOwnerTypesList { get; set; }
        public ObservableCollection<ContactType> ContactOwnerTypesList { get; set; }
        public ObservableCollection<MaritalStatas> MaritalStatusList { get; set; }
        public ObservableCollection<Bank> BankLookUpList { get; set; }
        public ObservableCollection<BankBranch> BankBranchLookUpList { get; set; }

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand AssignSelectedCommand { get; set; }
        public RelayCommand AssignAllCommand { get; set; }
        public RelayCommand UnassignSelectedCommand { get; set; }
        public RelayCommand UnassignAllCommand { get; set; }
        public RelayCommand BankChangedCommand { get; set; }

        public const string CommodityOwnerPropertyName = "CommodityOwner";
        private CommodityOwner _commodityOwner = null;

        public CommodityOwner CommodityOwner
        {
            get { return _commodityOwner; }

            set
            {
                if (_commodityOwner == value)
                {
                    return;
                }

                RaisePropertyChanging(CommodityOwnerPropertyName);
                _commodityOwner = value;
                RaisePropertyChanged(CommodityOwnerPropertyName);
            }
        }

        public const string SelectedCommodityOwnerTypePropertyName = "SelectedCommodityOwnerType";
        private CommodityOwnerType _commodityOwnerType = null;
        public CommodityOwnerType SelectedCommodityOwnerType
        {
            get
            {
                return _commodityOwnerType;
            }

            set
            {
                if (_commodityOwnerType == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCommodityOwnerTypePropertyName);
                _commodityOwnerType = value;
                RaisePropertyChanged(SelectedCommodityOwnerTypePropertyName);
            }
        }

        public const string SelectedContactOwnerTypePropertyName = "SelectedContactOwnerType";
        private ContactType _contactOwnerType = null;
        public ContactType SelectedContactOwnerType
        {
            get
            {
                return _contactOwnerType;
            }

            set
            {
                if (_contactOwnerType == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedContactOwnerTypePropertyName);
                _contactOwnerType = value;
                RaisePropertyChanged(SelectedContactOwnerTypePropertyName);
            }
        }
        public const string SelectedMaritalStatusPropertyName = "SelectedMaritalStatus";
        private MaritalStatas _selectedMaritalStatud = MaritalStatas.Unknown;
        public MaritalStatas SelectedMaritalStatus
        {
            get
            {
                return _selectedMaritalStatud;
            }

            set
            {
                //if (_selectedMaritalStatud == value)
                //{
                //    return;
                //}

                RaisePropertyChanging(SelectedMaritalStatusPropertyName);
                _selectedMaritalStatud = value;
                RaisePropertyChanged(SelectedMaritalStatusPropertyName);
            }
        }

        private CommodityOwnerType _defaultCommodityOwnerType;
        private CommodityOwnerType DefaultCommodityOwnerType
        {
            get
            {
                return _defaultCommodityOwnerType ??
                       (_defaultCommodityOwnerType =
                        new CommodityOwnerType(Guid.Empty) { Name = "--Select commodity owner type--" });
            }
        }
        
        public const string DateOfBirthPropertyName = "DateOfBirth";
        private DateTime _dob = DateTime.Now.AddYears(-18);
        public DateTime DateOfBirth
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

                RaisePropertyChanging(DateOfBirthPropertyName);
                _dob = value;
                RaisePropertyChanged(DateOfBirthPropertyName);
            }
        }

        public ObservableCollection<VMCentreItem> AssignedCentresList { get; set; }
        public ObservableCollection<VMCentreItem> UnassignedCentresList { get; set; }

        public const string CommodityProducerPropertyName = "CommodityProducer";
        private CommodityProducer _commodityProducer = null;
        public CommodityProducer CommodityProducer
        {
            get { return _commodityProducer; }

            set
            {
                if (_commodityProducer == value)
                {
                    return;
                }

                RaisePropertyChanging(CommodityProducerPropertyName);
                _commodityProducer = value;
                RaisePropertyChanged(CommodityProducerPropertyName);
            }
        }

        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "Create Farmer";
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

        public const string CommoditySupplierPropertyName = "CommoditySupplier";
        private CommoditySupplier _commoditySupplier = null;
        public CommoditySupplier CommoditySupplier
        {
            get
            {
                return _commoditySupplier;
            }

            set
            {
                if (_commoditySupplier == value)
                {
                    return;
                }

                RaisePropertyChanging(CommoditySupplierPropertyName);
                _commoditySupplier = value;
                RaisePropertyChanged(CommoditySupplierPropertyName);
            }
        }

        public const string SelectedCommoditySupplierTypePropertyName = "SelectedCommoditySupplierType";
        private CommoditySupplierType _commoditySupplierType = CommoditySupplierType.Individual;
        [Required(ErrorMessage = "Commodity supplier type is required..")]
        public CommoditySupplierType SelectedCommoditySupplierType
        {
            get
            {
                return _commoditySupplierType;
            }

            set
            {
                //if (_commoditySupplierType == value)
                //{
                //    return;
                //}

                RaisePropertyChanging(SelectedCommoditySupplierTypePropertyName);
                _commoditySupplierType = value;
                RaisePropertyChanged(SelectedCommoditySupplierTypePropertyName);
            }
        }


        
        public const string SelectedBankPropertyName = "SelectedBank";
        private Bank _selectedBank = null;
        public Bank SelectedBank
        {
            get
            {
                return _selectedBank;
            }

            set
            {
                if (_selectedBank == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedBankPropertyName);
                _selectedBank = value;
                RaisePropertyChanged(SelectedBankPropertyName);
              
            }
        }

       
        public const string SelectedBankBranchPropertyName = "SelectedBankBranch";
        private BankBranch _bankBranchItem = null;
        public BankBranch SelectedBankBranch
        {
            get
            {
                return _bankBranchItem;
            }

            set
            {
                if (_bankBranchItem == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedBankBranchPropertyName);
                _bankBranchItem = value;
                RaisePropertyChanged(SelectedBankBranchPropertyName);
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

        #endregion

        #region methods

        protected override void LoadPage(Page page)
        {
            Guid commodityOwnerId = PresentationUtility.ParseIdFromUrl(page.NavigationService.CurrentSource);
            ContactsList.Clear();
            using (var c = NestedContainer)
            {
                if (commodityOwnerId == Guid.Empty)
                {
                    PageTitle = "Add Farmer";
                    CommoditySupplier = Using<CostCentreFactory>(c).CreateCostCentre(Guid.NewGuid(), CostCentreType.CommoditySupplier,
                                                                     Using<ICostCentreRepository>(c).GetById(
                                                                         GetConfigParams().CostCentreId)) as
                        CommoditySupplier;
                    CommodityOwner = new CommodityOwner(Guid.NewGuid()) {CommoditySupplier = CommoditySupplier};
                    CommodityProducer = new CommodityProducer(Guid.NewGuid())
                                            {
                                                CommoditySupplier = CommoditySupplier,
                                                CommodityProducerCentres = new List<Centre>()
                                            };
                }
                else
                {
                    PageTitle = "Edit Farmer";
                    var commodityOwner = Using<ICommodityOwnerRepository>(c).GetById(commodityOwnerId);
                    CommodityOwner = commodityOwner.DeepClone<CommodityOwner>();
                    CommoditySupplier = commodityOwner.CommoditySupplier.DeepClone<CommoditySupplier>();
                    CommodityProducer =
                        Using<ICommodityProducerRepository>(c).GetAll().FirstOrDefault(
                            n => n.CommoditySupplier.Id == CommoditySupplier.Id);

                }
                Setup();
            }
        }

        private void Setup()
        {
            LoadCentresLists();
            LoadCommodityOwnerTypes();
            LoadMaritalStatuses();
            LoadContactType();
            LoadBankItems();

        }

        private void BankChanged()
        {
            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                BankBranchLookUpList.Clear();
                BankBranchLookUpList.Add(new BankBranch(Guid.Empty) {Name = "--Select Bank Branch--"});
                var bankBranchRepository = c.GetInstance<IBankBranchRepository>();
                var bankbranchList = bankBranchRepository.GetByBankMasterId(SelectedBank.Id);
                bankbranchList.ToList().ForEach(n => BankBranchLookUpList.Add(n));
                SelectedBankBranch = BankBranchLookUpList.FirstOrDefault();

            }

        }

        private void LoadBankItems()
        {
            using (var c=ObjectFactory.Container.GetNestedContainer())
            {
                BankLookUpList.Clear();
                BankLookUpList.Add(new Bank(Guid.Empty) {Name = "--Select Bank--"});
                var bankRepository = c.GetInstance<IBankRepository>();
                var banklist = bankRepository.GetAll();
                banklist.ToList().ForEach(n => BankLookUpList.Add(n));
                SelectedBank = BankLookUpList.FirstOrDefault();
            }
        }

        private void LoadContactType()
        {
            using (var c = NestedContainer)
            {
                ContactOwnerTypesList.Clear();
               // ContactOwnerTypesList.Add(DefaultCommodityOwnerType);
                Using<IContactTypeRepository>(c).GetAll().OrderBy(n => n.Name).ToList().ForEach(
                    n => ContactOwnerTypesList.Add(n as ContactType));
                //SelectedContactOwnerType = DefaultCommodityOwnerType;

                //if (CommodityOwner.CommodityOwnerType != null)
                //    SelectedCommodityOwnerType = CommodityOwnerTypesList.FirstOrDefault(n => n.Id == CommodityOwner.CommodityOwnerType.Id);
            }
        }

        private void LoadCentresLists()
        {
            AssignedCentresList.Clear();
            UnassignedCentresList.Clear();

            if (CommodityProducer.CommodityProducerCentres != null)
                CommodityProducer.CommodityProducerCentres.OrderBy(n => n.Name).ToList().ForEach(
                    n => AssignedCentresList.Add(new VMCentreItem { Centre = n, IsSelected = false }));

            using (var c = NestedContainer)
            {
                var allCentres = Using<ICentreRepository>(c).GetAll();
                allCentres.Where(n => AssignedCentresList.All(p => p.Centre.Id != n.Id)).OrderBy(
                    n => n.Name).ToList().ForEach
                    (n => UnassignedCentresList.Add(new VMCentreItem { Centre = n, IsSelected = false }));
            }
        }

        private void LoadCommodityOwnerTypes()
        {
            using (var c = NestedContainer)
            {
                CommodityOwnerTypesList.Clear();
                CommodityOwnerTypesList.Add(DefaultCommodityOwnerType);
                Using<ICommodityOwnerTypeRepository>(c).GetAll().OrderBy(n => n.Name).ToList().ForEach(
                    n => CommodityOwnerTypesList.Add(n as CommodityOwnerType));
                SelectedCommodityOwnerType = DefaultCommodityOwnerType;

                if (CommodityOwner.CommodityOwnerType != null)
                    SelectedCommodityOwnerType = CommodityOwnerTypesList.FirstOrDefault(n => n.Id == CommodityOwner.CommodityOwnerType.Id);
            }
        }

        private void LoadMaritalStatuses()
        {
            Type _enumType = typeof(MaritalStatas);
            FieldInfo[] infos;
            infos = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            MaritalStatusList.Clear();

            foreach (FieldInfo fi in infos)
                MaritalStatusList.Add((MaritalStatas)Enum.Parse(_enumType, fi.Name, true));

            SelectedMaritalStatus = MaritalStatusList.FirstOrDefault(n => n == CommodityOwner.MaritalStatus);
        }

        private void AssignCentre()
        {
            List<VMCentreItem> selected = new List<VMCentreItem>();
            UnassignedCentresList.Where(n => n.IsSelected).ToList().ForEach(selected.Add);
            foreach (var item in selected)
            {
                if (AssignedCentresList.All(n => n.Centre.Id != item.Centre.Id))
                {
                    AssignedCentresList.Add(item);
                    UnassignedCentresList.Remove(item);
                }
            }
        }

        private void AssignAllCentres()
        {
            List<VMCentreItem> selected = new List<VMCentreItem>();
            UnassignedCentresList.ToList().ForEach(selected.Add);
            foreach (var item in selected)
            {
                if (AssignedCentresList.All(n => n.Centre.Id != item.Centre.Id))
                {
                    AssignedCentresList.Add(item);
                    UnassignedCentresList.Remove(item);
                }
            }
        }

        private void UnassignCentre()
        {
            List<VMCentreItem> selected = new List<VMCentreItem>();
            AssignedCentresList.Where(n => n.IsSelected).ToList().ForEach(selected.Add);
            foreach (var item in selected)
            {
                if (UnassignedCentresList.All(n => n.Centre.Id != item.Centre.Id))
                {
                    UnassignedCentresList.Add(item);
                    AssignedCentresList.Remove(item);
                }
            }
        }

        private void UnassignAllCentres()
        {
            List<VMCentreItem> selected = new List<VMCentreItem>();
            AssignedCentresList.ToList().ForEach(selected.Add);
            foreach (var item in selected)
            {
                if (UnassignedCentresList.All(n => n.Centre.Id != item.Centre.Id))
                {
                    UnassignedCentresList.Add(item);
                    AssignedCentresList.Remove(item);
                }
            }
        }
        private async void Save()
        {
            DateTime now = DateTime.Now;
            CommoditySupplier.CommoditySupplierType = CommoditySupplierType.Individual;
            CommoditySupplier.JoinDate = now;
            CommoditySupplier.Contact.Clear();
            CommoditySupplier.Name = CommodityOwner.FullName;
            CommoditySupplier.BankId =SelectedBank!=null? SelectedBank.Id:Guid.Empty;
            CommoditySupplier.BankBranchId = SelectedBankBranch != null ? SelectedBankBranch.Id : Guid.Empty;
            CommoditySupplier.AccountNo = AccountNumber;
            CommoditySupplier.AccountName = AccountName;
            

            if (!IsValid(CommoditySupplier))
                return;

            CommodityOwner.CommodityOwnerType = SelectedCommodityOwnerType;
            CommodityOwner.MaritalStatus = SelectedMaritalStatus;
            CommodityOwner.DateOfBirth = DateOfBirth;
            if (DateOfBirth.Year < 18)
            {
                MessageBox.Show("Farmer must be over 18 years old.", "Agrimanagr: Farmer Management",
                                MessageBoxButton.OK);
                return;
            }
           
            CommodityOwner._SetStatus(EntityStatus.Active);

            if (!IsValid() || !IsValid(CommodityOwner)) return;

            CommodityProducer.CommodityProducerCentres.Clear();
            CommodityProducer.CommodityProducerCentres.AddRange(AssignedCentresList.Select(n => n.Centre));
            if (!IsValid() || !IsValid(CommodityProducer)) return;

            using (var c = NestedContainer)
            {
                string responseMsg = "";
                _proxy = Using<IDistributorServiceProxy>(c);
                ResponseBool response = await _proxy.CommoditySupplierAddAsync(CommoditySupplier);
                responseMsg += response.ErrorInfo + "\n";

                string log = string.Format("Created commodity supplier: {0}; Code: {1}; And Type {2}",
                                           CommoditySupplier.Name,
                                           CommoditySupplier.CostCentreCode, CommoditySupplier.CommoditySupplierType);
                Using<IAuditLogWFManager>(c).AuditLogEntry("Manage Commodity Suppliers", log);


                if (response.Success)
                {ContactsList.Clear();
                    var coResponse = await SaveCommodityOwner();
                    if (coResponse.ErrorInfo != "") responseMsg += coResponse.ErrorInfo + "\n";

                    var cpResponse = await SaveCommodityProducer();
                    if (cpResponse.ErrorInfo != "") responseMsg += cpResponse.ErrorInfo + "\n";
                    var enve = new VMContactItem();
                    var contact = new Contact(Guid.NewGuid());
                    contact.BusinessPhone = CommodityOwner.BusinessNumber;
                    contact.ContactClassification=ContactClassification.PrimaryContact;
                    contact.ContactOwnerMasterId = CommoditySupplier.Id;
                    contact.DateOfBirth = CommodityOwner.DateOfBirth;
                    contact.Email = CommodityOwner.Email;
                    contact.Fax = CommodityOwner.FaxNumber;
                    contact.Firstname = CommodityOwner.FirstName;
                    contact.HomePhone = CommodityOwner.PhoneNumber;
                    contact.JobTitle = "Farmer";
                    contact.Lastname = CommodityOwner.LastName;
                    contact.MStatus =CommodityOwner.MaritalStatus;
                    contact.PhysicalAddress = CommodityOwner.PhysicalAddress;
                    contact.PostalAddress = CommodityOwner.PostalAddress;
                    contact.MobilePhone = CommodityOwner.PhoneNumber;
                    contact.ContactType = SelectedContactOwnerType;
                    
                    enve.IsDirty = true;
                    enve.IsNew = true;
                    enve.Contact = contact;
                    ContactsList.Add(enve);

                    bool success = await SaveContacts(CommoditySupplier);

                    if (!coResponse.Success || !cpResponse.Success || !success)
                        response.Success = false;
                }

                MessageBox.Show(responseMsg, "Agrimanagr: Manage Farmer", MessageBoxButton.OK,
                                MessageBoxImage.Information);

                if (response.Success)
                    SendNavigationRequestMessage(
                        new Uri("views/admin/commodityowners/listcommodityowners.xaml", UriKind.Relative));
            }
        }

        public async Task<ResponseBool> SaveCommodityOwner()
        {
            ResponseBool response = new ResponseBool {Success = false};
            List<string> logs = new List<string>();

            CommodityOwnerItem itemToSave = Map(CommodityOwner);
            string log = string.Format("Created farmer: {0}; Code: {1}; In Account {2}",
                                       CommodityOwner.FullName,
                                       CommodityOwner.Code, CommodityOwner.CommoditySupplier.Name);
            logs.Add(log);
            using (var c = NestedContainer)
            {
                response = await _proxy.CommodityOwnerAddAsync(itemToSave);
                if (response.Success)
                    logs.ForEach(n => Using<IAuditLogWFManager>(c).AuditLogEntry("Farmer Management", n));
            }
            return response;
        }

        CommodityOwnerItem Map(CommodityOwner co)
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

        public async Task<ResponseBool> SaveCommodityProducer()
        {
            ResponseBool response = new ResponseBool {Success = false};

            string log = string.Format("Created farm: {0}; Code: {1}; In Account {2}",
                                       CommodityProducer.Name,
                                       CommodityProducer.Code, CommodityProducer.CommoditySupplier.Name);

            using (var c = NestedContainer)
            {
                response = await _proxy.CommodityProducerAddAsync(CommodityProducer);
                if (response.Success)
                    Using<IAuditLogWFManager>(c).AuditLogEntry("Farm Management", log);
            }
            return response;
        }

        private void Cancel()
        {
            if (MessageBox.Show("Unsaved changes will be lost. Do you want to continue?", "Agrimanagr: Edit farmer details", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                SendNavigationRequestMessage(
                    new Uri("views/admin/commodityowners/listcommodityowners.xaml", UriKind.Relative));
            }
        }

        protected override void AddOrEditContact(Button btnAdd)
        {
            base.AddOrEditContact(btnAdd, CommoditySupplier);
        }

        protected override void EditContact(VMContactItem contactItem)
        {
            base.EditContact(contactItem, CommoditySupplier);
        }

        #endregion

    }
}
