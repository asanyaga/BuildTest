namespace Distributr.Mobile.Core.Sync.Incoming
{
    public class SyncPausedEvent
    {
        public readonly string message;

        public SyncPausedEvent(string message)
        {
            this.message = message;
        }
    }
}