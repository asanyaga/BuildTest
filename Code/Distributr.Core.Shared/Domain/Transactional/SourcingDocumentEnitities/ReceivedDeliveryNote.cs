using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;

namespace Distributr.Core.Domain.Transactional.SourcingDocumentEnitities
{
  public  class ReceivedDeliveryNote: SourcingDocument
    {
      public ReceivedDeliveryNote(Guid id)
          : base(id, DocumentType.ReceivedDelivery)
      {
          _lineItems = new List<ReceivedDeliveryLineItem>();
      }
      public void AddLineItem(ReceivedDeliveryLineItem lineItem)
      {
          if (Status != DocumentSourcingStatus.New)
              return;
          _lineItems.Add(lineItem);
          _AddAddLineItemCommandToExecute(lineItem);
      }
      private List<ReceivedDeliveryLineItem> _lineItems;
      public void MarkAsStoredLineItem(ReceivedDeliveryLineItem lineItem)
      {
          if (Status != DocumentSourcingStatus.Confirmed)
              throw new Exception("Delivery must be confirmed");
          var item = _lineItems.FirstOrDefault(s => s.Id == lineItem.Id);
          item.LineItemStatus = SourcingLineItemStatus.Stored;
          _AddStoredLineItemCommandToExecute(lineItem);
      }

      private void _AddStoredLineItemCommandToExecute(ReceivedDeliveryLineItem lineItem)
      {
          if (!_CanAddCommands)
              return;
          ReceivedDeliveryLineItem item = lineItem as ReceivedDeliveryLineItem;
          var ic = new StoredReceivedDeliveryLineItemCommand(
              Guid.NewGuid(),
              Id,
              item.Id,
              DocumentIssuerUser.Id
              , DocumentIssuerCostCentre.Id, 0, DocumentIssuerCostCentreApplicationId, Id
              );

          _AddCommand(ic);
      }

      public List<ReceivedDeliveryLineItem> LineItems
      {
          get { return _lineItems; }

      }
      public Guid RouteId { get; set; }
      public Guid CentreId { get; set; }
      public decimal TotalNetWeight
      {
          get { return LineItems.Sum(n => n.Weight); }
      }
      public override void Confirm()
      {
          if (Status != DocumentSourcingStatus.New)
              throw new InvalidDocumentOperationException("Cannot confirm an commodity Delivery note that is not new");

          Status = DocumentSourcingStatus.Confirmed;
          _AddCreateCommandToExecute();
          _AddConfirmCommandToExecute();
      }
    
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
          var ic = new CreateReceivedDeliveryCommand(
              Guid.NewGuid(),
              Id,
              DocumentIssuerUser.Id,
              DocumentIssuerCostCentre.Id,
              0,
              DocumentIssuerCostCentreApplicationId,
              DocumentParentId,
            DocumentReference,
              Description,
              DocumentDateIssued, DocumentDate, RouteId, CentreId, VehicleArrivalTime,
              VehicleDepartureTime, VehicleArrivalMileage, VehicleDepartureMileage);
          ic.DocumentRecipientCostCentreId = DocumentRecipientCostCentre.Id;
          ic.VersionNumber = "H-" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
          _AddCommand(ic);
     
      }

      protected override void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
      {
          if (!_CanAddCommands)
              return;
          var item = lineItem as ReceivedDeliveryLineItem;
          if (item.CommodityGrade != null)
          {
              var ic = new AddReceivedDeliveryLineItemCommand(
                  Guid.NewGuid(),
                  Id,
                  DocumentIssuerUser.Id
                  , DocumentIssuerCostCentre.Id, 0, DocumentIssuerCostCentreApplicationId,
                  item.CommodityGrade!=null?item.CommodityGrade.Id:Guid.Empty,item.ParentDocId,
                  item.ContainerNo, item.Weight,
                  item.DeliveredWeight, item.LineItemStatus, item.Description
                  );
              ic.CommodityId = item.Commodity.Id;
            //  ic.ContainerTypeId = item.ContainerType.Id;
          
              _AddCommand(ic);
          }
      }

      protected override void _AddConfirmCommandToExecute(bool isHybrid = false)
      {
          if (!_CanAddCommands)
              return;
          var ic = new ConfirmReceivedDeliveryCommand(
              Guid.NewGuid(), Id, DocumentIssuerUser.Id, DocumentIssuerCostCentre.Id, 0,
              DocumentIssuerCostCentreApplicationId, DocumentParentId);
          _AddCommand(ic);
      }
    }
}
