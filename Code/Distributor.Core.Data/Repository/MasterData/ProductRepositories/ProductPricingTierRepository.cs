using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Data.Utility;
using Distributr.Core.Factory.Master;
using Distributr.Core.Utility;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using log4net;

namespace Distributr.Core.Data.Repository.MasterData.ProductRepositories
{
    internal class ProductPricingTierRepository : RepositoryMasterBase<ProductPricingTier>, IProductPricingTierRepository
    {
        //calling the coke data contect
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        private IProductPricingRepository _productPricingRepository;
        public ProductPricingTierRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _log.Debug("");
        }
        // creating the save action
        public Guid Save(ProductPricingTier entity, bool? isSync = null)
        {
            _log.Debug("Saving and Updating the Region");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                throw new DomainValidationException(vri, CoreResourceHelper.GetText("hq.ptier.validation.error"));
            }
            tblPricingTier tblPricingTierToSave = _ctx.tblPricingTier.FirstOrDefault(n => n.id == entity.Id);
            if (tblPricingTierToSave == null)
            {
                entity._SetDateCreated(DateTime.Now);
                
                tblPricingTierToSave = new tblPricingTier();
                tblPricingTierToSave.id = entity.Id;
                _ctx.tblPricingTier.AddObject(tblPricingTierToSave);
                tblPricingTierToSave.IM_DateCreated = entity._DateCreated;
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (tblPricingTierToSave.IM_Status != (int)entityStatus)
                tblPricingTierToSave.IM_Status = (int)entity._Status;
           
            entity._SetDateLastUpdated(DateTime.Now);
            tblPricingTierToSave.Name = entity.Name;
            tblPricingTierToSave.Code = entity.Code;
            tblPricingTierToSave.Description = entity.Description;

            tblPricingTierToSave.IM_DateLastUpdated = entity._DateLastUpdated;
            //tblPricingTierToSave.IM_Status = productPricingTier._Status;
            tblPricingTierToSave.IM_Status = (int)EntityStatus.Active;//true;

            _log.Debug("Saving Tier Pricing");
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblPricingTier.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, tblPricingTierToSave.id));
            // validating cacheing

            return tblPricingTierToSave.id;
        }

        public void SetInactive(ProductPricingTier entity)
        {
            _log.Debug("Inactivating region");

            ValidationResultInfo vri = Validate(entity);
             bool hasPricingDependencies = _ctx.tblPricing.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p => p.Tier == entity.Id);
             bool hasDiscountsDependency = _ctx.tblDiscounts.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p => p.TierId == entity.Id);
             bool hasSaleValueDiscountDependency = _ctx.tblSaleValueDiscount.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p=>p.TierId == entity.Id);
             if (hasPricingDependencies || hasDiscountsDependency || hasSaleValueDiscountDependency)
             {
                 throw new DomainValidationException(vri, "Cannot deactivate\r\nDependencies found");
             }
             else
             {
                 tblPricingTier pricingTier = _ctx.tblPricingTier.FirstOrDefault(p => p.id == entity.Id);
                 if (pricingTier != null)
                 {
                     pricingTier.IM_Status = (int)EntityStatus.Inactive;//false;
                     
                     pricingTier.IM_DateLastUpdated = DateTime.Now;
                     _ctx.SaveChanges();
                     _cacheProvider.Put(_cacheListKey, _ctx.tblPricingTier.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                     _cacheProvider.Remove(string.Format(_cacheKey, pricingTier.id));

                 }
             }
            
        }

        public void SetActive(ProductPricingTier entity)
        {
            _log.Debug("Activing Product Pricing Tier: " + entity.Id);
            tblPricingTier tblPricingTier = _ctx.tblPricingTier.FirstOrDefault(n => n.id == entity.Id);
            if (tblPricingTier != null)
            {
                tblPricingTier.IM_Status = (int) EntityStatus.Active;
                tblPricingTier.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblPricingTier.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(n => n.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblPricingTier.id));
            }
        }

        public void SetAsDeleted(ProductPricingTier entity)
        {
            ValidationResultInfo vri = Validate(entity);
            bool hasPricingDependencies = _ctx.tblPricing.Where(s => s.IM_Status != (int)EntityStatus.Deleted)
                .Any(p => p.Tier == entity.Id);
            bool hasOutletDependency = _ctx.tblCostCentre.Where(n => n.IM_Status != (int) EntityStatus.Deleted)
                .Where(n => n.CostCentreType == (int) CostCentreType.Outlet)
                .Any(n => n.Tier_Id == entity.Id);
            if (hasPricingDependencies || hasOutletDependency)
            {
                throw new DomainValidationException(vri, "Cannot delete\r\nDependencies found");
            }
            else
            {
                _log.Debug("Deleting Product Pricing Tier");
                tblPricingTier pricingTier = _ctx.tblPricingTier.FirstOrDefault(p => p.id == entity.Id);
                if (pricingTier != null)
                {
                    pricingTier.IM_Status = (int)EntityStatus.Deleted;//false;

                    pricingTier.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblPricingTier.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, pricingTier.id));
                }
            }
        }


        public ProductPricingTier GetById(Guid id, bool includeDeactivated = false)
        {
            ProductPricingTier entity = (ProductPricingTier)_cacheProvider.Get(string.Format(_cacheKey, id));
            if (entity == null)
            {
                var tbl = _ctx.tblPricingTier.FirstOrDefault(s => s.id == id);
                if (tbl != null)
                {
                    entity = tbl.Map();
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;

        }

        protected override string _cacheKey
        {
            get { return "ProductPricingTier-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "ProductPricingTierList"; }
        }

        public override IEnumerable<ProductPricingTier> GetAll(bool includeDeactivated = false)
        {
            _log.Debug("IEnumerable<ProductPricingTier> GetAll");
            IList<ProductPricingTier> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<ProductPricingTier>(ids.Count);
                foreach (Guid id in ids)
                {
                    ProductPricingTier entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblPricingTier.ToList().Select(s => s.Map()).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (ProductPricingTier p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        public ProductPricingTier GetByCode(string tierCode)
        {
            var tier = _ctx.tblPricingTier.FirstOrDefault(p => p.Code != null && p.Code.ToLower() == tierCode.ToLower());
            return tier != null ? tier.Map():null;
        }

        public QueryResult<ProductPricingTier> Query(QueryStandard query)
        {
            IQueryable<tblPricingTier> pricingtierquery;
            if (query.ShowInactive)
                pricingtierquery = _ctx.tblPricingTier.Where(p => p.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
            else
                pricingtierquery = _ctx.tblPricingTier.Where(p => p.IM_Status == (int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<ProductPricingTier>();
            if (!string.IsNullOrEmpty(query.Name))
            {
                pricingtierquery = pricingtierquery.Where(p => p.Name.ToLower().Contains(query.Name.ToLower())
                                                         || p.Code.ToLower().Contains(query.Name.ToLower())
                                                         || p.Code.ToLower().Contains(query.Name.ToLower()));

            }

            pricingtierquery = pricingtierquery.OrderBy(p => p.Code).ThenBy(p => p.Name);
            queryResult.Count = pricingtierquery.Count();

            if (query.Skip.HasValue && query.Take.HasValue)
                pricingtierquery = pricingtierquery.Skip(query.Skip.Value).Take(query.Take.Value);
            queryResult.Data = pricingtierquery.Select(Map).ToList();

            return queryResult;
        }


        public ProductPricingTier Map(tblPricingTier Tier)
        {
            ProductPricingTier retTires = new ProductPricingTier(Tier.id)
            {

                Name = Tier.Name,
                Description = Tier.Description,
                Code = Tier.Code 
            };
            retTires._SetDateCreated(Tier.IM_DateCreated);
            retTires._SetDateLastUpdated(Tier.IM_DateLastUpdated);
            retTires._SetStatus((EntityStatus)Tier.IM_Status);

            return retTires;
        }






        public ValidationResultInfo Validate(ProductPricingTier itemToValidate)
        {
            //no duplicates TODO

            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
            {
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            }
           
            bool containDuplicate =
               _ctx.tblPricingTier.Where(n => n.id != itemToValidate.Id && n.IM_Status != (int)EntityStatus.Deleted).Any(n => n.Name == itemToValidate.Name);
            bool hasDuplicateCode =_ctx.tblPricingTier.Where(n => n.id != itemToValidate.Id && n.IM_Status != (int)EntityStatus.Deleted )
                .Any(n => n.Code == itemToValidate.Code);
            if (hasDuplicateCode)
                vri.Results.Add(new ValidationResult(CoreResourceHelper.GetText("hq.ptier.validation.dupcode")));
            if (containDuplicate)
            {
                vri.Results.Add(new ValidationResult(CoreResourceHelper.GetText("hq.ptier.validation.dupname")));
            }

            return vri;
        }
        







       
    }
}
