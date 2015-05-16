using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Utility.Caching;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.InventoryRepositoryFixture
{
    [TestFixture]
    public class InventoryRepositoryFixture
    {
        private static IInventoryRepository _inventoryRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _inventoryRepository = _testHelper.Ioc<IInventoryRepository>();
        }

        [Test]
        public void InventoryRepositoryOwnerUnitTest()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START INVENTORY REPOSITORY UNIT TEST....");

                //Save inventory
                var inventory = _testHelper.BuildInventory();
                Trace.WriteLine(string.Format("Created inventory for product [{0}]", inventory.Product.Description));
                var toSaveInventory = _inventoryRepository.AddInventory(inventory);
                Trace.WriteLine(string.Format("Saved inventory Id [{0}]", toSaveInventory));
                var savedInventory = _inventoryRepository.GetById(toSaveInventory);

                AssertInventory(inventory, savedInventory);

                //Inventory listing
                var queryResult =
                    _inventoryRepository.Query(inventory.Warehouse.Id);
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Inventory [{0}] exists in listing", inventory.Product.Description));

                //Update inventory
                var toUpdateInventory = savedInventory;
                toUpdateInventory.Balance = 10;
                toUpdateInventory.Value = 10;

                _inventoryRepository.UpdateFromServer(toUpdateInventory);

                var updatedInventory = _inventoryRepository.GetById(toUpdateInventory.Id);
                Trace.WriteLine(string.Format("Updated inventory."));

                AssertInventory(toUpdateInventory, updatedInventory);

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertInventory(Inventory infection, Inventory savedInfection)
        {
            Assert.AreEqual(infection.Warehouse, savedInfection.Warehouse);
            Assert.AreEqual(infection.Product, savedInfection.Product);
            Assert.AreEqual(infection.Balance, savedInfection.Balance);
            Assert.AreEqual(infection.Value, savedInfection.Value);
            Assert.AreEqual(infection._Status, savedInfection._Status);
        }
    }
}