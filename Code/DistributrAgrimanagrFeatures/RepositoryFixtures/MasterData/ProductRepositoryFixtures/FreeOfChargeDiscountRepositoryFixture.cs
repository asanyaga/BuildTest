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
   public class FreeOfChargeDiscountRepositoryFixture
    {
        private IFreeOfChargeDiscountRepository _freeOfChargeDiscountRepository;
        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _freeOfChargeDiscountRepository = _testHelper.Ioc<IFreeOfChargeDiscountRepository>();
        }

        [Test]
        public void FreeOfChargeDiscountRepositoryUnitTest()
        {
            using (var tra =new TransactionScope())
            {

                Trace.WriteLine("START PRODUCT BRAND REPOSITORY UNIT TEST....");
                //Create
                FreeOfChargeDiscount newFreeOfChargeDiscount = _testHelper.BuildFreeOfChargeDiscount();
                Guid newFreeOfChargeDiscountId = _freeOfChargeDiscountRepository.Save(newFreeOfChargeDiscount);

                //GetById
                FreeOfChargeDiscount createdFreeOfChargeDiscount = _freeOfChargeDiscountRepository.GetById(newFreeOfChargeDiscountId);
                Product freeOfChargeProduct =
                    _testHelper.Ioc<IProductRepository>().GetById(createdFreeOfChargeDiscount.ProductRef.ProductId);
                AssertFreeOfChargeDiscount(createdFreeOfChargeDiscount, newFreeOfChargeDiscount);
                Trace.WriteLine(string.Format("FreeOfChargeDiscount GetById[{0}], Name[{1}]", createdFreeOfChargeDiscount.Id,
                   freeOfChargeProduct.Description));

                //Update
                var FreeOfChargeDiscountToUpdate = createdFreeOfChargeDiscount;
                FreeOfChargeDiscountToUpdate._DateCreated = DateTime.Now;
                FreeOfChargeDiscountToUpdate._DateLastUpdated = DateTime.Now;
                FreeOfChargeDiscountToUpdate._Status = EntityStatus.Active;
                FreeOfChargeDiscountToUpdate.StartDate = DateTime.Now.AddDays(2);
                FreeOfChargeDiscountToUpdate.EndDate = DateTime.Now.AddDays(5);
    

                Guid updatedFreeOfChargeDiscountId = _testHelper.Ioc<IFreeOfChargeDiscountRepository>().Save(FreeOfChargeDiscountToUpdate);
                FreeOfChargeDiscount updatedFreeOfChargeDiscount =
                    _testHelper.Ioc<IFreeOfChargeDiscountRepository>().GetById(updatedFreeOfChargeDiscountId);
                AssertFreeOfChargeDiscount(updatedFreeOfChargeDiscount, FreeOfChargeDiscountToUpdate);

                Trace.WriteLine(string.Format("Updated FreeOfChargeDiscountId[{0}]", updatedFreeOfChargeDiscountId));


                //Query
                var queryResult = _testHelper.Ioc<IFreeOfChargeDiscountRepository>().QueryResult(new QueryFOCDiscount() {BrandId =freeOfChargeProduct.Brand.Id, Name = freeOfChargeProduct.Description,Take = 10, Skip = 0});
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("FreeOfChargeDiscount Repository Query Count[{0}]", queryResult.Count));

                //Set As Inactive
                _testHelper.Ioc<IFreeOfChargeDiscountRepository>().SetInactive(updatedFreeOfChargeDiscount);
                FreeOfChargeDiscount inactiveFreeOfChargeDiscount =
                    _testHelper.Ioc<IFreeOfChargeDiscountRepository>().GetById(updatedFreeOfChargeDiscountId);
                Assert.AreEqual(inactiveFreeOfChargeDiscount._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("FreeOfChargeDiscount Status[{0}]", inactiveFreeOfChargeDiscount._Status));

                //Set As Active
                _testHelper.Ioc<IFreeOfChargeDiscountRepository>().SetActive(updatedFreeOfChargeDiscount);
                FreeOfChargeDiscount activeFreeOfChargeDiscount =
                    _testHelper.Ioc<IFreeOfChargeDiscountRepository>().GetById(updatedFreeOfChargeDiscountId);
                Assert.AreEqual(activeFreeOfChargeDiscount._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("FreeOfChargeDiscount Status[{0}]", activeFreeOfChargeDiscount._Status));

                //Set As Deleted
                _testHelper.Ioc<IFreeOfChargeDiscountRepository>().SetAsDeleted(updatedFreeOfChargeDiscount);
                FreeOfChargeDiscount deletedFreeOfChargeDiscount =
                    _testHelper.Ioc<IFreeOfChargeDiscountRepository>().GetById(updatedFreeOfChargeDiscountId);
                Assert.AreEqual(deletedFreeOfChargeDiscount._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted FreeOfChargeDiscount Status[{0}]", deletedFreeOfChargeDiscount._Status));
                Trace.WriteLine(string.Format("Product FreeOfChargeDiscount Repository Unit Tests Successful"));


                _testHelper.Ioc<ICacheProvider>().Reset();

            }

        }

        private void AssertFreeOfChargeDiscount(FreeOfChargeDiscount freeOfChargeDiscountX, FreeOfChargeDiscount freeOfChargeDiscountY)
        {

            Assert.IsNotNull(freeOfChargeDiscountX);
            Assert.AreEqual(freeOfChargeDiscountX.ProductRef.ProductId.ToString(), freeOfChargeDiscountY.ProductRef.ProductId.ToString());
            //Assert.AreEqual(freeOfChargeDiscountX.StartDate, freeOfChargeDiscountY.StartDate);
            Assert.AreEqual(freeOfChargeDiscountX._Status, freeOfChargeDiscountY._Status);
                
        }
    }
}
