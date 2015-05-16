using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Factory.Master;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products.Impl
{
   public class ZProductDiscountViewModelBuilder:ZIProductDiscountViewModelBuilder
    {
       IProductDiscountRepository _productDiscountRepository;
       IProductPricingTierRepository _productPricingTierRepository;
       IProductRepository _productRepository;
       IProductDiscountFactory _productDiscountFactory;
       public ZProductDiscountViewModelBuilder(IProductDiscountRepository productDiscountRepository, IProductRepository productRepository, IProductDiscountFactory productDiscountFactory, IProductPricingTierRepository productPricingTierRepository)
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
            return
                GetAll().Where(s =>(s.DiscountRate.ToString().StartsWith(searchParam)) 
                    ||s.EffectiveDate.ToString("dd-MMM-yyyy").StartsWith(searchParam)
                    ||s.TierName.ToString().ToLower().StartsWith(searchParam.ToLower())
                    ||s.ProductName.ToString().ToLower().StartsWith(searchParam.ToLower())
                    ).ToList();
           
        }

        public ProductDiscountViewModel Get(Guid id)
        {
           
            ProductDiscount pd = _productDiscountRepository.GetById(id);
            ProductDiscountViewModel pdvm = new ProductDiscountViewModel
            {
                Id = pd.Id,
                DiscountRate = pd.CurrentDiscountRate(0),
                EffectiveDate = pd.CurrentEffectiveDate(0),
                ProductId = pd.ProductRef.ProductId,
                ProductName = _productRepository.GetById(pd.ProductRef.ProductId).Description,
                isActive = pd._Status == EntityStatus.Active ? true : false,
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
                pd = _productDiscountFactory.CreateProductDiscount(productDiscountViewModel.ProductId, productDiscountViewModel.TierId, productDiscountViewModel.DiscountRate, productDiscountViewModel.EffectiveDate, productDiscountViewModel.EndDate,false,0);
                _productDiscountRepository.Save(pd);
            }
            else
            {
                _productDiscountRepository.AddDiscount(productDiscountViewModel.Id, productDiscountViewModel.EffectiveDate, productDiscountViewModel.DiscountRate, productDiscountViewModel.EndDate,false,0);
            }
        }

        public void SetInactive(Guid id)
        {
            ProductDiscount pp = _productDiscountRepository.GetById(id);
            _productDiscountRepository.SetInactive(pp);
        }

        public Dictionary<Guid, string> ProductList()
        {
            return _productRepository.GetAll().ToList().Select(n => new { n.Id, n.Description }).ToDictionary(n => n.Id, n => n.Description);
        }

        public Dictionary<Guid, string> TierList()
        {
            return _productPricingTierRepository.GetAll().ToList().Select(n => new { n.Id, n.Name }).ToDictionary(n => n.Id, n => n.Name);
        }
        ProductDiscountViewModel Map(ProductDiscount prdDiscount)
        {
            var tier=_productPricingTierRepository.GetById(prdDiscount.Tier.Id);
            var product = _productRepository.GetById(prdDiscount.ProductRef.ProductId);
          
            return new ProductDiscountViewModel
            { 
                Id=prdDiscount.Id,
                 
                DiscountRate = prdDiscount.CurrentDiscountRate(0),
                EffectiveDate = prdDiscount.CurrentEffectiveDate(0),
                TierId = prdDiscount.Tier.Id,
                isActive = prdDiscount._Status == EntityStatus.Active ? true : false,
                ProductId = prdDiscount.ProductRef.ProductId,
                 TierName=tier!=null? tier.Name:"",
                ProductName = product != null?product.Description:"",
                
            };
        }


        public void AddDiscountItem(Guid productDiscountId, decimal discountRate, DateTime effectiveDate, DateTime endDate)
        {
            ProductDiscount pd = _productDiscountRepository.GetById(productDiscountId);
            _productDiscountRepository.AddDiscount(productDiscountId, effectiveDate, discountRate, endDate,false,0);
        }
    }
}
