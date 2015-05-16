using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.Contact;
using Distributr.HQ.Lib.Helper;
using System.Web.Mvc;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Contacts;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Contacts
{
  public  class ContactViewModelBuilder:IContactViewModelBuilder
    {
      IContactRepository _contactRepository;
      IUserRepository _userRepository;
      IContactTypeRepository _contactTypeRepository;
      ICostCentreRepository _costCentreRepository;
      
      public ContactViewModelBuilder(IContactRepository contactRepository, ICostCentreRepository costCentreRepository, IContactTypeRepository contactTypeRepository, IUserRepository userRepository)
      {
          _contactRepository = contactRepository;
          _costCentreRepository = costCentreRepository;
          _contactTypeRepository = contactTypeRepository;
          _userRepository = userRepository;
      }
        public IList<ContactViewModel> GetAll(bool inactive = false)
        {
       
          var contacts= _contactRepository.GetAll(inactive).Select(s=>Map(s)).ToList();
          return contacts;
        }

        public ContactViewModel Get(Guid Id)
        {
            Contact cont = _contactRepository.GetById(Id);

            if (cont == null) return null;
               
            return Map(cont);
        }
      public   ContactViewModel Map(Contact cont)
        {
            //if (cont == null)
            //    return null;
            //else
            //{
                var contTypes = _contactTypeRepository.GetAll().ToDictionary(n=>n.Id,n=>n.Name);
                var contOwner = _contactRepository.GetAll().OrderBy(n => n.ContactOwnerType).Distinct().ToDictionary(n=>n.Id,n=>n.ContactOwnerType);
                Dictionary<int, string> cl = EnumHelper.EnumToList<ContactClassification>()
                               .ToDictionary(n => (int)n, n => n.ToString());
                ContactViewModel contactVM = new ContactViewModel();
               
                    contactVM.Id = cont.Id;
                    contactVM.BusinessPhone = cont.BusinessPhone;
                    contactVM.Firstname = cont.Firstname;
                    contactVM.Lastname = cont.Lastname;
                    contactVM.Fax = cont.Fax;
                    contactVM.MobilePhone = cont.MobilePhone;
                    contactVM.PostalAddress = cont.PostalAddress;
                    contactVM.PhysicalAddress = cont.PhysicalAddress;
                    contactVM.Email = cont.Email;
                    contactVM.HomePhone = cont.HomePhone ;
                    contactVM.City = cont.City;
                    contactVM.CostCentre = cont.ContactOwnerMasterId;
                    contactVM.HomeTown = cont.HomeTown;
                    contactVM.IsActive = cont._Status == EntityStatus.Active ? true : false;
                    contactVM.JobTitle = cont.JobTitle;
                    contactVM.ChildrenNames = cont.ChildrenNames;
                     contactVM.Company=cont.Company;
                     contactVM.WorkExtPhone=cont.WorkExtPhone;
                     contactVM.SpouseName = cont.SpouseName;
                     contactVM.ContactOwner = (int)cont.ContactOwnerType;
                     contactVM.ContactOwnerList = new SelectList(contOwner, "Key", "Value", (int)cont.ContactOwnerType);
                     contactVM.ContactTypeList = new SelectList(contTypes, "Key", "Value", cont.ContactType == null ? Guid.Empty : cont.ContactType.Id);
                     contactVM.Fullnames = cont.Firstname + " " + cont.Lastname;
                     if (cont.DateOfBirth.HasValue)
                         contactVM.DateofBirth = cont.DateOfBirth.Value;
                if (cont.MStatus != null)
                {
                    contactVM.MaritalStatus = cont.MStatus.ToString();
                    // contactVM.MaritalStatusId = _maritalStatusRepository.GetById(cont.MStatus.Id).Id;
                    //contactVM.MaritalStatus = _maritalStatusRepository.GetById(cont.MStatus.Id).MStatus;
                }

                if (cont.ContactType != null)
                {
                    contactVM.ContactTypeId = _contactTypeRepository.GetById(cont.ContactType.Id).Id;
                    contactVM.ContactTypeName = _contactTypeRepository.GetById(cont.ContactType.Id).Name;
                }
                    contactVM.ClassificationList=new SelectList(cl,"Key","Value");
                    return contactVM;
            //}
        }
        public void save(ContactViewModel contactViewModel)
        {

            Contact cont = new Contact(contactViewModel.Id);
           // {
                cont.Firstname = contactViewModel.Firstname;
                cont.Lastname = contactViewModel.Lastname;
                cont.BusinessPhone = contactViewModel.BusinessPhone;
                cont.Fax = contactViewModel.Fax;
                cont.MobilePhone = contactViewModel.MobilePhone;
                cont.PhysicalAddress = contactViewModel.PhysicalAddress;
                cont.PostalAddress = contactViewModel.PostalAddress;
                cont.Company = contactViewModel.Company;
                cont.Email = contactViewModel.Email;
                cont.City = contactViewModel.City;
                cont.ContactClassification = contactViewModel.Classification;
                cont.ContactOwnerMasterId = contactViewModel.CostCentre;
                if (contactViewModel.ContactFor == "User")
                {
                    cont.ContactOwnerType = Distributr.Core.Domain.Master.CostCentreEntities.ContactOwnerType.User;
                }
                if (contactViewModel.ContactFor == "Distributor")
                {
                    cont.ContactOwnerType = Distributr.Core.Domain.Master.CostCentreEntities.ContactOwnerType.Distributor;
                }
                if (contactViewModel.ContactFor == "Outlet")
                {
#if(KEMSA)

                    cont.ContactOwnerType = Distributr.Core.Domain.Master.CostCentreEntities.ContactOwnerType.HealthFacility;
#else
                    cont.ContactOwnerType = Distributr.Core.Domain.Master.CostCentreEntities.ContactOwnerType.Outlet;
#endif
                }
                else
                {
                    cont.ContactOwnerType = (ContactOwnerType)contactViewModel.ContactOwner;
                }
            cont.ContactOwnerMasterId = contactViewModel.CostCentre;
                cont.HomePhone = contactViewModel.HomePhone;
                cont.HomeTown = contactViewModel.HomeTown;
                cont.DateOfBirth = contactViewModel.DateofBirth;
            cont.MStatus = contactViewModel.MaritalStatusId;
               cont.ContactType = _contactTypeRepository.GetById(contactViewModel.ContactTypeId);
                cont.SpouseName = contactViewModel.SpouseName;
                cont.WorkExtPhone = contactViewModel.WorkExtPhone;
                cont.ChildrenNames = contactViewModel.ChildrenNames;
                cont.JobTitle = contactViewModel.JobTitle;
            //};
         
               _contactRepository.Save(cont);
           
        }

        public void SetInactive(Guid Id)
        {
            Contact cont = _contactRepository.GetById(Id);
            if (cont == null) throw new ArgumentNullException("cont");
            _contactRepository.SetInactive(cont);
        }


        public Dictionary<Guid, string> CostCentre()
        {
            return _costCentreRepository.GetAll().OrderBy(n=>n.Name)
                .Select(c => new { c.Id, c.Name }).ToList().ToDictionary(d=>d.Id,d=>d.Name);
        }




        public Dictionary<int, string> ContactClassification()
        {
            return EnumHelper.EnumToList<ContactClassification>()
                                 .ToDictionary(n => (int)n, n => n.ToString());
        }


        public Dictionary<int, string> GetMarialStatus()
        {
            return 
            EnumHelper.EnumToList<MaritalStatas>()
                                 .ToDictionary(n => (int)n, n => n.ToString());
        }
        public Dictionary<Guid, string> GetContactTypes()
        {
            return _contactTypeRepository.GetAll().ToList().OrderBy(m => m.Name)
                .Select(m => new { m.Id, m.Name }).ToDictionary(m => m.Id, m => m.Name);
        }


        public Dictionary<int, string> ContactOwner()
        {
            return EnumHelper.EnumToList<ContactOwnerType>()
                .ToDictionary(n => (int)n, n => n.ToString());
        }


        public List<ContactViewModel> SearchContacts(string srchParam, bool inactive = false)
        {
            var search = _contactRepository.GetAll(inactive)
                .Where(n => n.Firstname != null && n.Firstname.ToLower().StartsWith(srchParam.ToLower())
                || n.Lastname != null && n.Lastname.ToLower().StartsWith(srchParam.ToLower())
                || n.Email != null && n.Email.ToLower().StartsWith(srchParam.ToLower())
                || n.Company != null && n.Company.ToLower().StartsWith(srchParam.ToLower()));
            var contacts = search.Select(n => Map(n)).ToList();
            return contacts;
        }

        public List<ContactViewModel> GetContactsByCostCentre(Guid costCentre, bool inactive = false)
        {
            return _contactRepository.GetAll(inactive).ToList().Where(n => n.ContactOwnerMasterId == costCentre).Select(n => Map(n)).ToList();
        }

      public QueryResult<ContactViewModel> Query(QueryStandard q)
      {
          var queryResults = _contactRepository.Query(q);
          var results = new QueryResult<ContactViewModel>();
          results.Data = queryResults.Data.Select(Map).ToList();
          results.Count = queryResults.Count;
         
          return results;
      }



      public void SetActive(Guid Id)
        {
            Contact cont = _contactRepository.GetById(Id);
            _contactRepository.SetActive(cont);
        }

        public void SetAsDeleted(Guid Id)
        {
            Contact cont = _contactRepository.GetById(Id);
            if (cont == null) throw new ArgumentNullException("cont");
            _contactRepository.SetAsDeleted(cont);
        }


    }
}
