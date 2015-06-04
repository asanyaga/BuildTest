using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Repository.Master.ProductRepositories
{
   public interface IProductDiscountGroupRepository:IRepositoryMaster<ProductGroupDiscount>
    {
       
       void SetLineItemsInactive(ProductGroupDiscount entity);
       List<ProductGroupDiscount> GetByDiscountGroup(Guid discountGroup, bool includeDeactivated = false);
      
       ProductGroupDiscount GetByDiscountGroupCode(string discountgroupCode);
       ProductGroupDiscount GetByGroupbyProductByQuantity(Guid groupId, Guid productId,decimal quantity);
       ProductGroupDiscount GetCurrentCustomerDiscount(Guid groupId, Guid productId, decimal quantity);

    }
}
