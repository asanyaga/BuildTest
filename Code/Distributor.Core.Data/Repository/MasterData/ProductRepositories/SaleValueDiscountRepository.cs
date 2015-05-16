using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.ProductRepositories
{
    internal class SaleValueDiscountRepository : RepositoryMasterBase<SaleValueDiscount>, ISaleValueDiscountRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        IProductPricingTierRepository _productPricingTier;
        public SaleValueDiscountRepository(CokeDataContext ctx, ICacheProvider cacheProvider, IProductPricingTierRepository productPricingTier)
        {
            _productPricingTier = productPricingTier;
            _cacheProvider = cacheProvider;
            _ctx = ctx;
        }


        public Guid Save(SaleValueDiscount entity, bool? isSync = null)
        {
            _log.DebugFormat("Saving/Updating Sale Value Discount");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                var failedDiscountItem = entity.DiscountItems
                    .FirstOrDefault(n => n._Status == EntityStatus.New);
                entity.DiscountItems.Remove(failedDiscountItem);
                throw new DomainValidationException(vri, CoreResourceHelper.GetText("hq.saleval.validation.error"));
            }
            DateTime dt = DateTime.Now;
            tblSaleValueDiscount tblSv = _ctx.tblSaleValueDiscount.FirstOrDefault(n => n.id == entity.Id);
            if (tblSv == null)
            {
                tblSv = new tblSaleValueDiscount
                            {
                                IM_DateCreated = dt,
                                IM_Status = (int) EntityStatus.Active,
                                id = entity.Id
                            };
                _ctx.tblSaleValueDiscount.AddObject(tblSv);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (tblSv.IM_Status != (int)entityStatus)
                tblSv.IM_Status = (int)entity._Status;


            tblSv.tblSaleValueDiscountItems
                .Where(n => n.SaleValueId == entity.Id && n.IM_Status != (int)EntityStatus.Deleted).ToList()
                .ForEach(n => n.IM_Status = (int)EntityStatus.Deleted);

            foreach (var discountItem in entity.DiscountItems)
            {
                if (discountItem._Status == EntityStatus.New)
                {
                    var tblSvi = _ctx.tblSaleValueDiscountItems.FirstOrDefault(n => n.id == discountItem.Id);
                    if (tblSvi == null)
                    {
                        tblSvi = new tblSaleValueDiscountItems
                                     {
                                         IM_DateCreated = dt,
                                         id = discountItem.Id,
                                         EffectiveDate = discountItem.EffectiveDate,
                                         EndDate = discountItem.EndDate,
                                         DiscountRate = discountItem.DiscountValue,
                                         SaleValue = discountItem.DiscountThreshold,
                                         IM_DateLastUpdated = dt,
                                         IM_Status = (int) EntityStatus.Active
                                     };
                        tblSv.tblSaleValueDiscountItems.Add(tblSvi);
                    }
                    else
                    {
                        tblSvi.IM_Status = (int) EntityStatus.Active;
                    }
                }
            }
            tblSv.IM_DateLastUpdated = dt;
            tblSv.TierId = entity.Tier.Id;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblSaleValueDiscount.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, tblSv.id));

            return tblSv.id;
        }

        
        public void SetAsDeleted(SaleValueDiscount entity)
        {
            var  svd = GetById(entity.Id);
            if (svd != null)
            {
                svd.DiscountItems.ForEach(i => i._Status = EntityStatus.Deleted);
                svd._Status = EntityStatus.Deleted;
                Save(svd);
            }
            else
            {
                throw new DomainValidationException(this.BasicValidation(), "Failed discount not set");
            }
        }

        public void SetInactive(SaleValueDiscount entity)
        {
            tblSaleValueDiscount tblsv = _ctx.tblSaleValueDiscount.FirstOrDefault(n => n.id == entity.Id);
            if(tblsv!=null)
            {
                tblsv.IM_Status = (int)EntityStatus.Inactive;//false;
                tblsv.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblSaleValueDiscount.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblsv.id));
            }
        }

        public void SetActive(SaleValueDiscount entity)
        {
            tblSaleValueDiscount tblsv = _ctx.tblSaleValueDiscount.FirstOrDefault(n => n.id == entity.Id);
            if (tblsv != null)
            {
                tblsv.IM_Status = (int)EntityStatus.Active;
                tblsv.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblSaleValueDiscount.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblsv.id));
            }
        }

        public SaleValueDiscount GetById(Guid Id, bool includeDeactivated = false)
        {
            SaleValueDiscount entity = (SaleValueDiscount)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblSaleValueDiscount.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }
            }
            return entity;
        }

        public ValidationResultInfo Validate(SaleValueDiscount itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));

            var discount = itemToValidate.DiscountItems
                .FirstOrDefault(n => n._Status == EntityStatus.New);

            if (discount != null)
            {
                if (discount.EndDate.Date < discount.EffectiveDate.Date)
                    vri.Results.Add(new ValidationResult("Invalid date range - End date cannot be lower than start date"));
            }
            return vri;
        }

        public ValidationResultInfo Validate(SaleValueDiscount.SaleValueDiscountItem itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;

            if (itemToValidate.EndDate < itemToValidate.EffectiveDate)
                vri.Results.Add(new ValidationResult("Invalid date range - End date cannot be lower than start date"));
            return vri;
        }

        public void AddSaleValueDiscount(Guid discountId, DateTime effectiveDate, decimal discountRate, decimal saleValue, DateTime endDate)
        {
            SaleValueDiscount svd = GetById(discountId);
            if (svd != null)
            {
                svd.DiscountItems.Add(new SaleValueDiscount.SaleValueDiscountItem(Guid.NewGuid())
                    {
                        EffectiveDate = effectiveDate,
                        DiscountValue = discountRate,
                        DiscountThreshold = saleValue,
                        EndDate = endDate,
                        _Status = EntityStatus.Active
                    });
                Save(svd);
            }
            else
            {
                throw new DomainValidationException(this.BasicValidation(), "Failed discount not set");
            }
        }

        public SaleValueDiscount GetByAmount(decimal Amount, Guid tier)
        {
            var svd = GetAll().Where(n => n.Tier.Id == tier)
                .FirstOrDefault(n => 
                    n.DiscountItems.Select(x => x.DiscountThreshold).Contains(Amount)  );
            return svd;
        }

       

       

        protected override string _cacheKey
        {
            get { return "SaleValueDiscount-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "SaleValueDiscountList"; }
        }

        public override IEnumerable<SaleValueDiscount> GetAll(bool includeDeactivated = false)
        {
            _log.DebugFormat("Get All");
            IList<SaleValueDiscount> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<SaleValueDiscount>(ids.Count);
                foreach (Guid id in ids)
                {
                    SaleValueDiscount entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblSaleValueDiscount.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (SaleValueDiscount p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }
        SaleValueDiscount Map(tblSaleValueDiscount tblSv)
        {
            SaleValueDiscount svd = new SaleValueDiscount(tblSv.id)
                                        {
                                            Tier = _productPricingTier.GetById(tblSv.TierId),
                                            DiscountItems =
                                                tblSv.tblSaleValueDiscountItems.Where(
                                                    n => n.IM_Status == (int) EntityStatus.Active).Select(
                                                        n =>
                                                        new SaleValueDiscount.SaleValueDiscountItem(n.id,
                                                                                                    n.IM_DateCreated,
                                                                                                    n.IM_DateLastUpdated,
                                                                                                    (EntityStatus)
                                                                                                    n.IM_Status)
                                                            {
                                                                DiscountThreshold = n.SaleValue,
                                                                DiscountValue = n.DiscountRate,
                                                                EffectiveDate = n.EffectiveDate,
                                                                EndDate = n.EndDate ?? n.EffectiveDate,
                                                                LineItemId = n.id,
                                                            }
                                                ).ToList(),

                                        };
            svd._SetDateCreated(tblSv.IM_DateCreated);
            svd._SetDateLastUpdated(tblSv.IM_DateLastUpdated);
            svd._SetStatus((EntityStatus)tblSv.IM_Status);
            return svd;
        }

        public void DeactivateLineItem(Guid lineItemId)
        {
            var discountlineItem = _ctx.tblSaleValueDiscountItems.FirstOrDefault(p => p.id == lineItemId);
            if (discountlineItem != null)
            {
                discountlineItem.IM_Status = (int)EntityStatus.Inactive;// false;
                discountlineItem.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblSaleValueDiscount.Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, discountlineItem.SaleValueId));
            }
        }

        public SaleValueDiscount GetCurrentDiscount(decimal Amount, Guid tier)
        {
            var all = GetAll().Where(s=>s.Tier.Id==tier).OrderBy(o=>o.CurrentSaleValue);
           
                return all.FirstOrDefault(t =>  t.CurrentSaleValue<=Amount&& t.CurrentEffectiveDate.Date<=DateTime.Now.Date && t.CurrentEndDate.Date>=DateTime.Now.Date);
           
        }

        public QueryResult<SaleValueDiscount> Query(QueryStandard q)
        {
            IQueryable<tblSaleValueDiscount> svdQuery;

            if (q.ShowInactive)
                svdQuery = _ctx.tblSaleValueDiscount.Where(j => j.IM_Status != (int) EntityStatus.Deleted).AsQueryable();
            svdQuery = _ctx.tblSaleValueDiscount.Where(j => j.IM_Status == (int) EntityStatus.Active).AsQueryable();

            if (!string.IsNullOrEmpty(q.Name))
                svdQuery = svdQuery.Where(j => j.tblPricingTier.Name.ToLower().Contains(q.Name));

            var queryresult = new QueryResult<SaleValueDiscount>();

            queryresult.Count = svdQuery.Count();

            svdQuery = svdQuery.OrderBy(h => h.TierId);

            if (q.Skip.HasValue && q.Take.HasValue)
                svdQuery = svdQuery.Skip(q.Skip.Value).Take(q.Take.Value);

            var result = svdQuery.ToList();

            queryresult.Data = svdQuery.Select(Map).ToList();

            q.ShowInactive = false;

            return queryresult;
        }
    }
}
