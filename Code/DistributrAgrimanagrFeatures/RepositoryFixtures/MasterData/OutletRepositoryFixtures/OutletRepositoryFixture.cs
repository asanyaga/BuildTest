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

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.OutletRepositoryFixtures
{
    [TestFixture]
    public class OutletRepositoryFixture
    {
        private static IOutletRepository _outletRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();

            _outletRepository = _testHelper.Ioc<IOutletRepository>();
        }

        [Test]
        public void OutletRepositoryUnitTests()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START OUTLET REPOSITORY UNIT TEST....");

                //Save outlet
                var outlet = _testHelper.BuildOutlet();
                Trace.WriteLine(string.Format("Created outlet [{0}]", outlet.Name));
                var toSaveOutlet = _outletRepository.Save(outlet);
                Trace.WriteLine(string.Format("Saved outlet Id [{0}]", toSaveOutlet));
                var savedOutlet = _outletRepository.GetById(toSaveOutlet) as Outlet;

                AssertCompetitor(outlet, savedOutlet);

                //Competitor listing
                var queryResult =
                    _outletRepository.Query(new QueryStandard() { Name = outlet.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Competitor [{0}] exists in listing", outlet.Name));

                //Update outlet
                var toUpdateOutlet = savedOutlet;
                toUpdateOutlet.Name = "Outlet 2";

                _outletRepository.Save(toUpdateOutlet);

                var updatedOutlet = _outletRepository.GetById(toUpdateOutlet.Id) as Outlet;
                Trace.WriteLine(string.Format("Updated outlet to Name  [{0}]", updatedOutlet.Name));

                AssertCompetitor(toUpdateOutlet, updatedOutlet);

                //Deactivate outlet
                var toDeactivate = updatedOutlet;
                toDeactivate._Status = EntityStatus.Inactive;

                _outletRepository.Save(toDeactivate);

                var deactivated = _outletRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated outlet  to status  [{0}]", deactivated._Status));

                //Activate outlet
                var toActivate = updatedOutlet;
                toActivate._Status = EntityStatus.Active;

                _outletRepository.Save(toActivate);

                var activated = _outletRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated outlet to status  [{0}]", activated._Status));

                //Delete outlet
                var toDelete = updatedOutlet;
                toDelete._Status = EntityStatus.Deleted;

                _outletRepository.Save(toDelete);

                var deleted = _outletRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted outlet to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertCompetitor(Outlet competitor, Outlet savedCompetitor)
        {
            Assert.AreEqual(competitor.Name,savedCompetitor.Name);
            Assert.AreEqual(competitor.OutletCategory.Code,savedCompetitor.OutletCategory.Code);
            Assert.AreEqual(competitor.OutletType.Code,savedCompetitor.OutletType.Code);
            Assert.AreEqual(competitor.Route.Code,savedCompetitor.Route.Code);
            Assert.AreEqual(competitor.VatClass.VatClass,savedCompetitor.VatClass.VatClass);
            Assert.AreEqual(competitor.OutletProductPricingTier.Code,savedCompetitor.OutletProductPricingTier.Code);
            Assert.AreEqual(competitor.CostCentreCode,savedCompetitor.CostCentreCode);
            Assert.AreEqual(competitor.ContactPerson,savedCompetitor.ContactPerson);
            Assert.AreEqual(competitor._Status,EntityStatus.Active);
        }
    }
}