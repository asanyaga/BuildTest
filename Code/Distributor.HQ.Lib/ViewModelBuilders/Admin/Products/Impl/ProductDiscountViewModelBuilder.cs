using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Factory.Master;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products.Impl
{
   public class ProductDiscountViewModelBuilder:IProductDiscountViewModelBuilder
    {
       IProductDiscountRepository _productDiscountRepository;
       IProductPricingTierRepository _productPricingTierRepository;
       IProductRepository _productRepository;
       IProductDiscountFactory _productDiscountFactory;
       public ProductDiscountViewModelBuilder(IProductDiscountRepository productDiscountRepository, IProductRepository productRepository, IProductDiscountFactory productDiscountFactory,IProductPricingTierRepository productPricingTierRepository)
       {
           _productDiscountRepository = productDiscountRepository;
           _productRepository = productRepository;
           _productDiscountFactory = productDiscountFactory;
           _productPricingTierRepository = productPricingTierRepository;
       }
        public IList<ProductDiscountViewModel> GetAll(bool inactive = false)
        {
           return _productDiscountRepository.GetAll(inactive).ToList().Select(n=>Map(n)).ToList();
        }

        public IList<ProductDiscountViewModel> Search(string searchParam, bool inactive = false)
        {
            var foundProductIds = _productRepository.GetAll().Where(n => n.Description.ToLower().Contains(searchParam.ToLower())).Select(n => n.Id).ToArray();
            var foundTierID = _productPricingTierRepository.GetAll().Where(n => n.Name.ToLower().Contains(searchParam.ToLower())).Select(n => n.Id).ToArray();
            return _productDiscountRepository.GetAll(inactive).ToList().Select(n => Map(n)).ToList().Where(s=>(s.discountRate.ToString().StartsWith(searchParam))||(s.effectiveDate.ToString("dd-MMM-yyyy").StartsWith(searchParam))||(foundProductIds.Contains(s.ProductId))||(foundTierID.Contains(s.TierId))).ToList();
        }

        public ProductDiscountViewModel Get(Guid id)
        {
           
            ProductDiscount pd = _productDiscountRepository.GetById(id);
            ProductDiscountViewModel pdvm = new ProductDiscountViewModel
            {
                Id = pd.Id,
                discountRate = pd.CurrentDiscount,
                effectiveDate = pd.CurrentEffectiveDate,
                ProductId = pd.ProductRef.ProductId,
                ProductName = _productRepository.GetById(pd.ProductRef.ProductId).Description,
                isActive = pd._IsActive,
                TierId = pd.Tier.Id,
                TierName = _productPricingTierRepository.GetById(pd.Tier.Id).Name,
                DiscountItemsList = pd.DiscountItems.Select(n => new ProductDiscountViewModel.DiscountItemsVM 
                { 
                 discountRate=n.DiscountRate,
                  effectiveDate=n.EffectiveDate
                }
                ).ToList()
            };
            return pdvm;
        }

        public void Save(ProductDiscountViewModel productDiscountViewModel)
        {

            ProductDiscount pd = _productDiscountRepository.GetById(productDiscountViewModel.Id);
            if (pd == null)
            {
                pd = _productDiscountFactory.CreateProductDiscount(productDiscountViewModel.ProductId, productDiscountViewModel.TierId, productDiscountViewModel.discountRate, productDiscountViewModel.effectiveDate);
                _productDiscountRepository.Save(pd);
            }
            else
            {
                _productDiscountRepository.AddDiscount(productDiscountViewModel.Id, productDiscountViewModel.effectiveDate, productDiscountViewModel.discountRate);
            }
        }

        public void SetInactive(Guid id)
        {
            ProductDiscount pp = _productDiscountRepository.GetById(id);
            _productDiscountRepository.SetInactive(pp);
        }

        public Dictionary<Guid, string> ProductList()
        {
            var productList = _productRepository.GetAll().ToList()
                .Select(n => new { n.Id, n.Description })
                .ToDictionary(n => n.Id, n => n.Description);
        }

        public Dictionary<Guid, string> TierList()
        {
            return _productPricingTierRepository.GetAll().ToList().Select(n => new { n.Id, n.Name }).ToDictionary(n => n.Id, n => n.Name);
        }
        ProductDiscountViewModel Map(ProductDiscount prdDiscount)
        {
            return new ProductDiscountViewModel
            { 
                Id=prdDiscount.Id,
                 
                discountRate = prdDiscount.CurrentDiscount,
                effectiveDate = prdDiscount.CurrentEffectiveDate,
                TierId = prdDiscount.Tier.Id,
                isActive = prdDiscount._IsActive,
                ProductId = prdDiscount.ProductRef.ProductId,
                 TierName=_productPricingTierRepository.GetById(prdDiscount.Tier.Id).Name,
                ProductName=_productRepository.GetById(prdDiscount.ProductRef.ProductId).Description,
                
            };
        }


        public void AddDiscountItem(Guid productDiscountId, decimal discountRate, DateTime effectiveDate)
        {
            ProductDiscount pd = _productDiscountRepository.GetById(productDiscountId);
            _productDiscountRepository.AddDiscount(productDiscountId, effectiveDate, discountRate);
        }


        public Dictionary<Guid, string> ProductListWithoutReturnables()
        {
            return _productRepository.GetAll().ToList().Where(n => (n.GetType() == typeof(SaleProduct)) || (n.GetType() == typeof(ConsolidatedProduct))).Select(n => new { n.Id, n.Description }).ToDictionary(n => n.Id, n => n.Description);
        }
    }
}
