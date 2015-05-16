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
    public class CommodityTypeRepositoryFixture
    {
        private static ICommodityTypeRepository _commodityTypeRepository;
        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _commodityTypeRepository = _testHelper.Ioc<ICommodityTypeRepository>();
        }

        [Test]
        public void CommodityTypeRepositoryUnitTest()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START COMMODITY TYPE REPOSITORY UNIT TEST....");

                //Save commodity type
                var commodityType = _testHelper.BuildCommodityType();
                Trace.WriteLine(string.Format("Created Commodity Type [{0}]", commodityType.Name));
                var toSaveCommodityType = _commodityTypeRepository.Save(commodityType);
                Trace.WriteLine(string.Format("Saved Commodity Type Id [{0}]", toSaveCommodityType));
                var savedCommodityType = _commodityTypeRepository.GetById(toSaveCommodityType);

                AssertCommodityType(commodityType, savedCommodityType);

                //Commodity type listing
                var queryResult =
                    _commodityTypeRepository.Query(new QueryStandard() { Name = commodityType.Name });
                Assert.GreaterOrEqual(queryResult.Count,1);
                Trace.WriteLine(string.Format("Commodity Type [{0}] exists in listing", commodityType.Name));

                //Update commodity type
                var toUpdateCommodityType = savedCommodityType;
                toUpdateCommodityType.Name = "Commodity Type 2";
                toUpdateCommodityType.Description = "Commodity Type 2";

                _commodityTypeRepository.Save(toUpdateCommodityType);

                var updatedCommodityType = _commodityTypeRepository.GetById(toUpdateCommodityType.Id);
                Trace.WriteLine(string.Format("Updated Commodity Type to Name  [{0}]", updatedCommodityType.Name));

                AssertCommodityType(toUpdateCommodityType,updatedCommodityType);

                //Deactivate commodity type
                var toDeactivate = updatedCommodityType;
                toDeactivate._Status = EntityStatus.Inactive;

                _commodityTypeRepository.Save(toDeactivate);

                var deactivated = _commodityTypeRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status,EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated Commodity Type to status  [{0}]", deactivated._Status));

                //Activate commodity type
                var toActivate = updatedCommodityType;
                toActivate._Status = EntityStatus.Active;

                _commodityTypeRepository.Save(toActivate);

                var activated = _commodityTypeRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated Commodity Type to status  [{0}]", activated._Status));
                
                //Delete commodity type
                var toDelete = updatedCommodityType;
                toDelete._Status = EntityStatus.Deleted;

                _commodityTypeRepository.Save(toDelete);

                var deleted = _commodityTypeRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted Commodity Type to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertCommodityType(CommodityType commodityType, CommodityType savedCommodityType)
        {
            Assert.AreEqual(commodityType.Code,savedCommodityType.Code);
            Assert.AreEqual(commodityType.Name,savedCommodityType.Name);
            Assert.AreEqual(commodityType.Description,savedCommodityType.Description);
            Assert.AreEqual(commodityType._Status,savedCommodityType._Status);
        }
    }
}
