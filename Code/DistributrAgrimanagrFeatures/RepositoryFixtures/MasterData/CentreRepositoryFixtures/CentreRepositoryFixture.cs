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
    public class CentreRepositoryFixture
    {
        private static ICentreRepository _centreRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();

            _centreRepository = _testHelper.Ioc<ICentreRepository>();
        }

        [Test]
        public void CentreRepositoryUnitTests()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START CENTRE REPOSITORY UNIT TEST....");

                //Save centre 
                var centre = _testHelper.BuildCentre();
                Trace.WriteLine(string.Format("Created centre [{0}]", centre.Name));
                var toSaveCentre = _centreRepository.Save(centre);
                Trace.WriteLine(string.Format("Saved centre Id [{0}]", toSaveCentre));
                var savedCentre = _centreRepository.GetById(toSaveCentre);

                AssertCentre(centre, savedCentre);

                //Centre listing
                var queryResult =
                    _centreRepository.Query(new QueryStandard() { Name = centre.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Centre [{0}] exists in listing", centre.Name));

                //Update centre
                var toUpdateCentre = savedCentre;
                toUpdateCentre.Name = "Centre 2";

                _centreRepository.Save(toUpdateCentre);

                var updatedCentre = _centreRepository.GetById(toUpdateCentre.Id);
                Trace.WriteLine(string.Format("Updated centre to Name  [{0}]", updatedCentre.Name));

                AssertCentre(toUpdateCentre, updatedCentre);

                //Deactivate centre
                var toDeactivate = updatedCentre;
                toDeactivate._Status = EntityStatus.Inactive;

                _centreRepository.Save(toDeactivate);

                var deactivated = _centreRepository.GetById(toDeactivate.Id,true);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated centre to status  [{0}]", deactivated._Status));

                //Activate centre
                var toActivate = updatedCentre;
                toActivate._Status = EntityStatus.Active;

                _centreRepository.Save(toActivate);

                var activated = _centreRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated centre to status  [{0}]", activated._Status));

                //Delete centre
                var toDelete = updatedCentre;
                toDelete._Status = EntityStatus.Deleted;

                _centreRepository.Save(toDelete);

                var deleted = _centreRepository.GetById(toActivate.Id,false);
                Assert.IsNull( deleted);
                Trace.WriteLine(string.Format("Deleted centre"));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertCentre(Centre centre, Centre savedCentre)
        {
            Assert.AreEqual(centre.Name,savedCentre.Name);
            Assert.AreEqual(centre.Code,savedCentre.Code);
            Assert.AreEqual(centre.Description,savedCentre.Description);
            Assert.AreEqual(centre.CenterType.Code,savedCentre.CenterType.Code);
            Assert.AreEqual(centre.Route.Code,savedCentre.Route.Code);
            Assert.AreEqual(centre.Hub.Id, savedCentre.Hub.Id);
            Assert.AreEqual(centre._Status,EntityStatus.Active);
        }
    }
}