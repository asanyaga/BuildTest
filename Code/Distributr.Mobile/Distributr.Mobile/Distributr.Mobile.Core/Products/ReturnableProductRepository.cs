using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core.Products
{

    public interface IReturnableProductRepository : IRepositoryMaster<ReturnableProduct>
    {
    }

    public class ReturnableProductRepository : BaseRepository<ReturnableProduct>, IReturnableProductRepository
    {
        public ReturnableProductRepository(Database database) : base(database)
        {
        }
    }
}
