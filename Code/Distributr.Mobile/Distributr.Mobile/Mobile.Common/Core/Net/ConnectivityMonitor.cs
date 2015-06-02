using Android.Content;
using Android.Net;
using System.Threading;

namespace Mobile.Common.Core.Net
{
    public class ConnectivityMonitor<U>
    {
        private readonly BaseApplication<U> app;
        private readonly ConnectivityManager connectivityManager;
        private readonly NetworkStateChangeReceiver networkStateChangeReceiver;
        private readonly NetworkLock networkLock = new NetworkLock();

        private readonly object @lock = new object();

        private bool registered;

        public ConnectivityMonitor(BaseApplication<U> app)
        {
            this.app = app;
            connectivityManager = (ConnectivityManager) app.GetSystemService(Context.ConnectivityService);
            networkStateChangeReceiver = new NetworkStateChangeReceiver(this);
        }

        public bool IsNetworkAvailable()
        {
            var available = _IsNetworkAvailable();
            if (!available)
            {
                networkLock.SetNetworkUnavailable();
                StartMonitoring();
            }
            return available;
        }

        private bool _IsNetworkAvailable()
        {
            var activeNetwork = connectivityManager.ActiveNetworkInfo;
            return activeNetwork != null && activeNetwork.IsConnected;
        }

        public void WaitForNetwork()
        {
            networkLock.WaitForNetwork();
        }

        private void StartMonitoring()
        {
            lock (@lock)
            {
                if (registered)
                {
                    return;
                }
                app.RegisterReceiver(networkStateChangeReceiver,
                    new IntentFilter("android.net.conn.CONNECTIVITY_CHANGE"));
                registered = true;
            }
        }

        private void OnNetworkStateChange()
        {
            lock (@lock)
            {
                if (_IsNetworkAvailable())
                {
                    app.UnregisterReceiver(networkStateChangeReceiver);
                    registered = false;
                    networkLock.SetNetworkAvailable();
                }
            }
        }

        private class NetworkStateChangeReceiver : BroadcastReceiver
        {
            private readonly ConnectivityMonitor<U> connectivityMonitor;

            public NetworkStateChangeReceiver(ConnectivityMonitor<U> connectivityMonitor)
            {
                this.connectivityMonitor = connectivityMonitor;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                connectivityMonitor.OnNetworkStateChange();
            }
        }
    }

    public class NetworkLock
    {
        private readonly ManualResetEvent signal = new ManualResetEvent(true);

        public void WaitForNetwork()
        {
            signal.WaitOne();
        }

        public void SetNetworkUnavailable()
        {
            signal.Reset();
        }

        public void SetNetworkAvailable()
        {
            signal.Set();
        }
    }
}