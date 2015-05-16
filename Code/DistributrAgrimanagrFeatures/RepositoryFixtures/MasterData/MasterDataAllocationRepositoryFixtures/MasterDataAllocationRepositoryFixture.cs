using System;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.MasterDataAllocationRepositories;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.MasterDataAllocationRepositoryFixtures
{
    [TestFixture]
    public class MasterDataAllocationRepositoryFixture
    {
        private static IMasterDataAllocationRepository _masterDataAllocationRepository;
        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _masterDataAllocationRepository = _testHelper.Ioc<IMasterDataAllocationRepository>();
        }
        [Test]
        public void MasterDataAllocationRepositoryUnitTest()
        {
            using (var tra = new TransactionScope())
            {
                Trace.WriteLine(">/ADD MASTERDATA ALLOCATION");
                MasterDataAllocation newMasterDataAllocation = _testHelper.BuildMasterDataAllocation();
                Guid newMasterDataAllocationCcId = _masterDataAllocationRepository.Save(newMasterDataAllocation);
                var createdMasterDataAllocation = _masterDataAllocationRepository.GetById(newMasterDataAllocationCcId);
                AssertCostCernter(createdMasterDataAllocation, newMasterDataAllocation);
                if (createdMasterDataAllocation != null)
                    Trace.WriteLine(string.Format("Created MasterDataAllocationCCId [{0}],-Name[{1}]",
                        newMasterDataAllocationCcId, createdMasterDataAllocation.EntityAId));

                Trace.WriteLine("Fetch By Allocation Type");
               var getByAlloctionType = _masterDataAllocationRepository.GetByAllocationType(MasterDataAllocationType.RouteCostCentreAllocation).FirstOrDefault();
                Trace.WriteLine(string.Format("AllocationType[{0}],RouteId[{1}],CostCentre[{2}]",getByAlloctionType.AllocationType, getByAlloctionType.EntityAId,
                    getByAlloctionType.EntityBId));

                Trace.WriteLine("Fetch By Id");
                var masterDataAllocationById = _masterDataAllocationRepository.GetById(createdMasterDataAllocation.Id);
                Trace.WriteLine(string.Format("AllocationType[{0}],RouteId[{1}],CostCentre[{2}]", masterDataAllocationById.AllocationType, masterDataAllocationById.EntityAId,
                    masterDataAllocationById.EntityBId));
 
            }

        }

        private void AssertCostCernter(MasterDataAllocation masterDataAllocationX, MasterDataAllocation masterDataAllocationY)
        {
            Assert.AreEqual(masterDataAllocationX.EntityAId, masterDataAllocationY.EntityAId);
            Assert.AreEqual(masterDataAllocationX.EntityBId, masterDataAllocationY.EntityBId);
            Assert.AreEqual(masterDataAllocationX.Id, masterDataAllocationY.Id);
            Assert.AreEqual(masterDataAllocationX.AllocationType, masterDataAllocationY.AllocationType);
        }
    }
}
