using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Commands;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Utility;
using Distributr.MongoDB.Repository;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Distributr.WSAPI.Lib.Services.Routing;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;

namespace Distributr.MongoDB.CommandRouting
{
    /// <summary>
    /// Main logic for routing here.
    /// </summary>

    public class CommandEnvelopeRouteOnRequestCostcentreRepository : MongoBase, ICommandEnvelopeRouteOnRequestCostcentreRepository, ICommandEnvelopeProcessingAuditRepository
    {
        private string _commandEnvelopeRouteOnRequestCostcentreCollectionName = "CommandEnvelopeRouteOnRequestCostcentre";
        private string _commandEnvelopeRoutingStatusCollectionName = "CommandEnvelopeRoutingStatus";
        private string _commandEnvelopeRoutingTrackerCollectionName = "CommandEnvelopeRoutingTracker";

        //make public for now to facilitate migration
        public MongoCollection<CommandEnvelopeRouteOnRequestCostcentre> _commandEnvelopeRouteOnRequestCostcentreCollection;
        public MongoCollection<CommandEnvelopeRoutingStatus> _commandEnvelopeRoutingStatusCollection;
        public MongoCollection<CommandEnvelopeRoutingTracker> _CommandEnvelopeRoutingTrackerCollection;
        //private ILog _log = LogManager.GetLogger("CommandEnvelopeRouteOnRequestCostcentreRepository");
        private string _commandEnvelopeProcessingAuditCollectionName = "CommandEnvelopeProcessingAudit";
        private MongoCollection<CommandEnvelopeProcessingAudit> _commandEnvelopeProcessingAuditCollection;
        public CommandEnvelopeRouteOnRequestCostcentreRepository(string connectionString)
            : base(connectionString)
        {
            _commandEnvelopeRouteOnRequestCostcentreCollection = CurrentMongoDB.GetCollection<CommandEnvelopeRouteOnRequestCostcentre>(_commandEnvelopeRouteOnRequestCostcentreCollectionName);
            _commandEnvelopeRouteOnRequestCostcentreCollection.EnsureIndex(IndexKeys<CommandEnvelopeRouteOnRequestCostcentre>.Ascending(n => n.Id), IndexOptions.SetUnique(true));
            _commandEnvelopeRouteOnRequestCostcentreCollection.EnsureIndex(IndexKeys<CommandEnvelopeRouteOnRequestCostcentre>.Ascending(n => n.EnvelopeId));
            _commandEnvelopeRouteOnRequestCostcentreCollection.EnsureIndex(IndexKeys<CommandEnvelopeRouteOnRequestCostcentre>.Ascending(n => n.CostCentreId));
            _commandEnvelopeRouteOnRequestCostcentreCollection.EnsureIndex(IndexKeys<CommandEnvelopeRouteOnRequestCostcentre>.Ascending(n => n.EnvelopeRoutePriority));
            _commandEnvelopeRouteOnRequestCostcentreCollection.EnsureIndex(IndexKeys<CommandEnvelopeRouteOnRequestCostcentre>.Ascending(n => n.EnvelopeArrivalAtServerTick));
            _commandEnvelopeRouteOnRequestCostcentreCollection.EnsureIndex(IndexKeys<CommandEnvelopeRouteOnRequestCostcentre>.Ascending(n => n.DocumentType));

            _commandEnvelopeRoutingStatusCollection = CurrentMongoDB.GetCollection<CommandEnvelopeRoutingStatus>(_commandEnvelopeRoutingStatusCollectionName);
            _commandEnvelopeRoutingStatusCollection.EnsureIndex(IndexKeys<CommandEnvelopeRoutingStatus>.Ascending(n => n.Id), IndexOptions.SetUnique(true));
            _commandEnvelopeRoutingStatusCollection.EnsureIndex(IndexKeys<CommandEnvelopeRoutingStatus>.Ascending(n => n.DestinationCostCentreApplicationId));
            _commandEnvelopeRoutingStatusCollection.EnsureIndex(IndexKeys<CommandEnvelopeRoutingStatus>.Ascending(n => n.EnvelopeDeliveredAtServerTick));

            _commandEnvelopeProcessingAuditCollection = CurrentMongoDB.GetCollection<CommandEnvelopeProcessingAudit>(_commandEnvelopeProcessingAuditCollectionName);
            _commandEnvelopeProcessingAuditCollection.EnsureIndex(IndexKeys<CommandEnvelopeProcessingAudit>.Ascending(n => n.Id), IndexOptions.SetUnique(true));
            _commandEnvelopeProcessingAuditCollection.EnsureIndex(IndexKeys<CommandEnvelopeProcessingAudit>.Ascending(n => n.GeneratedByCostCentreApplicationId));
            _commandEnvelopeProcessingAuditCollection.EnsureIndex(IndexKeys<CommandEnvelopeProcessingAudit>.Ascending(n => n.RecipientCostCentreId));
            _commandEnvelopeProcessingAuditCollection.EnsureIndex(IndexKeys<CommandEnvelopeProcessingAudit>.Ascending(n => n.DocumentId));
            _commandEnvelopeProcessingAuditCollection.EnsureIndex(IndexKeys<CommandEnvelopeProcessingAudit>.Ascending(n => n.Status));
            _commandEnvelopeProcessingAuditCollection.EnsureIndex(IndexKeys<CommandProcessingAudit>.Ascending(n => n.ParentDocumentId));
            _commandEnvelopeProcessingAuditCollection.EnsureIndex(IndexKeys<CommandProcessingAudit>.Ascending(n => n.DateInserted));

            _CommandEnvelopeRoutingTrackerCollection = CurrentMongoDB.GetCollection<CommandEnvelopeRoutingTracker>(_commandEnvelopeRoutingTrackerCollectionName);

            _CommandEnvelopeRoutingTrackerCollection.CreateIndex(IndexKeys<CommandEnvelopeRoutingTracker>.Ascending(n => n.Id), IndexOptions.SetUnique(true));
            _CommandEnvelopeRoutingTrackerCollection.CreateIndex(IndexKeys<CommandEnvelopeRoutingTracker>.Ascending(n => n.EnvelopeArrivalAtServerTick));
            _CommandEnvelopeRoutingTrackerCollection.CreateIndex(IndexKeys<CommandEnvelopeRoutingTracker>.Ascending(n => n.EnvelopeId));
        }

        public void AddCommandEnvelopeRouteCentre(CommandEnvelopeRouteOnRequestCostcentre commandRouteCentre)
        {
            _commandEnvelopeRouteOnRequestCostcentreCollection.Save(commandRouteCentre);
        }

        public void AddCommandEnvelopeRouteCentre(CommandEnvelope envelope)
        {
            //add recepient
            List<Guid> centreList = new List<Guid>();
            centreList.Add(envelope.RecipientCostCentreId);
            centreList.Add(envelope.GeneratedByCostCentreId);
            if (envelope.OtherRecipientCostCentreList != null && envelope.OtherRecipientCostCentreList.Count > 0)
            {
                envelope.OtherRecipientCostCentreList.ForEach(centreList.Add);
            }
            foreach (var centreId in centreList.Distinct().ToList())
            {
                _commandEnvelopeRouteOnRequestCostcentreCollection.Save(
               new CommandEnvelopeRouteOnRequestCostcentre
               {
                   CostCentreId = centreId,
                   DocumentType = ((DocumentType)envelope.DocumentTypeId).ToString(),
                   DateAdded = DateTime.Now,
                   EnvelopeId = envelope.Id,
                   IsRetired = false,
                   IsValid = true,
                   Id = Guid.NewGuid(),
                   EnvelopeArrivalAtServerTick = envelope.EnvelopeArrivedAtServerTick,
                   EnvelopeRoutePriority = EnvelopeRoutePriority.Level1,
                   EnvelopeRoutedAtServerTick = DateTime.Now.Ticks,
                   GeneratedByCostCentreApplicationId = envelope.GeneratedByCostCentreApplicationId,
                   DocumentId = envelope.DocumentId,
                   ParentDocumentId = envelope.ParentDocumentId

               });
            }


        }

        public void AddCommandEnvelopeRoutingStatus(CommandEnvelopeRoutingStatus commandEnvelopeRoutingStatus)
        {
            _commandEnvelopeRoutingStatusCollection.Save(commandEnvelopeRoutingStatus);
        }

        public void MarkEnvelopesAsDelivered(List<Guid> envelopesIdList, Guid costCentreApplicationId, Guid costCentreId)
        {
            var envelopes =
                _commandEnvelopeProcessingAuditCollection.AsQueryable()
                    .Where(s => envelopesIdList.Contains(s.Id))
                    .ToList()
                    .Select(n => new { Id = n.Id, ArrivalTick = n.EnvelopeArrivalAtServerTick, n.DocumentType })
                    .OrderBy(n => n.ArrivalTick);
            foreach (var envelope in envelopes)
            {
                var alreadyMarked = _commandEnvelopeRoutingStatusCollection.AsQueryable()
                    .Any(
                        s =>
                            s.EnvelopeId == envelope.Id &&
                            s.DestinationCostCentreApplicationId == costCentreApplicationId);
                if (!alreadyMarked)
                {
                    bool isReconcile =
                        _commandEnvelopeRoutingStatusCollection.AsQueryable().
                        Where(s => s.DestinationCostCentreApplicationId == costCentreApplicationId)
                            .Any(s => s.EnvelopeArrivalAtServerTick > envelope.ArrivalTick);
                    if (isReconcile)
                    {
                        var tracker = _CommandEnvelopeRoutingTrackerCollection.AsQueryable().FirstOrDefault(s => s.Id == costCentreApplicationId);
                        if (tracker == null)
                        {
                            tracker = new CommandEnvelopeRoutingTracker
                            {
                                Id = costCentreApplicationId,
                                EnvelopeArrivalAtServerTick = envelope.ArrivalTick,
                                EnvelopeId = envelope.Id
                            };
                            _CommandEnvelopeRoutingTrackerCollection.Save(tracker);
                        }
                        else
                        {
                            tracker = new CommandEnvelopeRoutingTracker
                            {
                                Id = costCentreApplicationId,
                                EnvelopeArrivalAtServerTick = envelope.ArrivalTick,
                                EnvelopeId = envelope.Id
                            };
                            _CommandEnvelopeRoutingTrackerCollection.Save(tracker);
                        }

                    }
                    _commandEnvelopeRoutingStatusCollection.Save(new CommandEnvelopeRoutingStatus
                    {
                        DateAdded = DateTime.Now,
                        DateDelivered = DateTime.Now,
                        DateExecuted = DateTime.Now,
                        Delivered = true,
                        DestinationCostCentreApplicationId =
                            costCentreApplicationId,
                        EnvelopeDeliveredAtServerTick = isReconcile ? envelope.ArrivalTick : DateTime.Now.Ticks,
                        EnvelopeArrivalAtServerTick = envelope.ArrivalTick,
                        EnvelopeId = envelope.Id,
                        Id = Guid.NewGuid(),
                        CostCentreId = costCentreId,
                        DocumentType = envelope.DocumentType.ToString(),
                        IsReconcile = isReconcile

                    });
                }
            }
        }

        public List<CommandEnvelope> GetUnDeliveredEnvelopesByDestinationCostCentreApplicationId(Guid costCentreApplicationId, Guid costCentreId,
            int batchSize, bool includeArchived)
        {
            var lastEnvelope = _commandEnvelopeRoutingStatusCollection
               .AsQueryable().Where(s => s.DocumentType != null)
               .Where(s => !s.IsReconcile)
               .Where(n => n.DestinationCostCentreApplicationId == costCentreApplicationId && n.DocumentType != DocumentType.InventoryTransferNote.ToString())
               .OrderByDescending(n => n.EnvelopeDeliveredAtServerTick).FirstOrDefault();
            var undeliveredQuery =
                _commandEnvelopeRouteOnRequestCostcentreCollection.AsQueryable()
                    .OrderBy(s => s.EnvelopeArrivalAtServerTick)
                   .Where(s => s.DocumentType != null)
                    .Where(s => s.CostCentreId == costCentreId && s.IsValid && s.DocumentType != DocumentType.InventoryTransferNote.ToString())
                    .Where(s => s.GeneratedByCostCentreApplicationId != costCentreApplicationId);
            var alreadyDeliveredIds = _commandEnvelopeRoutingStatusCollection
                  .AsQueryable()
                  .Where(n => n.DestinationCostCentreApplicationId == costCentreApplicationId)
                  .Where(n => n.DocumentType != DocumentType.InventoryTransferNote.ToString())
                  .Select(s => s.EnvelopeId).Distinct().ToList();
            if (lastEnvelope != null)
            {
                undeliveredQuery =
                    undeliveredQuery.Where(s => s.EnvelopeArrivalAtServerTick > lastEnvelope.EnvelopeArrivalAtServerTick);

            }

            var routedIds = undeliveredQuery.Select(s => s.EnvelopeId).ToList().Distinct().ToList();
            var allUndeliveredEnvelopeIds = routedIds.Except(alreadyDeliveredIds).ToList();
            // var undeliveredEnvelopeIds = allUndeliveredEnvelopeIds.Take(batchSize).ToList();
            var undeliveredEnvelopeIds =
              _commandEnvelopeRouteOnRequestCostcentreCollection.AsQueryable()
                  .Where(s => s.GeneratedByCostCentreApplicationId != costCentreApplicationId)
                  .Where(s => allUndeliveredEnvelopeIds.Contains(s.EnvelopeId))
                  .OrderBy(n => n.EnvelopeArrivalAtServerTick)
                  .Select(s => s.EnvelopeId).ToList().Distinct().ToList()
                  .Take(batchSize)
                  .ToList();
            var envelopes =
                _commandEnvelopeProcessingAuditCollection.AsQueryable().Where(s => s.GeneratedByCostCentreApplicationId != costCentreApplicationId)
                    .Where(s => undeliveredEnvelopeIds.Contains(s.Id)).OrderBy(s => s.EnvelopeArrivalAtServerTick);

            var envelopeToSend = new List<CommandEnvelope>();
            foreach (var commandEnvelopeProcessingAudit in envelopes)
            {
                var envelope =
                    JsonConvert.DeserializeObject<CommandEnvelope>(commandEnvelopeProcessingAudit.JsonEnvelope);
                envelopeToSend.Add(envelope);
            }
            if (envelopeToSend.Count == 0)
            {
                envelopeToSend =
                    GetUnDeliveredEnvelopesFromThePastByDestinationCostCentreApplicationId(costCentreApplicationId,
                        costCentreId, batchSize, includeArchived);
            }
            return envelopeToSend;

        }

        public List<CommandEnvelope> GetUnDeliveredInventoryEnvelopesByDestinationCostCentreApplicationId(Guid costCentreApplicationId,
            Guid costCentreId, int batchSize, bool includeArchived)
        {
            //
            CommandEnvelopeRoutingStatus lastEnvelope = _commandEnvelopeRoutingStatusCollection
                .AsQueryable()
                  .AsQueryable().Where(s => !string.IsNullOrEmpty(s.DocumentType))
                .Where(n => n.DestinationCostCentreApplicationId == costCentreApplicationId && n.DocumentType == DocumentType.InventoryTransferNote.ToString())
                .OrderByDescending(n => n.EnvelopeDeliveredAtServerTick).FirstOrDefault();
            //undelivered envelopes
            var undeliveredQuery =
                _commandEnvelopeRouteOnRequestCostcentreCollection.AsQueryable()
                  .AsQueryable().OrderBy(s => s.EnvelopeArrivalAtServerTick)
                  .Where(s => !string.IsNullOrEmpty(s.DocumentType))
                    .Where(s => s.CostCentreId == costCentreId && s.IsValid)
                    .Where(s => s.GeneratedByCostCentreApplicationId != costCentreApplicationId && s.DocumentType == DocumentType.InventoryTransferNote.ToString());

            var envelopeToSend = new List<CommandEnvelope>();
            //check already delivered
            var alreadyDeliveredIds = _commandEnvelopeRoutingStatusCollection
                 .AsQueryable()
                 .Where(n => n.DestinationCostCentreApplicationId == costCentreApplicationId)
                 .Where(n => n.DocumentType == DocumentType.InventoryTransferNote.ToString())
                 .Select(s => s.EnvelopeId).Distinct().ToList();
            if (lastEnvelope != null)
            {
                undeliveredQuery =
                    undeliveredQuery.Where(s => s.EnvelopeArrivalAtServerTick > lastEnvelope.EnvelopeArrivalAtServerTick);
            }


            var routedIds = undeliveredQuery.Select(s => s.EnvelopeId).ToList().Distinct().ToList();
            var allUndeliveredEnvelopeIds = routedIds.Except(alreadyDeliveredIds).ToList();
            //   var undeliveredEnvelopeIds = allUndeliveredEnvelopeIds.Take(batchSize).ToList();
            var undeliveredEnvelopeIds =
               _commandEnvelopeRouteOnRequestCostcentreCollection.AsQueryable()
                   .Where(s => s.GeneratedByCostCentreApplicationId != costCentreApplicationId)
                   .Where(s => allUndeliveredEnvelopeIds.Contains(s.EnvelopeId))
                   .OrderBy(n => n.EnvelopeArrivalAtServerTick)
                   .Select(s => s.EnvelopeId).ToList().Distinct().ToList()
                   .Take(batchSize)
                   .ToList();
            var envelopes =
                _commandEnvelopeProcessingAuditCollection.AsQueryable().Where(s => s.GeneratedByCostCentreApplicationId != costCentreApplicationId)
                    .Where(s => undeliveredEnvelopeIds.Contains(s.Id)).OrderBy(s => s.EnvelopeArrivalAtServerTick);


            foreach (var commandEnvelopeProcessingAudit in envelopes)
            {
                var envelope =
                    JsonConvert.DeserializeObject<CommandEnvelope>(commandEnvelopeProcessingAudit.JsonEnvelope);
                envelopeToSend.Add(envelope);
            }

            return envelopeToSend;
        }

        private List<CommandEnvelope> GetUnDeliveredEnvelopesFromThePastByDestinationCostCentreApplicationId(Guid costCentreApplicationId,
            Guid costCentreId, int batchSize, bool includeArchived)
        {
            var tracker = _CommandEnvelopeRoutingTrackerCollection.AsQueryable().FirstOrDefault(s => s.Id == costCentreApplicationId);
            if (tracker == null)
            {
                tracker = new CommandEnvelopeRoutingTracker
               {
                   Id = costCentreApplicationId,
                   EnvelopeArrivalAtServerTick = 0,
                   EnvelopeId = Guid.NewGuid()
               };
                _CommandEnvelopeRoutingTrackerCollection.Save(tracker);
            }
            List<Guid> undeliveredIds;

            var allDeliveredIds = _commandEnvelopeRoutingStatusCollection
                .AsQueryable()
                .Where(n => n.DestinationCostCentreApplicationId == costCentreApplicationId)
                .Where(n => n.DocumentType != DocumentType.InventoryTransferNote.ToString())
                .Where(n => n.EnvelopeArrivalAtServerTick > tracker.EnvelopeArrivalAtServerTick)
                .Select(s => s.EnvelopeId).Distinct().ToList();
            var allRoutedIds = _commandEnvelopeRouteOnRequestCostcentreCollection.AsQueryable()
                  .Where(s => s.CostCentreId == costCentreId && s.IsValid && s.DocumentType != DocumentType.InventoryTransferNote.ToString())
                  .Where(s => s.GeneratedByCostCentreApplicationId != costCentreApplicationId)
                  .Where(n => n.EnvelopeArrivalAtServerTick > tracker.EnvelopeArrivalAtServerTick)
                  .Select(s => s.EnvelopeId).Distinct().ToList();
            undeliveredIds = allRoutedIds.Except(allDeliveredIds).ToList();


            var undeliveredEnvelopeIds =
             _commandEnvelopeRouteOnRequestCostcentreCollection.AsQueryable()
                 .Where(s => s.GeneratedByCostCentreApplicationId != costCentreApplicationId)
                 .Where(s => undeliveredIds.Contains(s.EnvelopeId))
                 .OrderBy(n => n.EnvelopeArrivalAtServerTick)
                 .Select(s => s.EnvelopeId).ToList().Distinct().ToList()
                 .Take(batchSize)
                 .ToList();



            var envelopes =
                _commandEnvelopeProcessingAuditCollection.AsQueryable()
                    .Where(s => s.GeneratedByCostCentreApplicationId != costCentreApplicationId)
                    .Where(s => undeliveredEnvelopeIds.Contains(s.Id)).OrderBy(s => s.EnvelopeArrivalAtServerTick);
            var envelopeToSend = new List<CommandEnvelope>();
            foreach (var commandEnvelopeProcessingAudit in envelopes)
            {
                var envelope =
                    JsonConvert.DeserializeObject<CommandEnvelope>(commandEnvelopeProcessingAudit.JsonEnvelope);
                envelopeToSend.Add(envelope);
            }

            return envelopeToSend;
        }

        public void MarkEnvelopeAsInvalid(Guid costCentreId)
        {
            //exclude all inventory based envelopes as snapshot will be taken
            DateTime currentdate = DateTime.Now;
            var commandTypelist = GetExcludedDocumentTypelist().ToList();
            var commandsToupdate = _commandEnvelopeRouteOnRequestCostcentreCollection
                .AsQueryable()
                .Where(s => s.CostCentreId == costCentreId && s.IsValid)
                .Where(s => commandTypelist.Contains(s.DocumentType)).ToArray();
            foreach (var commandRouteOnRequestCostcentre in commandsToupdate)
            {
                commandRouteOnRequestCostcentre.IsValid = false;
                _commandEnvelopeRouteOnRequestCostcentreCollection.Save(commandRouteOnRequestCostcentre);
            }

        }

        public void RetireEnvelopes(Guid documentId)
        {
            var commandOnRouteRequestIdsToUpdate = _commandEnvelopeRouteOnRequestCostcentreCollection
                .AsQueryable()
                .Where(n => n.ParentDocumentId == documentId).ToList();


            foreach (var idToUpdate in commandOnRouteRequestIdsToUpdate)
            {

                idToUpdate.IsRetired = true;
                _commandEnvelopeRouteOnRequestCostcentreCollection.Save(idToUpdate);

            }
        }

        public void UpdateStatus()
        {
            while (true)
            {
                var data = _commandEnvelopeRoutingStatusCollection.AsQueryable().Where(s => s.DocumentType == null).Take(10000).ToList();
                if (data.Count == 0)
                {
                    break;

                }
                foreach (var status in data)
                {
                    var audit = GetById(status.EnvelopeId);
                    if (audit != null)
                    {
                        status.DocumentType = audit.DocumentTypeName;
                        _commandEnvelopeRoutingStatusCollection.Save(status);
                    }
                }
            }

        }

        private IEnumerable<string> GetExcludedDocumentTypelist()
        {
            return new[]
                   {
                       DocumentType.InventoryAdjustmentNote.ToString(),
                       DocumentType.InventoryTransferNote.ToString(),
                       DocumentType.InventoryReceivedNote.ToString(),
                       DocumentType.CreateInventorySerialsPlaceholder.ToString(),
                       DocumentType.RecollectionNote.ToString(),
                       DocumentType.PaymentNote.ToString(),
                       DocumentType.RetirePlaceholder.ToString(),
                   };
        }

        public void AddCommand(CommandEnvelopeProcessingAudit processingAudit)
        {
            _commandEnvelopeProcessingAuditCollection.Save(processingAudit);
        }

        public List<CommandEnvelopeProcessingAudit> GetByDocumentId(Guid documentId)
        {
            return _commandEnvelopeProcessingAuditCollection
                .AsQueryable()
                .Where(n => n.DocumentId == documentId).ToList();
        }

        public CommandEnvelopeProcessingAudit GetById(Guid id)
        {
            return _commandEnvelopeProcessingAuditCollection
                .AsQueryable()
                .FirstOrDefault(n => n.Id == id);
        }

        public void SetStatus(Guid id, EnvelopeProcessingStatus status, int lastExcutedCommand)
        {
            CommandEnvelopeProcessingAudit command = GetById(id);
            if (command != null)
            {
                command.Status = status;
                command.LastExecutedCommand = lastExcutedCommand;
                _commandEnvelopeProcessingAuditCollection.Save(command);
            }
        }

        public bool IsConnected()
        {
            return base.TestConnection();
        }
    }
}