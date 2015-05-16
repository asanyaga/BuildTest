namespace Distributr.Mobile.Core.Net
{
    public interface IConnectivityMonitor
    {
        bool IsNetworkAvailable();
        void WaitForNetwork();
    }
}