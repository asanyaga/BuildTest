using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Commands.CommandPackage;
using Distributr.MongoDB.Repository;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace Distributr.MongoDB.CommandRouting
{
    //public class CommandEnvelopeProcessingAuditRepository : MongoBase, ICommandEnvelopeProcessingAuditRepository
    //{
    //    private string _commandEnvelopeProcessingAuditCollectionName = "CommandEnvelopeProcessingAudit";
    //    private MongoCollection<CommandEnvelopeProcessingAudit> _commandEnvelopeProcessingAuditCollection;
    //    public CommandEnvelopeProcessingAuditRepository(string connectionString) : base(connectionString)
    //    {
    //        _commandEnvelopeProcessingAuditCollection = CurrentMongoDB.GetCollection<CommandEnvelopeProcessingAudit>(_commandEnvelopeProcessingAuditCollectionName);
    //        _commandEnvelopeProcessingAuditCollection.EnsureIndex(IndexKeys<CommandEnvelopeProcessingAudit>.Ascending(n => n.Id), IndexOptions.SetUnique(true));
    //        _commandEnvelopeProcessingAuditCollection.EnsureIndex(IndexKeys<CommandEnvelopeProcessingAudit>.Ascending(n => n.GeneratedByCostCentreApplicationId));
    //        _commandEnvelopeProcessingAuditCollection.EnsureIndex(IndexKeys<CommandEnvelopeProcessingAudit>.Ascending(n => n.RecipientCostCentreId));
    //        _commandEnvelopeProcessingAuditCollection.EnsureIndex(IndexKeys<CommandEnvelopeProcessingAudit>.Ascending(n => n.DocumentId));
    //        _commandEnvelopeProcessingAuditCollection.EnsureIndex(IndexKeys<CommandEnvelopeProcessingAudit>.Ascending(n => n.Status));
    //        _commandEnvelopeProcessingAuditCollection.EnsureIndex(IndexKeys<CommandProcessingAudit>.Ascending(n => n.ParentDocumentId));
    //        _commandEnvelopeProcessingAuditCollection.EnsureIndex(IndexKeys<CommandProcessingAudit>.Ascending(n => n.DateInserted));

    //    }

    //    public void AddCommand(CommandEnvelopeProcessingAudit processingAudit)
    //    {
    //        _commandEnvelopeProcessingAuditCollection.Save(processingAudit);
    //    }

    //    public List<CommandEnvelopeProcessingAudit> GetByDocumentId(Guid documentId)
    //    {
    //        return _commandEnvelopeProcessingAuditCollection
    //            .AsQueryable()
    //            .Where(n => n.DocumentId == documentId).ToList();
    //    }

    //    public CommandEnvelopeProcessingAudit GetById(Guid id)
    //    {
    //        return _commandEnvelopeProcessingAuditCollection
    //            .AsQueryable()
    //            .FirstOrDefault(n => n.Id == id);
    //    }

    //    public void SetStatus(Guid id, EnvelopeProcessingStatus status,int lastExcutedCommand)
    //    {
    //        CommandEnvelopeProcessingAudit command = GetById(id);
    //        if (command != null)
    //        {
    //            command.Status = status;
    //            command.LastExecutedCommand = lastExcutedCommand;
    //            _commandEnvelopeProcessingAuditCollection.Save(command);
    //        }
    //    }

    //    public bool IsConnected()
    //    {
    //        return base.TestConnection();
    //    }
    //}
}