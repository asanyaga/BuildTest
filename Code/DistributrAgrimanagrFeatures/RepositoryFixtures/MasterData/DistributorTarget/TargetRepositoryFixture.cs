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
    public class TargetRepositoryFixture
    {
        private static ITargetRepository _targetPeriodRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();

            _targetPeriodRepository = _testHelper.Ioc<ITargetRepository>();
        }

        [Test]
        public void TargetRepositoryUnitTests()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START TARGET REPOSITORY UNIT TEST....");

                //Save target
                var target = _testHelper.BuildTarget();
                Trace.WriteLine(string.Format("Created target [{0}]", target.Id));
                var toSaveTarget = _targetPeriodRepository.Save(target);
                Trace.WriteLine(string.Format("Saved target Id [{0}]", toSaveTarget));
                var savedTarget = _targetPeriodRepository.GetById(toSaveTarget);

                AssertTarget(target, savedTarget);

                //Target listing
                var queryResult =
                    _targetPeriodRepository.Query(new QueryStandard() {});
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Target [{0}] exists in listing", target.Id));

                //Update target 
                var toUpdateTarget = savedTarget;
                toUpdateTarget.IsQuantityTarget = false;
                toUpdateTarget.TargetValue = 10;

                _targetPeriodRepository.Save(toUpdateTarget);

                var updatedTarget = _targetPeriodRepository.GetById(toUpdateTarget.Id);
                Trace.WriteLine(string.Format("Updated target to Name  [{0}]", updatedTarget.Id));

                AssertTarget(toUpdateTarget, updatedTarget);

                //Deactivate target 
                var toDeactivate = updatedTarget;
                toDeactivate._Status = EntityStatus.Inactive;

                _targetPeriodRepository.Save(toDeactivate);

                var deactivated = _targetPeriodRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated target to status  [{0}]", deactivated._Status));

                //Activate target 
                var toActivate = updatedTarget;
                toActivate._Status = EntityStatus.Active;

                _targetPeriodRepository.Save(toActivate);

                var activated = _targetPeriodRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated target to status  [{0}]", activated._Status));

                //Delete target 
                var toDelete = updatedTarget;
                toDelete._Status = EntityStatus.Deleted;

                _targetPeriodRepository.Save(toDelete);

                var deleted = _targetPeriodRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted target to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertTarget(Target targetPeriod, Target savedTargetPeriod)
        {
            Assert.AreEqual(targetPeriod.CostCentre.CostCentreCode, savedTargetPeriod.CostCentre.CostCentreCode);
            Assert.AreEqual(targetPeriod.TargetPeriod.Name, savedTargetPeriod.TargetPeriod.Name);
            Assert.AreEqual(targetPeriod.IsQuantityTarget, savedTargetPeriod.IsQuantityTarget);
            Assert.AreEqual(targetPeriod.TargetValue, savedTargetPeriod.TargetValue);
            Assert.AreEqual(targetPeriod._Status, EntityStatus.Active);
        }
    }
}