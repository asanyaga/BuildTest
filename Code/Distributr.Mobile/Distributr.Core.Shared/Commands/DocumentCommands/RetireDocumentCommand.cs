using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands
{
    public class RetireDocumentCommand : DocumentCommand
    {
        public RetireDocumentCommand()
        {
            
        }
        public RetireDocumentCommand(Guid commandId, 
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,
            Guid parentDocId,
            Guid commandRecipientCostCentreId,
            double? longitude,
            double? latitude
            ) :
            base(commandId, 
            documentId, 
            commandGeneratedByUserId,
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId, 
            commandGeneratedByCostCentreApplicationId, 
            parentDocId, longitude, latitude)
        {
            CommandRecipientCostCentreId = commandRecipientCostCentreId;
        }
        public Guid CommandRecipientCostCentreId { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.RetireDocument.ToString(); }
        }
    }
}
