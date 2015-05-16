using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Repository.Master.DistributorTargetRepositories;
using Distributr.Core.Utility.Caching;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.DistributorTarget
{
    [TestFixture]
    public class TargetItemFixture
    {
        private static ITargetItemRepository _targetItemRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();

            _targetItemRepository = _testHelper.Ioc<ITargetItemRepository>();
        }

        [Test]
        public void TargetItemUnitTests()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START TARGET ITEM REPOSITORY UNIT TEST....");

                //Save target item
                var targetItem = _testHelper.BuildTargetItem();
                Trace.WriteLine(string.Format("Created target item [{0}]", targetItem.Product.ProductId));
                var toSaveTargetItem = _targetItemRepository.Save(targetItem);
                Trace.WriteLine(string.Format("Saved target item Id [{0}]", toSaveTargetItem));
                var savedTargetItem = _targetItemRepository.GetById(toSaveTargetItem);

                AssertTargetItem(targetItem, savedTargetItem);

                //Update target item 
                var toUpdateTargetItem = savedTargetItem;
                toUpdateTargetItem.Quantity = 30;

                _targetItemRepository.Save(toUpdateTargetItem);

                var updatedTargetItem = _targetItemRepository.GetById(toUpdateTargetItem.Id);
                Trace.WriteLine(string.Format("Updated target item to Name  [{0}]", updatedTargetItem.Product.ProductId));

                AssertTargetItem(toUpdateTargetItem, updatedTargetItem);

                //Deactivate target item 
                var toDeactivate = updatedTargetItem;
                toDeactivate._Status = EntityStatus.Inactive;

                _targetItemRepository.Save(toDeactivate);

                var deactivated = _targetItemRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated target item to status  [{0}]", deactivated._Status));

                //Activate target item 
                var toActivate = updatedTargetItem;
                toActivate._Status = EntityStatus.Active;

                _targetItemRepository.Save(toActivate);

                var activated = _targetItemRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated target item to status  [{0}]", activated._Status));

                //Delete target item 
                var toDelete = updatedTargetItem;
                toDelete._Status = EntityStatus.Deleted;

                _targetItemRepository.Save(toDelete);

                var deleted = _targetItemRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted target item to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertTargetItem(TargetItem targetItem, TargetItem savedTargetItem)
        {
            Assert.AreEqual(targetItem.Product.ProductId, savedTargetItem.Product.ProductId);
            Assert.AreEqual(targetItem.Quantity, savedTargetItem.Quantity);
            Assert.AreEqual(targetItem.Target.Id, savedTargetItem.Target.Id);
            Assert.AreEqual(targetItem._Status, EntityStatus.Active);
        }
    }
}