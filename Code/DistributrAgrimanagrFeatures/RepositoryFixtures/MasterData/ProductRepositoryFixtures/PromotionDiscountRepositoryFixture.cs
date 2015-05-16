using System;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.Caching;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.ProductRepositoryFixtures
{
    [TestFixture]
    public class PromotionDiscountRepositoryFixture
    {
        private IPromotionDiscountRepository _promotionDiscountRepository;
        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _promotionDiscountRepository = _testHelper.Ioc<IPromotionDiscountRepository>();
        }

        [Test]
        public void PromotionDiscountRepositoryUnitTest()
        {
            using (var tra = new TransactionScope())
            {


                Trace.WriteLine("START CUSTOMER DISCOUNT REPOSITORY UNIT TEST....");
                //Create
                PromotionDiscount newPromotionDiscount = _testHelper.BuildPromotionDiscount();
                Guid newPromotionDiscountId = _promotionDiscountRepository.Save(newPromotionDiscount);

                //GetById
                PromotionDiscount createdPromotionDiscount = _promotionDiscountRepository.GetById(newPromotionDiscountId);
                AssertPromotionDiscount(createdPromotionDiscount, newPromotionDiscount);
                var cpd = createdPromotionDiscount.PromotionDiscountItems.FirstOrDefault();
                Trace.WriteLine(string.Format("PromotionDiscount GetById[{0}], Free of Charge Discount[{1}],Quantity[{2}]", createdPromotionDiscount.Id,
                    cpd.FreeOfChargeProduct.ProductId, cpd.ParentProductQuantity));

                //Update
                var promotionDiscountToUpdate = _testHelper.Ioc<IPromotionDiscountFactory>().CreateFreeOfChargeDiscount(new ProductRef { ProductId = createdPromotionDiscount.ProductRef.ProductId },
                                                                             cpd.FreeOfChargeProduct.ProductId, 6, 2, DateTime.Now, 12, DateTime.Now.AddDays(30));
                promotionDiscountToUpdate._Status = EntityStatus.Active;
                Guid updatedPromotionDiscountId = _promotionDiscountRepository.Save(promotionDiscountToUpdate);
                PromotionDiscount updatedPromotionDiscount =
                    _promotionDiscountRepository.GetById(updatedPromotionDiscountId);
                AssertPromotionDiscount(updatedPromotionDiscount, promotionDiscountToUpdate);
                var upd = updatedPromotionDiscount.PromotionDiscountItems.FirstOrDefault();
                Trace.WriteLine(string.Format("Promotion DiscountId[{0}], ProductId[{1}], Quantity[{2}]", updatedPromotionDiscountId, upd.FreeOfChargeProduct.ProductId, upd.ParentProductQuantity));


                //Set As Inactive
                _promotionDiscountRepository.SetInactive(updatedPromotionDiscount);
                PromotionDiscount inactivePromotionDiscount =
                    _promotionDiscountRepository.GetById(updatedPromotionDiscountId);
                Assert.AreEqual(inactivePromotionDiscount._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Promotion Discount Status[{0}]", inactivePromotionDiscount._Status));

                //Set As Active
                _promotionDiscountRepository.SetActive(updatedPromotionDiscount);
                PromotionDiscount activePromotionDiscount =
                    _promotionDiscountRepository.GetById(updatedPromotionDiscountId);
                Assert.AreEqual(activePromotionDiscount._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Promotion Discount Status[{0}]", activePromotionDiscount._Status));

                //Set As Deleted
                _promotionDiscountRepository.SetAsDeleted(updatedPromotionDiscount);
                PromotionDiscount deletedPromotionDiscount =
                    _promotionDiscountRepository.GetById(updatedPromotionDiscountId);
                Assert.AreEqual(deletedPromotionDiscount._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted Product PromotionDiscount Status[{0}]", deletedPromotionDiscount._Status));
                Trace.WriteLine(string.Format("Promotion Discount Repository Unit Tests Successful"));
                _testHelper.Ioc<ICacheProvider>().Reset();




            }

        }

        private void AssertPromotionDiscount(PromotionDiscount promotionDiscountX, PromotionDiscount PromotionDiscountY)
        {
            var pdX = promotionDiscountX.PromotionDiscountItems.FirstOrDefault();
            var pdY = PromotionDiscountY.PromotionDiscountItems.FirstOrDefault();
            Assert.IsNotNull(promotionDiscountX);
            Assert.AreEqual(promotionDiscountX.Id.ToString(), PromotionDiscountY.Id.ToString());
            Assert.AreEqual(promotionDiscountX.ProductRef.ProductId.ToString(), PromotionDiscountY.ProductRef.ProductId.ToString());
            if (pdX != null && pdY != null)
            {
                Assert.AreEqual(pdX.EffectiveDate, pdY.EffectiveDate);
                Assert.AreEqual(pdX.DiscountRate, pdY.DiscountRate);
                Assert.AreEqual(promotionDiscountX._Status, PromotionDiscountY._Status); 
            }
               
       
        }
    }
}
