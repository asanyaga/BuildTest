using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.ProductRepositories
{
    public interface ISaleValueDiscountRepository : IRepositoryMaster<SaleValueDiscount>
    {
        void AddSaleValueDiscount(Guid discountId, DateTime effectiveDate, decimal discountRate, decimal saleValue, DateTime endDate);
        SaleValueDiscount GetByAmount(decimal Amount, Guid tier);
       
        void DeactivateLineItem(Guid lineItemId);
        SaleValueDiscount GetCurrentDiscount(decimal amount, Guid tierId);

        QueryResult<SaleValueDiscount> Query(QueryStandard q);
    }
}
