using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Transactional.DocumentEntities
{
    public enum DiscountType
    {
        None=0,
         ProductDiscount = 1,
         SaleValueDiscount = 2,
         GroupDiscount = 3,
         FreeOfChargeDiscount = 4,
         PromotionDiscount = 5,
         CertainValueCertainProductDiscount = 6,
    }
}
