using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Factory.Master;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products.Impl
{
   public class EditProductPricingViewModelBuilder:IEditProductPricingViewModelBuilder
    {
       IProductPricingRepository _productPricingRepository;
       IProductPricingFactory _productPricingFactory;
       IProductRepository _productRepository;
       public EditProductPricingViewModelBuilder(IProductPricingRepository productPricingRepository, IProductPricingFactory productPricingFactory, IProductRepository productRepository)
       {
           _productPricingRepository = productPricingRepository;
           _productPricingFactory = productPricingFactory;
           _productRepository = productRepository;
       }
       public EditProductPricingViewModel Get(Guid id)
        {
            ProductPricing pPricing = _productPricingRepository.GetById(id);
            EditProductPricingViewModel ePV = new EditProductPricingViewModel
            {
                  Id=pPricing.Id,
                  Active = pPricing._Status == EntityStatus.Active ? true : false,
                    TierId=pPricing.Tier.Id,
                      ProductId=pPricing.ProductRef.ProductId,
                      ProductName=_productRepository.GetById(pPricing.ProductRef.ProductId).Description,
                TierName = pPricing.Tier.Name,
                        PItems=pPricing.ProductPricingItems.Select(n=>new EditProductPricingViewModel.PricingItemVM
                         {
                          CurrentEffectiveDate=n.EffectiveDate,
                           CurrentExFactory=n.ExFactoryRate,
                            CurrentSellingPrice=n.SellingPrice
                         }
                        ).ToList()
             };
            return ePV;
        }

        public void Save(EditProductPricingViewModel pItem)
        {
            Guid id = pItem.Id;
            ProductPricing pPrice;
            pPrice = _productPricingRepository.GetById(id);
           
        }

        public void AddPricingItem(Guid pricingId, decimal exFactory, decimal sellingPrice, DateTime effectiveDate)
        {
           // ProductPricing pP = _productPricingRepository.GetById(pricingId);
           //// _productPricingRepository.AddProductPricing(pP, exFactory, sellingPrice, effectiveDate);
           // ProductPricing pp = _productPricingRepository.GetById(pricingId);
            _productPricingRepository.AddProductPricing(pricingId,exFactory,sellingPrice,effectiveDate);
        }
    }
}
