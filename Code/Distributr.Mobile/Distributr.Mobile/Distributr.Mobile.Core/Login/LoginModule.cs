using Distributr.Mobile.Login.Settings;
using StructureMap.Configuration.DSL;

namespace Distributr.Mobile.Login
{
    public class LoginModule : Registry
    {
        public LoginModule()
        {
            For<ILoginRepository>().Use<LoginRepository>();
            For<ILoginClient>().Use<LoginClient>();
            For<LoginSettingsRepository>().Use<LoginSettingsRepository>();
        }
    }
}