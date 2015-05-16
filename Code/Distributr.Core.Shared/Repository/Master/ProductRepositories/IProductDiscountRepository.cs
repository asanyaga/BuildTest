using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.ProductRepositories
{
   public interface IProductDiscountRepository : IRepositoryMaster<ProductDiscount>
    {
       void AddDiscount(Guid discountId, DateTime effectiveDate, decimal discountRate, DateTime endDate, bool isByQuantity, decimal quantity);
       void DeactivateLineItem(Guid lineItemId);
       ProductDiscount GetProductDiscount(Guid productId, Guid tierId);
       QueryResult<ProductDiscount> Query(QueryStandard q);
    }
}
