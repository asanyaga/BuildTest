using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityStorageCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityTransferCommands;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;

namespace Distributr.Core.Domain.Transactional.SourcingDocumentEnitities
{
    public class CommodityTransferNote : SourcingDocument
    {
        public enum CommodityTransferNoteTypeId { ToHq = 1, ToOtherStore = 2 }

        public CommodityTransferNote(Guid id)
            : base(id, DocumentType.CommodityTransferNote)
        {
            _lineItems = new List<CommodityTransferLineItem>();
        }

        public CostCentre WareHouseToStore { get; set; }

        public CommodityTransferNoteTypeId TransferNoteTypeId { get; set; }

        private List<CommodityTransferLineItem> _lineItems;

        public void AddLineItem(CommodityTransferLineItem lineItem)
        {
            if (Status != DocumentSourcingStatus.New)
                return;
            _lineItems.Add(lineItem);
            _AddAddLineItemCommandToExecute(lineItem);
        }

        public List<CommodityTransferLineItem> LineItems
        {
            get { return _lineItems; }

        }
        public void _SetLineItems(List<CommodityTransferLineItem> items)
        {
            _lineItems = items;
        }
        public override void Confirm()
        {
            if (Status != DocumentSourcingStatus.New)
                throw new InvalidDocumentOperationException("Cannot confirm a commodity Transfer note that is not new");
            else
                Status = DocumentSourcingStatus.Confirmed;
            _AddCreateCommandToExecute();
            _AddConfirmCommandToExecute();
        }

        public override void Approve()
        {
            if (Status != DocumentSourcingStatus.Confirmed)
                throw new InvalidDocumentOperationException("Cannot Approve a commodity Transfer note that is not Confirmed");
            Status = DocumentSourcingStatus.Approved;
            _CanAddCommands = true;
            _AddApproveCommandToExecute();
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        protected override void _AddCreateCommandToExecute(bool isHybrid = false)
        {
            var command =
                new CreateCommodityTransferCommand(
                    Guid.NewGuid(),
                    Id,
                    DocumentIssuerUser.Id,
                    DocumentIssuerCostCentre.Id,
                    0,
                    DocumentIssuerCostCentreApplicationId,
                    DocumentParentId,
                    DocumentRecipientCostCentre.Id,
                    Note,
                    DocumentReference,
                    Description,
                    DocumentDateIssued,
                    DocumentDate,
                    DocumentIssuerCostCentre.Id,
                    DocumentIssuerUser.Id, 
                    (int)TransferNoteTypeId,
                    (WareHouseToStore != null ? WareHouseToStore.Id : (Guid?) null)
                    );
            command.VersionNumber = "H-" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _AddCommand(command);
        }

        protected override void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
        {
            var item = lineItem as CommodityTransferLineItem;
            var command =
                new  AddCommodityTransferLineItemCommand
                    (Guid.NewGuid(),
                     Id,
                     item.Id,
                     DocumentIssuerUser.Id,
                     DocumentIssuerCostCentre.Id,
                     0,
                     DocumentIssuerCostCentreApplicationId,
                     DocumentParentId,
                     item.ParentLineItemId,
                     item.Commodity.Id,
                     item.CommodityGrade.Id,
                     Guid.Empty,
                     item.ContainerNo,
                     item.Weight,
                     item.Description,
                     item.Note
                    );
            _AddCommand(command);
        }

        protected override void _AddConfirmCommandToExecute(bool isHybrid = false)
        {
            var command = new ConfirmCommodityTransferCommand
               (Guid.NewGuid(),
                Id,
                DocumentIssuerUser.Id,
                DocumentIssuerCostCentre.Id,
                0,
                DocumentIssuerCostCentreApplicationId,
                DocumentParentId
               );
            _AddCommand(command);
        }

        protected void _AddApproveCommandToExecute(bool isHybrid = false)
        {
            var command = new ApproveCommodityTransferCommand
               (Guid.NewGuid(),
                Id,
                DocumentIssuerUser.Id,
                DocumentIssuerCostCentre.Id,
                0,
                DocumentIssuerCostCentreApplicationId,
                DocumentParentId,
                WareHouseToStore.Id
               );
            _AddCommand(command);
        }

        public void MarkAsTransferedLineItem(CommodityStorageLineItem lineItem)
        {
            if (lineItem == null) return;
            /*var item = _lineItems.FirstOrDefault(s => s.Id == lineItem.Id);
            item.LineItemStatus = SourcingLineItemStatus.Transfered;*/
            _AddTransferedLineItemCommandToExecute(lineItem);
        }

        private void _AddTransferedLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
        {
            if (!_CanAddCommands)
                return;
            var item = lineItem as CommodityStorageLineItem;
            var ic = new TransferedCommodityStorageLineItemCommand(
                Guid.NewGuid(),
                Id,
                item.Id,
                DocumentIssuerUser.Id
                , DocumentIssuerCostCentre.Id, 0, DocumentIssuerCostCentreApplicationId, Id
                );

            _AddCommand(ic);
        }

    }
}
