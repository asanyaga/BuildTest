using System;

namespace Distributr.Mobile.Sync
{
    public class SyncFailedEvent<T>
    {
        public readonly Exception Exception;
        public readonly string Message;

        public SyncFailedEvent(string message = "An unexpected error occurred.", Exception exception = default(Exception))
        {
            Message = message;
            Exception = exception;
        }
    }
}