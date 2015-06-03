using System;

namespace Distributr.Core.Commands.DocumentCommands.InventoryReceivedNotes
{
    public class CreateInventoryReceivedNoteCommand : CreateCommand
    {
        public CreateInventoryReceivedNoteCommand()
        {
            
        }

        public CreateInventoryReceivedNoteCommand(
           Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,
            Guid inventoryReceivedNoteIssuerCostCentreId,
            Guid inventoryReceivedNoteIssuerUserId,
            Guid inventoryReceivedNoteIssuedOnBehalfOfUserId,
            Guid inventoryReceivedFromCostCentreId,
            string orderReferences,
            string loadNo,
            string documentReference,
            DateTime dateDocumentIssued
            )
            : base(commandId, documentId, commandGeneratedByUserId,
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId,
            commandGeneratedByCostCentreApplicationId, documentId,
            dateDocumentIssued, inventoryReceivedNoteIssuerCostCentreId,
            inventoryReceivedNoteIssuerUserId,documentReference
            )
        {
            InventoryReceivedNoteIssuedOnBehalfOfUserId = inventoryReceivedNoteIssuedOnBehalfOfUserId;
            InventoryReceivedFromCostCentreId = inventoryReceivedFromCostCentreId;
            OrderReferences = orderReferences;
            LoadNo = loadNo;
            DocumentReference = documentReference;
            DocumentDateIssued = dateDocumentIssued;
        }

        public Guid InventoryReceivedNoteIssuedOnBehalfOfUserId { get; set; }
        
        /// <summary>
        /// Can be zero if received from outside
        /// </summary>
        public Guid InventoryReceivedFromCostCentreId { get; set; }
        public string OrderReferences { get; set; }
        public string LoadNo { get; set; }

        public override string CommandTypeRef
        {
            get { return CommandType.CreateInventoryReceivedNote.ToString(); }
        }
    }
}