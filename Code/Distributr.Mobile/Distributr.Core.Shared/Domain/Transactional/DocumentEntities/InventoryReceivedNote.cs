using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Distributr.Core.Commands.DocumentCommands.InventoryReceivedNotes;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;

namespace Distributr.Core.Domain.Transactional.DocumentEntities
{
    public class InventoryReceivedNote : Document
    {//
        private InventoryReceivedNote(Guid id)
            : base(id)
        {
            _lineItems = new List<InventoryReceivedNoteLineItem>();
            _CanAddCommands = false;
        }

        public string OrderReferences { get; set; }
        public string LoadNo { get; set; }

        //Invoice
        //Supplier/Producer Phone No

        List<InventoryReceivedNoteLineItem> _lineItems;
        public List<InventoryReceivedNoteLineItem> LineItems
        {
            get { return _lineItems; }
        }


        /// <summary>
        /// Can be null if delivery received from outside system
        /// </summary>
        public CostCentre GoodsReceivedFromCostCentre { get; set; }

        public void AddLineItem(InventoryReceivedNoteLineItem inventoryReceivedNoteLineItem)
        {
            if (Status != DocumentStatus.New)
                throw new InvalidDocumentOperationException("Cannot add line items to an inventory received note that is not new");
            inventoryReceivedNoteLineItem.IsNew = true;
            _lineItems.Add(inventoryReceivedNoteLineItem);
            _AddAddLineItemCommandToExecute(inventoryReceivedNoteLineItem);
        }

        public override void Confirm()
        {
            if (Status != DocumentStatus.New)
                throw new InvalidDocumentOperationException("Cannot confirm an inventory received note that is not new");
            Status = DocumentStatus.Confirmed;
            _AddCreateCommandToExecute();
            _AddConfirmCommandToExecute();
        }


        protected override void _AddCreateCommandToExecute(bool isHybrid = false)
        {
            var cirn = new CreateInventoryReceivedNoteCommand(Guid.NewGuid(),
                Id, 
                DocumentIssuerUser.Id,
                DocumentIssuerCostCentre.Id,
                0,
                DocumentIssuerCostCentreApplicationId,
                DocumentIssuerCostCentre.Id,
                DocumentIssuerUser.Id,
                DocumentIssuerUser.Id,
                GoodsReceivedFromCostCentre.Id,
                OrderReferences,
                LoadNo,
                DocumentReference,
                DocumentDateIssued);
            cirn.VersionNumber = "H-" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _AddCommand(cirn);
        }

        protected override void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
        {
            var item = lineItem as InventoryReceivedNoteLineItem;
            var aliirn =
                    new AddInventoryReceivedNoteLineItemCommand(
                        Guid.NewGuid(),
                        Id,
                        item.Id,
                        DocumentIssuerUser.Id,
                       DocumentIssuerCostCentre.Id,
                        0,
                        DocumentIssuerCostCentreApplicationId,
                        item.LineItemSequenceNo,
                        item.Product.Id,
                        item.Qty,
                        item.Expected,
                        item.Value);
            _AddCommand(aliirn);
        }


        protected override void _AddConfirmCommandToExecute(bool isHybrid = false)
        {
            var cc = new ConfirmInventoryReceivedNoteCommand(
                 Guid.NewGuid(),
                 Id,
                 DocumentIssuerUser.Id,
                 DocumentIssuerCostCentre.Id,
                 0,
                 DocumentIssuerCostCentreApplicationId);

            _AddCommand(cc);
        }

        public void _SetLineItems(List<InventoryReceivedNoteLineItem> items)
        {
            _lineItems = items;
        }
    }
}
