using Distributr.Core.Commands;
using Distributr.Core.Commands.DocumentCommands.OutletDocument;
using Distributr.Core.Commands.DocumentCommands.Receipts;

namespace Distributr.Core.CommandHandler.DocumentCommandHandlers.OutletDocument
{
    public interface ICreateOutletVisitNoteCommandHandler : ICommandHandler<CreateOutletVisitNoteCommand>
    {
    }
}
