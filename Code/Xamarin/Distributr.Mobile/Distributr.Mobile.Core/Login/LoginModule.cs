using Ninject.Modules;

namespace Distributr.Mobile.Login
{
    public class LoginModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ILoginRepository>().To<LoginRepository>();
            Bind<ILoginClient>().To<LoginClient>();
        }
    }
}