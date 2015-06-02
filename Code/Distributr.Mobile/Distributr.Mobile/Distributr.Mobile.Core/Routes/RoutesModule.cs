using StructureMap.Configuration.DSL;

namespace Distributr.Mobile.Routes
{
    public class RoutesModule : Registry
    {
        public RoutesModule()
        {
            For<IRoutesRepository>().Use<RoutesRepository>();
        }
    }
}