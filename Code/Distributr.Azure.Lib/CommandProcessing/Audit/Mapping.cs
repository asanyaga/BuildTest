using Distributr.Core.Commands.DocumentCommands;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Newtonsoft.Json;
using TestAzureTables.Impl;

namespace Distributr.Azure.Lib.CommandProcessing
{
    public static class Mapping
    {
        public static CommandProcessingAuditTable Map(this CommandProcessingAudit commandProcessingAudit)
        {
            DocumentCommand dc = JsonConvert.DeserializeObject<DocumentCommand>(commandProcessingAudit.JsonCommand);
            return new CommandProcessingAuditTable
                {
                   Id = commandProcessingAudit.Id,
                   CostCentreCommandSequence = commandProcessingAudit.CostCentreCommandSequence,
                   CostCentreApplicationId = commandProcessingAudit.CostCentreApplicationId,
                   DocumentId =  commandProcessingAudit.DocumentId,
                   ParentDocumentId = commandProcessingAudit.ParentDocumentId,
                   JsonCommand = commandProcessingAudit.JsonCommand,
                   CommandType = commandProcessingAudit.CommandType,
                   Status = (int) commandProcessingAudit.Status,
                   RetryCounter = commandProcessingAudit.RetryCounter,
                   DateInserted = commandProcessingAudit.DateInserted,
                   SendDateTime = commandProcessingAudit.SendDateTime,
                   PartitionKey = dc.CommandGeneratedByCostCentreId.ToString(),
                   RowKey = commandProcessingAudit.Id.ToString()
                };
        }

        public static CommandProcessingAudit Map(this CommandProcessingAuditTable commandProcessingAuditTable)
        {
            return new CommandProcessingAudit
                {
                    Id = commandProcessingAuditTable.Id,
                    CostCentreCommandSequence = commandProcessingAuditTable.CostCentreCommandSequence,
                    CostCentreApplicationId = commandProcessingAuditTable.CostCentreApplicationId,
                    DocumentId = commandProcessingAuditTable.DocumentId,
                    ParentDocumentId = commandProcessingAuditTable.ParentDocumentId,
                    JsonCommand = commandProcessingAuditTable.JsonCommand,
                    CommandType = commandProcessingAuditTable.CommandType,
                    Status = (CommandProcessingStatus) commandProcessingAuditTable.Status,
                    RetryCounter = commandProcessingAuditTable.RetryCounter,
                    DateInserted = commandProcessingAuditTable.DateInserted,
                    SendDateTime = commandProcessingAuditTable.SendDateTime,
                };
        }


    }
}
