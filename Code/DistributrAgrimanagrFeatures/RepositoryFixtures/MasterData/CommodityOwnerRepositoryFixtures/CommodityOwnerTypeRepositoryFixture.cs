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

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.CommodityOwnerRepositoryFixtures
{
    [TestFixture]
    public class CommodityOwnerTypeRepositoryFixture
    {
        private static ICommodityOwnerTypeRepository _commodityOwnerTypeRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _commodityOwnerTypeRepository = _testHelper.Ioc<ICommodityOwnerTypeRepository>();
        }

        [Test]
        public void CommodityOwnerRepositoryOwnerUnitTest()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START COMMODITY OWNER TYPE REPOSITORY UNIT TEST....");

                //Save commodity type
                var commodityOwnerType = _testHelper.BuildCommodityOwnerType();
                Trace.WriteLine(string.Format("Created Commodity Owner Type [{0}]", commodityOwnerType.Name));
                var toSaveCommodityOwnerType = _commodityOwnerTypeRepository.Save(commodityOwnerType);
                Trace.WriteLine(string.Format("Saved Commodity Owner Type Id [{0}]", toSaveCommodityOwnerType));
                var savedCommodityOwnerType = _commodityOwnerTypeRepository.GetById(toSaveCommodityOwnerType);

                AssertCommodityType(commodityOwnerType, savedCommodityOwnerType);

                //Commodity type listing
                var queryResult =
                    _commodityOwnerTypeRepository.Query(new QueryStandard() { Name = commodityOwnerType.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Commodity Owner Type [{0}] exists in listing", commodityOwnerType.Name));

                //Update commodity type
                var toUpdateCommodityOwnerType = savedCommodityOwnerType;
                toUpdateCommodityOwnerType.Name = "Commodity Owner Type 2";
                toUpdateCommodityOwnerType.Description = "Commodity Owner Type 2";

                _commodityOwnerTypeRepository.Save(toUpdateCommodityOwnerType);

                var updatedCommodityOwnerType = _commodityOwnerTypeRepository.GetById(toUpdateCommodityOwnerType.Id);
                Trace.WriteLine(string.Format("Updated Commodity Owner Type to Name  [{0}]", updatedCommodityOwnerType.Name));

                AssertCommodityType(toUpdateCommodityOwnerType, updatedCommodityOwnerType);

                //Deactivate commodity type
                var toDeactivate = updatedCommodityOwnerType;
                toDeactivate._Status = EntityStatus.Inactive;

                _commodityOwnerTypeRepository.Save(toDeactivate);

                var deactivated = _commodityOwnerTypeRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated Commodity Owner Type to status  [{0}]", deactivated._Status));

                //Activate commodity type
                var toActivate = updatedCommodityOwnerType;
                toActivate._Status = EntityStatus.Active;

                _commodityOwnerTypeRepository.Save(toActivate);

                var activated = _commodityOwnerTypeRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated Commodity Owner Type to status  [{0}]", activated._Status));

                //Delete commodity type
                var toDelete = updatedCommodityOwnerType;
                toDelete._Status = EntityStatus.Deleted;

                _commodityOwnerTypeRepository.Save(toDelete);

                var deleted = _commodityOwnerTypeRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted Commodity Owner Type to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertCommodityType(CommodityOwnerType commodityOwnerType, CommodityOwnerType savedCommodityOwnerType)
        {
            Assert.AreEqual(commodityOwnerType.Code,savedCommodityOwnerType.Code);
            Assert.AreEqual(commodityOwnerType.Name,savedCommodityOwnerType.Name);
            Assert.AreEqual(commodityOwnerType.Description,savedCommodityOwnerType.Description);
            Assert.AreEqual(commodityOwnerType._Status,savedCommodityOwnerType._Status);
        }
    }
}
