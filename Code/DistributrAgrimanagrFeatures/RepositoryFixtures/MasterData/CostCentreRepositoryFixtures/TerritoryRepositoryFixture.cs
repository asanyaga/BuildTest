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
    public class TerritoryRepositoryFixture
    {
        private static ITerritoryRepository _territoryRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();

            _territoryRepository = _testHelper.Ioc<ITerritoryRepository>();
        }

        [Test]
        public void TerritoryRepositoryUnitTests()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START TERRITORY REPOSITORY UNIT TEST....");

                //Save territory
                var territory = _testHelper.BuildTerritory();
                Trace.WriteLine(string.Format("Created territory [{0}]", territory.Name));
                var toSaveTerritory = _territoryRepository.Save(territory);
                Trace.WriteLine(string.Format("Saved territory Id [{0}]", toSaveTerritory));
                var savedTerritory = _territoryRepository.GetById(toSaveTerritory);

                AssertOutletType(territory, savedTerritory);

                //Territory listing
                var queryResult =
                    _territoryRepository.Query(new QueryStandard() { Name = territory.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Territory [{0}] exists in listing", territory.Name));

                //Update territory
                var toUpdateTerritory = savedTerritory;
                toUpdateTerritory.Name = "Territory 2";

                _territoryRepository.Save(toUpdateTerritory);

                var updatedTerritory = _territoryRepository.GetById(toUpdateTerritory.Id);
                Trace.WriteLine(string.Format("Updated territory to Name  [{0}]", updatedTerritory.Name));

                AssertOutletType(toUpdateTerritory, updatedTerritory);

                //Deactivate territory
                var toDeactivate = updatedTerritory;
                toDeactivate._Status = EntityStatus.Inactive;

                _territoryRepository.Save(toDeactivate);

                var deactivated = _territoryRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated territory  to status  [{0}]", deactivated._Status));

                //Activate territory
                var toActivate = updatedTerritory;
                toActivate._Status = EntityStatus.Active;

                _territoryRepository.Save(toActivate);

                var activated = _territoryRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated territory to status  [{0}]", activated._Status));

                //Delete territory
                var toDelete = updatedTerritory;
                toDelete._Status = EntityStatus.Deleted;

                _territoryRepository.Save(toDelete);

                var deleted = _territoryRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted territory to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertOutletType(Territory territory, Territory savedTerritory)
        {
            Assert.AreEqual(territory.Name,savedTerritory.Name);
            Assert.AreEqual(territory._Status,EntityStatus.Active);
        }
    }
}
