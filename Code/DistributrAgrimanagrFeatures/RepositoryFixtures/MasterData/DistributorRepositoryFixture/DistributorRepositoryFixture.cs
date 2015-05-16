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

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.DistributorRepositoryFixture
{
    [TestFixture]
    public class DistributorRepositoryFixture
    {
        private static IDistributorRepository _distributorRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();

            _distributorRepository = _testHelper.Ioc<IDistributorRepository>();
        }

        [Test]
        public void DistributorRepositoryUnitTests()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START DISTRIBUTOR REPOSITORY UNIT TEST....");

                //Save distributor
                var region = _testHelper.BuildRegion();
                var toSaveRegion = _testHelper.Ioc<IRegionRepository>().Save(region);
                var savedRegion = _testHelper.Ioc<IRegionRepository>().GetById(toSaveRegion);

                var distributor = _testHelper.BuildDistributorCC(savedRegion);
                Trace.WriteLine(string.Format("Created distributor [{0}]", distributor.Name));
                var toSaveDistributor = _distributorRepository.Save(distributor);
                Trace.WriteLine(string.Format("Saved distributor Id [{0}]", toSaveDistributor));
                var savedDistributor = _distributorRepository.GetById(toSaveDistributor) as Distributor;

                AssertDistributor(distributor, savedDistributor);

                //Centre distributor
                var queryResult =
                    _distributorRepository.Query(new QueryStandard() { Name = distributor.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Distributor [{0}] exists in listing", distributor.Name));

                //Update distributor
                var toUpdateDistributor = savedDistributor;
                toUpdateDistributor.Name = "Distributor 2";

                _distributorRepository.Save(toUpdateDistributor);

                var updatedCentreTypeDistributor = _distributorRepository.GetById(toUpdateDistributor.Id) as Distributor;
                Trace.WriteLine(string.Format("Updated distributor to Name  [{0}]", updatedCentreTypeDistributor.Name));

                AssertDistributor(toUpdateDistributor, updatedCentreTypeDistributor);

                //Deactivate distributor
                var toDeactivate = updatedCentreTypeDistributor;
                toDeactivate._Status = EntityStatus.Inactive;

                _distributorRepository.Save(toDeactivate);

                var deactivated = _distributorRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated distributor to status  [{0}]", deactivated._Status));

                //Activate distributor
                var toActivate = updatedCentreTypeDistributor;
                toActivate._Status = EntityStatus.Active;

                _distributorRepository.Save(toActivate);

                var activated = _distributorRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated distributor to status  [{0}]", activated._Status));

                //Delete distributor
                var toDelete = updatedCentreTypeDistributor;
                toDelete._Status = EntityStatus.Deleted;

                _distributorRepository.Save(toDelete);

                var deleted = _distributorRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted distributor to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertDistributor(Distributor competitor, Distributor savedCompetitor)
        {
            Assert.AreEqual(competitor.Name,savedCompetitor.Name);
            Assert.AreEqual(competitor._Status,EntityStatus.Active);
        }
    }
}