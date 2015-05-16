using System;
using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Caching;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.CostCentreRepositoryFixtures
{
    [TestFixture]
    public class DistributrSalesmanRepositoryFixture
    {
        private static ICostCentreRepository _costCentreRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _costCentreRepository = _testHelper.Ioc<ICostCentreRepository>();
        }

        [Test]
        public void DistributrSalesmanRepositoryUnitTest()
        {
            using (var tra = new TransactionScope())
            {
                Trace.WriteLine("START DISTRIBUTOR SALESMAN REPOSITORY UNIT TEST....");
                //Create
                CostCentre newDistributrSalesmanCostCentre = _testHelper.BuildDistributrSalesmanCostCentre();
                Guid newDistributrSalesmanCcId = _costCentreRepository.Save(newDistributrSalesmanCostCentre);
                var createdDistributrSalesmanCostCentre = _costCentreRepository.GetById(newDistributrSalesmanCcId) as DistributorSalesman;
                AssertCostCernter(createdDistributrSalesmanCostCentre, newDistributrSalesmanCostCentre);
                if (createdDistributrSalesmanCostCentre != null)
                    Trace.WriteLine(string.Format("Created DistributrSalesmanCCId [{0}],-Name[{1}]", newDistributrSalesmanCcId, createdDistributrSalesmanCostCentre.Name));

                //Update
                CostCentre distributrSalesmanToUpdate = _testHelper.Ioc<ICostCentreFactory>().CreateCostCentre(createdDistributrSalesmanCostCentre.Id,
                                                                     CostCentreType.DistributorSalesman,
                                                                     _testHelper.Ioc<ICostCentreRepository>().GetById(createdDistributrSalesmanCostCentre.ParentCostCentre.Id));
                distributrSalesmanToUpdate.Name = "UpdateDistSalemanCC";
                distributrSalesmanToUpdate.CostCentreCode = "UdatedCode_2001";
                Guid distributrSalesmanToUpdateId = _costCentreRepository.Save(distributrSalesmanToUpdate);
                var updatedDistributrSalesman = _costCentreRepository.GetById(distributrSalesmanToUpdateId);
                AssertCostCernter(distributrSalesmanToUpdate, updatedDistributrSalesman);
                Trace.WriteLine(string.Format("Updated DistributrSalesmanCCId [{0}],-Name[{1}]", distributrSalesmanToUpdateId, updatedDistributrSalesman.Name));


                //DistributrSalesmanCC Status Change
                _costCentreRepository.SetInactive(updatedDistributrSalesman);
                CostCentre distributrSalesmanStatus =
                    _costCentreRepository.GetById(distributrSalesmanToUpdateId);
                Assert.AreEqual(distributrSalesmanStatus._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("DistributrSalesmanCC  Status[{0}]", distributrSalesmanStatus._Status));


                _costCentreRepository.SetActive(updatedDistributrSalesman);
                 distributrSalesmanStatus =
                    _costCentreRepository.GetById(distributrSalesmanToUpdateId);
                Assert.AreEqual(distributrSalesmanStatus._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("DistributrSalesmanCC  Status[{0}]", distributrSalesmanStatus._Status));

                _costCentreRepository.SetAsDeleted(updatedDistributrSalesman);
                distributrSalesmanStatus =
                   _costCentreRepository.GetById(distributrSalesmanToUpdateId);
                Assert.AreEqual(distributrSalesmanStatus._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("DistributrSalesmanCC  Status[{0}]", distributrSalesmanStatus._Status));

            }
            _testHelper.Ioc<ICacheProvider>().Reset();
        }

        private void AssertCostCernter(CostCentre producerCostCentreX, CostCentre producerCostCentreY)
        {
            Assert.NotNull(producerCostCentreX.Id);
            Assert.AreEqual(producerCostCentreX.CostCentreType, producerCostCentreY.CostCentreType);
            Assert.AreEqual(producerCostCentreX.Name, producerCostCentreY.Name);
        }
    }
}
