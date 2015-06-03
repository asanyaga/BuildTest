using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityPurchaseCommands;

namespace Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityPurchaseCommandHandler
{
    public interface IConfirmCommodityPurchaseCommandHandler : ICommandHandler<ConfirmCommodityPurchaseCommand>
    {
    }
}
