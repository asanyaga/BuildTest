using System;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository;

namespace Distributr.Mobile.Core.Products
{
    public interface ISaleProductRepository : IRepositoryMaster<SaleProduct>
    {
        SaleProduct FindById(Guid id);
    }
}
