using System.Transactions;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.OrderRepositoryFixtures
{
    [TestFixture]
   public class OrderRepositoryFixture
    {
        private static IOrderRepository _orderRepository;
        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _orderRepository = _testHelper.Ioc<IOrderRepository>();
        }

        [Test]
        [Ignore]
        public void OrderRepositoryUnitTest()
        {
            using (var tra = new TransactionScope())
            {
                //var t = _testHelper.BuildOrder();





            }
        }

    }
}
