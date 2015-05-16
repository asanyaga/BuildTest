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
    public class VehicleRepositoryFixture
    {
        private static IVehicleRepository _vehicleRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _vehicleRepository = _testHelper.Ioc<IVehicleRepository>();
        }

        [Test]
        public void VehicleRepositoryOwnerUnitTest()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START VEHICLE REPOSITORY UNIT TEST....");

                //Save vehicle
                var vehicle = _testHelper.BuildVehicle();
                Trace.WriteLine(string.Format("Created vehicle [{0}]", vehicle.Name));
                var toSaveVehicle = _vehicleRepository.Save(vehicle);
                Trace.WriteLine(string.Format("Saved vehicle Id [{0}]", toSaveVehicle));
                var savedVehicle = _vehicleRepository.GetById(toSaveVehicle);

                AssertVehicle(vehicle, savedVehicle);

                //Vehicle listing
                QueryResult<Vehicle> queryResult =
                    _vehicleRepository.Query(new QueryEquipment() { Name = savedVehicle.Name ,EquipmentType = 4});
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Vehiclee [{0}] exists in listing", vehicle.Name));

                //Update vehicle
                var toUpdateVehicle = savedVehicle;
                toUpdateVehicle.Name = "Vehicle 2";
                toUpdateVehicle.Description = "Vehicle 2";

                _vehicleRepository.Save(toUpdateVehicle);

                var updatedVehicle = _vehicleRepository.GetById(toUpdateVehicle.Id);
                Trace.WriteLine(string.Format("Updated vehiclee to Name  [{0}]", updatedVehicle.Name));

                AssertVehicle(toUpdateVehicle, updatedVehicle);

                //Deactivate vehicle
                var toDeactivate = updatedVehicle;
                toDeactivate._Status = EntityStatus.Inactive;

                _vehicleRepository.Save(toDeactivate);

                var deactivated = _vehicleRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated vehicle to status  [{0}]", deactivated._Status));

                //Activate vehicle
                var toActivate = updatedVehicle;
                toActivate._Status = EntityStatus.Active;

                _vehicleRepository.Save(toActivate);

                var activated = _vehicleRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated vehicle to status  [{0}]", activated._Status));

                //Delete vehicle
                var toDelete = updatedVehicle;
                toDelete._Status = EntityStatus.Deleted;

                _vehicleRepository.Save(toDelete);

                var deleted = _vehicleRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted vehicle to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertVehicle(Vehicle commodityOwnerType, Vehicle savedCommodityOwnerType)
        {
            Assert.AreEqual(commodityOwnerType.Code, savedCommodityOwnerType.Code);
            Assert.AreEqual(commodityOwnerType.Name, savedCommodityOwnerType.Name);
            Assert.AreEqual(commodityOwnerType.Code, savedCommodityOwnerType.Code);
            Assert.AreEqual(commodityOwnerType.Description, savedCommodityOwnerType.Description);
            Assert.AreEqual(commodityOwnerType.Make, savedCommodityOwnerType.Make);
            Assert.AreEqual(commodityOwnerType.Model, savedCommodityOwnerType.Model);
            Assert.AreEqual(commodityOwnerType.EquipmentNumber, savedCommodityOwnerType.EquipmentNumber);
            Assert.AreEqual(commodityOwnerType.EquipmentType, savedCommodityOwnerType.EquipmentType);
            Assert.AreEqual(commodityOwnerType.CostCentre.CostCentreCode, savedCommodityOwnerType.CostCentre.CostCentreCode);
            Assert.AreEqual(commodityOwnerType._Status, savedCommodityOwnerType._Status);
        }
    }
}