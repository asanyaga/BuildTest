using System;

namespace Distributr.Mobile.Sync
{
    public class SyncUpdateEvent<T>
    {
        public readonly string Message;
        public readonly double PercentDone;

        public SyncUpdateEvent(double percentDone)
        {
            Message = string.Format("Downloading {0}%", Convert.ToInt32(percentDone * 100));
            PercentDone = percentDone;
        }

        public SyncUpdateEvent(string message)
        {
            Message = message;
            PercentDone = default(double);
        }
    }
}