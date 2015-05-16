using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Distributr.Core.Commands.DocumentCommands.DispatchNotes;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;

namespace Distributr.Core.Domain.Transactional.DocumentEntities
{
    public class DispatchNote : Document
    {//
        public DispatchNote(Guid id) : base(id)
        {
            _lineItem = new List<DispatchNoteLineItem>();
        }

        public DispatchNote(Guid id, string documentReference,
            CostCentre documentIssuerCostCentre,
            Guid documentIssueCostCentreApplicationId,
            User documentIssuerUser,
            DateTime documentDateIssued,
           CostCentre documentRecipientCostCentre,
            DocumentStatus status, DispatchNoteType dispatchType,
            Guid orderId,
            List<DispatchNoteLineItem> lineItems
            )
            : base(
            id, 
            documentReference, 
            documentIssuerCostCentre,
            documentIssueCostCentreApplicationId,
            documentIssuerUser, documentDateIssued,
            documentRecipientCostCentre,
            status)
        {
            _lineItem = lineItems;
            this.DocumentType = DocumentType.DispatchNote;
            DispatchType = dispatchType;
            OrderId = orderId;
        }

        public void AddLineItem(DispatchNoteLineItem dispatchNoteLineItem)
        {
            if (Status != DocumentStatus.New)
                throw new InvalidDocumentOperationException("Cannot add line items to a dispatch note that is not new");
            dispatchNoteLineItem.IsNew = true;
            _lineItem.Add(dispatchNoteLineItem);
            _AddAddLineItemCommandToExecute(dispatchNoteLineItem);
        }
      
        public List<DispatchNoteLineItem> LineItems
        {
            get { return _lineItem; }

        }

        public Guid OrderId { get; set; }

        private List<DispatchNoteLineItem> _lineItem { get;  set; }
        public DispatchNoteType DispatchType { set; get; }

        public void _SetLineItems(List<DispatchNoteLineItem> items)
        {
            _lineItem = items;
        }
        public decimal TotalNet { get { return LineItems.Sum(n => n.LineItemTotal); } }

        public override void Confirm()
        {
            if (Status != DocumentStatus.New)
                throw new InvalidDocumentOperationException("Cannot confirm a dispatch note that is not new");
            Status = DocumentStatus.Confirmed;
            _AddCreateCommandToExecute();
            _AddConfirmCommandToExecute();
        }

        protected override void _AddCreateCommandToExecute(bool isHybrid = false)
        {
            var coc = new CreateDispatchNoteCommand(
                Guid.NewGuid(),
                Id,
                DocumentIssuerUser.Id,
                DocumentIssuerCostCentre.Id,
                0,
                DocumentIssuerCostCentreApplicationId,
                DocumentIssuerCostCentre.Id,
                DocumentIssuerUser.Id,
                DocumentRecipientCostCentre.Id,
                (int) DispatchType,
                DocumentParentId,
                OrderId,
                DocumentDateIssued,
                DocumentReference
                );
            coc.VersionNumber = "H-" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _AddCommand(coc);
        }

        protected override void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
        {
            var item = lineItem as DispatchNoteLineItem;
           var ali = new AddDispatchNoteLineItemCommand(
                    Guid.NewGuid(),
                    Id,
                    DocumentIssuerUser.Id,
                    DocumentIssuerCostCentre.Id,
                    0,
                    DocumentIssuerCostCentreApplicationId,
                    item.LineItemSequenceNo,
                    item.Product.Id,
                    item.Qty,
                    item.Value,
                    item.LineItemVatValue,
                    (int)item.LineItemType,
                    DocumentParentId,
                    item.ProductDiscount,
                    (int)item.DiscountType
                    );
            _AddCommand(ali);
        }

        protected override void _AddConfirmCommandToExecute(bool isHybrid = false)
        {
            var co = new ConfirmDispatchNoteCommand(
                Guid.NewGuid(),
                Id,
                DocumentIssuerUser.Id,
                DocumentIssuerCostCentre.Id,
                0,
                DocumentIssuerCostCentreApplicationId,
                DocumentParentId);

            _AddCommand(co);
        }
    }
}
