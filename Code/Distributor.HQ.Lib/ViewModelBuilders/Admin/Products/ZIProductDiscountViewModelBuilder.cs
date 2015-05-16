using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.Product;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products
{
   public interface ZIProductDiscountViewModelBuilder
    {
       IList<ProductDiscountViewModel> GetAll(bool inactive = false);
       IList<ProductDiscountViewModel> Search(string searchParam, bool inactive = false);
       ProductDiscountViewModel Get(Guid id);
       void Save(ProductDiscountViewModel productDiscountViewModel);
       void AddDiscountItem(Guid vatproductDiscountId, decimal discountRate, DateTime effectiveDate, DateTime endDate);
       void SetInactive(Guid id);
       Dictionary<Guid, string> ProductList();
       Dictionary<Guid, string> TierList();
    }
}
