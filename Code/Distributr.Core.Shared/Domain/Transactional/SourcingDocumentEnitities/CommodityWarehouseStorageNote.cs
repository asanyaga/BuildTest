using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityWarehouseStorageCommands;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;

namespace Distributr.Core.Domain.Transactional.SourcingDocumentEnitities
{
  public  class CommodityWarehouseStorageNote: SourcingDocument
    {
      private CommodityWarehouseStorageNote(Guid id)
          : base(id, DocumentType.CommodityWarehouseStorage)
      {
          _lineItems = new List<CommodityWarehouseStorageLineItem>();
      }
      public void AddLineItem(CommodityWarehouseStorageLineItem lineItem)
      {
          if (Status != DocumentSourcingStatus.New)
              return;
          _lineItems.Add(lineItem);
          _AddAddLineItemCommandToExecute(lineItem);
      }

      public void UpdateLineItem(CommodityWarehouseStorageLineItem lineItem)
      {
          if (Status != DocumentSourcingStatus.Confirmed)
              return;
          //_lineItems.Add(lineItem);
          _AddUpdateLineItemCommandToExecute(lineItem);
      }
     
      private List<CommodityWarehouseStorageLineItem> _lineItems;
      public Guid RouteId { get; set; }
      public Guid CentreId { get; set; }
      public Guid CommodityOwnerId { get; set; }
      public Guid StoreId { get; set; }
      public List<CommodityWarehouseStorageLineItem> LineItems
      {
          get { return _lineItems; }

      }
      public decimal TotalNetWeight
      {
          get { return LineItems.Sum(n => n.Weight); }
      }

      public decimal FinalWeight
      {
          get { return LineItems.Sum(n => n.FinalWeight); }
      }
      public override void Confirm()
      {
          if (Status != DocumentSourcingStatus.New)
              throw new InvalidDocumentOperationException("Cannot confirm a commodity warehouse storage note that is not new");

          Status = DocumentSourcingStatus.Confirmed;
          _AddCreateCommandToExecute();
          _AddConfirmCommandToExecute();
      }

      public override void Approve()
      {
         

          Status = DocumentSourcingStatus.Approved;
          
          _AddApproveCommandToExecute();
      }


      public void GenerateReceipt()
      {
          Status = DocumentSourcingStatus.ReceiptGenerated;

          _AddGenerateReceiptCommandToExecute();
      }

      public  void Store()
      {
          Status = DocumentSourcingStatus.Closed;
          
          _AddStoreCommandToExecute();
      }



      public string DriverName { get; set; }
      public string VehiclRegNo { get; set; }
      

      public override void Close()
      {
          
      }

      protected override void _AddCreateCommandToExecute(bool isHybrid = false)
      {
          if (!_CanAddCommands)
              return;
          var ic = new CreateCommodityWarehouseStorageCommand();
         ic.CommandId= Guid.NewGuid();
          ic.DocumentId = Id;
          ic.CommandGeneratedByUserId = DocumentIssuerUser.Id;
          ic.DocumentIssuerCostCentreId = DocumentIssuerCostCentre.Id;
          ic.CommandGeneratedByCostCentreApplicationId = DocumentIssuerCostCentreApplicationId;
          ic.PDCommandId = DocumentParentId;
          ic.DocumentRecipientCostCentreId = DocumentRecipientCostCentre.Id;
          ic.Note = Note;
          ic.DocumentReference = DocumentReference;
          ic.Description = Description;
          ic.DocumentDateIssued = DocumentDateIssued;
          ic.DateCreated = DocumentDate;
          ic.DriverName = DriverName;
          ic.VehicleRegNo = VehiclRegNo;
          ic.RouteId = RouteId;
          ic.CentreId = CentreId;
          ic.CommodityOwnerId = CommodityOwnerId;
          ic.VehicleArrivalTime = VehicleArrivalTime;
          ic.VehicleDepartureTime = VehicleDepartureTime;
          ic.VehicleArrivalMileage = VehicleArrivalMileage;
          ic.VehicleDepartureMileage= VehicleDepartureMileage;

          ic.VersionNumber = "H-" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
          _AddCommand(ic);
     
      }

      protected override void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
      {
          if (!_CanAddCommands)
              return;
          CommodityWarehouseStorageLineItem item = lineItem as CommodityWarehouseStorageLineItem;
          var ic = new AddCommodityWarehouseStorageLineItemCommand();
          ic.CommandId = Guid.NewGuid();
          ic.DocumentId = Id;
          ic.DocumentLineItemId = item.Id;
           ic.CommandGeneratedByUserId=   DocumentIssuerUser.Id;
          ic.CommandGeneratedByCostCentreId = DocumentIssuerCostCentre.Id;
          ic.CommandGeneratedByCostCentreApplicationId = DocumentIssuerCostCentreApplicationId;
          ic.PDCommandId = Id;
          ic.ParentLineItemId = item.ParentLineItemId;
          ic.CommodityId = item.Commodity.Id;
          ic.CommodityGradeId = item.CommodityGrade!=null?item.CommodityGrade.Id:Guid.Empty;
          ic.Weight = item.Weight;
          ic.Description = item.Description;
          ic.Note= item.Note;
          
         _AddCommand(ic);
      }

      protected  void _AddUpdateLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
      {
          if (!_CanAddCommands)
              return;
          CommodityWarehouseStorageLineItem item = lineItem as CommodityWarehouseStorageLineItem;
          var ic = new UpdateCommodityWarehouseStorageLineItemCommand();
          ic.CommandId = Guid.NewGuid();
          ic.DocumentId = Id;
          ic.DocumentLineItemId = item.Id;
          ic.CommandGeneratedByUserId = DocumentIssuerUser.Id;
          ic.CommandGeneratedByCostCentreId = DocumentIssuerCostCentre.Id;
          ic.CommandGeneratedByCostCentreApplicationId = DocumentIssuerCostCentreApplicationId;
          ic.PDCommandId = Id;
          ic.ParentLineItemId = item.ParentLineItemId;
          ic.CommodityId = item.Commodity.Id;
          ic.CommodityGradeId = item.CommodityGrade != null ? item.CommodityGrade.Id : Guid.Empty;
          ic.Weight = item.Weight;
          ic.Description = item.Description;
          ic.Note = item.Note;

          _AddCommand(ic);
      }
     
      protected override void _AddConfirmCommandToExecute(bool isHybrid = false)
      {
          if (!_CanAddCommands)
              return;
          var ic = new ConfirmCommodityWarehouseStorageCommand();
          ic.CommandId = Guid.NewGuid();
          ic.DocumentId = Id;
          ic.CommandGeneratedByUserId = DocumentIssuerUser.Id;
          ic.CommandGeneratedByCostCentreId = DocumentIssuerCostCentre.Id;
          ic.CommandGeneratedByCostCentreApplicationId = DocumentIssuerCostCentreApplicationId;
          ic.PDCommandId= DocumentParentId;

          _AddCommand(ic);
      }

      protected  void _AddApproveCommandToExecute(bool isHybrid = false)
      {
          if (!_CanAddCommands)
              return;
          var ic = new ApproveCommodityWarehouseStorageCommand();
          ic.CommandId = Guid.NewGuid();
          ic.DocumentId = Id;
          ic.CommandGeneratedByUserId = DocumentIssuerUser.Id;
          ic.CommandGeneratedByCostCentreId = DocumentIssuerCostCentre.Id;
          ic.CommandGeneratedByCostCentreApplicationId = DocumentIssuerCostCentreApplicationId;
          ic.PDCommandId = DocumentParentId;

          _AddCommand(ic);
      }

      protected void _AddGenerateReceiptCommandToExecute(bool isHybrid = false)
      {
          if (!_CanAddCommands)
              return;
          var ic = new GenerateReceiptCommodityWarehouseStorageCommand();
          ic.CommandId = Guid.NewGuid();
          ic.DocumentId = Id;
          ic.CommandGeneratedByUserId = DocumentIssuerUser.Id;
          ic.CommandGeneratedByCostCentreId = DocumentIssuerCostCentre.Id;
          ic.CommandGeneratedByCostCentreApplicationId = DocumentIssuerCostCentreApplicationId;
          ic.PDCommandId = DocumentParentId;

          _AddCommand(ic);
      }

      protected void _AddStoreCommandToExecute(bool isHybrid = false)
      {
          if (!_CanAddCommands)
              return;
          var ic = new StoreCommodityWarehouseStorageCommand();
          ic.CommandId = Guid.NewGuid();
          ic.DocumentId = Id;
          ic.CommandGeneratedByUserId = DocumentIssuerUser.Id;
          ic.CommandGeneratedByCostCentreId = DocumentIssuerCostCentre.Id;
          ic.CommandGeneratedByCostCentreApplicationId = DocumentIssuerCostCentreApplicationId;
          ic.PDCommandId = DocumentParentId;
          ic.StoreId = StoreId;

          _AddCommand(ic);
      }

     

    }
}
