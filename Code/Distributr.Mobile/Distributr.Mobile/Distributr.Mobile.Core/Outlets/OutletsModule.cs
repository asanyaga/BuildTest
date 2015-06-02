using StructureMap.Configuration.DSL;

namespace Distributr.Mobile.Core.Outlets
{
    public class OutletsModule : Registry
    {
        public OutletsModule()
        {
            For<IOutletRepository>().Use<OutletRepository>();
        }
    }
}
