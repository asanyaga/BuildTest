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
    public class BankBranchRepositoryFixture
    {

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();

        }

        [Test]
        public void BankBranchRepositoryUnitTest()
        {
            using (var tra = new TransactionScope())
            {
                Trace.WriteLine("START BANKBRANCH REPOSITORY UNIT TEST....");

                var bankBranch = new
                {
                    _Status = EntityStatus.Active,
                    Code = "BankBranchCode",
                    Description = "BankBranchDescription",
                    Name = "Nairobi Branch"
                };

                //Create
                Bank bank1 = _testHelper.BuildBank();
                Guid bankId = _testHelper.Ioc<IBankRepository>().Save(bank1);
                Bank bank = _testHelper.Ioc<IBankRepository>().GetById(bankId);
                Guid bankBranchId = _testHelper.AddBankBranch(Guid.Empty, bankBranch.Code, bankBranch.Description, bankBranch.Name, bankBranch._Status, bank);
                Trace.WriteLine(string.Format(">Created BankBranch [{0}]", bankBranchId));

                //GetById
                BankBranch createdBankBranch = _testHelper.Ioc<IBankBranchRepository>().GetById(bankBranchId);
                Assert.IsNotNull(createdBankBranch);
                Assert.AreEqual(createdBankBranch.Code, bankBranch.Code);
                Assert.AreEqual(createdBankBranch.Name, bankBranch.Name);
                Assert.AreEqual(createdBankBranch.Description, bankBranch.Description);
                Assert.AreEqual(createdBankBranch._Status, bankBranch._Status);
                Trace.WriteLine(string.Format(">BankBranch GetById[{0}], Name[{1}]", createdBankBranch.Id, createdBankBranch.Name));


                //Update
                var bankBranchToUpdate = createdBankBranch;
                bankBranchToUpdate._DateCreated = DateTime.Now;
                bankBranchToUpdate._DateLastUpdated = DateTime.Now;
                bankBranchToUpdate._Status = EntityStatus.Active;
                bankBranchToUpdate.Code = "BankBranch Code Update";
                bankBranchToUpdate.Description = "BankBranch Description";
                bankBranchToUpdate.Name = "Kisii Branch";

                Guid updatedBankBranchId = _testHelper.Ioc<IBankBranchRepository>().Save(bankBranchToUpdate);
                BankBranch updatedBankBranch = _testHelper.Ioc<IBankBranchRepository>().GetById(updatedBankBranchId);
                string code = updatedBankBranch.Code;
                string name = updatedBankBranch.Name;
                string description = updatedBankBranch.Description;
                EntityStatus status = updatedBankBranch._Status;

                Assert.AreEqual(code, bankBranchToUpdate.Code);
                Assert.AreEqual(name, bankBranchToUpdate.Name);
                Assert.AreEqual(description, bankBranchToUpdate.Description);
                Assert.AreEqual(status, bankBranchToUpdate._Status);
                Trace.WriteLine(string.Format(">BankBranch Updated Id[{0}],Bank Name[{1}], Bank Branch Name[{2}],Bank Branch Description[{3}], Status[{4}]", updatedBankBranch.Id, updatedBankBranch.Bank.Name, updatedBankBranch.Name, updatedBankBranch.Description, updatedBankBranch._Status));

                
                //Query
                var queryResult = _testHelper.Ioc<IBankBranchRepository>().Query(new QueryStandard() {Name = updatedBankBranch.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format(">BankBranch Query Count[{0}]", queryResult.Count));


                //Set As Inactive
                _testHelper.Ioc<IBankBranchRepository>().SetInactive(updatedBankBranch);
                BankBranch inactiveBankBranch = _testHelper.Ioc<IBankBranchRepository>().GetById(updatedBankBranchId);
                Assert.AreEqual(inactiveBankBranch._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format(">BankBranch Name set as Inactive[{0}]", inactiveBankBranch.Name));

                //Set As Active
                _testHelper.Ioc<IBankBranchRepository>().SetActive(updatedBankBranch);
                BankBranch activeBankBranch = _testHelper.Ioc<IBankBranchRepository>().GetById(updatedBankBranchId);
                Assert.AreEqual(activeBankBranch._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format(">BankBranch Name set as Active[{0}]", activeBankBranch.Name));

                //Set As Deleted
                _testHelper.Ioc<IBankBranchRepository>().SetAsDeleted(updatedBankBranch);
                BankBranch deletedBankBranch = _testHelper.Ioc<IBankBranchRepository>().GetById(updatedBankBranchId);
                Assert.AreEqual(deletedBankBranch._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format(">BankBranch Name set as Deleted[{0}]", deletedBankBranch.Name));

                Trace.WriteLine(">BankBranch Repository Unit Tests Successful");

                _testHelper.Ioc<ICacheProvider>().Reset();
            }

        }
    }
}
