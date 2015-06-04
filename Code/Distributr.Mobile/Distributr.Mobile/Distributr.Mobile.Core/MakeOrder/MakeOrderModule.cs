using Distributr.Mobile.Core.MakeSale;
using Distributr.Mobile.Core.OrderSale;
using StructureMap.Configuration.DSL;

namespace Distributr.Mobile.MakeOrder
{
    public class MakeOrderModule : Registry
    {
        public MakeOrderModule()
        {
            For<OrderProcessor>().Use<OrderProcessor>();
            For<ISaleRepository>().Use<SaleRepository>();
        }
    }
}