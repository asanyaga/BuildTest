using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.Product;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products
{
   public interface IProductPricingViewModelBuilder
    {
       IList<ProductPricingViewModel> GetAll(bool inactive = false);
       IList<ProductPricingViewModel>Search(string searchParam,bool inactive=false);
       ProductPricingViewModel Get(Guid id);
       void Save(ProductPricingViewModel productPricingViewModel);
       void SetInactive(Guid id);
       void SetAsDeleted(Guid id);
       void SetActive(Guid id);
       Dictionary<Guid, string> ProductList();
       Dictionary<Guid, string> TierList();
        ProductPricingViewModel SearchPricing(string searchParam,bool inactive=false);

       QueryResult<ProductPricingViewModel> Query(QueryStandard q);
    }
}
