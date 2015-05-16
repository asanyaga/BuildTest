using Ninject.Modules;

namespace Distributr.Mobile.Login.Settings
{
    public class LoginSettingsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<LoginSettingsRepository>().To<LoginSettingsRepository>();
        }
    }
}