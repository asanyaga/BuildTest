using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.SuppliersEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Data.EF;
using System.Data.EntityClient;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.Resources.Util;
using Distributr.Core.Utility;
using Distributr.Core.Repository.Master;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.ProductRepositories
{
    internal class ProductBrandRepository :RepositoryMasterBase<ProductBrand>, IProductBrandRepository
    {
        CokeDataContext _ctx;       
        ICacheProvider _cacheProvider;
        private readonly ISupplierRepository _supplierRepository;
        private IMessageSourceAccessor _messageSourceAccessor;
        public ProductBrandRepository(CokeDataContext ctx, ICacheProvider cacheProvider, IMessageSourceAccessor messageSourceAccessor, ISupplierRepository supplierRepository)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _messageSourceAccessor = messageSourceAccessor;
            _supplierRepository = supplierRepository;
        }
        public Guid Save(ProductBrand entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            DateTime dt = DateTime.Now;
            if (!vri.IsValid)
            {
                string info = string.Join(",", vri.Results.Select(n => n.ErrorMessage));
                _log.Debug(CoreResourceHelper.GetText("hq.brand.validation.error") + "-->" + info );
                throw new DomainValidationException(vri, CoreResourceHelper.GetText("hq.brand.validation.error"));
            }
            tblProductBrand tblProductBrandToSave = _ctx.tblProductBrand.FirstOrDefault(n => n.id == entity.Id);
            if (tblProductBrandToSave ==null)
            {
                
                tblProductBrandToSave = new tblProductBrand();
                tblProductBrandToSave.IM_DateCreated = dt;
                tblProductBrandToSave.IM_Status = (int)EntityStatus.Active;//true;
                tblProductBrandToSave.id = entity.Id;
                _ctx.tblProductBrand.AddObject(tblProductBrandToSave);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (tblProductBrandToSave.IM_Status != (int)entityStatus)
                tblProductBrandToSave.IM_Status = (int)entity._Status;
              

           
            tblProductBrandToSave.name = entity.Name;
            if (entity.Supplier != null) tblProductBrandToSave.SupplierId = entity.Supplier.Id;
            tblProductBrandToSave.description = entity.Description;
            tblProductBrandToSave.code = entity.Code;
            tblProductBrandToSave.IM_DateLastUpdated = dt;          
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblProductBrand.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, tblProductBrandToSave.id));
            return tblProductBrandToSave.id;
          
        }

       


        public void SetInactive(ProductBrand entity)
        {
            ValidationResultInfo vri = Validate(entity);
            bool hasDependencies = _ctx.tblProduct.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p => p.BrandId == entity.Id);
            bool hasCompetitorProductsDependencies = _ctx.tblCompetitorProducts.Where(s => s.IM_Status ==(int)EntityStatus.Active).Any(d=>d.BrandId==entity.Id);
            if (hasDependencies || hasCompetitorProductsDependencies)
               
            {
                throw new DomainValidationException(vri, "Cannot deactivate\r\nDependencies found");
            }
            else
            {
                tblProductBrand product = _ctx.tblProductBrand.FirstOrDefault(p => p.id == entity.Id);
                if (product != null)
                {
                    product.IM_Status = (int)EntityStatus.Inactive;// false;
                    product.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblProductBrand.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, product.id));
                }
            }
        }

        public void SetActive(ProductBrand entity)
        {
            tblProductBrand product = _ctx.tblProductBrand.FirstOrDefault(p => p.id == entity.Id);
            if (product != null)
            {
                product.IM_Status = (int) EntityStatus.Active;
                product.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblProductBrand.Where(n =>n.IM_Status != (int)EntityStatus.Deleted).Select(s=>s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, product.id));
            }
        }

        public void SetAsDeleted(ProductBrand entity)
        {
            ValidationResultInfo vri = Validate(entity);
            bool hasDependencies = _ctx.tblProduct.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p => p.BrandId == entity.Id);
            bool hasCompetitorProductsDependencies = _ctx.tblCompetitorProducts.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(d => d.BrandId == entity.Id);
            if (hasDependencies || hasCompetitorProductsDependencies)
            {
                throw new DomainValidationException(vri, "Cannot Delete\r\nDependencies found");
            }
            else
            {
                tblProductBrand product = _ctx.tblProductBrand.FirstOrDefault(p => p.id == entity.Id);
                if (product != null)
                {
                    product.IM_Status = (int)EntityStatus.Deleted;// false;
                    product.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblProductBrand.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, product.id));
                }
            }
        }

        public ProductBrand GetById(Guid Id, bool includeDeactivated = false)
        {
            ProductBrand entity = (ProductBrand)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblProductBrand.FirstOrDefault(s => s.id == Id);
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
            get { return "ProductBrand-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "ProductBrandList"; }
        }

        public override IEnumerable<ProductBrand> GetAll(bool includeDeactivated = false)
        {
            IList<ProductBrand> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<ProductBrand>(ids.Count);
                foreach (Guid id in ids)
                {
                    ProductBrand entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblProductBrand.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => s.Map()).OrderBy(i => i.Name).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (ProductBrand p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;

        }

        public QueryResult<ProductBrand> Query(QueryStandard query)
        {
            var q = query as QueryStandard;
            IQueryable<tblProductBrand> productBrandQuery;
            if (q.ShowInactive)
                productBrandQuery = _ctx.tblProductBrand.Where(p => p.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
            else
                productBrandQuery = _ctx.tblProductBrand.Where(s => s.IM_Status == (int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<ProductBrand>();
            if (!string.IsNullOrWhiteSpace(q.Name))
            {
                productBrandQuery = productBrandQuery
                    .Where(s => s.name.ToLower().Contains(q.Name.ToLower()) || s.code.ToLower().Contains(q.Name.ToLower()));
            }

            if (query.SupplierId.HasValue)
                productBrandQuery = productBrandQuery.Where(k => k.SupplierId == query.SupplierId.Value);

            queryResult.Count = productBrandQuery.Count();
            productBrandQuery = productBrandQuery.OrderBy(s => s.name).ThenBy(s => s.code);
            if (q.Skip.HasValue && q.Take.HasValue)
                productBrandQuery = productBrandQuery.Skip(q.Skip.Value).Take(q.Take.Value);
            var result = productBrandQuery.ToList();
            queryResult.Data = result.Select(Map).OfType<ProductBrand>().ToList();
            q.ShowInactive = false;
            return queryResult;
        }

        ProductBrand Map(tblProductBrand tblBrand)
        {
            var supplier = tblBrand.SupplierId.HasValue
                               ? _supplierRepository.GetById(tblBrand.SupplierId.Value)
                               :new Supplier(Guid.NewGuid());
            ProductBrand productBrand = new ProductBrand(tblBrand.id)
                {
                    Name = tblBrand.name,
                    Code = tblBrand.code,
                    Description = tblBrand.description,
                    Supplier = supplier
                };
         
            productBrand._SetDateCreated(tblBrand.IM_DateCreated);
            productBrand._SetDateLastUpdated(tblBrand.IM_DateLastUpdated);
            productBrand._SetStatus((EntityStatus)tblBrand.IM_Status);
            return productBrand;
        }


        public ValidationResultInfo Validate(ProductBrand itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
            {
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            }

            if (string.IsNullOrEmpty(itemToValidate.Name))
            {
                return vri;
            }
            if (string.IsNullOrEmpty(itemToValidate.Code))
            {
                return vri;
            }
           bool hasDuplicateName = GetAll(true)
                .Where(s=>s.Id!=itemToValidate.Id)
                .Any(p => p.Name.ToLower() == itemToValidate.Name.ToLower());
            if (hasDuplicateName)
                vri.Results.Add(new ValidationResult(CoreResourceHelper.GetText("hq.brand.validation.dupname")));
            
            bool hasDuplicateCode = _ctx.tblProductBrand
                .Where(n=>n.id != itemToValidate.Id && n.IM_Status != (int)EntityStatus.Deleted)
                .Count(p => p.code == itemToValidate.Code)>2;
            if (hasDuplicateCode)
                vri.Results.Add(new ValidationResult(CoreResourceHelper.GetText("hq.brand.validation.dupcode")));
            //dependencies

            //bool hasDependencies = _ctx.tblProduct.Where(s=>s.IM_Status==true).Any(p=>p.BrandId==itemToValidate.Id);
            //if (hasDependencies)
            //    vri.Results.Add(new ValidationResult("Dependency Found"));
            //    .Where(s => s.Id != itemToValidate.Id)
            //    .Any(p=>p.Brand.Id==itemToValidate.Id);
            return vri;

        }


       
    }
}
