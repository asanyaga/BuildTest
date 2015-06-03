using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Distributr.Core.Commands.DocumentCommands.AdjustmentNotes;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;

namespace Distributr.Core.Domain.Transactional.DocumentEntities
{
    public enum InventoryAdjustmentNoteType { Available = 1, UnAvailable = 2, Returns = 3, StockTake = 4, AdjustOnly = 5, OutletStockTake=6 }

    public class InventoryAdjustmentNote : Document
    {

        private InventoryAdjustmentNote(Guid id)
            : base(id)
        {
            _lineItem = new List<InventoryAdjustmentNoteLineItem>();
            _CanAddCommands = false;
        }

        public InventoryAdjustmentNoteType InventoryAdjustmentNoteType { get; set; }

        public void AddLineItem(InventoryAdjustmentNoteLineItem lineItem)
        {
            if (Status != DocumentStatus.New)
                return;
            _lineItem.Add(lineItem);
            _AddAddLineItemCommandToExecute(lineItem);
        }

        List<InventoryAdjustmentNoteLineItem> _lineItem;
        public List<InventoryAdjustmentNoteLineItem> LineItem
        {
            get { return _lineItem; }

        }

        public override void Confirm()
        {
            if (Status != DocumentStatus.New)
                return;
            Status = DocumentStatus.Confirmed;
            _AddCreateCommandToExecute();
            _AddConfirmCommandToExecute();
        }


        protected override void _AddCreateCommandToExecute(bool isHybrid = false)
        {
            var coc = new CreateInventoryAdjustmentNoteCommand(
               Guid.NewGuid(),
               Id,
               DocumentIssuerUser.Id,
               DocumentIssuerCostCentre.Id,
               0,
               DocumentIssuerCostCentreApplicationId,
               DocumentIssuerCostCentre.Id,
               DocumentIssuerUser.Id,
               (int)InventoryAdjustmentNoteType,
               DocumentReference,DocumentDateIssued
            );
            coc.VersionNumber = "H-" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _AddCommand(coc);
        }

        protected override void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
        {
            var item = lineItem as InventoryAdjustmentNoteLineItem;

            var lic = new AddInventoryAdjustmentNoteLineItemCommand(
                    Guid.NewGuid(),
                    Id,
                    DocumentIssuerUser.Id,
                    DocumentIssuerCostCentre.Id,
                    0,
                   DocumentIssuerCostCentreApplicationId,
                    item.Product.Id,
                    item.Actual,
                    item.Qty,
                    1, item.Description);
            _AddCommand(lic);
        }


        protected override void _AddConfirmCommandToExecute(bool isHybrid = false)
        {
            var co = new ConfirmInventoryAdjustmentNoteCommand(
                Guid.NewGuid(),
                Id,
                DocumentIssuerUser.Id,
                DocumentIssuerCostCentre.Id,
                0,
                DocumentIssuerCostCentreApplicationId);
            _AddCommand(co);
        }

        public void _SetLineItems(List<InventoryAdjustmentNoteLineItem> items)
        {
            _lineItem = items;
        }
    }
}
