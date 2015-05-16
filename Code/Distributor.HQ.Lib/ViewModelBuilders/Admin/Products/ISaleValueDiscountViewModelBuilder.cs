using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.Product;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products
{
    public interface ISaleValueDiscountViewModelBuilder
    {
        IList<SaleValueDiscountViewModel> GetAll(bool inactive = false);
        IList<SaleValueDiscountViewModel> Search(string searchParam, bool inactive = false);
        SaleValueDiscountViewModel Get(Guid id);
        void Save(SaleValueDiscountViewModel saleValueDiscountViewModel);

        void AddSaleValueDiscountItem(Guid saleValueDiscountId, decimal discountRate, decimal saleValue,
                                      DateTime effectiveDate, DateTime endDate);
        void SetAsDeleted(Guid id);
        void SetInactive(Guid id);
        void SetActive(Guid id);
        void DeacivateLineItem(Guid lineItemId);
        Dictionary<Guid, string> TierList();
        void ThrowIfExists(SaleValueDiscountViewModel svdvm);

        QueryResult<SaleValueDiscountViewModel> Query(QueryStandard q);
    }
}
