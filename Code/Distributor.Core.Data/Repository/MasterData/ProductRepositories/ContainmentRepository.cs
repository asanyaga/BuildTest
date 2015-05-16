using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.ProductRepositories
{
    internal class ContainmentRepository : RepositoryMasterBase<Containment>, IContainmentRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        IProductPackagingRepository _productPackagingRepository;
        IProductRepository _productRepository;
        IProductPackagingTypeRepository _productPackagingTypeRepository;

        public ContainmentRepository(CokeDataContext ctx, ICacheProvider cacheProvider, IProductPackagingRepository productPackagingRepository, IProductRepository productRepository, IProductPackagingTypeRepository productPackagingTypeRepository)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _productPackagingRepository = productPackagingRepository;
            _productRepository = productRepository;
            _productPackagingTypeRepository = productPackagingTypeRepository;
            _log.Debug("Containment Repository Constructor Bootstrap");
        }

        public Guid Save(Containment entity, bool? isSync = null)
        {
            _log.Debug("Saving/Updating Containment");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                _log.Debug("Failed to save invalid Containment");
                throw new DomainValidationException(vri, "Failed to save invalid Containment");
            }
            tblContainment containment = _ctx.tblContainment.FirstOrDefault(n => n.id == entity.Id);
            DateTime date = DateTime.Now;
            if( containment == null)
            {
                containment = new tblContainment
                {
                    IM_DateCreated = date,
                    IM_Status =(int)EntityStatus.Active,// true,
                    id= entity.Id
                };
                _ctx.tblContainment.AddObject(containment);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (containment.IM_Status != (int)entityStatus)
                containment.IM_Status = (int)entity._Status;

            containment.Quantity = entity.Quantity;
            containment.ReturnableProduct = entity.ProductRef.ProductId;
            containment.ProductPackagingType = entity.ProductPackagingType.Id;

            containment.IM_DateLastUpdated = date;

            _log.Debug("Saving Containment");
            _ctx.SaveChanges();
            _log.Debug("Invalidating cache");
            _cacheProvider.Put(_cacheListKey, _ctx.tblContainment.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, containment.id));
            _log.DebugFormat("Successfully saved item id:{0}", containment.id);
            return containment.id;
        }

        public void SetInactive(Containment entity)
        {
            _log.Debug("Inactivating Containment");
            //dependency Exists on Distributor and asm
            bool dependenciesPresent = false;

            string failureReason = "";
            if (dependenciesPresent)
            {
                failureReason = "DEPENDENCIES FOUND:";//populate with ids
                throw new ArgumentException(failureReason);
            }
            else
            {
                tblContainment containment = _ctx.tblContainment.First(n => n.id == entity.Id);
                if (containment == null || containment.IM_Status==(int)EntityStatus.Active)
                {//not existing or already deactivated.
                    return;
                }
                containment.IM_Status = (int)EntityStatus.Inactive;// false;
                containment.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblContainment.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, containment.id));
            }

        }

        public void SetActive(Containment entity)
        {
            throw new NotImplementedException();
        }

        public void SetAsDeleted(Containment entity)
        {
            throw new NotImplementedException();
        }

        public Containment GetById(Guid Id, bool includeDeactivated = false)
        {
            _log.DebugFormat("Getting Containment by ID: {0}", Id);

            Containment entity = (Containment)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblContainment.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity = tbl.Map();
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public ValidationResultInfo Validate(Containment itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            return vri;
        }


        protected override string _cacheKey
        {
            get { return "Containment-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "ContainmentList"; }
        }

        public override IEnumerable<Containment> GetAll(bool includeDeactivated = false)
        {
            _log.Debug("Get all Containments");
            IList<Containment> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<Containment>(ids.Count);
                foreach (Guid id in ids)
                {
                    Containment entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblContainment.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => s.Map()).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (Containment p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        
    }
}
