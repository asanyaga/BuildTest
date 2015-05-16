using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.HQ.Lib.Validation;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Validation;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter.Impl
{
   public class AddPricingLineItemsViewModelBuilder:IAddPricingLineItemsViewModelBuilder
    {
       IProductPricingRepository _productPricingRepository;
       public AddPricingLineItemsViewModelBuilder(IProductPricingRepository productPricingRepository)
       {
           _productPricingRepository = productPricingRepository;
       }
        public IList<AddPricingLineItemsViewModel> GetByPricing(Guid vatClassId, bool inactive = false)
        {
            throw new NotImplementedException();
        }

        public void AddPricingLineItem(Guid pricingId, DateTime effectiveDate, decimal currentSellingPrice, decimal currentExFactory)
        {
            ProductPricing ppr = _productPricingRepository.GetById(pricingId);
            ValidationResultInfo vri = ppr.BasicValidation();
            if (ppr.ProductPricingItems.Any(p => p.EffectiveDate > effectiveDate))
            {
                vri.Results.Add(new ValidationResult("A past date exists"));
                throw new DomainValidationException(vri,"A past date exists");
            }
            else
            {
                _productPricingRepository.AddProductPricing(ppr.Id, currentExFactory, currentSellingPrice, effectiveDate);
            }
        }


        public AddPricingLineItemsViewModel GetById(Guid id)
        {
            ProductPricing ppr=_productPricingRepository.GetById(id);
            AddPricingLineItemsViewModel alvm = new AddPricingLineItemsViewModel
            {
                id = ppr.Id,
                CurrentEffectiveDate = ppr.CurrentEffectiveDate,
                CurrentExFactory = ppr.CurrentExFactory,
                CurrentSellingPrice = ppr.CurrentSellingPrice,
                
            };
            return alvm;

        }
    }
}
