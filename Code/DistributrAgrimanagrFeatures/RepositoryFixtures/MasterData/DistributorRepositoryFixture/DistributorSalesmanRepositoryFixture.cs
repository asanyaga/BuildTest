using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Caching;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.DistributorRepositoryFixture
{
    [TestFixture]
    public class DistributorSalesmanRepositoryFixture
    {
        private static ICostCentreRepository _distributorSalesmanRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();

            _distributorSalesmanRepository = _testHelper.Ioc<ICostCentreRepository>();
        }

        [Test]
        public void DistributorSalesmanRepositoryUnitTests()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START DISTRIBUTOR SALESMAN REPOSITORY UNIT TEST....");

                //Save distributor salesman
                var distributorSalesman = _testHelper.BuildDistributorSalesmanWarehouse();
                Trace.WriteLine(string.Format("Created distributor salesman [{0}]", distributorSalesman.Name));
                var toSaveDistributorSalesman = _distributorSalesmanRepository.Save(distributorSalesman);
                Trace.WriteLine(string.Format("Saved distributor salesman Id [{0}]", toSaveDistributorSalesman));
                var savedDistributorSalesman = _distributorSalesmanRepository.GetById(toSaveDistributorSalesman);

                AssertDistributorSalesman(distributorSalesman, savedDistributorSalesman);

                //Update distributor salesman
                var toUpdateDistributorSalesman = savedDistributorSalesman;
                toUpdateDistributorSalesman.Name = "Distributor CC 2";

                _distributorSalesmanRepository.Save(toUpdateDistributorSalesman);

                var updatedDistributorSalesman = _distributorSalesmanRepository.GetById(toUpdateDistributorSalesman.Id);
                Trace.WriteLine(string.Format("Updated distributor salesman to Name  [{0}]", updatedDistributorSalesman.Name));

                AssertDistributorSalesman(toUpdateDistributorSalesman, updatedDistributorSalesman);

                //Deactivate distributor salesman
                var toDeactivate = updatedDistributorSalesman;
                toDeactivate._Status = EntityStatus.Inactive;

                _distributorSalesmanRepository.Save(toDeactivate);

                var deactivated = _distributorSalesmanRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated distributor salesman to status  [{0}]", deactivated._Status));

                //Activate distributor salesman
                var toActivate = updatedDistributorSalesman;
                toActivate._Status = EntityStatus.Active;

                _distributorSalesmanRepository.Save(toActivate);

                var activated = _distributorSalesmanRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated distributor salesman to status  [{0}]", activated._Status));

                //Delete distributor salesman
                var toDelete = updatedDistributorSalesman;
                toDelete._Status = EntityStatus.Deleted;

                _distributorSalesmanRepository.Save(toDelete);

                var deleted = _distributorSalesmanRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted distributor salesman to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertDistributorSalesman(CostCentre distributorSalesman, CostCentre savedDistributorSalesman)
        {
            Assert.AreEqual(distributorSalesman.Name,savedDistributorSalesman.Name);
            Assert.AreEqual(distributorSalesman.CostCentreCode,savedDistributorSalesman.CostCentreCode);
            Assert.AreEqual(distributorSalesman._Status,EntityStatus.Active);
        }
    }
}