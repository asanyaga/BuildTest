using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ReOrdeLevelEntities;
using Distributr.Core.Repository.Master.ReOrderLevelRepository;
using Distributr.Core.Utility.Caching;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.ReorderLevelRepositoryFixtures
{
    [TestFixture]
    public class ReorderLevelRepositoryFixture
    {
        private static IReOrderLevelRepository _reOrderLevelRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _reOrderLevelRepository = _testHelper.Ioc<IReOrderLevelRepository>();
        }

        [Test]
        [Ignore]
        public void ReorderLevelRepositoryOwnerUnitTest()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START REORDER LEVEL REPOSITORY UNIT TEST....");

                //Save reorder
                var reorder = _testHelper.BuildReorderLevel();
                Trace.WriteLine(string.Format("Created reorder [{0}]", reorder.ProductId.Description));
                var toSaveReorder = _reOrderLevelRepository.Save(reorder);
                Trace.WriteLine(string.Format("Saved reorder Id [{0}]", toSaveReorder));
                var savedReorder = _reOrderLevelRepository.GetById(toSaveReorder);

                AssertReorder(reorder, savedReorder);

                //Update reorder
                var toUpdateReorder = savedReorder;
                toUpdateReorder.ProductReOrderLevel = 15;

                _reOrderLevelRepository.Save(toUpdateReorder);

                var updatedReorder = _reOrderLevelRepository.GetById(toUpdateReorder.Id);
                Trace.WriteLine(string.Format("Updated reorder to Name  [{0}]", updatedReorder.ProductId.Description));

                AssertReorder(toUpdateReorder, updatedReorder);

                //Deactivate reorder
                var toDeactivate = updatedReorder;
                toDeactivate._Status = EntityStatus.Inactive;

                _reOrderLevelRepository.Save(toDeactivate);

                ;var deactivated = _reOrderLevelRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated reorder to status  [{0}]", deactivated._Status));

                //Activate reorder
                var toActivate = updatedReorder;
                toActivate._Status = EntityStatus.Active;

                _reOrderLevelRepository.Save(toActivate);

                var activated = _reOrderLevelRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated reorder to status  [{0}]", activated._Status));

                //Delete reorder
                var toDelete = updatedReorder;
                toDelete._Status = EntityStatus.Deleted;

                _reOrderLevelRepository.Save(toDelete);

                var deleted = _reOrderLevelRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted reorder to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertReorder(ReOrderLevel producer, ReOrderLevel savedProducer)
        {
            Assert.AreEqual(producer.ProductId.Description, savedProducer.ProductId.Description);
            Assert.AreEqual(producer.ProductReOrderLevel, savedProducer.ProductReOrderLevel);
            Assert.AreEqual(EntityStatus.Active, savedProducer._Status);
        }
    }
}