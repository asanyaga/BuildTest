using System;
using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.ProductRepositoryFixtures
{
    [TestFixture]
    public class PricingRepositoryFixture
    {
        private static IProductPricingRepository _productPricingRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _productPricingRepository = _testHelper.Ioc<IProductPricingRepository>();
        }

        [Test]
        public void ProductPricingRepositoryUnitTest()

        {
            using (var tra = new TransactionScope())
            {

                Trace.WriteLine("START PRODUCT PRICING REPOSITORY UNIT TEST....");
                //Create
                ProductPricing newProductPricing = _testHelper.BuildProductPricing();
                Guid newProductPricingId = _productPricingRepository.Save(newProductPricing);

                //GetById
                ProductPricing createdProductPricing = _productPricingRepository.GetById(newProductPricingId);

                AssertProductPricing(createdProductPricing, newProductPricing);
                Trace.WriteLine(string.Format("ProductPricing GetById[{0}],ProductId[{1}]", createdProductPricing.Id,
                    createdProductPricing.ProductRef.ProductId));
                Trace.WriteLine(string.Format("Price[{0}]", createdProductPricing.CurrentSellingPrice.ToString()));
                //Update
            
                DateTime date = DateTime.Now;
                date = date.AddDays(-3);
                ProductPricing productPricingToUpdate = _testHelper.Ioc<IProductPricingFactory>().CreateProductPricing(createdProductPricing.ProductRef.ProductId, createdProductPricing.Tier.Id, 30, 30, date);

                Guid updatedProductPricingId = _testHelper.Ioc<IProductPricingRepository>().Save(productPricingToUpdate);
                ProductPricing updatedProductPricing =
                    _testHelper.Ioc<IProductPricingRepository>().GetById(updatedProductPricingId);
                AssertProductPricingUpdate(updatedProductPricing, productPricingToUpdate);

                Trace.WriteLine(string.Format("Updated ProductPricingId[{0}], Updated Price[{1}]", updatedProductPricingId, updatedProductPricing.CurrentSellingPrice));


                //Query
                var queryResult = _testHelper.Ioc<IProductPricingRepository>().Query(new QueryStandard());
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Product Pricing Repository Query Count[{0}]", queryResult.Count));

                //Set As Inactive
                _testHelper.Ioc<IProductPricingRepository>().SetInactive(updatedProductPricing);
                ProductPricing inactiveProductPricing =
                    _testHelper.Ioc<IProductPricingRepository>().GetById(updatedProductPricingId);
                Assert.AreEqual(inactiveProductPricing._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Product Pricing Status[{0}]", inactiveProductPricing._Status));

                //Set As Active
                _testHelper.Ioc<IProductPricingRepository>().SetActive(updatedProductPricing);
                ProductPricing activeProductPricing =
                    _testHelper.Ioc<IProductPricingRepository>().GetById(updatedProductPricingId);
                Assert.AreEqual(activeProductPricing._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Product Pricing Status[{0}]", activeProductPricing._Status));

                //Set As Deleted
                _testHelper.Ioc<IProductPricingRepository>().SetAsDeleted(updatedProductPricing);
                ProductPricing deletedProductPricing =
                    _testHelper.Ioc<IProductPricingRepository>().GetById(updatedProductPricingId);
                Assert.AreEqual(deletedProductPricing._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted Product Pricing Status[{0}]", deletedProductPricing._Status));
                Trace.WriteLine(string.Format("Product Pricing Repository Unit Tests Successful"));


                _testHelper.Ioc<ICacheProvider>().Reset();
            }

        }

        private void AssertProductPricingUpdate(ProductPricing updatedProductPricing, ProductPricing productPricingToUpdate)
        {
            Assert.IsNotNull(updatedProductPricing);
            Assert.AreEqual(updatedProductPricing.ProductRef.ProductId.ToString(), productPricingToUpdate.ProductRef.ProductId.ToString());
            Assert.AreEqual(updatedProductPricing.Tier.Id.ToString(), productPricingToUpdate.Tier.Id.ToString());

            Assert.AreEqual(updatedProductPricing._Status, EntityStatus.Active);
        }

        private void AssertProductPricing(ProductPricing productPricingX, ProductPricing productPricingY)
        {
            Assert.IsNotNull(productPricingX);
            Assert.AreEqual(productPricingX.ProductRef.ProductId.ToString(), productPricingY.ProductRef.ProductId.ToString());
            Assert.AreEqual(productPricingX.Tier.Id.ToString(), productPricingY.Tier.Id.ToString());
         
        }
    }
    }

