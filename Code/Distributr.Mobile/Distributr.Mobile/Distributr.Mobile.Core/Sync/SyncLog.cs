using System;
using SQLite.Net.Attributes;

namespace Distributr.Mobile.Core.Sync
{
    public class SyncLog
    {
        [PrimaryKey]
        public string SyncType { get; set; }      

        public DateTime LastFinishTime { get; set; }
    }
}
