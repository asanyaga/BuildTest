using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Factory.Master
{
    public interface IProductFactory
    {
        SaleProduct CreateSaleProduct(Guid Id);
        ConsolidatedProduct CreateConsolidatedProduct(Guid Id,Product productToAdd, int productToAddQuantity)    ;
        ReturnableProduct CreateReturnableProduct(Guid Id);
        ConsolidatedProduct SaveConsolidatedProduct(Guid Id);
    }
}
