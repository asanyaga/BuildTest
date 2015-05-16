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
    public class EquipmentRepositoryFixture
    {
        private static IEquipmentRepository _equipmentRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _equipmentRepository = _testHelper.Ioc<IEquipmentRepository>();
        }

        [Test]
        public void EquipmentRepositoryUnitTest()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START EQUIPMENT REPOSITORY UNIT TEST....");

                //Save equipment
                var equipments = _testHelper.BuildEquipments();
                foreach (var container in equipments)
                {
                    Trace.WriteLine(string.Format("Created equipment [{0}]", container.Name));
                    var toSaveContainer = _equipmentRepository.Save(container);
                    Trace.WriteLine(string.Format("Saved equipment Id [{0}]", toSaveContainer));
                    var savedContainer = _equipmentRepository.GetById(toSaveContainer);

                    AssertContainer(container, savedContainer);

                    //Equipment listing
                    var queryResult =
                        _equipmentRepository.Query(new QueryEquipment() { Name = container.Code, EquipmentType = (int)container.EquipmentType});
                    Assert.GreaterOrEqual(queryResult.Count, 1);
                    Trace.WriteLine(string.Format("Equipment [{0}] exists in listing", container.Name));

                    //Update equipment
                    var toUpdateCommodity = savedContainer;
                    toUpdateCommodity.Name = "Equipment 2";
                    toUpdateCommodity.Description = "Equipment 2";

                    _equipmentRepository.Save(toUpdateCommodity);

                    var updatedCommodity = _equipmentRepository.GetById(toUpdateCommodity.Id);
                    Trace.WriteLine(string.Format("Updated equipment to Name  [{0}]", updatedCommodity.Name));

                    AssertContainer(toUpdateCommodity, updatedCommodity);

                    //Deactivate equipment
                    var toDeactivate = updatedCommodity;
                    toDeactivate._Status = EntityStatus.Inactive;

                    _equipmentRepository.Save(toDeactivate);

                    var deactivated = _equipmentRepository.GetById(toDeactivate.Id);
                    Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                    Trace.WriteLine(string.Format("Deactivated equipment to status  [{0}]", deactivated._Status));

                    //Activate equipment
                    var toActivate = updatedCommodity;
                    toActivate._Status = EntityStatus.Active;

                    _equipmentRepository.Save(toActivate);

                    var activated = _equipmentRepository.GetById(toActivate.Id);
                    Assert.AreEqual(activated._Status, EntityStatus.Active);
                    Trace.WriteLine(string.Format("Activated equipment to status  [{0}]", activated._Status));

                    //Delete equipment
                    var toDelete = updatedCommodity;
                    toDelete._Status = EntityStatus.Deleted;

                    _equipmentRepository.Save(toDelete);

                    var deleted = _equipmentRepository.GetById(toActivate.Id);
                    Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                    Trace.WriteLine(string.Format("Deleted equipment to status  [{0}]", deleted._Status));

                    _testHelper.Ioc<ICacheProvider>().Reset();
                }
            }
        }

        private void AssertContainer(Equipment equipment, Equipment savedEquipment)
        {
            Assert.AreEqual(equipment.Code, savedEquipment.Code);
            Assert.AreEqual(equipment.Name, savedEquipment.Name);
            Assert.AreEqual(equipment.Code, savedEquipment.Code);
            Assert.AreEqual(equipment.Description, savedEquipment.Description);
            Assert.AreEqual(equipment.Make, savedEquipment.Make);
            Assert.AreEqual(equipment.Model, savedEquipment.Model);
            Assert.AreEqual(equipment.EquipmentNumber, savedEquipment.EquipmentNumber);
            Assert.AreEqual(equipment.EquipmentType, savedEquipment.EquipmentType);
            Assert.AreEqual(equipment.CostCentre.CostCentreCode, savedEquipment.CostCentre.CostCentreCode);
            Assert.AreEqual(equipment._Status, savedEquipment._Status);
        }
    }
}