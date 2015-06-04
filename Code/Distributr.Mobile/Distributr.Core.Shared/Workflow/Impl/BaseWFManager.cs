using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Workflow.Impl
{
    public abstract class BaseWFManager
    {
        protected bool TryGetCreateCommand(List<DocumentCommand> commands, out CreateCommand command)
        {
            command = null;
            var createCommand = commands.OfType<CreateCommand>().FirstOrDefault();
            if (createCommand == null)
                return false;
            command = createCommand;
            return true;
        }

        protected bool TryGetAfterCreateCommands(List<DocumentCommand> commands,
                                                 out List<AfterCreateCommand> afterCreateCommands)
        {
            afterCreateCommands = null;
            var lineItemCommands = commands.OfType<AfterCreateCommand>();
            if (lineItemCommands == null || lineItemCommands.Count() == 0)
                return false;
            afterCreateCommands = lineItemCommands.ToList();
            return true;
        }

        protected bool TryGetConfirmCommand(List<DocumentCommand> commands, out ConfirmCommand command)
        {
            command = null;
            var confirmcommand = commands.OfType<ConfirmCommand>().FirstOrDefault();
            if (confirmcommand == null)
                return false;
            command = confirmcommand;
            return true;
        }
    }
}
