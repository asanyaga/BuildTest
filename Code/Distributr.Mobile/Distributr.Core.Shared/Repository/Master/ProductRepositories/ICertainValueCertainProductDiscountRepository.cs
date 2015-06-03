using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.ProductRepositories
{
    public interface ICertainValueCertainProductDiscountRepository : IRepositoryMaster<CertainValueCertainProductDiscount>
    {
        void AddCertainValueCertainProductDiscount(Guid cvcpId, int ProductQuantity, ProductRef ProductRef, decimal CertainValue, DateTime effectiveDate, DateTime endDate);
        void EditCertainValueCertainProductDiscount(Guid cvcpId, Guid lineItemId, int productQuantity, ProductRef productRef, decimal certainValue, DateTime effectiveDate, DateTime endDate);//cn
        CertainValueCertainProductDiscount GetByAmount(decimal Amount);
        CertainValueCertainProductDiscount GetByAmountAndProduct(decimal Amount, Guid productId);
        void DeactivateLineItem(Guid lineItemId);
        CertainValueCertainProductDiscount GetByInitialValue(decimal value);



        QueryResult<CertainValueCertainProductDiscount > Query(QueryStandard query);
    }
}
