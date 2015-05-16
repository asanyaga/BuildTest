using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Data.EF;
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
    internal class ProductPackagingTypeRepository :RepositoryMasterBase<ProductPackagingType> ,IProductPackagingTypeRepository
    {

        CokeDataContext _cokeDataContext;
          
        ICacheProvider _cacheProvider;
        public ProductPackagingTypeRepository(CokeDataContext cokeDataContext,ICacheProvider cacheProvider )
        {
            _cokeDataContext = cokeDataContext;
            _cacheProvider = cacheProvider;
        }

        public Guid Save(ProductPackagingType entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
                throw new DomainValidationException(vri, "Product Entity Not valid");
            tblProductPackagingType tbl = _cokeDataContext.tblProductPackagingType.FirstOrDefault(n => n.id == entity.Id);
            DateTime dt = DateTime.Now;
            if (tbl == null)
            {
                tbl = new tblProductPackagingType { name = entity.Name};
                tbl.IM_DateCreated = dt;
                tbl.IM_Status = (int)EntityStatus.Active;//true;
                tbl.id = entity.Id;
                _cokeDataContext.tblProductPackagingType.AddObject(tbl);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (tbl.IM_Status != (int)entityStatus)
                tbl.IM_Status = (int)entity._Status;
            tbl.code = entity.Code;
            tbl.IM_DateLastUpdated = dt;
            tbl.name = entity.Name;
            tbl.description = entity.Description;
            _cokeDataContext.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _cokeDataContext.tblProductPackagingType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, tbl.id));
            return tbl.id;

        }

        public void SetInactive(ProductPackagingType entity)
        {
           // string msg = "";
             ValidationResultInfo vri = Validate(entity);
             bool hasProductDependencies = _cokeDataContext.tblProduct
                 .Where(s => s.IM_Status == (int)EntityStatus.Active)
                 .Any(p => p.PackagingTypeId == entity.Id);
             bool hasCompetitorProductsDependencies = _cokeDataContext.tblCompetitorProducts
                 .Where(c => c.IM_Status ==(int)EntityStatus.Active)
                 .Any(cp => cp.PackagingTypeId == entity.Id);
            if (hasProductDependencies || hasCompetitorProductsDependencies)
            {
                throw new DomainValidationException(vri, "Cannot deactivate\r\nDependencies found");
            }
            else
            {
                tblProductPackagingType productPackagingType = _cokeDataContext.tblProductPackagingType.FirstOrDefault(p => p.id == entity.Id);
                if (productPackagingType != null)
                {
                    productPackagingType.IM_Status = (int)EntityStatus.Inactive;//false;
                    productPackagingType.IM_DateLastUpdated = DateTime.Now;
                    _cokeDataContext.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _cokeDataContext.tblProductPackagingType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, productPackagingType.id));
                }
            }
           
        }

        public void SetActive(ProductPackagingType entity)
        {
            tblProductPackagingType ppt = _cokeDataContext.tblProductPackagingType.FirstOrDefault(p => p.id == entity.Id);
            if (ppt != null)
            {
                ppt.IM_Status = (int) EntityStatus.Active;
                ppt.IM_DateLastUpdated = DateTime.Now;
                _cokeDataContext.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _cokeDataContext.tblProductPackagingType.Where(n=>n.IM_Status != (int)EntityStatus.Deleted).Select(s =>s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, ppt.id));
            }
        }

        public void SetAsDeleted(ProductPackagingType entity)
        {
            ValidationResultInfo vri = Validate(entity);
            bool hasProductDependecies =
                _cokeDataContext.tblProduct.Where(s => s.IM_Status == (int) EntityStatus.Active).Any(
                    p => p.PackagingTypeId == entity.Id);
            bool hasCompetitorProductsDependencies =
                _cokeDataContext.tblCompetitorProducts.Where(c => c.IM_Status == (int) EntityStatus.Active).Any(
                    cp => cp.PackagingTypeId == entity.Id);
            if (hasCompetitorProductsDependencies || hasProductDependecies)
            {
                throw new DomainValidationException(vri, "Cannot Delete\r\nDependencies found"); 
            }
            else
            {
                tblProductPackagingType productPackagingType = _cokeDataContext.tblProductPackagingType.FirstOrDefault(p => p.id == entity.Id);
                if (productPackagingType != null)
                {
                    productPackagingType.IM_Status = (int)EntityStatus.Deleted;//false;
                    productPackagingType.IM_DateLastUpdated = DateTime.Now;
                    _cokeDataContext.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _cokeDataContext.tblProductPackagingType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, productPackagingType.id));
                }
            }

        }

        public ProductPackagingType GetById(Guid Id, bool includeDeactivated = false)
        {
            ProductPackagingType entity = (ProductPackagingType)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _cokeDataContext.tblProductPackagingType.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity =tbl.Map();
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;

           
        }

        protected override string _cacheKey
        {
            get { return "ProductPackagingType-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "ProductPackagingTypeList"; }
        }

        public override IEnumerable<ProductPackagingType> GetAll(bool includeDeactivated=false)
        {
            IList<ProductPackagingType> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<ProductPackagingType>(ids.Count);
                foreach (Guid id in ids)
                {
                    ProductPackagingType entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _cokeDataContext.tblProductPackagingType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => s.Map()).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (ProductPackagingType p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

  
        public QueryResult<ProductPackagingType> Query(QueryBase query)
        {
            var q = query as QueryStandard;
            IQueryable<tblProductPackagingType> productPackagingTypeQuery;
            if (q.ShowInactive)
                productPackagingTypeQuery = _cokeDataContext.tblProductPackagingType.Where(p => p.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
            else
                productPackagingTypeQuery = _cokeDataContext.tblProductPackagingType.Where(s => s.IM_Status == (int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<ProductPackagingType>();
            if (!string.IsNullOrWhiteSpace(q.Name))
            {
                productPackagingTypeQuery = productPackagingTypeQuery
                    .Where(s => s.name.ToLower().Contains(q.Name.ToLower()) || s.code.ToLower().Contains(q.Name.ToLower()));
            }

            queryResult.Count = productPackagingTypeQuery.Count();
            productPackagingTypeQuery = productPackagingTypeQuery.OrderBy(s => s.name).ThenBy(s => s.code);
            if (q.Skip.HasValue && q.Take.HasValue)
                productPackagingTypeQuery = productPackagingTypeQuery.Skip(q.Skip.Value).Take(q.Take.Value);
            var result = productPackagingTypeQuery.ToList();
            queryResult.Data = result.Select(Map).OfType<ProductPackagingType>().ToList();
            q.ShowInactive = false;
            return queryResult;
        }

        ProductPackagingType Map(tblProductPackagingType tbl)
        {
            var productPackagingType = new ProductPackagingType(tbl.id)
                {
                    Name = tbl.name,
                    Code = tbl.code,
                    Description = tbl.description
                };
            productPackagingType._SetStatus((EntityStatus)tbl.IM_Status);
            productPackagingType._SetDateCreated(tbl.IM_DateCreated);
            productPackagingType._SetDateLastUpdated(tbl.IM_DateLastUpdated);
           
            return productPackagingType;
        }


        public ValidationResultInfo Validate(ProductPackagingType itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));

            if (string.IsNullOrEmpty(itemToValidate.Name))
            {
               // vri.Results.Add(new ValidationResult("Name is a required field"));
                return vri;
            }
            if (string.IsNullOrEmpty(itemToValidate.Code))
            {
                vri.Results.Add(new ValidationResult("Code is a required field"));
                return vri;
            }

            bool hasDuplicateName = GetAll(true)
                .Where(s => s.Id != itemToValidate.Id)
                .Any(p => p.Name.ToLower() == itemToValidate.Name.ToLower());
            if (hasDuplicateName)
                vri.Results.Add(new ValidationResult(CoreResourceHelper.GetText("hq.packagingtype.validation.dupname")));
            return vri;
        }


       
    }
}
