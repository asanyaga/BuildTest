using System;
using System.Diagnostics;
using System.Linq;
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
    public class CertainValueCertainProductDiscountRepositoryFixture
    {
        private ICertainValueCertainProductDiscountRepository _certainValueCertainProductDiscountRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _certainValueCertainProductDiscountRepository =
                _testHelper.Ioc<ICertainValueCertainProductDiscountRepository>();
        }

        [Test]
        public void CertainValueCertainProductDiscountRepositoryUnitTest()
        {
            using (var tra = new TransactionScope())
            {

                Trace.WriteLine("START CERTAIN VALUE CERTAIN PRODUCT DISCOUNT REPOSITORY UNIT TEST....");
                //Create
                CertainValueCertainProductDiscount newCertainValueCertainProductDiscount = _testHelper.BuildCertainValueCertainProductDiscount();

                Guid newCertainValueCertainProductDiscountId = _certainValueCertainProductDiscountRepository.Save(newCertainValueCertainProductDiscount);

                //GetById
                CertainValueCertainProductDiscount createdCertainValueCertainProductDiscount = _certainValueCertainProductDiscountRepository.GetById(newCertainValueCertainProductDiscountId);
                var cvcd =
                    createdCertainValueCertainProductDiscount.CertainValueCertainProductDiscountItems.FirstOrDefault();
                AssertCertainValueCertainProductDiscount(createdCertainValueCertainProductDiscount, newCertainValueCertainProductDiscount);
                Trace.WriteLine(string.Format("CertainValueCertainProductDiscount GetById[{0}], Quantity[{1}], CertainValue[{2}]", createdCertainValueCertainProductDiscount.Id, cvcd.Quantity
                    , cvcd.CertainValue));

                //Update
                CertainValueCertainProductDiscount certainValueCertainProductDiscountToUpdate = _testHelper.Ioc<ICertainValueCertainProductDiscountFactory>().CreateCertainValueCertainProductDiscount(
                        new ProductRef { ProductId = cvcd.Product.ProductId }, 20, 1000, DateTime.Now, DateTime.Now.AddDays(5));
                certainValueCertainProductDiscountToUpdate.Id = createdCertainValueCertainProductDiscount.Id;
                certainValueCertainProductDiscountToUpdate._Status = EntityStatus.Active;
                Guid updatedCertainValueCertainProductDiscountId = _certainValueCertainProductDiscountRepository.Save(certainValueCertainProductDiscountToUpdate);
                CertainValueCertainProductDiscount updatedCertainValueCertainProductDiscount =
                    _certainValueCertainProductDiscountRepository.GetById(updatedCertainValueCertainProductDiscountId);
                AssertCertainValueCertainProductDiscount(updatedCertainValueCertainProductDiscount, certainValueCertainProductDiscountToUpdate);
                var cvcdUdated =
                    updatedCertainValueCertainProductDiscount.CertainValueCertainProductDiscountItems.FirstOrDefault();
                Trace.WriteLine(string.Format("Updated CertainValueCertainProductDiscountId[{0}],Quantity[{1}], Value[{2}]", updatedCertainValueCertainProductDiscountId, cvcdUdated.Quantity, cvcdUdated.CertainValue));


                //Query
                var queryResult = _certainValueCertainProductDiscountRepository.Query(new QueryStandard() { });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Certain Value Certain ProductDiscount Repository Query Count[{0}]", queryResult.Count));

                //Set As Inactive
                _certainValueCertainProductDiscountRepository.SetInactive(updatedCertainValueCertainProductDiscount);
                CertainValueCertainProductDiscount inactiveCertainValueCertainProductDiscount =
                    _certainValueCertainProductDiscountRepository.GetById(updatedCertainValueCertainProductDiscountId);
                Assert.AreEqual(inactiveCertainValueCertainProductDiscount._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Certain Value Certain ProductDiscount Status[{0}]", inactiveCertainValueCertainProductDiscount._Status));

                //Set As Active
                _certainValueCertainProductDiscountRepository.SetActive(updatedCertainValueCertainProductDiscount);
                CertainValueCertainProductDiscount activeCertainValueCertainProductDiscount =
                    _certainValueCertainProductDiscountRepository.GetById(updatedCertainValueCertainProductDiscountId);
                Assert.AreEqual(activeCertainValueCertainProductDiscount._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Certain Value Certain ProductDiscount Status[{0}]", activeCertainValueCertainProductDiscount._Status));

                //Set As Deleted
                _certainValueCertainProductDiscountRepository.SetAsDeleted(updatedCertainValueCertainProductDiscount);
                CertainValueCertainProductDiscount deletedCertainValueCertainProductDiscount =
                    _certainValueCertainProductDiscountRepository.GetById(updatedCertainValueCertainProductDiscountId);
                Assert.AreEqual(deletedCertainValueCertainProductDiscount._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Certain Value Certain ProductDiscount Status[{0}]", deletedCertainValueCertainProductDiscount._Status));
                Trace.WriteLine(string.Format("Certain Value Certain ProductDiscount Repository Unit Tests Successful"));


                _testHelper.Ioc<ICacheProvider>().Reset();

            }
        }

        private void AssertCertainValueCertainProductDiscount(CertainValueCertainProductDiscount certainValueCertainProductDiscountX, CertainValueCertainProductDiscount CertainValueCertainProductDiscountY)
        {
            Assert.IsNotNull(certainValueCertainProductDiscountX);
            Assert.AreEqual(certainValueCertainProductDiscountX.Quantity, CertainValueCertainProductDiscountY.Quantity);
            Assert.AreEqual(certainValueCertainProductDiscountX.CertainValue, CertainValueCertainProductDiscountY.CertainValue);
            Assert.AreEqual(certainValueCertainProductDiscountX.EffectiveDate, CertainValueCertainProductDiscountY.EffectiveDate);
            Assert.AreEqual(certainValueCertainProductDiscountX._Status, CertainValueCertainProductDiscountY._Status);
        }
    }
}
