using Distributr.Mobile.Core.Data;
using Distributr.Mobile.Core.Sync.Incoming;
using Distributr.Mobile.Envelopes;
using Distributr.Mobile.Sync;

namespace Distributr.Mobile.Core.Sync
{

    public interface ISyncStatusWatcher
    {
        bool IsPaused { get; }
    }

    public class SyncStatusWatchers
    {
        public static SyncStatusWatcher<MasterDataUpdate> MasterDataSyncWatcher = new SyncStatusWatcher<MasterDataUpdate>();
        public static SyncStatusWatcher<DownloadEnvelopeRequest> InboundTransactionSyncWatcher = new SyncStatusWatcher<DownloadEnvelopeRequest>();
        public static SyncStatusWatcher<LocalCommandEnvelope> OutboundTransactionSyncWatcher = new SyncStatusWatcher<LocalCommandEnvelope>();        
    }

    public class SyncStatusWatcher<T> : ISyncStatusWatcher
    {
        public bool IsPaused { get; private set; }

        public void OnEvent(SyncUpdateEvent<T> update)
        {
            IsPaused = false;
        }

        public void OnEvent(SyncPausedEvent<T> paused)
        {
            IsPaused = true;
        }

        public void OnEvent(SyncFailedEvent<T> failed)
        {
            IsPaused = false;
        }

        public void OnEvent(SyncCompletedEvent<T> completed)
        {
            IsPaused = false;
        }
    }
}
