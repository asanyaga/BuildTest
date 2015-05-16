using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.CommodityOwnerrepositories
{
    internal class CommodityOwnerRepository : RepositoryMasterBase<CommodityOwner>, ICommodityOwnerRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
       
        private ICommodityOwnerTypeRepository _commodityOwnerTypeRepository;
        private ICommoditySupplierRepository _commoditySupplierRepository;

        public CommodityOwnerRepository(CokeDataContext ctx, ICacheProvider cacheProvider,  ICommodityOwnerTypeRepository commodityOwnerTypeRepository, ICommoditySupplierRepository commoditySupplierRepository)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
           
            _commodityOwnerTypeRepository = commodityOwnerTypeRepository;
            _commoditySupplierRepository = commoditySupplierRepository;
        }

        protected override string _cacheKey
        {
            get { return "CommodityOwner-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "CommodityOwnerList"; }
        }

        public Guid Save(CommodityOwner entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            } 
           
            if (!vri.IsValid)
            {
                _log.Debug(CoreResourceHelper.GetText("CommodityOwner.validation.error"));
                throw new DomainValidationException(vri,"" /*CoreResourceHelper.GetText("CommodityOwner.validation.error")*/);
            }
            DateTime dt = DateTime.Now;

            tblCommodityOwner commodityOwner = _ctx.tblCommodityOwner.FirstOrDefault(n => n.Id == entity.Id);
            if (commodityOwner == null)
            {
                commodityOwner = new tblCommodityOwner();
                commodityOwner.Id = entity.Id;
                commodityOwner.IM_Status = (int)EntityStatus.Active;
                commodityOwner.IM_DateCreated = dt;
                _ctx.tblCommodityOwner.AddObject(commodityOwner);

            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (commodityOwner.IM_Status != (int)entityStatus)
                commodityOwner.IM_Status = (int)entity._Status;
            commodityOwner.Code = entity.Code;
            commodityOwner.Surname = entity.Surname;
            commodityOwner.BusinessNo = entity.BusinessNumber;
            commodityOwner.CommodityOwnerTypeId = entity.CommodityOwnerType.Id;
            commodityOwner.CostCentreId = entity.CommoditySupplier.Id;
            commodityOwner.DOB = entity.DateOfBirth == new DateTime() ? DateTime.Now : entity.DateOfBirth;
            commodityOwner.Description = entity.Description;
            commodityOwner.Email = entity.Email;
            commodityOwner.FaxNo = entity.FaxNumber;
            commodityOwner.FirstName = entity.FirstName;
            commodityOwner.Gender = (int)entity.Gender;
            commodityOwner.IM_DateLastUpdated = dt;
            commodityOwner.IM_Status = (int)entity._Status;
            commodityOwner.IdNo = entity.IdNo;
            commodityOwner.LastName = entity.LastName;
            commodityOwner.MaritalStatus = (int?) entity.MaritalStatus;
            commodityOwner.OfficeNo = entity.OfficeNumber;
            commodityOwner.PINNo = entity.PinNo;
            commodityOwner.PhoneNo = entity.PhoneNumber;
            commodityOwner.PhysicalAddress = entity.PhysicalAddress;
            commodityOwner.PostalAddress = entity.PostalAddress;
            
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblCommodityOwner.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, commodityOwner.Id));
            return commodityOwner.Id; 
        }

        public void SetInactive(CommodityOwner entity)
        {
            tblCommodityOwner commodityOwner = _ctx.tblCommodityOwner.FirstOrDefault(n => n.Id == entity.Id);
            if (commodityOwner != null)
            {
                commodityOwner.IM_Status = (int)EntityStatus.Inactive;
                commodityOwner.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCommodityOwner.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, commodityOwner.Id));
            }
        }

        public void SetActive(CommodityOwner entity)
        {
            tblCommodityOwner commodityOwner = _ctx.tblCommodityOwner.FirstOrDefault(n => n.Id == entity.Id);
            if (commodityOwner != null)
            {
                commodityOwner.IM_Status = (int)EntityStatus.Active;
                commodityOwner.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCommodityOwner.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, commodityOwner.Id));
            }
        }

        public void SetAsDeleted(CommodityOwner entity)
        {
            tblCommodityOwner commodityOwner = _ctx.tblCommodityOwner.FirstOrDefault(n => n.Id == entity.Id);
            if (commodityOwner != null)
            {
                commodityOwner.IM_Status = (int)EntityStatus.Deleted;
                commodityOwner.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCommodityOwner.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, commodityOwner.Id));
            }
        }

        public CommodityOwner GetById(Guid id, bool includeDeactivated = false)
        {
            CommodityOwner entity = (CommodityOwner)_cacheProvider.Get(string.Format(_cacheKey, id));
            if (entity == null)
            {
                var tbl = _ctx.tblCommodityOwner.FirstOrDefault(s => s.Id == id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public override IEnumerable<CommodityOwner> GetAll(bool includeDeactivated = true)
        {
            IList<CommodityOwner> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<CommodityOwner>(ids.Count);
                foreach (Guid id in ids)
                {
                    CommodityOwner entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblCommodityOwner.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList();
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (CommodityOwner p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }
            entities.ToList();
            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        public List<CommodityOwner> GetBySupplier(Guid supplierId)
        {
            return GetAll().Where(s => s.CommoditySupplier.Id == supplierId).ToList();
        }

        public QueryResult<CommodityOwner> Query(QueryBase query)
        {
            var q = query as QueryCommodityOwner;
            IQueryable<tblCommodityOwner> commodityOwnerQuery;
            if(q.ShowInactive)
                commodityOwnerQuery = _ctx.tblCommodityOwner.Where(s => (s.IM_Status == (int)EntityStatus.Active || s.IM_Status == (int)EntityStatus.Inactive) && s.CostCentreId == q.SupplierId).AsQueryable();
            else
               commodityOwnerQuery = _ctx.tblCommodityOwner.Where(s=>s.CostCentreId==q.SupplierId && s.IM_Status==(int)EntityStatus.Active).AsQueryable();

            if(q.CommodityOwnerId.HasValue)
            {
                commodityOwnerQuery = commodityOwnerQuery.Where(n => n.Id == q.CommodityOwnerId.Value);
            }
            var queryResult = new QueryResult<CommodityOwner>();
            if (!string.IsNullOrWhiteSpace(q.Name))
            {
                commodityOwnerQuery = commodityOwnerQuery
                    .Where(s => s.Surname.ToLower().Contains(q.Name.ToLower()) || s.Code.ToLower().Contains(q.Name.ToLower()) || s.FirstName.ToLower().Contains(q.Name.ToLower()) || s.LastName.ToLower().Contains(q.Name.ToLower()));
            }

            queryResult.Count = commodityOwnerQuery.Count();
            commodityOwnerQuery = commodityOwnerQuery.OrderBy(s => s.Surname).ThenBy(s => s.Code);
            if (q.Skip.HasValue && q.Take.HasValue)
                commodityOwnerQuery = commodityOwnerQuery.Skip(q.Skip.Value).Take(q.Take.Value);
            var result = commodityOwnerQuery.ToList();
            queryResult.Data = result.Select(Map).OfType<CommodityOwner>().ToList();
            q.ShowInactive = false;
            return queryResult;
        }

        public ValidationResultInfo Validate(CommodityOwner itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;

            var query = _ctx.tblCommodityOwner.Where(p => p.IM_Status != (int) EntityStatus.Deleted);
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            bool hasDuplicateId = query
                .Any(s => s.Id != itemToValidate.Id && s.IdNo == itemToValidate.IdNo);
               
            if (hasDuplicateId)
                vri.Results.Add(new ValidationResult("Duplicate Commodity Owner ID Number found"));
            bool hasDuplicateCode = query
                .Any(s => s.Id != itemToValidate.Id && s.Code == itemToValidate.Code);
            if (hasDuplicateCode)
                vri.Results.Add(new ValidationResult("Duplicate Commodity Owner Code found"));



            return vri;
        }

        public CommodityOwner Map(tblCommodityOwner owner)
        {
            var commodityOwnerType = _commodityOwnerTypeRepository.GetById(owner.CommodityOwnerTypeId);
            var commoditySupplier = (CommoditySupplier) _commoditySupplierRepository.GetById(owner.CostCentreId);

            var commodityOwner = new CommodityOwner(owner.Id)
                                     {
                                         BusinessNumber = owner.BusinessNo,
                                         Code = (owner.Code ?? ""),
                                         DateOfBirth = owner.DOB,
                                         Description = owner.Description,
                                         Email = owner.Email,
                                         FaxNumber = owner.FaxNo,
                                         FirstName = owner.FirstName,
                                         Gender = (Gender) owner.Gender,
                                         IdNo = owner.IdNo,
                                         LastName = owner.LastName,
                                         MaritalStatus =
                                             owner.MaritalStatus == null
                                                 ? MaritalStatas.Unknown
                                                 : (MaritalStatas) owner.MaritalStatus.Value,
                                         OfficeNumber = owner.OfficeNo,
                                         PhoneNumber = owner.PhoneNo,
                                         PhysicalAddress = owner.PhysicalAddress,
                                         PinNo = owner.PINNo,
                                         PostalAddress = owner.PostalAddress,
                                         Surname = owner.Surname,
                                         CommodityOwnerType = commodityOwnerType,
                                         CommoditySupplier = commoditySupplier
                                     };
            commodityOwner._SetDateCreated(owner.IM_DateCreated);
            commodityOwner._SetDateLastUpdated(owner.IM_DateLastUpdated);
            commodityOwner._SetStatus((EntityStatus)owner.IM_Status);
            return commodityOwner;
        }
    }
}
