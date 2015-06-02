using System;

namespace Distributr.Core.Commands.DocumentCommands.InventoryTransferNotes
{
    public class CreateInventoryTransferNoteCommand : CreateCommand
    {
        public CreateInventoryTransferNoteCommand()
        {
            
        }

        public CreateInventoryTransferNoteCommand(            
           Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,
            //-- 
            Guid inventoryTransferNoteIssuerCostCentreId,
            Guid inventoryTransferNoteIssuerUserId,
            Guid inventoryTransferNoteIssuedOnBehalfOfUserId,
            Guid issuedOnBehalfOfCostCentreId,
            Guid documentRecipientCostCentreId,
            DateTime documentDateIssued,
            string documentReference
            )
            : base(commandId, documentId, commandGeneratedByUserId,
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId,
            commandGeneratedByCostCentreApplicationId, documentId,
            documentDateIssued, inventoryTransferNoteIssuerCostCentreId,
            inventoryTransferNoteIssuerUserId, documentReference
            )
        {
            InventoryTransferNoteIssuedOnBehalfOfUserId = inventoryTransferNoteIssuedOnBehalfOfUserId;
            IssuedOnBehalfOfCostCentreId = issuedOnBehalfOfCostCentreId;
            DocumentRecipientCostCentreId = documentRecipientCostCentreId;
        }

        public Guid InventoryTransferNoteIssuedOnBehalfOfUserId { get; set; }
        public Guid IssuedOnBehalfOfCostCentreId { get; set; }
        public Guid DocumentRecipientCostCentreId { get; set; }

        public override string CommandTypeRef
        {
            get { return CommandType.CreateInventoryTransferNote.ToString(); }
        }
    }
}