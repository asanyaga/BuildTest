using System;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core.OrderSale
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        private readonly Database database;

        public OrderRepository(Database database) : base(database)
        {
            this.database = database;
        }

        public Order FindById(Guid id)
        {
            if (database.Find<Order>(id) != null)
            {
                return GetById(id);
            }
            return default(Order);
        }
    }
}
