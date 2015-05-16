using System;

namespace Distributr.Mobile.Sync.Incoming
{
    public class SyncFailedEvent
    {
        public readonly Exception exception;
        public readonly string message;

        public SyncFailedEvent(string message = "An unexpected error occurred.", Exception exception = default(Exception))
        {
            this.message = message;
            this.exception = exception;
        }
    }
}