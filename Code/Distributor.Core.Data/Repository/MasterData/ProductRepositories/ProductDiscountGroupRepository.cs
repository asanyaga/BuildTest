using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility.Caching;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.ProductRepositories
{
    internal class ProductDiscountGroupRepository : RepositoryMasterBase<ProductGroupDiscount>, IProductDiscountGroupRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        IProductRepository _productRepository;
        IDiscountGroupRepository _discountGroupRepository;

        public ProductDiscountGroupRepository(CokeDataContext ctx,
        ICacheProvider cacheProvider,
        IProductRepository productRepository,
        IDiscountGroupRepository discountGroupRepository)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _productRepository = productRepository;
            _discountGroupRepository = discountGroupRepository;
        }

        public Guid Save(ProductGroupDiscount entity, bool? isSync = null)
        {
            _log.InfoFormat("Saving/Updating product discount group");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
               
                throw new DomainValidationException(vri, CoreResourceHelper.GetText("hq.pdgroup.validation.error"));
            }
            DateTime dt = DateTime.Now;
            tblProductDiscountGroup tblPDG = _ctx.tblProductDiscountGroup.FirstOrDefault(n =>n.IM_Status==(int)EntityStatus.Active&& n.DiscountGroup == entity.GroupDiscount.Id && n.Quantity==entity.Quantity && n.ProductRef==entity.Product.ProductId);
            if (tblPDG == null)
            {
                tblPDG = new tblProductDiscountGroup
                             {
                                 IM_Status = (int) EntityStatus.Active,
                                 IM_DateCreated = dt,
                                 id = entity.Id
                             };
                _ctx.tblProductDiscountGroup.AddObject(tblPDG);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (tblPDG.IM_Status != (int)entityStatus)
                tblPDG.IM_Status = (int)entity._Status;
            tblPDG.DiscountRate = entity.DiscountRate;
            tblPDG.EndDate = entity.EndDate;
            tblPDG.EffectiveDate = entity.EffectiveDate;
            tblPDG.Quantity = entity.Quantity;
            tblPDG.ProductRef = entity.Product.ProductId;
            var discItem = new tblProductDiscountGroupItem
                           {
                               IM_DateCreated = dt,
                               id = Guid.NewGuid(),
                               IM_DateLastUpdated = dt,
                               IM_Status = (int) EntityStatus.Active,
                               ProductRef = entity.Product.ProductId,
                               EffectiveDate = entity.EffectiveDate,
                               EndDate = entity.EndDate,
                               DiscountRate = entity.DiscountRate,
                               IsByQuantity = entity.IsByQuantity,
                               Quantity = entity.Quantity
                           };
            tblPDG.tblProductDiscountGroupItem.Add(discItem);
            tblPDG.IM_DateLastUpdated = dt;
            tblPDG.DiscountGroup = entity.GroupDiscount.Id;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblProductDiscountGroup.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, tblPDG.id));
            return tblPDG.id;
        }

        
        public void SetInactive(ProductGroupDiscount entity)
        {
            _log.InfoFormat("Setting Product Group Discount  Inactive");
            tblProductDiscountGroup tblPDG = _ctx.tblProductDiscountGroup.FirstOrDefault(n => n.id == entity.Id);
            if (tblPDG != null)
            {
                tblPDG.IM_DateLastUpdated = DateTime.Now;
                tblPDG.IM_Status = (int)EntityStatus.Inactive;// false;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblProductDiscountGroup.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblPDG.id));
            }
           
        }

        public void SetActive(ProductGroupDiscount entity)
        {
            _log.InfoFormat("Setting Product Group Discount  Active");
            tblProductDiscountGroup tblPDG = _ctx.tblProductDiscountGroup.FirstOrDefault(n => n.id == entity.Id);
            if (tblPDG != null)
            {
                tblPDG.IM_DateLastUpdated = DateTime.Now;
                tblPDG.IM_Status = (int)EntityStatus.Active;// false;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblProductDiscountGroup.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblPDG.id));
            }
        }

        public void SetAsDeleted(ProductGroupDiscount entity)
        {
            ProductGroupDiscount pgd = GetById(entity.Id);
            _log.InfoFormat("Setting Product Group Discount  Inactive");
            tblProductDiscountGroup tblPDG = _ctx.tblProductDiscountGroup.FirstOrDefault(n => n.id == entity.Id);
            if (tblPDG != null)
            {
                tblPDG.IM_DateLastUpdated = DateTime.Now;
                tblPDG.IM_Status = (int)EntityStatus.Deleted;// false;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblProductDiscountGroup.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblPDG.id));
            }
           
        }

        public ProductGroupDiscount GetById(Guid Id, bool includeDeactivated = false)
        {
            _log.InfoFormat("Gettting by Product Group Discount by id");
            ProductGroupDiscount entity = (ProductGroupDiscount)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblProductDiscountGroup.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public ValidationResultInfo Validate(ProductGroupDiscount itemToValidate)
        {
            _log.InfoFormat("Validation");
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));

          




            if (itemToValidate.EndDate.Date < itemToValidate.EffectiveDate.Date)
                    vri.Results.Add(new ValidationResult("Invalid date range - End date cannot be lower than start date"));
           

            return vri;
        }

       

        protected override string _cacheKey
        {
            get { return "ProductGroupDiscount-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "ProductGroupDiscountList"; }
        }

        public override IEnumerable<ProductGroupDiscount> GetAll(bool includeDeactivated = false)
        {
            _log.InfoFormat("Getting all");
            IList<ProductGroupDiscount> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<ProductGroupDiscount>(ids.Count);
                foreach (Guid id in ids)
                {
                    ProductGroupDiscount entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblProductDiscountGroup.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (ProductGroupDiscount p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities.Where(s => s.GroupDiscount != null).ToList();
           
        }

        ProductGroupDiscount Map(tblProductDiscountGroup tblPDG)
        {
            ProductGroupDiscount pgd = new ProductGroupDiscount(tblPDG.id)
            {
                GroupDiscount = _discountGroupRepository.GetById(tblPDG.DiscountGroup),
                
            };
            pgd.DiscountRate =tblPDG.DiscountRate.HasValue? tblPDG.DiscountRate.Value:0;
            pgd.EffectiveDate = tblPDG.EffectiveDate.HasValue ? tblPDG.EffectiveDate.Value : new DateTime(2000,01,01);
            pgd.EndDate = tblPDG.EndDate.HasValue ? tblPDG.EndDate.Value : new DateTime(2000, 01, 01);
            pgd.Quantity = tblPDG.Quantity.HasValue ? tblPDG.Quantity.Value : 0;
            pgd.Product = tblPDG.ProductRef.HasValue ? new ProductRef {ProductId = tblPDG.ProductRef.Value} : new ProductRef{ProductId = Guid.Empty};
            pgd._SetDateCreated(tblPDG.IM_DateCreated);
            pgd._SetDateLastUpdated(tblPDG.IM_DateLastUpdated);
            pgd._SetStatus((EntityStatus)tblPDG.IM_Status);
            return pgd;
        }

        public void SetLineItemsInactive(ProductGroupDiscount entity)
        {
            _log.InfoFormat("Setting product group discount line itemsinactive");
            tblProductDiscountGroupItem tblPDGItems = _ctx.tblProductDiscountGroupItem.FirstOrDefault(n => n.id == entity.Id);
            if (tblPDGItems != null)
            {
                tblPDGItems.IM_DateLastUpdated = DateTime.Now;
                tblPDGItems.IM_Status = (int)EntityStatus.Inactive;// false;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblProductDiscountGroup.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblPDGItems.tblProductDiscountGroup.id));
            }
            
        }
        public List<ProductGroupDiscount> GetByDiscountGroup(Guid discountGroup, bool includeDeactivated = false)
        {
            var all = _ctx.tblProductDiscountGroup.Where(
                p =>
                p.DiscountGroup == discountGroup &&
                (includeDeactivated
                     ? (p.IM_Status == (int) EntityStatus.Inactive || p.IM_Status == (int) EntityStatus.Active)
                     : p.IM_Status == (int) EntityStatus.Active)).ToList();
            return all.Select(Map).ToList();
            //return GetAll(includeDeactivated).Where(p => p.GroupDiscount.Id == discountGroup).ToList();
        }

       
        public ProductGroupDiscount GetByDiscountGroupCode(string discountgroupCode)
        {
            var discountGroup =
                _ctx.tblDiscountGroup.FirstOrDefault(
                    p => p.Code != null && p.Code.ToLower() == discountgroupCode.ToLower());
            if(discountGroup !=null)
            {
                var item = _ctx.tblProductDiscountGroup.FirstOrDefault(p => p.DiscountGroup == discountGroup.id);
                return item != null ? Map(item):null;
            }
            return null;
        }

        public ProductGroupDiscount GetByGroupbyProductByQuantity(Guid groupId, Guid productId, decimal quantity)
        {

            var pgdl =
                _ctx.tblProductDiscountGroup.FirstOrDefault(
                    p =>
                    p.DiscountGroup == groupId && p.ProductRef == productId && p.Quantity==quantity);
            return pgdl != null ? Map(pgdl) : null;
        }

        public ProductGroupDiscount GetCurrentCustomerDiscount(Guid groupId, Guid productId, decimal quantity)
        {
            var date = DateTime.Now;
            var discountdate = new DateTime(date.Year, date.Month, date.Day);
            var query =
                _ctx.tblProductDiscountGroup.Where(s =>
                    s.ProductRef.HasValue && 
                    s.EffectiveDate.HasValue &&
                    s.EndDate.HasValue &&
                    s.DiscountGroup == groupId &&
                    s.ProductRef == productId && 
                    s.Quantity <= quantity &&
                    s.IM_Status == (int) EntityStatus.Active).ToList()
                    .Where(
                        p=> 
                                p.EffectiveDate.Value.Date<= discountdate.Date &&
                          
                                p.EndDate.Value.Date>= discountdate.Date

                    )
                    .OrderByDescending(s => s.Quantity)
                    .ThenByDescending(s => s.EffectiveDate).ToList();
            var latest = query.FirstOrDefault(n => n.Quantity <= quantity);
            return latest != null ? Map(latest) : null;
        }
    }
}
