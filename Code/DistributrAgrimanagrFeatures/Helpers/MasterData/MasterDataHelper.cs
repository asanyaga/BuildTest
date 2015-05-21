using System.Linq;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Repository.Master.ProductRepositories;
using StructureMap;

namespace DistributrAgrimanagrFeatures.Helpers.MasterData
{
    public class MasterDataHelper
    {
        private readonly IProductRepository _productRepository;
        private readonly ICommodityRepository _commodityRepository;
        private readonly IContainerTypeRepository _containerTypeRepository;
        
        public MasterDataHelper()
        {
            _commodityRepository = ObjectFactory.GetInstance<ICommodityRepository>();
            _productRepository = ObjectFactory.GetInstance<IProductRepository>();
            _containerTypeRepository = ObjectFactory.GetInstance<IContainerTypeRepository>();
        }

        public Product GetSaleProduct()
        {
            return _productRepository.GetAll().First();
        }

        public Commodity GetCommodity()
        {
            return _commodityRepository.GetAll().First();
        }

        public CommodityGrade GetCommodityGrade()
        {
            var commodity = _commodityRepository.GetAll().First();
            return _commodityRepository.GetAllGradeByCommodityId(commodity.Id).First();
        }

        public ContainerType GetContainerType()
        {
            return _containerTypeRepository.GetAll().First();
        }
    }
}
