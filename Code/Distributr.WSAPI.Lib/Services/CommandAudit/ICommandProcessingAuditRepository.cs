using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands.CommandPackage;

namespace Distributr.WSAPI.Lib.Services.CommandAudit
{
    public interface ICommandEnvelopeProcessingAuditRepository
    {
        void AddCommand(CommandEnvelopeProcessingAudit processingAudit);
      
        List<CommandEnvelopeProcessingAudit> GetByDocumentId(Guid documentId);
        CommandEnvelopeProcessingAudit GetById(Guid id);
        void SetStatus(Guid id, EnvelopeProcessingStatus status,int lastExcecutedCommand);

        bool IsConnected();
    }

    public interface ICommandProcessingAuditRepository
    {
        CommandProcessingAudit GetByCommandId(Guid commandId);
        void AddCommand(CommandProcessingAudit commandProcessingAudit);
        void SetCommandStatus(Guid commandId, CommandProcessingStatus status);
        List<CommandProcessingAudit> GetAll();
        List<CommandProcessingAudit> GetAllByStatus(CommandProcessingStatus status);
        List<CommandProcessingAudit> GetByCCAppId(Guid costCentreApplicationId, int dayOfYear, int year);
        List<CommandProcessingAudit> GetByApplicationId(Guid costCentreApplicationId, int index, int size, CommandProcessingStatus status, out int count);
        bool IsCreateCommandExecuted(Guid documentId);
        bool IsAddCommandExecuted(Guid documentId);
        bool IsCommandExecuted(Guid commandId);
        bool IsConfirmExecuted(Guid documentId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDate">Defaults to 30 Days</param>
        /// <returns></returns>
        IEnumerable<CommandProcessingAuditSummaryDTO> GetCommandProcessedSummary(DateTime? fromDate);

        decimal GetUnQueuedCommands();
        void QueueCommands();
        void Test();
        bool TestMyConnection();
       
    }

    public class CommandProcessingAuditSummaryDTO
    {
        public Guid CostCentreApplicationId { get; set; }
        public int Count { get; set; }
        public int OnQueue { get; set; }
        public int SubscriberProcessBegin { get; set; }
        public int MarkedForRetry { get; set; }
        public int Complete { get; set; }
        public int Failed { get; set; }
    }
}
