using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Utility;
using Distributr.Core.Data.Utility.Caching;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Repository.Master;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.ProductRepositories
{
    internal class ProductPackagingRepository : RepositoryMasterBase<ProductPackaging>, IProductPackagingRepository
    {
        CokeDataContext _ctx;
        //private IContainmentRepository _containmentRepository;
        //string _cacheRegion = "product";
        //string _cacheGet = "productpackaging_{0}";
        ICacheProvider _cacheProvider;
        public ProductPackagingRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
        }

        public Guid Save(ProductPackaging entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                throw new DomainValidationException(vri, "Product packaging  Entity Not valid");
            }
            tblProductPackaging tbl = _ctx.tblProductPackaging.FirstOrDefault(n => n.Id == entity.Id);
            DateTime dt = DateTime.Now;
            if (tbl==null)
            {
                tbl = new tblProductPackaging ();
                tbl.IM_DateCreated = dt;
                tbl.IM_Status = (int)EntityStatus.Active;//true;
                tbl.Id = entity.Id;
                _ctx.tblProductPackaging.AddObject(tbl);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (tbl.IM_Status != (int)entityStatus)
                tbl.IM_Status = (int)entity._Status;
            tbl.code = entity.Code;  
            tbl.Name = entity.Name;
            tbl.description = entity.Description;
            if (entity.Containment!=null) {
                tbl.Containment = entity.Containment.Id;
            }
            if (entity.ReturnableProductRef != null)
            {
                tbl.ReturnableProduct = entity.ReturnableProductRef.ProductId;
            }  
            tbl.IM_DateLastUpdated = dt;
            tbl.IM_DateCreated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblProductPackaging.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, tbl.Id));
            return tbl.Id;
        }

       
        public void SetInactive(ProductPackaging entity)
        {
            ValidationResultInfo vri = Validate(entity);
            bool hasProductDependencies = _ctx.tblProduct.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p => p.PackagingId == entity.Id);
            bool hasCompetitorProductsDependencies = _ctx.tblCompetitorProducts.Where(c => c.IM_Status ==(int)EntityStatus.Active).Any(cp => cp.PackagingId == entity.Id);
            if (hasProductDependencies || hasCompetitorProductsDependencies)
            {
                throw new DomainValidationException(vri, "Cannot deactivate\r\nDependencies found");
            }
            else
            {
                tblProductPackaging productPackaging = _ctx.tblProductPackaging.FirstOrDefault(p => p.Id == entity.Id);
                if (productPackaging != null)
                {
                    productPackaging.IM_Status = (int)EntityStatus.Inactive;//false;
                    productPackaging.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblProductPackaging.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, productPackaging.Id));
                }
            }
            
        }

        public void SetActive(ProductPackaging entity)
        {
            tblProductPackaging pp = _ctx.tblProductPackaging.FirstOrDefault(p => p.Id == entity.Id);
            if (pp != null)
            {
                pp.IM_Status = (int) EntityStatus.Active;
                pp.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblProductPackaging.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s=>s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, pp.Id));
            }
        }

        public void SetAsDeleted(ProductPackaging entity)
        {
            ValidationResultInfo vri = Validate(entity);
            bool hasProductDependencies = _ctx.tblProduct.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p => p.PackagingId == entity.Id);
            bool hasCompetitorProductsDependencies = _ctx.tblCompetitorProducts.Where(c => c.IM_Status == (int)EntityStatus.Active).Any(cp => cp.PackagingId == entity.Id);
            if (hasProductDependencies || hasCompetitorProductsDependencies)
            {
                throw new DomainValidationException(vri, "Cannot Delete\r\nDependencies found");
            }
            else
            {
                tblProductPackaging productPackaging = _ctx.tblProductPackaging.FirstOrDefault(p => p.Id == entity.Id);
                if (productPackaging != null)
                {
                    productPackaging.IM_Status = (int)EntityStatus.Deleted;//false;
                    productPackaging.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblProductPackaging.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, productPackaging.Id));
                }
            }
        }

        public ProductPackaging GetById(Guid Id, bool includeDeactivated = false)
        {
            ProductPackaging entity = (ProductPackaging)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblProductPackaging.FirstOrDefault(s => s.Id == Id);
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
            get { return "ProductPackaging-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "ProductPackagingList"; }
        }

        public override IEnumerable<ProductPackaging> GetAll(bool includeDeactivated=false)
        {
           
            IList<ProductPackaging> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<ProductPackaging>(ids.Count);
                foreach (Guid id in ids)
                {
                    ProductPackaging entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblProductPackaging.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => s.Map()).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (ProductPackaging p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

      

        public QueryResult<ProductPackaging> Query(QueryBase query)
        {
            var q = query as QueryStandard;
            IQueryable<tblProductPackaging> productPackagingQuery;
            if (q.ShowInactive)
                productPackagingQuery = _ctx.tblProductPackaging.Where(p => p.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
            else
                productPackagingQuery = _ctx.tblProductPackaging.Where(s => s.IM_Status == (int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<ProductPackaging>();
            if (!string.IsNullOrWhiteSpace(q.Name))
            {
                productPackagingQuery = productPackagingQuery
                    .Where(s => s.Name.ToLower().Contains(q.Name.ToLower()) || s.code.ToLower().Contains(q.Name.ToLower()));
            }

            queryResult.Count = productPackagingQuery.Count();
            productPackagingQuery = productPackagingQuery.OrderBy(s => s.Name).ThenBy(s => s.code);
            if (q.Skip.HasValue && q.Take.HasValue)
                productPackagingQuery = productPackagingQuery.Skip(q.Skip.Value).Take(q.Take.Value);
            var result = productPackagingQuery.ToList();
            queryResult.Data = result.Select(Map).OfType<ProductPackaging>().ToList();
            q.ShowInactive = false;
            return queryResult;
        }

        ProductPackaging Map(tblProductPackaging tblProductPackaging)
        {

            var productPackaging = new ProductPackaging(tblProductPackaging.Id)
                {
                    Name = tblProductPackaging.Name,
                    Code = tblProductPackaging.code,
                    Description = tblProductPackaging.description
                };
            productPackaging._SetStatus((EntityStatus)tblProductPackaging.IM_Status);
            productPackaging._SetDateCreated(tblProductPackaging.IM_DateCreated);
            productPackaging._SetDateLastUpdated(tblProductPackaging.IM_DateLastUpdated);
            return productPackaging;
        }


        public ValidationResultInfo Validate(ProductPackaging itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));

            if (string.IsNullOrEmpty(itemToValidate.Name))
            {
                return vri;
            }
            if (string.IsNullOrEmpty(itemToValidate.Code))
            {
                vri.Results.Add(new ValidationResult("Code is a required field"));
                return vri;
            }
              
            bool hasDuplicateName = GetAll(true)
               .Where(s => s.Id != itemToValidate.Id)
               .Any(p => p.Name.ToString().ToLower() == itemToValidate.Name.ToString().ToLower());
            if (hasDuplicateName)
                vri.Results.Add(new ValidationResult("Duplicate Name found"));
            return vri;

        }

       

    }
}
