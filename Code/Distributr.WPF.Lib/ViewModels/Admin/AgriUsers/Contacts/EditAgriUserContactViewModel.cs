using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.WPF.Lib.ViewModels.Admin.Contacts;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.AgriUsers.Contacts
{
    public class EditAgriUserContactViewModel : DistributrViewModelBase
    {
        public bool setAsPrimContact = false;

        public EditAgriUserContactViewModel()
        {
            ContactTypesList = new ObservableCollection<ContactType>();
            MaritalStatusList = new ObservableCollection<MaritalStatas>();
        }

        #region Properties
        public const string ContactPropertyName = "Contact";
        private Contact _contact = null;
        public Contact Contact
        {
            get { return _contact; }

            internal set
            {
                if (_contact == value)
                {
                    return;
                }

                var oldValue = _contact;
                _contact = value;
                RaisePropertyChanged(ContactPropertyName);
            }
        }

        public ObservableCollection<ContactType> ContactTypesList { get; set; }
        public ObservableCollection<MaritalStatas> MaritalStatusList { get; set; }

        public const string SelectedContactOwnerTypePropertyName = "SelectedContactOwnerType";
        private ContactOwnerType _selectedContactOwnerType = ContactOwnerType.None;
        public ContactOwnerType SelectedContactOwnerType
        {
            get { return _selectedContactOwnerType; }

            set
            {
                //if (_selectedContactOwnerType == value)
                //{
                //    return;
                //}

                var oldValue = _selectedContactOwnerType;
                _selectedContactOwnerType = value;
                RaisePropertyChanged(SelectedContactOwnerTypePropertyName);
            }
        }


        public const string IsPrimaryContactPropertyName = "IsPrimaryContact";
        private bool _isPrimaryContact = false;
        public bool IsPrimaryContact
        {
            get { return _isPrimaryContact; }

            set
            {
                if (_isPrimaryContact == value)
                {
                    return;
                }
                Contact.ContactClassification = value
                                                    ? ContactClassification.PrimaryContact
                                                    : ContactClassification.SecondaryContact;

                var oldValue = _isPrimaryContact;
                _isPrimaryContact = value;
                RaisePropertyChanged(IsPrimaryContactPropertyName);
            }
        }

        public const string ContactTypePropertyName = "SelectedContactType";
        private ContactType _selectedContactType = null;
        public ContactType SelectedContactType
        {
            get { return _selectedContactType; }

            set
            {
                if (_selectedContactType == value)
                {
                    return;
                }
                var oldValue = _selectedContactType;
                _selectedContactType = value;
                RaisePropertyChanged(ContactTypePropertyName);
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

        #endregion

        #region methods

        private void LoadContactTypes()
        {
            ContactTypesList.Clear();
            var contactType = new ContactType(Guid.Empty)
                                  {
                                      Name = GetLocalText("sl.contacts.edit.contactType.default")
                                      /*"--Please Select Contact Type--"*/
                                  };
            ContactTypesList.Add(contactType);
            using (StructureMap.IContainer c = NestedContainer)
            {
                Using<IContactTypeRepository>(c).GetAll().ToList().ForEach(ContactTypesList.Add);
            }
            SelectedContactType = contactType;
        }

        public void Setup(Contact contact)
        {
            Contact = contact.Clone<Contact>();
            LoadContactTypes();
            LoadMaritalStatuses();
            if (Contact.ContactType != null)
            {
                SelectedContactType = ContactTypesList.FirstOrDefault(n => n.Id == Contact.ContactType.Id);
                SelectedMaritalStatus = Contact.MStatus;
            }
        }

        private void LoadMaritalStatuses()
        {
            Type _enumType = typeof (MaritalStatas);
            FieldInfo[] infos;
            infos = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            MaritalStatusList.Clear();
            foreach (FieldInfo fi in infos)
                MaritalStatusList.Add((MaritalStatas) Enum.Parse(_enumType, fi.Name, true));
            SelectedMaritalStatus = MaritalStatusList.FirstOrDefault(n => n == Contact.MStatus);
        }

        public bool IsValid()
        {
            Contact.MStatus = SelectedMaritalStatus;
            Contact.ContactType = _selectedContactType;
            return IsValid(Contact);
        }
        #endregion

    }
}
