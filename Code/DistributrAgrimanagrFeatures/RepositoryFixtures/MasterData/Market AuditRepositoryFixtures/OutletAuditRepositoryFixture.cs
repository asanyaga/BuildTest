using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.MarketAudit;
using Distributr.Core.Repository.Master.MarketAuditRepositories;
using Distributr.Core.Utility.Caching;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures
{
    [TestFixture]
    public class OutletAuditRepositoryFixture
    {
        private static IOutletAuditRepository _outletAuditRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _outletAuditRepository = _testHelper.Ioc<IOutletAuditRepository>();
        }

        [Test]
        public void OutletAuditRepositoryOwnerUnitTest()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START OUTLET AUDIT REPOSITORY UNIT TEST....");

                //Save Outlet audit
                var marketAudit = _testHelper.BuildOutletAudit();
                Trace.WriteLine(string.Format("Created outlet audit [{0}]", marketAudit.Question));
                var toSaveMarketAudit = _outletAuditRepository.Save(marketAudit);
                Trace.WriteLine(string.Format("Saved outlet audit Id [{0}]", toSaveMarketAudit));
                var savedMarketAudit = _outletAuditRepository.GetById(toSaveMarketAudit);

                AssertMarketAudit(marketAudit, savedMarketAudit);

                //Update outlet audit
                var toUpdateMarketAudit = savedMarketAudit;
                toUpdateMarketAudit.Question = "Outlet 2";

                _outletAuditRepository.Save(toUpdateMarketAudit);

                var updatedMarketAudit = _outletAuditRepository.GetById(toUpdateMarketAudit.Id);
                Trace.WriteLine(string.Format("Updated market outlet to Name  [{0}]", updatedMarketAudit.Question));

                AssertMarketAudit(toUpdateMarketAudit, updatedMarketAudit);

                //Deactivate outlet audit
                var toDeactivate = updatedMarketAudit;
                toDeactivate._Status = EntityStatus.Inactive;

                _outletAuditRepository.Save(toDeactivate);

                var deactivated = _outletAuditRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated outlet audit to status  [{0}]", deactivated._Status));

                //Activate outlet audit
                var toActivate = updatedMarketAudit;
                toActivate._Status = EntityStatus.Active;

                _outletAuditRepository.Save(toActivate);

                var activated = _outletAuditRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated outlet audit to status  [{0}]", activated._Status));

                //Delete outlet audit
                var toDelete = updatedMarketAudit;
                toDelete._Status = EntityStatus.Deleted;

                _outletAuditRepository.Save(toDelete);

                var deleted = _outletAuditRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted outlet audit to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertMarketAudit(OutletAudit infection, OutletAudit savedInfection)
        {
            Assert.AreEqual(infection.Question, savedInfection.Question);
            Assert.AreEqual(infection._Status, savedInfection._Status);
        }
    }
}