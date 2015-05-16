using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.Product;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products
{
    public interface ICertainValueCertainProductDiscountViewModelBuilder
    {
        CertainValueCertainProductDiscountViewModel Get(Guid id);
        void Save(CertainValueCertainProductDiscountViewModel focDiscount);
        void SetInactive(Guid id);
        void SetActive(Guid id);
        void SetDeleted(Guid id);
        List<CertainValueCertainProductDiscountViewModel> GetAll(bool inactive = false);
        List<CertainValueCertainProductDiscountViewModel> Search(string srcParam, bool inactive = false);
        Dictionary<Guid, string> ProductList();

        void AddFreeOfChargeDiscount(Guid cvcpId, int ProductQuantity, Guid Product, decimal Value,
                                     DateTime effectiveDate, DateTime endDate);

        void DeacivateLineItem(Guid lineItemId);
        void ThrowIfExists(CertainValueCertainProductDiscountViewModel cvcpvm);

        QueryResult<CertainValueCertainProductDiscountViewModel> Query(QueryStandard query);
    }
}
