using System;
using System.Linq;
using Distributr.Core.Commands.DocumentCommands.DispatchNotes;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.MakeSale;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.OrderSale.EnvelopeBuilders;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.OrderSale
{
    [TestFixture]
    public class CreateDispatchNoteBuilderTest : BaseEnvelopeBuilderTest
    {
        [Test]
        public void CanGenerateADispatchNoteEnvelope()
        {
            //Given
            var saleAndContext = AFullyPaidCashSaleAndContext();
            var order = saleAndContext.Sale;
            var context = saleAndContext.Context;

            var builder = new SaleEnvelopeBuilder(order, new DispatchNoteEnvelopeBuilder(context));

            //When 
            var result = builder.Build();

            Console.WriteLine(JsonConvert.SerializeObject(result[0], Formatting.Indented));

            AssertCommonEnvelopeChecks(result, context);

            var documentCommands = ExtractDocumentCommands(result);
            
            //Check CreateDispatchNote 
            var createDispatchNote = documentCommands.OfType<CreateDispatchNoteCommand>().First();
            Assert.AreEqual(context.ParentDocumentId, createDispatchNote.OrderId, "Order Id");
            Assert.AreEqual(2, createDispatchNote.DispatchNoteType, "DispatchNote Type");

            var addDispatchNoteLineItems = documentCommands.OfType<AddDispatchNoteLineItemCommand>().ToList();

            CheckDispatchNoteLineItemCommand(order.LineItems[0], addDispatchNoteLineItems[0]);
        }

        private static void CheckDispatchNoteLineItemCommand(ProductLineItem item, AddDispatchNoteLineItemCommand command)
        {
            Assert.AreEqual(item.Product.Id, command.ProductId, "Line item Product ID");
            Assert.AreEqual(item.Quantity, command.Qty, "Line item quantity");
            Assert.AreEqual(item.Value, command.Value, "Line item value");
            Assert.AreEqual(item.VatValue, command.LineItemVatValue, "Line item VAT value");
        }
    }
}
