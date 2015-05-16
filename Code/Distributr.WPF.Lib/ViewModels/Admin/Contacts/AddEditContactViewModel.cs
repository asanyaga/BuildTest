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
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.MasterDataDTO.DataContracts;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility.Validation;
using Distributr.Core.Workflow.Impl.AuditLogs;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.UI.Pages;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.Contacts
{
  public class AddEditContactViewModel : DistributrViewModelBase
  {
      public AddEditContactViewModel()
      {
          //Commands Instantiation
          SaveCommand = new RelayCommand(Save);
          CancelCommand = new RelayCommand(CancelAll);
          ComboSelectionChangedCommand = new RelayCommand<object>(ComboSelectionChanged);
          ContactOwnerTypeDropDownOpenedCommand = new RelayCommand(ContactOwnerTypeDropDownOpened);
          MaritalStatusDropDownOpenedCommand = new RelayCommand(MaritalStatusDropDownOpened);
          ContactTypeDropDownOpenedCommand = new RelayCommand(ContactTypeDropDownOpened);


          //Collections Instantiation

          ContactOwnerTypes = new ObservableCollection<ContactOwnerType>();
          ContactOwners = new ObservableCollection<ContactOwnerLookUp>();
          ContactClassifications = new ObservableCollection<ContactClassification>();
          ContactTypes = new ObservableCollection<ContactType>();
          MaritalStatuses = new ObservableCollection<MaritalStatas>();
      }

      #region Commands and Collections

      public RelayCommand SaveCommand { get; set; }
      public RelayCommand ClearViewModelCommand { get; set; }
      public RelayCommand<object> ComboSelectionChangedCommand { get; set; }
      public RelayCommand CancelCommand { get; set; }
      public RelayCommand ContactOwnerTypeDropDownOpenedCommand { get; set; }
      public RelayCommand MaritalStatusDropDownOpenedCommand { get; set; }
      public RelayCommand ContactTypeDropDownOpenedCommand { get; set; }

      public ObservableCollection<ContactOwnerType> ContactOwnerTypes { get; set; }
      public ObservableCollection<ContactOwnerLookUp> ContactOwners { get; set; }
      public ObservableCollection<MaritalStatas> MaritalStatuses { get; set; }
      public ObservableCollection<ContactType> ContactTypes { get; set; }
      public ObservableCollection<ContactClassification> ContactClassifications { get; set; }
#endregion

   

      #region Methods


      public void ContactTypeDropDownOpened()
      {
          using (var container = NestedContainer)
          {
              SelectedContacttype = Using<IItemsLookUp>(container).SelectContactType();//??default;

              if (SelectedContacttype != null)
              { ContactTypeName = SelectedContacttype.Name; }
              else
              {
                  if (MessageBox.Show("Enter Contact Type", "Distributr", MessageBoxButton.YesNo,
                                   MessageBoxImage.Question) == MessageBoxResult.Yes)
                  {
                      ContactTypeDropDownOpened();
                  }
              }
          }
      }


      private void MaritalStatusDropDownOpened()
      {
          using (var container = NestedContainer)
          {
              SelectedMaritalStatus = Using<IItemsEnumLookUp>(container).SelectMaritalStatus();//??default;
              MaritalStatusesName = SelectedMaritalStatus.ToString();

          }
      }

      private void ContactOwnerTypeDropDownOpened()
      {
          using (var container = NestedContainer)
          {
              SelectedContactOwnerType = Using<IItemsEnumLookUp>(container).SelectContactOwnerType();//??default;
              ContactOwnerTypeName = SelectedContactOwnerType.ToString();

          }
      }

      private void ComboSelectionChanged(object sender)
      {

          ComboBox combo = sender as ComboBox;
          switch (combo.Name)
          {
              case "cmbContactOwnerType":
                  LoadContactOwners();
                  break;
              case "cmbContactOwner":
                  SetDefaultContactClassification();
                  // LoadContactOwners();
                  break;
          }

      }

      protected override void LoadPage(Page page)
      {
          SetUp();
          if (page != null)
          {
              if (page.NavigationService != null)
                  Id = PresentationUtility.ParseIdFromUrl(page.NavigationService.CurrentSource);
              if (Id != Guid.Empty)
                  LoadForContactEdit();
          }
      }




      void SetUp()
      {
          ClearViewModel();
          using (var c=NestedContainer)
          {
              CanEdit = Using<IConfigService>(c).ViewModelParameters.CurrentUserRights.CanManageContacts;
              LoadContactOwnerTypes();
              LoadContactClassifications();
              LoadMaritalStatuses();
              LoadContactTypes();
              PageTitle = GetLocalText("sl.contacts.edit.title.new"); //"Create Contact";
              SelectedContactOwner = new ContactOwnerLookUp()
              {
                  Id = Guid.Empty,
                  Name = GetLocalText("sl.contacts.edit.contactOwner.default")
                  /*"--Please Select Contact Owner--"*/
              };
              ContactOwners.Add(SelectedContactOwner);

          }
          SelectedContactOwnerType = ContactOwnerType.Distributor;
          SelectedMaritalStatus=MaritalStatas.Unknown;
         
      }
      async void Save()
      {
          if (!IsValid())
              return;

          using (StructureMap.IContainer c = NestedContainer)
          {

              ResponseBool response = null;
              var proxy = Using<IDistributorServiceProxy>(c);

              ContactItem contactItem = null;
              var contactItems = new List<ContactItem>();

              if (Id == Guid.Empty)
              {
                  #region contactItem = new ContactItem
        
                  if (SelectedContactOwner != null)
                  {
                      var masterid=
                          contactItem = new ContactItem
                          {
                              MasterId = Guid.NewGuid(),
                              DateOfBirth = DateOfBirth,
                              MaritalStatusMasterId = (int)SelectedMaritalStatus,
                              BusinessPhone = BusinessPhone,
                              ChildrenNames = ChildrenNames,
                              City = City,
                              Company = Company,
                              ContactClassification = (int)SelectedContactClassification,
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
                              StatusId = (int)EntityStatus.Active,
                              IsNew = true
                          };
                  }

                  #endregion

                  contactItems.Add(contactItem);
                  string AuditLogEntry = string.Format("Created New Contact: {0}; Contact Owner: ", FirstName + " " + LastName);
                  Using<IAuditLogWFManager>(c).AuditLogEntry("Contacts Administration", AuditLogEntry);
              }
              else
              {
                  #region contactItem = new ContactItem

                  contactItem = new ContactItem
                  {
                      MasterId = Id,
                      DateOfBirth = DateOfBirth,
                      MaritalStatusMasterId = (int)SelectedMaritalStatus,
                      BusinessPhone = BusinessPhone,
                      ChildrenNames = ChildrenNames,
                      City = City,
                      Company = Company,
                      ContactClassification = (int)SelectedContactClassification,
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
                      StatusId = (int)EntityStatus.Active,
                      IsNew = false
                  };

                  #endregion

                  contactItems.Add(contactItem);

                  string AuditLogEntry = string.Format("Updated Contact: {0}; Contact Owner: ", FirstName + " " + LastName);
                  Using<IAuditLogWFManager>(c).AuditLogEntry("Contacts Administration", AuditLogEntry);
              }


              if (contactItem.ContactClassification == (int)ContactClassification.PrimaryContact)
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
                          MaritalStatusMasterId = (int)SelectedMaritalStatus,
                          BusinessPhone = item.BusinessPhone,
                          ChildrenNames = item.ChildrenNames,
                          City = item.City,
                          Company = item.Company,
                          ContactClassification = (int)ContactClassification.SecondaryContact,
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
                          StatusId = (int)EntityStatus.Active,
                          ContactOwnerMasterId = item.ContactOwnerMasterId,
                          ContactOwnerType = item.ContactOwnerType,
                          ContactTypeMasterId = item.ContactType != null ? item.ContactType.Id : Guid.Empty,
                          IsNew = false
                      };

                      #endregion

                      string AuditLogEntry = string.Format("Updated Contact: {0}; Contact Owner: ",
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

     void LoadForContactEdit()
      {
          using (StructureMap.IContainer c = NestedContainer)
          {
              PageTitle = GetLocalText("sl.contacts.edit.title.edit"); // "Edit Contact";


              var contact = Using<IContactRepository>(c).GetById(Id);
              FirstName = contact.Firstname;
              LastName = contact.Lastname;
              DateOfBirth = contact.DateOfBirth == null ? DateTime.Now : (DateTime)contact.DateOfBirth;
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
             
              if (contact.ContactOwnerMasterId != Guid.Empty)
              {
                  var costcentre = Using<ICostCentreRepository>(c).GetById(contact.ContactOwnerMasterId);
                  if(costcentre !=null)
                  SelectedContactOwner = new ContactOwnerLookUp() {Id =costcentre.Id,Name = costcentre.Name};
                  ContactOwners.Add(SelectedContactOwner);
                  //costcentre != null ? ContactOwners.FirstOrDefault(n => n.Name == costcentre.Name)
                  // : ContactOwners.FirstOrDefault();
              }
              if (contact.ContactType != null)
                  SelectedContactType = ContactTypes.First(n => n.Id == contact.ContactType.Id);
              if (contact.ContactClassification == ContactClassification.PrimaryContact)
                  
              SelectedContactClassification = contact.ContactClassification != 0
                                                  ? contact.ContactClassification
                                                  : ContactClassification.SecondaryContact;
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
          SelectedContactOwnerType=ContactOwnerType.Distributor;
      }

      void LoadContactOwners()
      {
          using (StructureMap.IContainer c = NestedContainer)
          {
              ContactOwners.Clear();
              var costCentreId = Using<IConfigService>(c).Load().CostCentreId;
              switch (SelectedContactOwnerType)
              {
                  case ContactOwnerType.Distributor:
                      ContactOwners.Add(new ContactOwnerLookUp()
                      {
                          Id = Guid.Empty,
                          Name = "Select Ditributor",
                          Type = typeof(Distributor)
                      });
                      
                       var cc = Using<ICostCentreRepository>(c).GetById(costCentreId);
                        ContactOwners.Add(new ContactOwnerLookUp()
                            {
                                Id=cc.Id,
                                Name = cc.Name,
                                Type = typeof (Distributor)
                            });

                      
                      break;
                #if(KEMSA)
                case ContactOwnerType.HealthFacility:
#else
                  case ContactOwnerType.Outlet:
                      ContactOwners.Add(new ContactOwnerLookUp()
                      {
                          Id = Guid.Empty,
                          Name = "Select Outlet",
                          Type = typeof(Outlet)
                      });
#endif  
                   Using<IOutletRepository>(c).GetByDistributor(costCentreId)
                          .ForEach(n=>ContactOwners.Add(new ContactOwnerLookUp()
                                          {
                                              Id = n.Id,
                                              Name = n.Name,
                                              Type = typeof(Outlet)
                                          }));
                  
                      break;
                  case ContactOwnerType.User:
                      var ccs = Using<ICostCentreRepository>(c).GetAll().Where(n => n.ParentCostCentre != null);
                        ccs.Where(n => n.ParentCostCentre.Id == costCentreId).ToList();
                        List<Guid> ccIds = ccs.Select(n => n.Id).ToList();
                        ccIds.Add(costCentreId);

                        var users = ccIds.ToList().SelectMany(n => Using<IUserRepository>(c).GetByCostCentre(n));
                        users.ToList().ForEach(n => ContactOwners.Add(new ContactOwnerLookUp()
                            {
                                 Id= n.CostCentre,
                                Name = n.Username,
                                Type = typeof (User)
                            }));
                      ContactOwners.Add(new ContactOwnerLookUp()
                                                 {
                                                     Id = Guid.Empty,
                                                     Name = "Select User",
                                                     Type = typeof(User)
                                                 });
                      break;
                  
              }
              SelectedContactOwner = ContactOwners.FirstOrDefault(p => p.Id == Guid.Empty);
          }
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

          SelectedContactClassification=ContactClassification.None;
      }

      private void LoadMaritalStatuses()
      {
          Type _enumType = typeof(MaritalStatas);
          FieldInfo[] infos = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
          MaritalStatuses.Clear();
          foreach (FieldInfo fi in infos)
              MaritalStatuses.Add((MaritalStatas)Enum.Parse(_enumType, fi.Name, true));

          SelectedMaritalStatus = MaritalStatas.Single;

      }

      void LoadContactTypes()
      {
          ContactTypes.Clear();
          SelectedContactType = new ContactType(Guid.Empty)
          {
              Name = GetLocalText("sl.contacts.edit.contactType.default") /*"--Please Select Contact Type--"*/
          };
          ContactTypes.Add(SelectedContactType);
          using (StructureMap.IContainer c = NestedContainer)
          {
              Using<IContactTypeRepository>(c).GetAll().ToList()
                  .ForEach(n => ContactTypes.Add(n));
          }
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
          MaritalStatuses.Clear();
          ContactOwnerTypes.Clear();
          ContactOwners.Clear();
          ContactClassifications.Clear();
          ContactTypes.Clear();
         

          SelectedContactOwnerType = ContactOwnerType.None;
         SelectedMaritalStatus = MaritalStatas.Unknown;
         SelectedContactType = new ContactType(Guid.Empty) { Name = "--Select contact Type--" };
         SelectedContactOwner = new ContactOwnerLookUp() { Id =Guid.Empty,Name="--Select Contact owner--" };
          ContactOwnerTypeName = "";


      }

      private void CancelAll()
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


      //if this user has a primary contact ... set this contact as default esle set as primary
      private void SetDefaultContactClassification()
      {
          if (Id == Guid.Empty)
          {
              if (!PrimaryContactIsSet())
              {
                  SelectedContactClassification = ContactClassification.PrimaryContact;
                  IsPrimaryContact = true;
                  IsSecondaryContact = false;
              }
              else
              {
                  SelectedContactClassification = ContactClassification.SecondaryContact;
                  IsPrimaryContact = false;
                  IsSecondaryContact = true;
              }
          }
      }

     private bool PrimaryContactIsSet()
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

             var oldValue = _id;
             _id = value;
             RaisePropertyChanged(IdPropertyName);
         }
     }


     public const string SelectedContacttypePropertyName = "SelectedContacttype";

     private ContactType _selectedContacttype = null;

     /// <summary>
     /// Sets and gets the SelectedContacttype property.
     /// Changes to that property's value raise the PropertyChanged event. 
     /// </summary>
     public ContactType SelectedContacttype
     {
         get
         {
             return _selectedContacttype;
         }

         set
         {
             if (_selectedContacttype == value)
             {
                 return;
             }

             RaisePropertyChanging(SelectedContacttypePropertyName);
             _selectedContacttype = value;
             RaisePropertyChanged(SelectedContacttypePropertyName);
         }
     }


     public const string ContactTypeNamePropertyName = "ContactTypeName";

     private string _contactTypeName = "";

     /// <summary>
     /// Sets and gets the ContactTypeName property.
     /// Changes to that property's value raise the PropertyChanged event. 
     /// </summary>
     public string ContactTypeName
     {
         get
         {
             return _contactTypeName;
         }

         set
         {
             if (_contactTypeName == value)
             {
                 return;
             }

             RaisePropertyChanging(ContactTypeNamePropertyName);
             _contactTypeName = value;
             RaisePropertyChanged(ContactTypeNamePropertyName);
         }
     }

     public const string MaritalStatusesNamePropertyName = "MaritalStatusesName";

     private string _maritalStatusesName = "";

     public string MaritalStatusesName
     {
         get
         {
             return _maritalStatusesName;
         }

         set
         {
             if (_maritalStatusesName == value)
             {
                 return;
             }

             RaisePropertyChanging(MaritalStatusesNamePropertyName);
             _maritalStatusesName = value;
             RaisePropertyChanged(MaritalStatusesNamePropertyName);
         }
     }



     /// <summary>
     /// The <see cref="ContactOwnerTypeName" /> property's name.
     /// </summary>
     public const string ContactOwnerTypeNamePropertyName = "ContactOwnerTypeName";

     private string _contactOwnerTypeName = "Distributor";

     /// <summary>
     /// Sets and gets the ContactOwnerTypeName property.
     /// Changes to that property's value raise the PropertyChanged event. 
     /// </summary>
     public string ContactOwnerTypeName
     {
         get
         {
             return _contactOwnerTypeName;
         }

         set
         {
             if (_contactOwnerTypeName == value)
             {
                 return;
             }

             RaisePropertyChanging(ContactOwnerTypeNamePropertyName);
             _contactOwnerTypeName = value;
             RaisePropertyChanged(ContactOwnerTypeNamePropertyName);
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
             if (_contact_owner_type != ContactOwnerType.None)
                 LoadContactOwners();
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
             _selectedContactClassification = value;
             RaisePropertyChanged(SelectedContactClassificationPropertyName);

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
     public string BtnCancelContent
     {
         get { return CanEdit ? "Cancel" : "Back"; }
     }


     public const string IsPrimaryContactPropertyName = "IsPrimaryContact";
     private bool _isPrimaryContact;
     public bool IsPrimaryContact
     {
         get
         {
             return _isPrimaryContact;
         }

         set
         {
             if (_isPrimaryContact == value)
             {
                 return;
             }

             _isPrimaryContact = value;
             RaisePropertyChanged(IsPrimaryContactPropertyName);
         }
     }

     public const string IsSecondaryContactPropertyName = "IsSecondaryContact";
     private bool _isSecondaryContact;
     public bool IsSecondaryContact
     {
         get
         {
             return _isSecondaryContact;
         }

         set
         {
             if (_isSecondaryContact == value)
             {
                 return;
             }

             _isSecondaryContact = value;
             RaisePropertyChanged(IsSecondaryContactPropertyName);
         }
     }
     #endregion
  }

   #region Classes
        public class ContactOwnerLookUp 
        {

            public Guid Id { get; set; }
            public string Name { get; set; }
            public object Type { get; set; }
        }
#endregion
}
