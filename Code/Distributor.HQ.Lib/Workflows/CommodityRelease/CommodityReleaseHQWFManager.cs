using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityReleaseCommands;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Workflow;
using Distributr.WSAPI.Lib.Services.Bus;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Newtonsoft.Json;

namespace Distributr.HQ.Lib.Workflows.CommodityRelease
{
    public class CommodityReleaseHQWFManager : ICommodityReleaseWFManager
    {
        private IBusPublisher _busPublisher;
        private ICommandProcessingAuditRepository _commandProcessingAuditRepository;

        public CommodityReleaseHQWFManager(IBusPublisher busPublisher, ICommandProcessingAuditRepository commandProcessingAuditRepository)
        {
            _busPublisher = busPublisher;
            _commandProcessingAuditRepository = commandProcessingAuditRepository;
        }

        public void SubmitChanges(CommodityReleaseNote document)
        {
            var commandsToExecute = document.GetDocumentCommandsToExecute();

            var createCommand = commandsToExecute.OfType<CreateCommand>().FirstOrDefault();

            if (createCommand != null)
            {
                AddToMongoDB(createCommand);
                _busPublisher.WrapAndPublish(createCommand,
                                             (CommandType)
                                             Enum.Parse(typeof(CommandType), createCommand.CommandTypeRef));

            }

            var lineItemCommands = commandsToExecute.OfType<AfterCreateCommand>();
            foreach (var _item in lineItemCommands)
            {
                AddToMongoDB(_item);
                _busPublisher.WrapAndPublish(_item, (CommandType)Enum.Parse(typeof(CommandType), _item.CommandTypeRef));


            }

            var confirmCommodityReleaseCommand = commandsToExecute.OfType<ConfirmCommodityReleaseNoteCommand>().FirstOrDefault();
            if (confirmCommodityReleaseCommand != null)
            {
                AddToMongoDB(confirmCommodityReleaseCommand);
                _busPublisher.WrapAndPublish(confirmCommodityReleaseCommand, (CommandType)Enum.Parse(typeof(CommandType), confirmCommodityReleaseCommand.CommandTypeRef));
            }
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
