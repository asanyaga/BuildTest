using System;
using System.Collections.Generic;
using Distributr.Core.CommandHandler.ActivityDocumentHandlers;
using Distributr.Core.CommandHandler.DocumentCommandHandlers;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.AdjustmentNotes;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.CreditNotes;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.DisbursementNotes;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.DispatchNotes;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.InventoryReceivedNotes;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.InventoryTransferNotes;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Invoices;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Losses;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Orders;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.OutletDocument;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Receipts;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Recollections;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.ReturnsNotes;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityDeliveryCommandHandler;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityPurchaseCommandHandler;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityReceptionCommandHandler;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityReleaseCommandHandler;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityStorageCommandHandler;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityTranferCommandHandler;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityWarehouseStorageCommandHandler;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.ReceivedDeliveryCommandHandler;
using Distributr.Core.Data.CommandHandlers.ActivityDocumentHandlers;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.AdjustmentNotes;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.CreditNotes;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.DisbursementNotes;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.DispatchNotes;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.InventoryReceivedNotes;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.InventoryTransferNotes;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Invoices;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Orders;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.OutletDocuments;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.PN;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Receipts;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Recollections;
using Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.ReturnsNotes;
using Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommoditityPurchaseCommandHandlers;
using Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommoditityReleaseCommandHandlers;
using Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommoditityStorageCommandHandlers;
using Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityDeliveryCommandHandlers;
using Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityReceptionCommandHandlers;
using Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityTransferCommandHandlers;
using Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityWarehouseStorageCommandHandlers;
using Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.ReceivedDeliveryCommandHandlers;
using Distributr.Core.Workflow.FinancialWorkflow;
using Distributr.Core.Workflow.FinancialWorkflow.Impl;

namespace Distributr.Core.Data.IOC
{
    /// <summary>
    /// Default command handler services for IOC
    /// </summary>
    public class DefaultCommandHandlers
    {
        public static List<Tuple<Type, Type>> ServiceList()
        {
            var serviceList = new List<Tuple<Type, Type>>
                {
                    
                    Tuple.Create(typeof (ICreateOutletVisitNoteCommandHandler), typeof (CreateOutletVisitNoteCommandHandler)),
                  
                   // Tuple.Create(typeof (ICreateOrderCommandHandler), typeof (CreateOrderCommandHandler)),
                    //Tuple.Create(typeof(IAddOrderLineItemCommandHandler),typeof (AddOrderLineItemCommandHandler)),
                    //Tuple.Create(typeof(IConfirmOrderCommandHandler),typeof (ConfirmOrderCommandHandler)),
                    Tuple.Create(typeof(ICreateInventoryAdjustmentNoteCommandHandler),typeof (CreateInventoryAdjustmentNoteCommandHandler)),
                    Tuple.Create(typeof(IAddInventoryAdjustmentNoteLineItemCommandHandler),typeof (AddInventoryAdjustmentNoteLineItemCommandHandler)),
                    Tuple.Create(typeof(IConfirmInventoryAdjustmentNoteCommandHandler),typeof (ConfirmInventoryAdjustmentNoteCommandHandler)),

                    Tuple.Create(typeof(IAddInventoryTransferNoteLineItemCommandHandler),typeof (AddInventoryTransferNoteLineItemCommandHandler)),
                    Tuple.Create(typeof(ICreateInventoryTransferNoteCommandHandler),typeof (CreateInventoryTransferNoteCommandHandler)),
                    Tuple.Create(typeof(IConfirmInventoryTransferNoteCommandHandler),typeof (ConfirmInventoryTransferNoteCommandHandler)),

                    Tuple.Create(typeof(ICreateCreditNoteCommandHandler),typeof (CreateCreditNoteCommandHandler)),
                    Tuple.Create(typeof(IAddCreditNoteLineItemCommandHandler),typeof (AddCreditNoteLineItemCommandHandler)),
                    Tuple.Create(typeof(IConfirmCreditNoteCommandHandler),typeof (ConfirmCreditNoteCommandHandler)),

                    Tuple.Create(typeof(ICreateReturnsNoteCommandHandler),typeof (CreateReturnsNoteCommandHandler)),
                    Tuple.Create(typeof(IAddReturnsNoteLineItemCommandHandler),typeof (AddReturnsNoteLineItemCommandHandler)),
                    Tuple.Create(typeof(IConfirmReturnsNoteCommandHandler),typeof (ConfirmReturnsNoteCommandHandler)),
                    Tuple.Create(typeof(ICreatePaymentNoteCommandHandler),typeof (CreatePaymentNoteCommandHandler)),
                    Tuple.Create(typeof(IConfirmPaymentNoteCommandHandler),typeof (ConfirmPaymentNoteCommandHandler)),
                    Tuple.Create(typeof(IAddPaymentNoteLineItemCommandHandler),typeof (AddPaymentNoteLineItemCommandHandler)),
                    //Tuple.Create(typeof(IApproveOrderCommandHandler),typeof (ApproveOrderCommandHandler)),
                    Tuple.Create(typeof(ICloseOrderCommandHandler),typeof (CloseOrderCommandHandler)),
                    //Tuple.Create(typeof(IChangeOrderLineItemCommandHandler),typeof (ChangeOrderLineItemCommandHandler)),
                    //Tuple.Create(typeof(IRejectOrderCommandHandler),typeof (RejectOrderCommandHandler)),
                   // Tuple.Create(typeof(IRemoveOrderLineItemCommandHandler),typeof (RemoveOrderLineItemCommandHandler)),
                    Tuple.Create(typeof(IAddDispatchNoteLineItemCommandHandler),typeof (AddDispatchNoteLineItemCommandHandler)),
                    Tuple.Create(typeof(ICreateDispatchNoteCommandHandler),typeof (CreateDispatchNoteCommandHandler)),
                    Tuple.Create(typeof(IConfirmDispatchNoteCommandHandler),typeof (ConfirmDispatchNoteCommandHandler)),
                    Tuple.Create(typeof(IAddInventoryReceivedNoteLineItemCommandHandler),typeof (AddInventoryReceivedNoteLineItemCommandHandler)),
                    Tuple.Create(typeof(ICreateInventoryReceivedNoteCommandHandler),typeof (CreateInventoryReceivedNoteCommandHandler)),
                    Tuple.Create(typeof(IConfirmInventoryReceivedNoteCommandHandler),typeof (ConfirmInventoryReceivedNoteCommandHandler)),
                    Tuple.Create(typeof(ICloseReturnsNoteCommandHandler),typeof (CloseReturnsNoteCommandHandler)),

                    Tuple.Create(typeof(ICreateInvoiceCommandHandler),typeof (CreateInvoiceCommandHandler)),
                    Tuple.Create(typeof(IAddInvoiceLineItemCommandHandler),typeof (AddInvoiceLineItemCommandHandler)),
                    Tuple.Create(typeof(IConfirmInvoiceCommandHandler),typeof (ConfirmInvoiceCommandHandler)),
                    Tuple.Create(typeof(ICreateReceiptCommandHandler),typeof (CreateReceiptCommandHandler)),
                    Tuple.Create(typeof(IAddReceiptLineItemCommandHandler),typeof (AddReceiptLineItemCommandHandler)),
                    Tuple.Create(typeof(IConfirmReceiptCommandHandler),typeof (ConfirmReceiptCommandHandler)),

                    Tuple.Create(typeof(ICreateDisbursementNoteCommandHandler),typeof (CreateDisbursementNoteCommandHandler)),
                    Tuple.Create(typeof(IAddDisbursementNoteLineItemCommandHandler),typeof (AddDisbursementNoteLineItemCommandHandler)),
                    Tuple.Create(typeof(IConfirmDisbursementNoteCommandHandler),typeof (ConfirmDisbursementNoteCommandHandler)),
                    Tuple.Create(typeof(IOrderPendingDispatchCommandHandler),typeof (OrderPendingDispatchCommandHandler)),
                    Tuple.Create(typeof(IOrderDispatchedToPhoneCommandHandler),typeof (OrderDispatchedToPhoneCommandHandler)),
                    Tuple.Create(typeof(IConfirmReceiptLineItemCommandHandler),typeof (ConfirmReceiptLineItemCommandHandler)),
                    Tuple.Create(typeof(IPaymentTrackerWorkflow),typeof (PaymentTrackerWorkflow)),

                    Tuple.Create(typeof(ICreateCommodityPurchaseCommandHandler),typeof (CreateCommodityPurchaseCommandHandler)),
                    Tuple.Create(typeof(IConfirmCommodityPurchaseCommandHandler),typeof (ConfirmCommodityPurchaseCommandHandler)),
                    Tuple.Create(typeof(IAddCommodityPurchaseLineItemCommandHandler),typeof (AddCommodityPurchaseLineItemCommandHandler)),

                    Tuple.Create(typeof(ICreateCommodityReceptionCommandHandler),typeof (CreateCommodityReceptionCommandHandler)),
                    Tuple.Create(typeof(IConfirmCommodityReceptionCommandHandler),typeof (ConfirmCommodityReceptionCommandHandler)),
                    Tuple.Create(typeof(IAddCommodityReceptionLineItemCommandHandler),typeof (AddCommodityReceptionLineItemCommandHandler)),
                    Tuple.Create(typeof(IStoredCommodityReceptionLineItemCommandHandler),
                                    typeof (StoredCommodityReceptionLineItemCommandHandler)),

                    Tuple.Create(typeof(ICreateCommodityStorageCommandHandler),typeof (CreateCommodityStorageCommandHandler)),
                    Tuple.Create(typeof(IConfirmCommodityStorageCommandHandler),typeof (ConfirmCommodityStorageCommandHandler)),
                    Tuple.Create(typeof(IAddCommodityStorageLineItemCommandHandler),typeof (AddCommodityStorageLineItemCommandHandler)),

                    Tuple.Create(typeof(ICreateMainOrderCommandHandler),typeof (CreateMainOrderCommandHandler)),
                    Tuple.Create(typeof(IAddMainOrderLineItemCommandHandler),typeof (AddMainOrderLineItemCommandHandler)),
                    Tuple.Create(typeof(IConfirmMainOrderCommandHandler),typeof (ConfirmMainOrderCommandHandler)),
                    Tuple.Create(typeof(IApproveOrderLineItemCommandHandler),typeof (ApproveOrderLineItemCommandHandler)),
                    Tuple.Create(typeof(IApproveMainOrderCommandHandler),typeof (ApproveMainOrderCommandHandler)),
                    Tuple.Create(typeof(IOrderDispatchApprovedLineItemsCommandHandler),typeof (OrderDispatchApprovedLineItemsCommandHandler)),
                    Tuple.Create(typeof(IChangeMainOrderLineItemCommandHandler),typeof (ChangeMainOrderLineItemCommandHandler)),
                    Tuple.Create(typeof(IRemoveMainOrderLineItemCommandHandler),typeof (RemoveMainOrderLineItemCommandHandler)),
                    Tuple.Create(typeof(IRejectMainOrderCommandHandler),typeof (RejectMainOrderCommandHandler)),
                    Tuple.Create(typeof(IAddExternalDocRefCommandHandler),typeof (AddExternalDocRefCommandHandler)),


                    Tuple.Create(typeof(IAddOrderPaymentInfoCommandHandler),typeof (AddOrderPaymentInfoCommandHandler)),

                    //received delivery document
                    Tuple.Create(typeof(IConfirmReceivedDeliveryCommandHandler),typeof (ConfirmReceivedDeliveryCommandHandler)),
                    Tuple.Create(typeof(ICreateReceivedDeliveryCommandHandler),typeof (CreateReceivedDeliveryCommandHandler)),
                    Tuple.Create(typeof(IAddReceivedDeliveryLineItemCommandHandler),typeof (AddReceivedDeliveryLineItemCommandHandler)),
                    Tuple.Create(typeof(IStoredReceivedDeliveryLineItemCommandHandler),typeof (StoredReceivedDeliveryLineItemCommandHandler)),
                    //commodity delivery document
                    Tuple.Create(typeof(IConfirmCommodityDeliveryCommandHandler),typeof (ConfirmCommodityDeliveryCommandHandler)),
                    Tuple.Create(typeof(ICreateCommodityDeliveryCommandHandler),typeof (CreateCommodityDeliveryCommandHandler)),
                    Tuple.Create(typeof(IAddCommodityDeliveryLineItemCommandHandler),typeof (AddCommodityDeliveryLineItemCommandHandler)),
                    Tuple.Create(typeof(IWeighedCommodityDeliveryLineItemCommandHandler),typeof (WeighedCommodityDeliveryLineItemCommandHandler)),
                    Tuple.Create(typeof(IApproveDeliveryCommandHandler),typeof (ApproveDeliveryCommandHandler)),
                    Tuple.Create(typeof(IReCollectionCommandHandler),typeof (ReCollectionCommandHandler)),

                    //commodity transfer document
                    Tuple.Create(typeof(IConfirmCommodityTransferCommandHandler),typeof (ConfirmCommodityTransferCommandHandler)),
                    Tuple.Create(typeof(ICreateCommodityTransferCommandHandler),typeof (CreateCommodityTransferCommandHandler)),
                    Tuple.Create(typeof(IAddCommodityTransferLineItemCommandHandler),typeof (AddCommodityTransferLineItemCommandHandler)),

					   Tuple.Create(typeof(IConfirmActivityCommandHandler), typeof(ConfirmActivityCommandHandler)),
         Tuple.Create(typeof(ICreateActivityCommandHandler), typeof(CreateActivityCommandHandler)),
          Tuple.Create(typeof(IAddActivityInfectionLineItemCommandHandler), typeof(AddActivityInfectionLineItemCommandHandler)),
           Tuple.Create(typeof(IAddActivityInputLineItemCommandHandler), typeof(AddActivityInputLineItemCommandHandler)),
           Tuple.Create(typeof(IAddActivityServiceLineItemCommandHandler), typeof(AddActivityServiceLineItemCommandHandler)),
           Tuple.Create(typeof(IAddActivityProduceLineItemCommandHandler), typeof(AddActivityProduceLineItemCommandHandler)),

            Tuple.Create(typeof (IAddCommodityWarehouseStorageLineItemCommandHandler), typeof (AddCommodityWarehouseStorageLineItemCommandHandler)),
                     Tuple.Create(typeof (ICreateCommodityWarehouseStorageCommandHandler), typeof (CreateCommodityWarehouseStorageCommandHandler)),
                     Tuple.Create(typeof (IConfirmCommodityWarehouseStorageCommandHandler), typeof (ConfirmCommodityWarehouseStorageCommandHandler)),
                    Tuple.Create(typeof (IUpdateCommodityWarehouseStorageLineItemCommandHandler), typeof (UpdateCommodityWarehouseStorageLineItemCommandHandler)),
					 Tuple.Create(typeof (IApproveCommodityWarehouseStorageCommandHandler), typeof (ApproveCommodityWarehouseStorageCommandHandler)),
                     Tuple.Create(typeof (IStoreCommodityWarehouseStorageCommandHandler), typeof (StoreCommodityWarehouseStorageCommandHandler)),
                     Tuple.Create(typeof (IGenerateReceiptCommodityWarehouseStorageCommandHandler), typeof (GenerateReceiptCommodityWarehouseStorageCommandHandler)),
                     
                     
                     Tuple.Create(typeof (ICreateCommodityReleaseCommandHandler), typeof (CreateCommodityReleaseCommandHandler)),
                     Tuple.Create(typeof (IAddCommodityReleaseLineItemCommandHandler), typeof (AddCommodityReleaseLineItemCommandHandler)),
                     Tuple.Create(typeof (IConfirmCommodityReleaseCommandHandler), typeof (ConfirmCommodityReleaseCommandHandler)),

					
                };
            return serviceList;
        }
    }
}
