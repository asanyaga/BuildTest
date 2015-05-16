


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.MasterDataDTO.DataContracts;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Admin.AgriUsers.Contacts;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.Contacts
{
    public abstract class MasterEntityContactUtilsViewModel : ListingsViewModelBase //DistributrViewModelBase
    {

        #region Properties
        private IPagenatedList<Contact> _pagedList;
        private IDistributorServiceProxy _proxy;
        private MasterEntity _entity;
        public ObservableCollection<VMContactItem> ContactsList { get; set; }

        private RelayCommand<Button> _addContactClickedCommand;
        public RelayCommand<Button> AddContactClickedCommand
        {
            get { return _addContactClickedCommand ?? (_addContactClickedCommand = new RelayCommand<Button>(AddOrEditContact)); }
        }

        private RelayCommand<VMContactItem> _editContactClickedCommand;
        public RelayCommand<VMContactItem> EditContactClickedCommand
        {
            get { return _editContactClickedCommand ?? (_editContactClickedCommand = new RelayCommand<VMContactItem>(EditContact)); }
        }

        private RelayCommand<VMContactItem> _deleteContactClickedCommand;

        public RelayCommand<VMContactItem> DeleteContactClickedCommand
        {
            get { return _deleteContactClickedCommand ?? (_deleteContactClickedCommand = new RelayCommand<VMContactItem>(DeleteContact)); }
        }

        #endregion


        #region methods

        protected void LoadEntityContacts(MasterEntity entity)
        {
            _entity = entity;
            using (var c = NestedContainer)
            {
                ContactsList.Clear();
                var contacts = Using<IContactRepository>(c).GetByContactsOwnerId(entity.Id, ShowInactive)
                    .Where(n => (n.Firstname.ToLower().Contains(SearchText.ToLower()) ||
                                 n.Lastname.ToLower().Contains(SearchText.ToLower()) ||
                                 n.MobilePhone.ToLower().Contains(SearchText.ToLower()))
                    );
                _pagedList = new PagenatedList<Contact>(contacts.AsQueryable(), CurrentPage, ItemsPerPage, contacts.Count());
                _pagedList.ToList().ForEach(
                    n => ContactsList.Add(new VMContactItem { Contact = n, IsDirty = false, IsNew = false }));
                UpdatePagenationControl();
            }
        }

        protected async Task<bool> SaveContacts(MasterEntity entity)
        {
            using (var c = NestedContainer)
            {
                if(!ContactsList.Any(n => n.IsDirty)) return true;

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

        private void AddContactToList(MasterEntity entity, Contact contact)
        {
            VMContactItem contactItem = new VMContactItem {IsNew = contact._Status == EntityStatus.New, IsDirty = true};
            if (contact.Id == Guid.Empty)
            {
                contact.Id = Guid.NewGuid();
                contact.ContactOwnerType = ContactOwnerType.User;
                if (contact.ContactClassification == ContactClassification.None)
                    contact.ContactClassification = ContactClassification.SecondaryContact;

                if(ContactsList.All(n => n.Contact.ContactClassification != ContactClassification.PrimaryContact))
                    contact.ContactClassification = ContactClassification.PrimaryContact;
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

            contact.ContactOwnerMasterId = entity.Id;
            contactItem.Contact = contact;

            ContactsList.Add(contactItem);
        }

        protected abstract void AddOrEditContact(Button btnAdd);

        protected void AddOrEditContact(Button btnAdd, MasterEntity entity)
        {
            using(var c = NestedContainer)
            {
                Expander expContact = btnAdd.FindParentByType<Expander>();
                if (expContact != null && !expContact.IsExpanded)
                {
                    expContact.IsExpanded = true;
                }
                Contact contact = null;
                if (Using<IEditContactModal>(c).AddUserContact(new Contact(Guid.Empty), out contact))
                    AddContactToList(entity, contact);
            }
        }

        protected abstract void EditContact(VMContactItem contactItem);
        protected void EditContact(VMContactItem contactItem, MasterEntity entity)
        {
            using(var c = NestedContainer)
            {
                Contact contact = null;
                if (Using<IEditContactModal>(c).AddUserContact(contactItem.Contact, out contact))
                    AddContactToList(entity, contact);
            }
        }

        private async void DeleteContact(VMContactItem contact)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                ResponseBool response = null;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.ContactDeleteAsync(contact.Contact.Id);

                MessageBox.Show(response.ErrorInfo, "Agrimanagr: Manage Contacts", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                if (response.Success)
                    ContactsList.Remove(ContactsList.FirstOrDefault(n => n.Contact.Id == contact.Contact.Id));
            }
        }

        protected override void Load(bool isFirstLoad = false)
        {
            LoadEntityContacts(_entity);
        }

        protected override void EditSelected()
        {
            throw new NotImplementedException();
        }

        protected override void ActivateSelected()
        {
            throw new NotImplementedException();
        }

        protected override void DeleteSelected()
        {
            throw new NotImplementedException();
        }

        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, _pagedList.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            LoadEntityContacts(_entity);
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(_pagedList.PageNumber, _pagedList.PageCount, _pagedList.TotalItemCount,
                                      _pagedList.IsFirstPage, _pagedList.IsLastPage);
        }
        #endregion
    }

    public class VMContactItem
    {
        public Contact Contact { get; set; }

        public bool IsPrimaryContact
        {
            get { return this.Contact.ContactClassification == ContactClassification.PrimaryContact; }
            set
            {
                if (value) this.Contact.ContactClassification = ContactClassification.PrimaryContact;
                else
                    this.Contact.ContactClassification = ContactClassification.SecondaryContact;
            }
        }

        public bool IsDirty { get; set; }
        public bool IsNew { get; set; }
    }
}
