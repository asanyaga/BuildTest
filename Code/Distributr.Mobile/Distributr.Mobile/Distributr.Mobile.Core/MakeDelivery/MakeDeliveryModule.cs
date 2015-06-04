using Distributr.Mobile.Core.MakeDelivery;
using StructureMap.Configuration.DSL;

namespace Distributr.Mobile.MakeDelivery
{
    public class MakeDeliveryModule : Registry
    {
        public MakeDeliveryModule()
        {
            For<DeliveryProcessor>().Use<DeliveryProcessor>();
        }
    }
}
