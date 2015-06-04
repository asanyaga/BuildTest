using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands.Discounts
{
    public class CreateDiscountCommand : CreateCommand
    {
        public CreateDiscountCommand()
        {
            
        }
        public CreateDiscountCommand(Guid commandId, Guid documentId, 
            Guid commandGeneratedByUserId, Guid commandGeneratedByCostCentreId, 
            int costCentreApplicationCommandSequenceId, 
            Guid commandGeneratedByCostCentreApplicationId, 
            double? longitude, double? latitude, 
            string documentReference, DateTime dateDiscountCreated, 
            Guid documentIssuerCostCentreId, 
            Guid documentRecipientCostCentreId, 
            Guid documentIssuerUserId,
            Guid orderId, Guid parentDocId) 
            : base(commandId, documentId, commandGeneratedByUserId, 
            commandGeneratedByCostCentreId, costCentreApplicationCommandSequenceId, 
            commandGeneratedByCostCentreApplicationId, parentDocId,
            dateDiscountCreated,
            documentIssuerCostCentreId, documentIssuerUserId,
            documentReference,
            longitude, latitude)
        {
            DocumentIssuerCostCentreId = documentIssuerCostCentreId;
            DocumentRecipientCostCentreId = documentRecipientCostCentreId;
            OrderId = orderId;
        }

        public Guid DocumentRecipientCostCentreId { get; set; }
        public Guid OrderId { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.CreateDiscount.ToString(); }
        }     
    }
}
