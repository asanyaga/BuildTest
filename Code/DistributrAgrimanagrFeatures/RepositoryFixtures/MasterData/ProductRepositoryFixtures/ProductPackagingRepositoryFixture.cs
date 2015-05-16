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
    public class ProductPackagingRepositoryFixture
    {
         private static IProductPackagingRepository _productPackagingRepository;


         private static TestHelper _testHelper;

         [SetUp]
         public void Setup()
         {
             _testHelper = ObjectFactory.GetInstance<TestHelper>();
             _productPackagingRepository = _testHelper.Ioc<IProductPackagingRepository>();
         }

         [Test]
         public void ProductPackagingRepositoryUnitTest()
         {
             using (var tra = new TransactionScope())
             {
                 Trace.WriteLine("START PRODUCT PACKAGING REPOSITORY UNIT TEST....");
                 //Create
                 ProductPackaging newProductPackaging = _testHelper.BuildProductPackaging();

                 Guid newProductPackagingId = _productPackagingRepository.Save(newProductPackaging);

                 //GetById
                 ProductPackaging createdProductPackaging = _productPackagingRepository.GetById(newProductPackagingId);

                 AssertProductPackaging(createdProductPackaging, newProductPackaging);
                 Trace.WriteLine(string.Format("ProductPackaging GetById[{0}], Name[{1}]", createdProductPackaging.Id,
                     createdProductPackaging.Name));

                 //Update
                 var productPackagingToUpdate = createdProductPackaging;
                 productPackagingToUpdate._DateCreated = DateTime.Now;
                 productPackagingToUpdate._DateLastUpdated = DateTime.Now;
                 productPackagingToUpdate._Status = EntityStatus.Active;
                 productPackagingToUpdate.Code = "Packaging Code Update";
                 productPackagingToUpdate.Description = "Packaging description update2";
                 productPackagingToUpdate.Name = "Packaging Name update";

                 Guid updatedProductPackagingId = _productPackagingRepository.Save(productPackagingToUpdate);
                 ProductPackaging updatedProductPackaging =
                     _productPackagingRepository.GetById(updatedProductPackagingId);
                 AssertProductPackaging(updatedProductPackaging, productPackagingToUpdate);

                 Trace.WriteLine(string.Format("Updated ProductPackagingId[{0}]", updatedProductPackagingId));


                 //Query
                 var queryResult = _productPackagingRepository.Query(new QueryStandard() { });
                 Assert.GreaterOrEqual(queryResult.Count, 1);
                 Trace.WriteLine(string.Format("Product Packaging Repository Query Count[{0}]", queryResult.Count));

                 //Set As Inactive
                 _productPackagingRepository.SetInactive(updatedProductPackaging);
                 ProductPackaging inactiveProductPackaging =
                     _productPackagingRepository.GetById(updatedProductPackagingId);
                 Assert.AreEqual(inactiveProductPackaging._Status, EntityStatus.Inactive);
                 Trace.WriteLine(string.Format("Product Packaging Status[{0}]", inactiveProductPackaging._Status));

                 //Set As Active
                 _productPackagingRepository.SetActive(updatedProductPackaging);
                 ProductPackaging activeProductPackaging =
                     _productPackagingRepository.GetById(updatedProductPackagingId);
                 Assert.AreEqual(activeProductPackaging._Status, EntityStatus.Active);
                 Trace.WriteLine(string.Format("Product Packaging Status[{0}]", activeProductPackaging._Status));

                 //Set As Deleted
                 _productPackagingRepository.SetAsDeleted(updatedProductPackaging);
                 ProductPackaging deletedProductPackaging =
                     _productPackagingRepository.GetById(updatedProductPackagingId);
                 Assert.AreEqual(deletedProductPackaging._Status, EntityStatus.Deleted);
                 Trace.WriteLine(string.Format("Deleted Product Packaging Status[{0}]", deletedProductPackaging._Status));
                 Trace.WriteLine(string.Format("Product Packaging Repository Unit Tests Successful"));


                 _testHelper.Ioc<ICacheProvider>().Reset();


             }
         }

         private void AssertProductPackaging(ProductPackaging createdProductPackaging, ProductPackaging newProductPackaging)
         {
             Assert.IsNotNull(createdProductPackaging);
             Assert.AreEqual(createdProductPackaging.Code, newProductPackaging.Code);
             Assert.AreEqual(createdProductPackaging.Name, newProductPackaging.Name);
             Assert.AreEqual(createdProductPackaging.Description, newProductPackaging.Description);
             Assert.AreEqual(createdProductPackaging._Status, newProductPackaging._Status);
         }
    }
}
