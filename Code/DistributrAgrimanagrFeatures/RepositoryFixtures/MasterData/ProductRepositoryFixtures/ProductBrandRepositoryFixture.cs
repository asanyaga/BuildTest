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
    public class ProductBrandRepositoryFixture
    {
        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
        }


        [Test]
        public void ProductBrandRepositoryUnitTest()
        {
            using (var tra = new TransactionScope())
            {

                Trace.WriteLine("START PRODUCT BRAND REPOSITORY UNIT TEST....");
                //Create
                ProductBrand newProductBrand = _testHelper.BuilProductBrand();
                Guid newProductBrandId = _testHelper.Ioc<IProductBrandRepository>().Save(newProductBrand);
                
                //GetById
                ProductBrand createdProductBrand = _testHelper.Ioc<IProductBrandRepository>().GetById(newProductBrandId);

                AssertProductBrand(createdProductBrand, newProductBrand);
                Trace.WriteLine(string.Format("ProductBrand GetById[{0}], Name[{1}]", createdProductBrand.Id,
                    createdProductBrand.Name));

                //Update
                var productBrandToUpdate = createdProductBrand;
                productBrandToUpdate._DateCreated = DateTime.Now;
                productBrandToUpdate._DateLastUpdated = DateTime.Now;
                productBrandToUpdate._Status = EntityStatus.Active;
                productBrandToUpdate.Code = "Brand Code Update";
                productBrandToUpdate.Description = "Brand description update2";
                productBrandToUpdate.Name = "Brand Name update";

                Guid updatedProductBrandId = _testHelper.Ioc<IProductBrandRepository>().Save(productBrandToUpdate);
                ProductBrand updatedProductBrand =
                    _testHelper.Ioc<IProductBrandRepository>().GetById(updatedProductBrandId);
                AssertProductBrand(updatedProductBrand, productBrandToUpdate);
               
                Trace.WriteLine(string.Format("Updated ProductBrandId[{0}]", updatedProductBrandId));


                //Query
                var queryResult = _testHelper.Ioc<IProductBrandRepository>().Query(new QueryStandard() {Name = updatedProductBrand.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Product Brand Repository Query Count[{0}]", queryResult.Count));

                //Set As Inactive
                _testHelper.Ioc<IProductBrandRepository>().SetInactive(updatedProductBrand);
                ProductBrand inactiveProductBrand =
                    _testHelper.Ioc<IProductBrandRepository>().GetById(updatedProductBrandId);
                Assert.AreEqual(inactiveProductBrand._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Product Brand Status[{0}]", inactiveProductBrand._Status));

                //Set As Active
                _testHelper.Ioc<IProductBrandRepository>().SetActive(updatedProductBrand);
                ProductBrand activeProductBrand =
                    _testHelper.Ioc<IProductBrandRepository>().GetById(updatedProductBrandId);
                Assert.AreEqual(activeProductBrand._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Product Brand Status[{0}]", activeProductBrand._Status));

                //Set As Deleted
                _testHelper.Ioc<IProductBrandRepository>().SetAsDeleted(updatedProductBrand);
                ProductBrand deletedProductBrand =
                    _testHelper.Ioc<IProductBrandRepository>().GetById(updatedProductBrandId);
                Assert.AreEqual(deletedProductBrand._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted Product Brand Status[{0}]", deletedProductBrand._Status));
                Trace.WriteLine(string.Format("Product Brand Repository Unit Tests Successful"));


                _testHelper.Ioc<ICacheProvider>().Reset();
            }


        }

        private static void AssertProductBrand(ProductBrand createdProductBrand, ProductBrand newProductBrand)
        {
            Assert.IsNotNull(createdProductBrand);
            Assert.AreEqual(createdProductBrand.Code, newProductBrand.Code);
            Assert.AreEqual(createdProductBrand.Name, newProductBrand.Name);
            Assert.AreEqual(createdProductBrand.Description, newProductBrand.Description);
            Assert.AreEqual(createdProductBrand._Status, newProductBrand._Status);
        }
    }
}
