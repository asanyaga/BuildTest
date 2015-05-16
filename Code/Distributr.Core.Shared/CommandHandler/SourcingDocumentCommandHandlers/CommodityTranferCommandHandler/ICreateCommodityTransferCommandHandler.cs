using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityTransferCommands;

namespace Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityTranferCommandHandler
{
    public interface ICreateCommodityTransferCommandHandler : ICommandHandler<CreateCommodityTransferCommand>
    {
    }
}
