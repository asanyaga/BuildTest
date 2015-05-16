using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master;
using Distributr.Core.Domain.Master.CompetitorManagement;
using Distributr.Core.Repository.Master.CompetitorManagement;
using Distributr.Core.Utility;
using Distributr.Core.Data.EF;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Data.Utility.Caching;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.CompetitorManagement
{
   internal class CompetitorProductsRepository:RepositoryMasterBase<CompetitorProducts>,ICompetitorProductsRepository
    {
       CokeDataContext _ctx;
       ICompetitorRepository _competitorRepository;
       IProductBrandRepository _brandRepository;
       IProductFlavourRepository _flavourRepository;
       IProductPackagingRepository _packagingRepository;
       IProductPackagingTypeRepository _packagingTypeRepository;
       IProductTypeRepository _productTypeRepository;
       ICacheProvider _cacheProvider;

       public CompetitorProductsRepository(CokeDataContext ctx,
            ICompetitorRepository competitorRepository,
       IProductBrandRepository brandRepository,
       IProductFlavourRepository flavourRepository,
       IProductPackagingRepository packagingRepository,
       IProductPackagingTypeRepository packagingTypeRepository,
       IProductTypeRepository productTypeRepository,
           ICacheProvider cacheProvider
              )
       {
           _ctx = ctx;
           _competitorRepository = competitorRepository;
           _brandRepository = brandRepository;
           _flavourRepository = flavourRepository;
           _packagingRepository = packagingRepository;
           _packagingTypeRepository = packagingTypeRepository;
           _productTypeRepository = productTypeRepository;
           _cacheProvider = cacheProvider;
       }

       public Guid Save(CompetitorProducts entity, bool? isSync = null)
        {
            _log.DebugFormat("Saving/Updating Competitor Products");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            } 
            
            if (!vri.IsValid)
            {
                string info = string.Join(",", vri.Results.Select(n => n.ErrorMessage));
                throw new DomainValidationException(vri,"Failed to validate competitor products -->" + info);
            }
            DateTime dt = DateTime.Now;
            tblCompetitorProducts tblCompeProds = _ctx.tblCompetitorProducts.FirstOrDefault(n => n.id == entity.Id); ;
            if (tblCompeProds==null)
            {
                tblCompeProds = new tblCompetitorProducts();
                tblCompeProds.id = entity.Id;
                tblCompeProds.IM_DateCreated = dt;
                tblCompeProds.IM_Status = (int)EntityStatus.Active;
                _ctx.tblCompetitorProducts.AddObject(tblCompeProds);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (tblCompeProds.IM_Status != (int)entityStatus)
                tblCompeProds.IM_Status = (int)entity._Status;
            tblCompeProds.Name = entity.ProductName;
            tblCompeProds.Description = entity.ProductDescription;
            tblCompeProds.CompetitorId = entity.Competitor.Id;
            tblCompeProds.BrandId = entity.Brand.Id;
            tblCompeProds.FlavourId = entity.Flavour.Id;
            tblCompeProds.PackagingTypeId =entity.PackagingType.Id;
            tblCompeProds.ProductTypeId =entity.ProductType.Id;
            tblCompeProds.PackagingId = entity.Packaging.Id;
            tblCompeProds.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblCompetitorProducts.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, tblCompeProds.id));
            return tblCompeProds.id;
            
        }

        public void SetInactive(CompetitorProducts entity)
        {
            tblCompetitorProducts tblCompeProds=_ctx.tblCompetitorProducts.FirstOrDefault(n=>n.id==entity.Id);
            if (tblCompeProds != null)
            {
                tblCompeProds.IM_DateLastUpdated = DateTime.Now;
                tblCompeProds.IM_Status = (int)EntityStatus.Inactive;//false;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCompetitorProducts.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblCompeProds.id));
            }
        }

       public void SetActive(CompetitorProducts entity)
       {
           tblCompetitorProducts tblCompeProds = _ctx.tblCompetitorProducts.FirstOrDefault(n => n.id == entity.Id);
           if (tblCompeProds != null)
           {
               tblCompeProds.IM_DateLastUpdated = DateTime.Now;
               tblCompeProds.IM_Status = (int)EntityStatus.Active;
               _ctx.SaveChanges();
               _cacheProvider.Put(_cacheListKey, _ctx.tblCompetitorProducts.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
               _cacheProvider.Remove(string.Format(_cacheKey, tblCompeProds.id));
           }
       }

       public void SetAsDeleted(CompetitorProducts entity)
       {
           tblCompetitorProducts tblCompeProds = _ctx.tblCompetitorProducts.FirstOrDefault(n => n.id == entity.Id);
           if (tblCompeProds != null)
           {
               tblCompeProds.IM_DateLastUpdated = DateTime.Now;
               tblCompeProds.IM_Status = (int)EntityStatus.Deleted;
               _ctx.SaveChanges();
               _cacheProvider.Put(_cacheListKey, _ctx.tblCompetitorProducts.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
               _cacheProvider.Remove(string.Format(_cacheKey, tblCompeProds.id));
           }
       }

       public CompetitorProducts GetById(Guid Id, bool includeDeactivated = false)
        {
            CompetitorProducts p = (CompetitorProducts)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (p == null)
            {
                var tbl = _ctx.tblCompetitorProducts.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    p = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                }

            }
            return p;
        }

        public ValidationResultInfo Validate(CompetitorProducts itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            //bool hasDuplicateName = GetAll()
            //    .Where(s => s.Id != itemToValidate.Id)
            //    .Any(p => p.CompetitorName == itemToValidate.CompetitorName);
            //if (hasDuplicateName)
            //    vri.Results.Add(new ValidationResult("Duplicate Competitor Name found"));
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            bool hasDuplicateCode = GetAll(true)
                .Where(s => s.Id != itemToValidate.Id)
                .Any(p => p.ProductName == itemToValidate.ProductName);
            if (hasDuplicateCode)
                vri.Results.Add(new ValidationResult("Duplicate Product name found"));
            

            return vri;
        }


       protected override string _cacheKey
       {
           get { return "CompetitorProducts-{0}"; }
       }

       protected override string _cacheListKey
       {
           get { return "CompetitorProductList"; }
       }

       public override IEnumerable<CompetitorProducts> GetAll(bool includeDeactivated = false)
        {
            _log.DebugFormat("Getting All");
            IList<CompetitorProducts> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<CompetitorProducts>(ids.Count);
                foreach (Guid id in ids)
                {
                    CompetitorProducts entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblCompetitorProducts.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (CompetitorProducts p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }
            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;

        }


       public QueryResult<CompetitorProducts> Query(QueryStandard query)
       {
            IQueryable<tblCompetitorProducts> competitorProdQuery;
               if (query.ShowInactive)
                   competitorProdQuery = _ctx.tblCompetitorProducts.Where(p => p.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
            else
                   competitorProdQuery = _ctx.tblCompetitorProducts.Where(p => p.IM_Status == (int)EntityStatus.Active).AsQueryable();

               var queryResult = new QueryResult<CompetitorProducts>();
            if (!string.IsNullOrEmpty(query.Name))
            {
                competitorProdQuery = competitorProdQuery.Where(p => p.Name.ToLower().Contains(query.Name.ToLower())
                                                         || p.Description.ToLower().Contains(query.Name.ToLower()));

            }
            competitorProdQuery = competitorProdQuery.OrderBy(p => p.Name).ThenBy(p => p.Description);
            queryResult.Count = competitorProdQuery.Count();

            if (query.Skip.HasValue && query.Take.HasValue)
                competitorProdQuery = competitorProdQuery.Skip(query.Skip.Value).Take(query.Take.Value);
            queryResult.Data = competitorProdQuery.Select(Map).OfType<CompetitorProducts>().ToList();

            return queryResult;
             
       
       }

       public CompetitorProducts Map(tblCompetitorProducts compeProds)
        {
            CompetitorProducts cmProducts = new CompetitorProducts(compeProds.id)
            {
                 ProductName=compeProds.Name,
                 ProductDescription=compeProds.Description,
                 ProductType=_productTypeRepository.GetById(compeProds.ProductTypeId),
                 Packaging=_packagingRepository.GetById(compeProds.PackagingId),
                 PackagingType=_packagingTypeRepository.GetById(compeProds.PackagingTypeId),
                 Flavour=_flavourRepository.GetById(compeProds.FlavourId),
                  Competitor=_competitorRepository.GetById(compeProds.CompetitorId),
                   Brand =_brandRepository.GetById(compeProds.BrandId)
                   
            };
            cmProducts._SetDateCreated(compeProds.IM_DateCreated);
            cmProducts._SetDateLastUpdated(compeProds.IM_DateLastUpdated);
            cmProducts._SetStatus((EntityStatus)compeProds.IM_Status);
            return cmProducts;
        }
    }
}
