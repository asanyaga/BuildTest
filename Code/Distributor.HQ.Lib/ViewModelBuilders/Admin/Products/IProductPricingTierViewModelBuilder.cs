using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.Product;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products
{
    public interface IProductPricingTierViewModelBuilder
    {
        IList<ProductPricingTierViewModel> GetAll(bool inactive = false);
        IList<ProductPricingTierViewModel> Search(string searchParam, bool inactive = false);
        ProductPricingTierViewModel Get(Guid id);
        void Save(ProductPricingTierViewModel productPricingTierViewModel);
        void SetInactive(Guid id);
        void SetActive(Guid id);
        void SetAsDeleted(Guid id);
        ProductPricingTierViewModel GetPricingTierSkipTake(bool inactive = false);
        QueryResult<ProductPricingTierViewModel> Query(QueryStandard query);
    }
}
