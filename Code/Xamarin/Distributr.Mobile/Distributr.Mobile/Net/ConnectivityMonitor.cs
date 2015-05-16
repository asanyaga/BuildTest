using Distributr.Mobile.Login;
using Mobile.Common.Core;

namespace Distributr.Mobile.Core.Net
{
    public class ConnectivityMonitor : IConnectivityMonitor
    {
        private readonly BaseApplication<User> app;

        public ConnectivityMonitor(BaseApplication<User> app)
        {
            this.app = app;
        }

        public bool IsNetworkAvailable()
        {
            return app.IsNetworkAvailable();
        }

        public void WaitForNetwork()
        {
            app.WaitForNetwork();
        }
    }
}