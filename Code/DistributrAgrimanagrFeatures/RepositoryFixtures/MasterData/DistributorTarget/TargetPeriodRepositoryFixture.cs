using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Repository.Master.DistributorTargetRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.DistributorTarget
{
    [TestFixture]
    public class TargetPeriodRepositoryFixture
    {
        private static ITargetPeriodRepository _targetPeriodRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();

            _targetPeriodRepository = _testHelper.Ioc<ITargetPeriodRepository>();
        }

        [Test]
        public void TargetPeriodRepositoryUnitTests()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START TARGET PERIOD REPOSITORY UNIT TEST....");

                //Save target period
                var targetPeriod = _testHelper.BuildTargetPeriod();
                Trace.WriteLine(string.Format("Created target period [{0}]", targetPeriod.Name));
                var toSaveTargetPeriod = _targetPeriodRepository.Save(targetPeriod);
                Trace.WriteLine(string.Format("Saved target period Id [{0}]", toSaveTargetPeriod));
                var savedTargetPeriod = _targetPeriodRepository.GetById(toSaveTargetPeriod);

                AssertTargetPeriod(targetPeriod, savedTargetPeriod);

                //Target period listing
                var queryResult =
                    _targetPeriodRepository.Query(new QueryStandard() { Name = targetPeriod.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Target period [{0}] exists in listing", targetPeriod.Name));

                //Update target period
                var toUpdateTargetPeriod = savedTargetPeriod;
                toUpdateTargetPeriod.Name = "Target period 2";

                _targetPeriodRepository.Save(toUpdateTargetPeriod);

                var updatedTargetPeriod = _targetPeriodRepository.GetById(toUpdateTargetPeriod.Id);
                Trace.WriteLine(string.Format("Updated target period to Name  [{0}]", updatedTargetPeriod.Name));

                AssertTargetPeriod(toUpdateTargetPeriod, updatedTargetPeriod);

                //Deactivate target period
                var toDeactivate = updatedTargetPeriod;
                toDeactivate._Status = EntityStatus.Inactive;

                _targetPeriodRepository.Save(toDeactivate);

                var deactivated = _targetPeriodRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated target period  to status  [{0}]", deactivated._Status));

                //Activate target period
                var toActivate = updatedTargetPeriod;
                toActivate._Status = EntityStatus.Active;

                _targetPeriodRepository.Save(toActivate);

                var activated = _targetPeriodRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated target period to status  [{0}]", activated._Status));

                //Delete target period
                var toDelete = updatedTargetPeriod;
                toDelete._Status = EntityStatus.Deleted;

                _targetPeriodRepository.Save(toDelete);

                var deleted = _targetPeriodRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted target period to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertTargetPeriod(TargetPeriod targetPeriod, TargetPeriod savedTargetPeriod)
        {
            Assert.AreEqual(targetPeriod.Name, savedTargetPeriod.Name);
            Assert.AreEqual(targetPeriod.StartDate, savedTargetPeriod.StartDate);
            Assert.AreEqual(targetPeriod.EndDate, savedTargetPeriod.EndDate);
            Assert.AreEqual(targetPeriod._Status, EntityStatus.Active);
        }
    }
}
