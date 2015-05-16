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
    public class ProductDiscountRepositoryFixture
    {
        private IProductDiscountRepository _productDiscountRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _productDiscountRepository = _testHelper.Ioc<IProductDiscountRepository>();

        }

        [Test]

        public void ProductDiscountRepositoryUnitTest()
        {
            using (var tra = new TransactionScope())
            {
                Trace.WriteLine("START PRODUCT DISCOUNT REPOSITORY UNIT TEST....");
                //Create
                ProductDiscount newProductDiscount = _testHelper.BuildProductDiscount();

                Guid newProductDiscountId = _productDiscountRepository.Save(newProductDiscount);

                //GetById
                ProductDiscount createdProductDiscount = _productDiscountRepository.GetById(newProductDiscountId);

                AssertProductDiscount(createdProductDiscount, newProductDiscount);
                Trace.WriteLine(string.Format("ProductDiscount GetById[{0}], ProductRef[{1}]", createdProductDiscount.Id,
                    createdProductDiscount.ProductRef));

                //Update

                var productPricingTier = new ProductPricingTier()
                {
                    Id = Guid.NewGuid(),
                    _DateCreated = DateTime.Now,
                    _DateLastUpdated = DateTime.Now,
                    _Status = EntityStatus.Active,
                    Code = "ppTierCode0013",
                    Description = "Tier0012",
                    Name = "Tier00023"
                };
                Guid productPricingTierId = _testHelper.Ioc<IProductPricingTierRepository>().Save(productPricingTier);
                ProductPricingTier productPricingTierUpedate = _testHelper.Ioc<IProductPricingTierRepository>().GetById(productPricingTierId);
                var productDiscountToUpdate = createdProductDiscount;
                productDiscountToUpdate._DateCreated = DateTime.Now;
                productDiscountToUpdate._DateLastUpdated = DateTime.Now;
                productDiscountToUpdate._Status = EntityStatus.Active;
                productDiscountToUpdate.Tier = productPricingTierUpedate;

                Guid updatedProductDiscountId = _productDiscountRepository.Save(productDiscountToUpdate);
                ProductDiscount updatedProductDiscount =
                    _productDiscountRepository.GetById(updatedProductDiscountId);
                AssertProductDiscountUpdate(updatedProductDiscount, productDiscountToUpdate);

                Trace.WriteLine(string.Format("Updated ProductDiscountId[{0}]", updatedProductDiscountId));

                //Query
                var queryResult = _productDiscountRepository.Query(new QueryStandard() { });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Product Discount Repository Query Count[{0}]", queryResult.Count));

                //Set As Inactive
                _productDiscountRepository.SetInactive(updatedProductDiscount);
                ProductDiscount inactiveProductDiscount =
                    _productDiscountRepository.GetById(updatedProductDiscountId);
                Assert.AreEqual(inactiveProductDiscount._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Product Discount Status[{0}]", inactiveProductDiscount._Status));

                //Set As Active
                _productDiscountRepository.SetActive(updatedProductDiscount);
                ProductDiscount activeProductDiscount =
                    _productDiscountRepository.GetById(updatedProductDiscountId);
                Assert.AreEqual(activeProductDiscount._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Product Discount Status[{0}]", activeProductDiscount._Status));

                //Set As Deleted
                _productDiscountRepository.SetAsDeleted(updatedProductDiscount);
                ProductDiscount deletedProductDiscount =
                    _productDiscountRepository.GetById(updatedProductDiscountId);
                Assert.AreEqual(deletedProductDiscount._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted Product Discount Status[{0}]", deletedProductDiscount._Status));
                Trace.WriteLine(string.Format("Product Discount Repository Unit Tests Successful"));
                _testHelper.Ioc<ICacheProvider>().Reset();

            }

        }

      
        private void AssertProductDiscount(ProductDiscount productDiscountX, ProductDiscount productDiscountY)
        {
            Assert.IsNotNull(productDiscountX);
            Assert.AreEqual(productDiscountX.ProductRef.ProductId.ToString(), productDiscountY.ProductRef.ProductId.ToString());
            Assert.AreEqual(productDiscountX.Tier.Id.ToString(), productDiscountY.Tier.Id.ToString());
          
             Assert.AreEqual(productDiscountX._Status, EntityStatus.Active);
        }

        private void AssertProductDiscountUpdate(ProductDiscount updatedProductDiscount, ProductDiscount productDiscountToUpdate)
        {
            Assert.IsNotNull(updatedProductDiscount);
            Assert.AreEqual(updatedProductDiscount.ProductRef.ProductId.ToString(), productDiscountToUpdate.ProductRef.ProductId.ToString());
            Assert.AreEqual(updatedProductDiscount.Tier.Name, productDiscountToUpdate.Tier.Name);
        }
    }
}
