using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.Product;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products.Impl
{
    public class ProductFlavoursViewModelBuilder : IProductFlavoursViewModelBuilder
    {

        IProductFlavourRepository _productFlavourRepository;
        IProductBrandRepository _productBrandRepository;

        public ProductFlavoursViewModelBuilder(IProductFlavourRepository productFlavourRepository, IProductBrandRepository productBrandRepository)
        {
            _productFlavourRepository = productFlavourRepository;
            _productBrandRepository = productBrandRepository;
        }

        public IList<ProductFlavoursViewModel> GetAll(bool inactive=false)
        {
            
            return _productFlavourRepository.GetAll(inactive).ToList().Select(n=>Map(n)).ToList();
        }

        public ProductFlavoursViewModel Get(Guid id)
        {
            ProductFlavour flavour = _productFlavourRepository.GetById(id);
            if (flavour == null) return null;
              
            return Map(flavour);
        }

       
        public void Save(ProductFlavoursViewModel productflavoursviewmodel)
        {
            if ( productflavoursviewmodel.BrandCode!=null)
            {
                ProductBrand pbCode = _productBrandRepository.GetAll().FirstOrDefault(n =>n.Code.TrimStart('0') == productflavoursviewmodel.BrandCode);
                if (pbCode != null)
                {
                    ProductFlavour pf = new ProductFlavour(productflavoursviewmodel.Id)
                    {
                        Name = productflavoursviewmodel.Name,
                        Description = productflavoursviewmodel.Description,
                        Code = productflavoursviewmodel.Code,
                        ProductBrand = _productBrandRepository.GetById(pbCode.Id)
                    };
                    _productFlavourRepository.Save(pf);
                }
                else
                {
                   // throw new Exception("Brand Code_{0}"+productflavoursviewmodel.BrandCode+" "+"Not Available.");
                }
            }
            else
            {
                ProductFlavour pf = new ProductFlavour(productflavoursviewmodel.Id)
                {
                    Name = productflavoursviewmodel.Name,
                    Description = productflavoursviewmodel.Description,
                    Code = productflavoursviewmodel.Code,
                    ProductBrand = _productBrandRepository.GetById(productflavoursviewmodel.BrandId)
                };
                _productFlavourRepository.Save(pf);
            }
        }



      

        public IList<ProductFlavoursViewModel> Search(string searchParam, bool inactive = false)
        {

            return _productFlavourRepository.GetAll(inactive).ToList().Where(n => (n.Code.ToLower().StartsWith(searchParam.ToLower())) || (n.Description.ToLower().StartsWith(searchParam.ToLower())) || (n.Name.ToLower().StartsWith(searchParam.ToLower()))||(n.ProductBrand.Name.ToLower().StartsWith(searchParam.ToLower()))).Select(n => Map(n)).ToList();
        }


        public void SetInactive(Guid id)
        {
            ProductFlavour pf = _productFlavourRepository.GetById(id);
            _productFlavourRepository.SetInactive(pf);
        }

        public void SetActive(Guid id)
        {
            ProductFlavour pf =_productFlavourRepository.GetById(id);
            _productFlavourRepository.SetActive(pf);

        }

        public void SetAsDeleted(Guid id)
        {
            ProductFlavour pf = _productFlavourRepository.GetById(id);
            _productFlavourRepository.SetAsDeleted(pf);
        }

        ProductFlavoursViewModel Map(ProductFlavour productFlavour)
        {
            return new ProductFlavoursViewModel
            {
                Id = productFlavour.Id,
                Name = productFlavour.Name,
                Description = productFlavour.Description,
                Code = productFlavour.Code,
                isActive = productFlavour._Status == EntityStatus.Active ? true : false,
                BrandId = productFlavour.ProductBrand == null ? Guid .Empty: productFlavour.ProductBrand.Id,
                BrandName =productFlavour.ProductBrand==null?"": _productBrandRepository.GetById(productFlavour.ProductBrand.Id).Name
            };

        }



        public Dictionary<Guid, string> GetBrands()
        {
            return _productBrandRepository.GetAll().ToList().Select(n => new { n.Id, n.Name }).ToDictionary(n => n.Id, n => n.Name);
          
        }


        public List<ProductFlavoursViewModel> GetByBrand(Guid brandId, bool inactive = false)
        {
            return _productFlavourRepository.GetAll(inactive).OrderBy(n=>n.Name).ToList().Where(n=>n.ProductBrand.Id==brandId).Select(n => Map(n)).ToList();
        }

        //public ProductFlavoursViewModel GetFlavourSkipTake() { }


        public ProductFlavoursViewModel GetFlavourSkipTake(bool inactive = false)
        {
            throw new NotImplementedException();
        }

        public QueryResult<ProductFlavoursViewModel> Query(QueryStandard query)
        {
            var queryResult = _productFlavourRepository.Query(query);
            var result = new QueryResult<ProductFlavoursViewModel>();
            result.Count = queryResult.Count;
            result.Data = queryResult.Data.Select(Map).ToList();
            return result;
        }
    }
}
