using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.Product;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products
{
   public interface IEditProductPricingViewModelBuilder
    {
       EditProductPricingViewModel Get(Guid id);
       void Save(EditProductPricingViewModel pItem);
       void AddPricingItem(Guid pricingId, decimal exFactory, decimal sellingPrice, DateTime effectiveDate);
    }
}
