using System;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.ReceivedDeliveryCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands;
using Distributr.Core.Data.EF;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.ReceivedDeliveryCommandHandlers
{
    public class AddReceivedDeliveryLineItemCommandHandler : BaseSourcingCommandHandler, IAddReceivedDeliveryLineItemCommandHandler
    {
        ILog _log = LogManager.GetLogger("AddReceivedDeliveryLineItemCommandHandler");
        public AddReceivedDeliveryLineItemCommandHandler(CokeDataContext context) : base(context)
        {
        }

        public void Execute(AddReceivedDeliveryLineItemCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                {
                    _log.InfoFormat("Cannot add line item. Document does not exist  Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
                    return;
                }

                if (DocumentLineItemExists(command.CommandId))
                {
                    _log.InfoFormat("Cannot add line item {0}. Line item already exists", command.CommandId);
                    return;
                }
                tblSourcingDocument doc = ExistingDocument(command.DocumentId);
                tblSourcingLineItem lineItem = NewLineItem(
                    command.CommandId, command.PDCommandId, command.DocumentId, Guid.Empty, command.CommodityGradeId,
                    Guid.Empty, command.DeliveredWeight, command.Description, command.ContainerNo);
                lineItem.Weight = command.Weight;
                lineItem.ContainerId = command.ContainerTypeId;
                lineItem.CommodityId = command.CommodityId;
                doc.tblSourcingLineItem.Add(lineItem);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("AddReceivedDeliveryLineItemCommandHandler exception", ex);
                throw;
            }
        }
    }
}
