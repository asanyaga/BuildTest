using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Login;
using Newtonsoft.Json;
using NUnit.Framework;
using Order = Distributr.Mobile.Core.OrderSale.Order;

namespace Distributr.Mobile.Core.Test.OrderSale
{
    public class BaseEnvelopeBuilderTest
    {

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
            var product = MockOrderBuilder.AProductWithPrice(1.5m);
            product.VATClass = MockOrderBuilder.AVatClassWithRate(0.10m);

            return AnOrderAndContextWithNoItems()
                .AddLineItem(product, 1)
                .Build();
        }

        public static OrderAndContext AFullyPaidCashSaleAndContext()
        {
            var product = MockOrderBuilder.AProductWithPrice(1.5m);
            product.VATClass = MockOrderBuilder.AVatClassWithRate(0.10m);

            var saleAndContext =  ASaleAndContextWithNoItems()
            .AddLineItem(product, 1, sellReturnables: true);

            saleAndContext.WithCashPayment(saleAndContext.Sale.BalanceOutstanding);
           
            return saleAndContext.Build();
        }

        public static OrderAndContextBuilder AnOrderAndContextWithNoItems()
        {
            var outlet = AnOutlet();
            var order = new Sale(Guid.NewGuid(), outlet);
            var bank = MockOrderBuilder.Bank;
            return new OrderAndContextBuilder(AnOutlet(), ADistributorSalesman(), AUser(), order, bank, bank.Branches.First());
        }

        public static SaleAndContextBuilder ASaleAndContextWithNoItems()
        {
            var outlet = AnOutlet();
            var order = new Sale(Guid.NewGuid(), outlet) {OrderType = OrderType.DistributorPOS};
            var bank = MockOrderBuilder.Bank;
            return new SaleAndContextBuilder(AnOutlet(), ADistributorSalesman(), AUser(), order, bank, bank.Branches.First());
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
