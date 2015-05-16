using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.EquipmentRepositoryFixtures
{
    [TestFixture]
    public class ContainerTypeRepositoryFixture
    {
        private static IContainerTypeRepository _containerTypeRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _containerTypeRepository = _testHelper.Ioc<IContainerTypeRepository>();
        }

        [Test]
        public void ContainerTypeRepositoryOwnerUnitTest()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START CONTAINER TYPE REPOSITORY UNIT TEST....");

                //Save container type
                var containerType = _testHelper.BuildContainerType();
                Trace.WriteLine(string.Format("Created Container Type [{0}]", containerType.Name));
                var toSaveContainerType = _containerTypeRepository.Save(containerType);
                Trace.WriteLine(string.Format("Saved Container Type Id [{0}]", toSaveContainerType));
                var savedContainerType = _containerTypeRepository.GetById(toSaveContainerType);

                AssertContainerType(containerType, savedContainerType);

                //Container Type listing
                var queryResult =
                    _containerTypeRepository.Query(new QueryStandard() { Name = containerType.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Container Type [{0}] exists in listing", containerType.Name));

                //Update container type
                var toUpdateCommodityOwner = savedContainerType;
                toUpdateCommodityOwner.Name = "Container Type 2";
                toUpdateCommodityOwner.Description = "Container Type 2";

                _containerTypeRepository.Save(toUpdateCommodityOwner);

                var updatedCommodityOwner = _containerTypeRepository.GetById(toUpdateCommodityOwner.Id);
                Trace.WriteLine(string.Format("Updated Container Type to Name  [{0}]", updatedCommodityOwner.Name));

                AssertContainerType(toUpdateCommodityOwner, updatedCommodityOwner);

                //Deactivate container type
                var toDeactivate = updatedCommodityOwner;
                toDeactivate._Status = EntityStatus.Inactive;

                _containerTypeRepository.Save(toDeactivate);

                var deactivated = _containerTypeRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated Container Type to status  [{0}]", deactivated._Status));

                //Activate container type
                var toActivate = updatedCommodityOwner;
                toActivate._Status = EntityStatus.Active;

                _containerTypeRepository.Save(toActivate);

                var activated = _containerTypeRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated Container Type to status  [{0}]", activated._Status));

                //Delete container type
                var toDelete = updatedCommodityOwner;
                toDelete._Status = EntityStatus.Deleted;

                _containerTypeRepository.Save(toDelete);

                var deleted = _containerTypeRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted Container Type to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertContainerType(ContainerType commodityOwnerType, ContainerType savedCommodityOwnerType)
        {
            Assert.AreEqual(commodityOwnerType.Code, savedCommodityOwnerType.Code);
            Assert.AreEqual(commodityOwnerType.Name, savedCommodityOwnerType.Name);
            Assert.AreEqual(commodityOwnerType.Code, savedCommodityOwnerType.Code);
            Assert.AreEqual(commodityOwnerType.Description, savedCommodityOwnerType.Description);
            Assert.AreEqual(commodityOwnerType.BubbleSpace, savedCommodityOwnerType.BubbleSpace);
            Assert.AreEqual(commodityOwnerType.FreezerTemp, savedCommodityOwnerType.FreezerTemp);
            Assert.AreEqual(commodityOwnerType.Height, savedCommodityOwnerType.Height);
            Assert.AreEqual(commodityOwnerType.Length, savedCommodityOwnerType.Length);
            Assert.AreEqual(commodityOwnerType.LoadCarriage, savedCommodityOwnerType.LoadCarriage);
            Assert.AreEqual(commodityOwnerType.TareWeight, savedCommodityOwnerType.TareWeight);
            Assert.AreEqual(commodityOwnerType.Volume, savedCommodityOwnerType.Volume);
            Assert.AreEqual(commodityOwnerType.Width, savedCommodityOwnerType.Width);
            Assert.AreEqual(commodityOwnerType.Make, savedCommodityOwnerType.Make);
            Assert.AreEqual(commodityOwnerType.Model, savedCommodityOwnerType.Model);
            Assert.AreEqual(commodityOwnerType.ContainerUseType, savedCommodityOwnerType.ContainerUseType);
            Assert.AreEqual(commodityOwnerType._Status, savedCommodityOwnerType._Status);
        }
    }
}