using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.MongoDB.Repository;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Distributr.WSAPI.Lib.Services.Routing;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace Distributr.MongoDB.CommandRouting
{
    public class CommandProcessingAuditRepository :MongoBase, ICommandProcessingAuditRepository
    {
        private string _commandProcessingAuditCollectionName = "CommandProcessingAudit";
        private MongoCollection<CommandProcessingAudit> _commandProcessingAuditCollection;

        public CommandProcessingAuditRepository(string connectionString) : base(connectionString)
        {
          _commandProcessingAuditCollection = CurrentMongoDB.GetCollection<CommandProcessingAudit>(_commandProcessingAuditCollectionName);
          _commandProcessingAuditCollection.EnsureIndex(IndexKeys<CommandProcessingAudit>.Ascending(n => n.Id), IndexOptions.SetUnique(true));
          _commandProcessingAuditCollection.EnsureIndex(IndexKeys<CommandProcessingAudit>.Ascending(n => n.CostCentreApplicationId));
          _commandProcessingAuditCollection.EnsureIndex(IndexKeys<CommandProcessingAudit>.Ascending(n => n.Status));
          _commandProcessingAuditCollection.EnsureIndex(IndexKeys<CommandProcessingAudit>.Ascending(n => n.DocumentId));
          _commandProcessingAuditCollection.EnsureIndex(IndexKeys<CommandProcessingAudit>.Ascending(n => n.ParentDocumentId));
          _commandProcessingAuditCollection.EnsureIndex(IndexKeys<CommandProcessingAudit>.Ascending(n => n.DateInserted));
        }

        public CommandProcessingAudit GetByCommandId(Guid commandId)
        {
            return _commandProcessingAuditCollection
                 .AsQueryable()
                 .FirstOrDefault(n => n.Id == commandId);
        }

        public void AddCommand(CommandProcessingAudit commandProcessingAudit)
        {
            _commandProcessingAuditCollection.Save(commandProcessingAudit);
        }

        public void SetCommandStatus(Guid commandId, CommandProcessingStatus status)
        {
            CommandProcessingAudit command = GetByCommandId(commandId);
            if(command!=null)
            {
                command.Status = status;
                _commandProcessingAuditCollection.Save(command);
            }
        }

        public List<CommandProcessingAudit> GetAll()
        {
            return _commandProcessingAuditCollection
                .AsQueryable().OrderBy(s => s.CostCentreCommandSequence).ThenBy(s => s.CostCentreApplicationId).ToList();
        }

        public List<CommandProcessingAudit> GetAllByStatus(CommandProcessingStatus status)
        {
            DateTime date = DateTime.Now.AddDays(-5);
            foreach (var cmd in _commandProcessingAuditCollection.AsQueryable().Where(s=>s.DateInserted< date && s.Status==CommandProcessingStatus.MarkedForRetry))
            {
                cmd.Status = CommandProcessingStatus.ManualProcessing;
                _commandProcessingAuditCollection.Save(cmd);
            }
            return _commandProcessingAuditCollection.AsQueryable().OrderBy(s=>s.CostCentreCommandSequence).ThenBy(s=>s.CostCentreApplicationId).Where(s=>s.Status==status).ToList();
        }

        public bool IsCreateCommandExecuted(Guid documentId)
        {
            var command = _commandProcessingAuditCollection.AsQueryable().FirstOrDefault(s => s.DocumentId == documentId && s.CommandType.Contains("Create"));
            if (command==null)
                return false;
            if (command.Status != CommandProcessingStatus.Complete)
                return false;
            return true;
        }


        public bool IsAddCommandExecuted(Guid documentId)
        {
            bool isCreated = IsCreateCommandExecuted(documentId);
            if (isCreated == false)
                return false;
         
            var commands = _commandProcessingAuditCollection.AsQueryable().Where(s => s.DocumentId == documentId && s.CommandType.Contains("Add")).ToList();
            if (commands.Count == 0)
                return false;
            if (commands.Any(s => s.Status != CommandProcessingStatus.Complete))
                return false;
            return true;
        }

        public bool IsCommandExecuted(Guid commandId)
        {
            var command = _commandProcessingAuditCollection.AsQueryable().FirstOrDefault(s => s.Id == commandId && s.Status==CommandProcessingStatus.Complete);
            if (command != null)
                return true;
            return false;
        }

        public bool IsConfirmExecuted(Guid documentId)
        {
            var command = _commandProcessingAuditCollection.AsQueryable().FirstOrDefault(s => s.DocumentId == documentId && s.CommandType.Contains("Confirm"));
            if (command == null)
                return false;
            if (command.Status != CommandProcessingStatus.Complete)
                return false;
            return true;
        }

        public List<CommandProcessingAudit> GetByCCAppId(Guid costCentreApplicationId, int dayOfYear, int year)
        {
            var dateRange = GetDateRangeFromDOY(dayOfYear, year);
            return _commandProcessingAuditCollection
               .AsQueryable()
               .Where(n => n.CostCentreApplicationId == costCentreApplicationId
                   && n.DateInserted > dateRange.Item1
                   && n.DateInserted < dateRange.Item2)
               .ToList();
        }

        public List<CommandProcessingAudit> GetByApplicationId(Guid costCentreApplicationId, int pageIndex, int pageSize, CommandProcessingStatus status, out int count)
        {
            IQueryable<CommandProcessingAudit> collection;
            if ((int)status == 0)
            {
                collection = _commandProcessingAuditCollection
                    .AsQueryable()
                    .Where(n => n.CostCentreApplicationId == costCentreApplicationId);
                count = collection.Count();
            }
            else
            {
                collection = _commandProcessingAuditCollection
                    .AsQueryable()
                    .Where(n => n.CostCentreApplicationId == costCentreApplicationId && n.Status == status);
                count = collection.Count();
            }
            return collection.OrderByDescending(n => n.DateInserted)
                .Skip(pageIndex * pageSize).Take(pageSize)
                .ToList(); 
        }

        public IEnumerable<CommandProcessingAuditSummaryDTO> GetCommandProcessedSummary(DateTime? fromDate)
        {
            DateTime _fromDate = fromDate ?? DateTime.Now.AddDays(-30);

            var qry = _commandProcessingAuditCollection
                .AsQueryable()
                .Where(n => n.DateInserted > _fromDate);
            var mongoQuery = ((MongoQueryable<CommandProcessingAudit>)qry).GetMongoQuery();
            var initial = new BsonDocument { { "Count", 0 }, { "OnQueue", 0 }, { "SubscriberProcessBegin", 0 }, { "MarkedForRetry", 0 }, { "Complete", 0 }, { "Failed", 0 } };
            var reduce = (BsonJavaScript)@"function(doc, prev) { prev.Count += 1; 
                                    if(doc.Status == 1) { prev.OnQueue += 1; }
                                    if(doc.Status == 2) { prev.SubscriberProcessBegin += 1; } 
                                    if(doc.Status == 3) { prev.MarkedForRetry += 1; } 
                                    if(doc.Status == 5) { prev.Failed += 1; }
                                    if(doc.Status == 4) { prev.Complete += 1; }  } ";
                    

            var results = _commandProcessingAuditCollection.Group(mongoQuery, "CostCentreApplicationId", initial, reduce, null).ToArray();
            var _results = results.Select(BsonSerializer.Deserialize<CommandProcessingAuditSummaryDTO>);

            return _results.ToList();
        }

        public decimal GetUnQueuedCommands()
        {
            return _commandProcessingAuditCollection
                .AsQueryable().Count(s => s.Status == CommandProcessingStatus.OnQueue);
        }

        public void QueueCommands()
        {
            foreach (var command in  _commandProcessingAuditCollection.AsQueryable().Where(s => s.Status == CommandProcessingStatus.OnQueue))
            {
                command.Status = CommandProcessingStatus.MarkedForRetry;
                AddCommand(command);
            }
        }

        public void Test()
        {
            var cmds =
                _commandProcessingAuditCollection.AsQueryable().Where(s => s.CostCentreApplicationId == Guid.Empty && s.Status != CommandProcessingStatus.Failed);
            int count = cmds.Count();
            foreach (var command in cmds)
            {
                command.Status = CommandProcessingStatus.Failed;
                AddCommand(command);
            }
        }

        public bool TestMyConnection()
        {
            return base.TestConnection();
        }
    }
}
