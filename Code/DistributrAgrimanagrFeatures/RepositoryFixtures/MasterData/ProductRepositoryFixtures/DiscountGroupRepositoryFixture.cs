using System;
using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.ProductRepositoryFixtures
{
    [TestFixture]
    public class DiscountGroupRepositoryFixture
    {

        private IDiscountGroupRepository _discountGroupRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _discountGroupRepository = _testHelper.Ioc<IDiscountGroupRepository>();
        }

        [Test]
        public void DiscountGroupRepositoryUnitTest()
        {

            using (var tra = new TransactionScope())
            {

                Trace.WriteLine("START DISCOUNT GROUP REPOSITORY UNIT TEST....");
                //Create
                DiscountGroup newDiscountGroup = _testHelper.BuildDiscountGroup();
                Guid newDiscountGroupId = _discountGroupRepository.Save(newDiscountGroup);

                //GetById
                DiscountGroup createdDiscountGroup = _discountGroupRepository.GetById(newDiscountGroupId);
                AssertDiscountGroup(createdDiscountGroup, newDiscountGroup);
                Trace.WriteLine(string.Format("DiscountGroup GetById[{0}], Name[{1}]", createdDiscountGroup.Id,
                    createdDiscountGroup.Name));

                //Update
                var DiscountGroupToUpdate = createdDiscountGroup;
                DiscountGroupToUpdate._Status = EntityStatus.Active;
                DiscountGroupToUpdate.Code = "DG Code Update";
                DiscountGroupToUpdate.Name = "DG Name update";

                Guid updatedDiscountGroupId = _discountGroupRepository.Save(DiscountGroupToUpdate);
                DiscountGroup updatedDiscountGroup =
                    _discountGroupRepository.GetById(updatedDiscountGroupId);
                AssertDiscountGroup(updatedDiscountGroup, DiscountGroupToUpdate);
                Trace.WriteLine(string.Format("Updated DiscountGroupId[{0}], Discount Name[{1}]", updatedDiscountGroupId, updatedDiscountGroup.Name));
                
                //Query
                var queryResult = _discountGroupRepository.QueryResult(new QueryStandard() { });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Product DiscountGroup Repository Query Count[{0}]", queryResult.Count));

                //Set As Inactive
                _discountGroupRepository.SetInactive(updatedDiscountGroup);
                DiscountGroup inactiveDiscountGroup =
                    _discountGroupRepository.GetById(updatedDiscountGroupId);
                Assert.AreEqual(inactiveDiscountGroup._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Product DiscountGroup Status[{0}]", inactiveDiscountGroup._Status));

                //Set As Active
                _discountGroupRepository.SetActive(updatedDiscountGroup);
                DiscountGroup activeDiscountGroup =
                    _discountGroupRepository.GetById(updatedDiscountGroupId);
                Assert.AreEqual(activeDiscountGroup._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Product DiscountGroup Status[{0}]", activeDiscountGroup._Status));

                //Set As Deleted
                _discountGroupRepository.SetAsDeleted(updatedDiscountGroup);
                DiscountGroup deletedDiscountGroup =
                    _discountGroupRepository.GetById(updatedDiscountGroupId);
                Assert.AreEqual(deletedDiscountGroup._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted Product DiscountGroup Status[{0}]", deletedDiscountGroup._Status));
                Trace.WriteLine(string.Format("Product DiscountGroup Repository Unit Tests Successful"));

                _testHelper.Ioc<ICacheProvider>().Reset();

            }

        }

        private void AssertDiscountGroup(DiscountGroup discountGroupX, DiscountGroup discountGroupY)
        {
            Assert.IsNotNull(discountGroupX);
            Assert.AreEqual(discountGroupX.Code, discountGroupY.Code);
            Assert.AreEqual(discountGroupX.Name, discountGroupY.Name);
            Assert.AreEqual(discountGroupX._Status, discountGroupY._Status);
        }
    }
}
