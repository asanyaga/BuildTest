using System;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Mobile.Core.Products;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core.Test
{
    public class SaleProductRepository : BaseRepository<SaleProduct>, ISaleProductRepository
    {
        private readonly Database database;

        public SaleProductRepository(Database database) : base (database)
        {
            this.database = database;
        }


        public SaleProduct FindById(Guid id)
        {
            if (database.Find<SaleProduct>(id) != null)
            {
                //Load complete object graph
                return GetById(id);
            }

            return default(SaleProduct);
        }
    }
}
