using System;
using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.Caching;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.ProductRepositoryFixtures
{
    [TestFixture]
  public class ProductDiscountGroupRepositoryFixture
    {
        private IProductDiscountGroupRepository _productDiscountGroupRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _productDiscountGroupRepository = _testHelper.Ioc<IProductDiscountGroupRepository>();
        }

        [Test]
        public void ProductDiscountGroupRepositoryUnitTest()
        {

            using (var tra = new TransactionScope())
            {

                Trace.WriteLine("START PRODUCT DISCOUNT GROUP REPOSITORY UNIT TEST....");
                //Create
                ProductGroupDiscount newproductDiscountGroup = _testHelper.BuildProductDiscountGroup();
                Guid newproductDiscountGroupId = _productDiscountGroupRepository.Save(newproductDiscountGroup);

                //GetById
                ProductGroupDiscount createdproductDiscountGroup = _productDiscountGroupRepository.GetById(newproductDiscountGroupId);
                AssertproductDiscountGroup(createdproductDiscountGroup, newproductDiscountGroup);
                Trace.WriteLine(string.Format("productDiscountGroup GetById[{0}], DiscountRate[{1}]", createdproductDiscountGroup.Id,
                    createdproductDiscountGroup.DiscountRate));

                //Update
                var productDiscountGroupToUpdate = createdproductDiscountGroup;
                productDiscountGroupToUpdate.DiscountRate = 25;
                productDiscountGroupToUpdate.GroupDiscount.Name = "tere";
                Guid updatedproductDiscountGroupId = _productDiscountGroupRepository.Save(productDiscountGroupToUpdate);
                ProductGroupDiscount updatedproductDiscountGroup =
                    _productDiscountGroupRepository.GetById(updatedproductDiscountGroupId);
                AssertproductDiscountGroup(updatedproductDiscountGroup, productDiscountGroupToUpdate);
                Trace.WriteLine(string.Format("Updated productDiscountGroupId[{0}], Discount [{1}]", updatedproductDiscountGroupId, updatedproductDiscountGroup.DiscountRate));
      
                //Set As Inactive
                _productDiscountGroupRepository.SetInactive(updatedproductDiscountGroup);
                ProductGroupDiscount inactiveproductDiscountGroup =
                    _productDiscountGroupRepository.GetById(updatedproductDiscountGroupId);
                Assert.AreEqual(inactiveproductDiscountGroup._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("ProductDiscountGroup Status[{0}]", inactiveproductDiscountGroup._Status));

                //Set As Active
                _productDiscountGroupRepository.SetActive(updatedproductDiscountGroup);
                ProductGroupDiscount activeproductDiscountGroup =
                    _productDiscountGroupRepository.GetById(updatedproductDiscountGroupId);
                Assert.AreEqual(activeproductDiscountGroup._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("ProductDiscountGroup Status[{0}]", activeproductDiscountGroup._Status));

                //Set As Deleted
                _productDiscountGroupRepository.SetAsDeleted(updatedproductDiscountGroup);
                ProductGroupDiscount deletedproductDiscountGroup =
                    _productDiscountGroupRepository.GetById(updatedproductDiscountGroupId);
                Assert.AreEqual(deletedproductDiscountGroup._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted productDiscountGroup Status[{0}]", deletedproductDiscountGroup._Status));
                Trace.WriteLine(string.Format("ProductDiscountGroup Repository Unit Tests Successful"));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }

        }

        private void AssertproductDiscountGroup(ProductGroupDiscount productDiscountGroupX ,ProductGroupDiscount productDiscountGroupY)
        {
            Assert.IsNotNull(productDiscountGroupX);
            Assert.AreEqual(productDiscountGroupX.EndDate, productDiscountGroupY.EndDate);
            Assert.AreEqual(productDiscountGroupX.DiscountRate, productDiscountGroupY.DiscountRate);
            Assert.AreEqual(productDiscountGroupX.EffectiveDate, productDiscountGroupY.EffectiveDate);
            Assert.AreEqual(productDiscountGroupX._Status, EntityStatus.Active);
        }
    }
}
