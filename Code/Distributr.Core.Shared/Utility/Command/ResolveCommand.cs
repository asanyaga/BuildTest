using System;
using System.Collections.Generic;
using System.Linq;
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
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Retire;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.ReturnsNotes;
using Distributr.Core.CommandHandler.EntityCommandHandlers.Inventory;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityDeliveryCommandHandler;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityPurchaseCommandHandler;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityReceptionCommandHandler;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityReleaseCommandHandler;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityStorageCommandHandler;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityTranferCommandHandler;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityWarehouseStorageCommandHandler;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.ReceivedDeliveryCommandHandler;
using Distributr.Core.Commands;
using Distributr.Core.Commands.ActivityDocumentCommands;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.AdjustmentNotes;
using Distributr.Core.Commands.DocumentCommands.CreditNotes;
using Distributr.Core.Commands.DocumentCommands.DisbursementNotes;
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
using Distributr.Core.Commands.EntityCommands.InventorySerials;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityPurchaseCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityReceptionCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityReleaseCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityStorageCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityTransferCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityWarehouseStorageCommands;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Utility.Command
{
    public class ResolveCommand : IResolveCommand
    {
        public ResolveCommand()
        {
            Init();
        }


        void Init()
        {
            _items = new List<ResolveCommandItem>();
            //----------- Orders --------------------------
            //_items.Add(Create("CreateOrder", CommandType.CreateOrder, typeof(CreateOrderCommand), typeof(ICreateOrderCommandHandler),DocumentType.Order));
            //_items.Add(Create("ApproveOrder", CommandType.ApproveOrder, typeof(ApproveOrderCommand), typeof(IApproveOrderCommandHandler), DocumentType.Order));
            //_items.Add(Create("AddOrderLineItem", CommandType.AddOrderLineItem, typeof(AddOrderLineItemCommand), typeof(IAddOrderLineItemCommandHandler), DocumentType.Order));
           // _items.Add(Create("ConfirmOrder", CommandType.ConfirmOrder, typeof(ConfirmOrderCommand), typeof(IConfirmOrderCommandHandler), DocumentType.Order));
            //_items.Add(Create("RejectOrder", CommandType.RejectOrder, typeof(RejectOrderCommand), typeof(IRejectOrderCommandHandler), DocumentType.Order));
            //_items.Add(Create("ChangeOrderLineItem", CommandType.ChangeOrderLineItem, typeof(ChangeOrderLineItemCommand), typeof(IChangeOrderLineItemCommandHandler), DocumentType.Order));
           // _items.Add(Create("RemoveOrderLineItem", CommandType.RemoveOrderLineItem, typeof(RemoveOrderLineItemCommand), typeof(IRemoveOrderLineItemCommandHandler), DocumentType.Order));
            _items.Add(Create("CloseOrder", CommandType.CloseOrder, typeof(CloseOrderCommand), typeof(ICloseOrderCommandHandler), DocumentType.Order));
            _items.Add(Create("BackOrder", CommandType.BackOrder, typeof(BackOrderCommand), typeof(IBackOrderCommandHandler), DocumentType.Order));
            _items.Add(Create("OrderPendingDispatch", CommandType.OrderPendingDispatch, typeof(OrderPendingDispatchCommand), typeof(IOrderPendingDispatchCommandHandler), DocumentType.Order));
            _items.Add(Create("DispatchToPhone", CommandType.DispatchToPhone, typeof(DispatchToPhoneCommand), typeof(IOrderDispatchedToPhoneCommandHandler), DocumentType.Order));

            // ------------------ IAN --------
            _items.Add(Create("CreateInventoryAdjustmentNote", CommandType.CreateInventoryAdjustmentNote, typeof(CreateInventoryAdjustmentNoteCommand), typeof(ICreateInventoryAdjustmentNoteCommandHandler),DocumentType.InventoryAdjustmentNote));
            _items.Add(Create("AddInventoryAdjustmentNoteLineItem", CommandType.AddInventoryAdjustmentNoteLineItem, typeof(AddInventoryAdjustmentNoteLineItemCommand), typeof(IAddInventoryAdjustmentNoteLineItemCommandHandler),DocumentType.InventoryAdjustmentNote));
            _items.Add(Create("ConfirmInventoryAdjustmentNote", CommandType.ConfirmInventoryAdjustmentNote, typeof(ConfirmInventoryAdjustmentNoteCommand), typeof(IConfirmInventoryAdjustmentNoteCommandHandler),DocumentType.InventoryAdjustmentNote));

            //------------- DN -----------
            _items.Add(Create("CreateDispatchNote", CommandType.CreateDispatchNote, typeof(CreateDispatchNoteCommand), typeof(ICreateDispatchNoteCommandHandler), DocumentType.DispatchNote));
            _items.Add(Create("AddDispatchNoteLineItem", CommandType.AddDispatchNoteLineItem, typeof(AddDispatchNoteLineItemCommand), typeof(IAddDispatchNoteLineItemCommandHandler), DocumentType.DispatchNote));
            _items.Add(Create("ConfirmDispatchNote", CommandType.ConfirmDispatchNote, typeof(ConfirmDispatchNoteCommand), typeof(IConfirmDispatchNoteCommandHandler), DocumentType.DispatchNote));

            //------------ IRN ----------
            _items.Add(Create("CreateInventoryReceivedNote", CommandType.CreateInventoryReceivedNote, typeof(CreateInventoryReceivedNoteCommand), typeof(ICreateInventoryReceivedNoteCommandHandler), DocumentType.InventoryReceivedNote));
            _items.Add(Create("AddInventoryReceivedNoteLineItem", CommandType.AddInventoryReceivedNoteLineItem, typeof(AddInventoryReceivedNoteLineItemCommand), typeof(IAddInventoryReceivedNoteLineItemCommandHandler), DocumentType.InventoryReceivedNote));
            _items.Add(Create("ConfirmInventoryReceivedNote", CommandType.ConfirmInventoryReceivedNote, typeof(ConfirmInventoryReceivedNoteCommand), typeof(IConfirmInventoryReceivedNoteCommandHandler), DocumentType.InventoryReceivedNote));

            //--------- ITN -----------
            _items.Add(Create("CreateInventoryTransferNote", CommandType.CreateInventoryTransferNote, typeof(CreateInventoryTransferNoteCommand), typeof(ICreateInventoryTransferNoteCommandHandler),DocumentType.InventoryTransferNote));
            _items.Add(Create("AddInventoryTransferNoteLineItem", CommandType.AddInventoryTransferNoteLineItem, typeof(AddInventoryTransferNoteLineItemCommand), typeof(IAddInventoryTransferNoteLineItemCommandHandler),DocumentType.InventoryTransferNote));
            _items.Add(Create("ConfirmInventoryTransferNote", CommandType.ConfirmInventoryTransferNote, typeof(ConfirmInventoryTransferNoteCommand), typeof(IConfirmInventoryTransferNoteCommandHandler),DocumentType.InventoryTransferNote));

            //--------- Invoice ----------
            _items.Add(Create("CreateInvoice", CommandType.CreateInvoice, typeof(CreateInvoiceCommand), typeof(ICreateInvoiceCommandHandler),DocumentType.Invoice));
            _items.Add(Create("AddInvoiceLineItem", CommandType.AddInvoiceLineItem, typeof(AddInvoiceLineItemCommand), typeof(IAddInvoiceLineItemCommandHandler),DocumentType.Invoice));
            _items.Add(Create("ConfirmInvoice", CommandType.ConfirmInvoice, typeof(ConfirmInvoiceCommand), typeof(IConfirmInvoiceCommandHandler),DocumentType.Invoice));
            _items.Add(Create("CloseInvoice", CommandType.CloseInvoice, typeof(CloseInvoiceCommand), typeof(ICloseInvoiceCommandHandler),DocumentType.Invoice));
       
            //--------- Receipt ------------------
            _items.Add(Create("CreateReceipt", CommandType.CreateReceipt, typeof (CreateReceiptCommand),typeof (ICreateReceiptCommandHandler), DocumentType.Receipt));
            _items.Add(Create("AddReceiptLineItem", CommandType.AddReceiptLineItem, typeof(AddReceiptLineItemCommand), typeof(IAddReceiptLineItemCommandHandler), DocumentType.Receipt));
            _items.Add(Create("ConfirmReceiptLineItem", CommandType.ConfirmReceiptLineItem, typeof (ConfirmReceiptLineItemCommand), typeof (IConfirmReceiptLineItemCommandHandler), DocumentType.Receipt));
            _items.Add(Create("ConfirmReceipt", CommandType.ConfirmReceipt, typeof(ConfirmReceiptCommand), typeof(IConfirmReceiptCommandHandler), DocumentType.Receipt));

            //--------- DisbursementNote ------------------
            _items.Add(Create("CreateDisbursementNote", CommandType.CreateDisbursementNote, typeof(CreateDisbursementNoteCommand), typeof(ICreateDisbursementNoteCommandHandler), DocumentType.DisbursementNote));
            _items.Add(Create("AddDisbursementNoteLineItem", CommandType.AddDisbursementNoteLineItem, typeof(AddDisbursementNoteLineItemCommand), typeof(IAddDisbursementNoteLineItemCommandHandler), DocumentType.DisbursementNote));
            _items.Add(Create("ConfirmDisbursementNote", CommandType.ConfirmDisbursementNote, typeof(ConfirmDisbursementNoteCommand), typeof(IConfirmDisbursementNoteCommandHandler), DocumentType.DisbursementNote));

            //--------- Credit Note ------------------
            _items.Add(Create("CreateCreditNote", CommandType.CreateCreditNote, typeof(CreateCreditNoteCommand), typeof(ICreateCreditNoteCommandHandler), DocumentType.CreditNote));
            _items.Add(Create("AddCreditNoteLineItem", CommandType.AddCreditNoteLineItem, typeof(AddCreditNoteLineItemCommand), typeof(IAddCreditNoteLineItemCommandHandler), DocumentType.CreditNote));
            _items.Add(Create("ConfirmCreditNote", CommandType.ConfirmCreditNote, typeof(ConfirmCreditNoteCommand), typeof(IConfirmCreditNoteCommandHandler), DocumentType.CreditNote));

            //--------- CreateReturnsNote ------------------
            _items.Add(Create("CreateReturnsNote", CommandType.CreateReturnsNote, typeof(CreateReturnsNoteCommand), typeof(ICreateReturnsNoteCommandHandler), DocumentType.ReturnsNote));
            _items.Add(Create("AddReturnsNoteLineItem", CommandType.AddReturnsNoteLineItem, typeof(AddReturnsNoteLineItemCommand), typeof(IAddReturnsNoteLineItemCommandHandler), DocumentType.ReturnsNote));
            _items.Add(Create("ConfirmReturnsNote", CommandType.ConfirmReturnsNote, typeof(ConfirmReturnsNoteCommand), typeof(IConfirmReturnsNoteCommandHandler), DocumentType.ReturnsNote));
            _items.Add(Create("CloseReturnsNote", CommandType.CloseReturnsNote, typeof(CloseReturnsNoteCommand), typeof(ICloseReturnsNoteCommandHandler), DocumentType.ReturnsNote));

            //--------- PaymentNote ------------------
            _items.Add(Create("CreatePaymentNote", CommandType.CreatePaymentNote, typeof(CreatePaymentNoteCommand), typeof(ICreatePaymentNoteCommandHandler), DocumentType.PaymentNote));
            _items.Add(Create("AddPaymentNoteLineItem", CommandType.AddPaymentNoteLineItem, typeof(AddPaymentNoteLineItemCommand), typeof(IAddPaymentNoteLineItemCommandHandler), DocumentType.PaymentNote));
            _items.Add(Create("ConfirmPaymentNote", CommandType.ConfirmPaymentNote, typeof(ConfirmPaymentNoteCommand), typeof(IConfirmPaymentNoteCommandHandler), DocumentType.PaymentNote));

             //--------- Retire Document ------------------
            _items.Add(Create("RetireDocument", CommandType.RetireDocument, typeof(RetireDocumentCommand), typeof(IRetireDocumentCommandHandler), DocumentType.RetirePlaceholder));

            _items.Add(Create("CreateInventorySerials", CommandType.CreateInventorySerials, typeof(CreateInventorySerialsCommand), typeof(ICreateInventorySerialsCommandHandler), DocumentType.CreateInventorySerialsPlaceholder));

            //--------- CommodityPurchase ------------------
            _items.Add(Create("CreateCommodityPurchase", CommandType.CreateCommodityPurchase, typeof(CreateCommodityPurchaseCommand), typeof(ICreateCommodityPurchaseCommandHandler), DocumentType.CommodityPurchaseNote));
            _items.Add(Create("AddCommodityPurchaseLineItem", CommandType.AddCommodityPurchaseLineItem, typeof(AddCommodityPurchaseLineItemCommand), typeof(IAddCommodityPurchaseLineItemCommandHandler), DocumentType.CommodityPurchaseNote));
            _items.Add(Create("ConfirmCommodityPurchase", CommandType.ConfirmCommodityPurchase, typeof(ConfirmCommodityPurchaseCommand), typeof(IConfirmCommodityPurchaseCommandHandler), DocumentType.CommodityPurchaseNote));

            //--------- CommodityReception ------------------
            _items.Add(Create("CreateCommodityReception", CommandType.CreateCommodityReception, typeof(CreateCommodityReceptionCommand), typeof(ICreateCommodityReceptionCommandHandler), DocumentType.CommodityReceptionNote));
            _items.Add(Create("AddCommodityReceptionLineItem", CommandType.AddCommodityReceptionLineItem, typeof(AddCommodityReceptionLineItemCommand), typeof(IAddCommodityReceptionLineItemCommandHandler), DocumentType.CommodityReceptionNote));
            _items.Add(Create("ConfirmCommodityReception", CommandType.ConfirmCommodityReception, typeof(ConfirmCommodityReceptionCommand), typeof(IConfirmCommodityReceptionCommandHandler), DocumentType.CommodityReceptionNote));
            _items.Add(Create("StoredCommodityReceptionLineItem", CommandType.StoredCommodityReceptionLineItem, typeof(StoredCommodityReceptionLineItemCommand), typeof(IStoredCommodityReceptionLineItemCommandHandler), DocumentType.CommodityReceptionNote));

            //--------- CommodityStorage ------------------
            _items.Add(Create("CreateCommodityStorage", CommandType.CreateCommodityStorage, typeof(CreateCommodityStorageCommand), typeof(ICreateCommodityStorageCommandHandler), DocumentType.CommodityStorageNote));
            _items.Add(Create("AddCommodityStorageLineItem", CommandType.AddCommodityStorageLineItem, typeof(AddCommodityStorageLineItemCommand), typeof(IAddCommodityStorageLineItemCommandHandler), DocumentType.CommodityStorageNote));
            _items.Add(Create("ConfirmCommodityStorage", CommandType.ConfirmCommodityStorage, typeof(ConfirmCommodityStorageCommand), typeof(IConfirmCommodityStorageCommandHandler), DocumentType.CommodityStorageNote));
            _items.Add(Create("TransferedCommodityStorage", CommandType.TransferedCommodityStorage, typeof(TransferedCommodityStorageLineItemCommand), typeof(ITransferedCommodityStorageCommandHandler), DocumentType.CommodityStorageNote));

            //----------Main order-------------------
            _items.Add(Create("CreateMainOrder", CommandType.CreateMainOrder, typeof(CreateMainOrderCommand), typeof(ICreateMainOrderCommandHandler), DocumentType.Order));
            _items.Add(Create("AddMainOrderLineItem", CommandType.AddMainOrderLineItem, typeof(AddMainOrderLineItemCommand), typeof(IAddMainOrderLineItemCommandHandler), DocumentType.Order));
            _items.Add(Create("ConfirmMainOrder", CommandType.ConfirmMainOrder, typeof(ConfirmMainOrderCommand), typeof(IConfirmMainOrderCommandHandler), DocumentType.Order));
            _items.Add(Create("ApproveMainOrder", CommandType.ApproveMainOrder, typeof(ApproveMainOrderCommand), typeof(IApproveMainOrderCommandHandler), DocumentType.Order));
            _items.Add(Create("ApproveOrderLineItem", CommandType.ApproveOrderLineItem, typeof(ApproveOrderLineItemCommand), typeof(IApproveOrderLineItemCommandHandler), DocumentType.Order));
            _items.Add(Create("OrderDispatchApprovedLineItems", CommandType.OrderDispatchApprovedLineItems, typeof(OrderDispatchApprovedLineItemsCommand), typeof(IOrderDispatchApprovedLineItemsCommandHandler), DocumentType.Order));
            _items.Add(Create("ChangeMainOrderLineItem", CommandType.ChangeMainOrderLineItem, typeof(ChangeMainOrderLineItemCommand), typeof(IChangeMainOrderLineItemCommandHandler), DocumentType.Order));
            _items.Add(Create("RemoveMainOrderLineItem", CommandType.RemoveMainOrderLineItem, typeof(RemoveMainOrderLineItemCommand), typeof(IRemoveMainOrderLineItemCommandHandler), DocumentType.Order));
            _items.Add(Create("RejectMainOrder", CommandType.RejectMainOrder, typeof(RejectMainOrderCommand), typeof(IRejectMainOrderCommandHandler), DocumentType.Order));
            _items.Add(Create("OrderPaymentInfo", CommandType.OrderPaymentInfo, typeof(AddOrderPaymentInfoCommand), typeof(IAddOrderPaymentInfoCommandHandler), DocumentType.Order));
            _items.Add(Create("AddExternalDocRef", CommandType.AddExternalDocRef, typeof(AddExternalDocRefCommand), typeof(IAddExternalDocRefCommandHandler), DocumentType.Order));



            //--------- CommodityDelivery ------------------
            _items.Add(Create("CreateCommodityDelivery", CommandType.CreateCommodityDelivery, typeof(CreateCommodityDeliveryCommand), typeof(ICreateCommodityDeliveryCommandHandler), DocumentType.CommodityDelivery));
            _items.Add(Create("AddCommodityDeliveryLineItem", CommandType.AddCommodityDeliveryLineItem, typeof(AddCommodityDeliveryLineItemCommand), typeof(IAddCommodityDeliveryLineItemCommandHandler), DocumentType.CommodityDelivery));
            _items.Add(Create("ConfirmCommodityDelivery", CommandType.ConfirmCommodityDelivery, typeof(ConfirmCommodityDeliveryCommand), typeof(IConfirmCommodityDeliveryCommandHandler), DocumentType.CommodityDelivery));
            _items.Add(Create("WeighedCommodityDeliveryLineItem", CommandType.WeighedCommodityDeliveryLineItem, typeof(WeighedCommodityDeliveryLineItemCommand), typeof(IWeighedCommodityDeliveryLineItemCommandHandler), DocumentType.CommodityDelivery));
            
            //--------- CreateReceivedDelivery------------------
            _items.Add(Create("CreateReceivedDelivery", CommandType.CreateReceivedDelivery, typeof(CreateReceivedDeliveryCommand), typeof(ICreateReceivedDeliveryCommandHandler), DocumentType.ReceivedDelivery));
            _items.Add(Create("AddReceivedDeliveryLineItem", CommandType.AddReceivedDeliveryLineItem, typeof(AddReceivedDeliveryLineItemCommand), typeof(IAddReceivedDeliveryLineItemCommandHandler), DocumentType.ReceivedDelivery));
            _items.Add(Create("ConfirmReceivedDelivery", CommandType.ConfirmReceivedDelivery, typeof(ConfirmReceivedDeliveryCommand), typeof(IConfirmReceivedDeliveryCommandHandler), DocumentType.ReceivedDelivery));
            _items.Add(Create("ApproveDelivery", CommandType.ApproveDelivery, typeof(ApproveDeliveryCommand), typeof(IApproveDeliveryCommandHandler), DocumentType.ReceivedDelivery));
            _items.Add(Create("StoredReceivedDeliveryLineItem", CommandType.StoredReceivedDeliveryLineItem, typeof(StoredReceivedDeliveryLineItemCommand), typeof(IStoredReceivedDeliveryLineItemCommandHandler), DocumentType.ReceivedDelivery));
            _items.Add(Create("ReCollection", CommandType.ReCollection, typeof(ReCollectionCommand), typeof(IReCollectionCommandHandler), DocumentType.ReceivedDelivery));

            //---------Commodity Inventory Transfer ----------------------
            _items.Add(Create("CreateCommodityTransfer", CommandType.CreateCommodityTransfer, typeof(CreateCommodityTransferCommand), typeof(ICreateCommodityTransferCommandHandler), DocumentType.CommodityTransferNote));
            _items.Add(Create("AddCommodityTransferLineItem", CommandType.AddCommodityTransferLineItem, typeof(AddCommodityTransferLineItemCommand), typeof(IAddCommodityTransferLineItemCommandHandler), DocumentType.CommodityTransferNote));
            _items.Add(Create("ConfirmCommodityTransfer", CommandType.ConfirmCommodityTransfer, typeof(ConfirmCommodityTransferCommand), typeof(IConfirmCommodityTransferCommandHandler), DocumentType.CommodityTransferNote));
            _items.Add(Create("ApproveCommodityTransfer", CommandType.ApproveCommodityTransfer, typeof(ApproveCommodityTransferCommand), typeof(IApproveCommodityTransferCommandHandler), DocumentType.CommodityTransferNote));
            
            // system command
            _items.Add(Create("ReRouteDocument", CommandType.ReRouteDocument, typeof(ReRouteDocumentCommand), null, DocumentType.SystemCommandPlaceholder));

            //activity command

            _items.Add(Create("ConfirmActivity", CommandType.ConfirmActivity, typeof(ConfirmActivityCommand), typeof(IConfirmActivityCommandHandler), DocumentType.ActivityNote));
            _items.Add(Create("CreateActivity", CommandType.CreateActivity, typeof(CreateActivityNoteCommand), typeof(ICreateActivityCommandHandler), DocumentType.ActivityNote));
            _items.Add(Create("AddActivityInfectionItem", CommandType.AddActivityInfectionItem, typeof(AddActivityInfectionLineItemCommand), typeof(IAddActivityInfectionLineItemCommandHandler), DocumentType.ActivityNote));
            _items.Add(Create("AddActivityInputItem", CommandType.AddActivityInputItem, typeof(AddActivityInputLineItemCommand), typeof(IAddActivityInputLineItemCommandHandler), DocumentType.ActivityNote));
            _items.Add(Create("AddActivityServiceItem", CommandType.AddActivityServiceItem, typeof(AddActivityServiceLineItemCommand), typeof(IAddActivityServiceLineItemCommandHandler), DocumentType.ActivityNote));
            _items.Add(Create("AddActivityProduceItem", CommandType.AddActivityProduceItem, typeof(AddActivityProduceLineItemCommand), typeof(IAddActivityProduceLineItemCommandHandler), DocumentType.ActivityNote));

            //Commodity Warehouse Storage
            _items.Add(Create("AddCommodityWarehouseStorageLineItem", CommandType.AddCommodityWarehouseStorageLineItem, typeof(AddCommodityWarehouseStorageLineItemCommand), typeof(IAddCommodityWarehouseStorageLineItemCommandHandler), DocumentType.CommodityWarehouseStorage));
            _items.Add(Create("CreateCommodityWarehouseStorage", CommandType.CreateCommodityWarehouseStorage, typeof(CreateCommodityWarehouseStorageCommand), typeof(ICreateCommodityWarehouseStorageCommandHandler), DocumentType.CommodityWarehouseStorage));
            _items.Add(Create("ConfirmCommodityWarehouseStorage", CommandType.ConfirmCommodityWarehouseStorage, typeof(ConfirmCommodityWarehouseStorageCommand), typeof(IConfirmCommodityWarehouseStorageCommandHandler), DocumentType.CommodityWarehouseStorage));
            _items.Add(Create("ApproveCommodityWarehouseStorage", CommandType.ApproveCommodityWarehouseStorage, typeof(ApproveCommodityWarehouseStorageCommand), typeof(IApproveCommodityWarehouseStorageCommandHandler), DocumentType.CommodityWarehouseStorage));
            _items.Add(Create("StoreCommodityWarehouseStorage", CommandType.StoreCommodityWarehouseStorage, typeof(StoreCommodityWarehouseStorageCommand), typeof(IStoreCommodityWarehouseStorageCommandHandler), DocumentType.CommodityWarehouseStorage));
            _items.Add(Create("UpdateCommodityWarehouseStorageLineItem", CommandType.UpdateCommodityWarehouseStorageLineItem, typeof(UpdateCommodityWarehouseStorageLineItemCommand), typeof(IUpdateCommodityWarehouseStorageLineItemCommandHandler), DocumentType.CommodityWarehouseStorage));
            _items.Add(Create("GenerateReceiptCommodityWarehouseStorage", CommandType.GenerateReceiptCommodityWarehouseStorage, typeof(GenerateReceiptCommodityWarehouseStorageCommand), typeof(IGenerateReceiptCommodityWarehouseStorageCommandHandler), DocumentType.CommodityWarehouseStorage));

            _items.Add(Create("CreateOutletVisitNote", CommandType.CreateOutletVisitNote, typeof(CreateOutletVisitNoteCommand), typeof(ICreateOutletVisitNoteCommandHandler), DocumentType.OutletVisitNote));

            //---------Commodity Release ----------------------
            _items.Add(Create("CreateCommodityReleaseNote", CommandType.CreateCommodityReleaseNote, typeof(CreateCommodityReleaseCommand), typeof(ICreateCommodityReleaseCommandHandler), DocumentType.CommodityRelease));
            _items.Add(Create("AddCommodityReleaseNoteLineItem", CommandType.AddCommodityReleaseNoteLineItem, typeof(AddCommodityReleaseNoteLineItemCommand), typeof(IAddCommodityReleaseLineItemCommandHandler), DocumentType.CommodityRelease));
            _items.Add(Create("ConfirmCommodityReleaseNote", CommandType.ConfirmCommodityReleaseNote, typeof(ConfirmCommodityReleaseNoteCommand), typeof(IConfirmCommodityReleaseCommandHandler), DocumentType.CommodityRelease));
            
       
        

        }


        ResolveCommandItem Create (string commandName, CommandType commandType, Type command, Type commandHandler, DocumentType documentType)
        {
            return new ResolveCommandItem { CommandName = commandName, CommandType = commandType, Command = command, CommandHandlerContract = commandHandler, DocumentType = documentType};
        }

        public ResolveCommandItem Get(string commandName)
        {
            return _items.FirstOrDefault(n => n.CommandName == commandName);
        }

        public ResolveCommandItem Get(CommandType commandType)
        {
            return _items.FirstOrDefault(n => n.CommandType == commandType);
        }

        public ResolveCommandItem Get(ICommand command)
        {
            ResolveCommandItem rv = null;
            foreach(var item in _items)
            {
                if (command.GetType() == item.Command)
                    rv = item;
            }
            return rv;
        }


        private List<ResolveCommandItem> _items;
       
    }
}
