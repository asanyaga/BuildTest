using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.DocumentCommands.Invoices
{
    public class ConfirmInvoiceCommand : ConfirmCommand
    {
        public ConfirmInvoiceCommand()
        {
            
        }
        public ConfirmInvoiceCommand(Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId, Guid parentDocId)
            : base(commandId, documentId, commandGeneratedByUserId, 
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId,
            commandGeneratedByCostCentreApplicationId, parentDocId
            )
        {
            
        }

        public override string CommandTypeRef
        {
            get { return CommandType.ConfirmInvoice.ToString(); }
        }
    }
}
