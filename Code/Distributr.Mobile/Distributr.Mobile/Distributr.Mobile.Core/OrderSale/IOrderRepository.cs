using System;
using Distributr.Core.Repository;

namespace Distributr.Mobile.Core.OrderSale
{
    public interface IOrderRepository : IRepositoryMaster<Order>
    {
        Order FindById(Guid id);
    }
}
