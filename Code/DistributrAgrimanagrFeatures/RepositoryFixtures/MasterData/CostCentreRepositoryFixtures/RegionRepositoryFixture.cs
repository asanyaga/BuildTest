using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.CostCentreRepositoryFixtures
{
    [TestFixture]
    public class RegionRepositoryFixture
    {
        private static IRegionRepository _regionRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();

            _regionRepository = _testHelper.Ioc<IRegionRepository>();
        }

        [Test]
        public void RegionRepositoryUnitTest()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START REGION REPOSITORY UNIT TEST....");

                //Save region
                var region = _testHelper.BuildRegion();
                Trace.WriteLine(string.Format("Created region [{0}]", region.Name));
                var toSaveRegion = _regionRepository.Save(region);
                Trace.WriteLine(string.Format("Saved region Id [{0}]", toSaveRegion));
                var savedRegion = _regionRepository.GetById(toSaveRegion);

                AssertRegion(region, savedRegion);

                //Region listing
                var queryResult =
                    _regionRepository.Query(new QueryStandard() { Name = region.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Region [{0}] exists in listing", region.Name));

                //Update region
                var toUpdateRegion = savedRegion;
                toUpdateRegion.Name = "Region 2";

                _regionRepository.Save(toUpdateRegion);

                var updatedRegion = _regionRepository.GetById(toUpdateRegion.Id);
                Trace.WriteLine(string.Format("Updated region to Name  [{0}]", updatedRegion.Name));

                AssertRegion(toUpdateRegion, updatedRegion);

                //Deactivate region
                var toDeactivate = updatedRegion;
                toDeactivate._Status = EntityStatus.Inactive;

                _regionRepository.Save(toDeactivate);

                var deactivated = _regionRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated region  to status  [{0}]", deactivated._Status));

                //Activate region
                var toActivate = updatedRegion;
                toActivate._Status = EntityStatus.Active;

                _regionRepository.Save(toActivate);

                var activated = _regionRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated region to status  [{0}]", activated._Status));

                //Delete region
                var toDelete = updatedRegion;
                toDelete._Status = EntityStatus.Deleted;

                _regionRepository.Save(toDelete);

                var deleted = _regionRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted region to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }

        }

        private void AssertRegion(Region province, Region savedProvince)
        {
            Assert.AreEqual(province.Name, savedProvince.Name);
            Assert.AreEqual(province.Description, savedProvince.Description);
            Assert.AreEqual(province.Country, savedProvince.Country);
            Assert.AreEqual(province._Status, EntityStatus.Active);
        }
    }
}