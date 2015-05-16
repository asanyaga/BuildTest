using System;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
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
    public class CustomerDiscountRepositoryFixture
    {
        private ICustomerDiscountRepository _customerDiscountRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _customerDiscountRepository = _testHelper.Ioc<ICustomerDiscountRepository>();
        }
        [Ignore]
        [Test]
        public void CustomerDiscountRepositoryUnitTest()
        {
            using (var tra = new TransactionScope())
            {

                Trace.WriteLine("START CUSTOMER DISCOUNT REPOSITORY UNIT TEST....");
                //Create
                CustomerDiscount newCustomerDiscount = _testHelper.BuildCustomerDiscount();
                Guid newCustomerDiscountId = _customerDiscountRepository.Save(newCustomerDiscount);

                //GetById
                CustomerDiscount createdCustomerDiscount = _customerDiscountRepository.GetById(newCustomerDiscountId);
                var createdCdItems = createdCustomerDiscount.CustomerDiscountItems.FirstOrDefault();
                AssertCustomerDiscount(createdCustomerDiscount, newCustomerDiscount);
                Trace.WriteLine(string.Format("CustomerDiscount GetById[{0}], Discount[{1}]", createdCustomerDiscount.Id,
                    createdCdItems.DiscountRate));

                //Update
                CustomerDiscount customerDiscountToUpdate = createdCustomerDiscount;
                customerDiscountToUpdate=_testHelper.Ioc<ICustomerDiscountFactory>().CreateCustomerDiscount(new CostCentreRef { Id = createdCustomerDiscount .Outlet.Id},
                    new ProductRef { ProductId = createdCustomerDiscount.Product.ProductId },18,
                    DateTime.Now.AddDays(-2));
                customerDiscountToUpdate.Id = createdCustomerDiscount.Id;
                customerDiscountToUpdate._Status = EntityStatus.Active;
                Guid updatedCustomerDiscountId = _customerDiscountRepository.Save(customerDiscountToUpdate);
                CustomerDiscount updatedCustomerDiscount =
                    _customerDiscountRepository.GetById(updatedCustomerDiscountId);
                var updatedCdItems = createdCustomerDiscount.CustomerDiscountItems.FirstOrDefault();
                AssertCustomerDiscount(customerDiscountToUpdate, updatedCustomerDiscount);
                Trace.WriteLine(string.Format("Updated CustomerDiscountId[{0}], Rate[{1}],Effective Date[{2}]", updatedCustomerDiscountId, updatedCdItems.DiscountRate, updatedCdItems.EffectiveDate));
                
                //Set As Inactive
                _customerDiscountRepository.SetInactive(updatedCustomerDiscount);
                CustomerDiscount inactiveCustomerDiscount =
                    _customerDiscountRepository.GetById(updatedCustomerDiscountId);
                Assert.AreEqual(inactiveCustomerDiscount._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Customer Discount Status[{0}]", inactiveCustomerDiscount._Status));

                //Set As Active
                _customerDiscountRepository.SetActive(updatedCustomerDiscount);
                CustomerDiscount activeCustomerDiscount =
                    _customerDiscountRepository.GetById(updatedCustomerDiscountId);
                Assert.AreEqual(activeCustomerDiscount._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Customer Discount Status[{0}]", activeCustomerDiscount._Status));

                //Set As Deleted
                _customerDiscountRepository.SetAsDeleted(updatedCustomerDiscount);
                CustomerDiscount deletedCustomerDiscount =
                    _customerDiscountRepository.GetById(updatedCustomerDiscountId);
                Assert.AreEqual(deletedCustomerDiscount._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Customer Discount Status[{0}]", deletedCustomerDiscount._Status));
                Trace.WriteLine(string.Format("Customer Discount Repository Unit Tests Successful"));
                _testHelper.Ioc<ICacheProvider>().Reset();

            }
        }

        private void AssertCustomerDiscount(CustomerDiscount customerDiscountX, CustomerDiscount customerDiscountY)
        {
            var cdX = customerDiscountX.CustomerDiscountItems.FirstOrDefault();
            var cdY = customerDiscountY.CustomerDiscountItems.FirstOrDefault();
            Assert.IsNotNull(customerDiscountX);
            Assert.AreEqual(customerDiscountX.Outlet.Id, customerDiscountY.Outlet.Id);
         
            Assert.AreEqual(customerDiscountX._Status, customerDiscountY._Status);
        }
    }
}
