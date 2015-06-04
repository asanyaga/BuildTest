using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Distributr.Core.Commands.DocumentCommands.CreditNotes;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions;

namespace Distributr.Core.Domain.Transactional.DocumentEntities
{
    public class CreditNote : Document
    {
        private List<CreditNoteLineItem> _lineItems;
        public List<CreditNoteLineItem> LineItems { get { return _lineItems; } }
        public CreditNote(Guid id)
            : base(id)
        {
            _lineItems = new List<CreditNoteLineItem>();
        }

        public CreditNote(Guid id,
            string documentReference,
            CostCentre documentIssuerCostCentre,
            Guid documentIssuerCostCentreApplicationId,
            User documentIssuerUser,
            DateTime documentDateIssued,           
            CostCentre documentRecipientCostCentre,
            DocumentStatus status,
            List<CreditNoteLineItem> lineItems,
            Guid invoiceId,
            CreditNoteType creditNoteType
            ):base(id, documentReference, documentIssuerCostCentre, 
            documentIssuerCostCentreApplicationId, documentIssuerUser, documentDateIssued,
            documentRecipientCostCentre, status)
        {
            _lineItems = new List<CreditNoteLineItem>();
            _lineItems = lineItems;
            InvoiceId = invoiceId;
            CreditNoteType = creditNoteType;
        }

        public Guid InvoiceId { get; set; }
        public CreditNoteType CreditNoteType { get; set; }

        public void AddLineItem(CreditNoteLineItem creditNoteLineItem)
        {
            if (Status == DocumentStatus.New)
            {
                _lineItems.Add(creditNoteLineItem);
                _AddAddLineItemCommandToExecute(creditNoteLineItem);
            }
        }

        public decimal Total
        {
            get { return LineItems.Sum(n => (n.Value*n.Qty)); }
        }

        public override void Confirm()
        {
            if (Status != DocumentStatus.New)
                throw new InvalidDocumentOperationException("Cannot confirm a credit note that is not new");
            Status = DocumentStatus.Confirmed;
            _AddCreateCommandToExecute();
            _AddConfirmCommandToExecute();

        }



        protected override void _AddCreateCommandToExecute(bool isHybrid = false)
        {
            var coc = new CreateCreditNoteCommand(
               Guid.NewGuid(),
               Id,
              DocumentIssuerUser.Id,
               DocumentIssuerCostCentre.Id,
               0,
       DocumentIssuerCostCentreApplicationId,
               DocumentReference, DocumentDateIssued,
             DocumentRecipientCostCentre.Id,
             InvoiceId,
               (int)CreditNoteType, DocumentParentId
               );
            coc.VersionNumber = "H-" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _AddCommand(coc);
        }

        protected override void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
        {
            var item = lineItem as CreditNoteLineItem;
            var acnli = new AddCreditNoteLineItemCommand(Guid.NewGuid(),
                    Id,
                   DocumentIssuerUser.Id,
                   DocumentIssuerCostCentre.Id,
                   0,
                   DocumentIssuerCostCentreApplicationId,
                   item.Description,
                   item.LineItemSequenceNo,
                   item.Id,
                   item.Value,
                   item.LineItemVatValue,
                   item.Product.Id,
                   item.Qty,
                  DocumentParentId
                   );
            _AddCommand(acnli);
        }


        protected override void _AddConfirmCommandToExecute(bool isHybrid = false)
        {
            var co = new ConfirmCreditNoteCommand(
                Guid.NewGuid(),
                Id,
                DocumentIssuerUser.Id,
                DocumentIssuerCostCentre.Id,
                0,
                DocumentIssuerCostCentreApplicationId,DocumentParentId
                );
            _AddCommand(co);
        }

        public void _SetLineItems(List<CreditNoteLineItem> lineItems)
        {
            _lineItems = lineItems;
        }
    }
}
