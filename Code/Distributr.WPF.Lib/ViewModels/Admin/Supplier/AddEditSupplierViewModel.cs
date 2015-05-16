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
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DataContracts;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CommodityDTOs;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.MasterDataAllocationRepositories;
using Distributr.Core.Utility.Mapping;
using Distributr.Core.Workflow.Impl.AuditLogs;
using Distributr.WPF.Lib.Messages;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.ViewModels.Admin.CommodityProducers;
using Distributr.WPF.Lib.ViewModels.Admin.Contacts;
using GalaSoft.MvvmLight.Command;
using StructureMap;


namespace Distributr.WPF.Lib.ViewModels.Admin.Supplier
{
    public class AddEditSupplierViewModel:DistributrViewModelBase
    {
        private IDistributorServiceProxy _proxy;
        public AddEditSupplierViewModel()
       {
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            AssignSelectedCommand = new RelayCommand(AssignCentre);
            AssignAllCommand = new RelayCommand(AssignAllCentres);
            UnassignSelectedCommand = new RelayCommand(UnassignCentre);
            UnassignAllCommand = new RelayCommand(UnassignAllCentres);
            BankChangedCommand = new RelayCommand(BankChanged);
            
          

            BankLookUpList = new ObservableCollection<Bank>();
            CommodityOwnerTypesList = new ObservableCollection<CommodityOwnerType>();
            ContactOwnerTypesList = new ObservableCollection<ContactType>();
            MaritalStatusList = new ObservableCollection<MaritalStatas>();
            BankBranchLookUpList = new ObservableCollection<BankBranch>();
            CommoditySupplierTypeList = new ObservableCollection<CommoditySupplierType>();

            AssignedCentresList = new ObservableCollection<VMCentreItem>();
            UnassignedCentresList = new ObservableCollection<VMCentreItem>();
            ContactsList = new ObservableCollection<VMContactItem>();
            LoadCommand = new RelayCommand(Setup);
          
        }

        public void SetForEdit(EditSupplierMessage messageTo)
        {
            Id = messageTo.Id;
            CommodityOwnerId = messageTo.CommodityOwnerId;
            CommodityProducerId = messageTo.CommodityProducerId;
            ContactId = messageTo.ContactId;
            IsEdit = true;

        }


        #region Class Members
      
        public ObservableCollection<CommodityOwnerType> CommodityOwnerTypesList { get; set; }
        public ObservableCollection<ContactType> ContactOwnerTypesList { get; set; }
        public ObservableCollection<MaritalStatas> MaritalStatusList { get; set; }
        public ObservableCollection<Bank> BankLookUpList { get; set; }
        public ObservableCollection<BankBranch> BankBranchLookUpList { get; set; }
        public ObservableCollection<VMCentreItem> AssignedCentresList { get; set; }
        public ObservableCollection<VMCentreItem> UnassignedCentresList { get; set; }
        public ObservableCollection<VMContactItem> ContactsList { get; set; }
        public ObservableCollection<CommoditySupplierType> CommoditySupplierTypeList { get; set; }

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand AssignSelectedCommand { get; set; }
        public RelayCommand AssignAllCommand { get; set; }
        public RelayCommand UnassignSelectedCommand { get; set; }
        public RelayCommand UnassignAllCommand { get; set; }
        public RelayCommand BankChangedCommand { get; set; }
        public RelayCommand LoadCommand { get; set; }

        #endregion
        #region Methods

        private void Setup()
        {
            LoadBanks();
            LoadContactType();
            LoadCommodityOwner();
            LoadCentresLists();
            LoadMaritalStatuses();
            LoadCommoditySupplierType();
            ClearViewModel();

            if(IsEdit)
            {
                LoadForEdit();
                IsEdit = false;
            }
            else
            {
                CommodityProducerId = Guid.NewGuid();
                CommodityOwnerId = Guid.NewGuid();
                ContactId = Guid.NewGuid();
                Id = Guid.NewGuid();
              //  DefaultData();
            }
            
        }

        private void ClearViewModel()
        {
            AccountFullName = "";
            AccountName = "";
            AccountNumber = "";
            PinNumber = "";
            CostCenterCode ="";

            SelectedBank = new Bank(Guid.Empty){Name="--Select Bank--"};
            SelectedBankBranch = new BankBranch(Guid.Empty) { Name = "--Select Bank Branch--" };
            SelectedCommoditySupplierType = CommoditySupplierType.Default;
            SelectedCommoditySupplierType = CommoditySupplierType.Individual;
            SelectedMaritalStatus = MaritalStatas.Default;
            SelectedMaritalStatus = MaritalStatas.Single;
            FarmCode = "";
            FarmName ="";
            Acerage = "";
            FarmRegistrationNo = "";
            FarmPhysicalAddress = "";
            FarmDescription = "";

            Surname = "";
            FirstName = "";
            MiddleName = "";
            OwnerCode = "";
            IdNumber = "";
            PinNumber = "";
            PhysicalAddress = "";
            PostalAddress = "";
            Email = "";
            PhoneNumber = "";
            BusinessNumber = "";
            FaxNumber = "";
            OfficeNumber = "";
            Description = "";
            DateOfBirth = DateTime.Now;

            AssignedCentresList.Clear();
            SupplierIsEnabled = true;
            SelectedCommodityOwnerType = new CommodityOwnerType(Guid.Empty){Name="--Select Commodity Type--"};
        }

        private void DefaultData()
        {
                    if(System.Diagnostics.Debugger.IsAttached)
                    {
                        Random rand = new Random();
                        AccountFullName = "Michael Wainaina";
                        FirstName = "Michael";
                        Surname = "Wainaina";
                        MiddleName = "Muthee";
                        IdNumber = rand.Next(1000000,9999999).ToString();
                        PinNumber = rand.Next(1000000, 9999999).ToString();
                        PhysicalAddress = "Marekani";
                        PostalAddress = "P.O Box 5654";
                        Email = "mm@gmail.com";
                        PhoneNumber = rand.Next(1000000, 9999999).ToString();
                        BusinessNumber = rand.Next(1000000, 9999999).ToString();
                        FaxNumber = rand.Next(1000000, 9999999).ToString();
                        OfficeNumber = rand.Next(1000000, 9999999).ToString();
                        Description = "Farmer to watch";
                        FarmCode = rand.Next(1000000, 9999999).ToString();
                        FarmDescription = "Robust shamba";
                        FarmName = "Virtual Farm";
                        FarmPhysicalAddress = "Ruiru,Central";
                        FarmRegistrationNo = rand.Next(1000000, 9999999).ToString();
                        Acerage = "500";
                        SelectedMaritalStatus = MaritalStatas.Single;
                        SelectedCommoditySupplierType = CommoditySupplierType.Individual;

                    }
        }

        private void LoadForEdit()
        {
            SupplierIsEnabled = false;
            using (var c = NestedContainer)
            {
                var supplier = Using<ICommoditySupplierRepository>(c).GetById(Id) as CommoditySupplier;
                if(supplier!=null)
                {
                    AccountName = supplier.AccountName;
                    AccountNumber = supplier.AccountNo;
                    PinNumber = supplier.PinNo;
                    CostCenterCode = supplier.CostCentreCode;
                    AccountFullName = supplier.Name;
                    SelectedBank =BankLookUpList.FirstOrDefault(n=>n.Id==supplier.BankId);
                    BankChanged();
                    SelectedBankBranch = BankBranchLookUpList.FirstOrDefault(y => y.Id == supplier.BankBranchId);
                    SelectedCommoditySupplierType = CommoditySupplierType.Default;
                    SelectedCommoditySupplierType =CommoditySupplierTypeList.FirstOrDefault(y => y == supplier.CommoditySupplierType);

                }
                var producer = Using<ICommodityProducerRepository>(c).GetById(CommodityProducerId);
                if(producer!=null)
                {

                    FarmCode = producer.Code;
                    FarmName = producer.Name;
                    Acerage = producer.Acrage;
                    FarmRegistrationNo = producer.RegNo;
                    FarmPhysicalAddress = producer.PhysicalAddress;
                    FarmDescription = producer.Description;
                    LoadForEditCentre(producer.CommodityProducerCentres.ToList());
                }
                var owner = Using<ICommodityOwnerRepository>(c).GetById(CommodityOwnerId);
                if(owner!=null)
                {
                    Surname = owner.Surname;
                    FirstName = owner.FirstName;
                    MiddleName = owner.LastName;
                    OwnerCode = owner.Code;
                    IdNumber = owner.IdNo;
                    PinNumber = owner.PinNo;
                    PhysicalAddress = owner.PhysicalAddress;
                    PostalAddress = owner.PostalAddress;
                    Email = owner.Email;
                    PhoneNumber = owner.PhoneNumber;
                    BusinessNumber = owner.BusinessNumber;
                    FaxNumber = owner.FaxNumber;
                    OfficeNumber = owner.OfficeNumber;
                    Description = owner.Description;
                    DateOfBirth = owner.DateOfBirth;
                    SelectedMaritalStatus = MaritalStatas.Default;
                    SelectedMaritalStatus = MaritalStatusList.FirstOrDefault(y => y == owner.MaritalStatus);
                    if(owner.CommodityOwnerType!=null)
                    {
                    SelectedCommodityOwnerType =
                        CommodityOwnerTypesList.FirstOrDefault(n => n.Id == owner.CommodityOwnerType.Id);
                    }

                }
                var contact = Using<IContactRepository>(c).GetById(ContactId);
                if (contact != null && contact.ContactType != null)
                {
                    SelectedContactOwnerType = ContactOwnerTypesList.FirstOrDefault(n => n.Id == contact.ContactType.Id);
                    if (owner != null) 
                        contact.BusinessPhone = owner.BusinessNumber;
                }
            }
        }

        private void LoadForEditCentre(IEnumerable<Centre> list)
        {
            AssignedCentresList.Clear();
            UnassignedCentresList.Clear();
            var listOfAssigned = new ObservableCollection<VMCentreItem>();
            using (var c=NestedContainer)
            {
                var allCentres = Using<ICentreRepository>(c).GetAll();
                foreach(var center in allCentres)
                {
                    UnassignedCentresList.Add(new VMCentreItem {Centre = center, IsSelected = false});
                }
            }

            foreach (var l in list)
            {
                var assignedCenter = new VMCentreItem { Centre = l, IsSelected = true };
                AssignedCentresList.Add(assignedCenter);
                if(UnassignedCentresList.Any(n=>n.Centre.Id==l.Id))
                {
                    UnassignedCentresList.Remove(UnassignedCentresList.Single(n => n.Centre.Id == l.Id));
                }
                
            }
        }

        private void BankChanged()
        {  
            BankBranchLookUpList.Clear();
               BankBranchLookUpList.Add(new BankBranch(Guid.Empty) {Name = "--Select Bank Branch--"});
               if (SelectedBank != null)
               {

                   using (var c = ObjectFactory.Container.GetNestedContainer())
                   {

                       var bankBranchRepository = c.GetInstance<IBankBranchRepository>();
                       var bankbranchList = bankBranchRepository.GetByBankMasterId(SelectedBank.Id);
                       bankbranchList.ToList().ForEach(n => BankBranchLookUpList.Add(n));
                      SelectedBankBranch = BankBranchLookUpList.FirstOrDefault();
                   }
               }
        }

        private void AssignCentre()
        {
            var selected = new List<VMCentreItem>();
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
            var selected = new List<VMCentreItem>();
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
            var selected = new List<VMCentreItem>();
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
            var selected = new List<VMCentreItem>();
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

       

        private void Cancel()
        {
            if (MessageBox.Show("Unsaved changes will be lost. Do you want to continue?", "Agrimanagr: Edit farmer details", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                ClearViewModel();
                SendNavigationRequestMessage(
                    new Uri("views/admin/Supplier/ListOfSuppliers.xaml", UriKind.Relative));
            }
        }

        private async void Save()
        {
              var commodityOwner =new CommodityOwner(CommodityOwnerId);
              var commoditySupplier=new CommoditySupplier(Id);
              var  commodityProducer = new CommodityProducer(CommodityProducerId);
              var contact = new Contact(ContactId);

         using (var c = NestedContainer)
            {
                commodityOwner.Code = OwnerCode;
                if (string.IsNullOrEmpty(commodityOwner.Code))
                {
                    MessageBox.Show("Farmer Code is a Required Field", "Agrimanagr: Farmer Management",
                                        MessageBoxButton.OK);
                    return;

                }
                commodityOwner.Surname = Surname;
                commodityOwner.FirstName = FirstName;
                commodityOwner.LastName = MiddleName;
                commodityOwner.IdNo = IdNumber;
                commodityOwner.PinNo = PinNumber;
                //CommodityOwner.Gender =;
                commodityOwner.PhysicalAddress = PhysicalAddress;
                commodityOwner.PostalAddress = PostalAddress;
                commodityOwner.Email = Email;
                commodityOwner.PhoneNumber = PhoneNumber;
                commodityOwner.BusinessNumber = BusinessNumber;
                commodityOwner.FaxNumber = FaxNumber;
                commodityOwner.OfficeNumber = OfficeNumber;
                commodityOwner.Description = Description;
                commodityOwner.DateOfBirth = DateOfBirth;
                commodityOwner.CommoditySupplier = commoditySupplier;
                commodityOwner._Status = EntityStatus.Active;
                if (DateTime.Now.Year - DateOfBirth.Year < 18)
                    {
                        MessageBox.Show("Farmer must be over 18 years old.", "Agrimanagr: Farmer Management",
                                        MessageBoxButton.OK);
                        return;
                    }
            
                commodityOwner.MaritalStatus = SelectedMaritalStatus;
                commodityOwner.CommodityOwnerType = SelectedCommodityOwnerType;

                commoditySupplier.JoinDate = DateTime.Now.Date;
                commoditySupplier.AccountName = AccountName;
                commoditySupplier.AccountNo = AccountNumber;

                commoditySupplier._Status = EntityStatus.Active;
                commoditySupplier.PinNo = PinNumber;
                commoditySupplier.BankId = SelectedBank.Id;
                commoditySupplier.BankBranchId = SelectedBankBranch.Id;
                commoditySupplier.CommoditySupplierType = SelectedCommoditySupplierType;
                commoditySupplier.CostCentreCode = CostCenterCode;
                commoditySupplier.Name = AccountFullName;
                    var config = Using<IConfigService>(c).Load();
                    commoditySupplier.ParentCostCentre = new CostCentreRef {Id = config.CostCentreId};

                if (!IsValid(commoditySupplier))
                    return;

                commodityProducer.CommoditySupplier = commoditySupplier;
                commodityProducer._SetStatus(EntityStatus.Active);
                commodityOwner.BusinessNumber = BusinessNumber;
               if (!IsValid(commodityOwner)) return;

                commodityProducer.CommodityProducerCentres.Clear();
             
             
                commodityProducer.CommodityProducerCentres.AddRange(AssignedCentresList.Select(n => n.Centre));
                commodityProducer.Code = FarmCode;
                if (string.IsNullOrEmpty(commodityProducer.Code))
                {
                    MessageBox.Show("Farm Code is a Required Field", "Agrimanagr: Farmer Management",
                                        MessageBoxButton.OK);
                    return;
                }
                commodityProducer.Acrage = Acerage;
                commodityProducer.Name = FarmName;
                commodityProducer._Status = EntityStatus.Active;
                commodityProducer.RegNo = FarmRegistrationNo;
                commodityProducer.PhysicalAddress = FarmPhysicalAddress;
                commodityProducer.Description = FarmDescription;
                commodityProducer.CommoditySupplier = commoditySupplier;


                contact.BusinessPhone = BusinessNumber;
                contact.Email = Email;
                contact.ContactOwnerMasterId = commoditySupplier.Id;
                contact.Firstname = AccountFullName;
                contact.PostalAddress = PostalAddress;
                contact.MobilePhone = PhoneNumber;
                contact.PhysicalAddress = PhysicalAddress;
                contact.ContactOwnerType=ContactOwnerType.CommoditySupplier;

                if (!IsValid() || !IsValid(commodityProducer)) return;


                   string responseMsg = "";
                   _proxy = Using<IDistributorServiceProxy>(c);
                   var mapper = Using<IMasterDataToDTOMapping>(c);
                    var commoditySupplierdto = mapper.Map(commoditySupplier);
                    ResponseBool response = await _proxy.CommoditySupplierAddAsync(commoditySupplierdto);
                    responseMsg += response.ErrorInfo + "\n";

                    string log = string.Format("Created commodity supplier: {0}; Code: {1}; And Type {2}",
                                               commoditySupplier.Name,
                                               commoditySupplier.CostCentreCode, commoditySupplier.CommoditySupplierType);
                    Using<IAuditLogWFManager>(c).AuditLogEntry("Manage Commodity Suppliers", log);


                    if (response.Success)
                    {
                        ContactsList.Clear();
                        var coResponse = await SaveCommodityOwner(commodityOwner);
                        if (coResponse.ErrorInfo != "") responseMsg += coResponse.ErrorInfo + "\n";

                        var contactResponse = await SaveSupplierContact(contact);
                        if (contactResponse.ErrorInfo != "") responseMsg += contactResponse.ErrorInfo + "\n";

                        var cpResponse = await SaveCommodityProducer(commodityProducer);
                        if (cpResponse.ErrorInfo != "") responseMsg += cpResponse.ErrorInfo + "\n";

                       

                        if (!coResponse.Success || !cpResponse.Success)
                            response.Success = false;
                    }
                    if (response.Success)
                    {
                          responseMsg = "Farmer Successfully Added ";
                        MessageBox.Show(responseMsg, "Agrimanagr: Manage Farmer", MessageBoxButton.OK,
                                    MessageBoxImage.Information);

                         SendNavigationRequestMessage(
                            new Uri("views/admin/Supplier/ListOfSuppliers.xaml", UriKind.Relative));
                    }
                    else
                    {
                        MessageBox.Show(responseMsg, "Agrimanagr: Manage Farmer", MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                    }

            
                       
            }
        }

        private async Task<ResponseBool> SaveSupplierContact(Contact contact)
        {
             var contactItems = new List<ContactItem>();
            var contactItem = new ContactItem();
            string responseMsg = string.Empty;
            var response = new ResponseBool { Success = false };


            using(var c=NestedContainer)
            {
              
                contactItem.MasterId =contact.Id;
                contactItem.Firstname = contact.Firstname;
                contactItem.BusinessPhone = contact.BusinessPhone;
                contactItem.ContactOwnerMasterId =contact.ContactOwnerMasterId;
                contactItem.MobilePhone = contact.MobilePhone;
                contactItem.ContactOwnerType = contact.ContactOwnerType;
                contactItem.Email = contact.Email;
                contactItem.PostalAddress = contact.PostalAddress;
                contactItem.PhysicalAddress = contact.PhysicalAddress;
                contactItem.IsNew = true;
            }
            
            contactItems.Add(contactItem);

           using(var c=NestedContainer)
           {
               response = await Using<IDistributorServiceProxy>(c).ContactsAddAsync(contactItems.ToList());
               return response;
           }
        }


        public async Task<ResponseBool> SaveCommodityProducer(CommodityProducer commodityProducer)
        {
            ResponseBool response = new ResponseBool { Success = false };

            string log = string.Format("Created farm: {0}; Code: {1}; In Account {2}",
                                       commodityProducer.Name,
                                       commodityProducer.Code, commodityProducer.CommoditySupplier.Name);

            using (var c = NestedContainer)
            {
                var mapper = Using<IMasterDataToDTOMapping>(c);
                var commodityProducerDto = mapper.Map(commodityProducer);
                response = await _proxy.CommodityProducerAddAsync(commodityProducerDto);
                if (response.Success)
                {
                    ChangeAllocation(commodityProducerDto);
                     Using<IAuditLogWFManager>(c).AuditLogEntry("Farm Management", log);
                }
                   
            }
            return response;
        }

        private void ChangeAllocation(CommodityProducerDTO dto)
        {
             CommodityProducer entity = new CommodityProducer(dto.MasterId);
             using (var container = NestedContainer)
             {
                 var centreRepository = Using<ICentreRepository>(container);
                 //var _commodityProducerRepository = Using<ICommodityProducerRepository>(container);
                 var masterDataAllocationRepository = Using<IMasterDataAllocationRepository>(container);
                 foreach (var centreId in dto.CenterIds)
                 {
                     entity.CommodityProducerCentres.Add(centreRepository.GetById(centreId));
                 }





                 try
                 {
                     //_commodityProducerRepository.Save(entity);
                     var existingAllocationForThisProducer = masterDataAllocationRepository.GetByAllocationType(
                         MasterDataAllocationType.CommodityProducerCentreAllocation)
                         .Where(n => n.EntityAId == entity.Id);

                     var unallocated =
                         existingAllocationForThisProducer.Where(
                             n =>
                             entity.CommodityProducerCentres.Select(c => c.Id).All(cId => n.EntityBId != cId));

                     //foreach (var centre in entity.CommodityProducerCentres)
                     //{
                     //    var allocation = new MasterDataAllocation(Guid.NewGuid())
                     //                         {
                     //                             _Status = EntityStatus.Active,
                     //                             AllocationType =
                     //                                 MasterDataAllocationType.CommodityProducerCentreAllocation,
                     //                             EntityAId = entity.Id,
                     //                             EntityBId = centre.Id
                     //                         };
                     //    _masterDataAllocationRepository.Save(allocation);
                     //}

                     foreach (var allocation in unallocated)
                     {
                         masterDataAllocationRepository.DeleteAllocation(allocation.Id);
                     }
                 }
                 catch (Exception)
                 {
                     //throw ;
                     //response.ErrorInfo = "Error: An error occurred when saving the commodity producer.\n" +
                     //                        ex.ToString();
                 }
             }
        }


        public async Task<ResponseBool> SaveCommodityOwner(CommodityOwner commodityOwner)
        {
            ResponseBool response = new ResponseBool { Success = false };
            List<string> logs = new List<string>();
           
            //var itemToSave = Map(commodityOwner);
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

        protected async Task<bool> SaveContacts(MasterEntity entity)
        {
            using (var c = NestedContainer)
            {
                if (!ContactsList.Any(n => n.IsDirty)) return true;

                _proxy = Using<IDistributorServiceProxy>(c);
                ResponseBool response = new ResponseBool { Success = false };
                var itemsToSave = new List<ContactItem>();

                foreach (var item in ContactsList.Where(n => n.IsDirty))
                {
                    var contactItem = new ContactItem
                    {
                        MasterId = item.Contact.Id,
                        DateOfBirth = item.Contact.DateOfBirth,
                        MaritalStatusMasterId = (int)item.Contact.MStatus,
                        BusinessPhone = item.Contact.BusinessPhone,
                        ChildrenNames = item.Contact.ChildrenNames,
                        City = item.Contact.City,
                        Company = item.Contact.Company,
                        ContactClassification = (int)item.Contact.ContactClassification,
                        ContactOwnerType = item.Contact.ContactOwnerType,
                        ContactOwnerMasterId = entity.Id,
                        DateCreated = item.Contact._DateCreated,
                        Email = item.Contact.Email,
                        Fax = item.Contact.Fax,
                        Firstname = item.Contact.Firstname,
                        HomePhone = item.Contact.HomePhone,
                        HomeTown = item.Contact.HomeTown,
                        JobTitle = item.Contact.JobTitle,
                        Lastname = item.Contact.Lastname,
                        MobilePhone = item.Contact.MobilePhone,
                        PhysicalAddress = item.Contact.PhysicalAddress,
                        PostalAddress = item.Contact.PostalAddress,
                        SpouseName = item.Contact.SpouseName,
                        WorkExtPhone = item.Contact.WorkExtPhone,
                        DateLastUpdated = DateTime.Now,
                        StatusId = (int)EntityStatus.Active,
                        IsNew = item.IsNew
                    };
                    if (item.Contact.ContactType != null) contactItem.ContactTypeMasterId = item.Contact.ContactType.Id;
                    itemsToSave.Add(contactItem);
                }
                if (itemsToSave.Count > 0)
                {
                    response = await _proxy.ContactsAddAsync(itemsToSave.ToList());
                    MessageBox.Show(response.ErrorInfo, "Agrimanagr: Manage contacts", MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
                else response.Success = true;

                return response.Success;
            }
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

        //protected override void AddOrEditContact(Button btnAdd)
        //{
        //    base.AddOrEditContact(btnAdd, CommoditySupplier);
        //}

        //protected override void EditContact(VMContactItem contactItem)
        //{
        //    base.EditContact(contactItem, CommoditySupplier);
        //}

        private void LoadBanks()
        {
            using(var c=ObjectFactory.Container.GetNestedContainer())
            {
                BankLookUpList.Clear();
                BankLookUpList.Add(new Bank(Guid.Empty) {Name = "--Select Bank--"});
                var bankRepository = c.GetInstance<IBankRepository>();
                var bankList = bankRepository.GetAll();
                bankList.ToList().ForEach(n => BankLookUpList.Add(n));
                SelectedBank = BankLookUpList.FirstOrDefault();

                BankBranchLookUpList.Clear();
                BankBranchLookUpList.Add(new BankBranch(Guid.Empty) { Name = "--Select Bank Branch--" });
                SelectedBankBranch = BankBranchLookUpList.FirstOrDefault();
            }
        }

        private void LoadContactType()
        {
            using(var c=ObjectFactory.Container.GetNestedContainer())
            {
                ContactOwnerTypesList.Clear();
                ContactOwnerTypesList.Add(new ContactType(Guid.Empty) {Name = "--Select Contact Type--"});
                var contactTypeRepository = c.GetInstance<IContactTypeRepository>();
                var contactTypeList = contactTypeRepository.GetAll();
                contactTypeList.ToList().ForEach(n => ContactOwnerTypesList.Add(n));

                SelectedContactOwnerType = ContactOwnerTypesList.FirstOrDefault();
            }
        }

        private void LoadCentresLists()
        {
            AssignedCentresList.Clear();
            UnassignedCentresList.Clear();

            using (var c = NestedContainer)
            {
                var allCentres = Using<ICentreRepository>(c).GetAll();
                allCentres.Where(n => AssignedCentresList.All(p => p.Centre.Id != n.Id)).OrderBy(
                    n => n.Name).ToList().ForEach
                    (n => UnassignedCentresList.Add(new VMCentreItem { Centre = n, IsSelected = false }));
            }
        }

        private void LoadMaritalStatuses()
        {
            MaritalStatusList.Clear();
            Type _enumType = typeof (MaritalStatas);
            FieldInfo[] infos;
            infos = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
           // MaritalStatusList.Clear();

            foreach (FieldInfo fi in infos)
                MaritalStatusList.Add((MaritalStatas) Enum.Parse(_enumType, fi.Name, true));

            MaritalStatusList.Remove(MaritalStatas.Default);


        }

        private void LoadCommoditySupplierType()
        {
            CommoditySupplierTypeList.Clear();
            Type _enumType = typeof(CommoditySupplierType);
            FieldInfo[] infos;
            infos = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            // MaritalStatusList.Clear();

            foreach (FieldInfo fi in infos)
                CommoditySupplierTypeList.Add((CommoditySupplierType)Enum.Parse(_enumType, fi.Name, true));

            CommoditySupplierTypeList.Remove(CommoditySupplierType.Default);

        }

        //Type of Farmer
        private void LoadCommodityOwner()
        {
            using(var c=ObjectFactory.Container.GetNestedContainer())
            {
                CommodityOwnerTypesList.Clear();
                CommodityOwnerTypesList.Add(new CommodityOwnerType(Guid.Empty) {Name = "--Select Type of Farmer--"});
                var commodityOwnerTypeRepository = c.GetInstance<ICommodityOwnerTypeRepository>();
                var commodityOwnerTypeList = commodityOwnerTypeRepository.GetAll();
                commodityOwnerTypeList.ToList().ForEach(n => CommodityOwnerTypesList.Add(n));
                SelectedCommodityOwnerType = CommodityOwnerTypesList.FirstOrDefault();
            }
        }

        

        #endregion

        #region properties

        
        public const string AssignedCodePropertyName = "AssignedCode";
        private string _assignedCode = "";
        public string AssignedCode
        {
            get
            {
                return _assignedCode;
            }

            set
            {
                if (_assignedCode == value)
                {
                    return;
                }

                RaisePropertyChanging(AssignedCodePropertyName);
                _assignedCode = value;
                RaisePropertyChanged(AssignedCodePropertyName);
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

        public const string CommodityProducerIdPropertyName = "CommodityProducerId";
        private Guid _commodityProducerId = Guid.Empty;
        public Guid CommodityProducerId
        {
            get
            {
                return _commodityProducerId;
            }

            set
            {
                if (_commodityProducerId == value)
                {
                    return;
                }

                RaisePropertyChanging(CommodityProducerIdPropertyName);
                _commodityProducerId = value;
                RaisePropertyChanged(CommodityProducerIdPropertyName);
            }
        }

        public const string CommodityOwnerIdPropertyName = "CommodityOwnerId";
        private Guid _commodityOwnerId = Guid.Empty;
        public Guid CommodityOwnerId
        {
            get
            {
                return _commodityOwnerId;
            }

            set
            {
                if (_commodityOwnerId == value)
                {
                    return;
                }

                RaisePropertyChanging(CommodityOwnerIdPropertyName);
                _commodityOwnerId = value;
                RaisePropertyChanged(CommodityOwnerIdPropertyName);
            }
        }
        
        public const string IdPropertyName = "Id";
        private Guid _id =Guid.Empty;
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

        
        public const string MiddleNamePropertyName = "MiddleName";
        private string _middleName = "";
        public string MiddleName
        {
            get
            {
                return _middleName;
            }

            set
            {
                if (_middleName == value)
                {
                    return;
                }

                RaisePropertyChanging(MiddleNamePropertyName);
                _middleName = value;
                RaisePropertyChanged(MiddleNamePropertyName);
            }
        }

        
        public const string OwnerCodePropertyName = "OwnerCode";
        private string _ownerCode = "";
        public string OwnerCode
        {
            get
            {
                return _ownerCode;
            }

            set
            {
                if (_ownerCode == value)
                {
                    return;
                }

                RaisePropertyChanging(OwnerCodePropertyName);
                _ownerCode = value;
                RaisePropertyChanged(OwnerCodePropertyName);
            }
        }

        public const string CostCenterCodePropertyName = "CostCenterCode";
        private string _costCenterCode = "";
        public string CostCenterCode
        {
            get
            {
                return _costCenterCode;
            }

            set
            {
                if (_costCenterCode == value)
                {
                    return;
                }

                RaisePropertyChanging(CostCenterCodePropertyName);
                _costCenterCode = value;
                RaisePropertyChanged(CostCenterCodePropertyName);
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
        private MaritalStatas _maritalStatus=MaritalStatas.Single;
        public MaritalStatas SelectedMaritalStatus
        {
            get
            {
                return _maritalStatus;
            }

            set
            {
                if (_maritalStatus == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedMaritalStatusPropertyName);
                _maritalStatus = value;
                RaisePropertyChanged(SelectedMaritalStatusPropertyName);
            }
        }

        
        public const string SelectedCommodityOwnerTypePropertyName = "SelectedCommodityOwnerType";
        private CommodityOwnerType _selectedCommnodityOwnerType = new CommodityOwnerType(Guid.NewGuid());
        public CommodityOwnerType SelectedCommodityOwnerType
        {
            get
            {
                return _selectedCommnodityOwnerType;
            }

            set
            {
                if (_selectedCommnodityOwnerType == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCommodityOwnerTypePropertyName);
                _selectedCommnodityOwnerType = value;
                RaisePropertyChanged(SelectedCommodityOwnerTypePropertyName);
            }
        }

        
        public const string SelectedBankPropertyName = "SelectedBank";
        private Bank _selectedBank = new Bank(Guid.Empty){Name = "--Select Bank---"};
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
        private BankBranch _selectedBankBranch = new BankBranch(Guid.Empty){Name = "--Select Bank Branch--"};
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

        
        public const string PostalAddressPropertyName = "PostalAddress";
        private string _postalAddres = "";
        public string PostalAddress
        {
            get
            {
                return _postalAddres;
            }

            set
            {
                if (_postalAddres == value)
                {
                    return;
                }

                RaisePropertyChanging(PostalAddressPropertyName);
                _postalAddres = value;
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
        private string _officeNumber ="";
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

        
        public const string SelectedContactOwnerTypePropertyName = "SelectedContactOwnerType";
        private ContactType _selectedContactOwnerType = new ContactType(Guid.NewGuid());
        public ContactType SelectedContactOwnerType
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

                RaisePropertyChanging(SelectedContactOwnerTypePropertyName);
                _selectedContactOwnerType = value;
                RaisePropertyChanged(SelectedContactOwnerTypePropertyName);
            }
        }

        
        public const string AccountFullNamePropertyName = "AccountFullName";
        private string _accountFullName = "";
        public string AccountFullName
        {
            get
            {
                return _accountFullName;
            }

            set
            {
                if (_accountFullName == value)
                {
                    return;
                }

                RaisePropertyChanging(AccountFullNamePropertyName);
                _accountFullName = value;
                RaisePropertyChanged(AccountFullNamePropertyName);
            }
        }

        public const string FarmNamePropertyName = "FarmName";
        private string _farmName ="";
        public string FarmName
        {
            get
            {
                return _farmName;
            }

            set
            {
                if (_farmName == value)
                {
                    return;
                }

                RaisePropertyChanging(FarmNamePropertyName);
                _farmName = value;
                RaisePropertyChanged(FarmNamePropertyName);
            }
        }

        
        public const string FarmCodePropertyName = "FarmCode";
        private string _farmCode = "";
        public string FarmCode
        {
            get
            {
                return _farmCode;
            }

            set
            {
                if (_farmCode == value)
                {
                    return;
                }

                RaisePropertyChanging(FarmCodePropertyName);
                _farmCode = value;
                RaisePropertyChanged(FarmCodePropertyName);
            }
        }

        
        public const string FarmRegistrationNoPropertyName = "FarmRegistrationNo";
        private string _farmRegistrationNo = "";
        public string FarmRegistrationNo
        {
            get
            {
                return _farmRegistrationNo;
            }

            set
            {
                if (_farmRegistrationNo == value)
                {
                    return;
                }

                RaisePropertyChanging(FarmRegistrationNoPropertyName);
                _farmRegistrationNo = value;
                RaisePropertyChanged(FarmRegistrationNoPropertyName);
            }
        }

        
        public const string AceragePropertyName = "Acerage";
        private string _acerage = "";
        public string Acerage
        {
            get
            {
                return _acerage;
            }

            set
            {
                if (_acerage == value)
                {
                    return;
                }

                RaisePropertyChanging(AceragePropertyName);
                _acerage = value;
                RaisePropertyChanged(AceragePropertyName);
            }
        }

        
        public const string FarmPhysicalAddressPropertyName = "FarmPhysicalAddress";
        private string _farmPhysicalAddress = "";
        public string FarmPhysicalAddress
        {
            get
            {
                return _farmPhysicalAddress;
            }

            set
            {
                if (_farmPhysicalAddress == value)
                {
                    return;
                }

                RaisePropertyChanging(FarmPhysicalAddressPropertyName);
                _farmPhysicalAddress = value;
                RaisePropertyChanged(FarmPhysicalAddressPropertyName);
            }
        }

        
        public const string FarmDescriptionPropertyName = "FarmDescription";
        private string _farmDescription = "";
        public string FarmDescription
        {
            get
            {
                return _farmDescription;
            }

            set
            {
                if (_farmDescription == value)
                {
                    return;
                }

                RaisePropertyChanging(FarmDescriptionPropertyName);
                _farmDescription = value;
                RaisePropertyChanged(FarmDescriptionPropertyName);
            }
        }

        
        public const string SelectedCommoditySupplierTypePropertyName = "SelectedCommoditySupplierType";
        private CommoditySupplierType _selectedCommoditySupplierType = CommoditySupplierType.Individual;
        public CommoditySupplierType SelectedCommoditySupplierType
        {
            get
            {
                return _selectedCommoditySupplierType;
            }

            set
            {
                if (_selectedCommoditySupplierType == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCommoditySupplierTypePropertyName);
                _selectedCommoditySupplierType = value;
                RaisePropertyChanged(SelectedCommoditySupplierTypePropertyName);
            }
        }


        
        public const string SupplierIsEnabledPropertyName = "SupplierIsEnabled";
        private bool _supplierIsEnabled = true;
        public bool SupplierIsEnabled
        {
            get
            {
                return _supplierIsEnabled;
            }

            set
            {
                if (_supplierIsEnabled == value)
                {
                    return;
                }

                RaisePropertyChanging(SupplierIsEnabledPropertyName);
                _supplierIsEnabled = value;
                RaisePropertyChanged(SupplierIsEnabledPropertyName);
            }
        }
        #endregion
    }
}
