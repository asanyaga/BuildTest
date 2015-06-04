namespace Distributr.Mobile.Core.Sync
{
    public class SyncPausedEvent<T>
    {
        public readonly string Message;

        public SyncPausedEvent(string message)
        {
            this.Message = message;
        }
    }
}