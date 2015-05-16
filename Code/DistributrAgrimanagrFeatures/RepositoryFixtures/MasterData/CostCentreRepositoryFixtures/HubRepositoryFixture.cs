using System;
using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Caching;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.CostCentreRepositoryFixtures
{
    [TestFixture]
   public class HubRepositoryFixture
    {
        private static ICostCentreRepository _costCentreRepository;
        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _costCentreRepository = _testHelper.Ioc<ICostCentreRepository>();
        }

        [Test]
        public void HubRepositoryUnitTest()
        {
            using (var tra = new TransactionScope())
            {
                Trace.WriteLine("START HUB REPOSITORY UNIT TEST....");

                var route = _testHelper.BuildRoute();
                var toSaveRoute = _testHelper.Ioc<IRouteRepository>().Save(route);
                var savedRoute = _testHelper.Ioc<IRouteRepository>().GetById(toSaveRoute);
                Hub newHub = _testHelper.BuildHub(savedRoute.Region);
                Guid newHubId = _costCentreRepository.Save(newHub);
                var createdHub = _costCentreRepository.GetById(newHubId) as Hub;
                AssertCostCernter(createdHub, newHub);
                if (createdHub != null)
                    Trace.WriteLine(string.Format("Created HubId [{0}],-Name[{1}]", newHubId, createdHub.Name));



                //Update
                Hub hubToUpdate = _testHelper.Ioc<ICostCentreFactory>().CreateCostCentre(createdHub.Id,
                    CostCentreType.Hub,
                    _testHelper.Ioc<ICostCentreRepository>().GetById(createdHub.ParentCostCentre.Id)) as Hub;
                hubToUpdate.Name = "UpdateHubCC";
                hubToUpdate.CostCentreCode = "UdatedCode_2001";
                hubToUpdate.Region = createdHub.Region;
                Guid hubToUpdateId = _costCentreRepository.Save(hubToUpdate);
                var updatedhub = _costCentreRepository.GetById(hubToUpdateId) as Hub;
                AssertCostCernter(hubToUpdate, updatedhub);
                Trace.WriteLine(string.Format("Updated hubCCId [{0}],-Name[{1}], RegionName[{2}]", hubToUpdateId, updatedhub.Name, updatedhub.Region.Name));


                //hubCC Status Change
                _costCentreRepository.SetInactive(updatedhub);
                Hub hubStatus =
                    _costCentreRepository.GetById(hubToUpdateId) as Hub;
                Assert.AreEqual(hubStatus._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("hubCC  Status[{0}]", hubStatus._Status));


                _costCentreRepository.SetActive(updatedhub);
                hubStatus =
                   _costCentreRepository.GetById(hubToUpdateId) as Hub;
                Assert.AreEqual(hubStatus._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("hubCC  Status[{0}]", hubStatus._Status));

                _costCentreRepository.SetAsDeleted(updatedhub);
                hubStatus =
                   _costCentreRepository.GetById(hubToUpdateId) as Hub;
                Assert.AreEqual(hubStatus._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("hubCC  Status[{0}]", hubStatus._Status));
                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertCostCernter(Hub hubX, Hub hubY)
        {
            Assert.NotNull(hubX.Id);
            Assert.AreEqual(hubX.CostCentreType, hubY.CostCentreType);
            Assert.AreEqual(hubX.Name, hubY.Name);
        }
    }
}
