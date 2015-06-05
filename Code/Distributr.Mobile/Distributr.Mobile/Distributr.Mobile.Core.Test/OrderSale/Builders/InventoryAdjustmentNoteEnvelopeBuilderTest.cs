using System.Linq;
using Distributr.Core.Commands.DocumentCommands.AdjustmentNotes;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.MakeSale;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.OrderSale.EnvelopeBuilders;
using Distributr.Mobile.Core.Products;
using Moq;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.OrderSale
{
    [TestFixture]
    public class InventoryAdjustmentNoteEnvelopeBuilderTest : BaseEnvelopeBuilderTest
    {
        [Test]
        public void CanCreateAnInventoryAdjustmentNoteEnvelope()
        {
            //Given 
            var saleAndContext = AFullyPaidCashSaleAndContext();
            var order = saleAndContext.Sale;
            var context = saleAndContext.Context;
            var invtentoryRepository = new Mock<IInventoryRepository>();
            
            const int availableStock = 2;
            
            invtentoryRepository.Setup(i => i.GetBalanceForProduct(order.LineItems[0].ProductMasterId)).Returns(availableStock);

            var builder = new SaleEnvelopeBuilder(order, new InventoryAdjustmentNoteEnvelopeBuilder(context, invtentoryRepository.Object));

            //When 
            var result = builder.Build();

            //Then
            AssertCommonEnvelopeChecks(result, context);

            var documentCommands = ExtractDocumentCommands(result);

            //Check CreateInventoryAdjustmentNote
            var createInventoryAdjustmentNote = documentCommands.OfType<CreateInventoryAdjustmentNoteCommand>().First();
            Assert.AreEqual(context.RecipientCostCentreId, createInventoryAdjustmentNote.DocumentRecipientCostCentreId, "DocumentRecipientCostCentreId");
            Assert.AreEqual((int)InventoryAdjustmentNoteType.Available, createInventoryAdjustmentNote.InventoryAdjustmentNoteTypeId, "InventoryAdjustmentNoteTypeId");

            //Check InventoryAdjustmentNoteLineItem
            var inventoryAdjustmentNoteLineItem = documentCommands.OfType<AddInventoryAdjustmentNoteLineItemCommand>().First();
            CheckInventoryAdjustmentNoteLineCommand(order.LineItems[0], inventoryAdjustmentNoteLineItem, availableStock);
        }

        private static void CheckInventoryAdjustmentNoteLineCommand(ProductLineItem item, AddInventoryAdjustmentNoteLineItemCommand command, decimal availableStock)
        {
            Assert.AreEqual(item.Product.Id, command.ProductId, "Line item Product ID");
            Assert.AreEqual(availableStock, command.Expected, "Line item available value");
            Assert.AreEqual(availableStock - item.Quantity, command.Actual, "Line item expected value");
        }
    }
}
