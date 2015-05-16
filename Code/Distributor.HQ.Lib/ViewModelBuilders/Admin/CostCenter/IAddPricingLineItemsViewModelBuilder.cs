using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.Product;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter
{
   public interface IAddPricingLineItemsViewModelBuilder
    {
       IList<AddPricingLineItemsViewModel> GetByPricing(Guid vatClassId, bool inactive = false);
       AddPricingLineItemsViewModel GetById(Guid id);
       void AddPricingLineItem(Guid pricingId, DateTime effectiveDate, decimal currentSellingPrice, decimal currentExFactory);
    }
}
