using System;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core.OrderSale
{
    public class SaleRepository : BaseRepository<Sale>, ISaleRepository
    {
        private readonly Database database;

        public SaleRepository(Database database) : base(database)
        {
            this.database = database;
        }

        public Sale FindById(Guid id)
        {
            if (database.Find<Sale>(id) != null)
            {
                return GetById(id);
            }
            return default(Sale);
        }
    }
}
