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

    public class ProductPackagingTypeRepositoryFixture
   {
       private static IProductPackagingTypeRepository _productPackagingTypeRepository;


       private static TestHelper _testHelper;

       [SetUp]
       public void Setup()
       {
           _testHelper = ObjectFactory.GetInstance<TestHelper>();
           _productPackagingTypeRepository = _testHelper.Ioc<IProductPackagingTypeRepository>();
       }


        [Test]
       public void ProductPackagingTypeRepositoryUnitTest()
        {
            using (var tra = new TransactionScope())
            {
                Trace.WriteLine("START PRODUCT PACKAGING TYPE REPOSITORY UNIT TEST....");
                //Create
                ProductPackagingType newProductPackagingType = _testHelper.BuildProductPackagingType();

                Guid newProductPackagingTypeId = _productPackagingTypeRepository.Save(newProductPackagingType);

                //GetById
                ProductPackagingType createdProductPackagingType = _productPackagingTypeRepository.GetById(newProductPackagingTypeId);

                AssertProductPackagingType(createdProductPackagingType, newProductPackagingType);
                Trace.WriteLine(string.Format("ProductPackagingType GetById[{0}], Name[{1}]", createdProductPackagingType.Id,
                    createdProductPackagingType.Name));

                //Update
                var productPackagingTypeToUpdate = createdProductPackagingType;
                productPackagingTypeToUpdate._DateCreated = DateTime.Now;
                productPackagingTypeToUpdate._DateLastUpdated = DateTime.Now;
                productPackagingTypeToUpdate._Status = EntityStatus.Active;
                productPackagingTypeToUpdate.Code = "PackagingType Code Update";
                productPackagingTypeToUpdate.Description = "PackagingType description update2";
                productPackagingTypeToUpdate.Name = "PackagingType Name update";

                Guid updatedProductPackagingTypeId = _productPackagingTypeRepository.Save(productPackagingTypeToUpdate);
                ProductPackagingType updatedProductPackagingType =
                    _productPackagingTypeRepository.GetById(updatedProductPackagingTypeId);
                AssertProductPackagingType(updatedProductPackagingType, productPackagingTypeToUpdate);

                Trace.WriteLine(string.Format("Updated ProductPackagingTypeId[{0}]", updatedProductPackagingTypeId));


                //Query
                var queryResult = _productPackagingTypeRepository.Query(new QueryStandard() { });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Product PackagingType Repository Query Count[{0}]", queryResult.Count));

                //Set As Inactive
                _productPackagingTypeRepository.SetInactive(updatedProductPackagingType);
                ProductPackagingType inactiveProductPackagingType =
                    _productPackagingTypeRepository.GetById(updatedProductPackagingTypeId);
                Assert.AreEqual(inactiveProductPackagingType._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Product PackagingType Status[{0}]", inactiveProductPackagingType._Status));

                //Set As Active
                _productPackagingTypeRepository.SetActive(updatedProductPackagingType);
                ProductPackagingType activeProductPackagingType =
                    _productPackagingTypeRepository.GetById(updatedProductPackagingTypeId);
                Assert.AreEqual(activeProductPackagingType._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Product PackagingType Status[{0}]", activeProductPackagingType._Status));

                //Set As Deleted
                _productPackagingTypeRepository.SetAsDeleted(updatedProductPackagingType);
                ProductPackagingType deletedProductPackagingType =
                    _productPackagingTypeRepository.GetById(updatedProductPackagingTypeId);
                Assert.AreEqual(deletedProductPackagingType._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted Product PackagingType Status[{0}]", deletedProductPackagingType._Status));
                Trace.WriteLine(string.Format("Product PackagingType Repository Unit Tests Successful"));


                _testHelper.Ioc<ICacheProvider>().Reset();



            }
        }

        private void AssertProductPackagingType(ProductPackagingType createdProductPackagingType, ProductPackagingType newProductPackagingTypeToUpdate)
        {
            Assert.IsNotNull(createdProductPackagingType);
            Assert.AreEqual(createdProductPackagingType.Code, newProductPackagingTypeToUpdate.Code);
            Assert.AreEqual(createdProductPackagingType.Name, newProductPackagingTypeToUpdate.Name);
            Assert.AreEqual(createdProductPackagingType.Description, newProductPackagingTypeToUpdate.Description);
            Assert.AreEqual(createdProductPackagingType._Status, newProductPackagingTypeToUpdate._Status);
        }
   }
}
