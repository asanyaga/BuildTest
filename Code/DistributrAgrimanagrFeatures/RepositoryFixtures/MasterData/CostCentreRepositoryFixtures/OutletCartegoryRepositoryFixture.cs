using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using NUnit.Framework;

namespace Distributr.Core.Data._2015.Tests.RepositoryFixtures.MasterData.CostCentreRepositoryFixtures
{
    [TestFixture]
    public class OutletCartegoryRepositoryFixture : RepositoryBaseFixture
    {
        private static IOutletCategoryRepository _outletCategoryRepository;

        [SetUp]
        public void Setup()
        {
            Setup_Helper();

            _outletCategoryRepository = _testHelper.Ioc<IOutletCategoryRepository>();
        }

        [Test]
        public void OutletCartegoryRepositoryUnitTests()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START OUTLET CARTEGORY REPOSITORY UNIT TEST....");

                //Save outlet cartegory
                var outletCartegory = _testHelper.BuildOutletCartegory();
                Trace.WriteLine(string.Format("Created outlet cartegory [{0}]", outletCartegory.Name));
                var toSaveOutletCartegory = _outletCategoryRepository.Save(outletCartegory);
                Trace.WriteLine(string.Format("Saved outlet cartegory Id [{0}]", toSaveOutletCartegory));
                var savedOutletCartegory = _outletCategoryRepository.GetById(toSaveOutletCartegory);

                AssertOutletCartegory(outletCartegory, savedOutletCartegory);

                //Outlet cartegory listing
                var queryResult =
                    _outletCategoryRepository.Query(new QueryStandard() { Name = outletCartegory.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Outlet cartegory [{0}] exists in listing", outletCartegory.Name));

                //Update outlet cartegory
                var toUpdateOutletCartegory = savedOutletCartegory;
                toUpdateOutletCartegory.Name = "Outlet Cartegory 2";

                _outletCategoryRepository.Save(toUpdateOutletCartegory);

                var updatedOutletCartegory = _outletCategoryRepository.GetById(toUpdateOutletCartegory.Id);
                Trace.WriteLine(string.Format("Updated outlet cartegory to Name  [{0}]", updatedOutletCartegory.Name));

                AssertOutletCartegory(toUpdateOutletCartegory, updatedOutletCartegory);

                //Deactivate outlet cartegory
                var toDeactivate = updatedOutletCartegory;
                toDeactivate._Status = EntityStatus.Inactive;

                _outletCategoryRepository.Save(toDeactivate);

                var deactivated = _outletCategoryRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated outlet cartegory  to status  [{0}]", deactivated._Status));

                //Activate outlet cartegory
                var toActivate = updatedOutletCartegory;
                toActivate._Status = EntityStatus.Active;

                _outletCategoryRepository.Save(toActivate);

                var activated = _outletCategoryRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated outlet cartegory to status  [{0}]", activated._Status));

                //Delete outlet cartegory
                var toDelete = updatedOutletCartegory;
                toDelete._Status = EntityStatus.Deleted;

                _outletCategoryRepository.Save(toDelete);

                var deleted = _outletCategoryRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted outlet cartegory to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertOutletCartegory(OutletCategory outletCartegory, OutletCategory savedOutletCartegory)
        {
            Assert.AreEqual(outletCartegory.Name,savedOutletCartegory.Name);
            Assert.AreEqual(savedOutletCartegory._Status,EntityStatus.Active);
        }
    }
}
