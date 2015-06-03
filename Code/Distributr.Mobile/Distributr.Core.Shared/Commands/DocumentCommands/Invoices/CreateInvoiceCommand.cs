using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands.Invoices
{
    public class CreateInvoiceCommand : CreateCommand
    {
        public CreateInvoiceCommand()
        {
            
        }
        public CreateInvoiceCommand( 
            //Command
            Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,

            //invoice
             string documentReference,//
            DateTime dateInvoiceCreated,//
            Guid documentIssuerCostCentreId,
            Guid documentRecipientCostCentreId,
            Guid documentIssuerUserId,
            Guid orderId,
            decimal saleDiscount = 0
            )
             : base(commandId, documentId, commandGeneratedByUserId,
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId,
            commandGeneratedByCostCentreApplicationId,orderId,
            dateInvoiceCreated, 
            documentIssuerCostCentreId,
            documentIssuerUserId,
            documentReference
            )
        {
            DocumentReference = documentReference;
            DocumentIssuerCostCentreId = documentIssuerCostCentreId;
            DocumentRecipientCostCentreId = documentRecipientCostCentreId;
            OrderId = orderId;
            SaleDiscount = saleDiscount;
        }

        public Guid DocumentRecipientCostCentreId { get; set; }
        public Guid OrderId { get; set; }
        public decimal SaleDiscount { get; set; }//cn: use this to put sale discount on the invoice
        public override string CommandTypeRef
        {
            get { return CommandType.CreateInvoice.ToString(); }
        }
    }
}
