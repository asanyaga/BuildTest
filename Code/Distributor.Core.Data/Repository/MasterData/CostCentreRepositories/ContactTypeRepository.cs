using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Utility;
using Distributr.Core.Data.Utility.Caching;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
   internal class ContactTypeRepository:RepositoryMasterBase<ContactType>,IContactTypeRepository
    {
       CokeDataContext _ctx;
       ICacheProvider _cacheProvider;
       public ContactTypeRepository(CokeDataContext ctx,ICacheProvider CacheProvider)
       {
           _ctx = ctx;
           _cacheProvider = CacheProvider;
       }

       public Guid Save(ContactType entity, bool? isSync = null)
        {
            _log.InfoFormat("Saving/Updating Contact Type");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                string info = string.Join(",", vri.Results.Select(n => n.ErrorMessage)); 
                _log.DebugFormat("Failed to validate invalid contact");
                throw new DomainValidationException(vri, "Failed to save invalid contact -->" + info);
            }
            DateTime dt = DateTime.Now;
            tblContactType tblContType  = _ctx.tblContactType.FirstOrDefault(n=>n.id==entity.Id);
            if (tblContType == null)
            {
                tblContType = new tblContactType();
                tblContType.IM_DateCreated = dt;
                tblContType.IM_Status = (int)EntityStatus.Active;//true;
                tblContType.id = entity.Id;
                _ctx.tblContactType.AddObject(tblContType);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (tblContType.IM_Status != (int)entityStatus)
                tblContType.IM_Status = (int)entity._Status;
            tblContType.IM_DateLastUpdated = dt;
            tblContType.Name = entity.Name;
            tblContType.Code = entity.Code;
            
            tblContType.Description = entity.Description;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblContactType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, tblContType.id));
            return tblContType.id;
        }

        public void SetInactive(ContactType entity)
        {
            _log.InfoFormat("Setting Contact Type Inactive");
            tblContactType contactType = _ctx.tblContactType.FirstOrDefault(n=>n.id==entity.Id);
            if (contactType != null)
            {
                contactType.IM_DateLastUpdated = DateTime.Now;
                contactType.IM_Status = (int)EntityStatus.Inactive;// false;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblContactType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, contactType.id));
            }
        }

       public void SetActive(ContactType entity)
       {
           _log.InfoFormat("Setting Contact Type Active");
		   tblContactType contactType = _ctx.tblContactType.FirstOrDefault(n => n.id == entity.Id);
		   if(contactType != null)
		   {
			   contactType.IM_DateLastUpdated = DateTime.Now;
			   contactType.IM_Status = (int) EntityStatus.Active;
			   _ctx.SaveChanges();
			   _cacheProvider.Put(_cacheListKey, _ctx.tblContactType.Where(n=>n.IM_Status != (int)EntityStatus.Deleted).Select(n=>n.id).ToList());
			   _cacheProvider.Remove(string.Format(_cacheKey, contactType.id));
		   }
       }

       public void SetAsDeleted(ContactType entity)
       {
           var vri = Validate(entity);
           var hasDependency = _ctx.tblContact
               .Where(n => n.IM_Status != (int)EntityState.Deleted)
               .Any(n => n.ContactType.Value == entity.Id);
           if (hasDependency)
               throw new DomainValidationException(vri, "Cannot delete\r\nDependencies found");
           _log.InfoFormat("Setting Contact Type Delete");
           tblContactType contactType = _ctx.tblContactType.FirstOrDefault(n => n.id == entity.Id);
           if (contactType != null)
           {
               contactType.IM_DateLastUpdated = DateTime.Now;
               contactType.IM_Status = (int)EntityStatus.Deleted;// false;
               _ctx.SaveChanges();
               _cacheProvider.Put(_cacheListKey, _ctx.tblContactType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
               _cacheProvider.Remove(string.Format(_cacheKey, contactType.id));
           }
       }

       public ContactType GetById(Guid id, bool includeDeactivated = false)
        {
            ContactType entity = (ContactType)_cacheProvider.Get(string.Format(_cacheKey, id));
            if (entity == null)
            {
                var tbl = _ctx.tblContactType.FirstOrDefault(s => s.id == id);
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
           get { return "ContactType-{0}"; }
       }

       protected override string _cacheListKey
       {
           get { return "ContactTypeList"; }
       }

       public override IEnumerable<ContactType> GetAll(bool includeDeactivated = false)
        {
            _log.Info("Get All Contact Types");
            IList<ContactType> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<ContactType>(ids.Count);
                foreach (Guid id in ids)
                {
                    ContactType entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblContactType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (ContactType p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

       
       public QueryResult<ContactType> Query(QueryStandard q)
       {
           IQueryable<tblContactType> contactQuery;
           if (q.ShowInactive)
               contactQuery = _ctx.tblContactType.Where(s => s.IM_Status == (int)EntityStatus.Active || s.IM_Status == (int)EntityStatus.Inactive).AsQueryable();
           else
               contactQuery = _ctx.tblContactType.Where(s => s.IM_Status == (int)EntityStatus.Active).AsQueryable();

           var queryResult = new QueryResult<ContactType>();
           if (!string.IsNullOrEmpty(q.Name))
           {
               contactQuery = contactQuery.Where(s => s.Name.ToLower().Contains(q.Name.ToLower()) || s.Code.ToLower().Contains(q.Name.ToLower()) );
           }

           queryResult.Count = contactQuery.Count();
           contactQuery = contactQuery.OrderBy(s => s.Name).ThenBy(s => s.Code);
           if (q.Skip.HasValue && q.Take.HasValue)
               contactQuery = contactQuery.Skip(q.Skip.Value).Take(q.Take.Value);
           var result = contactQuery.ToList();
           queryResult.Data = result.Select(Map).OfType<ContactType>().ToList();
           q.ShowInactive = false;
           return queryResult;
       }

       ContactType Map(tblContactType contactType)
        {
            ContactType contType = new ContactType(contactType.id)
            {
                Code=contactType.Code,
                Name=contactType.Name,
                Description=contactType.Description
            };
            contType._SetDateCreated(contactType.IM_DateCreated);
            contType._SetDateLastUpdated(contactType.IM_DateLastUpdated);
            contType._SetStatus((EntityStatus)contactType.IM_Status);
            return contType;
        }

        public ValidationResultInfo Validate(ContactType itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            bool hasDuplicateName = GetAll(true)
                .Where(s => s.Id != itemToValidate.Id)
                .Any(n=>n.Name.ToLower()==itemToValidate.Name.ToLower());
            if (hasDuplicateName)
                vri.Results.Add(new ValidationResult("Duplicate Name Found"));
            bool hasDuplicateCode = GetAll(true)
                .Where(s => s.Id != itemToValidate.Id)
                .Any(c=>c.Code.ToLower()==itemToValidate.Code.ToLower());
            if (hasDuplicateCode)
                vri.Results.Add(new ValidationResult("Duplicate Code Found"));
            return vri;
        }
    }
}
