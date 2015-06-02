using Distributr.Core.Commands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityReleaseCommands;

namespace Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityReleaseCommandHandler
{
    public interface IAddCommodityReleaseLineItemCommandHandler : ICommandHandler<AddCommodityReleaseNoteLineItemCommand>
    {
    }
}
