using Distributr.Core.Commands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityTransferCommands;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.WSAPI.Lib.Services.Bus;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Newtonsoft.Json;

namespace Distributr.HQ.Lib.Workflows.CommodityTransfer
{
    public class CommodityTransferHqWorkFlow:ICommodityTransferWFManager
    {
        private IBusPublisher _busPublisher;
        private ICommandProcessingAuditRepository _commandProcessingAuditRepository;

        public CommodityTransferHqWorkFlow(IBusPublisher busPublisher, ICommandProcessingAuditRepository commandProcessingAuditRepository)
        {
            _busPublisher = busPublisher;
            _commandProcessingAuditRepository = commandProcessingAuditRepository;
        }

        public void SubmitChanges(Core.Domain.Transactional.SourcingDocumentEnitities.CommodityTransferNote document)
        {
            var commandsToExecute = document.GetDocumentCommandsToExecute();
            var approveCommodityTransferCommand =
                commandsToExecute.OfType<ApproveCommodityTransferCommand>().FirstOrDefault();
            if (approveCommodityTransferCommand == null) return;
                AddToMongoDB(approveCommodityTransferCommand);
                _busPublisher.WrapAndPublish(approveCommodityTransferCommand, (CommandType)
                          Enum.Parse(typeof(CommandType), approveCommodityTransferCommand.CommandTypeRef));
        }

        private void AddToMongoDB(ICommand createCommand)
        {
            var commandProcessingAudit = new CommandProcessingAudit
            {
                CommandType = createCommand.CommandTypeRef,
                CostCentreApplicationId =
                    createCommand.CommandGeneratedByCostCentreApplicationId,
                CostCentreCommandSequence = createCommand.CommandSequence,
                DateInserted = DateTime.Now,
                Id = createCommand.CommandId,
                JsonCommand = JsonConvert.SerializeObject(createCommand),
                RetryCounter = 0,
                Status = CommandProcessingStatus.OnQueue,
                SendDateTime = DateTime.Now.ToShortDateString(),
                DocumentId = createCommand.DocumentId,
                ParentDocumentId = createCommand.PDCommandId
            };
            _commandProcessingAuditRepository.AddCommand(commandProcessingAudit);
        }
    }
}
