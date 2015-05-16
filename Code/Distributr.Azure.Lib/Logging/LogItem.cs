using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace Distributr.Azure.Lib.Logging
{
    public sealed class LogItem : TableEntity
    {
        public string Exception { get; set; }
        public string Level { get; set; }
        public string LoggerName { get; set; }
        public string Message { get; set; }
        public string RoleInstance { get; set; }

        public LogItem()
        {
            var dt = DateTime.Now;
            PartitionKey = string.Format("{0}-{1:00}-{2:00}-{3:00}-{4:00}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);
            RowKey = dt.Ticks.ToString();
        }
    }
}
