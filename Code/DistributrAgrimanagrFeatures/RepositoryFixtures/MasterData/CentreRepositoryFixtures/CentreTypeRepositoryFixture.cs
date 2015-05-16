using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.CentreRepositoryFixtures
{
    [TestFixture]
    public class CentreTypeRepositoryFixture
    {
        private static ICentreTypeRepository _centreTypeRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();

            _centreTypeRepository = _testHelper.Ioc<ICentreTypeRepository>();
        }

        [Test]
        public void CentreTypeRepositoryUnitTests()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START CENTRE TYPE REPOSITORY UNIT TEST....");

                //Save centre type
                var centreType = _testHelper.BuildCentreType();
                Trace.WriteLine(string.Format("Created centre type [{0}]", centreType.Name));
                var toSaveCentreType = _centreTypeRepository.Save(centreType);
                Trace.WriteLine(string.Format("Saved centre type Id [{0}]", toSaveCentreType));
                var savedCentreType = _centreTypeRepository.GetById(toSaveCentreType);

                AssertCentreType(centreType, savedCentreType);

                //Centre type listing
                var queryResult =
                    _centreTypeRepository.Query(new QueryStandard() { Name = centreType.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Centre type [{0}] exists in listing", centreType.Name));

                //Update centre type
                var toUpdateCentreType = savedCentreType;
                toUpdateCentreType.Name = "Centre type 2";

                _centreTypeRepository.Save(toUpdateCentreType);

                var updatedCentreType = _centreTypeRepository.GetById(toUpdateCentreType.Id);
                Trace.WriteLine(string.Format("Updated centre type to Name  [{0}]", updatedCentreType.Name));

                AssertCentreType(toUpdateCentreType, updatedCentreType);

                //Deactivate centre type
                var toDeactivate = updatedCentreType;
                toDeactivate._Status = EntityStatus.Inactive;

                _centreTypeRepository.Save(toDeactivate);

                var deactivated = _centreTypeRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated centre type  to status  [{0}]", deactivated._Status));

                //Activate centre type
                var toActivate = updatedCentreType;
                toActivate._Status = EntityStatus.Active;

                _centreTypeRepository.Save(toActivate);

                var activated = _centreTypeRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated centre type to status  [{0}]", activated._Status));

                //Delete centre type
                var toDelete = updatedCentreType;
                toDelete._Status = EntityStatus.Deleted;

                _centreTypeRepository.Save(toDelete);

                var deleted = _centreTypeRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted centre type to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertCentreType(CentreType competitor, CentreType savedCompetitor)
        {
            Assert.AreEqual(competitor.Name,savedCompetitor.Name);
            Assert.AreEqual(competitor.Code,savedCompetitor.Code);
            Assert.AreEqual(competitor.Description,savedCompetitor.Description);
            Assert.AreEqual(competitor._Status,EntityStatus.Active);
        }
    }
}