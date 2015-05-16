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
using Distributr.Core.Repository.Master;
using log4net;
using Distributr.Core.Data.Utility.Caching;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.ProductRepositories
{
    internal class ProductFlavourRepository :RepositoryMasterBase<ProductFlavour>, IProductFlavourRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
         IProductBrandRepository _productBrandRepository;

         public ProductFlavourRepository(CokeDataContext ctx, ICacheProvider cacheProvider, IProductBrandRepository productBrandRepository)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _log.Debug("Product_Falvor_ Repository Constractor");
             _productBrandRepository = productBrandRepository;
        }

         public Guid Save(ProductFlavour entity, bool? isSync = null)
        {
            _log.Debug("Saving Product_Flavor");
            //validation
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                _log.Debug(CoreResourceHelper.GetText("hq.subbrand.validation.error"));
                throw new DomainValidationException(vri, CoreResourceHelper.GetText("hq.subbrand.validation.error"));
            }
            tblProductFlavour tbl = _ctx.tblProductFlavour.FirstOrDefault(n => n.id == entity.Id); ;
            DateTime dt = DateTime.Now;
            if (tbl == null)
            {
               // productFlavour._SetDateCreated(DateTime.Now);
               // productFlavour._SetStatus(true);
                tbl = new tblProductFlavour();
                tbl.IM_DateCreated = dt;
                tbl.IM_Status = (int)EntityStatus.Active;//true;
                tbl.id = entity.Id;
             

                _ctx.tblProductFlavour.AddObject(tbl);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (tbl.IM_Status != (int)entityStatus)
                tbl.IM_Status = (int)entity._Status;
             //productFlavour._SetDateLastUpdated(DateTime.Now);
            tbl.code = entity.Code;
            tbl.description = entity.Description;
            tbl.name = entity.Name;
            tbl.BrandId = entity.ProductBrand != null ? entity.ProductBrand.Id : Guid.Empty;
            //tbl.IM_DateCreated = productFlavour._DateCreated;
            tbl.IM_DateLastUpdated = dt;
           // tbl.IM_Status = productFlavour._Status;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblProductFlavour.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, tbl.id));
            return tbl.id;
        }

        protected override string _cacheKey
        {
            get { return "ProductFlavour-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "ProductFlavourList"; }
        }

        public override IEnumerable<ProductFlavour> GetAll(bool includeDeactivated = false)
        {
            _log.Debug("Get all countries");
            IList<ProductFlavour> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<ProductFlavour>(ids.Count);
                foreach (Guid id in ids)
                {
                    ProductFlavour entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblProductFlavour.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (ProductFlavour p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;

        }

        public QueryResult<ProductFlavour> Query(QueryStandard query)
        {
            var joinQuery = (from f in _ctx.tblProductFlavour
                             join b in _ctx.tblProductBrand on f.BrandId equals b.id
                             select new {flavour = f, brand = b});
            if (query.ShowInactive)
                joinQuery = joinQuery.Where(p => p.flavour.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
            else
                joinQuery = joinQuery.Where(s => s.flavour.IM_Status == (int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<ProductFlavour>();
            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                joinQuery = joinQuery
                    .Where(s => s.flavour.name.ToLower().Contains(query.Name.ToLower()) || s.flavour.code.ToLower().Contains(query.Name.ToLower()));
            }

            if (query.SupplierId.HasValue)
            {
                joinQuery = joinQuery.Where(l=>l.brand.SupplierId == query.SupplierId.Value);
            }

            queryResult.Count = joinQuery.Count();
            joinQuery = joinQuery.OrderBy(s => s.flavour.name).ThenBy(s => s.flavour.code);
            if (query.Skip.HasValue && query.Take.HasValue)
                joinQuery = joinQuery.Skip(query.Skip.Value).Take(query.Take.Value);
            var result = joinQuery.ToList();
            queryResult.Data = result.Select(k => Map(k.flavour)).ToList();
            query.ShowInactive = false;
            return queryResult;
        }

  
        public Dictionary<Guid,string> GetByBrandId(Guid brandId)
        {
            return GetAll().ToList().Where(n => n.ProductBrand.Id == brandId).Select(n => new { n.Id, n.Name }).ToDictionary(n=>n.Id,n=>n.Name);
        }


        public void SetInactive(ProductFlavour entity)
        {
            _log.Debug("Set a country as inactive; Delete");
            ValidationResultInfo vri = Validate(entity);
            bool hasProductDepedencies = _ctx.tblProduct.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p=>p.FlavourId==entity.Id);
            bool hasCompetitorProductsDependencies = _ctx.tblCompetitorProducts.Where(c => c.IM_Status ==(int)EntityStatus.Active).Any(cp=>cp.FlavourId==entity.Id);

            if (hasProductDepedencies || hasCompetitorProductsDependencies)
            {
                throw new DomainValidationException(vri, "Cannot deactivate\r\nDependencies found");
            }
            else
            {
                tblProductFlavour flavour = _ctx.tblProductFlavour.FirstOrDefault(p => p.id == entity.Id);
                if (flavour != null)
                {
                    flavour.IM_Status = (int)EntityStatus.Inactive;// false;
                    flavour.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblProductFlavour.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, flavour.id));
                }
               
            } 
           
        }

        public void SetActive(ProductFlavour entity)
        {
            tblProductFlavour flavour = _ctx.tblProductFlavour.FirstOrDefault(p => p.id == entity.Id);
            if (flavour != null)
            {
                flavour.IM_Status = (int) EntityStatus.Active;
                flavour.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblProductFlavour.Where(n=>n.IM_Status != (int)EntityStatus.Deleted).Select(s=>s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, flavour.id));

            }

        }

        public void SetAsDeleted(ProductFlavour entity)
        {
            _log.Debug("Delete Product Flavour");
            ValidationResultInfo vri = Validate(entity);
            bool hasProductDependencies =_ctx.tblProduct.Where(s => s.IM_Status == (int) EntityStatus.Active).Any(p => p.FlavourId == entity.Id);
            bool hasCompetitorProductDependencies =_ctx.tblCompetitorProducts.Where(c => c.IM_Status == (int) EntityStatus.Active).Any(
                    cp => cp.FlavourId == entity.Id);

            if (hasCompetitorProductDependencies || hasProductDependencies)
            {
                throw new DomainValidationException(vri, "Cannot Delete \r\n Dependencies found");
            }
            else
            {
                tblProductFlavour flavour = _ctx.tblProductFlavour.FirstOrDefault(p => p.id == entity.Id);
                if (flavour != null)
                {
                    flavour.IM_Status = (int) EntityStatus.Deleted;
                    flavour.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblProductFlavour.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s =>s.id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, flavour.id));
                }
            }

        }

        public ProductFlavour GetById(Guid Id, bool includeDeactivated = false)
        {
            _log.Debug("Get by sub brand ID");
            ProductFlavour entity = (ProductFlavour)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblProductFlavour.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }


        ProductFlavour Map(tblProductFlavour productFlavour)
        {
            var productBrandId = productFlavour.BrandId ?? Guid.NewGuid();

            var productBrand = _productBrandRepository.GetById(productBrandId) ?? new ProductBrand(productBrandId);
            
                var pf=new ProductFlavour(productFlavour.id)
                              {
                                  Name = productFlavour.name,
                                  Description = productFlavour.description,
                                  Code = productFlavour.code,
                                  ProductBrand =productBrand,
                              };
            pf._SetStatus((EntityStatus)productFlavour.IM_Status);
            pf._SetDateCreated(productFlavour.IM_DateCreated);
            pf._SetDateLastUpdated(productFlavour.IM_DateLastUpdated);
            return pf;
        
        }

        public ValidationResultInfo Validate(ProductFlavour itemToValidate)
        {
            _log.Debug("Validate Product Flavour");
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            bool isDuplicate_name = _ctx.tblProductFlavour.Where(n => n.id != itemToValidate.Id && n.IM_Status != (int)EntityStatus.Deleted).Any(n => n.name == itemToValidate.Name);
            if (isDuplicate_name)
                vri.Results.Add(new ValidationResult(CoreResourceHelper.GetText("hq.subbrand.validation.dupname")));

            bool isDuplicate_code = _ctx.tblProductFlavour.Where(n => n.id != itemToValidate.Id && n.IM_Status != (int)EntityStatus.Deleted).Any(n => n.code == itemToValidate.Code);
            if (isDuplicate_code)
                vri.Results.Add(new ValidationResult(CoreResourceHelper.GetText("hq.subbrand.validation.dupcode")));
           
            return vri;

        }


       

    }
}
