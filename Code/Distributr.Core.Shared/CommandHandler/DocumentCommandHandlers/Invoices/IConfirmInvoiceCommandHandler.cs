using Distributr.Core.Commands;
using Distributr.Core.Commands.DocumentCommands.Invoices;

namespace Distributr.Core.CommandHandler.DocumentCommandHandlers.Invoices
{
    public interface IConfirmInvoiceCommandHandler : ICommandHandler<ConfirmInvoiceCommand>
    {
    }
}
