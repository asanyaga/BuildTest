using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.Discounts;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Discounts
{
    public interface IProductDiscountViewModelBuilder
    {
        IList<ProductDiscountViewModel> GetAll(bool inactive = false);
        IList<ProductDiscountViewModel.ProductDiscountItemViewModel> GetProductDiscountItem(Guid productDiscountId);
        IList<ProductDiscountViewModel> Search(string searchParam, bool inactive = false);
        ProductDiscountViewModel Get(Guid id);
        void Save(ProductDiscountViewModel productDiscountViewModel);
        void AddDiscountItem(Guid productDiscountId, decimal rate, DateTime effectiveDate, DateTime endDate,bool isByQuantity,decimal quantity);
        void SetInactive(Guid id);
        void SetDeleted(Guid id);
        Dictionary<Guid, string> ProductList();
        Dictionary<Guid, string> TierList();
        void DeacivateLineItem(Guid lineItemId);
        string GetProductName(Guid productId);
        void ThrowIfExists(ProductDiscountViewModel pdvm);

        QueryResult<ProductDiscountViewModel> Query(QueryStandard q);
    }
}
