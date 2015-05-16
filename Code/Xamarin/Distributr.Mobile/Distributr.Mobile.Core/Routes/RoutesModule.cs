using Ninject.Modules;

namespace Distributr.Mobile.Routes
{
    public class RoutesModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IRoutesRepository>().To<RoutesRepository>();
        }
    }
}