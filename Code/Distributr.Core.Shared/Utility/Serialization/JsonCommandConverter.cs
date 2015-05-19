using System;
using Distributr.Core.Commands;
using Distributr.Core.Commands.ActivityDocumentCommands;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.AdjustmentNotes;
using Distributr.Core.Commands.DocumentCommands.CreditNotes;
using Distributr.Core.Commands.DocumentCommands.DisbursementNotes;
using Distributr.Core.Commands.DocumentCommands.Discounts;
using Distributr.Core.Commands.DocumentCommands.DispatchNotes;
using Distributr.Core.Commands.DocumentCommands.InventoryReceivedNotes;
using Distributr.Core.Commands.DocumentCommands.InventoryTransferNotes;
using Distributr.Core.Commands.DocumentCommands.Invoices;
using Distributr.Core.Commands.DocumentCommands.Losses;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Commands.DocumentCommands.OutletDocument;
using Distributr.Core.Commands.DocumentCommands.Receipts;
using Distributr.Core.Commands.DocumentCommands.Recollections;
using Distributr.Core.Commands.DocumentCommands.ReturnsNotes;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityPurchaseCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityReceptionCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityReleaseCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityStorageCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityTransferCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityWarehouseStorageCommands;
using Newtonsoft.Json.Linq;

namespace Distributr.Core.Utility.Serialization
{
    public class JsonCommandConverter : JsonCreationConverter<DocumentCommand>
    {
        protected override DocumentCommand Create(Type objectType, JObject jObject)
        {
            DocumentCommand returnType = null;
            string commandTypeRef = jObject.Value<string>("CommandTypeRef");

            CommandType commandType = (CommandType)Enum.Parse(typeof(CommandType), commandTypeRef);
            switch (commandType)
            {

                //case CommandType.AddOrderLineItem:
                //    returnType = new AddOrderLineItemCommand();
                //    break;
                //case CommandType.ApproveOrder:
                //    returnType = new ApproveOrderCommand();
                //    break;
                //case CommandType.ConfirmOrder:
                //    returnType = new ConfirmOrderCommand();
                //    break;
                //case CommandType.CreateOrder:
                //    returnType = new CreateOrderCommand();
                //    break;
                case CommandType.RejectOrder:
                    returnType = new RejectOrderCommand();
                    break;
                case CommandType.ChangeOrderLineItem:
                    returnType = new ChangeOrderLineItemCommand(); ;
                    break;
                //IAN
                case CommandType.CreateInventoryAdjustmentNote:
                    returnType = new CreateInventoryAdjustmentNoteCommand();
                    break;
                case CommandType.AddInventoryAdjustmentNoteLineItem:
                    returnType = new AddInventoryAdjustmentNoteLineItemCommand(); ;
                    break;
                case CommandType.ConfirmInventoryAdjustmentNote:
                    returnType = new ConfirmInventoryAdjustmentNoteCommand(); ;
                    break;
                case CommandType.RemoveOrderLineItem:
                    returnType = new RemoveOrderLineItemCommand(); ;
                    break;
                //ITN
                case CommandType.CreateInventoryTransferNote:
                    returnType = new CreateInventoryTransferNoteCommand(); ;
                    break;
                case CommandType.AddInventoryTransferNoteLineItem:
                    returnType = new AddInventoryTransferNoteLineItemCommand(); ;
                    break;
                case CommandType.ConfirmInventoryTransferNote:
                    returnType = new ConfirmInventoryTransferNoteCommand(); ;
                    break;
                //DN
                case CommandType.CreateDispatchNote:
                    returnType = new CreateDispatchNoteCommand(); ;
                    break;
                case CommandType.AddDispatchNoteLineItem:
                    returnType = new AddDispatchNoteLineItemCommand(); ;
                    break;
                case CommandType.ConfirmDispatchNote:
                    returnType = new ConfirmDispatchNoteCommand(); ;
                    break;
                //CN
                case CommandType.CreateCreditNote:
                    returnType = new CreateCreditNoteCommand(); ;
                    break;
                case CommandType.AddCreditNoteLineItem:
                    returnType = new AddCreditNoteLineItemCommand(); ;
                    break;
                case CommandType.ConfirmCreditNote:
                    returnType = new ConfirmCreditNoteCommand(); ;
                    break;
                case CommandType.OrderPendingDispatch:
                    returnType = new OrderPendingDispatchCommand(); ;
                    break;
                case CommandType.DispatchToPhone:
                    returnType = new DispatchToPhoneCommand(); ;
                    break;
                case CommandType.CreateInventoryReceivedNote:
                    returnType = new CreateInventoryReceivedNoteCommand(); ;
                    break;
                case CommandType.AddInventoryReceivedNoteLineItem:
                    returnType = new AddInventoryReceivedNoteLineItemCommand(); ;
                    break;
                case CommandType.ConfirmInventoryReceivedNote:
                    returnType = new ConfirmInventoryReceivedNoteCommand(); ;
                    break;
                case CommandType.CloseOrder:
                    returnType = new CloseOrderCommand(); ;
                    break;
                case CommandType.CreateInvoice:
                    returnType = new CreateInvoiceCommand(); ;
                    break;
                case CommandType.AddInvoiceLineItem:
                    returnType = new AddInvoiceLineItemCommand(); ;
                    break;
                case CommandType.ConfirmInvoice:
                    returnType = new ConfirmInvoiceCommand(); ;
                    break;
                case CommandType.CreateReceipt:
                    returnType = new CreateReceiptCommand(); ;
                    break;
                case CommandType.AddReceiptLineItem:
                    returnType = new AddReceiptLineItemCommand(); ;
                    break;
                case CommandType.ConfirmReceiptLineItem:
                    returnType = new ConfirmReceiptLineItemCommand(); ;
                    break;
                case CommandType.ConfirmReceipt:
                    returnType = new ConfirmReceiptCommand(); ;
                    break;
                //Returns note
                case CommandType.CreateReturnsNote:
                    returnType = new CreateReturnsNoteCommand(); ;
                    break;
                case CommandType.AddReturnsNoteLineItem:
                    returnType = new AddReturnsNoteLineItemCommand(); ;
                    break;
                case CommandType.ConfirmReturnsNote:
                    returnType = new ConfirmReturnsNoteCommand(); ;
                    break;
                case CommandType.CloseReturnsNote:
                    returnType = new CloseReturnsNoteCommand(); ;
                    break;
                case CommandType.CreateDisbursementNote:
                    returnType = new CreateDisbursementNoteCommand(); ;
                    break;
                case CommandType.AddDisbursementNoteLineItem:
                    returnType = new AddDisbursementNoteLineItemCommand(); ;
                    break;
                case CommandType.ConfirmDisbursementNote:
                    returnType = new ConfirmDisbursementNoteCommand(); ;
                    break;
                case CommandType.CreateDiscount:
                    returnType = new CreateDiscountCommand(); ;
                    break;
                case CommandType.AddDiscountLineItem:
                    returnType = new AddDiscountLineItemCommand(); ;
                    break;
                case CommandType.ConfirmDiscount:
                    returnType = new ConfirmDiscountCommand(); ;
                    break;
                //Payment note
                case CommandType.CreatePaymentNote:
                    returnType = new CreatePaymentNoteCommand(); ;
                    break;
                case CommandType.AddPaymentNoteLineItem:
                    returnType = new AddPaymentNoteLineItemCommand(); ;
                    break;
                case CommandType.ConfirmPaymentNote:
                    returnType = new ConfirmPaymentNoteCommand();
                    break;
                case CommandType.RetireDocument:
                    returnType = new RetireDocumentCommand();
                    break;
                //CommodityPurchase
                case CommandType.CreateCommodityPurchase:
                    returnType = new CreateCommodityPurchaseCommand(); ;
                    break;
                case CommandType.AddCommodityPurchaseLineItem:
                    returnType = new AddCommodityPurchaseLineItemCommand(); ;
                    break;
                case CommandType.ConfirmCommodityPurchase:
                    returnType = new ConfirmCommodityPurchaseCommand();
                    break;
                //CommodityReception
                case CommandType.CreateCommodityReception:
                    returnType = new CreateCommodityReceptionCommand(); ;
                    break;
                case CommandType.AddCommodityReceptionLineItem:
                    returnType = new AddCommodityReceptionLineItemCommand(); ;
                    break;
                case CommandType.ConfirmCommodityReception:
                    returnType = new ConfirmCommodityReceptionCommand();
                    break;
                case CommandType.StoredCommodityReceptionLineItem:
                    returnType = new StoredCommodityReceptionLineItemCommand();
                    break;
                //CommodityStorage
                case CommandType.CreateCommodityStorage:
                    returnType = new CreateCommodityStorageCommand(); ;
                    break;
                case CommandType.AddCommodityStorageLineItem:
                    returnType = new AddCommodityStorageLineItemCommand(); ;
                    break;
                case CommandType.ConfirmCommodityStorage:
                    returnType = new ConfirmCommodityStorageCommand();
                    break;
                case CommandType.TransferedCommodityStorage:
                    returnType = new TransferedCommodityStorageLineItemCommand();
                    break;
                case CommandType.CreateMainOrder:
                    returnType = new CreateMainOrderCommand();
                    break;
                case CommandType.AddMainOrderLineItem:
                    returnType = new AddMainOrderLineItemCommand();
                    break;
                case CommandType.ConfirmMainOrder:
                    returnType = new ConfirmMainOrderCommand();
                    break;
                case CommandType.ApproveOrderLineItem:
                    returnType = new ApproveOrderLineItemCommand();
                    break;
                case CommandType.ApproveMainOrder:
                    returnType = new ApproveMainOrderCommand();
                    break;
                case CommandType.OrderDispatchApprovedLineItems:
                    returnType = new OrderDispatchApprovedLineItemsCommand();
                    break;
                case CommandType.ChangeMainOrderLineItem:
                    returnType = new ChangeMainOrderLineItemCommand();
                    break;
                case CommandType.RemoveMainOrderLineItem:
                    returnType = new RemoveMainOrderLineItemCommand();
                    break;
                case CommandType.RejectMainOrder:
                    returnType = new RejectMainOrderCommand();
                    break;
                case CommandType.CreateCommodityDelivery:
                    returnType = new CreateCommodityDeliveryCommand();
                    break;
                case CommandType.AddCommodityDeliveryLineItem:
                    returnType = new AddCommodityDeliveryLineItemCommand();
                    break;
                case CommandType.ConfirmCommodityDelivery:
                    returnType = new ConfirmCommodityDeliveryCommand();
                    break;
                case CommandType.WeighedCommodityDeliveryLineItem:
                    returnType = new WeighedCommodityDeliveryLineItemCommand();
                    break;
                    // received delivery
                case CommandType.CreateReceivedDelivery:
                    returnType = new CreateReceivedDeliveryCommand();
                    break;
                case CommandType.AddReceivedDeliveryLineItem:
                    returnType = new AddReceivedDeliveryLineItemCommand();
                    break;
                case CommandType.ConfirmReceivedDelivery:
                    returnType = new ConfirmReceivedDeliveryCommand();
                    break;
                case CommandType.StoredReceivedDeliveryLineItem:
                    returnType = new StoredReceivedDeliveryLineItemCommand();
                    break;
                case CommandType.ApproveDelivery:
                    returnType = new ApproveDeliveryCommand();
                    break;
                case CommandType.ReRouteDocument:
                    returnType = new ReRouteDocumentCommand();
                    break;
                case CommandType.OrderPaymentInfo:
                    returnType = new AddOrderPaymentInfoCommand();
                    break;
                case CommandType.AddExternalDocRef:
                    returnType = new AddExternalDocRefCommand();
                    break;
                case CommandType.ReCollection:
                    returnType = new ReCollectionCommand();
                    break;
                case CommandType.CreateCommodityTransfer:
                    returnType = new CreateCommodityTransferCommand();
                    break;
                case CommandType.AddCommodityTransferLineItem:
                    returnType = new AddCommodityTransferLineItemCommand();
                    break;
                case CommandType.ConfirmCommodityTransfer:
                    returnType = new ConfirmCommodityTransferCommand();
                    break;
                case CommandType.ApproveCommodityTransfer:
                    returnType = new ApproveCommodityTransferCommand();
                    break;
                case CommandType.CreateActivity:
                    returnType = new CreateActivityNoteCommand();
                    break;
                case CommandType.ConfirmActivity:
                    returnType = new ConfirmActivityCommand();
                    break;
                case CommandType.AddActivityInfectionItem:
                    returnType = new AddActivityInfectionLineItemCommand();
                    break;
                case CommandType.AddActivityInputItem:
                    returnType = new AddActivityInputLineItemCommand();
                    break;
                case CommandType.AddActivityProduceItem:
                    returnType = new AddActivityProduceLineItemCommand();
                    break;
                case CommandType.AddActivityServiceItem:
                    returnType = new AddActivityServiceLineItemCommand();
                    break;
                case CommandType.CreateCommodityWarehouseStorage:
                    returnType = new CreateCommodityWarehouseStorageCommand();
                    break;
                case CommandType.AddCommodityWarehouseStorageLineItem:
                    returnType = new AddCommodityWarehouseStorageLineItemCommand();
                    break;

                case CommandType.ConfirmCommodityWarehouseStorage:
                    returnType = new ConfirmCommodityWarehouseStorageCommand();
                    break;
                case CommandType.UpdateCommodityWarehouseStorageLineItem:
                    returnType = new UpdateCommodityWarehouseStorageLineItemCommand();
                    break;
                case CommandType.ApproveCommodityWarehouseStorage:
                    returnType = new ApproveCommodityWarehouseStorageCommand();
                    break;
                case CommandType.StoreCommodityWarehouseStorage:
                    returnType = new StoreCommodityWarehouseStorageCommand();
                    break;
                case CommandType.GenerateReceiptCommodityWarehouseStorage:
                    returnType = new GenerateReceiptCommodityWarehouseStorageCommand();
                    break;
                case CommandType.CreateOutletVisitNote:
                    returnType = new CreateOutletVisitNoteCommand();
                    break;
                case CommandType.CreateCommodityReleaseNote:
                    returnType = new CreateCommodityReleaseCommand();
                    break;
                case CommandType.AddCommodityReleaseNoteLineItem:
                    returnType = new AddCommodityReleaseNoteLineItemCommand();
                    break;
                case CommandType.ConfirmCommodityReleaseNote:
                    returnType = new ConfirmCommodityReleaseNoteCommand();
                    break;
            }

            return returnType;
        }
    }
}
