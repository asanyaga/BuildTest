using System;
using Distributr.Core.Repository;

namespace Distributr.Mobile.Core.OrderSale
{
    public interface ISaleRepository : IRepositoryMaster<Sale>
    {
        Sale FindById(Guid id);
    }
}
