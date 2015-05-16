
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using StructureMap;
using Distributr.Core.Data.IOC;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.HQ.Lib.IOC;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Products;

namespace Distributr.Core.Data.Tests.IOC
{
    [TestFixture]
    public class StructuremapRegistryFixture
    {
        [SetUp]
        public void setup()
        {

            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry<DataRegistry>();
                x.AddRegistry<ViewModelBuilderRegistry>();
            });
        }

        [Test]
        public void test()
        {
           
           var test =  ObjectFactory.GetInstance<IProductBrandRepository>();
           Assert.IsNotNull(test);
           test.GetAll();
        }
        [Test]
        public void testUser()
        {
            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry<DataRegistry>();
            });
            var test = ObjectFactory.GetInstance<IUserRepository >();
            Assert.IsNotNull(test);
            test.GetAll();
        }
        [Test]
        public void ViewBuilderProductPrice()
        {

            var test = ObjectFactory.GetInstance<IProductPricingViewModelBuilder>();
            Assert.IsNotNull(test);
            test.GetAll();
        }
    }
}
