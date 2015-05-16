using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master;
using Distributr.Core.Data.EF;
using Distributr.Core.Utility;
using log4net;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Domain.Master.CostCentreEntities;
using System.Text.RegularExpressions;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
    internal class ContactRepository : RepositoryMasterBase<Contact>, IContactRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        
        IContactTypeRepository _contactTypeRepository;
        public ContactRepository(CokeDataContext ctx, ICacheProvider cacheProvider, IContactTypeRepository contactTypeRepository)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            
            _contactTypeRepository = contactTypeRepository;
            _log.DebugFormat("Contact Repository Constructor BootStrap");
        }
        public Guid Save(Contact entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                _log.DebugFormat("Failed to validate invalid contact");
                throw new DomainValidationException(vri, "Failed to save invalid contact");
            }
            tblContact tblCont = _ctx.tblContact.FirstOrDefault(n => n.id == entity.Id); ;
            DateTime date = DateTime.Now;
            if (tblCont == null)
            {
                tblCont = new tblContact
                              {
                                  IM_DateCreated = date,
                                  IM_Status = (int)EntityStatus.Active,//true,
                                  id = entity.Id
                              };
                _ctx.tblContact.AddObject(tblCont);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (tblCont.IM_Status != (int)entityStatus)
                tblCont.IM_Status = (int)entity._Status;
            tblCont.IM_DateLastUpdated = date;
            tblCont.Firstname = entity.Firstname;
            tblCont.Lastname = entity.Lastname;
            tblCont.Fax = entity.Fax;
            tblCont.MobilePhone = entity.MobilePhone;
            tblCont.PhysicalAddress = entity.PhysicalAddress;
            tblCont.PostalAddress = entity.PostalAddress;
            tblCont.BusinessPhone = entity.BusinessPhone;
            tblCont.HomePhone = entity.HomePhone;
            tblCont.HomeTown = entity.HomeTown;
            tblCont.City = entity.City;
            tblCont.JobTitle = entity.JobTitle;
            tblCont.Company = entity.Company;
            tblCont.SpouseName = entity.SpouseName;
            tblCont.ContactOwner = (int)entity.ContactOwnerType;
            if (entity.MStatus != null)
                tblCont.MaritalStatusId = (int) entity.MStatus;

            if (entity.ContactType != null && entity.ContactType.Id != Guid.Empty)
                tblCont.ContactType = entity.ContactType.Id;
            else
                tblCont.ContactType = null;

            if (entity.DateOfBirth != null)
                tblCont.DateOfBirth = entity.DateOfBirth.Value;
            else
                tblCont.DateOfBirth = null;

            tblCont.ContactClassification = (int)entity.ContactClassification;
            tblCont.Email = entity.Email;
            tblCont.ChildrenNames = entity.ChildrenNames;
            tblCont.CostCenterId = entity.ContactOwnerMasterId;
            tblCont.WorkExtPhone = entity.WorkExtPhone;
            _log.DebugFormat("Saving Contact");
            _log.DebugFormat("Contact id:{0}", tblCont.CostCenterId);
            _log.DebugFormat("entity id:{0}", entity.ContactOwnerMasterId);
            _ctx.SaveChanges();
            _log.DebugFormat("Invalidating Cache");
            _cacheProvider.Put(_cacheListKey, _ctx.tblContact.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, tblCont.id));
            return tblCont.id;
        }

        public void SetInactive(Contact entity)
        {
            _log.DebugFormat("Inactivating Contact ");
            tblContact contact;
            if (_ctx.tblContact != null)
            {
                contact = _ctx.tblContact.FirstOrDefault(n => n.id  == entity.Id);
                if (contact != null)
                {
                    contact.IM_Status = (int)EntityStatus.Inactive;// false;
                    contact.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblContact.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, contact.id));
                }
            }
        }

        public void SetActive(Contact entity)
        {
            tblContact tblContact = _ctx.tblContact.FirstOrDefault(n => n.id == entity.Id);
            if (tblContact != null)
            {
                tblContact.IM_Status = (int) EntityStatus.Active;
                tblContact.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblContact.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(n => n.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblContact.id));
            }
        }

        public void SetAsDeleted(Contact entity)
        {
            var vri = Validate(entity);
            //var hasOutletDependency = _ctx.tblCostCentre.Where(n => n.IM_Status != (int)EntityStatus.Deleted)
            //    .Where(n => n.)
            _log.DebugFormat("Deleting Contact ");
            tblContact contact = _ctx.tblContact.FirstOrDefault(n => n.id == entity.Id);
            if (contact != null)
            {
                contact.IM_Status = (int)EntityStatus.Deleted;
                contact.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblContact.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, contact.id));
            }
        }

        public Contact GetById(Guid id, bool includeDeactivated = false)
        {
            _log.DebugFormat("Getting Contact by id:{0}", id);
            Contact entity = (Contact)_cacheProvider.Get(string.Format(_cacheKey, id));
            if (entity == null)
            {
                var tbl = _ctx.tblContact.FirstOrDefault(s => s.id  == id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        protected override string _cacheKey
        {
            get { return "Contact-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "ContactList"; }
        }

        public override IEnumerable<Contact> GetAll(bool includeDeactivated = false)
        {
            IList<Contact> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<Contact>(ids.Count);
                foreach (Guid id in ids)
                {
                    Contact entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblContact.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (Contact p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        public Contact Map(tblContact contact)
        {

            Contact con = new Contact(contact.id);

            con.Firstname = contact.Firstname;
            con.Lastname = contact.Lastname == null ? "" : contact.Lastname;
            con.Fax = contact.Fax;
            if (contact.DateOfBirth.HasValue)
                con.DateOfBirth = contact.DateOfBirth.Value;
            if (contact.MaritalStatusId != null)
                con.MStatus = (MaritalStatas)contact.MaritalStatusId;
            con.MobilePhone = contact.MobilePhone;
            con.PhysicalAddress = contact.PhysicalAddress;
            con.PostalAddress = contact.PostalAddress;
            con.HomeTown = contact.HomeTown;
            con.City = contact.City;
            con.HomePhone = contact.HomePhone;
            con.Company = contact.Company;
            con.JobTitle = contact.JobTitle;
            con.SpouseName = contact.SpouseName;
            con.BusinessPhone = contact.BusinessPhone;
            con.WorkExtPhone = contact.WorkExtPhone;
            con.Email = contact.Email;
            con.ChildrenNames = contact.ChildrenNames;
            if (contact.ContactType != null)
                con.ContactType = _contactTypeRepository.GetById(contact.ContactType.Value);
            con.ContactOwnerType = (ContactOwnerType)contact.ContactOwner;
            con.ContactOwnerMasterId = contact.CostCenterId;// _costCenterRepository.GetById(contact.CostCenterId);
            con._SetStatus((EntityStatus)contact.IM_Status);
            con._SetDateCreated(contact.IM_DateCreated);
            con._SetDateLastUpdated(contact.IM_DateLastUpdated);
            if (contact.ContactClassification != null)
                con.ContactClassification = (ContactClassification)contact.ContactClassification;

            return con;
        }

        public ValidationResultInfo Validate(Contact itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            //bool hasDuplicate = GetAll(true).Where(n => n.Id != itemToValidate.Id)
            //    .Any(n => n.MobilePhone == itemToValidate.MobilePhone);

            
            //bool hasDuplicate =
            //    GetAll(true).Any(n => n.Id != itemToValidate.Id && n.MobilePhone == itemToValidate.MobilePhone);
           
            //if (hasDuplicate)
            //    vri.Results.Add(new ValidationResult("Duplicate Mobile Number found"));
            if (itemToValidate.ContactOwnerMasterId == Guid.Empty)
                vri.Results.Add(new ValidationResult("You must assign a costcentre to a contact"));

            

            //if (itemToValidate.ContactClassification == null)
            //    vri.Results.Add(new ValidationResult("You must assign a contact classification to a contact"));
            //bool hasDuplicateBP = false;
            //if (itemToValidate.BusinessPhone != null && itemToValidate.BusinessPhone.Trim() != "")
            //{
            //    hasDuplicateBP =
            //        _ctx.tblContact.Where(n => n.id != itemToValidate.Id).Any(
            //            n => n.BusinessPhone == itemToValidate.BusinessPhone);
            //}
            //if (hasDuplicateBP)
            //    vri.Results.Add(new ValidationResult("Duplicate Business Phone number."));

            return vri;
        }

        public List<Contact> GetByContactsOwnerId(Guid costCentreId, bool includeDeactivated = false)
        {
            _log.DebugFormat("Getting Contact by id:{0}", costCentreId);
            var contacts = _ctx.tblContact.Where(s => s.CostCenterId == costCentreId && s.IM_Status == (int)EntityStatus.Active).ToList().Select(Map).ToList();
            return contacts;
        }

        public Contact GetByContactOwnerId(Guid costCentreId)
        {
            _log.DebugFormat("Getting Contact by id:{0}", costCentreId);
            var contact = _ctx.tblContact.Where(s => s.CostCenterId == costCentreId).Select(Map).FirstOrDefault();
            return contact;
        }


        public QueryResult<Contact> Query(QueryBase query)
        {
           
            IQueryable<tblContact> contactQuery;

            if (query is QuerySupplierContact)
            {
                var q = query as QuerySupplierContact;

                if (q.ShowInactive)
                    contactQuery = _ctx.tblContact.Where(s => s.CostCenterId == q.SupplierId).AsQueryable();
                else
                    contactQuery = _ctx.tblContact.Where(s => s.CostCenterId == q.SupplierId && s.IM_Status == (int)EntityStatus.Active).AsQueryable();


                var queryResult = new QueryResult<Contact>();
                if (!string.IsNullOrEmpty(q.Name))
                {
                    contactQuery = contactQuery.Where(s => s.Firstname.ToLower().Contains(q.Name.ToLower()) ||
                                                           s.Lastname.ToLower().Contains(q.Name.ToLower()) ||
                                                           s.Email.ToLower().Contains(q.Name.ToLower()));
                }

                queryResult.Count = contactQuery.Count();
                contactQuery = contactQuery.OrderBy(s => s.Firstname).ThenBy(s => s.Lastname);
                if (q.Skip.HasValue && q.Take.HasValue)
                    contactQuery = contactQuery.Skip(q.Skip.Value).Take(q.Take.Value);
                var result = contactQuery.ToList();
                queryResult.Data = result.Select(Map).OfType<Contact>().ToList();
                q.ShowInactive = false;

                return queryResult;
            }

            else
            {
                var q = query as QueryStandard;
                contactQuery = q.ShowInactive ? _ctx.tblContact.Where(s => s.IM_Status != (int)EntityStatus.Deleted).AsQueryable() : _ctx.tblContact.Where(s => s.IM_Status == (int)EntityStatus.Active).AsQueryable();

                if(q.SupplierId!=null)
                    contactQuery = _ctx.tblContact.Where(s => s.CostCenterId == q.SupplierId).AsQueryable();
                

                var queryResult = new QueryResult<Contact>();
                if (!string.IsNullOrEmpty(q.Name))
                {
                    contactQuery = contactQuery.Where(s => s.Firstname.ToLower().Contains(q.Name.ToLower()) ||
                                                           s.Lastname.ToLower().Contains(q.Name.ToLower()) ||
                                                           s.Email.ToLower().Contains(q.Name.ToLower()));
                }

                queryResult.Count = contactQuery.Count();
                contactQuery = contactQuery.OrderBy(s => s.Firstname).ThenBy(s => s.Lastname);
                if (q.Skip.HasValue && q.Take.HasValue)
                    contactQuery = contactQuery.Skip(q.Skip.Value).Take(q.Take.Value);
                var result = contactQuery.ToList();
                queryResult.Data = result.Select(Map).OfType<Contact >().ToList();
                q.ShowInactive = false;

                return queryResult;
            }
            
        }
    }
}
