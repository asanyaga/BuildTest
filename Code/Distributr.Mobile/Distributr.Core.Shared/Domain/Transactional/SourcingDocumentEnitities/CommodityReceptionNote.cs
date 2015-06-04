using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityReceptionCommands;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;

namespace Distributr.Core.Domain.Transactional.SourcingDocumentEnitities
{
    public class CommodityReceptionNote : SourcingDocument
    {
        public CommodityReceptionNote(Guid id)
            : base(id, DocumentType.CommodityReceptionNote)
        {
            _lineItems = new List<CommodityReceptionLineItem>();
        }

        public void AddLineItem(CommodityReceptionLineItem lineItem)
        {
            if (Status != DocumentSourcingStatus.New)
                return;
            _lineItems.Add(lineItem);
            _AddAddLineItemCommandToExecute(lineItem);
        }


        private List<CommodityReceptionLineItem> _lineItems;

        public List<CommodityReceptionLineItem> LineItems
        {
            get { return _lineItems; }

        }

        public decimal TotalNetWeight
        {
            get { return LineItems.Sum(n => n.Weight); }
        }

        public void _SetLineItems(List<CommodityReceptionLineItem> items)
        {
            _lineItems = items;
        }

        public override void Confirm()
        {
            if (Status != DocumentSourcingStatus.New)
                throw new InvalidDocumentOperationException("Cannot confirm an commodity Reception note that is not new");
            else
                Status = DocumentSourcingStatus.Confirmed;
            _AddCreateCommandToExecute();
            _AddConfirmCommandToExecute();
        }

        public void MarkAsStoredLineItem(CommodityReceptionLineItem lineItem)
        {
            if (Status != DocumentSourcingStatus.Confirmed)
                throw new Exception("Delivery must be confirmed");
            var item = _lineItems.FirstOrDefault(s => s.Id == lineItem.Id);
            item.LineItemStatus = SourcingLineItemStatus.Stored;
            _AddStoreLineItemCommandToExecute(lineItem);
        }

        private void _AddStoreLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
        {
            if (!_CanAddCommands)
                return;
            CommodityReceptionLineItem item = lineItem as CommodityReceptionLineItem;
            var ic = new StoredCommodityReceptionLineItemCommand(
                Guid.NewGuid(),
                Id,
                item.Id,
                DocumentIssuerUser.Id
                , DocumentIssuerCostCentre.Id, 0, DocumentIssuerCostCentreApplicationId, Id
                );

            _AddCommand(ic);
        }

        public override void Approve()
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        protected override void _AddCreateCommandToExecute(bool isHybrid = false)
        {
            var command =
                new CreateCommodityReceptionCommand
                    (Guid.NewGuid(),
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
                     DocumentIssuerUser.Id
                     , VehicleArrivalTime, VehicleDepartureTime, VehicleArrivalMileage, VehicleDepartureMileage
                    );
            command.VersionNumber = "H-" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _AddCommand(command);
        }

        protected override void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
        {
            var item = lineItem as CommodityReceptionLineItem;
            var command =
                new AddCommodityReceptionLineItemCommand
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
                     item.ContainerType.Id,
                     item.ContainerNo,
                     item.Weight,
                     item.WeighType,
                     item.Description,
                     item.Note
                    );
            _AddCommand(command);
            
        }

        protected override void _AddConfirmCommandToExecute(bool isHybrid = false)
        {
            var command = new ConfirmCommodityReceptionCommand
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
    }
}
