using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using log4net.Appender;
using log4net.Core;

namespace Distributr.Azure.Lib.Logging
{

    public class AzureWebsiteTableAppender : AzureTableAppender
    {
        public AzureWebsiteTableAppender()
        {
            AzureConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            RoleInfo = "WebApi";
        }

    }

    public abstract class AzureTableAppender : AppenderSkeleton
    {
        protected string AzureConnectionString { get; set; }
        protected CloudTable _log4NetTable;
        protected string RoleInfo { get; set; }

        public override void ActivateOptions()
        {
            base.ActivateOptions();

            var storageAccount = CloudStorageAccount.Parse(AzureConnectionString);

            var tableClient = storageAccount.CreateCloudTableClient();
            _log4NetTable = tableClient.GetTableReference("wepapilogs");
            _log4NetTable.CreateIfNotExists();
        }

        protected override void Append(LoggingEvent e)
        {
            try
            {
                Trace.TraceInformation("AzureTableAppender Append called" + e.LoggerName);
                var logitem = new LogItem
                    {
                        Exception = e.GetExceptionString(),
                        Level = e.Level.Name,
                        LoggerName = e.LoggerName,
                        Message = e.RenderedMessage,
                        RoleInstance = RoleInfo
                    };
                TableOperation tableOperation1 = TableOperation.Insert(logitem);
                var result = _log4NetTable.Execute(tableOperation1);
                Trace.TraceInformation("AzureTableAppender Append result " + result.HttpStatusCode);

            }
            catch (Exception ex)
            {
                Trace.TraceError("AzureTableAppender Append " +  ex.Message);
            }
        }
        
        protected override void Append(LoggingEvent[] loggingEvents)
        {
            try
            {

                TableBatchOperation batchOperation = new TableBatchOperation();
                base.Append(loggingEvents);
                foreach (var e in loggingEvents)
                {
                    var logitem = new LogItem
                        {
                            Exception = e.GetExceptionString(),
                            Level = e.Level.Name,
                            LoggerName = e.LoggerName,
                            Message = e.RenderedMessage,
                            RoleInstance = RoleInfo
                        };
                    batchOperation.Insert(logitem);
                }
                _log4NetTable.ExecuteBatch(batchOperation);

            }
            catch (Exception ex)
            {
                Trace.TraceError("AzureTableAppender Append " + ex.Message);
            }
        }
    }
}
