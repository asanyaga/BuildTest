using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands.Receipts
{
    public class AddReceiptLineItemCommand : AfterCreateCommand
    {
        public AddReceiptLineItemCommand()
        {
            
        }
        public AddReceiptLineItemCommand(    
            //Command
            Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,

            //receipt line item
            string description,
            int lineItemSequenceno,
            Guid lineItemid,
            decimal value,
           // int accountTypeId,
            int paymentTypeId,
            string paymentTypeReference,
            int lineItemType,
            string mMoneyPaymentType,
            Guid parentDocId,
            Guid paymentDocLineItemId = new Guid(),
            string notificationId = ""
            ) : base(commandId, documentId, commandGeneratedByUserId, commandGeneratedByCostCentreId, costCentreApplicationCommandSequenceId,
            commandGeneratedByCostCentreApplicationId, parentDocId)
        {
            Description = description;
            LineItemSequenceNo = lineItemSequenceno;
            LineItemId = lineItemid;
            Value = value;
            //AccountTypeId = accountTypeId;
            PaymentTypeId = paymentTypeId;
            PaymentTypeReference = paymentTypeReference;
            LineItemType = lineItemType;
            MMoneyPaymentType = mMoneyPaymentType;
            PaymentDocLineItemId = paymentDocLineItemId;
            NotificationId = notificationId;
        }

        public int LineItemSequenceNo { get; set; }
        public Guid LineItemId { get; set; }
        public decimal Value { get; set; }
        //public int AccountTypeId { get; set; }
        public Guid PaymentDocLineItemId { get; set; }
        public int PaymentTypeId { get; set; }
        public string PaymentTypeReference { get; set; }
        public int LineItemType { get; set; } //cn confirmed or unconfirmed receipt payment
        public string MMoneyPaymentType { get; set; }//
        public string NotificationId { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.AddReceiptLineItem.ToString(); }
        }
    }
}
