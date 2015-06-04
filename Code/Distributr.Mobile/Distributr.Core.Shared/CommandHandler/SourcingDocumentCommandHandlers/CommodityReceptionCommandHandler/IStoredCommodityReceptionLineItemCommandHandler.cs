using Distributr.Core.Commands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityReceptionCommands;

namespace Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityReceptionCommandHandler
{
    public interface IStoredCommodityReceptionLineItemCommandHandler : ICommandHandler<StoredCommodityReceptionLineItemCommand>
    {
    }
}
