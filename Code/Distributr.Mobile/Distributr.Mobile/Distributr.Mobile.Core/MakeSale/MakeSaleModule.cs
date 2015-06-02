using Distributr.Core.ClientApp;
using Distributr.Mobile.Core.MakeSale;
using Distributr.Mobile.Sync.Outgoing;
using StructureMap.Configuration.DSL;

namespace Distributr.Mobile.MakeSale
{
    public class MakeSaleModule : Registry
    {
        public MakeSaleModule()
        {
            For<IOutgoingCommandEnvelopeRouter>().Use<OutgoingCommandEnvelopeRouter>();                                             
            For<SaleProcessor>().Use<SaleProcessor>();
        }
    }
}