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
    public class ProductFlavourRepositoryFixture
    {
        private static IProductFlavourRepository _productFlavourRepository;
       
        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
             _productFlavourRepository = _testHelper.Ioc<IProductFlavourRepository>();
        }

        [Test]
        public void ProductFlavourRepositoryUnitTest()
        {
            using (var tra = new TransactionScope())
            {
                Trace.WriteLine("START PRODUCT FLAVOUR REPOSITORY UNIT TEST....");
                //Create
                ProductFlavour newProductFlavour = _testHelper.BuildProductFlavour();

                Guid newProductFlavourId = _productFlavourRepository.Save(newProductFlavour);

                //GetById
                ProductFlavour createdProductFlavour = _productFlavourRepository.GetById(newProductFlavourId);

                AssertProductFlavour(createdProductFlavour, newProductFlavour);
                Trace.WriteLine(string.Format("ProductFlavour GetById[{0}], Name[{1}]", createdProductFlavour.Id,
                    createdProductFlavour.Name));

                //Update
                var productFlavourToUpdate = createdProductFlavour;
                productFlavourToUpdate._DateCreated = DateTime.Now;
                productFlavourToUpdate._DateLastUpdated = DateTime.Now;
                productFlavourToUpdate._Status = EntityStatus.Active;
                productFlavourToUpdate.Code = "Flavour Code Update";
                productFlavourToUpdate.Description = "Flavour description update2";
                productFlavourToUpdate.Name = "Flavour Name update";

                Guid updatedProductFlavourId = _productFlavourRepository.Save(productFlavourToUpdate);
                ProductFlavour updatedProductFlavour =
                    _productFlavourRepository.GetById(updatedProductFlavourId);
                AssertProductFlavour(updatedProductFlavour, productFlavourToUpdate);

                Trace.WriteLine(string.Format("Updated ProductFlavourId[{0}]", updatedProductFlavourId));


                //Query
                var queryResult = _productFlavourRepository.Query(new QueryStandard() {});
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Product Flavour Repository Query Count[{0}]", queryResult.Count));

                //Set As Inactive
                _productFlavourRepository.SetInactive(updatedProductFlavour);
                ProductFlavour inactiveProductFlavour =
                    _productFlavourRepository.GetById(updatedProductFlavourId);
                Assert.AreEqual(inactiveProductFlavour._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Product Flavour Status[{0}]", inactiveProductFlavour._Status));

                //Set As Active
                _productFlavourRepository.SetActive(updatedProductFlavour);
                ProductFlavour activeProductFlavour =
                    _productFlavourRepository.GetById(updatedProductFlavourId);
                Assert.AreEqual(activeProductFlavour._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Product Flavour Status[{0}]", activeProductFlavour._Status));

                //Set As Deleted
                _productFlavourRepository.SetAsDeleted(updatedProductFlavour);
                ProductFlavour deletedProductFlavour =
                    _productFlavourRepository.GetById(updatedProductFlavourId);
                Assert.AreEqual(deletedProductFlavour._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted Product Flavour Status[{0}]", deletedProductFlavour._Status));
                Trace.WriteLine(string.Format("Product Flavour Repository Unit Tests Successful"));


                _testHelper.Ioc<ICacheProvider>().Reset();


            }

          
        }
        private static void AssertProductFlavour(ProductFlavour createdProductFlavour, ProductFlavour newProductFlavour)
        {
            Assert.IsNotNull(createdProductFlavour);
            Assert.AreEqual(createdProductFlavour.Code, newProductFlavour.Code);
            Assert.AreEqual(createdProductFlavour.Name, newProductFlavour.Name);
            Assert.AreEqual(createdProductFlavour.Description, newProductFlavour.Description);
            Assert.AreEqual(createdProductFlavour._Status, newProductFlavour._Status);
        }
    }
}
