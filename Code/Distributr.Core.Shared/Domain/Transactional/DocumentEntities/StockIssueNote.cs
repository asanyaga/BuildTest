using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions;

namespace Distributr.Core.Domain.Transactional.DocumentEntities
{
    public class StockIssueNote : Document
    {
        public int DocumentIssuedTo { get; set; }
        public StockIssueNote(Guid id)
            : base(id)
        {
            _lineItems = new List<StockIssueNoteLineItem>();
        }

        public StockIssueNote(Guid id,
            string documentReference,
            CostCentre documentIssuerCostCentre,
            int documentIssueCostCentreApplicationId,
            User documentIssuerUser,
            DateTime documentDateIssued,
            CostCentre documentRecipientCostCentre,
            DocumentStatus status,
            List<StockIssueNoteLineItem> lineItems
            )
            : base(id, documentReference, documentIssuerCostCentre, documentIssueCostCentreApplicationId, documentIssuerUser, documentDateIssued,
            documentRecipientCostCentre, status)
        {
            _lineItems = lineItems;
            this.DocumentType = DocumentType.StockIssueNote;
        }

        List<StockIssueNoteLineItem> _lineItems;

        public void AddLineItem(StockIssueNoteLineItem lineItem)
        {
            if (Status != DocumentStatus.New)
                return;
            _lineItems.Add(lineItem);
        }

        public List<StockIssueNoteLineItem> LineItems
        {
            get { return _lineItems; }
        }

        public void _SetLineItems(List<StockIssueNoteLineItem> LineItems)
        {
            _lineItems = LineItems;
        }

        public override void Confirm()
        {
            if (Status != DocumentStatus.New)
                throw new InvalidDocumentOperationException("Cannot confirm an Stock Issue Note that is not new");
            Status = DocumentStatus.Confirmed;
        }

        public override void Approve()
        {
            if (Status != DocumentStatus.Confirmed)
                throw new InvalidDocumentOperationException("Cannot Approve an Stock Issue Note that is not Confirmed");
            Status = DocumentStatus.Approved;
        }

        public override void Reject()
        {
            if (Status != DocumentStatus.Confirmed)
                throw new InvalidDocumentOperationException("Stock Issue Note needs to be Confirmed");
            Status = DocumentStatus.Rejected;
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }
    }
}
