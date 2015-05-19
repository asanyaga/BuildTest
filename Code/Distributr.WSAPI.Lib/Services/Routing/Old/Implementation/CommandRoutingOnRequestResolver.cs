using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Repository.Transactional.DocumentRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.Core.Utility;
using Distributr.WSAPI.Lib.Services.Bus.Impl;
using Newtonsoft.Json;

namespace Distributr.WSAPI.Lib.Services.Routing.Implementation
{
    public class CommandRoutingOnRequestResolver : ICommandRoutingOnRequestResolver
    {
        ICommandRoutingOnRequestRepository _commandRoutingOnRequestRepository;
        private IGenericSourceDocumentRepository _sourcingDocumentRepository;
        private IInventoryTransferNoteRepository _inventoryTransferNoteRepository;
        private IOrderRepository _orderRepository;

        private IGenericDocumentRepository _genericDocumentRepository;

        public CommandRoutingOnRequestResolver(ICommandRoutingOnRequestRepository commandRoutingOnRequestRepository, IGenericSourceDocumentRepository sourcingDocumentRepository, IInventoryTransferNoteRepository inventoryTransferNoteRepository, IOrderRepository orderRepository, IGenericDocumentRepository genericDocumentRepository)
        {
            _commandRoutingOnRequestRepository = commandRoutingOnRequestRepository;
            _sourcingDocumentRepository = sourcingDocumentRepository;
            _inventoryTransferNoteRepository = inventoryTransferNoteRepository;
            _orderRepository = orderRepository;
            _genericDocumentRepository = genericDocumentRepository;
        }


        public CommandRouteOnRequestDTO GetCommand(ICommand command, CommandType commandType)
        {

            //has the command already been resolved??
            if (_commandRoutingOnRequestRepository.GetByCommandId(command.CommandId) != null)
            {
                return null;

            }
           
            //string commandType = command.GetType().ToString().Split('.').Last();
            CommandRouteOnRequestDTO  commandRoute = null;

            switch (commandType)
            {

               
                //-------------------------- InvoiceNote -------------------------

                case CommandType.CreateInvoice: // "CreateInvoiceCommand":
                    CreateInvoiceCommand createInvoiceCommand = command as CreateInvoiceCommand;
                    commandRoute = GetInvoiceCommandRoutes(createInvoiceCommand,commandType);
                    break;
                case CommandType.AddInvoiceLineItem: // "AddInvoiceLineItemCommand":
                    AddInvoiceLineItemCommand addInvoiceLineItemCommand = command as AddInvoiceLineItemCommand;
                    commandRoute = GetInvoiceCommandRoutes(addInvoiceLineItemCommand,commandType);
                    break;

                case CommandType.ConfirmInvoice: // "ConfirmInvoiceCommand":
                    ConfirmInvoiceCommand confirmInvoiceCommand = command as ConfirmInvoiceCommand;
                    commandRoute = GetInvoiceCommandRoutes(confirmInvoiceCommand,commandType);
                    break;

                case CommandType.CloseInvoice: // "CloseInvoiceCommand":
                    CloseInvoiceCommand closeInvoiceCommand = command as CloseInvoiceCommand;
                    commandRoute = GetInvoiceCommandRoutes(closeInvoiceCommand,commandType);
                    break;
                // --------------- End Invoice -----------------------------------


                // --------------- Inventory Adjustment Note -----------------------------------

                case CommandType.CreateInventoryAdjustmentNote: // "CreateInventoryAdjustmentNoteCommand":
                    CreateInventoryAdjustmentNoteCommand createInventoryAdjustmentNoteCommand = command as CreateInventoryAdjustmentNoteCommand;
                    commandRoute = GetInventoryAdjustmentNoteCommandRoutes(createInventoryAdjustmentNoteCommand,commandType);
                    break;
                case CommandType.AddInventoryAdjustmentNoteLineItem: // "AddInventoryAdjustmentNoteLineItemCommand":
                    AddInventoryAdjustmentNoteLineItemCommand addInventoryAdjustmentNoteLineItemCommand = command as AddInventoryAdjustmentNoteLineItemCommand;
                    commandRoute = GetInventoryAdjustmentNoteCommandRoutes(addInventoryAdjustmentNoteLineItemCommand,commandType);
                    break;
                case CommandType.ConfirmInventoryAdjustmentNote: // "ConfirmInventoryAdjustmentNoteCommand":
                    ConfirmInventoryAdjustmentNoteCommand confirInventoryAdjustmentNoteCommand = command as ConfirmInventoryAdjustmentNoteCommand;
                    commandRoute = GetInventoryAdjustmentNoteCommandRoutes(confirInventoryAdjustmentNoteCommand,commandType);
                    break;
                // ---------------End  Inventory Adjustment Note -----------------------------------


                //-------------- ORDERS -----------------------

                //case CommandType.CreateOrder: // "CreateOrderCommand":
                //    CreateOrderCommand createOrderCommand = command as CreateOrderCommand;
                //    commandRoute = GetOrderCommandRoutes(createOrderCommand,commandType);
                //    break;
                //case CommandType.AddOrderLineItem: // "AddOrderLineItemCommand":
                //    AddOrderLineItemCommand addLineItemCommand = command as AddOrderLineItemCommand;
                //    commandRoute = GetOrderCommandRoutes(addLineItemCommand,commandType);
                //    break;
                //case CommandType.ConfirmOrder: // "ConfirmOrderCommand":
                //    ConfirmOrderCommand confirmOrderCommand = command as ConfirmOrderCommand;
                //    commandRoute = GetOrderCommandRoutes(confirmOrderCommand,commandType);
                //    break;
                case CommandType.RemoveOrderLineItem: // "RemoveOrderLineItemCommand":
                    RemoveOrderLineItemCommand c1 = command as RemoveOrderLineItemCommand;
                    commandRoute = GetOrderCommandRoutes(c1,commandType);
                    break;
                case CommandType.ChangeOrderLineItem: // "ChangeOrderLineItemCommand":
                    ChangeOrderLineItemCommand c2 = command as ChangeOrderLineItemCommand;
                    commandRoute = GetOrderCommandRoutes(c2,commandType);
                    break;
                //case CommandType.ApproveOrder:// "ApproveOrderCommand":
                //    ApproveOrderCommand c3 = command as ApproveOrderCommand;
                //    commandRoute = GetOrderCommandRoutes(c3,commandType);
                //    break;
                case CommandType.CloseOrder: // "CloseOrderCommand":
                    CloseOrderCommand c4 = command as CloseOrderCommand;
                    commandRoute = GetOrderCommandRoutes(c4,commandType);
                    break;
                case CommandType.BackOrder: // "BackOrderCommand":
                    BackOrderCommand c5 = command as BackOrderCommand;
                    commandRoute = GetOrderCommandRoutes(c5,commandType);
                    break;
                case CommandType.OrderPendingDispatch: // "OrderPendingDispatchCommand":
                    OrderPendingDispatchCommand c6 = command as OrderPendingDispatchCommand;
                    commandRoute = GetOrderCommandRoutes(c6,commandType);
                    break;
                case CommandType.DispatchToPhone:// "DispatchToPhoneCommand":
                    DispatchToPhoneCommand c7 = command as DispatchToPhoneCommand;
                    commandRoute = GetOrderCommandRoutes(c7,commandType);
                    break;
                case CommandType.RejectOrder: // "RejectOrderCommand":
                    RejectOrderCommand c8 = command as RejectOrderCommand;
                    commandRoute = GetOrderCommandRoutes(c8,commandType);
                    break;
                //--------------End  ORDERS -----------------------

                // ------------ Inventory Received Note -----------------------

                case CommandType.CreateInventoryReceivedNote: // "CreateInventoryReceivedNoteCommand":
                    CreateInventoryReceivedNoteCommand c14 = command as CreateInventoryReceivedNoteCommand;
                    commandRoute = GetInventoryReceivedNoteCommandRoutes(c14,commandType);
                    break;
                case CommandType.AddInventoryReceivedNoteLineItem:// "AddInventoryReceivedNoteLineItemCommand":
                    AddInventoryReceivedNoteLineItemCommand c15 = command as AddInventoryReceivedNoteLineItemCommand;
                    commandRoute = GetInventoryReceivedNoteCommandRoutes(c15,commandType);
                    break;
                case CommandType.ConfirmInventoryReceivedNote: // "ConfirmInventoryReceivedNoteCommand":
                    ConfirmInventoryReceivedNoteCommand c16 = command as ConfirmInventoryReceivedNoteCommand;
                    commandRoute = GetInventoryReceivedNoteCommandRoutes(c16,commandType);
                    break;
                // ------------End Inventory Received Note -----------------------

                //------------- Inventory Transfer Note ------------------------------------------------------

                case CommandType.CreateInventoryTransferNote: // "CreateInventoryTransferNoteCommand":
                    CreateInventoryTransferNoteCommand createInventoryTransferNoteCommand = command as CreateInventoryTransferNoteCommand;
                    commandRoute = GetInventoryTransferNoteCommandRoutes(createInventoryTransferNoteCommand,commandType);
                    break;
                case CommandType.AddInventoryTransferNoteLineItem: // "AddInventoryTransferNoteLineItemCommand":
                    AddInventoryTransferNoteLineItemCommand addInventoryTransferNoteLineItemCommand = command as AddInventoryTransferNoteLineItemCommand;
                    commandRoute = GetInventoryTransferNoteCommandRoutes(addInventoryTransferNoteLineItemCommand,commandType);
                    break;
                case CommandType.ConfirmInventoryTransferNote: // "ConfirmInventoryTransferNoteCommand":
                    ConfirmInventoryTransferNoteCommand confirInventoryTransferNoteCommand = command as ConfirmInventoryTransferNoteCommand;
                    commandRoute = GetInventoryTransferNoteCommandRoutes(confirInventoryTransferNoteCommand,commandType);
                    break;
                //------------- End Inventory Transfer Note ------------------------------------------------------

                // -------------- Dispatch Note -----------------------------------

                case CommandType.CreateDispatchNote:// "CreateDispatchNoteCommand":
                    CreateDispatchNoteCommand createDispatchNoteCommand = command as CreateDispatchNoteCommand;
                    commandRoute = GetDispatchNoteCommandRoutes(createDispatchNoteCommand,commandType);
                    break;
                case CommandType.AddDispatchNoteLineItem: // "AddDispatchNoteLineItemCommand":
                    AddDispatchNoteLineItemCommand addDispatchNoteLineItemCommand = command as AddDispatchNoteLineItemCommand;
                    commandRoute = GetDispatchNoteCommandRoutes(addDispatchNoteLineItemCommand,commandType);
                    break;
                case CommandType.ConfirmDispatchNote: // "ConfirmDispatchNoteCommand":
                    ConfirmDispatchNoteCommand confirDispatchNoteCommand = command as ConfirmDispatchNoteCommand;
                    commandRoute = GetDispatchNoteCommandRoutes(confirDispatchNoteCommand,commandType);
                    break;
                // -------------- End Dispatch Note -----------------------------------
                //-------------------------- Receipt -------------------------

                case CommandType.CreateReceipt: // "CreateReceiptCommand":
                    CreateReceiptCommand createReceiptCommand = command as CreateReceiptCommand;
                    commandRoute = GetReceiptCommandRoutes(createReceiptCommand,commandType);
                    break;

                case CommandType.AddReceiptLineItem: // "AddReceiptLineItemCommand":
                    AddReceiptLineItemCommand addReceiptLineItemCommand = command as AddReceiptLineItemCommand;
                    commandRoute = GetReceiptCommandRoutes(addReceiptLineItemCommand,commandType);
                    break;

                case CommandType.ConfirmReceipt: // "ConfirmReceiptCommand":
                    ConfirmReceiptCommand confirmReceiptCommand = command as ConfirmReceiptCommand;
                    commandRoute = GetReceiptCommandRoutes(confirmReceiptCommand,commandType);
                    break;

                case CommandType.ConfirmReceiptLineItem: //  "ConfirmReceiptLineItemCommand":
                    ConfirmReceiptLineItemCommand confirmReceiptLineItemCommand = command as ConfirmReceiptLineItemCommand;
                    commandRoute = GetReceiptCommandRoutes(confirmReceiptLineItemCommand,commandType);
                    break;

                //-------------------------- DisbursementNote -------------------------

                case CommandType.CreateDisbursementNote: // "CreateDisbursementNoteCommand":
                    CreateDisbursementNoteCommand createDisbursementNoteCommand = command as CreateDisbursementNoteCommand;
                    commandRoute = GetDisbursementNoteCommandRoutes(createDisbursementNoteCommand,commandType);
                    break;

                case CommandType.AddDisbursementNoteLineItem: // "AddDisbursementNoteLineItemCommand":
                    AddDisbursementNoteLineItemCommand addDisbursementNoteLineItemCommand = command as AddDisbursementNoteLineItemCommand;
                    commandRoute = GetDisbursementNoteCommandRoutes(addDisbursementNoteLineItemCommand,commandType);
                    break;

                case CommandType.ConfirmDisbursementNote: // "ConfirmDisbursementNoteCommand":
                    ConfirmDisbursementNoteCommand confirmDisbursementNoteCommand = command as ConfirmDisbursementNoteCommand;
                    commandRoute = GetDisbursementNoteCommandRoutes(confirmDisbursementNoteCommand,commandType);
                    break;
                //-------------------------- ReturnsNote -------------------------

                case CommandType.CreateReturnsNote: // "CreateReturnsNoteCommand":
                    CreateReturnsNoteCommand createReturnsNoteCommand = command as CreateReturnsNoteCommand;
                    commandRoute = GetReturnsNoteCommandRoutes(createReturnsNoteCommand,commandType);
                    break;

                case CommandType.AddReturnsNoteLineItem:// "AddReturnsNoteLineItemCommand":
                    AddReturnsNoteLineItemCommand addReturnsNoteLineItemCommand = command as AddReturnsNoteLineItemCommand;
                    commandRoute = GetReturnsNoteCommandRoutes(addReturnsNoteLineItemCommand,commandType);
                    break;

                case CommandType.ConfirmReturnsNote: // "ConfirmReturnsNoteCommand":
                    ConfirmReturnsNoteCommand confirmReturnsNoteCommand = command as ConfirmReturnsNoteCommand;
                    commandRoute = GetReturnsNoteCommandRoutes(confirmReturnsNoteCommand,commandType);
                    break;
                case CommandType.CloseReturnsNote: // "CloseReturnsNoteCommand":
                    CloseReturnsNoteCommand closeReturnsNoteCommand = command as CloseReturnsNoteCommand;
                    commandRoute = GetReturnsNoteCommandRoutes(closeReturnsNoteCommand,commandType);
                    break;

                //-------------------------- LossNote -------------------------

                case CommandType.CreatePaymentNote: // "CreatePaymentNoteCommand":
                    CreatePaymentNoteCommand createLossCommand = command as CreatePaymentNoteCommand;
                    commandRoute = GetPaymentNoteCommandRoutes(createLossCommand,commandType);
                    break;

                case CommandType.AddPaymentNoteLineItem: //"AddPaymentNoteLineItemCommand":
                    AddPaymentNoteLineItemCommand addLossLineItemCommand = command as AddPaymentNoteLineItemCommand;
                    commandRoute = GetPaymentNoteCommandRoutes(addLossLineItemCommand,commandType);
                    break;

                case CommandType.ConfirmPaymentNote: // "ConfirmPaymentNoteCommand":
                    ConfirmPaymentNoteCommand confirmLossCommand = command as ConfirmPaymentNoteCommand;
                    commandRoute = GetPaymentNoteCommandRoutes(confirmLossCommand,commandType);
                    break;
                //-------------------------- Credit note -------------------------

                case CommandType.CreateCreditNote: // "CreateCreditNoteCommand":
                    CreateCreditNoteCommand createCreditNoteCommand = command as CreateCreditNoteCommand;
                    commandRoute = GetCreateCreditNoteCommand(createCreditNoteCommand,commandType);
                    break;
                case CommandType.AddCreditNoteLineItem: // "AddCreditNoteLineItemCommand":
                    AddCreditNoteLineItemCommand addCreditNoteLineItemCommand = command as AddCreditNoteLineItemCommand;
                    commandRoute = GetCreateCreditNoteCommand(addCreditNoteLineItemCommand,commandType);
                    break;
                case CommandType.ConfirmCreditNote: // "ConfirmCreditNoteCommand":
                    ConfirmCreditNoteCommand confirmCreditNoteCommand = command as ConfirmCreditNoteCommand;
                    commandRoute = GetCreateCreditNoteCommand(confirmCreditNoteCommand,commandType);
                    break;

                //-------------------------- Discount note -------------------------

                case CommandType.CreateDiscount: // "CreateDiscountCommand":
                    CreateDiscountCommand createDiscountCommand = command as CreateDiscountCommand;
                    commandRoute = GetCreateDiscountCommand(createDiscountCommand,commandType);
                    break;
                case CommandType.AddDiscountLineItem: // "AddDiscountLineItemCommand":
                    AddDiscountLineItemCommand addDiscountLineItemCommand = command as AddDiscountLineItemCommand;
                    commandRoute = GetCreateDiscountCommand(addDiscountLineItemCommand,commandType);
                    break;
                case CommandType.ConfirmDiscount: // "ConfirmDiscountCommand":
                    ConfirmDiscountCommand confirmDiscountCommand = command as ConfirmDiscountCommand;
                    commandRoute = GetCreateDiscountCommand(confirmDiscountCommand,commandType);
                    break;
                case CommandType.RetireDocument: // "RetireDocumentCommand":
                    RetireDocumentCommand retireDocumentCommand = command as RetireDocumentCommand;
                    commandRoute = GetRetireDocumentCommand(retireDocumentCommand,commandType);
                    break;
                // CommodityPurchaseCommand
                case CommandType.CreateCommodityPurchase:
                    CreateCommodityPurchaseCommand commodityPurchaseCommand = command as CreateCommodityPurchaseCommand;
                    commandRoute = GetCreateCommodityPurchaseCommand(commodityPurchaseCommand, commandType);
                    break;
                case CommandType.AddCommodityPurchaseLineItem:
                    AddCommodityPurchaseLineItemCommand addCommodityPurchaseLineItemCommand = command as AddCommodityPurchaseLineItemCommand;
                    commandRoute = GetCreateCommodityPurchaseCommand(addCommodityPurchaseLineItemCommand, commandType);
                    break;
                case CommandType.ConfirmCommodityPurchase:
                    ConfirmCommodityPurchaseCommand confirmCommodityPurchaseCommand = command as ConfirmCommodityPurchaseCommand;
                    commandRoute = GetCreateCommodityPurchaseCommand(confirmCommodityPurchaseCommand, commandType);
                    break;
                // CommodityReceptionCommand
                case CommandType.CreateCommodityReception:
                    CreateCommodityReceptionCommand createCommodityReceptionCommand = command as CreateCommodityReceptionCommand;
                    commandRoute = GetCreateCommodityReceptionCommand(createCommodityReceptionCommand, commandType);
                    break;
                case CommandType.AddCommodityReceptionLineItem:
                    AddCommodityReceptionLineItemCommand addCommodityReceptionLineItemCommand = command as AddCommodityReceptionLineItemCommand;
                    commandRoute = GetCreateCommodityReceptionCommand(addCommodityReceptionLineItemCommand, commandType);
                    break;
                case CommandType.ConfirmCommodityReception:
                    ConfirmCommodityReceptionCommand confirmCommodityReceptionCommand = command as ConfirmCommodityReceptionCommand;
                    commandRoute = GetCreateCommodityReceptionCommand(confirmCommodityReceptionCommand, commandType);
                    break;
                case CommandType.StoredCommodityReceptionLineItem:
                    StoredCommodityReceptionLineItemCommand storedCommodityReceptionLineItemCommand = command as StoredCommodityReceptionLineItemCommand;
                    commandRoute = GetCreateCommodityReceptionCommand(storedCommodityReceptionLineItemCommand, commandType);
                    break;
                // CommodityStorageCommand
                case CommandType.CreateCommodityStorage:
                    CreateCommodityStorageCommand createCommodityStorageCommand = command as CreateCommodityStorageCommand;
                    commandRoute = GetCreateCommodityStorageCommand(createCommodityStorageCommand, commandType);
                    break;
                case CommandType.AddCommodityStorageLineItem:
                    AddCommodityStorageLineItemCommand addCommodityStorageLineItemCommand = command as AddCommodityStorageLineItemCommand;
                    commandRoute = GetCreateCommodityStorageCommand(addCommodityStorageLineItemCommand, commandType);
                    break;
                case CommandType.ConfirmCommodityStorage:
                    ConfirmCommodityStorageCommand confirmCommodityStorageCommand = command as ConfirmCommodityStorageCommand;
                    commandRoute = GetCreateCommodityStorageCommand(confirmCommodityStorageCommand, commandType);
                    break;
                case CommandType.CreateMainOrder:
                    CreateMainOrderCommand cmo = command as CreateMainOrderCommand;
                    commandRoute = GetOrderMainCommandRoutes(cmo, commandType);
                    break;
                case CommandType.AddMainOrderLineItem :
                    AddMainOrderLineItemCommand cami = command as AddMainOrderLineItemCommand;
                    commandRoute = GetOrderMainCommandRoutes(cami, commandType);
                    break;
                case CommandType.ConfirmMainOrder:
                    ConfirmMainOrderCommand ccmo = command as ConfirmMainOrderCommand;
                    commandRoute = GetOrderMainCommandRoutes(ccmo, commandType);
                    break;
                case CommandType.ApproveOrderLineItem:
                    ApproveOrderLineItemCommand camol = command as ApproveOrderLineItemCommand;
                    commandRoute = GetOrderMainCommandRoutes(camol, commandType);
                    break;
                case CommandType.ApproveMainOrder:
                    ApproveMainOrderCommand camo = command as ApproveMainOrderCommand;
                    commandRoute = GetOrderMainCommandRoutes(camo, commandType);
                    break;
                case CommandType.OrderDispatchApprovedLineItems:
                    OrderDispatchApprovedLineItemsCommand codal = command as OrderDispatchApprovedLineItemsCommand;
                    commandRoute = GetOrderMainCommandRoutes(codal, commandType);
                    break;
                case CommandType.ChangeMainOrderLineItem:
                    ChangeMainOrderLineItemCommand cmol = command as ChangeMainOrderLineItemCommand;
                    commandRoute = GetOrderMainCommandRoutes(cmol, commandType);
                    break;
                case CommandType.RemoveMainOrderLineItem:
                    RemoveMainOrderLineItemCommand crmol = command as RemoveMainOrderLineItemCommand;
                    commandRoute = GetOrderMainCommandRoutes(crmol, commandType);
                    break;
                case CommandType.RejectMainOrder:
                    RejectMainOrderCommand crmo = command as RejectMainOrderCommand;
                    commandRoute = GetOrderMainCommandRoutes(crmo, commandType);
                    break;
                case CommandType.OrderPaymentInfo:
                    AddOrderPaymentInfoCommand aopi = command as AddOrderPaymentInfoCommand;
                    commandRoute = GetOrderMainCommandRoutes(aopi, commandType);
                    break;
                // CommodityDeliveryCommand
                case CommandType.CreateCommodityDelivery:
                    CreateCommodityDeliveryCommand createCommodityDeliveryCommand  = command as CreateCommodityDeliveryCommand;
                    commandRoute = GetCreateCommodityDeliveryCommand(createCommodityDeliveryCommand, commandType);
                    break;
                case CommandType.AddCommodityDeliveryLineItem:
                    AddCommodityDeliveryLineItemCommand addCommodityDeliveryLineItemCommand = command as AddCommodityDeliveryLineItemCommand;
                    commandRoute = GetCreateCommodityDeliveryCommand(addCommodityDeliveryLineItemCommand, commandType);
                    break;
                case CommandType.ConfirmCommodityDelivery:
                    ConfirmCommodityDeliveryCommand confirmCommodityDeliveryCommand = command as ConfirmCommodityDeliveryCommand;
                    commandRoute = GetCreateCommodityDeliveryCommand(confirmCommodityDeliveryCommand, commandType);
                    break;
                case CommandType.WeighedCommodityDeliveryLineItem:
                    WeighedCommodityDeliveryLineItemCommand weighedCommodityDeliveryLineItemCommand = command as WeighedCommodityDeliveryLineItemCommand;
                    commandRoute = GetCreateCommodityDeliveryCommand(weighedCommodityDeliveryLineItemCommand, commandType);
                    break;
                case CommandType.ApproveDelivery:
                    ApproveDeliveryCommand receivedDeliveryCommand = command as ApproveDeliveryCommand;
                    commandRoute = GetCreateCommodityDeliveryCommand(receivedDeliveryCommand, commandType);
                    break;
                // ReceivedDelivery
                case CommandType.CreateReceivedDelivery:
                    CreateReceivedDeliveryCommand createReceivedDeliveryCommand = command as CreateReceivedDeliveryCommand;
                    commandRoute = GetCreateReceiveDeliveryCommand(createReceivedDeliveryCommand, commandType);
                    break;
                case CommandType.AddReceivedDeliveryLineItem:
                    AddReceivedDeliveryLineItemCommand addReceivedDeliveryLineItem = command as AddReceivedDeliveryLineItemCommand;
                    commandRoute = GetCreateReceiveDeliveryCommand(addReceivedDeliveryLineItem, commandType);
                    break;
                case CommandType.StoredReceivedDeliveryLineItem:
                    StoredReceivedDeliveryLineItemCommand storedReceivedDeliveryLineItemCommand = command as StoredReceivedDeliveryLineItemCommand;
                    commandRoute = GetCreateReceiveDeliveryCommand(storedReceivedDeliveryLineItemCommand, commandType);
                    break;
                case CommandType.ConfirmReceivedDelivery:
                    ConfirmReceivedDeliveryCommand confirmReceivedDeliveryCommand = command as ConfirmReceivedDeliveryCommand;
                    commandRoute = GetCreateReceiveDeliveryCommand(confirmReceivedDeliveryCommand, commandType);
                    break;
                case CommandType.ReCollection:
                    ReCollectionCommand reCollectionCommand = command as ReCollectionCommand;
                    commandRoute = GetReCollectionCommand(reCollectionCommand, commandType);
                    break;
                case CommandType.CreateCommodityTransfer:
                    CreateCommodityTransferCommand createCommodityTransferCommand = command as CreateCommodityTransferCommand;
                    commandRoute = GetCreateCommodityTransferCommand(createCommodityTransferCommand, commandType);
                    break;
                case CommandType.AddCommodityTransferLineItem:
                    AddCommodityTransferLineItemCommand commodityTransferLineItemCommand = command as AddCommodityTransferLineItemCommand;
                    commandRoute = GetCreateCommodityTransferCommand(commodityTransferLineItemCommand, commandType);
                    break;
                case CommandType.ConfirmCommodityTransfer:
                    ConfirmCommodityTransferCommand confirmCommodityTransferCommand = command as ConfirmCommodityTransferCommand;
                    commandRoute = GetCreateCommodityTransferCommand(confirmCommodityTransferCommand, commandType);
                    break;
                case CommandType.ApproveCommodityTransfer:
                    ApproveCommodityTransferCommand approveCommodityTransferCommand = command as ApproveCommodityTransferCommand;
                    commandRoute = GetCreateCommodityTransferCommand(approveCommodityTransferCommand, commandType);
                    break;
                case CommandType.TransferedCommodityStorage:
                    TransferedCommodityStorageLineItemCommand transferedCommodityStorageCommand = command as TransferedCommodityStorageLineItemCommand;
                    commandRoute = GetCreateCommodityTransferCommand(transferedCommodityStorageCommand, commandType);
                    break;
                case CommandType.CreateActivity:
                    CreateActivityNoteCommand  createActivityNoteCommand = command as CreateActivityNoteCommand;
                    commandRoute = GetCreateActivityCommand(createActivityNoteCommand, commandType);
                    break;
                case CommandType.ConfirmActivity:
                    ConfirmActivityCommand confirmActivityCommand = command as ConfirmActivityCommand;
                    commandRoute = GetCreateActivityCommand(confirmActivityCommand, commandType);
                    break;
                case CommandType.AddActivityInfectionItem:
                    AddActivityInfectionLineItemCommand addActivityInfectionLineItemCommand = command as AddActivityInfectionLineItemCommand;
                    commandRoute = GetCreateActivityCommand(addActivityInfectionLineItemCommand, commandType);
                    break;
                case CommandType.AddActivityInputItem:
                    AddActivityInputLineItemCommand addActivityInputLineItemCommand = command as AddActivityInputLineItemCommand;
                    commandRoute = GetCreateActivityCommand(addActivityInputLineItemCommand, commandType);
                    break;
                case CommandType.AddActivityServiceItem:
                    AddActivityServiceLineItemCommand addActivityServiceLineItemCommand = command as AddActivityServiceLineItemCommand;
                    commandRoute = GetCreateActivityCommand(addActivityServiceLineItemCommand, commandType);
                    break;
                case CommandType.AddActivityProduceItem:
                    AddActivityProduceLineItemCommand addActivityProduceLineItemCommand = command as AddActivityProduceLineItemCommand;
                    commandRoute = GetCreateActivityCommand(addActivityProduceLineItemCommand, commandType);
                    break;

                case CommandType.CreateCommodityWarehouseStorage:
                    CreateCommodityWarehouseStorageCommand createCommodityWarehouseStorageCommand = command as CreateCommodityWarehouseStorageCommand;
                    commandRoute = GetCommodityWarehouseStorageCommand(createCommodityWarehouseStorageCommand, commandType);
                    break;
                case CommandType.ConfirmCommodityWarehouseStorage:
                    ConfirmCommodityWarehouseStorageCommand confirmCommodityWarehouseStorageCommand = command as ConfirmCommodityWarehouseStorageCommand;
                    commandRoute = GetCommodityWarehouseStorageCommand(confirmCommodityWarehouseStorageCommand, commandType);
                    break;
                case CommandType.AddCommodityWarehouseStorageLineItem:
                    AddCommodityWarehouseStorageLineItemCommand addCommodityWarehouseStorageLineItemCommand = command as AddCommodityWarehouseStorageLineItemCommand;
                    commandRoute = GetCommodityWarehouseStorageCommand(addCommodityWarehouseStorageLineItemCommand, commandType);
                    break;
                case CommandType.UpdateCommodityWarehouseStorageLineItem:
                    UpdateCommodityWarehouseStorageLineItemCommand updateCommodityWarehouseStorageLineItemCommand = command as UpdateCommodityWarehouseStorageLineItemCommand;
                    commandRoute = GetCommodityWarehouseStorageCommand(updateCommodityWarehouseStorageLineItemCommand, commandType);
                    break;

                case CommandType.ApproveCommodityWarehouseStorage:
                    ApproveCommodityWarehouseStorageCommand approveCommodityWarehouseStorageCommand = command as ApproveCommodityWarehouseStorageCommand;
                    commandRoute = GetCommodityWarehouseStorageCommand(approveCommodityWarehouseStorageCommand, commandType);
                    break;
                case CommandType.StoreCommodityWarehouseStorage:
                    StoreCommodityWarehouseStorageCommand storeCommodityWarehouseStorageCommand = command as StoreCommodityWarehouseStorageCommand;
                    commandRoute = GetCommodityWarehouseStorageCommand(storeCommodityWarehouseStorageCommand, commandType);
                    break;
                case CommandType.CreateOutletVisitNote:
                    CreateOutletVisitNoteCommand createOutletVisitNoteCommand = command as CreateOutletVisitNoteCommand;
                    commandRoute = GetOutletVisitNoteCommand(createOutletVisitNoteCommand, commandType);
                    break;
                
                case CommandType.CreateCommodityReleaseNote:
                    CreateCommodityReleaseCommand createcommodityReleaseNoteCommand = command as CreateCommodityReleaseCommand;
                    commandRoute = GetCommodityReleaseCommand(createcommodityReleaseNoteCommand, commandType);
                    break;
                
                case CommandType.AddCommodityReleaseNoteLineItem:
                    AddCommodityReleaseNoteLineItemCommand addcommodityReleaseNoteCommand = command as AddCommodityReleaseNoteLineItemCommand;
                    commandRoute = GetCommodityReleaseCommand(addcommodityReleaseNoteCommand, commandType);
                    break;

                case CommandType.ConfirmCommodityReleaseNote:
                    ConfirmCommodityReleaseNoteCommand confirmcommodityReleaseNoteCommand = command as ConfirmCommodityReleaseNoteCommand;
                    commandRoute = GetCommodityReleaseCommand(confirmcommodityReleaseNoteCommand, commandType);
                    break;

                default:
                    throw new Exception(string.Format("COMMAND ROUTING FOR {0} NOT HANDLED ON SERVER", commandType));
            }
            return commandRoute;
        }
        private CommandRouteOnRequestDTO GetCommodityReleaseCommand(ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();

            if (command is CreateCommodityReleaseCommand)
            {
                var ic = command as CreateCommodityReleaseCommand;
                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();
                SetupCommnd(commandRouteOnRequest, ic);
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.CommandGeneratedByCostCentreId, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentRecipientCostCentreId, CommandType = commandType.ToString() });
            }
            if (command is AddCommodityReleaseNoteLineItemCommand || command is ConfirmCommodityReleaseNoteCommand)
            {
                SetupExistingSourcingCommandRouteCentres(commandRouteOnRequest, command, commandType);

                SetupCommnd(commandRouteOnRequest, (DocumentCommand)command);
            }
            return commandRouteOnRequest;
        }

        private CommandRouteOnRequestDTO GetOutletVisitNoteCommand(ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();

            if (command is CreateOutletVisitNoteCommand)
            {
                var ic = command as CreateOutletVisitNoteCommand;
                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();
                SetupCommnd(commandRouteOnRequest, ic);
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.CommandGeneratedByCostCentreId, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentRecipientCostCentreId, CommandType = commandType.ToString() });
            }
            
            return commandRouteOnRequest;
        }
        private CommandRouteOnRequestDTO GetCommodityWarehouseStorageCommand(ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();

            if (command is CreateCommodityWarehouseStorageCommand)
            {
                var ic = command as CreateCommodityWarehouseStorageCommand;
                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();
                SetupCommnd(commandRouteOnRequest, ic);
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.CommandGeneratedByCostCentreId, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentRecipientCostCentreId, CommandType = commandType.ToString() });
            }
            if (command is AddCommodityWarehouseStorageLineItemCommand || command is ConfirmCommodityWarehouseStorageCommand
                || command is UpdateCommodityWarehouseStorageLineItemCommand || command is ApproveCommodityWarehouseStorageCommand || command is StoreCommodityWarehouseStorageCommand)
            {
                SetupExistingSourcingCommandRouteCentres(commandRouteOnRequest, command, commandType);

                SetupCommnd(commandRouteOnRequest, (DocumentCommand)command);
            }
            return commandRouteOnRequest;
        }


        private CommandRouteOnRequestDTO GetCreateActivityCommand(ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();

            if (command is CreateActivityNoteCommand)
            {
                var ic = command as CreateActivityNoteCommand;
                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();
                SetupCommnd(commandRouteOnRequest, ic);
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.CommandGeneratedByCostCentreId, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.HubId, CommandType = commandType.ToString() });
            }
            if (command is AddActivityInputLineItemCommand || command is ConfirmActivityCommand
                || command is AddActivityInfectionLineItemCommand || command is AddActivityProduceLineItemCommand || command is AddActivityServiceLineItemCommand)
            {
                SetupExistingActivityCommandRouteCentres(commandRouteOnRequest, command, commandType);

                SetupCommnd(commandRouteOnRequest, (DocumentCommand)command);
            }
            return commandRouteOnRequest;
        }

        private CommandRouteOnRequestDTO GetCreateCommodityTransferCommand(ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();

            if (command is CreateCommodityTransferCommand)
            {
                var ic = command as CreateCommodityTransferCommand;
                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();
                SetupCommnd(commandRouteOnRequest, ic);
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.CommandGeneratedByCostCentreId, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentRecipientCostCentreId, CommandType = commandType.ToString() });
            }
            if (command is AddCommodityTransferLineItemCommand || command is ConfirmCommodityTransferCommand
                || command is ApproveCommodityTransferCommand || command is TransferedCommodityStorageLineItemCommand)
            {
                SetupExistingSourcingCommandRouteCentres(commandRouteOnRequest, command, commandType);

                SetupCommnd(commandRouteOnRequest, (DocumentCommand)command);
            }
            return commandRouteOnRequest;
        }

        private CommandRouteOnRequestDTO GetReCollectionCommand(ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();

            if (command is ReCollectionCommand)
            {
                var ic = command as ReCollectionCommand;
                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();
                SetupCommnd(commandRouteOnRequest, ic);
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.CommandGeneratedByCostCentreId, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.FromCostCentreId, CommandType = commandType.ToString() });
            
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentRecipientCostCentreId, CommandType = commandType.ToString() });
            }
            
            return commandRouteOnRequest;
        }
        private CommandRouteOnRequestDTO GetCreateReceiveDeliveryCommand(ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();

            if (command is CreateReceivedDeliveryCommand)
            {
                var ic = command as CreateReceivedDeliveryCommand;
                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();
                SetupCommnd(commandRouteOnRequest, ic);
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.CommandGeneratedByCostCentreId, CommandType = commandType.ToString() });
               // commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentRecipientCostCentreId, CommandType = commandType.ToString() });
            }
            if (command is AddReceivedDeliveryLineItemCommand || command is ConfirmReceivedDeliveryCommand || command is StoredReceivedDeliveryLineItemCommand)
            {
                SetupExistingSourcingCommandRouteCentres(commandRouteOnRequest, command, commandType);

                SetupCommnd(commandRouteOnRequest, (DocumentCommand)command);
            }
            return commandRouteOnRequest;
        }
        private CommandRouteOnRequestDTO GetCreateCommodityDeliveryCommand(ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();

            if (command is CreateCommodityDeliveryCommand)
            {
                var ic = command as CreateCommodityDeliveryCommand;
                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();
                SetupCommnd(commandRouteOnRequest, ic);
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.CommandGeneratedByCostCentreId, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentRecipientCostCentreId, CommandType = commandType.ToString() });
            }
            if (command is AddCommodityDeliveryLineItemCommand || command is ConfirmCommodityDeliveryCommand || command is WeighedCommodityDeliveryLineItemCommand || command is ApproveDeliveryCommand)
            {
                SetupExistingSourcingCommandRouteCentres(commandRouteOnRequest, command, commandType);

                SetupCommnd(commandRouteOnRequest, (DocumentCommand)command);
            }
            return commandRouteOnRequest;
        }
        private CommandRouteOnRequestDTO GetOrderMainCommandRoutes(ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();

            if (command is CreateMainOrderCommand)
            {
                var ic = command as CreateMainOrderCommand;
                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();
                SetupCommnd(commandRouteOnRequest, ic);
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentRecipientCostCentreId, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentIssuerCostCentreId, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.IssuedOnBehalfOfCostCentreId, CommandType = commandType.ToString() });
            }
            if (command is AddMainOrderLineItemCommand || command is ConfirmMainOrderCommand ||
                command is ApproveMainOrderCommand ||
                command is CloseOrderCommand  ||
                 command is RejectMainOrderCommand || command is RemoveMainOrderLineItemCommand
                 || command is ChangeMainOrderLineItemCommand || command is OrderDispatchApprovedLineItemsCommand
                || command is ApproveOrderLineItemCommand || command is AddOrderPaymentInfoCommand)
            {
                Document doc = _genericDocumentRepository.GetById(command.DocumentId) ;
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = doc.DocumentIssuerCostCentre.Id, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = doc.DocumentRecipientCostCentre.Id, CommandType = commandType.ToString() });
               // commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = doc.IssuedOnBehalfOf.Id, CommandType = commandType.ToString() });

                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();

                SetupCommnd(commandRouteOnRequest, (DocumentCommand)command);
            }
            return commandRouteOnRequest;
        }
        private CommandRouteOnRequestDTO GetCreateCommodityPurchaseCommand (ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();

            if (command is CreateCommodityPurchaseCommand)
            {
                var ic = command as CreateCommodityPurchaseCommand;
                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();
                SetupCommnd(commandRouteOnRequest, ic);
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.CommandGeneratedByCostCentreId, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentRecipientCostCentreId, CommandType = commandType.ToString() });
            }
            if (command is AddCommodityPurchaseLineItemCommand || command is ConfirmCommodityPurchaseCommand)
            {
                SetupExistingSourcingCommandRouteCentres(commandRouteOnRequest, command, commandType);

                SetupCommnd(commandRouteOnRequest, (DocumentCommand)command);
            }
            return commandRouteOnRequest;  
        }

        private CommandRouteOnRequestDTO GetCreateCommodityReceptionCommand(ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();

            if (command is CreateCommodityReceptionCommand)
            {
                var ic = command as CreateCommodityReceptionCommand;
                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();
                SetupCommnd(commandRouteOnRequest, ic);
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.CommandGeneratedByCostCentreId, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentRecipientCostCentreId, CommandType = commandType.ToString() });
            }
            if (command is AddCommodityReceptionLineItemCommand || command is ConfirmCommodityReceptionCommand || command is StoredCommodityReceptionLineItemCommand)
            {
                SetupExistingSourcingCommandRouteCentres(commandRouteOnRequest, command, commandType);

                SetupCommnd(commandRouteOnRequest, (DocumentCommand)command);
            }
            return commandRouteOnRequest;
        }
        private CommandRouteOnRequestDTO GetCreateCommodityStorageCommand(ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();

            if (command is CreateCommodityStorageCommand)
            {
                var ic = command as CreateCommodityStorageCommand;
                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();
                SetupCommnd(commandRouteOnRequest, ic);
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.CommandGeneratedByCostCentreId, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentRecipientCostCentreId, CommandType = commandType.ToString() });
            }
            if (command is AddCommodityStorageLineItemCommand || command is ConfirmCommodityStorageCommand)
            {
                SetupExistingSourcingCommandRouteCentres(commandRouteOnRequest, command, commandType);

                SetupCommnd(commandRouteOnRequest, (DocumentCommand)command);
            }
            return commandRouteOnRequest;
        }

        private CommandRouteOnRequestDTO GetRetireDocumentCommand(ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();

            if (command is RetireDocumentCommand)
            {
                SetupExistingCommandRouteCentres(commandRouteOnRequest, command, commandType);
                SetupCommnd(commandRouteOnRequest, (DocumentCommand)command);
            }
            return commandRouteOnRequest;  
        }

        private CommandRouteOnRequestDTO GetCreateDiscountCommand(ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();
            if (command is CreateDiscountCommand)
            {
                var ic = command as CreateDiscountCommand;
                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();
                SetupCommnd(commandRouteOnRequest, ic);
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentIssuerCostCentreId,CommandType = commandType.ToString()});
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentRecipientCostCentreId, CommandType = commandType.ToString()});
            }
            if (command is AddDiscountLineItemCommand || command is ConfirmDiscountCommand)
            {
                SetupExistingCommandRouteCentres(commandRouteOnRequest, command, commandType);

                SetupCommnd(commandRouteOnRequest, (DocumentCommand)command);
            }
            return commandRouteOnRequest;  
        }

        private CommandRouteOnRequestDTO GetCreateCreditNoteCommand(ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();

            if (command is CreateCreditNoteCommand)
            {
                var ic = command as CreateCreditNoteCommand;
                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();
                SetupCommnd(commandRouteOnRequest, ic);
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentIssuerCostCentreId, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentRecipientCostCentreId, CommandType = commandType.ToString() });
            }
            if (command is AddCreditNoteLineItemCommand || command is ConfirmCreditNoteCommand)
            {
                SetupExistingCommandRouteCentres(commandRouteOnRequest, command, commandType);

                SetupCommnd(commandRouteOnRequest, (DocumentCommand)command);
            }
            return commandRouteOnRequest;  
        }

        private CommandRouteOnRequestDTO GetPaymentNoteCommandRoutes(ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();

            if (command is CreatePaymentNoteCommand)
            {
                var ic = command as CreatePaymentNoteCommand;
                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();
                SetupCommnd(commandRouteOnRequest, ic);
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentIssuerCostCentreId, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.PaymentNoteRecipientCostCentreId, CommandType = commandType.ToString() });
            }
            if (command is AddPaymentNoteLineItemCommand || command is ConfirmPaymentNoteCommand)
            {
                SetupExistingCommandRouteCentres(commandRouteOnRequest, command, commandType);
                SetupCommnd(commandRouteOnRequest, (DocumentCommand)command);
            }
            return commandRouteOnRequest;
        }

        private CommandRouteOnRequestDTO GetReturnsNoteCommandRoutes(ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();

            if (command is CreateReturnsNoteCommand)
            {
                var ic = command as CreateReturnsNoteCommand;
                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();
                SetupCommnd(commandRouteOnRequest, ic);
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentRecipientCostCentreId, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentIssuerCostCentreId, CommandType = commandType.ToString() });
            }
            if (command is AddReturnsNoteLineItemCommand || command is ConfirmReturnsNoteCommand || command is CloseReturnsNoteCommand)
            {
                SetupExistingCommandRouteCentres(commandRouteOnRequest, command, commandType);
                SetupCommnd(commandRouteOnRequest, (DocumentCommand)command);
            }
            return commandRouteOnRequest;
        }

        private CommandRouteOnRequestDTO GetDisbursementNoteCommandRoutes(ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();

            if (command is CreateDisbursementNoteCommand)
            {
                var ic = command as CreateDisbursementNoteCommand;
                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();
                SetupCommnd(commandRouteOnRequest, ic);
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentRecipientCostCentreId, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentIssuerCostCentreId, CommandType = commandType.ToString() });
            }
            if (command is AddDisbursementNoteLineItemCommand || command is ConfirmDisbursementNoteCommand)
            {
                SetupExistingCommandRouteCentres(commandRouteOnRequest, command, commandType);
                SetupCommnd(commandRouteOnRequest, (DocumentCommand)command);
            }
            return commandRouteOnRequest;
        }
        private CommandRouteOnRequestDTO GetReceiptCommandRoutes(ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();

            if (command is CreateReceiptCommand)
            {
                var ic = command as CreateReceiptCommand;
                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();
                SetupCommnd(commandRouteOnRequest, ic);
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentRecipientCostCentreId, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentIssuerCostCentreId, CommandType = commandType.ToString() });
            }
            if (command is AddReceiptLineItemCommand || command is ConfirmReceiptCommand || command is ConfirmReceiptLineItemCommand)
            {
                SetupExistingCommandRouteCentres(commandRouteOnRequest, command, commandType);
                SetupCommnd(commandRouteOnRequest, (DocumentCommand)command);
            }
            return commandRouteOnRequest;
        }
        private CommandRouteOnRequestDTO GetDispatchNoteCommandRoutes(ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();

            if (command is CreateDispatchNoteCommand)
            {
                var ic = command as CreateDispatchNoteCommand;
                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();
                SetupCommnd(commandRouteOnRequest, ic);
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DispatchNoteRecipientCostCentreId, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentIssuerCostCentreId, CommandType = commandType.ToString() });
            }
            if (command is AddDispatchNoteLineItemCommand || command is ConfirmDispatchNoteCommand)
            {
                SetupExistingCommandRouteCentres(commandRouteOnRequest, command, commandType);
                SetupCommnd(commandRouteOnRequest, (DocumentCommand)command);
            }
            return commandRouteOnRequest;
        }
        private CommandRouteOnRequestDTO GetInventoryTransferNoteCommandRoutes(ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();

            if (command is CreateInventoryTransferNoteCommand)
            {
                var ic = command as CreateInventoryTransferNoteCommand;
                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();
                SetupCommnd(commandRouteOnRequest, ic);
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentRecipientCostCentreId, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentIssuerCostCentreId, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.IssuedOnBehalfOfCostCentreId, CommandType = commandType.ToString() });
            }
            if (command is AddInventoryTransferNoteLineItemCommand || command is ConfirmInventoryTransferNoteCommand)
            {
                InventoryTransferNote doc = _inventoryTransferNoteRepository.GetById(command.DocumentId);
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = doc.DocumentIssuerCostCentre.Id, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = doc.DocumentRecipientCostCentre.Id, CommandType = commandType.ToString() });
                if (doc.DocumentIssueredOnBehalfCostCentre != null)
                    commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = doc.DocumentIssueredOnBehalfCostCentre.Id, CommandType = commandType.ToString() });
                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();

                SetupCommnd(commandRouteOnRequest, (DocumentCommand)command);
            }
            return commandRouteOnRequest;
        }
        private CommandRouteOnRequestDTO GetInventoryReceivedNoteCommandRoutes(ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();

            if (command is CreateInventoryReceivedNoteCommand)
            {
                var ic = command as CreateInventoryReceivedNoteCommand;
                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();
                SetupCommnd(commandRouteOnRequest, ic);
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.InventoryReceivedFromCostCentreId, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentIssuerCostCentreId, CommandType = commandType.ToString() });
            }
            if (command is AddInventoryReceivedNoteLineItemCommand || command is ConfirmInventoryReceivedNoteCommand)
            {
                SetupExistingCommandRouteCentres(commandRouteOnRequest, command, commandType);

                SetupCommnd(commandRouteOnRequest, (DocumentCommand)command);
            }
            return commandRouteOnRequest;
        }

        private CommandRouteOnRequestDTO GetOrderCommandRoutes(ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();

            //if (command is CreateOrderCommand)
            //{
            //    var ic = command as CreateOrderCommand;
            //    commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();
            //    SetupCommnd(commandRouteOnRequest, ic);
            //    commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentRecipientCostCentreId, CommandType = commandType.ToString() });
            //    commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentIssuerCostCentreId, CommandType = commandType.ToString() });
            //    commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.IssuedOnBehalfOfCostCentreId, CommandType = commandType.ToString() });
            //}
            if ( 
                 command is BackOrderCommand ||
                command is CloseOrderCommand || command is DispatchToPhoneCommand ||
                command is OrderPendingDispatchCommand || command is RejectOrderCommand || command is RemoveOrderLineItemCommand || command is ChangeOrderLineItemCommand)
            {
                Order doc = _orderRepository.GetById(command.DocumentId) as Order;
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = doc.DocumentIssuerCostCentre.Id, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = doc.DocumentRecipientCostCentre.Id, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = doc.IssuedOnBehalfOf.Id, CommandType = commandType.ToString() });

                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();

                SetupCommnd(commandRouteOnRequest, (DocumentCommand)command);
            }
            return commandRouteOnRequest;
        }
        private CommandRouteOnRequestDTO GetInventoryAdjustmentNoteCommandRoutes(ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();

            if (command is CreateInventoryAdjustmentNoteCommand)
            {
                var ic = command as CreateInventoryAdjustmentNoteCommand;
                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();
                SetupCommnd(commandRouteOnRequest, ic);
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentIssuerCostCentreId, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentRecipientCostCentreId, CommandType = commandType.ToString() });
                //cmd.CommandRouteCentre.Add(new CommandRouteCentre { CostCentreId = ic.DocumentRecipientCostCentreId });
            }
            if (command is AddInventoryAdjustmentNoteLineItemCommand || command is ConfirmInventoryAdjustmentNoteCommand )
            {
                SetupExistingCommandRouteCentres(commandRouteOnRequest, command, commandType);
                SetupCommnd(commandRouteOnRequest, (DocumentCommand)command);
            }
            return commandRouteOnRequest;
        }

        private CommandRouteOnRequestDTO GetInvoiceCommandRoutes(ICommand command, CommandType commandType)
        {
            var commandRouteOnRequest = new CommandRouteOnRequestDTO();

            if (command is CreateInvoiceCommand)
            {
                var ic = command as CreateInvoiceCommand;
                commandRouteOnRequest.RouteOnRequest.CommandType = commandType.ToString();
                SetupCommnd(commandRouteOnRequest, ic);
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentIssuerCostCentreId, CommandType = commandType.ToString() });
                commandRouteOnRequest.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = ic.DocumentRecipientCostCentreId, CommandType = commandType.ToString() });
            }
            if (command is AddInvoiceLineItemCommand || command is ConfirmInvoiceCommand || command is CloseInvoiceCommand)
            {
                SetupExistingCommandRouteCentres(commandRouteOnRequest, command, commandType);
                SetupCommnd(commandRouteOnRequest, (DocumentCommand)command);
            }
            return commandRouteOnRequest;
        }

        private void SetupExistingCommandRouteCentres(CommandRouteOnRequestDTO commandRouteOnRequestDTO, ICommand command, CommandType commandType)
        {
            Document document = _genericDocumentRepository.GetById(command.DocumentId);
            commandRouteOnRequestDTO.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = document.DocumentIssuerCostCentre.Id, CommandType = commandType.ToString()});
            if (document.DocumentRecipientCostCentre != null)
            {
                commandRouteOnRequestDTO.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre{CostCentreId =document.DocumentRecipientCostCentre.Id,CommandType =commandType.ToString()});
            }
            commandRouteOnRequestDTO.RouteOnRequest.CommandType = commandType.ToString();
        }
        private void SetupExistingActivityCommandRouteCentres(CommandRouteOnRequestDTO commandRouteOnRequestDTO, ICommand command, CommandType commandType)
        {
            GenericSourcingDocument document = _sourcingDocumentRepository.GetActivityById(command.DocumentId);
            commandRouteOnRequestDTO.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = document.DocumentIssuer, CommandType = commandType.ToString() });
            commandRouteOnRequestDTO.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = document.DocumentRecepient, CommandType = commandType.ToString() });
            commandRouteOnRequestDTO.RouteOnRequest.CommandType = commandType.ToString();
        }

        private void SetupExistingSourcingCommandRouteCentres(CommandRouteOnRequestDTO commandRouteOnRequestDTO, ICommand command, CommandType commandType)
        {
            GenericSourcingDocument document = _sourcingDocumentRepository.GetById(command.DocumentId);
            commandRouteOnRequestDTO.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = document.DocumentIssuer, CommandType = commandType.ToString() });
            commandRouteOnRequestDTO.CommandRouteCentres.Add(new CommandRouteOnRequestCostcentre { CostCentreId = document.DocumentRecepient, CommandType = commandType.ToString() });
            commandRouteOnRequestDTO.RouteOnRequest.CommandType = commandType.ToString();
        }

        private void SetupCommnd(CommandRouteOnRequestDTO commandRouteOnRequest, DocumentCommand ic)
        {
            commandRouteOnRequest.RouteOnRequest.CommandGeneratedByCostCentreApplicationId = ic.CommandGeneratedByCostCentreApplicationId;
            commandRouteOnRequest.RouteOnRequest.DocumentId = ic.DocumentId;
            commandRouteOnRequest.RouteOnRequest.DateCommandInserted = DateTime.Now;
            commandRouteOnRequest.RouteOnRequest.CommandGeneratedByUserId = ic.CommandGeneratedByUserId;
            commandRouteOnRequest.RouteOnRequest.CommandId = ic.CommandId;
            commandRouteOnRequest.RouteOnRequest.JsonCommand = JsonConvert.SerializeObject(ic);
            commandRouteOnRequest.RouteOnRequest.DocumentParentId = ic.PDCommandId;

        }
    }
}
