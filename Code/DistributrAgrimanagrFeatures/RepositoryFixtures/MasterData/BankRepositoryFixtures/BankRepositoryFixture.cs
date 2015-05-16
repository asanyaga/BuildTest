using System;
using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.BankRepositoryFixtures
{
    [TestFixture]
    public class BankRepositoryFixture
    {

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
        }

        [Test]
        public void BankRepositoryUnitTest()
        {
            using (var tra = new TransactionScope())
            {
                Trace.WriteLine("START BANK REPOSITORY UNIT TEST....");

                //Create
                var bank = _testHelper.BuildBank();

                var bankId = _testHelper.Ioc<IBankRepository>().Save(bank);
                Trace.WriteLine(string.Format("Created Bank [{0}]", bank.Name));
                //GetById
                Bank createdBank = _testHelper.Ioc<IBankRepository>().GetById(bank.Id);

                AssertBank(bank,createdBank);

                //Update
                var bankToUpdate = createdBank;
                bankToUpdate._DateCreated = DateTime.Now;
                bankToUpdate._DateLastUpdated = DateTime.Now;
                bankToUpdate._Status = EntityStatus.Active;
                bankToUpdate.Code = "Bank Code Update";
                bankToUpdate.Description = "bank description update";
                bankToUpdate.Name = "Bank of Africa update";

                var updatedBankId = _testHelper.Ioc<IBankRepository>().Save(bankToUpdate);

                Bank updatedBank = _testHelper.Ioc<IBankRepository>().GetById(updatedBankId);

                AssertBank(bankToUpdate, updatedBank);
               

                //Query
                var queryResult = _testHelper.Ioc<IBankRepository>().Query(new QueryStandard(){Name = updatedBank.Name});
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Bank Query Count[{0}]", queryResult.Count));

                //Set As Inactive
                _testHelper.Ioc<IBankRepository>().SetInactive(updatedBank);
                Bank InactiveBank = _testHelper.Ioc<IBankRepository>().GetById(updatedBankId);
                Assert.AreEqual(InactiveBank._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Bank Status[{0}]", InactiveBank._Status));

                //Set As Active
                _testHelper.Ioc<IBankRepository>().SetActive(updatedBank);
                Bank ActiveBank = _testHelper.Ioc<IBankRepository>().GetById(updatedBankId);
                Assert.AreEqual(ActiveBank._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Bank Status[{0}]", ActiveBank._Status));
                
                //Set As Deleted
                _testHelper.Ioc<IBankRepository>().SetAsDeleted(updatedBank);
                Bank deletedBank = _testHelper.Ioc<IBankRepository>().GetById(updatedBankId);
                Assert.AreEqual(deletedBank._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Bank Status[{0}]", deletedBank._Status));
                Trace.WriteLine(string.Format("Bank Repository Unit Tests Successful"));
                _testHelper.Ioc<ICacheProvider>().Reset();
                
               }
        }

        private static void AssertBank(Bank bank, Bank createdBank)
        {
            Assert.IsNotNull(createdBank);
            Assert.AreEqual(createdBank.Code, bank.Code);
            Assert.AreEqual(createdBank.Name, bank.Name);
            Assert.AreEqual(createdBank.Description, bank.Description);
            Assert.AreEqual(createdBank._Status, bank._Status);
            Trace.WriteLine(string.Format("Bank GetById[{0}], Name[{1}]", createdBank.Id, createdBank.Name));
        }
    }
}
