using System.Linq;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using StructureMap;

namespace DistributrAgrimanagrFeatures.Helpers.MasterData
{
    public class MasterDataHelper
    {
        private readonly IProductRepository _productRepository;
        
        public MasterDataHelper()
        {
            _productRepository = ObjectFactory.GetInstance<IProductRepository>();
        }

        public Product GetSaleProduct()
        {
            return _productRepository.GetAll().First();
        }
    }
}
