using System;
using System.Collections.Generic;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.MakeOrder;
using Distributr.Mobile.Core.MakeSale;
using Distributr.Mobile.Core.Products;
using Distributr.Mobile.Login;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Order = Distributr.Mobile.Core.OrderSale.Order;

namespace Distributr.Mobile.Core.Test.OrderSale
{
    public class BaseEnvelopeBuilderTest
    {
        public static Mock<SaleProduct> AProductWithPrice(decimal price)
        {
            var product = new Mock<SaleProduct>();
            product.Object.Id = Guid.NewGuid();

            product.Setup(p => p.ProductPrice(It.IsAny<ProductPricingTier>()))
                .Returns(price);

            return product;
        }

        public static Mock<ReturnableProduct> AReturnableProductWithPrice(decimal price)
        {
            var returnable = new Mock<ReturnableProduct>();
            returnable.Object.Id = Guid.NewGuid();
            returnable.Object.Description = "returnable prodct";

            returnable.Setup(p => p.ProductPrice(It.IsAny<ProductPricingTier>()))
                .Returns(price);

            return returnable;
        }

        public static VATClass AVatClassWithRate(decimal rate)
        {
            var vatClassItem = new VATClass.VATClassItem()
            {
                EffectiveDate = DateTime.Now.AddDays(-1),
                Rate = rate
            };
            var vatClass = new VATClass { VATClassItems = new List<VATClass.VATClassItem>() { vatClassItem } };

            return vatClass;
        }

        public static User AUser()
        {
            return new User()
            {
                Id = Guid.NewGuid(),
                CostCentreApplicationId = Guid.NewGuid().ToString(),
                Username = "Ander"
            };
        }

        public static Outlet AnOutlet()
        {
            return new Outlet(Guid.NewGuid())
            {

            };
        }

        public static CostCentre ADistributorSalesman()
        {
            return new DistributorSalesman(Guid.NewGuid())
            {
                Id = Guid.NewGuid(),
                ParentCostCentreId = Guid.NewGuid()
            };
        }

        public static OrderAndContext AnOrderWithItemAndReturnable()
        {
            var returnable = AReturnableProductWithPrice(0.15m).Object;
            returnable.VATClass = AVatClassWithRate(0);

            var product = AProductWithPrice(1.5m).Object;
            product.VATClass = AVatClassWithRate(0.10m);
            product.ReturnableProduct = returnable;
            product.ReturnableProductMasterId = returnable.Id;

            var saleAndContext = AnOrderAndContextWithNoItems();
            var sale = saleAndContext.Order;

            
            sale.AddOrUpdateOrderLineItem(new ProductWrapper() {SaleProduct = product, EachQuantity = 1});

            return saleAndContext;
        }

        public static OrderAndContext AFullyPaidCashSaleAndContext()
        {
            var product = AProductWithPrice(1.5m).Object;
            product.VATClass = AVatClassWithRate(0.10m);

            var saleAndContext = ASaleAndContextWithNoItems();
            var sale = saleAndContext.Order;

            sale.AddOrUpdateSaleLineItem(new ProductWrapper() { SaleProduct = product, EachQuantity = 1, MaxEachQuantity = 1});
            sale.AddCashPayment("My Ref", sale.TotalValueIncludingVat);

            return saleAndContext;
        }

        public static OrderAndContext AnOrderAndContextWithNoItems()
        {
            var outlet = AnOutlet();
            var costCentre = ADistributorSalesman();
            var user = AUser();

            var order = new Order(Guid.NewGuid(), outlet);
            var context = new MakeOrderEnvelopeContext(1, outlet, user, costCentre, order);

            return new OrderAndContext (order, context);
        }

        public static OrderAndContext ASaleAndContextWithNoItems()
        { 
            var outlet = AnOutlet();
            var costCentre = ADistributorSalesman();
            var user = AUser();

            var sale = new Order(Guid.NewGuid(), outlet);
            sale.OrderType = OrderType.DistributorPOS;
            var context = new MakeSaleEnvelopeContext(1, outlet, user, costCentre, sale);

            return new OrderAndContext(sale, context);            
        }

        public static List<DocumentCommand> ExtractDocumentCommands(List<CommandEnvelope> envelopes)
        {
            var documentCommands = new List<DocumentCommand>();

            foreach (var commandEnvelope in envelopes)
            {
                commandEnvelope.CommandsList.ForEach(e => { documentCommands.Add(e.Command); });
            }
            return documentCommands;
        }

        public static void AssertCommonEnvelopeChecks(List<CommandEnvelope> envelopes, IEnvelopeContext context)
        {
            Console.WriteLine(JsonConvert.SerializeObject(envelopes, Formatting.Indented));
            Assert.AreEqual(1, envelopes.Count, "Envelope count");

            CheckEnvelopeReferences(envelopes, context);

            var documentCommands = ExtractDocumentCommands(envelopes);
            
            CheckDocumentCommandReferences(documentCommands, envelopes[0], context);
        }

        public static void CheckEnvelopeReferences(List<CommandEnvelope> envelopes, IEnvelopeContext context)
        {
            envelopes.ForEach( e =>
            {
                var type = (DocumentType) e.DocumentTypeId;

                Assert.AreEqual(context.GeneratedByCostCentreApplicationId, e.GeneratedByCostCentreApplicationId,
                    "GeneratedByCostCentreApplicationId in message {0}", type);

                Assert.AreEqual(context.GeneratedByCostCentreId, e.GeneratedByCostCentreId, 
                    "GeneratedByCostCentreId in message {0}", type);

                Assert.AreEqual(context.ParentDocumentId, e.ParentDocumentId, 
                    "ParentDocumentId in message {0}", type);
            });
        }

        public static void CheckDocumentCommandReferences(List<DocumentCommand> documentCommands, CommandEnvelope commandEnvelope, IEnvelopeContext context)
        {
            documentCommands.ForEach(c =>
            {
                Assert.AreEqual(commandEnvelope.ParentDocumentId, c.PDCommandId,
                    "PDCommandId in command {0}", c.CommandTypeRef);

                Assert.AreEqual(commandEnvelope.DocumentId, c.DocumentId,
                    "DocumentId in command {0}", c.CommandTypeRef);

                Assert.AreEqual(context.GeneratedByUserId, c.CommandGeneratedByUserId, 
                    "GeneratedByUserId in command {0}", c.CommandTypeRef);

                Assert.AreEqual(context.GeneratedByCostCentreId, c.CommandGeneratedByCostCentreId,
                    "CommandGeneratedByCostCentreId in command {0}", c.CommandTypeRef);

                Assert.AreEqual(context.GeneratedByCostCentreApplicationId, c.CommandGeneratedByCostCentreApplicationId,
                    "CommandGeneratedByCostCentreApplicationId in command {0}", c.CommandTypeRef);

                var createCommand = c as CreateCommand;
                if (createCommand != null)
                {
                    Assert.AreEqual(context.GeneratedByCostCentreId, createCommand.DocumentIssuerCostCentreId,
                        "DocumentIssuerCostCentreId in command {0}", c.CommandTypeRef);

                    Assert.AreEqual(context.GeneratedByUserId, createCommand.DocIssuerUserId,
                        "DocIssuerUserId in command {0}", c.CommandTypeRef);                    
                }
            });
        }

        public static void CheckReference(string prefix, string reference, string userName)
        {
            var referenceParts = reference.Split('_');
            Assert.AreEqual(referenceParts.Length, 6, "parts length");
            Assert.AreEqual(referenceParts[0], prefix, "prefix");
            Assert.AreEqual(referenceParts[1], userName, "username");
        }

        public static void CheckOrderPaymentInfo(Order order, AddOrderPaymentInfoCommand paymentInfo)
        {
            Assert.AreEqual(order.TotalValueIncludingVat, paymentInfo.Amount, "amount");
            Assert.AreEqual(order.TotalValueIncludingVat, paymentInfo.ConfirmedAmount, "confirmed amount");
            Assert.AreEqual(order.Payments[0].PaymentMode, (PaymentMode)paymentInfo.PaymentModeId, "payment mode");
            Assert.AreEqual(order.Payments[0].Bank, paymentInfo.Bank, "bank");
            Assert.AreEqual(order.Payments[0].BankBranch, paymentInfo.BankBranch, "bank branch");
            Assert.AreEqual(order.Payments[0].DueDate, paymentInfo.DueDate, "due date");
            Assert.AreEqual(order.Payments[0].PaymentReference, paymentInfo.PaymentRefId, "payment reference");
        }
    }
}
