using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Distributr.Azure.Lib.CommandProcessing;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using log4net;

namespace TestAzureTables.Impl
{
    public class AzureCommandProcessingAuditRepository : ICommandProcessingAuditRepository
    {
        private CloudTable _commandTable;
        private CloudTable _commandIndexTable;
        private CloudTable _documentIndexTable;
        private CloudTable _ccAppIdIndexTable;
        private CloudTable _commandStatus;
        private ILog _logger = LogManager.GetLogger("AzureCommandProcessingAuditRepository");
        public AzureCommandProcessingAuditRepository(string storageConnectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            ServicePoint tableServicePoint = ServicePointManager.FindServicePoint(storageAccount.TableEndpoint);
            ServicePointManager.DefaultConnectionLimit = 20;
            tableServicePoint.UseNagleAlgorithm = false;
            tableServicePoint.Expect100Continue = false;
            CloudTableClient cloudTableClient = storageAccount.CreateCloudTableClient();
            _commandTable = cloudTableClient.GetTableReference("command");
            _commandIndexTable = cloudTableClient.GetTableReference("commandindex");
            _documentIndexTable = cloudTableClient.GetTableReference("commanddocumentindex");
            _ccAppIdIndexTable = cloudTableClient.GetTableReference("commandccappidindex");
            _commandStatus = cloudTableClient.GetTableReference("commandstatusindex");
            _commandTable.CreateIfNotExists();
            _commandIndexTable.CreateIfNotExists();
            _documentIndexTable.CreateIfNotExists();
            _ccAppIdIndexTable.CreateIfNotExists();
            _commandStatus.CreateIfNotExists();
        }
        public CommandProcessingAudit GetByCommandId(Guid commandId)
        {
            TableResult rcl = CommandAuditTableLookup(commandId);
            if (rcl == null) return null;
            var command = (CommandProcessingAuditTable)rcl.Result;
            return command.Map();
        }

        private TableResult CommandAuditTableLookup(Guid commandId)
        {
            //index lookup
            TableOperation commandLookup = TableOperation.Retrieve<CommandProcessingAuditCommandIndexTable>("commandlu",
                                                                                                            commandId.ToString());
            var commandLookupResult = _commandIndexTable.Execute(commandLookup);
            if (commandLookupResult.Result == null)
                return null;
            var commandIndex = (CommandProcessingAuditCommandIndexTable)commandLookupResult.Result;

            //command lookup
            TableOperation commandQuery =
                TableOperation.Retrieve<CommandProcessingAuditTable>(commandIndex.CostCentreId.ToString(),
                                                                     commandId.ToString());
            TableResult rcl = _commandTable.Execute(commandQuery);
            return rcl;
        }

        public List<CommandProcessingAudit> GetByDocumentId(Guid documentId)
        {
            string documentIndexfc = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,
                                                                        documentId.ToString());
            var documentIndexQry = new TableQuery<CommandProcessingAuditDocumentIndexTable>()
                .Where(documentIndexfc);
            var documentResult = _documentIndexTable.ExecuteQuery(documentIndexQry);

            var result = new List<CommandProcessingAudit>();
            foreach (var item in documentResult)
            {
                string commandId = item.RowKey;
                string ccid = item.CostCentreId.ToString();
                TableOperation commandQuery = TableOperation.Retrieve<CommandProcessingAuditTable>(ccid, commandId);
                var rcl = _commandTable.Execute(commandQuery);
                if (rcl.Result == null)
                    continue;
                var command = (CommandProcessingAuditTable)rcl.Result;
                CommandProcessingAudit audit = command.Map();
                result.Add(audit);

            }
            return result;
        }

        public void AddCommand(CommandProcessingAudit commandProcessingAudit)
        {
            CommandProcessingAuditTable commandProcessingAuditTable = commandProcessingAudit.Map();
            var tasks = new List<Task<TableResult>>();

            tasks.Add(Task.Run(() =>
            {
                TableOperation tableOperation1 = TableOperation.Insert(commandProcessingAuditTable);
                return _commandTable.Execute(tableOperation1);
            }));

            //commandindex
            tasks.Add(Task.Run(() =>
                {

                    var commandIndex = new CommandProcessingAuditCommandIndexTable
                    {
                        PartitionKey = "commandlu",
                        CostCentreId = new Guid(commandProcessingAuditTable.PartitionKey),
                        RowKey = commandProcessingAudit.Id.ToString()
                    };
                    TableOperation to1 = TableOperation.Insert(commandIndex);
                    TableResult r1 = _commandIndexTable.Execute(to1);
                    return r1;
                }));

            //documentindex
            tasks.Add(Task.Run(() =>
                {

                    var documentIndex = new CommandProcessingAuditDocumentIndexTable
                        {
                            PartitionKey = commandProcessingAudit.DocumentId.ToString(),
                            RowKey = commandProcessingAudit.Id.ToString(),
                            CostCentreId = new Guid(commandProcessingAuditTable.PartitionKey)
                        };
                    TableOperation to2 = TableOperation.Insert(documentIndex);
                    TableResult r2 = _documentIndexTable.Execute(to2);
                    return r2;
                }));

            //ccappid index
            tasks.Add(Task.Run(() =>
            {
                var ccappidindex = new CommandProcessingAuditCCApplicationIndexTable
                {
                    PartitionKey = commandProcessingAudit.CostCentreApplicationId.ToString(),
                    RowKey = commandProcessingAudit.Id.ToString(),
                    CostCentreId = new Guid(commandProcessingAuditTable.PartitionKey)
                };
                TableOperation to3 = TableOperation.Insert(ccappidindex);
                TableResult r3 = _ccAppIdIndexTable.Execute(to3);
                return r3;
            }));

            //command status
            tasks.Add(Task.Run(() =>
                {
                    return AddCommandStatusIndex(commandProcessingAuditTable.Id, CommandProcessingStatus.OnQueue,
                                                 commandProcessingAuditTable.PartitionKey);
                }));


            Task.WaitAll(tasks.ToArray());

        }


        public void SetCommandStatus(Guid commandId, CommandProcessingStatus status)
        {
            _logger.InfoFormat("#### ---> Command {0} SetCommandStatus {1} ", commandId.ToString(), status.ToString());

            string existingStatus = null;
            //update command
            TableResult commandAudit = CommandAuditTableLookup(commandId);
            if (commandAudit == null)
                return;

            CommandProcessingAuditTable command = (CommandProcessingAuditTable)commandAudit.Result;
            existingStatus = ((CommandProcessingStatus)command.Status).ToString();

            var tasks = new List<Task<TableResult>>();

            tasks.Add(Task.Run(() =>
                {
                    command.Status = (int)status;
                    TableOperation updateOperation = TableOperation.Replace(command);
                    return _commandTable.Execute(updateOperation);
                }));


            //add to command status
            if (status == CommandProcessingStatus.Complete) //should not need to index complete??
                return;
            tasks.Add(Task.Run(() =>
                {
                    //remove existing
                    var csFC = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal,commandId.ToString());
                    var csQry = new TableQuery<CommandProcessingAuditStatusIndexTable>().Where(csFC);
                    var csResults = _commandStatus.ExecuteQuery(csQry);
                    foreach (var csResult in csResults)
                    {
                        TableOperation deleteOperation = TableOperation.Delete(csResult);
                        _commandStatus.Execute(deleteOperation);
                    }
                    //add new
                    string ccid = command.PartitionKey;
                    return AddCommandStatusIndex(commandId, status, ccid);
                }));

            Task.WaitAll(tasks.ToArray());
        }

        private TableResult AddCommandStatusIndex(Guid commandId, CommandProcessingStatus status, string ccid)
        {
            var statusIndex = new CommandProcessingAuditStatusIndexTable
                {
                    PartitionKey = status.ToString(),
                    RowKey = commandId.ToString(),
                    CostCentreId = new Guid(ccid)
                };
            TableOperation to3 = TableOperation.Insert(statusIndex);
            TableResult r3 = _commandStatus.Execute(to3);
            return r3;
        }

        public List<CommandProcessingAudit> GetAll()
        {
            throw new NotImplementedException();
        }

        public List<CommandProcessingAudit> GetAllByStatus(CommandProcessingStatus status)
        {
            throw new NotImplementedException();
        }

        public List<CommandProcessingAudit> GetByCCAppId(Guid costCentreApplicationId, int dayOfYear, int year)
        {
            throw new NotImplementedException();
        }

        public List<CommandProcessingAudit> GetByApplicationId(Guid costCentreApplicationId, int index, int size, CommandProcessingStatus status, out int count)
        {
            throw new NotImplementedException();
        }


        private readonly Func<string,CommandProcessingAudit, bool> _commandExecuted =
           (command, audit)  => audit.CommandType.Contains(command) && audit.Status == CommandProcessingStatus.Complete; 
        public bool IsCreateCommandExecuted(Guid documentId)
        {
            return GetByDocumentId(documentId).Any(n => _commandExecuted("Create",n) );
        }

        public bool IsAddCommandExecuted(Guid documentId)
        {
            var documentCommands = GetByDocumentId(documentId);
            bool isCreated = documentCommands.Any(n => _commandExecuted("Create",n));
            if (!isCreated) return false;

            if (documentCommands.Count(n => n.CommandType.Contains("Add")) == 0)
                return false;

            return documentCommands.Where(n => n.CommandType.Contains("Add")).All(n => _commandExecuted("Add", n));
        }

        public bool IsCommandExecuted(Guid commandId)
        {
            CommandProcessingAudit audit = GetByCommandId(commandId);
            if (audit == null) return false;
            return audit.Status == CommandProcessingStatus.Complete;
        }

        public bool IsConfirmExecuted(Guid documentId)
        {
            return GetByDocumentId(documentId).Any(n=> _commandExecuted("Confirm",n));
        }

        public IEnumerable<CommandProcessingAuditSummaryDTO> GetCommandProcessedSummary(DateTime? fromDate)
        {
            throw new NotImplementedException();
        }

        public decimal GetUnQueuedCommands()
        {
            throw new NotImplementedException();
        }

        public void QueueCommands()
        {
            throw new NotImplementedException();
        }

        public void Test()
        {
            throw new NotImplementedException();
        }

        public bool TestMyConnection()
        {
            return true;
        }

        
    }
}
