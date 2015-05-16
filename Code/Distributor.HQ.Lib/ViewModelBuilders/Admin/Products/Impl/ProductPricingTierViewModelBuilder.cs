using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products.Impl
{
    public class ProductPricingTierViewModelBuilder : IProductPricingTierViewModelBuilder
    {
        IProductPricingTierRepository _productPricingTierRepository;
        public ProductPricingTierViewModelBuilder(IProductPricingTierRepository productPricingTierRepository)
        {
            _productPricingTierRepository = productPricingTierRepository;
        }

        public IList<ProductPricingTierViewModel> GetAll(bool inactive = false)
        {
           return _productPricingTierRepository.GetAll(inactive).Select(n => Map(n)).ToList();
        }

        public ProductPricingTierViewModel Get(Guid id)
        {
            ProductPricingTier pptVM = _productPricingTierRepository.GetById(id);
            if (pptVM == null) return null;
               
            return Map(pptVM);
        }

        public void Save(ProductPricingTierViewModel productPricingTierViewModel)
        {
            ProductPricingTier pptVM = new ProductPricingTier(productPricingTierViewModel.Id)
            {
                Name = productPricingTierViewModel.Name,
                Code=productPricingTierViewModel.TierCode,
                Description=productPricingTierViewModel.Description
            };
            _productPricingTierRepository.Save(pptVM);
        }

        public void SetInactive(Guid id)
        {
            ProductPricingTier pptVM = _productPricingTierRepository.GetById(id);
            _productPricingTierRepository.SetInactive(pptVM);
        }

        public void SetActive(Guid id)
        {
            ProductPricingTier pptVM = _productPricingTierRepository.GetById(id);
            _productPricingTierRepository.SetActive(pptVM);
        }

        private ProductPricingTierViewModel Map(ProductPricingTier productPricingTier)
        {
            return new ProductPricingTierViewModel
            {
                Id = productPricingTier.Id,
                Name = productPricingTier.Name,
                Description=productPricingTier.Description,
                TierCode=productPricingTier.Code,
                IsActive = productPricingTier._Status == EntityStatus.Active ? true : false
            };
        }


        public IList<ProductPricingTierViewModel> Search(string searchParam, bool inactive = false)
        {
            return _productPricingTierRepository.GetAll(inactive).Select(s => Map(s)).ToList().Where(n => (n.Name.ToLower().StartsWith(searchParam.ToLower())) || (n.TierCode.ToLower().StartsWith(searchParam.ToLower())) || (n.Description.ToLower().StartsWith(searchParam.ToLower()))).ToList();
        }


        public ProductPricingTierViewModel GetPricingTierSkipTake(bool inactive = false)
        {
            throw new NotImplementedException();
        }

        public QueryResult<ProductPricingTierViewModel> Query(QueryStandard query)
        {
            var queryResult = _productPricingTierRepository.Query(query);
            var result = new QueryResult<ProductPricingTierViewModel>();
            result.Count = queryResult.Count;
            result.Data = queryResult.Data.Select(Map).ToList();
            return result;
        }


        public void SetAsDeleted(Guid id)
        {
            ProductPricingTier pptVM = _productPricingTierRepository.GetById(id);
            _productPricingTierRepository.SetAsDeleted(pptVM);
        }
    }
}
