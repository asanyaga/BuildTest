using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.Product;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products
{
    public interface IPromotionDiscountViewModelBuilder
    {
        PromotionDiscountViewModel Get(Guid id);
        void Save(PromotionDiscountViewModel focDiscount);
        void SetDeleted(Guid id);
        List<PromotionDiscountViewModel> GetAll(bool inactive = false);
        List<PromotionDiscountViewModel> Search(string srcParam, bool inactive = false);
        Dictionary<Guid, string> ProductList();
        void DeacivateLineItem(Guid lineItemId);

        void AddFreeOfChargeDiscount(Guid focId, int parentProductQuantity, Guid freeOfChargeProduct,
                                     int freeOfChargeQuantity, DateTime effectiveDate, decimal DiscountRate,
                                     DateTime endDate);

        void ThrowIfExists(PromotionDiscountViewModel vm);

        QueryResult<PromotionDiscountViewModel> Query(QueryStandard q);
    }
}
