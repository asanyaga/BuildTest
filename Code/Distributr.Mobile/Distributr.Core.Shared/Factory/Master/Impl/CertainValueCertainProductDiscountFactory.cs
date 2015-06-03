using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Factory.Master.Impl
{
   public class CertainValueCertainProductDiscountFactory:ICertainValueCertainProductDiscountFactory
    {
        public CertainValueCertainProductDiscount CreateCertainValueCertainProductDiscount(ProductRef productRef, int productQuantity, decimal certainValue, DateTime effectiveDate, DateTime endDate)
        {
           var cvcp = new CertainValueCertainProductDiscount(Guid.NewGuid())
            {
                InitialValue = certainValue
            };
            
            cvcp.CertainValueCertainProductDiscountItems.Add(new CertainValueCertainProductDiscount.CertainValueCertainProductDiscountItem(Guid.NewGuid())
            {
                Product = new ProductRef { ProductId = productRef.ProductId },
                EffectiveDate = effectiveDate,
                EndDate = endDate,
                Quantity = productQuantity,
                CertainValue = certainValue,
                _Status = EntityStatus.New
            });
            return cvcp;
        }
    }
}
