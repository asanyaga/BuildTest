using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
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
    internal class CertainValueCertainProductDiscountRepository : RepositoryMasterBase<CertainValueCertainProductDiscount>, ICertainValueCertainProductDiscountRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        IProductRepository _productRepository;
        public CertainValueCertainProductDiscountRepository(CokeDataContext ctx,
        ICacheProvider cacheProvider,
        IProductRepository productRepository)
        {
            _ctx = ctx;
            _productRepository = productRepository;
            _cacheProvider = cacheProvider;
        }

        public void AddCertainValueCertainProductDiscount(Guid cvcpId, int ProductQuantity, ProductRef ProductRef, decimal CertainValue, DateTime effectiveDate, DateTime endDate)
        {
            CertainValueCertainProductDiscount cvcp = GetById(cvcpId);
            if (cvcp != null)
            {
                cvcp.CertainValueCertainProductDiscountItems.Add(
                    new CertainValueCertainProductDiscount.CertainValueCertainProductDiscountItem(Guid.NewGuid())
                        {
                            Product = new ProductRef {ProductId = ProductRef.ProductId},
                            CertainValue = CertainValue,
                            Quantity = ProductQuantity,
                            EffectiveDate = effectiveDate,
                            EndDate = endDate,
                            _Status = EntityStatus.Active,
                        }
                    );
                Save(cvcp);
            }
            else
            {
                throw new DomainValidationException(this.BasicValidation(), "Certain value certain product discount not set");
            }
        }

        public void EditCertainValueCertainProductDiscount(Guid cvcpId, Guid lineItemId, int productQuantity, ProductRef productRef, decimal certainValue, DateTime effectiveDate, DateTime endDate)
        {
            CertainValueCertainProductDiscount cvcp = GetById(cvcpId);
            cvcp.InitialValue = certainValue;

            CertainValueCertainProductDiscount.CertainValueCertainProductDiscountItem item = cvcp.CertainValueCertainProductDiscountItems.FirstOrDefault(n => n.Id == lineItemId);

            item.EffectiveDate = effectiveDate;
            item.CertainValue = certainValue;
            item.Product = new ProductRef {ProductId = productRef.ProductId};
            item.Quantity = productQuantity;
            item.EndDate = endDate;
            Save(cvcp);
        }

        public CertainValueCertainProductDiscount GetByAmount(decimal Amount)
        {
            var cvcpd = GetAll().OrderByDescending(s=>s.InitialValue)
                .FirstOrDefault(c => Amount >= c.InitialValue && c.CurrentEffectiveDate.Date<=DateTime.Now.Date && c.CurrentEndDate>=DateTime.Now.Date);

            return cvcpd;
        }

        public CertainValueCertainProductDiscount GetByInitialValue(decimal Amount)
        {
            var tblcvcpd = _ctx.tblCertainValueCertainProductDiscount.FirstOrDefault(c => Amount == c.Value && c.IM_Status != (int)EntityStatus.Deleted);
            if (tblcvcpd != null)
                return Map(tblcvcpd);
            return null;
        }

       public QueryResult<CertainValueCertainProductDiscount> Query(QueryStandard query)
        {
            IQueryable<tblCertainValueCertainProductDiscount> cvcpDisQuery;
            IQueryable<tblProduct> productQuery;
            IQueryable<tblCertainValueCertainProductDiscountItem> cvcpDisItemQuery;

            cvcpDisQuery = _ctx.tblCertainValueCertainProductDiscount.AsQueryable();
            productQuery = _ctx.tblProduct.AsQueryable();
            cvcpDisItemQuery = _ctx.tblCertainValueCertainProductDiscountItem.AsQueryable().Where(p => p.IM_Status == (int) EntityStatus.Active);

            //var q = from p in productQuery
            //            join cI in cvcpDisItemQuery on p.id equals cI.Product
            //            join c in cvcpDisQuery on cI.CertainValueCertainDiscountID equals c.id
            //    select new {Product = p, CertainValueCertainProductDiscount = c, CertainValueCertainProductDiscountItem = cI};

            var q = from c in cvcpDisQuery
                    join cI in cvcpDisItemQuery on c.id equals cI.CertainValueCertainDiscountID
                join p in productQuery on cI.Product equals p.id
                    select new { Product = p, CertainValueCertainProductDiscount = c, CertainValueCertainProductDiscountItem = cI };
           
            if (query.ShowInactive)
                q = q.Where(k => k.CertainValueCertainProductDiscount.IM_Status != (int)EntityStatus.Deleted);
           else
                q = q.Where(k => k.CertainValueCertainProductDiscount.IM_Status == (int)EntityStatus.Active);
            
            if (!string.IsNullOrWhiteSpace(query.Name))
                q = q.Where(j => j.Product.Description.ToLower().Contains(query.Name.ToLower()));

            if (query.SupplierId.HasValue)
                q = q.Where(k => k.Product.tblProductBrand.SupplierId == query.SupplierId.Value);
            var queryResult = new QueryResult<CertainValueCertainProductDiscount>();
            queryResult.Count = q.Count();
            
            q = q.OrderBy(l => l.Product.Description);
            queryResult.Count = q.Count();

            if (query.Skip.HasValue && query.Take.HasValue)
                q = q.Skip(query.Skip.Value).Take(query.Take.Value);
            queryResult.Data = q.ToList().Select(p=>Map(p.CertainValueCertainProductDiscount)).ToList();

            return queryResult;
        }

        public CertainValueCertainProductDiscount GetByAmountAndProduct(decimal Amount, Guid productId)
        {
            var cvcpd = GetAll()
                .FirstOrDefault(c => Amount >= c.InitialValue && c.CertainValueCertainProductDiscountItems
                    .Select(ci => ci.Product.ProductId)
                    .Contains(productId));

            return cvcpd;
        }

        public Guid Save(CertainValueCertainProductDiscount entity, bool? isSync = null)
        {
            _log.InfoFormat("Saving/Upadating");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                var discount = entity.CertainValueCertainProductDiscountItems
                    .FirstOrDefault(n => n._Status == EntityStatus.New); 
                entity.CertainValueCertainProductDiscountItems.Remove(discount);
                throw new DomainValidationException(vri, CoreResourceHelper.GetText("hq.cvcp.validation.error"));
            }
            DateTime dt = DateTime.Now;
            tblCertainValueCertainProductDiscount tblCVCP = _ctx.tblCertainValueCertainProductDiscount.FirstOrDefault(n => n.id == entity.Id);
            if (tblCVCP == null)
            {
                tblCVCP = new tblCertainValueCertainProductDiscount();
                tblCVCP.IM_DateCreated = dt;
                tblCVCP.IM_Status = (int)EntityStatus.Active;// true;
                tblCVCP.id = entity.Id;
                _ctx.tblCertainValueCertainProductDiscount.AddObject(tblCVCP);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (tblCVCP.IM_Status != (int)entityStatus)
                tblCVCP.IM_Status = (int)entity._Status;

            tblCVCP.tblCertainValueCertainProductDiscountItem 
                    .Where(n => n.CertainValueCertainDiscountID == entity.Id && n.IM_Status != (int) EntityStatus.Deleted).ToList()
                    .ForEach(n => n.IM_Status = (int) EntityStatus.Deleted);
            foreach (var discountItem in entity.CertainValueCertainProductDiscountItems)
            {
                if (discountItem._Status == EntityStatus.New)
                {
                    var tblcvpI = _ctx.tblCertainValueCertainProductDiscountItem.FirstOrDefault(n => n.id == discountItem.Id);
                    if (tblcvpI == null)
                    {
                        tblcvpI = new tblCertainValueCertainProductDiscountItem
                                      {
                                          id = discountItem.Id,
                                          IM_DateCreated = dt,
                                          EffectiveDate = discountItem.EffectiveDate,
                                          EndDate = discountItem.EndDate,
                                          Product = discountItem.Product.ProductId,
                                          Value = discountItem.CertainValue,
                                          Quantity = discountItem.Quantity,
                                          IM_Status = (int) EntityStatus.Active,
                                          IM_DateLastUpdated = dt,
                                      };
                        tblCVCP.tblCertainValueCertainProductDiscountItem.Add(tblcvpI);
                    }
                    else
                    {
                        tblcvpI.IM_Status = (int)EntityStatus.Active;
                    }
                }
            }

            tblCVCP.IM_DateLastUpdated = dt;
            tblCVCP.Value = entity.InitialValue;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblCertainValueCertainProductDiscount.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, tblCVCP.id));
            return tblCVCP.id;
        }

        public void SetInactive(CertainValueCertainProductDiscount entity)
        {
            tblCertainValueCertainProductDiscount tblcvcp = _ctx.tblCertainValueCertainProductDiscount.FirstOrDefault(n=>n.id==entity.Id);
            if (tblcvcp != null)
            {
                tblcvcp.IM_DateLastUpdated = DateTime.Now;
                tblcvcp.IM_Status = (int)EntityStatus.Inactive;// false;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCertainValueCertainProductDiscount.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblcvcp.id));

            }
            
        }

        public void SetActive(CertainValueCertainProductDiscount entity)
        {
            tblCertainValueCertainProductDiscount tblcvcp = _ctx.tblCertainValueCertainProductDiscount.FirstOrDefault(n => n.id == entity.Id);
            if (tblcvcp != null)
            {
                tblcvcp.IM_DateLastUpdated = DateTime.Now;
                tblcvcp.IM_Status = (int)EntityStatus.Active;// false;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCertainValueCertainProductDiscount.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblcvcp.id));

            }
        }

        public void SetAsDeleted(CertainValueCertainProductDiscount entity)
        {
            var cvcp = GetById(entity.Id);
            if (cvcp != null)
            {
                //cvcp.CertainValueCertainProductDiscountItems.ForEach(i => i._Status = EntityStatus.Deleted);
                cvcp._Status = EntityStatus.Deleted;
                Save(cvcp);
            }
            else
            {
                throw new DomainValidationException(this.BasicValidation(), "Failed discount not set");
            }
        }

        public CertainValueCertainProductDiscount GetById(Guid Id, bool includeDeactivated = false)
        {
            CertainValueCertainProductDiscount entity = (CertainValueCertainProductDiscount)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblCertainValueCertainProductDiscount.FirstOrDefault(s => s.id == Id);
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
            get { return "CertainValueCertainProductDiscount-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "CertainValueCertainProductDiscountList"; }
        }

        public override IEnumerable<CertainValueCertainProductDiscount> GetAll(bool includeDeactivated = false)
        {
            _log.InfoFormat("Get All");
            IList<CertainValueCertainProductDiscount> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<CertainValueCertainProductDiscount>(ids.Count);
                foreach (Guid id in ids)
                {
                    CertainValueCertainProductDiscount entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblCertainValueCertainProductDiscount.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (CertainValueCertainProductDiscount p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        CertainValueCertainProductDiscount Map(tblCertainValueCertainProductDiscount tblcvcp)
        {
            CertainValueCertainProductDiscount cvcp = new CertainValueCertainProductDiscount(tblcvcp.id) 
            {
                InitialValue = tblcvcp.Value,
            
            };
            cvcp.CertainValueCertainProductDiscountItems = tblcvcp.tblCertainValueCertainProductDiscountItem.Where(s => s.IM_Status == (int)EntityStatus.Active)
                .Select(s => new CertainValueCertainProductDiscount.CertainValueCertainProductDiscountItem(s.id, s.IM_DateCreated, s.IM_DateLastUpdated, (EntityStatus)s.IM_Status)
            {
                Quantity = s.Quantity,
                CertainValue = s.Value,
                Product = new ProductRef { ProductId = s.Product },
                EffectiveDate = s.EffectiveDate,
                EndDate = s.EndDate ?? s.EffectiveDate,
                LineItemId = s.id,
                IsActive = (EntityStatus)s.IM_Status
            }).ToList();
            cvcp._SetDateCreated(tblcvcp.IM_DateCreated);
            cvcp._SetDateLastUpdated(tblcvcp.IM_DateLastUpdated);
            cvcp._SetStatus((EntityStatus)tblcvcp.IM_Status);
            return cvcp;
        }

        public ValidationResultInfo Validate(CertainValueCertainProductDiscount itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID."));

            CertainValueCertainProductDiscount.CertainValueCertainProductDiscountItem discount = null;
            discount = itemToValidate.CertainValueCertainProductDiscountItems
                    .LastOrDefault(n => n._Status == EntityStatus.New);
            //.FirstOrDefault(n => n._Status == EntityStatus.Active);
            if (discount != null)
            {
                if (discount.EndDate.Date <  discount.EffectiveDate.Date)
                    vri.Results.Add(new ValidationResult("Invalid date range - End date cannot be lower than start date"));
            }
            return vri;
        }

        public ValidationResultInfo Validate(CertainValueCertainProductDiscount.CertainValueCertainProductDiscountItem itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.EndDate < itemToValidate.EffectiveDate)
                vri.Results.Add(new ValidationResult("Invalid date range - End date cannot be lower than start date"));
            return vri;
        }

        public void DeactivateLineItem(Guid lineItemId)
        {
            var discountlineItem = _ctx.tblCertainValueCertainProductDiscountItem.FirstOrDefault(p => p.id == lineItemId);
            if (discountlineItem != null)
            {
                discountlineItem.IM_Status = (int)EntityStatus.Inactive;//false;
                discountlineItem.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCertainValueCertainProductDiscount.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, discountlineItem.tblCertainValueCertainProductDiscount.id));
            }
        }
    }
}
