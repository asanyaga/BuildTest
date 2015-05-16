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
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.MasterDataDTO.DataContracts;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.UI.Pages;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.Contacts
{
    public class AgriListContactsViewModel : ListingsViewModelBase
    {
        private PagenatedList<Contact> _pagedList;
        private IDistributorServiceProxy _proxy;
        public AgriListContactsViewModel()
        {
            ContactsList = new ObservableCollection<VMContactItem>();
            ContactOwnerTypesList = new ObservableCollection<ContactOwnerType>();
            ContactOwnersList = new ObservableCollection<VMContactOwnerItem>();
            ContactOwnerTypeChanged = new RelayCommand(LoadContactOwnersList);
        }

        public RelayCommand ContactOwnerTypeChanged { get; set; }

        public ObservableCollection<VMContactItem> ContactsList { get; set; }
        public ObservableCollection<ContactOwnerType> ContactOwnerTypesList { get; set; }
        public ObservableCollection<VMContactOwnerItem> ContactOwnersList { get; set; }
         
        public const string SelectedContactOwnerPropertyName = "SelectedContactOwner";
        private VMContactOwnerItem _selectedContactOwner = null;
        public VMContactOwnerItem SelectedContactOwner
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

                RaisePropertyChanging(SelectedContactOwnerPropertyName);
                _selectedContactOwner = value;
                RaisePropertyChanged(SelectedContactOwnerPropertyName);
            }
        } 

        public const string SelectedContactOwnerTypePropertyName = "SelectedContactOwnerType";
        private ContactOwnerType _selectedContactOwnerType = ContactOwnerType.User;
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

                RaisePropertyChanging(SelectedContactOwnerTypePropertyName);
                _selectedContactOwnerType = value;
                RaisePropertyChanged(SelectedContactOwnerTypePropertyName);
            }
        }

        public const string SelectedContactPropertyName = "SelectedContact";
        private VMContactItem _selectedContact = null;
        public VMContactItem SelectedContact
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

                RaisePropertyChanging(SelectedContactPropertyName);
                _selectedContact = value;
                RaisePropertyChanged(SelectedContactPropertyName);
            }
        }

        private VMContactOwnerItem _defaultSelectedContactOwner;

        public VMContactOwnerItem DefaultSelectedContactOwner
        {
            get
            {
                return _defaultSelectedContactOwner ??
                       (_defaultSelectedContactOwner = new VMContactOwnerItem {Id =Guid.Empty, Name = "--Select conact owner--"});
            }
        }
        protected override void Load(bool isFirstLoad = false)
        {
            using (var c = NestedContainer)
            {
                if (isFirstLoad)
                    Setup();

                ContactsList.Clear();
                List<Contact> contacts = new List<Contact>();
                if (SelectedContactOwner != null && SelectedContactOwner.Id != Guid.Empty)
                {
                    contacts = Using<IContactRepository>(c).GetByContactsOwnerId(SelectedContactOwner.Id);
                }
                else
                {
                    contacts = Using<IContactRepository>(c).GetAll(ShowInactive).ToList();
                }
                contacts = contacts
                    .Where(n => n.Firstname.ToLower().Contains(SearchText.ToLower()) ||
                                n.Lastname.ToLower().Contains(SearchText.ToLower()) ||
                                n.MobilePhone.ToLower().Contains(SearchText.ToLower()) ||
                                n.PhysicalAddress.ToLower().Contains(SearchText.ToLower()))
                    .OrderBy(n => n.Firstname).ThenBy(n => n.Lastname)
                    .ToList();

                _pagedList = new PagenatedList<Contact>(contacts.AsQueryable(), CurrentPage, ItemsPerPage,
                                                        contacts.Count);
                contacts.ForEach(
                    n => ContactsList.Add(new VMContactItem { Contact = n, IsDirty = false, IsNew = false }));
            }
        }

        private void Setup()
        {
            LoadContactOwnerTypesList();
        }

        private void LoadContactOwnersList()
        {
            using (var c = NestedContainer)
            {
                if (SelectedContactOwnerType == ContactOwnerType.User)
                {
                    var users = Using<IUserRepository>(c).GetAll().OrderBy(n => n.Username).ToList();
                    ContactOwnersList.Clear();
                    users.ForEach(n => ContactOwnersList.Add(Map(n)));
                }
                else if (SelectedContactOwnerType == ContactOwnerType.Distributor)
                {
                    var dists = Using<ICostCentreRepository>(c).GetAll().OrderBy(n => n.Name).ToList();
                    ContactOwnersList.Clear();
                    dists.ForEach(n => ContactOwnersList.Add(Map(n)));
                }
                else if (SelectedContactOwnerType == ContactOwnerType.None)
                {
                    ContactOwnersList.Clear();
                }
         
                ContactOwnersList.Add(DefaultSelectedContactOwner);
                SelectedContactOwner = DefaultSelectedContactOwner;
            }
        }

        VMContactOwnerItem Map(MasterEntity entity)
        {
            if(entity is User)
            {
                return new VMContactOwnerItem() {Id = entity.Id, Name = ((User) entity).Username};
            }
            else if(entity is CostCentre)
            {
                return new VMContactOwnerItem {Id = entity.Id, Name = ((CostCentre) entity).Name};
            }
            return null;
        }

        private void LoadContactOwnerTypesList()
        {
            Type _enumType = typeof(ContactOwnerType);
            ContactOwnerTypesList.Clear();
            FieldInfo[] infos;
            infos = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo fi in infos)
                ContactOwnerTypesList.Add((ContactOwnerType)Enum.Parse(_enumType, fi.Name, true));
            SelectedContactOwnerType = ContactOwnerType.User;
        }

        protected override void EditSelected()
        {
            using (var c = NestedContainer)
            {
                Contact contact = null;
                if (Using<IEditContactModal>(c).AddUserContact(SelectedContact.Contact, out contact))
                {
                    var contactItem = AddContactToList(SelectedContact.Contact.ContactOwnerMasterId, contact);
                    SaveContacts(SelectedContact.Contact.ContactOwnerMasterId, contactItem);
                }
            }
        }

        private VMContactItem AddContactToList(Guid entityId, Contact contact)
        {
            VMContactItem contactItem = new VMContactItem();
            if (contact.Id == Guid.Empty)
            {
                contact.Id = Guid.NewGuid();
                contactItem.IsNew = true;
                contact.ContactOwnerType = ContactOwnerType.User;
            }

            if (contact.ContactClassification == ContactClassification.PrimaryContact)
            {
                foreach (var cont in ContactsList.Where(n => n.IsPrimaryContact))
                {
                    cont.Contact.ContactClassification = ContactClassification.SecondaryContact;
                    cont.IsDirty = true;
                }
            }

            var existing = ContactsList.FirstOrDefault(n => n.Contact.Id == contact.Id);
            if (existing != null)
                ContactsList.Remove(existing);

            contact.ContactOwnerMasterId = entityId;
            contactItem.Contact = contact;
            contactItem.IsDirty = true;

            ContactsList.Add(contactItem);
            return contactItem;
        }

        protected async override void ActivateSelected()
        {
            if (SelectedContact == null) return;
            if (SelectedContact.Contact._Status == EntityStatus.Inactive)
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    ResponseBool response = null;
                    IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                    response = await proxy.ContactActivateAsync(SelectedContact.Contact.Id);

                    MessageBox.Show(response.ErrorInfo, "Agrimanagr: Manage Contacts", MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
            }
            else if (SelectedContact.Contact._Status == EntityStatus.Inactive)
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    ResponseBool response = null;
                    IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                    response = await proxy.ContactDeleteAsync(SelectedContact.Contact.Id);

                    MessageBox.Show(response.ErrorInfo, "Agrimanagr: Manage Contacts", MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
            }
        }

        protected async override void DeleteSelected()
        {
            if (SelectedContact == null) return;
            using (StructureMap.IContainer c = NestedContainer)
            {
                ResponseBool response = null;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.ContactDeleteAsync(SelectedContact.Contact.Id);

                MessageBox.Show(response.ErrorInfo, "Agrimanagr: Manage Contacts", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                if (response.Success)
                    ContactsList.Remove(ContactsList.FirstOrDefault(n => n.Contact.Id == SelectedContact.Contact.Id));
            }
        }

        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, _pagedList.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            Load();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(_pagedList.PageNumber, _pagedList.PageCount, _pagedList.TotalItemCount,
                                        _pagedList.IsFirstPage, _pagedList.IsLastPage);
        }

        protected async Task<bool> SaveContacts(Guid entityId, VMContactItem item)
        {
            using (var c = NestedContainer)
            {
                _proxy = Using<IDistributorServiceProxy>(c);
                ResponseBool response = null;
                var itemsToSave = new List<ContactItem>();
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
                        ContactOwnerMasterId = entityId,
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

                response = await _proxy.ContactsAddAsync(itemsToSave.ToList());
                MessageBox.Show(response.ErrorInfo, "Agrimanagr: Manage contacts", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return response.Success;
            }
        }
    }

    public class VMContactOwnerItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
