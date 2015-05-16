using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Data.Repository.MasterData.ProductRepositories
{
    /// <summary>
    /// For now just used to indicate product type from db to domain
    /// No need for now to expose as part of domain model
    /// </summary>
    internal enum ProductDomainType
    {
        SaleProduct = 1,
        ConsolidatedProduct = 2,
        ReturnableProduct = 3
    }
}
