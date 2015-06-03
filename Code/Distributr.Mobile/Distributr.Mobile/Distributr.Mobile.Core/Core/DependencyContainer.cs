using System.Collections.Generic;
using Distributr.Mobile.Core.Data;
using Distributr.Mobile.Core.Outlets;
using Distributr.Mobile.Core.Payments;
using Distributr.Mobile.Core.Products;
using Distributr.Mobile.Core.Sync;
using Distributr.Mobile.Login;
using Distributr.Mobile.MakeDelivery;
using Distributr.Mobile.MakeOrder;
using Distributr.Mobile.MakeSale;
using Distributr.Mobile.Routes;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace Distributr.Mobile.Core
{
    public class DependencyContainerBuilder
    {   
        //Add any additional modules from Distributr.Mobile.Core here
        private readonly List<Registry> coreModules = new List<Registry>()
        {
            new DataModule(),
            new LoginModule(),
            new RoutesModule(),
            new OutletsModule(),
            new PaymentsModule(),
            new ProductsModule(),
            new SyncModule(),
            new MakeSaleModule(),
            new MakeOrderModule(),
            new MakeDeliveryModule()
        };

        private readonly List<Registry> additional = new List<Registry>();

        //This is to allow modules that use Distributr.Mobile.Core to provide environment specific-stuff
        public DependencyContainerBuilder AddModule(Registry module)
        {
            additional.Add(module);
            return this;
        }

        public DependencyContainer Build()
        {
            coreModules.AddRange(additional);
            return new DependencyContainer(coreModules);
        }
    }

    public class DependencyContainer
    {
        private readonly Container container;

        internal DependencyContainer(List<Registry> modules)
        {
            var builder = new PluginGraphBuilder();
            modules.ForEach(e => builder.Add(e));
            container = new Container(builder.Build());
        }

        public T Resolve<T>()
        {
            return container.GetInstance<T>();
        }
    }
}
