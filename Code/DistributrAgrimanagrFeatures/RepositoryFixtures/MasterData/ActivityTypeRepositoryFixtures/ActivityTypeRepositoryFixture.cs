using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.ActivityTypeRepositoryFixtures
{
    [TestFixture]
    public class ActivityTypeRepositoryFixture
    {
        private static IActivityTypeRepository _activityTypeRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _activityTypeRepository = _testHelper.Ioc<IActivityTypeRepository>();
        }

        [Test]
        public void ActivityTypeRepositoryUnitTests()
        {
            using (var scope = new TransactionScope())
            {
                //Save Activity type
                var activityType = _testHelper.BuildActivityType();
                Trace.WriteLine(string.Format("Created activity type [{0}]", activityType.Name));
                var toSaveActivityType = _activityTypeRepository.Save(activityType);
                Trace.WriteLine(string.Format("Saved activity type Id [{0}]", toSaveActivityType));
                var savedActivityType = _activityTypeRepository.GetById(toSaveActivityType);

                AssertActivityType(activityType, savedActivityType);

                //Activity type listing
                var queryResult =
                    _activityTypeRepository.Query(new QueryActivityType() { Name = activityType.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Activity type [{0}] exists in listing", activityType.Name));

                //Update activity type
                var toUpdateActivityType = savedActivityType;
                toUpdateActivityType.Name = "Activity type 2";
                toUpdateActivityType.Description = "Activity type 2";

                _activityTypeRepository.Save(toUpdateActivityType);

                var updatedActivityType = _activityTypeRepository.GetById(toUpdateActivityType.Id);
                Trace.WriteLine(string.Format("Updated activity type to Name  [{0}]", updatedActivityType.Name));

                AssertActivityType(toUpdateActivityType, updatedActivityType);

                //Deactivate activity type
                var toDeactivate = updatedActivityType;
                toDeactivate._Status = EntityStatus.Inactive;

                _activityTypeRepository.Save(toDeactivate);

                var deactivated = _activityTypeRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated activity type  to status  [{0}]", deactivated._Status));

                //Activate activity type
                var toActivate = updatedActivityType;
                toActivate._Status = EntityStatus.Active;

                _activityTypeRepository.Save(toActivate);

                var activated = _activityTypeRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated activity type to status  [{0}]", activated._Status));

                //Delete activity type
                var toDelete = updatedActivityType;
                toDelete._Status = EntityStatus.Deleted;

                _activityTypeRepository.Save(toDelete);

                var deleted = _activityTypeRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted activity type to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertActivityType(ActivityType assetType, ActivityType savedAssetType)
        {
            Assert.AreEqual(assetType.Code, savedAssetType.Code);
            Assert.AreEqual(assetType.Name, savedAssetType.Name);
            Assert.AreEqual(assetType.Description, savedAssetType.Description);
            Assert.AreEqual(assetType.IsInfectionsRequired, savedAssetType.IsInfectionsRequired);
            Assert.AreEqual(assetType.IsInputRequired, savedAssetType.IsInputRequired);
            Assert.AreEqual(assetType.IsProduceRequired, savedAssetType.IsProduceRequired);
            Assert.AreEqual(assetType.IsServicesRequired, savedAssetType.IsServicesRequired);
            Assert.AreEqual(assetType._Status, EntityStatus.Active);
        }
    }
}