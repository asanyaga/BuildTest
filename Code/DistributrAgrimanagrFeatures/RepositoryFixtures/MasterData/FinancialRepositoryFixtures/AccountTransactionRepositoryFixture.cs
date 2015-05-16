using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Financials;
using Distributr.Core.Utility.Caching;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.FinancialRepositoryFixtures
{
    [TestFixture]
   public class AccountTransactionRepositoryFixture
    {
        private static IAccountTransactionRepository _acountTransactionRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _acountTransactionRepository = _testHelper.Ioc<IAccountTransactionRepository>();
        }

        [Test]
        public void AccountTransactionRepositoryUnitTest()
        {
            using (var tra =new TransactionScope())
            {
                Trace.WriteLine("START ACCOUNT TRANSACTION REPOSITORY UNIT TEST....");

                //Save container type
                var newAccountTransaction = _testHelper.BuildAccountTransaction();
                var newAccountTransactionId = _acountTransactionRepository.Add(newAccountTransaction);
                var createdAccountTransaction = _acountTransactionRepository.GetById(newAccountTransactionId);
                AssertAccountTransaction(newAccountTransaction, createdAccountTransaction);
                Trace.WriteLine(string.Format("AccountTransaction Type[{0}],AccountTransactionBalance[{1}]", createdAccountTransaction.Amount, createdAccountTransaction.Account.Balance));


                var accountTransactionToUpdate = createdAccountTransaction;

                accountTransactionToUpdate.Account = createdAccountTransaction.Account;
                accountTransactionToUpdate.Amount = 530;
                accountTransactionToUpdate.DocumentType = DocumentType.Invoice;
                accountTransactionToUpdate.DocumentId = createdAccountTransaction.DocumentId;
                var updatedAccountTransactionId = _acountTransactionRepository.Add(accountTransactionToUpdate);
                var updatedAccountTransaction = _acountTransactionRepository.GetById(updatedAccountTransactionId);
                Trace.WriteLine(string.Format("UpdateAccountTransactionId [{0}],AccountTransaction Type[{1}],AccountBalance[{2}]",updatedAccountTransaction.Id, updatedAccountTransaction.Account, updatedAccountTransaction.Account.Balance));
            }
            _testHelper.Ioc<ICacheProvider>().Reset();
        }

        private void AssertAccountTransaction(AccountTransaction accountTraX, AccountTransaction accountTraY)
        {
            Assert.AreEqual(accountTraX.Account.Balance, accountTraY.Account.Balance);
            Assert.AreEqual(accountTraX.Amount, accountTraY.Amount);
            Assert.AreEqual(accountTraX.DocumentType, accountTraY.DocumentType);
            Assert.AreEqual(accountTraX.Id, accountTraY.Id);
        }
    }
}
