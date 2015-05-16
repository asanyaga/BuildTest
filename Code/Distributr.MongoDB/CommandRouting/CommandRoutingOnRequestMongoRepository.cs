using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.Recollections;
using Distributr.MongoDB.Repository;
using Distributr.WSAPI.Lib.Services.Routing;
using MongoDB.Driver;
using Distributr.MongoDB.IdCounter;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Distributr.MongoDB.CommandRouting
{
    public class CommandRoutingOnRequestMongoRepository : MongoBase, ICommandRoutingOnRequestRepository
    {
        private string _commandRouteOnRequestCollectionName = "CommandRouteOnRequest";
        private string _commandRouteOnRequestCostcentreCollectionName = "CommandRouteOnRequestCostcentre";
        private string _commandCommandRoutingStatusCollectionName = "CommandRoutingStatus";
        private MongoCollection<CommandRouteOnRequest> _CommandRouteOnRequestCollection;
        private MongoCollection<CommandRouteOnRequestCostcentre> _CommandRouteOnRequestCostcentreCollection;
        private MongoCollection<CommandRoutingStatus> _commandRoutingStatusCollection;
        private IdCounterHelper _counterHelper;

        public CommandRoutingOnRequestMongoRepository(string connectionStringM1):base(connectionStringM1)
        {
            _CommandRouteOnRequestCollection = CurrentMongoDB.GetCollection<CommandRouteOnRequest>(_commandRouteOnRequestCollectionName);
            _CommandRouteOnRequestCostcentreCollection = CurrentMongoDB.GetCollection<CommandRouteOnRequestCostcentre>(_commandRouteOnRequestCostcentreCollectionName);
            _commandRoutingStatusCollection = CurrentMongoDB.GetCollection<CommandRoutingStatus>(_commandCommandRoutingStatusCollectionName);
            _counterHelper= new IdCounterHelper(connectionStringM1);
            
            _CommandRouteOnRequestCollection.EnsureIndex(IndexKeys<CommandRouteOnRequest>.Ascending(n => n.CommandId),IndexOptions.SetUnique(true));
            _CommandRouteOnRequestCollection.EnsureIndex(IndexKeys<CommandRouteOnRequest>.Ascending(n => n.DocumentId));
            _CommandRouteOnRequestCostcentreCollection.EnsureIndex(IndexKeys<CommandRouteOnRequestCostcentre>.Ascending(n=> n.CommandRouteOnRequestId).Ascending(n => n.CostCentreId), IndexOptions.SetUnique(true));
            _CommandRouteOnRequestCostcentreCollection.EnsureIndex(IndexKeys<CommandRouteOnRequestCostcentre>.Ascending(n => n.CostCentreId), IndexOptions.SetUnique(false));
            _commandRoutingStatusCollection.EnsureIndex(IndexKeys<CommandRoutingStatus>.Ascending(n => n.CommandRouteOnRequestId).Ascending(n => n.DestinationCostCentreApplicationId), IndexOptions.SetUnique(true));
            _commandRoutingStatusCollection.EnsureIndex(IndexKeys<CommandRoutingStatus>.Ascending(n => n.CommandId).Ascending(n => n.DestinationCostCentreApplicationId), IndexOptions.SetUnique(true));
        }

        public CommandRouteOnRequest GetById(long id)
        {
            return _CommandRouteOnRequestCollection
                .AsQueryable()
                .FirstOrDefault(n => n.Id == id);
        }

       public List<CommandRouteOnRequestCostcentre> GetByCommandRouteOnRequestId(long id)
       {
           return _CommandRouteOnRequestCostcentreCollection
               .AsQueryable()
               .Where(n => n.CommandRouteOnRequestId == id)
               .ToList();
       }

        public CommandRouteOnRequest GetByCommandId(Guid id)
        {
            return _CommandRouteOnRequestCollection
               .AsQueryable()
               .FirstOrDefault(n => n.CommandId == id);
        }

        public List<CommandRouteOnRequest> GetByDocumentId(Guid id)
        {
            return _CommandRouteOnRequestCollection
                .AsQueryable()
                .Where(n => n.DocumentId == id)
                .ToList();
        }

        public List<CommandRouteOnRequest> GetByParentDocumentId(Guid parentId)
        {
            return _CommandRouteOnRequestCollection
                .AsQueryable()
                .Where(n => n.DocumentParentId == parentId)
                .ToList();
        }

        public CommandRouteOnRequestCostcentre GetByRouteCentreByIdAndCostcentreId(long id, Guid costcentreId)
        {
            return _CommandRouteOnRequestCostcentreCollection.AsQueryable().FirstOrDefault(n => n.CostCentreId == costcentreId && n.CommandRouteOnRequestId == id);
        }

        public CommandRouteOnRequest GetUndeliveredByDestinationCostCentreApplicationId(Guid costCentreApplicationId, Guid costCentreId)
        {
            var items = GetUnexecutedBatchByDestinationCostCentreApplicationId(costCentreApplicationId, costCentreId, 1,false);
            if (!items.Any())
                return null;
            return items[0];
        }

        public long Add(CommandRouteOnRequest commandRouteItem)
        {
            commandRouteItem.DateAdded = DateTime.Now;
            if (commandRouteItem.Id == 0) //To allow for migration
            {
                long nextId = _counterHelper.GetNextId(_commandRouteOnRequestCollectionName);
                commandRouteItem.Id = nextId;
            }
            _CommandRouteOnRequestCollection.Save(commandRouteItem);
            return commandRouteItem.Id;
        }

        public void AddRoutingCentre(CommandRouteOnRequestCostcentre commandRouteOnRequestCostcentre)
        {
            commandRouteOnRequestCostcentre.IsValid = true;
            commandRouteOnRequestCostcentre.DateAdded = DateTime.Now;
            _CommandRouteOnRequestCostcentreCollection.Save(commandRouteOnRequestCostcentre);
        }

        public void MarkAsDelivered(long commandRouteOnRequestId, Guid costCentreApplicationId)
        {
            try
            {
                DateTime dt = DateTime.Now;
                CommandRouteOnRequest cmd = _CommandRouteOnRequestCollection
                                        .AsQueryable()
                                        .FirstOrDefault(n => n.Id == commandRouteOnRequestId);
                if (cmd == null)
                    throw new Exception("MarkAsDelivered could not retrieve commandRouteOnRequestId : " + commandRouteOnRequestId);

                var commandRoutingStatus = new CommandRoutingStatus
                {
                    DateAdded = dt,
                    Delivered = true,
                    DateDelivered = dt,
                    DestinationCostCentreApplicationId = costCentreApplicationId,
                    CommandRouteOnRequestId = commandRouteOnRequestId,
                    CommandId = cmd.CommandId,
                };

                _commandRoutingStatusCollection.Save(commandRoutingStatus);
            }
            catch (Exception)
            {
            }
           
        }

        public void MardAdDeliveredAndExecuted(long commandRouteOnRequestId, Guid costCentreApplicationId)
        {
            MarkAsDelivered(commandRouteOnRequestId, costCentreApplicationId);

        }

        public void MarkBatchAsDelivered(List<long> commandRouteOnRequestIds, Guid costCentreApplicationId)
        {
            foreach (long id in commandRouteOnRequestIds)
            {
                if (id != 0)
                {
                    MarkAsDelivered(id, costCentreApplicationId);
                }
            }
        }

        public List<CommandRouteOnRequest> GetUnexecutedBatchByDestinationCostCentreApplicationId(Guid costCentreApplicationId, Guid costCentreId, int batchSize, bool includeArchived)
        {
            long[] commandDeliveredId = _commandRoutingStatusCollection
                .AsQueryable()
                .Where(n => n.DestinationCostCentreApplicationId == costCentreApplicationId)
                .OrderByDescending(n => n.CommandRouteOnRequestId).Select(s=>s.CommandRouteOnRequestId).ToArray();
               

           
            //get a bunch of commandrouteonrequestids
            long[] commandRouteRequestIds = _CommandRouteOnRequestCostcentreCollection
                .AsQueryable()
                .Where(
                    n =>
                    n.CostCentreId == costCentreId && !commandDeliveredId.Contains(n.CommandRouteOnRequestId) &&
                    n.IsValid)
                .OrderBy(n => n.CommandRouteOnRequestId)
                .Select(n => n.CommandRouteOnRequestId)
                //.Take(batchSize*10)
                .ToArray();

            var items = _CommandRouteOnRequestCollection.AsQueryable();
            items = items.Where(n => n.Id.In(commandRouteRequestIds));
            items =items.Where(n => n.CommandGeneratedByCostCentreApplicationId != costCentreApplicationId );
            if(!includeArchived)
            {
                items = items.Where(n => !n.IsRetired);
            }
            items = items.OrderBy(n => n.Id);
            items = items.Take(batchSize);
            return items.ToList();
        }
        public List<CommandRouteOnRequest> GetUnexecutedBatchByDestinationCostCentreApplicationIdold(Guid costCentreApplicationId, Guid costCentreId, int batchSize)
        {
            //get last commandrouteonrequestid for ccappid
            var lastCommandRouteOnRequest = _commandRoutingStatusCollection
                .AsQueryable()
                .Where(n => n.DestinationCostCentreApplicationId == costCentreApplicationId)
                .OrderByDescending(n => n.CommandRouteOnRequestId)
                .FirstOrDefault();
            long lastCommandRouteOnRequestId = 0;
            if (lastCommandRouteOnRequest != null)
                lastCommandRouteOnRequestId = lastCommandRouteOnRequest.CommandRouteOnRequestId;


            //get a bunch of commandrouteonrequestids
            long[] commandRouteRequestIds = _CommandRouteOnRequestCostcentreCollection
                .AsQueryable()
                .Where(
                    n =>
                    n.CostCentreId == costCentreId && n.CommandRouteOnRequestId > lastCommandRouteOnRequestId &&
                    n.IsValid)
                .OrderBy(n => n.CommandRouteOnRequestId)
                .Select(n => n.CommandRouteOnRequestId)
                //.Take(batchSize*10)
                .ToArray();

            var items = _CommandRouteOnRequestCollection.AsQueryable();
            items = items.Where(n => n.Id.In(commandRouteRequestIds));
            items = items.Where(n => n.CommandGeneratedByCostCentreApplicationId != costCentreApplicationId && !n.IsRetired);
            items = items.OrderBy(n => n.Id);
            items = items.Take(batchSize);
            return items.ToList();
        }

        public void RetireCommands(Guid documentParentId)
        {

            long[] commandOnRouteRequestIdsToUpdate = _CommandRouteOnRequestCollection
                .AsQueryable()
                .Where(n => n.DocumentParentId == documentParentId)
                .Select(n => n.Id)
                .ToArray();
            foreach (long idToUpdate in commandOnRouteRequestIdsToUpdate)
            {
                CommandRouteOnRequest cmd = GetById(idToUpdate);
                cmd.IsRetired = true;
                _CommandRouteOnRequestCollection.Save(cmd);
                CommandRouteOnRequestCostcentre[] ccToUpdate = _CommandRouteOnRequestCostcentreCollection
                    .AsQueryable()
                    .Where(n => n.CommandRouteOnRequestId == idToUpdate)
                    .ToArray();
                foreach (var commandRouteOnRequestCostcentre in ccToUpdate)
                {
                    commandRouteOnRequestCostcentre.IsRetired = true;
                    _CommandRouteOnRequestCostcentreCollection.Save(commandRouteOnRequestCostcentre);
                }
            }
        }

        public void UnRetireCommands(Guid documentParentId)
        {
            long[] commandOnRouteRequestIdsToUpdate = _CommandRouteOnRequestCollection
                .AsQueryable()
                .Where(n => n.DocumentParentId == documentParentId)
                .Select(n => n.Id)
                .ToArray();
            foreach (long idToUpdate in commandOnRouteRequestIdsToUpdate)
            {
                CommandRouteOnRequest cmd = GetById(idToUpdate);
                cmd.IsRetired = false;
                _CommandRouteOnRequestCollection.Save(cmd);
                CommandRouteOnRequestCostcentre[] ccToUpdate = _CommandRouteOnRequestCostcentreCollection
                    .AsQueryable()
                    .Where(n => n.CommandRouteOnRequestId == idToUpdate)
                    .ToArray();
                foreach (var commandRouteOnRequestCostcentre in ccToUpdate)
                {
                    commandRouteOnRequestCostcentre.IsRetired = false;
                    _CommandRouteOnRequestCostcentreCollection.Save(commandRouteOnRequestCostcentre);
                }
            }
        }

        public void MarkCommandsAsInvalid(Guid costCentreId)
        {

            DateTime currentdate = DateTime.Now;
            IEnumerable<string> commandTypelist = GetExcludedCommandTypelist();
            CommandRouteOnRequestCostcentre[] commandsToupdate = _CommandRouteOnRequestCostcentreCollection
                .AsQueryable()
                .Where(s=>s.CostCentreId==costCentreId && s.IsValid)
                .Where(s =>commandTypelist.Contains(s.CommandType)).ToArray();
            foreach (var commandRouteOnRequestCostcentre in commandsToupdate)
            {
                commandRouteOnRequestCostcentre.IsValid = false;
                _CommandRouteOnRequestCostcentreCollection.Save(commandRouteOnRequestCostcentre);
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
           // to do clean up 
        }

        public List<CommandRouteOnRequestCostcentre> TestCC(Guid ccid)
        {
            return _CommandRouteOnRequestCostcentreCollection
                .AsQueryable()
                .Where(n => n.CostCentreId == ccid)
                .ToList();
        }


        public List<RouteOnRequestSummary> GetCCRouteOnRequestSummary(DateTime? fromDate)
        {
            DateTime _fromDate = fromDate ?? DateTime.Now.AddDays(-30);
            var qry = _CommandRouteOnRequestCostcentreCollection
                .AsQueryable()
                .Where(n => n.DateAdded > _fromDate);
            var mongoQuery = ((MongoQueryable<CommandRouteOnRequestCostcentre>) qry).GetMongoQuery();
            var initial = new BsonDocument { {"Count",0}, {"ValidCount",0}, {"RetiredCount",0}  };
            var reduce = (BsonJavaScript)@"function(doc, prev) { prev.Count += 1; 
                                    if(doc.IsValid == 1) { prev.ValidCount += 1; }
                                    if(doc.IsRetired == 1) { prev.RetiredCount += 1; }
                                    } ";

            var results = _CommandRouteOnRequestCostcentreCollection.Group(mongoQuery, "CostCentreId", initial, reduce, null).ToArray();
            var _results = results.Select(BsonSerializer.Deserialize<RouteOnRequestSummary>);
            return _results.ToList();
        }

        public List<RouteOnRequestDeliveredSummary> GetCCAppIdRouteOnRequestDeliveredSummary(DateTime? fromDate)
        {
            DateTime _fromDate = fromDate ?? DateTime.Now.AddDays(-30);
            var qry = _commandRoutingStatusCollection
                .AsQueryable()
                .Where(n => n.DateDelivered > _fromDate);
            var mongoQry = ((MongoQueryable<CommandRoutingStatus>) qry).GetMongoQuery();
            var initial = new BsonDocument {{"DeliveredCount", 0}};
            var reduce = (BsonJavaScript)@"function(doc, prev) { prev.DeliveredCount += 1; } ";
            var results = _commandRoutingStatusCollection.Group(mongoQry, "DestinationCostCentreApplicationId", initial, reduce, null).ToArray();
            var _results = results.Select(BsonSerializer.Deserialize<RouteOnRequestDeliveredSummary>);
            return _results.ToList();
        }


        public List<CostCentreRouteOnRequestDetail> GetCostCentreRouteOnRequestDetail(Guid costCentreId, int dayOfYear, int year)
        {
            var dateRange = GetDateRangeFromDOY(dayOfYear, year);
            var rorCCItems = _CommandRouteOnRequestCostcentreCollection
               .AsQueryable()
               .Where(n => n.CostCentreId == costCentreId
                   && n.DateAdded > dateRange.Item1
                   && n.DateAdded < dateRange.Item2)
               .ToList();

            long[] routeOnRequestIds = rorCCItems.Select(n => n.CommandRouteOnRequestId).ToArray();

            var rors = _CommandRouteOnRequestCollection
                .AsQueryable()
                .Where(n => n.Id.In(routeOnRequestIds))
                .ToList();

            var routedItems = _commandRoutingStatusCollection
                .AsQueryable()
                .Where(n => n.CommandRouteOnRequestId.In(routeOnRequestIds))
                .ToList();
            var results = new List<CostCentreRouteOnRequestDetail>();
            foreach (var rorid in routeOnRequestIds)
            {
                CommandRouteOnRequestCostcentre rorCC = rorCCItems.First(n => n.CommandRouteOnRequestId == rorid);
                CommandRouteOnRequest ror = rors.First(n => n.Id == rorid);
                CommandRoutingStatus rs = routedItems.FirstOrDefault(n => n.CommandRouteOnRequestId == rorid);
                var ri = new CostCentreRouteOnRequestDetail
                    {
                        CommandRouteOnRequestId = rorid,
                        IsValid = rorCC.IsValid,
                        DateAdded = rorCC.DateAdded,
                        IsRetired = rorCC.IsRetired,
                        DocumentId = ror.DocumentId.ToString(),
                        CommandGeneratedByCostCentreApplicationId = ror.CommandGeneratedByCostCentreApplicationId,
                        ParentDocumentId = ror.DocumentParentId.ToString(),
                        JsonCommand = ror.JsonCommand,
                        CommandType = ror.CommandType
                    };
                if (rs != null)
                {
                    ri.Delivered = true;
                    ri.DateDelivered = rs.DateDelivered;
                }
                results.Add(ri);
            }

            return results;
        }

        public List<long> GetCommandsAsIntegers(Guid costCentreId, DateTime date, int pageIndex, int pageSize, out int count)
        {
            IQueryable<long> commandIds = _CommandRouteOnRequestCostcentreCollection
                .AsQueryable()
                .Where(n => n.CostCentreId == costCentreId && n.DateAdded > date.Date && n.DateAdded < date.Date.AddDays(1))
                .OrderByDescending(n => n.DateAdded)
                .Select(n => n.CommandRouteOnRequestId);
            count = commandIds.Count();
            return commandIds.Skip(pageIndex*pageSize).Take(pageSize).ToList();
        }

        public List<CCComandRoutingItem> GetCommandRoutingItems(List<long> costCentreCommands, List<Guid> costCentreApps)
        {
            var deliveredItems = _commandRoutingStatusCollection.AsQueryable()
                .Where(n => costCentreCommands.Contains(n.CommandRouteOnRequestId)
                            && costCentreApps.Contains(n.DestinationCostCentreApplicationId))
                .OrderByDescending(n => n.DateAdded).ToList()
                .Select(n => new CCComandRoutingItem
                                 {
                                     CommandId = n.CommandId,
                                     CommandAsInteger = n.CommandRouteOnRequestId,
                                     Delivered = n.Delivered,
                                     DateDelivered = n.DateDelivered,
                                     DateProcessed = n.DateAdded
                                 });
            List<long> deliveredCommands = deliveredItems.Select(n => n.CommandAsInteger).ToList();
            List<long> undeliveredCommands = costCentreCommands.Where(x => deliveredCommands.All(y => y != x)).ToList();
            var undeliveredItems = undeliveredCommands
                .Select(n => new CCComandRoutingItem
                                 {
                                     CommandId = Guid.Empty,
                                     CommandAsInteger = n,
                                     Delivered = false,
                                     DateDelivered = null,
                                     DateProcessed = null
                                 }).ToList();
            var items = deliveredItems.ToList().Concat(undeliveredItems);
            return items.ToList();
        }

        public List<CommandRef> GetCommandRefs(List<long> costCentreCommands)
        {
            var commandRefs = _CommandRouteOnRequestCollection
                .AsQueryable()
                .Where(n => costCentreCommands.Contains(n.Id))
                .Select(n => new CommandRef
                                 {
                                     CommandIdAsInteger = n.Id,
                                     CommandId = n.CommandId,
                                     DocumentId = n.DocumentId,
                                     CommandType = n.CommandType,
                                     CommandGeneratedByApplicationId = n.CommandGeneratedByCostCentreApplicationId,
                                     CommandGeneratedByUserId = n.CommandGeneratedByUserId
                                 })
                .ToList();
            return commandRefs;
        }
    }
}
