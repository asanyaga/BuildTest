using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;


namespace Distributr.Core.Domain.Transactional.DocumentEntities
{
    public class DisbursementNote : Document
    {
        public DisbursementNote(Guid id) : base(id)
        {
            _lineItems = new List<DisbursementNoteLineItem>();
        }
        public DisbursementNote(Guid id,
             string documentReference,
             CostCentre documentIssuerCostCentre,
             Guid documentIssuerCostCentreApplicationId,
             User documentIssuerUser,
             DateTime documentDateIssued,
            CostCentre documentRecipientCostCentre,
             DocumentStatus status,
             List<DisbursementNoteLineItem> lineItems
             )
            : base(id, documentReference, documentIssuerCostCentre,
                documentIssuerCostCentreApplicationId, documentIssuerUser, documentDateIssued,
                documentRecipientCostCentre, status)
        {
            _lineItems = new List<DisbursementNoteLineItem>();
            _lineItems = lineItems;
        }
        private List<DisbursementNoteLineItem> _lineItems;

        public void AddLineItem(DisbursementNoteLineItem dispatchNoteLineItem)
        {
            if (Status != DocumentStatus.New)
                throw new InvalidDocumentOperationException("Cannot add line items to a Disbursement note that is not new");
            dispatchNoteLineItem.IsNew = true;
            _lineItems.Add(dispatchNoteLineItem);
        }

        public List<DisbursementNoteLineItem> LineItems
        {
            get { return _lineItems; }

        }
     

        public void _SetLineItems(List<DisbursementNoteLineItem> items)
        {
            _lineItems = items;
        }
        public override void Confirm()
        {
            if (Status != DocumentStatus.New)
                throw new InvalidDocumentOperationException("Cannot confirm an Disbursement Note that is not new");
            if (LineItems.Count() == 0)
                throw new InvalidDocumentOperationException("Must add at least one lineitem to Disbursement Note before confirming");
            Status = DocumentStatus.Confirmed;
        }


        protected override void _AddCreateCommandToExecute(bool isHybrid = false)
        {
            throw new NotImplementedException();
        }

        protected override void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
        {
            throw new NotImplementedException();
        }


        protected override void _AddConfirmCommandToExecute(bool isHybrid = false)
        {
            throw new NotImplementedException();
        }
    }
}
