using System;

namespace Distributr.Mobile.Sync.Incoming
{
    public class SyncUpdateEvent
    {
        public readonly string message;
        public readonly double PercentDone;

        public SyncUpdateEvent(double percentDone)
        {
            this.message = string.Format("Downloading {0}%", Convert.ToInt32(percentDone * 100));
            this.PercentDone = percentDone;
        }

        public SyncUpdateEvent(string message)
        {
            this.message = message;
            this.PercentDone = default(double);
        }
    }
}