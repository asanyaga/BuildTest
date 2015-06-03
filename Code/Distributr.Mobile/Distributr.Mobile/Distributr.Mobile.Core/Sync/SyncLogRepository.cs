using System;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core.Sync
{
    public class SyncLogRepository
    {
        private readonly Database database;

        public SyncLogRepository(Database database)
        {
            this.database = database;
        }

        public string LastSyncTime(Type type)
        {
            var syncLog = database.Find<SyncLog>(type.Name);
            if (syncLog == null)
            {
                return "never";
            }
            return syncLog.LastFinishTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public void UpdateLastSyncTime(Type type)
        {
            var syncLog = database.Find<SyncLog>(type.Name) ?? new SyncLog() {SyncType = type.Name};
            syncLog.LastFinishTime = DateTime.Now;
            database.InsertOrReplace(syncLog);
        }

    }
}
