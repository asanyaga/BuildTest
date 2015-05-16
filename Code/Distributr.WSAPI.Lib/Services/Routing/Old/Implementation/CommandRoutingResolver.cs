//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Distributr.Core.Commands;
//using Distributr.Core.Commands.DocumentCommands;
//using Distributr.Core.Commands.DocumentCommands.AdjustmentNotes;
//using Distributr.Core.Commands.DocumentCommands.DisbursementNotes;
//using Distributr.Core.Commands.DocumentCommands.InventoryReceivedNotes;
//using Distributr.Core.Commands.DocumentCommands.InventoryTransferNotes;
//using Distributr.Core.Commands.DocumentCommands.Orders;
//using Distributr.Core.Commands.DocumentCommands.Receipts;
//using Distributr.Core.Domain.Master.CostCentreEntities;
//using Distributr.Core.Repository.Transactional.DocumentRepositories;
//using Distributr.WSAPI.Lib.Services.Routing.Repository;
//using Newtonsoft.Json;
//using Distributr.Core.Utility;
//using Distributr.Core.Repository.Master.CostCentreRepositories;
//using log4net;
//using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
//using Distributr.Core.Domain.Transactional.DocumentEntities;
//using Newtonsoft.Json.Converters;
//using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
//using Distributr.Core.Repository.Transactional.DocumentRepositories.DispatchRepositories;
//using Distributr.Core.Commands.DocumentCommands.DispatchNotes;
//using Distributr.Core.Commands.DocumentCommands.ReturnsNotes;
//using Distributr.Core.Commands.DocumentCommands.Invoices;

//namespace Distributr.WSAPI.Lib.Services.Routing.Implementation
//{
//    public class CommandRoutingResolver : ICommandRoutingResolver
//    {
//        ICommandRoutingRepository _commandRoutingRepository;
//        ICoreObjectSerializationHelper _objectSerialization;
//        ICostCentreApplicationRepository _costCentreApplicationRepository;
//        private IOrderRepository _orderRepository;
//        private IInventoryAdjustmentNoteRepository _inventoryAdjustmentNoteRepository;
//        private IInventoryTransferNoteRepository _inventoryTransferNoteRepository;
//        IDispatchNoteRepository _dispatchNoteRepository;
//        private IProducerRepository _producerRepository;
//        ILog _logger;
//        private ICostCentreRepository _costCentreRepository;
//        private IDocumentRepository _documentRepository;



//        public CommandRoutingResolver(ICommandRoutingRepository commandRoutingRepository, ICoreObjectSerializationHelper objectSerialization, ICostCentreApplicationRepository costCentreApplicationRepository, IOrderRepository orderRepository, IInventoryAdjustmentNoteRepository inventoryAdjustmentNoteRepository, IInventoryTransferNoteRepository inventoryTransferNoteRepository, IDispatchNoteRepository dispatchNoteRepository, IProducerRepository producerRepository, ICostCentreRepository costCentreRepository, IDocumentRepository documentRepository)
//        {
//            _commandRoutingRepository = commandRoutingRepository;
//            _objectSerialization = objectSerialization;
//            _costCentreApplicationRepository = costCentreApplicationRepository;
//            _orderRepository = orderRepository;
//            _inventoryAdjustmentNoteRepository = inventoryAdjustmentNoteRepository;
//            _inventoryTransferNoteRepository = inventoryTransferNoteRepository;
//            _dispatchNoteRepository = dispatchNoteRepository;
//            _producerRepository = producerRepository;
//            _logger = LogManager.GetLogger("CommandRoutingResolver");
//            _costCentreRepository = costCentreRepository;
//            _documentRepository = documentRepository;
//        }

//        public List<CommandRouteItem> GetCommandRoutes(ICommand command)
//        {
//            //has the command already been resolved??
//            if (_commandRoutingRepository.GetByCommandId(command.CommandId) != null)
//                return new List<CommandRouteItem>();
//            string commandType = command.GetType().ToString().Split('.').Last();
//            List<CommandRouteItem> commandRouteItems = null;

//            switch (commandType)
//            {
//                //-------------- ORDERS -----------------------

//                case "CreateOrderCommand":
//                    CreateOrderCommand createOrderCommand = command as CreateOrderCommand;
//                    commandRouteItems = GetCreateOrderCommandRoutes(createOrderCommand);
//                    break;
//                case "AddOrderLineItemCommand":
//                    AddOrderLineItemCommand addLineItemCommand = command as AddOrderLineItemCommand;
//                    commandRouteItems = GetAddOrderLineItemCommandRoutes(addLineItemCommand);
//                    break;
//                case "ConfirmOrderCommand":
//                    ConfirmOrderCommand confirmOrderCommand = command as ConfirmOrderCommand;
//                    commandRouteItems = GetConfirmOrderCommandRoutes(confirmOrderCommand);
//                    break;
//                case "RemoveOrderLineItemCommand":
//                    RemoveOrderLineItemCommand c1 = command as RemoveOrderLineItemCommand;
//                    commandRouteItems = GetRemoveOrderLineItemCommandRoutes(c1);
//                    break;
//                case "ChangeOrderLineItemCommand":
//                    ChangeOrderLineItemCommand c2 = command as ChangeOrderLineItemCommand;
//                    commandRouteItems = GetChangeORderLineItemCommandRoutes(c2);
//                    break;
//                case "ApproveOrderCommand":
//                    ApproveOrderCommand c3 = command as ApproveOrderCommand;
//                    commandRouteItems = GetApproveOrderCommandRoutes(c3);
//                    break;
//                case "CloseOrderCommand":
//                    CloseOrderCommand c4 = command as CloseOrderCommand;
//                    commandRouteItems = GetCloseOrderCommandRoutes(c4);
//                    break;
//                case "BackOrderCommand":
//                    BackOrderCommand c5 = command as BackOrderCommand;
//                    commandRouteItems = GetBackOrderCommandRoutes(c5);
//                    break;
//                case "OrderPendingDispatchCommand":
//                    OrderPendingDispatchCommand c6 = command as OrderPendingDispatchCommand;
//                    commandRouteItems = GetOrderPendingDispatchCommandRoutes(c6);
//                    break;
//                case "DispatchToPhoneCommand":
//                    DispatchToPhoneCommand c7 = command as DispatchToPhoneCommand;
//                    commandRouteItems = GetDispatchToPhoneCommandRoutes(c7);
//                    break;

//                // ------------ Inventory Received Note -----------------------

//                case "CreateInventoryReceivedNoteCommand":
//                    CreateInventoryReceivedNoteCommand c14 = command as CreateInventoryReceivedNoteCommand;
//                    commandRouteItems = GetInventoryReceivedNoteCommandRoutes(c14);
//                    break;
//                case "AddInventoryReceivedNoteLineItemCommand":
//                    AddInventoryReceivedNoteLineItemCommand c15 = command as AddInventoryReceivedNoteLineItemCommand;
//                    commandRouteItems = GetInventoryReceivedNoteCommandRoutes(c15);
//                    break;
//                case "ConfirmInventoryReceivedNoteCommand":
//                    ConfirmInventoryReceivedNoteCommand c16 = command as ConfirmInventoryReceivedNoteCommand;
//                    commandRouteItems = GetInventoryReceivedNoteCommandRoutes(c16);
//                    break;

//                // --------------- Inventory Adjustment Note -----------------------------------

//                case "CreateInventoryAdjustmentNoteCommand":
//                    CreateInventoryAdjustmentNoteCommand createInventoryAdjustmentNoteCommand = command as CreateInventoryAdjustmentNoteCommand;
//                    commandRouteItems = GetInventoryAdjustmentNoteCommandRoutes(createInventoryAdjustmentNoteCommand);
//                    break;
//                case "AddInventoryAdjustmentNoteLineItemCommand":
//                    AddInventoryAdjustmentNoteLineItemCommand addInventoryAdjustmentNoteLineItemCommand = command as AddInventoryAdjustmentNoteLineItemCommand;
//                    commandRouteItems = GetInventoryAdjustmentNoteCommandRoutes(addInventoryAdjustmentNoteLineItemCommand);
//                    break;
//                case "ConfirmInventoryAdjustmentNoteCommand":
//                    ConfirmInventoryAdjustmentNoteCommand confirInventoryAdjustmentNoteCommand = command as ConfirmInventoryAdjustmentNoteCommand;
//                    commandRouteItems = GetInventoryAdjustmentNoteCommandRoutes(confirInventoryAdjustmentNoteCommand);
//                    break;

//                //------------- Inventory Transfer Note ------------------------------------------------------

//                case "CreateInventoryTransferNoteCommand":
//                    CreateInventoryTransferNoteCommand createInventoryTransferNoteCommand = command as CreateInventoryTransferNoteCommand;
//                    commandRouteItems = GetCreateInventoryTransferNoteCommandRoutes(createInventoryTransferNoteCommand);
//                    break;
//                case "AddInventoryTransferNoteLineItemCommand":
//                    AddInventoryTransferNoteLineItemCommand addInventoryTransferNoteLineItemCommand = command as AddInventoryTransferNoteLineItemCommand;
//                    commandRouteItems = GetAddInventoryTransferNoteLineItemCommandRoutes(addInventoryTransferNoteLineItemCommand);
//                    break;
//                case "ConfirmInventoryTransferNoteCommand":
//                    ConfirmInventoryTransferNoteCommand confirInventoryTransferNoteCommand = command as ConfirmInventoryTransferNoteCommand;
//                    commandRouteItems = GetConfirmInventoryTransferNoteCommandRoutes(confirInventoryTransferNoteCommand);
//                    break;

//                // -------------- Dispatch Note -----------------------------------

//                case "CreateDispatchNoteCommand":
//                    CreateDispatchNoteCommand createDispatchNoteCommand = command as CreateDispatchNoteCommand;
//                    commandRouteItems = GetCreateDispatchNoteCommandRoutes(createDispatchNoteCommand);
//                    break;
//                case "AddDispatchNoteLineItemCommand":
//                    AddDispatchNoteLineItemCommand addDispatchNoteLineItemCommand = command as AddDispatchNoteLineItemCommand;
//                    commandRouteItems = GetAddDispatchNoteLineItemCommandRoutes(addDispatchNoteLineItemCommand);
//                    break;
//                case "ConfirmDispatchNoteCommand":
//                    ConfirmDispatchNoteCommand confirDispatchNoteCommand = command as ConfirmDispatchNoteCommand;
//                    commandRouteItems = GetConfirmDispatchNoteCommandRoutes(confirDispatchNoteCommand);
//                    break;

//                //-------------------------- Receipt -------------------------

//                case "CreateReceiptCommand":
//                    CreateReceiptCommand createReceiptCommand = command as CreateReceiptCommand;
//                    commandRouteItems = GetReceiptCommandRoutes(createReceiptCommand);
//                    break;

//                case "AddReceiptLineItemCommand":
//                    AddReceiptLineItemCommand addReceiptLineItemCommand = command as AddReceiptLineItemCommand;
//                    commandRouteItems = GetReceiptCommandRoutes(addReceiptLineItemCommand);
//                    break;

//                case "ConfirmReceipt":
//                    ConfirmReceiptCommand confirmReceiptCommand = command as ConfirmReceiptCommand;
//                    commandRouteItems = GetReceiptCommandRoutes(confirmReceiptCommand);
//                    break;


//                //-------------------------- DisbursementNote -------------------------

//                case "CreateDisbursementNoteCommand":
//                    CreateDisbursementNoteCommand createDisbursementNoteCommand = command as CreateDisbursementNoteCommand;
//                    commandRouteItems = GetDisbursementNoteCommandRoutes(createDisbursementNoteCommand);
//                    break;

//                case "AddDisbursementNoteLineItemCommand":
//                    AddDisbursementNoteLineItemCommand addDisbursementNoteLineItemCommand = command as AddDisbursementNoteLineItemCommand;
//                    commandRouteItems = GetDisbursementNoteCommandRoutes(addDisbursementNoteLineItemCommand);
//                    break;

//                case "ConfirmDisbursementNoteCommand":
//                    ConfirmDisbursementNoteCommand confirmDisbursementNoteCommand = command as ConfirmDisbursementNoteCommand;
//                    commandRouteItems = GetDisbursementNoteCommandRoutes(confirmDisbursementNoteCommand);
//                    break;

//                //-------------------------- ReturnsNote -------------------------

//                case "CreateReturnsNoteCommand":
//                    CreateReturnsNoteCommand createReturnsNoteCommand = command as CreateReturnsNoteCommand;
//                    commandRouteItems = GetReturnsNoteCommandRoutes(createReturnsNoteCommand);
//                    break;

//                case "AddReturnsNoteLineItemCommand":
//                    AddReturnsNoteLineItemCommand addReturnsNoteLineItemCommand = command as AddReturnsNoteLineItemCommand;
//                    commandRouteItems = GetReturnsNoteCommandRoutes(addReturnsNoteLineItemCommand);
//                    break;

//                case "ConfirmReturnsNoteCommand":
//                    ConfirmReturnsNoteCommand confirmReturnsNoteCommand = command as ConfirmReturnsNoteCommand;
//                    commandRouteItems = GetReturnsNoteCommandRoutes(confirmReturnsNoteCommand);
//                    break;

//                //-------------------------- InvoiceNote -------------------------

//                case "CreateInvoiceCommand":
//                    CreateInvoiceCommand createInvoiceCommand = command as CreateInvoiceCommand;
//                    commandRouteItems = GetInvoiceCommandRoutes(createInvoiceCommand);
//                    break;

//                case "AddInvoiceLineItemCommand":
//                    AddInvoiceLineItemCommand addInvoiceLineItemCommand = command as AddInvoiceLineItemCommand;
//                    commandRouteItems = GetInvoiceCommandRoutes(addInvoiceLineItemCommand);
//                    break;

//                case "ConfirmInvoiceCommand":
//                    ConfirmInvoiceCommand confirmInvoiceCommand = command as ConfirmInvoiceCommand;
//                    commandRouteItems = GetInvoiceCommandRoutes(confirmInvoiceCommand);
//                    break;

//                case "CloseInvoiceCommand":
//                    CloseInvoiceCommand closeInvoiceCommand = command as CloseInvoiceCommand;
//                    commandRouteItems = GetInvoiceCommandRoutes(closeInvoiceCommand);
//                    break;

//                default:
//                    throw new Exception(string.Format("COMMAND ROUTING FOR {0} NOT HANDLED ON SERVER", commandType));
//            }
//            return commandRouteItems;
//        }

//        List<CommandRouteItem> GetCreateOrderCommandRoutes(CreateOrderCommand createOrderCommand)
//        {
//            List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//            string serializedCommand = _objectSerialization.SerializeCommandToJSON(createOrderCommand);

//            List<Guid> destinationIds = GetDestinationApplicationIdsForCostCentres(createOrderCommand.DocumentIssuerCostCentreId, createOrderCommand.DocumentRecipientCostCentreId);

//            Guid[] _destinationIds = RemoveSourceCCAppId(destinationIds.ToArray(), createOrderCommand);


//            foreach (Guid appId in _destinationIds)
//                commandRouteItems.Add(CreateRouteItem(createOrderCommand, "CreateOrder", appId, serializedCommand));

//            return commandRouteItems;
//        }

//        private List<Guid> GetDestinationApplicationIdsForCostCentres(Guid documentIssuerCostCentreId, Guid documentRecipientCCId)
//        {
//            List<Guid> destinationIds = new List<Guid> { Guid.Empty }; //For Hosted Environment

//            List<Guid> issuerCCAppIds = GetDestinationApplicationIdsForCostCentre(documentIssuerCostCentreId);
//            destinationIds.AddRange(issuerCCAppIds);

//            List<Guid> recipientCCAppIds = new List<Guid>();
//            recipientCCAppIds = GetDestinationApplicationIdsForCostCentre(documentRecipientCCId);
//            destinationIds.AddRange(recipientCCAppIds);
//            return destinationIds;
//        }

//        private List<Guid> GetDestinationApplicationIdsForCostCentre(Guid costCentreId)
//        {
//            return _costCentreApplicationRepository
//                .GetByCostCentreId(costCentreId)
//                .Select(n => n.Id)
//                .ToList();
//        }

//        private List<Guid> GetDestinationIdsFromOrderId(Guid orderId)
//        {
//            Order o = _orderRepository.GetById(orderId) as Order;
//            return GetDestinationApplicationIdsForCostCentres(o.DocumentIssuerCostCentre.Id, o.DocumentRecipientCostCentre.Id);
//        }

//        List<CommandRouteItem> GetAddOrderLineItemCommandRoutes(AddOrderLineItemCommand addOrderLineItemCommand)
//        {
//            List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//            string serializedCommand = _objectSerialization.SerializeCommandToJSON(addOrderLineItemCommand);

//            List<Guid> ids = GetDestinationIdsFromOrderId(addOrderLineItemCommand.DocumentId);

//            Guid[] _destinationIds = RemoveSourceCCAppId(ids.ToArray(), addOrderLineItemCommand);

//            foreach (Guid id in _destinationIds)
//                commandRouteItems.Add(CreateRouteItem(addOrderLineItemCommand, "AddOrderLineItem", id, serializedCommand));

//            return commandRouteItems;
//        }

//        List<CommandRouteItem> GetConfirmOrderCommandRoutes(ConfirmOrderCommand confirmOrderCommand)
//        {
//            List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//            string serializedCommand = _objectSerialization.SerializeCommandToJSON(confirmOrderCommand);
//            List<Guid> ids = GetDestinationIdsFromOrderId(confirmOrderCommand.DocumentId);

//            Guid[] _destinationIds = RemoveSourceCCAppId(ids.ToArray(), confirmOrderCommand);

//            foreach (Guid id in _destinationIds)
//                commandRouteItems.Add(CreateRouteItem(confirmOrderCommand, "ConfirmOrder", id, serializedCommand));

//            return commandRouteItems;

//        }

//        List<CommandRouteItem> GetRemoveOrderLineItemCommandRoutes(RemoveOrderLineItemCommand removeOrderLineItemCommand)
//        {
//            List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//            string serializedCommand = _objectSerialization.SerializeCommandToJSON(removeOrderLineItemCommand);
//            List<Guid> ids = GetDestinationIdsFromOrderId(removeOrderLineItemCommand.DocumentId);

//            Guid[] _destinationIds = RemoveSourceCCAppId(ids.ToArray(), removeOrderLineItemCommand);

//            foreach (Guid id in _destinationIds)
//                commandRouteItems.Add(CreateRouteItem(removeOrderLineItemCommand, "RemoveOrderLineItem", id, serializedCommand));

//            return commandRouteItems;
//        }

//        private List<CommandRouteItem> GetChangeORderLineItemCommandRoutes(ChangeOrderLineItemCommand changeOrderLineItemCommand)
//        {
//            List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//            string serializedCommand = _objectSerialization.SerializeCommandToJSON(changeOrderLineItemCommand);
//            List<Guid> ids = GetDestinationIdsFromOrderId(changeOrderLineItemCommand.DocumentId);

//            Guid[] _destinationIds = RemoveSourceCCAppId(ids.ToArray(), changeOrderLineItemCommand);

//            foreach (Guid id in _destinationIds)
//                commandRouteItems.Add(CreateRouteItem(changeOrderLineItemCommand, "ChangeOrderLineItem", id, serializedCommand));

//            return commandRouteItems;
//        }

//        private List<CommandRouteItem> GetApproveOrderCommandRoutes(ApproveOrderCommand approveOrderCommand)
//        {
//            List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//            string serializedCommand = _objectSerialization.SerializeCommandToJSON(approveOrderCommand);
//            List<Guid> ids = GetDestinationIdsFromOrderId(approveOrderCommand.DocumentId);

//            Guid[] _destinationIds = RemoveSourceCCAppId(ids.ToArray(), approveOrderCommand);

//            foreach (Guid id in _destinationIds)
//                commandRouteItems.Add(CreateRouteItem(approveOrderCommand, "ApproveOrder", id, serializedCommand));

//            return commandRouteItems;
//        }

//        private List<CommandRouteItem> GetCloseOrderCommandRoutes(CloseOrderCommand closeOrderCommand)
//        {
//            _logger.Info("GetCloseOrderCommandRoutes, closeOrderCommand.DocumentId:" + closeOrderCommand.DocumentId);
//            List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//            string serializedCommand = _objectSerialization.SerializeCommandToJSON(closeOrderCommand);
//            _logger.Info("serializedCommand: " + serializedCommand);
//            List<Guid> ids = GetDestinationIdsFromOrderId(closeOrderCommand.DocumentId);

//            Guid[] _destinationIds = RemoveSourceCCAppId(ids.ToArray(), closeOrderCommand);

//            foreach (Guid id in _destinationIds)
//                commandRouteItems.Add(CreateRouteItem(closeOrderCommand, "CloseOrder", id, serializedCommand));

//            return commandRouteItems;
//        }

//        //IAN
//        //List<CommandRouteItem> GetCreateInventoryAdjustmentNoteCommandRoutes(CreateInventoryAdjustmentNoteCommand createInventoryAdjustmentNoteCommand)
//        //{
//        //    List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//        //    string serializedCommand = _objectSerialization.SerializeCommandToJSON(createInventoryAdjustmentNoteCommand);

//        //    List<int> destinationIds = GetDestinationApplicationIdsForCostCentres(createInventoryAdjustmentNoteCommand.DocumentIssuerCostCentreId, createInventoryAdjustmentNoteCommand.DocumentIssuerCostCentreId);

//        //    int[] _destinationIds = RemoveSourceCCAppId(destinationIds.ToArray(), createInventoryAdjustmentNoteCommand);

//        //    foreach (int appId in _destinationIds)
//        //        commandRouteItems.Add(CreateRouteItem(createInventoryAdjustmentNoteCommand, "CreateInventoryAdjustmentNote", appId, serializedCommand));

//        //    return commandRouteItems;
//        //}

//        //private List<int> GetDestinationIdsFromInventoryAdjustmentNoteId(Guid IANId)
//        //{
//        //    InventoryAdjustmentNote ian = _inventoryAdjustmentNoteRepository.GetById(IANId) as InventoryAdjustmentNote;
//        //    return GetDestinationApplicationIdsForCostCentres(ian.DocumentIssuerCostCentre.Id, ian.DocumentRecipientCostCentre.Id);
//        //}

//        //List<CommandRouteItem> GetAddInventoryAdjustmentNoteLineItemCommandRoutes(AddInventoryAdjustmentNoteLineItemCommand addInventoryAdjustmentNoteLineItemCommand)
//        //{
//        //    List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//        //    string serializedCommand = _objectSerialization.SerializeCommandToJSON(addInventoryAdjustmentNoteLineItemCommand);

//        //    List<int> ids = GetDestinationIdsFromInventoryAdjustmentNoteId(addInventoryAdjustmentNoteLineItemCommand.DocumentId);

//        //    int[] _destinationIds = RemoveSourceCCAppId(ids.ToArray(), addInventoryAdjustmentNoteLineItemCommand);

//        //    foreach (int id in _destinationIds)
//        //        commandRouteItems.Add(CreateRouteItem(addInventoryAdjustmentNoteLineItemCommand, "AddInventoryAdjustmentNoteLineItem", id, serializedCommand));
//        //    return commandRouteItems;
//        //}

//        //List<CommandRouteItem> GetConfirmInventoryAdjustmentNoteCommandRoutes(ConfirmInventoryAdjustmentNoteCommand confirmInventoryAdjustmentNoteCommand)
//        //{
//        //    List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//        //    string serializedCommand = _objectSerialization.SerializeCommandToJSON(confirmInventoryAdjustmentNoteCommand);
//        //    List<int> ids = GetDestinationIdsFromInventoryAdjustmentNoteId(confirmInventoryAdjustmentNoteCommand.DocumentId);

//        //    int[] _destinationIds = RemoveSourceCCAppId(ids.ToArray(), confirmInventoryAdjustmentNoteCommand);

//        //    foreach (int id in _destinationIds)
//        //        commandRouteItems.Add(CreateRouteItem(confirmInventoryAdjustmentNoteCommand, "ConfirmInventoryAdjustmentNote", id, serializedCommand));

//        //    return commandRouteItems;

//        //}

//        //ITN
//        List<CommandRouteItem> GetCreateInventoryTransferNoteCommandRoutes(CreateInventoryTransferNoteCommand createInventoryTransferNoteCommand)
//        {
//            List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//            string serializedCommand = _objectSerialization.SerializeCommandToJSON(createInventoryTransferNoteCommand);

//            List<Guid> destinationIds = GetDestinationApplicationIdsForCostCentres(createInventoryTransferNoteCommand.InventoryTransferNoteIssuerCostCentreId, createInventoryTransferNoteCommand.InventoryTransferNoteIssuerCostCentreId);

//            Guid[] _destinationIds = RemoveSourceCCAppId(destinationIds.ToArray(), createInventoryTransferNoteCommand);

//            foreach (Guid appId in _destinationIds)
//                commandRouteItems.Add(CreateRouteItem(createInventoryTransferNoteCommand, "CreateInventoryTransferNote", appId, serializedCommand));

//            return commandRouteItems;
//        }

//        private List<Guid> GetDestinationIdsFromInventoryTransferNoteId(Guid ITNId)
//        {
//            InventoryTransferNote itn = _inventoryAdjustmentNoteRepository.GetById(ITNId) as InventoryTransferNote;
//            return GetDestinationApplicationIdsForCostCentres(itn.DocumentIssuerCostCentre.Id, itn.DocumentRecipientCostCentre.Id);
//        }

//        List<CommandRouteItem> GetAddInventoryTransferNoteLineItemCommandRoutes(AddInventoryTransferNoteLineItemCommand addInventoryTransferNoteLineItemCommand)
//        {
//            List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//            string serializedCommand = _objectSerialization.SerializeCommandToJSON(addInventoryTransferNoteLineItemCommand);

//            List<Guid> ids = GetDestinationIdsFromInventoryTransferNoteId(addInventoryTransferNoteLineItemCommand.DocumentId);

//            Guid[] _destinationIds = RemoveSourceCCAppId(ids.ToArray(), addInventoryTransferNoteLineItemCommand);

//            foreach (Guid id in _destinationIds)
//                commandRouteItems.Add(CreateRouteItem(addInventoryTransferNoteLineItemCommand, "AddInventoryTransferNoteLineItem", id, serializedCommand));
//            return commandRouteItems;
//        }

//        List<CommandRouteItem> GetConfirmInventoryTransferNoteCommandRoutes(ConfirmInventoryTransferNoteCommand confirmInventoryTransferNoteCommand)
//        {
//            List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//            string serializedCommand = _objectSerialization.SerializeCommandToJSON(confirmInventoryTransferNoteCommand);
//            List<Guid> ids = GetDestinationIdsFromInventoryTransferNoteId(confirmInventoryTransferNoteCommand.DocumentId);

//            Guid[] _destinationIds = RemoveSourceCCAppId(ids.ToArray(), confirmInventoryTransferNoteCommand);

//            foreach (Guid id in _destinationIds)
//                commandRouteItems.Add(CreateRouteItem(confirmInventoryTransferNoteCommand, "ConfirmInventoryTransferNote", id, serializedCommand));

//            return commandRouteItems;

//        }
//        //DN
//        List<CommandRouteItem> GetCreateDispatchNoteCommandRoutes(CreateDispatchNoteCommand createDispatchNoteCommand)
//        {
//            List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//            string serializedCommand = _objectSerialization.SerializeCommandToJSON(createDispatchNoteCommand);

//            List<Guid> destinationIds = GetDestinationApplicationIdsForCostCentres(createDispatchNoteCommand.DispatchNoteIssuerCostCentreId, createDispatchNoteCommand.DispatchNoteIssuerCostCentreId);

//            Guid[] _destinationIds = RemoveSourceCCAppId(destinationIds.ToArray(), createDispatchNoteCommand);

//            foreach (Guid appId in _destinationIds)
//                commandRouteItems.Add(CreateRouteItem(createDispatchNoteCommand, "CreateDispatchNote", appId, serializedCommand));

//            return commandRouteItems;
//        }

//        private List<Guid> GetDestinationIdsFromDispatchNoteId(Guid DNId)
//        {
//            DispatchNote dn = _dispatchNoteRepository.GetById(DNId) as DispatchNote;
//            return GetDestinationApplicationIdsForCostCentres(dn.DocumentIssuerCostCentre.Id, dn.DocumentRecipientCostCentre.Id);
//        }

//        List<CommandRouteItem> GetAddDispatchNoteLineItemCommandRoutes(AddDispatchNoteLineItemCommand addDispatchNoteLineItemCommand)
//        {
//            List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//            string serializedCommand = _objectSerialization.SerializeCommandToJSON(addDispatchNoteLineItemCommand);

//            List<Guid> ids = GetDestinationIdsFromDispatchNoteId(addDispatchNoteLineItemCommand.DocumentId);

//            Guid[] _destinationIds = RemoveSourceCCAppId(ids.ToArray(), addDispatchNoteLineItemCommand);

//            foreach (Guid id in _destinationIds)
//                commandRouteItems.Add(CreateRouteItem(addDispatchNoteLineItemCommand, "AddDispatchNoteLineItem", id, serializedCommand));
//            return commandRouteItems;
//        }

//        List<CommandRouteItem> GetConfirmDispatchNoteCommandRoutes(ConfirmDispatchNoteCommand confirmDispatchNoteCommand)
//        {
//            List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//            string serializedCommand = _objectSerialization.SerializeCommandToJSON(confirmDispatchNoteCommand);
//            List<Guid> ids = GetDestinationIdsFromDispatchNoteId(confirmDispatchNoteCommand.DocumentId);

//            Guid[] _destinationIds = RemoveSourceCCAppId(ids.ToArray(), confirmDispatchNoteCommand);

//            foreach (Guid id in _destinationIds)
//                commandRouteItems.Add(CreateRouteItem(confirmDispatchNoteCommand, "ConfirmDispatchNote", id, serializedCommand));

//            return commandRouteItems;

//        }

//        public List<CommandRouteItem> GetBackOrderCommandRoutes(BackOrderCommand command)
//        {
//            List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//            string serializedCommand = _objectSerialization.SerializeCommandToJSON(command);
//            List<Guid> ids = GetDestinationIdsFromOrderId(command.DocumentId);

//            Guid[] _destinationIds = RemoveSourceCCAppId(ids.ToArray(), command);

//            foreach (Guid id in _destinationIds)
//                commandRouteItems.Add(CreateRouteItem(command, "BackOrder", id, serializedCommand));

//            return commandRouteItems;
//        }

//        public List<CommandRouteItem> GetOrderPendingDispatchCommandRoutes(OrderPendingDispatchCommand command)
//        {
//            List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//            string serializedCommand = _objectSerialization.SerializeCommandToJSON(command);
//            List<Guid> ids = GetDestinationIdsFromOrderId(command.DocumentId);

//            Guid[] _destinationIds = RemoveSourceCCAppId(ids.ToArray(), command);

//            foreach (Guid id in _destinationIds)
//                commandRouteItems.Add(CreateRouteItem(command, "OrderPendingDispatch", id, serializedCommand));

//            return commandRouteItems;
//        }

//        public List<CommandRouteItem> GetDispatchToPhoneCommandRoutes(DispatchToPhoneCommand command)
//        {
//            List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//            string serializedCommand = _objectSerialization.SerializeCommandToJSON(command);
//            List<Guid> ids = GetDestinationIdsFromOrderId(command.DocumentId);

//            Guid[] _destinationIds = RemoveSourceCCAppId(ids.ToArray(), command);

//            foreach (Guid id in _destinationIds)
//                commandRouteItems.Add(CreateRouteItem(command, "DispatchToPhone", id, serializedCommand));

//            return commandRouteItems;
//        }

//        CommandRouteItem CreateRouteItem(ICommand command, string commandType, Guid commandDestinationCCAppId, string serializedCommand)
//        {
//            int commandSequence = command.CostCentreApplicationCommandSequenceId;
//            //check costcentreapplicationcommandsequence is not zero for producer cost centre
//            if (command.CostCentreApplicationCommandSequenceId == 0)
//            {
//                Guid producerId = _producerRepository.GetProducer().Id;
//                if (command.CommandGeneratedByCostCentreId == producerId)
//                {
//                    int cs = _commandRoutingRepository.GetMaxCommandSequenceNoForCostcentreApplicationId(Guid.Empty);
//                    commandSequence = cs + 1;
//                }
//            }

//            CommandRouteItem cri = new CommandRouteItem
//            {
//                CommandId = command.CommandId,
//                DocumentId = command.DocumentId,
//                DateCommandInserted = DateTime.Now,
//                CommandDestinationCostCentreApplicationId = commandDestinationCCAppId,
//                CommandGeneratedByUserId = command.CommandGeneratedByUserId,
//                CommandGeneratedByCostCentreApplicationId = command.CommandGeneratedByCostCentreApplicationId,
//                CostCentreApplicationCommandSequenceId = commandSequence,
//                CommandType = commandType,
//                JsonCommand = serializedCommand,
//                CommandExecuted = false,
//                CommandDelivered = false,
//            };
//            return cri;
//        }

//        Guid[] RemoveSourceCCAppId(Guid[] allIds, ICommand command)
//        {
//            if (command.CommandGeneratedByCostCentreApplicationId == Guid.Empty)
//                return allIds;
//            return allIds
//                .Where(n => n != command.CommandGeneratedByCostCentreApplicationId)
//                .Distinct()
//                .ToArray();
//        }

//        public List<CommandRouteItem> GetInventoryAdjustmentNoteCommandRoutes(ICommand command)
//        {
//            string strCommand = "";
//            Guid ccId = Guid.Empty;
//            string serializedCommand = "";
//            List<Guid> ccAppIds = new List<Guid> { Guid.Empty };

//            if (command is CreateInventoryAdjustmentNoteCommand)
//            {
//                strCommand = "CreateInventoryAdjustmentNote";
//                var ic = command as CreateInventoryAdjustmentNoteCommand;
//                ccId = ic.DocumentIssuerCostCentreId;
//                serializedCommand = _objectSerialization.SerializeCommandToJSON(ic);
//            }

//            if (command is AddInventoryAdjustmentNoteLineItemCommand || command is ConfirmInventoryAdjustmentNoteCommand)
//            {
//                InventoryAdjustmentNote an = _documentRepository.GetById(command.DocumentId) as InventoryAdjustmentNote;
//                ccId = an.DocumentIssuerCostCentre.Id;
//                if (command is AddInventoryAdjustmentNoteLineItemCommand)
//                {
//                    strCommand = "AddInventoryAdjustmentNoteLineItem";
//                    serializedCommand = _objectSerialization.SerializeCommandToJSON((AddInventoryAdjustmentNoteLineItemCommand)command);
//                }
//                if (command is ConfirmInventoryAdjustmentNoteCommand)
//                {
//                    strCommand = "ConfirmInventoryAdjustmentNote";
//                    serializedCommand = _objectSerialization.SerializeCommandToJSON((ConfirmInventoryAdjustmentNoteCommand)command);

//                }
//            }

//            List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//            ccAppIds.AddRange(GetDestinationApplicationIdsForCostCentre(ccId));
//            Guid[] _destinationIds = RemoveSourceCCAppId(ccAppIds.ToArray(), command);

//            foreach (Guid id in _destinationIds)
//                commandRouteItems.Add(CreateRouteItem(command, strCommand, id, serializedCommand));

//            return commandRouteItems;

//        }

//        public List<CommandRouteItem> GetInventoryReceivedNoteCommandRoutes(ICommand command)
//        {
//            string strCommand = "";
//            Guid ccId = Guid.Empty;
//            string serializedCommand = "";
//            List<Guid> ccAppIds = new List<Guid> { Guid.Empty};

//            if (command is CreateInventoryReceivedNoteCommand)
//            {
//                strCommand = "CreateInventoryReceivedNote";
//                var ic = command as CreateInventoryReceivedNoteCommand;
//                ccId = ic.InventoryReceivedNoteIssuerCostCentreId;
//                serializedCommand = _objectSerialization.SerializeCommandToJSON(ic);
//            }
//            if (command is AddInventoryReceivedNoteLineItemCommand || command is ConfirmInventoryReceivedNoteCommand)
//            {
//                InventoryReceivedNote irn = _documentRepository.GetById(command.DocumentId) as InventoryReceivedNote;
//                ccId = irn.DocumentIssuerCostCentre.Id;
//                if (command is AddInventoryReceivedNoteLineItemCommand)
//                {
//                    strCommand = "AddInventoryReceivedNoteLineItem";
//                    serializedCommand = _objectSerialization.SerializeCommandToJSON((AddInventoryReceivedNoteLineItemCommand)command);
//                }
//                if (command is ConfirmInventoryReceivedNoteCommand)
//                {
//                    strCommand = "ConfirmInventoryReceivedNote";
//                    serializedCommand = _objectSerialization.SerializeCommandToJSON((ConfirmInventoryReceivedNoteCommand)command);

//                }
//            }
//            List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//            ccAppIds.AddRange(GetDestinationApplicationIdsForCostCentre(ccId));
//            Guid[] _destinationIds = RemoveSourceCCAppId(ccAppIds.ToArray(), command);

//            foreach (Guid id in _destinationIds)
//                commandRouteItems.Add(CreateRouteItem(command, strCommand, id, serializedCommand));

//            return commandRouteItems;
//        }

//        public List<CommandRouteItem> GetReceiptCommandRoutes(ICommand command)
//        {
//            string strCommand = "";
//            Guid ccId = Guid.Empty;
//            string serializedCommand = "";
//            List<Guid> ccAppIds = new List<Guid> { Guid.Empty };
//            if (command is CreateReceiptCommand)
//            {
//                strCommand = "CreateReceipt";
//                var ic = command as CreateReceiptCommand;
//                ccId = ic.DocumentIssuerCostCentreId;
//                serializedCommand = _objectSerialization.SerializeCommandToJSON(ic);
//            }

//            if (command is AddReceiptLineItemCommand || command is ConfirmReceiptCommand)
//            {
//                Receipt r = _documentRepository.GetById(command.DocumentId) as Receipt;
//                ccId = r.DocumentIssuerCostCentre.Id;
//                if (command is AddReceiptLineItemCommand)
//                {
//                    strCommand = "AddReceiptLineItem";
//                    serializedCommand = _objectSerialization.SerializeCommandToJSON((AddReceiptLineItemCommand)command);
//                }
//                if (command is ConfirmReceiptCommand)
//                {
//                    strCommand = "ConfirmCommand";
//                    serializedCommand = _objectSerialization.SerializeCommandToJSON((ConfirmReceiptCommand)command);
//                }
//            }
//            List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//            ccAppIds.AddRange(GetDestinationApplicationIdsForCostCentre(ccId));
//            Guid[] _destinationIds = RemoveSourceCCAppId(ccAppIds.ToArray(), command);

//            foreach (Guid id in _destinationIds)
//                commandRouteItems.Add(CreateRouteItem(command, strCommand, id, serializedCommand));

//            return commandRouteItems;
//        }

//        private List<CommandRouteItem> GetDisbursementNoteCommandRoutes(ICommand command)
//        {
//            string strCommand = "";
//            Guid ccId = Guid.Empty;
//            string serializedCommand = "";
//            List<Guid> ccAppIds = new List<Guid> { Guid.Empty };
//            if (command is CreateDisbursementNoteCommand)
//            {
//                strCommand = "CreateDisbursementNote";
//                var ic = command as CreateDisbursementNoteCommand;
//                ccId = ic.DocumentIssuerCostCentreId;
//                serializedCommand = _objectSerialization.SerializeCommandToJSON(ic);
//            }

//            if (command is AddDisbursementNoteLineItemCommand || command is ConfirmDisbursementNoteCommand)
//            {
//                DisbursementNote r = _documentRepository.GetById(command.DocumentId) as DisbursementNote;
//                ccId = r.DocumentIssuerCostCentre.Id;
//                if (command is AddDisbursementNoteLineItemCommand)
//                {
//                    strCommand = "AddDisbursementNoteLineItem";
//                    serializedCommand = _objectSerialization.SerializeCommandToJSON((AddDisbursementNoteLineItemCommand)command);
//                }
//                if (command is ConfirmDisbursementNoteCommand)
//                {
//                    strCommand = "ConfirmDisbursementNoteCommand";
//                    serializedCommand = _objectSerialization.SerializeCommandToJSON((ConfirmDisbursementNoteCommand)command);
//                }
//            }
//            List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//            ccAppIds.AddRange(GetDestinationApplicationIdsForCostCentre(ccId));
//            Guid[] _destinationIds = RemoveSourceCCAppId(ccAppIds.ToArray(), command);

//            foreach (Guid id in _destinationIds)
//                commandRouteItems.Add(CreateRouteItem(command, strCommand, id, serializedCommand));

//            return commandRouteItems;
//        }

//        public List<CommandRouteItem> GetReturnsNoteCommandRoutes(ICommand command)
//        {
//            string strCommand = "";
//            Guid ccId = Guid.Empty;
//            string serializedCommand = "";
//            List<Guid> ccAppIds = new List<Guid> { Guid.Empty };

//            if (command is CreateReturnsNoteCommand)
//            {
//                strCommand = "CreateReturnsNote";
//                var ic = command as CreateReturnsNoteCommand;
//                ccId = ic.DocumentIssuerCostCentreId;
//                serializedCommand = _objectSerialization.SerializeCommandToJSON(ic);
//            }

//            if (command is AddReturnsNoteLineItemCommand || command is ConfirmReturnsNoteCommand)
//            {
//                ReturnsNote rn = _documentRepository.GetById(command.DocumentId) as ReturnsNote;
//                ccId = rn.DocumentIssuerCostCentre.Id;
//                if (command is AddReturnsNoteLineItemCommand)
//                {
//                    strCommand = "AddReturnsNoteLineItem";
//                    serializedCommand = _objectSerialization.SerializeCommandToJSON((AddReturnsNoteLineItemCommand)command);
//                }
//                if (command is ConfirmReturnsNoteCommand)
//                {
//                    strCommand = "ConfirmReturnsNote";
//                    serializedCommand = _objectSerialization.SerializeCommandToJSON((ConfirmReturnsNoteCommand)command);
//                }
//            }

//            List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//            ccAppIds.AddRange(GetDestinationApplicationIdsForCostCentre(ccId));
//            Guid[] _destinationIds = RemoveSourceCCAppId(ccAppIds.ToArray(), command);

//            foreach (Guid id in _destinationIds)
//                commandRouteItems.Add(CreateRouteItem(command, strCommand, id, serializedCommand));

//            return commandRouteItems;

//        }

//        public List<CommandRouteItem> GetInvoiceCommandRoutes(ICommand command)
//        {
//            string strCommand = "";
//            Guid ccId = Guid.Empty;
//            string serializedCommand = "";
//            List<Guid> ccAppIds = new List<Guid> { Guid.Empty };

//            if (command is CreateInvoiceCommand)
//            {
//                strCommand = "CreateInvoice";
//                var ic = command as CreateInvoiceCommand;
//                ccId = ic.DocumentIssuerCostCentreId;
//                serializedCommand = _objectSerialization.SerializeCommandToJSON(ic);
//            }

//            if (command is AddInvoiceLineItemCommand || command is ConfirmInvoiceCommand)
//            {
//                Invoice invoice = _documentRepository.GetById(command.DocumentId) as Invoice;
//                ccId = invoice.DocumentIssuerCostCentre.Id;
//                if (command is AddInvoiceLineItemCommand)
//                {
//                    strCommand = "AddInvoiceLineItem";
//                    serializedCommand = _objectSerialization.SerializeCommandToJSON((AddInvoiceLineItemCommand)command);
//                }
//                if (command is ConfirmInvoiceCommand)
//                {
//                    strCommand = "ConfirmInvoice";
//                    serializedCommand = _objectSerialization.SerializeCommandToJSON((ConfirmInvoiceCommand)command);
//                }
//            }

//            if (command is CloseInvoiceCommand)
//            {
//                strCommand = "CloseInvoice";
//                var ic = command as CloseInvoiceCommand;
//                ccId = ic.CommandGeneratedByCostCentreId;
//                serializedCommand = _objectSerialization.SerializeCommandToJSON(ic);
//            }

//            List<CommandRouteItem> commandRouteItems = new List<CommandRouteItem>();
//            ccAppIds.AddRange(GetDestinationApplicationIdsForCostCentre(ccId));
//            Guid[] _destinationIds = RemoveSourceCCAppId(ccAppIds.ToArray(), command);

//            foreach (Guid id in _destinationIds)
//                commandRouteItems.Add(CreateRouteItem(command, strCommand, id, serializedCommand));

//            return commandRouteItems;

//        }
//    }
//}
