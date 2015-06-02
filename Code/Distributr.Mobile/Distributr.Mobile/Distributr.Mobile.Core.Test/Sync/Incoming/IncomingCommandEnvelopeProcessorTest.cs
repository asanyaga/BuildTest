using System;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.Net;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.Sync.Incoming;
using Distributr.Mobile.Core.Test.Support;
using NUnit.Framework;
using SQLiteNetExtensions.Extensions;

namespace Distributr.Mobile.Core.Test.Sync.Incoming
{
    [TestFixture]
    public class IncomingCommandEnvelopeProcessorTest : WithFakeServerTest
    {
        //See Data\Distributr_v3_0_0\MakeOrderWithoutPayment\Part2_Hub_Dispatches_Order\Order.json
        public const string ApproveOrderEnvelopeWithTwoLineItems = @"{ ""Envelopes"": [ { ""DocumentType"": ""Order"", ""Envelope"": { ""Id"": ""1e2b7adc-432c-4e0a-a220-175e96428e96"", ""DocumentId"": ""@PDCommandId@"", ""DocumentTypeId"": 1, ""GeneratedByCostCentreId"": ""4c243448-8e87-4454-8a87-23f6706eb3ab"", ""RecipientCostCentreId"": ""51a6bde4-d645-43fa-b81b-3708f87a5e6d"", ""GeneratedByCostCentreApplicationId"": ""c60f5426-c165-4649-bfc6-a0f8ef552612"", ""ParentDocumentId"": ""@PDCommandId@"", ""CommandsList"": [ { ""Command"": { ""CommandTypeRef"": ""ApproveOrderLineItem"", ""LineItemId"": ""@LineItemOneId@"", ""ApprovedQuantity"": 2.0, ""LossSaleQuantity"": 0.0, ""CommandId"": ""c51f2bc2-09ac-4da1-8988-133552139bce"", ""PDCommandId"": ""@PDCommandId@"", ""DocumentId"": ""@PDCommandId@"", ""CommandGeneratedByUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""CommandGeneratedByCostCentreId"": ""4c243448-8e87-4454-8a87-23f6706eb3ab"", ""CostCentreApplicationCommandSequenceId"": 0, ""CommandGeneratedByCostCentreApplicationId"": ""c60f5426-c165-4649-bfc6-a0f8ef552612"", ""SendDateTime"": ""0001-01-01T00:00:00"", ""CommandSequence"": 0, ""CommandCreatedDateTime"": ""2015-04-15T18:57:15.0590899+01:00"", ""Longitude"": null, ""Latitude"": null, ""Description"": null, ""IsSystemCommand"": false }, ""Order"": 1 }, { ""Command"": { ""CommandTypeRef"": ""ApproveOrderLineItem"", ""LineItemId"": ""@LineItemTwoId@"", ""ApprovedQuantity"": 2.0, ""LossSaleQuantity"": 0.0, ""CommandId"": ""e4041b5d-916a-408d-88a5-69837243a763"", ""PDCommandId"": ""@PDCommandId@"", ""DocumentId"": ""@PDCommandId@"", ""CommandGeneratedByUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""CommandGeneratedByCostCentreId"": ""4c243448-8e87-4454-8a87-23f6706eb3ab"", ""CostCentreApplicationCommandSequenceId"": 0, ""CommandGeneratedByCostCentreApplicationId"": ""c60f5426-c165-4649-bfc6-a0f8ef552612"", ""SendDateTime"": ""0001-01-01T00:00:00"", ""CommandSequence"": 0, ""CommandCreatedDateTime"": ""2015-04-15T18:57:15.0590899+01:00"", ""Longitude"": null, ""Latitude"": null, ""Description"": null, ""IsSystemCommand"": false }, ""Order"": 2 }, { ""Command"": { ""DateApproved"": ""2015-04-15T18:57:15.0590899+01:00"", ""ApproverUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""CommandTypeRef"": ""ApproveMainOrder"", ""CommandId"": ""62055332-46d3-4ba6-a25d-2404f60e1186"", ""PDCommandId"": ""@PDCommandId@"", ""DocumentId"": ""b6537d37-62ac-4099-96d8-f0c65983f55d"", ""CommandGeneratedByUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""CommandGeneratedByCostCentreId"": ""4c243448-8e87-4454-8a87-23f6706eb3ab"", ""CostCentreApplicationCommandSequenceId"": 0, ""CommandGeneratedByCostCentreApplicationId"": ""c60f5426-c165-4649-bfc6-a0f8ef552612"", ""SendDateTime"": ""0001-01-01T00:00:00"", ""CommandSequence"": 0, ""CommandCreatedDateTime"": ""2015-04-15T18:57:15.0590899+01:00"", ""Longitude"": null, ""Latitude"": null, ""Description"": null, ""IsSystemCommand"": false }, ""Order"": 3 } ], ""EnvelopeGeneratedTick"": 635647210350590899, ""EnvelopeArrivedAtServerTick"": 635647210521050067, ""IsSystemEnvelope"": false }, ""EnvelopeArrivedAtServerTick"": 635647210521050067 } ], ""ErrorInfo"": ""Success"" }";
        //See Data\Distributr_v3_0_0\MakeOrderWithoutPayment\Part2_Hub_Dispatches_Order\Invoice.json
        public const string InvoiceEnvelopeWithTwoLinesItems = @"{ ""Envelopes"": [ { ""DocumentType"": ""Invoice"", ""Envelope"": { ""Id"": ""d14d42a5-87ca-4979-bf25-e6d19cae9e18"", ""DocumentId"": ""33a11b44-024a-4e2d-ad9d-6d283435bd28"", ""DocumentTypeId"": 5, ""GeneratedByCostCentreId"": ""51a6bde4-d645-43fa-b81b-3708f87a5e6d"", ""RecipientCostCentreId"": ""4c243448-8e87-4454-8a87-23f6706eb3ab"", ""GeneratedByCostCentreApplicationId"": ""c60f5426-c165-4649-bfc6-a0f8ef552612"", ""ParentDocumentId"": ""@PDCommandId@"", ""CommandsList"": [ { ""Command"": { ""DocumentRecipientCostCentreId"": ""4c243448-8e87-4454-8a87-23f6706eb3ab"", ""OrderId"": ""@PDCommandId@"", ""SaleDiscount"": 0.0, ""CommandTypeRef"": ""CreateInvoice"", ""DocumentDateIssued"": ""2015-04-15T18:57:15.0691043+01:00"", ""DocumentIssuerCostCentreId"": ""51a6bde4-d645-43fa-b81b-3708f87a5e6d"", ""DocIssuerUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""DocumentReference"": ""I_John_O003_20150415_065715_00021"", ""ExtDocumentReference"": null, ""VersionNumber"": ""H-2.0.0.8"", ""CommandId"": ""17527340-4214-4fa1-82d2-dca34cbddfdc"", ""PDCommandId"": ""@PDCommandId@"", ""DocumentId"": ""33a11b44-024a-4e2d-ad9d-6d283435bd28"", ""CommandGeneratedByUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""CommandGeneratedByCostCentreId"": ""51a6bde4-d645-43fa-b81b-3708f87a5e6d"", ""CostCentreApplicationCommandSequenceId"": 0, ""CommandGeneratedByCostCentreApplicationId"": ""c60f5426-c165-4649-bfc6-a0f8ef552612"", ""SendDateTime"": ""0001-01-01T00:00:00"", ""CommandSequence"": 0, ""CommandCreatedDateTime"": ""2015-04-15T18:57:15.0691043+01:00"", ""Longitude"": null, ""Latitude"": null, ""Description"": null, ""IsSystemCommand"": false }, ""Order"": 1 }, { ""Command"": { ""LineItemSequenceNo"": 0, ""ValueLineItem"": 22.0, ""ProductId"": ""c8a07a84-fd98-44cf-86e2-e8983dc81ed6"", ""Qty"": 2.0, ""LineItemVatValue"": 6.6, ""LineItemProductDiscount"": 3.0, ""LineItemId"": ""@LineItemOneId@"", ""LineItemType"": 0, ""DiscountType"": 0, ""CommandTypeRef"": ""AddInvoiceLineItem"", ""CommandId"": ""5f1243b7-188c-4f74-af6b-687e86e181fb"", ""PDCommandId"": ""@PDCommandId@"", ""DocumentId"": ""33a11b44-024a-4e2d-ad9d-6d283435bd28"", ""CommandGeneratedByUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""CommandGeneratedByCostCentreId"": ""51a6bde4-d645-43fa-b81b-3708f87a5e6d"", ""CostCentreApplicationCommandSequenceId"": 0, ""CommandGeneratedByCostCentreApplicationId"": ""c60f5426-c165-4649-bfc6-a0f8ef552612"", ""SendDateTime"": ""0001-01-01T00:00:00"", ""CommandSequence"": 0, ""CommandCreatedDateTime"": ""2015-04-15T18:57:15.0691043+01:00"", ""Longitude"": null, ""Latitude"": null, ""Description"": ""O_john_O003_20150415_185553_00008"", ""IsSystemCommand"": false }, ""Order"": 2 }, { ""Command"": { ""LineItemSequenceNo"": 0, ""ValueLineItem"": 15.0, ""ProductId"": ""faa82a9a-a10f-4e24-bc8c-53739d382127"", ""Qty"": 2.0, ""LineItemVatValue"": 0.0, ""LineItemProductDiscount"": 0.0, ""LineItemId"": ""@LineItemTwoId@"", ""LineItemType"": 0, ""DiscountType"": 0, ""CommandTypeRef"": ""AddInvoiceLineItem"", ""CommandId"": ""054a1fcf-f82e-4a6e-9624-b6659444b4bf"", ""PDCommandId"": ""@PDCommandId@"", ""DocumentId"": ""33a11b44-024a-4e2d-ad9d-6d283435bd28"", ""CommandGeneratedByUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""CommandGeneratedByCostCentreId"": ""51a6bde4-d645-43fa-b81b-3708f87a5e6d"", ""CostCentreApplicationCommandSequenceId"": 0, ""CommandGeneratedByCostCentreApplicationId"": ""c60f5426-c165-4649-bfc6-a0f8ef552612"", ""SendDateTime"": ""0001-01-01T00:00:00"", ""CommandSequence"": 0, ""CommandCreatedDateTime"": ""2015-04-15T18:57:15.0691043+01:00"", ""Longitude"": null, ""Latitude"": null, ""Description"": ""O_john_O003_20150415_185553_00008"", ""IsSystemCommand"": false }, ""Order"": 3 }, { ""Command"": { ""CommandTypeRef"": ""ConfirmInvoice"", ""CommandId"": ""d615f806-1d33-4b43-8cda-d018f1b4820d"", ""PDCommandId"": ""b6537d37-62ac-4099-96d8-f0c65983f55d"", ""DocumentId"": ""33a11b44-024a-4e2d-ad9d-6d283435bd28"", ""CommandGeneratedByUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""CommandGeneratedByCostCentreId"": ""51a6bde4-d645-43fa-b81b-3708f87a5e6d"", ""CostCentreApplicationCommandSequenceId"": 0, ""CommandGeneratedByCostCentreApplicationId"": ""c60f5426-c165-4649-bfc6-a0f8ef552612"", ""SendDateTime"": ""0001-01-01T00:00:00"", ""CommandSequence"": 0, ""CommandCreatedDateTime"": ""2015-04-15T18:57:15.0691043+01:00"", ""Longitude"": null, ""Latitude"": null, ""Description"": null, ""IsSystemCommand"": false }, ""Order"": 4 } ], ""EnvelopeGeneratedTick"": 635647210350691043, ""EnvelopeArrivedAtServerTick"": 635647210519948483, ""IsSystemEnvelope"": false }, ""EnvelopeArrivedAtServerTick"": 635647210519948483 } ], ""ErrorInfo"": ""Success"" }";
        //See Data\Distributr_v3_0_0\MakeOrderWithoutPayment\Part2_Hub_Dispatches_Order\InventoryTransferNote.json
        public const string InventoryTransferNoteWithTwoLineItems = @"{ ""Envelopes"": [ { ""DocumentType"": ""InventoryTransferNote"", ""Envelope"": { ""Id"": ""b7efa038-7f0a-4da1-a22c-2ad6ef58a056"", ""DocumentId"": ""b75b9676-0f0f-456e-946b-ccbef5a531b0"", ""DocumentTypeId"": 4, ""GeneratedByCostCentreId"": ""53c69073-d408-487f-aa2e-195533d0400d"", ""RecipientCostCentreId"": ""4c243448-8e87-4454-8a87-23f6706eb3ab"", ""GeneratedByCostCentreApplicationId"": ""c60f5426-c165-4649-bfc6-a0f8ef552612"", ""ParentDocumentId"": ""@PDCommandId@"", ""CommandsList"": [ { ""Command"": { ""InventoryTransferNoteIssuedOnBehalfOfUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""IssuedOnBehalfOfCostCentreId"": ""53c69073-d408-487f-aa2e-195533d0400d"", ""DocumentRecipientCostCentreId"": ""4c243448-8e87-4454-8a87-23f6706eb3ab"", ""CommandTypeRef"": ""CreateInventoryTransferNote"", ""DocumentDateIssued"": ""2015-04-15T18:57:22.7293459+01:00"", ""DocumentIssuerCostCentreId"": ""53c69073-d408-487f-aa2e-195533d0400d"", ""DocIssuerUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""DocumentReference"": ""O_john_O003_20150415_185553_00008"", ""ExtDocumentReference"": null, ""VersionNumber"": ""H-2.0.0.8"", ""CommandId"": ""553eb6ea-9ae2-4eeb-b838-eec84c93cd36"", ""PDCommandId"": ""@PDCommandId@"", ""DocumentId"": ""b75b9676-0f0f-456e-946b-ccbef5a531b0"", ""CommandGeneratedByUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""CommandGeneratedByCostCentreId"": ""53c69073-d408-487f-aa2e-195533d0400d"", ""CostCentreApplicationCommandSequenceId"": 0, ""CommandGeneratedByCostCentreApplicationId"": ""c60f5426-c165-4649-bfc6-a0f8ef552612"", ""SendDateTime"": ""0001-01-01T00:00:00"", ""CommandSequence"": 0, ""CommandCreatedDateTime"": ""2015-04-15T18:57:22.7293459+01:00"", ""Longitude"": null, ""Latitude"": null, ""Description"": null, ""IsSystemCommand"": false }, ""Order"": 1 }, { ""Command"": { ""LineItemSequenceNo"": 1, ""ProductId"": ""c8a07a84-fd98-44cf-86e2-e8983dc81ed6"", ""Qty"": 2.0, ""CommandTypeRef"": ""AddInventoryTransferNoteLineItem"", ""CommandId"": ""7a1de207-94cd-4c43-b400-b1baf7453bfb"", ""PDCommandId"": ""@PDCommandId@"", ""DocumentId"": ""b75b9676-0f0f-456e-946b-ccbef5a531b0"", ""CommandGeneratedByUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""CommandGeneratedByCostCentreId"": ""53c69073-d408-487f-aa2e-195533d0400d"", ""CostCentreApplicationCommandSequenceId"": 0, ""CommandGeneratedByCostCentreApplicationId"": ""c60f5426-c165-4649-bfc6-a0f8ef552612"", ""SendDateTime"": ""0001-01-01T00:00:00"", ""CommandSequence"": 0, ""CommandCreatedDateTime"": ""2015-04-15T18:57:22.7293459+01:00"", ""Longitude"": null, ""Latitude"": null, ""Description"": null, ""IsSystemCommand"": false }, ""Order"": 2 }, { ""Command"": { ""LineItemSequenceNo"": 1, ""ProductId"": ""faa82a9a-a10f-4e24-bc8c-53739d382127"", ""Qty"": 2.0, ""CommandTypeRef"": ""AddInventoryTransferNoteLineItem"", ""CommandId"": ""f162551d-1b41-463f-bd2d-1d4383d2e609"", ""PDCommandId"": ""@PDCommandId@"", ""DocumentId"": ""b75b9676-0f0f-456e-946b-ccbef5a531b0"", ""CommandGeneratedByUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""CommandGeneratedByCostCentreId"": ""53c69073-d408-487f-aa2e-195533d0400d"", ""CostCentreApplicationCommandSequenceId"": 0, ""CommandGeneratedByCostCentreApplicationId"": ""c60f5426-c165-4649-bfc6-a0f8ef552612"", ""SendDateTime"": ""0001-01-01T00:00:00"", ""CommandSequence"": 0, ""CommandCreatedDateTime"": ""2015-04-15T18:57:22.7293459+01:00"", ""Longitude"": null, ""Latitude"": null, ""Description"": null, ""IsSystemCommand"": false }, ""Order"": 3 }, { ""Command"": { ""CommandTypeRef"": ""ConfirmInventoryTransferNote"", ""CommandId"": ""81aaf403-d128-4edc-86b6-16e06118dd15"", ""PDCommandId"": ""@PDCommandId@"", ""DocumentId"": ""b75b9676-0f0f-456e-946b-ccbef5a531b0"", ""CommandGeneratedByUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""CommandGeneratedByCostCentreId"": ""53c69073-d408-487f-aa2e-195533d0400d"", ""CostCentreApplicationCommandSequenceId"": 0, ""CommandGeneratedByCostCentreApplicationId"": ""c60f5426-c165-4649-bfc6-a0f8ef552612"", ""SendDateTime"": ""0001-01-01T00:00:00"", ""CommandSequence"": 0, ""CommandCreatedDateTime"": ""2015-04-15T18:57:22.7293459+01:00"", ""Longitude"": null, ""Latitude"": null, ""Description"": null, ""IsSystemCommand"": false }, ""Order"": 4 } ], ""EnvelopeGeneratedTick"": 635647210427293459, ""EnvelopeArrivedAtServerTick"": 635647210521751075, ""IsSystemEnvelope"": false }, ""EnvelopeArrivedAtServerTick"": 635647210521751075 } ], ""ErrorInfo"": ""Success"" }";
        //See Data\Distributr_v3_0_0\MakeOrderWithoutPayment\Part2_Hub_Dispatches_Order\InventoryTransferNote.json
        public const string OrderDispatchApprovedLineItems = @"{ ""Envelopes"": [ { ""DocumentType"": ""Order"", ""Envelope"": { ""Id"": ""c42b4063-3cfc-4545-a909-d12fd59f2504"", ""DocumentId"": ""@PDCommandId@"", ""DocumentTypeId"": 1, ""GeneratedByCostCentreId"": ""4c243448-8e87-4454-8a87-23f6706eb3ab"", ""RecipientCostCentreId"": ""51a6bde4-d645-43fa-b81b-3708f87a5e6d"", ""GeneratedByCostCentreApplicationId"": ""c60f5426-c165-4649-bfc6-a0f8ef552612"", ""ParentDocumentId"": ""@PDCommandId@"", ""CommandsList"": [ { ""Command"": { ""CommandTypeRef"": ""OrderDispatchApprovedLineItems"", ""CommandId"": ""bca1f573-83c0-498e-99da-e3cdfbcf7ddb"", ""PDCommandId"": ""@PDCommandId@"", ""DocumentId"": ""b6537d37-62ac-4099-96d8-f0c65983f55d"", ""CommandGeneratedByUserId"": ""4ad1ee7d-a1e8-4876-9de9-a4cee99288e6"", ""CommandGeneratedByCostCentreId"": ""4c243448-8e87-4454-8a87-23f6706eb3ab"", ""CostCentreApplicationCommandSequenceId"": 0, ""CommandGeneratedByCostCentreApplicationId"": ""c60f5426-c165-4649-bfc6-a0f8ef552612"", ""SendDateTime"": ""0001-01-01T00:00:00"", ""CommandSequence"": 0, ""CommandCreatedDateTime"": ""2015-04-15T18:57:22.7293459+01:00"", ""Longitude"": null, ""Latitude"": null, ""Description"": null, ""IsSystemCommand"": false }, ""Order"": 1 } ], ""EnvelopeGeneratedTick"": 635647210427293459, ""EnvelopeArrivedAtServerTick"": 635647210522452083, ""IsSystemEnvelope"": false }, ""EnvelopeArrivedAtServerTick"": 635647210522452083 } ], ""ErrorInfo"": ""Success"" }";

        public const string GetNextEnvelopesRequestBody = @"{""DeliveredEnvelopeIds"":[@DeliveredEnvelopeIds@],""CostCentreApplicationId"":""2ca8657c-ef64-4f0a-92d0-ca7fb1e73942"",""BatchSize"":1}";

        public const string NoMoreEnvelopesResponse = @"{""Envelopes"":[],""ErrorInfo"":""No Pending Download""}";

        public static string CreateApproveOrderResponse(Guid saleGuid, Guid lineItemOneGuid, Guid lineItemTwoGuid)
        {
            return ApproveOrderEnvelopeWithTwoLineItems
                .Replace("@PDCommandId@", saleGuid.ToString())
                .Replace("@LineItemOneId@", lineItemOneGuid.ToString())
                .Replace("@LineItemTwoId@", lineItemTwoGuid.ToString());
        }

        public static string CreateInvoiceResponse(Guid saleGuid, Guid lineItemOneGuid, Guid lineItemTwoGuid)
        {
            return InvoiceEnvelopeWithTwoLinesItems
                .Replace("@PDCommandId@", saleGuid.ToString())
                .Replace("@LineItemOneId@", lineItemOneGuid.ToString())
                .Replace("@LineItemTwoId@", lineItemTwoGuid.ToString());
        }

        public static string CreateInventoryTransferNoteResponse(Guid saleGuid)
        {
            return InventoryTransferNoteWithTwoLineItems
                .Replace("@PDCommandId@", saleGuid.ToString());
        }

        public static string CreateOrderDispatchApprovedLineItemsResponse(Guid saleGuid)
        {
            return OrderDispatchApprovedLineItems
                .Replace("@PDCommandId@", saleGuid.ToString());
        }

        public static string CreateGetNextEnvelopesRequestBody(string lastDeliveredEnvelopeId = "")
        {
            if (lastDeliveredEnvelopeId != "")
            {
                return GetNextEnvelopesRequestBody.Replace("@DeliveredEnvelopeIds@", "\"" + lastDeliveredEnvelopeId + "\"");
            }
            return GetNextEnvelopesRequestBody.Replace("@DeliveredEnvelopeIds@", lastDeliveredEnvelopeId);            
        }

        [TearDown]
        public void Delete()
        {
            Database.DeleteAll(typeof(Order));
            Database.DeleteAll(typeof (IncomingEnvelopeLog));
        }

        [Test]
        public void CanDownloadEnvelopesWhenHubDispatchesUnpaidOrderWithTwoLineItems()
        {          
            //Given             
            var sale = AnUnpaidSaleForTwoItems().Build().Order;
            sale.ProcessingStatus = ProcessingStatus.Submitted;

            Database.InsertOrReplaceWithChildren(sale, recursive: true);

            AddFakePostResponse(CommandEnvelopeDownloadClient.NextEnvelopeEndpoint, 
                new HttpParams(),
                CreateApproveOrderResponse(sale.Id, sale.LineItems[0].Id, sale.LineItems[1].Id), 
                CreateGetNextEnvelopesRequestBody());

            AddFakePostResponse(CommandEnvelopeDownloadClient.NextEnvelopeEndpoint,
                new HttpParams(),
                CreateInvoiceResponse(sale.Id, sale.LineItems[0].Id, sale.LineItems[1].Id),
                //This is the ID of the Approved Order Response above. 
                CreateGetNextEnvelopesRequestBody("1e2b7adc-432c-4e0a-a220-175e96428e96"));

            AddFakePostResponse(CommandEnvelopeDownloadClient.NextEnvelopeEndpoint,
                new HttpParams(),
                CreateOrderDispatchApprovedLineItemsResponse(sale.Id),
                //This is the ID of the Invoice Response above. 
                CreateGetNextEnvelopesRequestBody("d14d42a5-87ca-4979-bf25-e6d19cae9e18"));

            AddFakePostResponse(CommandEnvelopeDownloadClient.NextEnvelopeEndpoint,
                new HttpParams(),
                CreateInventoryTransferNoteResponse(sale.Id),
                //This is the ID of the Dispatch Approve Line Items Response above. 
                CreateGetNextEnvelopesRequestBody("c42b4063-3cfc-4545-a909-d12fd59f2504"));

            AddFakePostResponse(CommandEnvelopeDownloadClient.NextEnvelopeEndpoint,
                new HttpParams(),
                NoMoreEnvelopesResponse,
                //This is the ID of the Inventory Transfer Note above. 
                CreateGetNextEnvelopesRequestBody("b7efa038-7f0a-4da1-a22c-2ad6ef58a056"));

            var envelopeProcessor = Resolve<IncomingCommandEnvelopeProcessor>();
            envelopeProcessor.StatusUpdate += Console.WriteLine;

            //When
            envelopeProcessor.DownloadPendingEnvelopes("2ca8657c-ef64-4f0a-92d0-ca7fb1e73942");

            //Then
            AssertFakeServerIsSatisfied();

            var saleFromDb = Database.FindWithChildren<Order>(sale.Id);

            Assert.AreEqual(ProcessingStatus.Deliverable, saleFromDb.ProcessingStatus, "Processing status");
            //The invoice reference from the Invoice above
            Assert.AreEqual("I_John_O003_20150415_065715_00021", saleFromDb.InvoiceReference, "Invoice reference");

            var lastProcessedEnvelopeId = Resolve<IncomingEnvelopeLogRepository>().GetIncomingEnvelopeLog().LastEnvelopeId;

            Assert.AreEqual("b7efa038-7f0a-4da1-a22c-2ad6ef58a056", lastProcessedEnvelopeId.ToString(), "Last envelope ID");
        }
    }
}
