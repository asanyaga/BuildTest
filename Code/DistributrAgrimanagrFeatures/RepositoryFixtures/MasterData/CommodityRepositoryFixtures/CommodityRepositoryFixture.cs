using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.CommodityRepositoryFixtures
{
    [TestFixture]
    public class CommodityRepositoryFixture
    {
        private static ICommodityRepository _commodityRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _commodityRepository = _testHelper.Ioc<ICommodityRepository>();
        }

        [Test]
        public void CommodityRepositoryUnitTest()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START COMMODITY REPOSITORY UNIT TEST....");

                //Save commodity
                var commodity = _testHelper.BuildCommodity();
                Trace.WriteLine(string.Format("Created Commodity [{0}]", commodity.Name));
                var toSaveCommodity = _commodityRepository.Save(commodity);
                Trace.WriteLine(string.Format("Saved Commodity Id [{0}]", toSaveCommodity));
                var savedCommodity = _commodityRepository.GetById(toSaveCommodity);

                AssertCommodity(commodity, savedCommodity);

                //Commodity listing
                var queryResult =
                    _commodityRepository.Query(new QueryStandard() { Name = commodity.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Commodity [{0}] exists in listing", commodity.Name));

                //Update commodity
                var toUpdateCommodity = savedCommodity;
                toUpdateCommodity.Name = "Commodity 2";
                toUpdateCommodity.Description = "Commodity 2";

                _commodityRepository.Save(toUpdateCommodity);

                var updatedCommodity = _commodityRepository.GetById(toUpdateCommodity.Id);
                Trace.WriteLine(string.Format("Updated Commodity to Name  [{0}]", updatedCommodity.Name));

                AssertCommodity(toUpdateCommodity, updatedCommodity);

                //Deactivate commodity
                var toDeactivate = updatedCommodity;
                toDeactivate._Status = EntityStatus.Inactive;

                _commodityRepository.Save(toDeactivate);

                var deactivated = _commodityRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated Commodity to status  [{0}]", deactivated._Status));

                //Activate commodity
                var toActivate = updatedCommodity;
                toActivate._Status = EntityStatus.Active;

                _commodityRepository.Save(toActivate);

                var activated = _commodityRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated Commodity to status  [{0}]", activated._Status));

                //Delete commodity
                var toDelete = updatedCommodity;
                toDelete._Status = EntityStatus.Deleted;

                _commodityRepository.Save(toDelete);

                var deleted = _commodityRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted Commodity to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertCommodity(Commodity commodity, Commodity savedCommodity)
        {
            Assert.AreEqual(commodity.Code, savedCommodity.Code);
            Assert.AreEqual(commodity.Name, savedCommodity.Name);
            Assert.AreEqual(commodity.Description, savedCommodity.Description);
            Assert.AreEqual(commodity._Status, savedCommodity._Status);
        }
    }
}
