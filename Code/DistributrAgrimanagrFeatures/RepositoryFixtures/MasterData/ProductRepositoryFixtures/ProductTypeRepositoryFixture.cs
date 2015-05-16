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
    public class ProductTypeRepositoryFixture
    {
        private IProductTypeRepository _productTypeRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _productTypeRepository = _testHelper.Ioc<IProductTypeRepository>();

        }

        [Test]
        public void ProductTypeRepositoryUnitTest()
        {
            using (var tra = new TransactionScope())
            {
                Trace.WriteLine("START PRODUCT TYPE REPOSITORY UNIT TEST.Type...");
                //Create
                ProductType newProductType = _testHelper.BuildProductType();

                Guid newProductTypeId = _productTypeRepository.Save(newProductType);

                //GetById
                ProductType createdProductType = _productTypeRepository.GetById(newProductTypeId);

                AssertProductType(createdProductType, newProductType);
                Trace.WriteLine(string.Format("ProductType GetById[{0}], Name[{1}]", createdProductType.Id,
                    createdProductType.Name));

                //Update
                var productTypeToUpdate = createdProductType;
                productTypeToUpdate._DateCreated = DateTime.Now;
                productTypeToUpdate._DateLastUpdated = DateTime.Now;
                productTypeToUpdate._Status = EntityStatus.Active;
                productTypeToUpdate.Code = "Type Code Update";
                productTypeToUpdate.Description = "Type description update2";
                productTypeToUpdate.Name = "Type Name update";

                Guid updatedProductTypeId = _productTypeRepository.Save(productTypeToUpdate);
                ProductType updatedProductType =
                    _productTypeRepository.GetById(updatedProductTypeId);
                AssertProductType(updatedProductType, productTypeToUpdate);

                Trace.WriteLine(string.Format("Updated ProductTypeId[{0}]", updatedProductTypeId));

                //Query
                var queryResult = _productTypeRepository.Query(new QueryStandard() {});
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Product Type Repository Query Count[{0}]", queryResult.Count));

                //Set As Inactive
                _productTypeRepository.SetInactive(updatedProductType);
                ProductType inactiveProductType =
                    _productTypeRepository.GetById(updatedProductTypeId);
                Assert.AreEqual(inactiveProductType._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Product Type Status[{0}]", inactiveProductType._Status));

                //Set As Active
                _productTypeRepository.SetActive(updatedProductType);
                ProductType activeProductType =
                    _productTypeRepository.GetById(updatedProductTypeId);
                Assert.AreEqual(activeProductType._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Product Type Status[{0}]", activeProductType._Status));

                //Set As Deleted
                _productTypeRepository.SetAsDeleted(updatedProductType);
                ProductType deletedProductType =
                    _productTypeRepository.GetById(updatedProductTypeId);
                Assert.AreEqual(deletedProductType._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted Product Type Status[{0}]", deletedProductType._Status));
                Trace.WriteLine(string.Format("Product Type Repository Unit Tests Successful"));
                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertProductType(ProductType productTypeX, ProductType productTypeY)
        {
            Assert.IsNotNull(productTypeX);
            Assert.AreEqual(productTypeX.Code, productTypeY.Code);
            Assert.AreEqual(productTypeX.Name, productTypeY.Name);
            Assert.AreEqual(productTypeX.Description, productTypeY.Description);
            Assert.AreEqual(productTypeX._Status, productTypeY._Status);
        }
    }
}
