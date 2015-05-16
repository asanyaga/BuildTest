using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.Product;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products
{
    public interface IProductGroupDiscountViewModelBuilder
    {
        ProductDiscountGroupViewModel Get(Guid id);
        ProductDiscountGroupViewModel GetByDiscountGroup(Guid id);
        void Save(ProductDiscountGroupViewModel productGroupDiscount);
        void SetInactive(Guid id);
        void SetLineItemsInactive(Guid id);
        List<ProductDiscountGroupViewModel> GetByDiscountGroup(Guid discountGroup, bool inactive = false);
        List<ProductDiscountGroupViewModel> GetAll(bool inactive = false);
        List<ProductDiscountGroupViewModel> Search(string srcParam, bool inactive = false);
        Dictionary<Guid, string> DiscountGroupList();
        Dictionary<Guid, string> ProductList();
        Dictionary<Guid, string> ProductListWithoutReturnables();

        void AddProductGroupDiscount(Guid discountGroupId, Guid Product, decimal discountRate, DateTime effectiveDate,
                                     DateTime endDate);

        void ThrowIfExists(ProductDiscountGroupViewModel vm);
        QueryResult<ProductDiscountGroupViewModel> Query(QueryStandard q);
    }
}
