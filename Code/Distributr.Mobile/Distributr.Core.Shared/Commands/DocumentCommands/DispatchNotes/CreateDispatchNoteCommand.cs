using System;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Commands.DocumentCommands.DispatchNotes
{
    public class CreateDispatchNoteCommand : CreateCommand
    {
        public CreateDispatchNoteCommand()
        {
            
        }

        public CreateDispatchNoteCommand(
           Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,
            //-- 
            Guid dispatchNoteIssuerCostCentreId,
            Guid dispatchNoteIssuerUserId,
            Guid dispatchNoteRecipientCostCentreId,
            int dispatctNoteType, Guid parentDocId,           
            Guid orderId  ,
            DateTime documentDateIssued,
            string documentReference,
            double? longitude=null, double? latitude=null
            )
            : base(commandId, documentId, commandGeneratedByUserId,
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId,
            commandGeneratedByCostCentreApplicationId, parentDocId,
            documentDateIssued,
            dispatchNoteIssuerCostCentreId,
            dispatchNoteIssuerUserId,
            documentReference,
            longitude, latitude)
        {
            DispatchNoteRecipientCostCentreId = dispatchNoteRecipientCostCentreId;
            DispatchNoteType = dispatctNoteType;
            OrderId = orderId;
        }


     
        public Guid DispatchNoteRecipientCostCentreId { get; set; }
      
        public int DispatchNoteType { get;  set; }

        public Guid OrderId { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.CreateDispatchNote.ToString(); }
        }
    }
}
