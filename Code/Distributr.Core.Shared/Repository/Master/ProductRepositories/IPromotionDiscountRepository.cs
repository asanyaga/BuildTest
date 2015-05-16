using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.ProductRepositories
{
   public interface IPromotionDiscountRepository:IRepositoryMaster<PromotionDiscount>
    {
       void AddFreeOfChargeDiscount(Guid focId, int parentProductQuantity, Guid? freeOfChargeProduct, int? freeOfChargeQuantity, DateTime effectiveDate, decimal DiscountRate, DateTime endDate);
       PromotionDiscount GetByProductId(Guid productMasterId);
       void DeactivateLineItem(Guid lineItemId);
       PromotionDiscount GetByProductAndQuantity(Guid productMasterId, int quantity);

       PromotionDiscount GetCurrentDiscount(Guid productMasterId);
       QueryResult<PromotionDiscount> Query(QueryStandard query);
    }
}
