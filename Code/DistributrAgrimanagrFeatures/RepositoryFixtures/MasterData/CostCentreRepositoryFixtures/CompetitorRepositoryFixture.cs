using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CompetitorManagement;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CompetitorManagement;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using NUnit.Framework;

namespace Distributr.Core.Data._2015.Tests.RepositoryFixtures.MasterData.CostCentreRepositoryFixtures
{
    [TestFixture]
    public class CompetitorRepositoryFixture : RepositoryBaseFixture
    {
        private static ICompetitorRepository _competitorRepository;

        [SetUp]
        public void Setup()
        {
            Setup_Helper();

            _competitorRepository = _testHelper.Ioc<ICompetitorRepository>();
        }

        [Test]
        public void CompetitorRepositoryUnitTests()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START COMPETITOR REPOSITORY UNIT TEST....");

                //Save competitor
                var competitor = _testHelper.BuildCompetitor();
                Trace.WriteLine(string.Format("Created competitor [{0}]", competitor.Name));
                var toSaveCompetitor = _competitorRepository.Save(competitor);
                Trace.WriteLine(string.Format("Saved competitor Id [{0}]", toSaveCompetitor));
                var savedCompetitor = _competitorRepository.GetById(toSaveCompetitor);

                AssertCompetitor(competitor, savedCompetitor);

                //Competitor listing
                var queryResult =
                    _competitorRepository.Query(new QueryStandard() { Name = competitor.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Competitor [{0}] exists in listing", competitor.Name));

                //Update competitor
                var toUpdateCompetitor = savedCompetitor;
                toUpdateCompetitor.Name = "Competitor 2";

                _competitorRepository.Save(toUpdateCompetitor);

                var updatedCompetitor = _competitorRepository.GetById(toUpdateCompetitor.Id);
                Trace.WriteLine(string.Format("Updated competitor to Name  [{0}]", updatedCompetitor.Name));

                AssertCompetitor(toUpdateCompetitor, updatedCompetitor);

                //Deactivate competitor
                var toDeactivate = updatedCompetitor;
                toDeactivate._Status = EntityStatus.Inactive;

                _competitorRepository.Save(toDeactivate);

                var deactivated = _competitorRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated competitor  to status  [{0}]", deactivated._Status));

                //Activate competitor
                var toActivate = updatedCompetitor;
                toActivate._Status = EntityStatus.Active;

                _competitorRepository.Save(toActivate);

                var activated = _competitorRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated competitor to status  [{0}]", activated._Status));

                //Delete competitor
                var toDelete = updatedCompetitor;
                toDelete._Status = EntityStatus.Deleted;

                _competitorRepository.Save(toDelete);

                var deleted = _competitorRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted competitor to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertCompetitor(Competitor competitor, Competitor savedCompetitor)
        {
            Assert.AreEqual(competitor.Name,savedCompetitor.Name);
            Assert.AreEqual(competitor.ContactPerson,savedCompetitor.ContactPerson);
            Assert.AreEqual(competitor.City,savedCompetitor.City);
            Assert.AreEqual(competitor.Lattitude,savedCompetitor.Lattitude);
            Assert.AreEqual(competitor.PhysicalAddress,savedCompetitor.PhysicalAddress);
            Assert.AreEqual(competitor.PostalAddress,savedCompetitor.PostalAddress);
            Assert.AreEqual(competitor.Telephone,savedCompetitor.Telephone);
            Assert.AreEqual(competitor._Status,EntityStatus.Active);
        }
    }
}