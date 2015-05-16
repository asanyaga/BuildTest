using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands;
using Distributr.WSAPI.Lib.Services.Routing;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using log4net;

namespace Distributr.Azure.Lib.CommandProcessing.Routing
{
    public class AzureCommandRoutingOnRequestRepository : ICommandRoutingOnRequestRepository
    {
        private CloudTable _routeOnRequestTable;
        private CloudTable _routeOnRequestIdIndexTable;
        private CloudTable _routeOnRequestDocumentIndexTable;
        private CloudTable _routeOnRequestParentDocIndexTable;

        private CloudTable _routeOnRequestCostCentreTable;

        private CloudTable _commandRoutingStatusTable;
        private CloudTable _commandRoutingStatusIdRORIndexTable;
        private string _sqlConnectionString;
        private ILog _log;
        public AzureCommandRoutingOnRequestRepository(string storageConnectionString, string sqlConnectionString)
        {
            _sqlConnectionString = sqlConnectionString;
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            ServicePoint tableServicePoint = ServicePointManager.FindServicePoint(storageAccount.TableEndpoint);
            ServicePointManager.DefaultConnectionLimit = 20;
            tableServicePoint.UseNagleAlgorithm = false;
            tableServicePoint.Expect100Continue = false;
            CloudTableClient cloudTableClient = storageAccount.CreateCloudTableClient();

            _routeOnRequestTable = cloudTableClient.GetTableReference("routeonrequest");
            _routeOnRequestTable.CreateIfNotExists();
            _routeOnRequestIdIndexTable = cloudTableClient.GetTableReference("routeonrequestidindex");
            _routeOnRequestIdIndexTable.CreateIfNotExists();
            _routeOnRequestDocumentIndexTable = cloudTableClient.GetTableReference("routeonrequestdocumentindex");
            _routeOnRequestDocumentIndexTable.CreateIfNotExists();
            _routeOnRequestParentDocIndexTable = cloudTableClient.GetTableReference("routeonrequestparentdocindex");
            _routeOnRequestParentDocIndexTable.CreateIfNotExists();
            _routeOnRequestCostCentreTable = cloudTableClient.GetTableReference("routeonrequestcostcentre");
            _routeOnRequestCostCentreTable.CreateIfNotExists();
            _commandRoutingStatusTable = cloudTableClient.GetTableReference("commandroutingstatus");
            _commandRoutingStatusTable.CreateIfNotExists();
            _commandRoutingStatusIdRORIndexTable = cloudTableClient.GetTableReference("commandroutingstatusrorindex");
            _commandRoutingStatusIdRORIndexTable.CreateIfNotExists();
            _log = LogManager.GetLogger("AzureCommandRoutingOnRequestRepository");

        }
        public CommandRouteOnRequest GetById(long id)
        {
            TableResult rorlu = CommandRouteOnRequestLookup(id);
            if (rorlu == null) return null;
            var ror = (CommandRouteOnRequestTable)rorlu.Result;
            return ror.Map();

        }

        private TableResult CommandRouteOnRequestLookup(long id)
        {
            TableOperation idLookup = TableOperation.Retrieve<CommandRouteOnRequestIdIndex>("idlu", id.RouteOnRequestIdFormat());
            var idlookupResult = _routeOnRequestIdIndexTable.Execute(idLookup);
            if (idlookupResult.Result == null) return null;
            var lookupindex = (CommandRouteOnRequestIdIndex)idlookupResult.Result;
            TableOperation rorQuery =
                TableOperation.Retrieve<CommandRouteOnRequestTable>(lookupindex.GeneratedByCostCentreId.ToString(), id.RouteOnRequestIdFormat());
            return _routeOnRequestTable.Execute(rorQuery);
        }

        public List<CommandRouteOnRequestCostcentre> GetByCommandRouteOnRequestId(long id)
        {
            var rorfc = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id.RouteOnRequestIdFormat());
            var rorqry = new TableQuery<CommandRouteOnRequestCostCentreTable>().Where(rorfc);
            var qryresult = _routeOnRequestCostCentreTable.ExecuteQuery(rorqry);
            var result = new List<CommandRouteOnRequestCostcentre>();
            foreach (CommandRouteOnRequestCostCentreTable commandRouteOnRequestCostCentreTable in qryresult)
            {
                result.Add(commandRouteOnRequestCostCentreTable.Map());
            }

            return result;
        }

        public CommandRouteOnRequest GetByCommandId(Guid id)
        {
            TableOperation commandlookup = TableOperation.Retrieve<CommandRouteOnRequestCommandIndex>("commandlu", id.ToString());
            var commandlookupresult = _routeOnRequestIdIndexTable.Execute(commandlookup);
            if (commandlookupresult.Result == null) return null;
            var lookupindex = (CommandRouteOnRequestCommandIndex)commandlookupresult.Result;
            TableOperation rorQuery =
                TableOperation.Retrieve<CommandRouteOnRequestTable>(lookupindex.GeneratedByCostCentreId.ToString(), lookupindex.Id.RouteOnRequestIdFormat());
            TableResult rorlu = _routeOnRequestTable.Execute(rorQuery);
            if (rorlu.Result == null) return null;
            var ror = (CommandRouteOnRequestTable)rorlu.Result;
            return ror.Map();
        }

        public List<CommandRouteOnRequest> GetByDocumentId(Guid id)
        {
            var rorfc = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id.ToString());
            var rorqry = new TableQuery<CommandRouteOnRequestDocumentIndex>().Where(rorfc);
            var qryresult = _routeOnRequestDocumentIndexTable.ExecuteQuery(rorqry);

            var result = new List<CommandRouteOnRequest>();
            foreach (CommandRouteOnRequestDocumentIndex commandRouteOnRequestDocumentIndex in qryresult)
            {
                TableOperation rorQuery =
                    TableOperation.Retrieve<CommandRouteOnRequestTable>(
                        commandRouteOnRequestDocumentIndex.GeneratedByCostCentreId.ToString(),
                        commandRouteOnRequestDocumentIndex.RowKey);
                TableResult rorlu = _routeOnRequestTable.Execute(rorQuery);
                if (rorlu != null)
                {
                    var ror = (CommandRouteOnRequestTable)rorlu.Result;
                    result.Add(ror.Map());
                }
            }

            return result;
        }

        public List<CommandRouteOnRequest> GetByParentDocumentId(Guid parentId)
        {
            var rorfc = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, parentId.ToString());
            var rorqry = new TableQuery<CommandRouteOnRequestParentDocIndex>().Where(rorfc);
            var qryresult = _routeOnRequestParentDocIndexTable.ExecuteQuery(rorqry);

            var result = new List<CommandRouteOnRequest>();
            foreach (CommandRouteOnRequestParentDocIndex commandRouteOnRequestDocumentIndex in qryresult)
            {
                TableOperation rorQuery =
                    TableOperation.Retrieve<CommandRouteOnRequestTable>(
                        commandRouteOnRequestDocumentIndex.GeneratedByCostCentreId.ToString(),
                        commandRouteOnRequestDocumentIndex.RowKey);
                TableResult rorlu = _routeOnRequestTable.Execute(rorQuery);
                if (rorlu != null)
                {
                    var ror = (CommandRouteOnRequestTable)rorlu.Result;
                    result.Add(ror.Map());
                }
            }

            return result;
        }

        public CommandRouteOnRequestCostcentre GetByRouteCentreByIdAndCostcentreId(long id, Guid costcentreId)
        {
            return GetByCommandRouteOnRequestId(id).FirstOrDefault(n => n.CostCentreId == costcentreId);
        }

        public CommandRouteOnRequest GetUndeliveredByDestinationCostCentreApplicationId(Guid costCentreApplicationId, Guid costCentreId)
        {
            var result = GetUnexecutedBatchByDestinationCostCentreApplicationId(costCentreApplicationId, costCentreId, 1, false);
            if (!result.Any())
                return null;
            return result.First();
        }

        public long Add(CommandRouteOnRequest commandRouteItem)
        {
            int id = NewRORId();
            if (commandRouteItem.Id == 0) commandRouteItem.Id = id;
            commandRouteItem.DateAdded = DateTime.Now;
            CommandRouteOnRequestTable commandRouteOnRequestTable = commandRouteItem.Map();
            var tasks = new List<Task<TableResult>>();
            tasks.Add(Task.Run(() =>
                {
                    TableOperation to1 = TableOperation.Insert(commandRouteOnRequestTable);
                    return _routeOnRequestTable.Execute(to1);
                }));

            //id index
            tasks.Add(Task.Run(() =>
                {
                    var idindex = new CommandRouteOnRequestIdIndex
                        {
                            PartitionKey = "idlu",
                            RowKey = id.RouteOnRequestIdFormat(),
                            GeneratedByCostCentreId = new Guid(commandRouteOnRequestTable.PartitionKey)
                        };
                    TableOperation to2 = TableOperation.Insert(idindex);
                    return _routeOnRequestIdIndexTable.Execute(to2);
                }));

            //documentid index
            tasks.Add(Task.Run(() =>
                {
                    var docindex = new CommandRouteOnRequestDocumentIndex
                        {
                            PartitionKey = commandRouteItem.DocumentId.ToString(),
                            RowKey = id.RouteOnRequestIdFormat(),
                            GeneratedByCostCentreId = new Guid(commandRouteOnRequestTable.PartitionKey)
                        };
                    TableOperation to3 = TableOperation.Insert(docindex);
                    return _routeOnRequestDocumentIndexTable.Execute(to3);
                }));

            //command index
            tasks.Add(Task.Run(() =>
            {
                var commandIndex = new CommandRouteOnRequestCommandIndex
                {
                    PartitionKey = "commandlu",
                    RowKey = commandRouteItem.CommandId.ToString(),
                    Id = id,
                    GeneratedByCostCentreId = new Guid(commandRouteOnRequestTable.PartitionKey)
                };
                TableOperation to4 = TableOperation.Insert(commandIndex);
                return _routeOnRequestIdIndexTable.Execute(to4);
            }));


            //parentdoc id index
            bool hasParentId = commandRouteItem.DocumentParentId != null &&
                               commandRouteItem.DocumentParentId != Guid.Empty;
            if (hasParentId)
            {
                tasks.Add(Task.Run(() =>
                {
                    var parentDocIndex = new CommandRouteOnRequestParentDocIndex
                    {
                        PartitionKey = commandRouteItem.DocumentParentId.ToString(),
                        RowKey = id.RouteOnRequestIdFormat(),
                        GeneratedByCostCentreId = new Guid(commandRouteOnRequestTable.PartitionKey)
                    };
                    TableOperation to5 = TableOperation.Insert(parentDocIndex);
                    return _routeOnRequestParentDocIndexTable.Execute(to5);
                }));

            }
            Task.WaitAll(tasks.ToArray());
            return id;
        }

        #region RORId Generation
        /// <summary>
        /// have to resort to sql server for id generation, not ideal but
        /// </summary>
        /// <returns></returns>
        private int NewRORId()
        {
            int newId = 0;
            string sql = "INSERT INTO rorIdGenerator ([f1]) VALUES ('a');SELECT CAST(scope_identity() AS int)";
            string connectionstring = getconnectionstringfromctx();
            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                var cmd = new SqlCommand(sql, conn);
                conn.Open();
                newId = (int)cmd.ExecuteScalar();
            }
            return newId;
        }

        string getconnectionstringfromctx()
        {
            _log.Info("Connection string " + _sqlConnectionString ) ;
            string datasource = extract("data source", _sqlConnectionString);
            string initialcat = extract("initial cat", _sqlConnectionString);
            string user = extract("user", _sqlConnectionString);
            string pw = extract("passw", _sqlConnectionString);
            string val1 = string.Join(";", new[] {datasource, initialcat, user, pw});
            _log.Info("cs ==> " + val1 );
            return val1;
        }

        string extract(string chunk, string main)
        {
            if (!main.Contains(chunk))
                return "";
            int si = main.IndexOf(chunk);
            int ei = main.IndexOf(";", si);
            return main.Substring(si, ei - si);
        }
        #endregion

        public void AddRoutingCentre(CommandRouteOnRequestCostcentre commandRouteOnRequestCostcentre)
        {
            commandRouteOnRequestCostcentre.IsValid = true;
            if (commandRouteOnRequestCostcentre.Id == Guid.Empty)
                commandRouteOnRequestCostcentre.Id = Guid.NewGuid();
            //_log.Info("##1 AddRoutingCentre ->" + SerializeForLogging(commandRouteOnRequestCostcentre) );
            TableOperation idLookup = TableOperation.Retrieve<CommandRouteOnRequestIdIndex>("idlu", commandRouteOnRequestCostcentre.CommandRouteOnRequestId.RouteOnRequestIdFormat());
            var idlookupResult = _routeOnRequestIdIndexTable.Execute(idLookup);
            CommandRouteOnRequestIdIndex id = (CommandRouteOnRequestIdIndex)idlookupResult.Result;
            //_log.Info("##2 Fetched  CommandRouteOnRequestIdIndex --> " + SerializeForLogging(idlookupResult.Result) );

            CommandRouteOnRequestCostCentreTable rorCC = commandRouteOnRequestCostcentre.Map(id.GeneratedByCostCentreId);
            TableOperation to1 = TableOperation.InsertOrReplace(rorCC);
            _log.Info("##3 Attempting To Insert --> " + SerializeForLogging(rorCC));
            _routeOnRequestCostCentreTable.Execute(to1);
            _log.Info("##4 Inserted OK");

        }

        private string SerializeForLogging<T>(T sample)
        {
            string result = "";
            try
            {
                result = JsonConvert.SerializeObject(sample);
            }
            catch (Exception ex)
            {
                result = "Failed to serialize";
            }
            return result;
        }

        public void MarkBatchAsDelivered(List<long> commandRouteOnRequestId, Guid costCentreApplicationId)
        {
            foreach (long id in commandRouteOnRequestId)
            {
                if (id != 0)
                {
                    MarkAsDelivered(id, costCentreApplicationId);
                }
            }
        }

        public void MarkAsDelivered(long commandRouteOnRequestId, Guid costCentreApplicationId)
        {
            _log.InfoFormat("MarkAsDelivered RORId {0} - CCAppId {1}", commandRouteOnRequestId, costCentreApplicationId);
            CommandRouteOnRequest ror = GetById(commandRouteOnRequestId);
            if (ror == null)
                throw new Exception("MarkAsDelivered could not retrieve commandRouteOnRequestId : " + commandRouteOnRequestId);
            var checkExistingTO = TableOperation.Retrieve<CommandRoutingStatusTable>(costCentreApplicationId.ToString(), commandRouteOnRequestId.RouteOnRequestIdFormat());
            TableResult checkExistingResult = _commandRoutingStatusTable.Execute(checkExistingTO);
            if (checkExistingResult.Result != null)
            {
                _log.InfoFormat("MarkAsDelivered - checkExisting -> Already exists  RORId {0} - CCAppId {1}", commandRouteOnRequestId, costCentreApplicationId);
                return;
            }
            DateTime dt = DateTime.Now;
            var commandRoutingStatus = new CommandRoutingStatusTable
                {
                    Id = Guid.NewGuid(),
                    PartitionKey = costCentreApplicationId.ToString(),
                    RowKey = commandRouteOnRequestId.RouteOnRequestIdFormat(),
                    DateAdded = dt,
                    Delivered = true,
                    DateExecuted = DateTime.Now,
                    DateDelivered = dt,
                    DestinationCostCentreApplicationId = costCentreApplicationId,
                    CommandRouteOnRequestId = commandRouteOnRequestId,
                    CommandId = ror.CommandId
                };
            var tasks = new List<Task<TableResult>>();
            tasks.Add(Task.Run(() =>
                {
                    TableOperation op1 = TableOperation.Insert(commandRoutingStatus);
                    return _commandRoutingStatusTable.Execute(op1);
                }));

            tasks.Add(Task.Run(() =>
                {
                    //can't do order by descending in Azure Table
                    //long reverseIndex = DateTime.MaxValue.Ticks - commandRouteOnRequestId;
                    long reverseIndex = Int32.MaxValue - commandRouteOnRequestId;
                    var indexROR = new CommandRoutingStatusRouteOnRequestIndexTable
                    {
                        PartitionKey = costCentreApplicationId.ToString(),
                        RowKey = reverseIndex.RouteOnRequestIdFormat(),
                        CommandRouteOnRequestId = commandRouteOnRequestId
                    };
                    TableOperation op2 = TableOperation.Insert(indexROR);
                    return _commandRoutingStatusIdRORIndexTable.Execute(op2);
                }));

            Task.WaitAll(tasks.ToArray());

        }

        public void MardAdDeliveredAndExecuted(long commandRouteOnRequestId, Guid costCentreApplicationId)
        {
            throw new NotImplementedException();
        }

        public List<CommandRouteOnRequest> GetUnexecutedBatchByDestinationCostCentreApplicationId(Guid costCentreApplicationId, Guid costCentreId,
                                                                           int batchSize, bool includeArchived)
        {
            _log.InfoFormat("GetUnexecutedBatchByDestinationCostCentreApplicationId for ccappid {0} ccid {1} ", costCentreApplicationId, costCentreId);
            //get last processed RORId for cost centre
            string statusfc = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, costCentreApplicationId.ToString());
            var statusIndexQry = new TableQuery<CommandRoutingStatusRouteOnRequestIndexTable>()
                .Where(statusfc)
                .Take(1);
            var statusIndexResult = _commandRoutingStatusIdRORIndexTable.ExecuteQuery(statusIndexQry);
            long lastProcessedRORId = 0;
            if (statusIndexResult.Count() > 0)
            {
                var lastProcessedROR = statusIndexResult.First();
                lastProcessedRORId = lastProcessedROR.CommandRouteOnRequestId;
            }

            //get RORIds from CommandRouteOnRequestCostcentre
            string qryCC = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, costCentreId.ToString());
            string qryRORId = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThan, lastProcessedRORId.RouteOnRequestIdFormat());
            string qryIsValid = TableQuery.GenerateFilterConditionForBool("IsValid", QueryComparisons.Equal, true);
            string qryIsRetired = TableQuery.GenerateFilterConditionForBool("IsRetired", QueryComparisons.Equal, false);
            string qryExcludeDestinationCC = TableQuery.GenerateFilterConditionForGuid("SourceCostCentreId", QueryComparisons.NotEqual, costCentreId);
            string qryCCCombined = string.Format("{0} and {1} and {2} and {3} ", qryCC, qryRORId, qryIsValid, qryExcludeDestinationCC);
            if (!includeArchived)
                qryCCCombined = string.Format("{0} and {1}", qryCCCombined, qryIsRetired);
            var ccQry = new TableQuery<CommandRouteOnRequestCostCentreTable>().Where(qryCCCombined);
            var ccQryResult = _routeOnRequestCostCentreTable.ExecuteQuery(ccQry);

            var results = new List<CommandRouteOnRequest>();
            var ccQryItems = ccQryResult
                .OrderBy(n => n.CommandRouteOnRequestId)
                .Take(batchSize)
                .Select(n => new { SourceCCId = n.SourceCostCentreId, RORId = n.CommandRouteOnRequestId });
            if (!ccQryItems.Any())
                return results;

            var tasks = new List<Task<TableResult>>();
            foreach (var qryItem in ccQryItems)
            {
                tasks.Add(Task.Run(() =>
                    {
                        TableOperation rorLookup =
                            TableOperation.Retrieve<CommandRouteOnRequestTable>(qryItem.SourceCCId.ToString(),
                                                                                qryItem.RORId.RouteOnRequestIdFormat());
                        return _routeOnRequestTable.Execute(rorLookup);
                    }));
            }

            Task.WaitAll(tasks.ToArray());

            //map
            foreach (var task in tasks)
            {
                TableResult result = task.Result;
                var tresult = (CommandRouteOnRequestTable)result.Result;
                results.Add(tresult.Map());
            }

            return results.OrderBy(n => n.Id).ToList();
        }

        public void RetireCommands(Guid parentCommandId)
        {
            throw new NotImplementedException();
        }

        public void UnRetireCommands(Guid docParentId)
        {
            throw new NotImplementedException();
        }

        public void MarkCommandsAsInvalid(Guid costCentreId)
        {
            try
            {
                _log.InfoFormat("Mark commands as invalid for ccid {0}", costCentreId);
                var rorIdsToInvalidate = new List<long>();
                //iterate through all CommandRouteOnRequestCostCentreTable items and mark as invalid
                //for commandtypes contained
                string qryCC = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal,costCentreId.ToString());
                string qryIsValid = TableQuery.GenerateFilterConditionForBool("IsValid", QueryComparisons.Equal, true);
                string qryCCCombined = string.Format("{0} and {1}", qryCC, qryIsValid);
                var ccQry = new TableQuery<CommandRouteOnRequestCostCentreTable>().Where(qryCCCombined);
                var token = new TableContinuationToken();
                var segment = _routeOnRequestCostCentreTable.ExecuteQuerySegmented(ccQry, token);
                while (token != null)
                {
                    foreach (var result in segment)
                    {
                        if (GetExcludedCommandTypelist().Contains(result.CommandType))
                            rorIdsToInvalidate.Add(result.CommandRouteOnRequestId);
                    }
                    segment = _routeOnRequestCostCentreTable.ExecuteQuerySegmented(ccQry, token);
                    token = segment.ContinuationToken;
                }
                _log.InfoFormat("Found {0} commands to invalidate", rorIdsToInvalidate.Count());
                foreach (long rorid in rorIdsToInvalidate)
                {
                    //TODO Task this up to run in parallel
                    TableOperation rorCCLookup =
                        TableOperation.Retrieve<CommandRouteOnRequestCostCentreTable>(rorid.RouteOnRequestIdFormat(),
                                                                                      costCentreId.ToString());
                    TableResult result = _routeOnRequestCostCentreTable.Execute(rorCCLookup);
                    CommandRouteOnRequestCostCentreTable rorCC = (CommandRouteOnRequestCostCentreTable)result.Result;
                    rorCC.IsValid = false;
                    TableOperation to1 = TableOperation.Replace(rorCC);
                    _routeOnRequestCostCentreTable.Execute(to1);
                }
                _log.InfoFormat("Updated {0} commands to invalid", rorIdsToInvalidate.Count());
            }
            catch (Exception ex)
            {
                _log.Error("MarkCommandsAsInvalid failed to execute", ex);
            }
        }

        private static IEnumerable<string> GetExcludedCommandTypelist()
        {
            return new string[]
                       {
                           CommandType.CreateInventoryAdjustmentNote.ToString(),
                           CommandType.AddInventoryAdjustmentNoteLineItem.ToString(),
                           CommandType.ConfirmInventoryAdjustmentNote.ToString(),
                           CommandType.CreateInventoryReceivedNote.ToString(),
                           CommandType.AddInventoryReceivedNoteLineItem.ToString(),
                           CommandType.ConfirmInventoryReceivedNote.ToString(),
                           CommandType.CreateInventoryTransferNote.ToString(),
                           CommandType.AddInventoryTransferNoteLineItem.ToString(),
                           CommandType.ConfirmInventoryTransferNote.ToString(),
                           CommandType.CreatePaymentNote.ToString(),
                           CommandType.AddPaymentNoteLineItem.ToString(),
                           CommandType.ConfirmPaymentNote.ToString(),
                           CommandType.RetireDocument.ToString(),
                           CommandType.CreateInventorySerials.ToString(),
                           CommandType.ReCollection.ToString()
                       };
        }

        public void CleanApplication(Guid applicationId)
        {
            _log.Error("CLEAN APPLICATION CALLED");
        }

        public List<CommandRouteOnRequestCostcentre> TestCC(Guid ccid)
        {
            throw new NotImplementedException();
        }

        public List<RouteOnRequestSummary> GetCCRouteOnRequestSummary(DateTime? fromDate)
        {
            throw new NotImplementedException();
        }

        public List<RouteOnRequestDeliveredSummary> GetCCAppIdRouteOnRequestDeliveredSummary(DateTime? fromDate)
        {
            throw new NotImplementedException();
        }

        public List<CostCentreRouteOnRequestDetail> GetCostCentreRouteOnRequestDetail(Guid costCentreId, int dayOfYear, int year)
        {
            throw new NotImplementedException();
        }

        public List<long> GetCommandsAsIntegers(Guid costCentreId, DateTime forDate, int pageIndex, int pageSize, out int count)
        {
            throw new NotImplementedException();
        }

        public List<CCComandRoutingItem> GetCommandRoutingItems(List<long> costCentreCommands, List<Guid> costCentreApps)
        {
            throw new NotImplementedException();
        }

        public List<CommandRef> GetCommandRefs(List<long> costCentreCommands)
        {
            throw new NotImplementedException();
        }
    }
}
