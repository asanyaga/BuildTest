using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityReleaseCommands;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;

namespace Distributr.Core.Domain.Transactional.SourcingDocumentEnitities
{
    public class CommodityReleaseNote : SourcingDocument
    {
        private CommodityReleaseNote(Guid id)
            : base(id, DocumentType.CommodityRelease)
        {
            _lineItems = new List<CommodityReleaseLineItem>();
        }
        public void AddLineItem(CommodityReleaseLineItem lineItem)
        {
            if (Status != DocumentSourcingStatus.New)
                return;
            _lineItems.Add(lineItem);
            _AddAddLineItemCommandToExecute(lineItem);
        }
        
        private List<CommodityReleaseLineItem> _lineItems;
        public Guid RouteId { get; set; }
        public Guid CentreId { get; set; }
        public List<CommodityReleaseLineItem> LineItems
        {
            get { return _lineItems; }

        }
        public decimal TotalNetWeight
        {
            get { return LineItems.Sum(n => n.Weight); }
        }
        public override void Confirm()
        {
            if (Status != DocumentSourcingStatus.New)
                throw new InvalidDocumentOperationException("Cannot confirm a commodity Delivery note that is not new");

            Status = DocumentSourcingStatus.Confirmed;
            _AddCreateCommandToExecute();
            _AddConfirmCommandToExecute();
        }




        public string DriverName { get; set; }
        public string VehiclRegNo { get; set; }
        public override void Approve()
        {

        }

        public override void Close()
        {

        }

        protected override void _AddCreateCommandToExecute(bool isHybrid = false)
        {
            if (!_CanAddCommands)
                return;
            var ic = new CreateCommodityReleaseCommand();
            ic.CommandId = Guid.NewGuid();
            ic.DocumentId = Id;
            ic.CommandGeneratedByUserId = DocumentIssuerUser.Id;
            ic.CommandGeneratedByCostCentreId = DocumentIssuerCostCentre.Id;
            ic.CommandSequence = 0;
            ic.CommandGeneratedByCostCentreApplicationId = DocumentIssuerCostCentreApplicationId;
            ic.PDCommandId = DocumentParentId;
            ic.DocumentRecipientCostCentreId = DocumentRecipientCostCentre.Id;
            ic.Note = Note;
            ic.DocumentReference = DocumentReference;
            ic.Description = Description;
            ic.DocumentDateIssued = DocumentDateIssued;
            ic.CommandCreatedDateTime = DocumentDate;
            ic.VersionNumber = "H-" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            ic.DocumentIssuerCostCentreId = DocumentIssuerCostCentre.Id;
            ic.DocIssuerUserId = DocumentIssuerUser.Id;
            _AddCommand(ic);

        }

        protected override void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
        {
            if (!_CanAddCommands)
                return;
            CommodityReleaseLineItem item = lineItem as CommodityReleaseLineItem;
            var ic = new AddCommodityReleaseNoteLineItemCommand();
            ic.CommandId = Guid.NewGuid();
            ic.DocumentId = Id;
            ic.DocumentLineItemId = item.Id;
            ic.CommandGeneratedByUserId=DocumentIssuerUser.Id;
            ic.CommandGeneratedByCostCentreId = DocumentIssuerCostCentre.Id;
            ic.CommandSequence = 0;
            ic.CommandGeneratedByCostCentreApplicationId=DocumentIssuerCostCentreApplicationId;
            ic.ParentLineItemId = item.ParentLineItemId;
            ic.CommodityId = item.Commodity.Id;
            ic.CommodityGradeId = item.CommodityGrade.Id;
            ic.ContainerNo = item.ContainerNo;
            ic.Weight = item.Weight;
            ic.Note = item.Description;
            ic.Note = item.Note;

            _AddCommand(ic);
        }

        protected override void _AddConfirmCommandToExecute(bool isHybrid = false)
        {
            if (!_CanAddCommands)
                return;
            var ic = new ConfirmCommodityReleaseNoteCommand();
            ic.CommandId = Guid.NewGuid();
            ic.DocumentId = Id;
            ic.CommandGeneratedByUserId = DocumentIssuerUser.Id;
            ic.CommandGeneratedByCostCentreId = DocumentIssuerCostCentre.Id;
            ic.CommandSequence = 0;
            ic.CommandGeneratedByCostCentreApplicationId = DocumentIssuerCostCentreApplicationId;
            ic.PDCommandId = DocumentParentId;

            _AddCommand(ic);
        }

    }
}