using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.CompetitorViewModel;
using Distributr.Core.Repository.Master.CompetitorManagement;
using Distributr.Core.Domain.Master.CompetitorManagement;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CompetitorViewModelBuilders.Impl
{
   public class CompetitorProductsViewModelBuilder:ICompetitorProductsViewModelBuilder
    {
       ICompetitorRepository _competitorRepository;
       ICompetitorProductsRepository _competitorProductsRepository;
       IProductBrandRepository _productBrandRepository;
       IProductFlavourRepository _flavourRepository;
       IProductPackagingRepository _packagingRepository;
       IProductPackagingTypeRepository _packagingTypeRepository;
       IProductTypeRepository _productTypeRepository;

       public CompetitorProductsViewModelBuilder(ICompetitorProductsRepository competitorProductsRepository,
       IProductBrandRepository productBrandRepository,
       IProductFlavourRepository flavourRepository,
       IProductPackagingRepository packagingRepository,
       IProductPackagingTypeRepository packagingTypeRepository,
       IProductTypeRepository productTypeRepository,
           ICompetitorRepository competitorRepository
           )
       {
           _competitorProductsRepository = competitorProductsRepository;
           _productBrandRepository = productBrandRepository;
           _competitorRepository = competitorRepository;
           _productTypeRepository = productTypeRepository;
           _flavourRepository =flavourRepository;
           _packagingRepository = packagingRepository;
           _packagingTypeRepository = packagingTypeRepository;

       }


       public List<CompetitorProductsViewModel> GetAll(bool inactive = false)
       {
           var items = _competitorProductsRepository.GetAll(inactive);
           return items
               .Select(n => new CompetitorProductsViewModel 
               {
                Brand=n.Brand.Id,
                BrandName=n.Brand.Name,
                 Flavour=n.Flavour.Id,
                 FlavourName=n.Flavour.Name,
                 Packaging=n.Packaging.Id,
                 PackagingName=n.Packaging.Name,
                 PackagingType=n.PackagingType.Id,
          PackagingTypeName=n.PackagingType.Name,
           Competitor=n.Competitor.Id,
           CompetitorName=n.Competitor.Name,
            ProductDescription=n.ProductDescription,
             ProductName=n.ProductName,
              Type=n.ProductType.Id,
              TypeName=n.ProductType.Name,
              isActive=n._Status==EntityStatus.Active?true:false,
              Id=n.Id
               }
               ).ToList();
       }

       public List<CompetitorProductsViewModel> Search(string srchParam, bool inactive = false)
       {
           var items = _competitorProductsRepository.GetAll(inactive).Where(n => (n.Brand.Name.ToLower().StartsWith(srchParam.ToLower())) || (n.ProductType.Name.ToLower().StartsWith(srchParam.ToLower())) || (n.Packaging.Name.ToLower().StartsWith(srchParam.ToLower())) || (n.PackagingType.Name.ToLower().StartsWith(srchParam.ToLower())) || (n.ProductName.ToLower().StartsWith(srchParam.ToLower())) || (n.ProductDescription.ToLower().StartsWith(srchParam.ToLower())) || (n.Competitor.Name.ToLower().StartsWith(srchParam.ToLower())) || (n.Flavour.Name.ToLower().StartsWith(srchParam.ToLower())));
           return items
               .Select(n => new CompetitorProductsViewModel
               {
                   Brand = n.Brand.Id,
                   BrandName = n.Brand.Name,
                   Flavour = n.Flavour.Id,
                   FlavourName = n.Flavour.Name,
                   Packaging = n.Packaging.Id,
                   PackagingName = n.Packaging.Name,
                   PackagingType = n.PackagingType.Id,
                   PackagingTypeName = n.PackagingType.Name,
                   Competitor = n.Competitor.Id,
                   CompetitorName = n.Competitor.Name,
                   ProductDescription = n.ProductDescription,
                   ProductName = n.ProductName,
                   Type = n.ProductType.Id,
                   TypeName = n.ProductType.Name,
                   isActive = n._Status == EntityStatus.Active ? true : false,
                   Id = n.Id
               }
               ).ToList();
       }

       public void Save(CompetitorProductsViewModel cvm)
       {
           CompetitorProducts cProd = new CompetitorProducts(cvm.Id) 
           {
           Brand=_productBrandRepository.GetById(cvm.Brand),
           Flavour=_flavourRepository.GetById(cvm.Flavour),
           PackagingType=_packagingTypeRepository.GetById(cvm.PackagingType),
           Packaging=_packagingRepository.GetById(cvm.Packaging),
            Competitor=_competitorRepository.GetById(cvm.Competitor),
             ProductDescription=cvm.ProductDescription,
             ProductName=cvm.ProductName,
              ProductType=_productTypeRepository.GetById(cvm.Type)
           };
           _competitorProductsRepository.Save(cProd);
       }

       public void SetInactive(Guid id)
       {
           CompetitorProducts cProds = _competitorProductsRepository.GetById(id);
           _competitorProductsRepository.SetInactive(cProds);
       }

       public CompetitorProductsViewModel GetById(Guid id)
       {
           CompetitorProducts compeProds = _competitorProductsRepository.GetById(id);
           if (compeProds == null) return null;
              
           return Map(compeProds);

       }
       CompetitorProductsViewModel Map(CompetitorProducts cProds)
       {
           CompetitorProductsViewModel competitorProduct = new CompetitorProductsViewModel();
           {
               competitorProduct.Id = cProds.Id;
               if(cProds.Brand !=null)
               competitorProduct.Brand = cProds.Brand.Id;
               
               if(cProds.Brand !=null)
               competitorProduct.BrandName = cProds.Brand.Name;
               if (cProds.Flavour  != null)
              competitorProduct. Flavour = cProds.Flavour.Id;
               if (cProds.Flavour != null)
              competitorProduct. FlavourName = cProds.Flavour.Name;
               if (cProds.Packaging  != null)
             competitorProduct.  Packaging = cProds.Packaging.Id;
               if (cProds.Packaging != null)
              competitorProduct. PackagingName = cProds.Packaging.Name;
               if (cProds.PackagingType  != null)
             competitorProduct.  PackagingType = cProds.PackagingType.Id;
               if (cProds.PackagingType != null)
              competitorProduct. PackagingTypeName = cProds.PackagingType.Name;
               if (cProds.Competitor  != null)
             competitorProduct.  Competitor = cProds.Competitor.Id;
               if (cProds.Competitor != null)
             competitorProduct.  CompetitorName = cProds.Competitor.Name;
               if (cProds.ProductDescription  != null)
              competitorProduct. ProductDescription = cProds.ProductDescription;
               if (cProds.ProductName  != null)
              competitorProduct. ProductName = cProds.ProductName;
               if (cProds.ProductType  != null)
              competitorProduct. Type = cProds.ProductType.Id;
               if (cProds.ProductType != null)
              competitorProduct. TypeName = cProds.ProductType.Name;
               competitorProduct.isActive = cProds._Status == EntityStatus.Active ? true : false;
           
           }
           return competitorProduct;
           //return new CompetitorProductsViewModel()
           //{
           //    Brand = cProds.Brand.Id,
           //    BrandName = cProds.Brand.Name,
           //    Flavour = cProds.Flavour.Id,
           //    FlavourName = cProds.Flavour.Name,
           //    Packaging = cProds.Packaging.Id,
           //    PackagingName = cProds.Packaging.Name,
           //    PackagingType = cProds.PackagingType.Id,
           //    PackagingTypeName = cProds.PackagingType.Name,
           //    Competitor = cProds.CompetitorName.Id,
           //    CompetitorName = cProds.CompetitorName.Name,
           //    ProductDescription = cProds.ProductDescription,
           //    ProductName = cProds.ProductName,
           //    Type = cProds.ProdType.Id,
           //    TypeName = cProds.ProdType.Name,
           //    isActive=cProds._Status,
           //    Id=cProds.Id
           //};
       }


       public Dictionary<Guid, string> GetCompetitor()
       {
           return _competitorRepository.GetAll().Select(n => new { n.Id, n.Name }).ToList().ToDictionary(n=>n.Id,n=>n.Name);
       }

       public Dictionary<Guid, string> GetBrand()
       {
           return _productBrandRepository.GetAll().Select(n => new { n.Id, n.Name }).ToList().ToDictionary(n=>n.Id,n=>n.Name);
       }

       public Dictionary<Guid, string> GetFlavour()
       {
           return _flavourRepository.GetAll().Select(n => new { n.Id, n.Name }).ToList().ToDictionary(n=>n.Id,n=>n.Name);
       }

       public Dictionary<Guid, string> GetPackType()
       {
           return _packagingTypeRepository.GetAll().Select(n => new { n.Id, n.Name }).ToList().ToDictionary(n => n.Id, n => n.Name);
       }

       public Dictionary<Guid, string> GetProdType()
       {
           return _productTypeRepository.GetAll().Select(n => new { n.Id, n.Name }).ToList().ToDictionary(n => n.Id, n => n.Name);
       }

       public Dictionary<Guid, string> GetPackaging()
       {
           return _packagingRepository.GetAll().Select(n => new { n.Id, n.Name }).ToList().ToDictionary(n => n.Id, n => n.Name);
       }

       public QueryResult<CompetitorProductsViewModel> Query(QueryStandard query)
       {
           var queryResult = _competitorProductsRepository.Query(query);
           var results = new QueryResult<CompetitorProductsViewModel>();
           results.Count = queryResult.Count;
           results.Data = queryResult.Data.Select(Map).ToList();
           return results;
       }

       /*public QueryResult<CompetitorProductsViewModel> Query(QueryStandard query)
       {
           var queryResult = _competitorRepository.Query(query);
           var results = new QueryResult<CompetitorProductsViewModel>();
           results.Count = queryResult.Count;
           results.Data = queryResult.Data.Select(Map).ToList();
           return results;

       }
*/

       public void SetActive(Guid id)
       {
           CompetitorProducts cProds = _competitorProductsRepository.GetById(id);
           _competitorProductsRepository.SetActive(cProds);
       }

       public void SetAsDeleted(Guid id)
       {
           CompetitorProducts cProds = _competitorProductsRepository.GetById(id);
           _competitorProductsRepository.SetAsDeleted(cProds);
       }
    }
}
