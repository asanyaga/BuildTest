using System;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core.Test
{
    public interface ISaleProductRepository : IRepositoryMaster<SaleProduct>
    {
        SaleProduct FindById(Guid id);
    }

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
