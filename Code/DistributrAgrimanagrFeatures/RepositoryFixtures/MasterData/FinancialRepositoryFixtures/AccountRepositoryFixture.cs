using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Repository.Financials;
using Distributr.Core.Utility.Caching;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.FinancialRepositoryFixtures
{
    [TestFixture]
   public class AccountRepositoryFixture
    {
        private static IAccountRepository _accountRepository;
        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _accountRepository = _testHelper.Ioc<IAccountRepository>();
        }

        [Test]
        public void AccountRepositoryUnitTest()
        {
            using (var tra = new TransactionScope())
            {
                Trace.WriteLine("START ACCOUNT REPOSITORY UNIT TEST....");

                //Save container type
                var newAccount = _testHelper.BuildAccount();
                var newAccountId = _accountRepository.Add(newAccount);
                var createdAccount = _accountRepository.GetById(newAccountId);
                AssertAccount(newAccount, createdAccount);
                Trace.WriteLine(string.Format("Account Type[{0}],AccountBalance[{1}]", createdAccount.AccountType, createdAccount.Balance));

                var accountToUpdate = createdAccount;
               _accountRepository.AdjustAccountBalance(createdAccount.CostcentreId, AccountType.Cash,800);
               var updatedAccount = _accountRepository.GetById(createdAccount.Id);
               Trace.WriteLine(string.Format("UpdateAccountId [{0}],Account Type[{1}]", updatedAccount.Balance, updatedAccount.AccountType));
            }
            _testHelper.Ioc<ICacheProvider>().Reset();
        }

        private void AssertAccount(Account accountX, Account accountY)
        {
           Assert.AreEqual(accountX.AccountType,accountY.AccountType);
           Assert.AreEqual(accountX.Balance, accountY.Balance);
           Assert.AreEqual(accountX.CostcentreId, accountY.CostcentreId);
           Assert.AreEqual(accountX.Id, accountY.Id);
        }
    }
}
