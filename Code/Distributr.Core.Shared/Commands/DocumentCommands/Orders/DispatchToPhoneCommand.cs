using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands.Orders
{
    public class DispatchToPhoneCommand : DocumentCommand
    {
        public DispatchToPhoneCommand()
        {
            
        }
        public DispatchToPhoneCommand(Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId)
            : base(commandId, documentId, commandGeneratedByUserId,
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId,
            commandGeneratedByCostCentreApplicationId,documentId
            )
        {

        }

        public override string CommandTypeRef
        {
            get { return CommandType.DispatchToPhone.ToString(); }
        }
    }
    public class OrderDispatchApprovedLineItemsCommand : DispatchCommand
    {
        public OrderDispatchApprovedLineItemsCommand()
        {

        }
        public OrderDispatchApprovedLineItemsCommand(Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId)
            : base(commandId, documentId, commandGeneratedByUserId,
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId,
            commandGeneratedByCostCentreApplicationId, documentId
            )
        {

        }

        public override string CommandTypeRef
        {
            get { return CommandType.OrderDispatchApprovedLineItems.ToString(); }
        }
    }
}
