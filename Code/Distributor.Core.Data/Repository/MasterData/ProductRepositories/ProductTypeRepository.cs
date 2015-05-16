using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Utility;
using Distributr.Core.Repository.Master;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;


namespace Distributr.Core.Data.Repository.MasterData.ProductRepositories
{
    internal class ProductTypeRepository :RepositoryMasterBase<ProductType>, IProductTypeRepository
    {
        
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        public ProductTypeRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _log.Debug("Product Type Repository Constructor Bootstrap");
        }



        public Guid Save(ProductType entity, bool? isSync = null)
        {
            tblProductType pt = _ctx.tblProductType.FirstOrDefault(n => n.id == entity.Id);
            DateTime dt = DateTime.Now;
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                _log.Debug("Product Type not valid");
                throw new DomainValidationException(vri, "Product Type Entity Not valid");
            }
            if (pt == null)
            {
               
                pt = new tblProductType();
                pt.IM_Status = (int)EntityStatus.Active;// true;
                pt.IM_DateCreated = dt;
                pt.id = entity.Id;
                _ctx.tblProductType.AddObject(pt);
            }
            else
            {
              
                
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (pt.IM_Status != (int)entityStatus)
                pt.IM_Status = (int)entity._Status;
            pt.name = entity.Name;
            pt.Description = entity.Description;
            pt.code = entity.Code;
            pt.IM_DateLastUpdated = dt;          
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblProductType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, pt.id));

            return pt.id;
        }

        public void SetAsDeleted(ProductType entity)
        {
            ValidationResultInfo vri = Validate(entity);
            bool hasProductDependecies =_ctx.tblProduct.Where(s => s.IM_Status != (int) EntityStatus.Active).Any(p => p.ProductTypeId == entity.Id);
            bool hasCompetitorProductsDependencies =_ctx.tblCompetitorProducts.Where(c => c.IM_Status == (int) EntityStatus.Active).Any(cp => cp.ProductTypeId == entity.Id);
            if (hasCompetitorProductsDependencies || hasProductDependecies)
            {
                throw  new DomainValidationException(vri,"Cannot Delete\r\nDependencies found");
            }
            else
            {
                tblProductType pt = _ctx.tblProductType.FirstOrDefault(p => p.id == entity.Id);
                if (pt != null)
                {
                    pt.IM_Status = (int) EntityStatus.Deleted;
                    pt.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblProductType.Where(n=>n.IM_Status !=(int)EntityStatus.Deleted).Select(s=>s.id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, pt.id));
                }
            }
        }

        public ProductType GetById(Guid Id, bool includeDeactivated = false)
        {
            ProductType entity = (ProductType)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblProductType.FirstOrDefault(s => s.id == Id);
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
            get { return "ProductType-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "ProductTypeList"; }
        }

        public override IEnumerable<ProductType> GetAll(bool includeDeactivated=false)
        {
            IList<ProductType> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<ProductType>(ids.Count);
                foreach (Guid id in ids)
                {
                    ProductType entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblProductType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => s.Map()).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (ProductType p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;

        }

        public QueryResult<ProductType> Query(QueryBase query)
        {
            var q = query as QueryStandard;
            IQueryable<tblProductType> productTypeQuery;
            if (q.ShowInactive)
                productTypeQuery = _ctx.tblProductType.Where(p => p.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
            else
                productTypeQuery = _ctx.tblProductType.Where(s => s.IM_Status == (int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<ProductType>();
            if (!string.IsNullOrWhiteSpace(q.Name))
            {
                productTypeQuery = productTypeQuery
                    .Where(s => s.name.ToLower().Contains(q.Name.ToLower()) || s.code.ToLower().Contains(q.Name.ToLower()));
            }

            queryResult.Count = productTypeQuery.Count();
            productTypeQuery = productTypeQuery.OrderBy(s => s.name).ThenBy(s => s.code);
            if (q.Skip.HasValue && q.Take.HasValue)
                productTypeQuery = productTypeQuery.Skip(q.Skip.Value).Take(q.Take.Value);
            var result = productTypeQuery.ToList();
            queryResult.Data = result.Select(Map).OfType<ProductType>().ToList();
            q.ShowInactive = false;
            return queryResult;
        }

        ProductType Map(tblProductType tblProd)
        {
            var productType = new ProductType(tblProd.id)
                {
                    Name = tblProd.name,
                    Code = tblProd.code,
                    Description = tblProd.Description
                };

            productType._SetDateCreated(tblProd.IM_DateCreated);
            productType._SetDateLastUpdated(tblProd.IM_DateLastUpdated);
            productType._SetStatus((EntityStatus)tblProd.IM_Status);
            return productType;
        }

        public void SetInactive(ProductType entity)
        {
            ValidationResultInfo vri = Validate(entity);
             bool hasProductDependencies = _ctx.tblProduct.Where(s => s.IM_Status ==(int)EntityStatus.Active).Any(p => p.ProductTypeId == entity.Id);
             bool hasCompetitorProductsDependencies = _ctx.tblCompetitorProducts.Where(c => c.IM_Status ==(int)EntityStatus.Active).Any(cp => cp.ProductTypeId == entity.Id);
             if (hasProductDependencies || hasCompetitorProductsDependencies)
             {
                 throw new DomainValidationException(vri, "Cannot deactivate\r\nDependencies found");
             }
             else
             {
                 tblProductType productType = _ctx.tblProductType.FirstOrDefault(p => p.id == entity.Id);
                 if (productType != null)
                 {
                     productType.IM_Status = (int)EntityStatus.Inactive;//false;
                     productType.IM_DateLastUpdated = DateTime.Now;
                     _ctx.SaveChanges();
                     _cacheProvider.Put(_cacheListKey, _ctx.tblProductType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                     _cacheProvider.Remove(string.Format(_cacheKey, productType.id));
                 }
             }
          
           


        }

        public void SetActive(ProductType entity)
        {
            tblProductType productType = _ctx.tblProductType.FirstOrDefault(p => p.id == entity.Id);
            if (productType != null)
            {
                productType.IM_Status = (int) EntityStatus.Active;
                productType.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey,_ctx.tblProductType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s =>s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, productType.id));
            }
        }

        #region IValidation<ProductType> Members

        public ValidationResultInfo Validate(ProductType itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            if(string.IsNullOrEmpty(itemToValidate.Name))
            {
                vri.Results.Add(new ValidationResult("Name is a Required Field"));
                return vri;
            }

            if (string.IsNullOrEmpty(itemToValidate.Code))
            {
                vri.Results.Add(new ValidationResult("Code is a Required Field"));
                return vri;
            }
           bool hasDuplicateName = GetAll(true)
               .Where(s => s.Id != itemToValidate.Id)
              .Any(p => p.Name.ToLower() == itemToValidate.Name.ToLower());
            if (hasDuplicateName)
                vri.Results.Add(new ValidationResult("Duplicate Name found"));

            return vri;
        }

        #endregion

        
    }
}
