using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Caching;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.ProducerRepositoryFixture
{
    [TestFixture]
    public class ProducerRepositoryFixture
    {
        private static ICostCentreRepository _producerRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _producerRepository = _testHelper.Ioc<ICostCentreRepository>();
        }

        [Test]
        public void ProducerRepositoryOwnerUnitTest()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START PRODUCER REPOSITORY UNIT TEST....");

                //Save producer
                var producer = _testHelper.BuildProducerCostCentre();
                Trace.WriteLine(string.Format("Created producer [{0}]", producer.Name));
                var toSaveProducer = _producerRepository.Save(producer);
                Trace.WriteLine(string.Format("Saved producer Id [{0}]", toSaveProducer));
                var savedProducer = _producerRepository.GetById(toSaveProducer) as Producer;

                AssertProducer(producer, savedProducer);

                //Update producer
                var toUpdateProducer = savedProducer;
                toUpdateProducer.Name = "ProducerCC 2";

                _producerRepository.Save(toUpdateProducer);

                var updatedProducer = _producerRepository.GetById(toUpdateProducer.Id) as Producer;
                Trace.WriteLine(string.Format("Updated producer to Name  [{0}]", updatedProducer.Name));

                AssertProducer(toUpdateProducer, updatedProducer);

                //Deactivate producer
                var toDeactivate = updatedProducer;
                toDeactivate._Status = EntityStatus.Inactive;

                _producerRepository.Save(toDeactivate);

                var deactivated = _producerRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated producer to status  [{0}]", deactivated._Status));

                //Activate producer
                var toActivate = updatedProducer;
                toActivate._Status = EntityStatus.Active;

                _producerRepository.Save(toActivate);

                var activated = _producerRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated producer to status  [{0}]", activated._Status));

                //Delete producer
                var toDelete = updatedProducer;
                toDelete._Status = EntityStatus.Deleted;

                _producerRepository.Save(toDelete);

                var deleted = _producerRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted producer to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertProducer(Producer producer, Producer savedProducer)
        {
            Assert.AreEqual(producer.Name, savedProducer.Name);
            Assert.AreEqual(EntityStatus.Active, savedProducer._Status);
        }
    }
}