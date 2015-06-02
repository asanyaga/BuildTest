using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands.ReturnsNotes
{
    public class AddReturnsNoteLineItemCommand : AfterCreateCommand
    {
        public AddReturnsNoteLineItemCommand()
        {
            
        }
        public AddReturnsNoteLineItemCommand(    
            //Command
            Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,
            //Returns Note line item
            string description,
            int lineItemSequenceno,
            Guid lineItemid,
            decimal expected,
            decimal value,
            decimal actual,
            int returnTypeId,
            Guid productId,
            int lossType,
            string reason,
            string other = ""
            ) : base(commandId, documentId, commandGeneratedByUserId, commandGeneratedByCostCentreId, costCentreApplicationCommandSequenceId,
            commandGeneratedByCostCentreApplicationId, documentId)
        {
            Description = description;
            LineItemSequenceNo = lineItemSequenceno;
            LineItemId = lineItemid;
            Expected = expected;
            Actual = actual;
            Value = value;
            ReturnTypeId = returnTypeId;
            ProductId = productId;
            LossType = lossType;
            Reason = reason;
            Other = other;
        }

        public int LineItemSequenceNo { get; set; }
        public Guid LineItemId { get; set; }
        public decimal Expected { get; set; }
        public decimal Actual { get; set; }
        public decimal Value { get; set; }
        public int ReturnTypeId { get; set; }
        public Guid ProductId { get; set; }
        public int LossType { get; set; }
        public string Reason { get; set; }
        public string Other { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.AddReturnsNoteLineItem.ToString(); }
        }
    }
}
