using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.MarketAudit;
using Distributr.Core.Repository.Master.MarketAuditRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures
{
    [TestFixture]
    public class MarketAuditRepositoryFixture
    {
        private static IMarketAuditRepository _marketAuditRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _marketAuditRepository = _testHelper.Ioc<IMarketAuditRepository>();
        }

        [Test]
        public void MarketAuditRepositoryOwnerUnitTest()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START MARKET AUDIT REPOSITORY UNIT TEST....");

                //Save market audit
                var marketAudit = _testHelper.BuildMarketAudit();
                Trace.WriteLine(string.Format("Created market audit [{0}]", marketAudit.Question));
                var toSaveMarketAudit = _marketAuditRepository.Save(marketAudit);
                Trace.WriteLine(string.Format("Saved market audit Id [{0}]", toSaveMarketAudit));
                var savedMarketAudit = _marketAuditRepository.GetById(toSaveMarketAudit);

                AssertMarketAudit(marketAudit, savedMarketAudit);

                //Market audit listing
                var queryResult =
                    _marketAuditRepository.Query(new QueryStandard() { Name = marketAudit.Question});
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Market audit [{0}] exists in listing", marketAudit.Question));

                //Update market audit
                var toUpdateMarketAudit = savedMarketAudit;
                toUpdateMarketAudit.Question = "Question 2";

                _marketAuditRepository.Save(toUpdateMarketAudit);

                var updatedMarketAudit = _marketAuditRepository.GetById(toUpdateMarketAudit.Id);
                Trace.WriteLine(string.Format("Updated market audit to Name  [{0}]", updatedMarketAudit.Question));

                AssertMarketAudit(toUpdateMarketAudit, updatedMarketAudit);

                //Deactivate market audit
                var toDeactivate = updatedMarketAudit;
                toDeactivate._Status = EntityStatus.Inactive;

                _marketAuditRepository.Save(toDeactivate);

                var deactivated = _marketAuditRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated market audit to status  [{0}]", deactivated._Status));

                //Activate market audit
                var toActivate = updatedMarketAudit;
                toActivate._Status = EntityStatus.Active;

                _marketAuditRepository.Save(toActivate);

                var activated = _marketAuditRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated market audit to status  [{0}]", activated._Status));

                //Delete market audit
                var toDelete = updatedMarketAudit;
                toDelete._Status = EntityStatus.Deleted;

                _marketAuditRepository.Save(toDelete);

                var deleted = _marketAuditRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted market audit to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertMarketAudit(MarketAudit infection, MarketAudit savedInfection)
        {
            Assert.AreEqual(infection.Question, savedInfection.Question);
            Assert.AreEqual(infection.StartDate, savedInfection.StartDate);
            Assert.AreEqual(infection.EndDate, savedInfection.EndDate);
            Assert.AreEqual(infection._Status, savedInfection._Status);
        }
    }
}