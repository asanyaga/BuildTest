using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands.CreditNotes
{
    public class AddCreditNoteLineItemCommand : AfterCreateCommand
    {
        public AddCreditNoteLineItemCommand()
        {
            
        }
        public AddCreditNoteLineItemCommand(
            //Command
            Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,
            //creditnote line item
            string description,
            int lineItemSequenceno,
            Guid lineItemid,
            decimal value,
            decimal lineItemVatValue,
            Guid productId,
            decimal qty, Guid parentDocId
            //int? lineItemType = 0//cn
            )
            : base(commandId, documentId, commandGeneratedByUserId, commandGeneratedByCostCentreId, costCentreApplicationCommandSequenceId,
                commandGeneratedByCostCentreApplicationId, parentDocId)
        {
            Description = description;
            LineItemSequenceNo = lineItemSequenceno;
            LineItemId = lineItemid;
            Value = value;
            //InvoiceId = invoiceId;//cn
            ProductId = productId;
            Qty = qty;
            LineItemVatValue = lineItemVatValue;
            //if (lineItemType != null) LineItemType = (int)lineItemType;//cn
        }

        public int LineItemSequenceNo { get; set; }
        public Guid LineItemId { get; set; }
        public decimal Value { get; set; }
        //public Guid InvoiceId { get; set; }
        public Guid ProductId { get; set; }
        public decimal Qty { get; set; }
        public decimal LineItemVatValue { get; set; }
        //public int LineItemType { get; set; } //cn
        public override string CommandTypeRef
        {
            get { return CommandType.AddCreditNoteLineItem.ToString(); }
        }

    }
}
