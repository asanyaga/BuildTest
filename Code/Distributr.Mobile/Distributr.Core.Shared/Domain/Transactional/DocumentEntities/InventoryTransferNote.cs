using System;
using System.Collections.Generic;
using System.Reflection;
using Distributr.Core.Commands.DocumentCommands.InventoryTransferNotes;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.Core.Domain.Transactional.DocumentEntities
{
    public class InventoryTransferNote : Document
    {
        private InventoryTransferNote(Guid id) : base(id)
        {
            _lineItems = new List<InventoryTransferNoteLineItem>();
            _CanAddCommands = false;
        }

        
        public CostCentre DocumentIssueredOnBehalfCostCentre { get; internal set; }

        public List<InventoryTransferNoteLineItem> LineItems
        {
            get { return _lineItems; }
        }

        public void AddLineItem(InventoryTransferNoteLineItem inventoryTransferNoteLineItem)
        {
            if (Status != DocumentStatus.New)
                throw new InvalidDocumentOperationException("Cannot add line items to an inventory transfer note that is not new");
            inventoryTransferNoteLineItem.IsNew = true;
            LineItems.Add(inventoryTransferNoteLineItem);
            _AddAddLineItemCommandToExecute(inventoryTransferNoteLineItem);
        }

        private List<InventoryTransferNoteLineItem> _lineItems; 



        public void _SetLineItems(List<InventoryTransferNoteLineItem> LineItems)
        {
            _lineItems = LineItems;
        }

        public override void Confirm()
        {
            if (Status != DocumentStatus.New)
                throw new InvalidDocumentOperationException("Cannot confirm an inventory transfer note that is not new");
            Status = DocumentStatus.Confirmed;
            _AddCreateCommandToExecute();
            _AddConfirmCommandToExecute();
        }

        protected override void _AddCreateCommandToExecute(bool isHybrid = false)
        {
            var coc = new CreateInventoryTransferNoteCommand(
               Guid.NewGuid(),
               Id,
               DocumentIssuerUser.Id,
               DocumentIssuerCostCentre.Id,
               0,
               DocumentIssuerCostCentreApplicationId,
               DocumentIssuerCostCentre.Id,
               DocumentIssuerUser.Id,
               DocumentIssuerUser.Id,
               DocumentIssuerCostCentre.Id,
               DocumentRecipientCostCentre.Id,
               DocumentDateIssued, DocumentReference
            );
            coc.PDCommandId = DocumentParentId;
            coc.VersionNumber = "H-" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _AddCommand(coc);
        }

        protected override void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
        {
            var item = lineItem as InventoryTransferNoteLineItem;
            var li = new AddInventoryTransferNoteLineItemCommand(Guid.NewGuid(),
                                                                 Id,
                                                                 DocumentIssuerUser.Id,
                                                                 DocumentIssuerCostCentre.Id,
                                                                 0,
                                                                 DocumentIssuerCostCentreApplicationId,
                                                                 1, item.Product.Id, item.Qty);
            _AddCommand(li);
        }


        protected override void _AddConfirmCommandToExecute(bool isHybrid = false)
        {
            var co = new ConfirmInventoryTransferNoteCommand(
                Guid.NewGuid(),
                Id,
                DocumentIssuerUser.Id,
                DocumentIssuerCostCentre.Id,
                0,
                DocumentIssuerCostCentreApplicationId
                );
            _AddCommand(co);
        }
    }
}
