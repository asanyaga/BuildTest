using System;

namespace Distributr.Core.Commands.DocumentCommands.AdjustmentNotes
{
    public class CreateInventoryAdjustmentNoteCommand : CreateCommand
    {
        public CreateInventoryAdjustmentNoteCommand()
        {
            
        }
        public CreateInventoryAdjustmentNoteCommand(
            Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,
            Guid documentIssuerCostCentreId, 
            Guid docIssuerUserId, 
            int inventoryAdjustmentNoteTypeId,
            string documentReference,
            DateTime documentDateIssued
            )
            : base(commandId, documentId, commandGeneratedByUserId,
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId,
            commandGeneratedByCostCentreApplicationId,documentId,
            documentDateIssued,documentIssuerCostCentreId, docIssuerUserId,documentReference)
        {
            InventoryAdjustmentNoteTypeId = inventoryAdjustmentNoteTypeId;
        }
        
        public int InventoryAdjustmentNoteTypeId { get; set; }
        public Guid VisitId { get; set; }
        public Guid DocumentRecipientCostCentreId { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.CreateInventoryAdjustmentNote.ToString(); }
        }
    }
}
