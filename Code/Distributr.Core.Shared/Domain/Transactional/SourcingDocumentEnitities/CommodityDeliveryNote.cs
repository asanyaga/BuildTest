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
  public  class CommodityDeliveryNote: SourcingDocument
    {
      private CommodityDeliveryNote(Guid id) : base(id, DocumentType.CommodityDelivery)
      {
          _lineItems= new List<CommodityDeliveryLineItem>();
      }
      public void AddLineItem(CommodityDeliveryLineItem lineItem)
      {
          if (Status != DocumentSourcingStatus.New)
              return;
          _lineItems.Add(lineItem);
          _AddAddLineItemCommandToExecute(lineItem);
      }
      public void MarkAsWeighedLineItem(CommodityDeliveryLineItem lineItem)
      {
          if (Status != DocumentSourcingStatus.Confirmed)
              throw new Exception("Delivery must be confirmed");
          var item = _lineItems.FirstOrDefault(s => s.Id == lineItem.Id);
          item.LineItemStatus = SourcingLineItemStatus.Weighed;
          _AddWeighLineItemCommandToExecute(lineItem);
      }
      private List<CommodityDeliveryLineItem> _lineItems;
      public Guid RouteId { get; set; }
      public Guid CentreId { get; set; }
      public List<CommodityDeliveryLineItem> LineItems
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

      public void MarkAsReceived()
      {
          if (Status != DocumentSourcingStatus.Confirmed)
              throw new InvalidDocumentOperationException("Cannot mark as receive a commodity Delivery note that is not new");

          Status = DocumentSourcingStatus.Received;
          _AddReceivedDeliveryCommandToExecute();
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
          var ic = new CreateCommodityDeliveryCommand(
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
              DocumentDateIssued, DocumentDate, DriverName, VehiclRegNo, RouteId, CentreId, VehicleArrivalTime,
              VehicleDepartureTime, VehicleArrivalMileage, VehicleDepartureMileage
              );
          ic.VersionNumber = "H-" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
          _AddCommand(ic);
     
      }

      protected override void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
      {
          if (!_CanAddCommands)
              return;
          CommodityDeliveryLineItem item = lineItem as CommodityDeliveryLineItem;
          var ic = new AddCommodityDeliveryLineItemCommand(
              Guid.NewGuid(),
              Id,
              item.Id,
              DocumentIssuerUser.Id
              , DocumentIssuerCostCentre.Id, 0, DocumentIssuerCostCentreApplicationId, Id, item.ParentLineItemId,
              item.Commodity.Id, item.CommodityGrade.Id,item.ContainerType.Id,item.ContainerNo, item.Weight,item.WeighType, item.Description, item.Note
              );
          ic.NoOfContainers = item.NoOfContainers;
          
         _AddCommand(ic);
      }
      private  void _AddWeighLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
      {
          if (!_CanAddCommands)
              return;
          CommodityDeliveryLineItem item = lineItem as CommodityDeliveryLineItem;
          var ic = new WeighedCommodityDeliveryLineItemCommand(
              Guid.NewGuid(),
              Id,
              item.Id,
              DocumentIssuerUser.Id
              , DocumentIssuerCostCentre.Id, 0, DocumentIssuerCostCentreApplicationId, Id
              );

          _AddCommand(ic);
      }
      protected override void _AddConfirmCommandToExecute(bool isHybrid = false)
      {
          if (!_CanAddCommands)
              return;
          var ic = new ConfirmCommodityDeliveryCommand(
              Guid.NewGuid(), Id, DocumentIssuerUser.Id, DocumentIssuerCostCentre.Id, 0,
              DocumentIssuerCostCentreApplicationId, DocumentParentId);

          _AddCommand(ic);
      }

      protected void _AddReceivedDeliveryCommandToExecute(bool isHybrid = false)
      {
          if (!_CanAddCommands)
              return;
          var ic = new ApproveDeliveryCommand(
              Guid.NewGuid(), Id, DocumentIssuerUser.Id, DocumentIssuerCostCentre.Id, 0,
              DocumentIssuerCostCentreApplicationId, DocumentParentId);
          _AddCommand(ic);
      }

    }
}
