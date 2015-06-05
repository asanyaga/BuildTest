using System;
using Distributr.Core.Commands.DocumentCommands.DispatchNotes;
using Distributr.Core.Commands.DocumentCommands.Invoices;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.Outlets;
using Distributr.Mobile.Core.Products;
using Distributr.Mobile.Core.Sync.Incoming;
using Distributr.Mobile.Core.Test.OrderSale;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.Sync.Incoming
{
    public class IncomingCommandHandlerTest
    {
        public const string ApproveOrderLineItemCommandJson = @"{ ""CommandTypeRef"": ""ApproveOrderLineItem"", ""LineItemId"": ""@LineItemId@"", ""ApprovedQuantity"": @ApprovedQuantity@, ""LossSaleQuantity"": 0.0, ""CommandId"": ""dc7094f5-f5a7-44b8-a092-48a25d9e7c99"", ""PDCommandId"": ""@PDCommandId@"", ""DocumentId"": ""ef1a5aba-fa71-4a06-aa7c-90c6088861cf"", ""CommandGeneratedByUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""CommandGeneratedByCostCentreId"": ""4c243448-8e87-4454-8a87-23f6706eb3ab"", ""CostCentreApplicationCommandSequenceId"": 0, ""CommandGeneratedByCostCentreApplicationId"": ""c60f5426-c165-4649-bfc6-a0f8ef552612"", ""SendDateTime"": ""0001-01-01T00:00:00"", ""CommandSequence"": 0, ""CommandCreatedDateTime"": ""2015-05-02T15:04:24.3353542+01:00"", ""Longitude"": null, ""Latitude"": null, ""Description"": null, ""IsSystemCommand"": false }";
        public const string AddMainOrderLineItemCommandJson = @"{ ""LineItemSequenceNo"": 0, ""ValueLineItem"": @ValueLineItem@, ""ProductDiscount"": 3.0, ""ProductId"": ""@ProductId@"", ""Qty"": @Quantity@, ""LineItemVatValue"": @LineItemVatValue@, ""LineItemType"": 1, ""DiscountType"": 0, ""CommandTypeRef"": ""AddMainOrderLineItem"", ""CommandId"": ""67cc8914-71fe-470f-ae8f-91d32e12744b"", ""PDCommandId"": ""@PDCommandId@"", ""DocumentId"": ""bd108fb0-a886-439d-a060-6ea4a2b606a6"", ""CommandGeneratedByUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""CommandGeneratedByCostCentreId"": ""4c243448-8e87-4454-8a87-23f6706eb3ab"", ""CostCentreApplicationCommandSequenceId"": 0, ""CommandGeneratedByCostCentreApplicationId"": ""c60f5426-c165-4649-bfc6-a0f8ef552612"", ""SendDateTime"": ""0001-01-01T00:00:00"", ""CommandSequence"": 0, ""CommandCreatedDateTime"": ""2015-05-02T14:02:59.2519785+01:00"", ""Longitude"": null, ""Latitude"": null, ""Description"": """", ""IsSystemCommand"": false }";
        public const string ApproveMainOrderCommandJson = @" { ""DateApproved"": ""2015-05-03T16:11:19.107568+01:00"", ""ApproverUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""CommandTypeRef"": ""ApproveMainOrder"", ""CommandId"": ""19972834-bdc3-4083-b458-740f585de823"", ""PDCommandId"": ""@PDCommandId@"", ""DocumentId"": ""94a1fb8d-be80-4e9b-9ebb-24e52464ca30"", ""CommandGeneratedByUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""CommandGeneratedByCostCentreId"": ""4c243448-8e87-4454-8a87-23f6706eb3ab"", ""CostCentreApplicationCommandSequenceId"": 0, ""CommandGeneratedByCostCentreApplicationId"": ""c60f5426-c165-4649-bfc6-a0f8ef552612"", ""SendDateTime"": ""0001-01-01T00:00:00"", ""CommandSequence"": 0, ""CommandCreatedDateTime"": ""2015-05-03T16:11:19.107568+01:00"", ""Longitude"": null, ""Latitude"": null, ""Description"": null, ""IsSystemCommand"": false }";
        public const string CreateInvoiceCommand = @"{ ""DocumentRecipientCostCentreId"": ""4c243448-8e87-4454-8a87-23f6706eb3ab"", ""OrderId"": ""3928f84c-e04f-4a57-ba35-4fdbf328da97"", ""SaleDiscount"": 0.0, ""CommandTypeRef"": ""CreateInvoice"", ""DocumentDateIssued"": ""2015-05-02T14:02:59.2619929+01:00"", ""DocumentIssuerCostCentreId"": ""51a6bde4-d645-43fa-b81b-3708f87a5e6d"", ""DocIssuerUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""DocumentReference"": ""I_John_O005_20150502_020259_00077"", ""ExtDocumentReference"": null, ""VersionNumber"": ""H-2.0.0.8"", ""CommandId"": ""ffb36d4a-4119-4c23-9d69-7bacc73bfad1"", ""PDCommandId"": ""@PDCommandId@"", ""DocumentId"": ""82c907a5-4b5b-4a53-b6b2-0a866ec3dbf8"", ""CommandGeneratedByUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""CommandGeneratedByCostCentreId"": ""51a6bde4-d645-43fa-b81b-3708f87a5e6d"", ""CostCentreApplicationCommandSequenceId"": 0, ""CommandGeneratedByCostCentreApplicationId"": ""c60f5426-c165-4649-bfc6-a0f8ef552612"", ""SendDateTime"": ""0001-01-01T00:00:00"", ""CommandSequence"": 0, ""CommandCreatedDateTime"": ""2015-05-02T14:02:59.2619929+01:00"", ""Longitude"": null, ""Latitude"": null, ""Description"": null, ""IsSystemCommand"": false }";
        public const string OrderDispatchApprovedLineItemsCommand = @"{ ""CommandTypeRef"": ""OrderDispatchApprovedLineItems"", ""CommandId"": ""4f615554-b527-463b-b656-fae2da152ad5"", ""PDCommandId"": ""@PDCommandId@"", ""DocumentId"": ""3928f84c-e04f-4a57-ba35-4fdbf328da97"", ""CommandGeneratedByUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""CommandGeneratedByCostCentreId"": ""4c243448-8e87-4454-8a87-23f6706eb3ab"", ""CostCentreApplicationCommandSequenceId"": 0, ""CommandGeneratedByCostCentreApplicationId"": ""c60f5426-c165-4649-bfc6-a0f8ef552612"", ""SendDateTime"": ""0001-01-01T00:00:00"", ""CommandSequence"": 0, ""CommandCreatedDateTime"": ""2015-05-02T14:03:07.2341417+01:00"", ""Longitude"": null, ""Latitude"": null, ""Description"": null, ""IsSystemCommand"": false }";
        public const string CreateMainOrderCommandJson = @"{ ""DateOrderRequired"": ""0001-01-01T00:00:00"", ""IssuedOnBehalfOfCostCentreId"": ""025815c3-f7fb-4448-af4a-f30d77d743d8"", ""DocumentRecipientCostCentreId"": ""51a6bde4-d645-43fa-b81b-3708f87a5e6d"", ""OrderTypeId"": 3, ""Note"": ""The Note"", ""SaleDiscount"": 100.0, ""OrderStatusId"": 0, ""ParentId"": ""c1c12d61-e20f-4ace-89e6-ad978c5c201e"", ""ShipToAddress"": ""The Ship To Address"", ""StockistId"": ""00000000-0000-0000-0000-000000000000"", ""VisitId"": ""c99a6c94-4890-4c13-9c1f-33ae658b517c"", ""CommandTypeRef"": ""CreateMainOrder"", ""DocumentDateIssued"": ""2015-05-01T14:00:32"", ""DocumentIssuerCostCentreId"": ""4c243448-8e87-4454-8a87-23f6706eb3ab"", ""DocIssuerUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""DocumentReference"": ""O_John_O002_20150501_140032_00001"", ""ExtDocumentReference"": null, ""VersionNumber"": null, ""CommandId"": ""587c2953-3325-4da8-ba31-b8955060af42"", ""PDCommandId"": ""@PDCommandId@"", ""DocumentId"": ""c1c12d61-e20f-4ace-89e6-ad978c5c201e"", ""CommandGeneratedByUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""CommandGeneratedByCostCentreId"": ""4c243448-8e87-4454-8a87-23f6706eb3ab"", ""CostCentreApplicationCommandSequenceId"": 0, ""CommandGeneratedByCostCentreApplicationId"": ""8968fe88-fbfc-44e8-8398-fc1aaac5bff2"", ""SendDateTime"": ""2015-05-01T14:00:32"", ""CommandSequence"": 0, ""CommandCreatedDateTime"": ""2015-05-01T14:00:32"", ""Longitude"": null, ""Latitude"": null, ""Description"": null, ""IsSystemCommand"": false }";

        private Mock<ISaleProductRepository> saleProductRepository;
        private Mock<ISaleRepository> orderRepository;
        private Mock<IOutletRepository> outletRepository;
        private Mock<IInventoryRepository> inventoryRepository;
        private Mock<IReturnableProductRepository> returnableProductRepository;
        private MockOrderBuilder orderBuilder;
        private IncomingCommandHandler commandHandler;

        [SetUp]
        public void Setup()
        {
            orderBuilder = new MockOrderBuilder();
            saleProductRepository = new Mock<ISaleProductRepository>();
            orderRepository = new Mock<ISaleRepository>();
            outletRepository = new Mock<IOutletRepository>();
            inventoryRepository = new Mock<IInventoryRepository>();
            returnableProductRepository = new Mock<IReturnableProductRepository>();

            commandHandler = new IncomingCommandHandler(orderRepository.Object, outletRepository.Object, saleProductRepository.Object, inventoryRepository.Object, returnableProductRepository.Object);
        }

        public static ApproveOrderLineItemCommand CreateApproveOrderLineItemCommand(Guid orderId, Guid lineItemId, decimal approvedQuantity = 1m)
        {
            return JsonConvert.DeserializeObject<ApproveOrderLineItemCommand>(
                ApproveOrderLineItemCommandJson
                    .Replace("@LineItemId@", lineItemId.ToString())
                    .Replace("@PDCommandId@", orderId.ToString())
                    .Replace("@ApprovedQuantity@", approvedQuantity.ToString()));
        }

        public static AddMainOrderLineItemCommand CreateAddMainOrderLineItemCommand(Guid orderId, Guid productId, decimal quantity = 1m, 
            decimal lineItemValue = 100m, decimal lineItemVatValue = 10)
        {
            return JsonConvert.DeserializeObject<AddMainOrderLineItemCommand>(
                AddMainOrderLineItemCommandJson
                    .Replace("@ProductId@", productId.ToString())
                    .Replace("@PDCommandId@", orderId.ToString())
                    .Replace("@ValueLineItem@", lineItemValue.ToString())
                    .Replace("@LineItemVatValue@", lineItemVatValue.ToString())
                    .Replace("@Quantity@", quantity.ToString()));
        }

        public static ApproveMainOrderCommand CreateApproveMainOrderCommand(Guid orderId)
        {
            return JsonConvert.DeserializeObject<ApproveMainOrderCommand>(
                ApproveMainOrderCommandJson
                    .Replace("@PDCommandId@", orderId.ToString()));
        }

        public static CreateInvoiceCommand CreateCreateInvoiceCommand(Guid orderId)
        {
            return JsonConvert.DeserializeObject<CreateInvoiceCommand>(
                CreateInvoiceCommand
                    .Replace("@PDCommandId@", orderId.ToString()));
        }

        public static OrderDispatchApprovedLineItemsCommand CreateOrderDispatchApprovedLineItemsCommand(Guid orderId)
        {
            return JsonConvert.DeserializeObject<OrderDispatchApprovedLineItemsCommand>(
                OrderDispatchApprovedLineItemsCommand
                    .Replace("@PDCommandId@", orderId.ToString()));
        }


        public static CreateMainOrderCommand CreateCreateMainOrderCommand(Guid orderId)
        {
            return JsonConvert.DeserializeObject<CreateMainOrderCommand>(
                CreateMainOrderCommandJson
                    .Replace("@PDCommandId@", orderId.ToString()));
        }

        [Test]
        public void CanApproveLineItem()
        {
            //Given
            var order = orderBuilder
                .WithOrderLineItem(quantity: 1)
                .Build();

            orderRepository.Setup(o => o.FindById(order.Id)) 
                .Returns(order);

            var approveCommand = CreateApproveOrderLineItemCommand(order.Id, order.LineItems[0].Id);

            //When
            commandHandler.Init(order.Id);
            commandHandler.Handle(approveCommand);
            commandHandler.Save();

            //Then
            Assert.AreEqual(1, order.LineItems[0].SaleQuantity, "Approved Quantity After");
            orderRepository.Verify(o => o.Save(order, null), Times.Once());
        }

        [Test]
        public void ApproveCommandUpdatesCurrentApprovedQuantity()
        {
            //Given
            var order = orderBuilder
                .WithOrderLineItem(quantity: 2)
                .Build();

            orderRepository.Setup(o => o.FindById(order.Id)) 
                .Returns(order);

            var approveCommand = CreateApproveOrderLineItemCommand(order.Id, order.LineItems[0].Id);

            //When
            commandHandler.Init(order.Id);
            commandHandler.Handle(approveCommand);
            commandHandler.Save();

            //Then
            Assert.AreEqual(1, order.LineItems[0].SaleQuantity, "Approved Quantity After");
            Assert.AreEqual(LineItemStatus.Approved, order.LineItems[0].LineItemStatus, "processing status");
            orderRepository.Verify(o => o.Save(order, null), Times.Once());
        }

        [Test]
        public void CanHandleApproveOrder()
        {
            //Given 
            var order = orderBuilder
                .WithOrderLineItem(quantity: 2)
                .Build();

            orderRepository.Setup(o => o.FindById(order.Id)) 
                .Returns(order);

            var approveOrderCommand = CreateApproveMainOrderCommand(order.Id);

            //When 
            commandHandler.Init(order.Id);
            commandHandler.Handle(approveOrderCommand);
            commandHandler.Save();

            //Then
            Assert.AreEqual(ProcessingStatus.Approved, order.ProcessingStatus, "processing status");
            orderRepository.Verify(o => o.Save(order, null), Times.Once());
        }

        [Test]
        public void CanHandleCreateInvoiceCommand()
        {
            //Given 
            var order = orderBuilder
                .WithOrderLineItem(quantity: 2)
                .Build();

            orderRepository.Setup(o => o.FindById(order.Id))
                .Returns(order);

            var createInvoiceCommand = CreateCreateInvoiceCommand(order.Id);

            //Then
            commandHandler.Init(order.Id);
            commandHandler.Handle(createInvoiceCommand);
            commandHandler.Save();

            //Copied from command above
            Assert.AreEqual("I_John_O005_20150502_020259_00077", order.InvoiceReference, "invoice reference");
            orderRepository.Verify(o => o.Save(order, null), Times.Once());
        }

        [Test]
        public void CanHandleDispatchApprovedLineItemsCommand()
        {
            //Given 
            var order = orderBuilder
                .WithOrderLineItem(quantity: 2)
                .Build();

            orderRepository.Setup(o => o.FindById(order.Id))
                .Returns(order);

            var dispatchApprovedItemsCommand = CreateOrderDispatchApprovedLineItemsCommand(order.Id);

            //When 
            commandHandler.Init(order.Id);
            commandHandler.Handle(dispatchApprovedItemsCommand);
            commandHandler.Save();

            //Then
            Assert.AreEqual(ProcessingStatus.Deliverable, order.ProcessingStatus, "processing status");
            orderRepository.Verify(o => o.Save(order, null), Times.Once());
        }

        [Test]
        public void CanAddNewLineItem()
        {
            //Given 
            var order = orderBuilder.Build();

            var product = MockOrderBuilder.AProductWithPrice(100);

            saleProductRepository.Setup(s => s.FindById(product.Id))
                .Returns(product);

            orderRepository.Setup(o => o.FindById(order.Id))
                .Returns(order);

            var addLineItemCommand = CreateAddMainOrderLineItemCommand(order.Id, product.Id, quantity: 2, lineItemValue:27m, lineItemVatValue: 5.4m);

            //When
            commandHandler.Init(order.Id);
            commandHandler.Handle(addLineItemCommand);
            commandHandler.Save();

            //Then
            Assert.AreEqual(1, order.LineItems.Count, "line items count");
            Assert.AreEqual(2, order.LineItems[0].Quantity, "line item quantity");
            Assert.AreEqual(27m, order.LineItems[0].Price, "line items price");
            Assert.AreEqual(54m, order.LineItems[0].Value, "line items value");
            Assert.AreEqual(10.8m, order.LineItems[0].VatValue, "line items vat value");
            Assert.AreEqual(0.20, order.LineItems[0].VatRate, "line items vat rate");
            orderRepository.Verify(o => o.Save(order, null), Times.Once());
        }

        [Test]
        public void CanAddNewReturnanleLineItem()
        {
            //Given 
            var order = orderBuilder.Build();

            var returnableProduct = MockOrderBuilder.AProductWithPrice(15m).ReturnableProduct;

            saleProductRepository.Setup(s => s.FindById(returnableProduct.Id))
                .Returns(default(SaleProduct));

            orderRepository.Setup(o => o.FindById(order.Id))
                .Returns(order);
            
            var addLineItemCommand = CreateAddMainOrderLineItemCommand(order.Id, returnableProduct.Id, quantity: 2, lineItemValue: 15m, lineItemVatValue: 0);
            
            //When
            commandHandler.Init(order.Id);
            commandHandler.Handle(addLineItemCommand);
            commandHandler.Save();


            Assert.AreEqual(1, order.ReturnableLineItems.Count, "line items count");
            Assert.AreEqual(2, order.ReturnableLineItems[0].Quantity, "line item quantity");
            Assert.AreEqual(15m, order.ReturnableLineItems[0].Price, "line items price");
            Assert.AreEqual(30m, order.ReturnableLineItems[0].Value, "line items value");
            orderRepository.Verify(o => o.Save(order, null), Times.Once());
        }

        [Test]
        public void CanHandleCreateMainOrderCommand()
        {
            //Given 
            var parentGuid = Guid.NewGuid();

            orderRepository.Setup(o => o.FindById(parentGuid))
                .Returns(default(Sale));

            var outlet = new Outlet(new Guid("025815c3-f7fb-4448-af4a-f30d77d743d8"));

            outletRepository.Setup(o => o.GetById(outlet.Id, false))
                .Returns(outlet);

            var createOrderCommand = CreateCreateMainOrderCommand(parentGuid);
            
            //When 
            commandHandler.Init(parentGuid);
            commandHandler.Handle(createOrderCommand);
            commandHandler.Save();

            //Then
            orderRepository.Verify(r => r.Save(It.Is<Sale>(
                o => o.Id == parentGuid
                && o.ShipToAddress == "The Ship To Address"
                && o.Note == "The Note"
                && o.OrderReference == "O_John_O002_20150501_140032_00001"
                && o.Outlet == outlet
                ), null), Times.Once());
        }

        [Test]
        public void IncludesReturnablesInSaleWhenReceivingDispatchNote()
        {
            //Given 
            var order = orderBuilder
                .WithSaleLineItem()
                .Build();

            orderRepository.Setup(o => o.FindById(order.Id))
                .Returns(order);

            var returnable = order.ReturnableLineItems[0];

            //When
            commandHandler.Init(order.Id);
            commandHandler.Handle(new CreateDispatchNoteCommand() {DispatchNoteType = 2});
            commandHandler.Handle(new AddDispatchNoteLineItemCommand(){ProductId = returnable.ProductMasterId, Qty = returnable.Quantity});

            //Then
            Assert.AreEqual(LineItemStatus.Approved, returnable.LineItemStatus, "status");
            Assert.AreEqual(returnable.Quantity, returnable.SaleQuantity, "quantity");            
        }

        [Test]
        public void CanHandleChangeItemCommand()
        {
            //Given 
            var order = orderBuilder
                .WithSaleLineItem()
                .Build();

            orderRepository.Setup(o => o.FindById(order.Id))
                .Returns(order);

            var item = order.LineItems[0];

            //When
            commandHandler.Init(order.Id);
            commandHandler.Handle(new ChangeMainOrderLineItemCommand() { LineItemId = item.Id, NewQuantity = 100});

            //Then
            Assert.AreEqual(item.Quantity, 100, "Quantity");
        }


        [Test]
        public void CanHandleRemoveItemCommand()
        {
            //Given 
            var order = orderBuilder
                .WithSaleLineItem()
                .Build();

            orderRepository.Setup(o => o.FindById(order.Id))
                .Returns(order);

            var item = order.LineItems[0];

            //When
            commandHandler.Init(order.Id);
            commandHandler.Handle(new RemoveMainOrderLineItemCommand() { LineItemId = item.Id});

            //Then
            Assert.AreEqual(0, order.LineItems.Count, "Quantity");
        }
    }
}

