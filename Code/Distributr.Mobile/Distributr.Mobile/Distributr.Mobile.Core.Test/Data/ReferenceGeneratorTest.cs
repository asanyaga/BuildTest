using System;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Mobile.Core.Data.References;
using Distributr.Mobile.Core.Data.Sequences;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.Data
{
    [TestFixture]
    public class ReferenceGeneratorTest
    {
        private readonly Outlet outlet = new Outlet() {CostCentreCode = "O100"};
        private readonly User user = new User() { Username = "Juan", Code = "S001"};

        [Test]
        public void CanGenerateIdsForMakeSale()
        {
            var sequence = new DatabaseSequence() { SequenceName = SequenceName.DocumentReference, NextValue = 1};
            var generator = new ReferenceGenerator(sequence.NextValue, user, outlet, DateTime.Now);

            var sequenceNumber = sequence.NextValue;

            var saleId = generator.NextSaleReference();
            CompareParts(saleId, "S", "Juan", outlet.CostCentreCode, sequenceNumber);

            var orderId = generator.NextOrderReference();
            CompareParts(orderId, "O", "Juan", outlet.CostCentreCode, sequenceNumber);

            var invoiceId = generator.NextInvoiceReference();
            CompareParts(invoiceId, "I", "Juan", outlet.CostCentreCode, sequenceNumber);

            var receiptId = generator.NextReceiptReference();
            CompareParts(receiptId, "R", "Juan", outlet.CostCentreCode, sequenceNumber);

            var externalDocRef = generator.NextExternalDocumentReference();
            Assert.AreEqual("S001000001", externalDocRef, "external doc ref");
        }

        //e.g  "S_john_O001_20150406_114353_00024"
        private void CompareParts(string orderId, string prefix, string username, string costCentreCode, long sequenceNumber)
        {
            var tokens = orderId.Split('_');
            Assert.AreEqual(tokens.Length, 6, "number of tokens");

            Assert.AreEqual(prefix, tokens[0], "prefix");
            Assert.AreEqual(username, tokens[1], "username");
            Assert.AreEqual(costCentreCode, tokens[2], "outlet cost centre code");
            Assert.AreEqual(8, tokens[3].Length, "date");
            Assert.AreEqual(6, tokens[4].Length, "time");
            Assert.AreEqual(sequenceNumber.ToString().PadLeft(5, '0'), tokens[5], "sequence");
        }
    }
}