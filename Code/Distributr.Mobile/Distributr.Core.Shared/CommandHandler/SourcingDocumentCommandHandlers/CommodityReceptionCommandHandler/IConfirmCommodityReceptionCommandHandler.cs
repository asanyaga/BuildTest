using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityReceptionCommands;

namespace Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityReceptionCommandHandler
{
    public interface IConfirmCommodityReceptionCommandHandler:ICommandHandler<ConfirmCommodityReceptionCommand>
    {
    }
}
