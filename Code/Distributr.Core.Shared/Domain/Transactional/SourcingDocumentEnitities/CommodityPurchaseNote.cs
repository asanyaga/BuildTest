using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityPurchaseCommands;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;

namespace Distributr.Core.Domain.Transactional.SourcingDocumentEnitities
{
    public class CommodityPurchaseNote : SourcingDocument
    {
        public CommodityPurchaseNote(Guid id) : base(id, DocumentType.CommodityPurchaseNote)
        {
            _lineItems= new List<CommodityPurchaseLineItem>();
        }
        
        public CommodityOwner CommodityOwner { get; set; }
      
        public CommodityProducer CommodityProducer { get; set; }
        [Required(ErrorMessage = "Document On Behalf Of CostCentre is required")]
        public CommoditySupplier CommoditySupplier { get; set; }
        public string DeliveredBy { get; set; }
        public Guid RouteId { get; set; }
        public Guid CentreId { get; set; }
        public void AddLineItem(CommodityPurchaseLineItem lineItem)
        {
            if (Status != DocumentSourcingStatus.New)
                return;
            _lineItems.Add(lineItem);
            _AddAddLineItemCommandToExecute(lineItem);
        }
        List<CommodityPurchaseLineItem> _lineItems;
        public List<CommodityPurchaseLineItem> LineItems
        {
            get { return _lineItems; }

        }
        public void _SetLineItems(List<CommodityPurchaseLineItem> items)
        {
            _lineItems = items;
        }
        public override void Confirm()
        {
            if (Status != DocumentSourcingStatus.New)
                throw new InvalidDocumentOperationException("Cannot confirm a commodity purchase note that is not new");
            else
                Status = DocumentSourcingStatus.Confirmed;
            _AddCreateCommandToExecute();
            _AddConfirmCommandToExecute();
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
            var ic = new CreateCommodityPurchaseCommand
                (Guid.NewGuid(),Id,
                DocumentIssuerUser.Id,
                DocumentIssuerCostCentre.Id,
                0,
                DocumentIssuerCostCentreApplicationId,
                DocumentParentId,
                CommodityProducer.Id,
                CommodityOwner.Id,CommoditySupplier.Id,
                DocumentRecipientCostCentre.Id,
                DeliveredBy,
                Description,
                DocumentDateIssued,
                DocumentDate,Note,
                DocumentReference,
                DocumentIssuerCostCentre.Id,//Go:Todo:redundant remove
                DocumentIssuerUser.Id ,RouteId,CentreId//redundant
                );
            ic.VersionNumber = "H-" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _AddCommand(ic);
        }

        protected override void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
        {
            var item = lineItem as CommodityPurchaseLineItem;
            var command = new AddCommodityPurchaseLineItemCommand
                (
                Guid.NewGuid(),
                Id, item.Id,
                DocumentIssuerUser.Id,
                DocumentIssuerCostCentre.Id,
                0, DocumentIssuerCostCentreApplicationId,
                DocumentParentId, item.ParentLineItemId, item.Commodity.Id,
                item.CommodityGrade.Id, item.ContainerType.Id,
                item.ContainerNo, item.Weight, item.Description, item.Note, item.TareWeight, item.NoOfContainers
                ) {TareWeight = item.TareWeight, NoOfContainers = item.NoOfContainers};
            _AddCommand(command);
        }

        protected override void _AddConfirmCommandToExecute(bool isHybrid = false)
        {
            var ic = new ConfirmCommodityPurchaseCommand(
             Guid.NewGuid(), Id, DocumentIssuerUser.Id, DocumentIssuerCostCentre.Id, 0,
             DocumentIssuerCostCentreApplicationId, DocumentParentId);
            _AddCommand(ic);
        }
    }
}
