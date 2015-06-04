using Distributr.Mobile.Core.Test;
using StructureMap.Configuration.DSL;

namespace Distributr.Mobile.Core.Products
{
    public class ProductsModule : Registry
    {
        public ProductsModule()
        {
            For<ISaleProductRepository>().Use<SaleProductRepository>();
            For<IInventoryRepository>().Use<InventoryRepository>();
            For<IReturnableProductRepository>().Use<ReturnableProductRepository>();
        }
    }
}
