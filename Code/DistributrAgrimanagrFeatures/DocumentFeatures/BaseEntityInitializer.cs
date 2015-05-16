using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.ProductRepositories;
using StructureMap;

namespace DistributrAgrimanagrFeatures.DocumentFeatures
{
    public class BaseEntityInitializer
    {
        private readonly IProductRepository _productRepository;

        public BaseEntityInitializer()
        {
            _productRepository = ObjectFactory.GetInstance<IProductRepository>();
        }

        public Product SaleProduct()
        {
            return _productRepository.GetAll().First();
        }
    }
}
