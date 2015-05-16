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
    public class ProductPricingTierRepositoryFixture
    {
        private IProductPricingTierRepository _productPricingTierRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _productPricingTierRepository = _testHelper.Ioc<IProductPricingTierRepository>();
        }

        [Test]
        public void ProductPricingTierRepositoryUnitTest()
        {
            using (var tra = new TransactionScope())
            {
                Trace.WriteLine("START PRODUCT PRICING TIRE REPOSITORY UNIT TEST....");
                //Create
                ProductPricingTier newProductPricingTier = _testHelper.BuildProductPricingTier();

                Guid newProductPricingTierId = _productPricingTierRepository.Save(newProductPricingTier);

                //GetById
                ProductPricingTier createdProductPricingTier = _productPricingTierRepository.GetById(newProductPricingTierId);

                AssertProductPricingTier(createdProductPricingTier, newProductPricingTier);
                Trace.WriteLine(string.Format("ProductPricingTier GetById[{0}], Name[{1}]", createdProductPricingTier.Id,
                    createdProductPricingTier.Name));

                //Update
                var productPricingTierToUpdate = createdProductPricingTier;
                productPricingTierToUpdate._DateCreated = DateTime.Now;
                productPricingTierToUpdate._DateLastUpdated = DateTime.Now;
                productPricingTierToUpdate._Status = EntityStatus.Active;
                productPricingTierToUpdate.Code = "PricingTier Code Update";
                productPricingTierToUpdate.Description = "PricingTier description update2";
                productPricingTierToUpdate.Name = "PricingTier Name update";

                Guid updatedProductPricingTierId = _productPricingTierRepository.Save(productPricingTierToUpdate);
                ProductPricingTier updatedProductPricingTier =
                    _productPricingTierRepository.GetById(updatedProductPricingTierId);
                AssertProductPricingTier(updatedProductPricingTier, productPricingTierToUpdate);

                Trace.WriteLine(string.Format("Updated ProductPricingTierId[{0}]", updatedProductPricingTierId));


                //Query
                var queryResult = _productPricingTierRepository.Query(new QueryStandard() { });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Product PricingTier Repository Query Count[{0}]", queryResult.Count));

                //Set As Inactive
                _productPricingTierRepository.SetInactive(updatedProductPricingTier);
                ProductPricingTier inactiveProductPricingTier =
                    _productPricingTierRepository.GetById(updatedProductPricingTierId);
                Assert.AreEqual(inactiveProductPricingTier._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Product PricingTier Status[{0}]", inactiveProductPricingTier._Status));

                //Set As Active
                _productPricingTierRepository.SetActive(updatedProductPricingTier);
                ProductPricingTier activeProductPricingTier =
                    _productPricingTierRepository.GetById(updatedProductPricingTierId);
                Assert.AreEqual(activeProductPricingTier._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Product PricingTier Status[{0}]", activeProductPricingTier._Status));

                //Set As Deleted
                _productPricingTierRepository.SetAsDeleted(updatedProductPricingTier);
                ProductPricingTier deletedProductPricingTier =
                    _productPricingTierRepository.GetById(updatedProductPricingTierId);
                Assert.AreEqual(deletedProductPricingTier._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted Product PricingTier Status[{0}]", deletedProductPricingTier._Status));
                Trace.WriteLine(string.Format("Product PricingTier Repository Unit Tests Successful"));


                _testHelper.Ioc<ICacheProvider>().Reset();





            }

        }

        private void AssertProductPricingTier(ProductPricingTier productPricingTierX, ProductPricingTier ProductPricingTierY)
        {
            Assert.IsNotNull(productPricingTierX);
            Assert.AreEqual(productPricingTierX.Code, ProductPricingTierY.Code);
            Assert.AreEqual(productPricingTierX.Name, ProductPricingTierY.Name);
            Assert.AreEqual(productPricingTierX.Description, ProductPricingTierY.Description);
            Assert.AreEqual(productPricingTierX._Status, ProductPricingTierY._Status);
        }
    }
}
