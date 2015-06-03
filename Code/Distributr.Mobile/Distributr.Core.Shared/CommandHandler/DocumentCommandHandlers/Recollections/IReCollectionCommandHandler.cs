using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands;
using Distributr.Core.Commands.DocumentCommands.Recollections;

namespace Distributr.Core.CommandHandler.DocumentCommandHandlers.Recollections
{
    public interface IReCollectionCommandHandler : ICommandHandler<ReCollectionCommand>
    {
    }
}
