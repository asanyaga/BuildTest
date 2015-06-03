using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Distributr.Core.Commands.DocumentCommands.Receipts;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;

namespace Distributr.Core.Domain.Transactional.DocumentEntities
{
    public class Receipt : Document
    {
        public Receipt(Guid id)
            : base(id)
        {
            _lineItems = new List<ReceiptLineItem>();
        }

        public Receipt(Guid id,
            string documentReference,
            CostCentre documentIssuerCostCentre,
            Guid documentIssuerCostCentreApplicationId,
            User documentIssuerUser,
            DateTime documentDateIssued,
            CostCentre documentRecipientCostCentre,
            DocumentStatus status,
            //receipt
            List<ReceiptLineItem> lineItems,
            Guid invoiceId,
            Guid paymentDocId/* = new Guid()*/
            )
            : base(id, documentReference, documentIssuerCostCentre,
                documentIssuerCostCentreApplicationId, documentIssuerUser, documentDateIssued,
                documentRecipientCostCentre, status)
        {
            _lineItems = new List<ReceiptLineItem>();
            _lineItems = lineItems;
            InvoiceId = invoiceId;
            PaymentDocId = paymentDocId;
        }

        public Guid PaymentDocId { get; set; }

        public void Add(ReceiptLineItem receiptLineItem)
        {
            if (Status == DocumentStatus.New)
            {
                _lineItems.Add(receiptLineItem);

            }
        }
        public void AddLineItem(ReceiptLineItem lineItem)
        {
            _lineItems.Add(lineItem);
            _AddAddLineItemCommandToExecute(lineItem);
        }

        private List<ReceiptLineItem> _lineItems;
        public List<ReceiptLineItem> LineItems
        {
            get { return _lineItems/*.Where(n => n.LineItemType == OrderLineItemType.PostConfirmation).ToList()*/; }
        }

        public decimal Total
        {
            get
            {
                return LineItems.Where(n => n.LineItemType == OrderLineItemType.PostConfirmation).Sum(n => n.Value);
            }
        }

        public Guid InvoiceId { get; set; }

        public override void Confirm()
        {
            if (Status != DocumentStatus.New)
                throw new InvalidDocumentOperationException("Cannot confirm a receipt that is not new");
            Status = DocumentStatus.Confirmed;
            _AddCreateCommandToExecute();
            _AddConfirmCommandToExecute();
        }





        protected override void _AddCreateCommandToExecute(bool isHybrid = false)
        {
            var ic = new CreateReceiptCommand(
               Guid.NewGuid(),
               Id,
               DocumentIssuerUser.Id,
               DocumentIssuerCostCentre.Id,
               0,
               DocumentIssuerCostCentreApplicationId,
               DocumentReference,
               DocumentDateIssued,
               DocumentIssuerCostCentre.Id,
               DocumentRecipientCostCentre.Id,
               DocumentIssuerUser.Id,
               InvoiceId,
               DocumentParentId,
               PaymentDocId
               );
            ic.VersionNumber = "H-" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _AddCommand(ic);
        }

        protected override void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
        {
            var item = lineItem as ReceiptLineItem;
            var ilic = new AddReceiptLineItemCommand(
                    item.Id,
                    Id,
                    DocumentIssuerUser.Id,
                    DocumentIssuerCostCentre.Id,
                    0,
                    DocumentIssuerCostCentreApplicationId,
                    item.Description,
                    0,
                    item.Id,//lineitemid??
                    item.Value,
                    (int)item.PaymentType,
                    item.PaymentRefId,
                    (int)item.LineItemType,
                    item.MMoneyPaymentType,
                    DocumentParentId,
                    default(Guid),item.NotificationId
                    );
            ilic.Description = item.Description;
            _AddCommand(ilic);
        }


        protected override void _AddConfirmCommandToExecute(bool isHybrid = false)
        {
            var icc = new ConfirmReceiptCommand(
               Guid.NewGuid(),
               Id,
               DocumentIssuerUser.Id,
               DocumentIssuerCostCentre.Id,
               0,
               DocumentIssuerCostCentreApplicationId,
               DocumentParentId
               );
            _AddCommand(icc);
        }

        public void _SetLineItems(List<ReceiptLineItem> lineItems)
        {
            _lineItems = lineItems;
        }
    }
}
